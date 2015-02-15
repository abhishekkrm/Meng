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
using System.Threading;

namespace QS._qss_c_.Base5_
{
    public class InterlockedChannel<C> : Base4_.IChannel, Base4_.ISource<C>
    {
        public InterlockedChannel(Base4_.SourceCallback<C> sourceCallback, Base4_.GetObjectsCallback<C> getObjectsCallback)
        {
            this.sourceCallback = sourceCallback;
            this.getObjectsCallback = getObjectsCallback;
        }

        private Base4_.SourceCallback<C> sourceCallback;
        private Base4_.GetObjectsCallback<C> getObjectsCallback;

/*
        private volatile int producerCounter = 0, consumerCounter = 0;
*/
        #region ISource<C> Members

        bool Base4_.ISource<C>.Ready
        {
            get 
            { 
/*
                return signalingCounter > 0; 
*/

                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This function either places some objects on the queue and returns true, 
        /// or sets channel as non-signaled and returns false.
        /// </summary>
        /// <param name="returnedObjects"></param>
        /// <param name="maximumSize"></param>
        /// <returns></returns>
        // semantics: 
        // a) if objects returned, keep signaled
        // b) if no objects returned, reset signal, but only if nobody broke in between function call and the resetting
        // c) if no objects returned, but somebody got in before we signaled to false, signaed stays true
        bool QS._qss_c_.Base4_.ISource<C>.GetObjects(ref System.Collections.Generic.Queue<C> returnedObjects, uint maximumSize)
        {

/*
            int mycounter;
            while (true)
            {
                mycounter = counter;
                if (getObjectsCallback(ref returnedObjects, maximumSize))
                    return true;
                if (Interlocked.
                if (Interlocked.CompareExchange(ref counter, 0, my_signalingCounter) == my_signalingCounter)
                    return false;
            }
*/

            throw new NotImplementedException();
        }

        #endregion

        #region IChannel Members

        // semantics:
        // a) we set signaled to true
        // b) we pass the signal down only if it was not signaled before
        void Base4_.IChannel.Signal()
        {
/*
            int consumerCounter1 = consumerCounter;
            int my_producerCounter = Interlocked.Increment(ref producerCounter);
            int consumerCounter2 = consumerCounter;

            if (consumerCounter == myCounter - 1)
                sourceCallback(this);
*/

            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
