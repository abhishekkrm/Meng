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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Forwarder : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule, IForwarder, IForwardingSinkContext
    {
        public Forwarder(Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> underlyingSinks,
            Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageCache, QS.Fx.Clock.IClock clock, WrappingCallback wrappingCallback,
            QS.Fx.Logging.ILogger logger)
        {
            this.underlyingSinks = underlyingSinks;
            this.messageCache = messageCache;
            this.clock = clock;
            this.wrappingCallback = wrappingCallback;
            this.logger = logger;

            ((QS._core_c_.Diagnostics2.IContainer) diagnosticsContainer).Register("ForwardingSinks", diagnosticsContainerForForwardingSinks);
            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> underlyingSinks;
        private Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageCache;
        private QS.Fx.Clock.IClock clock;
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsContainerForForwardingSinks = new QS._core_c_.Diagnostics2.Container();
        private WrappingCallback wrappingCallback;
        private QS.Fx.Logging.ILogger logger;

        private uint maxclean;
        private IList<Base1_.Range<uint>> missing = new List<Base1_.Range<uint>>();
        private IDictionary<uint, IList<IForwardingSink>> missingmap = new Dictionary<uint, IList<IForwardingSink>>();

        [QS._core_c_.Diagnostics.ComponentCollection("Forwarding Sinks")]
        private IDictionary<QS._core_c_.Base3.InstanceID, IForwardingSink> forwardingSinks = new Dictionary<QS._core_c_.Base3.InstanceID, IForwardingSink>();

        #region Stuff for debugging purposes

        private string _CurrentStatusAsString
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine("maxclean \t = \t " + maxclean.ToString());
                s.AppendLine("missing \t = \t " + Base1_.Ranges.ToString(missing));
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
        [QS._core_c_.Diagnostics.Component("Registering Times (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("RegisteringTimes")]
        private Statistics_.Samples2D timeseries_registeringTimes = new Statistics_.Samples2D("RegisteringTimes");

        [QS._core_c_.Diagnostics.Component("Completion Times (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("CompletionTimes")]
        private Statistics_.Samples2D timeseries_completionTimes = new Statistics_.Samples2D("CompletionTimes");
#endif

        #endregion

        #region GetForwardingSink

        private void GetForwardingSink(QS._core_c_.Base3.InstanceID destinationAddress, out IForwardingSink forwardingSink)
        {
            if (!forwardingSinks.TryGetValue(destinationAddress, out forwardingSink))
            {
                forwardingSink = new ForwardingSink(
                    destinationAddress, underlyingSinks[destinationAddress], wrappingCallback, messageCache, clock, logger, this);
                forwardingSinks.Add(destinationAddress, forwardingSink);

                ((QS._core_c_.Diagnostics2.IContainer) diagnosticsContainerForForwardingSinks).Register(
                    destinationAddress.ToString(), ((QS._core_c_.Diagnostics2.IModule) forwardingSink).Component);
            }
        }

        #endregion

        #region IForwardingSinkContext Members

        void IForwardingSinkContext.Request(IForwardingSink sink, IList<QS._qss_c_.Base1_.Range<uint>> requests)
        {
            lock (this)
            {
                Base1_.Ranges.Trim(ref requests, maxclean);
                Base1_.Ranges.Add(ref missing, requests);

                foreach (Base1_.Range<uint> range in requests)
                {
                    for (uint seqno = range.From; seqno <= range.To; seqno++)
                    {
                        IList<IForwardingSink> clients;
                        if (!missingmap.TryGetValue(seqno, out clients))
                        {
#if DEBUG_CollectStatistics
                            timeseries_registeringTimes.Add(clock.Time, seqno);
#endif

                            missingmap.Add(seqno, clients = new List<IForwardingSink>());
                        }
                        clients.Add(sink);
                    }
                }
            }
        }

        #endregion

        #region IForwarder Members

        void IForwarder.Forward(QS._core_c_.Base3.InstanceID destinationAddress, IList<QS._qss_c_.Base1_.Range<uint>> requests)
        {
            IForwardingSink forwardingSink;
            lock (this)
            {
                GetForwardingSink(destinationAddress, out forwardingSink);
            }

            forwardingSink.Forward(requests);
        }

        void IForwarder.Cancel(uint maxtoclean)
        {
            ICollection<IForwardingSink> affected = null;

            lock (this)
            {
                if (maxtoclean > maxclean)
                {
                    maxclean = maxtoclean;
                    affected = forwardingSinks.Values;

                    foreach (Base1_.Range<uint> range in missing)
                    {
                        if (range.From <= maxclean)
                        {
                            for (uint seqno = range.From; seqno <= Math.Min(range.To, maxclean); seqno++)
                            {
                                if (missingmap.Remove(seqno))
                                {
#if DEBUG_CollectStatistics
                                    timeseries_completionTimes.Add(clock.Time, seqno);
#endif
                                }
                            }
                        }
                        else
                            break;
                    }

                    Base1_.Ranges.Trim(ref missing, maxclean);
                }
            }

            if (affected != null)
            {
                foreach (IForwardingSink sink in affected)
                    sink.Cancel(maxclean);
            }
        }

        void IForwarder.Receive(uint seqno)
        {
            IList<IForwardingSink> clients = null;

            lock (this)
            {
                if (seqno > maxclean)
                {
                    if (missingmap.TryGetValue(seqno, out clients))
                    {
#if DEBUG_CollectStatistics
                        timeseries_completionTimes.Add(clock.Time, seqno);
#endif

                        missingmap.Remove(seqno);
                    }

                    Base1_.Ranges.Remove(ref missing, seqno);
                }
            }

            if (clients != null)
            {
                foreach (IForwardingSink sink in clients)
                    sink.Receive(seqno);
            }
        }


        #region Old Junk

/*
        void IForwarder.Schedule(QS.CMS.QS._core_c_.Base3.InstanceID destinationAddress, IList<QS.CMS.Base.Range<uint>> requests)
        {
            lock (this)
            {
                IList<Base.Range<uint>> newtracked, newlyadded;
                Base.Ranges.Add(tracked, Base.Ranges.Trim(requests, maxclean), out newtracked, out newlyadded);
 
                if (newlyadded.Count > 0)
                {
                    this.tracked = newtracked;
 
                    IList<Base.Range<uint>> newcached, newmissing;
                    messageCache.Find(newlyadded, out newcached, out newmissing);

                    Base.Ranges.Add(ref this.cached, newcached);
                    Base.Ranges.Add(ref this.missing, newmissing);
 
                    IList<QS.CMS.Base.Range<uint>> added;
                    forwardingSink.Schedule(requests, out added);

                    foreach (Base.Range<uint> range in added)
                    {
                        // .........................................................................................................................................................
                    }
                }
            }
        }
*/ 

/*
        void IForwarder.Canceled(uint maximumCanceled)
        {
            lock (this)
            {
                if (maximumCanceled > this.maxcanceled)
                {
                    foreach (IForwardingSink forwardingSink in forwardingSinks.Values)
                        forwardingSink.Canceled(maximumCanceled);

                    this.maxcanceled = maximumCanceled;
                }
            }
        }
*/

        #endregion

        #endregion
    }
}
