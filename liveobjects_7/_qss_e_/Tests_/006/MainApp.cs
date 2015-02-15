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
using System.IO;
using System.Threading;

namespace QS._qss_e_.Tests_.Test006
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		private void processExitedCallback(QS._qss_d_.Base_.ProcessRef processRef)
		{
			logger.Log(null, "process " + processRef.ToString() + " exited");

			lock (this)
			{
				processesToGo--;

				if (processesToGo == 0)
					done.Set();
			}
		}

		private QS.Fx.Logging.ILogger logger;
		private uint processesToGo;
		private AutoResetEvent done = new AutoResetEvent(false);

		public MainApp(QS.Fx.Platform.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.logger = platform.Logger;

			IPAddress localAddress = QS._qss_c_.Devices_2_.Network.AnyAddressOn(new QS._qss_c_.Base1_.Subnet("128.84.0.0:255.255.0.0"));
			QS._qss_d_.Service_2_.IClient serviceClient = new QS._qss_d_.Service_2_.Client(localAddress, null, QS._qss_d_.Base_.Win32Config.DefaultCryptographicKeyFile);

			// string remotePath =  "C:\\.testing\\applications\\quicksilver-test.exe";

			uint[] node_numbers = new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 25, 26, 27, 29, 30, 31, 32 };
			
			uint numberOfNodes = (uint) node_numbers.Length;
			IPAddress[] nodeAddresses = new IPAddress[numberOfNodes];
			for (uint ind = 0; ind < numberOfNodes; ind++)
				nodeAddresses[ind] = Dns.GetHostAddresses("node" + node_numbers[ind].ToString("00") + ".cs.cornell.edu")[0];

			platform.Logger.Log(null, "\nUploading files to " + numberOfNodes.ToString() + " nodes...\n\n");

			processesToGo = 2 * numberOfNodes;

			QS._qss_d_.Base_.ProcessController[] controllers = new QS._qss_d_.Base_.ProcessController[2 * numberOfNodes];
			for (uint ind = 0; ind < numberOfNodes; ind++)
			{
				controllers[2 * ind] = new QS._qss_d_.Base_.ProcessController("xcopy", 
					"/y C:\\krzys\\work\\quicksilver\\test\\bin\\debug\\quicksilver.dll \\\\" + nodeAddresses[ind].ToString() + "\\testing\\applications\\", 
					TimeSpan.FromSeconds(1), new QS._qss_d_.Base_.ProcessExitedCallback(this.processExitedCallback));

				controllers[2 * ind + 1] = new QS._qss_d_.Base_.ProcessController("xcopy", 
					"/y C:\\krzys\\work\\quicksilver\\test\\bin\\debug\\quicksilver-test.exe \\\\" + nodeAddresses[ind].ToString() + "\\testing\\applications\\", 
					TimeSpan.FromSeconds(1), new QS._qss_d_.Base_.ProcessExitedCallback(this.processExitedCallback));
			}

			done.WaitOne();

/*
			using (Deployment.IUploader uploader = new Deployment.ServiceUploader(serviceClient, platform.Logger, TimeSpan.FromSeconds(300)))
			{
				foreach (IPAddress nodeAddress in nodeAddresses)
				{
					uploader.schedule("C:\\krzys\\work\\quicksilver\\test\\bin\\debug\\quicksilver.dll", nodeAddress, "C:\\.testing\\applications\\quicksilver.dll");
					uploader.schedule("C:\\krzys\\work\\quicksilver\\test\\bin\\debug\\quicksilver-test.exe", nodeAddress, remotePath);
				}
			}

			TMS.Runtime.IRemoteNode[] remoteNodes = new TMS.Runtime.IRemoteNode[numberOfNodes];
			foreach (IPAddress nodeAddress in nodeAddresses)
			{
				remoteNodes[ind] = new TMS.Runtime.ServiceControlledNode(
					platform.Logger, localAddress, 12000 + 2 * ind, 12001 + 2 * ind, serviceClient, 
					new QS.Fx.Network.NetworkAddress(nodeAddress, (int) HMS.Base.Win32Config.DefaultMainTCPServicePortNo), 
					TimeSpan.FromSeconds(10), remotePath);					
			}

			platform.Logger.Log(null, "\nStarting the experiment NOW.\n");

			using (TMS.Runtime.DistributedEnvironment environment = new QS.TMS.Runtime.DistributedEnvironment(remoteNodes))
			{
				foreach (TMS.Runtime.INodeRef node in environment.Nodes)
				{
					CMS.Components.AttributeSet launchArgs = new QS.CMS.Components.AttributeSet(2);
					launchArgs["sendto"] = localAddress.ToString() + ":13000";
					launchArgs["count"] = "1";

					using (TMS.Runtime.IApplicationRef applicationRef = node.launch("QS.TMS.Tests.Test001.MainApp", launchArgs))
					{
					}
				}
			}			
*/			
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
