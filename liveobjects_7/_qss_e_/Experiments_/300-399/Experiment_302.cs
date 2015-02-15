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
    public class Experiment_302 : Experiment_300
    {
        public Experiment_302()
        {
        }

        protected override void Start()
        {
            Log("wait 3s");
            Sleep(TimeSpan.FromSeconds(3));

            Log("start");
            for (int appno = 0; appno < NumberOfApplications; appno++)
                Invoke(appno, "ExecuteProcess", new object[] { (string) arguments["executable"], (string) arguments["parameters"] });
        }

        protected override void Stop()
        {
            Log("done, wait 3s");
            Sleep(TimeSpan.FromSeconds(3));
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        protected new class Application : Experiment_300.Application
        {
            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
            }

            private QS._qss_d_.Base_.ProcessController processController;

            public void ExecuteProcess(string executable, string parameters)
            {
                processController = 
                    new QS._qss_d_.Base_.ProcessController(
                        executable, 
                        parameters, 
                        TimeSpan.FromSeconds(1),
                        new QS._qss_d_.Base_.ProcessExitedCallback(this.ProcessFinished));

                Log("launched pid " + processController.Ref.PID.ToString());
            }

            private void ProcessFinished(QS._qss_d_.Base_.ProcessRef processRef)
            {
                Log("finished pid " + processRef.PID.ToString());
                Completed();
            }
        }
    }
}
