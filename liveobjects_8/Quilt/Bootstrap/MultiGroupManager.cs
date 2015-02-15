﻿/*

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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using QS.Fx.Base;
using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS._qss_x_.Properties_.Component_;

using Quilt.Multicast;
using Quilt.MulticastRules;
using Quilt.Transmitter;
using Quilt.Heuristic;

namespace Quilt.Bootstrap
{
    public class MultiGroupManager : IGroupManager
    {
        #region Field

        private QS.Fx.Logging.ILogger _logger;
        private Transmitter.Transmitter _transmitter;
        private Random _random;

        // Parameter
        private int _timeout = 10 * 1000; // 10 sec
        private int RETRY = 1;

        // Patchname format: prototype|patchid
        // A partial/global view of current online nodes [nodeid : node]
        private Dictionary<string, GroupManager.MonitorNode> _online_nodes;
        private Dictionary<string, List<string>> _subscritions;

        // Heuristic
        private IHeuristic _heuristic;

        private int _heuristic_type; // 1 for SimpleHeuristic, 2 for pure trees, 3 for c/s

        // For experiment only
        private int _total_node_num;
        //streams that have subscriptions
        private int _total_streams;
        //the total number of streams
        private int _total_stream_num;
        private int _total_subs;
        private int _total_subs2;

        private int _joined_node_num;
        private int _joined_subs;
        private int _shown_subs;
        private int _show_interval = 20;

        // Special successors for source
        private Dictionary<string, Dictionary<string, double>> _source_succs = new Dictionary<string, Dictionary<string, double>>();

        private Dictionary<string, Dictionary<string, KeyValuePair<string, double>>> _trees = null;
        private bool _need_note_overlay = true;
        private bool _need_src_overlay = false;
        private int _src_wait = 0;
        private static int SUB_SHORTAGE = 0;
        private int SUB_DONE = 0;

#if VERBOSE
        private StreamWriter _manager_log = new StreamWriter("c:\\bootstrap.log");
#endif

        #endregion

        #region Constructor

        public MultiGroupManager(Transmitter.Transmitter _transmitter, QS.Fx.Logging.ILogger _logger, int _heuristic_type)
        {
            this._logger = _logger;
            this._transmitter = _transmitter;

#if VERBOSE
            //Debug code for logging transmitter flow
            this._transmitter.SetLogTarget(_manager_log);
#endif

            this._online_nodes = new Dictionary<string, GroupManager.MonitorNode>();
            this._subscritions = new Dictionary<string, List<string>>();

            this._heuristic_type = _heuristic_type;

#if VERBOSE

            switch (this._heuristic_type)
            {
                case 1:
                    {
                        // Trees generated by simple heuristic
                        this._heuristic = new PrimalDualHeuristic();
                        //this._heuristic = new SimpleHeuristic();                        
                    }
                    break;
                case 2:
                    {
                        // Pure trees multicast
                        this._heuristic = new MulticastHeuristic();
                    }
                    break;
                case 3:
                    {
                        // Pure trees multicast
                        this._heuristic = new CSHeuristic();
                    }
                    break;
            }

            this._total_node_num = _heuristic.GetNetInfo()._nodes.Count;
            this._total_streams = _heuristic.GetSubInfo()._terminals.Count;
            this._total_stream_num = _heuristic.GetSubInfo()._streamInfo.Count;

            foreach (KeyValuePair<string, List<string>> kvp in _heuristic.GetSubInfo()._terminals)
            {
                //this._total_subs += ( kvp.Value.Count + 1);
                this._total_subs += kvp.Value.Count;
            }

            foreach (KeyValuePair<string, Dictionary<string, int>> kvp2 in _heuristic.GetSubInfo()._utilities)
            {
                this._total_subs2 += kvp2.Value.Count;
            }

            if (this._logger != null)
            {
                this._logger.Log("Quilt.Bootstrap.MultiGroupManager: Heuristic " + _heuristic_type + " Read Total Num " + _total_node_num);
                this._logger.Log("Quilt.Bootstrap.MultiGroupManager: Heuristic " + _heuristic_type + " Read Total Sub " + _total_subs);
                this._logger.Log("Quilt.Bootstrap.MultiGroupManager: Heuristic " + _heuristic_type + " Read Total Stream " + _total_stream_num);
            }

#endif
        }

        #endregion

        #region Test heuristic

#if VERBOSE
        public void TestHeuristic()
        {
            _heuristic.DagFormation(out _trees);
            StreamWriter output = new StreamWriter("C:\\output.txt");
            foreach (KeyValuePair<string, Dictionary<string, KeyValuePair<string, double>>> kvp in _trees)
            {
                output.WriteLine("stream\r" + kvp.Key);

                foreach (KeyValuePair<string, KeyValuePair<string, double>> kvp2 in kvp.Value)
                {
                    output.WriteLine(" \r" + kvp2.Key + "\r" + kvp2.Value.Key);
                }
            }
            output.Close();
        }
#endif

        #endregion

        #region IGroupManager Members

        void IGroupManager.ProcessAlive(EUIDAddress euid, BootstrapAlive alive)
        {
            // TODO, update nodes' liveness information
            // not ergent for experiment
        }

        void IGroupManager.ProcessJoin(EUIDAddress euid, BootstrapJoin join)
        {
            // Build user's online map and subscription information, to
            // generate the optimized flow trees by running heuristic

            string stream_id = join.GroupName.String;
            IMember<Name, Incarnation, Name> node = join.BootstrapMember;
            EUIDAddress node_euid = ((IMember<Name, Incarnation, Name, EUIDAddress>)node).Addresses.First();

            // Add node to online node map
            GroupManager.MonitorNode monitor_node;
            if (_online_nodes.TryGetValue(node.Identifier.String, out monitor_node))
            {
                monitor_node.last_time = DateTime.Now.Ticks;
            }
            else
            {
                // Not in the _online_nodes
                monitor_node = new GroupManager.MonitorNode();
                monitor_node.last_time = DateTime.Now.Ticks;
                monitor_node._node = join.BootstrapMember;
                _online_nodes.Add(node.Identifier.String, monitor_node);

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Quilt.Bootstrap.MultiGroupManager node " + node.Identifier.String + " join!");
                _joined_node_num++;
#endif
            }

            // Add node to subscriptions
            List<string> topicsubs;
            if (!_subscritions.TryGetValue(stream_id, out topicsubs))
            {
                topicsubs = new List<string>();
                _subscritions.Add(stream_id, topicsubs);
            }

            if (!topicsubs.Contains(node.Identifier.String))
            {
                _joined_subs++;
                topicsubs.Add(node.Identifier.String);
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("MultiGroupManager node " + node.Identifier.String + " join " + stream_id);
#endif

#if VERBOSE

            // All nodes have joined the system
            
            if (_joined_subs - _shown_subs >= _show_interval)
            {
#if VERBOSE
                if (this._logger != null)
                {
                    this._logger.Log("Quilt.Bootstrap.MultiGroupManager received " + _joined_subs + " subs!");
                    //this._logger.Log("Quilt.Bootstrap.MultiGroupManager " + Convert.ToString(_heuristic.GetSubInfo()._utilities["cs6"].Count + _heuristic.GetSubInfo()._utilities["cs7"].Count + _heuristic.GetSubInfo()._utilities["cs17"].Count + 44));
                }
                _shown_subs = _joined_subs;
#endif
            }
            if (_total_node_num <= _joined_node_num && SUB_DONE == 0 && (_total_subs + _joined_node_num + _total_stream_num - _joined_subs - SUB_SHORTAGE == 0))
            //if (_subscritions.ContainsKey("0") && _subscritions["0"].Count == 2)
            //if ((_total_node_num >= _joined_node_num) &&
            //    (_total_subs + _joined_node_num - _joined_subs == 0))
            //if ((_total_node_num >= _joined_node_num) &&
            //    (_total_subs + _joined_node_num + _total_stream_num - _joined_subs - SUB_SHORTAGE == 0))
            //if (_joined_node_num == 4 &&
            //    _joined_subs == (_heuristic.GetSubInfo()._utilities["cs2"].Count + _heuristic.GetSubInfo()._utilities["cs3"].Count + _heuristic.GetSubInfo()._utilities["cs5"].Count + 44))
            {
                if (this._logger != null)
                    this._logger.Log("Quilt.Bootstrap.MultiGroupManager:ProcessJoin Begin to run heuristic. subs " + _joined_subs + " " + (_total_subs + _joined_node_num));
                SUB_DONE = 1;
                _heuristic.DagFormation(out _trees);
            }
#endif

        }

        void IGroupManager.ProcessLeave(EUIDAddress euid, BootstrapLeave leave)
        {
            // TODO, clear node information
        }

        void IGroupManager.Callback_Maintain()
        {
            if (_trees == null) return;

            // Parse trees and send packet back
            if (_need_note_overlay)
            {
                _need_note_overlay = false;

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Quilt.Bootstrap.MultiGroupManager:Callback_Maintain start to parse and disseminate tree overlay");
#endif

                Dictionary<string, Dictionary<string, double>> stream_successors;
                foreach (KeyValuePair<string, Dictionary<string, KeyValuePair<string, double>>> kvp in _trees)
                {
                    string stream_id = kvp.Key;
                    string src_id = _heuristic.GetSubInfo().getSource(stream_id);
                    stream_successors = new Dictionary<string, Dictionary<string, double>>();

                    // Add succes for source in each stream
                    Dictionary<string, double> src_succs = new Dictionary<string,double>();
                    _source_succs.Add(stream_id, src_succs);

                    foreach (KeyValuePair<string, KeyValuePair<string, double>> kvp2 in kvp.Value)
                    {
                        string cur = kvp2.Value.Key;
                        string succ = kvp2.Key;
                        double rate = kvp2.Value.Value;

                        if (cur == "undefined") continue;

                        // Source has itself succes map
                        if (cur == src_id)
                        {
                            src_succs.Add(succ, rate);
                            //continue;
                        }

                        Dictionary<string, double> succs;
                        if (!stream_successors.TryGetValue(cur, out succs))
                        {
                            succs = new Dictionary<string, double>();
                            stream_successors.Add(cur, succs);
                        }

                        succs.Add(succ, rate);
                    }

                    // Send successor list for each node for a stream
                    // GRADIENT|0|# of streams
                    string patch_name = PROTOTYPE.GRADIENT + "|0";
                    string patch_description = patch_name + "|" + _total_streams + "|";

#if VERBOSE
                    this._manager_log.WriteLine(stream_id + "\t Successors:");
                    this._manager_log.Flush();
#endif

                    foreach (KeyValuePair<string, Dictionary<string, double>> kvp3 in stream_successors)
                    {

                        // This node has not joined the system yet
                        if (!_online_nodes.ContainsKey(kvp3.Key)) continue;

                        if (kvp3.Key == src_id ||
                            kvp3.Value.Count == 0) continue;

#if VERBOSE
                        this._manager_log.WriteLine(" \t" + kvp3.Key);
                        this._manager_log.Flush();
#endif

                        EUIDAddress remote_addr = ((IMember<Name, Incarnation, Name, EUIDAddress>)_online_nodes[kvp3.Key]._node).Addresses.First();
                        List<BootstrapMember> to_send_succs = new List<BootstrapMember>();
                        string rates = "";

                        foreach (KeyValuePair<string, double> kvp4 in kvp3.Value)
                        {
                            // Node not joined for died
                            if (!_online_nodes.ContainsKey(kvp4.Key)) continue;

                            to_send_succs.Add(_online_nodes[kvp4.Key]._node);
                            rates += (kvp4.Value + ",");

#if VERBOSE
                            this._manager_log.WriteLine(" \t \t" + kvp4.Key + "\t" + kvp4.Value);
                            this._manager_log.Flush();
#endif
                        }

                        rates = rates.TrimEnd(',');

                        // Send membership
                        PatchInfo new_patch_info = new PatchInfo(PROTOTYPE.GRADIENT, patch_description + to_send_succs.Count + "|" + rates, new EUIDAddress());
                        BootstrapMembership membership = new BootstrapMembership(
                            new Name(stream_id),
                            new Incarnation((ulong)DateTime.Now.Ticks),
                            to_send_succs, new_patch_info);
                        for (int i = 0; i < RETRY; i++)
                        {
                            _transmitter.SendMessage(remote_addr, membership);
                            //System.Threading.Thread.Sleep(500);
                        }
                    }
                }

                _need_src_overlay = true;
                //_need_src_overlay = false;

#if VERBOSE
                //_manager_log.Close();

                if (_logger != null)
                    _logger.Log("Quilt.Bootstrap.MultiGroupManager sent succesors to subscribers.");
#endif
            }

            if (_need_src_overlay)
            {
                if (_src_wait-- == 0)
                {
                    System.Threading.Thread.Sleep(10000);

#if VERBOSE
                    if (_logger != null)
                        _logger.Log("Quilt.Bootstrap.MultiGroupManager sent succesors to publisher.");
#endif

                    _need_src_overlay = false;

                    string patch_name = PROTOTYPE.GRADIENT + "|0";
                    string patch_description = patch_name + "|" + _total_streams + "|";

                    foreach (KeyValuePair<string, Dictionary<string, double>> kvp in _source_succs)
                    {

                        EUIDAddress remote_addr =
                            ((IMember<Name, Incarnation, Name, EUIDAddress>)_online_nodes[_heuristic.GetSubInfo().getSource(kvp.Key)]._node).Addresses.First();
                        List<BootstrapMember> to_send_succs = new List<BootstrapMember>();
                        string rates = "";

                        foreach (KeyValuePair<string, double> kvp2 in kvp.Value)
                        {
                            // Node not joined for died
                            if (!_online_nodes.ContainsKey(kvp2.Key)) continue;

                            to_send_succs.Add(_online_nodes[kvp2.Key]._node);
                            rates += (kvp2.Value + ",");
                        }

                        rates = rates.TrimEnd(',');

                        // Send membership
                        PatchInfo new_patch_info = new PatchInfo(PROTOTYPE.GRADIENT, patch_description + to_send_succs.Count + "|" + rates, new EUIDAddress());
                        BootstrapMembership membership = new BootstrapMembership(
                            new Name(kvp.Key),
                            new Incarnation((ulong)DateTime.Now.Ticks),
                            to_send_succs, new_patch_info);

                        for (int i = 0; i < RETRY; i++)
                        {
                            _transmitter.SendMessage(remote_addr, membership);
                            //System.Threading.Thread.Sleep(500);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
