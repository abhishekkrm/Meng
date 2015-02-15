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

namespace QS._qss_c_.Routing_1_
{
	public class BalancedTreeRouting : IRoutingAlgorithm
	{
        public static BalancedTreeRouting Algorithm(uint routingBase)
        {
            return new BalancedTreeRouting(routingBase);
        }

		public BalancedTreeRouting(uint routingBase)
		{
			if (routingBase < 1)
				throw new Exception("Routing base must not be smaller than 1.");
			this.routingBase = routingBase;
		}

		private uint routingBase;

		#region IRoutingAlgorithm Members

        public IRoutingStructure instantiate(uint size, int modifying_index)
		{
			return new Structure(size, routingBase, modifying_index);
		}

		#endregion

		private class Structure : IRoutingStructure
		{
            public Structure(uint size, uint routingBase, int modifying_index)
			{
				this.size = size;
				this.routingBase = routingBase;
			}

			private uint size, routingBase;

			#region IRoutingStructure Members

			public uint Size
			{
				get
				{
					return size;
				}
			}

			public uint[] outgoingAt(uint memberIndex, uint destinationIndex)
			{
				if (memberIndex == destinationIndex)
					return new uint[] { };
				else
				{
					uint backwards_distance = (memberIndex + size - destinationIndex) % size;
					uint parent_distance = ((backwards_distance + 1) / routingBase) - 1;
					return new uint[] { (destinationIndex + parent_distance) % size };
				}
			}

			public uint[] incomingAt(uint memberIndex, uint destinationIndex)
			{
				uint backwards_distance = (memberIndex + size - destinationIndex) % size;
				int tail_size = (int) size - 1 - (int) (routingBase * backwards_distance);
				if (tail_size > 0)
				{
					uint nchildren = (uint)tail_size;
					if (routingBase < nchildren)
						nchildren = routingBase;
					uint[] children = new uint[nchildren];
					for (uint ind = 0; ind < nchildren; ind++)
					{
						uint child_distance = routingBase * backwards_distance + ind + 1;
						children[ind] = (destinationIndex + child_distance) % size;
					}
					return children;
				}
				else
					return new uint[] {};
			}

			#endregion
		}
	}
}
