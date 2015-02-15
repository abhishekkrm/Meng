/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

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

// #define DEBUG_DisableAgent
// #define DEBUG_Logging

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.FailureDetection_.Centralized
{
    public class Agent
    {
        public TimeSpan HeartbeatInterval
        {
            get { return intervalBetweenHeartbeats; }
            set { intervalBetweenHeartbeats = value; }
        }

        public Agent(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, QS._qss_c_.Base3_.ISerializableSender serverSender, QS.Fx.Clock.IAlarmClock alarmClock, 
            TimeSpan intervalBetweenHeartbeats, bool enabled)
        {
            this.logger = logger;
            this.serverSender = serverSender;
            this.alarmClock = alarmClock;
            this.enabled = enabled;
            this.intervalBetweenHeartbeats = intervalBetweenHeartbeats;

#if !DEBUG_DisableAgent
            if (enabled)
                alarm = alarmClock.Schedule(intervalBetweenHeartbeats.TotalSeconds, new QS.Fx.Clock.AlarmCallback(heartbeatCallback), null);

            heartbeatMessage = new Server.HeartbeatMessage(localIID);
            heartbeat();
#endif
        }

        #region Enabling and disabling

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                lock (this)
                {
                    if (value != enabled)
                    {
                        enabled = value;
                        if (enabled)
                        {
                            if (alarm != null)
                                alarm.Reschedule(intervalBetweenHeartbeats.TotalSeconds);
                            else
                                alarm = alarmClock.Schedule(intervalBetweenHeartbeats.TotalSeconds,
                                    new QS.Fx.Clock.AlarmCallback(heartbeatCallback), null);
                        }
                        else
                        {
                            if (alarm != null)
                            {
                                alarm.Cancel();
                                alarm = null;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private QS.Fx.Logging.ILogger logger;
        private QS._qss_c_.Base3_.ISerializableSender serverSender;
        private bool enabled;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IAlarm alarm;
        private TimeSpan intervalBetweenHeartbeats;

#if !DEBUG_DisableAgent
        private Server.HeartbeatMessage heartbeatMessage;

        private void heartbeatCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
#if DEBUG_Logging
            logger.Log(this, "Sending heartbeat to " + serverSender.Address.ToString());
#endif

            heartbeat();

#if DEBUG_Logging
            logger.Log(this, "Rescheduling heartbeat alarm.");
#endif

            alarmRef.Reschedule();

#if DEBUG_Logging
            logger.Log(this, "Heartbeat callback exiting.");
#endif
        }

        private void heartbeat()
        {
#if DEBUG_Logging
            logger.Log(this, "Sending heartbeat to " + serverSender.Address.ToString());
#endif
            serverSender.send((uint)ReservedObjectID.FailureDetection_Centralized_Server, heartbeatMessage);
        }
#endif
    }
}
