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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QS.Fx.Base;
using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS._qss_x_.Properties_.Component_;

using Quilt.Multicast;
using Quilt.MulticastRules;
using Quilt.Transmitter;

namespace Quilt.Bootstrap
{
    public class GroupManager : IGroupManager
    {
        #region Constructor

        public GroupManager(Name _group_name, Transmitter.Transmitter _transmitter, PROTOTYPE _inter_proto, ref Dictionary<string, IRule> _multicast_rules, QS.Fx.Logging.ILogger _logger)
        {
            this._logger = _logger;
            this._transmitter = _transmitter;
            this._group_name = _group_name;
            this._group_incarnation = new Incarnation((ulong)DateTime.Now.Ticks);
            this._random = new Random();
            this._inter_proto = _inter_proto;
            string inter_proto_str = _inter_proto.ToString();

            this._multicast_rules = _multicast_rules;

            // Initialize the fields for maintaining patches
            _online_nodes = new Dictionary<string, MonitorNode>();
            _patch_leader_num = new Dictionary<string, int>();

            _patch_nodes = new Dictionary<string, Dictionary<string, Dictionary<string, MonitorNode>>>();
            _patch_nodes.Add(PROTOTYPE.IPMC.ToString(), new Dictionary<string, Dictionary<string, MonitorNode>>());
            _patch_nodes.Add(PROTOTYPE.DONET.ToString(), new Dictionary<string, Dictionary<string, MonitorNode>>());
            _patch_nodes.Add(PROTOTYPE.OMNI.ToString(), new Dictionary<string, Dictionary<string, MonitorNode>>());

            // Inter-patch has one and only one patch
            _patch_nodes[inter_proto_str].Add(inter_proto_str + "|0", new Dictionary<string, MonitorNode>());

            _patch_infos = new Dictionary<string, PatchInfo>();
        }

        #endregion

        #region Field
        private QS.Fx.Logging.ILogger _logger;
        private PROTOTYPE _inter_proto;
        private Transmitter.Transmitter _transmitter;
        private Name _group_name;
        private Incarnation _group_incarnation;
        private Random _random;

        // Parameter
        private int _min_delegate = 1;
        private int _max_delegate = 1;
        private int _timeout = 10 * 1000; // 10 sec

        // Patchname format: prototype|patchid

        // A partial/global view of current online nodes [nodeid : node]
        private Dictionary<string, MonitorNode> _online_nodes;
        // Patch leaders' numbers [patchname : number of leaders in the patch]
        private Dictionary<string, int> _patch_leader_num;
        // Patch members [prototype : [patchname : [nodeid : node]]]
        private Dictionary<string, Dictionary<string, Dictionary<string, MonitorNode>>> _patch_nodes;
        // Multicast rules [prototype : rule]
        private Dictionary<string, IRule> _multicast_rules;
        // Patch information [patchname : patchinfo]
        private Dictionary<string, PatchInfo> _patch_infos;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region AssignRegionalPatch

        private void AssignRegionalPatch(EUIDAddress euid, MonitorNode monitor_node)
        {
            // Assign patches to this node
            foreach (KeyValuePair<string, IRule> kvp in _multicast_rules)
            {
                string prototype = kvp.Key;

                if (prototype == _inter_proto.ToString())
                {
                    // Assign regional patches first
                    continue;
                }

                IRule rule = kvp.Value;

                // Check the availability of running protocol
                if (!rule.IsPermitted(monitor_node))
                {
                    continue;
                }

                // Check which patch can join
                Dictionary<string, Dictionary<string, MonitorNode>> proto_patches = _patch_nodes[prototype];

                int patch_count = 0;

                foreach (KeyValuePair<string, Dictionary<string, MonitorNode>> kvp2 in proto_patches)
                {
                    patch_count++;

                    string patch_name = kvp2.Key;
                    PatchInfo info = _patch_infos[patch_name];

                    if (!rule.CanJoin(monitor_node, info))
                    {
                        // Can not join this patch
                        continue;
                    }

                    // Join this patch
                    rule.Join(monitor_node, info);
                    monitor_node._patches.Add(patch_name);

                    // Send membership
                    List<BootstrapMember> member_list = new List<BootstrapMember>();

                    foreach (KeyValuePair<string, MonitorNode> kvp_member in kvp2.Value)
                    {
                        member_list.Add(kvp_member.Value._node);
                    }

                    BootstrapMembership membership = new BootstrapMembership(
                        _group_name, _group_incarnation, member_list, info);

                    _transmitter.SendMessage(euid, membership);

#if VERBOSE
                    if (_logger != null)
                        _logger.Log("Quilt.Bootstrap.GroupManager assign regional patch " + info._patch_description.String);
#endif

                    kvp2.Value.Add(((IMember<Name, Incarnation, Name>)monitor_node._node).Identifier.String, monitor_node);

                    // Only join one patch of the same protocol family
                    break;
                }

                if (monitor_node._patches.Count == 0)
                {
                    // Haven't joined any patch yet, create one
                    string information;
                    EUIDAddress new_patch_euid = rule.CreatePatch(monitor_node, _group_name, out information);
                    string patch_name = prototype + "|" + patch_count.ToString();
                    PatchInfo new_patch_info = new PatchInfo(ProtoType.StringToType(prototype), patch_name + "|" + information, new_patch_euid);

                    // Insert new patch and register new patch inside node
                    monitor_node._patches.Add(patch_name);
                    _patch_infos.Add(patch_name, new_patch_info);
                    Dictionary<string, MonitorNode> new_patch_nodes = new Dictionary<string, MonitorNode>();
                    new_patch_nodes.Add(((IMember<Name, Incarnation, Name>)monitor_node._node).Identifier.String, monitor_node);
                    proto_patches.Add(patch_name, new_patch_nodes);

                    // Send membership
                    BootstrapMembership membership = new BootstrapMembership(
                        _group_name, _group_incarnation, new List<BootstrapMember>(), new_patch_info);

#if VERBOSE
                    if (_logger != null)
                        _logger.Log("Quilt.Bootstrap.GroupManager create regional patch " + new_patch_info._patch_description.String);
#endif

                    _transmitter.SendMessage(euid, membership);
                }
            }

        }

        #endregion

        #region AssignInterPatch

        private void AssignInterPatch(EUIDAddress euid, MonitorNode monitor_node)
        {
            string inter_proto_str = _inter_proto.ToString();
            IRule inter_rule = _multicast_rules[inter_proto_str];

            // Check the availability of running protocol
            if (!inter_rule.IsPermitted(monitor_node))
            {
                return;
            }

            // Get the inter patch dictionary
            Dictionary<string, MonitorNode> inter_patch_nodes = _patch_nodes[inter_proto_str][inter_proto_str + "|0"];

            PatchInfo inter_patch_info;
            string inter_patch_name = inter_proto_str + "|0";
            bool root = false;
            if (!_patch_infos.TryGetValue(inter_patch_name, out inter_patch_info))
            {
                // First node, should be root of the patch
                // No patch info yet, create the patch
                string group_info;
                EUIDAddress inter_patch_euid = inter_rule.CreatePatch(monitor_node, _group_name, out group_info);

                inter_patch_info = new PatchInfo(_inter_proto, inter_patch_name + "|" + group_info, inter_patch_euid);
                
                // Register the patch information
                _patch_infos.Add(inter_patch_name, inter_patch_info);

                root = true;
            }
            
            if (!inter_rule.CanJoin(monitor_node, inter_patch_info))
            {
                // Can not join this patch

                if (root)
                {
                    // Destroy the unusable inter-patch
                    _patch_infos.Remove(inter_patch_name);
                }

                return;
            }

            if (!root)
            {
                // Inter rulle join
                inter_rule.Join(monitor_node, inter_patch_info);
            }

            foreach (string patch in monitor_node._patches)
            {
                if (!_patch_leader_num.ContainsKey(patch))
                {
                    _patch_leader_num.Add(patch, 1);
                }
                else
                {
                    _patch_leader_num[patch]++;
                }
            }

            // Register the inter_patch to node
            monitor_node._patches.Add(inter_patch_name);

            // Send membership
            List<BootstrapMember> member_list = new List<BootstrapMember>();

            foreach (KeyValuePair<string, MonitorNode> kvp_member in inter_patch_nodes)
            {
                member_list.Add(kvp_member.Value._node);
            }

            BootstrapMembership membership = new BootstrapMembership(
                _group_name, _group_incarnation, member_list, inter_patch_info);

            _transmitter.SendMessage(euid, membership);

#if VERBOSE
            if (_logger != null)
                _logger.Log("Quilt.Bootstrap.GroupManager assign inter patch " + inter_patch_info._patch_description.String + " for " + euid.String);
#endif

            // Put node in the patch dictionary
            inter_patch_nodes.Add(((IMember<Name, Incarnation, Name>)monitor_node._node).Identifier.String, monitor_node);
        }

        #endregion

        #region FindDelegate

        private MonitorNode FindDelegate(string patch)
        {
            string proto = patch.Split('|')[0];

            foreach (KeyValuePair<string, MonitorNode> kvp in _patch_nodes[proto][patch])
            {
                EUIDAddress euid = ((IMember<Name, Incarnation, Name, EUIDAddress>)kvp.Value._node).Addresses.First();

                if (euid.GetProtocolInfo("TCP").proto_direct == Quilt.HostDetector.NATCheck.Direction.DIRECTION.BIDIRECTION
                    || euid.GetProtocolInfo("UDP").proto_direct == Quilt.HostDetector.NATCheck.Direction.DIRECTION.BIDIRECTION)
                {
                    return kvp.Value;
                }
            }

            // Random if not a single node is good enough
            return _patch_nodes[proto][patch].ElementAt(_random.Next(0, _patch_nodes[proto][patch].Count)).Value; 
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MonitorNode

        public class MonitorNode
        {
            public BootstrapMember _node;
            public double last_time;
            public List<string> _patches = new List<string>();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IGroupManager Members

        void IGroupManager.ProcessAlive(EUIDAddress euid, BootstrapAlive alive)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            IMember<Name, Incarnation, Name> node = alive.BootstrapMember;
            EUIDAddress node_euid = ((IMember<Name, Incarnation, Name, EUIDAddress>)node).Addresses.First();

            MonitorNode monitor_node;
            // Check if the node is already in the _online_nodes
            if (_online_nodes.TryGetValue(node.Identifier.String, out monitor_node))
            {
                monitor_node.last_time = now;
            }
            else
            {
                // Insert
                monitor_node = new MonitorNode();
                monitor_node.last_time = DateTime.Now.Ticks;
                monitor_node._node = alive.BootstrapMember;
                _online_nodes.Add(node.Identifier.String, monitor_node);

                // Assign regional patch
                AssignRegionalPatch(euid, monitor_node);

                // Assign inter-patch
                AssignInterPatch(euid, monitor_node);
            }
        }

        void IGroupManager.ProcessJoin(EUIDAddress euid, BootstrapJoin join)
        {            
            IMember<Name, Incarnation, Name> node = join.BootstrapMember;
            EUIDAddress node_euid = ((IMember<Name, Incarnation, Name, EUIDAddress>)node).Addresses.First();

            MonitorNode monitor_node;
            // Check if the node is already in the _online_nodes
            if (_online_nodes.TryGetValue(node.Identifier.String, out monitor_node))
            {
                // Environment is the same
                if (((IMember<Name, Incarnation, Name, EUIDAddress>)monitor_node._node).Addresses.First().Equals(node_euid))
                {
                    // Update the time
                    monitor_node.last_time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                    // Reassign the same patches
                    foreach (string patch in monitor_node._patches)
                    {
                        string proto = patch.Split('|')[0];
                        PatchInfo info = _patch_infos[patch];
                        List<BootstrapMember> member_list = new List<BootstrapMember>();

                        Dictionary<string, MonitorNode> patch_members = _patch_nodes[proto][patch];

                        foreach (KeyValuePair<string, MonitorNode> kvp in patch_members)
                        {
                            if (((IMember<Name, Incarnation, Name>)kvp.Value._node).Identifier.Equals(node.Identifier))
                            {
                                // Do not send itself back
                                continue;
                            }

                            member_list.Add(kvp.Value._node);
                        }

                        BootstrapMembership membership = new BootstrapMembership(
                            _group_name, _group_incarnation, member_list, info);

                        _transmitter.SendMessage(euid, membership);
                    }

                    // Function ends
                    return;
                }
                else
                {
                    // Replace the node
                    monitor_node._node = join.BootstrapMember;
                    monitor_node.last_time = DateTime.Now.Ticks;

                    // Remove the old management information of this node
                    foreach (string patch in monitor_node._patches)
                    {
                        string proto = patch.Split('|')[0];
                        Dictionary<string, MonitorNode> patch_members = _patch_nodes[proto][patch];
                        patch_members.Remove(patch);

                        // If this node used to be a delegate
                        if (monitor_node._patches.Count > 1 && proto != _inter_proto.ToString())
                        {
                            // Decrease the number of delegates of this patch
                            _patch_leader_num[patch]--;
                        }
                    }

                    monitor_node._patches.Clear();
                }
            }
            else
            {
                // Not in the _online_nodes
                monitor_node = new MonitorNode();
                monitor_node.last_time = DateTime.Now.Ticks;
                monitor_node._node = join.BootstrapMember;
                _online_nodes.Add(node.Identifier.String, monitor_node);
            }

            // Assign regional patch
            AssignRegionalPatch(euid, monitor_node);

            // Assign inter-patch
            AssignInterPatch(euid, monitor_node);        
        }

        void IGroupManager.ProcessLeave(EUIDAddress euid, BootstrapLeave leave)
        {
            // Do nothing
        }

        void IGroupManager.Callback_Maintain()
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            List<string> to_remove = new List<string>();
            // Check the availability of online nodes
            foreach (KeyValuePair<string, MonitorNode> kvp in _online_nodes)
            {
                MonitorNode node = kvp.Value;
                bool isDelegate = false;

                if (now - node.last_time > _timeout)
                {
                    // Need to remove this node from patches
                    foreach (string patch in node._patches)
                    {
                        string proto = patch.Split('|')[0];
                        _patch_nodes[proto][patch].Remove(kvp.Key);

#if VERBOSE
                        if (_logger != null)
                            _logger.Log("Quilt.Bootstrap.GroupManager timeout " + kvp.Key + " from " + patch);
#endif

                        if (proto == _inter_proto.ToString())
                        {
                            isDelegate = true;
                        }

                        // TODO: might need to remove the whole patch
                    }

                    // If delegate, adjust regional patches
                    if (isDelegate)
                    {
                        foreach (string patch in node._patches)
                        {
                            string proto = patch.Split('|')[0];
                            if (proto != _inter_proto.ToString())
                            {
                                _patch_leader_num[patch]--;

                                if (_patch_leader_num[patch] < _min_delegate)
                                {
                                    //Need to find a delegate for that patch
                                    MonitorNode next_del = FindDelegate(patch);
                                    EUIDAddress next_del_euid = ((IMember<Name, Incarnation, Name, EUIDAddress>)next_del._node).Addresses.First();
                                    AssignInterPatch(next_del_euid, next_del);
                                }
                            }
                        }
                    }

                    // Clear patchs
                    node._patches.Clear();

                    to_remove.Add(kvp.Key);
                }
            }

            // Remove nodes
            foreach (string id in to_remove)
            {
                _online_nodes.Remove(id);
            }
        }

        #endregion

    }
}
