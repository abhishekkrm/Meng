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

namespace QS.Fx.Interface.Classes
{
    [QS.Fx.Reflection.InterfaceClass(QS.Fx.Reflection.InterfaceClasses.Channel)]
    public interface IChannel<
        [QS.Fx.Reflection.Parameter("MessageClass", "The type of transmitted objects.", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : IInterface
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        /// <summary>
        /// Start delivering data from this channel.
        /// </summary>
        [QS.Fx.Reflection.Operation("Start")]
        void Start();

        /// <summary>
        /// Stop delivering data from this channel.
        /// </summary>
        [QS.Fx.Reflection.Operation("Stop")]
        void Stop();

        /// <summary>
        /// Push one message along the channel.
        /// </summary>
        /// <param name="_message">Message to push.</param>
        [QS.Fx.Reflection.Operation("Push")]
        void Push(MessageClass _message);
    }
}
