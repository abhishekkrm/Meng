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
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;

using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS.Fx.Base;

using Quilt.Core;
using Quilt.Bootstrap;

namespace Quilt.Multicast
{
    public class IPMCProtocol : IMulticast
    {
        #region Field

        private string _patch_description;
        private QuiltPeer.ShareState _share_state;
        private Transmitter.Transmitter _transmitter;
        private double _gossip_interval = 1000; // 1 sec
        private int _member_timeout = 3 * 1000;

        private Dictionary<string, Tuple_<HeartbeatMember, Name>> _members = new Dictionary<string, Tuple_<HeartbeatMember, Name>>();
        private List<string> _delegates = new List<string>();
        private Queue<double> _synchronous_queue = new Queue<double>();
        private Random _rand = new Random();

        // Bo
        private IPAddress _localAddr;
        private Dictionary<IPAddress, RecvSocket> _mRecvSocks;
        private Dictionary<IPAddress, SendSocket> _mSendSocks;

        private QuiltPeer.ReceivedData _callback;

        public class RecvSocket
        {
            public Socket _sock = null;
            public bool _state = false;
        }

        public class SendSocket
        {
            public Socket _sock = null;
            //public byte[] _msg = null;
            public Queue<byte[]> _msgs = null;
        }

        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public static int BufferSize = 10 * 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
        }

        //check if the packet is sent by nodes in Quilt
        public static byte[] quiltheader = Encoding.ASCII.GetBytes("Quilt");

        #endregion

        #region Constructor

        public IPMCProtocol()
        {
            this._mRecvSocks = new Dictionary<IPAddress, RecvSocket>();
            this._mSendSocks = new Dictionary<IPAddress, SendSocket>();

            //buffersize should be the message size + quiltheader size
            StateObject.BufferSize = int.Parse("10240") + IPMCProtocol.quiltheader.Length;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IMulticast Members

        double IMulticast.GetScheduleInterval()
        {
            return this._gossip_interval;
        }

        void IMulticast.Schedule()
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Gossip membership
            // 1. Membership maintenance
            List<string> to_del = new List<string>();
            foreach (KeyValuePair<string, Tuple_<HeartbeatMember, Name>> kvp in _members)
            {
                if ((now - (kvp.Value.x._heartbeat)) > _member_timeout)
                    to_del.Add(kvp.Key);
            }

            foreach (string id in to_del)
            {
                _members.Remove(id);

                if (_delegates.Contains(id))
                {
                    _delegates.Remove(id);
                }
            }

            if (_members.Count == 0) return;

            IPMCGossip membership = new IPMCGossip(_members, _share_state._self, _share_state._isDelegate);
            int i = _rand.Next(_members.Count);
            EUIDAddress remote = ((IMember<Name, Incarnation, Name, EUIDAddress>)_members.ElementAt(i).Value.x._member).Addresses.First();
            _transmitter.SendMessage(remote, membership);
        }

        void IMulticast.PublishData(DataBuffer.Data data)
        {
            if (!_share_state._isDelegate || _members.Count == 0)
            {
                //return;
            }

            if (_delegates.Count == 0)
            {
                SendData(data);
            }
            else
            {

            }
            //throw new NotImplementedException();
           
        }

        void IMulticast.Join(Quilt.Bootstrap.PatchInfo patch_info, List<Quilt.Bootstrap.BootstrapMember> members, Quilt.Transmitter.Transmitter transmitter)
        {
            // Transmitter is for control messages
            this._transmitter = transmitter;

            //patch description is the group address of this patch
            IPAddress mip = IPAddress.Parse(patch_info._patch_description.ToString().Split('|')[2]);

            bool same = false;

            if (_patch_description == patch_info._patch_description.ToString())
            {
                same = true;
            }
            else
            {
                _patch_description = patch_info._patch_description.ToString();
            }

            // Initializing membership
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            foreach (BootstrapMember member in members)
            {
                string id = ((IMember<Name, Incarnation, Name, EUIDAddress>)member).Identifier.String;

                if (id == _share_state._self_id.String) continue;

                HeartbeatMember ipmc_member = new HeartbeatMember(member, now);
                Name is_delegate = new Name("unknown");
                _members.Add(id, new Tuple_<HeartbeatMember, Name>(ipmc_member, is_delegate, 0));
            }

            if (!same)
            {
                if (this._Join(mip) == false)
                {
                    throw new Exception("Unable to join this patch");
                }
            }
        }

        void IMulticast.ProcessMessage(QS.Fx.Value.EUIDAddress remote_addr, QS.Fx.Serialization.ISerializable message)
        {
            try
            {
                switch (message.SerializableInfo.ClassID)
                {
                    case (ushort)QS.ClassID.IpmcGossip:
                        ProcessGossip(remote_addr, (IPMCGossip)message);
                        break;
                    //case (ushort)QS.ClassID.DonetBuffermap:
                    //    ProcessBuffermap(remote_addr, (DONetBuffermap)message);
                    //    break;
                    //case (ushort)QS.ClassID.DonetRequest:
                    //    ProcessRequest(remote_addr, (DONetRequest)message);
                    //   break;
                    //case (ushort)QS.ClassID.DonetData:
                    //    ProcessData(remote_addr, (DONetData)message);
                    //    break;
                    default:
                        throw new Exception("Unknown message type");
                }
            }
            catch (Exception exc)
            {
                throw new Exception("DONetProtocol.ProcessMessage " + exc.Message);
            }
        }

        void IMulticast.SetCallback(QuiltPeer.ReceivedData call_back)
        {
            this._callback = call_back;
        }

        void IMulticast.SetShareState(QuiltPeer.ShareState share_state)
        {
            this._share_state = share_state;
            string udp_ip_str = _share_state._self_euid.GetProtocolInfo("UDP").proto_addr.Split('/', ':')[2];
            this._localAddr = IPAddress.Parse(udp_ip_str);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ProcessGossip

        void ProcessGossip(EUIDAddress remote_addr, IPMCGossip message)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            foreach (Tuple_<HeartbeatMember, Name> member in message.Members)
            {
                BootstrapMember node = member.x._member;
                string id = ((IMember<Name, Incarnation, Name, EUIDAddress>)node).Identifier.String;

                // Do not put self in the membership
                if (id == _share_state._self_id.String) continue;

                Tuple_<HeartbeatMember, Name> local;
                if (_members.TryGetValue(id, out local))
                {
                    if (local.x._heartbeat < member.x._heartbeat)
                    {
                        local.x._heartbeat = member.x._heartbeat;
                    }

                    if (local.y.String == "unknown")
                    {
                        local.y = member.y;
                    }
                }
                else
                {
                    _members.Add(id, member);
                }

                if (member.y.String == "True" && !_delegates.Contains(id))
                {
                    _delegates.Add(id);
                }
            }
        }

        #endregion

        #region SendData

        unsafe private void SendData(DataBuffer.Data data)
        {
            IPMCData ipmc_data = new IPMCData(data);
            try
            {
                //construct message
                QS.Fx.Serialization.SerializableInfo info = ((QS.Fx.Serialization.ISerializable)ipmc_data).SerializableInfo;
                QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(
                    (uint)(info.HeaderSize + 2 * sizeof(uint) + sizeof(ushort)));
                IList<QS.Fx.Base.Block> blocks = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
                blocks.Add(header.Block);
                fixed (byte* headerptr_0 = header.Array)
                {
                    byte* headerptr = headerptr_0 + header.Offset;
                    *((ushort*)(headerptr)) = (ushort)info.ClassID;
                    headerptr += sizeof(ushort);
                    *((uint*)headerptr) = (uint)info.HeaderSize;
                    headerptr += sizeof(uint);
                    *((uint*)headerptr) = (uint)info.Size;
                }
                header.consume(2 * sizeof(uint) + sizeof(ushort));
                ((QS.Fx.Serialization.ISerializable)ipmc_data).SerializeTo(ref header, ref blocks);
                MemoryStream memorystream = new MemoryStream();
                Stream _outs;

                _outs = memorystream;

                foreach (QS.Fx.Base.Block _block in blocks)
                    _outs.Write(_block.buffer, (int)_block.offset, (int)_block.size);

                byte[] _packetData = memorystream.ToArray();

                //send data to all groups this peer has joined
                if (this._MulticastToAll(_packetData) == false)
                {
                    throw new Exception("Send data fails");
                }
            }
            catch
            {
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IPMCProtocol Members

        //send udpmsg to the ip address
        private bool _Multicast(byte[] udpmsg, IPAddress mip)
        {
            SendSocket s;
            try
            {
                if (!this._mSendSocks.TryGetValue(mip, out s))
                {
                    s = new SendSocket();
                    s._msgs = new Queue<byte[]>();
                    s._msgs.Enqueue(udpmsg);
                    s._sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    s._sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 255);

                    // Add by Qi
                    IPEndPoint ipep = new IPEndPoint(this._localAddr, 5001);
                    s._sock.Bind(ipep);

                    IPEndPoint rmep = new IPEndPoint(mip, 5000);
                    s._sock.BeginConnect(rmep, new AsyncCallback(_ConnCallback), s);

                    _mSendSocks.Add(mip, s);
                }
                else
                {
                    bool is_sending = s._msgs.Count > 0;
                    s._msgs.Enqueue(udpmsg);

                    if (!is_sending)
                    {
                        byte[] packet = new byte[StateObject.BufferSize];
                        IPMCProtocol.quiltheader.CopyTo(packet, 0);
                        s._msgs.Dequeue().CopyTo(packet, IPMCProtocol.quiltheader.Length);

                        //send
                        s._sock.BeginSend(packet, 0, packet.Length, SocketFlags.None, new AsyncCallback(this._SendCallback), s._sock);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool _MulticastToAll(byte[] udpmsg)
        {
            foreach (IPAddress m in this._mRecvSocks.Keys)
            {
                if (!this._Multicast(udpmsg, m))
                {
                    return false;
                }
            }
            return true;
        }

        private bool _Join(IPAddress mip)
        {
            RecvSocket s = null;
            try
            {
                if (!this._mRecvSocks.TryGetValue(mip, out s))
                {
                    //new a socket
                    s = new RecvSocket();
                    s._sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                   
                    // Should be bound to Any, using specific interface to join IGMP membership
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5000);
                    s._sock.Bind(ipep);

                    s._sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(mip, _localAddr));
                    s._sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 5);

                    s._state = true;

                    //add to multicast address dictionary
                    this._mRecvSocks.Add(mip, s);

                    //start receiving incoming packets
                    StateObject state = new StateObject();
                    state.workSocket = s._sock;

                    //s._sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(this._RecvCallback), state);
                    EndPoint rmep = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                    s._sock.BeginReceiveFrom(state.buffer, 0, StateObject.BufferSize, 0, ref rmep, new AsyncCallback(this._RecvCallback), state);
                }
                else
                {
                    if (s._state == false)
                    {
                        s._state = true;
                    }
                }
            }
            catch (Exception exc)
            {
                this._mRecvSocks.Remove(mip);
                if (s != null && s._sock != null)
                {
                    try
                    {
                        s._sock.Close();
                    }
                    catch
                    {
                    }
                }
                return false;
            }
            return true;
        }

        private bool _Leave(IPAddress mip)
        {
            RecvSocket s = null;
            if (this._mRecvSocks.TryGetValue(mip, out s))
            {
                //leave the multicast group
                try
                {
                    s._state = false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        #region Private Callbacks

        private void _ConnCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                SendSocket s = (SendSocket)ar.AsyncState;

                // Complete the connection.
                s._sock.EndConnect(ar);

                //construct the packet to send, msg + quiltheader
                if (s._msgs.Count > 0)
                {
                    byte[] packet = new byte[StateObject.BufferSize];
                    IPMCProtocol.quiltheader.CopyTo(packet, 0);
                    s._msgs.Dequeue().CopyTo(packet, IPMCProtocol.quiltheader.Length);

                    //send
                    s._sock.BeginSend(packet, 0, packet.Length, SocketFlags.None, new AsyncCallback(this._SendCallback), s);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }

        private unsafe void _RecvCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket s = state.workSocket;
            

            try
            {
                // Read data from the remote device.
                //IPEndPoint _remoteEp = (IPEndPoint)s.RemoteEndPoint;
                //int bytesRead = s.EndReceive(ar);
                EndPoint rmep = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                int bytesRead = s.EndReceiveFrom(ar, ref rmep);
                IPEndPoint _remoteEp = (IPEndPoint)rmep;

                if (bytesRead > 0 && _remoteEp != null)// && _remoteEp.Address != this._localAddr)
                {
                    RecvSocket rs = null;
                    foreach (IPAddress mip in this._mRecvSocks.Keys)
                    {
                        this._mRecvSocks.TryGetValue(mip, out rs);
                        if (rs._sock == s)
                        {
                            if (rs._state == true)//still in the multicast group
                            {
                                // Deserialize packet and check
                                int i = 0;
                                for (; i < IPMCProtocol.quiltheader.Length; i++)
                                {
                                    if (IPMCProtocol.quiltheader[i] != state.buffer[i])
                                    {
                                        //this packet is not from a quilt node
                                        break;
                                    }
                                }

                                if (i == IPMCProtocol.quiltheader.Length)
                                {
                                    byte[] msg = new byte[StateObject.BufferSize - IPMCProtocol.quiltheader.Length];
                                    Array.Copy(state.buffer, i, msg, 0, msg.Length);
                                    MemoryStream memorystream = new MemoryStream(msg);
                                    Stream _ins;
                                    _ins = memorystream;
                                    byte[] incomingheader = new byte[2 * sizeof(uint) + sizeof(ushort)];

                                    int _ndecrypted = 0;
                                    while (_ndecrypted < incomingheader.Length)
                                    {
                                        int _ndecrypted_now = _ins.Read(incomingheader, _ndecrypted, incomingheader.Length - _ndecrypted);
                                        if (_ndecrypted_now < 0)
                                            throw new Exception("Decrypted less than zero bytes?!");
                                        _ndecrypted += _ndecrypted_now;
                                    }

                                    ushort _incoming_header_classid;
                                    uint _incoming_header_headersize;
                                    uint _incoming_header_messagesize;
                                    fixed (byte* headerptr = incomingheader)
                                    {
                                        _incoming_header_classid = *((ushort*)(headerptr));
                                        _incoming_header_headersize = *((uint*)(headerptr + sizeof(ushort)));
                                        _incoming_header_messagesize = *((uint*)(headerptr + sizeof(uint) + sizeof(ushort)));
                                    }

                                    byte[] _packet = new byte[_incoming_header_messagesize];
                                    _ins.Read(_packet, 0, (int)_incoming_header_messagesize);

                                    //IPMCData ipmc_data = new IPMCData();
                                    QS.Fx.Serialization.ISerializable ipmc_data = QS._core_c_.Base3.Serializer.CreateObject(_incoming_header_classid);
                                    QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(_packet, 0, _incoming_header_headersize);
                                    QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(
                                        _packet, _incoming_header_headersize, _incoming_header_messagesize - _incoming_header_headersize);
                                    try
                                    {
                                        ((QS.Fx.Serialization.ISerializable)ipmc_data).DeserializeFrom(ref header, ref data);
                                    }
                                    catch (Exception _exc)
                                    {
                                        throw new Exception("Could not deserialize the incoming message.", _exc);
                                    }

                                    //call callback
                                    try
                                    {
                                        //((IMulticast)this).ProcessMessage(*mip*, ipmc_data);
                                        this._callback(((IPMCData)ipmc_data)._data, PROTOTYPE.IPMC);
                                    }
                                    catch (Exception _x)
                                    {
                                        throw new Exception("Callback is empty.", _x);
                                    }
                                    //start waiting for other packets
                                    //s.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(this._RecvCallback), state);
                                    EndPoint rmep1 = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                                    s.BeginReceiveFrom(state.buffer, 0, StateObject.BufferSize, 0, ref rmep1, new AsyncCallback(this._RecvCallback), state);
                                }
                                else
                                {
                                    //drop this packet, start waiting for another packet
                                    //s.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(this._RecvCallback), state);
                                    EndPoint rmep1 = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                                    s.BeginReceiveFrom(state.buffer, 0, StateObject.BufferSize, 0, ref rmep1, new AsyncCallback(this._RecvCallback), state);
                                }
                            }
                            else
                            {
                                rs._sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(mip, _localAddr));
                                this._mRecvSocks.Remove(mip);
                            }
                            break;
                        }
                    }
                    if (rs == null)
                    {
                        throw new Exception("socket missed");
                    }
                }
                else
                {
                    EndPoint rmep1 = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                    s.BeginReceiveFrom(state.buffer, 0, StateObject.BufferSize, 0, ref rmep1, new AsyncCallback(this._RecvCallback), state);
                    //s.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.Multicast, new AsyncCallback(this._RecvCallback), state);
                }
            }
            catch (Exception _x)
            {
                throw new Exception("IPMCProtocol._RecvCallback Exception " + _x.Message);
            }
        }

        private void _SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                SendSocket s = (SendSocket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = s._sock.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to the group.", bytesSent);

                if (s._msgs.Count > 0)
                {
                    byte[] packet = new byte[StateObject.BufferSize];
                    IPMCProtocol.quiltheader.CopyTo(packet, 0);
                    s._msgs.Dequeue().CopyTo(packet, IPMCProtocol.quiltheader.Length);

                    //send
                    s._sock.BeginSend(packet, 0, packet.Length, SocketFlags.None, new AsyncCallback(this._SendCallback), s._sock);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }

        }

        #endregion

        #endregion
    }
}
