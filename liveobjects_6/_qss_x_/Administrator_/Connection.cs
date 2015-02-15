/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace QS._qss_x_.Administrator_
{
    public sealed class Connection : IDisposable
    {
        #region Constants

        public const int DefaultLingerTime = 60;
        public const int DefaultBufferSize = 1024 * 1024;

        #endregion

        #region Constructor

        public Connection(int id, IPAddress clientip, int clientport, Socket socket, ICryptoTransform encryptor, ICryptoTransform decryptor,
            QS.Fx.Base.ContextCallback<Connection> disconnectcallback, QS.Fx.Logging.ILogger logger, bool verbose, string rootfolder)
        {
            this.rootfolder = rootfolder;
            this.logger = logger;
            this.verbose = verbose;
            this.id = id;
            this.clientip = clientip;
            this.clientport = clientport;
            this.socket = socket;
            this.encryptor = encryptor;
            this.decryptor = decryptor;
            this.disconnectcallback = disconnectcallback;
            this.isconnected = false;
            this.isdisposed = false;

            lock (this)
            {
                this.timer = new Timer(new TimerCallback(this._TimeoutCallback), null, TimeSpan.FromMinutes(5), TimeSpan.Zero);
                this.socket.BeginAccept(new AsyncCallback(this._ConnectCallback), null);
            }
        }

        #endregion

        #region Fields

        private int id, clientport;
        private IPAddress clientip;
        private Socket socket, newsocket;
        private ICryptoTransform encryptor, decryptor;
        private QS.Fx.Base.ContextCallback<Connection> disconnectcallback;
        private Timer timer;
        private bool isconnected, isdisposed;        
        private byte[] incomingpacket;
        private int incomingcount, seqno, responseseqno;
        private Queue<Request> requests;
        private Request request;
        private QS.Fx.Logging.ILogger logger;
        private bool verbose;
        private string rootfolder;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                _Disconnect();
            }
        }

        #endregion

        #region _TimeoutCallback

        private void _TimeoutCallback(object state)
        {
            lock (this)
            {
                if (!isconnected && !isdisposed)
                    _Disconnect();
            }
        }

        #endregion

        #region _Disconnect

        private void _Disconnect()
        {
            if (verbose)
                logger.Log("Disconnecting connection [ " + id.ToString() + " ].");
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
            if (this.socket != null)
            {
                this.socket.LingerState.Enabled = false;
                this.socket.LingerState.LingerTime = 0;
                this.socket.Close();
                this.socket = null;
            }
            if (this.newsocket != null)
            {
                this.newsocket.LingerState.Enabled = false;
                this.newsocket.LingerState.LingerTime = QS._qss_x_.Administrator_.Connection.DefaultLingerTime;
                this.newsocket.Close();
                this.newsocket = null;
            }
            this.isconnected = false;
            this.isdisposed = true;
            if (disconnectcallback != null)
                disconnectcallback(this);
        }

        #endregion

        #region _ConnectCallback

        private void _ConnectCallback(IAsyncResult result)
        {
            lock (this)
            {
                try
                {
                    if (!isconnected && !isdisposed)
                    {
                        isconnected = true;
                        newsocket = socket.EndAccept(result);
                        // newsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, QS.Fx.Administrator.Connection.DefaultBufferSize);
                        // newsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, QS.Fx.Administrator.Connection.DefaultBufferSize);
                        this.socket.LingerState.Enabled = false;
                        this.socket.LingerState.LingerTime = 0;
                        this.socket.Close();
                        this.socket = null;
                        IPEndPoint remoteendpoint = (IPEndPoint) newsocket.RemoteEndPoint;
                        if (!remoteendpoint.Address.Equals(this.clientip) || !remoteendpoint.Port.Equals(this.clientport))
                            throw new Exception("Client address or port is invalid, disconnecting...");
                        IPEndPoint localendpoint = (IPEndPoint) newsocket.LocalEndPoint;
                        if (verbose)
                            logger.Log("Established a secure connection [ " + id.ToString() + " ] from [ " +
                                clientip.ToString() + " : " + clientport.ToString() + " ] to [ " +
                                localendpoint.Address.ToString() + " : " + localendpoint.Port.ToString() + " ].");
                        timer.Dispose();
                        timer = null;
                        _Receive();
                    }
                }
                catch (Exception exc)
                {
                    logger.Log(exc.ToString());
                    _Disconnect();
                }
            }
        }

        #endregion

        #region _Receive

        private void _Receive()
        {
            if (verbose)
                logger.Log("ready to receive the next request");
            byte[] _incomingsize = BitConverter.GetBytes((int)0);
            newsocket.BeginReceive(_incomingsize, 0, _incomingsize.Length, SocketFlags.None,
                new AsyncCallback(this._ReceiveCallback), _incomingsize);
        }

        #endregion

        #region _ReceiveCallback

        private void _ReceiveCallback(IAsyncResult result)
        {
            lock (this)
            {
                try
                {
                    byte[] _header = (byte[]) result.AsyncState;
                    int nreceived = newsocket.EndReceive(result);
                    if (nreceived != _header.Length)
                        throw new Exception("Received a bad header.");
                    int _packetsize = BitConverter.ToInt32(_header, 0);
                    if (_packetsize > 0)
                    {
                        incomingpacket = new byte[_packetsize];
                        incomingcount = 0;
                        newsocket.BeginReceive(incomingpacket, 0, incomingpacket.Length, SocketFlags.None, 
                            new AsyncCallback(this._ReceiveCallback_2), null);
                    }
                    else
                    {
                        _Disconnect();
                    }
                }
                catch (Exception exc)
                {
                    logger.Log(exc.ToString());
                    _Disconnect();
                }
            }
        }

        #endregion

        #region _ReceiveCallback_2

        private void _ReceiveCallback_2(IAsyncResult result)
        {
            lock (this)
            {
                try
                {
                    incomingcount += newsocket.EndReceive(result);
                    if (incomingcount < incomingpacket.Length)
                        newsocket.BeginReceive(incomingpacket, incomingcount, incomingpacket.Length - incomingcount, SocketFlags.None,
                            new AsyncCallback(this._ReceiveCallback_2), null);
                    else
                    {
                        MemoryStream memorystream = new MemoryStream(incomingpacket);
                        incomingpacket = null;
                        CryptoStream cryptostream = new CryptoStream(memorystream, decryptor, CryptoStreamMode.Read);
                        GZipStream compressedstream = new GZipStream(cryptostream, CompressionMode.Decompress);
                        Stream _instream = compressedstream;
                        byte[] _buffer1, _buffer2;
                        _buffer1 = BitConverter.GetBytes((int) 0);
                        _instream.Read(_buffer1, 0, _buffer1.Length);
                        int _seqno = BitConverter.ToInt32(_buffer1, 0);
                        if (_seqno != seqno + 1)
                            throw new Exception("Incorrect sequence number.");
                        seqno = _seqno;
                        responseseqno = 0;
                        RequestType _requesttype = (RequestType)((byte)_instream.ReadByte());
                        requests = null;
                        switch (_requesttype)
                        {
                            case RequestType.Download:
                                {
                                    _instream.Read(_buffer1, 0, _buffer1.Length);
                                    _buffer2 = new byte[BitConverter.ToInt32(_buffer1, 0)];
                                    _instream.Read(_buffer2, 0, _buffer2.Length);
                                    string _from = Encoding.Unicode.GetString(_buffer2);
                                    if (verbose)
                                        logger.Log(" - received a request #" + seqno.ToString() + ", to download from \"" + _from + "\"");
                                    requests = new Queue<Request>((new Request(_from, string.Empty, RequestType.Download)).Resolve());
                                }
                                break;

                            case RequestType.Upload:
                                {
                                    _instream.Read(_buffer1, 0, _buffer1.Length);
                                    _buffer2 = new byte[BitConverter.ToInt32(_buffer1, 0)];
                                    _instream.Read(_buffer2, 0, _buffer2.Length);
                                    string _to = Encoding.Unicode.GetString(_buffer2);
                                    string _tofolder = Path.GetDirectoryName(_to);
                                    if (!Directory.Exists(_tofolder))
                                        Directory.CreateDirectory(_tofolder);
                                    _RootCheck(_to);
                                    using (FileStream filestream = new FileStream(_to, FileMode.Create, FileAccess.Write))
                                    {
                                        _instream.Read(_buffer1, 0, _buffer1.Length);
                                        int length = BitConverter.ToInt32(_buffer1, 0);
                                        if (verbose)
                                            logger.Log(" - received a request #" + seqno.ToString() + 
                                                ", to upload ( " + length.ToString() + " bytes ) to \"" + _to + "\"");
                                        _buffer2 = new byte[1000];
                                        int position = 0;
                                        while (position < length)
                                        {
                                            int count = _instream.Read(_buffer2, 0, Math.Min(length - position, _buffer2.Length));
                                            if (count > 0)
                                            {
                                                filestream.Write(_buffer2, 0, count);
                                                position += count;
                                            }
                                            else
                                                throw new Exception("Could not complete transmission: could not write all bytes to file \"" + _to + "\".");
                                        }
                                    }
                                }
                                break;
                        }
                        _instream = null;
                        compressedstream.Close();
                        cryptostream.Close();
                        memorystream.Close();
                        _Send();
                    }
                }
                catch (Exception exc)
                {
                    logger.Log(exc.ToString());
                    _Disconnect();
                }
            }
        }

        #endregion

        #region _Send

        private void _Send()
        {
            MemoryStream memorystream = new MemoryStream();
            CryptoStream cryptostream = new CryptoStream(memorystream, encryptor, CryptoStreamMode.Write);
            GZipStream compressedstream = new GZipStream(cryptostream, CompressionMode.Compress);
            Stream _outstream = compressedstream;
            byte[] _buffer1, _buffer2;
            _buffer1 = BitConverter.GetBytes(this.seqno);
            _outstream.Write(_buffer1, 0, _buffer1.Length);
            if (requests != null && requests.Count > 0)
            {
                _buffer1 = BitConverter.GetBytes(++responseseqno);
                _outstream.Write(_buffer1, 0, _buffer1.Length);
                request = requests.Dequeue();
                _buffer1 = Encoding.Unicode.GetBytes(request.From);
                _buffer2 = BitConverter.GetBytes(_buffer1.Length);
                _outstream.Write(_buffer2, 0, _buffer2.Length);
                _outstream.Write(_buffer1, 0, _buffer1.Length);
                _buffer1 = Encoding.Unicode.GetBytes(request.To);
                _buffer2 = BitConverter.GetBytes(_buffer1.Length);
                _outstream.Write(_buffer2, 0, _buffer2.Length);
                _outstream.Write(_buffer1, 0, _buffer1.Length);
                _RootCheck(request.From);
                using (FileStream filestream = new FileStream(request.From, FileMode.Open, FileAccess.Read))
                {
                    int length = (int)filestream.Length;
                    if (verbose)
                        logger.Log(" - responding to request #" + seqno.ToString() + " with file #" + responseseqno.ToString() + " ( \"" +
                            request.From + "\", " + length.ToString() + " bytes ) to \"" + request.To + "\"");
                    _buffer1 = BitConverter.GetBytes(length);
                    _outstream.Write(_buffer1, 0, _buffer1.Length);
                    _buffer2 = new byte[1000];
                    int position = 0;
                    while (position < length)
                    {
                        int count = filestream.Read(_buffer2, 0, Math.Min(length - position, _buffer2.Length));
                        if (count > 0)
                        {
                            _outstream.Write(_buffer2, 0, count);
                            position += count;
                        }
                        else
                            throw new Exception("Could not complete transmission: could not read all bytes from file \"" + request.From + "\".");
                    }
                }
            }
            else
            {
                if (verbose)
                    logger.Log(" - acknowledging successful completion of request #" + seqno.ToString());
                responseseqno = 0;
                _buffer1 = BitConverter.GetBytes(responseseqno);
                _outstream.Write(_buffer1, 0, _buffer1.Length);
            }
            _outstream = null;
            compressedstream.Close();
            cryptostream.Close();
            memorystream.Close();
            byte[] _data = memorystream.ToArray();
            IList<ArraySegment<byte>> _packet = new List<ArraySegment<byte>>();
            _packet.Add(new ArraySegment<byte>(BitConverter.GetBytes(_data.Length)));
            _packet.Add(new ArraySegment<byte>(_data));
            newsocket.BeginSend(_packet, SocketFlags.None, new AsyncCallback(this._SendCallback), _packet);
        }

        #endregion

        #region _SendCallback

        private void _SendCallback(IAsyncResult result)
        {
            lock (this)
            {
                try
                {
                    IList<ArraySegment<byte>> _packet = (IList<ArraySegment<byte>>)result.AsyncState;
                    int _packetsize = 0;
                    foreach (ArraySegment<byte> _segment in _packet)
                        _packetsize += _segment.Count;
                    int ntransmitted = newsocket.EndSend(result);
                    if (ntransmitted != _packetsize)
                        throw new Exception("Could not transmit the entire packet.");
                    if (responseseqno > 0)
                        _Send();
                    else
                        _Receive();
                }
                catch (Exception exc)
                {
                    logger.Log(exc.ToString());
                    _Disconnect();
                }
            }
        }

        #endregion

        #region _RootCheck

        private void _RootCheck(string _path)
        {
            Administrator._RootCheck(this.rootfolder, _path);
        }

        #endregion
    }
}
