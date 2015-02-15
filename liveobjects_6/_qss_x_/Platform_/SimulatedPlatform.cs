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

namespace QS._qss_x_.Platform_
{
    public class SimulatedPlatform : QS.Fx.Inspection.Inspectable, QS.Fx.Platform.IPlatform, QS.Fx.Clock.IAlarmClock, QS.Fx.Scheduling.IScheduler
    {
        #region Constructor

        public SimulatedPlatform(string name, QS._qss_c_.Simulations_2_.ISimulatedCPU simulatedCPU, 
            QS.Fx.Logging.IEventLogger eventLogger, QS._qss_x_.Network_.IVirtualNetwork network)
        {
            this.eventcallback = new AsyncCallback(this._EventCallback);

            this.name = name;
            this.simulatedCPU = simulatedCPU;
            this.eventLogger = eventLogger;

            logger = new QS._qss_c_.Base3_.Logger(simulatedCPU, true, null);

            disk = new QS._qss_x_.Filesystem_.VirtualDisk(logger, simulatedCPU);
            filesystem = new QS._qss_x_.Filesystem_.VirtualFilesystem(logger, disk);           

            networkConnection = new QS._qss_x_.Network_.VirtualNetworkConnection(name, logger, simulatedCPU, simulatedCPU, simulatedCPU,
                statisticsController, network);
        }

        #endregion

        #region Fields

        private string name;
        private QS._qss_c_.Simulations_2_.ISimulatedCPU simulatedCPU;

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base3_.Logger logger;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Logging.IEventLogger eventLogger;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.MemoryController statisticsController = new QS._qss_c_.Statistics_.MemoryController();
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Filesystem_.VirtualDisk disk;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Filesystem_.VirtualFilesystem filesystem;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Network_.VirtualNetworkConnection networkConnection;
        
        private AsyncCallback eventcallback;

        #endregion

        #region IPlatform Members

        QS.Fx.Clock.IClock QS.Fx.Platform.IPlatform.Clock
        {
            get { return simulatedCPU; }
        }

        QS.Fx.Clock.IAlarmClock QS.Fx.Platform.IPlatform.AlarmClock
        {
            get { return this; } // simulatedCPU; 
        }

        QS.Fx.Logging.ILogger QS.Fx.Platform.IPlatform.Logger
        {
            get { return logger; }
        }

        QS.Fx.Logging.IEventLogger QS.Fx.Platform.IPlatform.EventLogger
        {
            get { return eventLogger; }
        }

        QS.Fx.Filesystem.IFilesystem QS.Fx.Platform.IPlatform.Filesystem
        {
            get { return filesystem; }
        }

        QS.Fx.Network.INetworkConnection QS.Fx.Platform.IPlatform.Network
        {
            get { return networkConnection; }
        }

        QS.Fx.Scheduling.IScheduler QS.Fx.Platform.IPlatform.Scheduler
        {
            get { return this; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO: Fix....................
        }

        #endregion

        #region Reset

        public void Reset()
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Platform.Reset : Beginning");
#endif

            ((Filesystem_.IVirtualDisk) disk).Reset();
            networkConnection.Reset();
            filesystem.Reset();
            simulatedCPU.Reset();
            
            statisticsController.Clear();

#if DEBUG_LogGenerously
            logger.Log(this, "Platform.Reset : Complete.");
#endif
        }

        #endregion

        #region Accessors

        public QS._core_c_.Base.IOutputReader Log
        {
            get { return logger; }
        }

        #endregion

        #region IAlarmClock Members

        QS.Fx.Clock.IAlarm QS.Fx.Clock.IAlarmClock.Schedule(double timeout, QS.Fx.Clock.AlarmCallback callback, object context)
        {
            return new Alarm_(simulatedCPU, this.name, timeout, callback, context);
        }

        private sealed class Alarm_ : QS.Fx.Clock.IAlarm
        {
            #region Constructor

            public Alarm_(QS.Fx.Clock.IAlarmClock underlyingclock, string name, double timeout, QS.Fx.Clock.AlarmCallback callback, object context)
            {
                this.underlyingclock = underlyingclock;
                this.name = name;
                this.callback = callback;
                this.context = context;
                this.alarm = underlyingclock.Schedule(timeout, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), this);
            }

            #endregion

            #region Fields

            private QS.Fx.Clock.IAlarmClock underlyingclock;
            private string name;
            private QS.Fx.Clock.IAlarm alarm;
            private QS.Fx.Clock.AlarmCallback callback;
            private object context;

            #endregion

            #region _AlarmCallback

            private void _AlarmCallback(QS.Fx.Clock.IAlarm _alarm)
            {
                try
                {
                    this.callback(this);
                }
                catch (Exception _exc)
                {
                    throw new Exception("Error while processing a scheduled alarm at \"" + name + "\".", _exc);
                }
            }

            #endregion

            #region IAlarm Members

            double QS.Fx.Clock.IAlarm.Time
            {
                get { return alarm.Time; }
            }

            double QS.Fx.Clock.IAlarm.Timeout
            {
                get { return alarm.Timeout; }
            }

            bool QS.Fx.Clock.IAlarm.Completed
            {
                get { return alarm.Completed; }
            }

            bool QS.Fx.Clock.IAlarm.Cancelled
            {
                get { return alarm.Cancelled; }
            }

            object QS.Fx.Clock.IAlarm.Context
            {
                get { return this.context; }
                set { this.context = value; }
            }

            void QS.Fx.Clock.IAlarm.Reschedule()
            {
                alarm.Reschedule();
            }

            void QS.Fx.Clock.IAlarm.Reschedule(double timeout)
            {
                alarm.Reschedule(timeout);
            }

            void QS.Fx.Clock.IAlarm.Cancel()
            {
                alarm.Cancel();
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                alarm.Dispose();
            }

            #endregion
        }

        #endregion

        #region IScheduler Members

        IAsyncResult QS.Fx.Scheduling.IScheduler.BeginExecute(AsyncCallback callback, object context)
        {
            IAsyncResult asyncresult = new ScheduledCall_(this.simulatedCPU, this.name, callback, context);
            return asyncresult;
        }

        void QS.Fx.Scheduling.IScheduler.EndExecute(IAsyncResult result)
        {
        }

        void QS.Fx.Scheduling.IScheduler.Execute(AsyncCallback callback, object context)
        {
            IAsyncResult asyncresult = new ScheduledCall_(this.simulatedCPU, this.name, callback, context);
        }

        private sealed class ScheduledCall_ : IAsyncResult
        {
            #region Constructor

            public ScheduledCall_(QS.Fx.Scheduling.IScheduler underlyingscheduler, string name, AsyncCallback callback, object context)
            {
                this.underlyingscheduler = underlyingscheduler;
                this.name = name;
                this.callback = callback;
                this.context = context;
                this.underlyingresult = underlyingscheduler.BeginExecute(new AsyncCallback(this._AsyncCallback), null);
            }

            #endregion

            #region Fields

            private QS.Fx.Scheduling.IScheduler underlyingscheduler;
            private string name;
            private AsyncCallback callback;
            private object context;
            private IAsyncResult underlyingresult;

            #endregion

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get { return context; }
            }

            System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return underlyingresult.AsyncWaitHandle; }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return underlyingresult.CompletedSynchronously; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return underlyingresult.IsCompleted; }
            }

            #endregion

            #region _AsyncCallback

            private void _AsyncCallback(IAsyncResult asyncresult)
            {
                try
                {
                    this.callback(this);
                }
                catch (Exception _exc)
                {
                    throw new Exception("Error while processing a scheduled call at \"" + name + "\".", _exc);
                }
            }

            #endregion
        }

        #endregion

        #region _EventCallback

        private void _EventCallback(IAsyncResult asyncresult)
        {
            ((QS.Fx.Base.IEvent)asyncresult.AsyncState).Handle();
        }

        #endregion

        #region IScheduler Members

        void QS.Fx.Scheduling.IScheduler.Schedule(QS.Fx.Base.IEvent e)
        {
            IAsyncResult asyncresult = new ScheduledCall_(this.simulatedCPU, this.name, this.eventcallback, e);
        }

        #endregion
    }
}
