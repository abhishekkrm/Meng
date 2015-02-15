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
using System.IO;

namespace Quilt.Heuristic
{
    public class PrimalDualHeuristic : IHeuristic
    {
        #region Fields

        // Network information, including bandwidth, latency of each link
        public NetInfos _netinfo;
        // Subscrition information, including bandwidth, quality rates and terminals of different streams
        public SubInfos _subinfo;

        // A map of trees for streams, the format is:
        // (stream_id, (node_id, (previous_id, quality_rate)))
        public Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees;

        private string path;

        #endregion

        #region Constructor

        public PrimalDualHeuristic()
        {
            _netinfo = new NetInfos();
            _subinfo = new SubInfos();

            path = "c:\\trees\\";
        }

        #endregion

        #region IHeuristic Members

        void IHeuristic.DagFormation(out Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees)
        {
            _trees = new Dictionary<string, Dictionary<string, KeyValuePair<string, double>>>();
            StreamReader reader;
            char[] set = {' ', '\t'};

            //Test
            //List<string> temp = new List<string>();
            //temp.Add("10");
            //temp.Add("35");
            //foreach(string stream in temp)
            foreach (string stream in _subinfo._bws.Keys)
            {
                Dictionary<string, KeyValuePair<string, double>> tree = new Dictionary<string, KeyValuePair<string, double>>();
                _trees.Add(stream, tree);

                reader = new StreamReader(path + stream);
                string line;
                while (null != (line = reader.ReadLine()))
                {
                    string[] elems = line.Split(set, StringSplitOptions.RemoveEmptyEntries);
                    KeyValuePair<string, double> predecessor;
                    //to handle the incorrect generated overlay
                    if (!tree.TryGetValue(elems[0], out predecessor))
                    {
                        predecessor = new KeyValuePair<string, double>(elems[1], double.Parse(elems[2]) / 100);
                        tree.Add(elems[0], predecessor);
                    }
                    else
                    {
                        if (predecessor.Value < double.Parse(elems[2]) / 100)
                        {
                            tree.Remove(elems[0]);
                            tree.Add(elems[0], new KeyValuePair<string, double>(elems[1], double.Parse(elems[2]) / 100));
                        }
                    }
                }
                reader.Close();
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
