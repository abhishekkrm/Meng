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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.MulticastClient, "Properties Framework Multicast Client")]
    public sealed class MulticastClient_ 
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MulticastClient_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                    QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> _channel_reference,
            [QS.Fx.Reflection.Parameter("length", QS.Fx.Reflection.ParameterClass.Value)] 
            int _length,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)] 
            double _rate,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_.Constructor");
#endif

            if (_channel_reference == null)
                _mycontext.Error("Channel reference cannot be null.");
            if (_rate < 0)
                _mycontext.Error("Rate cannot be negative.");
            if (_length < 0)
                _mycontext.Error("Length cannot be negative.");

            this._channel_reference = _channel_reference;
            this._identifier = new QS.Fx.Base.Identifier(Guid.NewGuid());
            this._rate = _rate;
            this._length = _length;
            this._seqno = 0;

            this._channel_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>(this);
            this._channel_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Connect))); });
            this._channel_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Disconnect))); });

            this._InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _activated;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private int _length;
        [QS.Fx.Base.Inspectable]
        private double _rate;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm;
        [QS.Fx.Base.Inspectable]
        private int _seqno;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> _channel_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
            QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable> _channel_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> _channel_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channel_connection;

        private IDictionary<QS.Fx.Base.Identifier, Message_> _messages = new Dictionary<QS.Fx.Base.Identifier, Message_>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Base.Identifier, Message_> __inspectable_messages;

        private void _InitializeInspection()
        {
            __inspectable_messages =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Base.Identifier, Message_>("__inspectable_messages", _messages);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICheckpointedCommunicationChannelClient<ISerializable,ISerializable> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Receive(
            QS.Fx.Serialization.ISerializable _message)
        {
            if ((_message != null) && (_message is Message_))
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<Message_>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Receive), (Message_) _message));
            else
                _mycontext.Error("Received a message that is either null or of the wrong type.");
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Initialize(
            QS.Fx.Serialization.ISerializable _checkpoint)
        {
            if ((_checkpoint != null) && (!(_checkpoint is Checkpoint_)))
                _mycontext.Error("Received a checkpoint of the wrong type.");

            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<Checkpoint_>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Initialize), ((_checkpoint != null) ? ((Checkpoint_) _checkpoint) : null)));
        }

        QS.Fx.Serialization.ISerializable
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Checkpoint()
        {
            return this._Channel_Checkpoint();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                this._channel_object = this._channel_reference.Dereference(_mycontext);
                if (this._channel_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication) this._channel_object).Start(this._platform, null);
                this._channel_connection = this._channel_endpoint.Connect(this._channel_object.Channel);
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.MulticastClient_._Dispose");
#endif

            lock (this)
            {
                this._activated = false;
                this._initialized = false;

                if ((this._alarm != null) && !this._alarm.Cancelled)
                    this._alarm.Cancel();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.MulticastClient_._Start");
#endif

            base._Start();

            lock (this)
            {
                if (this._activated)
                    _mycontext.Error("Already activated!");
                {
                    if (this._initialized)
                    {
                        this._activated = true;
                        this._Channel_Send();
                    }
                }
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.MulticastClient_._Stop");
#endif

            lock (this)
            {
                this._activated = false;
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Channel_Connect

        private void _Channel_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Channel_Connect ");
#endif
        }

        #endregion

        #region _Channel_Disconnect

        private void _Channel_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Channel_Disconnect");
#endif
            
            lock (this)
            {
                this._activated = false;
                this._initialized = false;
            }
        }

        #endregion

        #region _Channel_Initialize

        private void _Channel_Initialize(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            Checkpoint_ _checkpoint = ((QS._qss_x_.Properties_.Base_.IEvent_<Checkpoint_>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Channel_Initialize\n\n" + QS.Fx.Printing.Printable.ToString(_checkpoint) + "\n\n");
#endif

            lock (this)
            {
                this._messages.Clear();
                if (_checkpoint != null)
                {
                    foreach (Message_ _message in _checkpoint._Messages)
                    {
                        if (this._messages.ContainsKey(_message._Identifier))
                            _mycontext.Error("Duplicate identifier in the checkpoint!");
                        this._messages.Add(_message._Identifier, _message);
                    }
                }

                if (this._activated)
                    _mycontext.Error("Already activated!");
                else
                {
                    if (this._initialized)
                        _mycontext.Error("Already initialized!");                    
                    this._initialized = true;
                    if (this._platform != null)
                    {
                        this._activated = true;
                        this._Channel_Send();
                    }
                }
            }
        }

        #endregion

        #region _Channel_Send

        private void _Channel_Send()
        {
            if ((this._length > 0) && (this._rate > 0))
            {
                this._alarm = this._platform.AlarmClock.Schedule
                (
                    (1 / this._rate),
                    new QS.Fx.Clock.AlarmCallback
                    (
                        delegate(QS.Fx.Clock.IAlarm _alarm)
                        {
                            if ((_alarm != null) && !_alarm.Cancelled && ReferenceEquals(this._alarm, _alarm) && this._activated)
                            {
                                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Channel_Send));
                            }
                        }
                    ),
                    null
                );
            }
        }

        private void _Channel_Send(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Channel_Send");
#endif

            lock (this)
            {
                if (this._activated && this._channel_endpoint.IsConnected && this._seqno < this._length)
                {
                    this._seqno++;
                    this._channel_endpoint.Interface.Send(new Message_(this._identifier, new QS.Fx.Base.Index(this._seqno)));
                    if (this._alarm != null)
                        this._alarm.Reschedule();
                }
            }
        }

        #endregion

        #region _Channel_Receive

        private void _Channel_Receive(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            Message_ _message = ((QS._qss_x_.Properties_.Base_.IEvent_<Message_>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Channel_Receive\n\n" + QS.Fx.Printing.Printable.ToString(_message) + "\n\n");
#endif

            lock (this)
            {
                Message_ _message_0;
                if (this._messages.TryGetValue(_message._Identifier, out _message_0))
                {
                    uint _i1 = ((uint) (((uint) _message_0._Index) + 1));
                    uint _i2 = (uint) _message._Index;
                    if (_i2 == _i1)
                    {
                        _message_0._Index = _message._Index;
                    }
                    else
                        _mycontext.Error("Received a message from \"" + _message._Identifier.ToString() +
                            "\" with an incorrect index; received " + _i2.ToString() + ", expected " + _i1.ToString() + ".");
                }
                else
                    this._messages.Add(_message._Identifier, _message);
            }
        }

        #endregion

        #region _Channel_Checkpoint

        private Checkpoint_ _Channel_Checkpoint()
        {
            Checkpoint_ _checkpoint;
            lock (this)
            {
                Message_[] _messages = new Message_[this._messages.Count];
                this._messages.Values.CopyTo(_messages, 0);
                _checkpoint = new Checkpoint_(_messages);
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastClient_._Channel_Checkpoint\n\n" + QS.Fx.Printing.Printable.ToString(_checkpoint) + "\n\n");
#endif

            return _checkpoint;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Checkpoint_

        [QS.Fx.Printing.Printable("Checkpoint", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.MulticastClient_Checkpoint_)]
        [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_multicastclient_checkpoint)]
        public sealed class Checkpoint_ : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public Checkpoint_(Message_[] _messages)
            {
                this._messages = _messages;
            }

            public Checkpoint_()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable("messages")]
            private Message_[] _messages;

            #endregion

            #region Accessors

            public Message_[] _Messages
            {
                get { return this._messages; }
            }

            #endregion

            #region ISerializable Members

            unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.MulticastClient_Checkpoint_, sizeof(uint));
                    foreach (Message_ _message in this._messages)
                        _info.AddAnother(((QS.Fx.Serialization.ISerializable) _message).SerializableInfo);
                    return _info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
            {
                fixed (byte* _pheader_0 = _header.Array)
                {
                    byte* _pheader = _pheader_0 + _header.Offset;
                    *((uint*)_pheader) = (uint)((this._messages != null) ? this._messages.Length : 0);
                }
                _header.consume(sizeof(uint));
                if (this._messages != null)
                {
                    foreach (Message_ _message in this._messages)
                        ((QS.Fx.Serialization.ISerializable)_message).SerializeTo(ref _header, ref _data);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
            {
                int _length;
                fixed (byte* _pheader_0 = _header.Array)
                {
                    byte* _pheader = _pheader_0 + _header.Offset;
                    _length = (int)(*((uint *) _pheader));
                }
                _header.consume(sizeof(uint));
                if (_length > 0)
                {
                    this._messages = new Message_[_length];
                    for (int _i = 0; _i < _length; _i++)
                    {
                        this._messages[_i] = new Message_();
                        ((QS.Fx.Serialization.ISerializable) this._messages[_i]).DeserializeFrom(ref _header, ref _data);
                    }
                }
                else
                    this._messages = null;
            }

            #endregion
        }

        #endregion

        #region Class Message_

        [QS.Fx.Printing.Printable("Message", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.MulticastClient_Message_)]
        [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_multicastclient_message)]
        public sealed class Message_ : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public Message_(QS.Fx.Base.Identifier _identifier, QS.Fx.Base.Index _index)
            {
                this._identifier = _identifier;
                this._index = _index;
            }

            public Message_()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable("identifier")]
            private QS.Fx.Base.Identifier _identifier;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable("index")]
            private QS.Fx.Base.Index _index;

            #endregion

            #region Accessors

            public QS.Fx.Base.Identifier _Identifier
            {
                get { return this._identifier; }
                set { this._identifier = value; }
            }

            public QS.Fx.Base.Index _Index
            {
                get { return this._index; }
                set { this._index = value; }
            }

            #endregion

            #region ISerializable Members

            unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.MulticastClient_Message_);
                    _info.AddAnother(this._identifier.SerializableInfo);
                    _info.AddAnother(this._index.SerializableInfo);
                    return _info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
            {
                this._identifier.SerializeTo(ref _header, ref _data);
                this._index.SerializeTo(ref _header, ref _data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
            {
                this._identifier = new QS.Fx.Base.Identifier();
                this._identifier.DeserializeFrom(ref _header, ref _data);
                this._index = new QS.Fx.Base.Index();
                this._index.DeserializeFrom(ref _header, ref _data);
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
