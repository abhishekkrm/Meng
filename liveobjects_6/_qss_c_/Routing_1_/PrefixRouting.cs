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

namespace QS._qss_c_.Routing_1_
{
	/// <summary>
	/// Summary description for PrefixRoutingStructure.
	/// </summary>
	public class PrefixRouting : IRoutingAlgorithm
	{
        public static PrefixRouting Algorithm(uint routingBase)
        {
            return new PrefixRouting(routingBase);
        }

        public PrefixRouting(uint routingBase)
		{
			if (routingBase < 2)
				throw new Exception("Routing base must not be smaller than 2.");
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
                this.modifying_index = (uint) modifying_index;
			}

			private uint size, routingBase, modifying_index;

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
					uint distance = (destinationIndex + size - memberIndex) % size;
					uint fingerid = ((uint) Math.Floor(Math.Log((double) distance, (double) routingBase)));
					uint stepsize = (uint) Math.Pow((double) routingBase, (double) fingerid);
					uint numsteps = distance / stepsize;
					uint hopstogo = stepsize * numsteps;
					return new uint[] { (memberIndex + hopstogo) % size };
				}
			}

			public uint[] incomingAt(uint memberIndex, uint destinationIndex)
			{
				uint fw_distance = (destinationIndex + size - memberIndex) % size;
				uint bk_distance = ((memberIndex + size - destinationIndex - 1) % size) + 1;

				int min_finger = 
					((int) Math.Ceiling(Math.Log((double) (fw_distance + 1), (double) routingBase)));
				int max_finger = 
					((int) Math.Ceiling(Math.Log((double) (bk_distance), (double) routingBase)) - 1);

				if (max_finger < min_finger)
					return new uint[] { };
				else
				{
					uint maxfinger_stepsize = (uint) Math.Pow((double) routingBase, (double) max_finger);
					uint maxfinger_numsteps = (bk_distance - 1) / maxfinger_stepsize;

					uint numof_full_fingers = (uint) (max_finger - min_finger);
					uint full_fingers_total = numof_full_fingers * (routingBase - 1);
					uint total_num_incoming = full_fingers_total + maxfinger_numsteps;
					uint[] fingers = new uint[total_num_incoming];

					for (uint ind = (uint) min_finger; ind < (uint) max_finger; ind++)
					{
						uint stepsize = (uint) Math.Pow((double) routingBase, (double) ind);
						for (uint fno = 1; fno < routingBase; fno++)
						{
							uint hopstogo = stepsize * fno;
							uint index = (memberIndex + size - hopstogo) % size;
							fingers[(ind - min_finger) * (routingBase - 1) + fno - 1] = index;								
						}
					}

					for (uint fno = 1; fno <= maxfinger_numsteps; fno++)
					{
						uint hopstogo = maxfinger_stepsize * fno;
						uint index = (memberIndex + size - hopstogo) % size;
						fingers[full_fingers_total + fno - 1] = index;							
					}

					return fingers;
				}
			}

			#endregion
		}
	}
}
