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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Channel_1, "Channel_1", "A typed checkpoint communication channel wrapper.")]
    public sealed class Channel_1<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
        : QS.Fx.Inspection.Inspectable,
        QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Channel_1(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("underlying_untyped_channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        QS.Fx.Serialization.ISerializable, 
                        QS.Fx.Serialization.ISerializable>> _underlying_untyped_communication_channel)
        {
            this._mycontext = _mycontext;

            lock (this)
            {
                if (_underlying_untyped_communication_channel == null)
                    throw new Exception("Cannot run without a valid reference to the underlying untyped channel.");

                this._underlying_untyped_communication_channel = _underlying_untyped_communication_channel;

                this._underlying_untyped_communication_channel_endpoint =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>(this);

                this._communication_channel_endpoint =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

                this._communication_channel_endpoint.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);
                this._communication_channel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                    _communication_channel_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                QS.Fx.Serialization.ISerializable,
                QS.Fx.Serialization.ISerializable>>
                    _underlying_untyped_communication_channel;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>
                    _underlying_untyped_communication_channel_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection
                    _underlying_untyped_communication_channel_connection;

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

        #region _ConnectCallback

        private void _ConnectCallback()
        {
            lock (this)
            {
                if (this._underlying_untyped_communication_channel_connection != null)
                    throw new Exception("Already connected to an underlying untyped communication channel.");

                this._underlying_untyped_communication_channel_connection =
                    ((QS.Fx.Endpoint.Classes.IEndpoint)this._underlying_untyped_communication_channel_endpoint).Connect(
                        _underlying_untyped_communication_channel.Dereference(_mycontext).Channel);
            }
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback()
        {
            lock (this)
            {
                if (this._underlying_untyped_communication_channel_connection != null)
                {
                    this._underlying_untyped_communication_channel_connection.Dispose();
                    this._underlying_untyped_communication_channel_connection = null;
                }
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            lock (this)
            {
                this._underlying_untyped_communication_channel_endpoint.Interface.Send((QS.Fx.Serialization.ISerializable)_message);
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<ISerializable,ISerializable> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Receive(
            QS.Fx.Serialization.ISerializable _message)
        {
            lock (this)
            {
                if (_message is MessageClass)
                    this._communication_channel_endpoint.Interface.Receive((MessageClass) _message);
                else
                    throw new Exception("Received a message of a wrong type.");
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Initialize(
            QS.Fx.Serialization.ISerializable _checkpoint)
        {
            lock (this)
            {
                if (_checkpoint is CheckpointClass)
                    this._communication_channel_endpoint.Interface.Initialize((CheckpointClass) _checkpoint);
                else
                    throw new Exception("Received a message of a wrong type.");
            }
        }

        QS.Fx.Serialization.ISerializable 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Checkpoint()
        {
            lock (this)
            {
                return (QS.Fx.Serialization.ISerializable) this._communication_channel_endpoint.Interface.Checkpoint();
            }
        }

        #endregion
    }
}
