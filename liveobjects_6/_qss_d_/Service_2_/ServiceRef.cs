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

// #define DEBUG_ServiceRef

using System;
using System.Threading;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace QS._qss_d_.Service_2_
{
	/// <summary>
	/// Summary description for Client.
	/// </summary>
	public class ServiceRef : IDisposable, QS._qss_c_.Base1_.IClient, QS._qss_d_.Service_2_.IServiceRef
	{
		private const uint defaultAnticipatedNumberOfProcesses = 10;

		public ServiceRef(IPAddress localAddress, QS.Fx.Network.NetworkAddress serviceAddress, QS.Fx.Logging.ILogger logger, 
			TimeSpan timeoutOnRequests, byte[] cryptographicKey)
		{
			this.timeoutOnRequests = timeoutOnRequests;
			this.logger = logger;
			this.tcpDevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice("ServiceRef_TCP", localAddress, logger, false, 0, 2);
			this.serviceAddress = new QS._qss_c_.Base1_.ObjectAddress(serviceAddress, (uint) ReservedObjectID.HostManagementObject);
			this.responseArrived = new AutoResetEvent(false);

			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.AnyMessage, QS._qss_c_.Base1_.AnyMessage.Factory);
			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.XmlMessage, QS._core_c_.Base.XmlObject.Factory);

			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.CompressedObject, QS._core_c_.Base.Serializable<QS._qss_c_.Base1_.CompressedObject>.CreateSerializable);
			QS._qss_c_.Base1_.Serializer.Get.register(QS.ClassID.Message, QS._core_c_.Base.Serializable<QS._qss_c_.Base1_.Message>.CreateSerializable);

			QS._qss_c_.Base1_.IDemultiplexer demultiplexer = new QS._qss_c_.Base1_.SimpleDemultiplexer(3);
			demultiplexer.register(this, new QS._qss_c_.Dispatchers_.DirectDispatcher(new QS._qss_c_.Base1_.OnReceive(this.receiveCallback)));
		
			QS._qss_c_.Base1_.ISender baseSender = new QS._qss_c_.Senders_1_.BaseSender(
				tcpDevice, null, new QS._qss_c_.Devices_1_.IReceivingDevice[] { tcpDevice }, demultiplexer, logger);

			cryptographicSender = new QS._qss_c_.Senders_1_.CryptoSender(
				baseSender, demultiplexer, logger, System.Security.Cryptography.SymmetricAlgorithm.Create(), cryptographicKey);

			sender = new QS._qss_c_.Senders_1_.CompressingSender(cryptographicSender, demultiplexer);

			this.processRefs = new QS._qss_c_.Collections_1_.LinkableHashSet(defaultAnticipatedNumberOfProcesses);
		}

//        private QS.TMS.Repository.IRepositoryClient repositoryClient;

		private QS.Fx.Logging.ILogger logger;
		private bool unusable = false;
		private QS._qss_c_.Devices_1_.TCPCommunicationsDevice tcpDevice;
		private QS._qss_c_.Base1_.ObjectAddress serviceAddress;
		private object response = null;
		private TimeSpan timeoutOnRequests;
		private AutoResetEvent responseArrived;
		private QS._qss_c_.Base1_.ISender sender, cryptographicSender;

		private QS._qss_c_.Collections_1_.ILinkableHashSet processRefs;

		public string killOSProcess(string processName)
		{
			return (string) (this.invoke(
				"killOSProcess", new QS._core_c_.Components.AttributeSet("processName", processName)))["operation_log"];
		}

		public string shutdownAllControlledProcesses()
		{
			return (string) (this.invoke(
				"shutdownAllControlledProcesses", QS._core_c_.Components.AttributeSet.None))["operation_log"];
	}

		#region WaitingGuy

		private class WaitingGuy
		{
			public WaitingGuy()
			{
			}

			public AutoResetEvent terminated = new AutoResetEvent(false);

			public void processAbortedCallback(IProcessRef processRef)
			{
				terminated.Set();
			}
		}

		#endregion

		public string restartServices(string serviceName, string[] nodeNames, bool shouldStop, bool shouldStart, double timeout)
		{
			QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet();
			arguments["ServiceName"] = serviceName;
			arguments["NodeNames"] = nodeNames;
			arguments["ShouldStop"] = shouldStop;
			arguments["ShouldStart"] = shouldStart;
			arguments["OperationTimeout"] = timeout;
			return (string) (this.invoke("restartServices", arguments))["operation_log"];
		}

        public string restartServices(string serviceName, string[] nodeNames, bool shouldStop, bool shouldStart, double timeout,
            string impersonatedUserName, string password, string domainName)
        {
            QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet();
            arguments["ServiceName"] = serviceName;
            arguments["NodeNames"] = nodeNames;
            arguments["ShouldStop"] = shouldStop;
            arguments["ShouldStart"] = shouldStart;
            arguments["OperationTimeout"] = timeout;
            arguments["UserName"] = impersonatedUserName;
            arguments["Password"] = password;
            arguments["DomainName"] = domainName;
            return (string)(this.invoke("restartServices", arguments))["operation_log"];
        }        

		#region Managing Files

		public string deploy(DeploymentRequest[] deploymentRequests, IPAddress[] destinations, System.TimeSpan timeout)
		{
			Service_2_.ServiceDeploymentRequest[] serviceDeploymentRequests = 
				new Service_2_.ServiceDeploymentRequest[deploymentRequests.Length];

			for (uint ind = 0; ind < deploymentRequests.Length; ind++)
			{
				using (System.IO.FileStream fileStream = new System.IO.FileStream(
					deploymentRequests[ind].sourcePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
					byte[] fileAsBytes = new byte[fileStream.Length];
					if (fileStream.Read(fileAsBytes, 0, (int) fileStream.Length) != fileStream.Length)
						throw new Exception("could not read the whole file");
					serviceDeploymentRequests[ind] = 
						new Service_2_.ServiceDeploymentRequest(fileAsBytes, deploymentRequests[ind].targetPath);
				}
			}

			QS._core_c_.Components.AttributeSet args = new QS._core_c_.Components.AttributeSet(3);
			args["requests"] = serviceDeploymentRequests;
			args["destinations"] = destinations;
			args["timeout"] = timeout;
			QS._core_c_.Components.AttributeSet result = this.invoke("deploy", args);			

			return (string) result["log"];
		}

		public void upload(byte[] fileAsBytes, string remotePath)
		{
			QS._core_c_.Components.AttributeSet args = new QS._core_c_.Components.AttributeSet(2);
			args["path"] = remotePath;
			args["data"] = Helpers_.Compressor.Compress(fileAsBytes);

			this.invoke("upload", args);			
		}

		public void upload(string localPath, string remotePath)
		{
			using (System.IO.FileStream fileStream = new System.IO.FileStream(localPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
#if DEBUG_ServiceRef
				logger.Log(this, "Reading " + fileStream.Length.ToString() + " bytes from file " + localPath);
#endif

				byte[] fileAsBytes = new byte[fileStream.Length];
				if (fileStream.Read(fileAsBytes, 0, (int) fileStream.Length) != fileStream.Length)
					throw new Exception("could not read the whole file");

				this.upload(fileAsBytes, remotePath);
			}
		}

		public void download(string remotePath, string localPath)
		{
			byte[] fileAsBytes = Helpers_.Compressor.Uncompress(
				(byte[]) (this.invoke("download", new QS._core_c_.Components.AttributeSet("path", remotePath)))["data"]);

			using (System.IO.FileStream fileStream = new System.IO.FileStream(localPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
			{
#if DEBUG_ServiceRef
				logger.Log(this, "Writing " + fileAsBytes.Length.ToString() + " bytes to file " + localPath);
#endif

				fileStream.Write(fileAsBytes, 0, (int) fileAsBytes.Length);
			}
		}

		#endregion

		#region Processing Logs

		public string CompleteLog
		{
			get
			{
				return (string) (this.invoke("completeLog", QS._core_c_.Components.AttributeSet.None))["log"];			
			}
		}

		public void disposeOfBufferedLogContents()
		{
			this.invoke("disposeOfBufferedLogContents", QS._core_c_.Components.AttributeSet.None);			
		}

		#endregion

		#region Viewing Process Status

		public Base_.ProcessRef[] CurrentProcesses
		{
			get
			{
				QS._core_c_.Components.AttributeSet result = this.invoke("listOfProcesses", QS._core_c_.Components.AttributeSet.None);
				return (Base_.ProcessRef[]) result["processes"];			
			}
		}

		public Base_.ProcessController.Status statusOf(Base_.ProcessRef processRef)
		{
			throw new Exception("not implemented");
		}

		#endregion

		#region Starting and Stopping of Processes

		#region HMS.Service2.IClient Members

		public string launch(string executablePath, string parameters)
		{
			WaitingGuy waitingGuy = new WaitingGuy();
			string result = null;
			using (IProcessRef processRef = this.launch(executablePath, parameters, new ProcessAbortedCallback(waitingGuy.processAbortedCallback)))
			{
				waitingGuy.terminated.WaitOne();
				result = ((ProcessRef) processRef).CompleteOutput;
			}

			return result;
		}

		public string launch(string executablePath, string parameters, TimeSpan timeout)
		{
			WaitingGuy waitingGuy = new WaitingGuy();
			string result = null;
			using (IProcessRef processRef = this.launch(executablePath, parameters, new ProcessAbortedCallback(waitingGuy.processAbortedCallback)))
			{
				if (!waitingGuy.terminated.WaitOne(timeout, false))
					throw new Exception("process has not completed within the specified time interval");
				result = ((ProcessRef) processRef).CompleteOutput;
			}

			return result;
		}

		public IProcessRef launch(string executablePath, string parameters, ProcessAbortedCallback processAbortedCallback)
		{
			QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet(2);
			arguments["executablePath"] = executablePath;
			arguments["parameterString"] = parameters;

			QS._core_c_.Components.AttributeSet result = this.invoke("launch", arguments);

			Base_.ProcessRef processID = (Base_.ProcessRef) result["processID"];
			
			ProcessRef processRef = new ProcessRef(executablePath, parameters, processID, processAbortedCallback, this);		

			lock (processRefs)
			{
				processRefs.insert(processRef);
			}

			return processRef;
		}

		#endregion

		private string shutdown(object processID)
		{
			QS._core_c_.Components.AttributeSet result = this.invoke("shutdown", new QS._core_c_.Components.AttributeSet("processID", processID));

			lock (processRefs)
			{
				processRefs.remove(processID);
			}

			return (string) result["output"];
		}

		private void exited(QS._core_c_.Components.AttributeSet arguments)
		{
			object processID = arguments["processID"];
			string output = (string) arguments["output"];

			ProcessRef processRef = null;
			lock (processRefs)
			{
				processRef = (ProcessRef) processRefs.remove(processID);
			}

			if (processRef != null)
			{
				processRef.exited(output);
			}
		}

		#endregion

		#region ProcessRef

		private class ProcessRef : QS._qss_c_.Collections_1_.GenericLinkable, IProcessRef
		{
			public ProcessRef(string process, string arguments, Base_.ProcessRef processID, ProcessAbortedCallback processAbortedCallback, 
				ServiceRef encapsulatingClient)
			{
				this.process = process;
				this.arguments = arguments;
				this.processID = processID;
				this.processAbortedCallback = processAbortedCallback;
				this.encapsulatingClient = encapsulatingClient;
			}

			private string process, arguments;
			private Base_.ProcessRef processID;
			private ProcessAbortedCallback processAbortedCallback;
			private ServiceRef encapsulatingClient;

			private string output = null;

			#region IProcessRef Members

			public Base_.ProcessRef ID
			{
				get
				{
					return this.processID;
				}
			}

			public string CompleteOutput
			{
				get
				{
					return output;
				}
			}

			#endregion

			public void exited(string output)
			{
				lock (this)
				{
					this.output = output;
					processAbortedCallback(this);
					processID = null;
				}
			}

			#region GenericLinkable Members

			public override object Contents
			{
				get
				{
					return processID;
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
                try
                {
                    if (processID != null)
                    {
                        ServiceRef client = encapsulatingClient;
                        encapsulatingClient = null;
                        client.shutdown(this.processID);
                    }
                }
                catch (Exception)
                {
                }
			}

			#endregion
		}

		#endregion

		#region Sending and Receiving

		private QS._core_c_.Components.AttributeSet invoke(string methodName, QS._core_c_.Components.AttributeSet argument)
		{
			if (unusable)
				throw new Exception("client unusable");

			ServiceRequest request = new QS._qss_d_.Service_2_.ServiceRequest(methodName, argument);
			object responseObject = null;

			lock (this)
			{
				sender.send(this, serviceAddress, new QS._qss_c_.Base1_.AnyMessage(request), null);

				if (!responseArrived.WaitOne(timeoutOnRequests, false))
				{
					this.Dispose();
					throw new Exception("remote server is not responding");
				}

				responseObject = this.response;
				this.response = null;
			}
				
			if (responseObject is QS._core_c_.Components.AttributeSet)
				return (QS._core_c_.Components.AttributeSet) responseObject;
			else if (responseObject is System.Exception)
				throw (System.Exception) responseObject;
			else
				throw new Exception("received an unrecognizable response");
		}

        private void processAsynchronousCallback(ServiceRequest request)
		{
			try
			{
				System.Reflection.MethodInfo methodInfo = GetType().GetMethod(request.methodName, System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) }, null);
				if (methodInfo == null)
					throw new Exception("the requested method could not be found");

				methodInfo.Invoke(this, new object[] { request.argument });
			}
			catch (Exception exc)
			{
				logger.Log(this, "While processing an asynchronous callback : " + exc.ToString());
			}
		}

		private void receiveCallback(QS._qss_c_.Base1_.IAddress source, QS._core_c_.Base.IMessage message)
		{
			if (message is QS._qss_c_.Base1_.AnyMessage)
			{
				object receivedObject = ((QS._qss_c_.Base1_.AnyMessage) message).Contents;
				if (receivedObject is ServiceRequest)
					this.processAsynchronousCallback((ServiceRequest) receivedObject);
				else 
				{
					this.response = receivedObject;
					responseArrived.Set();
				}
			}
			else
				logger.Log(this, "received an unrecognizable message");
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			lock (this)
			{
                try
                {
                    foreach (ProcessRef processRef in processRefs.Elements)
                    {
                        try
                        {
                            processRef.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception)
                {
                }

				this.unusable = true;

                if (tcpDevice != null)
                {
                    try
                    {
                        tcpDevice.shutdown();
                    }
                    catch (Exception)
                    {
                    }
                }

				tcpDevice = null;
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

        public QS._qss_e_.Repository_.IRepositoryClient RepositoryClient
        {
            get 
            {
                throw new Exception("This functionality is obsolete.");

/*
                if (repositoryClient == null)
                    repositoryClient = new QS.TMS.Repository.Client((QS.TMS.Repository.IRepositoryService)
                        Activator.GetObject(typeof(QS.TMS.Repository.IRepositoryService), "tcp://" + serviceAddress.HostIPAddress.ToString() + ":" + 
                            QS.HMS.Service2.Service.DefaultTcpChannelPort.ToString() + "/Reposiory.rem"));
                return repositoryClient; 
*/ 
            }
        }
	}
}
