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
	/// This is a simple test with the real GMS clients and a server on one of the nodes, to catch basic bugs in GMS.
	/// </summary>
	public class Experiment_003 : IExperiment
	{
		public Experiment_003()
		{
		}

		public void run(Runtime.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS.CMS.Components.IAttributeSet arguments, QS.CMS.Components.IAttributeSet results)
        {
			try
			{
				uint numberOfNodes = 5; // environment.Nodes.Length;

				System.Net.IPAddress[] addresses = new System.Net.IPAddress[numberOfNodes];
				apps = new Runtime.IApplicationRef[numberOfNodes];
				for (uint ind = 0; ind < numberOfNodes; ind++)
				{
					CMS.Components.AttributeSet args = new QS.CMS.Components.AttributeSet(2);
					args["base"] = (addresses[ind] = environment.Nodes[ind].NICs[0]).ToString();
					args["gms"] = addresses[0].ToString();

					apps[ind] = environment.Nodes[ind].launch("QS.TMS.Tests.Test101.MainApp", args);
				}

				logger.Log(null, "middle_enter");

				GMS.GroupId groupID = new GMS.GroupId(2000);
				foreach (Runtime.IApplicationRef app in apps)
					app.invoke(typeof(QS.TMS.Tests.Test101.MainApp).GetMethod("join"), new object[] { groupID });

				logger.Log(null, "middle_leave");
			}
			catch (Exception exc)
			{
				logger.Log(this, "Experiment_003.run : " + exc.ToString());
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
