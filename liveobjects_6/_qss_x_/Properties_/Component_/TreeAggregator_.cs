/* Copyright (c) 2004-2009 Jared Cantwell (jmc279@cornell.edu). All rights reserved.

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
SUCH DAMAGE. */

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TreeAggregator, "Properties Framework Aggregator (Tree)")]
    class TreeAggregator_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.Properties_.Component_.Tree_<QS._qss_x_.Properties_.Value_.AggregateToken_>,
        QS.Fx.Object.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>,
        QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public TreeAggregator_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("group", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference,
            [QS.Fx.Reflection.Parameter("fanout", QS.Fx.Reflection.ParameterClass.Value)]
            int _fanout,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _rate,
            [QS.Fx.Reflection.Parameter("MTTA", QS.Fx.Reflection.ParameterClass.Value)]
            double _mtta,
            [QS.Fx.Reflection.Parameter("MTTB", QS.Fx.Reflection.ParameterClass.Value)]
            double _mttb,
            [QS.Fx.Reflection.Parameter("aggregate_rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _agg_rate,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _group_reference, _fanout, _rate, _mtta, _mttb, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Aggregator_.Constructor");
#endif

            _endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, MessageClass>,
                    QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>>(this);

            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.ScheduleAlarm), null));
            this._agg_rate = _agg_rate;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, MessageClass>,
            QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>> _endpoint;

        [QS.Fx.Base.Inspectable]
        private double _agg_rate;
        [QS.Fx.Base.Inspectable]
        private BufferList _bufferlist = new BufferList();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm_disseminate;
        [QS.Fx.Base.Inspectable]
        private int _agg_index = 0;


        #endregion

        #region ScheduleAlarm

        private void ScheduleAlarm(object o)
        {
            this._alarm_disseminate = this._platform.AlarmClock.Schedule
            (
                1.0 / this._agg_rate,
                new QS.Fx.Clock.AlarmCallback
                (
                    delegate(QS.Fx.Clock.IAlarm _alarm)
                    {
                        if ((_alarm_disseminate != null) && !_alarm_disseminate.Cancelled && ReferenceEquals(_alarm_disseminate, _alarm))
                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Callback_Disemminate));
                    }
                ),
                null
            );
        }

        #endregion

        #region Buffer Classes

        class BufferList
        {
            public BufferList() { }

            private IList<BufferEntry> _buffers = new List<BufferEntry>();

            public void BeginBuffer(QS._qss_x_.Properties_.Value_.Round_ _round, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _members)
            {
                List<QS.Fx.Base.Identifier> _pending = new List<QS.Fx.Base.Identifier>();

                for (int i = 0; i < _members.Length; i++)
                    _pending.Add(_members[i].Identifier);

                _buffers.Add(new BufferEntry(_round, _pending, 0, 0));
            }

            public BufferEntry Get(QS._qss_x_.Properties_.Value_.Round_ _round)
            {
                foreach (BufferEntry _b in this._buffers)
                    if (_b._Round.Equals(_round))
                        return _b;

                return null;
            }

            public void Remove(QS._qss_x_.Properties_.Value_.Round_ _round)
            {
                _buffers.Remove(Get(_round));
            }


        }

        class BufferEntry
        {
            public BufferEntry(QS._qss_x_.Properties_.Value_.Round_ _round, IList<QS.Fx.Base.Identifier> _pending, long _begin_time, long _timeout)
            {
                this._round = _round;
                this._pending = _pending;
                this._begin_time = _begin_time;
                this._timeout = _timeout;
            }

            private QS._qss_x_.Properties_.Value_.Round_ _round;
            private IList<QS.Fx.Base.Identifier> _pending;
            private IList<MessageClass> _received = new List<MessageClass>();
            private long _begin_time;
            private long _timeout;

            public QS._qss_x_.Properties_.Value_.Round_ _Round
            {
                get { return this._round; }
            }

            public bool _IsReady
            {
                get { return this._pending.Count == 0; }
            }

            public bool _IsExpired
            {
                get { return false; }
            }

            public IList<MessageClass> _Messages
            {
                get { return _received; }
            }

            public bool Receive(QS.Fx.Base.Identifier _id, QS._qss_x_.Properties_.Value_.AggregateToken_ _message)
            {
                bool _found = false;
                QS.Fx.Base.Identifier _temp = null;

                foreach (QS.Fx.Base.Identifier id in this._pending)
                {
                    if (id.CompareTo(_id) == 0)
                    {
                        _found = true;
                        _temp = id;
                        this._pending.Remove(_temp);
                        this._received.Add((MessageClass)_message.Payload);
                        break;
                    }
                }

                return _found;
            }
        }

        #endregion

        protected override void _Tree_Incoming(QS.Fx.Base.Identifier _identifier, QS._qss_x_.Properties_.Value_.AggregateToken_ _message)
        {
            lock (this)
            {
                if (!this._IsRoot && _identifier.Equals(this._Parent.Identifier))
                {
                    _endpoint.Interface.Disseminating(_message.Round, (MessageClass)_message.Payload);

                    if (!this._IsLeaf)
                    {
                        foreach (QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _child in this._Children)
                        {
                            this._Tree_Outgoing(_child.Identifier, _message);
                        }

                        _bufferlist.BeginBuffer(_message.Round, this._Children);
                    }
                    else
                    {
                        this._endpoint.Interface.Aggregate(_message.Round, null);
                    }
                }
                else          // Assume its from a child
                {
                    BufferEntry _e = _bufferlist.Get(_message.Round);

                    if (_e == null)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_.Aggregator_._Tree_Incoming: BufferEntry not found for message:\n("
                                + QS.Fx.Printing.Printable.ToString(_message) + " \n)");
#endif
                        // LOG A MESSAGE
                        // I GOT A MESSAGE THAT I'M NOT PREPARED FOR
                    }
                    else
                    {
                        _e.Receive(_identifier, _message);
                        if (_e._IsReady)
                        {
                            if (_endpoint.IsConnected)
                            {
                                _endpoint.Interface.Aggregate(_message.Round, _e._Messages);
                                _bufferlist.Remove(_message.Round);
                            }
                        }

                        // RUN BUFFER LIST CLEAN HERE
                    }
                }
            }
        }

        #region IAggregator<Round_,MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, MessageClass>, QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>> QS.Fx.Object.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>.Aggregator
        {
            get { return this._endpoint; }
        }

        #endregion

        #region IAggregator<Round_,MessageClass> Members

        void QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>.Aggregate(QS._qss_x_.Properties_.Value_.Round_ _round, MessageClass _message)
        {
            if (!this._IsRoot)
            {
                QS._qss_x_.Properties_.Value_.AggregateToken_ _t =
                            new QS._qss_x_.Properties_.Value_.AggregateToken_(_round, _message);

                this._Tree_Outgoing(_Parent.Identifier, _t);
            }
        }

        void QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, MessageClass>.Disseminate(QS._qss_x_.Properties_.Value_.Round_ _round, MessageClass _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Aggregator_.Disseminate ( " +
                    QS.Fx.Printing.Printable.ToString(_round) + " ) : disseminate called on Aggregator. ");
#endif
            _endpoint.Interface.Disseminating(_round, _message);

            // No dissemination unless you are the root.
            if (this._IsRoot)
            {
                if (this._IsLeaf)
                {
                    _endpoint.Interface.Aggregate(_round, null);
                }
                else
                {
                    foreach (QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _child in this._Children)
                    {
                        QS._qss_x_.Properties_.Value_.AggregateToken_ _t =
                                new QS._qss_x_.Properties_.Value_.AggregateToken_(_round, _message);

                        this._Tree_Outgoing(_child.Identifier, _t);
                    }

                    _bufferlist.BeginBuffer(_round, this._Children);
                }
            }
        }

        #endregion

        private void _Callback_Disemminate(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            if (this._IsRoot)
            {
                if (this._endpoint != null && this._endpoint.IsConnected)
                {
                    _agg_index++;
                    QS._qss_x_.Properties_.Value_.Round_ _r = new QS._qss_x_.Properties_.Value_.Round_(
                        this._Group_Incarnation, new QS.Fx.Base.Index(_agg_index));

                    this._endpoint.Interface.Phase(_r);
                }
            }

            this._alarm_disseminate.Reschedule();
        }
    }
}
