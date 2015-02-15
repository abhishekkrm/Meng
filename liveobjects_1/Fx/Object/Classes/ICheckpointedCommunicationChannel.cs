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

namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass(QS.Fx.Reflection.ObjectClasses.CheckpointedCommunicationChannel, "Checkpointed communication channel",
        "A communication channel with messages and checkpoints representing current state, transmitted between the communicating parties.")]
    public interface ICheckpointedCommunicationChannel<
        [QS.Fx.Reflection.Parameter("MessageClass", "The type of messages in the channel.", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", "The type of checkpoints representing state.", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
        : QS.Fx.Object.Classes.IObject
        where MessageClass : class //, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class // , QS.Fx.Serialization.ISerializable
    {
        [QS.Fx.Reflection.Endpoint("Channel")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> Channel
        {
            get;
        }
    }

/*
    [QS.Fx.Reflection.ObjectClass(QS.Fx.Reflection.ObjectClasses.CheckpointedCommunicationChannel_)]
    public interface ICheckpointedCommunicationChannel_ : IObject
    {
        [QS.Fx.Reflection.Endpoint("Metadata")]
        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> Channel
        {
            get;
        }

        [QS.Fx.Reflection.Endpoint("Channel")]
        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> Channel
        {
            get;
        }
    }
*/ 
}
