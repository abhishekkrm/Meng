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
    public sealed class QsmControllerClient_ :
        QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QsmControl_>,
        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>,
        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
        IDisposable
    {
        #region Constructor

        public QsmControllerClient_(QS.Fx.Object.IContext _mycontext, QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver, QsmController_ _controller)
        {
            this._myquicksilver = _myquicksilver;
            this._controller = _controller;
            this._endpoint =
                _mycontext.DualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>(this);
            this._endpoint.OnConnect += new QS.Fx.Base.Callback(this._OnConnect);
            this._endpoint.OnConnected += new QS.Fx.Base.Callback(this._OnConnected);
            this._endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);
        }

        #endregion

        #region Destructor

        ~QsmControllerClient_()
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
        private QsmController_ _controller;
        private int _disposed;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>> _endpoint;
        private string _name;
        private Queue<QsmControl_> _outgoing = new Queue<QsmControl_>();
        private IDictionary<QS.Fx.Base.ID, QsmControllerConnection_> _connections = new Dictionary<QS.Fx.Base.ID, QsmControllerConnection_>();
        private QsmControl_ _lastincoming;
        private Stack<QsmControl_> _incoming = new Stack<QsmControl_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICommunicationChannel<QsmControl_> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>, QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>> QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QsmControl_>._Channel
        {
            get { return this._endpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICommunicationChannelClient<QsmControl_> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>._Receive(QsmControl_ _message)
        {
            lock (this._controller)
            {
                this._outgoing.Enqueue(_message);
                if (this._endpoint.IsConnected)
                {
                    while (this._outgoing.Count > 0)
                    {
                        QsmControl_ _m = _outgoing.Dequeue();
                        this._endpoint.Interface._Receive(_m);
                    }
                }
            }
        }

        #endregion

        #region ICommunicationChannel<QsmControl_> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>._Send(QsmControl_ _message)
        {
            QsmControl_ _o;
            do
            {
                _o = this._lastincoming;
                _message._link = _o;
            }
            while (!ReferenceEquals(Interlocked.CompareExchange<QsmControl_>(ref this._lastincoming, _message, _o), _o));
            this._myquicksilver._Core.Schedule(
                new QS.Fx.Base.Event<object>(
                    new QS.Fx.Base.ContextCallback<object>(this._ReceiveCallback), null));
        }

        private void _ReceiveCallback(object _x)
        {
            QsmControl_  _o = Interlocked.Exchange<QsmControl_>(ref this._lastincoming, null);
            while (_o != null)
            {
                this._incoming.Push(_o);
                _o = _o._link;
            }
            lock (this._controller)
            {
                while (this._incoming.Count > 0)
                {
                    QsmControl_ _message = this._incoming.Pop();
                    switch (_message._operation)
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
                            throw new Exception("Operation of type \"" + _message._operation.ToString() + "\" was not expected in this context.");
                        case QsmOperation_.Create_:
                        default:
                            throw new NotImplementedException();
                    }
                    if (this._endpoint.IsConnected)
                    {
                        while (this._outgoing.Count > 0)
                        {
                            QsmControl_ _m = this._outgoing.Dequeue();
                            this._endpoint.Interface._Receive(_m);
                        }
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _OnConnect

        private void _OnConnect()
        {
        }

        #endregion

        #region _OnConnected

        private void _OnConnected()
        {
        }

        #endregion

        #region _OnDisconnect ************************** should remove client

        private void _OnDisconnect()
        {
            lock (this._controller)
            {
                throw new NotImplementedException();
/*
                this._controller._clients.Remove(this);
                foreach (_Connection _connection in this._connections.Values)
                    _connection.Channel.Disconnected(_connection);
*/
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _HandleIncoming_Connect

        private void _HandleIncoming_Connect(QsmControl_ _message)
        {
            this._outgoing.Enqueue
            (
                new QsmControl_
                (
                    QsmOperation_.Connected_,
                    0,
                    0,
                    0,
                    null
                )                
            );
        }

        #endregion

        #region _HandleIncoming_Open

        private void _HandleIncoming_Open(QsmControl_ _message)
        {
            throw new NotImplementedException();            
/*
            if (this._connections.ContainsKey(_message.Channel))
                throw new Exception("Client \"" + this._name + "\" is trying to open a channel " + _message.Channel.ToString() +
                    ", which it either has already opened, or has not previously closed.");
            _Channel _channel;
            if (this._controller._channels.TryGetValue(_message.Channel, out _channel))
            {
                _Connection _connection = new _Connection(this, _channel);
                this._connections.Add(_channel.ID, _connection);
                _channel.Connections.Add(_connection);
                _channel.Uninitialized.Add(_connection);
                _channel.TakeCheckpoint();
            }
            else
                throw new Exception("Client \"" + this._name + "\" is trying to open a nonexisting channel " + _message.Channel.ToString() + ".");
*/ 
        }

        #endregion

        #region _HandleIncoming_Close

        private void _HandleIncoming_Close(QsmControl_ _message)
        {
            throw new NotImplementedException();
/*
            _Channel _channel;
            if (!this._controller._channels.TryGetValue(_message.Channel, out _channel))
                throw new Exception("Client \"" + this._name + "\" is trying to close a nonexisting channel " + _message.Channel.ToString() + ".");
            _Connection _connection;
            if (!this._connections.TryGetValue(_message.Channel, out _connection) || !_channel.Connections.Remove(_connection))
                throw new Exception("Client \"" + this._name + "\" is trying to close a channel " + _message.Channel.ToString() +
                    ", which it has not opened yet, or which it has already closed.");
            this._connections.Remove(_channel.ID);
            _channel.Initialized.Remove(_connection);
            _channel.Uninitialized.Remove(_connection);
*/ 
        }

        #endregion

        #region _HandleIncoming_Initialize

        private void _HandleIncoming_Initialize(QsmControl_ _message)
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

        private void _HandleIncoming_Send(QsmControl_ _message)
        {
            throw new NotImplementedException();
/*
            _Connection _connection;
            if (!this._connections.TryGetValue(_message.Channel, out _connection))
                throw new Exception("Client \"" + this._name + "\" is trying to send a message even though it is not connected.");
            _connection.Channel.Message(_message.Object);
*/ 
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
