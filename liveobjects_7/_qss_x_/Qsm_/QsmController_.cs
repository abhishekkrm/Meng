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

#define DEBUG_CheckThreads

using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace QS._qss_x_.Qsm_
{
    public sealed class QsmController_ : 
        QS._qss_x_.Interface_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>,
        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
        QS._core_c_.Core.IChannelController,
        IDisposable
    {
        #region Constructor

        public QsmController_
        (
            QS.Fx.Object.IContext _mycontext,
            QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver, 
            bool _root,
            string _rootfolder,
            QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QsmControl_>> _connection,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer
        )
        {
            lock (this)
            {
                this._mycontext = _mycontext;
                this._myquicksilver = _myquicksilver;
                this._root = _root;
                this._rootfolder = (_rootfolder != null) ?
                    QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + _rootfolder : null;
                this._connection = _connection;
                this._deserializer = _deserializer;
                this._deserializer_connection =
                    QS._qss_x_.Component_.Classes_.Service<QS._qss_x_.Interface_.Classes_.IDeserializer>.Connect(
                        _mycontext,
                        _deserializer.Dereference(_mycontext),
                        out this._deserializer_interface);
                this._myquicksilver._Core.Schedule(
                    new QS.Fx.Base.Event<object>(
                        new QS.Fx.Base.ContextCallback<object>(this._RescanCallback), null));
                if (this._connection != null)
                {
                    this._channelendpoint = _mycontext.DualInterface<
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>,
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>>(this);
                    this._channelendpointconnection = this._channelendpoint.Connect(_connection.Dereference(_mycontext)._Channel);
                    this._channelendpoint.Interface._Send
                    (
                        new QsmControl_
                        (
                            QsmOperation_.Connect_,
                            0,
                            0,
                            0,
                            null
                        )
                    );
                }
            }
        }

        #endregion

        #region Destructor

        ~QsmController_()
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
        private int _disposed;
        private bool _root;
        private string _rootfolder;
        private QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QsmControl_>> _connection;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        private QS.Fx.Endpoint.IConnection _deserializer_connection;
        private QS._qss_x_.Interface_.Classes_.IDeserializer _deserializer_interface;
        private DateTime _lastchecked = DateTime.MinValue;
        private ICollection<QsmControllerClient_> _clients = new System.Collections.ObjectModel.Collection<QsmControllerClient_>();
        private IDictionary<QS.Fx.Base.ID, QsmControllerChannel_> _controllerchannels = new Dictionary<QS.Fx.Base.ID, QsmControllerChannel_>();
        private IDictionary<long, QsmChannel_> _channels = new Dictionary<long, QsmChannel_>();
        private long _lastchannel;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>> _channelendpoint;
        private QS.Fx.Endpoint.IConnection _channelendpointconnection;
        private bool _ready;
        private IDictionary<string, QsmClient_> _qsmclients = new Dictionary<string, QsmClient_>();
        private QsmControl_ _lastincoming;
        private Stack<QsmControl_> _incoming = new Stack<QsmControl_>();
        private ICollection<QsmChannel_> _subscribing = new System.Collections.ObjectModel.Collection<QsmChannel_>();
        private ICollection<QsmChannel_> _unsubscribing = new System.Collections.ObjectModel.Collection<QsmChannel_>();
        private bool _waitingToIssueMembershipChangeRequest;
        private QS.Fx.Clock.IAlarm _deferredMembershipChangeRequestAlarm;
        private double _batchinginterval = 0.1;
        private bool _subscribedformembershipchanges, _registeredreceivecallback;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IFactory2<IDualInterface<ICommunicationChannelClient<QsmControl_>,ICommunicationChannel<QsmControl_>>> Members

        QS.Fx.Endpoint.IReference<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>> 
        QS._qss_x_.Interface_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>.Create()
        {
            QsmControllerClient_ _client = new QsmControllerClient_(_mycontext, this._myquicksilver, this);
            ManualResetEvent _done = new ManualResetEvent(false);
            this._myquicksilver._Core.Schedule(
                new QS.Fx.Base.Event<QsmControllerClient_, ManualResetEvent>(
                    new QS.Fx.Base.ContextCallback<QsmControllerClient_, ManualResetEvent>(this._CreateCallback),
                    _client,
                    _done));
            _done.WaitOne();                        
            return new QS._qss_x_.Endpoint_.Reference<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>(
                        ((QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QsmControl_>) _client)._Channel);
        }

        private void _CreateCallback(QsmControllerClient_ _client, ManualResetEvent _done)
        {
            lock (this)
            {
                this._clients.Add(_client);
                foreach (QsmControllerChannel_ _channel in this._controllerchannels.Values)
                {
                    ((QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>)_client)._Receive
                    (
                        new QsmControl_
                        (
                            QsmOperation_.Created_,
                            _channel._Channel,
                            0,
                            0,
                            new QsmMetadata_
                            (
                                _channel._ID.ToString(),
                                _channel._Name,
                                _channel._Comment,
                                _channel._MessageClass.Serialize,
                                _channel._CheckpointClass.Serialize,
                                _channel._Index
                            )
                        )
                    );
                }
                _done.Set();
            }
        }

        #endregion

        #region IChannelController Members

        QS._core_c_.Core.IChannel
            QS._core_c_.Core.IChannelController.Open(
                string _id, string _name, QS._core_c_.Core.IChannel _underlyingchannel)
        {
            lock (this)
            {
                if (this._qsmclients.ContainsKey(_id))
                    throw new Exception("A client with id \"" + _id + "\" already exists.");
                QsmClient_ _client = new QsmClient_(this._myquicksilver, this, _id, _name, _underlyingchannel);
                this._qsmclients.Add(_id, _client);
                foreach (QsmControllerChannel_ _channel in this._controllerchannels.Values)
                {
                    int _datalength;
                    IList<QS.Fx.Base.Block> _datablocks;
                    QsmHelpers_._Serialize
                    (
                        new QsmMetadata_
                        (
                            _channel._ID.ToString(),
                            _channel._Name,
                            _channel._Comment,
                            _channel._MessageClass.Serialize,
                            _channel._CheckpointClass.Serialize,
                            _channel._Index
                        ),
                        out _datalength,
                        out _datablocks
                    );
                    _client._Handle
                    (
                        new QS._core_c_.Core.ChannelObject
                        (
                            (int)QsmOperation_.Created_,
                            _channel._Channel,
                            0,
                            0,
                            _datalength,
                            _datablocks
                        )
                    );
                }
                return _client;
            }
        }

        void QS._core_c_.Core.IChannelController.Close(string _id)
        {
            lock (this)
            {
                QsmClient_ _client;
                if (!this._qsmclients.TryGetValue(_id, out _client))
                    throw new Exception("A client with id \"" + _id + "\" does not exist.");
                this._qsmclients.Remove(_id);
                ((IDisposable)_client).Dispose();
            }
        }

        #endregion

        #region ICommunicationChannelClient<QsmControl_> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>._Receive(QsmControl_ _message)
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
            lock (this)
            {
                while (this._incoming.Count > 0)
                {
                    QsmControl_ _message = this._incoming.Pop();
                    switch (_message._operation)
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
                            throw new Exception("Operation of type \"" + _message._operation.ToString() + "\" was not expected in this context.");
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Channel

        public QsmChannel_ _Channel(long _id)
        {
            lock (this)
            {
                QsmChannel_ _channel;
                if (!this._channels.TryGetValue(_id, out _channel))
                    throw new Exception("Cannot open channel \"" + _id.ToString() + "\" because no such channel seems to exist.");
                return _channel;
            }
        }

        #endregion

        #region _ControllerChannel

        public QsmControllerChannel_ _ControllerChannel(long _id)
        {
            lock (this)
            {
                QsmChannel_ _channel;
                if (!this._channels.TryGetValue(_id, out _channel))
                    throw new Exception("Cannot open channel \"" + _id.ToString() + "\" because no such channel seems to exist.");
                return _channel._ControllerChannel;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Open

        public void _Open(QsmChannel_ _channel)
        {
            lock (this)
            {
                if (!this._unsubscribing.Remove(_channel))
                    this._subscribing.Add(_channel);
                this._UpdateMembershipChangeAlarm();
            }
        }

        #endregion

        #region _Close

        public void _Close(QsmChannel_ _channel)
        {
            lock (this)
            {
                if (!this._subscribing.Remove(_channel))
                    this._unsubscribing.Add(_channel);
                this._UpdateMembershipChangeAlarm();
            }            
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _RescanCallback

        private void _RescanCallback(object _o)
        {
            this._Rescan();
        }

        #endregion

        #region _Rescan

        private void _Rescan()
        {
            lock (this)
            {
                if (this._rootfolder != null)
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
                                if (!this._controllerchannels.ContainsKey(_id))
                                {
                                    string _metadata_filename = _folder + Path.DirectorySeparatorChar + "metadata.xml";
                                    if (!File.Exists(_metadata_filename))
                                        throw new Exception("Could not access channel metadata of \"" + _id.ToString() + "\".");
                                    QsmMetadata_ _metadata;
                                    using (StreamReader _reader = new StreamReader(_metadata_filename))
                                    {
                                        _metadata = (QsmMetadata_)(new XmlSerializer(typeof(QsmMetadata_))).Deserialize(_reader);
                                    }
                                    if (!new QS.Fx.Base.ID(_metadata._ID).Equals(_id))
                                        throw new Exception("Bad id in the metadata.");
                                    QS.Fx.Reflection.IValueClass _messageclass = this._deserializer_interface.DeserializeValueClass(_metadata._MessageClass);
                                    QS.Fx.Reflection.IValueClass _checkpointclass = this._deserializer_interface.DeserializeValueClass(_metadata._CheckpointClass);
                                    QsmControllerChannel_ _channel = new QsmControllerChannel_(
                                        this, (++this._lastchannel), _id, _metadata._Name, _metadata._Comment, _messageclass, _checkpointclass, _metadata._Index);
                                    this._controllerchannels.Add(_channel._ID, _channel);
                                    this._channels.Add(_channel._Channel, new QsmChannel_(this._myquicksilver, this, _channel));
                                    foreach (QsmControllerClient_ _client in this._clients)
                                    {
                                        ((QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>)_client)._Receive
                                        (
                                            new QsmControl_
                                            (
                                                QsmOperation_.Created_,
                                                _channel._Channel,
                                                0,
                                                0,
                                                _metadata
                                            )
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _HandleIncoming_Connected

        private void _HandleIncoming_Connected(QsmControl_ _message)
        {
            this._ready = true;
        }

        #endregion

        #region _HandleIncoming_Created

        private void _HandleIncoming_Created(QsmControl_ _message)
        {
            QsmMetadata_ _metadata = (QsmMetadata_) _message._object;
            QS.Fx.Base.ID _id = new QS.Fx.Base.ID(_metadata._ID);
            if (this._controllerchannels.ContainsKey(_id))
                throw new Exception("Channel \"" + _id.ToString() + "\" already exists.");
            QS.Fx.Reflection.IValueClass _messageclass = this._deserializer_interface.DeserializeValueClass(_metadata._MessageClass);
            QS.Fx.Reflection.IValueClass _checkpointclass = this._deserializer_interface.DeserializeValueClass(_metadata._CheckpointClass);
            QsmControllerChannel_ _channel = new QsmControllerChannel_
            (
                this,
                _message._channel,
                _id,
                _metadata._Name,
                _metadata._Comment,
                _messageclass,
                _checkpointclass,
                _metadata._Index
            );
            this._controllerchannels.Add(_id, _channel);
            this._channels.Add(_message._channel, new QsmChannel_(this._myquicksilver, this, _channel));
        }

        #endregion

        #region _HandleIncoming_Initialize

        private void _HandleIncoming_Initialize(QsmControl_ _message)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region _HandleIncoming_Checkpoint

        private void _HandleIncoming_Checkpoint(QsmControl_ _message)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region _HandleIncoming_Receive

        private void _HandleIncoming_Receive(QsmControl_ _message)
        {
            throw new NotImplementedException();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _UpdateMembershipChangeAlarm

        private void _UpdateMembershipChangeAlarm()
        {
            if (this._subscribing.Count + this._unsubscribing.Count > 0)
            {
                if (!this._waitingToIssueMembershipChangeRequest)
                {
                    this._waitingToIssueMembershipChangeRequest = true;
                    if (this._deferredMembershipChangeRequestAlarm == null)
                        this._deferredMembershipChangeRequestAlarm =
                            this._myquicksilver._Core.Schedule(
                                this._batchinginterval,
                                new QS.Fx.Clock.AlarmCallback(this._MembershipChangeIssueRequestsCallback), 
                                null);
                    else
                        this._deferredMembershipChangeRequestAlarm.Reschedule(this._batchinginterval);
                }
            }
            else
            {
                if (this._waitingToIssueMembershipChangeRequest)
                {
                    this._waitingToIssueMembershipChangeRequest = false;
                    try
                    {
                        this._deferredMembershipChangeRequestAlarm.Cancel();
                    }
                    catch (Exception)
                    {
                    }
                    this._deferredMembershipChangeRequestAlarm = null;
                }
            }
        }

        #endregion

        #region _MembershipChangeIssueRequestsCallback

        private void _MembershipChangeIssueRequestsCallback(QS.Fx.Clock.IAlarm _alarmref)
        {
            lock (this)
            {
                this._waitingToIssueMembershipChangeRequest = false;
                this._deferredMembershipChangeRequestAlarm = null;
                List<QS._qss_c_.Base3_.GroupID> _tojoin = new List<QS._qss_c_.Base3_.GroupID>();
                List<QS._qss_c_.Base3_.GroupID> _toleave = new List<QS._qss_c_.Base3_.GroupID>();
                foreach (QsmChannel_ _channel in this._subscribing)
                {
                    if (_channel._Subscribing())
                        _tojoin.Add(_channel._GroupID);
                }
                this._subscribing.Clear();
                foreach (QsmChannel_ _channel in this._unsubscribing)
                {
                    if (_channel._Unsubscribing())
                        _toleave.Add(_channel._GroupID);
                }
                this._unsubscribing.Clear();
                if (_tojoin.Count + _toleave.Count > 0)
                {
                    if (!this._subscribedformembershipchanges)
                    {
                        ((QS._qss_c_.Membership2.Consumers.IGroupCreationAndRemovalProvider)
                            this._myquicksilver._Framework.MembershipController).OnChange +=
                                new QS._qss_c_.Membership2.Consumers.GroupCreationOrRemovalCallback(this._GroupCreationOrRemovalCallback);
                        this._subscribedformembershipchanges = true;
                    }
                    if (!this._registeredreceivecallback)
                    {
                        this._myquicksilver._Framework.Demultiplexer.register(
                            (uint) ReservedObjectID.QsmChannel, 
                            new QS._qss_c_.Base3_.ReceiveCallback(this._ReceiveCallback));
                        this._registeredreceivecallback = true;
                    }
                    this._myquicksilver._Framework.MembershipAgent.ChangeMembership(_tojoin, _toleave);
                }
            }
        }

        #endregion

        #region _GroupCreationOrRemovalCallback

        private void _GroupCreationOrRemovalCallback(
            IEnumerable<QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval> _notifications)
        {
            lock (this)
            {
                foreach (QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval _groupcreationorremoval in _notifications)
                {
                    long _channelid = (long) _groupcreationorremoval.ID.ToUInt32;
                    QsmChannel_ _channel;
                    if (this._channels.TryGetValue(_channelid, out _channel))
                    {
                        if (_groupcreationorremoval.Creation)
                            _channel._Subscribed();
                        else
                            _channel._Unsubscribed();
                    }
                }
            }
        }

        #endregion

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable _ReceiveCallback(
            QS._core_c_.Base3.InstanceID _sender, QS.Fx.Serialization.ISerializable _message)
        {
            QS._core_c_.Core.ChannelObject _channelobject;
            if (_message is QsmOutgoing_)
            {
                QsmOutgoing_ _outgoingobject = (QsmOutgoing_) _message;
                _channelobject = new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Receive_,
                    _outgoingobject._channel,
                    0,
                    0,
                    _outgoingobject._datalength,
                    _outgoingobject._datablocks
                );
            }
            else if (_message is QsmIncoming_)
            {
                QsmIncoming_ _incomingobject = (QsmIncoming_) _message;
                _channelobject = new QS._core_c_.Core.ChannelObject
                (
                    (int) QsmOperation_.Receive_,
                    _incomingobject._channel,
                    0,
                    0,
                    _incomingobject._datalength,
                    _incomingobject._datablocks
                );
            }
            else
                throw new Exception("Unknown message type.");
            QsmChannel_ _channel;
            if (_channels.TryGetValue(_channelobject.channel, out _channel))
            {
                _channel._Receive(_channelobject);
            }
            return null;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
