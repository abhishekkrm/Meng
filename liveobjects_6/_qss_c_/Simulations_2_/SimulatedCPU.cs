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

// #define DEBUG_SimulatedCPU
// #define DEBUG_EnableLoggingOfAlarmCallbacks

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#endregion

namespace QS._qss_c_.Simulations_2_
{
	public class SimulatedCPU : QS.Fx.Inspection.Inspectable, ISimulatedCPU
    {
        #region Constructor

        public SimulatedCPU(QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Clock.IClock simulatedClock, QS.Fx.Clock.IAlarmClock simulatedAlarmClock)
            : this(eventLogger, QS._core_c_.Base2.PreciseClock.Clock, simulatedClock, simulatedAlarmClock)
		{
		}

        public SimulatedCPU(QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Clock.IClock physicalClock, QS.Fx.Clock.IClock simulatedClock, QS.Fx.Clock.IAlarmClock simulatedAlarmClock)
		{
            this.eventcallback = new AsyncCallback(this._EventCallback);

            this.eventLogger = eventLogger;

			this.physicalClock = physicalClock;
			this.simulatedClock = simulatedClock;
			this.simulatedAlarmClock = simulatedAlarmClock;

			logger = new QS._qss_c_.Base3_.Logger(this, true, null);
        }

        #endregion

        #region Fields

        private QS.Fx.Logging.IEventLogger eventLogger;
		[QS.Fx.Base.Inspectable("Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._core_c_.Base.IReadableLogger logger;
		private QS.Fx.Clock.IClock physicalClock, simulatedClock;
		private QS.Fx.Clock.IAlarmClock simulatedAlarmClock;
		[QS.Fx.Base.Inspectable("IsWorking", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private bool working;
		private double timestampStarted, timeSlept, accumulatedWorkTime = 0, acumulatedSleepTime = 0;
		[QS.Fx.Base.Inspectable("Scheduled Calls", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private System.Collections.Generic.Queue<ScheduledCall> scheduledCalls = new Queue<ScheduledCall>();
		private string name = string.Empty;

        private int cpu_incarnation; // used to implement resetting
        private AsyncCallback eventcallback;

        #endregion

        #region Accessors

        /*
        public QS._core_c_.Base3.InstanceID Location
        {
            set { location = value; }
            get { return location; }
        }
*/

        #endregion

        #region Class ScheduledCall

        private class ScheduledCall : IAsyncResult
		{
			public ScheduledCall(int cpu_incarnation, AsyncCallback callback, object asynchronousState)
			{
                this.CPUIncarnation = cpu_incarnation;
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

		#region Internal Processing

		private void executeCall(ScheduledCall scheduledCall)
		{
            if (scheduledCall.CPUIncarnation == this.cpu_incarnation)
            {
                working = true;
                timestampStarted = physicalClock.Time;
                timeSlept = 0;

#if DEBUG_SimulatedCPU
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
                acumulatedSleepTime += timeSlept;

                simulatedAlarmClock.Schedule(
                    timeConsumed, new QS.Fx.Clock.AlarmCallback(processingCompleteCallback), null);
            }
            else
            {
#if DEBUG_SimulatedCPU
                logger.Log(this, "Call " + scheduledCall.ToString() + " is not executed because it was scheduled in old incarnation " + 
                    scheduledCall.CPUIncarnation.ToString() + " of the CPU, while the current incarnation is " + this.cpu_incarnation.ToString() + ".");
#endif
            }
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

		private double TimeOffset
		{
			get { return timeSlept + physicalClock.Time - timestampStarted; }
		}

        private IAsyncResult _beginexecute(AsyncCallback callback, object asynchronousState)
        {
            ScheduledCall scheduledCall = new ScheduledCall(cpu_incarnation, callback, asynchronousState);
            if (working)
                scheduledCalls.Enqueue(scheduledCall);
            else
                executeCall(scheduledCall);
            return scheduledCall;
        }

		#endregion

		#region ISimulatedCPU Members

		string ISimulatedCPU.Name
		{
			get { return name; }
			set { name = value; }
		}

        void QS.Fx.Scheduling.IScheduler.Execute(AsyncCallback callback, object asynchronousState)
        {
            lock (this)
            {
                _beginexecute(callback, asynchronousState);
            }
        }

		IAsyncResult QS.Fx.Scheduling.IScheduler.BeginExecute(AsyncCallback callback, object asynchronousState)
		{
            IAsyncResult result;
			lock (this)
			{
                result = _beginexecute(callback, asynchronousState);
			}
			return result;
		}

        void QS.Fx.Scheduling.IScheduler.EndExecute(IAsyncResult asynchronousResult)
		{
		}

//		double ISimulatedCPU.AccumulatedWorkTime
//		{
//			get { return accumulatedWorkTime; }
//		}

		void ISimulatedCPU.Sleep(double time)
		{
			lock (this)
			{
				timeSlept += time;
			}
		}

        void ISimulatedCPU.Reset()
        {
            lock (this)
            {
#if DEBUG_SimulatedCPU
                logger.Log(this, "Advancing CPU incarnation from " + cpu_incarnation.ToString() + " to " + (cpu_incarnation + 1).ToString() + ".");
#endif

                cpu_incarnation++;
            }
        }

		#endregion

		#region IClock Members

		double QS.Fx.Clock.IClock.Time
		{
			get { return simulatedClock.Time + TimeOffset; }
		}

		#endregion

		#region Class AlarmWrapper

        private class AlarmWrapper : QS.Fx.Clock.IAlarm
		{
			public AlarmWrapper(SimulatedCPU owner, double timeSpanInSeconds, QS.Fx.Clock.AlarmCallback alarmCallback, object argument)
			{
				this.owner = owner;
                this.cpu_incarnation = owner.cpu_incarnation;
				this.timespan = timeSpanInSeconds;
				this.alarmCallback = alarmCallback;
				this.argument = argument;

                shouldBeLogged = !Logging_1_.IgnoreCallbacksAttribute.IsDefined(alarmCallback);
			}

			private SimulatedCPU owner;
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
						owner.simulatedAlarmClock.Schedule(timespan + owner.TimeOffset, 
						    new QS.Fx.Clock.AlarmCallback(underlyingAlarmCallback), null);
				}
			}

			private void underlyingAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
                lock (owner)
                {
                    if (this.cpu_incarnation == owner.cpu_incarnation)
                        owner._beginexecute(executionCallback, null);
                    else
                    {
#if DEBUG_SimulatedCPU
                        owner.logger.Log(this, "Alarm was not fired because it was initiated by an old incarnation " + this.cpu_incarnation.ToString() + 
                            " of the CPU, while the current incarnation of the CPU is " + owner.cpu_incarnation + ".");
#endif
                    }
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

        #region Disposing

/*
		#region IDisposable Members

		void IDisposable.Dispose()
		{
		}

		#endregion
*/

        #endregion

        #region IManagedComponent Members

        string QS._qss_e_.Management_.IManagedComponent.Name
		{
			get { return "Simulated CPU"; }
		}

		QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
		{
			get { return null; } //  new QS.TMS.Management.IManagedComponent[] {};
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

        #region ToString

        public override string ToString()
		{
/*
            StringBuilder s = new StringBuilder();
            if (location != null)
            {
                s.Append(location.ToString());
                if (name != null)
                {
                    s.Append(" ");
                    s.Append(name);
                }
            }
            else 
                if (name != null) 
                    s.Append(name);
            return s.ToString();
*/

            return name != null ? name : "";
        }

        #endregion

        #region _EventCallback

        private void _EventCallback(IAsyncResult asyncresult)
        {
            ((QS.Fx.Base.IEvent) asyncresult.AsyncState).Handle();
        }

        #endregion

        #region IScheduler Members

        void QS.Fx.Scheduling.IScheduler.Schedule(QS.Fx.Base.IEvent e)
        {
            this._beginexecute(this.eventcallback, e);            
        }

        #endregion
    }
}
