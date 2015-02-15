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

namespace QS._qss_x_._Machine_3_.Host
{
    public sealed class ServiceContext : QS._qss_x_._Machine_3_.Base.IServiceContext, IDisposable, QS.Fx.Clock.IAlarmClock
    {
        #region Constructors

        public ServiceContext(QS.Fx.Platform.IPlatform platform)
        {
            this.platform = platform;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Platform.IPlatform platform;

        #endregion

        #region IServiceContext Members

        QS.Fx.Clock.IClock QS._qss_x_._Machine_3_.Base.IServiceContext.Clock
        {
            get { return platform.Clock; }
        }

        QS.Fx.Clock.IAlarmClock QS._qss_x_._Machine_3_.Base.IServiceContext.AlarmClock
        {
            get { return this; }
        }

        #endregion

        #region IAlarmClock Members

        QS.Fx.Clock.IAlarm QS.Fx.Clock.IAlarmClock.Schedule(double timeout, QS.Fx.Clock.AlarmCallback callback, object context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Class Alarm

        private sealed class Alarm : QS.Fx.Clock.IAlarm
        {
            public Alarm()
            {
            }



            #region IAlarm Members

            double QS.Fx.Clock.IAlarm.Time
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            double QS.Fx.Clock.IAlarm.Timeout
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            bool QS.Fx.Clock.IAlarm.Completed
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            bool QS.Fx.Clock.IAlarm.Cancelled
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            object QS.Fx.Clock.IAlarm.Context
            {
                get
                {
                    throw new Exception("The method or operation is not implemented.");
                }
                set
                {
                    throw new Exception("The method or operation is not implemented.");
                }
            }

            void QS.Fx.Clock.IAlarm.Reschedule()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void QS.Fx.Clock.IAlarm.Reschedule(double timeout)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void QS.Fx.Clock.IAlarm.Cancel()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion
    }
}
