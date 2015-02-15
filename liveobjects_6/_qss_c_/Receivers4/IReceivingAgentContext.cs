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

namespace QS._qss_c_.Receivers4
{
    /// <summary>
    /// The context in which receiving agents are running.
    /// </summary>
    public interface IReceivingAgentContext
    {
        /// <summary>
        /// An identifier of this context.
        /// </summary>
        QS.Fx.Serialization.ISerializable ID
        {
            get;
        }

        /// <summary>
        /// The sorted list of addresses of the receivers in the group.
        /// </summary>
        QS._core_c_.Base3.InstanceID[] ReceiverAddresses
        {
            get;
        }

        /// <summary>
        /// Source of failures for this agent collection.
        /// </summary>
        Failure_.ISource FailureSource
        {
            get;
        }

        /// <summary>
        /// Prepares a message for sending to another receiving agent.
        /// </summary>
        /// <param name="message">A message to be delivered to the other receiving agent.</param>
        /// <returns>A wrapped message that will be correctly dispatched by protocol stack at the other end.</returns>
        QS._core_c_.Base3.Message Message(QS._core_c_.Base3.Message message, DestinationType destinationType);

        /// <summary>
        /// Demultiplexer for components registered in this scope.
        /// </summary>
        Base3_.IDemultiplexer Demultiplexer
        {
            get;
        }
    }
}
