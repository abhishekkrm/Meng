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
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Properties_
{

    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TcpTransport, "TcpTransport")]
    public sealed class TcpTransport_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        :
        QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
        IDisposable
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        #region Constructors and Destructors

        public TcpTransport_(
            QS.Fx.Object.IContext _mycontext, 
            [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)] string _address_param)
        {
            this._mycontext = _mycontext;

            this._transport_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
                QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>>(this);

            this._transport_endpt.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);
            this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._ConnectedCallback);
            this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);

            if (_address_param != null)
            {

                string[] dest = _address_param.Split(':');
                this._ipaddr = IPAddress.Parse(dest[0]);
                if (dest.Length < 2)
                {
                    Random gen = new Random();
                    this._port = 20000 + gen.Next(20000);
                }
                else
                {

                    this._port = Convert.ToInt32(dest[1]);
                }
            }
            else
            {
                // this function needs to be moved ??
                QS._qss_c_.Base1_.Subnet _subnet = new QS._qss_c_.Base1_.Subnet(_address_param.Substring(0, ':'));

                bool _found = false;
                foreach (IPAddress _ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_ipaddress.AddressFamily == AddressFamily.InterNetwork && _subnet.contains(_ipaddress))
                    {
                        this._ipaddr = _ipaddress;
                        _found = true;
                        break;
                    }
                }
                if (!_found)
                    throw new Exception("Could not find route to " + _subnet.ToString() + ".");

                Random gen = new Random();

                this._port = 20000 + gen.Next(20000);
            }
            try
            {
                this._transport_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._transport_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this._transport_socket.Bind(new IPEndPoint(this._ipaddr, this._port));
            }
            catch (Exception)
            {
                throw new Exception("Failed to setup socket.");
            }

            if (this._transport_socket == null)
                throw new Exception("Could not setup the TCP transport socket.");
            this._transport_socket.Listen(5);
            this._addr = this._ipaddr + ":" + _port;
            this._transport_socket.BeginAccept(this._AcceptCallback, null);

        }

        ~TcpTransport_()
        {
            this._Dispose(false);
        }

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                    this._Disconnect();
                }
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private int _disposed;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>> _transport_endpt;

        private IPAddress _ipaddr;
        private Socket _transport_socket;
        private string _addr;
        private int _port;

        private IDictionary<string, TcpCommunicationChannel> _channels = new Dictionary<string, TcpCommunicationChannel>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Disconnect

        private void _Disconnect()
        {
            lock (this._transport_socket)
            {
                if (this._transport_socket != null)
                {
                    try
                    {
                        this._transport_socket.Close();
                    }
                    catch (Exception) { }
                }
            }
        }

        #endregion

        #region _ConnectCallback

        private void _ConnectCallback()
        {
        }

        #endregion

        #region _ConnectedCallback

        private void _ConnectedCallback()
        {
            this._transport_endpt.Interface.Address(new QS.Fx.Base.Address(this._addr));
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback()
        {
        }

        #endregion

        #region _AcceptCallback

        private void _AcceptCallback(IAsyncResult _result)
        {
            lock (this._transport_socket)
            {
                if (this._transport_socket != null)
                {
                    try
                    {

                        SocketReceiver__ _initialization = new SocketReceiver__(this, this._transport_socket.EndAccept(_result));
                        this._transport_socket.BeginAccept(this._AcceptCallback, null);
                    }
                    catch (Exception)
                    {
                        this._Disconnect();
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransport<string,string> Members

        void QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>.Connect(QS.Fx.Base.Address address)
        {


            lock (this._channels)
            {
                TcpCommunicationChannel _channel;
                if (!this._channels.TryGetValue(address.String, out _channel))
                {
                    _channel = new TcpCommunicationChannel(this, address.String);

                    this._channels.Add(address.String, _channel);
                    _channel._Outgoing();
                }

                else if (_channel.Connected)
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>
                                                > refer = QS._qss_x_.Object_.Reference<
                                                    QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                                                        _channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));
                    _transport_endpt.Interface.Connected(new QS.Fx.Base.Address(address.String), refer);
                }
                else if (!_channel.Connected && !_channel.Connecting)
                {
                    _channel._Outgoing();
                }

            }
        }

        #endregion

        #region ITransport<string,string> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>>
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, MessageClass>.Transport
        {
            get { return this._transport_endpt; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class SocketReceiver_

        private sealed class SocketReceiver__
        {
            public SocketReceiver__(TcpTransport_<MessageClass> _transport, Socket _socket)
            {
                this._transport = _transport;

                this._endpoint = (IPEndPoint)_socket.RemoteEndPoint;
                TcpCommunicationChannel __channel;



                if (!_transport._channels.TryGetValue(_endpoint.ToString(), out __channel))
                {
                    lock (_transport._channels)
                    {
                        TcpCommunicationChannel _channel = new TcpCommunicationChannel(this._transport, _endpoint.ToString());
                        _channel._Incoming(_socket);

                        this._transport._channels.Add(_endpoint.ToString(), _channel);


                        QS.Fx.Object.IReference<
                            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>
                            > refer = QS._qss_x_.Object_.Reference<
                                QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                                    _channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));
                        this._transport._transport_endpt.Interface.Connected(new QS.Fx.Base.Address(_endpoint.ToString()), refer);
                    }

                }
                else
                {


                    //lock (_transport._channels)
                    //{
                    //    __channel._DisconnectCallback();
                    //    TcpCommunicationChannel _channel = new TcpCommunicationChannel(this._transport, _endpoint.ToString());
                    //    _channel._Incoming(_socket);

                    //    this._transport._channels.Add(_endpoint.ToString(), _channel);


                    //    QS.Fx.Object.IReference<
                    //        QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>
                    //        > refer = QS._qss_x_.Object_.Reference<
                    //            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                    //                _channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));
                    //    this._transport._transport_endpt.Interface.Connected(new QS.Fx.Base.Address(_endpoint.ToString()), refer);
                    //}

                }
            }


            private TcpTransport_<MessageClass> _transport;
            private IPEndPoint _endpoint;

        }



        #endregion

        #region Class TcpCommunicationChannel

        public class TcpCommunicationChannel :
            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>,
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>
        {
            #region Constructor

            public TcpCommunicationChannel(TcpTransport_<MessageClass> _transport, string _address)
            {
                this._tcp_endpt = _transport._mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>(this);
                this._tcp_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
                this._tcp_endpt.OnConnected += new QS.Fx.Base.Callback(this._ConnectedCallback);
                if (_address != null)
                {

                    string[] dest = _address.Split(':');
                    this._ipaddr = IPAddress.Parse(dest[0]);
                    if (dest.Length < 2)
                    {
                        throw new Exception("Need to provide a port to connect to.");

                    }
                    else
                    {
                        this._port = Convert.ToInt32(dest[1]);
                    }
                }
                else
                {
                    throw new Exception("Need to provide an address to connect to.");
                }

                this._transport = _transport;

                incomingheader = BitConverter.GetBytes((int)0);
                incomingheader2 = new byte[sizeof(long) + 3 * sizeof(uint) + sizeof(ushort)];
            }

            #endregion

            #region _ConnectedCallback

            private void _ConnectedCallback()
            {
                while (this.incoming.Count > 0)
                {

                    this._tcp_endpt.Interface.Message((MessageClass)this.incoming.Dequeue());
                }
            }
            
            #endregion

            #region _DisconnectCallback


            public void _DisconnectCallback()
            {
                Connected = false;
                Connecting = false;
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                if (this._tcp_endpt != null && this._tcp_endpt.IsConnected)
                    this._tcp_endpt.Disconnect();
                _transport._channels.Remove(_ipaddr.ToString() + ":" + _port.ToString());
            }

            #endregion

            #region Fields

            public QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> _tcp_endpt;
            private TcpTransport_<MessageClass> _transport;

            private IPAddress _ipaddr;
            private int _port;
            private Socket _socket;
            private int incomingcount, incomingsize;
            private byte[] incomingheader, incomingheader2, incomingpacket;
            private int incoming_seqno;
            private long connectionid;
            private Queue<QS.Fx.Serialization.ISerializable> incoming = new Queue<QS.Fx.Serialization.ISerializable>();
            public bool Connecting = false;
            public bool Connected = false;

            private int outgoingcount, outgoingsize;
            private IList<ArraySegment<byte>> outgoingpacket;
            private int outgoing_seqno;
            private Queue<QS.Fx.Serialization.ISerializable> outgoing = new Queue<QS.Fx.Serialization.ISerializable>();

            #endregion



            #region _Outgoing

            public void _Outgoing()
            {
                if (_socket != null && _socket.Connected)
                    throw new Exception("Outgoing socket already exists.");
                try
                {
                    this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _socket.Bind(new IPEndPoint(this._transport._ipaddr, this._transport._port));

                    // should probly become beginconnect 
                    _socket.BeginConnect(new IPEndPoint(this._ipaddr, this._port), new AsyncCallback(this._ConnectCallback), null);
                    //_socket.Connect(new IPEndPoint(this._ipaddr, this._port));
                    Connecting = true;



                }
                catch (Exception e)
                {
                    _socket = null;

                    // silent fail?
                }
            }

            #endregion

            #region _Incoming

            public void _Incoming(Socket _incomingsocket)
            {
                if (_socket != null)
                    throw new Exception("Already have a socket for this Tcp connection");
                Connected = true;
                this.incomingcount = 0;
                this.incomingsize = incomingheader.Length;
                this._socket = _incomingsocket;
                this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
            }

            #endregion





            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region ICommunicationChannel<MessageClass> Members

            QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>, QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>.Channel
            {
                get { return _tcp_endpt; }
            }

            #endregion

            #region ICommunicationChannel<MessageClass> Members

            unsafe void QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>.Message(MessageClass message)
            {

                lock (this.outgoing)
                {
                    this.outgoing.Enqueue(message);
                    this._SerializeAndSend();
                }
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _ConnectCallback


            private void _ConnectCallback(IAsyncResult _result)
            {
                SocketError _errorcode;
                try
                {

                    this._socket.EndConnect(_result);

                }
                catch (SocketException e)
                {


                    this._socket = null;

                    return;
                    // connect fails

                    // maybe remove the channel from dictionary here
                    // alternative is to allow them to call connect again
                    // either way they can just call connect again to try, though they never know a connect attempt fails, 
                    // perhaps give them a channel and immediately disconnect it or something
                }
                if (this._socket.Connected)
                {
                    Connected = true;
                    QS.Fx.Object.IReference<
                        QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>
                        > refer = QS._qss_x_.Object_.Reference<
                                    QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                                        this, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));

                    this.incomingcount = 0;
                    this.incomingsize = incomingheader.Length;


                    this._transport._transport_endpt.Interface.Connected(new QS.Fx.Base.Address(this._ipaddr, this._port), refer);


                    this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                    //_socket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), this);
                }
            }

            #endregion

            #region _SendCallback

            private void _SendCallback(IAsyncResult result)
            {
                try
                {
                    lock (this.outgoing)
                    {
                        SocketError _errorcode;
                        int _ntransmitted;
                        lock (this._socket)
                        {
                            _ntransmitted = this._socket.EndSend(result, out _errorcode);
                        }
                        if (_errorcode != SocketError.Success)
                            throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                        if (_ntransmitted < 0)
                            throw new Exception("Transmitted less than zero bytes?!");
                        this.outgoingcount += _ntransmitted;
                        if (this.outgoingcount < this.outgoingsize)
                        {
                            while (_ntransmitted >= this.outgoingpacket[0].Count)
                            {
                                _ntransmitted -= this.outgoingpacket[0].Count;
                                this.outgoingpacket.RemoveAt(0);
                            }
                            this.outgoingpacket[0] = new ArraySegment<byte>(
                                this.outgoingpacket[0].Array, this.outgoingpacket[0].Offset + _ntransmitted, this.outgoingpacket[0].Count - _ntransmitted);
                            lock (this._socket)
                            {
                                _socket.BeginSend(this.outgoingpacket, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
                            }
                        }
                        else
                        {
                            if (outgoing.Count > 0)
                                _SerializeAndSend();
                        }
                    }
                }
                catch (Exception exc)
                {
                    _DisconnectCallback();
                    try
                    {
                        throw new Exception("Caught exception trying to send, disconnecting.\r\n" + exc.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            #endregion

            #region _ReceiveCallback

            private void _ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    lock (this.incoming)
                    {
                        SocketError _errorcode;
                        int _nreceived;
                        lock (this._socket)
                        {
                            _nreceived = this._socket.EndReceive(result, out _errorcode);
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
                                lock (this._socket)
                                {
                                    this._socket.BeginReceive(this.incomingheader, this.incomingcount, this.incomingsize - this.incomingcount,
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
                                lock (this._socket)
                                {
                                    this._socket.BeginReceive(incomingpacket, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback_2), null);
                                }
                            }
                            else
                                throw new Exception("Could not receive, the initial header indicates that the message is empty, disconnecting the channel.");
                        }
                    }
                }
                catch (Exception exc)
                {
                    this._DisconnectCallback();
                    try
                    {
                        throw new Exception("Caught exception receiving, disconnecting.\r\n" + exc.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            #endregion

            #region _ReceiveCallback_2

            private unsafe void _ReceiveCallback_2(IAsyncResult result)
            {
                try
                {
                    lock (this.incoming)
                    {
                        SocketError _errorcode;
                        int _nreceived;
                        lock (this._socket)
                        {

                            _nreceived = this._socket.EndReceive(result, out _errorcode);
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
                                lock (this._socket)
                                {
                                    this._socket.BeginReceive(this.incomingpacket, this.incomingcount, this.incomingsize - this.incomingcount,
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
                            _ins = memorystream;

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
                            //bool _this_locked = true;
                            incoming.Enqueue(message);
                            this.incomingcount = 0;
                            this.incomingsize = this.incomingheader.Length;
                            lock (this._socket)
                            {
                                this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                            }
                            if (this._tcp_endpt.IsConnected)
                            {
                                while (this.incoming.Count > 0)
                                    this._tcp_endpt.Interface.Message((MessageClass)incoming.Dequeue());
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    this._DisconnectCallback();
                    try
                    {
                        throw new Exception("Caught exception trying to receive2, disconnecting.\r\n" + exc.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            #endregion

            #region _SerializeAndSend

            private unsafe bool _SerializeAndSend()
            {
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

                    _outs = memorystream;

                    foreach (QS.Fx.Base.Block _block in blocks)
                        _outs.Write(_block.buffer, (int)_block.offset, (int)_block.size);

                    byte[] _packetdata = memorystream.ToArray();
                    int length = (int)_packetdata.Length;
                    this.outgoingpacket = new List<ArraySegment<byte>>();
                    byte[] lengthbytes = BitConverter.GetBytes(length);
                    this.outgoingpacket.Add(new ArraySegment<byte>(lengthbytes));
                    this.outgoingpacket.Add(new ArraySegment<byte>(memorystream.ToArray()));
                    this.outgoingsize = lengthbytes.Length + length;
                    this.outgoingcount = 0;
                    _socket.BeginSend(this.outgoingpacket, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
                }
                catch (Exception exc)
                {
                    try
                    {
                        _DisconnectCallback();
                        try
                        {
                            throw new Exception("Caught exception trying to send, disconnecting.\r\n" + exc.ToString());
                        }
                        catch (Exception)
                        {
                        }
                    }
                    catch (Exception) { }

                }
                return true;
            }

            #endregion

        }

        #endregion

    }
}
