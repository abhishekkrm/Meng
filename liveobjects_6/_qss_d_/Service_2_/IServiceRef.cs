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

namespace QS._qss_d_.Service_2_
{
	public interface IClient
	{
		QS.Fx.Network.NetworkAddress[] scan(TimeSpan timeoutOnScanning);
		IServiceRef connectTo(QS.Fx.Network.NetworkAddress serviceAddress, QS.Fx.Logging.ILogger logger, TimeSpan timeoutOnRequests);
	}

	/// <summary>
	/// Summary description for IClient.
	/// </summary>
	public interface IServiceRef : System.IDisposable
	{
		string CompleteLog
		{
			get;
		}

		void disposeOfBufferedLogContents();

		string launch(string executablePath, string parameters);
		string launch(string executablePath, string parameters, TimeSpan timeout);
		IProcessRef launch(string executablePath, string parameters, ProcessAbortedCallback processAbortedCallback);

		Base_.ProcessRef[] CurrentProcesses
		{
			get;
		}

		Base_.ProcessController.Status statusOf(Base_.ProcessRef processRef);

		void upload(byte[] fileAsBytes, string remotePath);
		void upload(string localPath, string remotePath);

		string deploy(DeploymentRequest[] deploymentRequests, IPAddress[] destinations, System.TimeSpan timeout);

		string shutdownAllControlledProcesses();
		string killOSProcess(string processName);

		string restartServices(string serviceName, string[] nodeNames, bool shouldStop, bool shouldStart, double timeout);
        string restartServices(string serviceName, string[] nodeNames, bool shouldStop, bool shouldStart, double timeout,
            string impersonatedUserName, string password, string domainName);

        QS._qss_e_.Repository_.IRepositoryClient RepositoryClient
        {
            get;
        }
    }

    [System.Serializable]
	public struct DeploymentRequest
	{
//        public DeploymentRequest()
//        {
//        }

        public DeploymentRequest(string sourcePath, string targetPath)
		{
			this.sourcePath = sourcePath;
			this.targetPath = targetPath;
		}

		public string sourcePath, targetPath;
	}

	public delegate void ProcessAbortedCallback(IProcessRef processRef);

	public interface IProcessRef : IDisposable
	{
		Base_.ProcessRef ID
		{
			get;
		}

		string CompleteOutput
		{
			get;
		}
	}
}

/*
			EXAMPLE OF USAGE
			----------------------------------------------------------------------------------------------			

			using (HMS.Service2.Client client = new QS.HMS.Service2.Client(
				IPAddress.Parse((string) args["base"]), new QS.Fx.Network.NetworkAddress((string) args["node"]), logger, TimeSpan.FromSeconds(5), 
				HMS.Service2.Service.loadCryptographicKey((string) args["key"])))
			{
				logger.Log(null, "RESULT : " + client.launch((string) args["path"], (string) args["parameters"], TimeSpan.FromSeconds(5)));

				HMS.Base.ProcessRef[] processes = client.CurrentProcesses;
				logger.Log(null, "There are " + processes.Length + " processes");
				foreach (HMS.Base.ProcessRef processRef in processes)
					logger.Log(null, "Process : " + processRef.ToString());
			}
*/
