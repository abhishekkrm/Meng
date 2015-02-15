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
using System.Diagnostics;

namespace QS._qss_x_.Centralized_CC_
{
    public sealed class Controller_SVR
        : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Interface_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>>
    {
        #region Contructor

        public Controller_SVR(
            QS.Fx.Object.IContext _mycontext,
            string _rootfolder,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer)
        {
            this._mycontext = _mycontext;
            this._rootfolder = QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + _rootfolder;
            this._deserializer = _deserializer;
            this._deserializer_connection = 
                QS._qss_x_.Component_.Classes_.Service<QS._qss_x_.Interface_.Classes_.IDeserializer>.Connect(
                    _mycontext,
                    _deserializer.Dereference(_mycontext),
                    out this._deserializer_interface);

            this._Rescan();
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private string _rootfolder;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _deserializer_connection;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Interface_.Classes_.IDeserializer _deserializer_interface;
        [QS.Fx.Base.Inspectable]
        private DateTime _lastchecked = DateTime.MinValue;
        
        private ICollection<_Client> _clients = new System.Collections.ObjectModel.Collection<_Client>();
        private IDictionary<QS.Fx.Base.ID, _Channel> _channels = new Dictionary<QS.Fx.Base.ID, _Channel>();

        #endregion

        #region IFactory2<IDualInterface<ICommunicationChannelClient<IMessage>,ICommunicationChannel<IMessage>>> Members

        QS.Fx.Endpoint.IReference<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>, 
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>> 
            QS._qss_x_.Interface_.Classes_.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>, 
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>>.Create()
        {
            lock (this)
            {
                _Client _client = new _Client(_mycontext, this);
                this._clients.Add(_client);
                return new QS._qss_x_.Endpoint_.Reference<
                    QS.Fx.Endpoint.Classes.IDualInterface<
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>>(_client.Endpoint);
            }
        }

        #endregion

        #region _Rescan

        private void _Rescan()
        {
            lock (this)
            {
                DateTime _now = DateTime.Now;
                if ((_now - this._lastchecked).TotalMilliseconds > 100)
                {
                    this._lastchecked = _now;
                    if (Directory.Exists(this._rootfolder))
                    {
                        foreach (string _folder in Directory.GetDirectories(this._rootfolder))
                        {
                            QS.Fx.Base.ID _id = new QS.Fx.Base.ID(_folder.Substring(_folder.LastIndexOf(Path.DirectorySeparatorChar) + 1));
                            if (!this._channels.ContainsKey(_id))
                            {
                                string _metadata_filename = _folder + Path.DirectorySeparatorChar + "metadata.xml";
                                if (!File.Exists(_metadata_filename))
                                    throw new Exception("Could not access channel metadata of \"" + _id.ToString() + "\".");
                                _Metadata _metadata;
                                using (StreamReader _reader = new StreamReader(_metadata_filename))
                                {
                                    _metadata = (_Metadata)(new XmlSerializer(typeof(_Metadata))).Deserialize(_reader);
                                }
                                if (!new QS.Fx.Base.ID(_metadata.ID).Equals(_id))
                                    throw new Exception("Bad id in the metadata.");
                                QS.Fx.Reflection.IValueClass _messageclass = this._deserializer_interface.DeserializeValueClass(_metadata.MessageClass);
                                QS.Fx.Reflection.IValueClass _checkpointclass = this._deserializer_interface.DeserializeValueClass(_metadata.CheckpointClass);
                                _Channel _channel = new _Channel(this, _id, _metadata.Name, _metadata.Comment, _folder, _messageclass, _checkpointclass);
                                this._channels.Add(_id, _channel);
                                foreach (_Client _client in this._clients)
                                {
                                    _client.Receive
                                    (
                                        new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                                        (
                                            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.ChannelCreated,
                                            _channel.ID,
                                            0,
                                            new QS._qss_x_.Channel_.Message_.Centralized_CC.Metadata
                                            (
                                                _channel.Name,
                                                _channel.Comment,
                                                _channel.MessageClass.Serialize,
                                                _channel.CheckpointClass.Serialize
                                            )
                                        )
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Class _Metadata

        [XmlType("Metadata")]
        public class _Metadata
        {
            public _Metadata(string _id, string _name, string _comment, 
                QS.Fx.Reflection.Xml.ValueClass _messageclass, QS.Fx.Reflection.Xml.ValueClass _checkpointclass)
            {
                this._id = _id;
                this._name = _name;
                this._comment = _comment;
                this._messageclass = _messageclass;
                this._checkpointclass = _checkpointclass;
            }

            public _Metadata()
            {
            }

            private string _id, _name, _comment;
            private QS.Fx.Reflection.Xml.ValueClass _messageclass, _checkpointclass;

            [XmlElement("ID")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [XmlElement("Name")]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            [XmlElement("Comment")]
            public string Comment
            {
                get { return _comment; }
                set { _comment = value; }
            }

            [XmlElement("MessageClass")]
            public QS.Fx.Reflection.Xml.ValueClass MessageClass
            {
                get { return _messageclass; }
                set { _messageclass = value; }
            }

            [XmlElement("CheckpointClass")]
            public QS.Fx.Reflection.Xml.ValueClass CheckpointClass
            {
                get { return _checkpointclass; }
                set { _checkpointclass = value; }
            }
        }

        #endregion

        #region Class _Channel

        private sealed class _Channel : QS.Fx.Inspection.Inspectable
        {
            #region Constructor

            public _Channel(Controller_SVR _controller, QS.Fx.Base.ID _id, string _name, string _comment, string _root,
                QS.Fx.Reflection.IValueClass _messageclass, QS.Fx.Reflection.IValueClass _checkpointclass)
            {
                this._controller = _controller;
                this._id = _id;
                this._name = _name;
                this._comment = _comment;
                this._root = _root;
                this._messageclass = _messageclass;
                this._checkpointclass = _checkpointclass;
            }

            #endregion

            #region Fields

            private Controller_SVR _controller;
            private QS.Fx.Base.ID _id;
            private string _name, _comment, _root;
            private QS.Fx.Reflection.IValueClass _messageclass, _checkpointclass;
            private ICollection<_Connection> _connections = new System.Collections.ObjectModel.Collection<_Connection>();
            private ICollection<_Connection> _initialized = new System.Collections.ObjectModel.Collection<_Connection>();
            private ICollection<_Connection> _uninitialized = new System.Collections.ObjectModel.Collection<_Connection>();
            private Timer _timer;
            private uint _last_message_seqno;
            private Queue<KeyValuePair<uint, QS.Fx.Serialization.ISerializable>> _cached_messages = 
                new Queue<KeyValuePair<uint,QS.Fx.Serialization.ISerializable>>();

            #endregion

            #region Accessors

            public QS.Fx.Base.ID ID
            {
                get { return _id; }
            }

            public string Name
            {
                get { return _name; }
            }

            public string Comment
            {
                get { return _comment; }
            }

            public QS.Fx.Reflection.IValueClass MessageClass
            {
                get { return _messageclass; }
            }

            public QS.Fx.Reflection.IValueClass CheckpointClass
            {
                get { return _checkpointclass; }
            }

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

            #endregion

            #region TakeCheckpoint

            public void TakeCheckpoint()
            {
                if (this._uninitialized.Count > 0)
                {
                    if (this._initialized.Count > 0)
                    {
                        IEnumerator<_Connection> _e = this._initialized.GetEnumerator();
                        if (!_e.MoveNext())
                            throw new Exception("Could not enumerate connections.");
                        _Connection _connection = _e.Current;
                        _connection.Client.Receive
                        (
                            new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                            (
                                QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.TakeCheckpoint,
                                this._id,
                                this._last_message_seqno,
                                null
                            )
                        );
                        _timer = new Timer(new TimerCallback(this._OnCheckpointTimeout), null, 1000, Timeout.Infinite);
                    }
                    else
                        this.Checkpoint(this._last_message_seqno, null);
                }
                else
                {
                    if (_timer != null)
                        _timer.Dispose();
                    _timer = null;
                }
            }

            #endregion

            #region _OnCheckpointTimeout

            private void _OnCheckpointTimeout(object _o)
            {
                lock (this._controller)
                {
                    this.TakeCheckpoint();
                }
            }

            #endregion

            #region Message

            public void Message(QS.Fx.Serialization.ISerializable _message)
            {
                uint _seqno = ++_last_message_seqno;
                if (_uninitialized.Count > 0)
                {
                    this._cached_messages.Enqueue(new KeyValuePair<uint, QS.Fx.Serialization.ISerializable>(_seqno, _message));
                }
                foreach (_Connection _connection in this._initialized)
                {
                    _connection.Client.Receive
                    (
                        new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                        (
                            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Message,
                            this._id,
                            _seqno,
                            _message
                        )
                    );
                }
            }

            #endregion

            #region Checkpoint

            public void Checkpoint(uint _seqno, QS.Fx.Serialization.ISerializable _checkpoint)
            {
                while (this._cached_messages.Count > 0 && this._cached_messages.Peek().Key <= _seqno)
                    this._cached_messages.Dequeue();

                foreach (_Connection _connection in this._uninitialized)
                {
                    _connection.Initialized = true;
                    this._initialized.Add(_connection);
                    _connection.Client.Receive
                    (
                        new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                        (
                            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Checkpoint,
                            this._id,
                            _seqno,
                            _checkpoint
                        )
                    );
                    foreach (KeyValuePair<uint, QS.Fx.Serialization.ISerializable> _element in this._cached_messages)
                    {
                        _connection.Client.Receive
                        (
                            new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                            (
                                QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Message,
                                this._id,
                                _element.Key,
                                _element.Value
                            )
                        );
                    }
                }
                this._uninitialized.Clear();
                this._cached_messages.Clear();                
                if (_timer != null)
                    _timer.Dispose();
                _timer = null;
            }

            #endregion

            #region Disconnected

            public void Disconnected(_Connection _connection)
            {
                this._connections.Remove(_connection);
                this._initialized.Remove(_connection);
                this._uninitialized.Remove(_connection);
            }

            #endregion
        }

        #endregion

        #region Class _Connection

        private sealed class _Connection
            : QS.Fx.Inspection.Inspectable
        {
            #region Constructor

            public _Connection(_Client _client, _Channel _channel)
            {
                this._client = _client;
                this._channel = _channel;
            }

            #endregion

            #region Fields

            private _Client _client;
            private _Channel _channel;
            private bool _initialized;

            #endregion

            #region Accessors

            public _Client Client
            {
                get { return this._client; }
            }

            public _Channel Channel
            {
                get { return this._channel; }
            }

            public bool Initialized
            {
                get { return _initialized; }
                set { _initialized = value; }
            }

            #endregion
        }

        #endregion

        #region Class _Client

        private sealed class _Client
            : QS.Fx.Inspection.Inspectable,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>
        {
            #region Constructor

            public _Client(QS.Fx.Object.IContext _mycontext, Controller_SVR _controller)
            {
                this._controller = _controller;
                
                this._endpoint = 
                    _mycontext.DualInterface<
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>(this);

                this._endpoint.OnConnect += new QS.Fx.Base.Callback(this._OnConnect);
                this._endpoint.OnConnected += new QS.Fx.Base.Callback(this._OnConnected);
                this._endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);
            }

            #endregion

            #region Fields

            private Controller_SVR _controller;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>> _endpoint;
            private string _name;
            private Queue<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage> _outgoing =
                new Queue<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>();
            private IDictionary<QS.Fx.Base.ID, _Connection> _connections = new Dictionary<QS.Fx.Base.ID, _Connection>();

            #endregion

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

            #region _OnDisconnect

            private void _OnDisconnect()
            {
                lock (this._controller)
                {
                    this._controller._clients.Remove(this);
                    foreach (_Connection _connection in this._connections.Values)
                        _connection.Channel.Disconnected(_connection);
                }
            }

            #endregion

            #region Endpoint

            public QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>> Endpoint
            {
                get { return this._endpoint; }
            }

            #endregion

            #region ICommunicationChannel<IMessage> Members

            void QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<
                QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>._Send(
                    QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
                lock (this._controller)
                {
                    switch (_message.MessageType)
                    {
                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Hello:
                            this._HandleIncoming_Hello(_message);
                            break;

                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.OpenChannel:
                            this._HandleIncoming_OpenChannel(_message);
                            break;

                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.CloseChannel:
                            this._HandleIncoming_CloseChannel(_message);
                            break;

                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Message:
                            this._HandleIncoming_Message(_message);
                            break;

                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Checkpoint:
                            this._HandleIncoming_Checkpoint(_message);
                            break;

                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.ChannelCreated:
                        case QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.TakeCheckpoint:
                            throw new Exception("Message of type \"" + _message.MessageType.ToString() + "\" is not expected to ever be seen on the server.");

                        default:
                            throw new NotImplementedException();
                    }

                    while (_outgoing.Count > 0)
                    {
                        QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _m = _outgoing.Dequeue();
                        this._endpoint.Interface._Receive(_m);
                    }
                }
            }

            #endregion 

            #region Receive

            public void Receive(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
                lock (this._controller)
                {
                    _outgoing.Enqueue(_message);
                    while (_outgoing.Count > 0)
                    {
                        QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _m = _outgoing.Dequeue();
                        this._endpoint.Interface._Receive(_m);
                    }
                }
            }

            #endregion

            #region _HandleIncoming_Hello

            private void _HandleIncoming_Hello(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
                // FIX
                this._controller._Rescan();

                this._name = ((QS.Fx.Value.Classes.IText) _message.Object).Text;

                foreach (_Channel _channel in this._controller._channels.Values)
                {
                    _outgoing.Enqueue
                    (
                        new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                        (
                            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.ChannelCreated,
                            _channel.ID,
                            0,
                            new QS._qss_x_.Channel_.Message_.Centralized_CC.Metadata
                            (
                                _channel.Name,
                                _channel.Comment,
                                _channel.MessageClass.Serialize,
                                _channel.CheckpointClass.Serialize
                            )
                        )
                    );
                }
                _outgoing.Enqueue
                (
                    new QS._qss_x_.Channel_.Message_.Centralized_CC.Message
                    (
                        QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType.Hello,
                        QS.Fx.Base.ID.Undefined,
                        0,
                        null
                    )
                );
            }

            #endregion

            #region _HandleIncoming_OpenChannel

            private void _HandleIncoming_OpenChannel(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
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
            }

            #endregion

            #region _HandleIncoming_CloseChannel

            private void _HandleIncoming_CloseChannel(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
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
            }

            #endregion

            #region _HandleIncoming_Message

            private void _HandleIncoming_Message(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
                _Connection _connection;
                if (!this._connections.TryGetValue(_message.Channel, out _connection))
                    throw new Exception("Client \"" + this._name + "\" is trying to send a message even though it is not connected.");
                _connection.Channel.Message(_message.Object);
            }

            #endregion

            #region _HandleIncoming_Checkpoint

            private void _HandleIncoming_Checkpoint(QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage _message)
            {
                _Connection _connection;
                if (!this._connections.TryGetValue(_message.Channel, out _connection))
                    throw new Exception("Client \"" + this._name + "\" is trying to provide a checkpoint even though it is not connected.");
                _connection.Channel.Checkpoint(_message.SequenceNo, _message.Object);
            }

            #endregion
        }

        #endregion
    }
}
