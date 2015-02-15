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

#region Using directives

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#endregion

using System.Threading;

namespace QS.GUI.Simulations
{
	public partial class Simulator 
		: Component, ISimulator, QS._qss_e_.Management_.IManagedComponent, QS.Fx.Inspection.IInspectable
	{
		private const int default_nnodes = 25;
		private const int default_polling_interval = 1000;

		public Simulator()
		{
			InitializeComponent();
			preinitialize();
		}

		public Simulator(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			preinitialize();
		}

        [QS.Fx.Base.Inspectable("Simulator Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Base.Logger mainlogger;
        [QS.Fx.Base.Inspectable("Experiment Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Base.Logger experiment_logger;
        [QS.Fx.Base.Inspectable("Environment", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Environments_.SimulatedEnvironment environment;

		[QS.Fx.Base.Inspectable]
		private System.Type experimentClass;
		[QS.Fx.Base.Inspectable]
		private QS._core_c_.Components.AttributeSet arguments;
		private bool initialized, running;
		[QS.Fx.Base.Inspectable]
		private QS._qss_e_.Experiments_.IExperiment experiment;
		private QS._qss_e_.Management_.ComponentWrapper experimentWrapper;
		private QS._qss_e_.Management_.IManagedComponent[] subcomponents;
		[QS.Fx.Base.Inspectable]
		private QS._core_c_.Components.AttributeSet results;
		private Thread experimentThread, monitoringThread;
		private int nnodes = default_nnodes;
		private double packetLossRate = 0.01;
		private double networkLatency = 0.001;
		private int incomingQueueSize = 100;
		private bool finishing = false;
		private AutoResetEvent checkAgain;
		private double lastChecked, lastSimTime;
		private int lastProcessed, polling_interval = default_polling_interval;
		private System.Threading.ManualResetEvent experimentComplete = new ManualResetEvent(false);

		private void preinitialize()
		{
			mainlogger = new QS._core_c_.Base.Logger(null, true, null, QS._core_c_.Base.Logger.TimestampingMode.REALTIME, string.Empty);
			experiment_logger = new QS._core_c_.Base.Logger(null, true, null, QS._core_c_.Base.Logger.TimestampingMode.CPUTIME, string.Empty);
			experimentWrapper = new QS._qss_e_.Management_.ComponentWrapper(null, "Experiment", experiment_logger, 
					new QS._qss_e_.Management_.IManagedComponent[] {});
			subcomponents = new QS._qss_e_.Management_.IManagedComponent[] { experimentWrapper, null };
		}

		public int NumberOfNodes
		{
			get { return nnodes; }
			set
			{
				lock (this)
				{
					if (initialized)
						throw new Exception("Cannot change this attribute, the experiment is already initialized.");

					nnodes = value;
				}
			}
		}

		public double PacketLossRate
		{
			set { packetLossRate = value; }
		}

		public double NetworkLatency
		{
			set { networkLatency = value; }
		}

		public int IncomingQueueSize
		{
			set { incomingQueueSize = value; }
		}

		#region Experiment Loop

		private void experimentLoop()
		{
			try
			{
				experiment.run(environment, experiment_logger, arguments, results);
			}
			catch (Exception exc)
			{
				mainlogger.Log(this, "__experimentLoop: " + exc.ToString());
			}

			experimentComplete.Set();
			environment.Stop();
		}

		#endregion 

		#region Monitoring Loop

		#region Class MonitoringCallbackArgs

		public class MonitoringCallbackArgs : EventArgs
		{
			public MonitoringCallbackArgs(int processed, double speed, int togo, double simtime, double sim_speedup)
			{
				this.processed = processed;
				this.speed = speed;
				this.togo = togo;
				this.simtime = simtime;
				this.sim_speedup = sim_speedup;
			}

			private double speed, simtime, sim_speedup;
			private int processed, togo;

			public int NumberOfEventsProcessed
			{
				get { return processed; }
			}

			public double CurrentProcessingSpeed
			{
				get { return speed; }
			}

			public int NumberOfEventsInQueue
			{
				get { return togo; }
			}

			public double SimulationTime
			{
				get { return simtime; }
			}

			public double SimulationSpeedup
			{
				get { return sim_speedup; }
			}
		}

		#endregion

		private EventHandler monitoringCallback;

		public event EventHandler MonitoringCallback
		{
			add { monitoringCallback += value; }
			remove { monitoringCallback -= value; }
		}

		private void monitoringLoop()
		{
			lastChecked = QS._core_c_.Base2.PreciseClock.Clock.Time;
			lastSimTime = 0;
			lastProcessed = 0;

			while (!finishing)
			{
				checkAgain.WaitOne(polling_interval, false);

				if (!finishing)
				{
					double now, speed, simtime, sim_speedup;
					int processed, togo;

					lock (this)
					{
						now = QS._core_c_.Base2.PreciseClock.Clock.Time;
						processed = environment.EventsProcessed;
						double time_delta = now - lastChecked;
						speed = (processed - lastProcessed) / time_delta;
						lastProcessed = processed;
						lastChecked = now;
						togo = environment.EventsScheduled;
						simtime = environment.Clock.Time;
						sim_speedup = (simtime - lastSimTime) / time_delta;
						lastSimTime = simtime;
					}

					monitoringCallback(this, 
						new MonitoringCallbackArgs(processed, speed, togo, simtime, sim_speedup));
				}
			}
		}

		#endregion

		#region ISimulator Members

		void ISimulator.Start()
		{
			lock (this)
			{
				if (initialized)
					throw new Exception("Simulator already initialized.");
				if (experimentClass == null)
					throw new Exception("Experiment not specified.");
				if (arguments == null)
				{
					arguments = new QS._core_c_.Components.AttributeSet(((experiment.GetType().GetCustomAttributes(
						typeof(QS._qss_e_.Base_1_.ArgumentsAttribute), false)[0]) as QS._qss_e_.Base_1_.ArgumentsAttribute).Arguments);
				}

				environment = new QS._qss_e_.Environments_.SimulatedEnvironment(mainlogger, 1, (uint) nnodes, 
					packetLossRate, networkLatency, incomingQueueSize);
				subcomponents[1] = environment;

				experiment = (QS._qss_e_.Experiments_.IExperiment) experimentClass.GetConstructor(
					System.Type.EmptyTypes).Invoke(QS._qss_c_.Helpers_.NoObject.Array);
				experimentWrapper.Component = experiment;

				results = new QS._core_c_.Components.AttributeSet();

				experimentComplete.Reset();
				experimentThread = new Thread(new ThreadStart(experimentLoop));
				experimentThread.Start();

				monitoringThread = new Thread(new ThreadStart(this.monitoringLoop));

				checkAgain = new AutoResetEvent(false);
				monitoringThread.Start();

				running = false;
				initialized = true;
			}
		}

		void ISimulator.Shutdown()
		{
			lock (this)
			{
				if (initialized)
				{
					finishing = true;
					checkAgain.Set();

					try { monitoringThread.Join(); }
					catch (Exception exc) { mainlogger.Log(this, exc.ToString()); } ;

					try { experimentThread.Join(); }
					catch (Exception exc) { mainlogger.Log(this, exc.ToString()); } ;

					try { ((QS._qss_c_.Simulations_1_.SimulatedClock) environment.AlarmClock).Clear(); }
					catch (Exception exc) { mainlogger.Log(this, exc.ToString()); } ;

					try { experiment.Dispose(); }
					catch (Exception exc) { mainlogger.Log(this, exc.ToString()); } ;
					experiment = null;
					experimentWrapper.Component = null;

					try { environment.Dispose(); }
					catch (Exception exc) { mainlogger.Log(this, exc.ToString()); } ;
					environment = null;
					subcomponents[1] = null;

					results = null;

					initialized = false;
				}
			}
		}

		void ISimulator.Pause()
		{
			lock (this)
			{
				if (!initialized)
					throw new Exception("Not initialized.");
				if (!running)
					throw new Exception("Not running.");

				environment.Stop();
				running = false;
			}
		}

		void ISimulator.StepForward()
		{
			((ISimulator)this).StepForward(1);
		}

		void ISimulator.StepForward(int nsteps)
		{
			lock (this)
			{
				if (!initialized)
					throw new Exception("Not initialized.");
				if (running)
					throw new Exception("Currently running.");

				for (int ind = 0; ind < nsteps; ind++)
					environment.Step();
			}
		}

		void ISimulator.Continue()
		{
			lock (this)
			{
				if (!initialized)
					throw new Exception("Not initialized.");
				if (running)
					throw new Exception("Already running.");
				
				running = true;
				environment.Start();
			}
		}

		SimulatorStatus ISimulator.Status
		{
			get 
			{ 
				return initialized ? (running ? SimulatorStatus.RUNNING : 
					SimulatorStatus.INTERRUPTED) : SimulatorStatus.UNINITIALIZED; 
			}
		}

		System.Type ISimulator.ExperimentClass
		{
			get { return experimentClass; }
			set
			{
				lock (this)
				{
					if (running)
						throw new Exception("Cannot change while simulator is running.");
					if (!typeof(QS._qss_e_.Experiments_.IExperiment).IsAssignableFrom(value))
						throw new Exception("This class does not implement QS.TMS.Experiments.IExperiment.");
					if (value.GetConstructor(System.Type.EmptyTypes) == null)
						throw new Exception("This class does not have a default parameter-less constructor.");

					experimentClass = value;
				}
			}
		}

		QS._core_c_.Components.AttributeSet ISimulator.Arguments
		{
			get { return arguments; }
			set
			{
				lock (this)
				{
					if (running)
						throw new Exception("Cannot change while simulator is running.");
					arguments = value;
				}
			}
		}

		QS._core_c_.Components.AttributeSet ISimulator.Results
		{
			get { return results; }
		}

		#endregion

		#region IManagedComponent Members

		string QS._qss_e_.Management_.IManagedComponent.Name
		{
			get { return "Simulator"; }
		}

		QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
		{
			get { return subcomponents; }
		}

		QS._core_c_.Base.IOutputReader QS._qss_e_.Management_.IManagedComponent.Log
		{
			get { return mainlogger; }
		}

		object QS._qss_e_.Management_.IManagedComponent.Component
		{
			get { return this; }
		}

		#endregion

		#region IInspectable Members

		private QS.Fx.Inspection.IAttributeCollection attributeCollection;
		QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
		{
			get 
			{
				lock (this)
				{
					if (attributeCollection == null)
						attributeCollection = new QS.Fx.Inspection.AttributesOf(this);
				}

				return attributeCollection;
			}
		}

		#endregion
	}
}
