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
	/// Works with test 105.
	/// </summary>
	public class Experiment_006 : IExperiment
	{
		public Experiment_006()
		{
		}

		public void run(Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger, 
            QS._core_c_.Components.IAttributeSet args, QS._core_c_.Components.IAttributeSet results)
        {
			uint numberOfNodes = 2; // (uint) environment.Nodes.Length;

			System.Net.IPAddress[] addresses = new System.Net.IPAddress[numberOfNodes];
			apps = new Runtime_.IApplicationRef[numberOfNodes];
			for (uint ind = 0; ind < numberOfNodes; ind++)
			{
				apps[ind] = environment.Nodes[ind].launch("QS.TMS.Tests.Test105.MainApp", 
					new QS._core_c_.Components.AttributeSet("base", 
					(addresses[ind] = environment.Nodes[ind].NICs[0]).ToString()));
			}

			logger.Log(null, "middle_enter");

			apps[0].invoke(typeof(QS._qss_e_.Tests_.Test105.MainApp).GetMethod("send"), 
				new object[] { addresses[1], "10000", "20" });

			logger.Log(null, "middle_leave");

//			QS.TMS.Tests.Test104.MainApp.Results results = (QS.TMS.Tests.Test104.MainApp.Results) 
//				apps[0].invoke(typeof(QS.TMS.Tests.Test104.MainApp).GetMethod("collectResults"), new object[] { });
//
//			logger.Log(this, "\nEXPERIMENT RESULTS:\n\n" + results.ToString() + "\n");
		}

		private Runtime_.IApplicationRef[] apps = null;

		#region IDisposable Members

		public void Dispose()
		{
			if (apps != null)
			{
				for (uint ind = 0; ind < apps.Length; ind++)
					apps[ind].Dispose();
			}
		}

		#endregion
	}
}
