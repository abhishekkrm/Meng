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
    public sealed class QsmApplication_ :
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS._core_c_.Core.IChannel,
        IDisposable
    {
        #region Constructor

        public QsmApplication_
        (
            QS.Fx.Object.IContext _mycontext, 
            QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer
        )
        {
            this._mycontext = _mycontext;
            this._myquicksilver = _myquicksilver;
            this._deserializer = _deserializer;
            this._deserializer_connection =
                QS._qss_x_.Component_.Classes_.Service<QS._qss_x_.Interface_.Classes_.IDeserializer>.Connect(
                    _mycontext,
                    _deserializer.Dereference(_mycontext),
                    out this._deserializer_interface);
        }

        #endregion

        #region Destructor

        ~QsmApplication_()
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
        private QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        private QS.Fx.Endpoint.IConnection _deserializer_connection;
        private QS._qss_x_.Interface_.Classes_.IDeserializer _deserializer_interface;
        private QS._core_c_.Core.IChannel _uplink;
        private int _disposed;
        private bool _ready;
        private ManualResetEvent _ready_event = new ManualResetEvent(false);
        private IDictionary<QS.Fx.Base.ID, QsmApplicationChannel_> _channels1 = new Dictionary<QS.Fx.Base.ID, QsmApplicationChannel_>();
        private IDictionary<long, QsmApplicationChannel_> _channels2 = new Dictionary<long, QsmApplicationChannel_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDictionary<string,IObject> Members

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            this._Synchronize();
            lock (this)
            {
                List<string> _keys = new List<string>();
                foreach (QS.Fx.Base.ID _id in this._channels1.Keys)
                    _keys.Add(_id.ToString());
                return _keys;
            }
        }

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            this._Synchronize();
            lock (this)
            {
                List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
                foreach (QsmApplicationChannel_ _channel in this._channels1.Values)
                    _objects.Add(_channel._Reference);
                return _objects;
            }
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
        {
            this._Synchronize();
            lock (this)
            {
                QS.Fx.Base.ID _id;
                try
                {
                    _id = new QS.Fx.Base.ID(_key);
                }
                catch (Exception)
                {
                    return false;
                }
                return this._channels1.ContainsKey(_id);
            }
        }

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            this._Synchronize();
            lock (this)
            {
                QS.Fx.Base.ID _id;
                try
                {
                    _id = new QS.Fx.Base.ID(_key);
                }
                catch (Exception)
                {
                    _id = null;
                }
                QsmApplicationChannel_ _channel;
                if ((_id == null) || !this._channels1.TryGetValue(_id, out _channel))
                    throw new Exception("No channel named \"" + _key + "\" has been defined.");
                return _channel._Reference;
            }
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            this._Synchronize();
            lock (this)
            {
                QS.Fx.Base.ID _id;
                try
                {
                    _id = new QS.Fx.Base.ID(_key);
                }
                catch (Exception)
                {
                    _object = null;
                    return false;
                }
                QsmApplicationChannel_ _channel;
                if (this._channels1.TryGetValue(_id, out _channel))
                {
                    _object = _channel._Reference;
                    return true;
                }
                else
                {
                    _object = null;
                    return false;
                }
            }
        }

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            throw new NotSupportedException("This interface does not permit adding communication channels.");
        }

        #region IDictionary<string,IObject>.Remove

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Remove(
            string _key)
        {
            throw new NotImplementedException();
        }

        #endregion

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            this._Synchronize();
            lock (this)
            {
                return this._channels1.Count;
            }
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            return true;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect

        public void _Connect(QS._core_c_.Core.IChannel _uplink)
        {
            this._uplink = _uplink;
            this._uplink.Handle
            (
                new QS._core_c_.Core.ChannelObject
                (
                    (int)QsmOperation_.Connect_,
                    0,
                    0,
                    0,
                    0,
                    null
                )
            );
        }

        #endregion

        #region _Synchronize

        private void _Synchronize()
        {
            if (!this._ready)
                this._ready_event.WaitOne();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Open

        public void _Open(QsmApplicationChannel_ _channel)
        {
            this._uplink.Handle
            (
                new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Open_,
                    _channel._Channel,
                    0,
                    0,
                    0,
                    null
                )
            );
        }

        #endregion

        #region _Close

        public void _Close(QsmApplicationChannel_ _channel)
        {
            this._uplink.Handle
            (
                new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Close_,
                    _channel._Channel,
                    0,
                    0,
                    0,
                    null
                )
            );
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IChannel Members

        void QS._core_c_.Core.IChannel.Handle(QS._core_c_.Core.ChannelObject _message)
        {
            QsmOperation_ _operation = (QsmOperation_) _message.operation;
            if (_message.datablocks != null)
            {
                if (_message.datablocks.Count != 1)
                    throw new Exception("Incoming channel object was expected to consist of a single contiguous data block.");
            }
            lock (this)
            {
                switch (_operation)
                {
                    case QsmOperation_.Connected_:
                        this._HandleIncoming_Connected(_message);
                        break;
                    case QsmOperation_.Created_:
                        this._HandleIncoming_Created(_message);
                        break;
                    case QsmOperation_.Initialize_:
                        this._HandleIncoming_Initialize(_message);
                        break;
                    case QsmOperation_.Checkpoint_:
                        this._HandleIncoming_Checkpoint(_message);
                        break;
                    case QsmOperation_.Receive_:
                        this._HandleIncoming_Receive(_message);
                        break;
                    case QsmOperation_.Connect_:
                    case QsmOperation_.Create_:
                    case QsmOperation_.Open_:
                    case QsmOperation_.Close_:
                    case QsmOperation_.Send_:
                        throw new Exception("Operation of type \"" + _operation.ToString() + "\" was not expected in this context.");
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _HandleIncoming_Connected

        private void _HandleIncoming_Connected(QS._core_c_.Core.ChannelObject _message)
        {
            this._ready = true;
            this._ready_event.Set();
        }

        #endregion

        #region _HandleIncoming_Created

        private void _HandleIncoming_Created(QS._core_c_.Core.ChannelObject _message)
        {
            QS.Fx.Serialization.ISerializable _data;
            QsmHelpers_._Deserialize
            (
                _message.datalength,
                _message.datablocks,
                out _data
            );
            QsmMetadata_ _metadata = (QsmMetadata_) _data;
            QS.Fx.Base.ID _id = new QS.Fx.Base.ID(_metadata._ID);
            if (this._channels1.ContainsKey(_id))
                throw new Exception("Channel \"" + _id.ToString() + "\" already exists.");
            QS.Fx.Reflection.IValueClass _messageclass = this._deserializer_interface.DeserializeValueClass(_metadata._MessageClass);
            QS.Fx.Reflection.IValueClass _checkpointclass = this._deserializer_interface.DeserializeValueClass(_metadata._CheckpointClass);
            QS.Fx.Reflection.IComponentClass _wrapperclass =
                QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Channel_2);
            _wrapperclass =
                ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>) _wrapperclass).Instantiate
                (
                    new QS.Fx.Reflection.IParameter[]
                    {
                        new QS.Fx.Reflection.Parameter("MessageClass", null, QS.Fx.Reflection.ParameterClass.ValueClass, null, null, _messageclass),
                        new QS.Fx.Reflection.Parameter("CheckpointClass", null, QS.Fx.Reflection.ParameterClass.ValueClass, null, null, _checkpointclass)
                    }
                );
            QsmApplicationChannel_ _channel =
                new QsmApplicationChannel_
                (
                    _mycontext,
                    this,
                    _message.channel,
                    _id,
                    _metadata._Name,
                    _metadata._Comment,
                    _messageclass,
                    _checkpointclass,
                    _wrapperclass                    
                );
            this._channels1.Add(_id, _channel);
            this._channels2.Add(_message.channel, _channel);
        }

        #endregion

        #region _HandleIncoming_Initialize

        private void _HandleIncoming_Initialize(QS._core_c_.Core.ChannelObject _message)
        {
            QsmApplicationChannel_ _channel;
            if (this._channels2.TryGetValue(_message.channel, out _channel))
            {
                QS.Fx.Serialization.ISerializable _data;
                QsmHelpers_._Deserialize
                (
                    _message.datalength,
                    _message.datablocks,
                    out _data
                );
                _channel._Initialize((uint) _message.sequenceno, _data); 
            }
        }

        #endregion

        #region _HandleIncoming_Checkpoint

        private void _HandleIncoming_Checkpoint(QS._core_c_.Core.ChannelObject _message)
        {
            QsmApplicationChannel_ _channel;
            if (this._channels2.TryGetValue(_message.channel, out _channel))
            {
                QS.Fx.Serialization.ISerializable _checkpoint;
                uint _seqno;
                if (_channel._Checkpoint((uint) _message.sequenceno, out _seqno, out _checkpoint))
                {                    
                    int _datalength;
                    IList<QS.Fx.Base.Block> _datablocks;
                    QsmHelpers_._Serialize
                    (
                        _checkpoint,
                        out _datalength,
                        out _datablocks
                    );
                    this._uplink.Handle
                    (
                        new QS._core_c_.Core.ChannelObject
                        (
                            (int) QsmOperation_.Initialize_,
                            _channel._Channel,
                            0,
                            (int) _seqno,
                            _datalength,
                            _datablocks
                        )
                    );
                }
            }
        }

        #endregion

        #region _HandleIncoming_Receive

        private void _HandleIncoming_Receive(QS._core_c_.Core.ChannelObject _message)
        {
            QsmApplicationChannel_ _channel;
            if (this._channels2.TryGetValue(_message.channel, out _channel))
            {
                QS.Fx.Serialization.ISerializable _data;
                QsmHelpers_._Deserialize
                (
                    _message.datalength,
                    _message.datablocks,
                    out _data
                );
                _channel._Receive((uint) _message.sequenceno, _data);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Send

        public unsafe void _Send(QsmApplicationChannel_ _channel, QS.Fx.Serialization.ISerializable _message)
        {
            int _datalength;
            IList<QS.Fx.Base.Block> _datablocks;
            QsmHelpers_._Serialize
            (
                _message,
                out _datalength,
                out _datablocks
            );
            this._uplink.Handle
            (
                new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Send_,
                    _channel._Channel,
                    0,
                    0,
                    _datalength,
                    _datablocks
                )
            );
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
