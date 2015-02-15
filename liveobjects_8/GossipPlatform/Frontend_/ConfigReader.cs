/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using GOTransport.Core;
using GOTransport.GossipTransport;
using GOTransport.Common;
using GOTransport.Frontend;

namespace GOTransport.Frontend
{
    /// <summary>
    /// read and store configuration values set in the config file
    /// </summary>
    public class PlatformConfigReader
    {
        #region private fields

        private List<InstanceContainer> definitions;

        #endregion

        #region constructor

        /// <summary>
        /// read the values from configuration file and save them in a list of InstanceContainer
        /// </summary>
        public PlatformConfigReader(string _outPorts, string _gossipIntervals, string _rumorTimeouts, string _rounds)
        {
            definitions = new List<InstanceContainer>();

            int iter = 0;
            if (_outPorts != null)
            {
                string[] outPorts = _outPorts.Split(',');
                string[] gossipIntervals = _gossipIntervals.Split(',');
                string[] rumorTimeouts = _rumorTimeouts.Split(',');
                string[] numberOfRoundsToGossip = _rounds.Split(',');

                foreach (string port in outPorts)
                {
                    definitions.Add(new InstanceContainer(port, gossipIntervals[iter], rumorTimeouts[iter], numberOfRoundsToGossip[iter]));
                    iter++;
                }
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// return the list of platform definitions, as specified in config file
        /// </summary>
        /// <returns>list of platform definitions</returns>
        public List<InstanceContainer> GetDefinitions()
        {
            return definitions;
        }

        /// <summary>
        /// check if at least one platform is defined in the config file
        /// </summary>
        /// <returns>true if at least one platform is define, false otherwise</returns>
        internal bool HasDefinitions()
        {
            return definitions != null && definitions.Count > 0
                   ? true
                   : false;
        }

        #endregion
    }
}
