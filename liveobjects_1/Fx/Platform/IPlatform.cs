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

namespace QS.Fx.Platform
{
    /// <summary>
    /// This class represents a simple virtual execution environment. Building the entire software stack on top of this environment
    /// allows the same code to be run in a simulator, emulator, or on physical hardware without changes, and eases the debugging.
    /// 
    /// For thread safety, the services provided by this class, with the exception of Clock and Scheduler, should be used only 
    /// from within the context of internal QuickSilver threads. To schedule the execution of code in the context of QuickSilver, one
    /// can use the Scheduler service.
    /// </summary>
    public interface IPlatform : IDisposable
    {
        /// <summary>
        /// This is a high-resolution clock, with microsecond-level accuracy on physical hardware.
        /// </summary>
        QS.Fx.Clock.IClock Clock
        {
            get;
        }

        /// <summary>
        /// This is a high-resolution timer that can be used to schedule code for execution. Code executed by the timer must be short
        /// and terminating. The timer can only be used from within the context of the QuickSilver threads.
        /// </summary>
        QS.Fx.Clock.IAlarmClock AlarmClock
        {
            get;
        }

        /// <summary>
        /// This service allows a callback to be scheduled for execution in the context of the internal QuickSilver threads. It can be used
        /// by any thread. This is the preferred means of transfering control between the application threads and QuickSilver threads. 
        /// Code executed by the scheduler must be short and terminating.
        /// </summary>
        QS.Fx.Scheduling.IScheduler Scheduler
        {
            get;
        }

        /// <summary>
        /// This is a simple logging facility that consumes plain text messages.
        /// </summary>
        QS.Fx.Logging.ILogger Logger
        {
            get;
        }

        /// <summary>
        /// This is a more sophisticated logging facility that consumes more descriptive event objects.
        /// </summary>
        QS.Fx.Logging.IEventLogger EventLogger
        {
            get;
        }

        /// <summary>
        /// This is a virtualized filesystem.
        /// </summary>
        QS.Fx.Filesystem.IFilesystem Filesystem
        {
            get;
        }

        /// <summary>
        /// This is a virtualized network stack.
        /// </summary>
        QS.Fx.Network.INetworkConnection Network
        {
            get;
        }
    }
}
