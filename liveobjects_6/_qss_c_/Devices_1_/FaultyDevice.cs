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

// #define DEBUG_FaultyDevice

using System;

namespace QS._qss_c_.Devices_1_
{
	/// <summary>
	/// Summary description for FaultyDevice.
	/// </summary>
	public class FaultyDevice : IUnicastingDevice
	{
		public FaultyDevice(IUnicastingDevice underlyingUnicastingDevice, QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IAlarmClock alarmClock,
			double lossRate, double duplicationRate, double maximumDelayInMilliseconds)
		{
			this.logger = logger;
			this.underlyingUnicastingDevice = underlyingUnicastingDevice;
			this.alarmClock = alarmClock;
			this.lossRate = lossRate;
			this.maximumDelayInMilliseconds = maximumDelayInMilliseconds;
			this.duplicationRate = duplicationRate ;

			this.random = new System.Random();
		}

		private IUnicastingDevice underlyingUnicastingDevice;
		private QS.Fx.Logging.ILogger logger;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private double lossRate, duplicationRate, maximumDelayInMilliseconds;
		private System.Random random;

		private void alarmCallback(QS.Fx.Clock.IAlarm alarmRef)
		{
			Packet packet = (Packet) alarmRef.Context;
			underlyingUnicastingDevice.unicast(
				packet.receiverAddress, packet.receiverPortNo, packet.buffer, packet.offset, packet.bufferSize);
		}

		private class Packet
		{
			public Packet(System.Net.IPAddress receiverAddress, int receiverPortNo, byte[] buffer, int offset, int bufferSize)
			{
				this.receiverAddress = receiverAddress;
				this.receiverPortNo = receiverPortNo;
				this.buffer = buffer;
				this.offset = offset;
				this.bufferSize = bufferSize;
			}

			public System.Net.IPAddress receiverAddress;
			public int receiverPortNo, offset, bufferSize;
			public byte[] buffer; 
		}

		#region IUnicastingDevice Members

		public int PortNumber
		{
			get
			{
				return underlyingUnicastingDevice.PortNumber;
			}
		}

		public void unicast(System.Net.IPAddress receiverAddress, int receiverPortNo, byte[] buffer, int offset, int bufferSize)
		{
			if (random.NextDouble() < lossRate)
			{
#if DEBUG_FaultyDevice
				logger.Log(this, "packet lost in the network");
#endif
				return;
			}
			
			while (true)
			{
				Packet packet = new Packet(receiverAddress, receiverPortNo, buffer, offset, bufferSize);

				alarmClock.Schedule(random.NextDouble() * maximumDelayInMilliseconds * 0.001, 
					new QS.Fx.Clock.AlarmCallback(this.alarmCallback), packet);

				if (random.NextDouble() < duplicationRate)
				{
#if DEBUG_FaultyDevice
					logger.Log(this, "packet duplicated in the network");
#endif
				}
				else
					break;
			}
		}

		#endregion
	}
}
