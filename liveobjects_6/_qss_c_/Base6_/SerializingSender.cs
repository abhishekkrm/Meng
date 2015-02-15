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

// #define DEBUG_CollectStatistics
// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_c_.Base6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class SerializingSender : ProcessingSink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, Asynchronous<Devices_6_.Block>>
    {
        public SerializingSender(
            QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IClock clock, QS._core_c_.Base6.ISink<Base6_.Asynchronous<Devices_6_.Block>> blockSender) 
            : base(blockSender)
        {
            this.localAddress = localAddress;
            this.clock = clock;
            completionCallback = new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback);
        }

        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Clock.IClock clock;
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> pendingQueue = new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();
        private QS._core_c_.Base6.CompletionCallback<object> completionCallback;

#if DEBUG_CollectStatistics
        [QS.CMS.Diagnostics.Component("Send Times")]
        private Statistics.Samples timeSeries_sendTimes = new QS.CMS.Statistics.Samples();
#endif

        #region GetObjects

        protected override void GetObjects(Queue<Asynchronous<Devices_6_.Block>> outgoingQueue,
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
            // TODO: Implement enhanced rate control

            lock (this)
            {
                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                    numberOfBytesReturned = 0;
#endif
                moreObjectsAvailable = true;

                while (true)
                {
                    if (incomingQueue.Count > 0)
                    {
                        if (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                        {
                            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback = incomingQueue.Dequeue();

                            int objectsReturned;
#if UseEnhancedRateControl
                            int bytesReturned;
#endif
                            bool moreAvailable;
                            getObjectsCallback(pendingQueue,
                                maximumNumberOfObjects - numberOfObjectsReturned, 
#if UseEnhancedRateControl
                                int.MaxValue, // maximumNumberOfBytes - numberOfBytesReturned, 
#endif
                                out objectsReturned, 
#if UseEnhancedRateControl
                                out bytesReturned, 
#endif
                                out moreAvailable);
                            if (moreAvailable)
                                incomingQueue.Enqueue(getObjectsCallback);

                            foreach (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request in pendingQueue)
                            {
                                IList<QS.Fx.Base.Block> segments;
                                uint transmittedSize;
                                Base3_.Root.Encode(localAddress, request.Argument.destinationLOID, request.Argument.transmittedObject,
                                    out segments, out transmittedSize);

#if DEBUG_CollectStatistics
                                if (timeSeries_sendTimes.Enabled)
                                    timeSeries_sendTimes.addSample(clock.Time);
#endif

                                outgoingQueue.Enqueue(new Asynchronous<QS._qss_c_.Devices_6_.Block>(
                                    new QS._qss_c_.Devices_6_.Block(segments, (int) transmittedSize), request, 
                                    ((request.CompletionCallback != null) ? completionCallback : null)));
                                numberOfObjectsReturned++;
                                // numberOfBytesReturned += ..........................................................HERE
                            }

                            pendingQueue.Clear();
                        }
                        else
                            break;
                    }
                    else
                    {
                        moreObjectsAvailable = false;
                        this.Done();
                        break;
                    }
                }
            }
        }

        #endregion

        #region Callback

        private void CompletionCallback(bool succeeded, Exception exception, object context)
        {
            QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request = (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>)context;
            request.CompletionCallback(succeeded, exception, request.Context);
        }

        #endregion
    }
}
