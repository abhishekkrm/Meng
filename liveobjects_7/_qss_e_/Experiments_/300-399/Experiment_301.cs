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
    public class Experiment_301 : Experiment_300
    {
        public Experiment_301()
        {
        }

        // this method is invoked on the coordinating node to initiate the experiment
        protected override void Start()
        {
            // use this to log messages
            Log("wait before everything stabilizes");

            // use this to sleep
            Sleep(TimeSpan.FromSeconds(5));

            // we have "NumberOfApplications" apps runnning on different nodes
            for (int appno = 0; appno < NumberOfApplications; appno++)
            {
                // invoke method "Welcome" on app running on node number "appno", with argument list including the application number
                Invoke(
                    appno, // node number
                    "Method1", // method name
                    new object[] // arguments as a list of objects
                    { 
                        appno 
                    }
                );
            }

            // call just the first guy
            Invoke(0, "Method2", new object[] {});
        }
        // after this method completes, the experiment doesn't end until some application calls "Completed()" in its body

        // this method is called to finish the experiment
        protected override void Stop()
        {
            Log("done");
        }

        protected override Type ApplicationClass
        {
            // returns the type of the application class, just leave it as it is and implement the class pasted below
            get { return typeof(Application); }
        }

        // this is a class implementing application that is to be run on each node
        protected new class Application : Experiment_300.Application
        {
            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                // type here some code to initialize the application
                Log("initializing");
            }

            // some method we declared that will be called by the experiment code
            public void Method1(int num)
            {
                Log("Number: " + num.ToString());
            }

            // some other method
            public void Method2()
            {
                // we used thread pool to start some work in another thread
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.Method3), null);
            }

            // method run in another thread
            private void Method3(object o)
            {
                Log("working hard...");
                System.Threading.Thread.Sleep(5000);
                Log("done");
                
                // when we call this, the experiment will complete
                // it is enough to call it only once in one of the applications to finish the experiment, it is not necessary to all it in all of them
                Completed();
            }
        }
    }
}
