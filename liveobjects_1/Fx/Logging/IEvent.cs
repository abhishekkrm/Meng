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

namespace QS.Fx.Logging
{
    /// <summary>
    /// This interface is implemented by events that can be posted on the event logger.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The time when event occurred.
        /// </summary>
        double Time
        {
            get;
            set;
        }

        /// <summary>
        /// The physical location at which the event originated.
        /// </summary>
        string Location
        {
            get;
            set;
        }

        /// <summary>
        /// The event class.
        /// </summary>
        IEventClass Class
        {
            get;
        }

        /// <summary>
        /// The software component where the event originated.
        /// </summary>
        string Source
        {
            get;
        }

        /// <summary>
        /// The details of the event.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// The list of properties associated with the event.
        /// </summary>
        /// <param name="property">The name of the property associated with the event.</param>
        /// <returns>The value of the property.</returns>
        object this[string property]
        {
            get;
        }
    }
}
