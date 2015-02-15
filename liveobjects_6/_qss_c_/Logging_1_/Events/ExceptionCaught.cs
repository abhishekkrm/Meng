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
    [EventName("Exception Caught")]
    public class ExceptionCaught : SimpleEvent1
    {
        public ExceptionCaught(double time, object location, System.Exception exception)
            : this(time, location, exception, null)
        {
        }

        public ExceptionCaught(double time, object location, System.Exception exception, string additionalComment)
            : base(time, location, exception.Source, exception.Message)
        {
            this.exception = exception;
            this.additionalComment = additionalComment;
        }

        private System.Exception exception;
        private string additionalComment;
        
        public static readonly new EventClass EventClass = new EventClass(typeof(ExceptionCaught));

        protected override QS.Fx.Logging.IEventClass ClassOf()
        {
            return EventClass;
        }

        protected override object PropertyOf(string property)
        {
            return EventClass.PropertyOf(this, property);
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("Exception:\n");
            s.AppendLine(exception.ToString());
            s.AppendLine("\nStack Trace:\n");
            s.AppendLine(exception.StackTrace);
            if (additionalComment != null)
            {
                s.AppendLine("\nComments:\n");
                s.AppendLine(additionalComment);
            }
            return s.ToString();
        }
    }
}
