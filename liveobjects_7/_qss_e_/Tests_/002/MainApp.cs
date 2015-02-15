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
using System.Text;

using QS._qss_c_.Virtualization_;

namespace QS._qss_e_.Tests_.Test002
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(QS.Fx.Platform.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.logger = platform.Logger;
			this.serviceClient = new QS._qss_d_.Service_2_.ServiceRef(IPAddress.Parse((string) args["base"]), new QS.Fx.Network.NetworkAddress((string) args["node"]), 
				logger, TimeSpan.FromSeconds(5), QS._qss_d_.Service_2_.ServiceHelper.LoadCryptographicKey((string) args["key"]));

			processRef = serviceClient.launch((string) args["path"], (string) args["parameters"], 
				new QS._qss_d_.Service_2_.ProcessAbortedCallback(this.processAbortedCallback));

			QS._qss_d_.Base_.ProcessRef[] processes = serviceClient.CurrentProcesses;
			logger.Log(null, "There are " + processes.Length + " processes");
			foreach (QS._qss_d_.Base_.ProcessRef pref in processes)
				logger.Log(null, "Process : " + pref.ToString());
		}

		private void processAbortedCallback(QS._qss_d_.Service_2_.IProcessRef processRef)
		{
			logger.Log(this, "Process Aborted: " + processRef.ToString() + " with output:\n" + processRef.CompleteOutput);
		}

		private QS.Fx.Logging.ILogger logger;
		private QS._qss_d_.Service_2_.ServiceRef serviceClient;
		private QS._qss_d_.Service_2_.IProcessRef processRef;

		#region IDisposable Members

		public void Dispose()
		{
			logger.Log(this, "Process Output:\n" + processRef.CompleteOutput);

			processRef.Dispose();
			serviceClient.Dispose();
		}

		#endregion
	}
}
