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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.UnreliableTransport, "Properties Framework Unreliable Transport")]
    public sealed class UnreliableTransport_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.Properties_.Component_.Transport_<
            MessageClass, QS.Fx.Network.NetworkAddress, UnreliableTransport_<MessageClass>, UnreliableTransport_<MessageClass>.CommunicationChannel_>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public UnreliableTransport_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)]
            string _address,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
        : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableTransport_.Constructor");
#endif

            int _i = _address.IndexOf(':');
            if ((_i >= 0) && (_i < _address.Length))
            {
                this._port = Convert.ToInt32(_address.Substring(_i + 1));
            }
            else
            {
                this._port = 0;
                _i = _address.Length;
            }
            List<QS._qss_c_.Base1_.Subnet> _subnets = new List<QS._qss_c_.Base1_.Subnet>();
            int _j = 0;
            while (_j < _i)
            {
                int _k = _address.IndexOf(',', _j);
                if ((_k >= 0) && (_k < _i))
                {
                    _subnets.Add(new QS._qss_c_.Base1_.Subnet(_address.Substring(_j, _k - _j)));
                    _j = _k + 1;
                }
                else
                    break;
            }
            _subnets.Add(new QS._qss_c_.Base1_.Subnet(_address.Substring(_j, _i - _j)));
            this._subnets = _subnets.ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _hostname;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet[] _subnets;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet _subnet;
        [QS.Fx.Base.Inspectable]
        private int _port;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.INetworkInterface _networkinterface;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.IListener _listener;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.NetworkAddress _networkaddress;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableTransport_._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableTransport_._Dispose");
#endif

            lock (this)
            {
                if ((this._listener != null) && (this._listener is IDisposable))
                {
                    this._listener.Dispose();
                }

                base._Dispose();
            }
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableTransport_._Start");
#endif

            lock (this)
            {
                base._Start();

                this._hostname = _platform.Network.GetHostName();
                this._networkinterface = null;
                foreach (QS._qss_c_.Base1_.Subnet _subnet in this._subnets)
                {
                    foreach (QS.Fx.Network.INetworkInterface _nic in _platform.Network.Interfaces)
                    {
                        if (_nic.InterfaceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (_subnet.contains(_nic.InterfaceAddress))
                            {
                                this._subnet = _subnet;
                                this._networkinterface = _nic;
                                break;
                            }
                        }
                    }
                    if (this._networkinterface != null)
                        break;
                }
                if (this._networkinterface == null)
                    _mycontext.Error("Could not locate any network adapter on any of the subnets.");
                this._listener =
                    this._networkinterface.Listen(
                        new QS.Fx.Network.NetworkAddress(this._networkinterface.InterfaceAddress, this._port),
                        new QS.Fx.Network.ReceiveCallback(this._ReceiveCallback), null);
                this._networkaddress = new QS.Fx.Network.NetworkAddress(this._listener.Address);
                this._port = this._networkaddress.PortNumber;
                
                this._Start(new QS.Fx.Base.Address(this._listener.Address.HostIPAddress, this._listener.Address.PortNumber));
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableTransport_._Stop");
#endif

            lock (this)
            {
                if (this._listener != null)
                    this._listener.Dispose();

                base._Stop();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Address

        protected override QS.Fx.Network.NetworkAddress _Address(QS.Fx.Base.Address _address)
        {
            string[] _x = _address.String.Split(':');
            if (_x.Length != 2)
                _mycontext.Error("Bad address format.");
            string _hostname = _x[0];
            int _port = Convert.ToInt32(_x[1]);
            System.Net.IPAddress _ipaddress;
            try
            {
                _ipaddress = System.Net.IPAddress.Parse(_hostname);
                if (!this._subnet.contains(_ipaddress))
                    _mycontext.Error("The request address \"" + _hostname + "\" is not on subnet " + this._subnet.ToString() + ".");
            }
            catch
            {
                _ipaddress = System.Net.IPAddress.None;
                bool _found = false;
                foreach (System.Net.IPAddress _some_ipaddress in this._platform.Network.GetHostEntry(_hostname).AddressList)
                {
                    if ((_some_ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) && this._subnet.contains(_some_ipaddress))
                    {
                        _ipaddress = _some_ipaddress;
                        _found = true;
                        break;
                    }
                }
                if (!_found)
                    _mycontext.Error("Cannot locate host \"" + _hostname + "\" on subnet " + this._subnet.ToString() + ".");
            }
             return new QS.Fx.Network.NetworkAddress(_ipaddress, _port);
        }

        #endregion

        #region _Channel

        protected override CommunicationChannel_ _Channel(QS.Fx.Network.NetworkAddress _networkaddress)
        {
            return new CommunicationChannel_(this, _networkaddress);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ReceiveCallback

        private unsafe void _ReceiveCallback(System.Net.IPAddress _ipaddress, int _port, QS.Fx.Base.Block _data, object _context)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableTransport_._ReceiveCallback " + _ipaddress.ToString() + ":" + _port.ToString());
#endif

            lock (this)
            {
                ushort _incoming_classid;
                uint _incoming_headersize, _incoming_size, _response_port;
                fixed (byte* _headerptr_0 = _data.buffer)
                {
                    byte* _headerptr = _headerptr_0 + _data.offset;
                    _incoming_classid = *((ushort*)_headerptr);
                    _headerptr += sizeof(ushort);
                    _incoming_headersize = *((uint*)_headerptr);
                    _headerptr += sizeof(uint);
                    _incoming_size = *((uint*)_headerptr);
                    _headerptr += sizeof(uint);
                    _response_port = *((uint*)_headerptr);
                }
                QS.Fx.Serialization.ISerializable _message = QS._core_c_.Base3.Serializer.CreateObject(_incoming_classid);
                QS.Fx.Base.ConsumableBlock _incoming_header =
                    new QS.Fx.Base.ConsumableBlock(
                        _data.buffer, _data.offset + 3 * sizeof(uint) + sizeof(ushort), _incoming_headersize);
                QS.Fx.Base.ConsumableBlock _incoming_data =
                    new QS.Fx.Base.ConsumableBlock(
                        _data.buffer, _data.offset + 3 * sizeof(uint) + sizeof(ushort) + _incoming_headersize, _incoming_size - _incoming_headersize);
                try
                {
                    _message.DeserializeFrom(ref _incoming_header, ref _incoming_data);
                }
                catch (Exception _exc)
                {
                    _mycontext.Error("Could not deserialize the incoming message.", _exc);
                }

                if (!(_message is MessageClass))
                    _mycontext.Error("Received a message of an unexpected type.");

                this._Receive(new QS.Fx.Network.NetworkAddress(_ipaddress, (int)_response_port), (MessageClass)_message);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class CommunicationChannel_

        public new sealed class CommunicationChannel_
            : QS._qss_x_.Properties_.Component_.Transport_<
                MessageClass, QS.Fx.Network.NetworkAddress, UnreliableTransport_<MessageClass>, CommunicationChannel_>.CommunicationChannel_
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Constructor

            public CommunicationChannel_(UnreliableTransport_<MessageClass> _transport, QS.Fx.Network.NetworkAddress _networkaddress)
                : base(_transport._mycontext, _transport, _networkaddress)
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private QS.Fx.Network.ISender _sender;

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Initialize

            protected override void _Initialize()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.UnreliableTransport_.CommunicationChannel_._Initialize");
#endif

                base._Initialize();
            }

            #endregion

            #region _Dispose

            protected override void _Dispose()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.UnreliableTransport_.CommunicationChannel_._Dispose");
#endif

                base._Dispose();
            }

            #endregion

            #region _Start

            protected override void _Start()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.UnreliableTransport_.CommunicationChannel_._Start");
#endif

                base._Start();
            }

            #endregion

            #region _Stop

            protected override void _Stop()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.UnreliableTransport_.CommunicationChannel_._Stop");
#endif

                base._Stop();
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Outgoing

            protected override unsafe void _Outgoing(MessageClass _message)
            {
                if (this._sender == null)
                    this._sender = this._transport._networkinterface.GetSender(this._networkaddress);

                QS.Fx.Serialization.SerializableInfo _info = _message.SerializableInfo;
                QS.Fx.Base.ConsumableBlock _header = new QS.Fx.Base.ConsumableBlock((uint)(_info.HeaderSize + 3 * sizeof(uint) + sizeof(ushort)));
                IList<QS.Fx.Base.Block> _blocks = new List<QS.Fx.Base.Block>(_info.NumberOfBuffers + 1);
                _blocks.Add(_header.Block);
                fixed (byte* _headerptr_0 = _header.Array)
                {
                    byte* _headerptr = _headerptr_0 + _header.Offset;
                    *((ushort*)_headerptr) = _info.ClassID;
                    _headerptr += sizeof(ushort);
                    *((uint*)_headerptr) = (uint)_info.HeaderSize;
                    _headerptr += sizeof(uint);
                    *((uint*)_headerptr) = (uint)_info.Size;
                    _headerptr += sizeof(uint);
                    *((uint*)_headerptr) = (uint)this._transport._port;
                }
                _header.consume(3 * sizeof(uint) + sizeof(ushort));
                _message.SerializeTo(ref _header, ref _blocks);
                _sender.Send(new QS.Fx.Network.Data(_blocks));
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
