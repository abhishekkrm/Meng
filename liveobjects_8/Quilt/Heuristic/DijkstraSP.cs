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
    class DijkstraSP
    {
        /// <summary>
        /// Get the Shortest-Path DAG from a terminal t_i (subscribed to stream i at quality r) to all other
        /// nodes in the whole topology; then select the shortest path from t_i to a node covered in Tree(i).
        /// </summary>
        /// <param name="_subinfo">Subscrition information, including bw, quality rates and terminals</param>
        /// <param name="_netinfo">Network information, including adjency node list, link bw/latency</param>
        /// <param name="_t">Terminal to build the DAG</param>
        /// <param name="_i">Stream id</param>
        /// <param name="_r">Quality rate</param>
        /// <param name="_tree">Constructed tree for stream i, format is (node,(previous node,rate))</param>
        /// <returns></returns>
        static public Dictionary<string, string> FindPath(SubInfos _subinfo, NetInfos _netinfo, string _t, string _i, double _r, 
            Dictionary<string, KeyValuePair<string, double>> _tree)
        {
            Dictionary<string, int> dist = new Dictionary<string, int>();
            Dictionary<string, string> previous = new Dictionary<string, string>();
            List<string> temp_nodes = new List<string>();

            // Initialize the dist and previous
            foreach (string node in _netinfo._nodes)
            {                
                if (node == _t)
                {
                    dist.Add(_t, 0);
                }
                else
                {
                    dist.Add(node, 10000);
                }

                previous.Add(node, "undefined");
                temp_nodes.Add(node);
            }

            // Build Dijkstra Shortest-Path DAG from t_i to all nodes in the topology
            string next_node;
            List<string> neighbors;

            while (temp_nodes.Count != 0)
            {
                next_node = FindNode(dist, temp_nodes);

                // Get neighbors
                if (next_node == "")
                {
                    break;
                }
                
                neighbors = _netinfo._adjency[next_node];

                int alt_lat;
                foreach (string neighbor in neighbors)
                {
                    string link = next_node + ";" + neighbor;
                    if (!_netinfo._link_latency.ContainsKey(link))
                    {
                        link = neighbor + ";" + next_node;
                    }

                    alt_lat = dist[next_node] + _netinfo._link_latency[link];

                    // Choose the link with lower latency subject to the required bandwidth
                    if (alt_lat < dist[neighbor] && _netinfo._link_bws[link] > _subinfo._bws[_i] * _r)
                    {
                        dist[neighbor] = alt_lat;
                        previous[neighbor] = next_node;
                    }
                }

                temp_nodes.Remove(next_node);
            }

            // Find the best path to a node in Tree(i)
            // Due to the bandwidth rate limitation, previous may not be full or even null
            int min_dist = 1000;
            string tree_node = "";
            foreach(KeyValuePair<string, KeyValuePair<string, double>> kvp in _tree)
            {
                //iterate the covered nodes
                if (previous.ContainsKey(kvp.Key))
                {
                    if (dist[kvp.Key] < min_dist)
                    {
                        // Update the node to connect from the covered tree
                        tree_node = kvp.Key;
                    }
                }
            }

            if (tree_node == "")
            {
                return null;
            }
            else
            {
                Dictionary<string, string> result_path = new Dictionary<string, string>();
                do
                {
                    result_path.Add(tree_node, previous[tree_node]);
                    tree_node = previous[tree_node];
                }while(tree_node != "undefined");

                return result_path;
            }
        }

        /// <summary>
        /// Get the Shortest-Path DAG from a terminal t_i to all nodes in the same stream.
        /// This function is for generating isolated multicast tree for each stream.
        /// </summary>
        /// <param name="_subinfo"></param>
        /// <param name="_netinfo"></param>
        /// <param name="_t"></param>
        /// <param name="_i"></param>
        /// <returns></returns>
        static public Dictionary<string, KeyValuePair<string, double>> GetDag(SubInfos _subinfo, NetInfos _netinfo, string _t)
        {
            Dictionary<string, int> dist = new Dictionary<string, int>();
            Dictionary<string, KeyValuePair<string, double>> previous = new Dictionary<string, KeyValuePair<string, double>>();
            List<string> temp_nodes = new List<string>();

            string separater = ";";

            // Add source in, as the first node
            //dist.Add(_t, 0);
            //temp_nodes.Add(_t);
            //previous.Add(_t, new KeyValuePair<string, double>("undefined", 1));

            // Initialize the dist and previous
            foreach (string node in _netinfo._nodes)
            {
                if (node == _t)
                {
                    dist.Add(_t, 0);
                }
                else
                {
                    dist.Add(node, 10000);
                }

                previous.Add(node, new KeyValuePair<string, double>("undefined", 1));
                temp_nodes.Add(node);
            }

            // Build Dijkstra Shortest-Path DAG from t_i to all nodes in the topology
            string next_node;
            List<string> neighbors;

            try
            {

                while (temp_nodes.Count != 0)
                {
                    next_node = FindNode(dist, temp_nodes);

                    // Get neighbors
                    if (next_node == "")
                    {
                        break;
                    }

                    neighbors = _netinfo._adjency[next_node];

                    int alt_lat;
                    foreach (string neighbor in neighbors)
                    {
                        string link = next_node + separater + neighbor;
                        if (!_netinfo._link_latency.ContainsKey(link))
                        {
                            link = neighbor + separater + next_node;
                        }

                        alt_lat = dist[next_node] + _netinfo._link_latency[link];

                        // Choose the link with lower latency subject to the required bandwidth
                        if (alt_lat < dist[neighbor])
                        {
                            dist[neighbor] = alt_lat;
                            previous[neighbor] = new KeyValuePair<string, double>(next_node, 1);
                        }
                    }

                    temp_nodes.Remove(next_node);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Dijkstra SP. GetDag for multicast " + exc.Message);
            }

            return previous;
        }

        // Get the next node
        static public string FindNode(Dictionary<string, int> _dist, List<string> _nodes)
        {
            int distance = 1000;
            int value;
            string node = "";

            foreach (KeyValuePair<string, int> kvp in _dist)
            {
                if (_nodes.Contains(kvp.Key))
                {
                    value = _dist[kvp.Key];
                    if (value < distance)
                    {
                        distance = value;
                        node = kvp.Key;
                    }
                }
            }

            return node;
        }
    }
}
