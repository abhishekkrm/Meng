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

namespace QS._qss_d_.Console_
{
/*
	/// <summary>
	/// Aaa...
	/// </summary>
	public class Console : Skeleton
	{
		public const string PathToLocalConsoleConfigurationFile = "C:\\.testing\\console_configuration.xml";

		public Console(CMS.Base.IReadableLogger logger) : base(logger, false) 
		{
			localRPCClient = new CMS.RPC.Client((uint) ReservedObjectID.TCPBasedRPCClient,  
				(uint) ReservedObjectID.TCPBasedRPCServer, sender, demultiplexer, logger, 5);
			multicastingRPCClient = new CMS.RPC.Client((uint) ReservedObjectID.UDPBasedRPCClient, 
				(uint) ReservedObjectID.UDPBasedRPCServer, multicastingSender, demultiplexer, logger, 5);

			this.localConsoleConfigurationFileName = PathToLocalConsoleConfigurationFile;
			
			try
			{
				System.Xml.Serialization.XmlSerializer configurationSerializer = 
					new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
				TextReader configurationReader = new StreamReader(
					localConsoleConfigurationFileName, System.Text.Encoding.Unicode);
				localConsoleConfiguration = 
					(Configuration) configurationSerializer.Deserialize(configurationReader);
				configurationReader.Close();
			}
			catch (Exception)
			{
				logger.Log(this, "Could not parse configuration file, using default one.");
				localConsoleConfiguration = new Configuration();

				saveConfiguration();
			}
		}

		private void saveConfiguration()
		{
			lock (localConsoleConfiguration)
			{
				System.Xml.Serialization.XmlSerializer s = 
					new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
				System.IO.TextWriter w = new System.IO.StreamWriter(localConsoleConfigurationFileName,
					false, System.Text.Encoding.Unicode);
				s.Serialize(w, localConsoleConfiguration);
				w.Close();
			}
		}

		public Configuration CurrentConfiguration
		{
			get
			{
				return localConsoleConfiguration;
			}
		}

		private CMS.RPC.IClient localRPCClient, multicastingRPCClient;
		private string localConsoleConfigurationFileName;
		private Configuration localConsoleConfiguration;

		public void neighborhoodScan()
		{
			try
			{				
				foreach (object obj in localConsoleConfiguration.hosts.Values)
					((Configuration.HostDescription) obj).responding = false;

				CMS.Base.IAddress destinationAddress = new CMS.Base.ObjectAddress(
					IPAddress.Parse(Service.Service.HOST_MANAGEMENT_SERVICE_IPMULTICASTADDR), 
					(int) Service.Service.HOST_MANAGEMENT_SERVICE_PORT_NUMBER_MCAST, this.LocalObjectID);

				CMS.RPC.IResult[] responses = multicastingRPCClient.synchronousCollect(
					this, destinationAddress, new CMS.Base.AnyMessage(
					new Service.Protocol.Request(Service.Protocol.Request.Operation.DISCOVER_ALL_SERVICES)), 
					TimeSpan.FromMilliseconds(250));

				foreach (CMS.RPC.IResult result in responses)
				{
					IPAddress address = result.Source.HostIPAddress;

					Service.Protocol.Response response = 
						(Service.Protocol.Response) (((CMS.Base.AnyMessage) result.Result).Contents);

					// logger.Log(this, "RESPONSE -> " + response.ToString());

					Service.Configuration configuration = (Service.Configuration) response["configuration"];
										
					if (configuration == null)
						throw new Exception("configuration == null");

					CMS.Collections.IDictionaryEntry dic_en = localConsoleConfiguration.hosts.lookupOrCreate(address);
					if (dic_en.Value != null)
					{
						((Configuration.HostDescription) dic_en.Value).configuration = configuration;
					}
					else 
					{
						dic_en.Value = new Configuration.HostDescription(address, configuration);
					}

					((Configuration.HostDescription) dic_en.Value).responding = true;
				}

				saveConfiguration();
			}
			catch (Exception exc)
			{
				logger.Log(this, "Neighborhood Scan Failed : " + exc.ToString());
			}
		}		

		public Service.Protocol.Response issueRequest(IPAddress instanceAddress, Service.Protocol.Request request)
		{
			CMS.Base.IAddress destinationAddress = new CMS.Base.ObjectAddress(instanceAddress, 
				(int) Service.Service.HOST_MANAGEMENT_SERVICE_PORT_NUMBER_UCAST_TCP, this.LocalObjectID);

			CMS.RPC.IResult[] responses = localRPCClient.synchronousCollect(this, destinationAddress, 
				new CMS.Base.AnyMessage(request), TimeSpan.FromMilliseconds(100));

			if (responses.Length == 0)
				throw new Exception("No responses were received!");

			if (responses.Length != 1)
				throw new Exception("Multiple response received!");

			object responseObject = ((CMS.Base.AnyMessage) responses[0].Result).Contents;

			if (!(responseObject is Service.Protocol.Response))
				throw new Exception("Response is not of type \"Service.Protocol.Response\"");

			Service.Protocol.Response response = (Service.Protocol.Response) responseObject;

			if (!response.operationSucceeded)
				throw new Exception("Operation failed : " + response.errorString);
			
			logger.Log(this, "Operation completed successfully.");
			return response;
		}

		public string downloadACompleteServiceLog(IPAddress instanceAddress)
		{
			return (string) issueRequest(instanceAddress, new Service.Protocol.Request(
				Service.Protocol.Request.Operation.DOWNLOAD_COMPLETE_SERVICE_LOG))["messages"];
		}

		public void launchAnApplication(IPAddress instanceAddress, string identifyingAppName)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.LAUNCH_APPLICATION);
			request["applicationName"] = identifyingAppName;
			issueRequest(instanceAddress, request);
		}

		public Service.Configuration.Application.Instance[] downloadApplicationInstances(
			IPAddress instanceAddress, string identifyingAppName)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.GET_ALL_APPLICATION_INSTANCES);
			request["applicationName"] = identifyingAppName;
			Service.Protocol.Response response = issueRequest(instanceAddress, request);
			Service.Configuration.Application.Instance[] instances = 
				(Service.Configuration.Application.Instance[]) response["instances"];

			Configuration.HostDescription host = 
				(Configuration.HostDescription) localConsoleConfiguration.hosts[instanceAddress];
			Service.Configuration.Application app = 
				(Service.Configuration.Application) host.configuration.lookupApp(identifyingAppName);
			app.Instances = instances;

			return instances;			
		}

		public void downloadApplicationInstanceOutput(IPAddress instanceAddress, string identifyingAppName,
			Service.Configuration.Application.Instance.Ref instanceRef, ref string stdOut, ref string stdErr, ref string logOut)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.DOWNLOAD_ALL_INSTANCE_OUTPUTS);
			request["applicationName"] = identifyingAppName;
			request["instanceRef"] = instanceRef;
			Service.Protocol.Response response = issueRequest(instanceAddress, request);
			stdOut = (string) response["stdOut"];
			stdErr = (string) response["stdErr"];
			logOut = (string) response["logOut"];
		}

		public void createAnApplication(IPAddress instanceAddress, 
			string identifyingName, string descriptiveName, string executableName, string startParameters)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.CREATE_APPLICATION);
			request["identifyingName"] = identifyingName;
			request["descriptiveName"] = descriptiveName;
			request["executableName"] = executableName;
			request["startParameters"] = startParameters;
			issueRequest(instanceAddress, request);
		}

		public void removeAnApplication(IPAddress instanceAddress, string identifyingAppName)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.REMOVE_APPLICATION);
			request["applicationName"] = identifyingAppName;
			issueRequest(instanceAddress, request);
		}


		public void shutdownApplication(IPAddress instanceAddress, string identifyingAppName, 
			Service.Configuration.Application.Instance.Ref instanceRef)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.SHUTDOWN_APPLICATION);
			request["applicationName"] = identifyingAppName;
			request["instanceRef"] = instanceRef;
			issueRequest(instanceAddress, request);
		}

		public void removeApplicationInstance(IPAddress instanceAddress, string identifyingAppName,
			Service.Configuration.Application.Instance.Ref instanceRef)
		{
			Service.Protocol.Request request = 
				new Service.Protocol.Request(Service.Protocol.Request.Operation.REMOVE_APPLICATION_INSTANCE);
			request["applicationName"] = identifyingAppName;
			request["instanceRef"] = instanceRef;
			issueRequest(instanceAddress, request);
		}

		public static void startupARemoteService(IPAddress instanceAddress, string serviceName)
		{
			System.ServiceProcess.ServiceController serviceController = 
				new System.ServiceProcess.ServiceController(serviceName, instanceAddress.ToString());

			System.ServiceProcess.ServiceControllerStatus currentStatus = serviceController.Status;
			if (currentStatus != System.ServiceProcess.ServiceControllerStatus.Stopped)
			{
				throw new Exception("Service on " + instanceAddress.ToString() + " is currently " + 
					currentStatus.ToString() + " and cannot be started.");
			}
			else
			{
				serviceController.Start();
				serviceController.WaitForStatus(
					System.ServiceProcess.ServiceControllerStatus.Running, TimeSpan.FromSeconds(3));
			}
		}

		public static void shutdownRemoteService(string serviceName, IPAddress instanceAddress)
		{
			System.ServiceProcess.ServiceController serviceController = 
				new System.ServiceProcess.ServiceController(serviceName, instanceAddress.ToString());

			System.ServiceProcess.ServiceControllerStatus currentStatus = serviceController.Status;
			if (currentStatus != System.ServiceProcess.ServiceControllerStatus.Running)
			{
				throw new Exception("Service on " + instanceAddress.ToString() + " is currently " + 
					currentStatus.ToString() + " and cannot be stopped.");
			}
			else
			{
				serviceController.Stop();
				serviceController.WaitForStatus(
					System.ServiceProcess.ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
			}
		}

		protected override void receiveCallback(CMS.Base.IAddress source, CMS.Base.IMessage message)
		{
/-*
			logger.Log(
				this, "MESSAGE from " + source.ToString() + " : " + message.ToString());
*-/				
		}
	}
*/
}
