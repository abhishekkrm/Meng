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

//#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Group, "Properties Framework Group")]
    public sealed class Group_<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("NameClass", QS.Fx.Reflection.ParameterClass.ValueClass)] NameClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass,        
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>,
        QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>,
        QS.Fx.Interface.Classes.IMembershipChannelClient<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
        QS.Fx.Interface.Classes.ITransportClient<AddressClass, MessageClass>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, IEquatable<IdentifierClass>, QS.Fx.Serialization.IStringSerializable
        where IncarnationClass : class, QS.Fx.Serialization.ISerializable, IComparable<IncarnationClass>, QS.Fx.Serialization.IStringSerializable
        where AddressClass : class, QS.Fx.Serialization.ISerializable, IEquatable<AddressClass>, QS.Fx.Serialization.IStringSerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Group_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("membership", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IMembershipChannel<
                    IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>> _membership_reference,
            [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<AddressClass, MessageClass>>
                    _transport_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_.Constructor");
#endif

            if (_membership_reference == null)
                _mycontext.Error("Membership channel reference cannot be NULL.");
            if (_transport_reference == null)
                _mycontext.Error("Transport reference cannot be NULL.");

            this._membership_reference = _membership_reference;
            this._transport_reference = _transport_reference;

            this._group_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IGroupClient<IdentifierClass, IncarnationClass, NameClass, MessageClass>,
                    QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>>(this);
            this._group_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Connect)));
                    });
            this._group_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Disconnect)));
                    });

            this._membership_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IMembershipChannel<
                        IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
                    QS.Fx.Interface.Classes.IMembershipChannelClient<
                        IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>(this);
            this._membership_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Connect)));
                    });
            this._membership_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Disconnect)));
                    });

            this._transport_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ITransport<AddressClass, MessageClass>,
                    QS.Fx.Interface.Classes.ITransportClient<AddressClass, MessageClass>>(this);
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

            this._InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IGroupClient<IdentifierClass, IncarnationClass, NameClass, MessageClass>,
                    QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>> _group_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>> _membership_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IMembershipChannel<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _membership_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannelClient<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>> _membership_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _membership_connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ITransport<AddressClass, MessageClass>> _transport_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ITransport<AddressClass, MessageClass> _transport_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<AddressClass, MessageClass>,
            QS.Fx.Interface.Classes.ITransportClient<AddressClass, MessageClass>> _transport_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _transport_connection;
        [QS.Fx.Base.Inspectable]
        private bool _is_connected_1;
        [QS.Fx.Base.Inspectable]
        private bool _is_connected_2;
        [QS.Fx.Base.Inspectable]
        private bool _is_registered;
        [QS.Fx.Base.Inspectable]
        private AddressClass _address;
        [QS.Fx.Base.Inspectable]
        private IdentifierClass _identifier;
        [QS.Fx.Base.Inspectable]
        private IncarnationClass _incarnation;
        [QS.Fx.Base.Inspectable]
        private NameClass _name;
        [QS.Fx.Base.Inspectable]
        private bool _isincluded;
        
        private IDictionary<IdentifierClass, Connection_> _connections_1 = new Dictionary<IdentifierClass, Connection_>();
        private IDictionary<AddressClass, Connection_> _connections_2 = new Dictionary<AddressClass, Connection_>();

        [QS.Fx.Base.Inspectable]
        protected QS.Fx.Value.Classes.IMembership<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
                _membership;

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<IdentifierClass, Connection_> __inspectable_connections_1;
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<AddressClass, Connection_> __inspectable_connections_2;

        private void _InitializeInspection()
        {
            __inspectable_connections_1 =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<IdentifierClass, Connection_>("__inspectable_connections_1", _connections_1);
            __inspectable_connections_2 =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<AddressClass, Connection_>("__inspectable_connections_2", _connections_2);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IGroup Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IGroupClient<IdentifierClass, IncarnationClass, NameClass, MessageClass>,
            QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>>
                QS.Fx.Object.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>.Group
        {
            get { return this._group_endpoint; }
        }

        #endregion

        #region IGroup<IdentifierClass,IncarnationClass,NameClass,MessageClass> Members

        void QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>.Register(
            IdentifierClass _identifier, IncarnationClass _incarnation, NameClass _name)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<IdentifierClass, IncarnationClass, NameClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Register), _identifier, _incarnation, _name));            
        }

        void QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>.Unregister(
            IdentifierClass _identifier, IncarnationClass _incarnation)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<IdentifierClass, IncarnationClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Unregister), _identifier, _incarnation));            
        }

        void QS.Fx.Interface.Classes.IGroup<IdentifierClass, IncarnationClass, NameClass, MessageClass>.Message(IdentifierClass _identifier, MessageClass _message)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<IdentifierClass, MessageClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Incoming), _identifier, _message));            
        }

        #endregion

        #region IMembershipChannelClient<IncarnationClass,IMember<IdentifierClass,IncarnationClass,AddressClass>> Members

        void QS.Fx.Interface.Classes.IMembershipChannelClient<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>.Membership(
                QS.Fx.Value.Classes.IMembership<
                    IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _membership)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<
                    QS.Fx.Value.Classes.IMembership<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Membership), _membership));
        }

        #endregion

        #region ITransportClient<AddressClass,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransportClient<AddressClass, MessageClass>.Address(AddressClass _address)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<AddressClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Transport_Address), _address));
        }

        void QS.Fx.Interface.Classes.ITransportClient<AddressClass, MessageClass>.Connected(
            AddressClass _address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> _channel)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<
                    AddressClass, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Transport_Connected), _address, _channel));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Dispose");
#endif

            lock (this)
            {
                if ((this._membership_object != null) && (this._membership_object is IDisposable))
                    ((IDisposable)this._membership_object).Dispose();

                if ((this._transport_object != null) && (this._transport_object is IDisposable))
                    ((IDisposable)this._transport_object).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._membership_object != null) && (this._membership_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._membership_object).Start(this._platform, null);

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
                this._logger.Log("Component_.Group_._Stop");
#endif

            lock (this)
            {
                if ((this._membership_object != null) && (this._membership_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._membership_object).Stop();

                if ((this._transport_object != null) && (this._transport_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._transport_object).Stop();
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Group_Connect

        private void _Group_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Connect");
#endif

            lock (this)
            {
                if (!this._is_connected_1 && this._is_registered && this._group_endpoint.IsConnected)
                {
                    this._is_connected_1 = true;
                    this._Internal_Pass_1_Connect();
                }
            }
        }

        #endregion

        #region _Group_Disconnect

        private void _Group_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Disconnect");
#endif

            lock (this)
            {
                this._is_connected_1 = false;
                this._Internal_Pass_1_Disconnect();
            }
        }

        #endregion

        #region _Group_Register

        private void _Group_Register(QS._qss_x_.Properties_.Base_.IEvent_ _event)            
        {
            QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass, IncarnationClass, NameClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass, IncarnationClass, NameClass>) _event;
            IdentifierClass _identifier = _event_._Object1;
            IncarnationClass _incarnation = _event_._Object2;
            NameClass _name = _event_._Object3;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Register ( " + QS.Fx.Printing.Printable.ToString(_identifier) + ", " +
                    QS.Fx.Printing.Printable.ToString(_incarnation) + ", " + QS.Fx.Printing.Printable.ToString(_name) + " ) ");
#endif

            lock (this)
            {
                this._identifier = _identifier;
                this._incarnation = _incarnation;
                this._name = _name;
                this._is_registered = true;
                if (!this._is_connected_1 && this._group_endpoint.IsConnected)
                {
                    this._is_connected_1 = true;
                    this._Internal_Pass_1_Connect();
                }
            }
        }

        #endregion

        #region _Group_Unregister

        private void _Group_Unregister(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass, IncarnationClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass, IncarnationClass>)_event;
            IdentifierClass _identifier = _event_._Object1;
            IncarnationClass _incarnation = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Unregister ( " + QS.Fx.Printing.Printable.ToString(_identifier) + ", " +
                    QS.Fx.Printing.Printable.ToString(_incarnation) + " ) ");
#endif

            lock (this)
            {
/*
                this._is_connected_1 = false;
                this._is_registered = false;
                this._Internal_Pass_1_Disconnect();
*/

                _membership_endpoint.Interface.Member
                (
                    new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                    (
                        _identifier,
                        false,
                        _incarnation,
                        default(NameClass),
                        null
                    )
                );
            }
        }

        #endregion

        #region _Group_Incoming

        private void _Group_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass, MessageClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass, MessageClass>)_event;
            IdentifierClass _identifier = _event_._Object1;
            MessageClass _message = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Incoming ( " + QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
#endif

            lock (this)
            {
                Connection_ _connection;
                if (_connections_1.TryGetValue(_identifier, out _connection))
                {
                    _Connection_Outgoing(_connection, _message);
                }
            }
        }

        #endregion

        #region _Group_Outgoing

        private void _Group_Outgoing(IdentifierClass _identifier, MessageClass _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Outgoing ( " + QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
#endif

            if (this._group_endpoint.IsConnected)
                this._group_endpoint.Interface.Message(_identifier, _message);
        }

        #endregion

        #region _Group_Membership

        private void _Group_Membership()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Group_Membership");
#endif

            if (this._group_endpoint.IsConnected)
            {
                List<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass>> _members = 
                    new List<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass>>();
                if (this._membership.Members != null)
                {
                    foreach (QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member in this._membership.Members)
                        _members.Add(_member);
                }
                this._group_endpoint.Interface.Membership
                (
                    new QS.Fx.Value.Membership<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass>>
                    (
                        this._membership.Incarnation,
                        this._membership.Incremental,
                        _members.ToArray()
                    )
                );
            }
        }

        #endregion

        // ..................................................................................................................................................................................................................................................................................

        #region _Transport_Connect

        private void _Transport_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Transport_Connect");
#endif
        }

        #endregion

        #region _Transport_Disconnect

        private void _Transport_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Transport_Disconnect");
#endif

            lock (this)
            {
                if (this._is_connected_2)
                {
                    this._is_connected_2 = false;
                    this._Internal_Pass_2_Disconnect();
                }
            }
        }

        #endregion

        #region _Transport_Address

        private void _Transport_Address(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            AddressClass _address = ((QS._qss_x_.Properties_.Base_.IEvent_<AddressClass>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Transport_Address  : " + QS.Fx.Printing.Printable.ToString(_address));
#endif

            lock (this)
            {
                this._address = _address;
                if (!this._is_connected_2 && _transport_endpoint.IsConnected && _membership_endpoint.IsConnected)
                {
                    this._is_connected_2 = true;
                    this._Internal_Pass_2_Connect();
                }
            }
        }

        #endregion

        #region _Transport_Connected

        private void _Transport_Connected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<
                AddressClass, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>> _event_ = 
                    (QS._qss_x_.Properties_.Base_.IEvent_<
                        AddressClass, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>>) _event;
            AddressClass _address = _event_._Object1;
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> _channel = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Transport_Connected : " + _address.ToString());
#endif

            lock (this)
            {
                Connection_ _connection;
                if (this._connections_2.TryGetValue(_address, out _connection))
                {
                    if (!_connection._Connected && !_connection._Connecting)
                    {
                        if (_connection._Initialized)
                            _mycontext.Error("Connection found in an initialized, but not connecting or connected state.");
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Group_._Transport_Connected : " + _connection._Address.ToString() + " : initialized, connecting");
#endif

                            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass> _channelobject = _channel.Dereference(_mycontext);
                            _connection._Initialize(_channelobject);
                            _connection._Initialized = true;
                            _connection._Initializing = false;
                            _connection._Connecting = true;
                        }
                    }
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Group_._Transport_Connected : " + _address.ToString() + " : nonexisting, disposing");
#endif

                    if (_channel is IDisposable)
                        ((IDisposable) _channel).Dispose();
                }
            }
        }

        #endregion

        // ..................................................................................................................................................................................................................................................................................

        #region _Membership_Connect

        private void _Membership_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Membership_Connect");
#endif

            lock (this)
            {
                if (!this._is_connected_2 && _transport_endpoint.IsConnected && this._address != null && _membership_endpoint.IsConnected)
                {
                    this._is_connected_2 = true;
                    this._Internal_Pass_2_Connect();
                }
            }
        }

        #endregion

        #region _Membership_Disconnect

        private void _Membership_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Membership_Disconnect");
#endif

            if (this._is_connected_2)
            {
                this._is_connected_2 = false;
                this._Internal_Pass_2_Disconnect();
            }
        }

        #endregion

        #region _Membership_Membership

        private void _Membership_Membership(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IMembership<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
                    _membership =
                        ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IMembership<
                            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Membership_Membership\n\n" + QS.Fx.Printing.Printable.ToString(_membership));
#endif

            lock (this)
            {
                bool _isincluded = false;
                this._membership = _membership;
                ICollection<IdentifierClass> _remove = new System.Collections.ObjectModel.Collection<IdentifierClass>(new List<IdentifierClass>(this._connections_1.Keys));
                foreach (QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member in this._membership.Members)
                {
                    if (_member.Identifier.Equals(this._identifier))
                    {
                        if (_member.Incarnation.Equals(this._incarnation))
                            _isincluded = true;
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Group_._Membership_Membership : OLD INCARNATION ( " + _member.Incarnation.ToString() + " )");
#endif

                            _membership_endpoint.Interface.Member
                            (
                                new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                                (
                                    _member.Identifier,
                                    false,
                                    _member.Incarnation,
                                    default(NameClass),
                                    null
                                )
                            );
                        }
                    }
                    else
                    {
                        Connection_ _connection;
                        bool _connect = true;
                        if (this._connections_1.TryGetValue(_member.Identifier, out _connection))
                        {
                            _remove.Remove(_member.Identifier);
                            if (_connection._Incarnation.CompareTo(_member.Incarnation) < 0)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log("Component_.Group_._Membership_Membership\n\n" + QS.Fx.Printing.Printable.ToString(_membership) + 
                                        " : deleting " + _connection._Identifier.ToString() + ", " + _connection._Address.ToString());
#endif

                                this._connections_1.Remove(_connection._Identifier);
                                this._connections_2.Remove(_connection._Address);
                                ((IDisposable)_connection).Dispose();
                                _connection = null;
                            }
                            else
                                _connect = false;
                        }
                        if (_connect)
                        {                            
                            List<AddressClass> _memberaddresses = new List<AddressClass>(_member.Addresses);
                            if (_memberaddresses.Count < 1)
                                _mycontext.Error("Member does not have any addresses.\n" + QS.Fx.Printing.Printable.ToString(_member));

                            AddressClass _address = _memberaddresses[0];
                            _connection = new Connection_(this, _member.Identifier, _member.Incarnation, _address);

#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Group_._Membership_Membership\n\n" + QS.Fx.Printing.Printable.ToString(_membership) +
                                    " : creating " + _connection._Identifier.ToString() + ", " + _connection._Address.ToString());
#endif

                            this._connections_1.Add(_member.Identifier, _connection);
                            this._connections_2.Add(_address, _connection);
                        }
                    }
                }
                foreach (IdentifierClass _identifier in _remove)
                {
                    Connection_ _connection;
                    if (this._connections_1.TryGetValue(_identifier, out _connection))
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Group_._Membership_Membership\n\n" + QS.Fx.Printing.Printable.ToString(_membership) +
                                " : deleting " + _connection._Identifier.ToString() + ", " + _connection._Address.ToString());
#endif

                        this._connections_1.Remove(_identifier);
                        this._connections_2.Remove(_connection._Address);
                        ((IDisposable) _connection).Dispose();
                    }
                }
                if (_isincluded)
                {
                    this._isincluded = true;
                }
                else
                {
                    if (this._isincluded)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Group_._Membership_Membership : EXCLUDED FROM MEMBERSHIP");
#endif

                        foreach (Connection_ _connection in (new List<Connection_>(this._connections_1.Values)))
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Group_._Membership_Membership\n\n" + QS.Fx.Printing.Printable.ToString(_membership) +
                                    " : deleting " + _connection._Identifier.ToString() + ", " + _connection._Address.ToString());
#endif

                            this._connections_1.Remove(_connection._Identifier);
                            this._connections_2.Remove(_connection._Address);
                            ((IDisposable)_connection).Dispose();
                        }

                        _membership_endpoint.Interface.Member
                        (
                            new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                            (
                                this._identifier,
                                true,
                                this._incarnation,
                                this._name,
                                new AddressClass[] { this._address }
                            )
                        );
                    }
                    this._isincluded = false;
                }
                this._Group_Membership();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Internal_Pass_1_Connect

        private void _Internal_Pass_1_Connect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Internal_Pass_1_Connect");
#endif

            this._membership_object = _membership_reference.Dereference(_mycontext);
            this._membership_connection = this._membership_endpoint.Connect(this._membership_object.Membership);

            this._transport_object = _transport_reference.Dereference(_mycontext);
            this._transport_connection = this._transport_endpoint.Connect(this._transport_object.Transport);

            if ((this._platform != null) && (this._membership_object is QS._qss_x_.Platform_.IApplication))
                ((QS._qss_x_.Platform_.IApplication)this._membership_object).Start(this._platform, null);

            if ((this._platform != null) && (this._transport_object is QS._qss_x_.Platform_.IApplication))
                ((QS._qss_x_.Platform_.IApplication)this._transport_object).Start(this._platform, null);
        }

        #endregion

        #region _Internal_Pass_1_Disconnect

        private void _Internal_Pass_1_Disconnect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Internal_Pass_1_Disconnect");
#endif

            if (this._is_connected_2)
            {
                this._is_connected_2 = false;
                this._Internal_Pass_2_Disconnect();
            }

            this._membership_endpoint.Disconnect();
            if (this._membership_object is IDisposable)
                ((IDisposable)this._membership_object).Dispose();

            this._transport_endpoint.Disconnect();
            if (this._transport_object is IDisposable)
                ((IDisposable)this._transport_object).Dispose();
        }

        #endregion

        #region _Internal_Pass_2_Connect

        private void _Internal_Pass_2_Connect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Internal_Pass_2_Connect");
#endif

            _membership_endpoint.Interface.Member
            (
                new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                (
                    this._identifier,
                    true,
                    this._incarnation,
                    this._name,
                    new AddressClass[] { this._address }
                )
            );
        }

        #endregion

        #region _Internal_Pass_2_Disconnect

        private void _Internal_Pass_2_Disconnect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Internal_Pass_2_Disconnect");
#endif

            if (this._membership_endpoint.IsConnected)
            {
                this._membership_endpoint.Interface.Member
                (
                    new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                    (
                        this._identifier,
                        false,
                        this._incarnation,
                        this._name,
                        new AddressClass[0]
                    )
                );
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connection_Connect

        private void _Connection_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            Connection_ _connection = ((QS._qss_x_.Properties_.Base_.IEvent_<Connection_>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Connection_Connect : " + _connection._Address.ToString());
#endif

            lock (this)
            {
                if (_connection._Connecting)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Group_._Connection_Connect : " + _connection._Address.ToString() + " : connected");
#endif

                    _connection._Connecting = false;
                    _connection._Connected = true;
                    this._Connection_Outgoing(_connection);
                }
            }
        }

        #endregion

        #region _Connection_Disconnect

        private void _Connection_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            Connection_ _connection = ((QS._qss_x_.Properties_.Base_.IEvent_<Connection_>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Connection_Disconnect : " + _connection._Address.ToString());
#endif
        }

        #endregion

        #region _Connection_Incoming

        private void _Connection_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<Connection_, MessageClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<Connection_, MessageClass>) _event;
            Connection_ _connection = _event_._Object1;
            MessageClass _message = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Connection_Incoming  : " + _connection._Address.ToString() + "\n\n" +
                    QS.Fx.Printing.Printable.ToString(_message));
#endif

            lock (this)
            {
                _Group_Outgoing(_connection._Identifier, _message);
            }
        }

        #endregion

        #region _Connection_Outgoing

        private void _Connection_Outgoing(Connection_ _connection, MessageClass _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Connection_Outgoing  : " + _connection._Address.ToString() + "\n\n" +
                    QS.Fx.Printing.Printable.ToString(_message));
#endif

            _connection._Outgoing.Enqueue(_message);
            if (_connection._Initialized)
            {
                if (_connection._Connected)
                    this._Connection_Outgoing(_connection);
                else
                {
                    if (_connection._Connecting)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Group_._Connection_Outgoing : " + _connection._Address.ToString() + " : still connecting...");
#endif
                    }
                    else
                        _mycontext.Error("Connection found initialized, not connected, and not connecting!");
                }
            }
            else
            {
                if (_connection._Initializing)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Group_._Connection_Outgoing : " + _connection._Address.ToString() + " : still initializing");
#endif
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Group_._Connection_Outgoing : " + _connection._Address.ToString() + " : initializing now");
#endif

                    _connection._Initializing = true;
                    this._transport_endpoint.Interface.Connect(_connection._Address);
                }
            }

/*
                this._pending.Enqueue(_member);
                if (this._memory_initialized)
                    this._Memory_Outgoing();
*/
        }

        private void _Connection_Outgoing(Connection_ _connection)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Group_._Connection_Outgoing  : " + _connection._Address.ToString());
#endif

            if (_connection._CommunicationChannel.IsConnected)
            {
                while (_connection._Outgoing.Count > 0)
                {
                    MessageClass _message = _connection._Outgoing.Dequeue();

#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Group_._Connection_Outgoing  : " + _connection._Address.ToString() + " : message\n\n" +
                            QS.Fx.Printing.Printable.ToString(_message));
#endif

                    _connection._CommunicationChannel.Interface.Message(_message);
                }
            }
            else
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Group_._Connection_Outgoing  : " + _connection._Address.ToString() + " : communication channel is disconnected...");
#endif
            }
        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Connection_

        private sealed class Connection_
            : QS.Fx.Inspection.Inspectable,
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
            IDisposable
        {
            #region Constructor

            public Connection_
            (
                Group_<IdentifierClass, IncarnationClass, NameClass, AddressClass, MessageClass> _group,
                IdentifierClass _identifier,
                IncarnationClass _incarnation,
                AddressClass _address
            )
            {
                this._group = _group;
                this._identifier = _identifier;
                this._incarnation = _incarnation;
                this._address = _address;

#if VERBOSE
                if (this._group._logger != null)
                    this._group._logger.Log("Component_.Group_.Connection_.Constructor ( " + 
                        this._identifier.ToString() + ", " + this._incarnation.ToString() + ", " + this._address.ToString() + " )");
#endif

                this._communicationchannel_endpoint =
                    this._group._mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>(this);
                this._communicationchannel_endpoint.OnConnected +=
                    new QS.Fx.Base.Callback
                    (
                        delegate
                        {
                            this._group._Enqueue(
                                new QS._qss_x_.Properties_.Base_.Event_<Connection_>(
                                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._group._Connection_Connect), this));
                        }
                    );
                this._communicationchannel_endpoint.OnDisconnect +=
                    new QS.Fx.Base.Callback
                    (
                        delegate
                        {
                            this._group._Enqueue(
                                new QS._qss_x_.Properties_.Base_.Event_<Connection_>(
                                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._group._Connection_Disconnect), this));
                        }
                    );
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private Group_<IdentifierClass, IncarnationClass, NameClass, AddressClass, MessageClass> _group;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Object.Classes.ICommunicationChannel<MessageClass> _communicationchannel_object;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> _communicationchannel_endpoint;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Endpoint.IConnection _communicationchannel_connection;
            [QS.Fx.Base.Inspectable]
            private IdentifierClass _identifier;
            [QS.Fx.Base.Inspectable]
            private IncarnationClass _incarnation;
            [QS.Fx.Base.Inspectable]
            private AddressClass _address;
            [QS.Fx.Base.Inspectable]
            private bool _initializing;
            [QS.Fx.Base.Inspectable]
            private bool _initialized;
            [QS.Fx.Base.Inspectable]
            private bool _connecting;
            [QS.Fx.Base.Inspectable]
            private bool _connected;
            [QS.Fx.Base.Inspectable]
            private Queue<MessageClass> _outgoing = new Queue<MessageClass>();

            #endregion

            #region Accessors

            public IdentifierClass _Identifier
            {
                get { return this._identifier; }
            }

            public IncarnationClass _Incarnation
            {
                get { return this._incarnation; }
            }

            public AddressClass _Address
            {
                get { return this._address; }
            }

            public bool _Initializing
            {
                get { return this._initializing; }
                set { this._initializing = value; }
            }

            public bool _Initialized
            {
                get { return this._initialized; }
                set { this._initialized = value; }
            }

            public bool _Connecting
            {
                get { return this._connecting; }
                set { this._connecting = value; }
            }

            public bool _Connected
            {
                get { return this._connected; }
                set { this._connected = value; }
            }

            public QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> _CommunicationChannel
            {
                get { return this._communicationchannel_endpoint; }
            }

            public Queue<MessageClass> _Outgoing
            {
                get { return this._outgoing; }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
#if VERBOSE
                if (this._group._logger != null)
                    this._group._logger.Log("Component_.Group_.Connection_.Dispose ( " +
                        this._identifier.ToString() + ", " + this._incarnation.ToString() + ", " + this._address.ToString() + " )");
#endif

                if (this._communicationchannel_endpoint.IsConnected)
                    this._communicationchannel_endpoint.Disconnect();
                if ((this._communicationchannel_object != null) && (this._communicationchannel_object is IDisposable))
                    ((IDisposable) this._communicationchannel_object).Dispose();
            }

            #endregion

            #region ICommunicationChannel<ISerializable> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>.Message(MessageClass _message)
            {
                this._group._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<Connection_, MessageClass>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._group._Connection_Incoming), this, _message));
            }

            #endregion

            #region _Initialize

            public void _Initialize(QS.Fx.Object.Classes.ICommunicationChannel<MessageClass> _communicationchannel_object)
            {
                try
                {
                    if (this._communicationchannel_object != null)
                        this._group._mycontext.Error("Connection has alread been initialized.");
                    this._communicationchannel_object = _communicationchannel_object;
                    this._communicationchannel_connection = this._communicationchannel_endpoint.Connect(this._communicationchannel_object.Channel);
                }
                catch (Exception _exc)
                {
                    this._group._mycontext.Error("Could not initialize connection \"" + this._address.ToString() + "\".", _exc);
                }
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
