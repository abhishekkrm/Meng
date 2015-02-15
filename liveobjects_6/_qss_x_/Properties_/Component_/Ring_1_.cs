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
#define FATALERRORS

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    public abstract class Ring_1_<MessageClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Ring_1_
        (
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference,
            double _rate,
            double _mtta,
            double _mttb,
            string debug_ID,
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_.Constructor");
#endif
            this.debug_identifier = debug_ID;
            if (_group_reference == null)
                _mycontext.Error("Group reference cannot be NULL.");
            if (_rate <= 0)
                _mycontext.Error("Rate must be positive.");
            if ((_mtta <= 0) || (_mttb <= 0))
                _mycontext.Error("Timeout must be positive.");
            this._rate = _rate;
            this._mtta = _mtta;
            this._mttb = _mttb;
            this._group_reference = _group_reference;
            this._group_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>>(this);
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
        }

        #endregion

        #region Fields

        private string debug_identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable> _group_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _group_connection;
        [QS.Fx.Base.Inspectable]
        private double _rate;
        [QS.Fx.Base.Inspectable]
        private double _mtta;
        [QS.Fx.Base.Inspectable]
        private double _mttb;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _incarnation;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _group_incarnation;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Name _name;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMembership<
            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _group_members;
        [QS.Fx.Base.Inspectable]
        private bool _ismember;
        [QS.Fx.Base.Inspectable]
        private bool _issingleton;
        [QS.Fx.Base.Inspectable]
        private bool _isleader;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _leader;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _predecessor;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _successor;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm1;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm2;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm3;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm4;
        [QS.Fx.Base.Inspectable]
        private bool _predecessor_ok;
        [QS.Fx.Base.Inspectable]
        private bool _successor_ok;
        [QS.Fx.Base.Inspectable]
        private bool _leader_ok;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index _index;
        [QS.Fx.Base.Inspectable]
        QS._qss_x_.Properties_.Value_.MessageToken_ _incoming_token;
        [QS.Fx.Base.Inspectable]
        QS._qss_x_.Properties_.Value_.MessageToken_ _outgoing_token;
#if STATISTICS
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_index = new QS._qss_c_.Statistics_.Samples2D(
            "number of rounds", "number of rounds as a function of time", "time", "s", "time in seconds", "number of rounds", "rounds", "number of rounds");
#endif

        #endregion

        #region Accessors

        protected QS.Fx.Base.Identifier _Identifier
        {
            get { return this._identifier; }
        }

        protected QS.Fx.Base.Incarnation _Incarnation
        {
            get { return this._incarnation; }
        }

        protected QS.Fx.Base.Name _Name
        {
            get { return this._name; }
        }

        protected QS.Fx.Base.Incarnation _Group_Incarnation
        {
            get { return this._group_incarnation; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _Group_Members
        {
            get { return this._group_members; }
        }

        protected bool _IsMember
        {
            get { return this._ismember; }
        }

        protected bool _IsSingleton
        {
            get { return this._issingleton; }
        }

        protected bool _IsLeader
        {
            get { return this._isleader; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _Leader
        {
            get { return this._leader; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _Successor
        {
            get { return this._successor; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _Predecessor
        {
            get { return this._predecessor; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IGroupClient<QS.Fx.Base.Identifier,QS.Fx.Base.Incarnation,QS.Fx.Base.Name,QS.Fx.Serialization.ISerializable> Members

        void QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>.Membership(
            QS.Fx.Value.Classes.IMembership<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<
                    QS.Fx.Value.Classes.IMembership<
                        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>>(
                            new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Membership), _membership));
        }

        void QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>.Message(
            QS.Fx.Base.Identifier _identifier, QS.Fx.Serialization.ISerializable _message)
        {
            if (!(_message is QS._qss_x_.Properties_.Value_.MessageToken_))
                _mycontext.Error("Received a message of an unknown type.");
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.MessageToken_>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Incoming), _identifier, (QS._qss_x_.Properties_.Value_.MessageToken_)_message));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected unsafe override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                if (this._platform != null)
                {
                    this._name = new QS.Fx.Base.Name(_platform.Network.GetHostName());
                    byte[] _a = _platform.Network.Interfaces[0].InterfaceAddress.GetAddressBytes();
                    ulong _n = 0;
                    for (int _i = 0; _i < 4; _i++)
                    {
                        byte _b = _a[_i];
                        _n = _n << 8;
                        if (_b >= 100)
                        {
                            byte _k = (byte)(_b / 100);
                            _n |= _k;
                            _b = (byte)(_b - _k * 100);
                        }
                        _n = _n << 4;
                        if (_b >= 10)
                        {
                            byte _k = (byte)(_b / 10);
                            _n |= _k;
                            _b = (byte)(_b - _k * 10);
                        }
                        _n = _n << 4;
                        if (_b >= 1)
                        {
                            byte _k = (byte)(_b);
                            _n |= _k;
                            _b = (byte)(_b - _k * 1);
                        }
                    }
                    //this._identifier = new QS.Fx.Base.Identifier(_n, (ulong)(new Random()).Next());
                    this._identifier = new QS.Fx.Base.Identifier(_n, _n);
                }
                else
                {
                    this._name = new QS.Fx.Base.Name("unnamed");
                    this._identifier = new QS.Fx.Base.Identifier(Guid.NewGuid());
                }
                this._incarnation = new QS.Fx.Base.Incarnation(1U);

                this._group_object = _group_reference.Dereference(_mycontext);
                if (this._group_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Start(this._platform, null);
                this._group_connection = this._group_endpoint.Connect(this._group_object.Group);

                this._index = new QS.Fx.Base.Index(0);
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Dispose");
#endif

            lock (this)
            {
                if ((this._group_object != null) && (this._group_object is IDisposable))
                    ((IDisposable)this._group_object).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._group_object != null) && (this._group_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Start(this._platform, null);
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Stop");
#endif

            lock (this)
            {
                if ((this._group_object != null) && (this._group_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Stop();
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
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Group_Connect");
#endif

            lock (this)
            {
                if (this._group_endpoint.IsConnected)
                    this._group_endpoint.Interface.Register(this._identifier, this._incarnation, this._name);
            }
        }

        #endregion

        #region _Group_Disconnect

        private void _Group_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Group_Disconnect");
#endif
        }

        #endregion

        protected abstract void _Ring_Membership(QS.Fx.Value.Classes.IMembership<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership);

        protected abstract void _Ring_Incoming(QS.Fx.Base.Identifier _identifier, MessageClass _message);


        // S/U will call this to send a message to someone through the tree
        protected void _Ring_Outgoing(QS.Fx.Base.Identifier _identifier, MessageClass _message)
        {
            // The tree needs to wrap this message into whatever it needs around it
            QS._qss_x_.Properties_.Value_.MessageToken_ _t =
                new QS._qss_x_.Properties_.Value_.MessageToken_(new QS.Fx.Base.Incarnation(0),
                                                                new QS.Fx.Base.Index(0),
                                                                _message);

            this._Group_Outgoing(_identifier, _t);
        }

        #region _Group_Incoming

        private void _Group_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {


            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.MessageToken_> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.MessageToken_>)_event;
            QS.Fx.Base.Identifier _identifier = _event_._Object1;
            QS._qss_x_.Properties_.Value_.MessageToken_ _message = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "__________________RING: GROUP INCOMING ( " +
                    QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
#endif
#if VERBOSE
            if (this._logger != null)
            {
                if (this._predecessor.Identifier.Equals(_identifier))
                {
                    if (this._Leader.Identifier.Equals(_identifier))
                        this._logger.Log(this.debug_identifier + " RING RECEIVING MESSAGE FROM LEADER && PREDECESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                    else if(this._IsLeader)
                        this._logger.Log(this.debug_identifier + " RING IS LEADER AND RECEIVING MESSAGE FROM PREDECESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                    else
                        this._logger.Log(this.debug_identifier + " RING RECEIVING MESSAGE FROM PREDECESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                }
                if (this._successor.Identifier.Equals(_identifier))
                {
                    if (this._Leader.Identifier.Equals(_identifier))
                        this._logger.Log(this.debug_identifier + " RING RECEIVING MESSAGE FROM LEADER && SUCCESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                    else
                        this._logger.Log(this.debug_identifier + " RING RECEIVING MESSAGE FROM SUCCESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                }
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Group_Outgoing ( " +
                    QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
            }
#endif
            if (_message.Payload != null)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log(this.debug_identifier + "__________________RING: PASSING TO IMPLEMENTING CLASS");
#endif
                this._Ring_Incoming(_identifier, (MessageClass)_message.Payload);
            }

            else
            {

                if (_message.Index.CompareTo(new QS.Fx.Base.Index(0)) == 0)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log(this.debug_identifier + "__________________RING: Received ping from " + _identifier.String + ": Sending a return token");
#endif
                    //this._Group_Outgoing(_identifier, new QS._qss_x_.Properties_.Value_.MessageToken_(this._membership.Incarnation, new QS.Fx.Base.Index(0), null));
                }

                lock (this)
                {
                    if (this._ismember)
                    {
                        if (_message.Incarnation.Equals(this._membership.Incarnation))
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log(this.debug_identifier + "__________________RING with ID " +
                                    QS.Fx.Printing.Printable.ToString(_identifier) + ": correct token incarnation, received " +
                                    _message.Incarnation.ToString() + ", expected " + _membership.Incarnation.ToString());
#endif
                            bool _got_predecessor = _identifier.Equals(_predecessor.Identifier);
                            bool _got_successor = _identifier.Equals(_successor.Identifier);

                            if (_got_predecessor)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log(this.debug_identifier + "__________________RING with ID " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + ": received a message from the predecessor");
#endif
                                this._predecessor_ok = true;
 
                                if (this._isleader)
                                {
#if VERBOSE
                                    if (this._logger != null)
                                        this._logger.Log(this.debug_identifier + "__________________RING sending return message to predecessor with incarnation: " + this._membership.Incarnation.String + " and index " + _message.Index.String);
#endif
                                    this._Group_Outgoing(_identifier, new QS._qss_x_.Properties_.Value_.MessageToken_(this._membership.Incarnation, _message.Index, null));
#if VERBOSE
                                    if (this._logger != null)
                                        this._logger.Log(this.debug_identifier + "__________________RING with ID " +
                                            QS.Fx.Printing.Printable.ToString(_identifier) + " : I am leader. message index=" +
                                            _message.Index.String + ", my index= " + this._index);
#endif
                                    if (_message.Index.Equals(this._index))
                                    {
                                        this._incoming_token = _message;
                                        this._Process_3();

                                        if (_alarm1 != null)
                                            _alarm1.Reschedule();
                                        else
                                            _mycontext.Error("The member does not have its alarm set.");
                                    }
                                }
                                else
                                {
#if VERBOSE
                                    if (this._logger != null)
                                        this._logger.Log(this.debug_identifier + "__________________RING wth ID " +
                                            QS.Fx.Printing.Printable.ToString(_identifier) + ": I am NOT leader. message index=" +
                                            _message.Index.String + ", my index= " + this._index);
#endif
                                    if (_message.Index.CompareTo(this._index) > 0)
                                    {
#if VERBOSE
                                        if (this._logger != null)
                                            this._logger.Log(this.debug_identifier + "__________________RING sending return message to predecessor with incarnation: " + this._membership.Incarnation.String + " and index " + _message.Index.String);
#endif
                                        this._Group_Outgoing(_identifier, new QS._qss_x_.Properties_.Value_.MessageToken_(this._membership.Incarnation, _message.Index, null));
                                        this._index = _message.Index;
#if VERBOSE
                                        if (this._logger != null)
                                            this._logger.Log(this.debug_identifier + "RING ( " +
                                                QS.Fx.Printing.Printable.ToString(_identifier) + " ) : I am NOT leader. index set. message index=" +
                                                _message.Index.String + ", my index= " + this._index);
#endif
#if STATISTICS
                                        if ((this._statistics_index != null) && (this._platform != null))
                                            this._statistics_index.Add(this._platform.Clock.Time, (double)((uint)this._index));
#endif

                                        this._leader_ok = true;
                                        this._incoming_token = _message;

                                        this._Process_2();
                                        if (this._outgoing_token != null)
                                            this._Group_Outgoing();
                                        else
                                            _mycontext.Error("The member did not generate any token.");
                                    }
                                }
                            }

                            if (_got_successor)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log(this.debug_identifier + "__________________RING with ID " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + ": received a message from the successor");
#endif
                                this._successor_ok = true;
                            }


                            if (!_got_predecessor && !_got_successor)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log(this.debug_identifier + "__________________RING with ID " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + ": not a predecessor or successor, ignoring");
#endif
                            }
                        }
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log(this.debug_identifier + "__________________RING with ID " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + ": wrong token incarnation, received " +
                                    _message.Incarnation.ToString() + ", expected " + _membership.Incarnation.ToString() + "; token ignored");
#endif
                        }
                    }
                }
            }
        }

        #endregion

        #region _Group_Outgoing

        private void _Group_Outgoing(QS.Fx.Base.Identifier _identifier, QS._qss_x_.Properties_.Value_.MessageToken_ _message)
        {
#if VERBOSE
            if (this._logger != null)
            {
                if (this._predecessor.Identifier.Equals(_identifier))
                {
                    this._logger.Log(this.debug_identifier + " RING SENDING MESSAGE TO PREDECESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                }
                else if (this._successor.Identifier.Equals(_identifier))
                {
                    if(this._Leader.Identifier.Equals(_identifier))
                        this._logger.Log(this.debug_identifier + " RING SENDING MESSAGE TO LEADER && SUCCESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                    else
                        this._logger.Log(this.debug_identifier + " RING SENDING MESSAGE TO SUCCESSOR with incarnation " + _message.Incarnation.String + " and index " + _message.Index.String);
                }
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Group_Outgoing ( " +
                    QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
            }
#endif

            if (this._group_endpoint.IsConnected)
                this._group_endpoint.Interface.Message(_identifier, _message);
        }

        private void _Group_Outgoing()
        {
            this._successor_ok = false;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "^^^^^^^^^^^^^^^^^^^^^RING with ID " +
                        QS.Fx.Printing.Printable.ToString(_identifier) + ":CANCELLING ALARM 4 ");
#endif
            if (this._alarm4 != null)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log(this.debug_identifier + "^^^^^^^^^^^^^^^^^^^^^RING with ID " +
                            QS.Fx.Printing.Printable.ToString(_identifier) + ":CANCELLED ALARM 4 ");
#endif
                this._alarm4.Cancel();
                this._alarm4 = null;
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "^^^^^^^^^^^^^^^^^^^^^RING with ID " +
                        QS.Fx.Printing.Printable.ToString(_identifier) + ":RESETTING ALARM 4 for " + this._mtta + " seconds later.");
#endif
            this._alarm4 = this._platform.AlarmClock.Schedule
            (
                this._mtta,
                new QS.Fx.Clock.AlarmCallback
                (
                    delegate(QS.Fx.Clock.IAlarm _alarm)
                    {
                        if ((_alarm4 != null) && !_alarm4.Cancelled && ReferenceEquals(_alarm4, _alarm))
                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_4));
                    }
                ),
                null
            );

            this._Group_Outgoing(this._successor.Identifier, this._outgoing_token);
        }

        #endregion

        #region _Group_Membership

        private void _Group_Membership(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IMembership<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership =
                    ((QS._qss_x_.Properties_.Base_.IEvent_<
                        QS.Fx.Value.Classes.IMembership<
                            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Group_Membership\n\n" + QS.Fx.Printing.Printable.ToString(_membership) + "\n\n");
#endif

            lock (this)
            {
                this._membership = _membership;
                this._group_incarnation = _membership.Incarnation;
                this._group_members =
                   new List<QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>(_membership.Members).ToArray();
                QS.Fx.Value.Classes.IMember<
                    QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _leader = null;
                QS.Fx.Value.Classes.IMember<
                    QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _predecessor = null;
                QS.Fx.Value.Classes.IMember<
                    QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _successor = null;
                bool _isleader = false;
                bool _gotme = false;
                bool _issucc = false;
                bool _issingleton = false;
                foreach (QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _memb in _membership.Members)
                {
                    if (_issucc)
                    {
                        _successor = _memb;
                        _issucc = false;
                    }
                    if (_memb.Identifier.Equals(this._identifier) && _memb.Incarnation.Equals(this._incarnation))
                    {
                        _gotme = true;
                        _issucc = true;
                    }
                    if (_leader == null)
                    {
                        _leader = _memb;
                        if (_gotme)
                            _isleader = true;
                    }
                    if (_isleader || !_gotme)
                        _predecessor = _memb;
                }
                if (_issucc)
                {
                    _successor = _leader;
                    if (_isleader)
                        _issingleton = true;
                }

                if (this._alarm1 != null)
                {
                    this._alarm1.Cancel();
                    this._alarm1 = null;
                }

                if (this._alarm2 != null)
                {
                    this._alarm2.Cancel();
                    this._alarm2 = null;
                }

                if (this._alarm3 != null)
                {
                    this._alarm3.Cancel();
                    this._alarm3 = null;
                }

                if (this._alarm4 != null)
                {
                    this._alarm4.Cancel();
                    this._alarm4 = null;
                }

                this._incoming_token = null;
                this._outgoing_token = null;

                if (_gotme)
                {
                    this._ismember = true;
                    this._issingleton = _issingleton;
                    this._isleader = _isleader;
                    this._leader = _leader;
                    this._predecessor = _predecessor;
                    this._successor = _successor;

#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log(this.debug_identifier + 
                            "RING RECONFIGURE (enter)\n\n_issingleton = " + _issingleton.ToString() +
                            "\n_isleader = " + _isleader.ToString() +
                            "\n_leader = " + QS.Fx.Printing.Printable.ToString(_leader) +
                            "\n_predecessor = " + QS.Fx.Printing.Printable.ToString(_predecessor) +
                            "\n_successor = " + QS.Fx.Printing.Printable.ToString(_successor) + "\n\n");
#endif
                    this._index = new QS.Fx.Base.Index(0);
                    if (_isleader)
                    {
                        this._index = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>)this._index).Incremented;
#if STATISTICS
                        if ((this._statistics_index != null) && (this._platform != null))
                            this._statistics_index.Add(this._platform.Clock.Time, (double)((uint)this._index));
#endif

                        if (_issingleton)
                            _Process_0();
                        else
                        {
                           /* _Process_1();
                            //Might not want leader to try and do this right away.
                            if (this._outgoing_token != null)
                                this._Group_Outgoing();
                            else
                                _mycontext.Error("The member did not generate any token.");*/
                            //Rate at which leader will send out tokens.
                            this._alarm1 = this._platform.AlarmClock.Schedule
                            (
                                (1 / this._rate),
                                new QS.Fx.Clock.AlarmCallback
                                (
                                    delegate(QS.Fx.Clock.IAlarm _alarm)
                                    {
                                        if ((_alarm1 != null) && !_alarm1.Cancelled && ReferenceEquals(_alarm1, _alarm))
                                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_1));
                                    }
                                ),
                                null
                            );
                        }
                    }
                    else
                    {
                        //Setting an alarm to check if the leader is still ok.
                        this._leader_ok = false;
                        this._alarm2 = this._platform.AlarmClock.Schedule
                        (
                            //(2 / this._rate),
                            this._mttb,
                            new QS.Fx.Clock.AlarmCallback
                            (
                                delegate(QS.Fx.Clock.IAlarm _alarm)
                                {
                                    if ((_alarm2 != null) && !_alarm2.Cancelled && ReferenceEquals(_alarm2, _alarm))
                                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_2));
                                }
                            ),
                            null
                        );
                    }
                 
                    if (!this._issingleton)
                    {
                        //Alarm for checking if predecessor is ok.
                        this._alarm3 = this._platform.AlarmClock.Schedule
                        (
                            this._mttb,
                            new QS.Fx.Clock.AlarmCallback
                            (
                                delegate(QS.Fx.Clock.IAlarm _alarm)
                                {
                                    if ((_alarm3 != null) && !_alarm3.Cancelled && ReferenceEquals(_alarm3, _alarm))
                                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_3));
                                }
                            ),
                            null
                        );
                        this._predecessor_ok = false;
                        
                        if (!this._isleader)
                        {
                            this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._membership.Incarnation, new QS.Fx.Base.Index(0), null);
                            //this._Group_Outgoing(this._successor.Identifier, this._outgoing_token);
                        }
                    }
                }
                else
                {
                    this._ismember = false;
                    this._issingleton = false;
                    this._isleader = false;
                    this._leader = null;
                    this._predecessor = null;
                    this._successor = null;
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log(this.debug_identifier + "Component_.Ring_._Reconfigure (leave)");
#endif
                }
            }
            this._Ring_Membership(_membership);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Callback_1

        private void _Callback_1(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "************************RING CALLBACK1 RESTARTING TOKEN PASSING");
#endif

            lock (this)
            {
                if (this._ismember)
                {
                    if (this._isleader)
                    {
                        this._index = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>)this._index).Incremented;
#if STATISTICS
                        if ((this._statistics_index != null) && (this._platform != null))
                            this._statistics_index.Add(this._platform.Clock.Time, (double)((uint)this._index));
#endif

                        if (_issingleton)
                        {
                            _Process_0();

                            if (_alarm1 != null)
                                _alarm1.Reschedule();
                            else
                                _mycontext.Error("The member does not have its alarm set.");
                        }
                        else
                        {
                            _Process_1();

                            if (this._outgoing_token != null)
                                this._Group_Outgoing();
                            else
                                _mycontext.Error("The member did not generate any token.");
                        }
                    }
                }
            }
        }

        #endregion

        #region _Callback_2

        private void _Callback_2(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "************************RING CALLBACK2 CHECKING IF LEADER IS OK");
#endif

            lock (this)
            {
                if (this._ismember && !this._issingleton)
                {
                    if (_leader_ok)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log(this.debug_identifier + "************************RING CALLBACK2 : LEADER OK");
#endif

                        this._leader_ok = false;
                        this._alarm2.Reschedule();
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log(this.debug_identifier + "************************RING CALLBACK2 : LEADER FAILED ( " + this._leader.Identifier.ToString() + ", " +
                                this._leader.Incarnation.ToString() + " )");
#endif

                        this._group_endpoint.Interface.Unregister(this._leader.Identifier, this._leader.Incarnation);
                        this._alarm2 = null;
                    }
                }
                else
                    this._alarm2 = null;
            }
        }

        #endregion

        #region _Callback_3

        private void _Callback_3(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "************************RING CALLBACK3: CHECKING FOR PREDECESSOR");
#endif

            lock (this)
            {
                if (this._ismember)
                {
                    if (!this._issingleton)
                    {
                        if (_predecessor_ok)
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log(this.debug_identifier + "************************RING CALLBACK3: PREDECESSOR OK");
#endif
                        }
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log(this.debug_identifier + "************************RING CALLBACK3: PREDECESSOR FAILED ( " + this._predecessor.Identifier.ToString() + ", " +
                                    this._predecessor.Incarnation.ToString() + " )");
#endif

                            this._group_endpoint.Interface.Unregister(this._predecessor.Identifier, this._predecessor.Incarnation);
                        }
                    }
                }

                this._alarm3 = null;
            }
        }

        #endregion

        #region _Callback_4

        private void _Callback_4(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "************************RING CALLBACK4: CHECKING IF SUCCESSOR IS OK");
#endif

            lock (this)
            {
                if (this._ismember)
                {
                    if (!this._issingleton)
                    {
                        if (_successor_ok)
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log(this.debug_identifier + "************************RING CALLBACK4: SUCCESSOR OK");
#endif
                        }
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log(this.debug_identifier + "************************RING CALLBACK4: SUCCESSOR FAILED ( " + this._successor.Identifier.ToString() + ", " +
                                    this._successor.Incarnation.ToString() + " )");
#endif

                            this._group_endpoint.Interface.Unregister(this._successor.Identifier, this._successor.Incarnation);
                        }
                    }
                }

                this._alarm4 = null;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Process_0

        private void _Process_0()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Process_0 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif
        }

        #endregion

        #region _Process_1

        private void _Process_1()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Process_1 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif

            this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._membership.Incarnation, this._index, null);

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        #region _Process_2

        private void _Process_2()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Process_2 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif

            this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._membership.Incarnation, this._index, null);
        }

        #endregion

        #region _Process_3

        private void _Process_3()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this.debug_identifier + "Component_.Ring_._Process_3 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
