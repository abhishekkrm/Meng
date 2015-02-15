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

namespace QS._qss_c_.Routing_1_
{
    public class LogRouting : IRoutingAlgorithm
    {
        public LogRouting()
        {
        }

        #region IRoutingAlgorithm Members

        IRoutingStructure IRoutingAlgorithm.instantiate(uint size, int modifying_index)
        {
            return new Structure(size, (uint) modifying_index);
        }

        #endregion

        #region Class Structure

        private class Structure : IRoutingStructure 
        {
            public Structure(uint size, uint modifying_index)
            {
                this.size = size;
                this.square_size = (uint) Math.Floor(Math.Sqrt((double)size));
                this.modifying_index = modifying_index;

                outgoing_indexes = new List<uint>[size];
                incoming_indexes = new List<uint>[size];

                for (uint distance = 0; distance < size; distance++)
                {
                    outgoing_indexes[distance] = new List<uint>();
                    incoming_indexes[distance] = new List<uint>();
                }

                for (uint distance = 0; distance < size; distance++)
                {
                    if (distance > 0)
                    {
                        uint position = (((distance - 1) + modifying_index) % (size - 1)) + 1; // distance in modified coordinates (after shifting)
                        uint position_parent = (uint)((position - 1) / square_size); // parent distance in modified coordinates

                        if (position_parent == 0)
                        {
                            outgoing_indexes[distance].Add(0);
                            incoming_indexes[0].Add(distance);
                        }
                        else
                        {
                            uint distance_parent = (((position_parent - 1) + (modifying_index * (size - 2))) % (size - 1)) + 1; // parent distance
                            outgoing_indexes[distance].Add(distance_parent);
                            incoming_indexes[distance_parent].Add(distance);
                        }
                    }
                }
            }

            private uint size, square_size, modifying_index;
            private List<uint>[] outgoing_indexes, incoming_indexes;

            #region IRoutingStructure Members

            uint IRoutingStructure.Size
            {
                get { return size; }
            }

            uint[] IRoutingStructure.outgoingAt(uint memberIndex, uint destinationIndex)
            {
                uint distance = (uint) (((memberIndex + size) - destinationIndex) % size);
                uint[] result = outgoing_indexes[distance].ToArray();
                for (int ind = 0; ind < result.Length; ind++)
                    result[ind] = (uint) ((destinationIndex + result[ind]) % size);
                return result;
            }

            uint[] IRoutingStructure.incomingAt(uint memberIndex, uint destinationIndex)
            {
                uint distance = (uint) (((memberIndex + size) - destinationIndex) % size);
                uint[] result = incoming_indexes[distance].ToArray();
                for (int ind = 0; ind < result.Length; ind++)
                    result[ind] = (uint) ((destinationIndex + result[ind]) % size);
                return result;
            }

            #endregion
        }

        #endregion
    }
}
