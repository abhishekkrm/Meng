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

namespace QS._qss_c_.Rings_1_
{
    /// <summary>
    /// An element of some protocol layer talking over the ring.
    /// </summary>
    public interface IRingClient
    {
        /// <summary>
        /// An identifier that identifies a specific protocol layer.
        /// </summary>
        uint ID
        {
            get;
        }

        /// <summary>
        /// Indicates whether this client is active, i.e. wants to talk over the ring. Ring stops working when all clients go inactive and
        /// resumes operation when at least one client activates.
        /// </summary>
        bool Active
        {
            get;
        }

        /// <summary>
        /// Returns a new object that is to be circulated forward along the ring
        /// </summary>
        void Process(out QS.Fx.Serialization.ISerializable forwardGoing);

        /// <summary>
        /// Processes a message received along the ring and returns a pair of messages to be passed forward (with the token)
        /// and backwards (with the acknowledgement), respectively. One hop in each case, not multicasting.
        /// </summary>
        void Process(QS.Fx.Serialization.ISerializable received, out QS.Fx.Serialization.ISerializable forwardGoing, out QS.Fx.Serialization.ISerializable backwardGoing);

        /// <summary>
        /// Processes a message received along the ring and returns a message that is to be sent backwards together with an
        /// acknowledgement.
        /// </summary>
        /// <param name="received"></param>
        void Process(QS.Fx.Serialization.ISerializable received, out QS.Fx.Serialization.ISerializable backwardGoing);
    }
}
