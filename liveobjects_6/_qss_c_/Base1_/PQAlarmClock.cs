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

// #define DEBUG_PQAlarmClock

using System;
using System.Threading;

namespace QS._qss_c_.Base1_
{
	/// <summary>
	/// Simple alarm clock based on a prority queue.
	/// </summary>
    public sealed class PQAlarmClock : QS.Fx.Clock.IAlarmClock, System.IDisposable
	{
		public PQAlarmClock(Collections_1_.IPriorityQueue underlyingQueue, QS.Fx.Logging.ILogger logger) : this(underlyingQueue, logger, false)
		{
		}

		public PQAlarmClock(Collections_1_.IPriorityQueue underlyingQueue, QS.Fx.Logging.ILogger logger, bool waitingUntilAllDone)
		{
			this.underlyingQueue = underlyingQueue;
			this.logger = logger;
			mythread = new Thread(new ThreadStart(this.mainloop));
			finishing = false;
			checkAgain = new AutoResetEvent(false);
			soonestFiringTime = _Now;
			allDone = new ManualResetEvent(true);
			this.waitingUntilAllDone = waitingUntilAllDone;
			mythread.Start();
		}

		public System.Threading.WaitHandle AllDone
		{
			get
			{
				return this.allDone;
			}
		}

		public void shutdown()
		{
			try
			{
				finishing = true;
				checkAgain.Set();
				mythread.Join(TimeSpan.FromMilliseconds(100));
				if (mythread.IsAlive)
				{
					logger.Log(this, "forcifully aborting the internal working thread after 100ms of unresponsiveness");
					mythread.Abort();
				}
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}
		}

		private bool finishing;
		private AutoResetEvent checkAgain;
		private Collections_1_.IPriorityQueue underlyingQueue;
		private Thread mythread;
		private double soonestFiringTime;
		private QS.Fx.Logging.ILogger logger;
		private System.Threading.ManualResetEvent allDone;
		private bool waitingUntilAllDone;

        private static double _Now
        {
            get { return DateTime.Now.Ticks / 10000000; }
        }

		private void mainloop()
		{
			while (!finishing)
			{
                try
                {
                    double mainloopWaitingTime = 60;

                    lock (this)
                    {
                        while (!underlyingQueue.isempty())
                        {
                            AlarmRef alarmRef = (AlarmRef)underlyingQueue.findmin();
                            double currentTime = _Now;
                            if (((QS.Fx.Clock.IAlarm) alarmRef).Time < currentTime)
                            {
                                underlyingQueue.deletemin();

#if DEBUG_PQAlarmClock
							logger.Log(this, "firing alarm secheduled for " + alarmRef.FiringTime.ToLongTimeString());
#endif

                                Monitor.Exit(this);

                                try
                                {
                                    alarmRef.fire();
                                }
                                catch (Exception exc)
                                {
                                    try
                                    {
                                        logger.Log(this, "__MainLoop, when firing @ " + ((QS.Fx.Clock.IAlarm) alarmRef).Time.ToString() + " : " + exc.ToString());
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                Monitor.Enter(this);
                            }
                            else
                            {
                                mainloopWaitingTime = ((QS.Fx.Clock.IAlarm) alarmRef).Time - currentTime;
                                break;
                            }
                        }

                        soonestFiringTime = _Now + mainloopWaitingTime;

                        if (underlyingQueue.isempty())
                            allDone.Set();
                    }

#if DEBUG_PQAlarmClock
				logger.Log(this, "waiting " + mainloopWaitingTime.ToString() + " before next alarm");
#endif

                    checkAgain.WaitOne(TimeSpan.FromSeconds(mainloopWaitingTime), false);
                }
                catch (Exception exc)
                {
                    logger.Log(this, "__MainLoop: " + exc.ToString());
                }
            }
		}

		#region IAlarmClock Members

		QS.Fx.Clock.IAlarm QS.Fx.Clock.IAlarmClock.Schedule(double timeSpan, QS.Fx.Clock.AlarmCallback alarmCallback, object argument)
		{
			AlarmRef alarmRef = new AlarmRef(argument, timeSpan, alarmCallback, this);

#if DEBUG_PQAlarmClock
			logger.Log(this, "scheduled a new alarm for " + alarmRef.FiringTime.ToLongTimeString());
#endif

			this.installNewAlarm(alarmRef);

			return alarmRef;
		}

		private void installNewAlarm(AlarmRef alarmRef)
		{
			lock (this)
			{
				underlyingQueue.insert(alarmRef);
				allDone.Reset();

				if (((QS.Fx.Clock.IAlarm) alarmRef).Time < soonestFiringTime)
					checkAgain.Set();
			}		
		}

		public void cancelAlarm(QS.Fx.Clock.IAlarm iref)
		{
			AlarmRef alarmRef = (AlarmRef) iref;
			((QS.Fx.Clock.IAlarm) alarmRef).Cancel();
		}

		#endregion

        private class AlarmRef : QS.Fx.Clock.IAlarm, System.IComparable
		{
			public AlarmRef(object argument, double timeSpan, QS.Fx.Clock.AlarmCallback callback, PQAlarmClock alarmClock)
			{
				this.argument = argument;
				this.callback = callback;
				this.timeSpan = timeSpan;
				this.firingTime = _Now + timeSpan;
				this.cancelled = false;
				this.alarmClock = alarmClock;
			}

			private object argument;
			private QS.Fx.Clock.AlarmCallback callback;
			private double timeSpan, firingTime;
			private bool cancelled;
			private PQAlarmClock alarmClock;

			public override string ToString()
			{
				System.Text.StringBuilder s = new System.Text.StringBuilder();
				s.AppendLine("FiringTime: " + firingTime.ToString());
				s.AppendLine("TimeSpan: " + timeSpan.ToString());
				s.AppendLine("Callback: " + QS._core_c_.Helpers.ToString.ObjectRef(callback));
				s.AppendLine("Argument: " + QS._core_c_.Helpers.ToString.ObjectRef(argument));
				s.AppendLine("Cancelled: " + cancelled.ToString());
				return s.ToString();
			}

			public void fire()
			{
				if (!cancelled)
				{
					callback(this);
				}
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				if (obj is AlarmRef)
					return firingTime.CompareTo(((AlarmRef) obj).firingTime);
				else
					throw new Exception("incompatible object in comparison");
			}

			#endregion

            #region IAlarm Members

            double QS.Fx.Clock.IAlarm.Time
            {
                get { return firingTime; }
            }

            double QS.Fx.Clock.IAlarm.Timeout
            {
//                set { timeSpan = value; }
                get { return timeSpan; }
            }

            bool QS.Fx.Clock.IAlarm.Completed
            {
                get { throw new NotImplementedException(); }
            }

            bool QS.Fx.Clock.IAlarm.Cancelled
            {
                get { return cancelled; }
            }

            object QS.Fx.Clock.IAlarm.Context
            {
                get { return argument; }
                set { argument = value; }
            }

            void QS.Fx.Clock.IAlarm.Reschedule()
            {
                lock (this)
                {
                    firingTime = _Now + timeSpan;
                    alarmClock.installNewAlarm(this);
                }
            }

            void QS.Fx.Clock.IAlarm.Reschedule(double timeout)
            {
                lock (this)
                {
                    this.timeSpan = timeout;
                    firingTime = _Now + timeSpan;
                    alarmClock.installNewAlarm(this);
                }
            }

            void QS.Fx.Clock.IAlarm.Cancel()
            {
                lock (this)
                {
#if DEBUG_PQAlarmClock
					alarmClock.logger.Log(this, "cancelling alarm scheduled for " + firingTime.ToLongTimeString());
#endif

                    cancelled = true;
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

		#region IDisposable Members

		public void Dispose()
		{
			if (this.waitingUntilAllDone)
				allDone.WaitOne();
			this.shutdown();
		}

		#endregion
	}
}
