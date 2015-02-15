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

namespace QS.Fx.Clock
{
    /// <summary>
    /// This class represents a scheduled or unscheduled timer event.
    /// </summary>
    public interface IAlarm : IDisposable
    {
        /// <summary>
        /// The time at which the event is scheduled to run.
        /// </summary>
		double Time
		{
			get;
		}

        /// <summary>
        /// The original timeout that the event was configured with.
        /// </summary>
		double Timeout
		{
			get;
		}

        /// <summary>
        /// Indicates whether the event has fired.
        /// </summary>
		bool Completed
		{
			get;
		}

        /// <summary>
        /// Indicates whether the event has been cancelled.
        /// </summary>
		bool Cancelled
		{
			get;
		}

        /// <summary>
        /// The context object provided at the time the event was scheduled (or modified by the application thereafter).
        /// </summary>
		object Context
		{
			get;
            set;
		}

        /// <summary>
        /// Reschedule the event for execution with the same timeout.
        /// </summary>
		void Reschedule();

        /// <summary>
        /// Reschedule the event for execution with the new timeout.
        /// </summary>
        /// <param name="timeout">The new timeout.</param>
		void Reschedule(double timeout);

        /// <summary>
        /// Cancel the timer event.
        /// </summary>
		void Cancel();
    }
}
