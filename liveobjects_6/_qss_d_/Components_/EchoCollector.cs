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

// #define DEBUG_EchoCollector

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_d_.Components_
{
	public class EchoCollector : System.IDisposable, QS._qss_c_.Devices_3_.IReceiver
	{
		public EchoCollector(QS.Fx.Network.NetworkAddress discoveryAddress)
			: this(discoveryAddress, null, null)
		{
		}

		public EchoCollector(QS.Fx.Network.NetworkAddress discoveryAddress, IEnumerable<QS._qss_c_.Base1_.Subnet> subnets) 
			: this(discoveryAddress, subnets, null)
		{
		}

		public EchoCollector(QS.Fx.Network.NetworkAddress discoveryAddress, QS.Fx.Logging.ILogger logger)
			: this(discoveryAddress, null, logger)
		{
		}

		public EchoCollector(QS.Fx.Network.NetworkAddress discoveryAddress, IEnumerable<QS._qss_c_.Base1_.Subnet> subnets, 
			QS.Fx.Logging.ILogger logger)
		{
			if (logger == null)
				logger = new Components_.FileLogger();
			this.logger = logger;

			foreach (QS._qss_c_.Devices_3_.INetworkInterface networkInterface in
				((QS._qss_c_.Devices_3_.INetwork)(new QS._qss_c_.Devices_3_.Network(logger))).NICs)
			{
				if (QS._qss_c_.Base1_.Subnet.OnSubnets(networkInterface.Address, subnets))
				{
					QS._qss_c_.Devices_3_.IListener listener =
						networkInterface[QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP].ListenAt(
						new QS.Fx.Network.NetworkAddress(networkInterface.Address, 0), this);

#if DEBUG_EchoCollector
					logger.Log(this, "Listening at " + networkInterface.Address.ToString() + " for " + 
						listener.Address.ToString());
#endif

					listeners.Add(listener);

					QS._qss_c_.Devices_3_.ISender sender =
						networkInterface[QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP].GetSender(discoveryAddress);

#if DEBUG_EchoCollector
					logger.Log(this, "Sending via " + networkInterface.Address.ToString() + " to " + 
						sender.Address.ToString());
#endif

					sender.send(QS._core_c_.Base3.Serializer.FlattenObject(listener.Address));
				}
			}
		}

		private QS.Fx.Logging.ILogger logger;
		private List<QS._qss_c_.Devices_3_.IListener> listeners = new List<QS._qss_c_.Devices_3_.IListener>();
		private System.Collections.Generic.ICollection<IPAddress> addresses =
			new System.Collections.ObjectModel.Collection<IPAddress>();

		public IEnumerable<IPAddress> Addresses
		{
			get { return addresses; }
		}

		#region IReceiver Members

		void QS._qss_c_.Devices_3_.IReceiver.receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
		{
			lock (this)
			{
				if (!addresses.Contains(sourceAddress.HostIPAddress))
					addresses.Add(sourceAddress.HostIPAddress);
			}
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (QS._qss_c_.Devices_3_.IListener listener in listeners)
				listener.Dispose();
		}

		#endregion
	}
}
