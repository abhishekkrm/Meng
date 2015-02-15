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

namespace QS._qss_x_.Component_.Classes_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CentralizedCommunicationChannel, 
        "CentralizedCommunicationChannel", "This is a simple, totally ordered communication channel based on a centralized server implementation.")]
    public sealed class CentralizedCommunicationChannel<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.TMS.Inspection.Inspectable, 
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICommunicationChannelClient<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public CentralizedCommunicationChannel(
            [QS.Fx.Reflection.Parameter("channel_id", QS.Fx.Reflection.ParameterClass.Value)] 
                uint _channel_id,
            [QS.Fx.Reflection.Parameter("underlying_communication_channel", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<
                    QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>> _underlying_communication_channel)
        {
            this._communication_channel_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            this._communication_channel_endpoint.OnConnect += new QS.Fx.Base.Callback(this._CommunicationChannelConnectCallback);
            this._communication_channel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._CommunicationChannelDisconnectCallback);

            this._channel_id = _channel_id;

            this._underlying_communication_channel_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>,
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>>(this);

            this._underlying_communication_channel_connection =
                ((QS.Fx.Endpoint.Classes.IEndpoint)this._underlying_communication_channel_endpoint).Connect(
                    _underlying_communication_channel.Object.Channel);
        }

        #endregion

        #region Fields

        [QS.TMS.Inspection.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _communication_channel_endpoint;
        
        [QS.TMS.Inspection.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>,
            QS.Fx.Interface.Classes.ICommunicationChannelClient<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>> _underlying_communication_channel_endpoint;
        
        [QS.TMS.Inspection.Inspectable]
        private QS.Fx.Endpoint.IConnection _underlying_communication_channel_connection;        
        
        [QS.TMS.Inspection.Inspectable]
        private uint _channel_id, _outgoing_seqno, _incoming_seqno, _incoming_buffered_from, _incoming_buffered_to;
        [QS.TMS.Inspection.Inspectable]
        private bool _incoming_buffered;
        [QS.TMS.Inspection.Inspectable]
        private Queue<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage> _incoming_messages =
            new Queue<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>();

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._communication_channel_endpoint; }
        }

        #endregion

        #region _CommunicationChannelConnectCallback

        private void _CommunicationChannelConnectCallback()
        {
            this._underlying_communication_channel_endpoint.Interface.Send(
                new QS.Fx.Channel.Message.CentralizedCommunicationChannel.Message(
                    QS.Fx.Channel.Message.CentralizedCommunicationChannel.MessageType.Open, _channel_id, 0, null));
        }

        #endregion

        #region _CommunicationChannelDisconnectCallback

        private void _CommunicationChannelDisconnectCallback()
        {
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            this._underlying_communication_channel_endpoint.Interface.Send(
                new QS.Fx.Channel.Message.CentralizedCommunicationChannel.Message(
                    QS.Fx.Channel.Message.CentralizedCommunicationChannel.MessageType.Send, 
                        _channel_id, ++_outgoing_seqno, (QS.Fx.Serialization.ISerializable) _message));
        }

        #endregion

        #region ICommunicationChannelClient<QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage> Members

        void QS.Fx.Interface.Classes.ICommunicationChannelClient<
            QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage>.Receive(
            QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage _message)
        {
            lock (this)
            {
                switch (_message.MessageType)
                {
                    case QS.Fx.Channel.Message.CentralizedCommunicationChannel.MessageType.Open:
                    case QS.Fx.Channel.Message.CentralizedCommunicationChannel.MessageType.Send:
                        throw new NotSupportedException();

                    case QS.Fx.Channel.Message.CentralizedCommunicationChannel.MessageType.Checkpoint:
                        {
                            if (_message.SequenceNo >= _incoming_seqno)
                            {
                                this._communication_channel_endpoint.Interface.Initialize((CheckpointClass) _message.DataObject);
                                _incoming_seqno = _message.SequenceNo + 1;
                                if (_incoming_buffered)
                                {
                                    while (_incoming_buffered_from < _incoming_seqno)
                                    {
                                        QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage _m = _incoming_messages.Dequeue();
                                        if (_m.SequenceNo != _incoming_buffered_from)
                                            throw new Exception("Found inconsistent protocol state.");
                                        _incoming_buffered_from++;
                                    }
                                    while (_incoming_buffered_from == _incoming_seqno)
                                    {
                                        QS.Fx.Channel.Message.CentralizedCommunicationChannel.IMessage _m = _incoming_messages.Dequeue();
                                        if (_m.SequenceNo != _incoming_buffered_from)
                                            throw new Exception("Found inconsistent protocol state.");
                                        _incoming_buffered_from++;
                                        _incoming_seqno++;
                                        this._communication_channel_endpoint.Interface.Receive((MessageClass) _m.DataObject);
                                    }
                                }
                            }
                        }
                        break;

                    case QS.Fx.Channel.Message.CentralizedCommunicationChannel.MessageType.Receive:
                        {
                            if (_message.SequenceNo >= _incoming_seqno)
                            {                             
                                if (_message.SequenceNo == _incoming_seqno)
                                {
                                    this._communication_channel_endpoint.Interface.Receive((MessageClass) _message.DataObject);
                                    _incoming_seqno++;
                                }
                                else if (_incoming_buffered)
                                {
                                    if (_message.SequenceNo != _incoming_buffered_to + 1)
                                        throw new Exception("Received an out of order message.");
                                    _incoming_buffered_to++;
                                    _incoming_messages.Enqueue(_message);
                                }
                                else
                                {
                                    _incoming_buffered = true;
                                    _incoming_buffered_from = _incoming_buffered_to = _message.SequenceNo;
                                    _incoming_messages.Enqueue(_message);
                                }
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion
    }
*/ 
}
