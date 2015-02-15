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

// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Base6_
{
    public class RateController<C> : Source<C>, IChannel, ISource<C>
    {
        public RateController(ISource<C> source, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock)
        {
            this.clock = clock;
            this.alarmClock = alarmClock;

            if (source != null)
            {
                this.source = source;
                source.Channel = this;
            }
        }

        private ISource<C> source;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;

        #region IChannel Members

        void IChannel.Signal()
        {
            lock (this)
            {
                // ............................

                throw new NotImplementedException();
            }
        }

        #endregion

        #region ISource<C> Members

        void ISource<C>.GetObjects(Queue<C> objectQueue,
                int maximumNumberOfObjects,
#if UseEnhancedRateControl
                int maximumNumberOfBytes, 
#endif
 out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
                out int numberOfBytesReturned,
#endif
                out bool moreObjectsAvailable)
        {
            lock (this)
            {
                // .....................

                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                numberOfBytesReturned = 0;
#endif
                moreObjectsAvailable = false;

                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
