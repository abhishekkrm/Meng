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

#define DEBUG_Service

using System;
using System.Net;

namespace QS._qss_d_.Service_2_
{
	/// <summary>
	/// Summary description for Service.
	/// </summary>
    [QS.Fx.Base.Inspectable]
	public class Service : QS.Fx.Inspection.Inspectable, QS._qss_c_.Base1_.IClient, System.IDisposable
	{
        public const int DefaultTcpChannelPort = 65519;

		private const uint defaultAnticipatedNumberOfClientConnections = 5;
		private const uint defaultAnticipatedNumberOfProcesses = 20;
		private static System.TimeSpan defaultProcessOutputPollingInterval = TimeSpan.FromSeconds(30);

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                int_shutdownAllControlledProcesses();

                tcpDevice.shutdown();

                guiThread.Abort();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private void GuiMain()
        {
            serviceController = new QS._qss_d_.ServiceController_.ServiceController(logger, this, this);
            System.Windows.Forms.Application.Run(serviceController);
        }

        private System.Threading.Thread guiThread;
		public Service(QS._core_c_.Base.IReadableLogger logger, string pathToConfigFile)
		{
			this.logger = logger;

            try
            {
                System.Runtime.Remoting.RemotingConfiguration.Configure(
                    System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + ".config", false);
            }
            catch (Exception exc)
            {
                logger.Log(this, exc.ToString());
            }

            guiThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.GuiMain));
            guiThread.Start();

            logger.Log(this, "Version: Build " + QS.BuildNo.SeqNo.ToString() + ", " + QS.BuildNo.Date.ToString());

/*
			try
			{
				foreach (System.Diagnostics.Process oldprocess in System.Diagnostics.Process.GetProcessesByName("....................");
				{
					try { oldprocess.Kill(); } catch (Exception) {};
				}
			}
			catch (Exception exc)
			{
				logger.Log(this, "While terminating old processes: " + exc.ToString());
			}
*/

			this.configuration = Configuration.load(pathToConfigFile, logger);
            this.processesStartedManuallyByTheUser = configuration.processesStartedManually;

			localAddress = null;
			IPAddress[] candidateIPs = QS._qss_c_.Devices_2_.Network.LocalAddresses;
			System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<IPAddress, QS._qss_c_.Base1_.Subnet>> additionalAddresses = 
				new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<IPAddress, QS._qss_c_.Base1_.Subnet>>();
			for (uint ind = 0; ind < candidateIPs.Length; ind++)
			{
				if (localAddress == null && configuration.requestedSubnet.contains(candidateIPs[ind]))
					localAddress = candidateIPs[ind];
				else
				{
					if (configuration.additionalSubnets != null)
					{
						foreach (QS._qss_c_.Base1_.Subnet subnet in configuration.additionalSubnets)
						{
							if (subnet.contains(candidateIPs[ind]))
								additionalAddresses.Add(
									new System.Collections.Generic.KeyValuePair<IPAddress, QS._qss_c_.Base1_.Subnet>(
										candidateIPs[ind], subnet));
						}
					}
				}
			}

			if (localAddress == null)
				throw new Exception("none of the available local addresses was on the requested subnet");

			logger.Log(null, "Local Address : " + localAddress.ToString() + ":" + configuration.mainPortNo.ToString() + 
				" on subnet " + configuration.requestedSubnet.ToString());

			foreach (System.Collections.Generic.KeyValuePair<IPAddress, QS._qss_c_.Base1_.Subnet> address in additionalAddresses)
			{
				logger.Log(null, "Additional Address : " + address.Key.ToString() + ":" + configuration.mainPortNo.ToString() + " on subnet " +
					address.Value.ToString() + ".");
			}

			this.tcpDevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice("Service_MainTCP",
				localAddress, logger, true, (int) configuration.mainPortNo, defaultAnticipatedNumberOfClientConnections);

			deviceCollection = new QS._qss_c_.Devices_1_.DeviceCollection(configuration.requestedSubnet, tcpDevice);

			if (additionalAddresses.Count > 0)
			{
				additionalTCPDevices = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice[additionalAddresses.Count];
				int index = 0;
				foreach (System.Collections.Generic.KeyValuePair<IPAddress, QS._qss_c_.Base1_.Subnet> address in additionalAddresses)
				{
					QS._qss_c_.Devices_1_.TCPCommunicationsDevice newdevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice(
                        "Service_AdditionalTCP[" + index.ToString() + "]",
						address.Key, logger, true, (int)configuration.mainPortNo, defaultAnticipatedNumberOfClientConnections);
					additionalTCPDevices[index++] = newdevice;
					deviceCollection.Register(address.Value, newdevice);
				}
			}

			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.AnyMessage, QS._qss_c_.Base1_.AnyMessage.Factory);
			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.XmlMessage, QS._core_c_.Base.XmlObject.Factory);

			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.CompressedObject, QS._core_c_.Base.Serializable<QS._qss_c_.Base1_.CompressedObject>.CreateSerializable);
			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.Message, QS._core_c_.Base.Serializable<QS._qss_c_.Base1_.Message>.CreateSerializable);

			QS._qss_c_.Base1_.IDemultiplexer demultiplexer = new QS._qss_c_.Base1_.SimpleDemultiplexer(3);
			demultiplexer.register(this, new QS._qss_c_.Dispatchers_.MultithreadedDispatcher(new QS._qss_c_.Base1_.OnReceive(this.receiveCallback)));
		
//			CMS.Base.ISender baseSender = new CMS.Senders.BaseSender(
//				tcpDevice, null, new CMS.Devices.IReceivingDevice[] { tcpDevice }, demultiplexer, logger);

			QS._qss_c_.Base1_.ISender baseSender = new QS._qss_c_.Senders_1_.BaseSender(
				deviceCollection, null, new QS._qss_c_.Devices_1_.IReceivingDevice[] { deviceCollection }, demultiplexer, logger);

			byte[] cryptographicKey = ServiceHelper.LoadCryptographicKey(configuration.cryptographicKeyFile);
			cryptographicSender = new QS._qss_c_.Senders_1_.CryptoSender(
				baseSender, demultiplexer, logger, System.Security.Cryptography.SymmetricAlgorithm.Create(), cryptographicKey);

			sender = new QS._qss_c_.Senders_1_.CompressingSender(cryptographicSender, demultiplexer);

			this.processes = new QS._qss_c_.Collections_1_.LinkableHashSet(defaultAnticipatedNumberOfProcesses);

//            tcpChannel = new System.Runtime.Remoting.Channels.Tcp.TcpChannel(DefaultTcpChannelPort);
//            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(tcpChannel, true);

//            if (configuration.repositoryRoot != null)
//            {
//                repositoryService = new QS.TMS.Repository.Service(configuration.repositoryRoot, logger);
//                System.Runtime.Remoting.RemotingServices.Marshal(repositoryService, "Repository.rem");
//                logger.Log(this, 
//                    "Repository service for \"" + configuration.repositoryRoot + "\" running at port " + DefaultTcpChannelPort);
//            }

			logger.Log(this, "Initialization completed");
		}

        // private System.Runtime.Remoting.Channels.Tcp.TcpChannel tcpChannel;
        // private QS.TMS.Repository.Service repositoryService;

        private QS._qss_d_.ServiceController_.ServiceController serviceController;
		private IPAddress localAddress;
		private QS._core_c_.Base.IReadableLogger logger;
		private QS._qss_c_.Devices_1_.TCPCommunicationsDevice tcpDevice;
		private QS._qss_c_.Devices_1_.TCPCommunicationsDevice[] additionalTCPDevices;
		private QS._qss_c_.Base1_.ISender sender, cryptographicSender;
		private Configuration configuration;
		private QS._qss_c_.Collections_1_.ILinkableHashSet processes;
		private QS._qss_c_.Devices_1_.IDeviceCollection deviceCollection;

        private bool processesStartedManuallyByTheUser = false;

        public bool ProcessesStartedManuallyByTheUser
        {
            get { return processesStartedManuallyByTheUser; }
            set { processesStartedManuallyByTheUser = value; }
        }

        private int pendingCreationSeqNo;
        private System.Collections.Generic.IDictionary<int, PendingCreation> pendingCreationRecords =
            new System.Collections.Generic.Dictionary<int, PendingCreation>();
        private class PendingCreation
        {
            public PendingCreation(int id, string executablePath, string parameterString)
            {
                this.id = id;
                this.executablePath = executablePath;
                this.parameterString = parameterString;
            }

            public int id, pid = 0;
            public string executablePath, parameterString;
            public System.Threading.ManualResetEvent created = new System.Threading.ManualResetEvent(false);
        }

        private int createProcessThroughTheUser(string executablePath, string parameterString)
        {
            PendingCreation pendingCreation;
            lock (pendingCreationRecords)
            {
                pendingCreation = new PendingCreation(++pendingCreationSeqNo, executablePath, parameterString);
                pendingCreationRecords.Add(pendingCreation.id, pendingCreation);
            }

            logger.Log(this, 
                "Waiting for the user to launch a process, id=" + pendingCreation.id.ToString() + 
                ".\n{\n  " + executablePath + "\n  " + parameterString + "\n}\n");
            pendingCreation.created.WaitOne();

            lock (pendingCreationRecords)
            {
                pendingCreationRecords.Remove(pendingCreation.id);
            }

            logger.Log(this, "The user has created the process, id=" + pendingCreation.id.ToString() + ".");
            return pendingCreation.pid;
        }

        public void RegisterProcess(int id, int pid)
        {
            lock (pendingCreationRecords)
            {
                PendingCreation pendingCreation;
                if (pendingCreationRecords.TryGetValue(id, out pendingCreation))
                {
                    pendingCreationRecords.Remove(id);

                    pendingCreation.pid = pid;
                    pendingCreation.created.Set();
                }
                else
                    logger.Log(this, "Could not find pending creation record for id=" + id.ToString() + ".");
            }
        }

        private QS._core_c_.Components.AttributeSet RegisterProcess(
            QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
        {
            int id = Convert.ToInt32((string)arguments["id"]);
            int pid = Convert.ToInt32((string)arguments["pid"]);

            RegisterProcess(id, pid);

            return QS._core_c_.Components.AttributeSet.None;
        }

		#region ProcessDescriptor

		private class ProcessDescriptor : QS._qss_c_.Collections_1_.GenericLinkable
		{
			public ProcessDescriptor(Base_.ProcessController processController, QS.Fx.Network.NetworkAddress ownerAddress)
			{
				this.processController = processController;
				this.ownerAddress = ownerAddress;
			}

			public Base_.ProcessController processController;
			public QS.Fx.Network.NetworkAddress ownerAddress;

			public override object Contents
			{
				get
				{
					return processController.Ref;
				}
			}
		}

		#endregion

		private QS._core_c_.Components.AttributeSet restartServices(
			QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			QS._core_c_.Base.Logger logger = new QS._core_c_.Base.Logger(null, true);
			try
			{
				string serviceName = (string) arguments["ServiceName"];
				string[] nodeNames = (string[]) arguments["NodeNames"];
				bool shouldStop = (bool) arguments["ShouldStop"];
				bool shouldStart = (bool) arguments["ShouldStart"];
				double timeout = (double) arguments["OperationTimeout"];

                string impersonatedUserName = null, password = null, domainName = null;
                if (arguments.contains("UserName"))
                {
                    impersonatedUserName = (string) arguments["UserName"];
                    password = (string)arguments["Password"];
                    domainName = (string)arguments["DomainName"];
                }

				IPAddress[] nodeAddresses = new IPAddress[nodeNames.Length];
				for (int ind = 0; ind < nodeNames.Length; ind++)
					nodeAddresses[ind] = Dns.GetHostAddresses(nodeNames[ind])[0];

                if (impersonatedUserName != null)
                {
                    QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                        serviceName, nodeAddresses, logger, shouldStop, shouldStart, timeout, impersonatedUserName, domainName, password);
                }
                else
                {
                    QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                        serviceName, nodeAddresses, logger, shouldStop, shouldStart, timeout);
                }
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}

			return new QS._core_c_.Components.AttributeSet("operation_log", logger.CurrentContents);
		}

		private QS._core_c_.Components.AttributeSet killOSProcess(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			string processName = (string) arguments["processName"];
			QS._core_c_.Base.Logger operation_log = new QS._core_c_.Base.Logger(null, true);
			foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName(processName))
			{
				try
				{
					process.Kill();
					operation_log.Log("Successfully terminated process " + process.Id.ToString());
				}
				catch (Exception exc)
				{
					operation_log.Log("Could not terminate process " + process.Id.ToString() + ", " +
						exc.ToString());
				}
			}
			return new QS._core_c_.Components.AttributeSet("operation_log", operation_log.CurrentContents);
		}

		private QS._core_c_.Components.AttributeSet shutdownAllControlledProcesses(
			QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			return new QS._core_c_.Components.AttributeSet("operation_log", int_shutdownAllControlledProcesses());
		}

		private string int_shutdownAllControlledProcesses()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();

			lock (this.processes)
			{
				foreach (ProcessDescriptor pdesc in processes.Elements)
				{
					try
					{
						pdesc.processController.shutdown();
						processes.remove(pdesc.processController.Ref);

						result.Append("Process " + pdesc.processController.Ref.ToString() + 
							" terminated successfully.\n");
					}
					catch (Exception exc)
					{
						result.Append(exc.ToString() + "\n");
					}
				}
			}

			return result.ToString();
		}

		#region Managing Files

		private QS._core_c_.Components.AttributeSet deploy(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			IPAddress[] destinations = (IPAddress[]) arguments["destinations"];
			ServiceDeploymentRequest[] requests = (ServiceDeploymentRequest[]) arguments["requests"];
			TimeSpan timeout = (TimeSpan) arguments["timeout"];

			QS._core_c_.Base.Logger deploymentLogger = new QS._core_c_.Base.Logger(null, true);

			QS._qss_d_.Service_2_.IClient serviceClient = new QS._qss_d_.Service_2_.Client(localAddress, null, configuration.cryptographicKeyFile);
			using (QS._qss_e_.Deployment_.ServiceUploader uploader = new QS._qss_e_.Deployment_.ServiceUploader(serviceClient, deploymentLogger, timeout))
			{
				foreach (IPAddress destination in destinations)
				{
					foreach (ServiceDeploymentRequest deploymentReq in requests)
					{
						logger.Log(this, 
							" - scheduling upload to " + destination.ToString() + ":" + deploymentReq.destinationPath);

						uploader.schedule(deploymentReq.fileAsByteArray, destination, deploymentReq.destinationPath);
					}
				}
			}

			logger.Log(this, " - all operations complete.\nResult of this upload -> " + deploymentLogger.CurrentContents);

			return new QS._core_c_.Components.AttributeSet("log", deploymentLogger.CurrentContents);
		}

		private QS._core_c_.Components.AttributeSet upload(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			string path = (string) arguments["path"];
			byte[] fileAsBytes = Helpers_.Compressor.Uncompress((byte[]) arguments["data"]);

			using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
			{
#if DEBUG_Service
				logger.Log(this, "Writing " + fileAsBytes.Length.ToString() + " bytes to file " + path);
#endif

				fileStream.Write(fileAsBytes, 0, fileAsBytes.Length);
			}

			return QS._core_c_.Components.AttributeSet.None;
		}

		private QS._core_c_.Components.AttributeSet download(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			string path = (string) arguments["path"];
			
			byte[] fileAsBytes = null;
			using (System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
#if DEBUG_Service
				logger.Log(this, "Reading " + fileStream.Length.ToString() + " bytes from file " + path);
#endif

				fileAsBytes = new byte[fileStream.Length];
				if (fileStream.Read(fileAsBytes, 0, (int) fileStream.Length) != fileStream.Length)
					throw new Exception("could not read the whole file");
			}

			return new QS._core_c_.Components.AttributeSet("data", Helpers_.Compressor.Compress(fileAsBytes));
		}

		#endregion

		#region Processing Logs

		private QS._core_c_.Components.AttributeSet completeLog(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			return new QS._core_c_.Components.AttributeSet("log", logger.CurrentContents);
		}

		private QS._core_c_.Components.AttributeSet disposeOfBufferedLogContents(QS.Fx.Network.NetworkAddress clientAddress, 
			QS._core_c_.Components.AttributeSet arguments)
		{
			logger.Clear();
			return QS._core_c_.Components.AttributeSet.None;
		}

		#endregion

		#region Monitoring Process Status

		private QS._core_c_.Components.AttributeSet listOfProcesses(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			Base_.ProcessRef[] processRefs = null;
			lock (processes)
			{
				processRefs = new QS._qss_d_.Base_.ProcessRef[processes.Count];
				uint processref_index = 0;
				foreach (ProcessDescriptor desc in processes.Elements)
					processRefs[processref_index++] = desc.processController.Ref;
			}

			return new QS._core_c_.Components.AttributeSet("processes", processRefs);
		}

		#endregion

		#region Starting and Stopping of Processes

		private QS._core_c_.Components.AttributeSet launch(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			string executablePath = (string) arguments["executablePath"];
			string parameterString = (string) arguments["parameterString"];

            Base_.ProcessController processController;

            if (processesStartedManuallyByTheUser)
            {
#if DEBUG_Service
                logger.Log(this, "Waiting for the user to launch process \"" + executablePath + "\" with parameters \"" + parameterString + "\".");
#endif

                int pid = createProcessThroughTheUser(executablePath, parameterString);

                processController = new QS._qss_d_.Base_.ProcessController(pid, 
                    defaultProcessOutputPollingInterval, new Base_.ProcessExitedCallback(this.processExitedCallback));
            }
            else
            {
#if DEBUG_Service
                logger.Log(this, "Launching process \"" + executablePath + "\" with parameters \"" +
                    parameterString + "\".");
#endif

                processController = new Base_.ProcessController(executablePath, parameterString,
                    defaultProcessOutputPollingInterval, new Base_.ProcessExitedCallback(this.processExitedCallback));

#if DEBUG_Service
                logger.Log(this, "Process \"" + executablePath + "\" was assigned id " +
                    processController.Ref.PID.ToString() + ".");
#endif
            }

			lock (processes)
			{
				processes.insert(new ProcessDescriptor(processController, clientAddress));
			}

            QS._core_c_.Components.AttributeSet response = new QS._core_c_.Components.AttributeSet("processID", processController.Ref);
			return response;
		}

		private void processExitedCallback(Base_.ProcessRef processRef)
		{
#if DEBUG_Service
            logger.Log(this, "Process \"" + processRef.PID.ToString() + " exited.");
#endif

            try
            {
                ProcessDescriptor processDescriptor = null;
                lock (processes)
                {
                    processDescriptor = (ProcessDescriptor)processes.remove(processRef);
                }

                if (processDescriptor != null)
                {
                    QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet(2);
                    arguments["processID"] = processDescriptor.processController.Ref;
                    arguments["output"] = processDescriptor.processController.Err + "\n" + processDescriptor.processController.Out +
                        "\n" + processDescriptor.processController.Log;

                    this.asynchronousCallback(processDescriptor.ownerAddress, new ServiceRequest("exited", arguments));
                }
            }
            catch (Exception)
            {
            }
		}

		private QS._core_c_.Components.AttributeSet shutdown(QS.Fx.Network.NetworkAddress clientAddress, QS._core_c_.Components.AttributeSet arguments)
		{
			Base_.ProcessRef processID = (Base_.ProcessRef) arguments["processID"];
			
			ProcessDescriptor processDescriptor = null;
			lock (processes)
			{
				processDescriptor = (ProcessDescriptor) processes.remove(processID);
			}

			if (processDescriptor != null)
			{
				Base_.ProcessController processController = processDescriptor.processController;
				processController.shutdown();

				return new QS._core_c_.Components.AttributeSet(
					"output", processController.Err + "\n" + processController.Out + "\n" + processController.Log);
			}
			else
				throw new Exception("no such process");
		}

		#endregion

		#region Requests and Responses

		private void receiveCallback(QS._qss_c_.Base1_.IAddress source, QS._core_c_.Base.IMessage message)
		{
			try
			{
				if (!(source is QS._qss_c_.Base1_.ObjectAddress))
					throw new Exception("received a message from an unrecognizable address");

				if ((message is QS._qss_c_.Base1_.AnyMessage) && ((QS._qss_c_.Base1_.AnyMessage) message).Contents is ServiceRequest)
				{
					object response = null;
					ServiceRequest request = (ServiceRequest) ((QS._qss_c_.Base1_.AnyMessage) message).Contents;

#if DEBUG_Service
					logger.Log(this, "received request : " + request.ToString());
#endif

					try
					{
						System.Reflection.MethodInfo methodInfo = this.GetType().GetMethod(request.methodName, 
							System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, 
							new System.Type[] { typeof(QS.Fx.Network.NetworkAddress), typeof(QS._core_c_.Components.AttributeSet) }, null);
						if (methodInfo == null)
							throw new Exception("the requested method could not be found");

						response = methodInfo.Invoke(this, new object[] { ((QS._qss_c_.Base1_.ObjectAddress) source), request.argument });

						if (!(response is QS._core_c_.Components.AttributeSet))
							throw new Exception("method returned an argument of incompatible type");
					}
					catch (Exception exc)
					{
						response = exc;
					}

#if DEBUG_Service
					logger.Log(this, "sending back response : " + response.ToString());
#endif

					sender.send(this, source, new QS._qss_c_.Base1_.AnyMessage(response), null);
				}
				else
					logger.Log(this, "warning : receive message of incorrect type");
			}
			catch (Exception exc)
			{
				logger.Log(this, "Receive Callback : " + exc.ToString());
			}
		}

		private void asynchronousCallback(QS.Fx.Network.NetworkAddress clientAddress, ServiceRequest request)
		{
			try
			{
				sender.send(this, new QS._qss_c_.Base1_.ObjectAddress(clientAddress, (uint) ReservedObjectID.HostManagementObject), 
					new QS._qss_c_.Base1_.AnyMessage(request), null);
			}
			catch (Exception exc)
			{
				logger.Log(this, "Asynchronous Callback : " + exc.ToString());
			}
		}

		#endregion

		#region Configuration

		[Serializable]
		[System.Xml.Serialization.XmlType("ServiceConfiguration")]
		public class Configuration
		{
			public Configuration()
			{
			}

			private void overrideWithDefaultSettings()
			{
				this.requestedSubnet = new QS._qss_c_.Base1_.Subnet("x.x.x.x");
				this.mainPortNo = QS._qss_d_.Base_.Win32Config.DefaultMainTCPServicePortNo;
				this.cryptographicKeyFile = QS._qss_d_.Base_.Win32Config.DefaultCryptographicKeyFile;
				this.additionalSubnets = new QS._qss_c_.Base1_.Subnet[] { }; // new QS.CMS.Base.Subnet("192.168.x.x") };
			}

			public QS._qss_c_.Base1_.Subnet requestedSubnet;
			public QS._qss_c_.Base1_.Subnet[] additionalSubnets;
			public uint mainPortNo;
			public string cryptographicKeyFile;
            public bool processesStartedManually;
            public string repositoryRoot;

			public static Configuration load(string localConfigurationFileName, QS.Fx.Logging.ILogger logger)
			{
				QS._qss_d_.Service_2_.Service.Configuration localConfiguration = null;
				try
				{
					System.Xml.Serialization.XmlSerializer configurationSerializer = 
						new System.Xml.Serialization.XmlSerializer(typeof(QS._qss_d_.Service_2_.Service.Configuration));
					System.IO.TextReader configurationReader = new System.IO.StreamReader(localConfigurationFileName, System.Text.Encoding.Unicode);
					localConfiguration = (QS._qss_d_.Service_2_.Service.Configuration) configurationSerializer.Deserialize(configurationReader);
					configurationReader.Close();
				}
				catch (Exception exc)
				{
					logger.Log(null, "Could not load configuration, overriding with defaults; the exception caught : " + exc.ToString());

					localConfiguration = new Configuration();
					localConfiguration.overrideWithDefaultSettings();
					localConfiguration.save(localConfigurationFileName);
				}

				return localConfiguration;
			}

			public void save(string localConfigurationFileName)
			{
				lock (this)
				{
					System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(QS._qss_d_.Service_2_.Service.Configuration));
					System.IO.TextWriter w = new System.IO.StreamWriter(localConfigurationFileName, false, System.Text.Encoding.Unicode);
					s.Serialize(w, this);
					w.Close();
				}
			}
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.HostManagementObject;
			}
		}

		#endregion
	}
}
