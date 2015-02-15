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

#define DEBUG_UseSimulation2
// #define DEBUG_SimulatedEnvironment
#define DEBUG_UseBinaryTreePQ

using System;

namespace QS._qss_e_.Environments_
{
	/// <summary>
	/// Summary description for SimulatedEnvironment.
	/// </summary>
	/// 

	public class SimulatedEnvironment : QS.Fx.Inspection.Inspectable, Runtime_.IEnvironment // , Management.IManagedComponent
	{
        public const uint DefaultNetworkMTU = 65000;

		private static QS.Fx.Clock.IClock physicalClock = QS._core_c_.Base2.PreciseClock.Clock;

        private QS._core_c_.Base.Logger mainlogger;

        [QS.Fx.Base.Inspectable("Event Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_c_.Logging_1_.EventLogger eventLogger;

        public SimulatedEnvironment(uint numberOfNetworks, uint numberOfNodesPerNetwork)
			: this(new QS._core_c_.Base.Logger(null, true), numberOfNetworks, numberOfNodesPerNetwork, 0.01, 0.001, 100)
		{
		}

        public SimulatedEnvironment(QS._core_c_.Base.Logger mainlogger, uint numberOfNetworks, uint numberOfNodesPerNetwork,
			double packetLossRate, double networkLatency, int incomingQueueSize)
		{
			nodes = new Runtime_.INodeRef[numberOfNetworks * numberOfNodesPerNetwork];
            nodeLogs = new QS._core_c_.Base.IOutputReader[nodes.Length];

            this.mainlogger = mainlogger;
            mainlogger.Log(this, "Initializing the simulation environment with " + numberOfNetworks.ToString() + " networks, " +
                numberOfNodesPerNetwork.ToString() + " nodes each.");

            this.simulatedClock = new QS._qss_c_.Simulations_1_.SimulatedClock(mainlogger); //, 
// #if DEBUG_UseBinaryTreePQ
//				new QS._qss_c_.Collections_4_.BinaryTree()
// #else
//				new QS.CMS.Collections.BHeap(100, 2)
// #endif
//			);

            this.eventLogger = new QS._qss_c_.Logging_1_.EventLogger(this.simulatedClock, true);

            this.subcomponents = new QS._qss_e_.Management_.IManagedComponent[numberOfNetworks];

			for (uint ind = 0; ind < numberOfNetworks; ind++)
			{
				string subnetAddressAsString = "100." + ind.ToString() + ".x.x";
                QS._core_c_.Base.Logger logger = new QS._core_c_.Base.Logger(null, true);
				
				QS._qss_c_.Virtualization_.VirtualNetwork simulatedNetwork = new QS._qss_c_.Virtualization_.VirtualNetwork(
                    new QS._qss_c_.Base1_.Subnet(subnetAddressAsString), DefaultNetworkMTU, this.simulatedClock, logger,
					packetLossRate, new QS._qss_c_.Random_.Uniform(0.9 * networkLatency, 1.1 * networkLatency));

				Management_.IManagedComponent[] nodesAsSubcomponents = new Management_.IManagedComponent[numberOfNodesPerNetwork];

				QS._qss_c_.Virtualization_.INetwork[] networks = new QS._qss_c_.Virtualization_.INetwork[] { simulatedNetwork };

				for (uint ind2 = 0; ind2 < numberOfNodesPerNetwork; ind2++)
				{
					QS._qss_e_.Management_.IManagedComponent nodeComponent;

#if DEBUG_UseSimulation2
					QS._qss_c_.Simulations_2_.ISimulatedNode simulatedNode = new QS._qss_c_.Simulations_2_.SimulatedNode(
                        eventLogger, simulatedClock, simulatedClock, networks, incomingQueueSize);
					QS._qss_c_.Simulations_2_.ISimulatedPlatform simulatedPlatform = simulatedNode.SimulatedPlatform;
					nodeComponent = simulatedNode;
#else
					QS.CMS.Simulations.SimulatedPlatform simulatedPlatform = 
						new QS.CMS.Simulations.SimulatedPlatform(networks, simulatedClock, physicalClock);
					Runtime.LocalNode simulatedNode = new Runtime.LocalNode(simulatedPlatform, true);
					nodeComponent = new Management.ComponentWrapper(simulatedNode, simulatedNode.NICs[0].ToString(), 
						((TMS.Management.IManagedComponent) simulatedPlatform).Log, null);
#endif

                    int nodeIndex = (int) (ind * numberOfNodesPerNetwork + ind2);
                    nodes[nodeIndex] = simulatedNode;
                    nodeLogs[nodeIndex] = simulatedNode.SimulatedPlatform.Log;

                    nodesAsSubcomponents[ind2] = nodeComponent;
				}

				this.subcomponents[ind] = new Management_.ComponentWrapper(simulatedNetwork, subnetAddressAsString,
					logger, nodesAsSubcomponents);
			}

            simulationThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.simulationLoop));
            canGo = new System.Threading.ManualResetEvent(false);
            simulationThread.Start();
        }

        [QS.Fx.Base.Inspectable("Node Logs", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Base.IOutputReader[] nodeLogs;
		
		[QS.Fx.Base.Inspectable("Clock", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_c_.Simulations_1_.SimulatedClock simulatedClock;
		private QS._qss_e_.Management_.IManagedComponent[] subcomponents;
		[QS.Fx.Base.Inspectable("Simulated Nodes", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Runtime_.INodeRef[] nodes;

		private System.Threading.Thread simulationThread;
        private System.Threading.ManualResetEvent canGo;
        private bool shouldQuit = false;

		[QS.Fx.Base.Inspectable("Monitoring Agent", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_c_.Monitoring_.Agent monitoringAgent = new QS._qss_c_.Monitoring_.Agent("Agent in a simulated platform.");

/*
		private int millisecondsOfDelay = 1;
		private int stepsBetweenDelays = 100;

		public int MillisecondsOfDelay
		{
			get { return millisecondsOfDelay; }
			set { millisecondsOfDelay = value; }
		}

		public int StepsBetweenDelays
		{
			get { return stepsBetweenDelays; }
			set { stepsBetweenDelays = vaue; }
		}

		private int stepsSinceLastDelay = 0;
*/

		private double timeOfStepsCumulative = 0;
		private double timeStartedCounting = physicalClock.Time;
		private double desiredPartOfTimeWorking = 0.9;

		public double DesiredPartOfTimeWorking
		{
			get { return desiredPartOfTimeWorking; }
			set 
			{
				lock (this)
				{
					timeOfStepsCumulative = 0;
					timeStartedCounting = physicalClock.Time;
					desiredPartOfTimeWorking = value;
				}
			}
		}

		private int eventsProcessed = 0;
		// private bool statisticsKnown = false;

		public int EventsProcessed
		{
			get { return eventsProcessed; }
		}

		public int EventsScheduled
		{
			get { return this.simulatedClock.QueueSize; }
		}

		public System.Threading.ManualResetEvent Completed = new System.Threading.ManualResetEvent(false);

		private bool canStop = false;
		public bool CanStop
		{
			set { canStop = value; }
		}

		private void simulationLoop()
        {
			try
			{
				while (!shouldQuit)
				{
					canGo.WaitOne();

					if (!shouldQuit)
					{
						double timeBefore = physicalClock.Time;

						if (!this.Step() && canStop)
						{
							Completed.Set();
							canGo.Reset();
						}

						double timeAfter = physicalClock.Time;
						double timeOfStep = timeAfter - timeBefore;

						int millisecondsExtra;
						lock (this)
						{
							timeOfStepsCumulative += timeOfStep;
							double timeSinceStartedCounting = timeAfter - timeStartedCounting;

							if (timeSinceStartedCounting > 1)
							{
								timeSinceStartedCounting -= 1;
								timeStartedCounting += 1;
								timeOfStepsCumulative -= desiredPartOfTimeWorking;
							}

							millisecondsExtra = (int)System.Math.Floor((timeOfStepsCumulative - desiredPartOfTimeWorking * timeSinceStartedCounting) * 1000);
						}

						if (millisecondsExtra > 0)
							System.Threading.Thread.Sleep(millisecondsExtra);

/*
					if (delay > 0)
					{
						bool shouldWait;
						lock (this)
						{
							stepsSinceLastDelay++;
							shouldWait = stepsSinceLastDelay >= stepsBetweenDelays;
							if (shouldWait)
								stepsSinceLastDelay = 0;
						}

						if (shouldWait)
							System.Threading.Thread.Sleep(millisecondsOfDelay);
					}
*/
					}
				}
			}
			catch (Exception exc)
			{
				mainlogger.Log(this, "Aborted simulationLoop: " + exc.ToString());
			}
		}

        public bool Step()
        {
			bool result;
			lock (this)
            {
				if (result = (simulatedClock.QueueSize > 0))
				{
					try
					{
						this.simulatedClock.advance();
					}
					catch (Exception exc)
					{
						mainlogger.Log(this,"__Step: " + exc.ToString());
						Stop();
					}
					eventsProcessed++;
				}
			}
			return result;
		}

        public void Start()
        {
			Completed.Reset();
			canGo.Set();
        }

        public void Stop()
        {
            canGo.Reset();
        }

        #region IDisposable Members

        public void Dispose()
        {
            ((System.IDisposable)monitoringAgent).Dispose();

            shouldQuit = true;
            canGo.Set();

            try
            {
                if (!simulationThread.Join(3000))
                    simulationThread.Abort();
            }
            catch (Exception exc)
            {
                mainlogger.Log(this, exc.ToString());
            }
        }

        #endregion

		#region IManagedComponent Members

		public object Component
		{
			get
			{
				return this;
			}
		}

		public string Name
		{
			get
			{
				return "simulated environment";
			}
		}

		public QS._qss_e_.Management_.IManagedComponent[] Subcomponents
		{
			get
			{
				return subcomponents;
			}
		}

		public QS._core_c_.Base.IOutputReader Log
		{
			get
			{
				return mainlogger;
			}
		}

		#endregion

		#region IEnvironment Members

		public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get
			{
                return simulatedClock;
                // throw new Exception("not implemented!");
			}
		}

		public QS.Fx.Clock.IClock Clock
		{
			get
			{
                return simulatedClock;
                // throw new Exception("not implemented!");
			}
		}

		public Runtime_.INodeRef[] Nodes
		{
			get
			{
				return nodes;
			}
		}

		#endregion

		public override string ToString()
		{
			return "Simulator";
		}
	}
}
