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

namespace QS._core_c_.Core
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Alarm : QS.Fx.Clock.IAlarm, IBSTNode<double, Alarm> //, QS.Fx.QS.Fx.Clock.IAlarm
    {
/*
        public Alarm(IAlarmController owner, double time, double timeout, QS.Fx.QS.Fx.Clock.AlarmCallback callback, Object context)
            : this(owner, time, timeout, (QS.Fx.QS.Fx.Clock.AlarmCallback)null, context)
        {
            this.baseCallback = callback;
        }
*/

        public Alarm(IAlarmController owner, double time, double timeout, QS.Fx.Clock.AlarmCallback callback, Object context)
		{
			this.owner = owner;
			this.time = time;
			this.timeout = timeout;
			this.callback = callback;
			this.context = context;
//            this.registered = false;
//			this.completed = false;
//			this.cancelled = false;
		}

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #region IDisposable Members
        
        void IDisposable.Dispose()
        {
            if (registered)
                owner.Unregister(this);
        }

        #endregion

		#region IAlarm Members

        double QS.Fx.Clock.IAlarm.Time
		{
			get { return time; }
		}

        double QS.Fx.Clock.IAlarm.Timeout
		{
			get { return timeout; }
		}

        bool QS.Fx.Clock.IAlarm.Completed
		{
			get { return completed; }
		}

        bool QS.Fx.Clock.IAlarm.Cancelled
		{
			get { return cancelled; }
		}

        Object QS.Fx.Clock.IAlarm.Context
		{
			get { return context; }
			set { context = value; }
		}

        void QS.Fx.Clock.IAlarm.Reschedule()
		{
            if (registered)
                owner.Unregister(this);
            completed = cancelled = false;
            owner.Register(this);
        }

        void QS.Fx.Clock.IAlarm.Reschedule(double timeout) 
		{ 
			this.timeout = timeout;
            ((QS.Fx.Clock.IAlarm)this).Reschedule();
		}

        void QS.Fx.Clock.IAlarm.Cancel()
		{
            cancelled = true;
            if (registered)
                owner.Unregister(this);
		}

		#endregion

        #region Accessors

        public object Context
        {
            get { return context; }
        }

        public double Time
        {
            get { return time; }
            set { time = value; }
        }

        public double Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public QS.Fx.Clock.AlarmCallback Callback
		{
			get { return callback; }
        }

/*
        public QS.Fx.QS.Fx.Clock.AlarmCallback Callback2
        {
            get { return baseCallback; }
        }
*/

        public bool Registered
        {
            get { return registered; }
            set { registered = value; }
        }

        #endregion

        #region IBTNode Members

        Alarm IBTNode<Alarm>.Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		Alarm IBTNode<Alarm>.Left
		{
			get { return left; }
			set { left = value; }
		}

		Alarm IBTNode<Alarm>.Right
		{
			get { return right; }
			set { right = value; }
		}

        #endregion

        #region ISNode Members

		double ISNode<double>.Key
		{
			get { return time; }
			set { time = value; }
		}

        #endregion

        #region IComparable<double> Members

        int IComparable<double>.CompareTo(double key)
		{
			return time.CompareTo(key);
		}

		#endregion

        #region Firing

        public void Fire()
        {
            if (!completed)
            {
                completed = true;

                if (!cancelled)
                {
                    if (callback != null)
                        callback(this);

/*
                    if (baseCallback != null)
                        baseCallback(this);
*/ 
                }
            }
            else
                throw new Exception("Canot fire, this alarm has already completed once and has not been rescheduled since.");
        }

        #endregion

/*
        #region QS.Fx.QS.Fx.Clock.IAlarm members

        void QS.Fx.QS.Fx.Clock.IAlarm.cancel()
        {
            ((QS.Fx.QS.Fx.Clock.IAlarm)this).Cancel();
        }

		double QS.Fx.QS.Fx.Clock.IAlarm.Interval
		{
			get { return timeout; }
		}

		Object QS.Fx.QS.Fx.Clock.IAlarm.Argument 
		{
			get { return context; }
		}

		void QS.Fx.QS.Fx.Clock.IAlarm.reschedule()
		{
            ((QS.Fx.QS.Fx.Clock.IAlarm)this).Reschedule();
		}

		void QS.Fx.QS.Fx.Clock.IAlarm.reschedule(double interval)
		{
            ((QS.Fx.QS.Fx.Clock.IAlarm)this).Reschedule(interval);
		}

		#endregion
*/

        public bool Cancelled
        {
            get { return cancelled; }
        }

        [QS.Fx.Printing.NonPrintable]
        private IAlarmController owner;
        [QS.Fx.Printing.NonPrintable]
        private Alarm parent, left, right;
                
        [QS.Fx.Printing.Printable]
        private Object context;

//        [QS.Fx.Printing.Printable]
//        private string _context
//        {
//            get { return /* context.ToString() + " : " + */ context.GetType().ToString(); }
//        }

        [QS.Fx.Printing.Printable]
        private double time, timeout;
        [QS.Fx.Printing.Printable]
		private bool registered, completed, cancelled;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Clock.AlarmCallback callback;

/*
        [QS.Fx.Printing.Printable]
        private QS.Fx.QS.Fx.Clock.AlarmCallback baseCallback;
*/ 
    }
}
