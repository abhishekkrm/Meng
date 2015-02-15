/*

Copyright 2009, Jared Cantwell. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions 
   and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of 
   conditions and the following disclaimer in the documentation and/or other materials provided
  with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S) AND ALL OTHER CONTRIBUTORS 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE ABOVE 
COPYRIGHT HOLDER(S) OR ANY OTHER CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE. 
 
*/

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Orderer,
        "Properties Framework Message Ordering Component", "")]
    public sealed class Orderer<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass>
        : QS._qss_x_.Properties_.Component_.Base_, QS.Fx.Object.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>,
          QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>,
          QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable
    {

        public Orderer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Aggregator", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>> _agg_reference
        )
            : base(_mycontext, true)
        {
            if (_agg_reference != null)
            {
                this._agg_endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>,
                    QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>>(this);
                this._agg_connection = this._agg_endpoint.Connect(_agg_reference.Dereference(_mycontext).Aggregator);
            }
            else
                throw new Exception("Ordering Component requires the parameter 'Aggregator' to perform correctly.");

            this._myendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, IdentifierClass>,
                QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>>(this);
            this._myendpoint.OnConnected += new QS.Fx.Base.Callback(_myendpoint_OnConnected);

            _myGlobalIndex = 1;
            _log = this._logger;
        }

        void _myendpoint_OnConnected()
        {
            this._myendpoint.Interface.Request(new QS.Fx.Base.Index(0));
        }

        #region Fields
        private QS.Fx.Endpoint.IConnection _agg_connection;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>,
            QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>>
                    _agg_endpoint;

        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, IdentifierClass>,
                QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>> _myendpoint;

        private static QS.Fx.Base.Index ORDER_UNSET = new QS.Fx.Base.Index(-1);
        private static QS.Fx.Base.Index TYPE_BEGIN_AGG = new QS.Fx.Base.Index(1);
        private static QS.Fx.Base.Index TYPE_AGG_DECISION = new QS.Fx.Base.Index(2);
        private static QS.Fx.Base.Index TYPE_NEW_LEADER = new QS.Fx.Base.Index(3);
        private static QS.Fx.Base.Index TYPE_EPOCH_AGG = new QS.Fx.Base.Index(4);
        private static QS.Fx.Base.Index TYPE_EPOCH_END = new QS.Fx.Base.Index(5);

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _rootIncarnation = null;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Value_.Round_ _lastRound = null;
        [QS.Fx.Base.Inspectable]
        private int _nextOrder = 1;
        [QS.Fx.Base.Inspectable]
        private int _myGlobalIndex;
        [QS.Fx.Base.Inspectable]
        private MessageList_ _list = new MessageList_();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index _maxKnownOrder = new QS.Fx.Base.Index(0);

        private static QS.Fx.Logging.ILogger _log;

        private enum RoundType { AGGREGATE, DECISION, NEW_LEADER, EPOCH_AGGREGATE, EPOCH_END}
        private RoundType roundType = RoundType.AGGREGATE;

        #endregion

        #region MessageList_

        class MessageList_
        {

            public MessageList_()
            {
                _items = new List<QS._qss_x_.Properties_.Value_.OrderToken_>();
            }

            private List<QS._qss_x_.Properties_.Value_.OrderToken_> _items;

            public List<QS._qss_x_.Properties_.Value_.OrderToken_> Items
            {
                get { return _items; }
            }

            public QS._qss_x_.Properties_.Value_.OrderToken_ Get(IdentifierClass _id)
            {
                foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in this._items)
                {
                    if (_id.Equals(_o.Payload))
                        return _o;
                }

                return null;
            }

            public QS._qss_x_.Properties_.Value_.OrderToken_ GetFromOrder(QS.Fx.Base.Index _order)
            {
                foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in this._items)
                {
                    if (_o.Order.Equals(_order))
                        return _o;
                }

                return null;
            }

            public void Add(QS._qss_x_.Properties_.Value_.OrderToken_ _o)
            {
                _items.Add(_o);
            }

            internal void Clear(QS.Fx.Base.Index _maxKnownOrder)
            {
                List < QS._qss_x_.Properties_.Value_.OrderToken_> _toRemove = 
                    new List<QS._qss_x_.Properties_.Value_.OrderToken_>();

                foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _t in _items)
                {
                    if (!_t.Order.Equals(ORDER_UNSET) && _t.Order.CompareTo(_maxKnownOrder) <= 0)
                    {
                        _toRemove.Add(_t);
                    }
                }

                foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _t in _toRemove)
                    _items.Remove(_t);
            }
        }

        #endregion

        #region IAggregatorClient<Round_,ReliableToken_> Members

        void QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>.Phase(QS._qss_x_.Properties_.Value_.Round_ _round)
        {
            lock (this)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Orderer.Phase ( " +
                        QS.Fx.Printing.Printable.ToString(_round) + " ) : Root initialized.  Disseminating blank message.");
#endif

                // Call disseminate with a blank message
                QS._qss_x_.Properties_.Value_.TokenArray_ _arr;

                if (this._rootIncarnation != null && this._rootIncarnation.Equals(_round.Incarnation))
                {
                    this._logger.Log("Phase: begin agg");
                    _arr = new QS._qss_x_.Properties_.Value_.TokenArray_(TYPE_BEGIN_AGG);
                }
                else
                {
                    this._rootIncarnation = _round.Incarnation;
                    this._logger.Log("Phase: new leader");
                    _arr = new QS._qss_x_.Properties_.Value_.TokenArray_(TYPE_NEW_LEADER);
                }

                // Record that I am responsible for the aggregation
                this._lastRound = _round;
                
                this._agg_endpoint.Interface.Disseminate(_round, _arr);
            }
        }

        void QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>.Disseminating(QS._qss_x_.Properties_.Value_.Round_ _round, QS._qss_x_.Properties_.Value_.TokenArray_ _message)
        {
            lock (this)
            {
#if VERBOSE 
                if (this._logger != null)
                    this._logger.Log("Component_.Orderer.Disseminating ( " +
                        QS.Fx.Printing.Printable.ToString(_round) + " , " +
                        QS.Fx.Printing.Printable.ToString(_message) + " ) : Receiving a disseminate message.");
#endif
                this._lastRound = _round;

                if (this._rootIncarnation != null && !this._rootIncarnation.Equals(_round.Incarnation))
                    this._rootIncarnation = null;

                // Record the most recent disemmination so that older ones can be ignored on aggregation.
                if (_message.Type.Equals(TYPE_NEW_LEADER))
                {
                    this._logger.Log("Diss: new leader");
                    roundType = RoundType.NEW_LEADER;
                }
                else if (_message.Type.Equals(TYPE_BEGIN_AGG))
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Orderer.Disseminating" +
                            " : Recording round.");
#endif
                    roundType = RoundType.AGGREGATE;
                }
                else if (_message.Type.Equals(TYPE_AGG_DECISION))
                {
                    roundType = RoundType.DECISION;
                    this._logger.Log("Diss: agg decision");
                    _UpdateLocal(_message);
                }
            }
        }

        void QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS._qss_x_.Properties_.Value_.TokenArray_>.Aggregate(QS._qss_x_.Properties_.Value_.Round_ _round, IList<QS._qss_x_.Properties_.Value_.TokenArray_> _messages)
        {
            lock (this)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Orderer.Aggregate ( " +
                        QS.Fx.Printing.Printable.ToString(_round) + " , " +
                        QS.Fx.Printing.Printable.ToString(_messages) + " ) : Token found, updating order.");
#endif
                if (this._lastRound == null)
                    this._logger.Log("lastRound == null");
                else
                    this._logger.Log("LastRound = " + QS.Fx.Printing.Printable.ToString(_lastRound));

                // We don't aggregate on this type of message
                if (roundType.Equals(RoundType.DECISION)) return;

                if (this._lastRound == null || !this._lastRound.Equals(_round)) return;

                if (roundType.Equals(RoundType.NEW_LEADER))
                {
                    this._logger.Log("Agg: new leader round");
                    // if its a new leader, report the max global order i've seen so far
                    QS.Fx.Base.Index _myMax = _maxKnownOrder;
                    
                    foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in _list.Items)
                    {
                        if (!_o.Order.Equals(ORDER_UNSET))
                            _myMax = (_myMax.CompareTo(_o.Order) > 0) ? _myMax : _o.Order;
                    }
                    

                    QS.Fx.Base.Index _max = _myMax;
                    if(_messages != null)
                    {
                        foreach(QS._qss_x_.Properties_.Value_.TokenArray_ _ta in  _messages)
                            _max = (_max.CompareTo(_ta.MaxIndex) > 0) ? _max : _ta.MaxIndex;
                    }

                    QS._qss_x_.Properties_.Value_.TokenArray_ rv =
                        new QS._qss_x_.Properties_.Value_.TokenArray_(TYPE_NEW_LEADER, _max);

                    // if i am the root.
                    if (this._rootIncarnation != null)
                    {
                        this._logger.Log("Agg: root leader");
                        _myGlobalIndex = Int32.Parse(_max.String) + 1;
                    }
                    else
                    {
                        this._logger.Log("Agg: calling agg_end.agg");
                        this._agg_endpoint.Interface.Aggregate(_round, rv);
                    }
                }
                else if(roundType.Equals(RoundType.AGGREGATE))
                {
                    this._logger.Log("Agg: not new leader round");
                    QS._qss_x_.Properties_.Value_.TokenArray_ _arr = new QS._qss_x_.Properties_.Value_.TokenArray_();
                    QS.Fx.Base.Index _min = new QS.Fx.Base.Index(Int32.MaxValue);

                    if (_messages != null)
                    {
                        foreach (QS._qss_x_.Properties_.Value_.TokenArray_ _a in _messages)
                        {
                            _arr.Add(_a.Tokens);

                            // Record the minimum value from all my children
                            if (_a.MaxIndex.CompareTo(_min) < 0)
                                _min = _a.MaxIndex;
                        }
                    }

                    // Add all my knowledge to the list
                    QS.Fx.Base.Index _max = (_maxKnownOrder == null) ? new QS.Fx.Base.Index(0) : _maxKnownOrder;

                    foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in _list.Items)
                    {
                        _arr.Add(_o);

                        // Record the maximum global index I have seen so far.
                        if (!_o.Order.Equals(ORDER_UNSET) && _o.Order.CompareTo(_max) > 0)
                            _max = _o.Order;
                    }

                    _arr.MaxIndex = _min.CompareTo(_max) > 0 ? _max : _min;     // should be set to the min of the two

#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Orderer.Aggregate ( " +
                            QS.Fx.Printing.Printable.ToString(_arr) + " ) : Final aggregation.");
#endif

                    // if i am the root.
                    if (this._rootIncarnation != null)
                    {
                        this._logger.Log("Agg: calling Decide");
                        this._Decide(_round, _arr);
                    }
                    else
                    {
                        this._logger.Log("Agg: calling agg_end.agg");
                        this._agg_endpoint.Interface.Aggregate(_round, _arr);
                    }
                }
            }
        }

        #endregion

        #region _Decide

        void _Decide(QS._qss_x_.Properties_.Value_.Round_ _round, QS._qss_x_.Properties_.Value_.TokenArray_ _arr)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Orderer.Decide ( " +
                    QS.Fx.Printing.Printable.ToString(_arr) + " ) : Deciding the global order.");
#endif

            // Remove duplicates, preference to ones with a global id
            List<QS._qss_x_.Properties_.Value_.OrderToken_> _list = new List<QS._qss_x_.Properties_.Value_.OrderToken_>();

            foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in _arr.Tokens)
            {
                this._logger.Log("Decide: foreach");
                QS._qss_x_.Properties_.Value_.OrderToken_ _found = null;
                foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _temp in _list)
                {
                    if (_o.Payload.Equals(_temp.Payload))
                    {
                        _found = _temp;
                        break;
                    }
                }

                if (_found != null)
                {
                    // We need to keep ones with orders already first
                    if (_found.Order.CompareTo(ORDER_UNSET) == 0 && _o.Order.CompareTo(ORDER_UNSET) != 0)
                    {
                        _found.Order = _o.Order;
                    }
                }
                else
                {
                    _list.Add(_o);
                }
            }

            // Split into decided and undecided
            List<QS.Fx.Serialization.ISerializable> _decided = new List<QS.Fx.Serialization.ISerializable>();
            List<QS.Fx.Serialization.ISerializable> _undecided = new List<QS.Fx.Serialization.ISerializable>();

            foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in _list)
            {
                if (_o.Order.CompareTo(ORDER_UNSET) == 0)
                {
                    _undecided.Add(_o);
                    this._logger.Log("Undecided++");
                }
                else
                {
                    // Only include messages that everyone doesn't know about
                    if (_o.Order.CompareTo(_arr.MaxIndex) > 0)
                    {
                        _decided.Add(_o);
                        this._logger.Log("Decided++");
                    }
                }
            }

            // Decide for the undecided ones based on my local index
            foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in _undecided)
            {
                this._logger.Log("Assigning " + _myGlobalIndex);
                _o.Order = new QS.Fx.Base.Index(_myGlobalIndex++);
            }

            // Let everyone know my ruling decision, I RULE!
            QS._qss_x_.Properties_.Value_.TokenArray_ _final = new QS._qss_x_.Properties_.Value_.TokenArray_(TYPE_AGG_DECISION);
            _final.Add(_decided);
            _final.Add(_undecided);
            _final.MaxIndex = _arr.MaxIndex;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Orderer.Decide ( " +
                    QS.Fx.Printing.Printable.ToString(_round) + " , " +
                    QS.Fx.Printing.Printable.ToString(_final) + " ) : Final decision.");
#endif

            this._agg_endpoint.Interface.Disseminate(_round, _final);
        }

        #endregion

        #region _UpdateLocal

        void _UpdateLocal(QS._qss_x_.Properties_.Value_.TokenArray_ _message)
        {
            // Take the message and process it for new decisions.
            foreach (QS._qss_x_.Properties_.Value_.OrderToken_ _o in _message.Tokens)
            {
                this._logger.Log("UpdateLocal: foreach");
                QS._qss_x_.Properties_.Value_.OrderToken_ _tok = _list.Get((IdentifierClass)_o.Payload);

                if (_tok == null)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Orderer._UpdateLocal ( " +
                            QS.Fx.Printing.Printable.ToString(_o) + " ) : Token not found, adding it to list.");
#endif
                    _list.Add(_o);
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Orderer._UpdateLocal ( " +
                            QS.Fx.Printing.Printable.ToString(_o) + " ) : Token found, updating order.");
#endif
                    _tok.Order = _o.Order;
                }
            }

            // Now all the orders are assigned.  Deliver appropriate messages;
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Orderer._UpdateLocal ( " +
                    _nextOrder + " ) : Delivering messages.");
#endif

            QS._qss_x_.Properties_.Value_.OrderToken_ _ot;

            while ((_ot = _list.GetFromOrder(new QS.Fx.Base.Index(_nextOrder))) != null)
            {
                this._logger.Log("UpdateLocal: deliver while");
                if (_ot.Payload != null)
                {
                    _nextOrder++;
                    this._myendpoint.Interface.Deliver((IdentifierClass)_ot.Payload);
                }
            }

            // Remove the messages everyone knows about
            _maxKnownOrder = _message.MaxIndex;
            _list.Clear(_maxKnownOrder);
        }

        #endregion


        #region IOrder<Index,Identifier> Members

        void QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>.Cached(QS.Fx.Base.Index epoch, IdentifierClass identifier)
        {
            lock (this)
            {
                QS._qss_x_.Properties_.Value_.OrderToken_ _ot = _list.Get(identifier);
                this._logger.Log("cached: 1");
                if(_ot == null)
                {
                    this._logger.Log("cached: not found");
                    QS._qss_x_.Properties_.Value_.OrderToken_ _o = 
                        new QS._qss_x_.Properties_.Value_.OrderToken_(ORDER_UNSET, identifier);
                    this._list.Add(_o);
                }
            }
        }

        void QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>.Confirmed(QS.Fx.Base.Index epoch)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IOrder<Index,IdentifierClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, IdentifierClass>, QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>> QS.Fx.Object.Classes.IOrder<QS.Fx.Base.Index, IdentifierClass>.Orderer
        {
            get { return _myendpoint; }
        }

        #endregion
    }
}
