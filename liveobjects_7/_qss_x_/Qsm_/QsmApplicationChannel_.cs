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
    public sealed class QsmApplicationChannel_ : IDisposable
    {
        #region Constructor

        public QsmApplicationChannel_(
            QS.Fx.Object.IContext _mycontext, 
            QsmApplication_ _qsm, long _channel, QS.Fx.Base.ID _id, string _name, string _comment,
            QS.Fx.Reflection.IValueClass _messageclass, QS.Fx.Reflection.IValueClass _checkpointclass,
            QS.Fx.Reflection.IComponentClass _wrapperclass)
        {
            this._mycontext = _mycontext;
            this._qsm = _qsm;
            this._channel = _channel;
            this._id = _id;
            this._name = _name;
            this._comment = _comment;
            this._messageclass = _messageclass;
            this._checkpointclass = _checkpointclass;
            this._wrapperclass = _wrapperclass;
        }

        #endregion

        #region Destructor

        ~QsmApplicationChannel_()
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

        private QS.Fx.Object.IContext _mycontext;
        private int _disposed;
        private QsmApplication_ _qsm;
        private long _channel;
        private QS.Fx.Base.ID _id;
        [QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASSID_name)]
        private string _name;
        [QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASSID_comment)]
        private string _comment;
        private QS.Fx.Reflection.IValueClass _messageclass, _checkpointclass;
        private QS.Fx.Reflection.IComponentClass _wrapperclass;
        private ICollection<QsmApplicationConnection_> _connections = 
            new System.Collections.ObjectModel.Collection<QsmApplicationConnection_>();
        private int _clientcount;
        private bool _pending_checkpoint;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

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

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Reference

        public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Reference
        {
            get
            {
                lock (this)
                {
                    QsmApplicationConnection_ _connection = new QsmApplicationConnection_(_mycontext, this);
                    this._connections.Add(_connection);
                    try
                    {
                        QS.Fx.Reflection.IParameter _class_parameter =
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>) _wrapperclass).ClassParameters["underlying_endpoint"];
                        QS.Fx.Reflection.IComponentClass _initialized_wrapperclass =
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>) _wrapperclass).Instantiate
                            (
                                new QS.Fx.Reflection.IParameter[]
                                    {
                                        new QS.Fx.Reflection.Parameter
                                        (
                                            _class_parameter.ID, 
                                            _class_parameter.Attributes, 
                                            _class_parameter.ParameterClass, 
                                            _class_parameter.ValueClass,
                                            null, 
                                            new QS._qss_x_.Endpoint_.Reference<
                                                QS.Fx.Endpoint.Classes.IDualInterface<
                                                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                                                        QS.Fx.Serialization.ISerializable, 
                                                        QS.Fx.Serialization.ISerializable>,
                                                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                                                        QS.Fx.Serialization.ISerializable, 
                                                        QS.Fx.Serialization.ISerializable>>>
                                            (
                                                ((QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                                                    QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>) _connection).Channel
                                            )
                                        )
                                    }
                            );
                        return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(
                            _initialized_wrapperclass, new QS.Fx.Attributes.Attributes(this));
                    }
                    catch (Exception _exc)
                    {
                        this._connections.Remove(_connection);
                        throw new Exception("Could not create a reference to the channel.", _exc);
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Open

        public void _Open(QsmApplicationConnection_ _connection)
        {
            lock (this._qsm)
            {
                lock (this)
                {
                    if (this._clientcount == 0)
                    {
                        this._pending_checkpoint = true;
                        this._qsm._Open(this);
                    }
                    else
                    {
                        uint _seqno;
                        QS.Fx.Serialization.ISerializable _checkpoint;
                        if (!this._Checkpoint(0, out _seqno, out _checkpoint))
                        {
                            if (!this._pending_checkpoint)
                            {
                                if (!System.Diagnostics.Debugger.IsAttached)
                                {
                                    System.Diagnostics.Debugger.Launch();
                                    System.Diagnostics.Debugger.Break();
                                }
                                throw new Exception();
                            }
                        }
                        this._Initialize(_seqno, _checkpoint);
                    }
                    this._clientcount++;
                }
            }
        }

        #endregion

        #region _Close

        public void _Close(QsmApplicationConnection_ _connection)
        {
            lock (this._qsm)
            {
                lock (this)
                {
                    this._clientcount--;
                    if (this._clientcount == 0)
                    {
                        this._pending_checkpoint = false;
                        this._qsm._Close(this);
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Send

        public void _Send(QsmApplicationConnection_ _connection, QS.Fx.Serialization.ISerializable _message)
        {            
            this._qsm._Send(this, _message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Checkpoint

        public bool _Checkpoint(uint _minimum_seqno, out uint _seqno, out QS.Fx.Serialization.ISerializable _checkpoint)
        {
            foreach (QsmApplicationConnection_ _connection in this._connections)
            {
                if (_connection._Checkpoint(_minimum_seqno, out _seqno, out _checkpoint))
                    return true;
            }
            _seqno = 0;
            _checkpoint = null;
            return false;
        }

        #endregion

        #region _Initialize

        public void _Initialize(uint _seqno, QS.Fx.Serialization.ISerializable _checkpoint)
        {
            foreach (QsmApplicationConnection_ _connection in this._connections)
                _connection._Initialize(_seqno, _checkpoint);
            _pending_checkpoint = false;
        }

        #endregion

        #region _Receive

        public void _Receive(uint _seqno, QS.Fx.Serialization.ISerializable _message)
        {
            foreach (QsmApplicationConnection_ _connection in this._connections)
                _connection._Receive(_seqno, _message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
