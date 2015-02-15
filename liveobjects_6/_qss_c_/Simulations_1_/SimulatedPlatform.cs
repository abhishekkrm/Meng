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
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace QS._qss_c_.Simulations_1_
{
	/// <summary>
	/// Summary description for SimulatedNode.
	/// </summary>
	public class SimulatedPlatform : Platform_.VirtualPlatform, QS.Fx.Clock.IAlarmClock, QS.Fx.Inspection.IInspectable, QS.Fx.Clock.IClock
	{
		private const double defaultAnticipatedContextSwitchingOverhead = 0.0001;

		public SimulatedPlatform(Virtualization_.INetwork[] networks, 
			Simulations_1_.SimulatedClock simulatedClock, QS.Fx.Clock.IClock physicalClock) : base(null, null, networks)
		{
			this.physicalClock = physicalClock;
            this.simulatedClock = simulatedClock;
            this.alarmClock = this;
			this.clock = this;
			this.enqueuedCallbacks = new Collections_1_.BiLinkableCollection();
			this.currentState = State.IDLING;
			this.switchingCallback = new QS.Fx.Clock.AlarmCallback(this.contextSwitchingCallback);
			this.interceptingAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.alarmArrived);

			this.contextSwitchingOverhead = defaultAnticipatedContextSwitchingOverhead;
		}

		protected enum State
		{
			IDLING, CONTEXT_SWITCHING, PROCESSING
		}

        protected Simulations_1_.SimulatedClock simulatedClock;
        protected QS.Fx.Clock.IClock physicalClock;
		protected State currentState;

		private Collections_1_.IBiLinkableCollection enqueuedCallbacks;
		private double contextSwitchingOverhead, processingBeginningTimeStamp;
		private QS.Fx.Clock.AlarmCallback switchingCallback, interceptingAlarmCallback;

		private double LocalTimeOffset
		{
			get
			{
				return physicalClock.Time - processingBeginningTimeStamp;
			}
		}

		protected override Virtualization_.VirtualCommunicationSubsystem.NetworkClient createClient(Virtualization_.INetwork network)
		{
			return new SimulatedClient(network, this);
		}

		#region Context Switching and Processing of Enqueued Callbacks

		private bool processEnqueuedCallbacks()
		{
			bool processingSomething = enqueuedCallbacks.Count > 0;

			if (processingSomething)
			{				
				currentState = State.PROCESSING;

				processingBeginningTimeStamp = physicalClock.Time;

				while (enqueuedCallbacks.Count > 0)
				{
					Collections_1_.IBiLinkable enqueuedCallback = enqueuedCallbacks.elementAtHead();
					enqueuedCallbacks.remove(enqueuedCallback);

					((IGenericWrapper) enqueuedCallback).dispatch();
				}

				((QS.Fx.Clock.IAlarmClock) simulatedClock).Schedule(this.LocalTimeOffset, this.switchingCallback, State.IDLING);					
			}
			else
			{
				currentState = State.IDLING;
			}

			return processingSomething;
		}

		private void contextSwitchingCallback(QS.Fx.Clock.IAlarm alarmRef)
		{
			lock (this)
			{
				// State requestedState = (State) alarmRef.Context;
				this.processEnqueuedCallbacks();
			}
		}

		#endregion

		#region Processing of Arriving Packets and Alarms

		private void packetArrived(PacketArrivedCallbackWrapper packetCallbackWrapper)
		{
			lock (this)
			{
				enqueuedCallbacks.insertAtTail(packetCallbackWrapper);

				if (currentState == State.IDLING)
				{
					currentState = State.CONTEXT_SWITCHING;
                    ((QS.Fx.Clock.IAlarmClock)simulatedClock).Schedule(contextSwitchingOverhead, this.switchingCallback, State.PROCESSING);
				}
			} 
		}

		private void alarmArrived(QS.Fx.Clock.IAlarm alarmRef)
		{
			lock (this)
			{
				AlarmWrapper alarmWrapper = (AlarmWrapper) alarmRef.Context;
				// alarmWrapper.underlyingAlarmRef = alarmRef;
				
				enqueuedCallbacks.insertAtTail(alarmWrapper);

				if (currentState == State.IDLING)
				{
					processEnqueuedCallbacks();
				}
			}
		}

		#endregion

		#region SimulatedClient Class

		protected class SimulatedClient : Virtualization_.VirtualCommunicationSubsystem.NetworkClient
		{
			public SimulatedClient(Virtualization_.INetwork network, Virtualization_.VirtualCommunicationSubsystem encapsulatingVCS) 
				: base(network, encapsulatingVCS)
			{
			}

			protected override void packetArrivedCallback(IPAddress sourceIPAddress, IPAddress destinationIPAddress, 
				QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				((SimulatedPlatform) encapsulatingVCS).packetArrived(new PacketArrivedCallbackWrapper(
					new Virtualization_.PacketArrivedCallback(base.packetArrivedCallback), sourceIPAddress, destinationIPAddress, blockOfData));
			}
		}

		#endregion
		
		#region Wrappers

		#region IGenericWrapper

		private interface IGenericWrapper : Collections_1_.IBiLinkable
		{
			void dispatch();
		}

		#endregion

		#region PacketArrivedCallbackWrapper Class

        private class PacketArrivedCallbackWrapper : Collections_1_.GenericBiLinkable, IGenericWrapper
        {
			public PacketArrivedCallbackWrapper(Virtualization_.PacketArrivedCallback callback, IPAddress sourceIPAddress, 
				IPAddress destinationIPAddress, QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				this.callback = callback;
				this.sourceIPAddress = sourceIPAddress;
				this.destinationIPAddress = destinationIPAddress;
				this.blockOfData = blockOfData;
			}

			public void dispatch()
			{
				callback(sourceIPAddress, destinationIPAddress, blockOfData);
			}

			private Virtualization_.PacketArrivedCallback callback;
			private IPAddress sourceIPAddress, destinationIPAddress;
			private QS._core_c_.Base2.IBlockOfData blockOfData;
		}

		#endregion

		#region AlarmWrapper Class

        private class AlarmWrapper : Collections_1_.GenericBiLinkable, QS.Fx.Clock.IAlarm, IGenericWrapper
        {
			public AlarmWrapper(QS.Fx.Clock.AlarmCallback alarmCallback, object argument, double interval, SimulatedPlatform encapsulatingSimulatedPlatform)
			{
				this.argument = argument;
				this.alarmCallback = alarmCallback;
				this.interval = interval;
				this.encapsulatingSimulatedPlatform = encapsulatingSimulatedPlatform;
			}

			public void dispatch()
			{
				alarmCallback(this);
			}

			public QS.Fx.Clock.IAlarm underlyingAlarmRef;

			private QS.Fx.Clock.AlarmCallback alarmCallback;
			private object argument;
			private SimulatedPlatform encapsulatingSimulatedPlatform;
			private double interval;

            #region IAlarm Members

            double QS.Fx.Clock.IAlarm.Time
            {
                get { throw new NotImplementedException(); }
            }

            double QS.Fx.Clock.IAlarm.Timeout
            {
                get { return interval; }
            }

            bool QS.Fx.Clock.IAlarm.Completed
            {
                get { throw new NotImplementedException(); }
            }

            bool QS.Fx.Clock.IAlarm.Cancelled
            {
                get { throw new NotImplementedException(); }
            }

            object QS.Fx.Clock.IAlarm.Context
            {
                get { return argument; }
                set { argument = value; }
            }

            void QS.Fx.Clock.IAlarm.Reschedule()
            {
                lock (this)
                {
                    underlyingAlarmRef.Reschedule(encapsulatingSimulatedPlatform.LocalTimeOffset + this.interval);
                }
            }

            void QS.Fx.Clock.IAlarm.Reschedule(double timeout)
            {
                lock (this)
                {
                    this.interval = timeout;
                    underlyingAlarmRef.Reschedule(encapsulatingSimulatedPlatform.LocalTimeOffset + this.interval);
                }
            }

            void QS.Fx.Clock.IAlarm.Cancel()
            {
                lock (this)
                {
                    underlyingAlarmRef.Cancel();
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

		#endregion

		#endregion

		#region IAlarmClock Members

		QS.Fx.Clock.IAlarm QS.Fx.Clock.IAlarmClock.Schedule(double timeSpanInSeconds, QS.Fx.Clock.AlarmCallback callerAlarmCallback, object argument)
		{
			AlarmWrapper alarmWrapper = new AlarmWrapper(callerAlarmCallback, argument, timeSpanInSeconds, this);
			lock (alarmWrapper)
			{
                alarmWrapper.underlyingAlarmRef = ((QS.Fx.Clock.IAlarmClock)simulatedClock).Schedule(this.LocalTimeOffset + timeSpanInSeconds, 
					this.interceptingAlarmCallback, alarmWrapper);
			}

			return alarmWrapper;
		}

		#endregion

		#region IClock Members

		[QS.Fx.Base.Inspectable]
		public double Time
		{
			get
			{
                return simulatedClock.Time + this.LocalTimeOffset;
			}
		}

		#endregion
	}
}
