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
/*
	/// <summary>
	/// Summary description for Experiment_007.
	/// </summary>
	public class Experiment_007 : IExperiment
	{
		public Experiment_007()
		{
		}

		public void run(Runtime.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS.CMS.Components.IAttributeSet args, QS.CMS.Components.IAttributeSet results)
        {
			Runtime.INodeRef node1 = environment.Nodes[0];
			Runtime.INodeRef node2 = environment.Nodes[1];

			System.Type testClass = typeof(QS.TMS.Tests.Test107.MainApp);
			using (Runtime.IApplicationRef app1 = node1.launch(testClass.FullName, CMS.Components.AttributeSet.None))
			{
				using (Runtime.IApplicationRef app2 = node2.launch(testClass.FullName, 
					new QS.CMS.Components.AttributeSet("allocationserver", node1.NICs[0].ToString())))
				{
					app2.invoke(testClass.GetMethod("call"), new object[] { new GMS.GroupId(120) });
				}
			}				
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion	
	}
*/ 
}

/*
	System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse("10.20.2.66");

	double t1 = CMS.Base2.PreciseClock.Clock.Time;

	QS.CMS.Base3.NetworkAddress addr;
	for (uint ind = 0; ind < 100000; ind++)
		addr = new QS.CMS.Base3.NetworkAddress(ipAddress, 22505);

	double t2 = CMS.Base2.PreciseClock.Clock.Time;

	logger.Log(((t2-t1)/100000).ToString(".000000000000"));

	double t3 = CMS.Base2.PreciseClock.Clock.Time;

	QS.Fx.Network.NetworkAddress addr2;
	for (uint ind = 0; ind < 100000; ind++)
		addr2 = new QS.Fx.Network.NetworkAddress(ipAddress, 22505);

	double t4 = CMS.Base2.PreciseClock.Clock.Time;

	logger.Log(((t4-t3)/100000).ToString(".000000000000"));
*/
