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
    [QS._core_c_.Diagnostics.ComponentContainer]
    public abstract class ProcessingSink<ConsumedClass, ProducedClass> : QS.Fx.Inspection.Inspectable, QS._core_c_.Base6.ISink<ConsumedClass>
    {
        public ProcessingSink(QS._core_c_.Base6.ISink<ProducedClass> consumingSink)
        {
            this.consumingSink = consumingSink;
            myCallback = new QS._core_c_.Base6.GetObjectsCallback<ProducedClass>(this.GetObjects);
        }

        [QS._core_c_.Diagnostics.Component]
        protected QS._core_c_.Base6.ISink<ProducedClass> consumingSink;
        protected bool registered;
        protected QS._core_c_.Base6.GetObjectsCallback<ProducedClass> myCallback;

        protected Queue<QS._core_c_.Base6.GetObjectsCallback<ConsumedClass>> incomingQueue = 
            new Queue<QS._core_c_.Base6.GetObjectsCallback<ConsumedClass>>();

        protected abstract void GetObjects(
            Queue<ProducedClass> outgoingQueue,
            int maximumNumberOfObjects,
#if UseEnhancedRateControl
            int maximumNumberOfBytes, 
#endif
            out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
            out int numberOfBytesReturned,
#endif
            out bool moreObjectsAvailable);

        protected void Done()
        {
            registered = false;
        }

        #region ISink<ConsumedClass> Members

        int QS._core_c_.Base6.ISink<ConsumedClass>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<ConsumedClass>.Send(QS._core_c_.Base6.GetObjectsCallback<ConsumedClass> getObjectsCallback)
        {
            bool signal_now;
            lock (this)
            {
                incomingQueue.Enqueue(getObjectsCallback);
                signal_now = !registered;
                registered = true;
            }

            if (signal_now)
                consumingSink.Send(myCallback);
        }

        #endregion
    }
}
