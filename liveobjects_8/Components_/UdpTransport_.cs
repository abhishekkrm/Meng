/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
    [QS.Fx.Reflection.ComponentClass("9B16F281587747b4B3CEBA1F63B4D423", "UdpTransport")]
    class UdpTransport_<

        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        :
        //QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
        IDisposable
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        #region Constructors and Destructors

        public UdpTransport_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)] string _address_param,
            [QS.Fx.Reflection.Parameter("PacketSizeLimit", QS.Fx.Reflection.ParameterClass.Value)] string _pack_size)
            : base(_mycontext, true)
        {
            this._mycontext = _mycontext;

            this._transport_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
                QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>>(this);

            //this._transport_endpt.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);

            this._transport_endpt.OnConnect +=
                new QS.Fx.Base.Callback(
                    this._Connect
                    );
            this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._ConnectedCallback);
            this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);

            QS._qss_c_.Base1_.Subnet _subnet;

            int _i = _address_param.IndexOf(':');
            if (_i >= 0)
            {
                _subnet = new QS._qss_c_.Base1_.Subnet(_address_param.Substring(0, _i));
                this._port = Convert.ToInt32(_address_param.Substring(_i + 1));
            }
            else
            {
                _subnet = new QS._qss_c_.Base1_.Subnet(_address_param);
                Random r = new Random();
                this._port = 20000 + r.Next(20000);
            }

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

            try
            {
                this._transport_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this._transport_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this._transport_socket.Bind(new IPEndPoint(this._ipaddr, this._port));
            }
            catch (Exception)
            {
                throw new Exception("Failed to setup socket.");
            }

            if (this._transport_socket == null)
                throw new Exception("Could not setup the TCP transport socket.");

            this._addr = this._ipaddr + ":" + _port;

            // Set to receive from all addresses
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempRemoteEP = (EndPoint)sender;

            // Set packet size limitation to receive
            this.incomingsize = int.Parse(_pack_size);
            this.incomingpacket = new byte[this.incomingsize];
            this.incomingheader = new byte[2 * sizeof(uint) + sizeof(ushort)];

            try
            {
                lock (this._transport_socket)
                {
                    this._transport_socket.BeginReceiveFrom(incomingpacket, 0, incomingpacket.Length, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(this._ReceiveCallback), null);
                }
            }
            catch (SocketException socketexc)
            {
                this._transport_socket.Close();
                throw new Exception("Socket exception.\r\n" + socketexc.ErrorCode);
            }
            catch (Exception exc)
            {
                this._transport_socket.Close();
                throw new Exception("Exception when BeginReceiveFrom.\r\n");
            }
        }

        ~UdpTransport_()
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

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>> _transport_endpt;

        [QS.Fx.Base.Inspectable]
        private IPAddress _ipaddr;

        [QS.Fx.Base.Inspectable]
        private Socket _transport_socket;

        public Socket TransportSocket
        {
            get { return _transport_socket; }
        }

        [QS.Fx.Base.Inspectable]
        private string _addr;

        [QS.Fx.Base.Inspectable]
        private int _port;

        [QS.Fx.Base.Inspectable]
        private IDictionary<string, UdpCommunicationChannel> _channels = new Dictionary<string, UdpCommunicationChannel>();

        [QS.Fx.Base.Inspectable]
        private int incomingsize;

        [QS.Fx.Base.Inspectable]
        private byte[] incomingheader, incomingpacket;

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

        private void _Connect()
        {
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect)));
        }

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            int a = 1;
        }

        private void _ConnectCallback()
        {
            int a = 1;
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

        #region _ReceiveCallback

        private unsafe void _ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int _nreceived;
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint _remep = (EndPoint)sender;
                lock (this._transport_socket)
                {
                    try
                    {
                        _nreceived = this._transport_socket.EndReceiveFrom(result, ref _remep);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("EndReceiveFrom Exception. /r/n");
                    }
                }
                if (_nreceived > 0)
                {
                    // Succeed received
                    IPEndPoint _sender = (IPEndPoint)_remep;
                    UdpCommunicationChannel _channel;

                    // Deserialize packet and check
                    MemoryStream memorystream = new MemoryStream(this.incomingpacket);
                    //this.incomingpacket = null;
                    Stream _ins;
                    _ins = memorystream;

                    int _ndecrypted = 0;
                    while (_ndecrypted < incomingheader.Length)
                    {
                        int _ndecrypted_now = _ins.Read(incomingheader, _ndecrypted, incomingheader.Length - _ndecrypted);
                        if (_ndecrypted_now < 0)
                            throw new Exception("Decrypted less than zero bytes?!");
                        _ndecrypted += _ndecrypted_now;
                    }
                    ushort _incoming_header_classid;
                    uint _incoming_header_headersize, _incoming_header_messagesize;
                    fixed (byte* headerptr = incomingheader)
                    {
                        _incoming_header_classid = *((ushort*)(headerptr));
                        _incoming_header_headersize = *((uint*)(headerptr + sizeof(ushort)));
                        _incoming_header_messagesize = *((uint*)(headerptr + sizeof(uint) + sizeof(ushort)));
                    }

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

                    // Check channel and push packet 
                    lock (this._channels)
                    {
                        if (!_channels.TryGetValue(_sender.ToString(), out _channel))
                        {
                            _channel = new UdpCommunicationChannel(this, _sender.ToString());

                            _channels.Add(_sender.ToString(), _channel);

                            // Give the new channel to transport client
                            QS.Fx.Object.IReference<
                                QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>
                                > refer = QS._qss_x_.Object_.Reference<
                                QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                            _channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));
                            this._transport_endpt.Interface.Connected(new QS.Fx.Base.Address(_sender.ToString()), refer);
                        }

                        if (this._transport_endpt.IsConnected)
                        {
                            EndPoint tempRemoteEP = (EndPoint)sender;
                            lock (_transport_socket)
                            {
                                try
                                {
                                    this._transport_socket.BeginReceiveFrom(incomingpacket, 0, incomingpacket.Length, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(this._ReceiveCallback), null);
                                }
                                catch (Exception exc)
                                {

                                }
                            }
                        }

                        try
                        {
                            _channel._Incoming((MessageClass)message);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Can not put message to channel. \r\n");
                        }
                    }
                }
                else
                {
                    throw new Exception("Receive data number.\r\n" + _nreceived);
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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransport<string,string> Members

        void QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>.Connect(QS.Fx.Base.Address address)
        {
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> refer;
            lock (this._channels)
            {
                UdpCommunicationChannel _channel;

                if (!this._channels.TryGetValue(address.String, out _channel))
                {
                    _channel = new UdpCommunicationChannel(this, address.String);

                    this._channels.Add(address.String, _channel);
                }

                refer = QS._qss_x_.Object_.Reference<
                                                    QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                                                        _channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));
            }

            try
            {
                _transport_endpt.Interface.Connected(new QS.Fx.Base.Address(address.String), refer);
            }
            catch (Exception exc)
            {

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

        #region Class UdpCommunicationChannel

        public class UdpCommunicationChannel :
            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>,
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>
        {
            #region Constructor

            public UdpCommunicationChannel(UdpTransport_<MessageClass> _transport, string _address)
            {
                this._udp_endpt = _transport._mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>(this);
                this._udp_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
                this._udp_endpt.OnConnected += new QS.Fx.Base.Callback(this._ConnectedCallback);
                if (_address != null)
                {

                    string[] dest = _address.Split(':');
                    this._ipaddr = IPAddress.Parse(dest[0]);
                    if (dest.Length < 2)
                    {
                        throw new Exception("Need to provide a port to send packet to.");

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
                this._socket = _transport.TransportSocket;
                IPEndPoint _remoteipep = new IPEndPoint(this._ipaddr, this._port);
                this._remoteep = (EndPoint)_remoteipep;
            }

            #endregion

            #region _ConnectedCallback

            private void _ConnectedCallback()
            {
                while (this.incoming.Count > 0)
                {

                    this._udp_endpt.Interface.Message((MessageClass)this.incoming.Dequeue());
                }
            }

            #endregion

            #region _DisconnectCallback

            public void _DisconnectCallback()
            {
                if (this._udp_endpt != null && this._udp_endpt.IsConnected)
                    this._udp_endpt.Disconnect();
                _transport._channels.Remove(_ipaddr.ToString() + ":" + _port.ToString());
            }

            #endregion

            #region Fields

            public QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> _udp_endpt;
            private UdpTransport_<MessageClass> _transport;

            private IPAddress _ipaddr;
            private int _port;
            private EndPoint _remoteep;
            private Socket _socket;

            private Queue<QS.Fx.Serialization.ISerializable> incoming = new Queue<QS.Fx.Serialization.ISerializable>();
            private Queue<QS.Fx.Serialization.ISerializable> outgoing = new Queue<QS.Fx.Serialization.ISerializable>();

            private bool _pending_send;

            #endregion

            #region _Outgoing

            private void _Outgoing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
                QS._qss_x_.Properties_.Base_.IEvent_<MessageClass> _event_
                    = (QS._qss_x_.Properties_.Base_.IEvent_<MessageClass>)_event;

                MessageClass message = _event_._Object;

                try
                {
                    lock (outgoing)
                    {
                        bool pending = outgoing.Count > 0;
                        this.outgoing.Enqueue(message);
                        if (!pending)
                        {
                            this._transport._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._ScheduleSend)));
                        }
                    }
                }
                catch (Exception exc)
                {
                    //To do
                }
            }

            #endregion

            #region _Incoming

            public void _Incoming(MessageClass message)
            {
                // Put deserialized packet in queue
                lock (incoming)
                {
                    incoming.Enqueue(message);
                    if (this._udp_endpt.IsConnected)
                    {
                        while (this.incoming.Count > 0)
                        {
                            this._udp_endpt.Interface.Message((MessageClass)(incoming.Dequeue()));
                        }
                    }
                }
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region ICommunicationChannel<MessageClass> Members

            QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>, QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>.Channel
            {
                get { return _udp_endpt; }
            }

            #endregion

            #region ICommunicationChannel<MessageClass> Members

            unsafe void QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>.Message(MessageClass message)
            {
                this._transport._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<MessageClass>(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Outgoing), message));
                //_Outgoing(message);
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

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
                            _ntransmitted = this._socket.EndSendTo(result);
                        }

                        // Need this?
                        if (_ntransmitted < 0)
                        {
                            throw new Exception("Socket error!");
                        }

                        //if (outgoing.Count > 0)
                        //    _SerializeAndSend();
                    }
                }
                catch (SocketException sockexc)
                {
                    _DisconnectCallback();
                    throw new Exception("Socket exception, close and code.\n\r" + sockexc.ErrorCode);
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

            #region _SerializeAndSend

            private unsafe bool _SerializeAndSend()
            {
                try
                {
                    QS.Fx.Serialization.ISerializable message = this.outgoing.Dequeue();
                    QS.Fx.Serialization.SerializableInfo info = message.SerializableInfo;
                    QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(
                        (uint)(info.HeaderSize + 2 * sizeof(uint) + sizeof(ushort)));
                    IList<QS.Fx.Base.Block> blocks = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
                    blocks.Add(header.Block);
                    fixed (byte* headerptr_0 = header.Array)
                    {
                        byte* headerptr = headerptr_0 + header.Offset;
                        *((ushort*)(headerptr)) = (ushort)info.ClassID;
                        headerptr += sizeof(ushort);
                        *((uint*)headerptr) = (uint)info.HeaderSize;
                        headerptr += sizeof(uint);
                        *((uint*)headerptr) = (uint)info.Size;
                    }
                    header.consume(2 * sizeof(uint) + sizeof(ushort));
                    message.SerializeTo(ref header, ref blocks);
                    MemoryStream memorystream = new MemoryStream();
                    Stream _outs;

                    _outs = memorystream;

                    foreach (QS.Fx.Base.Block _block in blocks)
                        _outs.Write(_block.buffer, (int)_block.offset, (int)_block.size);

                    byte[] _packetdata = memorystream.ToArray();
                    int length = (int)_packetdata.Length;

                    lock (_socket)
                    {
                        try
                        {
                            _socket.BeginSendTo(_packetdata, 0, length, SocketFlags.None, this._remoteep, new AsyncCallback(this._SendCallback), null);
                        }
                        catch (SocketException socketc)
                        {
                            _DisconnectCallback();
                        }
                        catch (Exception exc)
                        {

                        }
                    }
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

            #region _ScheduleSend

            private void _ScheduleSend(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
                lock (this.outgoing)
                {
                    while (this.outgoing.Count > 0)
                    {
                        _SerializeAndSend();
                    }
                }
            }

            #endregion

        }

        #endregion

    }
}
