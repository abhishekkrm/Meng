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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Devices_1_
{
	public interface IDeviceCollection : ICommunicationsDevice
	{
		void Register(Base1_.Subnet subnet, ICommunicationsDevice device);

		Base1_.Subnet DefaultSubnet
		{
			get;
			set;
		}
	}

	public class DeviceCollection : IDeviceCollection
	{
		public DeviceCollection()
		{
		}

		public DeviceCollection(Base1_.Subnet primarySubnet, ICommunicationsDevice communicationsDevice)
		{
			((IDeviceCollection)this).Register(primarySubnet, communicationsDevice);
			((IDeviceCollection)this).DefaultSubnet = primarySubnet;
		}

		private System.Collections.Generic.IDictionary<Base1_.Subnet, ICommunicationsDevice> devices =
			new System.Collections.Generic.Dictionary<Base1_.Subnet, ICommunicationsDevice>();
		private System.Collections.Generic.IDictionary<System.Net.IPAddress, Base1_.Subnet> addressCache =
			new System.Collections.Generic.Dictionary<System.Net.IPAddress, Base1_.Subnet>();
		private System.Nullable<Base1_.Subnet> defaultSubnet = null;		

		#region IDeviceCollection Members

		void IDeviceCollection.Register(QS._qss_c_.Base1_.Subnet subnet, ICommunicationsDevice device)
		{
			devices[subnet] = device;
		}

		Base1_.Subnet IDeviceCollection.DefaultSubnet
		{
			get 
			{
				if (defaultSubnet.HasValue)
					return defaultSubnet.Value;
				else
					throw new Exception("Default subnet not set.");
			}
			set { defaultSubnet = value; }
		}

		#endregion

		#region IReceivingDevice Members

		void IReceivingDevice.registerOnReceiveCallback(OnReceiveCallback callback)
		{
			foreach (ICommunicationsDevice device in devices.Values)
				device.registerOnReceiveCallback(callback);
		}

		#endregion

		#region IUnicastingDevice Members

		int IUnicastingDevice.PortNumber
		{
			get 
			{
				if (defaultSubnet.HasValue && devices.ContainsKey(defaultSubnet.Value))
					return devices[defaultSubnet.Value].PortNumber;
				else
					throw new Exception("Cannot determine port number.");
			}
		}

		void IUnicastingDevice.unicast(System.Net.IPAddress receiverAddress, int receiverPortNo, byte[] buffer, int offset, int bufferSize)
		{
			System.Nullable<Base1_.Subnet> assignedSubnet;

			lock (this)
			{
				if (!addressCache.ContainsKey(receiverAddress))
				{
					assignedSubnet = null;
					foreach (Base1_.Subnet subnet in devices.Keys)
					{
						if (subnet.contains(receiverAddress))
						{
							assignedSubnet = subnet;
							break;
						}
					}

					if (!assignedSubnet.HasValue)
					{
						if (defaultSubnet.HasValue && devices.ContainsKey(defaultSubnet.Value))
							assignedSubnet = defaultSubnet.Value;
						else
							throw new Exception("Cannot send to the destination address: cannot find the appropriate subnet.");
					}
				}
				else
				{
					assignedSubnet = addressCache[receiverAddress];
				}
			}

			devices[assignedSubnet.Value].unicast(receiverAddress, receiverPortNo, buffer, offset, bufferSize);
		}

		#endregion
	}
}
