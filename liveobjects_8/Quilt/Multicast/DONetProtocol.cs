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

using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS.Fx.Base;

using Quilt.Bootstrap;
using Quilt.Core;
using Quilt.Transmitter;

namespace Quilt.Multicast
{
    public class DONetProtocol : IMulticast
    {
        #region Constructor

        public DONetProtocol()
        {
            // set timeout intervals
            this._member_timeout = 10 * _schedule_interval;
        }

        #endregion

        #region Fields

        private QuiltPeer.ShareState _share_state;
        private QuiltPeer.ReceivedData _received_callback;
        private Transmitter.Transmitter _transmitter;

        private int _schedule_interval = 1 * 1000; // milliseconds
        private int _member_timeout;
        private int _partner_limit = 6; // max number of partner to diesseminate buffermap
        private int _member_low = 1;

        private Random _rand = new Random();
        private Dictionary<string, HeartbeatMember> _members = new Dictionary<string, HeartbeatMember>();
        //private Dictionary<string, DONetPartner> _partners = new Dictionary<string, DONetPartner>();

        // Serial_no of held data in DataBuffer
        private List<double> _snapshot = new List<double>();
        private double _latest_expired;

        // Interested data condition
        private Dictionary<string, List<string>> _interested = new Dictionary<string, List<string>>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ProcessMembership

        void ProcessMembership(EUIDAddress remote_addr, DONetMembership membership)
        {
            try
            {
                double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                foreach (HeartbeatMember member in membership.Members)
                {
                    BootstrapMember node = member._member;
                    string id = ((IMember<Name, Incarnation, Name, EUIDAddress>)node).Identifier.String;

                    // Do not put self in the membership
                    if (id == _share_state._self_id.String) continue;

                    HeartbeatMember local;
                    if (_members.TryGetValue(id, out local))
                    {
                        if (local._heartbeat < member._heartbeat)
                        {
                            local._heartbeat = member._heartbeat;
                        }
                    }
                    else
                    {
                        _members.Add(id, member);
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Exception("DONetProtocol ProcessMembership " + exc.Message);
            }
        }

        #endregion

        #region ProcessBuffermap

        private void ProcessBuffermap(EUIDAddress remote_addr, DONetBuffermap buffermap)
        {
            try
            {
                double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                BootstrapMember node = buffermap._self;
                string id = ((IMember<Name, Incarnation, Name, EUIDAddress>)node).Identifier.String;

                // Check if this is a new node
                HeartbeatMember new_node;
                if (_members.TryGetValue(id, out new_node))
                {
                    new_node._heartbeat = now;
                }
                else
                {
                    new_node = new HeartbeatMember(node, now);
                    _members.Add(id, new_node);
                }

                // Update interest dictionary
                if (buffermap._snapshot == null)
                {
                    return;
                }

#if VERBOSE
                lock (_share_state._log)
                {
                    _share_state._log.WriteLine(now + "\trecv\tbuffermap\t" + buffermap._snapshot.Count + "\t" + ((QS.Fx.Serialization.ISerializable)buffermap).SerializableInfo.Size);
                    _share_state._log.Flush();
                }
#endif

                foreach (double seq in buffermap._snapshot)
                {
                    if (seq <= _latest_expired || _share_state._data_buffer.HasReceived(seq))
                    {
                        continue;
                    }

                    List<string> candidates;
                    if (_interested.TryGetValue(seq.ToString(), out candidates))
                    {
                        if (!candidates.Contains(id))
                        {
                            candidates.Add(id);
                        }
                    }
                    else
                    {
#if VERBOSE
                        //lock (_share_state._log)
                        //{
                        //    _share_state._log.WriteLine(now + "DONet add interested data " + seq.ToString());
                        //    _share_state._log.Flush();
                        //}
#endif

                        candidates = new List<string>();
                        candidates.Add(id);
                        _interested.Add(seq.ToString(), candidates);
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Exception("DONetProtocol ProcessBuffermap " + exc.Message);
            }
        }

        #endregion

        #region ProcessRequest

        private void ProcessRequest(EUIDAddress remote_addr, DONetRequest message)
        {
            try
            {
                double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                DataBuffer.Data local_data = _share_state._data_buffer.GetData(message._seq_no);

                if (local_data != null)
                {
                    DONetData data = new DONetData(local_data);
                    _transmitter.SendMessage(remote_addr, data);

#if VERBOSE
                    lock (_share_state._log)
                    {
                        _share_state._log.WriteLine(now + "\tsend\tdonetdata\t" + message._seq_no + "\t" + ((QS.Fx.Serialization.ISerializable)data).SerializableInfo.Size);
                        _share_state._log.Flush();
                    }
#endif
                }
            }
            catch (Exception exc)
            {
                throw new Exception("DONetProtocol ProcessRequest " + exc.Message);
            }
        }

        #endregion

        #region ProcessData

        private void ProcessData(EUIDAddress remote_addr, DONetData data)
        {
            try
            {
                // Remove from interest list
                _interested.Remove(data._data._serial_no.ToString());

                _received_callback(data._data, PROTOTYPE.DONET);
            }
            catch (Exception exc)
            {
                throw new Exception("DONetProtocol ProcessData " + exc.Message);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IMulticast Members

        double IMulticast.GetScheduleInterval()
        {
            return _schedule_interval;
        }

        void IMulticast.Schedule()
        {
            double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // 1. Membership maintenance
            List<string> to_del = new List<string>();
            foreach (KeyValuePair<string, HeartbeatMember> kvp in _members)
            {
                if ((cur - kvp.Value._heartbeat) > _member_timeout)
                    to_del.Add(kvp.Key);
            }

            foreach (string id in to_del)
            {
                if (_members.Count > _member_low)
                {
                    _members.Remove(id);
                }
            }

            if (_members.Count == 0) return;

            DONetMembership membership = new DONetMembership(_members, _share_state._self);
            int i = _rand.Next(_members.Count);
            EUIDAddress remote = ((IMember<Name, Incarnation, Name, EUIDAddress>)_members.ElementAt(i).Value._member).Addresses.First();
            _transmitter.SendMessage(remote, membership);

            // 2. Partner maintenance
            _latest_expired = _share_state._data_buffer.GetSnapshot(ref _snapshot);

            if (_snapshot.Count > 0)
            {

                DONetBuffermap bufermap = new DONetBuffermap(_snapshot, _share_state._self);

#if VERBOSE
                if (bufermap._snapshot.Count > 0)
                {
                    lock (_share_state._log)
                    {
                        _share_state._log.WriteLine("Advocating buffermap with " + bufermap._snapshot.Count + " data");
                        _share_state._log.Flush();
                    }
                }
#endif

                if (_members.Count <= _partner_limit)
                {
                    foreach (KeyValuePair<string, HeartbeatMember> kvp in _members)
                    {
                        EUIDAddress remote2 = ((IMember<Name, Incarnation, Name, EUIDAddress>)kvp.Value._member).Addresses.First();
                        _transmitter.SendMessage(remote2, bufermap);
                    }
                }
                else
                {
                    List<string> selected = new List<string>();
                    int index = _rand.Next(_members.Count);

                    while (selected.Count < _partner_limit)
                    {
                        KeyValuePair<string, HeartbeatMember> kvp = _members.ElementAt(index);
                        if (!selected.Contains(kvp.Key))
                        {
                            selected.Add(kvp.Key);
                            EUIDAddress remote3 = ((IMember<Name, Incarnation, Name, EUIDAddress>)kvp.Value._member).Addresses.First();
                            _transmitter.SendMessage(remote3, bufermap);
                        }

                        index += _members.Count / _partner_limit;
                    }

                    selected.Clear();
                }
            }

            // 3. Data schedule
            foreach (KeyValuePair<string, List<string>> kvp in _interested)
            {
                int index = _rand.Next(kvp.Value.Count);
                HeartbeatMember source = _members[kvp.Value[index]];
                EUIDAddress remote3 = ((IMember<Name, Incarnation, Name, EUIDAddress>)source._member).Addresses.First();
                DONetRequest request = new DONetRequest(double.Parse(kvp.Key));
                _transmitter.SendMessage(remote3, request);
            }
        }

        void IMulticast.PublishData(DataBuffer.Data data)
        {
            // Update the local snapshot, do nothing or aggressively advertise the data availability
        }

        void IMulticast.Join(Quilt.Bootstrap.PatchInfo patch_info, List<Quilt.Bootstrap.BootstrapMember> members, Quilt.Transmitter.Transmitter transmitter)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            _transmitter = transmitter;

            // Initialize the membership, partnership
            foreach (BootstrapMember member in members)
            {
                string id = ((IMember<Name, Incarnation, Name, EUIDAddress>)member).Identifier.String;

                if (id == _share_state._self_id.String) continue;

                HeartbeatMember donet_member = new HeartbeatMember(member, now);
                _members.Add(id, donet_member);
            }
        }

        void IMulticast.ProcessMessage(QS.Fx.Value.EUIDAddress remote_addr, QS.Fx.Serialization.ISerializable message)
        {
            try
            {
                switch (message.SerializableInfo.ClassID)
                {
                    case (ushort)QS.ClassID.DonetMembership:
                        ProcessMembership(remote_addr, (DONetMembership)message);
                        break;
                    case (ushort)QS.ClassID.DonetBuffermap:
                        ProcessBuffermap(remote_addr, (DONetBuffermap)message);
                        break;
                    case (ushort)QS.ClassID.DonetRequest:
                        ProcessRequest(remote_addr, (DONetRequest)message);
                        break;
                    case (ushort)QS.ClassID.DonetData:
                        ProcessData(remote_addr, (DONetData)message);
                        break;
                    default:
                        throw new Exception("Unknown message type");
                }
            }
            catch (Exception exc)
            {
                throw new Exception("DONetProtocol.ProcessMessage " + exc.Message);
            }
        }

        void IMulticast.SetCallback(Quilt.Core.QuiltPeer.ReceivedData call_back)
        {
            _received_callback = call_back;
        }

        void IMulticast.SetShareState(Quilt.Core.QuiltPeer.ShareState share_state)
        {
            _share_state = share_state;
        }

        #endregion
    }
}
