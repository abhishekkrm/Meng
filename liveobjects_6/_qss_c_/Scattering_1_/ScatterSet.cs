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

// #define DEBUG_ScatterSet

using System;
using System.Threading;

namespace QS._qss_c_.Scattering_1_
{
	/// <summary>
	/// Summary description for ScatterSet.
	/// </summary>
	public class ScatterSet : IScatterSet, Scattering_1_.IScatterAddress, System.Collections.ICollection
	{
		public ScatterSet(QS.Fx.Network.NetworkAddress[] destinationAddresses) : this((uint) destinationAddresses.Length)
		{
			foreach (QS.Fx.Network.NetworkAddress address in destinationAddresses)
				this.insert(new ReceiverStatus(address));
		}

		protected ScatterSet(uint numberOfReceivers)
		{			
			receivers = new Collections_1_.LinkableHashSet(numberOfReceivers);
			toscatter = new Collections_1_.BiLinkableCollection();

			this.numberOfReceivers = numberOfReceivers;
			this.numberAcknowledged = this.numberCrashed = 0;
		}

		protected void insert(IReceiverStatus receiverStatus)
		{
			receivers.insert(receiverStatus);
			toscatter.insertAtTail(receiverStatus);
		}

#if DEBUG_ScatterSet
		private QS.Fx.Logging.ILogger logger;

		public QS.Fx.Logging.ILogger Logger
		{
			set
			{
				this.logger = value;
			}
		}

		public override string ToString()
		{
			return "ScatterSet:" + this.GetType().Name + "(acks:" + numberAcknowledged.ToString() + 
				",dead:" + numberCrashed.ToString() + ",size:" + numberOfReceivers.ToString() + ")";
		}
#endif

		private QS._qss_c_.Scattering_1_.CompletionCallback completionCallback = null;
		private bool completed = false;
		private Collections_1_.IBiLinkableCollection toscatter;
		private Collections_1_.ILinkableHashSet receivers;

		protected QS._qss_c_.Scattering_1_.IScatterAddress scatterAddress = null;
		protected bool succeeded = true;
		protected System.Exception error = null;
		protected uint numberOfReceivers, numberAcknowledged, numberCrashed;

		#region Calculating scatter address

		private void calculateScatterAddress()
		{
			this.scatterAddress = this;
		}

		#endregion

		#region Checking completion

		protected virtual void signalCompletion()
		{
			if (completionCallback != null)
				completionCallback(succeeded, error);
		}

		private bool receiversChanged()
		{
            bool completed_thistime = false;

            lock (this)
			{
				if (!completed)
				{
					completed = this.Completed;

					if (completed)
					{
                        completed_thistime = true;
                        this.signalCompletion();
					}
					else
						this.scatterAddress = null;
				}
			}

            return completed_thistime;
        }

		protected virtual bool Completed
		{	
			get
			{
				return numberOfReceivers <= numberAcknowledged + numberCrashed;
			}
		}

		#endregion

		#region Interface IReceiverStatus

		protected interface IReceiverStatus : Collections_1_.ITriLinkable
		{
			QS.Fx.Network.NetworkAddress Address
			{
				get;
			}

			bool Acknowledged
			{
				set;
				get;
			}

			bool Crashed
			{
				set;
				get;
			}
		}

		#endregion

		#region Class ReceiverStatus

		protected class ReceiverStatus : Collections_1_.GenericTriLinkable, IReceiverStatus
		{
			public ReceiverStatus(QS.Fx.Network.NetworkAddress networkAddress)
			{
				this.networkAddress = networkAddress;
			}

			protected QS.Fx.Network.NetworkAddress networkAddress;
			protected bool acknowledged, crashed;

			#region IReceiverStatus Members

			public QS.Fx.Network.NetworkAddress Address
			{
				get
				{
					return networkAddress;
				}
			}

			public bool Acknowledged
			{
				get
				{
					return acknowledged;
				}
				set
				{
                    acknowledged = value;
                }
			}

			public bool Crashed
			{
				get
				{
					return crashed;
				}
				set
				{
                    crashed = value;
                }
			}

			#endregion

			#region ILinkable Members

			public override object Contents
			{
				get
				{
					return this.networkAddress;
				}
			}

			#endregion
		}

		#endregion

		#region Helpers

		private IReceiverStatus statusOf(QS.Fx.Network.NetworkAddress address)
		{
			lock (receivers)
			{
				IReceiverStatus receiver = (IReceiverStatus) receivers.lookup(address);
				if (receiver != null)
					Monitor.Enter(receiver);
				return receiver;
			}
		}

		#endregion

		#region IScatterSet Members

		public void reset()
		{
			lock (this)
			{
				foreach (ReceiverStatus receiverStatus in receivers.Elements)
				{
					if (receiverStatus.Acknowledged && !receiverStatus.Crashed)
					{
						toscatter.insertAtTail(receiverStatus);
                        receiverStatus.Acknowledged = false;
                    }
				}

				numberAcknowledged = 0;
				completed = false;
			}
		}

		public System.Collections.ICollection AllPending
		{
			get
			{
				return this.toscatter;
			}
		}

		public QS._qss_c_.Scattering_1_.IScatterAddress ScatterAddress
		{
			get
			{
				lock (this)
				{
					if (completed)
						return null;
					else
					{
						if (scatterAddress == null)
							calculateScatterAddress();
						return scatterAddress;
					}
				}
			}

			set
			{
				lock (this)
				{
					scatterAddress = value;
				}
			}
		}

		public bool acknowledged(QS.Fx.Network.NetworkAddress address)
		{
            bool completed_thistime = false;

#if DEBUG_ScatterSet
			logger.Log(this, "acknowledged_enter(" + address.ToString() + ")");
#endif

			IReceiverStatus receiver = this.statusOf(address);
            if (receiver != null)
            {
                bool updated = !receiver.Acknowledged && !receiver.Crashed;
                receiver.Acknowledged = true;
                Monitor.Exit(receiver);

                if (updated)
                {
                    lock (toscatter)
                    {
                        toscatter.remove(receiver);
                    }

                    numberAcknowledged++;

                    completed_thistime = receiversChanged();
                }
            }

#if DEBUG_ScatterSet
			logger.Log(this, "acknowledged_leave(" + address.ToString() + ")\n" + this.ToString());
#endif

            return completed_thistime;
        }

		public void crashed(QS.Fx.Network.NetworkAddress address)
		{		
			IReceiverStatus receiver = this.statusOf(address);
			if (receiver != null)
			{
				bool updated = !receiver.Crashed && !receiver.Acknowledged;
				receiver.Crashed = true;
				Monitor.Exit(receiver);

				if (updated)
				{
					lock (toscatter)
					{
						toscatter.remove(receiver);
					}

					numberCrashed++;
					receiversChanged();
				}
			}
		}

		public void registerCallback(QS._qss_c_.Scattering_1_.CompletionCallback completionCallback)
		{
			lock (this)
			{
				this.completionCallback = completionCallback;
				if (completed)
					completionCallback(succeeded, error);
			}
		}

		#endregion

		#region IScatterAddress Members

		public QS._qss_c_.Scattering_1_.AddressClass AddressClass
		{
			get
			{
				return Scattering_1_.AddressClass.ADDRESS_COLLECTION;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return toscatter.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return toscatter.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new Exception("not supported");
		}

		public object SyncRoot
		{
			get
			{
				return toscatter;
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Components_1_.WrapperEnumerator(toscatter.GetEnumerator(), unwrapCallback);
		}

		#endregion

		private static Components_1_.UnwrapCallback unwrapCallback = new Components_1_.UnwrapCallback(extractAddress);
		private static object extractAddress(object wrappedObject)
		{
			return ((IReceiverStatus) wrappedObject).Address;
		}
	}
}
