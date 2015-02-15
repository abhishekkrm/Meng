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

﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Reflection;

namespace QS._qss_d_.Agent_
{
	public class Service : IDisposable
	{
		public static string GenerateURL(string hostname)
		{
			return "http://" + hostname + ":" + PortNo + "/" + URL;
		}

		private const int PortNo = 10661;
//		private const string DiscoveryGroupAddress = "224.0.33.33";
//		private const int DiscoveryPortNo = 10662;
		private const string URL = "QuickSilver_Agent.soap";
		private static readonly string LogFile = Components_.Logging.DefaultPath + "QuickSilver_Agent.errorlog";
        private static readonly string CryptographicKeyPath = Components_.Logging.DefaultPath + "SecretKey.dat";

		private static readonly QS.Fx.Network.NetworkAddress DiscoveryAddress =
			// new QS.Fx.Network.NetworkAddress(IPAddress.Parse(DiscoveryGroupAddress), DiscoveryPortNo);
			new QS.Fx.Network.NetworkAddress("224.0.33.33:10662");

		public static IEnumerable<IPAddress> DiscoverAgents(TimeSpan timeout)
		{
			return DiscoverAgents(null, timeout);
		}

		public static IEnumerable<IPAddress> DiscoverAgents(QS._qss_c_.Base1_.Subnet subnet, TimeSpan timeout)
		{
			return DiscoverAgents(new QS._qss_c_.Base1_.Subnet[] { subnet }, timeout);
		}

		public static IEnumerable<IPAddress> DiscoverAgents(IEnumerable<QS._qss_c_.Base1_.Subnet> subnets, TimeSpan timeout)
		{
			IEnumerable<IPAddress> addresses;

			using (Components_.EchoCollector echoCollector = new Components_.EchoCollector(DiscoveryAddress, subnets))
			{
				System.Threading.Thread.Sleep(timeout);
				addresses = echoCollector.Addresses;
			}

			return addresses;
		}

		public Service()
		{
			logger = new Components_.FileLogger();
			logger.Log(this, "Starting.");

			try
			{
				echoAgent = new QS._qss_d_.Components_.EchoAgent(DiscoveryAddress);
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}

			QS._qss_c_.Remoting_.Channels.CustomizableChannels.Initialize(PortNo, CryptographicKeyPath);

/*
			worker = new Worker(new Components.FileLogger(LogFile))); 
*/

            MarshalByRefObject serverObject = (agent = new Agent());
            RemotingServices.Marshal(serverObject, URL);
		}

		private QS.Fx.Logging.ILogger logger;
		private Components_.EchoAgent echoAgent;
        private Agent agent;
/*
		private Worker worker;
*/ 

/*
        #region Struct Request

        [Serializable]
        public struct Request : System.Runtime.Serialization.ISerializable
        {
            public Request(string methodName, object[] arguments)
            {
                this.methodName = methodName;
                this.arguments = arguments;
            }

            public string methodName;
            public object[] arguments;

            #region Accessors

            public string MethodName
            {
                get { return methodName; }
            }

            public object[] Arguments
            {
                get { return arguments; }
            }

            #endregion

            #region ISerializable Members

            void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                info.AddValue("MethodName", methodName);
                
            }

            #endregion
        }

        #endregion

        #region Worker

        public interface IWorker
		{
//			System.Runtime.Remoting.Messaging.IMethodReturnMessage Call(
//				System.Runtime.Remoting.Messaging.IMethodCallMessage message);


			byte[] Call(byte[] request);
		}

		public class Worker : MarshalByRefObject, IDisposable, IWorker
		{
			public Worker(QS.Fx.Logging.ILogger logger)
			{
				this.logger = logger;
				logger.Log(this, "Initialization Complete.");
				agent = new Agent();
			}

			private Agent agent;
			private QS.Fx.Logging.ILogger logger;

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				try
				{
					((IDisposable)agent).Dispose();
				}
				catch (Exception)
				{
				}
			}

			#endregion

			#region IWorker Members

			byte[] IWorker.Call(byte[] request_bytes)
                // string methodName, object[] arguments)
			{
                Service.Request request = Helpers.Serialization.Deserialize<Service.Request>(request_bytes);

				MethodInfo methodInfo = typeof(IAgent).GetMethod(request.MethodName);
				if (methodInfo == null)
					throw new ArgumentException("Could not find method \"" + request.MethodName + "\".");

				object response_object = methodInfo.Invoke(agent, request.Arguments);

                return Helpers.Serialization.ToBytes(Helpers.Serialization.Serialize(response_object));
			}

//			System.Runtime.Remoting.Messaging.IMethodReturnMessage IWorker.Call(
//				System.Runtime.Remoting.Messaging.IMethodCallMessage message)
//			{
//				logger.Log(this, "_____Foo");
//				// logger.Log(this, message.ToString());
//				return RemotingServices.ExecuteMessage(agent, message);				
//			}


			#endregion
		}

		#endregion
*/

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			try
			{
				logger.Log(this, "Shutting down.");
			}
			catch (Exception)
			{
			}			

			((IDisposable) echoAgent).Dispose();
            ((IDisposable) agent).Dispose();
/*
			((IDisposable) worker).Dispose();
*/

			QS._qss_c_.Remoting_.Channels.CustomizableChannels.Cleanup();
		}

		#endregion
	}
}
