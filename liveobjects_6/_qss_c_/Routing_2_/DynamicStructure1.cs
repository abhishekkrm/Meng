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
/*
	public class DynamicStructure1<C> : IDynamicStructure<C>
	{
		public DynamicStructure1(IList<C> addresses, Routing.IRoutingAlgorithm routingAlgorithm)
		{
			underlyingStructure = routingAlgorithm.instantiate((uint) addresses.Count);
			mynodes = new C[addresses.Count];
			mappings = new Dictionary<C, uint>(addresses.Count);
			uint index = 0;
			foreach (C address in addresses)
			{
				mynodes[index] = new Node(address);
				mappings[address] = index++;
			}
		}

		private readonly Routing.IRoutingStructure underlyingStructure;
		private readonly Node[] mynodes;
		private readonly IDictionary<C, uint> mappings;

		#region Class Node

		private struct Node
		{
			public Node(C address)
			{
				this.Address = address;
				Removed = false;
				OutgoingAddresses = IncomingAddresses = null;
			}

			public C[] Address;
			public IList<C> OutgoingAddresses, IncomingAddresses;
			public bool Removed;
		}

		#endregion

		private IList<C> toAddresses(uint[] indexes)
		{
			List<C> result = new List<C>(indexes.Length);
			foreach (uint index in indexes)
				result.Add(addresses[index]);
			return result;
		}

		#region IStructure<C> Members

		IList<C> IStructure<C>.Outgoing(C referenceAddress, C rootAddress)
		{
			uint[] outgoing_indexes = underlyingStructure.outgoingAt(mappings[referenceAddress], mappings[rootAddress]);
			if (patched)
			{
				Queue<uint> elements = new Queue<uint>();


				for (int ind = 0; ind < outgoing_indexes.Length; ind++)
				{
					while (mynodes[outgoing_indexes[ind]].Removed)
						outgoing_indexes[ind] = underlying  outgoing_indexes
				}
			}
			else
				return toAddresses(outgoing_indexes);
		}

		IList<C> IStructure<C>.Incoming(C referenceAddress, C rootAddress)
		{
			
			
			
			throw new NotImplementedException();
		}

		#endregion

		#region IDynamicStructure<C> Members

		void IDynamicStructure<C>.Remove(C address)
		{
			patched = true;
			mynodes[mappings[address]].Removed = true;
		}

		#endregion
	}
*/
}
