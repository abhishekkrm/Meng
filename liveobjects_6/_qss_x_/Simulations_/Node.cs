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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Simulations_
{
    public class Node : QS.Fx.Inspection.Inspectable, INode
    {
        public Node(string name, double mttb, double mttf, double mttr,
            QS._qss_c_.Base3_.Constructor<QS._qss_x_.Platform_.IApplication> applicationConstructor, Platform_.IApplicationContext context,
            QS.Fx.Clock.IClock physicalClock, QS.Fx.Clock.IClock simulatedClock, QS.Fx.Clock.IAlarmClock simulatedAlarmClock, 
            Network_.IVirtualNetwork simulatedNetwork, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.name = name;
            this.applicationConstructor = applicationConstructor;
            this.context = context;
            this.mttf = mttf;
            this.mttr = mttr;
            this.mttb = mttb;

            simulatedCPU = new QS._qss_c_.Simulations_2_.SimulatedCPU(eventLogger, physicalClock, simulatedClock, simulatedAlarmClock);
            
            platform = new QS._qss_x_.Platform_.SimulatedPlatform(name, simulatedCPU, eventLogger, simulatedNetwork);

            startCallback = new AsyncCallback(this.StartCallback);
            stopCallback = new AsyncCallback(this.StopCallback);

            stateChangeAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.StateChangeAlarmCallback);
        }

        #region State

        private enum State
        {
            Off, Booting, Running, Rebooting
        }

        #endregion

        [QS.Fx.Base.Inspectable]
        private string name;
        [QS.Fx.Base.Inspectable]
        private State state = State.Off;
        [QS.Fx.Base.Inspectable]
        private double mttb, mttf, mttr;
        [QS.Fx.Base.Inspectable]
        private Platform_.IApplication application;
        private QS._qss_c_.Base3_.Constructor<QS._qss_x_.Platform_.IApplication> applicationConstructor;        
        [QS.Fx.Base.Inspectable]
        private Platform_.IApplicationContext context;

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Simulations_2_.SimulatedCPU simulatedCPU;
        [QS.Fx.Base.Inspectable]
        private Platform_.SimulatedPlatform platform;

        private static Random random = new Random();

        private AsyncCallback startCallback, stopCallback;
        private QS.Fx.Clock.AlarmCallback stateChangeAlarmCallback;

        #region StartCallback

        private void StartCallback(IAsyncResult result)
        {
            lock (this)
            {
                switch (state)
                {
                    case State.Off:
                        {
                            double delay = - mttb * Math.Log(random.NextDouble());

#if DEBUG_LogGenerously
                            ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Booting for " + delay.ToString() + " seconds.");
#endif

                            state = State.Booting;
                            ((QS.Fx.Platform.IPlatform)platform).AlarmClock.Schedule(delay, stateChangeAlarmCallback, null); 
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion

        #region StopCallback

        private void StopCallback(IAsyncResult result)
        {
            lock (this)
            {
                switch (state)
                {
                    case State.Running:
                    {
                        if (application == null)
                            throw new Exception("Internal error, application is null while running.");

#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Stopping application.");
#endif

                        application.Stop();
                        application = null;

#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Resetting platform.");
#endif

                        platform.Reset();

                        state = State.Off;
                    }
                    break;

                    case State.Booting:
                    case State.Rebooting:
                    {
#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Aborting boot or reboot process and turning off.");
#endif

                        state = State.Off;
                    }
                    break;

                    default:
                        break;
                }
            }
        }

        #endregion

        #region StateChangeAlarmCallback

        private void StateChangeAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                switch (state)
                {
                    case State.Booting:
                    case State.Rebooting:
                    {
                        if (application != null)
                            throw new Exception("Internal error, application is not null while booting or rebooting.");

                        state = State.Running;

#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Starting application.");
#endif

                        application = applicationConstructor();
                        application.Start(platform, context);

                        if (mttf == double.PositiveInfinity)
                        {
#if DEBUG_LogGenerously
                            ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Running forever.");
#endif
                        }
                        else
                        {
                            double delay = -mttf * Math.Log(random.NextDouble());

#if DEBUG_LogGenerously
                            ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Running for  " + delay.ToString() + " seconds.");
#endif

                            ((QS.Fx.Platform.IPlatform)platform).AlarmClock.Schedule(delay, stateChangeAlarmCallback, null);
                        }
                    }
                    break;

                    case State.Running:
                    {
                        if (application == null)
                            throw new Exception("Internal error, application is null while running.");

#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Stopping application.");
#endif

                        application.Stop();
                        application = null;

#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Resetting platform.");
#endif

                        platform.Reset();

                        state = State.Rebooting;

                        double delay = -mttr * Math.Log(random.NextDouble());

#if DEBUG_LogGenerously
                        ((QS.Fx.Platform.IPlatform)platform).Logger.Log("Rebooting for  " + delay.ToString() + " seconds.");
#endif

                        ((QS.Fx.Platform.IPlatform)platform).AlarmClock.Schedule(delay, stateChangeAlarmCallback, null);                         
                    }
                    break;

                    default:
                        break;
                }
            }
        }

        #endregion

        #region INode Members

        void INode.Start()
        {
            ((QS.Fx.Scheduling.IScheduler)simulatedCPU).Execute(startCallback, null);
        }

        void INode.Stop()
        {
            ((QS.Fx.Scheduling.IScheduler)simulatedCPU).Execute(stopCallback, null);
        }

        QS._core_c_.Base.IOutputReader INode.Log
        {
            get { return platform.Log; }
        }

        #endregion
    }
}
