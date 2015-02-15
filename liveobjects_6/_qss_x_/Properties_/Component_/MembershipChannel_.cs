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
#define STATISTICS
#define HISTORY

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.MembershipChannel, "Properties Framework Membership Channel")]
    public sealed class MembershipChannel_<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("NameClass", QS.Fx.Reflection.ParameterClass.ValueClass)] NameClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : QS._qss_x_.Properties_.Component_.Channel_<QS._qss_x_.Properties_.Value_.Membership_, QS._qss_x_.Properties_.Value_.Member_, 
            MembershipChannel_<IdentifierClass, IncarnationClass, NameClass, AddressClass>.Connection_>,
        QS.Fx.Object.Classes.IMembershipChannel<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
        QS.Fx.Interface.Classes.IMembershipChannel<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, IEquatable<IdentifierClass>
        where IncarnationClass : class, QS.Fx.Serialization.ISerializable, QS.Fx.Base.IIncrementable<IncarnationClass>, IComparable<IncarnationClass>, new()
        where NameClass : class, QS.Fx.Serialization.ISerializable
        where AddressClass : class, QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MembershipChannel_
        (
            QS.Fx.Object.IContext _mycontext,

            [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<
                    QS.Fx.Base.Address,
                    QS.Fx.Serialization.ISerializable>> _transport_reference,            
            
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _address,
            
            [QS.Fx.Reflection.Parameter("memory", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        QS.Fx.Serialization.ISerializable, 
                        QS.Fx.Serialization.ISerializable>> _memory_reference,

            [QS.Fx.Reflection.Parameter("batching", QS.Fx.Reflection.ParameterClass.Value)]
            double _batching,            

            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _transport_reference, _address, (_memory_reference != null), _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_.Constructor");
#endif

            this._memory_reference = _memory_reference;
            this._batching = _batching;
            this._membership_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IMembershipChannelClient<
                        IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
                    QS.Fx.Interface.Classes.IMembershipChannel<
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
            this._memory_endpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                        QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                        QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>(this);
            this._memory_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Memory_Connect)));
                    });
            this._memory_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Memory_Disconnect)));
                    });
        }

        #endregion
            
        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannelClient<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>
                    _membership_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                    QS.Fx.Serialization.ISerializable,
                    QS.Fx.Serialization.ISerializable>> _memory_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
            QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable> _memory_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> _memory_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _memory_connection;        
        [QS.Fx.Base.Inspectable]
        private bool _memory_initialized;
        [QS.Fx.Base.Inspectable]
        private Queue<QS._qss_x_.Properties_.Value_.Member_> _pending = new Queue<QS._qss_x_.Properties_.Value_.Member_>();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMembership<
            IncarnationClass,
            QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _membership;
        [QS.Fx.Base.Inspectable]
        private double _batching;
        [QS.Fx.Base.Inspectable]
        private double _lastsubmission;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _submissionalarm;
        [QS.Fx.Base.Inspectable]
        private double _size;
#if STATISTICS
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_size = new QS._qss_c_.Statistics_.Samples2D(
            "membership sizes", "sizes of memberships as a function of time", "time", "s", "time in seconds", "membership size", "nodes", "number of nodes"); 
#endif
#if HISTORY
        [QS.Fx.Base.Inspectable]
        private List<QS.Fx.Value.Classes.IMembership<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>> _memberships =
                new List<QS.Fx.Value.Classes.IMembership<
                    IncarnationClass,QS.Fx.Value.Classes.IMember<IdentifierClass,IncarnationClass,NameClass,AddressClass>>>();
#endif

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IMembershipChannel Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannelClient<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>
        QS.Fx.Object.Classes.IMembershipChannel<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>.Membership
        {
            get { return this._membership_endpoint; }
        }

        #endregion

        #region IMembershipChannel Members

        void QS.Fx.Interface.Classes.IMembershipChannel<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>.Member(
                QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Member), _member));
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<ISerializable,ISerializable> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Receive(
            QS.Fx.Serialization.ISerializable _message)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Serialization.ISerializable>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Memory_Receive), _message));
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Initialize(
            QS.Fx.Serialization.ISerializable _checkpoint)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Serialization.ISerializable>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Memory_Initialize), _checkpoint));
        }

        QS.Fx.Serialization.ISerializable 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Checkpoint()
        {
            return this._Memory_Checkpoint();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                if (this._memory_reference != null)
                {
                    this._memory_object = this._memory_reference.Dereference(_mycontext);

                    if ((this._platform != null) && (this._memory_object is QS._qss_x_.Platform_.IApplication))
                        ((QS._qss_x_.Platform_.IApplication) this._memory_object).Start(this._platform, null);

                    this._memory_connection = this._memory_endpoint.Connect(this._memory_object.Channel);
                }
            }
        }
        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Dispose");
#endif

            lock (this)
            {
                if ((this._memory_object != null) && (this._memory_object is IDisposable))
                    ((IDisposable) this._memory_object).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._memory_object != null) && (this._memory_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._memory_object).Start(this._platform, null);

                _Memory_Outgoing();
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Stop");
#endif

            lock (this)
            {
                if ((this._memory_object != null) && (this._memory_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._memory_object).Stop();
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Membership_Connect

        private void _Membership_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Membership_Connect");
#endif
        }

        #endregion

        #region _Membership_Disconnect

        private void _Membership_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Membership_Disconnect");
#endif
        }

        #endregion

        #region _Membership_Member

        private void _Membership_Member(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member =
                ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Member\n\n" + QS.Fx.Printing.Printable.ToString(_member) + "\n\n");
#endif

            lock (this)
            {
                if (this._isreplica)
                    throw new NotSupportedException();
                else
                {
                    List<QS.Fx.Serialization.ISerializable> _addresses = new List<QS.Fx.Serialization.ISerializable>();
                    if (_member.Addresses != null)
                        foreach (AddressClass _address in _member.Addresses)
                            _addresses.Add(_address);
                    this._Channel_Outgoing
                    (
                        new QS._qss_x_.Properties_.Value_.Member_
                        (
                            _member.Identifier,
                            _member.Operational,
                            _member.Incarnation,
                            _member.Name,
                            _addresses.ToArray()
                        )
                    );
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Memory_Connect

        private void _Memory_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Memory_Connect");
#endif
        }

        #endregion

        #region _Memory_Disconnect

        private void _Memory_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Memory_Disconnect");
#endif
        }

        #endregion

        #region _Memory_Initialize

        private void _Memory_Initialize(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Serialization.ISerializable _checkpoint = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Serialization.ISerializable>)_event)._Object;

            QS.Fx.Value.Classes.IMembership<
                IncarnationClass,
                QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _membership_0;

            if (_checkpoint != null)
            {
                if (!(_checkpoint is QS._qss_x_.Properties_.Value_.Membership_))
                    _mycontext.Error("Received a checkpoint of an unknown type.");
                
                QS._qss_x_.Properties_.Value_.Membership_ _membership = (QS._qss_x_.Properties_.Value_.Membership_) _checkpoint;
                _membership_0 = _SerializableToExternalizable(_membership);
            }
            else
            {
                _membership_0 =
                    new QS.Fx.Value.Membership<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
                    (
                        new IncarnationClass(),
                        false, 
                        new QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>[0]
                    );
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Memory_Initialize\n\n" + QS.Fx.Printing.Printable.ToString(_membership_0) + "\n\n");
#endif

            lock (this)
            {
                this._memory_initialized = true;
                this._membership = _membership_0;
#if HISTORY
                this._memberships.Add(this._membership);
#endif
                this._Memory_Outgoing();
            }
        }

        #endregion

        #region _Memory_Checkpoint

        private QS.Fx.Serialization.ISerializable _Memory_Checkpoint()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Memory_Checkpoint");
#endif

            lock (this)
            {
                return (this._membership != null) ? _ExternalizableToSerializable(this._membership) : null;
            }
        }

        #endregion

        #region _Memory_Receive

        private void _Memory_Receive(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Serialization.ISerializable _message = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Serialization.ISerializable>)_event)._Object;
            if (!(_message is QS._qss_x_.Properties_.Value_.Membership_))
                _mycontext.Error("Received a wrong message type.");
            QS._qss_x_.Properties_.Value_.Membership_ _membership_0 = (QS._qss_x_.Properties_.Value_.Membership_) _message;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Memory_Receive\n\n" + QS.Fx.Printing.Printable.ToString(_message) + "\n\n");
#endif

            lock (this)
            {
                if (_Update(ref this._membership, _membership_0, this._membership.Incarnation.Incremented))
                {
#if HISTORY
                    this._memberships.Add(this._membership);
#endif
                    int _n = 0;
                    foreach (object o in this._membership.Members)
                        _n++;
                    this._size = _n;
#if STATISTICS
                    if ((this._statistics_size != null) && (this._platform != null))
                        this._statistics_size.Add(this._platform.Clock.Time, _n);
#endif

#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.MembershipChannel_._Memory_Receive : =====> NEW MEMBERSHIP ( " +
                            this._membership.Incarnation.ToString() + " ) OF SIZE " + _n.ToString() + " <=====\n\n" + 
                            QS.Fx.Printing.Printable.ToString(this._membership) + "\n\n");
#endif

                    QS._qss_x_.Properties_.Value_.Membership_ _membership_1 = (QS._qss_x_.Properties_.Value_.Membership_)  this._Memory_Checkpoint();
                    QS._qss_x_.Properties_.Value_.Membership_ _membership_2 =
                        new QS._qss_x_.Properties_.Value_.Membership_
                        (   
                            _membership_1.Incarnation,
                            true,
                            _membership_0.Members
                        );
                    foreach (Connection_ _connection in this._connections.Values)
                    {
                        if (_connection._Connected)
                        {
                            if (_connection._Initialized)
                                this._Connection_Outgoing(_connection, _membership_2);
                            else
                            {
                                this._Connection_Outgoing(_connection, _membership_1);
                                _connection._Initialized = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region _Memory_Outgoing

        private void _Memory_Outgoing()
        {
            if ((_pending.Count > 0) && (_platform != null) && (_memory_endpoint.IsConnected))
            {
                double _time = _platform.Clock.Time;
                if (_time >= _lastsubmission + _batching)
                {
                    _lastsubmission = _time;

                    IDictionary<IdentifierClass, QS._qss_x_.Properties_.Value_.Member_> _members =
                        new Dictionary<IdentifierClass, QS._qss_x_.Properties_.Value_.Member_>();
                    foreach (QS._qss_x_.Properties_.Value_.Member_ _member in _pending)
                    {
                        QS._qss_x_.Properties_.Value_.Member_ _member_0;
                        if (_members.TryGetValue((IdentifierClass) _member.Identifier, out _member_0))
                        {
                            int _c = ((IncarnationClass)_member.Incarnation).CompareTo((IncarnationClass)_member_0.Incarnation);
                            if ((_c > 0) || ((_c == 0) && !_member_0.Operational))
                            {
                                _members.Remove((IdentifierClass)_member.Identifier);
                                _members.Add((IdentifierClass)_member.Identifier, _member);
                            }
                        }
                        else
                            _members.Add((IdentifierClass) _member.Identifier, _member);
                    }
                    QS._qss_x_.Properties_.Value_.Member_[] _members_0 = new QS._qss_x_.Properties_.Value_.Member_[_members.Count];
                    _members.Values.CopyTo(_members_0, 0);
                    _memory_endpoint.Interface.Send
                    (
                        new QS._qss_x_.Properties_.Value_.Membership_
                        (
                            new IncarnationClass(),
                            true,
                            _members_0
                        )
                    );
                    _pending.Clear();
                }
                else
                {
                    if (_submissionalarm == null)
                    {
                        _submissionalarm = this._platform.AlarmClock.Schedule
                        (
                            _lastsubmission + _batching - _time,
                            new QS.Fx.Clock.AlarmCallback
                            (
                                delegate(QS.Fx.Clock.IAlarm _alarm)
                                {
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Memory_Outgoing));
                                }
                            ),
                            null
                        );
                    }
                }
            }
        }

        private void _Memory_Outgoing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            lock (this)
            {
                _submissionalarm = null;
                _Memory_Outgoing();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Channel_Incoming

        protected override void _Channel_Incoming(QS._qss_x_.Properties_.Value_.Membership_ _membership)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MembershipChannel_._Channel_Incoming  : " + QS.Fx.Printing.Printable.ToString(_membership));
#endif

            bool _modified = false;
            if (_membership.Incremental)
            {
                _modified = _Update(ref this._membership, _membership, (IncarnationClass)_membership.Incarnation);
#if HISTORY
                if (_modified)
                    this._memberships.Add(this._membership);
#endif
            }
            else
            {
                this._membership = _SerializableToExternalizable(_membership);
#if HISTORY
                this._memberships.Add(this._membership);
#endif
                _modified = true;
            }

            if (_modified && this._membership_endpoint.IsConnected)
                this._membership_endpoint.Interface.Membership(this._membership);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connection_Connected

        protected override void _Connection_Connected(Connection_ _connection)
        {
            if (this._memory_initialized)
            {
                QS._qss_x_.Properties_.Value_.Membership_ _membership = (QS._qss_x_.Properties_.Value_.Membership_)this._Memory_Checkpoint();
                this._Connection_Outgoing(_connection, _membership);
                _connection._Initialized = true;
            }
        }

        #endregion

        #region _Connection_Disconnecting

        protected override void _Connection_Disconnecting(Connection_ _connection)
        {
        }

        #endregion

        #region _Connection_Incoming

        protected override void _Connection_Incoming(Connection_ _connection, QS._qss_x_.Properties_.Value_.Member_ _message)
        {
            this._pending.Enqueue(_message);
            if (this._memory_initialized)
                this._Memory_Outgoing();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _SerializableToExternalizable

        private static QS.Fx.Value.Classes.IMembership<
            IncarnationClass,
            QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
                _SerializableToExternalizable(QS._qss_x_.Properties_.Value_.Membership_ _membership)
        {
            List<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _members =
                new List<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>();
            if (_membership.Members != null)
            {
                foreach (QS._qss_x_.Properties_.Value_.Member_ _member in _membership.Members)
                {
                    List<AddressClass> _addresses = new List<AddressClass>();
                    if (_member.Addresses != null)
                    {
                        foreach (QS.Fx.Serialization.ISerializable _address in _member.Addresses)
                            _addresses.Add((AddressClass)_address);
                    }
                    _members.Add
                    (
                        new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                        (
                            (IdentifierClass)_member.Identifier,
                            _member.Operational,
                            (IncarnationClass)_member.Incarnation,
                            (NameClass)_member.Name,
                            _addresses.ToArray()
                        )
                    );
                }
            }
            return new QS.Fx.Value.Membership<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
            (
                (IncarnationClass)_membership.Incarnation,
                _membership.Incremental,
                _members.ToArray()
            );
        }

        #endregion

        #region _ExternalizableToSerializable

        private static QS._qss_x_.Properties_.Value_.Membership_ _ExternalizableToSerializable(
            QS.Fx.Value.Classes.IMembership<
                IncarnationClass,
                QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _membership)
        {
            List<QS._qss_x_.Properties_.Value_.Member_> _members = new List<QS._qss_x_.Properties_.Value_.Member_>();
            foreach (QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member in _membership.Members)
            {
                List<QS.Fx.Serialization.ISerializable> _addresses = new List<QS.Fx.Serialization.ISerializable>();
                if (_member.Addresses != null)
                {
                    foreach (AddressClass _address in _member.Addresses)
                        _addresses.Add(_address);
                }
                _members.Add
                (
                    new QS._qss_x_.Properties_.Value_.Member_
                    (
                        _member.Identifier,
                        _member.Operational,
                        _member.Incarnation,
                        _member.Name,
                        _addresses.ToArray()
                    )
                );
            }
            return new QS._qss_x_.Properties_.Value_.Membership_
            (
                _membership.Incarnation,
                _membership.Incremental,
                _members.ToArray()
            );
        }

        #endregion

        #region _Update

        private static bool _Update
        (
            ref QS.Fx.Value.Classes.IMembership<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _membership,
            QS._qss_x_.Properties_.Value_.Membership_ _membership_update,
            IncarnationClass _incarnation
        )
        {
            bool _modified_0 = false;
            foreach (QS._qss_x_.Properties_.Value_.Member_ _new_member in _membership_update.Members)
            {
                bool _modified = false;
                bool _add = true;
                List<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>> _members =
                    new List<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>();
                foreach (QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _old_member in _membership.Members)
                {
                    if (_old_member.Identifier.Equals(_new_member.Identifier))
                    {
                        if (_old_member.Incarnation.CompareTo((IncarnationClass)_new_member.Incarnation) > 0)
                        {
                            _add = false;
                            _members.Add(_old_member);
                        }
                        else
                            _modified = true;
                    }
                    else
                        _members.Add(_old_member);
                }
                if (_add && _new_member.Operational)
                {
                    _modified = true;
                    List<AddressClass> _addresses = new List<AddressClass>();
                    if (_new_member.Addresses != null)
                    {
                        foreach (QS.Fx.Serialization.ISerializable _address in _new_member.Addresses)
                            _addresses.Add((AddressClass)_address);
                    }
                    _members.Add
                    (
                        new QS.Fx.Value.Member<IdentifierClass, IncarnationClass, NameClass, AddressClass>
                        (
                            (IdentifierClass)_new_member.Identifier,
                            true,
                            (IncarnationClass)_new_member.Incarnation,
                            (NameClass)_new_member.Name,
                            _addresses.ToArray()
                        )
                    );
                }
                if (_modified)
                {
                    _membership = new QS.Fx.Value.Membership<
                        IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
                        (
                            _incarnation,
                            false,
                            _members.ToArray()
                        );
                    _modified_0 = true;
                }
            }
            return _modified_0;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Connection_

        public sealed class Connection_ 
            : QS._qss_x_.Properties_.Component_.Channel_<QS._qss_x_.Properties_.Value_.Membership_, QS._qss_x_.Properties_.Value_.Member_, 
                MembershipChannel_<IdentifierClass, IncarnationClass, NameClass, AddressClass>.Connection_>.Connection_
        {
            #region Constructor

            public Connection_()
                : base()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private bool _initialized;

            #endregion

            #region Accessors

            public bool _Initialized
            {
                get { return this._initialized; }
                set { this._initialized = value; }
            }

            #endregion
        }

        #endregion     

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
