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

// #define DEBUG_DirectMulticastingDevice

using System;
using System.IO;

namespace QS._qss_c_.Multicasting_1_
{
/*
	/// <summary>
	/// Summary description for MulticastingSender.
	/// </summary>
	public class DirectMulticastingDevice : IMulticastingDevice
	{
		private static TimeSpan sendingTimeout = TimeSpan.FromMilliseconds(100);

		public DirectMulticastingDevice(Base.ISender underlyingSender, QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger)
		{
			this.underlyingSender = underlyingSender;
			this.logger = logger;
			this.alarmClock = alarmClock;
		}

		private Base.ISender underlyingSender;
		private QS.Fx.Logging.ILogger logger;
		private QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock;

		private void alarmCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
		{
			MulticastRequestRef requestRef = (MulticastRequestRef) alarmRef.Context;

#if DEBUG_DirectMulticastingDevice
			logger.Log(this, "resending : " + requestRef.ToString());
#endif

			if (requestRef.resend(underlyingSender))
				alarmRef.Reschedule();
		}

		#region IMulticastingSender Members

		public IMulticastRequestRef multicast(Base.IClient source, IAddressSet destinationAddresses, uint destinationLOID, 
			Base.IMessage message, MulticastRequestCallback callback)
		{
#if DEBUG_DirectMulticastingDevice
			logger.Log(this, "called multicast(address = " + destinationAddresses.ToString() + ", targetLOID : " + destinationLOID + ", message : " + message.ToString() + ")"); 
#endif

			MulticastRequestRef requestRef = new MulticastRequestRef(source, destinationAddresses, destinationLOID, message, callback);

			requestRef.multicast(underlyingSender);

			lock (requestRef)
			{
				requestRef.AlarmRef = alarmClock.Schedule(
					sendingTimeout.TotalSeconds, new QS.Fx.QS.Fx.Clock.AlarmCallback(this.alarmCallback), requestRef);
			}

			return requestRef;
		}

		#endregion

		private class MulticastRequestRef : IMulticastRequestRef
		{
			public MulticastRequestRef(Base.IClient source, IAddressSet addresses, uint destinationLOID, 
				Base.IMessage message, MulticastRequestCallback callback)
			{
				this.source = source;
				this.addresses = addresses;
				this.destinationLOID = destinationLOID;
				this.message = message;
				this.callback = callback;

				this.acknowledged = new bool[addresses.Count];
				this.excluded = new bool[addresses.Count];
				for (uint ind = 0; ind < addresses.Count; ind++)
					acknowledged[ind] = excluded[ind] = false;
				this.numberOfPendingACKs = addresses.Count;

				this.cancelled = false;
				this.alarmRef = null;
			}

			private Base.IClient source;
			private IAddressSet addresses;
			private uint destinationLOID;
			private Base.IMessage message;
			private MulticastRequestCallback callback;
			private bool cancelled;
			private uint numberOfPendingACKs;
			private bool[] acknowledged;
			private bool[] excluded;
			private QS.Fx.QS.Fx.Clock.IAlarm alarmRef;

			public void multicast(Base.ISender underlyingSender)
			{
				for (uint ind = 0; ind < addresses.Count; ind++)
				{
					underlyingSender.send(source, new Base.ObjectAddress(addresses[ind], destinationLOID), message, null);
				}
			}

			public bool resend(Base.ISender underlyingSender)
			{
				lock (this)
				{
					if (!cancelled && !AllConfirmed)
					{
						for (uint ind = 0; ind < addresses.Count; ind++)
						{
							if (pending(ind))
							{
								underlyingSender.send(source, new Base.ObjectAddress(addresses[ind], destinationLOID), message, null);
							}
						}

						return true;
					}
					else
						return false;
				}

			}

			public QS.Fx.QS.Fx.Clock.IAlarm AlarmRef
			{
				set
				{
					alarmRef = value;
				}

				get
				{
					return alarmRef;
				}
			}

			private bool AllConfirmed
			{
				get
				{
					return numberOfPendingACKs == 0;
				}
			}

//			public bool Cancelled
//			{
//				get
//				{
//					return cancelled;
//				}
//			}

			private bool pending(uint indexOf)
			{
				return !acknowledged[indexOf] && !excluded[indexOf];
			}

			private void notifyAllConfirmed()
			{
				this.callback(this, true);
			}

			#region IMulticastRequestRef Members

			public void confirm(uint indexOf)
			{
				if (pending(indexOf))
					numberOfPendingACKs--;
				acknowledged[indexOf] = true;
				if (AllConfirmed)
					notifyAllConfirmed();
			}

			public void exclude(uint indexOf)
			{
				if (pending(indexOf))
					numberOfPendingACKs--;
				excluded[indexOf] = true;
				if (AllConfirmed)
					notifyAllConfirmed();
			}

			public void cancel()
			{
				lock (this)
				{
					if (alarmRef != null)
						alarmRef.Cancel();

					cancelled = true;
				}
			}

//			public IAddressSet Addresses
//			{
//				get
//				{
//					return addresses;
//				}
//			}

			public QS.CMS.Base.IMessage Message
			{
				get
				{
					return message;
				}
			}

			#endregion
		}
	}
*/
}
