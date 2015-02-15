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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class ForwardingBucket : QS.Fx.Inspection.Inspectable, IForwardingBucket
    {
        public ForwardingBucket(ForwardingCallback forwardingCallback, 
            QS._qss_c_.Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageCache, QS.Fx.Clock.IClock clock)
        {
            this.forwardingCallback = forwardingCallback;
            this.clock = clock;
            this.messageCache = messageCache;
        }

        public ForwardingBucket()
        {
        }

        private ForwardingCallback forwardingCallback;
        private QS._qss_c_.Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageCache;
        private IDictionary<uint, IDictionary<QS._core_c_.Base3.InstanceID, Request>> requests =
            new Dictionary<uint, IDictionary<QS._core_c_.Base3.InstanceID, Request>>();
        private QS.Fx.Clock.IClock clock;
        private uint numberOfPacketsRequested;

#if DEBUG_CollectStatistics
        [QS.CMS.Diagnostics.Component("Number of Packets Requested")]
        private Statistics.SamplesXY timeSeries_numberOfPacketsRequested = new QS.CMS.Statistics.SamplesXY();        
#endif

        #region Class Request

        private class Request : IDisposable
        {
            public Request(ForwardingBucket owner, uint sequenceNo, QS._core_c_.Base3.InstanceID destinationAddress)
            {
                this.owner = owner;
                this.sequenceNo = sequenceNo;
                this.destinationAddress = destinationAddress;

                QS._core_c_.Base3.Message message = default(QS._core_c_.Base3.Message);
                if (owner.messageCache.Get(sequenceNo, ref message))
                    sendOperation = owner.forwardingCallback(destinationAddress, sequenceNo, message, 
                        new Base3_.AsynchronousOperationCallback(this.CompletionCallback), null);
            }

            private ForwardingBucket owner;
            private uint sequenceNo;
            private QS._core_c_.Base3.InstanceID destinationAddress;
            private Base3_.IAsynchronousOperation sendOperation;

            public void Add(QS._core_c_.Base3.Message message)
            {
                if (sendOperation == null)
                    sendOperation = owner.forwardingCallback(destinationAddress, sequenceNo, message,
                        new Base3_.AsynchronousOperationCallback(this.CompletionCallback), this);
            }

            #region Callback

            private void CompletionCallback(Base3_.IAsynchronousOperation asynchronousOperation)
            {
                lock (owner)
                {
                    IDictionary<QS._core_c_.Base3.InstanceID, Request> dic;
                    if (owner.requests.TryGetValue(sequenceNo, out dic))
                    {
                        dic.Remove(destinationAddress);
                        if (dic.Count == 0)
                        {
                            owner.requests.Remove(sequenceNo);
                            owner.numberOfPacketsRequested--;

#if DEBUG_CollectStatistics
                            if (owner.timeSeries_numberOfPacketsRequested.Enabled)
                                owner.timeSeries_numberOfPacketsRequested.addSample(
                                    owner.clock.Time, owner.numberOfPacketsRequested);
#endif
                        }
                    }
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if (sendOperation != null)
                {
                    try
                    {
                        sendOperation.Cancel();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            #endregion
        }

        #endregion

        #region IForwardingBucket Members

        void IForwardingBucket.Add(uint sequenceNo, QS._core_c_.Base3.Message message)
        {
            lock (this)
            {
                if (requests.ContainsKey(sequenceNo))
                {
                    foreach (Request request in requests[sequenceNo].Values)
                        request.Add(message);
                }
            }
        }

        void IForwardingBucket.Schedule(uint sequenceNo, QS._core_c_.Base3.InstanceID destinationAddress)
        {
            lock (this)
            {
                IDictionary<QS._core_c_.Base3.InstanceID, Request> dic;
                if (requests.ContainsKey(sequenceNo))
                    dic = requests[sequenceNo];
                else
                    requests[sequenceNo] = dic = new Dictionary<QS._core_c_.Base3.InstanceID, Request>();

                if (!dic.ContainsKey(destinationAddress))
                {
                    Request req = new Request(this, sequenceNo, destinationAddress);
                    dic[destinationAddress] = req;
                    numberOfPacketsRequested++;

#if DEBUG_CollectStatistics
                    if (timeSeries_numberOfPacketsRequested.Enabled)
                        timeSeries_numberOfPacketsRequested.addSample(clock.Time, numberOfPacketsRequested);
#endif
                }
            }
        }

        QS._qss_c_.Receivers4.IMessageRepository<QS._core_c_.Base3.Message> IForwardingBucket.Repository
        {
            get { return messageCache; }
            set { messageCache = value; }
        }

        ForwardingCallback IForwardingBucket.ForwardingCallback
        {
            get { return forwardingCallback; }
            set { forwardingCallback = value; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                foreach (IDictionary<QS._core_c_.Base3.InstanceID, Request> dic in requests.Values)
                    foreach (Request req in dic.Values)
                        ((IDisposable)req).Dispose();
                requests.Clear();

                numberOfPacketsRequested = 0;
#if DEBUG_CollectStatistics
                if (timeSeries_numberOfPacketsRequested.Enabled)
                    timeSeries_numberOfPacketsRequested.addSample(clock.Time, numberOfPacketsRequested);
#endif
            }
        }

        #endregion
    }
}
