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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass("38DB7BE80BB948BF877BA168233030C8", "State", "")]
    public sealed partial class State_<
        [QS.Fx.Reflection.Parameter("ValueClass", QS.Fx.Reflection.ParameterClass.ValueClass)] ValueClass>
        : QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>, 
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>
        where ValueClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public State_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>>(this);
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>>
                _channelendpoint;
        private ValueClass _state = default(ValueClass);

        #endregion

        #region ICheckpointedCommunicationChannelClient<ValueClass,ValueClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.ChannelClient
        {
            get { return this._channelendpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<ValueClass,ValueClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Receive(ValueClass _message)
        {
            Interlocked.Exchange<ValueClass>(ref this._state, _message);
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Initialize(ValueClass _checkpoint)
        {
            Interlocked.Exchange<ValueClass>(ref this._state, _checkpoint);
        }

        ValueClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Checkpoint()
        {
            return this._state;
        }

        #endregion
    }
}
