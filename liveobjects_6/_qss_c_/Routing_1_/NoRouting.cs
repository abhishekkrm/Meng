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
    public class NoRouting : IRoutingAlgorithm
    {
        private static NoRouting algorithm = null;
        public static NoRouting Algorithm
        {
            get
            {
                if (algorithm == null)
                    algorithm = new NoRouting();
                return algorithm;
            }
        }

        private NoRouting()
        {
        }

        #region Class Structure

        private class Structure : IRoutingStructure
        {
            public Structure(uint size)
            {
                this.size = size;
            }

            private uint size;

            #region IRoutingStructure Members

            public uint Size
            {
                get { return size; }
            }

            public uint[] outgoingAt(uint memberIndex, uint destinationIndex)
            {
                return (memberIndex == destinationIndex) ? (new uint[] {}) : (new uint[] { destinationIndex });
            }

            public uint[] incomingAt(uint memberIndex, uint destinationIndex)
            {
                if (memberIndex == destinationIndex)
                {
                    uint[] indexes = new uint[size - 1];
                    uint position = 0;
                    for (uint ind = 0; ind < size; ind++)
                    {
                        if (ind != destinationIndex)
                            indexes[position++] = ind;
                    }

                    return indexes;
                }
                else
                    return new uint[] {};
            }

            #endregion
        }

        #endregion

        #region IRoutingAlgorithm Members

        public IRoutingStructure instantiate(uint size, int modifying_index)
        {
            return new Structure(size);
        }

        #endregion
    }
}
