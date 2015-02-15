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

namespace QS._qss_x_._Machine_1_.Components
{
    public class ServiceController : ServiceControl.IServiceController, IDisposable
    {
        public ServiceController(ServiceControl.IServiceControllerContext context)
        {
            this.context = context;
            context.Logger.Log("ServiceController : Created.");
        }

        private ServiceControl.IServiceControllerContext context;
        private QS.Fx.Clock.IAlarm myalarm;

        private int num;

        #region IServiceController Members

        void QS._qss_x_._Machine_1_.ServiceControl.IServiceController.Start()
        {
            myalarm = context.AlarmClock.Schedule(10, new QS.Fx.Clock.AlarmCallback(this.MyAlarmCallback), null);
        }

        void QS._qss_x_._Machine_1_.ServiceControl.IServiceController.Stop()
        {
            if (myalarm != null && !myalarm.Cancelled)
            {
                myalarm.Cancel();
                myalarm = null;
            }
        }

        #endregion

        #region MyAlarmCallback

        private void MyAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            ServiceControl.IServiceControllerOperation operation = new ServiceControllerOperation(true, "Operation_" + (++num).ToString());

            context.Logger.Log("ServiceController : Submitting \"" + operation.Comment + "\".");

            context.Submit(operation, new QS.Fx.Base.ContextCallback<ServiceControl.IServiceControllerOperation, object>(this.MyOperationCallback), null);
            
            alarm.Reschedule();
        }

        #endregion

        #region MyOperationCallback

        private void MyOperationCallback(ServiceControl.IServiceControllerOperation operation, object operationContext)
        {
            context.Logger.Log("ServiceController : Completed \"" + operation.Comment + "\".");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            context.Logger.Log("ServiceController : Disposed.");
        }

        #endregion
    }
}
