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

namespace QS._qss_e_.Runtime_
{
	/// <summary>
	/// Summary description for Win32ServiceControlledNode.
	/// </summary>
	public class ServiceControlledNode : GenericRemoteNode
	{
        public ServiceControlledNode(QS._core_c_.Base.IReadableLogger logger, IPAddress localAddress, uint consolePortNo, uint controllerPortNo,
            QS._qss_d_.Service_2_.IClient serviceClient, QS.Fx.Network.NetworkAddress serviceAddress, TimeSpan defaultTimeoutOnRemoteOperations,
            string remotePathToAppLauncher)
            : this(logger, localAddress, consolePortNo, controllerPortNo, serviceClient, serviceAddress, 
                defaultTimeoutOnRemoteOperations, remotePathToAppLauncher, null)
        {
        }

		public ServiceControlledNode(QS._core_c_.Base.IReadableLogger logger, IPAddress localAddress, uint consolePortNo, uint controllerPortNo, 
			QS._qss_d_.Service_2_.IClient serviceClient, QS.Fx.Network.NetworkAddress serviceAddress, TimeSpan defaultTimeoutOnRemoteOperations,
			string remotePathToAppLauncher, string identifyingName)
			: base(logger, localAddress, consolePortNo, controllerPortNo, serviceAddress.HostIPAddress, defaultTimeoutOnRemoteOperations)
		{
			this.remotePathToAppLauncher = remotePathToAppLauncher;
			this.serviceClient = serviceClient;
			this.serviceAddress = serviceAddress;
            this.identifyingName = identifyingName;
		}

		private QS._qss_d_.Service_2_.IClient serviceClient;
		private QS.Fx.Network.NetworkAddress serviceAddress;
		private QS._qss_d_.Service_2_.IServiceRef serviceRef;
		private QS._qss_d_.Service_2_.IProcessRef processRef;
		private string remotePathToAppLauncher;
        private string identifyingName;

		protected override void startupRemoteAgent()
		{
			serviceRef = serviceClient.connectTo(serviceAddress, logger, defaultTimeoutOnRemoteOperations);

			processRef = serviceRef.launch(remotePathToAppLauncher, typeof(QS._qss_e_.Runtime_.RemoteAgent).ToString() + 
				" -here" + " -base:" + nodeAddress.ToString() + 
                " -logfile:\"" + // System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName 
                remotePathToAppLauncher + ((identifyingName != null) ? ("." + identifyingName) : "") + ".errorlog\"" + 
                " -sendlog:" + localAddress.ToString() + ":" + consolePortNo.ToString() + 
                " -copylog" +
				" -rsync:" + localAddress.ToString() + ":" + controllerPortNo.ToString(), 
				new QS._qss_d_.Service_2_.ProcessAbortedCallback(this.processAbortedCallback));
		}

		private void processAbortedCallback(QS._qss_d_.Service_2_.IProcessRef processRef)
		{
			logger.Log(this, "Process " + processRef.ToString() + " aborted unexpectedly.");
		}
 
		protected override void shutdownRemoteAgent()
		{
            try
            {
                processRef.Dispose();
            }
            catch (Exception)
            {
            }

            try
            {
                serviceRef.Dispose();
            }
            catch (Exception)
            {
            }
		}
	}
}
