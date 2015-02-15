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

#define OPTION_UNORDERED

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
    public sealed class QsmApplicationConnection_ : 
        QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
        IDisposable
    {
        #region Constructor

        public QsmApplicationConnection_(QS.Fx.Object.IContext _mycontext, QsmApplicationChannel_ _channel)
        {
            this._channel = _channel;
            this._endpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>(this);
            this._endpoint.OnConnect += new QS.Fx.Base.Callback(this._OnConnect);
            this._endpoint.OnConnected += new QS.Fx.Base.Callback(this._OnConnected);
            this._endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);
        }

        #endregion

        #region Destructor

        ~QsmApplicationConnection_()
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

        private int _disposed;
        private QsmApplicationChannel_ _channel;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> _endpoint;
        private bool _isconnected;
        private uint _last_message_seqno;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable,QS.Fx.Serialization.ISerializable> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Channel
        {
            get { return this._endpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Send(
            QS.Fx.Serialization.ISerializable _message)
        {
            this._channel._Send(this, _message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _IsConnected

        public bool _IsConnected
        {
            get { return this._isconnected; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Checkpoint

        public bool _Checkpoint(uint _minimum_seqno, out uint _seqno, out QS.Fx.Serialization.ISerializable _checkpoint)
        {
            if (this._isconnected && this._last_message_seqno >= _minimum_seqno)
            {
                _seqno = this._last_message_seqno;
                _checkpoint = this._endpoint.Interface.Checkpoint();
                return true;
            }
            else
            {
                _seqno = 0;
                _checkpoint = null;
                return false;
            }
        }

        #endregion

        #region _Initialize

        public void _Initialize(uint _seqno, QS.Fx.Serialization.ISerializable _checkpoint)
        {
            if 
            (
                !this._isconnected
                ||
#if OPTION_UNORDERED
                true
#else
                _seqno > this._last_message_seqno
#endif
            )
            {
                this._isconnected = true;
                this._last_message_seqno = _seqno;
                this._endpoint.Interface.Initialize(_checkpoint);
            }
        }

        #endregion

        #region _Receive

        public void _Receive(uint _seqno, QS.Fx.Serialization.ISerializable _message)
        {
            if 
            (
                this._isconnected 
                && 
#if OPTION_UNORDERED                
                true                    
#else
                _seqno > this._last_message_seqno
#endif
            )
            {
#if OPTION_UNORDERED                
#else
                if (_seqno != (this._last_message_seqno + 1))
                    throw new Exception("Received a message with seqno too fat ahead in the future: expecting " +
                        (this._last_message_seqno + 1).ToString() + ", but received " + _seqno.ToString() + ".");
#endif
                this._endpoint.Interface.Receive(_message);
                this._last_message_seqno = _seqno;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _OnConnect

        private void _OnConnect()
        {
            this._channel._Open(this);
        }

        #endregion

        #region _OnConnected

        private void _OnConnected()
        {
        }

        #endregion

        #region _OnDisconnect

        private void _OnDisconnect()
        {
            this._channel._Close(this);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
