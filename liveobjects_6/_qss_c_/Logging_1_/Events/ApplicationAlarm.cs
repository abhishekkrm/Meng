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

namespace QS._qss_c_.Logging_1_.Events
{
    [EventName("Application Alarm")] [QS.Fx.Printing.Printable("Application Alarm", QS.Fx.Printing.PrintingStyle.Expanded)]
    public class ApplicationAlarm : GenericEvent
    {
        public ApplicationAlarm(double time, object location, object source, 
            QS.Fx.Clock.AlarmCallback alarmCallback, QS.Fx.Clock.IAlarm alarmRef) : base(time, location, source)
        {
            this.callback = alarmCallback;
            this.interval = alarmRef.Timeout;
            this.argument = alarmRef.Context != null ? alarmRef.Context.ToString() : null;
        }

        private QS.Fx.Clock.AlarmCallback callback;
        [EventProperty] [QS.Fx.Printing.Printable]
        private string Class
        {
            get { return callback.Target.GetType().ToString(); }
        }

        [EventProperty] [QS.Fx.Printing.Printable]
        private string Method
        {
            get { return callback.Method.Name; }
        }

        [EventProperty] [QS.Fx.Printing.Printable]
        private string Target
        {
            get { return callback.Target.ToString(); }
        }

        [EventProperty("Interval")] [QS.Fx.Printing.Printable("Interval")]
        private double interval;

        [EventProperty("Argument")] [QS.Fx.Printing.Printable("Argument")]
        private string argument;

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        public static readonly EventClass EventClass = new EventClass(typeof(ApplicationAlarm));

        protected override QS.Fx.Logging.IEventClass ClassOf()
        {
            return EventClass;    
        }

        protected override string DescriptionOf()
        {
            return "Alarm Fired";
        }

        protected override object PropertyOf(string property)
        {
            return EventClass.PropertyOf(this, property);
        }
    }
}
