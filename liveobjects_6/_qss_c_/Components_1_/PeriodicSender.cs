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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Components_1_
{
    public class PeriodicSender : IDisposable
    {
        public PeriodicSender(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IAlarmClock alarmClock, TimeSpan interval, QS._qss_c_.Base3_.ISerializableSender sender,
            uint destinationLOID, QS._qss_x_.Serialization_.CreateSerializableObjectCallback createCallback)
        {
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.interval = interval;
            this.sender = sender;
            this.destinationLOID = destinationLOID;
            this.createCallback = createCallback;

            alarmRef = alarmClock.Schedule(interval.TotalSeconds, new QS.Fx.Clock.AlarmCallback(this.sendingCallback), null);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private TimeSpan interval;
        private QS._qss_c_.Base3_.ISerializableSender sender;
        private uint destinationLOID;
        private QS._qss_x_.Serialization_.CreateSerializableObjectCallback createCallback;
        private QS.Fx.Clock.IAlarm alarmRef;

        private void sendingCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            sender.send(destinationLOID, this.createCallback());
            alarmRef.Reschedule();
        }

        #region IDisposable Members

        public void Dispose()
        {
            alarmRef.Cancel();
        }

        #endregion
    }
}
