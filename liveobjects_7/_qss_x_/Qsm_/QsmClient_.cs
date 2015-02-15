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
    public sealed class QsmClient_ : IDisposable, QS._core_c_.Core.IChannel
    {
        #region Constructor

        public QsmClient_(QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver, 
            QsmController_ _controller, string _id, string _name, QS._core_c_.Core.IChannel _underlyingchannel)
        {
            this._myquicksilver = _myquicksilver;
            this._controller = _controller;
            this._id = _id;
            this._name = _name;
            this._underlyingchannel = _underlyingchannel;
        }

        #endregion

        #region Destructor

        ~QsmClient_()
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
        private QsmController_ _controller;
        private string _id, _name;
        private QS._core_c_.Core.IChannel _underlyingchannel;
        private Queue<QS._core_c_.Core.ChannelObject> _outgoing = new Queue<QS._core_c_.Core.ChannelObject>();
        private IDictionary<long, QsmConnection_> _connections = new Dictionary<long, QsmConnection_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Handle

        public void _Handle(QS._core_c_.Core.ChannelObject _message)
        {
            lock (this._controller)
            {
                this._outgoing.Enqueue(_message);
                if (this._underlyingchannel != null)
                {
                    while (this._outgoing.Count > 0)
                    {
                        QS._core_c_.Core.ChannelObject _m = this._outgoing.Dequeue();
                        this._underlyingchannel.Handle(_m);
                    }
                }
            }
        }

        #endregion

        #region IChannel Members

        void QS._core_c_.Core.IChannel.Handle(QS._core_c_.Core.ChannelObject _message)
        {
            QsmOperation_ _operation = (QsmOperation_) _message.operation;
            lock (this._controller)
            {
                switch (_operation)
                {
                    case QsmOperation_.Connect_:
                        this._HandleIncoming_Connect(_message);
                        break;
                    case QsmOperation_.Open_:
                        this._HandleIncoming_Open(_message);
                        break;
                    case QsmOperation_.Close_:
                        this._HandleIncoming_Close(_message);
                        break;
                    case QsmOperation_.Initialize_:
                        this._HandleIncoming_Initialize(_message);
                        break;
                    case QsmOperation_.Send_:
                        this._HandleIncoming_Send(_message);
                        break;
                    case QsmOperation_.Connected_:
                    case QsmOperation_.Created_:
                    case QsmOperation_.Checkpoint_:
                    case QsmOperation_.Receive_:
                        throw new Exception("Operation of type \"" + _operation.ToString() + "\" was not expected in this context.");
                    case QsmOperation_.Create_:
                    default:
                        throw new NotImplementedException();
                }
                if (this._underlyingchannel != null)
                {
                    while (this._outgoing.Count > 0)
                    {
                        QS._core_c_.Core.ChannelObject _m = this._outgoing.Dequeue();
                        this._underlyingchannel.Handle(_m);
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _HandleIncoming_Connect

        private void _HandleIncoming_Connect(QS._core_c_.Core.ChannelObject _message)
        {
            this._outgoing.Enqueue
            (
                new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Connected_,
                    0,
                    0,
                    0,
                    0,
                    null
                )
            );
        }

        #endregion

        #region _HandleIncoming_Open

        private void _HandleIncoming_Open(QS._core_c_.Core.ChannelObject _message)
        {
            if (this._connections.ContainsKey(_message.channel))
                throw new Exception("Client \"" + this._id + "\" is trying to open a channel " + 
                    _message.channel.ToString() + ", which it either has already opened, or has not previously closed.");
            QsmChannel_ _channel = this._controller._Channel(_message.channel);
            QsmConnection_ _connection = new QsmConnection_(this, _channel);
            this._connections.Add(_message.channel, _connection);
            _channel._Open(_connection);
        }

        #endregion

        #region _HandleIncoming_Close

        private void _HandleIncoming_Close(QS._core_c_.Core.ChannelObject _message)
        {
            QsmConnection_ _connection;
            if (this._connections.TryGetValue(_message.channel, out _connection))
                _connection._Channel._Close(_connection);
            else
                throw new Exception("This client is not connected to channel \"" + _message.channel.ToString() + "\".");
        }

        #endregion

        #region _HandleIncoming_Initialize

        private void _HandleIncoming_Initialize(QS._core_c_.Core.ChannelObject _message)
        {
            throw new NotImplementedException();
/*
            _Connection _connection;
            if (!this._connections.TryGetValue(_message.Channel, out _connection))
                throw new Exception("Client \"" + this._name + "\" is trying to provide a checkpoint even though it is not connected.");
            _connection.Channel.Checkpoint(_message.SequenceNo, _message.Object);
*/ 
        }

        #endregion

        #region _HandleIncoming_Send

        private void _HandleIncoming_Send(QS._core_c_.Core.ChannelObject _message)
        {
            QsmConnection_ _connection;
            if (!this._connections.TryGetValue(_message.channel, out _connection))
                throw new Exception("Client \"" + this._id + "\" is trying to send to a channel " +
                    _message.channel.ToString() + ", which is not currently opened.");
            _connection._Channel._Send(_message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
