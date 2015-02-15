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

namespace QS._qss_e_.Environments_
{
	/// <summary>
	/// Summary description for DistributedEnvironment.
	/// </summary>
	public class DistributedEnvironment : QS.Fx.Inspection.Inspectable, Runtime_.IEnvironment // , Management.IManagedComponent
	{
        public DistributedEnvironment(Runtime_.IRemoteNode[] remoteNodes) : this(null, remoteNodes)
        {
        }

		public DistributedEnvironment(QS.Fx.Logging.ILogger logger, Runtime_.IRemoteNode[] remoteNodes)
		{
			alarmClock = new QS._qss_c_.Base1_.PQAlarmClock(new QS._qss_c_.Collections_1_.BHeap(20, 2), null); // null for now

			nodes = new Runtime_.INodeRef[remoteNodes.Length];
			subcomponents = new Management_.IManagedComponent[remoteNodes.Length];

			NodeWrapper[] nodeWrappers = new NodeWrapper[remoteNodes.Length];
			for (uint ind = 0; ind < nodes.Length; ind++)
			{
				nodes[ind] = remoteNodes[ind];
				subcomponents[ind] = remoteNodes[ind];

				nodeWrappers[ind] = new NodeWrapper(logger, remoteNodes[ind]);
			}

            if (logger != null)
                logger.Log(this, "__wrapper.synchronize: begin");
            
			foreach (NodeWrapper wrapper in nodeWrappers)
				wrapper.synchronize();

			inspectableNodeCollection = new QS._qss_e_.Inspection_.Array("Nodes", nodes);
		}

		private QS._qss_c_.Base1_.PQAlarmClock alarmClock;
		private Runtime_.INodeRef[] nodes;
		private Management_.IManagedComponent[] subcomponents;

		[QS.Fx.Base.Inspectable("Nodes", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.Array inspectableNodeCollection;

		#region NodeWrapper

		private class NodeWrapper
		{
			public NodeWrapper(QS.Fx.Logging.ILogger logger, Runtime_.IRemoteNode node)
			{
				this.node = node;
                this.logger = logger;
				thread = new Thread(new ThreadStart(this.mainloop));
				thread.Start();
			}

			private void mainloop()
			{
				node.connect();
			}

			private Runtime_.IRemoteNode node;
			private Thread thread;
            private QS.Fx.Logging.ILogger logger;

			public void synchronize()
			{
                if (logger != null)
                    logger.Log(this, "__synchronize.beginning: " + node.Name);

				thread.Join();

                if (logger != null)
                    logger.Log(this, "__synchronize.completed: " + node.Name);
			}
		}

		#endregion

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
			foreach (Runtime_.IRemoteNode node in nodes)
				node.Dispose();
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
				return "distributed environment";
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
