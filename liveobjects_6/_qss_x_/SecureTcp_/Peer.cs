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

#define DEBUG_DISABLE_ENCRYPTION
//#define DEBUG_LINUX
//#define LINUX_NO_WCF

//#define LINUX_BEGINSEND_BUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;

namespace QS._qss_x_.SecureTcp_
{
    public abstract class Peer : QS.Fx.Inspection.Inspectable
    {
        #region Constructor

        protected Peer(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._logger = _mycontext.Platform.Logger;
            incomingheader = BitConverter.GetBytes((int)0);
            incomingheader2 = new byte[sizeof(long) + 3 * sizeof(uint) + sizeof(ushort)];
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        // Lock 1
        private ICryptoTransform encryptor, decryptor;
        [QS.Fx.Base.Inspectable]
        private long connectionid;
        [QS.Fx.Base.Inspectable]
        private bool connected;

        // Lock 2
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Serialization.ISerializable> incoming = new Queue<QS.Fx.Serialization.ISerializable>();
        [QS.Fx.Base.Inspectable]
        private int incomingcount, incomingsize;
        private byte[] incomingheader, incomingheader2, incomingpacket;
        [QS.Fx.Base.Inspectable]
        private int incoming_seqno;

        // Lock 3
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Serialization.ISerializable> outgoing = new Queue<QS.Fx.Serialization.ISerializable>();
        [QS.Fx.Base.Inspectable]
        private int outgoingcount, outgoingsize;
        private IList<ArraySegment<byte>> outgoingpacket;
        [QS.Fx.Base.Inspectable]
        private int outgoing_seqno;
        [QS.Fx.Base.Inspectable]
        private bool working;

        // Lock 4
        private Socket socket;


        private QS.Fx.Logging.ILogger _logger;

        private int msgct = 0;

        #endregion

        private void _double_log(string message)
        {
            _logger.Log(message);
            System.Console.WriteLine(msgct.ToString() + ": " + message.Trim());
            msgct++;
        }

        #region _Configure

        protected void _Configure(long connectionid, ICryptoTransform encryptor, ICryptoTransform decryptor)
        {
            lock (this)
            {
                lock (this.incoming)
                {
                    lock (this.outgoing)
                    {
                        try
                        {
                            if (connected || working)
                                throw new Exception("Already connected or still working, must disconnect and cleanup first.");
                            this.connectionid = connectionid;
                            lock (this.outgoing)
                            {
                                this.outgoing.Clear();
                            }
                            lock (this.incoming)
                            {
                                this.incoming.Clear();
                            }
                            this.outgoing_seqno = 0;
                            this.incoming_seqno = 0;
                            this.encryptor = encryptor;
                            this.decryptor = decryptor;
                        }
                        catch (Exception exc)
                        {
                            _Disconnect();
                            throw new Exception("Could not connect the communication channel.", exc);
                        }
                    }
                }
            }
        }

        #endregion

        #region _Connect

        protected void _Connect(Socket socket)
        {
            lock (this)
            {
                lock (this.incoming)
                {
                    lock (this.outgoing)
                    {
                        try
                        {
                            if (connected || working)
                                throw new Exception("Already connected or still working, must disconnect and cleanup first.");
                            this.connected = true;
                            this.socket = socket;
                            this.incomingcount = 0;
                            this.incomingsize = incomingheader.Length;
                            this.socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                            if (!this.working && (this.outgoing.Count > 0))
                            {
                                this.working = true;
                                if (!_Outgoing())
                                    throw new Exception("Could not send the message, the channel has been disconnected.");
                            }
                        }
                        catch (Exception exc)
                        {
                            _Disconnect();
                            throw new Exception("Could not connect the communication channel.", exc);
                        }
                    }
                }
            }
        }

        #endregion

        #region _Disconnect

        protected void _Disconnect()
        {
            lock (this)
            {
                lock (this.incoming)
                {
                    lock (this.outgoing)
                    {
                        this.connected = false;
                        this.working = false;
                        this.connectionid = 0L;
                        this.incoming.Clear();
                        this.incoming_seqno = 0;
                        this.outgoing.Clear();
                        this.outgoing_seqno = 0;
                        this.encryptor = null;
                        this.decryptor = null;
                        if (this.socket != null)
                        {
                            try
                            {
                                this.socket.Close();
                            }
                            catch (Exception)
                            {
                            }
                            this.socket = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region _Exception

        protected abstract void _Exception(System.Exception _exception);

        #endregion

        #region _Send

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Asynchronous)]
        protected void _Send(QS.Fx.Serialization.ISerializable message)
        {
#if DEBUG_LINUX
            _double_log("Entering Peer._Send");
#endif
            lock (this.outgoing)
                this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Send_0_), message));
        }

        private unsafe void _Send_0_(object _o)
        {
            QS.Fx.Serialization.ISerializable message = (QS.Fx.Serialization.ISerializable)_o;
            lock (this)
            {
                lock (this.incoming)
                {
                    lock (this.outgoing)
                    {
                        this.outgoing.Enqueue(message);
                        if (this.connected && !this.working)
                        {
                            this.working = true;
                            if (!_Outgoing())
                                throw new Exception("Could not send the message, the channel has been disconnected.");
                        }
                    }
                }
            }
#if DEBUG_LINUX
            _double_log("Leaving Peer._Send");
#endif
        }

        #endregion

        #region _Receive

        protected abstract void _Receive(QS.Fx.Serialization.ISerializable message);

        #endregion

        #region _Outgoing

        private unsafe bool _Outgoing()
        {
#if DEBUG_LINUX
            _double_log("Entering Peer._Outgoing");
#endif
            try
            {
                QS.Fx.Serialization.ISerializable message = outgoing.Dequeue();
                QS.Fx.Serialization.SerializableInfo info = message.SerializableInfo;
                QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint)(info.HeaderSize + sizeof(long) + 3 * sizeof(uint) + sizeof(ushort)));
                IList<QS.Fx.Base.Block> blocks = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
                blocks.Add(header.Block);
                fixed (byte* headerptr = header.Array)
                {
                    *((long*)(headerptr)) = this.connectionid;
                    *((uint*)(headerptr + sizeof(long))) = (uint)++outgoing_seqno;
                    *((ushort*)(headerptr + sizeof(long) + sizeof(uint))) = info.ClassID;
                    *((uint*)(headerptr + sizeof(long) + sizeof(uint) + sizeof(ushort))) = (uint)info.HeaderSize;
                    *((uint*)(headerptr + sizeof(long) + 2 * sizeof(uint) + sizeof(ushort))) = (uint)info.Size;
                }
                header.consume(sizeof(long) + 3 * sizeof(uint) + sizeof(ushort));
                message.SerializeTo(ref header, ref blocks);
                MemoryStream memorystream = new MemoryStream();
                Stream _outs;
#if DEBUG_DISABLE_ENCRYPTION
                _outs = memorystream;
#else
                CryptoStream cryptostream = new CryptoStream(memorystream, this.encryptor, CryptoStreamMode.Write);
                _outs = cryptostream;
#endif
                foreach (QS.Fx.Base.Block _block in blocks)
                    _outs.Write(_block.buffer, (int)_block.offset, (int)_block.size);
#if DEBUG_DISABLE_ENCRYPTION
#else
                cryptostream.FlushFinalBlock();
                cryptostream.Close();
#endif
                byte[] _packetdata = memorystream.ToArray();
                int length = (int)_packetdata.Length;
                byte[] lengthbytes = BitConverter.GetBytes(length);
                this.outgoingpacket = new List<ArraySegment<byte>>();
                this.outgoingpacket.Add(new ArraySegment<byte>(lengthbytes));
                this.outgoingpacket.Add(new ArraySegment<byte>(_packetdata));
                this.outgoingsize = lengthbytes.Length + length;
                this.outgoingcount = 0;

#if LINUX_BEGINSEND_BUG
                byte[] newout = new byte[outgoingsize];
                int index = 0;
                foreach (ArraySegment<byte> arr in outgoingpacket)
                {
                    Array.Copy(arr.Array, 0, newout, index, arr.Array.Length);
                    index = index + arr.Array.Length;
                }

                socket.BeginSend(newout, 0, outgoingsize, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
#else
                socket.BeginSend(this.outgoingpacket, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
#endif

#if DEBUG_LINUX
                _double_log("Peer._Outgoing: Called BeginSend");
#endif
            }
            catch (Exception exc)
            {
#if DEBUG_LINUX
                _double_log("Caught exception in Peer._Outgoing");
#endif
                Monitor.Exit(this.outgoing);
                try
                {
                    _Disconnect();
                    try
                    {
                        _Exception(exc);
                    }
                    catch (Exception)
                    {
#if DEBUG_LINUX
                        _double_log("Caught inner exception in Peer._Outgoing");
#endif
                    }
                }
                finally
                {
                    Monitor.Enter(this.outgoing);
                }
                return false;
            }
#if DEBUG_LINUX
            _double_log("Leaving Peer._Outgoing");
#endif
            return true;
        }

        #endregion

        #region _SendCallback

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Asynchronous)]
        private unsafe void _SendCallback(IAsyncResult result)
        {
#if DEBUG_LINUX
            _double_log("Entering Peer._SendCallback");
#endif
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._SendCallback_0_), result));
        }

        private unsafe void _SendCallback_0_(object _o)
        {
            IAsyncResult result = (IAsyncResult)_o;
            try
            {
                lock (this)
                {
                    lock (this.incoming)
                    {
                        lock (this.outgoing)
                        {
                            SocketError _errorcode;
                            int _ntransmitted;
                            lock (this.socket)
                            {
                                _ntransmitted = this.socket.EndSend(result, out _errorcode);
                            }
                            if (_errorcode != SocketError.Success)
                                throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                            if (_ntransmitted < 0)
                                throw new Exception("Transmitted less than zero bytes?!");
                            this.outgoingcount += _ntransmitted;
                            if (this.outgoingcount < this.outgoingsize)
                            {
#if DEBUG_LINUX
                        _double_log("Peer._SendCallback: Sending more of the packet");
#endif
                                while (_ntransmitted >= this.outgoingpacket[0].Count)
                                {
                                    _ntransmitted -= this.outgoingpacket[0].Count;
                                    this.outgoingpacket.RemoveAt(0);
                                }
                                this.outgoingpacket[0] = new ArraySegment<byte>(
                                    this.outgoingpacket[0].Array, this.outgoingpacket[0].Offset + _ntransmitted, this.outgoingpacket[0].Count - _ntransmitted);
                                lock (this.socket)
                                {

#if LINUX_BEGINSEND_BUG
                                    byte[] newout = new byte[outgoingsize];
                                    int index = 0;
                                    foreach (ArraySegment<byte> arr in outgoingpacket)
                                    {
                                        Array.Copy(arr.Array, 0, newout, index, arr.Array.Length);
                                        index = index + arr.Array.Length;
                                    }

                                    socket.BeginSend(newout, 0, outgoingsize, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
#else
                                    socket.BeginSend(this.outgoingpacket, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
#endif
#if DEBUG_LINUX
                            _double_log("Peer._SendCallback: Calling BeginSend");
#endif
                                }
                            }
                            else
                            {
#if DEBUG_LINUX
                        _double_log("Peer._SendCallback: Done sending packet");
#endif
                                if (connected && outgoing.Count > 0)
                                    _Outgoing();
                                else
                                    this.working = false;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
#if DEBUG_LINUX
                _double_log("Caught exception in Peer._SendCallback");
#endif
                _Disconnect();
                try
                {
                    _Exception(exc);
                }
                catch (Exception)
                {
#if DEBUG_LINUX
                    _double_log("Caught inner exception in Peer._SendCallback");
#endif
                }
            }
#if DEBUG_LINUX
            _double_log("Leaving Peer._SendCallback");
#endif
        }

        #endregion

        #region _ReceiveCallback

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Asynchronous)]
        private unsafe void _ReceiveCallback(IAsyncResult result)
        {
#if DEBUG_LINUX
            _double_log("Entering Peer._ReceiveCallback");
#endif
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._ReceiveCallback_0_), result));
        }

        private unsafe void _ReceiveCallback_0_(object _o)
        {
            IAsyncResult result = (IAsyncResult)_o;
            try
            {
                lock (this)
                {
                    lock (this.incoming)
                    {
                        lock (this.outgoing)
                        {
                            SocketError _errorcode;
                            int _nreceived;
                            lock (this.socket)
                            {
                                _nreceived = this.socket.EndReceive(result, out _errorcode);
                            }
                            if (_errorcode != SocketError.Success)
                                throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                            if (_nreceived < 0)
                                throw new Exception("Received less than zero bytes?!");
                            this.incomingcount += _nreceived;
                            if (this.incomingcount < this.incomingsize)
                            {
                                if (this.incomingheader != null && this.incomingcount < this.incomingheader.Length && this.incomingsize <= this.incomingheader.Length)
                                {
#if DEBUG_LINUX
                            _double_log("Peer._ReceiveCallback: Still more header to receive, calling BeginReceive");
#endif
                                    lock (this.socket)
                                    {
                                        this.socket.BeginReceive(this.incomingheader, this.incomingcount, this.incomingsize - this.incomingcount,
                                            SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                                    }
                                }
                                else
                                    throw new Exception("Receive buffer is null, or the size of it does not match the expected size of the incoming packet.");
                            }
                            else
                            {
                                this.incomingcount = 0;
                                this.incomingsize = BitConverter.ToInt32(this.incomingheader, 0);
                                if (this.incomingsize > 0)
                                {
                                    incomingpacket = new byte[this.incomingsize];
                                    lock (this.socket)
                                    {
                                        this.socket.BeginReceive(incomingpacket, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback_2), null);
                                    }
                                }
                                else
                                    throw new Exception("Could not receive, the initial header indicates that the message is empty, disconnecting the channel.");
#if DEBUG_LINUX
                                _double_log("Peer._ReceiveCallback: Got all of header, calling BeginReceive on main packet");
#endif
                                
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
#if DEBUG_LINUX
                _double_log("Caught exception in Peer._ReceiveCallback");
#endif
                _Disconnect();
                try
                {
                    _Exception(exc);
                }
                catch (Exception)
                {
#if DEBUG_LINUX
                    _double_log("Caught inner exception in Peer._ReceiveCallback");
#endif
                }
            }
#if DEBUG_LINUX
            _double_log("Leaving Peer._ReceiveCallback");
#endif
        }

        #endregion

        #region _ReceiveCallback_2

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Asynchronous)]
        private unsafe void _ReceiveCallback_2(IAsyncResult result)
        {
#if DEBUG_LINUX
            _double_log("Entering Peer._ReceiveCallback_2");
#endif
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._ReceiveCallback_2_0_), result));
        }

        private unsafe void _ReceiveCallback_2_0_(object _o)
        {
            IAsyncResult result = (IAsyncResult)_o;
            try
            {
                lock (this)
                {
                    lock (this.incoming)
                    {
                        lock (this.outgoing)
                        {
                            SocketError _errorcode;
                            int _nreceived;
                            lock (this.socket)
                            {
                                _nreceived = this.socket.EndReceive(result, out _errorcode);
                            }
                            if (_errorcode != SocketError.Success)
                                throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                            if (_nreceived < 0)
                                throw new Exception("Received less than zero bytes?!");
                            this.incomingcount += _nreceived;
                            if (this.incomingcount < this.incomingsize)
                            {
                                if (this.incomingpacket != null && this.incomingcount < this.incomingpacket.Length && this.incomingsize <= this.incomingpacket.Length)
                                {
#if DEBUG_LINUX
                            _double_log("Peer._ReceiveCallback_2: Still more packet data to be received, calling BeginReceive again");
#endif

                                    lock (this.socket)
                                    {
                                        this.socket.BeginReceive(this.incomingpacket, this.incomingcount, this.incomingsize - this.incomingcount,
                                            SocketFlags.None, new AsyncCallback(this._ReceiveCallback_2), null);
                                    }
                                }
                                else
                                    throw new Exception("Receive buffer is null, or the size of it does not match the expected size of the incoming packet.");
                            }
                            else
                            {
                                MemoryStream memorystream = new MemoryStream(this.incomingpacket);
                                this.incomingpacket = null;
                                Stream _ins;
#if DEBUG_DISABLE_ENCRYPTION
                                _ins = memorystream;
#else
                        CryptoStream cryptostream = new CryptoStream(memorystream, this.decryptor, CryptoStreamMode.Read);
                        _ins = cryptostream;
#endif
                                int _ndecrypted = 0;
                                while (_ndecrypted < incomingheader2.Length)
                                {
                                    int _ndecrypted_now = _ins.Read(incomingheader2, _ndecrypted, incomingheader2.Length - _ndecrypted);
                                    if (_ndecrypted_now < 0)
                                        throw new Exception("Decrypted less than zero bytes?!");
                                    _ndecrypted += _ndecrypted_now;
                                }
                                long _incoming_header_connectionid;
                                int _incoming_header_seqno;
                                ushort _incoming_header_classid;
                                uint _incoming_header_headersize, _incoming_header_messagesize;
                                fixed (byte* headerptr = incomingheader2)
                                {
                                    _incoming_header_connectionid = *((long*)(headerptr));
                                    _incoming_header_seqno = (int)(*((uint*)(headerptr + sizeof(long))));
                                    _incoming_header_classid = *((ushort*)(headerptr + sizeof(long) + sizeof(uint)));
                                    _incoming_header_headersize = *((uint*)(headerptr + sizeof(long) + sizeof(uint) + sizeof(ushort)));
                                    _incoming_header_messagesize = *((uint*)(headerptr + sizeof(long) + 2 * sizeof(uint) + sizeof(ushort)));
                                }
                                if (_incoming_header_connectionid != this.connectionid)
                                    throw new Exception("Cannot receive, the received packet has connection id = " + _incoming_header_connectionid.ToString() +
                                        ", which is different from the local connectionid = " + this.connectionid.ToString());
                                this.incoming_seqno++;
                                if (_incoming_header_seqno != this.incoming_seqno)
                                    throw new Exception("Cannot receive, the received packet has seqno = " + _incoming_header_seqno.ToString() +
                                        ", which is different from the local seqno = " + this.incoming_seqno.ToString());
                                // int length = (int)cryptostream.Length;
                                // if (_incoming_header_messagesize != length)
                                //    throw new Exception("Cannot receive, the encrypted stream has " + length.ToString() +
                                //        " bytes, but according to the header, the message was supposed to have " + _incoming_header_messagesize.ToString() + " bytes.");
                                byte[] _packet = new byte[_incoming_header_messagesize];
                                _ins.Read(_packet, 0, (int)_incoming_header_messagesize);
#if DEBUG_DISABLE_ENCRYPTION
#else
                        cryptostream.Close();
#endif
                                QS.Fx.Serialization.ISerializable message = QS._core_c_.Base3.Serializer.CreateObject(_incoming_header_classid);
                                QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(_packet, 0, _incoming_header_headersize);
                                QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(
                                    _packet, _incoming_header_headersize, _incoming_header_messagesize - _incoming_header_headersize);
                                try
                                {
                                    message.DeserializeFrom(ref header, ref data);
                                }
                                catch (Exception _exc)
                                {
                                    throw new Exception("Could not deserialize the incoming message.", _exc);
                                }
                                bool _this_locked = true;
                                incoming.Enqueue(message);
                                this.incomingcount = 0;
                                this.incomingsize = this.incomingheader.Length;
#if DEBUG_LINUX
                        _double_log("Peer._ReceiveCallback_2: Received all packet data ("+message.ToString()+"), calling BeginReceive for a new packet");
#endif
                                lock (this.socket)
                                {
                                    this.socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                                }
                                while (incoming.Count > 0)
                                    _Receive(incoming.Dequeue());
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
#if DEBUG_LINUX
                _double_log("Caught exception in Peer._ReceiveCallback_2");
#endif
                _Disconnect();
                try
                {
                    _Exception(exc);
                }
                catch (Exception)
                {
#if DEBUG_LINUX
                    _double_log("Caught inner exception in Peer._ReceiveCallback_2");
#endif
                }
            }
#if DEBUG_LINUX
            _double_log("Leaving Peer._ReceiveCallback_2");
#endif
        }

        #endregion
    }
}
