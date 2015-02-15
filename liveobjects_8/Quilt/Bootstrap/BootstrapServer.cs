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
using QS.Fx.Serialization;

using Quilt.Transmitter;
using Quilt.MulticastRules;
using Quilt.Multicast;

namespace Quilt.Bootstrap
{
    [QS.Fx.Reflection.ComponentClass("21244D6977BE46c09FC206C8F9CA4027", "Quilt Bootstrap Server")]
    public sealed class BootstrapServer
        : QS._qss_x_.Properties_.Component_.Base_
    {
        #region Constructor

        public BootstrapServer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Experiment mode, default 0, ipmc 1, donet 2, omni 3, gradient 4", QS.Fx.Reflection.ParameterClass.Value)] 
                int _mode,
            [QS.Fx.Reflection.Parameter("EUIDTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>>
                _transport_object_reference)
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Bootstrap.BootstrapServer.Constructor");
#endif

            this._mycontext = _mycontext;
            this._transport_object_reference = _transport_object_reference;
            this._message_hander = MessageHandler;

            // Initialize the rules
            _multicast_rules = new Dictionary<string, IRule>();

            switch (_mode)
            {
                case 0:
                    {
                        _multicast_rules.Add(PROTOTYPE.IPMC.ToString(), new IPMCRule());
                        _multicast_rules.Add(PROTOTYPE.DONET.ToString(), new DONetRule());
                        _multicast_rules.Add(PROTOTYPE.OMNI.ToString(), new OMNIRule());
                        _inter_proto = PROTOTYPE.OMNI;
                    }
                    break;
                case 1:
                    {
                        _multicast_rules.Add(PROTOTYPE.IPMC.ToString(), new IPMCRule());
                        _inter_proto = PROTOTYPE.IPMC;
                    }
                    break;
                case 2:
                    {
                        _multicast_rules.Add(PROTOTYPE.DONET.ToString(), new DONetRule());
                        _inter_proto = PROTOTYPE.DONET;
                    }
                    break;
                case 3:
                    {
                        _multicast_rules.Add(PROTOTYPE.OMNI.ToString(), new OMNIRule());
                        _inter_proto = PROTOTYPE.OMNI;
                    }
                    break;
                case 4:
                    {
                        //Gradient, no rule
                        _is_multigroup = true;
                        _heuristic_type = 1;
#if VERBOSE
                        // Test before real running with clients
                        // IGroupManager mgr = new MultiGroupManager(_transmitter, _logger, _heuristic_type);
                        //((MultiGroupManager)mgr).TestHeuristic();
#endif

                    }
                    break;
                case 5:
                    {
                        _is_multigroup = true;
                        _heuristic_type = 2;
                    }
                    break;
                case 6:
                    {
                        _is_multigroup = true;
                        _heuristic_type = 3;
                    }
                    break;
                default:
                    break;
            }
           
            this._transmitter = new Quilt.Transmitter.Transmitter(_mycontext, _transport_object_reference, _message_hander);

            this._group_mgr_interval = 5000; // 5000 msec
        }

        #endregion

        #region Field

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>> _transport_object_reference;
        private Transmitter.Transmitter _transmitter;
        private Transmitter.Transmitter.UpperMessageHandler _message_hander;

        private EUIDAddress _self_euid;

        // Group manager related
        private Dictionary<string, IGroupManager> _group_managers = new Dictionary<string, IGroupManager>();
        private double _group_mgr_interval;
        private Dictionary<string, QS.Fx.Clock.IAlarm> _group_alarms = new Dictionary<string, QS.Fx.Clock.IAlarm>();
        private bool _is_multigroup = false;
        private int _heuristic_type;

        // Multicast rules [prototype : rule]
        private Dictionary<string, IRule> _multicast_rules;
        private PROTOTYPE _inter_proto;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Transmitter Initialized

        private void TransmitterInited()
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Bootstrap.BootstrapServer.TransmitterInited Transmitter has been set up!");
#endif

            // Create the Bootstrap Module for bootstrapping
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MessageHandler

        private void MessageHandler(EUIDAddress remote_euid, ISerializable message)
        {
            if (message == null)
            {
                // Transmitter has been initalized
                TransmitterInited();
                _self_euid = remote_euid;
            }
            else
            {
                // Process Messages

                // Process Bootstrap Messages
                if (message.SerializableInfo.ClassID < (ushort)QS.ClassID.BootstrapMax &&
                    message.SerializableInfo.ClassID > (ushort)QS.ClassID.BootstrapMin)
                {
                    ProcessBootstrapMsg(remote_euid, message);
                }
            }
        }

        #endregion

        #region ProcessBootstrapMsg

        private void ProcessBootstrapMsg(EUIDAddress _remote_euid, ISerializable _message)
        {
            switch (_message.SerializableInfo.ClassID)
            {
                case (ushort)QS.ClassID.BootstrapJoin:
                    {
                        IGroupManager mgr;
                        Name group_name = ((BootstrapJoin)_message).GroupName;
                        if (!_group_managers.TryGetValue(group_name.String, out mgr))
                        {
                            if (!_is_multigroup)
                            {
                                mgr = new GroupManager(group_name, _transmitter, _inter_proto, ref _multicast_rules, this._logger);
                            }
                            else
                            {
                                if (_group_managers.Count == 0)
                                {
                                    mgr = new MultiGroupManager(_transmitter, _logger, _heuristic_type);
                                }
                                else
                                {
                                    //Use the same reference
                                    mgr = _group_managers.First().Value;
                                }
                            }

                            _group_managers.Add(group_name.String, mgr);

                            if (!_is_multigroup)
                            {
                                QS.Fx.Clock.IAlarm group_alarm = this._platform.AlarmClock.Schedule
                                (
                                    this._group_mgr_interval / 1000, // sec level
                                    new QS.Fx.Clock.AlarmCallback
                                    (
                                        delegate(QS.Fx.Clock.IAlarm _alarm)
                                        {
                                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string>(this.Callback_Maintain, group_name.String));
                                        }
                                    ),
                                    null
                                );
                                _group_alarms.Add(group_name.String, group_alarm);
                            }
                            else
                            {
                                if (!_group_alarms.ContainsKey("background"))
                                {
                                    QS.Fx.Clock.IAlarm group_alarm = this._platform.AlarmClock.Schedule
                                (
                                    this._group_mgr_interval / 1000, // sec level
                                    new QS.Fx.Clock.AlarmCallback
                                    (
                                        delegate(QS.Fx.Clock.IAlarm _alarm)
                                        {
                                            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<string>(this.Callback_Maintain, group_name.String));
                                        }
                                    ),
                                    null
                                );
                                    _group_alarms.Add("background", group_alarm);
                                }
                            }
                        }

                        mgr.ProcessJoin(_remote_euid, (BootstrapJoin)_message);
                    }
                    break;
                case (ushort)QS.ClassID.BootstrapAlive:
                    {
                        IGroupManager mgr;
                        Name group_name = ((BootstrapAlive)_message).GroupName;
                        if (!_group_managers.TryGetValue(group_name.String, out mgr))
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Quilt.Bootstrap.BootstrapServer.ProcessBootstrapMsg MemberAlive for unknown group: " + group_name.String);
#endif
                            return;
                        }
                        mgr.ProcessAlive(_remote_euid, (BootstrapAlive)_message);
                    }
                    break;
                default:
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Quilt.Bootstrap.BootstrapServer.ProcessBootstrapMsg Unknown message type!");
#endif
                    return;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Maintain

        public void Callback_Maintain(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<string> _event_ = (QS._qss_x_.Properties_.Base_.IEvent_<string>)_event;
            string group_name = _event_._Object;

            IGroupManager mgr;
            if (_group_managers.TryGetValue(group_name, out mgr))
            {
                mgr.Callback_Maintain();
            }

            _group_alarms[group_name].Reschedule();
        }

        #endregion
    }
}
