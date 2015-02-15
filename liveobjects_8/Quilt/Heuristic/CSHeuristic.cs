/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
using System.Linq;
using System.Text;

namespace Quilt.Heuristic
{
    public class CSHeuristic : IHeuristic
    {
        #region Fields

        public NetInfos _netinfo;
        public SubInfos _subinfo;

        public Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees;

        #endregion

        #region Constructor

        public CSHeuristic()
        {
            _netinfo = new NetInfos();
            _subinfo = new SubInfos();

        }

        #endregion

        #region IHeuristic Members

        void IHeuristic.DagFormation(out Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees)
        {
            _trees = new Dictionary<string, Dictionary<string, KeyValuePair<string, double>>>();

            foreach (string stream in _subinfo._bws.Keys)
            {
                Dictionary<string, KeyValuePair<string, double>> tree = new Dictionary<string, KeyValuePair<string, double>>();
                tree.Add(_subinfo._sourceID[stream], new KeyValuePair<string,double>("undefined", 1));
                _trees.Add(stream, tree);

                foreach (string sub in _subinfo._terminals[stream])
                {
                    tree.Add(sub, new KeyValuePair<string, double>(_subinfo._sourceID[stream], 1));
                }
            }
        }

        SubInfos IHeuristic.GetSubInfo()
        {
            return _subinfo;
        }

        NetInfos IHeuristic.GetNetInfo()
        {
            return _netinfo;
        }

        #endregion
    }
}