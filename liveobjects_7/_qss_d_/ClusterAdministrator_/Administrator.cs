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
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace QS._qss_d_.ClusterAdministrator_
{
	public class Administrator : MarshalByRefObject, IAdministrator, System.IDisposable
	{
		public static readonly TimeSpan DefaultDiscoveryTimeout = TimeSpan.FromSeconds(2);

		private const string ConfigurationFile = "Configuration.xml";
		private static readonly string ConfigurationPath = 
			Helpers_.File.ExtractPath(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
			"\\" + ConfigurationFile;

		public Administrator()
		{
			configuration = Configuration.Load(ConfigurationPath);
			((IAdministrator) this).Discover(DefaultDiscoveryTimeout);			
		}

		private Configuration configuration;
		private List<IPAddress> nodes;
		
		#region IAdministrator Members

		void IAdministrator.Discover(TimeSpan timeout)
		{
			lock (this)
			{
				nodes = new List<IPAddress>(Agent_.Service.DiscoverAgents(timeout));
			}
		}

		List<IPAddress> IAdministrator.Nodes
		{
			get { return nodes; }
		}

/*
		void IAdministrator.UpdateService(string serviceName)
		{
			lock (this)
			{
				foreach (IPAddress address in nodes)
				{
					Agent.IAgent agent = Agent.Client.GetAgent(address.ToString());
					agent.RestartService(serviceName, true, false, TimeSpan.FromSeconds(10));
					
			
			throw new Exception("The method or operation is not implemented.");
		}
*/

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
		}

		#endregion
}
}
