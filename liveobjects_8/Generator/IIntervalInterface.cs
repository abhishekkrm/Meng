/*

Copyright (c) 2010 Matt Pearson. All rights reserved.

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
using System.Linq;
using System.Text;

namespace Generator
{
    /// <summary>
    /// Generate interval/length pairs for TextGenerator.
    /// </summary>
    [QS.Fx.Reflection.InterfaceClass("ECAED515445D4098A97F901472858F76", "IIntervalInterface",
        "Interface for interval generators for TextGenerator")]
    public interface IIntervalInterface : QS.Fx.Interface.Classes.IInterface
    {
        /// <summary>
        /// Move to the next interval/length pair. Returns true if there
        /// is such a pair, false if there are none left.
        /// </summary>
        [QS.Fx.Reflection.Operation("Next")]
        bool Next();

        /* these two should really be properties, but it's not supported currently.
         * Could we just allow the annotations to be used and have it work? who knows!
         */
        /// <summary>
        /// Retrieve the interval for this pair (delay in seconds).
        /// </summary>
        [QS.Fx.Reflection.Operation("Interval")]
        double Interval();

        /// <summary>
        /// Retrieve the length of the message to send (bytes).
        /// </summary>
        [QS.Fx.Reflection.Operation("Length")]
        int Length();
    }
}
