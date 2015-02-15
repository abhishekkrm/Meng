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

// #define DEBUG_SimulatedClock

using System;
using System.Diagnostics;

namespace QS._qss_c_.Simulations_1_
{
	/// <summary>
	/// Summary description for SimulatedClock.
	/// </summary>
    public class SimulatedClock : QS.Fx.Inspection.Inspectable, QS.Fx.Clock.IAlarmClock, QS.Fx.Clock.IClock
	{
		public SimulatedClock(QS.Fx.Logging.ILogger logger) // , Collections_1_.IPriorityQueue underlyingPriorityQueue)
		{
            this.underlyingPriorityQueue = new QS._core_c_.Core.SplayTree<double, ScheduledAlarm>(); // underlyingPriorityQueue;
			this.currentTime = 0;
            this.logger = logger;
        }

		[QS.Fx.Base.Inspectable("Event Queue", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Core.IPriorityQueue<ScheduledAlarm> underlyingPriorityQueue; // Collections_1_.IPriorityQueue 
		[QS.Fx.Base.Inspectable("Current Time", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private double currentTime;
		private QS.Fx.Logging.ILogger logger;

		public void Clear()
		{
			lock (this)
			{
				underlyingPriorityQueue.Clear();
			}
		}

		public int QueueSize
		{
			get 
			{
				lock (this)
				{
                    return underlyingPriorityQueue.Count;
					// return (int) underlyingPriorityQueue.size();
				}
			}
		}

		private System.Object advancelock = new Object();
		public void advance()
		{
			lock (advancelock)
			{
				ScheduledAlarm alarm = null;
				lock (this)
				{
					while (!underlyingPriorityQueue.IsEmpty) // (!underlyingPriorityQueue.isempty())
					{
                        alarm = underlyingPriorityQueue.Dequeue(); // (ScheduledAlarm) underlyingPriorityQueue.deletemin();
                        alarm.isscheduled = false;
						if (alarm != null)
						{
							if (!alarm.Cancelled)
							{
                                if (alarm.FiringTime < currentTime)
                                {
                                    string error_message = "SimulatedClock: Alarm set in the past: CurrentTime = " +
                                        currentTime.ToString() + ", Alarm = " + alarm.ToString();
                                    // logger.Log(this, "__advance: " + error_message);
                                    throw new Exception(error_message);
                                }

                                currentTime = alarm.FiringTime;

#if DEBUG_SimulatedClock
                                logger.Log(this, "Processing_Event: \"" + alarm.ToString() + "\", To_Go: " + underlyingPriorityQueue.size().ToString());
#endif

								// alarm.fire();
								break;
							}
							else
								alarm = null;
						}
					}
				}

				if (alarm != null)
					alarm.fire();
			}
		}

		#region ScheduledAlarm Class

        private class ScheduledAlarm : QS.Fx.Clock.IAlarm, System.IComparable, QS._core_c_.Core.IBSTNode<double, ScheduledAlarm>	
		{
			public ScheduledAlarm(double interval, object argument, QS.Fx.Clock.AlarmCallback alarmCallback, 
				SimulatedClock encapsulatingSimulatedClock)
			{
				if (interval < 0)
					throw new ArgumentException("Interval < 0.");

				this.alarmCallback = alarmCallback;
				this.encapsulatingSimulatedClock = encapsulatingSimulatedClock;
				this.interval = interval;
				this.argument = argument;

				this.firingTime = encapsulatingSimulatedClock.currentTime + interval;
			}

			private QS.Fx.Clock.AlarmCallback alarmCallback;
			private SimulatedClock encapsulatingSimulatedClock;
			private double interval, firingTime;
			private object argument;
			private bool cancelled;

            public bool isscheduled;

            [QS.Fx.Printing.NonPrintable]
            private ScheduledAlarm parent, left, right;

			public void fire()
			{
				alarmCallback(this);
			}

			public bool Cancelled
			{
				get
				{
					return cancelled;
				}
			}

			public double FiringTime
			{
				get
				{
					return this.firingTime;
				}
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				if (obj is ScheduledAlarm)
					return firingTime.CompareTo(((ScheduledAlarm) obj).firingTime);
				else
					throw new Exception("incompatible object in comparison");
			}

			#endregion

            public override string ToString()
            {
                return "[" + firingTime.ToString("000000.000000000") + "] " + QS._core_c_.Helpers.ToString.Delegate(alarmCallback) + "(" + 
					QS._core_c_.Helpers.ToString.ObjectRef(argument) + ")";
            }

            #region IAlarm Members

            double QS.Fx.Clock.IAlarm.Time
            {
                get { return this.firingTime; }
            }

            double QS.Fx.Clock.IAlarm.Timeout
            {
                get { return this.interval; }
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
                get { return this.argument; }
                set { this.argument = value; }
            }

            void QS.Fx.Clock.IAlarm.Reschedule()
            {
                lock (encapsulatingSimulatedClock)
                {
//					lock (this)
//					{
                    this.firingTime = encapsulatingSimulatedClock.currentTime + interval;
                    encapsulatingSimulatedClock.installAlarm(this);
//					}
                }
            }

            void QS.Fx.Clock.IAlarm.Reschedule(double timeout)
            {
                lock (encapsulatingSimulatedClock)
                {
//					lock (this)
//					{
                    if (timeout < 0)
                        throw new ArgumentException("Interval < 0.");
                    this.interval = timeout;

                    this.firingTime = encapsulatingSimulatedClock.currentTime + timeout;
                    encapsulatingSimulatedClock.installAlarm(this);
//					}
                }
            }

            void QS.Fx.Clock.IAlarm.Cancel()
            {
                lock (encapsulatingSimulatedClock)
                {
//					lock (this)
//					{
                    this.cancelled = true;
//					}
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region ISNode<double> Members

            double QS._core_c_.Core.ISNode<double>.Key
            {
                get { return this.firingTime; }
                set { this.firingTime = value; }
            }

            #endregion

            #region IComparable<double> Members

            int IComparable<double>.CompareTo(double other)
            {
                return this.firingTime.CompareTo(other);
            }

            #endregion

            #region IBTNode<ScheduledAlarm> Members

            ScheduledAlarm QS._core_c_.Core.IBTNode<ScheduledAlarm>.Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            ScheduledAlarm QS._core_c_.Core.IBTNode<ScheduledAlarm>.Left
            {
                get { return left; }
                set { left = value; }
            }

            ScheduledAlarm QS._core_c_.Core.IBTNode<ScheduledAlarm>.Right
            {
                get { return right; }
                set { right = value; }
            }

            #endregion
        }

		#endregion

		#region IAlarmClock Members

		private void installAlarm(ScheduledAlarm scheduledAlarm)
		{
			if (scheduledAlarm == null)
				throw new ArgumentException("Argument (scheduledAlarm) should not be NULL.");

            if (scheduledAlarm.isscheduled)
                underlyingPriorityQueue.Remove(scheduledAlarm);

			if (scheduledAlarm.FiringTime < currentTime)
				throw new Exception("Attempting to schedule an alarm in the past: Curent Time = " + currentTime.ToString() +
					", Alarm: " + scheduledAlarm.ToString());

#if DEBUG_SimulatedClock
                logger.Log(this, "Scheduling_Event: \"" + scheduledAlarm.ToString() + 
                    "\", Total_ToGo: " + (underlyingPriorityQueue.size() + 1).ToString());
#endif

            scheduledAlarm.isscheduled = true;
            underlyingPriorityQueue.Enqueue(scheduledAlarm);
            // underlyingPriorityQueue.insert(scheduledAlarm);
		}

		QS.Fx.Clock.IAlarm QS.Fx.Clock.IAlarmClock.Schedule(double timeSpanInSeconds, QS.Fx.Clock.AlarmCallback alarmCallback, object argument)
		{
			lock (this)
			{
				ScheduledAlarm scheduledAlarm = new ScheduledAlarm(timeSpanInSeconds, argument, alarmCallback, this);
				this.installAlarm(scheduledAlarm);
				return scheduledAlarm;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		#region IClock Members

		[QS.Fx.Base.Inspectable]
		public double Time
		{
			get
			{
				lock (this)
				{
					return this.currentTime;
				}
			}
		}

		#endregion
	}
}
