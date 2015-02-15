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
using System.IO;

using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS.Fx.Base;
using QS.Fx.Serialization;

using Quilt.Bootstrap;
using Quilt.Transmitter;
using Quilt.Multicast;
using Quilt.PubsubApp;

namespace Quilt.Core
{
    [QS.Fx.Reflection.ComponentClass("C174CA92624349ff9307794B2E2BB3B5", "Gradient Peer")]
    public sealed class GradientPeer :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>,
        IMember<Name, Incarnation, Name, EUIDAddress>,
        QS.Fx.Object.Classes.IPubSub,
        QS.Fx.Interface.Classes.IPubSubOps
    {
        #region Constructor

        public GradientPeer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("EUIDTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>>
                _transport_object_reference,
            [QS.Fx.Reflection.Parameter("Peer Identification, default by using STUN address", QS.Fx.Reflection.ParameterClass.Value)]
                string _self_id,
            [QS.Fx.Reflection.Parameter("Peer Name,default by Anonymous", QS.Fx.Reflection.ParameterClass.Value)]
                string _self_name,
            [QS.Fx.Reflection.Parameter("Bootstrap Serer EUID String", QS.Fx.Reflection.ParameterClass.Value)]
                string _bootstrap_server_euid
            )
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.GradientPeer.Constructor");
#endif
            // Set Peer id, incarnation, name
            if (_self_id != null && _self_id != "")
            {
                this._self_id = new Name(_self_id);
            }
            this._self_incarnation = new Incarnation(1);
            if (_self_name != "")
            {
                this._self_name = new Name(_self_name);
            }
            else
            {
                this._self_name = new Name("Anonymous");
            }

            this._mycontext = _mycontext;
            this._transport_object_reference = _transport_object_reference;
            this._message_hander = new Quilt.Transmitter.Transmitter.UpperMessageHandler(MessageHandler);
            this._bootstrap_server_euid = new EUIDAddress(_bootstrap_server_euid);

            this._pubsub_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IPubSubClient,
                QS.Fx.Interface.Classes.IPubSubOps>(this);

            this._successors = new Dictionary<string, Dictionary<string, double>>();
            this._subscriptions = new Dictionary<string, double>();
            this._predeccessors = new Dictionary<string, string>();

            this._data_filter = new DataFilter();

            this._transmitter = new Quilt.Transmitter.Transmitter(_mycontext, _transport_object_reference, _message_hander);
        }

        #endregion

        #region Delegate

        public delegate void ReceivedData(DataBuffer.Data data, PROTOTYPE proto);

        #endregion

        #region Field

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>> _transport_object_reference;

        private Name _self_id;
        private Name _self_name;
        private Incarnation _self_incarnation;
        private bool _isSource = false;
        private DataFilter _data_filter;
        private int RETRY = 1;

        // Transmitter
        private Transmitter.Transmitter _transmitter;
        private Transmitter.Transmitter.UpperMessageHandler _message_hander;

        // Bootstrap
        private EUIDAddress _bootstrap_server_euid;
        private Bootstrap.Bootstrap _bootstrap;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>,
            QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>> _bootstrap_endpt;
        private QS.Fx.Endpoint.IConnection _bootstrap_conn;

        // Scheduler for alive report
        private QS.Fx.Clock.IAlarm _alive_alarm;
        private int _alive_interval = 1 * 1000; // 1 sec    
    
        // Share state
        public QuiltPeer.ShareState _share_state;

        #endregion

        #region Field for PubSub

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IPubSubClient,
            QS.Fx.Interface.Classes.IPubSubOps> _pubsub_endpt;

        private Dictionary<string, double> _subscriptions;
        private Dictionary<string, Dictionary<string, double>> _successors;
        private Dictionary<string, string> _predeccessors;

        private int _seq_no = 1;

        private Dictionary<string, EUIDAddress> _node_map = new Dictionary<string,EUIDAddress>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Transmitter Initialized

        private void TransmitterInited()
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.GradientPeer.TransmitterInited Transmitter has been set up!");
#endif

            // Create the Bootstrap Module for bootstrapping
            QS.Fx.Object.Classes.IBootstrap<BootstrapMember, BootstrapMembership>
                    bootstrap2 = new Bootstrap.Bootstrap(_mycontext, _transmitter, _bootstrap_server_euid);

            this._bootstrap = (Bootstrap.Bootstrap)bootstrap2;

            _bootstrap_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>,
                QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>>(this);

            _bootstrap_endpt.OnConnected += new QS.Fx.Base.Callback(
                delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
            _bootstrap_endpt.OnDisconnect += new QS.Fx.Base.Callback(
                delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._DisConnect))); });

            _bootstrap_conn = _bootstrap_endpt.Connect(bootstrap2.Bootstrap);

        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Bootstrap Client Endpoit Connect and DisConnect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.GradientPeer._Connect Bootstrap has been connected!");
#endif            
            // Automatically join a background group
            JoinGroup("background");

            // Notify upper application to join groups
            if (this._pubsub_endpt.IsConnected)
            {
                this._pubsub_endpt.Interface.Ready();
            }
        }

        private void _DisConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.GradientPeer._Connect Bootstrap has been disconnected!");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        public void JoinGroup(string _group_name)
        {
            // Call the boostrap to join a new group
            //this._bootstrap_endpt.Interface.Join(_group_name, _share_state._self);

#if VERBOSE

            double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //Time, Seq_num, Group, Src, Srcrate, Target, Targrate, Type
            _share_state._log.WriteLine(cur + "\tNULL\t" + _group_name + "\t" + _self_id + "\t1\t" + "bootstrap" + "\t" + "null" + "\tJOIN");
            _share_state._log.Flush();

            // Send a BootstrapJoin message to bootstrap server
            BootstrapJoin join_msg = new BootstrapJoin(new Name(_group_name), _share_state._self);
            for (int i = 0; i < RETRY; i++)
            {
                _transmitter.SendMessage(this._bootstrap._bootstrap_server, join_msg);
                //System.Threading.Thread.Sleep(1000);
            }
#endif
        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MessageHandler

        private void MessageHandler(EUIDAddress remote_euid, ISerializable message)
        {
            if (message == null)
            {
                // Set the SharedState
                _share_state = new QuiltPeer.ShareState();
                _share_state._self_euid = remote_euid;
                if (_self_id == null)
                {
                    //_self_id = new Name(_share_state._self_euid.GetProtocolInfo("UDP").proto_addr);
                    string id = System.Net.Dns.GetHostName();

                    StreamReader map = new StreamReader("c:/map.txt");
                    string line;
                    Dictionary<string, string> map_dict = new Dictionary<string, string>();
                    while (null != (line = map.ReadLine()))
                    {
                        char[] set = { ' ' };
                        string[] elems = line.Split(set, StringSplitOptions.RemoveEmptyEntries);

                        map_dict.Add(elems[3], elems[0]);
                    }
                    map.Close();

                    try
                    {
                        _self_id = new Name(map_dict[id]);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Quilt.PubsubApp.PubSubClient.Constructor " + exc);
                    }
                }
                _share_state._self_id = _self_id;
                _share_state._self = new BootstrapMember(this);
                _share_state._log = new StreamWriter("c:\\" + _self_id + "_gradient_peer.log");

#if VERBOSE
                // Debug code for logging transmitter message flow
                _transmitter.SetLogTarget(_share_state._log);
#endif

                // Transmitter has been initalized
                TransmitterInited();
            }
            else
            {
                // Process Messages
                double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

#if VERBOSE
                //if (this._logger != null)
                    //this._logger.Log("Quilt.Core.GradientPeer.Message received " + message.SerializableInfo.ClassID);
#endif

                // Process Bootstrap Messages
                if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.BootstrapMax &&
                    message.SerializableInfo.ClassID > (ushort)QS.ClassID.BootstrapMin)
                {
                    _bootstrap.ProcessBootstrapMsg(remote_euid, message);
                }
                else if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.GradientMax &&
                    message.SerializableInfo.ClassID > (ushort)QS.ClassID.GradientMin)
                {
                    ProcessGradientMessages(remote_euid, message);
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IBootstrapClient<BootstrapMember,PatchInfo> Members

        void QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>.BootstrapMembership(BootstrapMembership bootstrap_membership)
        {
            // Initialize Patch
#if VERBOSE
            //if (this._logger != null)
                //this._logger.Log("Quilt.Core.GradientPeer.IBootstrapClient.BootstrapMember received successors for stream " + bootstrap_membership.GroupName.String);
#endif
            PatchInfo info = bootstrap_membership.PatchInfo;
            string proto_str = info._patch_proto.ToString();

            //int total_streams = int.Parse(info._patch_description.String.Split('|')[2]);

            // Set successors for each stream group
            Dictionary<string, double> stream_succesors;
            if (!_successors.TryGetValue(bootstrap_membership.GroupName.String, out stream_succesors))
            {
                stream_succesors = new Dictionary<string, double>();
                _successors.Add(bootstrap_membership.GroupName.String, stream_succesors);
            }

            string to_log = "";
            string[] rate_strings = info._patch_description.String.Split('|')[4].Split(',');
            int index_rates = 0;            
            foreach (BootstrapMember succ in bootstrap_membership.Members)
            {
                string id = ((IMember<Name, Incarnation, Name>)succ).Identifier.String;
                EUIDAddress addr = ((IMember<Name, Incarnation, Name, EUIDAddress>)succ).Addresses.First();
                double rate = double.Parse(rate_strings[index_rates++]);
                if (!stream_succesors.ContainsKey(id))
                {
                    stream_succesors.Add(id, rate);
                }

                if (!_node_map.ContainsKey(id))
                {
                    _node_map.Add(id, addr);
                }

                to_log += id + ":" + rate + ",";  
            }

            to_log.TrimEnd(',');

#if VERBOSE
            double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _share_state._log.WriteLine(cur + "\t" + bootstrap_membership.GroupName.String + "\t" + to_log + "\tTREE");
            _share_state._log.Flush();
#endif

            // Check whether the source has setup all streams
            if (_isSource)
            {
                // if (total_streams == _successors.Count)
                // All successors have been setup
                {
                    // Start publish data in this group
                    _pubsub_endpt.Interface.Subscribed(bootstrap_membership.GroupName.String, true);
                }
            }

            // Set alive report alarm
            if (this._alive_alarm == null)
            {
                this._alive_alarm = this._platform.AlarmClock.Schedule
                    (
                        _alive_interval / 1000,
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this.Alive));
                            }
                        ),
                        null
                    );
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IMember<Name,Incarnation,Name,EUIDAddress> Members

        bool IMember<Name, Incarnation, Name, EUIDAddress>.Operational
        {
            get { return true; }
        }

        IEnumerable<EUIDAddress> IMember<Name, Incarnation, Name, EUIDAddress>.Addresses
        {
            get
            {
                List<EUIDAddress> list = new List<EUIDAddress>();
                if (_share_state != null && _share_state._self_euid != null)
                {
                    list.Add(_share_state._self_euid);
                }
                return list;
            }
        }

        #endregion

        #region IMember<Name,Incarnation,Name> Members

        Name IMember<Name, Incarnation, Name>.Identifier
        {
            get
            {
                try
                {
                    if (_self_id == null)
                        throw new Exception("Quilt.Core.GradientPeer Id initialization has not finished yet");
                }
                catch
                {
                    // Do nothing
                }
                return _self_id;
            }
        }

        Incarnation IMember<Name, Incarnation, Name>.Incarnation
        {
            get { return _self_incarnation; }
        }

        Name IMember<Name, Incarnation, Name>.Name
        {
            get { return _self_name; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Alive

        public void Alive(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            try
            {
                // Send alive message to the server
                BootstrapAlive alive_msg = new BootstrapAlive(new Name("background"), new BootstrapMember(this));
                _transmitter.SendMessage(this._bootstrap_server_euid, alive_msg);

                this._alive_alarm.Reschedule();
            }
            catch (Exception exc)
            {
#if VERBOSE
                //if (this._logger != null)
                    //this._logger.Log("Quilt.Core.GradientPeer.Schedule " + exc.Message);
#endif
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Process Gradient Messages

        private void ProcessGradientMessages(EUIDAddress remote_euid, ISerializable gradient_msg)
        {
            //GC.Collect();
            if (gradient_msg.SerializableInfo.ClassID == (ushort)QS.ClassID.GradientData)
            {
                GradientData gradient_data = gradient_msg as GradientData;
                PubSubData pubsub_data = gradient_data._data._data as PubSubData;
                double rate = (double)pubsub_data._rate / 100;

                // Check if it is for subscribed streams
                string stream_id = gradient_data._stream.String;

#if VERBOSE
                //if (this._logger != null)
                    //this._logger.Log("Quilt.Core.GradientPeer.ProcessMessages: received stream" + stream_id);
                {
                    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    _share_state._log.WriteLine(cur + "\t" + gradient_data._data._serial_no + "\t" + stream_id + "\t" + _self_id + "\t" + rate + "\tapp\t" + ((ISerializable)gradient_data).SerializableInfo.Size + "\tCOME");
                    _share_state._log.Flush();
                }
#endif

                if (_subscriptions.ContainsKey(stream_id))
                {
                    // Notify Upper Application
                    _pubsub_endpt.Interface.Data(stream_id, gradient_data._data._data);

#if VERBOSE
                    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    //Time, Seq_num, Group, Src, Srcrate, Target, Targrate, Type
                    _share_state._log.WriteLine(cur + "\t" + gradient_data._data._serial_no + "\t" + stream_id + "\t" + _self_id + "\t" + rate + "\tapp\t" + _subscriptions[stream_id] + "\t" + ((ISerializable)gradient_data).SerializableInfo.Size + "\tRECV");
                    _share_state._log.Flush();
#endif
                }

                // Forward to Successors
                if (!_successors.ContainsKey(stream_id))
                {
#if VERBOSE
                    //double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    //Time, Seq_num, Group, Src, Srcrate, Target, Targrate, Type
                    //_share_state._log.WriteLine(stream_id + "\t" + _self_id + "\t" + rate + "\tapp\t" + _subscriptions[stream_id] + "\tno seccessor!");
                    //_share_state._log.Flush();
#endif
                    // No successors for this stream, return
                    return;
                }

                foreach (KeyValuePair<string, double> succesor in _successors[stream_id])
                {
                    // Shrink if needed
                    //PubSubData shrinked_data = (succesor.Value > rate) ? null : this._data_filter.shrink(stream_id, (int)(succesor.Value * 100));

                    PubSubData shrinked_data = this._data_filter.shrink(stream_id, (int)(succesor.Value * 100));

                    if (shrinked_data != null)
                    {
                        // Update data
                        gradient_data._data._data = shrinked_data;
                        _transmitter.SendMessage(_node_map[succesor.Key], gradient_data);

#if VERBOSE
                        double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                        //Time, Group, Src, Srcrate, Target, Targrate, Type
                        _share_state._log.WriteLine(cur + "\t" + gradient_data._data._serial_no + '\t' + stream_id + "\t" + _self_id + "\t" + rate + "\t" + succesor.Key + "\t" + succesor.Value + "\t" + ((ISerializable)gradient_data).SerializableInfo.Size + "\tFORW");
                        _share_state._log.Flush();

                        //if (this._logger != null)
                            //this._logger.Log("Quilt.Core.GradientPeer.ProcessMessages: forward stream" + stream_id);
#endif
                    }
                    else
                    {
#if VERBOSE
                        double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                        //Time, Group, Src, Srcrate, Target, Targrate, Type
                        _share_state._log.WriteLine(cur + "\t" + stream_id + "\t" + _self_id + "\t" + rate + "\t" + succesor.Key + "\t" + succesor.Value + "\tERROR");
                        _share_state._log.Flush();

                        if (this._logger != null)
                            this._logger.Log("Quilt.Core.GradientPeer.ProcessMessages: forward error" + stream_id);
#endif
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        
        #region IPubSub Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IPubSubClient, QS.Fx.Interface.Classes.IPubSubOps> QS.Fx.Object.Classes.IPubSub.PubSub
        {
            get { return this._pubsub_endpt; }
        }

        #endregion

        #region IPubSubOps Members

        void QS.Fx.Interface.Classes.IPubSubOps.Subscribe(string group_id, double quality, bool is_publisher)
        {
            // Set if source
            _isSource = is_publisher;

            _subscriptions.Add(group_id, quality / 100);

            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<string>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.Join), group_id));
           
        }

        private void Join(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<string> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<string>)_event;

            string group_id = _event_._Object;

            // Contact bootstrap server
            JoinGroup(group_id);

        }

        void QS.Fx.Interface.Classes.IPubSubOps.UnSubscribe(string group_id)
        {
            // Call the boostrap to join a new group
            this._bootstrap_endpt.Interface.Leave(group_id, _share_state._self_id.String);
        }

        void QS.Fx.Interface.Classes.IPubSubOps.Publish(string group_id, ISerializable data)
        {
            // Source check
            if (!_isSource) return;

            // Push to successors;
            DataBuffer.Data seqdata = new DataBuffer.Data();           
            seqdata._serial_no = _seq_no++;
            //seqdata._timeout = 
            GradientData gradient_data = new GradientData(seqdata, new Name(group_id));

#if INFO
            if (this._logger != null)
            {
                this._logger.Log("Quilt.Gradient.Publish: Publish data " + seqdata._serial_no + " in group " + group_id);
            }
#endif

            foreach (KeyValuePair<string, double> succesor in _successors[group_id])
            {
                PubSubData shrinked_data = (succesor.Value > 1) ? null : this._data_filter.shrink(group_id, (int)(succesor.Value * 100));

                if (shrinked_data != null)
                {
                    gradient_data._data._data = shrinked_data;
#if VERBOSE
                    //if (this._logger != null)
                    //{
                        //this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " GradientPeer.Publish begin");
                    //}
#endif

                    _transmitter.SendMessage(_node_map[succesor.Key], gradient_data);

#if VERBOSE
                    //if (this._logger != null)
                    //{
                        //this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " GradientPeer.Publish end");
                    //}
#endif

#if VERBOSE
                    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    //Time, Seq_num,  Group, Src, Target, Rate, Type
                    _share_state._log.WriteLine(cur + "\t" + seqdata._serial_no + "\t" + group_id + "\t" + _self_id + "\t" + succesor.Key + "\t" + succesor.Value + "\t" + ((ISerializable)gradient_data).SerializableInfo.Size + "\tPUB");
                    _share_state._log.Flush();

#endif
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                    {
                        this._logger.Log("GradientPeer.Publish Shrinked data null group " + group_id + " " + succesor.Value);
                    }
#endif
                }
            }            
            
        }

        #endregion
    }
}
