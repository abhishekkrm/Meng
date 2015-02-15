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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Incubator_
{
    public sealed class Node : QS.Fx.Inspection.Inspectable, QS._qss_x_.Agents_.Base.IContainerContext, IClientContext
    {
        internal Node(Incubator incubator, int nodeno, string name)
        {
            this.incubator = incubator;
            this.nodeno = nodeno;
            this.name = name;
            this.logger = new QS._qss_c_.Base3_.Logger(incubator.simulatedclock, true, null);
            this.container = new QS._qss_x_.Agents_.Base.Container(this);
            isup = true;

            _InitializeInspection();

            alarm = ((QS.Fx.Clock.IAlarmClock) incubator.simulatedclock).Schedule(incubator.mttf * (- Math.Log(incubator.random.NextDouble())),
                new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), null);
        }

        internal Incubator incubator;

        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Base3_.Logger logger;
        [QS.Fx.Base.Inspectable]
        internal int nodeno;
        [QS.Fx.Base.Inspectable]
        internal string name;
        [QS.Fx.Base.Inspectable]
        internal Domain localdomain;
        [QS.Fx.Base.Inspectable]
        internal Agent localagent;
        [QS.Fx.Base.Inspectable]
        internal uint maxincarnation;
        [QS.Fx.Base.Inspectable]
        internal List<Domain> remotedomains = new List<Domain>();
        [QS.Fx.Base.Inspectable]
        internal List<Agent> remoteagents = new List<Agent>();
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Agents_.Base.Container container;
        [QS.Fx.Base.Inspectable]
        internal bool isup;
        [QS.Fx.Base.Inspectable]
        internal QS.Fx.Clock.IAlarm alarm;
        
        internal IDictionary<string, Client> clients = new Dictionary<string, Client>();

        #region Inspection

        [QS.Fx.Base.Inspectable("_clients")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, Client> __inspectable_clients;

        private void _InitializeInspection()
        {
            __inspectable_clients =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, Client>("_clients", clients,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, Client>.ConversionCallback(
                        delegate(string s) { return s; }));
        }

        #endregion

        #region IContainerContext Members

        QS.Fx.Logging.ILogger QS._qss_x_.Agents_.Base.IContainerContext.Logger
        {
            get { return logger; }
        }

        QS.Fx.Clock.IClock QS._qss_x_.Agents_.Base.IContainerContext.Clock
        {
            get { return incubator.simulatedclock; }
        }

        QS.Fx.Clock.IAlarmClock QS._qss_x_.Agents_.Base.IContainerContext.AlarmClock
        {
            get { return incubator.simulatedclock; }
        }

        #endregion

        #region IClientContext Members

        QS.Fx.Logging.ILogger IClientContext.Logger
        {
            get { return logger; }
        }

        QS.Fx.Clock.IClock IClientContext.Clock
        {
            get { return incubator.simulatedclock; }
        }

        QS.Fx.Clock.IAlarmClock IClientContext.AlarmClock
        {
            get { return incubator.simulatedclock; }
        }

        #endregion

        #region _AlarmCallback

        private void _AlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (isup)
                {
                    isup = false;
                    alarm.Reschedule(incubator.mttr * (- Math.Log(incubator.random.NextDouble())));
                }
                else
                {
                    isup = true;
                    alarm.Reschedule(incubator.mttf * (- Math.Log(incubator.random.NextDouble())));
                }

                incubator.Reconfigure(this);
            }
        }

        #endregion
    }
}
