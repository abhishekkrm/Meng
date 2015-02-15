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
    public sealed class QsmControllerChannel_ : IDisposable
    {
        #region Constructor

        public QsmControllerChannel_(QsmController_ _controller, long _channel, QS.Fx.Base.ID _id, string _name, string _comment, 
            QS.Fx.Reflection.IValueClass _messageclass, QS.Fx.Reflection.IValueClass _checkpointclass, int _index)
        {
            this._controller = _controller;
            this._id = _id;
            this._name = _name;
            this._comment = _comment;
            this._messageclass = _messageclass;
            this._checkpointclass = _checkpointclass;
            this._channel = _channel;
            this._index = _index;
        }

        #endregion

        #region Destructor

        ~QsmControllerChannel_()
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

        private QsmController_ _controller;
        private int _disposed, _index;
        private QS.Fx.Base.ID _id;
        private string _name, _comment;
        private QS.Fx.Reflection.IValueClass _messageclass, _checkpointclass;
        private long _channel;
        private ICollection<QsmConnection_> _connections = new System.Collections.ObjectModel.Collection<QsmConnection_>();

/*
        private ICollection<QsmConnection_> _initialized = new System.Collections.ObjectModel.Collection<QsmConnection_>();
        private ICollection<QsmConnection_> _uninitialized = new System.Collections.ObjectModel.Collection<QsmConnection_>();
        private Timer _timer;
        private uint _last_message_seqno;
        private Queue<KeyValuePair<uint, QS.Fx.Serialization.ISerializable>> _cached_messages =
            new Queue<KeyValuePair<uint, QS.Fx.Serialization.ISerializable>>();
*/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Accessors

        public long _Channel
        {
            get { return this._channel; }
        }

        public QS.Fx.Base.ID _ID
        {
            get { return this._id; }
        }

        public string _Name
        {
            get { return this._name; }
        }

        public string _Comment
        {
            get { return this._comment; }
        }

        public QS.Fx.Reflection.IValueClass _MessageClass
        {
            get { return this._messageclass; }
        }

        public QS.Fx.Reflection.IValueClass _CheckpointClass
        {
            get { return this._checkpointclass; }
        }

        public int _Index
        {
            get { return this._index; }
        }

/*
        public ICollection<_Connection> Connections
        {
            get { return this._connections; }
        }

        public ICollection<_Connection> Initialized
        {
            get { return this._initialized; }
        }

        public ICollection<_Connection> Uninitialized
        {
            get { return this._uninitialized; }
        }
*/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Open

        public void _Open(QsmConnection_ _connection)
        {
            lock (this._controller)
            {
                this._connections.Add(_connection);
            }
        }
        
        #endregion

        #region _Close

        public void _Close(QsmConnection_ _connection)
        {
            this._connections.Remove(_connection);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
