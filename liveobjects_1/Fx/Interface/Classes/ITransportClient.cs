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
    /// <summary>
    /// Interface exposed by clients of transport object, including all outgoing event handlers.
    /// </summary>
    /// <typeparam name="AddressClass">The type of the class representing addresses.</typeparam>
    /// <typeparam name="MessageClass">The type of the class representing messages.</typeparam>
    [QS.Fx.Reflection.InterfaceClass(QS.Fx.Reflection.InterfaceClasses.TransportClient)]
    public interface ITransportClient<
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass,
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass> 
        : QS.Fx.Interface.Classes.IInterface
    {
        /// <summary>
        /// Invoked once, asynchronously, after the transport endpoint is connected, provides the client with its own address.
        /// </summary>
        /// <param name="address">The client's own address.</param>
        [QS.Fx.Reflection.Operation("Address")]
        void Address(AddressClass address);

        /// <summary>
        /// Invoked when the channel to a given destination is created. This could be upon the client's request, but it could 
        /// also happen when the destination wants to connect to this client, even if the client hasn't requested to connect
        /// to the destination.
        /// </summary>
        /// <param name="address">The address of the destination to which the channel links.</param>
        /// <param name="channel">The reference to the channel object.</param>
        [QS.Fx.Reflection.Operation("Connected")]
        void Connected(
            AddressClass address, 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> channel);
    }
}
