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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_e_.Experiments_
{
	[QS._qss_e_.Base_1_.Arguments("-nnodes:100 -time:60 -mttf:20 -downtime:2")]
    [QS.Fx.Base.Inspectable]
	public abstract class Experiment_200 : QS.Fx.Inspection.Inspectable, IExperiment
	{
		public Experiment_200()
		{
			applicationCollection = new ApplicationCollection(this);
        }

        #region Helpers

        protected void Log(string s)
        {
            logger.Log(null, s);
        }

        protected void Sleep(TimeSpan time)
        {
            sleeper.sleep(time.TotalSeconds);
        }

        #endregion

        private ApplicationCollection applicationCollection;

        [QS.Fx.Base.Inspectable]
		protected abstract class Application : Base_1_.ControlledApplication
		{
			private const int port_number = 12022;

            private int appno;

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
			{
                this.launchargs = args;
                this.appno = Convert.ToInt32((string)args["_appno"]);
                experimentPath = (string)args["experimentPath"] + "\\Process_" + appno.ToString();
                repository_root = experimentPath;

                this.platform = platform;
                this.logger = new QS._qss_c_.Base1_.LoggerSet(platform.Logger,
                    new QS._qss_d_.Components_.FileLogger(experimentPath + "\\message_log.txt"));
				
				this.incarnation = (QS._core_c_.Base3.Incarnation) args["_incarnation"];
                repository_key = "Incarnation_" + incarnation.ToString();

                logger.Log(this, "Experiment Path : \"" + experimentPath + "\"");
 
				QS._core_c_.Base2.Serializer.CommonSerializer.registerClasses(logger);
				System.Net.IPAddress localIPAddress;
				if (args.contains("_base"))
					localIPAddress = System.Net.IPAddress.Parse((string)args["_base"]);
				else
					localIPAddress = platform.NICs[0];
				localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, port_number + appno);
				logger.Log(null, "Base Address Chosen : " + localAddress.ToString());

				coordinatorAddress = 
					new QS.Fx.Network.NetworkAddress(System.Net.IPAddress.Parse((string)args["_coordinator"]), port_number);

				logger.Log(null, (isCoordinator = localAddress.Equals(coordinatorAddress)) ? "Acting as Coordinator." :
					("Coordinator Address : " + coordinatorAddress.ToString() + "."));

				logger.Log(this, "Application starting up now, incarnation " + incarnation.ToString() + ".");

                QS.GUI.Components.RepositorySubmit1.DefaultPath = new string[] { 
                    this.GetType().DeclaringType.Name, 
                    (string) args["_timestamp"], 
                    System.Net.Dns.GetHostName() + "_app" + (string) args["_appno"], "" }; 
			}

            [QS._core_c_.Diagnostics.Component("Platform")]
			protected QS._qss_c_.Platform_.IPlatform platform;
			protected QS.Fx.Logging.ILogger logger;
			protected QS._core_c_.Components.AttributeSet launchargs;
			protected QS._core_c_.Base3.Incarnation incarnation;
			protected QS.Fx.Network.NetworkAddress localAddress, coordinatorAddress;
            protected bool isCoordinator;

            [QS.Fx.Base.Inspectable("Experiment Path")]
            protected string experimentPath;

            protected string repository_root, repository_key;

/*
            public QS.Fx.Network.NetworkAddress Address
            {
                get { return localAddress; }
            }
*/

			public virtual void TerminateApplication(bool smoothly)
			{
				logger.Log(this, "Application of incarnation " + incarnation.ToString() + " is being " +
					(smoothly ? "smoothly" : "unexpectedly") + " terminated.");
			}

			public override void Dispose()
			{
				TerminateApplication(false);
			}
		}

		#region Class MyNode

        [QS.Fx.Base.Inspectable]
		protected class MyNode : QS.Fx.Inspection.Inspectable
		{
			public MyNode(QS._qss_e_.Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger, Runtime_.INodeRef nodeRef, int appno,
				System.Type appClass, QS._core_c_.Components.AttributeSet arguments, bool coordinator, bool up, bool stayon,
				QS._qss_c_.Random_.IRandomGenerator uptimeGenerator, QS._qss_c_.Random_.IRandomGenerator downtimeGenerator)
			{
				this.logger = logger;
				this.environment = environment;
				this.nodeRef = nodeRef;
				this.appClass = appClass;
				this.arguments = new QS._core_c_.Components.AttributeSet(arguments);
                this.arguments["_appno"] = appno.ToString();
				this.coordinator = coordinator;
                this.stayon = stayon;
				this.arguments["_base"] = nodeRef.NICs[0].ToString();
				// this.up = false; 
				// this.launching = false;
				this.uptimeGenerator = uptimeGenerator;
				this.downtimeGenerator = downtimeGenerator;
				// this.alarmRef = null;
				this.applicationRef = null;
				this.shutting_down = false;

				if (up || coordinator)
				{
					BootUp();
				}
				else
				{
					StayOff();
				}
			}

            private Runtime_.IApplicationRef applicationRef;
            private Runtime_.INodeRef nodeRef;
            private QS.Fx.Logging.ILogger logger;
			private QS._qss_e_.Runtime_.IEnvironment environment;
			private QS._core_c_.Components.AttributeSet arguments;
			private bool coordinator, shutting_down, stayon; // , up, launching = false;
			private System.Type appClass;
			private QS.Fx.Clock.IAlarm alarmRef;
			private QS._qss_c_.Random_.IRandomGenerator uptimeGenerator, downtimeGenerator;

            public Runtime_.INodeRef NodeRef
            {
                get { return nodeRef; }
            }

			public Runtime_.IApplicationRef Application
			{
				get { return applicationRef; }
			}

            public void Crash(bool reboot)
            {
                lock (this)
                {
                    if (!reboot)
                        shutting_down = true;
                    if (alarmRef != null)
                        alarmRef.Cancel();

                    TerminateApplication(false);
                }
            }

			public void Shutdown()
			{
				lock (this)
				{
					try
					{
						shutting_down = true;
						if (alarmRef != null)
							alarmRef.Cancel();

						TerminateApplication(true);
					}
					catch (Exception exc)
					{
						logger.Log(this, "__Shutdown : " + exc.ToString());
					}
				}
			}

			#region Internal Processing

			private void BootUp()
			{
				// launching = true;
				// up = false;
				this.arguments["_incarnation"] = new QS._core_c_.Base3.Incarnation(environment.Clock.Time);
				if (!shutting_down)
					nodeRef.BeginLaunch(appClass.FullName, arguments, new AsyncCallback(launchingCallback), null);
			}

			private void TerminateApplication(bool smoothly)
			{
				applicationRef.BeginInvoke(appClass.GetMethod("TerminateApplication", new Type[] { typeof(bool) }),
					new object[] { smoothly }, new AsyncCallback(terminationCallback), null);
			}

			private void StayOff()
			{
				// launching = false;
				// up = false;
				if (!shutting_down)
					alarmRef = environment.AlarmClock.Schedule(
						downtimeGenerator.Get, new QS.Fx.Clock.AlarmCallback(rebootingCallback), null);
			}

			private void rebootingCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
				lock (this)
				{
					BootUp();
				}
			}

			private void launchingCallback(IAsyncResult asyncResult)
			{
				lock (this)
				{
					// launching = false;
					try
					{
						if (shutting_down)
							throw new Exception("Shutting down.");

						applicationRef = nodeRef.EndLaunch(asyncResult);
						// up = true;

						if (!coordinator && !stayon)
						{
							alarmRef = environment.AlarmClock.Schedule(
								uptimeGenerator.Get, new QS.Fx.Clock.AlarmCallback(shutdownCallback), null);
						}
					}
					catch (Exception exc)
					{
						logger.Log(this, "Application at address " + nodeRef.NICs[0].ToString() + 
							" failed to boot and will remain down.\n" + exc.ToString());

						// up = false;
					}
				}
			}

			private void shutdownCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
                Crash(true);
			}

			private void terminationCallback(IAsyncResult asyncResult)
			{
				lock (this)
				{
					try
					{
						applicationRef.EndInvoke(asyncResult);
						applicationRef.Dispose();

                        nodeRef.ReleaseResources();
					}
					catch (Exception exc)
					{
						logger.Log(this, "Count not properly dispose application at address " +
							nodeRef.NICs[0].ToString() + ".\n" + exc.ToString());
					}

					applicationRef = null;

					StayOff();
				}
			}

			#endregion
		}

		#endregion

		#region IExperiment Members

		protected abstract System.Type ApplicationClass
		{
			get;
		}

        protected abstract void experimentWork(QS._core_c_.Components.IAttributeSet results);

		protected Random myrandom = new Random();
		protected MyNode[] mynodes;
		protected QS._qss_e_.Components_.Sleeper sleeper;
		protected QS._core_c_.Components.IAttributeSet arguments;
        protected Runtime_.IEnvironment environment;
        protected QS.Fx.Logging.ILogger logger;

        private const double DefaultDowntime = 5; // in seconds

		void IExperiment.run(QS._qss_e_.Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger, 
			QS._core_c_.Components.IAttributeSet arguments, QS._core_c_.Components.IAttributeSet results)
		{
			this.arguments = arguments;
            this.environment = environment;
            this.logger = logger;

			int nnodes = environment.Nodes.Length;		
			if (arguments.contains("nnodes"))
			{
				int maximum_nnodes = Convert.ToInt32(arguments["nnodes"]);
				if (nnodes > maximum_nnodes && maximum_nnodes > 0)
					nnodes = maximum_nnodes;
			}

			logger.Log(this, "Starting experiment on " + nnodes.ToString() + " nodes.");

            bool stayon = arguments.contains("stayon");
            double mttf, downtime, probability_up;
            QS._qss_c_.Random_.IRandomGenerator uptimeGenerator, downtimeGenerator;

            if (arguments.contains("downtime"))
            {
                if ((downtime = Convert.ToDouble(arguments["downtime"])) < 1)
                    throw new ArgumentException("Downtime must be set to at least 1s.");
            }
            else
                downtime = DefaultDowntime;
            downtimeGenerator = new QS._qss_c_.Random_.Shifted(new QS._qss_c_.Random_.Exponential(downtime - 1), 1);

            if (stayon)
            {
                probability_up = 1;
                uptimeGenerator = null;
            }
            else
            {
                mttf = Convert.ToDouble(arguments["mttf"]);
                probability_up = 1 - downtime / mttf;
                uptimeGenerator = new QS._qss_c_.Random_.Exponential(mttf);
            }			

			QS._core_c_.Components.AttributeSet experiment_arguments = new QS._core_c_.Components.AttributeSet(arguments.Attributes);
			experiment_arguments["_coordinator"] = environment.Nodes[0].NICs[0].ToString();
            System.DateTime now = System.DateTime.Now;
            experiment_arguments["_timestamp"] = now.ToString("yyMMdd_HHmmss");
			System.Type applicationClass = this.ApplicationClass;
			if (!typeof(Application).IsAssignableFrom(applicationClass))
				throw new Exception("Wrong application type.");

			mynodes = new MyNode[nnodes];
            int appno = 0;
            System.Net.IPAddress lastAddress = System.Net.IPAddress.Any;
			for (int ind = 0; ind < nnodes; ind++)
			{
                System.Net.IPAddress thisAddress = environment.Nodes[ind].NICs[0];
                if (thisAddress == lastAddress)
                    appno++;
                else
                    appno = 0;
                lastAddress = thisAddress;

				mynodes[ind] = new MyNode(environment, logger, environment.Nodes[ind], appno, applicationClass,
                    experiment_arguments, ind == 0, (stayon ? true : (myrandom.NextDouble() < probability_up)), stayon, 
                    uptimeGenerator, downtimeGenerator);
			}

			sleeper = new QS._qss_e_.Components_.Sleeper(environment.AlarmClock);

            // we have to use active waiting here because we don't know whether we're running in a simulator or on a real hardware
            for (int ind = 0; ind < this.NumberOfApplications; ind++)
                while (ApplicationOf(ind) == null)
                    sleeper.sleep(1);

            for (int ind = 0; ind < this.NumberOfApplications; ind++)
                ApplicationOf(ind).Controller = new Base_1_.ObjectController(this, logger);

			experimentWork(results);

			logger.Log(this, "Experiment Completed.");
		}

		#endregion

		protected Runtime_.IApplicationRef Coordinator
		{
			get { return mynodes[0].Application; }
		}

        protected Runtime_.IApplicationRef ApplicationOf(int nodeno)
        {
            return mynodes[nodeno].Application;
        }

        protected int NumberOfApplications
        {
            get { return mynodes.Length; }
        }

        protected Runtime_.INodeRef Node(int nodeno)
        {
            return mynodes[nodeno].NodeRef;
        }

        protected MyNode NodeController(int nodeno)
        {
            return mynodes[nodeno];
        }

		#region Class ApplicationCollection

        [QS.Fx.Base.Inspectable]
		private class ApplicationCollection : QS.Fx.Inspection.Inspectable, 
            System.Collections.Generic.IEnumerable<Runtime_.IApplicationRef>
		{
			public ApplicationCollection(Experiment_200 owner)
			{
				this.owner = owner;
			}

			private Experiment_200 owner;

			#region IEnumerable<IApplicationRef> Members

			IEnumerator<QS._qss_e_.Runtime_.IApplicationRef> IEnumerable<QS._qss_e_.Runtime_.IApplicationRef>.GetEnumerator()
			{
				foreach (MyNode node in owner.mynodes)
					yield return node.Application;
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				foreach (MyNode node in owner.mynodes)
					yield return node.Application;
			}

			#endregion
		}

		#endregion

		protected System.Collections.Generic.IEnumerable<Runtime_.IApplicationRef> Applications
		{
			get { return applicationCollection; }
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (MyNode node in mynodes)
				node.Shutdown();
		}

		#endregion
	}
}
