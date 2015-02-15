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
    public abstract class Channel_<MessageClass, CheckpointClass>
        : QS._qss_x_.Properties_.Component_.Interface_,
        QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class
        where CheckpointClass : class
    {
        #region Constructor

        protected Channel_
        (
            [QS.Fx.Reflection.Parameter("properties", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IProperties_> _properties_reference
        )
            : base(_properties_reference)
        {
            this._bottom_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
            this._bottom_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Bottom_Connect);
            this._bottom_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Bottom_Disconnect);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>
                _bottom_endpoint;

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Channel_._Dispose");
#endif

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
            base._Start();

#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Channel_._Start");
#endif
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Channel_._Stop");
#endif

            base._Stop();
        }

        #endregion

        #region _Top_Connect

        protected override void _Top_Connect()
        {
            base._Top_Connect();
        }

        #endregion

        #region _Top_Disconnect

        protected override void _Top_Disconnect()
        {
            base._Top_Disconnect();
        }

        #endregion

        #region _Top_Receive

        protected override void _Top_Receive(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
            base._Top_Receive(_id, _version, _value);
        }

        #endregion

        #region _Bottom_Connect

        protected virtual void _Bottom_Connect()
        {
        }

        #endregion

        #region _Bottom_Disconnect

        protected virtual void _Bottom_Disconnect()
        {
        }

        #endregion

        #region _Bottom_Send

        protected void _Bottom_Send(MessageClass _message)
        {
            this._bottom_endpoint.Interface.Receive(_message);
        }

        #endregion

        #region _Bottom_Initialize

        protected void _Bottom_Initialize(CheckpointClass _checkpoint)
        {
            this._bottom_endpoint.Interface.Initialize(_checkpoint);
        }

        #endregion

        #region _Bottom_Checkpoint

        protected CheckpointClass _Bottom_Checkpoint()
        {
            return this._bottom_endpoint.Interface.Checkpoint();
        }

        #endregion

        #region _Bottom_Receive

        protected virtual void _Bottom_Receive(MessageClass _message)
        {
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._bottom_endpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            this._Bottom_Receive(_message);
        }

        #endregion
    }
*/ 
}
