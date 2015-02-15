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

using QS._qss_c_.Virtualization_;

namespace QS._qss_e_.Tests_.Test004
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(QS.Fx.Platform.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.logger = platform.Logger;
			System.Collections.ArrayList nodes = new System.Collections.ArrayList(20);

/*
			foreach (uint node_ind in new uint[] { 1 })
				nodes.Add("cfs" + node_ind.ToString("00") + ".cs.cornell.edu");

			foreach (string nodeName in nodes)
			{
				IPAddress nodeAddress = Dns.Resolve(nodeName).AddressList[0];

				logger.Log(null, "Contacting node : " + nodeName + " (" + nodeAddress.ToString() + ")");

				using (TMS.Runtime.INodeRef nodeRef = new TMS.Runtime.SSHControlledNode(logger, IPAddress.Parse("128.84.223.163"), 12000, 12001,
					nodeAddress, ".mono/bin/mono .work/quicksilver/quicksilver-test.exe", TimeSpan.FromSeconds(10)))
				{
					using (TMS.Runtime.IApplicationRef appRef = nodeRef.launch(
						"QS.TMS.Tests.Test001.MainApp", "-here -base:" + nodeAddress.ToString() + " -sendto:128.84.223.163:13000 -count:1"))
					{
						System.Threading.Thread.Sleep(System.TimeSpan.FromSeconds(1));
					}
				}
			}
*/


/*
			foreach (uint node_ind in new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })
				nodes.Add("node" + node_ind.ToString("00") + ".cs.cornell.edu");
			foreach (uint node_ind in new uint[] { 1, 2, 3, 4, 5 })
				nodes.Add("ogorek" + node_ind.ToString() + ".u.cs.cornell.edu");

			foreach (string nodeName in nodes)
			{
				IPAddress nodeAddress = Dns.Resolve(nodeName).AddressList[0];

				logger.Log(null, "Contacting node : " + nodeName + " (" + nodeAddress.ToString() + ")");

				using (QS.HMS.Service2.ServiceRef serviceClient = new QS.HMS.Service2.ServiceRef(IPAddress.Parse("128.84.223.163"),
					new QS.Fx.Network.NetworkAddress(nodeAddress, (int) HMS.Base.Win32Config.DefaultMainTCPServicePortNo), 
					logger, TimeSpan.FromSeconds(10), HMS.Service2.Service.loadCryptographicKey(HMS.Base.Win32Config.DefaultCryptographicKeyFile)))
				{
					serviceClient.disposeOfBufferedLogContents();
					foreach (HMS.Base.ProcessRef processRef in serviceClient.CurrentProcesses)
						logger.Log(null, "Process : " + processRef.ToString());
					logger.Log(null, serviceClient.launch("C:\\.testing\\applications\\quicksilver-test.exe",
						"QS.TMS.Tests.Test001.MainApp -here -base:" + nodeAddress.ToString() + " -sendto:128.84.223.163:13000 -count:1"));
				}
			}
*/
		}

		private QS.Fx.Logging.ILogger logger;

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
