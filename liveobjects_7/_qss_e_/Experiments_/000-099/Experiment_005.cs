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
using System.Net;

namespace QS._qss_e_.Experiments_
{
/*
	/// <summary>
	/// A simple test of the basic functionality of a simple view controller.
	/// </summary>
	[TMS.Base.Arguments("-nnodes:99 -count:1000 -window:1 -timeout:1.0")]
	public class Experiment_005 : IExperiment
	{
		public Experiment_005()
		{
		}

		public void run(Runtime.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS.CMS.Components.IAttributeSet arguments, QS.CMS.Components.IAttributeSet result_attributes)
        {
			try
			{
				CMS.Base.Serializer.Get.register(QS.ClassID.Mahesh_CSGMSImmutableView, 
					new CMS.Base.CreateSerializable(GMS.ClientServer.ImmutableView.factory));

				uint numberOfNodes = (uint) environment.Nodes.Length;

				if (arguments.contains("nnodes"))
				{
					uint maxnodes = Convert.ToUInt32(arguments["nnodes"]);
					if (numberOfNodes > maxnodes)
						numberOfNodes = maxnodes;
				}

				logger.Log("Starting experiment on " + numberOfNodes.ToString() + " nodes.");

				System.Net.IPAddress[] addresses = new System.Net.IPAddress[numberOfNodes];
				apps = new Runtime.IApplicationRef[numberOfNodes];
				for (uint ind = 0; ind < numberOfNodes; ind++)
				{
					CMS.Components.AttributeSet args = new QS.CMS.Components.AttributeSet(2);
					args["base"] = (addresses[ind] = environment.Nodes[ind].NICs[0]).ToString();
					args["gms"] = addresses[0].ToString();
					args["allocsrv"] = addresses[0].ToString();
					args["timeout"] = arguments["timeout"];

					apps[ind] = environment.Nodes[ind].launch("QS.TMS.Tests.Test104.MainApp", args);
				}

				logger.Log(null, "middle_enter");

				GMS.GroupId groupID = new GMS.GroupId(2000);
				GMS.ISubView[] subviews = new GMS.ISubView[numberOfNodes];
				for (uint ind = 0; ind < numberOfNodes; ind++)
					subviews[ind] = new GMS.ClientServer.ImmutableSubView(12022, addresses[ind], new uint[] { 1000 } );
				GMS.ClientServer.ImmutableView membershipView = new GMS.ClientServer.ImmutableView(1, subviews);

				apps[0].invoke(typeof(QS.TMS.Tests.Test104.MainApp).GetMethod("initializeExperiment"), 
					new object[] { groupID, CMS.Base2.CompatibilitySOWrapper.Object2ByteArray(membershipView),
					Convert.ToUInt32(arguments["count"]), Convert.ToUInt32(arguments["window"]) });

				logger.Log(null, "middle_leave");

				QS.TMS.Tests.Test104.MainApp.Results results = (QS.TMS.Tests.Test104.MainApp.Results) 
					apps[0].invoke(typeof(QS.TMS.Tests.Test104.MainApp).GetMethod("collectResults"), 
                    new object[] { });


                double[] rtt_delays = new double[results.startingTimes.Length];
                for (uint ind = 0; ind < results.startingTimes.Length; ind++)
                    rtt_delays[ind] = results.finishingTimes[ind] - results.startingTimes[ind];

                result_attributes["acknowledgement times"] = new Data.DataSeries(rtt_delays);
                result_attributes["completion times"] = new Data.DataSeries(results.finishingTimes);

/-*
                double[] multicast_rate = new double[results.finishingTimes.Length - 1];
                for (uint ind = 0; ind < results.finishingTimes.Length - 1; ind++)
                {
                    double interarrival_time = results.finishingTimes[ind + 1] - results.finishingTimes[ind];
                    multicast_rate[ind] = (interarrival_time > 0) ? (1.0 / interarrival_time) : 0;
                }

                result_attributes["multicast_rate"] = multicast_rate;
*-/

                logger.Log(this, "Experiment Completed.\n");
			}
			catch (Exception exc)
			{
				logger.Log(this, "Experiment_005.run : " + exc.ToString());
				throw new Exception("Could not run experiment.", exc);
			}
		}

		private Runtime.IApplicationRef[] apps = null;

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
*/
}
