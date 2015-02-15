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

// #define DEBUG_DoNotRestartServices
// #define DEBUG_DoNotUploadFiles

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_e_.Launchers_
{
	public class RunExperiment : Base_1_.ControlledApplication
	{
        private static readonly TimeSpan DefaultTimeoutOnRemoteOperations = TimeSpan.FromSeconds(60);

        public RunExperiment(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.constructorArgs = args;
			platform.Logger.Log(this, "RunExperiment Started, Arguments: " + args.ToString());

			this.platform = platform;

            this.experimentPath = (string)args["experimentPath"];

            bool restartWorkers = (bool)(args["restartWorkers"]);
            bool uploadFiles = (bool)(args["uploadFiles"]);

			string[] workerAddresses_AsStringArray = (string[])args["workerAddresses"];

            StringBuilder s = new StringBuilder("Worker addresses:\n");
            foreach (string x in workerAddresses_AsStringArray)
                s.AppendLine("Address : { " + x + " }");
            platform.Logger.Log(this, s.ToString());

			workerAddresses = new System.Net.IPAddress[workerAddresses_AsStringArray.Length];

            List<System.Net.IPAddress> nonlocal_workerAddresses = new List<System.Net.IPAddress>();
            for (int ind = 0; ind < workerAddresses_AsStringArray.Length; ind++)
            {
                try
                {
                    workerAddresses[ind] = System.Net.Dns.GetHostAddresses(workerAddresses_AsStringArray[ind])[0];
                }
                catch (Exception exc)
                {
                    throw new Exception("Cannot resolve hostname \"" + workerAddresses_AsStringArray[ind] + "\".", exc);
                }
                
                bool nonlocal = true;
                foreach (System.Net.IPAddress local_addr in platform.NICs)
                {
                    if (local_addr.Equals(workerAddresses[ind]))
                    {
                        nonlocal = false;
                        break;
                    }
                }

                if (nonlocal)
                    nonlocal_workerAddresses.Add(workerAddresses[ind]);
            }

            nonlocal_workerAddresses_AsArray = nonlocal_workerAddresses.ToArray(); 

#if !DEBUG_DoNotRestartServices
            if (restartWorkers)
            {
                try
                {
                    QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                        "QuickSilver_HostAdministrator", nonlocal_workerAddresses_AsArray,
                        platform.Logger, true, true, 10,
                        (string)args["username"], (string)args["domain"], (string)args["password"]);
                }
                catch (Exception exc)
                {
                    platform.Logger.Log(this,
                        "\nCouldn't restart services on the worker nodes... may not be able to run the experiment.\n" + exc.ToString() + "\n");
                }
            }
            else
                platform.Logger.Log(this, "Not restarting services");
#endif

			localAddress = System.Net.IPAddress.Parse((string) args["localAddress"]);

			serviceClient = new QS._qss_d_.Service_2_.Client(localAddress, null, QS._qss_d_.Base_.Win32Config.DefaultCryptographicKeyFile);

			string deploymentPath = (string) args["deploymentPath"];
			string[] deploymentFiles = (string[]) args["deploymentFiles"];
			executablePath = (string)args["executablePath"];

#if !DEBUG_DoNotUploadFiles
            if (uploadFiles)
            {
                using (QS._qss_e_.Deployment_.IUploader uploader = new QS._qss_e_.Deployment_.ServiceUploader(serviceClient, platform.Logger, TimeSpan.FromSeconds(10)))
                {
                    foreach (string filename in deploymentFiles)
                    {
                        string path = deploymentPath + filename;
                        foreach (System.Net.IPAddress destination in nonlocal_workerAddresses_AsArray)
                            uploader.schedule(path, destination, path);
                    }
                }
            }
            else
                platform.Logger.Log(this, "Not uploading files");
#endif

            processesPerNode = (args.contains("processesPerNode")) ? ((int) args["processesPerNode"]) : 1;
            platform.Logger.Log(this, "Running " + processesPerNode.ToString() + " processes/node.");

            nprocesses = workerAddresses.Length * processesPerNode;

			loggers = new QS._core_c_.Base.Logger[nprocesses];
			loggerCollection = new QS._qss_e_.Inspection_.LoggerCollection();
            for (int ind = 0; ind < nprocesses; ind++)
			{
				loggers[ind] = new QS._core_c_.Base.Logger(null, true, null, true, string.Empty);
				loggerCollection.add(workerAddresses[ind / processesPerNode].ToString() + "/" + (ind % processesPerNode).ToString(), loggers[ind]);
			}

			debugging = (bool) args["debugging"];
		}

        [QS.Fx.Base.Inspectable("Experiment Path")]
        private string experimentPath;
        private QS._qss_c_.Platform_.IPlatform platform;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._core_c_.Components.AttributeSet constructorArgs;
		private System.Net.IPAddress[] workerAddresses;
        private System.Net.IPAddress[] nonlocal_workerAddresses_AsArray;
        private int processesPerNode, nprocesses;
		private QS._qss_d_.Service_2_.Client serviceClient;
		private QS._core_c_.Base.Logger[] loggers;
		private System.Net.IPAddress localAddress;
		[QS.Fx.Base.Inspectable("WorkerNodes_LogCollection", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Inspection_.LoggerCollection loggerCollection;
		private string executablePath;
		private bool debugging;
		// [Inspection.Inspectable("Environment", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Runtime_.IEnvironment environment;
		private QS._qss_e_.Experiments_.IExperiment experiment;
		[QS.Fx.Base.Inspectable("Result Attributes", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._core_c_.Components.AttributeSet experimentAttributes = new QS._core_c_.Components.AttributeSet(10);
		[QS.Fx.Base.Inspectable]
		public QS._qss_e_.Experiments_.IExperiment Experiment
		{
			get { return (experiment != null) ? experiment : QS._qss_e_.Experiments_.Experiment.None; }
		}
		private System.Object experimentLock = new Object();
		private System.Threading.ManualResetEvent experimentCanStop = new System.Threading.ManualResetEvent(false);
        private System.Threading.ManualResetEvent experimentStopped = new System.Threading.ManualResetEvent(false);

		public QS._core_c_.Components.AttributeSet runExperiment(QS._core_c_.Components.AttributeSet args)
		{
			System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(internal_runExperiment), args);
			return QS._core_c_.Components.AttributeSet.None;
		}

		public bool stopExperiment()
		{
			if (experiment != null)
			{
				experimentCanStop.Set();
                experimentStopped.WaitOne(5000, false);
				return true;
			}
			else
				return false;
		}

		private void internal_runExperiment(object obj)
		{
			lock (experimentLock)
			{
                DateTime timeStarted = DateTime.Now;

				QS._core_c_.Components.AttributeSet args = obj as QS._core_c_.Components.AttributeSet;

				System.Type experimentClass = System.Type.GetType((string)args["experimentClass"]);
				string experimentArguments = (string)args["experimentArguments"];                

                platform.Logger.Log(this, "__RunExperiment : ExperimentClass = " + experimentClass.ToString() +
                    ", Arguments = " + experimentArguments); 

				QS._qss_e_.Runtime_.IRemoteNode[] remoteNodes = new QS._qss_e_.Runtime_.IRemoteNode[nprocesses];

				for (uint ind = 0; ind < nprocesses; ind++)
				{
					loggers[ind].Clear();
					if ((ind == 0) && debugging)
					{
						remoteNodes[ind] = new QS._qss_e_.Runtime_.ManuallyAttachedNode(
							loggers[ind], platform.Logger, localAddress, workerAddresses[ind / processesPerNode],
                            DefaultTimeoutOnRemoteOperations, 65500, 65501);
					}
					else
					{
						remoteNodes[ind] = new QS._qss_e_.Runtime_.ServiceControlledNode(
							loggers[ind], localAddress, 0, 0, serviceClient, new QS.Fx.Network.NetworkAddress(
                            workerAddresses[ind / processesPerNode], (int)QS._qss_d_.Base_.Win32Config.DefaultMainTCPServicePortNo),
                            DefaultTimeoutOnRemoteOperations, executablePath, "App_" + (ind % processesPerNode).ToString());
					}
				}

				platform.Logger.Log(this, "Creating the environment.");

				using (environment = new QS._qss_e_.Environments_.DistributedEnvironment(platform.Logger, remoteNodes))
				{
					experimentAttributes = new QS._core_c_.Components.AttributeSet(10);
					using (experiment = (QS._qss_e_.Experiments_.IExperiment)experimentClass.GetConstructor(System.Type.EmptyTypes).Invoke(new object[] {}))
					{
						platform.Logger.Log(this, "Launching experiment.");

                        QS._core_c_.Components.AttributeSet thisapp_experimentArgs = 
                            new QS._core_c_.Components.AttributeSet(experimentArguments);
                        thisapp_experimentArgs["experimentPath"] = experimentPath;

                        applicationController.upcall("Experiment_Started", QS._core_c_.Components.AttributeSet.None);

                        experiment.run(environment, platform.Logger, thisapp_experimentArgs, experimentAttributes);

						platform.Logger.Log(this, "Experiment completed.");

                        applicationController.upcall("Experiment_Completed", QS._core_c_.Components.AttributeSet.None);

                        if (constructorArgs.contains("sendmail") && ((bool)constructorArgs["sendmail"]))
                        {
                            try
                            {
                                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient((string)constructorArgs["mail-hostname"]);
                                client.Credentials = new System.Net.NetworkCredential((string)constructorArgs["mail-account"],
                                    (string)constructorArgs["mail-password"], (string)constructorArgs["mail-domain"]);
                                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(
                                    new System.Net.Mail.MailAddress((string)constructorArgs["mail-from"], "QuickSilver"),
                                    new System.Net.Mail.MailAddress((string)constructorArgs["mail-to"], (string)constructorArgs["mail-toname"]));
                                message.Subject = "Experiment " + experiment.GetType().Name + " started at " +
                                    timeStarted.ToString() + " has completed.";
                                StringBuilder s = new StringBuilder();
                                s.AppendLine("Arguments:\n");
                                s.AppendLine(thisapp_experimentArgs.ToString());
                                s.AppendLine("\nResults:\n");
                                s.AppendLine(experimentAttributes.ToString());
                                message.Body = s.ToString();
                                client.Send(message);
                            }
                            catch (Exception exc)
                            {
                                platform.Logger.Log(this, "Could not send email notification.\n" + exc.ToString());
                            }
                        }

						experimentCanStop.WaitOne();

						platform.Logger.Log(this, "Experiment destroyed.");

                        applicationController.upcall("Experiment_Destroyed", QS._core_c_.Components.AttributeSet.None);
					}
					experiment = null;
				}
				environment = null;

				for (uint ind = 0; ind < nprocesses; ind++)
				{
					remoteNodes[ind].Dispose();
				}

                try
                {
                    QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                        "QuickSilver_HostAdministrator", nonlocal_workerAddresses_AsArray,
                        platform.Logger, true, false, 10, 
                        (string)constructorArgs["username"], (string)constructorArgs["domain"], (string)constructorArgs["password"]);
                }
                catch (Exception exc)
                {
                    platform.Logger.Log(this,
                        "\nCouldn't stop services on the worker nodes... may not be able to cleanup after the experiment.\n" + exc.ToString() + "\n");
                }
                experimentStopped.Set();
			}
		}

		#region IDisposable Members

		public override void Dispose()
		{
		}

		#endregion
	}
}
