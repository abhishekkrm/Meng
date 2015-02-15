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

// #define DEBUG_SSHControlledNode

using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace QS._qss_e_.Runtime_
{
	/// <summary>
	/// Summary description for SSHControlledNode.
	/// </summary>
	public class SSHControlledNode : GenericRemoteNode
	{
		public SSHControlledNode(QS._core_c_.Base.IReadableLogger logger, IPAddress localAddress, uint consolePortNo, uint controllerPortNo, 
			IPAddress nodeAddress, string remotePathToAppLauncher, TimeSpan defaultTimeoutOnRemoteOperations)
			: base(logger, localAddress, consolePortNo, controllerPortNo, nodeAddress, defaultTimeoutOnRemoteOperations)
		{
			this.remotePathToAppLauncher = remotePathToAppLauncher;
		}

		protected override void startupRemoteAgent()
		{
			process = new Process();

			process.StartInfo.FileName = "ssh";
			process.StartInfo.Arguments = "-n " + nodeAddress.ToString() + " " + remotePathToAppLauncher + " " + 
				typeof(QS._qss_e_.Runtime_.RemoteAgent).ToString() + 
				" -here" + " -base:" + nodeAddress.ToString() + " -logfile:log.txt" +
				" -sendlog:" + localAddress.ToString() + ":" + consolePortNo.ToString() +
				" -rsync:" + localAddress.ToString() + ":" + controllerPortNo.ToString();
			
			bool redirecting = true;

			process.StartInfo.UseShellExecute = !redirecting;
			process.StartInfo.CreateNoWindow = process.StartInfo.RedirectStandardOutput = 
				process.StartInfo.RedirectStandardError = redirecting;

			logger.Log(null, "Starting process... " + process.StartInfo.Arguments);

			process.EnableRaisingEvents = true;
			process.Exited += new EventHandler(thisProcess_Exited);
			process.Start();

		}

		private string remotePathToAppLauncher;
		private Process process;

		private void thisProcess_Exited(object sender, EventArgs e)
		{
			try
			{
				this.shutdown();
			}
			catch (Exception exc)
			{
				logger.Log(null, "Exited Unexpectedly : " + exc.ToString());
			}
		}

		protected override void shutdownRemoteAgent()
		{
			if (process != null)
			{
				// logger.Log(null, process.StandardError.ReadToEnd());
				// logger.Log(null, process.StandardOutput.ReadToEnd());

				Monitor.Exit(this);

				process.WaitForExit(5000);

				Monitor.Enter(this);

				if (process != null && !process.HasExited)
				{
					logger.Log(null, "forcifully killing this process");
					process.Kill();
				}
				else
					logger.Log(null, "process terminated by itself");

				process = null;
			}
		}
	}
}
