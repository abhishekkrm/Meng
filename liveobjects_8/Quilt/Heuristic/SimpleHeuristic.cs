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
    public class SimpleHeuristic : IHeuristic
    {
        #region Fields

        // Network information, including bandwidth, latency of each link
        public NetInfos _netinfo;
        // Subscrition information, including bandwidth, quality rates and terminals of different streams
        public SubInfos _subinfo;

        // Structure used for best candidate
        public class Candidate
        {
            public Dictionary<string, string> _shortest_path;
            public string _terminal;
            public string _tree_node;
            public string _stream;
            public double _rate;
        }

        // A map of trees for streams, the format is:
        // (stream_id, (node_id, (previous_id, quality_rate)))
        public Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees;

        #endregion

        #region Constructor

        public SimpleHeuristic()
        {
            _netinfo = new NetInfos();
            _subinfo = new SubInfos();

        }

        #endregion

        #region IHeuristic Members

        /// <summary>
        /// Output the trees
        /// </summary>
        /// <param name="_trees"></param>
        void IHeuristic.DagFormation(out Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees)
        {
            _trees = new Dictionary<string, Dictionary<string, KeyValuePair<string, double>>>();

            // At first put the source (data center node) into all trees
            foreach (string stream in _subinfo._bws.Keys)
            {
                Dictionary<string, KeyValuePair<string, double>> tree = new Dictionary<string, KeyValuePair<string, double>>();
                tree.Add(_subinfo._sourceID[stream], new KeyValuePair<string,double>("undefined", 0));
                _trees.Add(stream, tree);

                _subinfo._terminals[stream].Remove(_subinfo._sourceID[stream]);
            }

            // Run algorithm
            while (true)
            {
                double best_benefit = 0.0;
                Candidate best_candidate = new Candidate();
                Dictionary<string, string> shortest_path;
                foreach (string stream in _subinfo._bws.Keys)
                {
                    // This list copy is added to avoid exception generated
                    // by removing an elem when iterating its container
                    List<string> terminals_to_iterate = new List<string>();
                    foreach (string terminal in _subinfo._terminals[stream])
                    {
                        terminals_to_iterate.Add(terminal);
                    }

                    foreach (string terminal in terminals_to_iterate)
                    {
                        foreach (double rate in _subinfo._rates)
                        {
                            shortest_path = DijkstraSP.FindPath(_subinfo, _netinfo, terminal, stream, rate, _trees[stream]);

                            if (shortest_path == null)
                            {
                                // Mark terminal as covered
                                _subinfo._terminals[stream].Remove(terminal);
                                break;
                            }
                            else
                            {
                                int cost = 0;
                                foreach (KeyValuePair<string, string> kvp in shortest_path)
                                {
                                    // The previous of the first node in a path is "undefined"
                                    if (kvp.Value == "undefined") continue;

                                    string link = kvp.Key + ";" + kvp.Value;
                                    if (!_netinfo._link_latency.ContainsKey(link))
                                    {
                                        link = kvp.Value + ";" + kvp.Key;
                                    }
                                    cost += _netinfo._link_latency[link];
                                }

                                if (cost == 0)
                                {
                                    throw new Exception("!!!!!!!! Cost Zero!!!!!!");
                                }


                                // TODO, include bandwidth consumption in the cost??
                                double benefit = (double)_subinfo._utilities[terminal][stream] * rate / cost;

                                if (benefit > best_benefit)
                                {
                                    best_candidate._rate = rate;
                                    best_candidate._shortest_path = shortest_path;
                                    best_candidate._stream = stream;
                                    best_candidate._terminal = terminal;
                                    //best_candidate._tree_node = 

                                    best_benefit = benefit;
                                }
                            }
                        }// end of rate iteration
                    }// end of terminal
                }// end of stream

                if (best_benefit == 0)
                    break;

                // Construct a path from covered tree to the best candidate
                foreach (KeyValuePair<string, string> kvp in best_candidate._shortest_path)
                {
                    if (kvp.Value == "undefined") continue;

                    string link = kvp.Key + ";" + kvp.Value;
                    if (!_netinfo._link_latency.ContainsKey(link))
                    {
                        link = kvp.Value + ";" + kvp.Key;
                    }
                    _netinfo._link_bws[link] -= (int)(best_candidate._rate * _subinfo._bws[best_candidate._stream]);

                    if (!_trees[best_candidate._stream].ContainsKey(kvp.Value))
                    {
                        _trees[best_candidate._stream].Add(kvp.Value, new KeyValuePair<string, double>(kvp.Key, best_candidate._rate));
                    }
                }

	            //Remove t from Ti  [Since the terminal is now covered]
                _subinfo._terminals[best_candidate._stream].Remove(best_candidate._terminal);
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
