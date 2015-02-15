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
    [QS.Fx.Reflection.ComponentClass(QS._qss_x_.Properties_.Component_.Classes_._ReliableTransport, "Properties Framework Reliable Transport")]
    public sealed class ReliableTransport_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.Properties_.Component_.Transport_<
            MessageClass, QS.Fx.Network.NetworkAddress, ReliableTransport_<MessageClass>, ReliableTransport_<MessageClass>.CommunicationChannel_>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public ReliableTransport_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("subnet", QS.Fx.Reflection.ParameterClass.Value)]
            string _subnet,
            [QS.Fx.Reflection.Parameter("port", QS.Fx.Reflection.ParameterClass.Value)]
            int _port,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.ReliableTransport_.Constructor");
#endif

            this._subnet = new QS._qss_c_.Base1_.Subnet(_subnet);
            this._port = _port;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _hostname;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet _subnet;
        [QS.Fx.Base.Inspectable]
        private int _port;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.INetworkInterface _networkinterface;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.NetworkAddress _networkaddress;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base3_.IDemultiplexer _demultiplexer;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base8_.Root _root;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Senders3.ReliableSender1 _sender;        

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.ReliableTransport_._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.ReliableTransport_._Dispose");
#endif

            lock (this)
            {
                if ((this._sender != null) && (this._sender is IDisposable))
                    ((IDisposable) this._sender).Dispose();
                if ((this._root != null) && (this._root is IDisposable))
                    ((IDisposable) this._root).Dispose();
                if ((this._demultiplexer != null) && (this._demultiplexer is IDisposable))
                    ((IDisposable) this._demultiplexer).Dispose();

                base._Dispose();
            }
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.ReliableTransport_._Start");
#endif

            lock (this)
            {
                base._Start();

                this._hostname = _platform.Network.GetHostName();
                this._networkinterface = null;
                foreach (QS.Fx.Network.INetworkInterface _nic in _platform.Network.Interfaces)
                {
                    if (_nic.InterfaceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (this._subnet.contains(_nic.InterfaceAddress))
                        {
                            this._networkinterface = _nic;
                            break;
                        }
                    }
                }
                if (this._networkinterface == null)
                    _mycontext.Error("Could not locate any network adapter on the requested subnet " + this._subnet.ToString() + ".");
                if (this._port == 0)
                {
                    QS.Fx.Network.IListener _listener = 
                        this._networkinterface.Listen(new QS.Fx.Network.NetworkAddress(this._networkinterface.InterfaceAddress, 0), null, null);
                    this._port = _listener.Address.PortNumber;
                    _listener.Dispose();
                    if (this._port == 0)
                        _mycontext.Error("Could not allocate port.");

#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.ReliableTransport_._Start ( " + this._port.ToString() + " )");
#endif
                }
                this._networkaddress = new QS.Fx.Network.NetworkAddress(this._networkinterface.InterfaceAddress, this._port);
                this._demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(this._platform.Logger, this._platform.EventLogger);
                this._demultiplexer.register(10000, new QS._qss_c_.Base3_.ReceiveCallback(this._ReceiveCallback));
                this._root = new QS._qss_c_.Base8_.Root(null, this._platform.Logger, this._platform.EventLogger, this._platform.Clock,
                    this._platform.Network, new QS._core_c_.Base3.InstanceID(this._networkaddress, DateTime.Now), this._demultiplexer, 20000);
                this._sender = new QS._qss_c_.Senders3.ReliableSender1(this._platform.Logger, this._demultiplexer, this._root, this._platform.AlarmClock, 1);
                this._Start(new QS.Fx.Base.Address(this._networkaddress.ToString()));
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.ReliableTransport_._Stop");
#endif

            lock (this)
            {
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
            System.Net.IPAddress _ipaddress = System.Net.IPAddress.None;
            try
            {
                _ipaddress = System.Net.IPAddress.Parse(_hostname);
            }
            catch (Exception)
            {
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

        private QS.Fx.Serialization.ISerializable _ReceiveCallback(QS._core_c_.Base3.InstanceID _from, QS.Fx.Serialization.ISerializable _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.ReliableTransport_._ReceiveCallback ( " + _from.ToString() + " )");
#endif

            lock (this)
            {
                if (!(_message is MessageClass))
                    _mycontext.Error("Received a message of an unexpected type.");

                this._Receive(_from.Address, (MessageClass) _message);
            }

            return null;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class CommunicationChannel_

        public new sealed class CommunicationChannel_
            : QS._qss_x_.Properties_.Component_.Transport_<
                MessageClass, QS.Fx.Network.NetworkAddress, ReliableTransport_<MessageClass>, CommunicationChannel_>.CommunicationChannel_
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Constructor

            public CommunicationChannel_(ReliableTransport_<MessageClass> _transport, QS.Fx.Network.NetworkAddress _networkaddress)
                : base(_transport._mycontext,  _transport, _networkaddress)
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Base3_.IReliableSerializableSender _sender;

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Initialize

            protected override void _Initialize()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableTransport_.CommunicationChannel_._Initialize");
#endif

                base._Initialize();
            }

            #endregion

            #region _Dispose

            protected override void _Dispose()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableTransport_.CommunicationChannel_._Dispose");
#endif

                lock (this)
                {
                    if ((this._sender != null) && (this._sender is IDisposable))
                        ((IDisposable)this._sender).Dispose();

                    base._Dispose();
                }
            }

            #endregion

            #region _Start

            protected override void _Start()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableTransport_.CommunicationChannel_._Start");
#endif

                base._Start();
            }

            #endregion

            #region _Stop

            protected override void _Stop()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableTransport_.CommunicationChannel_._Stop");
#endif

                base._Stop();
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Outgoing

            protected override unsafe void _Outgoing(MessageClass _message)
            {
                if (this._sender == null)
                    this._sender = this._transport._sender.SenderCollection[this._networkaddress];

                _sender.send(10000, _message);
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
