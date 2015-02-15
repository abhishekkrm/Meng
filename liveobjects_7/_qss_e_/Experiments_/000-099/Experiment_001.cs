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

namespace QS._qss_e_.Experiments_
{
	/// <summary>
	/// This is just about sending a packet between a pair of hosts, it tests basic functionality.
	/// </summary>
	public class Experiment_001 : IExperiment
	{
        private const uint numberOfMessages = 10;

        public Experiment_001()
		{
		}

        private QS.Fx.Logging.ILogger logger;

        public void run(Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS._core_c_.Components.IAttributeSet args, QS._core_c_.Components.IAttributeSet results)
        {
            this.logger = logger;

            Runtime_.INodeRef node1 = environment.Nodes[0];
			Runtime_.INodeRef node2 = environment.Nodes[1];

            System.Type testClass = System.Type.GetType("QS.TMS.Tests.Test001.MainApp");

			QS._core_c_.Components.AttributeSet args1 = new QS._core_c_.Components.AttributeSet();
			args1["base"] = node1.NICs[0].ToString();
			args1["listen"] = "0.0.0.0:1000";
			using (Runtime_.IApplicationRef app1 = node1.launch(testClass.FullName, args1))
			{
                app1.Controller = new Base_1_.ObjectController(this, logger);

                QS._core_c_.Components.AttributeSet args2 = new QS._core_c_.Components.AttributeSet();
				args2["base"] = node2.NICs[0].ToString();
				args2["sendto"] = node1.NICs[0].ToString() + ":1000";
                args2["count"] = numberOfMessages.ToString();
                using (Runtime_.IApplicationRef app2 = node2.launch(testClass.FullName, args2))
				{
                    logger.Log("synchronizing...");
                    app1.invoke(testClass.GetMethod("synchronize"), new object[] { numberOfMessages }, TimeSpan.FromSeconds(3));

                    t_completed.WaitOne();

                    logger.Log("ALL_COMPLETED.");
                }
            }	
		}

        private System.Threading.ManualResetEvent t_completed = new System.Threading.ManualResetEvent(false);
        public void transmission_completed(QS._core_c_.Components.AttributeSet arguments)
        {
            logger.Log(this, "__transmission_completed: " + arguments.ToString());
            t_completed.Set();
        }

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
