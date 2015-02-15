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

namespace QS._qss_e_.Tests_.Test105
{
	/// <summary>
	/// Works with experiment 006.
	/// </summary>
	public class MainApp : System.IDisposable
	{
        public MainApp(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.platform = platform;

			IPAddress localIPAddress;
			if (args.contains("base"))
				localIPAddress = IPAddress.Parse((string) args["base"]);
			else if (args.contains("subnet"))
				localIPAddress = QS._qss_c_.Devices_2_.Network.AnyAddressOn(
					new QS._qss_c_.Base1_.Subnet((string) args["subnet"]), platform);
			else 
				localIPAddress = platform.NICs[0];
			localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, 12022);

			platform.Logger.Log(null, "Base Address Chosen : " + localAddress.ToString());

			platform.UDPDevice.listenAt(localIPAddress, localAddress, 
				new QS._qss_c_.Devices_2_.OnReceiveCallback(this.receiveCallback));

			sendingSourceAddress = new QS.Fx.Network.NetworkAddress(localAddress.HostIPAddress, 0);
		}

        private QS._qss_c_.Platform_.IPlatform platform;
		private QS.Fx.Network.NetworkAddress localAddress, sendingSourceAddress;

		private bool is_sender = false;		
		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			// platform.Logger.Log(this, "__receiveCallback : " + sourceAddress.ToString() + " " +
			//	destinationAddress.ToString() + " " + blockOfData.SizeOfData.ToString());

			if (is_sender)
			{
				// uint seqno = CMS.Base2.Serializer.loadUInt32(blockOfData);
				// platform.Logger.Log(this, "__acknowledged : " + sourceAddress.ToString() + " " + seqno.ToString());

				lock (this)
				{
					if (numberAcknowledged < numberOfMessagesToSend)
					{
						acknowledgementTimes[numberAcknowledged++] = platform.Clock.Time;
					}

					if (numberAcknowledged == numberOfMessagesToSend)
					{
						System.Text.StringBuilder output = new System.Text.StringBuilder();
						for (uint ind = 0; ind < numberOfMessagesToSend; ind++)
						{
							output.Append(sendingTimes[ind].ToString() + "\t" + 
								acknowledgementTimes[ind].ToString() + "\n");
						}
						platform.Logger.Log(output.ToString());
					}
				}

				sendOne();
			}
			else
			{
				// platform.Logger.writeLine("sending ack");
				platform.UDPDevice.sendto(sendingSourceAddress, 
					new QS.Fx.Network.NetworkAddress(sourceAddress.HostIPAddress, 12022), blockOfData);
			}
		}

		private uint numberSent, numberAcknowledged, numberOfMessagesToSend, windowSize;
		private double[] sendingTimes;
		private double[] acknowledgementTimes;

		private IPAddress destination;
		public void send(IPAddress destination, string numberOfMessagesToSend_AsString, string windowSize_AsString)		
		{
			this.destination = destination;
			this.is_sender = true;

			numberSent = numberAcknowledged = 0; 
			this.numberOfMessagesToSend = Convert.ToUInt32(numberOfMessagesToSend_AsString);
			sendingTimes = new double[numberOfMessagesToSend];
			acknowledgementTimes = new double[numberOfMessagesToSend];
			this.windowSize = Convert.ToUInt32(windowSize_AsString);

			for (uint ind = 0; ind < windowSize; ind++)
				sendOne();
		}

		private void sendOne()
		{
			bool should_send;
			uint seqno = 666;

			lock (this)
			{
				if (should_send = (numberSent < numberOfMessagesToSend))
				seqno = numberSent++;
			}

			if (should_send)
			{
				sendingTimes[seqno] = platform.Clock.Time;

				QS._core_c_.Base2.BlockOfData blockOfData = new QS._core_c_.Base2.BlockOfData(QS._core_c_.Base2.SizeOf.UInt32);
				QS._core_c_.Base2.Serializer.saveUInt32(seqno, blockOfData);
				blockOfData.resetCursor();
				platform.UDPDevice.sendto(sendingSourceAddress, 
					new QS.Fx.Network.NetworkAddress(destination, 12022), blockOfData);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
