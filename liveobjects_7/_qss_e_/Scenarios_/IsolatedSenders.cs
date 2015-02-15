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

namespace QS._qss_e_.Scenarios_
{
    public class IsolatedSenders : IScenarioClass
    {
        public IsolatedSenders()
        {
        }

        #region IScenario Members

        IScenario IScenarioClass.Create(int nnodes, int nsenders, int ngroups, int nregions, IDictionary<string, string> parameters)
        {
            return new Scenario(nnodes, nsenders, ngroups, nregions);
        }

        #endregion

        #region Class Scenario

        private class Scenario : IScenario
        {
            public Scenario(int nnodes, int nsenders, int ngroups, int nregions)
            {
                this.nnodes = nnodes;
                this.nsenders = nsenders;
                this.ngroups = ngroups;
                this.nregions = nregions;

                if (ngroups < nsenders)
                    throw new Exception("Bad parameters.");
            }

            private int nnodes, nsenders, ngroups, nregions;

            #region IScenario Members

            int[] IScenario.WhereToSubscribe(int node_index)
            {
                if (node_index < nsenders)
                {
                    return new int[] { node_index };
                }
                else
                {
                    int[] groupids = new int[ngroups + ((nregions > 1) ? 1 : 0)];
                    for (int ind = 0; ind < ngroups; ind++)
                        groupids[ind] = ind;
                    if (nregions > 1)
                        groupids[ngroups] = ngroups + (node_index - nsenders) % nregions;
                    return groupids;
                }
            }

            int[] IScenario.WhereToPublish(int node_index)
            {
                if (node_index < nsenders)
                {
                    return new int[] { node_index };
                }
                else
                    return new int[0];
            }

            #endregion
        }

        #endregion
    }
}
