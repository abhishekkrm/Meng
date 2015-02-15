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

namespace QS._qss_c_.Devices_6_
{
    public class Adapter2AS : QS._core_c_.Base6.ISink<Base6_.Asynchronous<Block>>
    {
        public Adapter2AS(
            Base4_.IAddressedSink<QS.Fx.Network.NetworkAddress, QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> sink)
        {
            channel = sink.Register(
                new QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>(
                    this.GetObjectsCallback));
        }

        private Base4_.IChannel channel;
        // private bool registered;
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._qss_c_.Base6_.Asynchronous<Block>>> callbackQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._qss_c_.Base6_.Asynchronous<Block>>>();
        private Queue<Base6_.Asynchronous<Block>> workingQueue = new Queue<QS._qss_c_.Base6_.Asynchronous<Block>>();

        #region CallbackWrapper

        private class CallbackWrapper
        {
            public CallbackWrapper(QS._core_c_.Base6.CompletionCallback<object> callback)
            {
                this.callback = callback;
            }

            private QS._core_c_.Base6.CompletionCallback<object> callback;

            public void CompletionCallback(bool succeeded, Exception exception, object context)
            {
                callback(succeeded, exception, context);
            }
        }

        #endregion

        #region GetObjectsCallback

        private bool GetObjectsCallback(
            ref Queue<Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> returnedObjects, uint maximumSize)
        {
            lock (this)
            {
                while (callbackQueue.Count > 0)
                {
                    QS._core_c_.Base6.GetObjectsCallback<Base6_.Asynchronous<Block>> callback = callbackQueue.Dequeue();

                    // TODO: Implement enhanced rate control

                    int nreturned;
#if UseEnhancedRateControl
                    int nbytesreturned;
#endif
                    bool hasmore;
                    callback(workingQueue, 1, 
#if UseEnhancedRateControl
                        int.MaxValue, // <--------------------FIX THIS .....................................HERE
#endif                        
                        out nreturned, 
#if UseEnhancedRateControl
                        out nbytesreturned, 
#endif                        
                        out hasmore);

                    if (hasmore)
                        callbackQueue.Enqueue(callback);

                    if (nreturned > 0)
                    {
                        foreach (Base6_.Asynchronous<Block> request in workingQueue)
                        {
                            if (request.Argument.Size > maximumSize)
                            {
                                // should report an error
                                throw new Exception("Object returned by the source callback is too big for the underlying sink.");
                            }
                            else
                            {
                                returnedObjects.Enqueue(
                                    new QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>(
                                        request.Argument.Segments, 
                                            ((request.CompletionCallback != null) ? (new Base4_.CompletionCallback(
                                                (new CallbackWrapper(request.CompletionCallback)).CompletionCallback)) : null), 
                                        request.Context));
                            }
                        }
                        workingQueue.Clear();
                        return true;
                    }
                    else
                    {
                        if (hasmore)
                        {
                            // should report an error
                            throw new Exception("Source callback returned no objects even though more objects are available.");
                        }
                    }
                }

                // registered = false;
                return false;
            }
        }

        #endregion

        #region ISink<Asynchronous<Block>> Members

        int QS._core_c_.Base6.ISink<QS._qss_c_.Base6_.Asynchronous<Block>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<QS._qss_c_.Base6_.Asynchronous<Block>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._qss_c_.Base6_.Asynchronous<Block>> getObjectsCallback)
        {
            lock (this)
            {
                callbackQueue.Enqueue(getObjectsCallback);
            }
/*
            bool register_now;
                register_now = !registered;
                registered = true;
            }

            if (register_now)
*/
            channel.Signal();
        }

        #endregion
    }
}
