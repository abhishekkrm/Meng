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
using System.Xml.Serialization;
using System.Net;

namespace QS._qss_d_.Console_
{
	[Serializable]
	[XmlType("ConsoleConfiguration")]
	public class Configuration
	{
		public Configuration()
		{
			hosts = new QS._core_c_.Collections.Hashtable(20);	
		}

		[XmlIgnore] 
		public QS._core_c_.Collections.Hashtable hosts;

		[Serializable]
		public class HostDescription // : System.Runtime.Serialization.ISerializable
		{
			public HostDescription()
			{
			}

			public HostDescription(IPAddress address, Service_1_.Configuration configuration)
			{
				this.address = address;
				this.configuration = configuration;
				this.locallyAssignedAlias = "";
			}

/*
			public HostDescription(System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context)
			{
				address = IPAddress.Parse(info.GetString("Address"));
				configuration = (Service.Configuration) info.GetValue("Configuration", typeof(Service.Configuration));
			}

			#region System.Runtime.Serialization.ISerializable Members

			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,
				System.Runtime.Serialization.StreamingContext context)
			{
				info.AddValue("Address", address.ToString());
				info.AddValue("Configuration", configuration);
			}

			#endregion
*/

			public override string ToString()
			{
				return address.ToString() + 
					((locallyAssignedAlias != null && locallyAssignedAlias.Length > 0) 
					? (" (" + locallyAssignedAlias + ")") : "");
			}

			public string IPAddressAsString
			{
				get
				{
					return address.ToString();
				}

				set
				{
					address = IPAddress.Parse(value);
				}
			}

			[XmlIgnore] public bool responding = false;
			[XmlIgnore] public IPAddress address;			
			public Service_1_.Configuration configuration;

			[XmlAttribute("LocalAlias")]
			public string locallyAssignedAlias;
		}

		public HostDescription[] Hosts
		{
			get
			{
				object[] theValues = hosts.Values;
				HostDescription[] result = new HostDescription[theValues.Length];
				for (int ind = 0; ind < theValues.Length; ind++)
					result[ind] = (HostDescription) theValues[ind]; 
				return result;
			}

			set
			{
				if (value != null)
				{
					foreach (HostDescription host in value)
						hosts[host.address] = host;
				}
			}
		}
	}
}
