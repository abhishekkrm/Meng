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
using System.Text;
using System.Xml.Serialization;

namespace QS._qss_d_.Agent_
{
	public static class Client
	{
/*
		public static void Initialize(string cryptographicKeyPath)
		{
			Initialize(QS.CMS.Remoting.Encryption.Keys.Load(cryptographicKeyPath));
		}

		public static void Initialize(byte[] cryptographicKey)
		{
			lock (typeof(Client))
			{
				if (channel != null)
					throw new Exception("Already initialized.");

				ChannelServices.RegisterChannel(
					channel = QS.CMS.Remoting.Channels.CustomizableChannels.CreateChannel(
					0, Service.ChannelOption, QS.CMS.Remoting.Encryption.Keys.AlgorithmName(Service.EncryptionAlgorithm),
					cryptographicKey));
			}
		}

		private static HttpChannel channel = null;
*/

		public static IAgent GetAgent(string hostname)
		{
            return (IAgent) Activator.GetObject(typeof(IAgent), Service.GenerateURL(hostname));
/*
			return (IAgent) ((new Proxy(hostname)).GetTransparentProxy());
*/ 
		}

/*
		#region Proxies

		private class Proxy : System.Runtime.Remoting.Proxies.RealProxy
		{
			public Proxy(string hostname) : base(typeof(Agent))
			{
				worker = (Service.IWorker) Activator.GetObject(typeof(Service.IWorker), Service.GenerateURL(hostname));
			}

			private Service.IWorker worker;

			public override System.Runtime.Remoting.Messaging.IMessage Invoke(
				System.Runtime.Remoting.Messaging.IMessage msg)
			{
				System.Runtime.Remoting.Messaging.IMethodCallMessage methodCall =
					(System.Runtime.Remoting.Messaging.IMethodCallMessage) msg;

				if (methodCall.InArgCount != methodCall.ArgCount)
					throw new ArgumentException("Outgoing arguments are not supported.");

				try
				{
                    object response = Helpers.Serialization.Deserialize(worker.Call(
                        Helpers.Serialization.ToBytes(Helpers.Serialization.Serialize<Service.Request>(new Service.Request(methodCall.MethodName, methodCall.Args)))));

					return new System.Runtime.Remoting.Messaging.ReturnMessage(
						response, null, 0, methodCall.LogicalCallContext, methodCall);
				}
				catch (Exception exc)
				{
					return new System.Runtime.Remoting.Messaging.ReturnMessage(exc, methodCall);
				}
			}
		}

		#endregion
*/ 
	}
}
