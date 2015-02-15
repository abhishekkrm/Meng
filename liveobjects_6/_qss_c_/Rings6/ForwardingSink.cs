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

#define DEBUG_CollectStatistics
// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class ForwardingSink : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule, IForwardingSink
    {
        public ForwardingSink(QS._core_c_.Base3.InstanceID destination, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink, WrappingCallback wrappingCallback,
            Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageCache, QS.Fx.Clock.IClock clock, QS.Fx.Logging.ILogger logger,
            IForwardingSinkContext context)
        {
            this.destination = destination;
            this.sink = sink;
            this.wrappingCallback = wrappingCallback;
            this.messageCache = messageCache;
            this.clock = clock;
            this.context = context;
            this.logger = logger;

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private QS._core_c_.Base3.InstanceID destination;
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink;
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private WrappingCallback wrappingCallback;
        private Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageCache;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Logging.ILogger logger;
        private IForwardingSinkContext context;

        private bool registered;

        private uint maxclean; // maximum clean, these requests are not supposed to be considered or tracked in any way
        private IList<Base1_.Range<uint>> pending = new List<Base1_.Range<uint>>(); // requested for sending and not known to be missing from cache
        private IList<Base1_.Range<uint>> missing = new List<Base1_.Range<uint>>(); // requested and already determined to be missing from cache
        private IList<Base1_.Range<uint>> handled = new List<Base1_.Range<uint>>(); // passed down to the lower reliable sending layers for forwarding

        #region Stuff for debugging purposes

        private string _CurrentStatusAsString
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine("registered \t = \t " + registered.ToString());
                s.AppendLine("maxclean \t = \t " + maxclean.ToString());
                s.AppendLine("pending \t = \t " + Base1_.Ranges.ToString(pending));
                s.AppendLine("missing \t = \t " + Base1_.Ranges.ToString(missing));
                s.AppendLine("handled \t = \t " + Base1_.Ranges.ToString(handled));
                return s.ToString();
            }
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Collecting Statistics

#if DEBUG_CollectStatistics
        [QS._core_c_.Diagnostics.Component("Requesting Times (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("RequestingTimes")]
        private Statistics_.Samples2D timeseries_requestingTimes = new Statistics_.Samples2D("RequestingTimes");

        [QS._core_c_.Diagnostics.Component("Forwarding Times (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("ForwardingTimes")]
        private Statistics_.Samples2D timeseries_forwardingTimes = new Statistics_.Samples2D("ForwardingTimes");

        [QS._core_c_.Diagnostics.Component("Missed Times (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("MissedTimes")]
        private Statistics_.Samples2D timeseries_missedTimes = new Statistics_.Samples2D("MissedTimes");
#endif

        #endregion

        #region Request

        private void Request(IList<Base1_.Range<uint>> requests)
        {
            context.Request(this, requests);
        }

        #endregion

        #region IForwardingSink Members

        void IForwardingSink.Forward(IList<Base1_.Range<uint>> requests)
        {
            bool register_now = false;

            lock (this)
            {
                Base1_.Ranges.Trim(ref requests, maxclean);
                Base1_.Ranges.Remove(ref requests, handled);
                Base1_.Ranges.Remove(ref requests, missing);
                
                IList<Base1_.Range<uint>> requested_now;
                Base1_.Ranges.Add(ref pending, requests, out requested_now);

                if (requested_now.Count > 0)
                {
#if DEBUG_CollectStatistics
                    double time = clock.Time;
                    foreach (Base1_.Range<uint> range in requested_now)
                        for (uint seqno = range.From; seqno <= range.To; seqno++)
                            timeseries_requestingTimes.Add(time, seqno);
#endif

                    if (!registered)
                        registered = register_now = true;
                }
            }

            if (register_now)
                sink.Send(new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback));
        }

        void IForwardingSink.Cancel(uint maxtoclean)
        {
            lock (this)
            {
                if (maxtoclean > maxclean)
                {
                    maxclean = maxtoclean;
                    Base1_.Ranges.Trim(ref pending, maxclean);
                    Base1_.Ranges.Trim(ref missing, maxclean);
                    Base1_.Ranges.Trim(ref handled, maxclean);
                }
            }
        }

        void IForwardingSink.Receive(uint seqno)
        {
            bool register_now = false;

            lock (this)
            {
                if (seqno > maxclean)
                {
                    Base1_.Ranges.Add(ref pending, seqno);
                    Base1_.Ranges.Remove(ref missing, seqno);

                    register_now = !registered;
                }
            }

            if (register_now)
                sink.Send(new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback));
        }

        #region Old Junk

/*
        void IForwardingSink.Schedule(IList<Base.Range<uint>> cached_toadd, IList<Base.Range<uint>> missing_toadd)
        {
            Base.Ranges.Remove(ref cached_toadd, maxclean, requested_handled);
            Base.Ranges.Remove(ref missing_toadd, maxclean, requested_handled);

            IList<Base.Range<uint>> added_cached;
            Base.Ranges.Add(ref requested_cached, cached_toadd, out added_cached);

#if DEBUG_CollectStatistics
            double time = clock.Time;
            foreach (Base.Range<uint> range in added_cached)
                for (uint seqno = range.From; seqno <= range.To; seqno++)
                    timeseries_requestingTimes.addSample(time, seqno);
#endif

            if (added_cached.Count > 0 && !registered)
            {
                registered = true;
                sink.Send(new QS.CMS.Base6.GetObjectsCallback<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>(this.GetObjectsCallback));
            }

            Base.Ranges.Add(ref requested_missing, missing_toadd);
        }
*/

/*
        void IForwardingSink.Canceled(uint maximumCanceled)
        {
            if (maximumCanceled > maxclean)
            {
                maxclean = maximumCanceled;
                Base.Ranges.Trim(ref requested_cached, maxclean);
                Base.Ranges.Trim(ref requested_missing, maxclean);
                Base.Ranges.Trim(ref requested_handled, maxclean);
            }
        }
*/

        #endregion

        #endregion

        #region GetObjectsCallback

        private void GetObjectsCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue,
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

            numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
            numberOfBytesReturned = 0;
#endif
            moreObjectsAvailable = false;

            Base1_.Ranges.Builder handled_now = default(QS._qss_c_.Base1_.Ranges.Builder);
            Base1_.Ranges.Builder missed_now = default(QS._qss_c_.Base1_.Ranges.Builder);

            lock (this)
            {
                foreach (Base1_.Range<uint> range in pending)
                {
                    for (uint seqno = range.From; seqno <= range.To; seqno++)
                    {
                        if (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                        {
                            QS._core_c_.Base3.Message message = default(QS._core_c_.Base3.Message);
                            if (messageCache.Get(seqno, ref message))
                            {
#if DEBUG_CollectStatistics
                                timeseries_forwardingTimes.Add(clock.Time, seqno);
#endif

                                objectQueue.Enqueue(new Base6_.Asynchronous<QS._core_c_.Base3.Message>(wrappingCallback(seqno, message), null, null));
                                numberOfObjectsReturned++;
                                // numberOfBytesReturned += .............................................................HERE

                                handled_now.Add(seqno);
                            }
                            else
                            {
#if DEBUG_CollectStatistics
                                timeseries_missedTimes.Add(clock.Time, seqno);
#endif

                                missed_now.Add(seqno);
                            }
                        }
                        else
                        {
                            moreObjectsAvailable = true;
                            break;
                        }
                    }
                }

                registered = moreObjectsAvailable;

                Base1_.Ranges.Remove(ref pending, handled_now.Ranges);
                Base1_.Ranges.Remove(ref pending, missed_now.Ranges);
                Base1_.Ranges.Add(ref handled, handled_now.Ranges);
                Base1_.Ranges.Add(ref missing, missed_now.Ranges);
            }

            Request(missed_now.Ranges);
        }

        #endregion
    }
}
