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

// #define DEBUG_SimulatedCommunicationSubsystem
// #define STATISTICS_CollectOneSecondStatistics

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;

namespace QS._qss_c_.Simulations_2_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class SimulatedCommunicationSubsystem 
		: Virtualization_.VirtualCommunicationSubsystem, QS._qss_e_.Management_.IManagedComponent
	{
		private const double interruptingTime = 0.000075;
		private const int default_maximumQueueSize = 100;

        private const double StatisticsCollectingFrequency = 1;

		public SimulatedCommunicationSubsystem(ISimulatedCPU simulatedCPU, Virtualization_.INetwork[] networks, 
            int incomingQueueSize) 
			: this(new Base3_.Logger(simulatedCPU), simulatedCPU, networks, incomingQueueSize)
		{
		}

		private SimulatedCommunicationSubsystem(QS._core_c_.Base.IReadableLogger logger, ISimulatedCPU simulatedCPU,
            Virtualization_.INetwork[] networks, int incomingQueueSize)
            : base(networks, logger)
		{
			// this.logger = logger;
			this.simulatedCPU = simulatedCPU;
			this.maximumQueueSize = incomingQueueSize;

            this.alarmClock = simulatedCPU;
            this.clock = simulatedCPU;

#if STATISTICS_CollectOneSecondStatistics
            alarmClock.Schedule(1 / SimulatedCommunicationSubsystem.StatisticsCollectingFrequency,
                new QS.Fx.QS.Fx.Clock.AlarmCallback(this.StatisticsCallback), null);
#endif
		}

		[QS.Fx.Base.Inspectable("CPU", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private ISimulatedCPU simulatedCPU;
		// private Base.IReadableLogger logger;
		[QS.Fx.Base.Inspectable("Incoming Queue", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private System.Collections.Generic.Queue<PacketArrived> packetQueue = new Queue<PacketArrived>();
		private int maximumQueueSize;
		[QS.Fx.Base.Inspectable("Interrupting", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private bool interrupting = false;

        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;

        [QS.Fx.Base.Inspectable]
        private int numberOfPacketsArrived, numberOfPacketsDropped;

        private double lastChecked = 0;
        private double accumulatedQueueSize, accumulatedQueueSizeChecked;
        private int lastArrived, lastDropped;

#if STATISTICS_CollectOneSecondStatistics
        [QS.CMS.Diagnostics.Component("Average Incoming Queue Size")]
        private QS.CMS.Statistics.SamplesXY averageQueueSizes = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Packet Arrivals Per Second")]
        private QS.CMS.Statistics.SamplesXY arrivalRatesPerSec = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Packets Dropped Per Second")]
        private QS.CMS.Statistics.SamplesXY droppingRatesPerSec = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Percentage of Packets Dropped")]
        private QS.CMS.Statistics.SamplesXY queueDroppingRatio = new QS.CMS.Statistics.SamplesXY();
#endif

        #region StatisticsCallback

        [Logging_1_.IgnoreCallbacks]
        private void StatisticsCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                double now = clock.Time;
                double interval = now - lastChecked;
                double sampleTime = (lastChecked + now) / 2;
                lastChecked = now;

                accumulatedQueueSize += packetQueue.Count * (now - accumulatedQueueSizeChecked);
                accumulatedQueueSizeChecked = now;
                double averageQueueSize = accumulatedQueueSize / interval;
                accumulatedQueueSize = 0;
#if STATISTICS_CollectOneSecondStatistics
                averageQueueSizes.addSample(sampleTime, averageQueueSize);
#endif

                double arrivalsPerSecond = (numberOfPacketsArrived - lastArrived) / interval;
                double dropsPerSecond = (numberOfPacketsDropped - lastDropped) / interval;
                double droppingRatio = 
                    (((double)(numberOfPacketsDropped - lastDropped)) / ((double)(numberOfPacketsArrived - lastArrived)));
                lastArrived = numberOfPacketsArrived;
                lastDropped = numberOfPacketsDropped;

#if STATISTICS_CollectOneSecondStatistics
                arrivalRatesPerSec.addSample(sampleTime, arrivalsPerSecond);
                droppingRatesPerSec.addSample(sampleTime, dropsPerSecond);
                queueDroppingRatio.addSample(sampleTime, droppingRatio);
#endif
            }
            alarmRef.Reschedule();
        }

        #endregion

        #region Class PacketArrived

        private class PacketArrived
		{
			public PacketArrived(Virtualization_.PacketArrivedCallback callback, IPAddress sourceIPAddress,
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

			public override string ToString()
			{
				return sourceIPAddress.ToString() + " --> " + destinationIPAddress.ToString() + ", " +
					blockOfData.Size.ToString() + " bytes, " + callback.Target.ToString() + ", " + callback.Method.ToString();
			}
		}

		#endregion

		#region Processing of Packets

		private void packetArrived(PacketArrived packetArrived)
		{
#if STATISTICS_CollectOneSecondStatistics
            lock (this)
            {
                double now = clock.Time;
                accumulatedQueueSize += packetQueue.Count * (now - accumulatedQueueSizeChecked);
                accumulatedQueueSizeChecked = now;
            }
#endif

            lock (packetQueue)
			{
                numberOfPacketsArrived++;

				if (packetQueue.Count >= maximumQueueSize)
				{
#if DEBUG_SimulatedCommunicationSubsystem
					logger.Log(this, "Packet queue full, dropping packet " + packetArrived.ToString());
#endif

					// dropping the packet
                    numberOfPacketsDropped++;
				}
				else
				{
					packetQueue.Enqueue(packetArrived);
					if (!interrupting)
					{
						interrupting = true;
						((QS.Fx.Scheduling.IScheduler) simulatedCPU).BeginExecute(new AsyncCallback(interruptCallback), null);
					}
				}
			}
		}

		private void interruptCallback(IAsyncResult asynchronousResult)
		{
			simulatedCPU.Sleep(interruptingTime);
            ((QS.Fx.Scheduling.IScheduler)simulatedCPU).BeginExecute(new AsyncCallback(receivingCallback), null);
		}

		private void receivingCallback(IAsyncResult asynchronousResult)
		{
#if STATISTICS_CollectOneSecondStatistics
            lock (this)
            {
                double now = clock.Time;
                accumulatedQueueSize += packetQueue.Count * (now - accumulatedQueueSizeChecked);
                accumulatedQueueSizeChecked = now;
            }
#endif

			Queue<PacketArrived> packets; 
			lock (packetQueue)
			{
				packets = new Queue<PacketArrived>(packetQueue);
				packetQueue.Clear();
				interrupting = false;
			}

            ((QS.Fx.Scheduling.IScheduler)simulatedCPU).BeginExecute(new AsyncCallback(dispatchCallback), packets);
		}

		private void dispatchCallback(IAsyncResult asynchronousResult)
		{
			Queue<PacketArrived> packets = (Queue<PacketArrived>)asynchronousResult.AsyncState;
			foreach (PacketArrived packet in packets)
			{
#if DEBUG_SimulatedCommunicationSubsystem
				logger.Log(this, "__DispatchCallback: " + packet.ToString());
#endif
				packet.dispatch();
			}
		}

		#endregion

		#region Class MyNetworkClient

		private class MyNetworkClient : NetworkClient
		{
			public MyNetworkClient(Virtualization_.INetwork network, SimulatedCommunicationSubsystem owner) 
				: base(network, owner)
			{
			}

			protected override void packetArrivedCallback(System.Net.IPAddress sourceIPAddress,
				System.Net.IPAddress destinationIPAddress, QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				((SimulatedCommunicationSubsystem)encapsulatingVCS).packetArrived(
					new PacketArrived(new Virtualization_.PacketArrivedCallback(base.packetArrivedCallback),
					sourceIPAddress, destinationIPAddress, blockOfData));
			}

            public override IAsyncResult BeginSendTo(
                IPAddress destinationAddress, WrappedData wrappedData, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
/*
                return owner.ScheduleSend(this, wrappedData.Size, new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        sendto(destinationAddress, wrappedData);
                        callback(result);
                    }), state);                
*/ 
            }
		}

		#endregion

/*
        private IAsyncResult ScheduleSend(
            MyNetworkClient networkClient, uint dataSize, AsyncCallback executionCallback, object context)
        {
            throw new NotImplementedException();
        }
*/

		protected override NetworkClient createClient(QS._qss_c_.Virtualization_.INetwork network)
		{
			return new MyNetworkClient(network, this);
		}

		#region IManagedComponent Members

		string QS._qss_e_.Management_.IManagedComponent.Name
		{
			get { return "Communication Subsystem"; }
		}

		QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
		{
			get { return null; }
		}

		QS._core_c_.Base.IOutputReader QS._qss_e_.Management_.IManagedComponent.Log
		{
			get { return logger; }
		}

		object QS._qss_e_.Management_.IManagedComponent.Component
		{
			get { return this; }
		}

		#endregion
	}
}
