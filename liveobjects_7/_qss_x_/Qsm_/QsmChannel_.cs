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

using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_x_.Qsm_
{
    public sealed class QsmChannel_ : IDisposable
    {
        #region Constructor

        public QsmChannel_(QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver, 
            QsmController_ _controller, QsmControllerChannel_ _controllerchannel)
        {
            this._myquicksilver = _myquicksilver;
            this._controller = _controller;
            this._controllerchannel = _controllerchannel;
            this._id = _controllerchannel._Channel;
            this._groupid = new QS._qss_c_.Base3_.GroupID((uint) this._id);
        }

        #endregion

        #region Destructor

        ~QsmChannel_()
        {
            this._Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                }
            }
        }

        #endregion

        #region Fields

        private QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver;
        private int _disposed;
        private long _id;
        private QS._qss_c_.Base3_.GroupID _groupid;
        private QsmController_ _controller;
        private QsmControllerChannel_ _controllerchannel;
        private ICollection<QsmConnection_> _connections = new System.Collections.ObjectModel.Collection<QsmConnection_>();
        private QsmChannelStatus_ _status = QsmChannelStatus_.Unsubscribed_;
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> _underlyingsink;
        private uint _last_message_seqno;
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> _outgoing =
            new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();
        private bool _waitingtosend;

/*
        private ICollection<QsmConnection_> _initialized = new System.Collections.ObjectModel.Collection<QsmConnection_>();
        private ICollection<QsmConnection_> _uninitialized = new System.Collections.ObjectModel.Collection<QsmConnection_>();
        private Timer _timer;
        private Queue<KeyValuePair<uint, QS.Fx.Serialization.ISerializable>> _cached_messages =
            new Queue<KeyValuePair<uint, QS.Fx.Serialization.ISerializable>>();
*/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Accessors

        public QS._qss_c_.Base3_.GroupID _GroupID
        {
            get { return this._groupid; }
        }

        public QsmControllerChannel_ _ControllerChannel
        {
            get { return this._controllerchannel; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Open

        public void _Open(QsmConnection_ _connection)
        {
            lock (this._controller)
            {
                this._connections.Add(_connection);
                switch (this._status)
                {
                    case QsmChannelStatus_.Unsubscribing_:
                        {
                            this._status = QsmChannelStatus_.UnsubscribingPendingSubscribe_;
                        }
                        break;

                    case QsmChannelStatus_.Unsubscribed_:
                        {
                            this._status = QsmChannelStatus_.PendingSubscribe_;
                            this._controller._Open(this);
                        }
                        break;

                    case QsmChannelStatus_.SubscribingPendingUnsubscribe_:
                        {
                            this._status = QsmChannelStatus_.Subscribing_;
                        }
                        break;

                    case QsmChannelStatus_.Subscribed_:
                        {
                            this._SubscribedCallback(_connection);
                        }
                        break;

                    case QsmChannelStatus_.PendingUnsubscribe_:
                        {
                            this._status = QsmChannelStatus_.Subscribed_;
                            this._controller._Open(this);
                            this._SubscribedCallback(_connection);
                        }
                        break;

                    case QsmChannelStatus_.PendingSubscribe_:
                    case QsmChannelStatus_.Subscribing_:
                    case QsmChannelStatus_.UnsubscribingPendingSubscribe_:
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }
        
        #endregion

        #region _Close

        public void _Close(QsmConnection_ _connection)
        {
            lock (this._controller)
            {
                this._connections.Remove(_connection);
                if (this._connections.Count == 0)
                {
                    switch (this._status)
                    {
                        case QsmChannelStatus_.Subscribing_:
                            {
                                this._status = QsmChannelStatus_.SubscribingPendingUnsubscribe_;
                            }
                            break;

                        case QsmChannelStatus_.Subscribed_:
                            {
                                this._status = QsmChannelStatus_.PendingUnsubscribe_;
                                this._controller._Close(this);
                            }
                            break;

                        case QsmChannelStatus_.UnsubscribingPendingSubscribe_:
                            {
                                this._status = QsmChannelStatus_.Unsubscribing_;
                            }
                            break;

                        case QsmChannelStatus_.PendingSubscribe_:
                            {
                                this._status = QsmChannelStatus_.Unsubscribed_;
                                this._controller._Close(this);
                            }
                            break;

                        case QsmChannelStatus_.PendingUnsubscribe_:
                        case QsmChannelStatus_.Unsubscribing_:
                        case QsmChannelStatus_.Unsubscribed_:
                        case QsmChannelStatus_.SubscribingPendingUnsubscribe_:
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region _Send

        public void _Send(QS._core_c_.Core.ChannelObject _message)
        {
            _message.sequenceno = (int) (++this._last_message_seqno);
            QsmOutgoing_ _outgoingobject = 
                new QsmOutgoing_(
                    this._id, 
                    _message.datalength, 
                    _message.datablocks,
                    new QS._core_c_.Base6.CompletionCallback<QsmOutgoing_>(this._CompletionCallback));
            this._outgoing.Enqueue(_outgoingobject);            
            if (!this._waitingtosend)
            {
                this._waitingtosend = true;
                this._underlyingsink.Send(
                    new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(
                            this._SendCallback));
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Subscribing

        public bool _Subscribing()
        {
            if (this._status == QsmChannelStatus_.PendingSubscribe_)
            {
                this._status = QsmChannelStatus_.Subscribing_;
                return true;
            }
            else 
                return false;
        }

        #endregion

        #region _Unsubscribing

        public bool _Unsubscribing()
        {
            if (this._status == QsmChannelStatus_.PendingUnsubscribe_)
            {
                this._status = QsmChannelStatus_.Unsubscribing_;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region _Subscribed

        public void _Subscribed()
        {
            switch (this._status)
            {
                case QsmChannelStatus_.Subscribing_:
                case QsmChannelStatus_.SubscribingPendingUnsubscribe_:
                    {
                        if (this._connections.Count > 0)
                        {
                            this._status = QsmChannelStatus_.Subscribed_;
                            this._underlyingsink = this._myquicksilver._GroupSinks[this._groupid];
                            foreach (QsmConnection_ _connection in this._connections)
                                this._SubscribedCallback(_connection);
                        }
                        else
                        {
                            this._status = QsmChannelStatus_.PendingUnsubscribe_;
                            this._controller._Close(this);
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _Unsubscribed

        public void _Unsubscribed()
        {
            switch (this._status)
            {
                case QsmChannelStatus_.Unsubscribing_:
                case QsmChannelStatus_.UnsubscribingPendingSubscribe_:
                    {
                        if (this._connections.Count > 0)
                        {
                            this._status = QsmChannelStatus_.PendingSubscribe_;
                            this._controller._Open(this);
                        }
                        else
                            this._status = QsmChannelStatus_.Unsubscribed_;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _Receive

        public void _Receive(QS._core_c_.Core.ChannelObject _message)
        {
            foreach (QsmConnection_ _connection in this._connections)
            {
                try
                {
                    _connection._Client._Handle(_message);
                }
                catch (Exception _exc)
                {
                    this._Close(_connection);
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _SubscribedCallback

        private void _SubscribedCallback(QsmConnection_ _connection)
        {
            _connection._Client._Handle
            (
                new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Initialize_,
                    this._id,
                    0,
                    0,
                    0,
                    null
                )
            );
        }

        #endregion

        #region _SendCallback

        private void _SendCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> _objectstosend,
            int _maximumnumberofobjects, 
            out int _numberofobjectsreturned, 
            out bool _moreobjectsavailable)
        {
            _numberofobjectsreturned = 0;
            _moreobjectsavailable = true;
            while (_numberofobjectsreturned < _maximumnumberofobjects)
            {
                if (this._outgoing.Count > 0)
                {
                    _objectstosend.Enqueue(this._outgoing.Dequeue());
                    _numberofobjectsreturned++;
                }
                else
                {
                    _moreobjectsavailable = false;
                    this._waitingtosend = false;
                    break;
                }
            }
        }

        #endregion

        #region _CompletionCallback

        private void _CompletionCallback(bool _succeeded, Exception _exception, QsmOutgoing_ _outgoingobject)
        {
            if (!_succeeded)
                throw new Exception("Could not deliver message.\n" + QS.Fx.Printing.Printable.ToString(_outgoingobject));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
