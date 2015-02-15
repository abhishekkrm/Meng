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
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace QS._qss_x_.Centralized_CC_
{
    public sealed class Controller
        : QS.Fx.Inspection.Inspectable,
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>
    {
        #region Contructor

        public Controller(
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<
                QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>> _connection,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer)
        {
            this._mycontext = _mycontext;
            this._name = System.Net.Dns.GetHostName() + ":" + System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

            this._deserializer = _deserializer;
            this._deserializer_connection = 
                QS._qss_x_.Component_.Classes_.Service<QS._qss_x_.Interface_.Classes_.IDeserializer>.Connect(
                    _mycontext,
                    _deserializer.Dereference(_mycontext),
                    out this._deserializer_interface);

            QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<
                QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage> _channel = _connection.Dereference(_mycontext);

            this._endpoint = 
                _mycontext.DualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>, 
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>(this);

            this._endpoint.OnConnect += new QS.Fx.Base.Callback(this._OnConnect);
            this._endpoint.OnConnected += new QS.Fx.Base.Callback(this._OnConnected);
            this._endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);

            this._connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._endpoint).Connect(_channel._Channel);
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private string _name;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>> _endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _deserializer_connection;
        [QS.Fx.Base.Inspectable]        
        private QS._qss_x_.Interface_.Classes_.IDeserializer _deserializer_interface;
        [QS.Fx.Base.Inspectable]
        private bool _ready;
        private ManualResetEvent _ready_event = new ManualResetEvent(false);
        private IDictionary<QS.Fx.Base.ID, _Channel> _channels = new Dictionary<QS.Fx.Base.ID, _Channel>();

        #endregion

        #region _OnConnect

        private void _OnConnect()
        {
        }

        #endregion

        #region _OnConnected

        private void _OnConnected()
        {
            _endpoint.Interface._Send
            (
                new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                (
                    QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Hello,
                    QS.Fx.Base.ID.Undefined,
                    0,
                    new QS.Fx.Value.UnicodeText(_name)
                )
            );
        }

        #endregion

        #region _OnDisconnect

        private void _OnDisconnect()
        {
            System.Windows.Forms.MessageBox.Show(
                "DISCONNECTED",
                "QS.Fx.Centralized_CC.Controller received a Disconnect callback from the underlying channel to the server.",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);     
        }

        #endregion

        #region IDictionary<string, IObject> Members

        #region IDictionary<string,IObject>.Keys

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            this._Synchronize();
            lock (this)
            {
                List<string> _keys = new List<string>();
                foreach (QS.Fx.Base.ID _id in this._channels.Keys)
                    _keys.Add(_id.ToString());
                return _keys;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.Objects

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> 
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            this._Synchronize();
            lock (this)
            {
                List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
                foreach (_Channel _channel in this._channels.Values)
                    _objects.Add(_channel.Reference);
                return _objects;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.ContainsKey

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
                return this._channels.ContainsKey(_id);
            }
        }

        #endregion

        #region IDictionary<string,IObject>.GetObject

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
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
                _Channel _channel;
                if ((_id == null) || !this._channels.TryGetValue(_id, out _channel))
                    throw new Exception("No channel named \"" + _key + "\" has been defined.");
                return _channel.Reference;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.TryGetObject

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(
            string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
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
                _Channel _channel;
                if (this._channels.TryGetValue(_id, out _channel))
                {
                    _object = _channel.Reference;
                    return true;
                }
                else
                {
                    _object = null;
                    return false;
                }
            }
        }

        #endregion

        #region IDictionary<string,IObject>.Add

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(
            string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            throw new Exception("This interface does not permit adding communication channels.");
        }

        #endregion

        #region IDictionary<string,IObject>.Remove

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Remove(
            string _key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDictionary<string,IObject>.Count

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            this._Synchronize();
            lock (this)
            {
                return this._channels.Count;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.IsReadOnly

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            return true;
        }

        #endregion

        #endregion

        #region _Synchronize

        private void _Synchronize()
        {
            if (!this._ready)
                this._ready_event.WaitOne();
        }

        #endregion

        #region _CreateChannel

        private void _CreateChannel(QS.Fx.Base.ID _id, QS._qss_x_.Channel_.Message_.Centralized_CC.IMetadata _metadata)
        {
            _Channel _channel;
            if (!_channels.TryGetValue(_id, out _channel))
            {
                QS.Fx.Reflection.IValueClass _messageclass = this._deserializer_interface.DeserializeValueClass(_metadata.MessageClass);
                QS.Fx.Reflection.IValueClass _checkpointclass = this._deserializer_interface.DeserializeValueClass(_metadata.CheckpointClass);

                QS.Fx.Reflection.IComponentClass _wrapperclass =
                    QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Channel_2);

                _wrapperclass =
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_wrapperclass).Instantiate
                    (
                        new QS.Fx.Reflection.IParameter[]
                        {
                            new QS.Fx.Reflection.Parameter("MessageClass", null, QS.Fx.Reflection.ParameterClass.ValueClass, null, null, _messageclass),
                            new QS.Fx.Reflection.Parameter("CheckpointClass", null, QS.Fx.Reflection.ParameterClass.ValueClass, null, null, _checkpointclass)
                        }
                    );

                _channel = new _Channel(this, _id, _metadata.Name, _metadata.Comment, _messageclass, _checkpointclass, _wrapperclass);
                this._channels.Add(_id, _channel);
            }
        }

        #endregion

        #region ICommunicationChannelClient<IMessage> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<
            QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>._Receive(
                QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
        {
            lock (this)
            {
                switch (_message.MessageType)
                {
                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Hello:
                        this._HandleIncoming_Hello(_message);
                        break;

                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.ChannelCreated:
                        this._HandleIncoming_ChannelCreated(_message);
                        break;

                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Message:
                        this._HandleIncoming_Message(_message);
                        break;

                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.TakeCheckpoint:
                        this._HandleIncoming_TakeCheckpoint(_message);
                        break;

                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Checkpoint:
                        this._HandleIncoming_Checkpoint(_message);
                        break;

                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.OpenChannel:
                    case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.CloseChannel:
                        throw new Exception("Message of type \"" + _message.MessageType.ToString() + "\" is not expected to ever be seen on the client.");

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region _HandleIncoming_Hello

        private void _HandleIncoming_Hello(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
        {
            this._ready = true;
            this._ready_event.Set();
        }

        #endregion

        #region _HandleIncoming_ChannelCreated

        private void _HandleIncoming_ChannelCreated(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
        {
            this._CreateChannel(_message.Channel,
                (QS._qss_x_.Channel_.Message_.Centralized_CC.IMetadata)_message.Object);
        }

        #endregion

        #region _HandleIncoming_Message

        private void _HandleIncoming_Message(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
        {
            _Channel _channel;
            if (this._channels.TryGetValue(_message.Channel, out _channel))
                _channel.Message(_message.SequenceNo, _message.Object);
        }

        #endregion

        #region _HandleIncoming_TakeCheckpoint

        private void _HandleIncoming_TakeCheckpoint(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
        {
            _Channel _channel;
            if (this._channels.TryGetValue(_message.Channel, out _channel))
            {
                QS.Fx.Serialization.ISerializable _checkpoint;
                uint _seqno;
                if (_channel.TakeCheckpoint(_message.SequenceNo, out _seqno, out _checkpoint))
                {
                    this._endpoint.Interface._Send
                    (
                        new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                        (
                            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Checkpoint,
                            _channel.ID,
                            _seqno,
                            _checkpoint
                        )
                    );
                }
            }
        }

        #endregion

        #region _HandleIncoming_Checkpoint

        private void _HandleIncoming_Checkpoint(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
        {
            _Channel _channel;
            if (this._channels.TryGetValue(_message.Channel, out _channel))
                _channel.Checkpoint(_message.SequenceNo, _message.Object);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Dictionary

        public QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> Dictionary
        {
            get
            {
                lock (this)
                {
                    _Dictionary _dictionary = new _Dictionary(_mycontext, this);

                    // for now, just do nothing

                    return _dictionary;
                }
            }
        }

        #endregion

        #region _Disconnect

        private void _Disconnect(_Dictionary _dictionary)
        {
            lock (this)
            {
                // for now, just do nothing
            }
        }

        #endregion

        #region Class _Dictionary

        private sealed class _Dictionary : QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>, IDisposable
        {
            #region Constructor

            public _Dictionary(QS.Fx.Object.IContext _mycontext, Controller _controller)
            {
                this._controller = _controller;
                this._endpoint = 
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>, 
                        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this._controller);
            }

            #endregion

            #region Fields

            private Controller _controller;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _endpoint;

            #endregion

            #region IDictionary<string,IObject> Members

            QS.Fx.Endpoint.Classes.IDualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>, 
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> 
                QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
            {
                get { return this._endpoint; }
            }

            #endregion

            #region IsConnected

            public bool IsConnected
            {
                get
                {
                    lock (this)
                    {
                        return this._endpoint.IsConnected;
                    }
                }
            }

            #endregion

            #region Client

            public QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject> Client
            {
                get
                {
                    lock (this)
                    {
                        return (this._endpoint.IsConnected) ? this._endpoint.Interface : null;
                    }
                }
            }

            #endregion

            #region Finalizer

            ~_Dictionary()
            {
                Dispose(false);
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            #region Dispose

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _controller._Disconnect(this);
                }
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class _Channel

        private sealed class _Channel : QS.Fx.Inspection.Inspectable
        {
            #region Constructor

            public _Channel(
                Controller _controller, QS.Fx.Base.ID _id, string _name, string _comment,
                QS.Fx.Reflection.IValueClass _messageclass, QS.Fx.Reflection.IValueClass _checkpointclass,
                QS.Fx.Reflection.IComponentClass _wrapperclass)
            {
                this._controller = _controller;
                this._id = _id;
                this._name = _name;
                this._comment = _comment;
                this._messageclass = _messageclass;
                this._checkpointclass = _checkpointclass;
                this._wrapperclass = _wrapperclass;
            }

            #endregion

            #region Fields

            private Controller _controller;
            private QS.Fx.Base.ID _id;
            [QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASSID_name)]
            private string _name;
            [QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASSID_comment)]
            private string _comment;
            private QS.Fx.Reflection.IValueClass _messageclass, _checkpointclass;
            private QS.Fx.Reflection.IComponentClass _wrapperclass;
            private int _clientcount;
            private ICollection<_Connection> _connections = new System.Collections.ObjectModel.Collection<_Connection>();
            private bool _pending_checkpoint;

            #endregion

            #region Reference

            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> Reference
            {
                get
                {
                    lock (this)
                    {
                        _Connection _connection = new _Connection(_controller._mycontext, this);
                        this._connections.Add(_connection);

                        try
                        {
                            QS.Fx.Reflection.IParameter _class_parameter =
                                ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_wrapperclass).ClassParameters["underlying_endpoint"];

                            QS.Fx.Reflection.IComponentClass _initialized_wrapperclass =
                                ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_wrapperclass).Instantiate
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

                            return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(_initialized_wrapperclass, new QS.Fx.Attributes.Attributes(this));
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

            #region Accessors

            public QS.Fx.Base.ID ID
            {
                get { return this._id; }
            }

            public QS.Fx.Reflection.IValueClass MessageClass
            {
                get { return this._messageclass; }
            }

            public QS.Fx.Reflection.IValueClass CheckpointClass
            {
                get { return this._checkpointclass; }
            }

            #endregion

            #region TakeCheckpoint

            public bool TakeCheckpoint(uint _minimum_seqno, out uint _seqno, out QS.Fx.Serialization.ISerializable _checkpoint)
            {
                foreach (_Connection _connection in this._connections)
                {
                    if (_connection.TakeCheckpoint(_minimum_seqno, out _seqno, out _checkpoint))
                        return true;
                }
                _seqno = 0;
                _checkpoint = null;
                return false;
            }

            #endregion

            #region Checkpoint

            public void Checkpoint(uint _seqno, QS.Fx.Serialization.ISerializable _checkpoint)
            {
                foreach (_Connection _connection in this._connections)
                    _connection.Checkpoint(_seqno, _checkpoint);
                _pending_checkpoint = false;
            }

            #endregion

            #region Message

            public void Message(uint _seqno, QS.Fx.Serialization.ISerializable _message)
            {
                foreach (_Connection _connection in this._connections)
                    _connection.Message(_seqno, _message);
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Class _Connection

            private sealed class _Connection : QS.Fx.Inspection.Inspectable,
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>
            {
                #region Constructor

                public _Connection(QS.Fx.Object.IContext _mycontext, _Channel _channel)
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

                #region Fields

                [QS.Fx.Base.Inspectable]
                private _Channel _channel;
                [QS.Fx.Base.Inspectable]
                private QS.Fx.Endpoint.Internal.IDualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> _endpoint;
                [QS.Fx.Base.Inspectable]
                private bool _isconnected;
                private uint _last_message_seqno;

                #endregion

                #region ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable,QS.Fx.Serialization.ISerializable> Members

                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>
                        QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Channel
                {
                    get { return this._endpoint; }
                }

                #endregion

                #region IsConnected

                public bool IsConnected
                {
                    get { return _isconnected; }
                }

                #endregion

                #region _OnConnect

                private void _OnConnect()
                {
                    lock (this._channel._controller)
                    {
                        lock (this._channel)
                        {
                            if (this._channel._clientcount == 0)
                            {
                                this._channel._pending_checkpoint = true;
                                this._channel._controller._endpoint.Interface._Send
                                (
                                    new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                                    (
                                        QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.OpenChannel,
                                        this._channel._id,
                                        0,
                                        null
                                    )
                                );
                            }
                            else
                            {
                                uint seqno;
                                QS.Fx.Serialization.ISerializable checkpoint;
                                if (!this._channel.TakeCheckpoint(0, out seqno, out checkpoint))
                                {
                                    if (!this._channel._pending_checkpoint)
                                    {
                                        if (!System.Diagnostics.Debugger.IsAttached)
                                        {
                                            System.Diagnostics.Debugger.Launch();
                                            System.Diagnostics.Debugger.Break();
                                        }
                                        throw new Exception("Cannot connect.");
                                    }
                                }
                                this.Checkpoint(seqno, checkpoint);
                            }
                            this._channel._clientcount++;
                        }
                    }
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
                    lock (this._channel._controller)
                    {
                        lock (this._channel)
                        {
                            this._channel._clientcount--;
                            if (this._channel._clientcount == 0)
                            {
                                this._channel._controller._endpoint.Interface._Send
                                (
                                    new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                                    (
                                        QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.CloseChannel,
                                        this._channel._id,
                                        0,
                                        null
                                    )
                                );
                            }
                        }
                    }
                }

                #endregion

                #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

                void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Send(
                    QS.Fx.Serialization.ISerializable _message)
                {
                    this._channel._controller._endpoint.Interface._Send
                    (
                        new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                        (
                            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Message,
                            this._channel._id,
                            0,
                            _message
                        )
                    );
                }

                #endregion

                #region TakeCheckpoint

                public bool TakeCheckpoint(uint _minimum_seqno, out uint _seqno, out QS.Fx.Serialization.ISerializable _checkpoint)
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

                #region Checkpoint

                public void Checkpoint(uint _seqno, QS.Fx.Serialization.ISerializable _checkpoint)
                {
                    if (!this._isconnected || _seqno > this._last_message_seqno)
                    {
                        this._isconnected = true;
                        this._last_message_seqno = _seqno;
                        this._endpoint.Interface.Initialize(_checkpoint);
                    }
                }

                #endregion

                #region Message

                public void Message(uint _seqno, QS.Fx.Serialization.ISerializable _message)
                {
                    if (this._isconnected && _seqno > this._last_message_seqno)
                    {
                        if (_seqno != (this._last_message_seqno + 1))
                            throw new Exception("Received a message with seqno too fat ahead in the future: expecting " + 
                                (this._last_message_seqno + 1).ToString() + ", but received " + _seqno.ToString() + ".");
                        this._endpoint.Interface.Receive(_message);
                        this._last_message_seqno = _seqno;
                    }
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
