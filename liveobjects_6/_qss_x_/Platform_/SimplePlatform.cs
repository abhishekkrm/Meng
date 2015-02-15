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
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_x_.Platform_
{
    public class SimplePlatform : QS.Fx.Inspection.Inspectable, QS.Fx.Platform.IPlatform, QS._core_c_.Diagnostics2.IModule, QS.Fx.Scheduling.IScheduler, QS.Fx.Clock.IAlarmClock
    {
        public SimplePlatform(
            QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Clock.IClock clock, string fsroot)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.clock = clock;
            this.alarm = this;
            //filesystem = new QS._qss_x_.Filesystem_.PhysicalFilesystem(core, fsroot);
            //networkConnection = new QS._qss_x_.Network_.PhysicalNetworkConnection(core);

            
            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Logging.ILogger logger;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Logging.IEventLogger eventLogger;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock clock;
        [QS.Fx.Base.Inspectable]
        private bool working;
        [QS.Fx.Base.Inspectable]
        private double timestampStarted, timeSlept, accumulatedWorkTime = 0, accumulatedSleepTime = 0;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarmClock alarm;
        [QS.Fx.Base.Inspectable]
        private System.Collections.Generic.Queue<ScheduledCall> scheduledCalls = new Queue<ScheduledCall>();
        [QS.Fx.Base.Inspectable]
        private AsyncCallback eventcallback;

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IPlatform Members

        QS.Fx.Clock.IClock QS.Fx.Platform.IPlatform.Clock
        {
            get { return clock; }
        }

        QS.Fx.Clock.IAlarmClock QS.Fx.Platform.IPlatform.AlarmClock
        {
            get { return this; }
        }

        QS.Fx.Scheduling.IScheduler QS.Fx.Platform.IPlatform.Scheduler
        {
            get { return this; }
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
            get { throw new NotImplementedException("SimplePlatform does not support filesystem access"); }
        }

        QS.Fx.Network.INetworkConnection QS.Fx.Platform.IPlatform.Network
        {
            get { throw new NotImplementedException("SimplePlatform does not support network"); }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // core.Dispose();
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        
        #region Internal Processing

        private void executeCall(ScheduledCall scheduledCall)
        {
            //if (scheduledCall.CPUIncarnation == this.cpu_incarnation)
            //{
                working = true;
                timestampStarted = clock.Time;
                timeSlept = 0;

#if DEBUG_SimplePlatform
                logger.Log(this, "__ExecuteCall: " + scheduledCall.ToString());
#endif
                Monitor.Exit(this);

                try
                {
                    scheduledCall.Callback(scheduledCall);
                }
                finally
                {
                    Monitor.Enter(this);
                }

                scheduledCall.Completed = true;
                double timeConsumed = TimeOffset;
                accumulatedWorkTime += timeConsumed - timeSlept;
                accumulatedSleepTime += timeSlept;

                alarm.Schedule(
                    timeConsumed, new QS.Fx.Clock.AlarmCallback(processingCompleteCallback), null);
//            }
//            else
//            {
//#if DEBUG_SimplePlatform
//                logger.Log(this, "Call " + scheduledCall.ToString() + " is not executed because it was scheduled in old incarnation " + 
//                    scheduledCall.CPUIncarnation.ToString() + " of the CPU, while the current incarnation is " + this.cpu_incarnation.ToString() + ".");
//#endif
//            }
        }

        private void processingCompleteCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                working = false;
                if (scheduledCalls.Count > 0)
                    executeCall(scheduledCalls.Dequeue());
            }
        }

        private IAsyncResult _beginexecute(AsyncCallback callback, object asynchronousState)
        {
            ScheduledCall scheduledCall = new ScheduledCall(callback, asynchronousState);
            if (working)
                scheduledCalls.Enqueue(scheduledCall);
            else
                executeCall(scheduledCall);
            return scheduledCall;
        }

        private double TimeOffset
        {
            get { return timeSlept + clock.Time - timestampStarted; }
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
            this._beginexecute(this.eventcallback, e);
        }

        IAsyncResult QS.Fx.Scheduling.IScheduler.BeginExecute(AsyncCallback callback, object context)
        {


            IAsyncResult result;
            lock (this)
            {
                result = _beginexecute(callback, context);
            }
            return result;
        }

        void QS.Fx.Scheduling.IScheduler.EndExecute(IAsyncResult result)
        {
            
        }

        void QS.Fx.Scheduling.IScheduler.Execute(AsyncCallback callback, object context)
        {
            lock (this)
            {
                _beginexecute(callback, context);
            }
        }

        #endregion

        #region Class ScheduledCall

        private class ScheduledCall : IAsyncResult
        {
            public ScheduledCall( AsyncCallback callback, object asynchronousState)
            {
                
                this.Callback = callback;
                this.AsynchronousState = asynchronousState;
            }

            public int CPUIncarnation;
            public AsyncCallback Callback;
            public object AsynchronousState;
            public bool Completed = false;

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get { return AsynchronousState; }
            }

            System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { throw new NotSupportedException(); }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return Completed; }
            }

            #endregion

            public override string ToString()
            {
                return QS._core_c_.Helpers.ToString.Delegate(Callback) + ", " + QS._core_c_.Helpers.ToString.Object(AsynchronousState);
            }
        }

        #endregion

        #region Class AlarmWrapper

        private class AlarmWrapper : QS.Fx.Clock.IAlarm
        {
            public AlarmWrapper(SimplePlatform owner, double timeSpanInSeconds, QS.Fx.Clock.AlarmCallback alarmCallback, object argument)
            {
                this.owner = owner;
                
                this.timespan = timeSpanInSeconds;
                this.alarmCallback = alarmCallback;
                this.argument = argument;

                //shouldBeLogged = !Logging_1_.IgnoreCallbacksAttribute.IsDefined(alarmCallback);
            }

            private SimplePlatform owner;
            private int cpu_incarnation;
            private QS.Fx.Clock.AlarmCallback alarmCallback;
            private bool shouldBeLogged;
            private object argument;
            private double timespan;
            private QS.Fx.Clock.IAlarm underlyingAlarmRef;

            public override string ToString()
            {
                return "(" + timespan.ToString() + ", " + QS._core_c_.Helpers.ToString.Delegate(alarmCallback) + "(" + QS._core_c_.Helpers.ToString.Object(argument) + "), " +
                    timespan.ToString() + ")";
            }

            #region Internal Processing

            public void schedule()
            {
                lock (this)
                {
                    underlyingAlarmRef =
                        owner.alarm.Schedule(timespan + owner.TimeOffset,
                            new QS.Fx.Clock.AlarmCallback(underlyingAlarmCallback), null);
                }
            }

            private void underlyingAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (owner)
                {
                    
                        owner._beginexecute(executionCallback, null);
                    
                }
            }

            private void executionCallback(IAsyncResult asynchronousResult)
            {
#if DEBUG_EnableLoggingOfAlarmCallbacks
                if (shouldBeLogged && owner.eventLogger.Enabled)
                {
                    try
                    {
                        owner.eventLogger.Log(
                            new Logging.Events.ApplicationAlarm(((QS.Fx.QS.Fx.Clock.IClock)owner).Time, owner.name, owner, alarmCallback, this));
                    }
                    catch (Exception exc)
                    {
                        owner.logger.Log(this, "Could not log alarm callback.\n" + exc.ToString());
                    }
                }
#endif

                alarmCallback(this);
            }

            #endregion

            #region IAlarm Members

            double QS.Fx.Clock.IAlarm.Time
            {
                get { throw new NotImplementedException(); }
            }

            double QS.Fx.Clock.IAlarm.Timeout
            {
                get { return timespan; }
            }

            bool QS.Fx.Clock.IAlarm.Completed
            {
                get { return underlyingAlarmRef.Completed; }
            }

            bool QS.Fx.Clock.IAlarm.Cancelled
            {
                get { return underlyingAlarmRef.Cancelled; }
            }

            object QS.Fx.Clock.IAlarm.Context
            {
                get { return argument; }
                set { argument = value; }
            }

            void QS.Fx.Clock.IAlarm.Reschedule()
            {
                ((QS.Fx.Clock.IAlarm)this).Reschedule(timespan);
            }

            void QS.Fx.Clock.IAlarm.Reschedule(double timeout)
            {
                lock (this)
                {
                    underlyingAlarmRef.Reschedule(timeout + owner.TimeOffset);
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
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        #region IAlarmClock Members

        QS.Fx.Clock.IAlarm QS.Fx.Clock.IAlarmClock.Schedule(double timeSpanInSeconds, QS.Fx.Clock.AlarmCallback alarmCallback, object argument)
        {
            AlarmWrapper alarmWrapper;

            lock (this)
            {
                alarmWrapper = new AlarmWrapper(this, timeSpanInSeconds, alarmCallback, argument);
                alarmWrapper.schedule();
            }

            return alarmWrapper;
        }

        #endregion
    }
}
