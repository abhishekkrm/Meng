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
    //[QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Tree_0_, "Properties Framework Token Tree")]
    public abstract class Tree_<MessageClass>
        : QS._qss_x_.Properties_.Component_.GroupClient_<QS._qss_x_.Properties_.Value_.MessageToken_>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Tree_
        (
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference,
            int _fanout,
            double _rate,
            double _mtta,
            double _mttb,
            bool _debug
        )
            : base(_mycontext, _group_reference, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_.Constructor");
#endif

            if (_rate <= 0)
                _mycontext.Error("Rate must be positive.");
            if ((_mtta <= 0) || (_mttb <= 0))
                _mycontext.Error("Timeout must be positive.");
            this._fanout = _fanout;
            this._rate = _rate;
            this._mtta = _mtta;
            this._mttb = _mttb;
            //this._index = new QS.Fx.Base.Index(0);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private int _fanout;
        [QS.Fx.Base.Inspectable]
        private double _rate;
        [QS.Fx.Base.Inspectable]
        private double _mtta;
        [QS.Fx.Base.Inspectable]
        private double _mttb;
        [QS.Fx.Base.Inspectable]
        private bool _isroot;
        [QS.Fx.Base.Inspectable]
        private bool _isleaf;
        [QS.Fx.Base.Inspectable]
        private int _depth;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _root;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _parent;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _children;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm_parent_ping;
        //private QS.Fx.Clock.IAlarm _alarm_leader_ping;
        //[QS.Fx.Base.Inspectable]
        //private QS.Fx.Clock.IAlarm _alarm_leader_ok;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm_children_ok;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm_parent_ok;
        [QS.Fx.Base.Inspectable]
        private bool[] _children_ok;
        [QS.Fx.Base.Inspectable]
        private bool _parent_ok;
        //[QS.Fx.Base.Inspectable]
        //private bool _leader_ok;
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


        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _Parent
        {
            get { return this._parent; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _Children
        {
            get { return this._children; }
        }

        protected bool _IsRoot
        {
            get { return this._isroot; }
        }

        protected bool _IsLeaf
        {
            get { return this._isleaf; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected unsafe override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Initialize");
#endif

            this._index = new QS.Fx.Base.Index(0);

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Dispose");
#endif

            base._Dispose();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        protected abstract void _Tree_Incoming(QS.Fx.Base.Identifier _identifier, MessageClass _message);


        // S/U will call this to send a message to someone through the tree
        protected void _Tree_Outgoing(QS.Fx.Base.Identifier _identifier, MessageClass _message)
        {
            // The tree needs to wrap this message into whatever it needs around it
            QS._qss_x_.Properties_.Value_.MessageToken_ _t =
                new QS._qss_x_.Properties_.Value_.MessageToken_(new QS.Fx.Base.Incarnation(0),
                                                                new QS.Fx.Base.Index(0),
                                                                _message);

            this._Group_Outgoing(_identifier, _t);
        }

        #region _Group_Incoming

        protected override void _Group_Incoming(QS.Fx.Base.Identifier _identifier, QS._qss_x_.Properties_.Value_.MessageToken_ _message)
        {
            if (_message.Payload != null)
            {
                this._Tree_Incoming(_identifier, (MessageClass)_message.Payload);
            }

            else
            {
                lock (this)
                {
                    // Send an ACK to the sender
                    if (_message.Index.CompareTo(new QS.Fx.Base.Index(0)) > 0)
                        this._Group_Outgoing(_identifier, new QS._qss_x_.Properties_.Value_.MessageToken_(this._Group_Incarnation, new QS.Fx.Base.Index(0), null));

                    if (this._IsMember)
                    {
                        if (_message.Incarnation.Equals(this._Group_Incarnation))
                        {
                            bool _got_parent = false;
                            if (_parent != null)
                                _got_parent = _identifier.Equals(_parent.Identifier);

                            int _got_child = -1;

                            if (!this._isleaf)
                            {
                                for (int i = 0; i < _children.Length; i++)
                                    if (_identifier.Equals(_children[i].Identifier))
                                        _got_child = i;
                            }

                            if (_got_parent)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log("Component_.Tree_._Group_Incoming ( " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + " ) : received a message from the parent");
#endif

                                this._parent_ok = true;

                                /*
                                if (!_isleaf && _message.Index.CompareTo(this._index) > 0)
                                {
                                    this._index = _message.Index;
                                    //#if STATISTICS
                                    //                                    if ((this._statistics_index != null) && (this._platform != null))
                                    //                                        this._statistics_index.Add(this._platform.Clock.Time, (double)((uint)this._index));
                                    //#endif
                                    
                                    this._incoming_token = _message;

                                    //this._Process_2();
                                    this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._Group_Incarnation, this._index, null);

                                    if (this._outgoing_token != null)
                                        this._Group_Outgoing();
                                    else
                                        _mycontext.Error("The member did not generate any token.");
                                     
                                }
                                 * * */
                            }

                            if (_got_child >= 0)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log("Component_.Tree_._Group_Incoming ( " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + " ) : received a message from child " + QS.Fx.Printing.Printable.ToString(_children[_got_child].Identifier));
#endif
                                this._children_ok[_got_child] = true;
                            }


                            if (!_got_parent && _got_child < 0)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log("Component_.Tree_._Group_Incoming ( " +
                                        QS.Fx.Printing.Printable.ToString(_identifier) + " ) : not a parent or child, ignoring");
#endif
                            }
                        }
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Tree_._Group_Incoming ( " +
                                    QS.Fx.Printing.Printable.ToString(_identifier) + " ) : wrong token incarnation, received " +
                                    _message.Incarnation.ToString() + ", expected " + this._Group_Incarnation.ToString() + "; token ignored.");
#endif
                        }
                    }
                }
            }
        }

        #endregion

        #region _Group_Outgoing

        private void _Group_Outgoing()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Group_Outgoing (set _alarm_children_ok)");
#endif

            if (this._alarm_children_ok == null)
            {
                for (int i = 0; i < _children_ok.Length; i++)
                    _children_ok[i] = false;

                this._alarm_children_ok = this._platform.AlarmClock.Schedule
                (
                    _mtta,
                    new QS.Fx.Clock.AlarmCallback
                    (
                        delegate(QS.Fx.Clock.IAlarm _alarm)
                        {
                            if ((_alarm_children_ok != null) && !_alarm_children_ok.Cancelled && ReferenceEquals(_alarm_children_ok, _alarm))
                                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Children_Ok));
                        }
                    ),
                    null
                );
            }

            for(int i = 0; i < _children.Length; i++)
                this._Group_Outgoing(this._children[i].Identifier, this._outgoing_token);
        }

        #endregion

        #region _Group_Reconfigure

        protected override void  _Group_Reconfigure()
        {
            lock (this)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Tree_._Reconfigure (enter)");
#endif

                this._isroot = this._IsLeader;
                this._root = this._Leader;
                //this._leader_ok = false;

                if (_alarm_parent_ping != null)
                {
                    _alarm_parent_ping.Cancel();
                    _alarm_parent_ping = null;
                }
                /*
                if (_alarm_leader_ok != null)
                {
                    _alarm_leader_ok.Cancel();
                    _alarm_leader_ok = null;
                }
                 * */
                if (_alarm_children_ok != null)
                {
                    _alarm_children_ok.Cancel();
                    _alarm_children_ok = null;
                }
                if (_alarm_parent_ok != null)
                {
                    _alarm_parent_ok.Cancel();
                    _alarm_parent_ok = null;
                }

                if (this._IsMember)
                {
                    // If I am not the first rank, then I will have a parent
                    if (this._Rank > 0)
                    {
                        int _parent = (this._Rank - 1) / this._fanout;
                        this._parent = this._Group_Members[_parent];
                    }
                    else
                        this._parent = null;

                    this._parent_ok = false;

                    // Setup my depth for timeout reasons
                    int index = this._Rank;
                    this._depth = 0;

                    while (index > 0)
                    {
                        this._depth++;
                        index = (index - 1) / this._fanout;
                    }


                    int _children = this._fanout * this._Rank + 1;
                    int _nchildren = Math.Min(Math.Max(this._Group_Members.Length - _children, 0), this._fanout);
                    if (_nchildren > 0)
                    {
                        this._isleaf = false;
                        this._children = new QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[_nchildren];
                        this._children_ok = new bool[_nchildren];

                        for (int _ind = 0; _ind < _nchildren; _ind++)
                        {
                            this._children_ok[_ind] = false;
                            this._children[_ind] = this._Group_Members[_children + _ind];
                        }
                    }
                    else
                    {
                        this._isleaf = true;
                        this._children = null;
                    }

                    if (!this._IsSingleton)
                    {
                        if (!this._isleaf)
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Tree_._Reconfigure (set _alarm_parent_ping)");
#endif
                            this._alarm_parent_ping = this._platform.AlarmClock.Schedule
                            (
                                (1 / this._rate),
                                new QS.Fx.Clock.AlarmCallback
                                (
                                    delegate(QS.Fx.Clock.IAlarm _alarm)
                                    {
                                        if ((_alarm_parent_ping != null) && !_alarm_parent_ping.Cancelled && ReferenceEquals(_alarm_parent_ping, _alarm))
                                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Parent_Ping));
                                    }
                                ),
                                null
                            );
                        }

                        if (!this._IsLeader)
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Tree_._Reconfigure (set _alarm_parent_ok)");
#endif
                            this._alarm_parent_ok = this._platform.AlarmClock.Schedule
                            (
                                this._mttb,
                                new QS.Fx.Clock.AlarmCallback
                                (
                                    delegate(QS.Fx.Clock.IAlarm _alarm)
                                    {
                                        if ((_alarm_parent_ok != null) && !_alarm_parent_ok.Cancelled && ReferenceEquals(_alarm_parent_ok, _alarm))
                                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Parent_Ok));
                                    }
                                ),
                                null
                            );
                        }
                    }
                }

                //if (_myendpoint.IsConnected)
                //{
                //    QS.Fx.Base.TreeConfiguration _config = new QS.Fx.Base.TreeConfiguration(
                //        this._IsLeader, this._IsLeader, this._parent, this._children);
                //    _myendpoint.Interface.Reconfigure(_config);
                //}




/*
                if (this._IsLeader)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Tree_._Reconfigure (set _alarm_leader_ping)");
#endif
                    this._alarm_leader_ping = this._platform.AlarmClock.Schedule
                    (
                        (1 / this._rate),
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                if ((_alarm_leader_ping != null) && !_alarm_leader_ping.Cancelled && ReferenceEquals(_alarm_leader_ping, _alarm))
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Leader_Ping));
                            }
                        ),
                        null
                    );
                }

                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Tree_._Reconfigure (set _alarm_leader_ok)");
#endif
                    this._alarm_leader_ok = this._platform.AlarmClock.Schedule
                    (
                        this._mttb,
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                if ((_alarm_leader_ok != null) && !_alarm_leader_ok.Cancelled && ReferenceEquals(_alarm_leader_ok, _alarm))
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Leader_Ok));
                            }
                        ),
                        null
                    );

                    */

                    /*
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Tree_._Reconfigure (set _alarm_parent_ok)");
#endif
                    this._alarm_parent_ok = this._platform.AlarmClock.Schedule
                    (
                        this._mttb,
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                if ((_alarm_parent_ok != null) && !_alarm_parent_ok.Cancelled && ReferenceEquals(_alarm_parent_ok, _alarm))
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Parent_Ok));
                            }
                        ),
                        null
                    );
                }
                     * */
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        #region _Callback_Parent_Ping

        private void _Callback_Parent_Ping(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Callback_Parent_Ping");
#endif

            lock (this)
            {
                if (this._IsMember)
                {
                    if (!this._isleaf)
                    {
                        this._index = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>)this._index).Incremented;
#if STATISTICS
                        if ((this._statistics_index != null) && (this._platform != null))
                            this._statistics_index.Add(this._platform.Clock.Time, (double)((uint)this._index));
#endif

                        if (_IsSingleton)
                        {
                            //                            _Process_0();

                            //                            if (_alarm_leader_ping != null)
                            //                                _alarm_leader_ping.Reschedule();
                            //                            else
                            //                                _mycontext.Error("The member does not have its alarm set.");
                        }
                        else
                        {
                            //_Process_1();
                            this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._Group_Incarnation, this._index, null);

                            if (this._outgoing_token != null)
                                this._Group_Outgoing();
                            else
                                _mycontext.Error("The member did not generate any token.");
                        }

                        if (_alarm_parent_ping != null)
                            _alarm_parent_ping.Reschedule();
                        else
                            _mycontext.Error("The member does not have its alarm set.");
                    }
                }
            }
        }

        #endregion

        /*
        #region _Callback_Leader_Ping

        private void _Callback_Leader_Ping(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Callback_Leader_Ping");
#endif

            lock (this)
            {
                if (this._IsMember)
                {
                    if (this._IsLeader)
                    {
                        this._index = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>)this._index).Incremented;
#if STATISTICS
                        if ((this._statistics_index != null) && (this._platform != null))
                            this._statistics_index.Add(this._platform.Clock.Time, (double)((uint)this._index));
#endif

                        if (_IsSingleton)
                        {
//                            _Process_0();

//                            if (_alarm_leader_ping != null)
//                                _alarm_leader_ping.Reschedule();
//                            else
//                                _mycontext.Error("The member does not have its alarm set.");
                        }
                        else
                        {
                            _Process_1();

                            if (this._outgoing_token != null)
                                this._Group_Outgoing();
                            else
                                _mycontext.Error("The member did not generate any token.");
                        }

                        if (_alarm_leader_ping != null)
                            _alarm_leader_ping.Reschedule();
                        else
                            _mycontext.Error("The member does not have its alarm set.");
                    }
                }
            }
        }

        #endregion
        

        #region _Callback_Leader_Ok

        private void _Callback_Leader_Ok(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Callback_Leader_Ok");
#endif

            lock (this)
            {
                if (this._IsMember && !this._IsSingleton)
                {
                    if (_leader_ok)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Tree_._Callback_Leader_Ok : leader ok");
#endif

                        this._leader_ok = false;
                        this._alarm_leader_ok.Reschedule();
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Tree_._Callback_Leader_Ok : leader failed ( " + this._Leader.Identifier.ToString() + ", " +
                                this._Leader.Incarnation.ToString() + " )");
#endif

                        this._Group_Endpoint.Interface.Unregister(this._Leader.Identifier, this._Leader.Incarnation);
                        this._alarm_leader_ok = null;
                    }
                }
                else
                    this._alarm_leader_ok = null;
            }
        }

        #endregion
        */

        #region _Callback_Children_Ok

        private void _Callback_Children_Ok(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Callback_Children_Ok");
#endif

            lock (this)
            {
                if (this._IsMember && !this._IsSingleton && !this._isleaf)
                {
                    for (int i = 0; i < _children.Length; i++)
                    {
                        if (_children_ok[i])
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Ring_._Callback_Children_Ok : child " + i + " ok");
#endif
                            _children_ok[i] = false;
                        }
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.Ring_._Callback_Children_Ok : child " + i + " failed ( " + this._children[i].Identifier.ToString() + ", " +
                                    this._children[i].Incarnation.ToString() + " )");
#endif

                            this._Group_Endpoint.Interface.Unregister(this._children[i].Identifier, this._children[i].Incarnation);
                        }
                    }
                    this._alarm_children_ok.Reschedule();
                }
                else
                {
                    this._alarm_children_ok = null;
                }
            }
        }

        #endregion

        #region _Callback_Parent_Ok

        private void _Callback_Parent_Ok(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Ring_._Callback_Parent_Ok");
#endif

            lock (this)
            {
                if (this._IsMember && !this._IsSingleton)
                {
                    if (this._parent_ok)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Ring_._Callback_Parent_Ok : parent ok");
#endif
                        this._parent_ok = false;
                        this._alarm_parent_ok.Reschedule();
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Ring_._Callback_Parent_Ok : parent failed ( " + this._parent.Identifier.ToString() + ", " +
                                this._parent.Incarnation.ToString() + " )");
#endif

                        this._Group_Endpoint.Interface.Unregister(this._parent.Identifier, this._parent.Incarnation);
                        this._alarm_parent_ok = null;
                    }
                }
                else
                {
                    this._alarm_parent_ok = null;
                }
            }
        }

        #endregion

        /*
        #region _Process_1

        private void _Process_1()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Process_1 ( "
                    + QS.Fx.Printing.Printable.ToString(this._Group_Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif

            this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._Group_Incarnation, this._index, null);

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        #region _Process_2

        private void _Process_2()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Tree_._Process_2 ( "
                    + QS.Fx.Printing.Printable.ToString(this._Group_Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif

            this._outgoing_token = new QS._qss_x_.Properties_.Value_.MessageToken_(this._Group_Incarnation, this._index, null);
        }

        #endregion 
        */
    }
}
