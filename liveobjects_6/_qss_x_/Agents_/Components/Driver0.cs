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

namespace QS._qss_x_.Agents_.Components
{
    /// <summary>
    /// This driver implements a singleton.
    /// </summary>
    public sealed class Driver0 : QS._qss_x_.Agents_.Base.IDriver
    {
        #region Constructor

        public Driver0()
        {
        }

        #endregion

        #region Fields

        private QS._qss_x_.Agents_.Base.IDriverContext context;
        private uint configuration_incarnation;
        private QS.Fx.Clock.IAlarm myalarm;

        #endregion

        #region IDriver Members

        void QS._qss_x_.Agents_.Base.IDriver.Initialize(QS._qss_x_.Agents_.Base.IDriverContext context)
        {
            lock (this)
            {
                this.context = context;
                configuration_incarnation = context.Configuration.Incarnation;

                if (context.Configuration.Members.Length != 1)
                    throw new Exception("The singleton driver support only singleton configurations.");

                myalarm = context.AlarmClock.Schedule(((double) 1.0) / context.Rate, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), null);
            }
        }

        void QS._qss_x_.Agents_.Base.IDriver.Receive(
            uint configuration_incarnation, uint sender_member_index, QS.Fx.Serialization.ISerializable message)
        {
            throw new Exception("The singleton driver does not support communication with other agents.");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (myalarm != null)
                {
                    if (!myalarm.Cancelled)
                        myalarm.Cancel();
                    myalarm = null;
                }
            }
        }

        #endregion

        #region _AlarmCallback

        private void _AlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (!alarm.Cancelled)
                {
                    _ProcessToken();
                    alarm.Reschedule();
                }
            }
        }

        #endregion

        #region _ProcessToken

        private void _ProcessToken()
        {
            uint round = context.Round + 1;

            QS.Fx.Serialization.ISerializable toaggregate;
            context.Aggregate(round, out toaggregate);
            context.Aggregate(round, new QS.Fx.Serialization.ISerializable[] { toaggregate });

            QS.Fx.Serialization.ISerializable[] todisseminate;
            context.Disseminate(round, 1, out todisseminate);
            if (todisseminate.Length != 1)
                throw new Exception("Wrong number of dissemination records.");
            context.Disseminate(round, todisseminate[0]);
        }

        #endregion
    }
}
