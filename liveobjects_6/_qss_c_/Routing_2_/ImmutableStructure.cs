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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Routing_2_
{
    public class ImmutableStructure<C> : IStructure<C>
    {
        public ImmutableStructure(IList<C> addresses, Routing_1_.IRoutingAlgorithm routingAlgorithm, int modifying_index)
        {
            underlyingStructure = routingAlgorithm.instantiate((uint)addresses.Count, modifying_index);

            this.addresses = new C[addresses.Count];
            mappings = new Dictionary<C, uint>(addresses.Count);

            uint index = 0;
            foreach (C address in addresses)
            {
                this.addresses[index] = address;
                mappings[address] = index++;
            }
        }

        private readonly Routing_1_.IRoutingStructure underlyingStructure;
        private readonly C[] addresses;
        private readonly IDictionary<C, uint> mappings;

        private IList<C> toAddresses(uint[] indexes)
        {
            List<C> result = new List<C>(indexes.Length);
            foreach (uint index in indexes)
                result.Add(addresses[index]);
            return result;
        }

        #region IStructure<InstanceID> Members

        public IList<C> Outgoing(C referenceAddress, C rootAddress)
        {
            return toAddresses(underlyingStructure.outgoingAt(mappings[referenceAddress], mappings[rootAddress]));
        }

        public IList<C> Incoming(C referenceAddress, C rootAddress)
        {
            return toAddresses(underlyingStructure.incomingAt(mappings[referenceAddress], mappings[rootAddress]));
        }

        #endregion
    }
}
