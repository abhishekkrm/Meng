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

namespace QS._qss_e_.Environments_
{
	/// <summary>
	/// Summary description for RTSEnvironment.
	/// </summary>
	public class EmulatedEnvironment : QS.Fx.Inspection.Inspectable, Runtime_.IEnvironment // , Management.IManagedComponent
	{
        public EmulatedEnvironment(uint numberOfNodes) : this(numberOfNodes, false)
        {
        }

        public EmulatedEnvironment(uint numberOfNodes, bool combineLogs) 
            : this(new QS._core_c_.Base.Logger(null, true), numberOfNodes, combineLogs)		
		{
		}

		public EmulatedEnvironment(QS.Fx.Logging.ILogger logger, uint numberOfNodes, bool combineLogs) 
			: this(logger, new QS._qss_c_.Base1_.Subnet("100.0.x.x"), 1500, numberOfNodes, combineLogs)
		{
		}

		public EmulatedEnvironment(QS.Fx.Logging.ILogger logger, QS._qss_c_.Base1_.Subnet subnet, uint packetSize, uint numberOfNodes, bool combineLogs)
		{
			// this.logger = logger;
			alarmClock = new QS._qss_c_.Base1_.PQAlarmClock(new QS._qss_c_.Collections_1_.BHeap(100, 2), logger, true);
			network = new QS._qss_c_.Virtualization_.VirtualNetwork(subnet, packetSize, alarmClock, logger);

			nodes = new Runtime_.INodeRef[numberOfNodes];			
			Management_.IManagedComponent[] nodesAsSubcomponents = new Management_.IManagedComponent[numberOfNodes];

			for (uint ind = 0; ind < numberOfNodes; ind++)
			{
				// CMS.Base.IReadableLogger nodelogger = new CMS.Base.Logger(false, logger, false, "Node_" + ind.ToString("000") + " : ");
				QS._core_c_.Base.IReadableLogger nodelogger = combineLogs 
                    ? (new QS._core_c_.Base.Logger(null, false, logger, false, "Node_" + ind.ToString("000") + " :"))
                    : (new QS._core_c_.Base.Logger(null, true));

                nodes[ind] = new QS._qss_e_.Runtime_.LocalNode(new QS._qss_c_.Platform_.VirtualPlatform(alarmClock, 
					QS._core_c_.Base2.PreciseClock.Clock, new QS._qss_c_.Virtualization_.INetwork[] { network }, nodelogger), true);

				nodesAsSubcomponents[ind] = new Management_.ComponentWrapper(
					nodes[ind], nodes[ind].NICs[0].ToString(), nodelogger, null);
			}

			this.subcomponents = new QS._qss_e_.Management_.IManagedComponent[] { 
				new Management_.ComponentWrapper(this.network, network.Subnet.ToString(), null, nodesAsSubcomponents) };

			inspectableSubcomponents = new QS._qss_e_.Inspection_.Array("Subcomponents", subcomponents);
		}

		// private CMS.Base.IReadableLogger logger;
		private QS._qss_c_.Base1_.PQAlarmClock alarmClock;
		private QS._qss_c_.Virtualization_.VirtualNetwork network;
		private Runtime_.INodeRef[] nodes;

		private QS._qss_e_.Management_.IManagedComponent[] subcomponents;

		[QS.Fx.Base.Inspectable("Subcomponents", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Inspection_.Array inspectableSubcomponents;

		public QS._qss_c_.Virtualization_.VirtualNetwork Network
		{
			get
			{
				return this.network;
			}
		}

		#region IEnvironment Members

		public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get
			{
				return this.alarmClock;
			}
		}

		public QS.Fx.Clock.IClock Clock
		{
			get
			{
				return QS._core_c_.Base2.PreciseClock.Clock;
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

		#region IDisposable Members

		public void Dispose()
		{
			alarmClock.AllDone.WaitOne();

			for (uint ind = 0; ind < nodes.Length; ind++)
				nodes[ind].Dispose();

			alarmClock.Dispose();
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
				return "emulated environment";
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
				return null;
			}
		}

		#endregion
	}
}
