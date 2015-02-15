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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quilt.Heuristic
{
    public class NetInfos
    {
        public List<string> _nodes = new List<string>();
        public Dictionary<string, int> _link_bws = new Dictionary<string, int>(); //nodeid1;nodeid2, bw_kbps
        public Dictionary<string, int> _link_latency = new Dictionary<string, int>(); //nodeid1;nodeid2, latency_latency
        public Dictionary<string, List<string>> _adjency = new Dictionary<string, List<string>>(); //nodeid, list<node, node, node>

        public NetInfos(string _file_path)
        {
            this.SetNetInfo(_file_path);
        }

        public NetInfos()
        {
            // use the default path
            this.SetNetInfo("C:/deterlab_topo.txt");
        }

        //private void SetNetInfo(string file_path)
        //{
        //    // read file, the file should reflect a topology in the format of NS file
        //    StreamReader topo_file = new StreamReader(file_path);
        //    string line = null;
        //    string[] toks;
        //    //link ID
        //    int linkID = 0;
        //    Dictionary<string, string> mappings = new Dictionary<string, string>();
        //    Dictionary<int, List<string>> links = new Dictionary<int, List<string>>();
        //    //read stream file
        //    while ((line = topo_file.ReadLine()) != null)
        //    {
        //        //parse each line to get stream info
        //        toks = line.Split(' ');
        //        //eligible line format:
        //        // "set link", linkname, "[$ns duplex-link", node1name($**), node2name($**), bandwidth(**kb), latency(**ms), "Droptail]"
        //        if (toks[0].Equals("set") && toks[1].Equals("link"))
        //        {
        //            if (toks[2].Contains("frcs"))
        //            {
        //                // if this is a link to map fake router to content server
        //                mappings.Add(toks[6], toks[5]);
        //            }
        //            else
        //            {
        //                // if this line is eligible
        //                List<string> link_temp = new List<string>(4);
        //                // add node1 "$csr**" or "$cs**"
        //                if (toks[5].Contains("fr"))
        //                {
        //                    link_temp.Add(mappings[toks[5]]);
        //                }
        //                else
        //                {
        //                    link_temp.Add(toks[5]);
        //                }
        //                // add node2 "$csr**" or "$cs**"
        //                if (toks[6].Contains("fr"))
        //                {
        //                    link_temp.Add(mappings[toks[6]]);
        //                }
        //                else
        //                {
        //                    link_temp.Add(toks[6]);
        //                }
        //                // add bandwidth ".0kb" removed
        //                link_temp.Add(toks[7].Remove(toks[7].Length - 4));
        //                // add latency ".0ms" removed
        //                link_temp.Add(toks[8].Remove(toks[8].Length - 4));
        //                links.Add(linkID++, link_temp);
        //            }
        //        }
        //    }

        //    // format the links according to info structure
        //    foreach (int i in links.Keys)
        //    {
        //        // remove '$' first, then if it is "csr", replace it with "cs", 
        //        string node1 = links[i][0].Remove(0, 1).Replace("csr", "cs");
        //        string node2 = links[i][1].Remove(0, 1).Replace("csr", "cs");
        //        if (!this._nodes.Contains(node1))
        //        {
        //            this._nodes.Add(node1);
        //        }
        //        if (!this._nodes.Contains(node2))
        //        {
        //            this._nodes.Add(node2);
        //        }
        //        this._link_bws[node1 + ";" + node2] = Convert.ToInt32(links[i][2]);
        //        this._link_latency[node1 + ";" + node2] = Convert.ToInt32(links[i][3]);

        //        if (this._adjency.ContainsKey(node1))
        //        {
        //            this._adjency[node1].Add(node2);
        //        }
        //        else
        //        {
        //            List<string> adjencies = new List<string>();
        //            adjencies.Add(node2);
        //            this._adjency[node1] = adjencies;
        //        }

        //        if (this._adjency.ContainsKey(node2))
        //        {
        //            this._adjency[node2].Add(node1);
        //        }
        //        else
        //        {
        //            List<string> adjencies = new List<string>();
        //            adjencies.Add(node1);
        //            this._adjency[node2] = adjencies;
        //        }
        //    }
        //}

        //modified by Bo Peng, 2011-04-07, to cope with the new topology format
        private void SetNetInfo(string file_path)
        {
            // read file, the file should reflect a topology in the format of NS file
            StreamReader topo_file = new StreamReader(file_path);
            string line = null;
            string[] toks;
            //link ID
            int linkID = 0;
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            Dictionary<int, List<string>> links = new Dictionary<int, List<string>>();
            List<string> src_lan = new List<string>();
            //read topo file
            while ((line = topo_file.ReadLine()) != null)
            {
                //parse each line to get link info
                toks = line.Split(' ');
                //eligible line format:
                // 1. "set", lanname, "[$ns make-lan", lanmember1("$cs**), lanmember2($cs**), ..., lanmemberN($cs**"), bandwidth(**Mb), lantency(**ms])
                if (toks.Length < 7)
                    continue;
                if (toks[3].Equals("make-lan"))
                {
                    // a list of lan members
                    List<string> lan_temp = new List<string>();
                    lan_temp.Add(toks[4].Remove(0, 1)); // addd the first member $cs**
                    int i = 5;
                    while (!toks[i].Contains("\"")) // if this is not the last lan member
                    {
                        lan_temp.Add(toks[i++]);
                    }
                    lan_temp.Add(toks[i].Remove(toks[i].Length - 1)); //remove the " in the last lan member tok
                    if (!toks[1].Equals("lan0")) //if this is not the source lan
                    {
                        // loop to create new link
                        int j = 0;
                        int k = 0;
                        for (j = 0; j < lan_temp.Count - 1; j++)
                            for (k = j + 1; k < lan_temp.Count; k++)
                            {
                                List<string> link_temp = new List<string>(4);
                                //add node1
                                link_temp.Add(lan_temp[j]);
                                //add node2
                                link_temp.Add(lan_temp[k]);
                                //add bandwidth, while converting Mb to kb
                                link_temp.Add(Convert.ToString(Convert.ToInt32(toks[i + 1].Remove(toks[i + 1].Length - 2)) * 1000));
                                //add latency
                                link_temp.Add(toks[i + 2].Remove(toks[i + 2].Length - 3));
                                links.Add(linkID++, link_temp);
                            }
                    }
                    else
                    {
                        src_lan = lan_temp;
                    }
                }
                // 2. "set link", linkname, "[$ns duplex-link", node1name($**), node2name($**), bandwidth(**Mb), latency(**ms), "Droptail]"
                if (toks[0].Equals("set") && toks[1].Equals("link"))
                {
                    if (src_lan.Contains(toks[5]))
                    {
                        foreach (string s in src_lan)
                        {
                            List<string> link_temp = new List<string>(4);
                            //add a src node as node1
                            link_temp.Add(s);
                            //add node2
                            link_temp.Add(toks[6]);
                            //add bandwidth
                            link_temp.Add(Convert.ToString(Convert.ToInt32(toks[7].Remove(toks[7].Length - 2)) * 1000));
                            link_temp.Add(toks[8].Remove(toks[8].Length - 2));
                            links.Add(linkID++, link_temp);
                        }
                    }
                    else if (src_lan.Contains(toks[6]))
                    {
                        foreach (string s in src_lan)
                        {
                            List<string> link_temp = new List<string>(4);
                            //add node1
                            link_temp.Add(toks[5]);
                            //add a src node as node2
                            link_temp.Add(s);
                            //add bandwidth
                            link_temp.Add(Convert.ToString(Convert.ToInt32(toks[7].Remove(toks[7].Length - 2)) * 1000));
                            link_temp.Add(toks[8].Remove(toks[8].Length - 2));
                            links.Add(linkID++, link_temp);
                        }
                    }
                    else
                    {
                        List<string> link_temp = new List<string>(4);
                        //add node1
                        link_temp.Add(toks[5]);
                        //add node2
                        link_temp.Add(toks[6]);
                        //add bandwidth
                        link_temp.Add(Convert.ToString(Convert.ToInt32(toks[7].Remove(toks[7].Length - 2)) * 1000));
                        link_temp.Add(toks[8].Remove(toks[8].Length - 2));
                        links.Add(linkID++, link_temp);
                    }
                }
            }

            // format the links according to info structure
            foreach (int i in links.Keys)
            {
                // remove '$' first
                string node1 = links[i][0].Remove(0, 1);
                string node2 = links[i][1].Remove(0, 1);
                if (!this._nodes.Contains(node1))
                {
                    this._nodes.Add(node1);
                }
                if (!this._nodes.Contains(node2))
                {
                    this._nodes.Add(node2);
                }
                this._link_bws[node1 + ";" + node2] = Convert.ToInt32(links[i][2]);
                this._link_latency[node1 + ";" + node2] = Convert.ToInt32(links[i][3]);

                if (this._adjency.ContainsKey(node1))
                {
                    this._adjency[node1].Add(node2);
                }
                else
                {
                    List<string> adjencies = new List<string>();
                    adjencies.Add(node2);
                    this._adjency[node1] = adjencies;
                }

                if (this._adjency.ContainsKey(node2))
                {
                    this._adjency[node2].Add(node1);
                }
                else
                {
                    List<string> adjencies = new List<string>();
                    adjencies.Add(node1);
                    this._adjency[node2] = adjencies;
                }
            }
        }
    }
}
