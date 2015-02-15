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

// #define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
/*
    public abstract class ChannelClient_<MessageClass, CheckpointClass>
        : QS._qss_x_.Properties_.Component_.Application_,        
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>
        where MessageClass : class
        where CheckpointClass : class
    {
        #region Constructor

        protected ChannelClient_
        (
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel_reference
        )
        : base()
        {
            this._channel_reference = _channel_reference;
            this._top_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>(this);
            this._top_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Top_Connect);
            this._top_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Top_Disconnect);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass> _channel_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channel_connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>
                _top_endpoint;

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
            base._Dispose();

#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.ChannelClient_._Dispose");
#endif

            this._channel_connection.Dispose();
            this._channel_connection = null;
            this._channel_endpoint = null;
            if (this._channel_object is IDisposable)
                ((IDisposable)this._channel_object).Dispose();
            this._channel_object = null;
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Receive(MessageClass _message)
        {
            this._Top_Receive(_message);
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
        {
            this._Top_Initialize(_checkpoint);
        }

        CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Checkpoint()
        {
            return this._Top_Checkpoint();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
            base._Start();

#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.ChannelClient_._Start");
#endif

            this._channel_object = this._channel_reference.Object;
            if (this._channel_object is QS._qss_x_.Platform_.IApplication)
                ((QS._qss_x_.Platform_.IApplication) this._channel_object).Start(this._platform, null);
            this._channel_endpoint = this._channel_object.Channel;
            this._channel_connection = this._top_endpoint.Connect(this._channel_endpoint);
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.ChannelClient_._Stop");
#endif

            if (this._channel_object is QS._qss_x_.Platform_.IApplication)
                ((QS._qss_x_.Platform_.IApplication) this._channel_object).Stop();

            base._Stop();
        }

        #endregion

        #region _Top_Connect

        protected virtual void _Top_Connect()
        {
        }

        #endregion

        #region _Top_Disconnect

        protected virtual void _Top_Disconnect()
        {
        }

        #endregion

        #region _Top_Send

        protected void _Top_Send(MessageClass _message)
        {
            this._top_endpoint.Interface.Send(_message);
        }

        #endregion

        #region _Top_Initialize

        protected virtual void _Top_Initialize(CheckpointClass _checkpoint)
        {
        }

        #endregion

        #region _Top_Checkpoint

        protected virtual CheckpointClass _Top_Checkpoint()
        {
            return null;
        }

        #endregion

        #region _Top_Receive

        protected virtual void _Top_Receive(MessageClass _message)
        {
        }

        #endregion
    }
*/ 
}
