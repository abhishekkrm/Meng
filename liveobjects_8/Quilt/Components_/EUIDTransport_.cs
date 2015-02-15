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
using Quilt;
using Quilt.HostDetector;
using Quilt.HostDetector.NATCheck;
using QS.Fx.Value;

namespace QS._qss_x_.Properties_
{
    [QS.Fx.Reflection.ComponentClass("79FEB8EB499F4261ADA93A031FDE8E17", "EUIDTransport")]
    public sealed class EUIDTransport_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>,
        QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {

        #region Construct

        public EUIDTransport_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Subnet, formatted as X.Y.Z.W/V", QS.Fx.Reflection.ParameterClass.Value)]
            string _self_subnet,
            [QS.Fx.Reflection.Parameter("Preferred Protocol, TCP or UDP, TCP by default", QS.Fx.Reflection.ParameterClass.Value)]
            string _preferred_protocol,
            [QS.Fx.Reflection.Parameter("TCP Port", QS.Fx.Reflection.ParameterClass.Value)]
            string _tcp_port,
            [QS.Fx.Reflection.Parameter("UDP Port", QS.Fx.Reflection.ParameterClass.Value)]
            string _udp_port,
            [QS.Fx.Reflection.Parameter("UDP Packet Size Limit, 4096 by default", QS.Fx.Reflection.ParameterClass.Value)] 
            string _pack_size,
            [QS.Fx.Reflection.Parameter("STUN Server Address, if you want support for NAT traverse", QS.Fx.Reflection.ParameterClass.Value)]
            string _server_addr,
            [QS.Fx.Reflection.Parameter("Traverse Timout Seconds, 60 By Default", QS.Fx.Reflection.ParameterClass.Value)]
            int _timeout,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug)
            : base(_mycontext, _debug)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MultiTransport Constructor");
#endif


            this._mycontext = _mycontext;

            this._euid_transport_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.EUIDAddress, MessageClass>,
                QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>>(this);

            // Check the availability of specified subnet
            this._subnet = new QS._qss_c_.Base1_.Subnet(_self_subnet);
            this._hostname = _platform.Network.GetHostName();
            this._networkinterface = null;
            foreach (QS.Fx.Network.INetworkInterface _nic in _platform.Network.Interfaces)
            {
                if (_nic.InterfaceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (this._subnet.contains(_nic.InterfaceAddress))
                    {
                        this._networkinterface = _nic;
                        this._local_addr = _networkinterface.InterfaceAddress.ToString();
                        break;
                    }
                }
            }
            if (this._networkinterface == null)
                throw new Exception("Could not locate any network adapter on the requested subnet " + this._subnet.ToString() + ".");

            if (_preferred_protocol == null || _preferred_protocol == "")
            {
                this._preferred_protocol = "UDP";
            }
            else
            {
                this._preferred_protocol = _preferred_protocol;
            }

            Random r = new Random();
            this._tcp_port = _tcp_port == " " ? 20000 + r.Next(20000) : int.Parse(_tcp_port);
            this._udp_port = _udp_port == " " ? 20000 + r.Next(20000) : int.Parse(_udp_port);

            this._timeout = _timeout;
            this._stun_server = _server_addr ;
            if (_pack_size == null || _pack_size == "")
            {
                this._udp_packet_size = "4096";
            }
            else
            {
                this._udp_packet_size = _pack_size;
            }

            // To be connected or disconnected as EUIDTransport
            // Set callback
            this._euid_transport_endpt.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
            this._euid_transport_endpt.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });

            _addr_map = new Dictionary<string, QS.Fx.Value.EUIDAddress>();
        }

        #endregion

        #region Fields for HostDetector

        [QS.Fx.Base.Inspectable]
        private HostDetector _detector;

        [QS.Fx.Base.Inspectable]
        private DetectResult _detect_result;

        #endregion

        #region Fields as EUIDTransport

        [QS.Fx.Base.Inspectable]
        private string _hostname;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet _subnet;
        [QS.Fx.Base.Inspectable]
        private int _tcp_port;
        [QS.Fx.Base.Inspectable]
        private int _udp_port;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.INetworkInterface _networkinterface;
        [QS.Fx.Base.Inspectable]
        private string _local_addr;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.EUIDAddress, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>> _euid_transport_endpt;

        [QS.Fx.Base.Inspectable]
        private string _preferred_protocol;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.EUIDAddress _euid_addr;

        // Map EUIDAddress connecting to the real STUNAddress or NetworkAddress
        // TODO: timeout maintenance
        [QS.Fx.Base.Inspectable]
        private Dictionary<string, QS.Fx.Value.EUIDAddress> _addr_map;

        #endregion

        #region Fields as STUNTransport client

        // As client for STUNTransport
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>> _stun_transport_endpt;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _stun_transport_conn;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.STUNAddress _stun_addr;

        [QS.Fx.Base.Inspectable]
        private string _stun_server;

        [QS.Fx.Base.Inspectable]
        private int _timeout;

        [QS.Fx.Base.Inspectable]
        private string _udp_packet_size;

        [QS.Fx.Base.Inspectable]
        private STUNTransport_<MessageClass> _stun_transport;

        #endregion

        #region Fields as TcpTransport2 client

        // As client for TcpTransport2
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>> _tcp_transport_endpt;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _tcp_transport_conn;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Address _tcp_addr;

        [QS.Fx.Base.Inspectable]
        private Component_.TcpTransport2_<MessageClass> _tcp_transport;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect for EUIDTransport

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MultiTransport Endpoint Connect");
#endif

            // Once EUIDTransport is connected, it should fetch the Detected information from Detecter Service
            // Create instance of HostDetector
            HostDetector.Callback host_detector_callback = new HostDetector.Callback(HostDetectorCallback);

            bool isexpeirment;
            if (_local_addr.Contains("10.1.") || _local_addr.Contains("10.0."))
            {
                // Emulab expeirment
                isexpeirment = true;
            }
            else
            {
                isexpeirment = false;
            }

            this._detector = new HostDetector(host_detector_callback, isexpeirment);
            // Start detect
            this._detector.Start(_local_addr);
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.EUIDTransport Endpoint Connect, check: " + _local_addr + " " + isexpeirment.ToString());
#endif
        }

        #endregion

        #region _Disconnect for EUIDTransport

        private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MultiTransport Endpoint Disconnect");
#endif

            // Disconnect STUNTransport
            _stun_transport_endpt.Disconnect();

            // Disconnect TcpTransport2
            _tcp_transport_endpt.Disconnect();

            // Clear local resource
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Connect

        private void Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.EUIDAddress> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.EUIDAddress>)_event;

            EUIDAddress addr = _event_._Object;
            EUIDAddress.ProtocolInfo local_tcp_info = _euid_addr.GetProtocolInfo("TCP");
            EUIDAddress.ProtocolInfo tcp_info = addr.GetProtocolInfo("TCP");
            EUIDAddress.ProtocolInfo stun_info = addr.GetProtocolInfo("UDP");
            string remote_preference = addr.GetPreferred();

            if (remote_preference == "UDP")
            {
                // UDP preferred

                // Check UDP
                // If the remote host can be connected from Internet and local host can connect out
                if (
                    (_euid_addr.GetProtocolInfo("UDP").proto_direct != Direction.DIRECTION.UNKNOWN
                    && stun_info.proto_direct <= Direction.DIRECTION.BIDIRECTION_TRAVERSE)
                    || (new QS.Fx.Value.STUNAddress(_euid_addr.GetProtocolInfo("UDP").proto_addr).PubAddr.Split(':')[0]
                    == new QS.Fx.Value.STUNAddress(stun_info.proto_addr).PubAddr.Split(':')[0])
                    )
                {
                    _stun_transport_endpt.Interface.Connect(new QS.Fx.Value.STUNAddress(stun_info.proto_addr));
                    _addr_map[stun_info.proto_addr] = addr;
                }

                // Check UDP
                // If the remote host can possibly be accessed fron Internet and local host can send out
                // Or the remote and local hosts are behind the same NAT
                else if (
                    (local_tcp_info.proto_direct != Direction.DIRECTION.UNKNOWN
                    && tcp_info.proto_direct == Direction.DIRECTION.BIDIRECTION)
                    || (local_tcp_info.proto_addr.Split(':')[0]
                    == tcp_info.proto_addr.Split(':')[0])
                    )
                {
                    _tcp_transport_endpt.Interface.Connect(new QS.Fx.Base.Address(tcp_info.proto_addr));
                    _addr_map[tcp_info.proto_addr] = addr;
                }

                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.ConnOptTransport skip to connect remote host: " + addr.String);
#endif
                }
            }
            else
            {
                // TCP preferred

                // Check TCP
                // If the remote host can be connected from Internet and local host can connect out
                if (
                    (local_tcp_info.proto_direct != Direction.DIRECTION.UNKNOWN
                    && tcp_info.proto_direct == Direction.DIRECTION.BIDIRECTION)
                    || (local_tcp_info.proto_addr.Split(':')[0]
                    == tcp_info.proto_addr.Split(':')[0]))
                {
                    _tcp_transport_endpt.Interface.Connect(new QS.Fx.Base.Address(tcp_info.proto_addr));
                    _addr_map[tcp_info.proto_addr] = addr;
                }

                // Check UDP
                // If the remote host can possibly be accessed fron Internet and local host can send out
                // Or the remote and local hosts are behind the same NAT
                else if (
                    (_euid_addr.GetProtocolInfo("UDP").proto_direct != Direction.DIRECTION.UNKNOWN
                    && stun_info.proto_direct <= Direction.DIRECTION.BIDIRECTION_TRAVERSE)
                    || (new QS.Fx.Value.STUNAddress(_euid_addr.GetProtocolInfo("UDP").proto_addr).PubAddr.Split(':')[0]
                    == new QS.Fx.Value.STUNAddress(stun_info.proto_addr).PubAddr.Split(':')[0])
                    )
                {
                    _stun_transport_endpt.Interface.Connect(new QS.Fx.Value.STUNAddress(stun_info.proto_addr));
                    _addr_map[stun_info.proto_addr] = addr;
                }

                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.ConnOptTransport skip to connect remote host: " + addr.String);
#endif
                }
            }


            
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransport<EUIDAddress,MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.EUIDAddress, MessageClass>, QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>> QS.Fx.Object.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>.Transport
        {
            get { return this._euid_transport_endpt; }
        }

        #endregion

        #region ITransport<EUIDAddress,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.EUIDAddress, MessageClass>.Connect(QS.Fx.Value.EUIDAddress address)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.EUIDAddress>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.Connect), address));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransportClient<STUNAddress,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>.Address(QS.Fx.Value.STUNAddress address)
        {
            if (address.IsNAT() || address.PubAddr.Contains(_detect_result.publicAddress.ToString()))
            {
                // Detected, or there is no NAT
                _stun_addr = address;
            }
            else
            {
                // It is possible the STUNTransport just cannot detect it
                if (_detect_result.natType == NATTYPE.UDP_BLOCKED)
                {
                    _stun_addr = new QS.Fx.Value.STUNAddress(_detect_result.publicAddress.ToString() + ":0/" + address.PriAddr);
                }
                else
                {
                    // Just a default return of PriAddr/PriAddr before detecion
                    
#if VERBOSE
                    // For experiment
                    _stun_addr = new STUNAddress(address.PriAddr + "/" + address.PriAddr);
#else
                    return;
#endif

                }
            }

            MakeEUID();

            _euid_transport_endpt.Interface.Address(_euid_addr);
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>.Connected(QS.Fx.Value.STUNAddress address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> channel)
        {
            QS.Fx.Value.EUIDAddress rem_addr;
            if (_addr_map.TryGetValue(address.String, out rem_addr))
            {
                _euid_transport_endpt.Interface.Connected(rem_addr, channel);

                //_addr_map.Remove(address.String);
            }
            else
            {
                // Possibly this is new incoming connection, withou knowing real EUID
                EUIDAddress incoming_euid = new EUIDAddress("UDP://UNKNOWN|" + address.String);
                _euid_transport_endpt.Interface.Connected(incoming_euid, channel);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransportClient<NetworkAddress,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>.Address(QS.Fx.Base.Address address)
        {
            this._tcp_addr = address;

#if VERBOSE
            if (this._logger != null)
            {
                this._logger.Log("Component_.EUIDTransport TCPTransport2_ initialized on: " + this._tcp_addr);
                this._logger.Log("Component_.EUIDTransport start to initialize STUNTransport");
            }
#endif

            // Initialize STUNTransport to connect
            QS.Fx.Object.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>
                stun_transport = new QS._qss_x_.Properties_.STUNTransport_<MessageClass>(_mycontext, _stun_server, _local_addr + ":" + _udp_port, _timeout, _udp_packet_size, _debug);

            this._stun_transport = (STUNTransport_<MessageClass>)stun_transport;

            _stun_transport_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, MessageClass>,
                QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, MessageClass>>(this);

            _stun_transport_conn = _stun_transport_endpt.Connect(stun_transport.Transport);
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> channel)
        {
            QS.Fx.Value.EUIDAddress rem_addr;
            if (_addr_map.TryGetValue(address.ToString(), out rem_addr))
            {
                _euid_transport_endpt.Interface.Connected(rem_addr, channel);

                _addr_map.Remove(address.ToString());
            }
            else
            {
                // Possibly this is new incoming connection, withou knowing real EUID
                EUIDAddress incoming_euid = new EUIDAddress("TCP://UNKNOWN|" + address.String);
                _euid_transport_endpt.Interface.Connected(incoming_euid, channel);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MakeEUID

        private void MakeEUID()
        {
            // Merge _tcp_addr, _stun_addr and _detect_result to _euid_addr
            // EUID Address consists three parts
            // 1) Connectivity Options
            // "[Protocol Name]://[Connectivity Direction]|[Protocol Address];"
            // Each protocol (TCP/UDP) can have one address
            // UDP uses STUNAddress for NAT traverse support
            // Multiple addresses are separated by semicolon
            // 2) Router Stack
            // [Range]|[IPMC Range]|[Router 1],[Router 2],[Router 3]...;
            // 3) Performance
            // TODO
            string tcp_opt = "TCP://" + _detect_result.tcpResult.ToString() + "|" + _tcp_addr.ToString();
            string udp_opt = "UDP://" + _detect_result.udpResult.ToString() + "|" + _stun_addr.String;

#if VERBOSE
            // For experiment on Deter, set to accessable
            tcp_opt = "TCP://" + Direction.DIRECTION.BIDIRECTION + "|" + _tcp_addr.ToString();
            udp_opt = "UDP://" + Direction.DIRECTION.BIDIRECTION + "|" + _stun_addr.String;
#endif

            string router_stack = _detect_result.route.Count.ToString() + "|" + _detect_result.index + "|";
            foreach (string router in _detect_result.route)
            {
                router_stack += router + ",";
            }
            router_stack = router_stack.TrimEnd(',');
            
            // Performance TODO

            _euid_addr = new QS.Fx.Value.EUIDAddress(tcp_opt + ";" + udp_opt + ";" + router_stack + ";" + _preferred_protocol);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region EUIDDetected

        void EUIDDetected(string id, DetectResult result)
        {
            if (this._local_addr == id)
            {
                // Set the detect result associated with specified NIC address
                this._detect_result = result;
            }

            if (this._detect_result == null)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.EUIDTransport EUID fetch failure for NIC: " + this._local_addr);
#endif
            }
            else
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.EUIDTransport starts to initialize TCP Transport components.");
#endif
                // Initialize TcpTransport2_ to connect
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, MessageClass>
                    tcp_transport2 = new QS._qss_x_.Properties_.Component_.TcpTransport2_<MessageClass>(_mycontext, _local_addr + "/24:" + _tcp_port, _debug);

                this._tcp_transport = (QS._qss_x_.Properties_.Component_.TcpTransport2_<MessageClass>)tcp_transport2;

                _tcp_transport_endpt = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
                    QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>>(this);

                _tcp_transport_conn = _tcp_transport_endpt.Connect(tcp_transport2.Transport);
                
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Callback for HostDetector

        private void HostDetectorCallback(string nic_addr, DetectResult result)
        {
            if (nic_addr == _local_addr)
            {
                EUIDDetected(_local_addr, result);
            }
        }

        #endregion
    }

}