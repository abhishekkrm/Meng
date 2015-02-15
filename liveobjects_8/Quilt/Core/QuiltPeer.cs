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
//line 479: publishdate?
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

namespace Quilt.Core
{
    [QS.Fx.Reflection.ComponentClass("FAF41AF0C5CD42c39A78FB0AC4A34654", "Quilt Peer")]
    public sealed class QuiltPeer :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>,
        IMember<Name, Incarnation, Name, EUIDAddress>
    {
        #region Constructor

        public QuiltPeer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("EUIDTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>>
                _transport_object_reference,
            [QS.Fx.Reflection.Parameter("Quilt Peer Identification, default by using STUN address", QS.Fx.Reflection.ParameterClass.Value)]
                string _self_id,
            [QS.Fx.Reflection.Parameter("Quilt Peer Name,default by Anonymous", QS.Fx.Reflection.ParameterClass.Value)]
                string _self_name,
            [QS.Fx.Reflection.Parameter("Bootstrap Serer EUID String", QS.Fx.Reflection.ParameterClass.Value)]
                string _bootstrap_server_euid,
            [QS.Fx.Reflection.Parameter("Bool for source fox exp", QS.Fx.Reflection.ParameterClass.Value)]
                bool _is_source,
            [QS.Fx.Reflection.Parameter("Source message size in bytes for exp", QS.Fx.Reflection.ParameterClass.Value)]
                int _msg_size,
            [QS.Fx.Reflection.Parameter("Throughput in kbps for exp", QS.Fx.Reflection.ParameterClass.Value)]
                int _throughput,
            [QS.Fx.Reflection.Parameter("Traffic start delay in sec", QS.Fx.Reflection.ParameterClass.Value)]
                int _start,
            [QS.Fx.Reflection.Parameter("Traffic last time in sec", QS.Fx.Reflection.ParameterClass.Value)]
                int _duration
            )
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer.Constructor");
#endif
            // Set Quilt Peer id, incarnation, name
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

            this._transmitter = new Quilt.Transmitter.Transmitter(_mycontext, _transport_object_reference, _message_hander);

            // Set callback
            this._recved_callback = new ReceivedData(this.ReceivedDataCallback);

            // Set shared state
            this._share_state = new ShareState();
            // It is set inside the MessageHandler function

            if (_is_source)
            {
                _isSource = true;
                this._msg_size = _msg_size;
                this._throughput = _throughput;
                this._duration = _duration;
                this._start = _start;
            }
            else
            {
                _isSource = false;
            }
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

        // Multicast container
        private Dictionary<string, IMulticast> _multicast_roles = new Dictionary<string,IMulticast>();
        private Dictionary<string, QS.Fx.Clock.IAlarm> _multicast_alarms = new Dictionary<string,QS.Fx.Clock.IAlarm>();

        // Scheduler for alive report
        private QS.Fx.Clock.IAlarm _alive_alarm;
        private int _alive_interval = 5 * 1000; // 5 sec

        // Data buffer
        private ShareState _share_state;

#if VERBOSE

        private int _msg_size;
        private string _msg;
        private int _throughput;
        private double _serial_no = 0;
        private bool _isSource;
        private int _start;
        private int _duration;
        private QS.Fx.Clock.IAlarm _start_alarm;
        private QS.Fx.Clock.IAlarm _test_alarm;

#endif

        #endregion

        #region Class ShareState

        public class ShareState
        {
            public DataBuffer _data_buffer = new DataBuffer(1000, 30 * 1000); //30 sec
            public bool _isDelegate = false;
            public EUIDAddress _self_euid;
            public Name _self_id;
            public BootstrapMember _self;
            public StreamWriter _log;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Transmitter Initialized

        private void TransmitterInited()
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer.TransmitterInited Transmitter has been set up!");
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


#if VERBOSE

            if (_isSource)
            {
                _start_alarm = this._platform.AlarmClock.Schedule
                    (
                        _start, //delay to send traffic
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                if ((_start_alarm != null) && !_start_alarm.Cancelled && ReferenceEquals(_start_alarm, _alarm))
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this.Start));
                            }
                        ),
                        null
                    );
            }
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Bootstrap Client Endpoit Connect and DisConnect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer._Connect Bootstrap has been connected!");
#endif
            // Start test bootstrapping
            JoinGroup("Test Group");
        }

        private void _DisConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer._Connect Bootstrap has been disconnected!");
#endif

        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MessageHandler

        private void MessageHandler(EUIDAddress remote_euid, ISerializable message)
        {
            if (message == null)
            {
                // Set the SharedState
                _share_state._self_euid = remote_euid;
                if (_self_id == null)
                {
                    _self_id = new Name(_share_state._self_euid.GetProtocolInfo("UDP").proto_addr);
                    //_self_id = new Name(System.Net.Dns.GetHostName());
                }
                _share_state._self_id = _self_id;
                _share_state._self = new BootstrapMember(this);
                _share_state._log  = new StreamWriter("c:\\" + System.Net.Dns.GetHostName() + "_" + _self_id.String.Split('|', ':', '/')[3] + ".log");

                // Transmitter has been initalized
                TransmitterInited();
            }
            else
            {
                // Process Messages
                double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                //lock (_share_state._log)
                //{
                //    _share_state._log.WriteLine(now + "\t" + "recv\t" + message.SerializableInfo.ClassID + "\t" + message.SerializableInfo.Size);
                //    _share_state._log.Flush();
                //}

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Quilt.Core.QuiltPeer.Message received " + message.SerializableInfo.ClassID);
#endif

                // Process Bootstrap Messages
                if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.BootstrapMax &&
                    message.SerializableInfo.ClassID > (ushort)QS.ClassID.BootstrapMin)
                {
                    _bootstrap.ProcessBootstrapMsg(remote_euid, message);
                }

                if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.IpmcMax &&
                    message.SerializableInfo.ClassID > (ushort)QS.ClassID.IpmcMin)
                {
                    _multicast_roles[PROTOTYPE.IPMC.ToString()].ProcessMessage(remote_euid, message);
                }

                if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.DonetMax &&
                    message.SerializableInfo.ClassID > (ushort)QS.ClassID.DonetMin)
                {
                    _multicast_roles[PROTOTYPE.DONET.ToString()].ProcessMessage(remote_euid, message);
                }

                if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.OmniMax &&
                   message.SerializableInfo.ClassID > (ushort)QS.ClassID.OmniMin)
                {
                    _multicast_roles[PROTOTYPE.OMNI.ToString()].ProcessMessage(remote_euid, message);
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
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer.IBootstrapClient.BootstrapMember received patch " + bootstrap_membership.PatchInfo._patch_description.String);
#endif
            PatchInfo info = bootstrap_membership.PatchInfo;
            string proto_str = info._patch_proto.ToString();
            try
            {
                IMulticast role;
                if (!_multicast_roles.TryGetValue(proto_str, out role))
                {
                    switch (info._patch_proto)
                    {
                        case PROTOTYPE.IPMC:
                            role = new IPMCProtocol();
                            break;
                        case PROTOTYPE.OMNI:
                            role = new OMNIProtocol();
                            break;
                        case PROTOTYPE.DONET:
                            role = new DONetProtocol();
                            break;
                        default:
                            throw new Exception("No support protocol");
                    }
                    role.SetShareState(this._share_state);
                    role.SetCallback(this._recved_callback);
                    _multicast_roles.Add(proto_str, role);
                    QS.Fx.Clock.IAlarm role_alarm = this._platform.AlarmClock.Schedule
                    (
                        role.GetScheduleInterval() / 1000, // sec level
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string>(this.Schedule, proto_str));
                            }
                        ),
                        null
                    );
                    _multicast_alarms.Add(proto_str, role_alarm);
                }

                // Join the patch
                role.Join(info, bootstrap_membership.Members, _transmitter);

                // Set delegate
                if (_multicast_roles.Count > 1)
                {
                    _share_state._isDelegate = true;
                }

                // Set alive report alarm
                if (this._alive_alarm == null)
                {
                    this._alive_alarm = this._platform.AlarmClock.Schedule
                        (
                            _alive_interval/1000,
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
            catch (Exception exc)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Quilt.Core.QuiltPeer.IBootstrapClient.BootstrapMember " + exc.Message);
#endif
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
                list.Add(_share_state._self_euid);
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
                        throw new Exception("Quilt.Core.QuiltPeer Id initialization has not finished yet");
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

        #region Schedule

        public void Schedule(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            try
            {
                string proto_str = ((QS._qss_x_.Properties_.Base_.IEvent_<string>)_event)._Object;

                IMulticast role;
                if (!_multicast_roles.TryGetValue(proto_str, out role))
                {
                    throw new Exception(proto_str + " role has been lost");
                }
                role.Schedule();

                QS.Fx.Clock.IAlarm alarm;
                if (!_multicast_alarms.TryGetValue(proto_str, out alarm))
                {
                    throw new Exception(proto_str + " alarm has been lost");
                }
                alarm.Reschedule();
            }
            catch (Exception exc)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Quilt.Core.QuiltPeer.Schedule " + exc.Message);
#endif
            }
        }

        #endregion

        #region Alive

        public void Alive(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            try
            {
                // Send alive message to the server
                BootstrapAlive alive_msg = new BootstrapAlive(new Name("Test Group"), new BootstrapMember(this));
                _transmitter.SendMessage(this._bootstrap_server_euid, alive_msg);

                this._alive_alarm.Reschedule();
            }
            catch (Exception exc)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Quilt.Core.QuiltPeer.Schedule " + exc.Message);
#endif
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Source related stuff

        public void PublishData(ISerializable _data_chunk)
        {
            if (!_isSource) return;

            ++_serial_no;

            // Put inside the data buffer
            _share_state._data_buffer.PushData(_serial_no, _data_chunk);
            DataBuffer.Data local_data = _share_state._data_buffer.GetData(_serial_no);

            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _share_state._log.WriteLine(now + "\t" + "pub\tdataseq\t" + _serial_no);
            _share_state._log.Flush();

            if (local_data == null)
            {
                throw new Exception("QuiltPeer.PublishData exception: cannot get the newly created data in DataBuffer");
            }

            // Publish to all the multicast protocols
            foreach(KeyValuePair<string, IMulticast> kvp in _multicast_roles)
            {
                IMulticast role = kvp.Value;
                
                role.PublishData(local_data);
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer.PublishData: published " + _serial_no);
#endif
        }

        public void JoinGroup(string _group_name)
        {
            // Call the boostrap to join a new group
            this._bootstrap_endpt.Interface.Join(_group_name, _share_state._self);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Receiver related stuff

        private ReceivedData _recved_callback;

        public void ReceivedDataCallback(DataBuffer.Data data, PROTOTYPE proto)
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //if (_isSource) return;

            if (_share_state._data_buffer.HasReceived(data._serial_no))
            {
                lock (_share_state._log)
                {
                    _share_state._log.WriteLine(now + "\t" + "recv\tdataseq\t" + data._serial_no + "\tdup");
                    _share_state._log.Flush();
                }
            }
            else
            {
                lock (_share_state._log)
                {
                    _share_state._log.WriteLine(now + "\t" + "recv\rdataseq\t" + data._serial_no + "\tnew");
                    _share_state._log.Flush();
                }

                _share_state._data_buffer.StoreData(data);

                if (_share_state._isDelegate)
                {
                    foreach (KeyValuePair<string, IMulticast> kvp in _multicast_roles)
                    {
                        if (proto.ToString() == kvp.Key) continue;
                        kvp.Value.PublishData(data);
                    }
                }
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Core.QuiltPeer.RceivedDataCallback: received " + data._serial_no);
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


#if VERBOSE

        #region Start

        public void Start(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            // This function has to be called inside core thread
            _test_alarm = this._platform.AlarmClock.Schedule
                    (
                        0.01,//(_msg_size * 8)/(_throughput * 1000),
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                if ((_test_alarm != null) && !_test_alarm.Cancelled && ReferenceEquals(_test_alarm, _alarm))
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this.Test));
                            }
                        ),
                        null
                    );

            char[] _msg_buf = new char[_msg_size];
            for (int i = 0; i < _msg_size; i++)
            {
                _msg_buf[i] = 'Q';
            }
            _msg = new string(_msg_buf);
        }

        #endregion

        #region Test

        public void Test(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            //string now = (DateTime.Now.Ticks / TimeSpan.TicksPerSecond).ToString();

            if (_serial_no * 0.01 > _duration)
            //if ((_serial_no * _msg_size * 8) / (_throughput * 1000) > _duration)
            {
                lock (_share_state._log)
                {
                    _share_state._log.WriteLine("End");
                    _share_state._log.Flush();
                    //_share_state._log.Close();
                }
                return;
            }

            for (int i = 0; i < 1; i++)
            {
                try
                {
                    PublishData(new UnicodeText(_msg));
                    
                }
                catch (Exception exc)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Quilt.Core.QuiltPeer.Test " + exc.Message);
#endif
                }
            }

            _test_alarm.Reschedule();
        }

        #endregion

#endif
    }
}
