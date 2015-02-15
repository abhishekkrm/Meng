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

using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS.Fx.Base;

using Quilt.MulticastRules;
using Quilt.Core;
using Quilt.Bootstrap;
using Quilt.Transmitter;

namespace Quilt.Multicast
{
    public class OMNIProtocol : IMulticast
    {
        #region Constructor

        public OMNIProtocol()
        {
        }

        #endregion

        #region Field

        private QuiltPeer.ShareState _share_state;
        private QuiltPeer.ReceivedData _received_callback;
        private double _interval = 5 * 1000;
        private double _timeout = 10 * 1000;
        private Transmitter.Transmitter _transmitter;
        private Random _rand = new Random();

        private BootstrapMember _root;

        // Returned from bootstrap, to join the tree
        private List<BootstrapMember> _caches;

        private bool _hasJoined = false;
        private OMNINode _parent;
        private int _outbounds;
        private Dictionary<string, OMNINode> _children;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region JoinTree

        private void JoinTree()
        {
            // TODO: This function can be optimized to select a connectable node from _caches

            // Root itself does not need to join
            Name id = ((IMember<Name, Incarnation, Name, EUIDAddress>)_root).Identifier;
            if (id.String == _share_state._self_id.String)
            {
                _hasJoined = true;
                return;
            }

            // Send root a join message

            EUIDAddress target = ((IMember<Name, Incarnation, Name, EUIDAddress>)_root).Addresses.First();
            OMNIJoin join = new OMNIJoin(_share_state._self_id);
            _transmitter.SendMessage(target, join);
        }

        #endregion

        #region ProcessJoin

        private void ProcessJoin(EUIDAddress remote, OMNIJoin join)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            OMNINode join_node = new OMNINode();
            join_node._heartbeat_time = now;
            List<EUIDAddress> join_addrs = new List<EUIDAddress>();
            join_addrs.Add(remote);
            join_node._node = new BootstrapMember(join.JoinId, new Incarnation(1), join.JoinId, join_addrs);

            if (_children.Count < _outbounds)
            {
                // Add child
                _children.Add(join.JoinId.String, join_node);

                // Return Ack
                List<EUIDAddress> self_addrs = new List<EUIDAddress>();
                self_addrs.Add(_share_state._self_euid);
                OMNIJoinAck ack = new OMNIJoinAck(new BootstrapMember(_share_state._self_id, new Incarnation(1), _share_state._self_id, self_addrs));
                _transmitter.SendMessage(remote, ack);
            }
            else
            {
                // Redirect to a child
                // Should be based on the latency

                // Temorarily pick a radom child
                int index = _rand.Next(0, _outbounds);
                OMNINode picked = _children.ElementAt(index).Value;

                OMNIRedirect redirect = new OMNIRedirect(picked._node);
                _transmitter.SendMessage(remote, redirect);
            }
        }

        #endregion

        #region ProcessRedirect

        private void ProcessRedirect(EUIDAddress remote, OMNIRedirect redirect)
        {
            // Send join to the redirected node
            EUIDAddress target = ((IMember<Name, Incarnation, Name, EUIDAddress>)redirect.Target).Addresses.First();
            OMNIJoin join = new OMNIJoin(_share_state._self_id);
            _transmitter.SendMessage(target, join);
        }

        #endregion

        #region ProcessAck

        private void ProcessAck(EUIDAddress remote, OMNIJoinAck ack)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Set parent and state
            _parent = new OMNINode();
            _parent._heartbeat_time = now;
            _parent._node = ack.Parent;

            _hasJoined = true;
        }

        #endregion

        #region ProcessMaintain

        private void ProcessMaintain(EUIDAddress remote, OMNIMaintain maintain)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Update heartbeat time of child
            OMNINode child;
            if (!_children.TryGetValue(maintain.Id.String, out child))
            {
                // Can also throw exception if needed
                return;
            }
            child._heartbeat_time = now;

            // Send ack back
            OMNIMaintainAck ack = new OMNIMaintainAck(maintain.LocalTime);
            _transmitter.SendMessage(remote, ack);
        }

        #endregion

        #region ProcessMaintainAck

        private void ProcessMaintainAck(EUIDAddress remote, OMNIMaintainAck maintain_ack)
        {
            // Update the heartbeat of parent
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _parent._heartbeat_time = now;
        }

        #endregion

        #region ProcessData

        private void ProcessData(EUIDAddress remote, OMNIData data)
        {
            // Use callback to notify the received data
            _received_callback(data._data, PROTOTYPE.OMNI);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IMulticast Members

        double IMulticast.GetScheduleInterval()
        {
            return _interval;
        }

        void IMulticast.Schedule()
        {
            if (!_hasJoined)
            {
                JoinTree();
                return;
            }

            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Check the availability of children and parent
            string root_id = ((IMember<Name, Incarnation, Name, EUIDAddress>)_root).Identifier.String;
            if (_share_state._self_id.String != root_id)
            {
                // Root itself does not have a parent

                if (now - _parent._heartbeat_time > _timeout)
                {
                    // Lost connection to the parent
                    _parent = null;
                    _hasJoined = false;
                }
                else
                {
                    // Send maintenance messages to parent
                    OMNIMaintain omni_maintain = new OMNIMaintain(now, _share_state._self_id);
                    EUIDAddress target = ((IMember<Name, Incarnation, Name, EUIDAddress>)_parent._node).Addresses.First();
                    _transmitter.SendMessage(target, omni_maintain);
                }
            }

            List<string> to_remove = new List<string>();
            foreach (KeyValuePair<string, OMNINode> kvp in _children)
            {
                if (now - kvp.Value._heartbeat_time > _timeout)
                {
                    to_remove.Add(kvp.Key);
                }
            }

            foreach (string id in to_remove)
            {
                _children.Remove(id);
            }
        }

        void IMulticast.PublishData(DataBuffer.Data data)
        {
            OMNIData omni_data = new OMNIData(data);

            foreach (KeyValuePair<string, OMNINode> kvp in _children)
            {
                EUIDAddress target = ((IMember<Name, Incarnation, Name, EUIDAddress>)kvp.Value._node).Addresses.First();
                _transmitter.SendMessage(target, omni_data);
            }
        }

        void IMulticast.Join(Quilt.Bootstrap.PatchInfo patch_info, List<Quilt.Bootstrap.BootstrapMember> members, Quilt.Transmitter.Transmitter transmitter)
        {
            if (_hasJoined)
            {
                // Has already joined the tree
                return;
            }

            _transmitter = transmitter;

            // Set root
            this._caches = members;
            string root_id = patch_info._patch_description.String.Split('|')[3];
            EUIDAddress root_euid = patch_info._patch_euid;
            List<EUIDAddress> temp = new List<EUIDAddress>();
            temp.Add(root_euid);
            _root = new BootstrapMember(new Name(root_id), new Incarnation(1), new Name(root_id), temp);

            JoinTree();
        }

        void IMulticast.ProcessMessage(QS.Fx.Value.EUIDAddress remote_addr, QS.Fx.Serialization.ISerializable message)
        {
            try
            {
                switch (message.SerializableInfo.ClassID)
                {
                    case (ushort)QS.ClassID.OmniJoin:
                        ProcessJoin(remote_addr, (OMNIJoin)message);
                        break;
                    case (ushort)QS.ClassID.OmniAck:
                        ProcessAck(remote_addr, (OMNIJoinAck)message);
                        break;
                    case (ushort)QS.ClassID.OmniRedirect:
                        ProcessRedirect(remote_addr, (OMNIRedirect)message);
                        break;
                    case (ushort)QS.ClassID.OmniMaintain:
                        ProcessMaintain(remote_addr, (OMNIMaintain)message);
                        break;
                    case (ushort)QS.ClassID.OmniMaintainAck:
                        ProcessMaintainAck(remote_addr, (OMNIMaintainAck)message);
                        break;
                    case (ushort)QS.ClassID.OmniData:
                        ProcessData(remote_addr, (OMNIData)message);
                        break;
                    default:
                        throw new Exception("Unknown message type");
                }
            }
            catch (Exception exc)
            {
                throw new Exception("OMNIProtocol.ProcessMessage " + exc.Message);
            }
        }

        void IMulticast.SetCallback(Quilt.Core.QuiltPeer.ReceivedData call_back)
        {
            this._received_callback = call_back;
        }

        void IMulticast.SetShareState(QuiltPeer.ShareState share_state)
        {
            this._share_state = share_state;

            // Should use EUID's performance to estimate the outbounds
            this._outbounds = 3;
            this._children = new Dictionary<string, OMNINode>(_outbounds);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class OMNINode

        public class OMNINode
        {
            public BootstrapMember _node;
            public double _heartbeat_time;
            public int _client_num;
            public double _latency_to_root;
        }

        #endregion

    }
}
