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
    public class IsolatedClusters : IScenarioClass
    {
        public IsolatedClusters()
        {
        }

        #region IScenarioClass Members

        IScenario IScenarioClass.Create(int nnodes, int nsenders, int ngroups, int nregions, IDictionary<string, string> parameters)
        {
            return new Scenario(nnodes, nsenders, ngroups);
        }

        #endregion

        #region Class Scenario

        private class Scenario : IScenario
        {
            public Scenario(int nnodes, int nsenders, int ngroups)
            {
                this.nnodes = nnodes;
                this.nsenders = nsenders;
                this.ngroups = ngroups;

                if (nsenders != ngroups)
                    throw new Exception("The number of senders should be equal to the number of groups.");
            }

            private int nnodes, nsenders, ngroups;

            #region IScenario Members

            int[] IScenario.WhereToSubscribe(int node_index)
            {
                return new int[] { node_index % ngroups };
            }

            int[] IScenario.WhereToPublish(int node_index)
            {
                return (node_index < ngroups) ? new int[] { node_index } : new int[0]; 
            }

            #endregion
        }

        #endregion
    }
}
