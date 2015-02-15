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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace QS._qss_d_.Service_1_
{
/*
	/// <summary>
	/// Aaa...
	/// </summary>
	public class Service : Skeleton
	{
		public Service(CMS.Base.IReadableLogger logger) : base(logger, true)
		{
			localRPCServer = new CMS.RPC.Server(
				(uint) ReservedObjectID.TCPBasedRPCServer, sender, demultiplexer, logger, 5);
			multicastingRPCServer = new CMS.RPC.Server(
				(uint) ReservedObjectID.UDPBasedRPCServer, multicastingSender, demultiplexer, logger, 5);
			
			localRPCServer.register(this, new CMS.RPC.SynchronousCallback(this.remoteCall));
			multicastingRPCServer.register(this, new CMS.RPC.SynchronousCallback(this.remoteCall));

			this.localConfigurationFileName = QS.HMS.Base.Win32Config.PathToLocalConfigurationFile;
			
			try
			{
				System.Xml.Serialization.XmlSerializer configurationSerializer = 
					new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
				TextReader configurationReader = new StreamReader(
					localConfigurationFileName, System.Text.Encoding.Unicode);
				localConfiguration = 
					(Configuration) configurationSerializer.Deserialize(configurationReader);
				configurationReader.Close();
			}
			catch (Exception)
			{
				logger.Log(this, "Could not parse configuration file, using default one.");
				localConfiguration = new Configuration();

				saveConfiguration();
			}
		}

		private void saveConfiguration()
		{
			lock (localConfiguration)
			{
				System.Xml.Serialization.XmlSerializer s = 
					new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
				System.IO.TextWriter w = new System.IO.StreamWriter(localConfigurationFileName,
					false, System.Text.Encoding.Unicode);
				s.Serialize(w, localConfiguration);
				w.Close();
			}
		}

		private CMS.RPC.IServer localRPCServer, multicastingRPCServer;
		private string localConfigurationFileName;
		private Configuration localConfiguration;
		
		private CMS.Base.IBase1Serializable remoteCall(
			CMS.Base.IAddress callerAddress, CMS.Base.IBase1Serializable argument)
		{
			// logger.Log(this, "REQUEST:\n" + argument.ToString());

			Protocol.Response response = new Protocol.Response(true, "");			

			try
			{
				if (!(argument is CMS.Base.AnyMessage))
					throw new Exception("The received request is not wrapped with \"CMS.Base.AnyMessage\"");

				object requestObject = ((CMS.Base.AnyMessage) argument).Contents;

				if (!(requestObject is Protocol.Request))
					throw new Exception("The received request is not of type \"CMS.HMS.Service.Protocol.Request\"");

				Protocol.Request request = (Protocol.Request) requestObject;

				logger.Log(this, "Received a request of type " + request.RequestedOperation);

				switch (request.RequestedOperation)
				{
					case Protocol.Request.Operation.DISCOVER_ALL_SERVICES:
					{
						logger.Log(this, "responding to discovery request from " + callerAddress.ToString());

						if (localConfiguration == null)
							throw new Exception("localConfiguration == null");

						// HACK: For now, we just return a structure that could be modified while we are serializing the RPC response,
						//       but going around this little bug doesn't seem to be worth the time required to fix it at this moment.
						response["configuration"] = localConfiguration;

						if (response["configuration"] == null)
							throw new Exception("response[\"configuration\"] = localConfiguration FAILED");
					}
					break;
					
					case Protocol.Request.Operation.DOWNLOAD_COMPLETE_SERVICE_LOG:
					{
						response["messages"] = logger.CurrentContents;				
					}
					break;

					case Protocol.Request.Operation.LAUNCH_APPLICATION:
					{
						lock (localConfiguration)
						{
							Configuration.Application app = (Configuration.Application) 
								localConfiguration.lookupApp((string) request["applicationName"]);
							app.launchAnInstance();
						}						
					}
					break;

					case Protocol.Request.Operation.GET_ALL_APPLICATION_INSTANCES:
					{
						lock (localConfiguration)
						{
							Configuration.Application app = (Configuration.Application) 
								localConfiguration.lookupApp((string) request["applicationName"]);
							response["instances"] = app.Instances;
						}
					}
					break;

					case Protocol.Request.Operation.DOWNLOAD_ALL_INSTANCE_OUTPUTS:
					{
						lock (localConfiguration)
						{
							Configuration.Application app = (Configuration.Application) 
								localConfiguration.lookupApp((string) request["applicationName"]);
							Configuration.Application.Instance instance = 
								app.lookupAnInstance((Configuration.Application.Instance.Ref) request["instanceRef"]);
							response["stdOut"] = instance.StdOut;
							response["stdErr"] = instance.StdErr;
							response["logOut"] = instance.LogOut;
						}
					}
					break;

					case Protocol.Request.Operation.CREATE_APPLICATION:
					{						
						lock (localConfiguration)
						{
							localConfiguration.createApp(new Configuration.Application(
								(string) request["identifyingName"], (string) request["descriptiveName"],
								(string) request["executableName"], (string) request["startParameters"]));
						}

						saveConfiguration();
					}
					break;

					case Protocol.Request.Operation.REMOVE_APPLICATION:
					{
						lock (localConfiguration)
						{
							string appname = (string) request["applicationName"];
							Configuration.Application app = 
								(Configuration.Application) localConfiguration.lookupApp(appname);

							if (!app.CanBeRemoved)
								throw new Exception("This application cannot be removed because it is running.");
							
							localConfiguration.removeApp(appname);
						}						

						saveConfiguration();
					}
					break;

					case Protocol.Request.Operation.SHUTDOWN_APPLICATION:
					{
						lock (localConfiguration)
						{
							Configuration.Application app = (Configuration.Application) 
								localConfiguration.lookupApp((string) request["applicationName"]);						

							app.shutdownInstance((Configuration.Application.Instance.Ref) request["instanceRef"]);
						}
					}
					break;

					case Protocol.Request.Operation.REMOVE_APPLICATION_INSTANCE:
					{
						lock (localConfiguration)
						{
							Configuration.Application app = (Configuration.Application) 
								localConfiguration.lookupApp((string) request["applicationName"]);
							
							app.removeInstance((Configuration.Application.Instance.Ref) request["instanceRef"]);
						}
					}
					break;

					default:
					{
						throw new Exception(
							"Requests of type " + request.RequestedOperation + " are not supported by this service.");
					}
				}
			}
			catch (Exception exc)
			{
				logger.Log(this, "Could not complete remote call : " + exc.ToString()); 
				
				response = new Protocol.Response(false, exc.ToString());
			}

			// logger.Log(this, "RESPONSE:\n" + response.ToString());

			return new CMS.Base.AnyMessage(response);
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
