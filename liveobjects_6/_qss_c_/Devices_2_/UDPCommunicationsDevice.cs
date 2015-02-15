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
using System.Net.Sockets;

namespace QS._qss_c_.Devices_2_
{
	/// <summary>
	/// Summary description for UDPUnicastingDevice.
	/// </summary>
	public class UDPCommunicationsDevice : UDPSimpleSender, ICommunicationsDevice
	{
		private const uint defaultAnticipatedNumberOfListeners = 100;

		public UDPCommunicationsDevice(QS.Fx.Logging.ILogger logger) : base(logger)
		{
			listeners = new QS._core_c_.Collections.Hashtable(defaultAnticipatedNumberOfListeners);
		}

		private QS._core_c_.Collections.IDictionary listeners;

		private class Listener : IListener
		{
			public Listener(IPAddress localAddress, QS.Fx.Network.NetworkAddress receivingAddress, Devices_2_.OnReceiveCallback receiveCallback,
				UDPCommunicationsDevice encapsulatingUDPDevice)				
			{
				this.encapsulatingUDPDevice = encapsulatingUDPDevice;
				this.udpReceiver = new UDPReceiver(localAddress, receivingAddress, encapsulatingUDPDevice.logger, false, receiveCallback);
			}

			private UDPCommunicationsDevice encapsulatingUDPDevice;
			private UDPReceiver udpReceiver;

			public void shutdownUDPReceiver()
			{
				udpReceiver.shutdown();
			}

			#region IListener Members

			public void shutdown()
			{
				shutdownUDPReceiver();
				lock (encapsulatingUDPDevice.listeners)
				{
					encapsulatingUDPDevice.listeners.remove(udpReceiver.Address);
				}
			}

			public QS.Fx.Network.NetworkAddress Address
			{
				get
				{
					return udpReceiver.Address;
				}
			}

			#endregion
		}

		#region ICommunicationsDevice Members

		public IListener listenAt(IPAddress localAddress, QS.Fx.Network.NetworkAddress receivingAddress, QS._qss_c_.Devices_2_.OnReceiveCallback receiveCallback)
		{
			IListener listener = null;

			lock (listeners)
			{
				QS._core_c_.Collections.IDictionaryEntry dic_en = listeners.lookupOrCreate(receivingAddress);
				if (dic_en.Value == null)
				{
					// listener = new UDPReceiver(receivingAddress, logger, true, receiveCallback);
					listener = new Listener(localAddress, receivingAddress, receiveCallback, this);
					dic_en.Value = listener;
				}
				else
				{
					throw new Exception("this address is already taken");
				}
			}

			return listener;
		}

		public void shutdown()
		{
			lock (listeners)
			{
				foreach (Listener listener in listeners.Values)
				{
					listener.shutdownUDPReceiver();
					listeners.remove(listener);
				}
			}
		}

		#endregion
	}
}
