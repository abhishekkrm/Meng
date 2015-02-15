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

using QS._qss_c_.Base1_;
using QS._qss_e_.Base_1_;
using QS._qss_e_.Runtime_;
using QS._qss_e_.Environments_;
using QS._qss_e_.Experiments_;
using QS._qss_c_.Helpers_;
using QS._qss_c_.Components_1_;

namespace QS._qss_e_.Simulations_
{
    public class Simulation : System.IDisposable
    {
        public Simulation(QS._core_c_.Base.Logger logger, int nnodes, double lossrate, double latency, int queuesize, Type experimentClass)
            : this(logger, nnodes, lossrate, latency, queuesize, experimentClass, (QS._core_c_.Components.AttributeSet)null)
        {
        }

        public Simulation(QS._core_c_.Base.Logger logger, int nnodes, double lossrate, double latency, int queuesize, Type experimentClass, 
            string arguments)
            : this(logger, nnodes, lossrate, latency, queuesize, experimentClass, (arguments == null) ? null : new QS._core_c_.Components.AttributeSet(arguments))
        {
        }

        public Simulation(QS._core_c_.Base.Logger logger, int nnodes, double lossrate, double latency, int queuesize, Type experimentClass,
            QS._core_c_.Components.AttributeSet arguments)
        {
            if (arguments == null)
                arguments = new QS._core_c_.Components.AttributeSet(
                    ((experimentClass.GetCustomAttributes(typeof(ArgumentsAttribute), false)[0]) as ArgumentsAttribute).Arguments);
            this.logger = logger;
            this.arguments = arguments;
            results = new QS._core_c_.Components.AttributeSet();
            environment = new SimulatedEnvironment(logger, 1, (uint)nnodes, lossrate, latency, queuesize);
            experiment = (IExperiment) experimentClass.GetConstructor(Type.EmptyTypes).Invoke(NoObject.Array);

            experimentThread = new Thread(new ThreadStart(experimentLoop));
        }

        private QS._core_c_.Base.Logger logger;
        private QS._core_c_.Components.AttributeSet arguments, results;
        private SimulatedEnvironment environment;
        private IExperiment experiment;
        private Thread experimentThread;
        private Exception exceptionThrown;
        private ManualResetEvent completed = new ManualResetEvent(false);

        public void Start()
        {
            experimentThread.Start();
            environment.Start();
        }

        public QS._core_c_.Components.AttributeSet Results
        {
            get 
            {
                if (exceptionThrown == null)
                    return results;
                else
                    throw new Exception("Simulation failed.", exceptionThrown);
            }
        }

        public WaitHandle Completed
        {
            get { return completed; }
        }

        private void experimentLoop()
        {
            try
            {
                experiment.run(environment, logger, arguments, results);
                exceptionThrown = null;
            }
            catch (Exception exc)
            {
                exceptionThrown = exc;
            }

            while (environment.EventsScheduled > 0)
                environment.Completed.WaitOne(TimeSpan.FromMilliseconds(100), false);
            environment.Stop();
            completed.Set();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            completed.WaitOne();
            if (!experimentThread.Join(TimeSpan.FromSeconds(3)))
            {
                experimentThread.Abort();
                experimentThread.Join(TimeSpan.FromSeconds(2));
            }
            experiment.Dispose();
            environment.Dispose();
        }

        #endregion
    }
}
