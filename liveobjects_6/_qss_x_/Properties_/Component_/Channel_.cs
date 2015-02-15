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
    public abstract class Channel_<IncomingClass, OutgoingClass, ConnectionClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>
        where IncomingClass : class, QS.Fx.Serialization.ISerializable
        where OutgoingClass : class, QS.Fx.Serialization.ISerializable
        where ConnectionClass : Channel_<IncomingClass, OutgoingClass, ConnectionClass>.Connection_, new()
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        protected Channel_
        (
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>> _transport_reference,                        
            QS.Fx.Base.Address _address,            
            bool _isreplica,
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_.Constructor");
#endif

            if (_transport_reference == null)
                _mycontext.Error("Transport reference cannot be NULL.");
            this._transport_reference = _transport_reference;
            this._address = _address;
            this._isreplica = _isreplica;
            if (!this._isreplica && (_address == null))
                _mycontext.Error("If not acting as a replica, address cannot be NULL.");
            this._transport_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>>(this);
            this._transport_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Transport_Connect)));
                    });
            this._transport_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Transport_Disconnect)));
                    });
            this._channel_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>>(this);
            this._channel_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Connect)));
                    });
            this._channel_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Disconnect)));
                    });
        }

        #endregion
            
        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>> _transport_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable> _transport_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>> _transport_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _transport_connection;
        [QS.Fx.Base.Inspectable]        
        private QS.Fx.Base.Address _address;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Address _localaddress;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _channel_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable> _channel_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _channel_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channel_connection;
        [QS.Fx.Base.Inspectable]
        private bool _isconnected;
        [QS.Fx.Base.Inspectable]
        private Queue<IncomingClass> _channel_incoming = new Queue<IncomingClass>();
        [QS.Fx.Base.Inspectable]
        private Queue<OutgoingClass> _channel_outgoing = new Queue<OutgoingClass>();

        [QS.Fx.Base.Inspectable]
        protected bool _isreplica;
        [QS.Fx.Base.Inspectable]
        protected IDictionary<QS.Fx.Base.Address, ConnectionClass> _connections = new Dictionary<QS.Fx.Base.Address, ConnectionClass>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransportClient<QS.Fx.Base.Address,QS.Fx.Serialization.ISerializable> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>.Address(QS.Fx.Base.Address _localaddress)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Address>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Transport_Address), _localaddress));
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>.Connected(
            QS.Fx.Base.Address _address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _channel)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<
                    QS.Fx.Base.Address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>>>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Transport_Connected), _address, _channel));
        }

        #endregion

        #region ICommunicationChannel<ISerializable> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>.Message(QS.Fx.Serialization.ISerializable _message)
        {
            if (this._isreplica)
                throw new NotSupportedException();
            else
            {
                if (!(_message is IncomingClass))
                    _mycontext.Error("Received a message of an unknown type.");
                IncomingClass _incoming = (IncomingClass) _message;
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<IncomingClass>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Incoming), _incoming));
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                this._transport_object = this._transport_reference.Dereference(_mycontext);

                if ((this._platform != null) && (this._transport_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._transport_object).Start(this._platform, null);

                this._transport_connection = this._transport_endpoint.Connect(this._transport_object.Transport);
            }
        }
        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Dispose");
#endif

            lock (this)
            {
                if (this._transport_endpoint.IsConnected)
                    this._transport_endpoint.Disconnect();

                if ((this._transport_object != null) && (this._transport_object is IDisposable))
                    ((IDisposable) this._transport_object).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._transport_object != null) && (this._transport_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._transport_object).Start(this._platform, null);
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Stop");
#endif

            lock (this)
            {
                if ((this._transport_object != null) && (this._transport_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._transport_object).Stop();
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Transport_Connect

        private void _Transport_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Transport_Connect");
#endif

            lock (this)
            {
                if (!this._isreplica && _transport_endpoint.IsConnected)
                    _transport_endpoint.Interface.Connect(this._address);
            }
        }

        #endregion

        #region _Transport_Disconnect

        private void _Transport_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Transport_Disconnect");
#endif

            lock (this)
            {
                foreach (ConnectionClass _connection in this._connections.Values)
                    ((IDisposable) _connection).Dispose();
                this._connections.Clear();
                this._isconnected = false;
            }
        }

        #endregion

        #region _Transport_Address

        private void _Transport_Address(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Base.Address _localaddress = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Address>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Address  : " + QS.Fx.Printing.Printable.ToString(_localaddress));
#endif

            lock (this)
            {
                this._localaddress = _localaddress;
            }
        }

        #endregion

        #region _Transport_Connected

        private void _Transport_Connected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<
                QS.Fx.Base.Address,
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>>> _event_ =
                        (QS._qss_x_.Properties_.Base_.IEvent_<
                            QS.Fx.Base.Address,
                            QS.Fx.Object.IReference<
                                QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>>>)_event;
            QS.Fx.Base.Address _address = _event_._Object1;
            QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _channel = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Connected : " + QS.Fx.Printing.Printable.ToString(_address));
#endif

            lock (this)
            {
                if (this._isreplica)
                {
                    QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable> _channelobject = _channel.Dereference(_mycontext);
                    ConnectionClass _connection = new ConnectionClass();
                    _connection._Initialize(this, _address, _channelobject);
                    this._connections.Add(_address, _connection);
                }
                else
                {
                    this._channel_reference = _channel;
                    this._channel_object = _channel_reference.Dereference(_mycontext);
                    this._channel_connection = this._channel_endpoint.Connect(this._channel_object.Channel);
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Channel_Connect

        private void _Channel_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Channel_Connect");
#endif

            lock (this)
            {
                if (this._channel_endpoint.IsConnected)
                {
                    this._isconnected = true;
                    while (this._channel_outgoing.Count > 0)
                        this._channel_endpoint.Interface.Message(this._channel_outgoing.Dequeue());
                }
            }
        }

        #endregion

        #region _Channel_Disconnect

        private void _Channel_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Channel_Disconnect");
#endif

            lock (this)
            {
                this._isconnected = false;
                this._channel_outgoing.Clear();
                this._channel_incoming.Clear();
            }
        }

        #endregion

        #region _Channel_Outgoing

        protected void _Channel_Outgoing(OutgoingClass _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Channel_Outgoing  : " + QS.Fx.Printing.Printable.ToString(_message));
#endif

            this._channel_outgoing.Enqueue(_message);
            if (this._isconnected && this._channel_endpoint.IsConnected)
            {
                while (this._channel_outgoing.Count > 0)
                    this._channel_endpoint.Interface.Message(this._channel_outgoing.Dequeue());
            }
        }

        #endregion

        #region _Channel_Incoming

        private void _Channel_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            IncomingClass _message = ((QS._qss_x_.Properties_.Base_.IEvent_<IncomingClass>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Channel_Incoming\n\n" + QS.Fx.Printing.Printable.ToString(_message) + "\n\n");
#endif

            lock (this)
            {
                if (this._isreplica)
                    throw new NotSupportedException();
                else
                {
                    this._channel_incoming.Enqueue(_message);
                    while (this._channel_incoming.Count > 0)
                        _Channel_Incoming(this._channel_incoming.Dequeue());
                }
            }
        }

        protected abstract void _Channel_Incoming(IncomingClass _message);

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connection_Connect

        private void _Connection_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            Connection_ _connection = ((QS._qss_x_.Properties_.Base_.IEvent_<Connection_>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Connection_Connect : " + _connection._Address.ToString());
#endif

            lock (this)
            {
                _connection._Connected = true;
                this._Connection_Connected((ConnectionClass) _connection);
            }
        }

        #endregion

        #region _Connection_Disconnect

        private void _Connection_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            Connection_ _connection = ((QS._qss_x_.Properties_.Base_.IEvent_<Connection_>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Connection_Disconnect : " + _connection._Address.ToString());
#endif

            lock (this)
            {
                if (this._connections.ContainsKey(_connection._Address))
                    this._connections.Remove(_connection._Address);

                this._Connection_Disconnecting((ConnectionClass)_connection);
            }

            if (_connection is IDisposable)
                ((IDisposable) _connection).Dispose();
        }

        #endregion

        #region _Connection_Connected

        protected abstract void _Connection_Connected(ConnectionClass _connection);

        #endregion

        #region _Connection_Disconnecting

        protected abstract void _Connection_Disconnecting(ConnectionClass _connection);

        #endregion

        #region _Connection_Incoming

        private void _Connection_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<Connection_, OutgoingClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<Connection_, OutgoingClass>)_event;
            Connection_ _connection = _event_._Object1;
            OutgoingClass _message = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Connection_Incoming  : " + _connection._Address.ToString() + "\n\n" +
                    QS.Fx.Printing.Printable.ToString(_message));
#endif

            lock (this)
            {
                this._Connection_Incoming((ConnectionClass) _connection, _message);
            }
        }

        protected abstract void _Connection_Incoming(ConnectionClass _connection, OutgoingClass _message);

        #endregion

        #region _Connection_Outgoing

        protected void _Connection_Outgoing(ConnectionClass _connection, IncomingClass _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Channel_._Connection_Outgoing  : " + _connection._Address.ToString() + "\n\n" +
                    QS.Fx.Printing.Printable.ToString(_message));
#endif

            if (_connection._CommunicationChannel.IsConnected)
                _connection._CommunicationChannel.Interface.Message(_message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Connection_

        public abstract class Connection_ 
            : QS.Fx.Inspection.Inspectable,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
            IDisposable
        {
            #region Constructor

            public Connection_()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private Channel_<IncomingClass, OutgoingClass, ConnectionClass> _encapsulatingchannel;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.Address _address;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable> _communicationchannel_object;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _communicationchannel_endpoint;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Endpoint.IConnection _communicationchannel_connection;
            [QS.Fx.Base.Inspectable]
            private bool _connected;

            #endregion

            #region Accessors

            public QS.Fx.Base.Address _Address
            {
                get { return this._address; }
            }

            public bool _Connected
            {
                get { return this._connected; }
                set { this._connected = value; }
            }

            public QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _CommunicationChannel
            {
                get { return this._communicationchannel_endpoint; }
            }

            #endregion

            #region _Initialize

            public void _Initialize
            (
                Channel_<IncomingClass, OutgoingClass, ConnectionClass> _encapsulatingchannel,
                QS.Fx.Base.Address _address,
                QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable> _communicationchannel_object
            )
            {
                this._encapsulatingchannel = _encapsulatingchannel;
                this._address = _address;
                this._communicationchannel_object = _communicationchannel_object;
                this._communicationchannel_endpoint =
                    _encapsulatingchannel._mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>>(this);
                this._communicationchannel_endpoint.OnConnected +=
                    new QS.Fx.Base.Callback
                    (
                        delegate
                        {
                            this._encapsulatingchannel._Enqueue(
                                new QS._qss_x_.Properties_.Base_.Event_<Connection_>(
                                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._encapsulatingchannel._Connection_Connect), this));
                        }
                    );
                this._communicationchannel_endpoint.OnDisconnect +=
                    new QS.Fx.Base.Callback
                    (
                        delegate
                        {
                            this._encapsulatingchannel._Enqueue(
                                new QS._qss_x_.Properties_.Base_.Event_<Connection_>(
                                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._encapsulatingchannel._Connection_Disconnect), this));
                        }
                    );
                this._communicationchannel_connection = this._communicationchannel_endpoint.Connect(this._communicationchannel_object.Channel);
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                lock (this)
                {
                    if ((this._communicationchannel_object != null) && (this._communicationchannel_object is IDisposable))
                        ((IDisposable)this._communicationchannel_object).Dispose();
                }
            }

            #endregion

            #region ICommunicationChannel<ISerializable> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>.Message(QS.Fx.Serialization.ISerializable _message)
            {
                if (!(_message is OutgoingClass))
                    this._encapsulatingchannel._mycontext.Error("Received a message of an unknown type.");
                OutgoingClass _outgoing = (OutgoingClass) _message;
                this._encapsulatingchannel._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<Connection_, OutgoingClass>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._encapsulatingchannel._Connection_Incoming), this, _outgoing));                
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
