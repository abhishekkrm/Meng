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

// #define DEBUG_EchoAgent

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_d_.Components_
{
	public class EchoAgent : System.IDisposable
	{
		public EchoAgent(QS.Fx.Network.NetworkAddress discoveryAddress) : this(discoveryAddress, null)
		{
		}

		public EchoAgent(QS.Fx.Network.NetworkAddress discoveryAddress, QS.Fx.Logging.ILogger logger)
		{
			if (logger == null)
				logger = new Components_.FileLogger();
			this.logger = logger;

			foreach (QS._qss_c_.Devices_3_.INetworkInterface networkInterface in
				((QS._qss_c_.Devices_3_.INetwork)(new QS._qss_c_.Devices_3_.Network(logger))).NICs)
			{
				Echo echo = new Echo(networkInterface, discoveryAddress, logger);
				echos.Add(echo);
			}
		}

		private QS.Fx.Logging.ILogger logger;
		private List<Echo> echos = new List<Echo>();

		#region Class Echo

		private class Echo : QS._qss_c_.Devices_3_.IReceiver, System.IDisposable
		{
			public Echo(QS._qss_c_.Devices_3_.INetworkInterface networkInterface, QS.Fx.Network.NetworkAddress address,
				QS.Fx.Logging.ILogger logger)
			{
				this.logger = logger;
				this.networkInterface = networkInterface;
				this.listener = networkInterface[QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP].ListenAt(address, this);

#if DEBUG_EchoAgent
				logger.Log(this, "Listening at " + networkInterface.Address.ToString() + " for " + 
					listener.Address.ToString());
#endif
			}

			private QS.Fx.Logging.ILogger logger;
			private QS._qss_c_.Devices_3_.INetworkInterface networkInterface;
			private QS._qss_c_.Devices_3_.IListener listener;

			#region IReceiver Members

			void QS._qss_c_.Devices_3_.IReceiver.receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
			{
				try
				{
#if DEBUG_EchoAgent
					logger.Log(this, "Received at " + networkInterface.Address.ToString() + " from " +
						sourceAddress.ToString());
#endif

					QS.Fx.Network.NetworkAddress responseAddress = new QS.Fx.Network.NetworkAddress();
                    QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(data);
                    QS.Fx.Base.ConsumableBlock dummy = new QS.Fx.Base.ConsumableBlock();
					responseAddress.DeserializeFrom(ref header, ref dummy);

#if DEBUG_EchoAgent
					logger.Log(this, "Responding to " + responseAddress.ToString());
#endif

					networkInterface[QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP].GetSender(responseAddress).send(
                        QS._qss_c_.Helpers_.ListOf<QS.Fx.Base.Block>.Singleton(new QS.Fx.Base.Block(new byte[1], 0, 1)));
				}
				catch (Exception)
				{
				}
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				try
				{
					listener.Dispose();
				}
				catch (Exception)
				{
				}
			}

			#endregion
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (Echo echo in echos)
				((IDisposable)echo).Dispose();
		}

		#endregion
	}
}
