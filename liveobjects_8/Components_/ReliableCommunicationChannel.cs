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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.ReliableCommunicationChannel, 
        "Properties Framework Reliable Communication Channel", "")]
    public sealed class ReliableCommunicationChannel<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
        : QS._qss_x_.Properties_.Component_.Base_,
          QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
          QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>,
          QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
          QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>,
          QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {

        public ReliableCommunicationChannel(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Orderer", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IOrder<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>> _order_reference,
            [QS.Fx.Reflection.Parameter("CommunicationChannel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>> _cc_reference,
            [QS.Fx.Reflection.Parameter("Reliable", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>> _reliable_reference,
            [QS.Fx.Reflection.Parameter("drop_rate", QS.Fx.Reflection.ParameterClass.Value)]
            double drop_rate
        ) : base(_mycontext, true)
        {
            if (_order_reference != null)
            {
                this._order_endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>,
                    QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>>(this);
                this._order_connection = this._order_endpoint.Connect(_order_reference.Dereference(_mycontext).Orderer);
            } 
            else
                throw new Exception("Reliable Communication Channel requires parameter 'Orderer' to perform correctly.");

            bool primary = false, secondary = false;

            if (_cc_reference != null)
            {
                this._cc_lower_endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>>(this);
                this._cc_lower_connection = _cc_lower_endpoint.Connect(_cc_reference.Dereference(_mycontext).Channel);
                primary = true;
            }

            if (_reliable_reference != null)
            {
                this._reliable_endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>,
                    QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>>(this);
                this._reliable_connection = _reliable_endpoint.Connect(_reliable_reference.Dereference(_mycontext).Reliable);
                secondary = true;
            }

            if (!(primary || secondary))
                throw new Exception("Reliable Communication Channel requires a communication object ('CommunicationChannel' or 'Reliable').");

            this._cc_higher_endpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            _myid = new QS.Fx.Base.Identifier(Guid.NewGuid());
            _myincarnation = new QS.Fx.Base.Incarnation(1U);
            _myindex = 0;

            this.drop_rate = drop_rate;
        }

        #region Fields
        private QS.Fx.Endpoint.IConnection _order_connection;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IOrder<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>,
            QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>> 
                    _order_endpoint;   

        private QS.Fx.Endpoint.IConnection _cc_lower_connection;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>>
                _cc_lower_endpoint;

        private QS.Fx.Endpoint.IConnection _reliable_connection;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>,
            QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>>
                _reliable_endpoint;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>
                _cc_higher_endpoint;

        private Random random = new Random();
        [QS.Fx.Base.Inspectable]
        private double drop_rate;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _myid;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _myincarnation;
        [QS.Fx.Base.Inspectable]
        private int _myindex;
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Base.MessageIdentifier> _to_deliver = new Queue<QS.Fx.Base.MessageIdentifier>();
        [QS.Fx.Base.Inspectable]
        private System.Collections.Hashtable _cache = new System.Collections.Hashtable();
        [QS.Fx.Base.Inspectable]
        private System.Collections.Hashtable _epochLists = new System.Collections.Hashtable();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index _currentepoch = null;

        #endregion


        #region ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>.Receive(QS._qss_x_.Properties_.Value_.IdToken_ _message)
        {
            lock (this)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableCommunicationChannel.Receive ( " +
                        QS.Fx.Printing.Printable.ToString(_message) + " ) : Receving message from lower channel. ");
#endif
                // Receiving a message from below me
                
                // To simulate a bad connection 
                if (drop_rate != 0.0 && random.NextDouble() <= drop_rate) 
                    return;
                
                // NEED TO KNOW WHICH EPOCH THE MESSAGE CAME FROM ON THE SENDER

                _order_endpoint.Interface.Cached(_currentepoch, _message.Id);
                CacheAdd(_message);

                if (_reliable_endpoint != null && _reliable_endpoint.IsConnected)
                    _reliable_endpoint.Interface.Cached(_currentepoch, _message.Id);

                // In case this message was being waited for
                _Deliver();
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>.Initialize(QS._qss_x_.Properties_.Value_.IdToken_ _checkpoint)
        {
            // lets not aggregate checkpoints for now
            _cc_higher_endpoint.Interface.Initialize((CheckpointClass)_checkpoint.Payload);
        }

        QS._qss_x_.Properties_.Value_.IdToken_ QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Properties_.Value_.IdToken_, QS._qss_x_.Properties_.Value_.IdToken_>.Checkpoint()
        {
            // lets not aggregate checkpoints for now
            QS.Fx.Base.MessageIdentifier _mid = 
                new QS.Fx.Base.MessageIdentifier(_myid, _myincarnation, new QS.Fx.Base.Index(_myindex));
            QS._qss_x_.Properties_.Value_.IdToken_ _tok = 
                new QS._qss_x_.Properties_.Value_.IdToken_(_mid, _cc_higher_endpoint.Interface.Checkpoint());

            return _tok;
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            lock (this)
            {
                if (_currentepoch == null) return;
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableCommunicationChannel.Send ( " +
                        QS.Fx.Printing.Printable.ToString(_message) + " ) : Sending message to unreliable channel. ");
#endif
                // Sending a message out to the group

                QS.Fx.Base.MessageIdentifier _mid =
                        new QS.Fx.Base.MessageIdentifier(_myid, _myincarnation, new QS.Fx.Base.Index(_myindex++));
                QS._qss_x_.Properties_.Value_.IdToken_ _tok = new QS._qss_x_.Properties_.Value_.IdToken_(_mid, _message);

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.ReliableCommunicationChannel.Send ( " +
                        QS.Fx.Printing.Printable.ToString(_tok) + " ) :  Adding token to my local list.");
#endif

                CacheAdd(_tok);
                _order_endpoint.Interface.Cached(_currentepoch, _mid);
                ((List<QS.Fx.Base.MessageIdentifier>)_epochLists[_currentepoch]).Add(_mid);

                if (_reliable_endpoint != null && _reliable_endpoint.IsConnected)
                    _reliable_endpoint.Interface.Cached(_currentepoch, _mid);

                if (_cc_lower_endpoint != null && _cc_lower_endpoint.IsConnected)
                    _cc_lower_endpoint.Interface.Send(_tok);
            }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass,CheckpointClass>>  
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass,CheckpointClass>.Channel
        {
	        get { return _cc_higher_endpoint; }
        }

        #endregion

        #region IOrderClient<Index, MessageIdentifier> Members

        void QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>.Request(QS.Fx.Base.Index epoch)
        {
            lock (this)
            {
                // Receiving a request to move onto a new epoch
                this._currentepoch = epoch;
                this._order_endpoint.Interface.Confirmed(epoch);

                _epochLists[epoch] = new List<QS.Fx.Base.MessageIdentifier>();
            }
        }

        void QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>.Completed(QS.Fx.Base.Index epoch)
        {
            lock (this)
            {
                // Remove everything in my local cache with this epoch
                if(_epochLists[epoch] != null)
                {
                    List<QS.Fx.Base.MessageIdentifier> eList = (List<QS.Fx.Base.MessageIdentifier>)_epochLists[epoch];
                    foreach (QS.Fx.Base.MessageIdentifier _mid in eList)
                    {
                        CacheRemove(_mid);
                    }

                    _epochLists[epoch] = null;
                }

                // Tell the reliability layer the epoch is closed!!
                if (this._reliable_endpoint != null && this._reliable_endpoint.IsConnected)
                {
                    this._reliable_endpoint.Interface.Completed(epoch);
                }
            }
        }

        void QS.Fx.Interface.Classes.IOrderClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier>.Deliver(QS.Fx.Base.MessageIdentifier identifier)
        {
            lock (this)
            {
                _Ordered(identifier);
                _Deliver();
            }
        }

        #endregion

        #region Delivery Methods

        void _Ordered(QS.Fx.Base.MessageIdentifier _id)
        {
            _to_deliver.Enqueue(_id);
        }

        void _Deliver()
        {
            QS._qss_x_.Properties_.Value_.IdToken_ _m;
            QS.Fx.Base.MessageIdentifier _id;
            bool done = false;

            while (!done && _to_deliver.Count > 0)
            {
                _id = _to_deliver.Peek();
                _m = CacheGet(_id);

                if (_m != null)
                {
                    _to_deliver.Dequeue();
                    this._cc_higher_endpoint.Interface.Receive((MessageClass)_m.Payload);
                }
                else
                    done = true;
            } 
        }

        #endregion

        #region Cache methods

        void CacheAdd(QS._qss_x_.Properties_.Value_.IdToken_ _t)
        {
            this._cache[_t.Id] = _t;
        }

        QS._qss_x_.Properties_.Value_.IdToken_ CacheGet(QS.Fx.Base.MessageIdentifier _id)
        {
            return (QS._qss_x_.Properties_.Value_.IdToken_) this._cache[_id];
        }

        void CacheRemove(QS._qss_x_.Properties_.Value_.IdToken_ _tok)
        {
            this._cache.Remove(_tok.Id);
        }

        void CacheRemove(QS.Fx.Base.MessageIdentifier _mid)
        {
            this._cache.Remove(_mid);
        }

        #endregion

        #region IReliableClient<Index,MessageIdentifier> Members

        void QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>.Receive(QS.Fx.Base.Index epoch, QS.Fx.Base.MessageIdentifier identifier, MessageClass msg)
        {
            //CacheAdd();
        }

        void QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>.Fetch(QS.Fx.Base.MessageIdentifier identifier)
        {
            lock (this)
            {
                if (_reliable_endpoint != null && _reliable_endpoint.IsConnected)
                {
                    QS._qss_x_.Properties_.Value_.IdToken_ _t = CacheGet(identifier);

                    if (_t == null)
                        _reliable_endpoint.Interface.Message(identifier, (MessageClass) _t.Payload);
                    else
                        _reliable_endpoint.Interface.Message(identifier, null);
                }
            }
        }

        #endregion
    }
}

