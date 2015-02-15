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
using System.Threading;
using System.Net;

namespace QS._qss_e_.Experiments_
{
    public abstract class Experiment_300 : Experiment_200
    {
        public Experiment_300()
        {
        }

        protected abstract void Start();
        protected abstract void Stop();

        protected QS._core_c_.Components.IAttributeSet Results;
        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            this.Results = results;
            
            Start();
            completed.WaitOne();
            Stop();
        }

        private ManualResetEvent completed = new ManualResetEvent(false);

        public void SetCompleted(QS._core_c_.Components.AttributeSet arguments)
        {
            completed.Set();
        }

        protected void Invoke(int appno, string methodName, object[] arguments)
        {
            ApplicationOf(appno).invoke(this.ApplicationClass.GetMethod(methodName), arguments);
        }

        protected new class Application : Experiment_200.Application
        {
            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
            }

            protected void Log(string s)
            {
                logger.Log(null, s);
            }

            protected void Completed()
            {
                applicationController.upcall("SetCompleted", QS._core_c_.Components.AttributeSet.None);
            }

            public override void TerminateApplication(bool smoothly)
            {
                platform.ReleaseResources();
            }

            public override void Dispose()
            {
            }
        }
    }
}
