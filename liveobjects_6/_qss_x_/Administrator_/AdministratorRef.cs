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
using System.ServiceModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;

namespace QS._qss_x_.Administrator_
{
    public sealed class AdministratorRef : IAdministratorRef, IDisposable
    {
        #region Constructor

        public AdministratorRef(string subnet, string servername, int serverport, QS.Fx.Logging.ILogger logger, bool verbose)
            : this(subnet, servername, serverport, logger, verbose, null, null, null)
        {
        }

        public AdministratorRef(string subnet, string servername, int serverport, QS.Fx.Logging.ILogger logger, bool verbose, string authentication, string user, string password)
        {
            this.logger = logger;
            this.verbose = verbose;
            string localname = Dns.GetHostName();
            IPHostEntry localhostentry;
            try
            {
                localhostentry = Dns.GetHostEntry(localname);
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot resolve local host name \"" + localname + "\".", exc);
            }
            IPHostEntry serverhostentry;
            try
            {
                serverhostentry = Dns.GetHostEntry(servername);
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot resolve server host name \"" + servername + "\".", exc);
            }
            QS._qss_c_.Base1_.Subnet _subnet = new QS._qss_c_.Base1_.Subnet(subnet);
            bool found = false;
            foreach (IPAddress address in localhostentry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && _subnet.contains(address))
                {
                    this.localaddress = address;
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new Exception("Cannot find any local ip address on the requested subnet " + _subnet.ToString() + ".");
            found = false;
            foreach (IPAddress address in serverhostentry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && _subnet.contains(address))
                {
                    this.serveraddress = address;
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new Exception("Cannot find any server ip address on the requested subnet " + _subnet.ToString() + ".");
            this.symmetricalgorithm = SymmetricAlgorithm.Create();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, QS._qss_x_.Administrator_.Connection.DefaultBufferSize);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, QS._qss_x_.Administrator_.Connection.DefaultBufferSize);
            this.socket.Bind(new IPEndPoint(localaddress, 0));
            int myport = ((IPEndPoint) this.socket.LocalEndPoint).Port;
            string endpointaddress = "http://" + serveraddress + ":" + serverport.ToString() + "/Administrator";
            if (verbose)
                logger.Log("Connecting to administrator service " + endpointaddress + ".");
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            if ((authentication == null) || authentication.Equals("windows"))
                binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            else if (authentication.Equals("username"))
                binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            else
                throw new Exception("Unknown authentication type: \"" + authentication + "\"");
            channelfactory = new ChannelFactory<IAdministrator>(binding, new EndpointAddress(endpointaddress));
            if (user != null)
                channelfactory.Credentials.UserName.UserName = user;            
            if (password != null)
                channelfactory.Credentials.UserName.Password = password;
            administratorservice = channelfactory.CreateChannel();
            string _serveripaddress;
            int _serverportno;
            byte[] _iv, _key;
            administratorservice.Connect(localaddress.ToString(), myport,
                out _serveripaddress, out _serverportno, out connectionid, out _iv, out _key);
            if (verbose)
                logger.Log("Negotiated a secure connection [ " + connectionid.ToString() + " ] from [ " + 
                    localaddress.ToString() + " : " + myport.ToString() + " ] to [ " + _serveripaddress + " : " + _serverportno.ToString() + " ].");
            using (this.symmetricalgorithm)
            {
                this.encryptor = this.symmetricalgorithm.CreateEncryptor(_key, _iv);
                this.decryptor = this.symmetricalgorithm.CreateDecryptor(_key, _iv);
            }
            this.socket.Connect(IPAddress.Parse(_serveripaddress), _serverportno);
            IPEndPoint remoteendpoint = (IPEndPoint)this.socket.RemoteEndPoint;
            if (verbose)
                logger.Log("Established a secure connection [ " + connectionid.ToString() + " ] from [ " +
                    localaddress.ToString() + " : " + myport.ToString() + " ] to [ " +
                    remoteendpoint.Address.ToString() + " : " + remoteendpoint.Port.ToString() + " ].");
            this.requests = new Queue<Request>();
            this.working = false;
            this.disposed = false;
            this.done = new ManualResetEvent(true);
        }

        #endregion

        #region Fields

        private IPAddress localaddress, serveraddress;
        private SymmetricAlgorithm symmetricalgorithm;
        private Socket socket;
        private ICryptoTransform encryptor, decryptor;
        private Queue<Request> requests;
        private bool working, disposed;
        private int connectionid, seqno, responseseqno;
        private Request request;
        private byte[] incomingpacket;
        private int incomingcount;
        private QS.Fx.Logging.ILogger logger;
        private ManualResetEvent done;
        private bool verbose, failed = false;
        private List<Exception> exceptions = new List<Exception>();
        private IAdministrator administratorservice;
        private ChannelFactory<IAdministrator> channelfactory;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                while (working)
                {
                    Monitor.Exit(this);
                    done.WaitOne();
                    Monitor.Enter(this);
                }
                _Disconnect();
                if (administratorservice != null)
                {
                    try
                    {
                        ((IDisposable)administratorservice).Dispose();
                    }
                    catch (Exception)
                    {
                    }
                    administratorservice = null;
                }
                if (channelfactory != null)
                {
                    try
                    {
                        channelfactory.Close();
                    }
                    catch (Exception)
                    {
                    }
                    channelfactory = null;
                }
                if (failed)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Could not successfully complete some of the requested operations.");
                    string horizontalbar = (new string('-', 80));
                    foreach (Exception exception in exceptions)
                    {
                        sb.AppendLine(horizontalbar);
                        sb.AppendLine(exception.ToString());
                    }
                    sb.AppendLine(horizontalbar);
                    throw new Exception(sb.ToString());
                }
            }
        }

        #endregion

        #region _Disconnect

        private void _Disconnect()
        {
            if (verbose)
                logger.Log("Disconnecting connection [ " + connectionid.ToString() + " ].");
            done.Set();
            if (socket != null)
            {
                socket.LingerState.Enabled = false;
                socket.LingerState.LingerTime = QS._qss_x_.Administrator_.Connection.DefaultLingerTime;
                socket.Close();
                socket = null;
            }
            disposed = true;
        }

        #endregion

        #region IAdministratorRef Members

        void IAdministratorRef.Upload(string from, string to)
        {
            lock (this)
            {
                if (verbose)
                    logger.Log("upload : " + from + " => " + to);
                foreach (Request request in (new Request(from, to, RequestType.Upload)).Resolve())
                    requests.Enqueue(request);
                _Send();
            }
        }

        void IAdministratorRef.Download(string from, string to)
        {
            lock (this)
            {
                if (verbose)
                    logger.Log("download : " + from + " => " + to);
                requests.Enqueue(new Request(from, to, RequestType.Download));
                _Send();
            }
        }

        void IAdministratorRef.Execute(string target, string folder, string arguments, TimeSpan timeout)
        {
            lock (this)
            {
                if (verbose)
                    logger.Log("execute : \"" + target + "\" with arguments \"" + arguments + "\" in \"" + folder + "\".");
                string output = string.Empty;
                try
                {
                    administratorservice.Execute(target, folder, arguments, timeout, out output);
                }
                finally
                {
                    logger.Log("\n" + output);
                }
            }
        }

        #endregion

        #region _Send

        private void _Send()
        {
            try
            {
                if (!working)
                {
                    if (requests.Count > 0)
                    {
                        working = true;
                        done.Reset();
                        request = requests.Dequeue();
                        MemoryStream memorystream = new MemoryStream();
                        CryptoStream cryptostream = new CryptoStream(memorystream, encryptor, CryptoStreamMode.Write);
                        GZipStream compressedstream = new GZipStream(cryptostream, CompressionMode.Compress);
                        Stream _outstream = compressedstream;
                        byte[] _buffer1, _buffer2;
                        _buffer1 = BitConverter.GetBytes(++seqno);
                        _outstream.Write(_buffer1, 0, _buffer1.Length);
                        _outstream.WriteByte((byte)request.Type);
                        switch (request.Type)
                        {
                            case RequestType.Download:
                                {
                                    if (verbose)
                                        logger.Log(" - sending a request #" + seqno.ToString() + ", to download from \"" + request.From + "\"");
                                    _buffer1 = Encoding.Unicode.GetBytes(request.From);
                                    _buffer2 = BitConverter.GetBytes(_buffer1.Length);
                                    _outstream.Write(_buffer2, 0, _buffer2.Length);
                                    _outstream.Write(_buffer1, 0, _buffer1.Length);
                                }
                                break;

                            case RequestType.Upload:
                                {
                                    _buffer1 = Encoding.Unicode.GetBytes(request.To);
                                    _buffer2 = BitConverter.GetBytes(_buffer1.Length);
                                    _outstream.Write(_buffer2, 0, _buffer2.Length);
                                    _outstream.Write(_buffer1, 0, _buffer1.Length);
                                    using (FileStream filestream = new FileStream(request.From, FileMode.Open, FileAccess.Read))
                                    {
                                        int length = (int)filestream.Length;
                                        if (verbose)
                                            logger.Log(" - sending a request #" + seqno.ToString() + ", to upload ( \"" +
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
                                break;
                        }
                        _outstream = null;
                        compressedstream.Close();
                        cryptostream.Close();
                        memorystream.Close();
                        responseseqno = 0;
                        byte[] _data = memorystream.ToArray();
                        IList<ArraySegment<byte>> _packet = new List<ArraySegment<byte>>();
                        _packet.Add(new ArraySegment<byte>(BitConverter.GetBytes(_data.Length)));
                        _packet.Add(new ArraySegment<byte>(_data));
                        socket.BeginSend(_packet, SocketFlags.None, new AsyncCallback(this._SendCallback), _packet);
                    }
                    else
                    {
                        if (verbose)
                            logger.Log("ready to send the next request");
                    }
                }
            }
            catch (Exception exc)
            {
                failed = true;
                exceptions.Add(exc);
                _Disconnect();
            }
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
                    int ntransmitted = socket.EndSend(result);
                    if (ntransmitted != _packetsize)
                        throw new Exception("Could not transmit the entire packet.");
                    _Receive();
                }
                catch (Exception exc)
                {
                    failed = true;
                    exceptions.Add(exc);
                    _Disconnect();
                }
            }
        }

        #endregion

        #region _Receive

        private void _Receive()
        {
            byte[] _incomingsize = BitConverter.GetBytes((int)0);
            socket.BeginReceive(_incomingsize, 0, _incomingsize.Length, SocketFlags.None, 
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
                    int nreceived = socket.EndReceive(result);
                    if (nreceived != _header.Length)
                        throw new Exception("Received a bad header.");
                    int _packetsize = BitConverter.ToInt32(_header, 0);
                    if (_packetsize > 0)
                    {
                        incomingpacket = new byte[_packetsize];
                        incomingcount = 0;
                        socket.BeginReceive(incomingpacket, 0, incomingpacket.Length, SocketFlags.None, new AsyncCallback(this._ReceiveCallback_2), null);
                    }
                    else
                    {
                        _Disconnect();
                    }
                }
                catch (Exception exc)
                {
                    failed = true;
                    exceptions.Add(exc);
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
                    incomingcount += socket.EndReceive(result);
                    if (incomingcount < incomingpacket.Length)
                        socket.BeginReceive(incomingpacket, incomingcount, incomingpacket.Length - incomingcount, SocketFlags.None,
                            new AsyncCallback(this._ReceiveCallback_2), null);
                    else
                    {
                        MemoryStream memorystream = new MemoryStream(incomingpacket);
                        incomingpacket = null;
                        CryptoStream cryptostream = new CryptoStream(memorystream, decryptor, CryptoStreamMode.Read);
                        GZipStream compressedstream = new GZipStream(cryptostream, CompressionMode.Decompress);
                        Stream _instream = compressedstream;
                        byte[] _buffer1, _buffer2;
                        _buffer1 = BitConverter.GetBytes((int)0);
                        _instream.Read(_buffer1, 0, _buffer1.Length);
                        int _seqno = BitConverter.ToInt32(_buffer1, 0);
                        if (_seqno != seqno)
                            throw new Exception("Incorrect sequence number.");
                        _instream.Read(_buffer1, 0, _buffer1.Length);
                        int _responseseqno = BitConverter.ToInt32(_buffer1, 0);
                        if (_responseseqno > 0)
                        {
                            if (_responseseqno != responseseqno + 1)
                                throw new Exception("Incorrect sequence number.");
                            responseseqno = _responseseqno;
                            _instream.Read(_buffer1, 0, _buffer1.Length);
                            _buffer2 = new byte[BitConverter.ToInt32(_buffer1, 0)];
                            _instream.Read(_buffer2, 0, _buffer2.Length);
                            string _from = Encoding.Unicode.GetString(_buffer2);
                            _instream.Read(_buffer1, 0, _buffer1.Length);
                            _buffer2 = new byte[BitConverter.ToInt32(_buffer1, 0)];
                            _instream.Read(_buffer2, 0, _buffer2.Length);
                            string _to = request.To + Path.DirectorySeparatorChar + Encoding.Unicode.GetString(_buffer2);
                            string _tofolder = Path.GetDirectoryName(_to);
                            if (!Directory.Exists(_tofolder))
                                Directory.CreateDirectory(_tofolder);
                            using (FileStream filestream = new FileStream(_to, FileMode.Create, FileAccess.Write))
                            {
                                _instream.Read(_buffer1, 0, _buffer1.Length);
                                int length = BitConverter.ToInt32(_buffer1, 0);
                                if (verbose)
                                    logger.Log(" - in response to request #" + seqno.ToString() +", downloaded a file #" + responseseqno.ToString() + 
                                        " ( \"" + _from + "\", " + length.ToString() + " bytes ) to \"" + _to + "\"");
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
                            _instream = null;
                            compressedstream.Close();
                            cryptostream.Close();
                            memorystream.Close();
                            _Receive();
                        }
                        else
                        {
                            if (verbose)
                                logger.Log(" - successfully completed request #" + seqno.ToString());
                            _instream = null;
                            compressedstream.Close();
                            cryptostream.Close();
                            memorystream.Close();
                            working = false;
                            done.Set();
                            _Send();
                        }
                    }
                }
                catch (Exception exc)
                {
                    failed = true;
                    exceptions.Add(exc);
                    _Disconnect();
                }
            }
        }

        #endregion
    }
}
