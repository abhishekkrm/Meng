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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.UIOnCheckpointedCommunicationChannel,
        "UIOnCheckpointedCommunicationChannel", "A base class for objects with a graphical user interface and based on checkpointed communication channels.")]
    public class UIOnCheckpointedCommunicationChannel<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable 
    {
        #region Constructor

        public UIOnCheckpointedCommunicationChannel(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel)
            : base(_mycontext)
        {
            this._mycontext = _mycontext;

            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>(this);
            this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._OnChannelConnect);
            this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._OnChannelDisconnect);
            this._channel = _channel;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> _channelendpoint;

        [QS.Fx.Base.Inspectable("channel")]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        #endregion

        #region _Connect

        protected void _Connect()
        {
            lock (this)
            {
                if (_channel != null)
                    this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channel.Dereference(_mycontext).Channel);
            }
        }

        #endregion

        #region _Disconnect

        protected void _Disconnect()
        {
            lock (this)
            {
                if (this._channelconnection != null)
                {
                    this._channelconnection.Dispose();
                    this._channelconnection = null;
                }
            }
        }

        #endregion

        #region _OnChannelConnect

        private void _OnChannelConnect()
        {
        }

        #endregion

        #region _OnChannelDisconnect

        private void _OnChannelDisconnect()
        {
        }

        #endregion

        #region _Checkpoint

        protected virtual CheckpointClass _Checkpoint
        {
            get 
            { 
                return null; 
            }
            
            set 
            {  
            }
        }

        #endregion

        #region _Send

        protected void _Send(MessageClass _message)
        {
            _channelendpoint.Interface.Send(_message);
        }

        #endregion

        #region _Receive

        protected virtual void _Receive(MessageClass _message)
        {
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Receive(MessageClass _message)
        {
            this._Receive(_message);
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
        {
            this._Checkpoint = _checkpoint;
        }

        CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Checkpoint()
        {
            return this._Checkpoint;
        }

        #endregion
    }
}
