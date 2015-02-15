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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CheckpointSynchronizer,
        "CheckpointSynchronizer", "A component that provides checkpointed communication by synchronizing checkpoints with messages.")]
    public sealed class CheckpointSynchronizer<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
            QS._qss_x_.Interface_.Classes_.ICheckpointingChannelClient<CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public CheckpointSynchronizer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("underlying_communication_channel", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<MessageClass>> _underlying_communication_channel,
            [QS.Fx.Reflection.Parameter("underlying_checkpointing_channel", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICheckpointingChannel<CheckpointClass>> _underlying_checkpointing_channel)
        {
            this._communication_channel_endpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
            
            this._communication_channel_endpoint.OnConnect += new QS.Fx.Base.Callback(this._CommunicationChannelConnectCallback);
            this._communication_channel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._CommunicationChannelDisconnectCallback);

            this._underlying_communication_channel_endpoint = 
                _mycontext.DualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>>(this);
            
            this._underlying_communication_channel_connection =
                ((QS.Fx.Endpoint.Classes.IEndpoint) this._underlying_communication_channel_endpoint).Connect(
                    _underlying_communication_channel.Dereference(_mycontext)._Channel);

            if (_underlying_checkpointing_channel != null)
            {
                this._underlying_checkpointing_channel_endpoint =
                    _mycontext.DualInterface<
                        QS._qss_x_.Interface_.Classes_.ICheckpointingChannel<CheckpointClass>,
                        QS._qss_x_.Interface_.Classes_.ICheckpointingChannelClient<CheckpointClass>>(this);

                this._underlying_checkpointing_channel_connection =
                    ((QS.Fx.Endpoint.Classes.IEndpoint)this._underlying_checkpointing_channel_endpoint).Connect(
                        _underlying_checkpointing_channel.Dereference(_mycontext).Channel);
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _communication_channel_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>> _underlying_communication_channel_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _underlying_communication_channel_connection;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICheckpointingChannel<CheckpointClass>,
            QS._qss_x_.Interface_.Classes_.ICheckpointingChannelClient<CheckpointClass>> _underlying_checkpointing_channel_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _underlying_checkpointing_channel_connection;

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
            this._communication_channel_endpoint.Interface.Initialize(null);
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
            this._underlying_communication_channel_endpoint.Interface._Send(_message);
        }

        #endregion

        #region ICommunicationChannelClient<MessageClass> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>._Receive(MessageClass _message)
        {
            this._communication_channel_endpoint.Interface.Receive(_message);
        }

        #endregion

        #region ICheckpointingChannelClient<CheckpointClass> Members

        void QS._qss_x_.Interface_.Classes_.ICheckpointingChannelClient<CheckpointClass>.Checkpoint(
            uint _minimum_timestamp, out uint _timestamp, CheckpointClass _checkpoint)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
