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

namespace QS._qss_c_.Logging_1_
{
    public abstract class GenericEvent : QS.Fx.Logging.IEvent
    {
        public GenericEvent() : this(double.NaN, null, null)
        {
        }

        public GenericEvent(object source) : this(double.NaN, null, source)
        {
        }

        public GenericEvent(double time, object source) : this(time, null, source)
        {
        }

        public GenericEvent(double time, object location, object source)
        {
            this.time = time;
            this.location = location;
            this.source = source;
        }

        private double time;
        private object location, source;

        #region Abstract and Virtual Members

        protected abstract QS.Fx.Logging.IEventClass ClassOf();        
        protected abstract string DescriptionOf();
        protected abstract object PropertyOf(string property);

        #endregion

        #region IEvent Members

        string QS.Fx.Logging.IEvent.Location
        {
            get { return location != null ? location.ToString() : null; }
            set { location = value; }
        }

        double QS.Fx.Logging.IEvent.Time
        {
            get { return time; }
            set { time = value; }
        }

        QS.Fx.Logging.IEventClass QS.Fx.Logging.IEvent.Class
        {
            get { return this.ClassOf(); }
        }

        string QS.Fx.Logging.IEvent.Source
        {
            get { return source != null ? source.ToString() : null; }
        }

        string QS.Fx.Logging.IEvent.Description
        {
            get { return this.DescriptionOf(); }
        }

        object QS.Fx.Logging.IEvent.this[string property]
        {
            get { return this.PropertyOf(property); }
        }

        #endregion
    }
}
