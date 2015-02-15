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

namespace QS._qss_x_._Machine_1_.Applications.Test
{
    public class CounterService : Service.Service<ICounter, Counter, IncrementCounter, ICounter, Counter, IncrementCounter>
    {
        public CounterService()
        {
            Context.Logger.Log("CounterService.Constructor"); 
        }

        protected override void Initialize(bool reset)
        {
            Context.Logger.Log("CounterService.Initialize");
            Context.AlarmClock.Schedule(10, new QS.Fx.Clock.AlarmCallback(this.AlarmCallback), null);            
        }

        protected override void Cleanup()
        {
            Context.Logger.Log("CounterService.Cleanup");
        }

        private void AlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            Context.Logger.Log("CounterService.AlarmCallback");
            Context.NonpersistentStateController.Submit(new IncrementCounter(1),
                new QS._qss_x_._Machine_1_.Service.OperationCallback<IncrementCounter>(this.OperationCallback), null);
            Context.PersistentStateController.Submit(new IncrementCounter(1),
                new QS._qss_x_._Machine_1_.Service.OperationCallback<IncrementCounter>(this.OperationCallback), null);
        }

        private void OperationCallback(IncrementCounter operation, object callbackContext)
        {
            Context.Logger.Log("CounterService.OperationCallback");
            Context.Logger.Log("Count : Persistent = " + Context.PersistentStateController.State.Count.ToString() + 
                ", Nonpersistent = " + Context.NonpersistentStateController.State.Count.ToString());
        }
    }
}
