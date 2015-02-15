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

#define VERBOSE

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
using System.Timers;

namespace QS._qss_x_.Properties_
{
    [QS.Fx.Reflection.ComponentClass("F9FA1873C0244c399406519A3AD373A4", "STUNTransport")]
    public sealed class STUNTransport_<

        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>,
        QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {

        #region Construct

        public STUNTransport_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("STUN Server Address", QS.Fx.Reflection.ParameterClass.Value)]
            string _server_addr,
            [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)]
            string _self_addr,
            [QS.Fx.Reflection.Parameter("Traverse Timout Seconds, 60 By Default", QS.Fx.Reflection.ParameterClass.Value)]
            int _timeout,
            [QS.Fx.Reflection.Parameter("PacketSizeLimit", QS.Fx.Reflection.ParameterClass.Value)] 
            string _pack_size,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug)
            : base(_mycontext, _debug)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNTransport Constructor");
#endif

            this._mycontext = _mycontext;

            this._stuntransport_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>,
                QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>>(this);

            this.punchingTimeout = _timeout;
            if (_timeout == null || _timeout == 0)
            {
                this.punchingTimeout = 60;
            }

            this._server_addr = _server_addr;
            try
            {
                IPAddress.Parse(_server_addr.Split(':')[0]);
            }
            catch (Exception exc)
            {
#if VERBOSE
                if (this._logger != null)
                {
                    this._logger.Log("Component_.STUNTransport STUN Server Address Check Exception: " + exc.Message);
                    this._logger.Log("Component_.STUNTransport STUN NAT-Traverse Function Down!");
                }
#endif
                this._server_addr = null;
            }

            // Bind local socket
            this._addr = _self_addr;
            Bind();

            // Create queue for messages
            this.incomingMessageQueue = new Queue<QS._qss_x_.Properties_.Base_.IEvent_>();
            this.outgoingMessageQueue = new Queue<QS._qss_x_.Properties_.Base_.IEvent_>();

            // Create channels
            this._channels = new Dictionary<string, UdpCommunicationChannel>();
            this.addrmapping = new Dictionary<string, QS.Fx.Value.STUNAddress>();

            // Create punching
            this.punchingDict = new Dictionary<string, PunchTask>();

            // Set callback
            this._stuntransport_endpt.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
            this._stuntransport_endpt.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });

            // Set packet size limitation to receive
            this.incomingsize = int.Parse(_pack_size);
            this.incomingpacket = new byte[this.incomingsize];
            this.incomingheader = new byte[2 * sizeof(uint) + sizeof(ushort)];

            // Start receiving
            Incoming();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>> _stuntransport_endpt;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.STUNAddress _local_stun_addr;

        [QS.Fx.Base.Inspectable]
        private string _addr;

        private string _server_addr;

        [QS.Fx.Base.Inspectable]
        private IPAddress _ipaddr;

        [QS.Fx.Base.Inspectable]
        private Socket _transport_socket;

        [QS.Fx.Base.Inspectable]
        private int _port;

        [QS.Fx.Base.Inspectable]
        private int incomingsize;

        [QS.Fx.Base.Inspectable]
        private byte[] incomingheader, incomingpacket;

        [QS.Fx.Base.Inspectable]
        private Queue<QS._qss_x_.Properties_.Base_.IEvent_> incomingMessageQueue;

        [QS.Fx.Base.Inspectable]
        private Queue<QS._qss_x_.Properties_.Base_.IEvent_> outgoingMessageQueue;

        [QS.Fx.Base.Inspectable]
        private Dictionary<string, UdpCommunicationChannel> _channels;

        [QS.Fx.Base.Inspectable]
        private Dictionary<string, QS.Fx.Value.STUNAddress> addrmapping;


        private class PunchTask
        {
            public enum State
            {
                ACTIVE,
                ACTIVESENT,
                PASSIVE,
                PASSIVESENT,
                ACTIVERCVD
            }

            public PunchTask(State state, QS.Fx.Value.STUNAddress remoteaddr)
            {
                this.state = state;
                this.remoteaddr = remoteaddr;
                this.datetime = System.DateTime.Now;
            }

            public State state;
            public System.DateTime datetime;
            public QS.Fx.Value.STUNAddress remoteaddr;
        }

        [QS.Fx.Base.Inspectable]
        private int punchingTimeout;

        [QS.Fx.Base.Inspectable]
        private Dictionary<string, PunchTask> punchingDict;

        [QS.Fx.Base.Inspectable]
        private System.Timers.Timer punchingTimer;

        [QS.Fx.Base.Inspectable]
        private System.Timers.Timer aliveTimer;

        #endregion

        #region Access

        public Socket TransportSocket
        {
            get { return this._transport_socket; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNTransport Endpoint Connect");
#endif

            // In case the host is behind the firewall
            // or there is no specification of STUNServer
            // It returns a STUNAddress with private address as its public address
            this._local_stun_addr = new QS.Fx.Value.STUNAddress(this._addr + "/" + this._addr);
            this._stuntransport_endpt.Interface.Address(this._local_stun_addr);

#if VERBOSE
            //if (this._logger != null)
            //    this._logger.Log("Component_.STUNTransport No STUN Server, Report Address As " + this._local_stun_addr.String);
#endif

            if (this._server_addr != null)
            {
                // Start checking the mapped public address
                this._local_stun_addr = new QS.Fx.Value.STUNAddress(this._ipaddr, this._port, this._ipaddr, this._port);
                QS.Fx.Value.BounceMessage message = new QS.Fx.Value.BounceMessage(
                    QS.Fx.Value.BounceMessage.Type.TEST, this._local_stun_addr);

                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), this._server_addr, message));
            }
        }

        #endregion

        #region _Disconnect

        private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNTransport Endpoint Disconnect");
#endif
            Disconnect();
        }

        private void Disconnect()
        {
            lock (this._transport_socket)
            {
                this._transport_socket.Close();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Bind

        private void Bind()
        {
            QS._qss_c_.Base1_.Subnet _subnet;

            int _i = _addr.IndexOf(':');
            if (_i >= 0)
            {
                _subnet = new QS._qss_c_.Base1_.Subnet(_addr.Substring(0, _i));
                this._port = Convert.ToInt32(_addr.Substring(_i + 1));
            }
            else
            {
                _subnet = new QS._qss_c_.Base1_.Subnet(_addr);
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

            this._addr = this._ipaddr + ":" + this._port;

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
                throw new Exception("Could not setup the UDP transport socket.");
        }

        #endregion

        #region Incoming

        private void Incoming()
        {
            // Set to receive from all addresses
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempRemoteEP = (EndPoint)sender;

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
                    catch (SocketException exc)
                    {
                        if (exc.ErrorCode == 10054)
                        {
                            // continue
                            Incoming();
                        }
                        throw new Exception("EndReceiveFrom Exception. " + exc.Message);
                    }
                    catch(Exception exc1)
                    {
                        throw new Exception("EndReceiveFrom Exception. " + exc1.Message);
                    }
                }
                if (_nreceived > 0)
                {
#if INFO
                    if (this._logger != null)
                        this._logger.Log("STUNTransport received msg from " + _remep.ToString());
#endif

#if VERBOSE
                    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    if (this._logger != null)
                        this._logger.Log(cur + "STUNTransport received msg");
#endif
                    // Succeed received
                    IPEndPoint _sender = (IPEndPoint)_remep;

                    // Deserialize packet and check
                    MemoryStream memorystream = new MemoryStream(this.incomingpacket);

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

                    this._Enqueue(
                        new QS._qss_x_.Properties_.Base_.Event_<IPEndPoint, QS.Fx.Serialization.ISerializable>(
                            new QS._qss_x_.Properties_.Base_.EventCallback_(this.MessageProcess), _sender, message));

                    Incoming();
                }
                else
                {
                    throw new Exception("Receive data number.\r\n" + _nreceived);
                }
            }
            catch (Exception exc)
            {
                this.Disconnect();
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

        #region MessageProcess

        private void MessageProcess(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            lock (incomingMessageQueue)
            {
                incomingMessageQueue.Enqueue(_event);
                while (incomingMessageQueue.Count > 0)
                {
                    MessageProcess();
                }
            }
        }

        private void MessageProcess()
        {

            QS._qss_x_.Properties_.Base_.IEvent_<IPEndPoint, QS.Fx.Serialization.ISerializable> _event_
                = (QS._qss_x_.Properties_.Base_.IEvent_<IPEndPoint, QS.Fx.Serialization.ISerializable>)(incomingMessageQueue.Dequeue());

            IPEndPoint remotesender = _event_._Object1;
            QS.Fx.Serialization.ISerializable message = _event_._Object2;

            if (message.SerializableInfo.ClassID == (ushort)QS.ClassID.BounceMessage)
            {
                STUNProcess((QS.Fx.Value.BounceMessage)message, remotesender.ToString());
            }
            else
            {
                //ChannelIncoming
                lock (_channels)
                {
                    QS.Fx.Value.STUNAddress stunaddr;
                    if (!addrmapping.TryGetValue(remotesender.ToString(), out stunaddr))
                    {
                        stunaddr = new QS.Fx.Value.STUNAddress(remotesender.Address, remotesender.Port,
                            remotesender.Address, remotesender.Port);
                        addrmapping.Add(remotesender.ToString(), stunaddr);
                    }

                    UdpCommunicationChannel channel;
                    if (!_channels.TryGetValue(stunaddr.String, out channel))
                    {
                        channel = new UdpCommunicationChannel(this, remotesender.ToString());

                        _channels.Add(stunaddr.String, channel);

                        // Give the new channel to transport client
                        QS.Fx.Object.IReference<
                            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>
                            > refer = QS._qss_x_.Object_.Reference<
                            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                        channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));
                        this._stuntransport_endpt.Interface.Connected(stunaddr, refer);
                    }

                    channel._Incoming((MessageClass)message);
                }
            }
        }

        #endregion

        #region STUNProcess

        private void STUNProcess(QS.Fx.Value.BounceMessage message, string remoteaddr)
        {
            switch (message.TYPE)
            {
                case QS.Fx.Value.BounceMessage.Type.BOUNCE:
                    {
                        this._local_stun_addr = message.Addr;
                        this._stuntransport_endpt.Interface.Address(this._local_stun_addr);

                        this.StartAlive();

                    }
                    break;
                case QS.Fx.Value.BounceMessage.Type.ACTIVE:
                    {
                        if (this.punchingTimer == null)
                        {
                            punchingTimer = new System.Timers.Timer();
                            punchingTimer.Elapsed += new ElapsedEventHandler(PunchingHandler);
                            punchingTimer.Interval = 500;
                            punchingTimer.AutoReset = true;
                            punchingTimer.Start();
                        }

                        PunchTask pt;
                        lock (punchingDict)
                        {
                            if (!punchingDict.TryGetValue(message.Addr.String, out pt))
                            {
                                pt = new PunchTask(PunchTask.State.PASSIVE, message.Addr);
                                punchingDict.Add(message.Addr.String, pt);
                            }
                            else
                            {
                                // If two nodes simultaneously actively punch, 
                                // keep the first one, or let the bigger address client play active role
                                if (pt.state == PunchTask.State.ACTIVE
                                    || (pt.state == PunchTask.State.ACTIVESENT
                                    && ((IComparable)pt.remoteaddr).CompareTo(this._local_stun_addr) > 0))
                                {
                                    pt.state = PunchTask.State.PASSIVE;
                                }
                            }
                        }
                    }
                    break;
                case QS.Fx.Value.BounceMessage.Type.PASSIVE:
                    {
                        PunchTask pt;
                        lock (punchingDict)
                        {
                            if (!punchingDict.TryGetValue(message.Addr.String, out pt))
                            {
                                //throw new Exception("No corresponded active punch task for passive msg! \r\n");
                            }
                            else
                            {
                                if (pt.state == PunchTask.State.ACTIVESENT)
                                {
                                    pt.state = PunchTask.State.ACTIVERCVD;
                                }
                            }
                        }
                    }
                    break;
                case QS.Fx.Value.BounceMessage.Type.PUNCH:
                    {
                        PunchTask pt;

                        lock (punchingDict)
                        {
                            if (!punchingDict.TryGetValue(message.Addr.String, out pt))
                            {
                                //throw new Exception("No corresponded punch task for punch msg! \r\n");
                            }
                            else
                            {
                                if (pt.state == PunchTask.State.ACTIVERCVD
                                    || pt.state == PunchTask.State.PASSIVESENT)
                                {
                                    //Punchback
                                    QS.Fx.Value.BounceMessage backmsg = new QS.Fx.Value.BounceMessage(QS.Fx.Value.BounceMessage.Type.PUNCH, this._local_stun_addr, pt.remoteaddr);
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), remoteaddr, backmsg));

                                    //Punch success
                                    UdpCommunicationChannel channel = new UdpCommunicationChannel(this, remoteaddr);
                                    lock (_channels)
                                    {
                                        _channels.Add(pt.remoteaddr.String, channel);
                                        addrmapping.Add(remoteaddr, pt.remoteaddr);
                                    }

                                    punchingDict.Remove(message.Addr.String);

                                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> refer
                                        = QS._qss_x_.Object_.Reference<
                                                QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                                                    channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));

                                    try
                                    {
                                        this._stuntransport_endpt.Interface.Connected(message.Addr, refer);
                                    }
                                    catch (Exception exc)
                                    {

                                    }

#if VERBOSE
                                    if (this._logger != null)
                                        this._logger.Log("Component_.STUNTransport punch task success, target: " + pt.remoteaddr.String + " from: " + remoteaddr);
#endif
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Unkonw type of BounceMessage in STUNTransport! ");
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region StartAlive

        private void StartAlive()
        {
            if (aliveTimer == null)
            {
                aliveTimer = new System.Timers.Timer();
                aliveTimer.Elapsed += new ElapsedEventHandler(StartAlive);
                aliveTimer.Interval = 5000;
                aliveTimer.AutoReset = true;
                aliveTimer.Start();
            }
        }

        private void StartAlive(object source, ElapsedEventArgs e)
        {
            try
            {
                QS.Fx.Value.BounceMessage message = new QS.Fx.Value.BounceMessage(
                QS.Fx.Value.BounceMessage.Type.ALIVE, this._local_stun_addr);

                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), this._server_addr, message));
            }
            catch (Exception exc)
            {
                throw new Exception("STUNTransport StartAlive " + exc.Message);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Outgoing

        private void Outgoing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            lock (outgoingMessageQueue)
            {
                outgoingMessageQueue.Enqueue(_event);
                while (outgoingMessageQueue.Count > 0)
                {
                    Outgoing();
                }
            }
        }

        unsafe private void Outgoing()
        {
            QS._qss_x_.Properties_.Base_.IEvent_<string, QS.Fx.Serialization.ISerializable> _event_
                = (QS._qss_x_.Properties_.Base_.IEvent_<string, QS.Fx.Serialization.ISerializable>)(outgoingMessageQueue.Dequeue());

            string remoteaddr = _event_._Object1;
            QS.Fx.Serialization.ISerializable message = _event_._Object2;

            string[] dest = remoteaddr.Split(':');
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(dest[0]), int.Parse(dest[1]));
            EndPoint remoteep = (EndPoint)ipep;

            try
            {
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

                lock (this._transport_socket)
                {
                    try
                    {
                        _transport_socket.BeginSendTo(_packetdata, 0, length, SocketFlags.None, remoteep, new AsyncCallback(this._SendCallback), null);
                    }
                    catch (SocketException socketc)
                    {
                        this.Disconnect();
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
                    this.Disconnect();
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
        }

        #endregion

        #region _SendCallback

        private void _SendCallback(IAsyncResult result)
        {
            try
            {
                int _ntransmitted;
                lock (this._transport_socket)
                {
                    _ntransmitted = this._transport_socket.EndSendTo(result);
                }

                // Need this?
                if (_ntransmitted < 0)
                {
                    throw new Exception("Socket error!");
                }

#if INFO
                if (this._logger != null)
                    this._logger.Log("STUNTransport send callback sent msg!");
#endif

            }
            catch (SocketException sockexc)
            {
                this.Disconnect();
                throw new Exception("Socket exception, close and code.\n\r" + sockexc.ErrorCode);
            }
            catch (Exception exc)
            {
                this.Disconnect();
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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Connect

        private void Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.STUNAddress> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.STUNAddress>)_event;

            QS.Fx.Value.STUNAddress addr = _event_._Object;

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> refer;
            UdpCommunicationChannel _channel;

            lock (this._channels)
            {
                if (!this._channels.TryGetValue(addr.String, out _channel))
                {
                    if (!addr.IsNAT())
                    {
                        _channel = new UdpCommunicationChannel(this, addr.PubAddr);
                        this._channels.Add(addr.String, _channel);
                        this.addrmapping[addr.PubAddr] = addr;
                    }
                    else
                    {
                        _channel = null;
                    }
                }
            }

            if (_channel != null)
            {
                refer = QS._qss_x_.Object_.Reference<
                                                QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                                                    _channel, null, QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>)));

                try
                {
                    this._stuntransport_endpt.Interface.Connected(addr, refer);
                }
                catch (Exception exc)
                {

                }
            }
            else if (addr.IsNAT())
            {
                if (this._server_addr != null)
                    Punch(addr);
                else
                {
#if VERBOSE
                    if (this._logger != null)
                    {
                        this._logger.Log("Component_.STUNTransport Give Up NAT-Traverse Try To: " + addr.String);
                    }
#endif
                }
            }
            else
            {
                throw new Exception("NO NAT address can not create channel! \r\n");
            }
        }

        #endregion

        #region Punch

        private void Punch(QS.Fx.Value.STUNAddress address)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNTransport actively punch to " + address.String);
#endif

            if (this.punchingTimer == null)
            {
                punchingTimer = new System.Timers.Timer();
                punchingTimer.Elapsed += new ElapsedEventHandler(PunchingHandler);
                punchingTimer.Interval = 500;
                punchingTimer.AutoReset = true;
                punchingTimer.Start();
            }

            lock (this.punchingDict)
            {
                PunchTask pt;
                if (!this.punchingDict.TryGetValue(address.String, out pt))
                {
                    pt = new PunchTask(PunchTask.State.ACTIVE, address);
                    this.punchingDict.Add(address.String, pt);
                }
            }
        }

        #endregion

        #region PunchingHandler

        private void PunchingHandler(object source, ElapsedEventArgs e)
        {
            Queue<string> tempQueue = new Queue<string>();

            lock (punchingDict)
            {
                foreach (KeyValuePair<string, PunchTask> kvp in punchingDict)
                {
                    PunchTask pt = kvp.Value;
                    // 10 sec punch timeout
                    System.DateTime now = System.DateTime.Now;
                    System.TimeSpan span = now - pt.datetime;
                    if ((span.TotalSeconds) > this.punchingTimeout)
                    {
                        tempQueue.Enqueue(kvp.Key);
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.STUNTransport punch task timeout, target: " + pt.remoteaddr.String);
#endif
                    }
                    else
                    {
                        switch (pt.state)
                        {
                            case PunchTask.State.ACTIVE:
                                {
                                    //Send active BounceMsg to STUN Server
                                    QS.Fx.Value.BounceMessage message = new QS.Fx.Value.BounceMessage(QS.Fx.Value.BounceMessage.Type.ACTIVE, this._local_stun_addr, pt.remoteaddr);

                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), this._server_addr, message));

                                    pt.state = PunchTask.State.ACTIVESENT;

                                }
                                break;
                            case PunchTask.State.ACTIVERCVD:
                                {
                                    //Send punch BounceMsg to Remote Client
                                    QS.Fx.Value.BounceMessage message = new QS.Fx.Value.BounceMessage(QS.Fx.Value.BounceMessage.Type.PUNCH, this._local_stun_addr, pt.remoteaddr);

                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), pt.remoteaddr.PubAddr, message));

#if VERBOSE
                                    if (this._logger != null)
                                        this._logger.Log("Component_.STUNTransport active punch task receive passive feedback, try punching: " + pt.remoteaddr.String);
#endif
                                    if ((pt.remoteaddr.PubAddr.Split(':'))[0] == (this._local_stun_addr.PubAddr.Split(':'))[0])
                                    {
                                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), pt.remoteaddr.PriAddr, message));

#if VERBOSE
                                        if (this._logger != null)
                                            this._logger.Log("Component_.STUNTransport active punch also try punching private address");
#endif
                                    }

                                    //Keep this state for continiously sendking punch messages, until punch success or timeout
                                }
                                break;
                            case PunchTask.State.PASSIVE:
                                {
                                    //If remote is a NAT client
                                    //if (pt.remoteaddr.IsNAT())
                                    {
                                        //Send passive BounceMsg to STUN Server
                                        QS.Fx.Value.BounceMessage message = new QS.Fx.Value.BounceMessage(QS.Fx.Value.BounceMessage.Type.PASSIVE, this._local_stun_addr, pt.remoteaddr);

                                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                            new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), this._server_addr, message));

                                        pt.state = PunchTask.State.PASSIVESENT;
                                    }
                                    //else
                                    //{

                                    //}
                                }
                                break;
                            case PunchTask.State.PASSIVESENT:
                                {
                                    //Send punch BounceMsg to Remote Client
                                    QS.Fx.Value.BounceMessage message = new QS.Fx.Value.BounceMessage(QS.Fx.Value.BounceMessage.Type.PUNCH, this._local_stun_addr, pt.remoteaddr);

                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), pt.remoteaddr.PubAddr, message));

                                    //Keep this state for continiously sendking punch messages, until punch success or timeout
#if VERBOSE
                                    if (this._logger != null)
                                        this._logger.Log("Component_.STUNTransport passive punch task has sent feedback, try punching " + pt.remoteaddr.String);
#endif
                                    if ((pt.remoteaddr.PubAddr.Split(':'))[0] == (this._local_stun_addr.PubAddr.Split(':'))[0])
                                    {
                                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string, QS.Fx.Serialization.ISerializable>(
                                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.Outgoing), pt.remoteaddr.PriAddr, message));

#if VERBOSE
                                        if (this._logger != null)
                                            this._logger.Log("Component_.STUNTransport passive punch also try punching private address");
#endif
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }//end of else
                }

                // Remove timeout punck task
                while (tempQueue.Count > 0)
                {
                    punchingDict.Remove(tempQueue.Dequeue());
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransport<STUNAddress,MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>, QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>> QS.Fx.Object.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>.Transport
        {
            get { return this._stuntransport_endpt; }
        }

        #endregion

        #region ITransport<STUNAddress,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>.Connect(QS.Fx.Value.STUNAddress address)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.STUNAddress>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.Connect), address));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class UdpCommunicationChannel

        public class UdpCommunicationChannel :
            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>,
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>
        {
            #region Constructor

            public UdpCommunicationChannel(STUNTransport_<MessageClass> _transport, string _address)
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
            private STUNTransport_<MessageClass> _transport;

            private IPAddress _ipaddr;
            private int _port;
            private EndPoint _remoteep;
            private Socket _socket;

            private Queue<QS.Fx.Serialization.ISerializable> incoming = new Queue<QS.Fx.Serialization.ISerializable>();
            private Queue<QS.Fx.Serialization.ISerializable> outgoing = new Queue<QS.Fx.Serialization.ISerializable>();

            private bool _pending_send;

            #endregion

            #region _Outgoing

            public void _Outgoing(MessageClass message)
            {
                lock (this.outgoing)
                {
                    _pending_send = this.outgoing.Count > 0;
                    try
                    {
                        this.outgoing.Enqueue(message);

                        //this._SerializeAndSend();
                        
                    }
                    catch (Exception exc)
                    {

                    }
                    if (!_pending_send)
                    {
                        this._transport._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.ScheduleSend)));
                    }
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
                _Outgoing(message);
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _SendCallback

            private void _SendCallback(IAsyncResult result)
            {
                try
                {
                    //lock (this.outgoing)
                    {
                        SocketError _errorcode;
                        int _ntransmitted;
                        lock (this._socket)
                        {
                            _ntransmitted = this._socket.EndSendTo(result);
                        }

#if VERBOSE
                        double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        if (this._transport._logger != null)
                            this._transport._logger.Log(cur + "STUNTransport sent msg");
#endif

                        // Need this?
                        if (_ntransmitted < 0)
                        {
                            throw new Exception("Socket error!");
                        }

#if INFO
                        if (_transport._logger != null)
                            this._transport._logger.Log("STUNTransport UdpChannel send callback sent msg!");
#endif

                        //if (outgoing.Count > 0)
                        //{
                        //    _SerializeAndSend();
                        //}
                    }
                }
                catch (SocketException sockexc)
                {
                    if (sockexc.ErrorCode != 10054)
                    {
                        _DisconnectCallback();
                        throw new Exception("Socket exception, close and code.\n\r" + sockexc.ErrorCode);
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

            #region _ScheduleSend

            private void ScheduleSend(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
                lock (outgoing)
                {
                    while (outgoing.Count > 0)
                    {
                        _SerializeAndSend();

                        Thread.Sleep(100);
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
        }

        #endregion

    }
}
