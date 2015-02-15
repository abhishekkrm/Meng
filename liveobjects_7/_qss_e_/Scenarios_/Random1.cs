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
    public class Random1 : IScenarioClass
    {
        public Random1()
        {
        }

        #region IScenarioClass Members

        IScenario IScenarioClass.Create(int nnodes, int nsenders, int ngroups, int nregions, IDictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("groups_pernode") || !parameters.ContainsKey("random_seedval"))
                throw new Exception("Bad arguments.");

            return new Scenario(nnodes, nsenders, ngroups,
                Convert.ToInt32(parameters["groups_pernode"]), Convert.ToInt32(parameters["random_seedval"]));
        }

        #endregion

        #region Class Scenario

        private class Scenario : IScenario
        {
            public Scenario(int nnodes, int nsenders, int ngroups, int groups_pernode, int random_seedval)
            {
                this.nnodes = nnodes;
                this.nsenders = nsenders;
                this.ngroups = ngroups;
                this.groups_pernode = groups_pernode;

                System.Random random = new System.Random(random_seedval);

                for (int ind = 0; ind < nnodes; ind++)
                {
                    List<int> its_groups = new List<int>();
                    for (int njoined = 0; njoined < groups_pernode; njoined++)
                    {
                        int gid;
                        do
                        {
                            gid = random.Next(ngroups);
                        }
                        while (its_groups.Contains(gid));
                        its_groups.Add(gid);
                    }

                    node_memberships.Add(ind, its_groups);
                    
                    foreach (int gid in its_groups)
                    {
                        List<int> gm;
                        if (!group_memberships.TryGetValue(gid, out gm))
                            group_memberships.Add(gid, gm = new List<int>());
                        gm.Add(ind);
                    }
                }
            }

            private int nnodes, nsenders, ngroups, groups_pernode;
            private IDictionary<int, List<int>> group_memberships = new Dictionary<int, List<int>>(), node_memberships = new Dictionary<int, List<int>>();

            #region IScenario Members

            int[] IScenario.WhereToSubscribe(int node_index)
            {
                return node_memberships[node_index].ToArray();
            }

            int[] IScenario.WhereToPublish(int node_index)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
}
