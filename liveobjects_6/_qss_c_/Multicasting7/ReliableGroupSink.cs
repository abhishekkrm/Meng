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

#define DEBUG_MeasureGetOverheads
// #define UseEnhancedRateControl

#define OPTION_LimitPendingCompletion
#define OPTION_ChangesToSignaling
#define OPTION_SynchronizeFeeds

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Multicasting7
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class ReliableGroupSink : QS.Fx.Inspection.Inspectable, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>, QS._core_c_.Diagnostics2.IModule, IDisposable
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public ReliableGroupSink(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Clock.IClock clock,
            Base3_.GroupID groupID, Membership2.Controllers.IMembershipController membershipController,
            Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks, QS.Fx.Logging.ILogger logger,
            int maximumNumberOfPendingCompletion, int feed_bufferlow, int feed_bufferhigh)
        {
            this.clock = clock;
            this.membershipController = membershipController;
            this.groupID = groupID;
            this.downstreamSinks = downstreamSinks;
            this.logger = logger;
            this.maximumNumberOfPendingCompletion = maximumNumberOfPendingCompletion;
            this.feed_bufferlow = feed_bufferlow;
            this.feed_bufferhigh = feed_bufferhigh;

            completionCallback = new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback);
            InitializeView();

#if DEBUG_MeasureGetOverheads
            ts_GetOverheads = statisticsController.Allocate2D("get overheads", "", "time", "s", "", "overhead", "s", "");
            ts_GetNumberOfObjectsReturned = statisticsController.Allocate2D(
                "number of objects returned by get", "", "time", "s", "", "number of objects returned", "", "");
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private int numberOfPendingCompletion, maximumNumberOfPendingCompletion, feed_bufferlow, feed_bufferhigh, num_feeds_blocked;
        private QS.Fx.Clock.IClock clock;
        private Base3_.GroupID groupID;
        private Membership2.Controllers.IMembershipController membershipController;
        private QS.Fx.Logging.ILogger logger;
        private bool waiting, completion_blocked, feed_blocked;
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> incomingQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> invokedCallbackQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        private Membership2.Controllers.IGroupController groupController;
        private Membership2.Controllers.IGroupViewController groupViewController;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<Base3_.RVID, Feed> feeds = new Dictionary<Base3_.RVID, Feed>();
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> bufferedQueue = new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();
        private QS._core_c_.Base6.CompletionCallback<object> completionCallback;

#if DEBUG_MeasureGetOverheads
        [QS._core_c_.Diagnostics2.Property("GetOverheads")]
        private QS._core_c_.Statistics.ISamples2D ts_GetOverheads;

        [QS._core_c_.Diagnostics2.Property("GetNumberOfObjectsReturned")]
        private QS._core_c_.Statistics.ISamples2D ts_GetNumberOfObjectsReturned;
#endif

        private bool disposed;

        #region Membership change callback

        public void MembershipChanged(Membership2.Controllers.IGroupViewController groupVC)
        {
            logger.Log(this, "__________MembershipChanged: [new view = " + groupVC.SeqNo.ToString() + "]");

            lock (this)
            {
                RecheckView();
            }
        }

        #endregion

        #region Initializing and updating views and feeds

        private void InitializeView()
        {
            groupController = membershipController[groupID];
            groupViewController = groupController.CurrentView;
            foreach (Membership2.ClientState.IRegionView element in groupViewController.RegionViews)
            {
                Membership2.Controllers.IRegionViewController regionViewController = (Membership2.Controllers.IRegionViewController) element;
                Base3_.RVID rvid = new QS._qss_c_.Base3_.RVID(regionViewController.Region.ID, regionViewController.SeqNo);
                Feed feed = new Feed(this, rvid, regionViewController, downstreamSinks[rvid], feed_bufferlow, feed_bufferhigh);
                feeds.Add(rvid, feed);
            }
        }

        private void RecheckView()
        {
            // TODO: We should check if the group controller we're holding onto was destroyed and needs to be updated.........
            // if (groupController. ...) ................. 

            Membership2.Controllers.IGroupViewController currentGroupViewController = groupController.CurrentView;
            if (!ReferenceEquals(currentGroupViewController, groupViewController))
            {
                logger.Log(this, "__________GV changed : " + 
                    groupViewController.SeqNo.ToString() + " -> " + currentGroupViewController.SeqNo.ToString());
                groupViewController = currentGroupViewController;

                ICollection<Base3_.RVID> toRemove = new System.Collections.ObjectModel.Collection<Base3_.RVID>();
                foreach (Base3_.RVID rvid in feeds.Keys)
                    toRemove.Add(rvid);

                foreach (Membership2.ClientState.IRegionView element in currentGroupViewController.RegionViews)
                {
                    Membership2.Controllers.IRegionViewController regionViewController = (Membership2.Controllers.IRegionViewController)element;
                    Base3_.RVID rvid = new QS._qss_c_.Base3_.RVID(regionViewController.Region.ID, regionViewController.SeqNo);
                    if (!toRemove.Remove(rvid))
                    {
                        logger.Log(this, "__________Creating feed for group " + groupID.ToString() + " : rvid = " + rvid.ToString());

                        Feed feed = new Feed(this, rvid, regionViewController, downstreamSinks[rvid], feed_bufferlow, feed_bufferhigh);
                        feeds.Add(rvid, feed);

#if OPTION_ChangesToSignaling
                        if (waiting)
                        {
#if OPTION_SynchronizeFeeds                    
                            bool got_blocked_just_now;
                            feed.Signal(out got_blocked_just_now);

                            if (got_blocked_just_now)
                            {
                                feed_blocked = true;
                                num_feeds_blocked++;
                            }
#else
                            feed.Signal();
#endif
                        }
#endif
                    }
                }

                foreach (Base3_.RVID rvid in toRemove)
                {
                    logger.Log(this, "__________Disconnecting feed for group " + groupID.ToString() + " : rvid = " + rvid.ToString());

                    Feed feed = feeds[rvid];

#if OPTION_SynchronizeFeeds
                    if (!feed.CanBuffer)
                        num_feeds_blocked--;
#endif

                    feed.Disconnect();
                    feeds.Remove(rvid);
                }

#if OPTION_SynchronizeFeeds                    
                // a little sanity check
                int actual_num_feeds_blocked = 0;
                foreach (Feed feed in feeds.Values)
                {
                    if (!feed.CanBuffer)
                        actual_num_feeds_blocked++;
                }

                if (actual_num_feeds_blocked != num_feeds_blocked)
                {
                    logger.Log(this, "Number of feeds thought to be blocked { " + num_feeds_blocked.ToString() +
                        " } is incorrect, it should currently be { " + actual_num_feeds_blocked.ToString() + " }.");
                    num_feeds_blocked = actual_num_feeds_blocked;
                }

                if (feed_blocked && num_feeds_blocked <= 0)
                {
                    feed_blocked = false;
                    if (!waiting && !completion_blocked && incomingQueue.Count > 0)
                    {
                        waiting = true;
                        foreach (Feed feed in feeds.Values)
                        {
                            bool got_blocked_just_now;
                            feed.Signal(out got_blocked_just_now);

                            if (got_blocked_just_now)
                            {
                                feed_blocked = true;
                                num_feeds_blocked++;
                            }
                        }
                    }
                }
#endif
            }
        }

        #endregion

        #region Callback

#if OPTION_LimitPendingCompletion
        private void CompletionCallback()
        {
            numberOfPendingCompletion--;
            if (completion_blocked && numberOfPendingCompletion < maximumNumberOfPendingCompletion / 2)
            {
                completion_blocked = false;
                if (!waiting
#if OPTION_SynchronizeFeeds                    
                        && !feed_blocked && incomingQueue.Count > 0
#endif
                    )
                {
                    waiting = true;
                    foreach (Feed feed in feeds.Values)
                    {
#if OPTION_SynchronizeFeeds                    
                        bool got_blocked_just_now;
                        feed.Signal(out got_blocked_just_now);

                        if (got_blocked_just_now)
                        {
                            feed_blocked = true;
                            num_feeds_blocked++;
                        }
#else
                        feed.Signal();
#endif
                    }
                }
            }
        }
#endif

        #endregion
        
        #region UnblockedCallback

#if OPTION_SynchronizeFeeds                    
        private void UnblockedCallback(Feed unblocked_feed)
        {
            num_feeds_blocked--;
            if (feed_blocked && num_feeds_blocked <= 0)
            {
                feed_blocked = false;
                if (!completion_blocked && !waiting && incomingQueue.Count > 0)
                {
                    waiting = true;
                    foreach (Feed feed in feeds.Values)
                    {
                        bool got_blocked_just_now;
                        feed.Signal(out got_blocked_just_now);

                        if (got_blocked_just_now)
                        {
                            feed_blocked = true;
                            num_feeds_blocked++;
                        }
                    }
                }
            }
        }
#endif

        #endregion

        #region Class Feed

        [QS.Fx.Base.Inspectable]
        [QS._core_c_.Diagnostics.Component]
        private sealed class Feed : QS.Fx.Inspection.Inspectable
        {
            public Feed(ReliableGroupSink owner, Base3_.RVID address, Membership2.Controllers.IRegionViewController regionVC, 
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink, int bufferlow, int bufferhigh)
            {
                this.owner = owner;
                this.address = address;
                this.regionVC = regionVC;
                this.downstreamSink = downstreamSink;
#if OPTION_SynchronizeFeeds                    
                this.bufferlow = bufferlow;
                this.bufferhigh = bufferhigh;
#endif

                myCallback = new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjects);
            }

            private ReliableGroupSink owner;
            private Base3_.RVID address;
            private Membership2.Controllers.IRegionViewController regionVC;
            private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink;
            private QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> myCallback;
            private bool waiting, disconnected;
#if OPTION_SynchronizeFeeds                    
            private int bufferlow, bufferhigh;
            private bool canbuffer = true;
#endif
            private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> bufferedQueue =
                new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();

            #region Disconnect

            public void Disconnect()
            {
                disconnected = true;
            }

            #endregion

            #region Signaling and sending

            public void Signal(
#if OPTION_SynchronizeFeeds                    
                out bool got_blocked_just_now
#endif
                )
            {
                Reactivate(null
#if OPTION_SynchronizeFeeds                    
                    , out got_blocked_just_now
#endif
                    );
            }

#if OPTION_SynchronizeFeeds                    
            public bool CanBuffer
            {
                get { return canbuffer; }
            }
#endif

            public void Send(Subrequest subrequest
#if OPTION_SynchronizeFeeds                    
                , out bool got_blocked_just_now
#endif
                )
            {
                Reactivate(subrequest
#if OPTION_SynchronizeFeeds                    
                    , out got_blocked_just_now
#endif
                    );
            }

            private void Reactivate(Subrequest subrequest
#if OPTION_SynchronizeFeeds                    
                , out bool got_blocked_just_now
#endif
                )
            {
                bool signal_now = false;
#if OPTION_SynchronizeFeeds                    
                got_blocked_just_now = false;
#endif

                lock (this)
                {
                    if (subrequest != null)
                        bufferedQueue.Enqueue(subrequest);

                    if (!waiting)
                        signal_now = waiting = true;
                }

                if (signal_now)
                    downstreamSink.Send(myCallback);

#if OPTION_SynchronizeFeeds
                if (canbuffer && bufferedQueue.Count >= bufferhigh)
                {
                    canbuffer = false;
                    got_blocked_just_now = true;
                }
#endif
            }

            #endregion

            #region GetObjects

            private void GetObjects(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoingQueue,
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
                moreObjectsAvailable = true;

#if OPTION_SynchronizeFeeds                    
                bool unblocked_now = false;
#endif

                lock (this)
                {
                    while (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                    {
                        if (bufferedQueue.Count > 0)
                        {
                            outgoingQueue.Enqueue(bufferedQueue.Dequeue());
                            numberOfObjectsReturned++;
                            // numberOfBytesReturned += .......................................................................HERE

#if OPTION_SynchronizeFeeds                    
                            if (!canbuffer && bufferedQueue.Count <= bufferlow)
                            {
                                canbuffer = true;
                                unblocked_now = true;
                            }
#endif
                        }
                        else
                        {
                            if (!disconnected)
                            {
                                int nreturned;
#if UseEnhancedRateControl
                                int nbytesreturned;
#endif
                                bool more;

                                Monitor.Exit(this);
                                try
                                {
                                    owner.GetObjects(
                                        this, maximumNumberOfObjects - numberOfObjectsReturned, 
#if UseEnhancedRateControl
                                        int.MaxValue, // maximumNumberOfBytes - numberOfBytesReturned, 
#endif
                                        out nreturned,
#if UseEnhancedRateControl
                                        out nbytesreturned, 
#endif
                                        out more);
                                }
                                finally
                                {
                                    Monitor.Enter(this);
                                }

                                if (nreturned > 0 || more)
                                    continue;
                            }

                            moreObjectsAvailable = waiting = false;
                            break;
                        }
                    }
                }

#if OPTION_SynchronizeFeeds                    
                if (unblocked_now && !disconnected)
                    owner.UnblockedCallback(this);
#endif
            }

            #endregion
        }

        #endregion

        #region Class Request

        private sealed class Request
        {
            public Request(ReliableGroupSink owner, QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> applicationRequest, int numberOfFeeds)
            {
                this.owner = owner;
                this.applicationRequest = applicationRequest;
                this.numberOfFeeds = numberOfFeeds;
            }

            private ReliableGroupSink owner;
            private QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> applicationRequest;
            private int numberOfFeeds;

            public void Completed(Feed feed)
            {
                numberOfFeeds--;
                if (numberOfFeeds == 0)
                {
#if OPTION_LimitPendingCompletion
                    owner.CompletionCallback();
#endif

                    QS._core_c_.Base6.CompletionCallback<object> callback = applicationRequest.CompletionCallback;
                    if (callback != null)
                        callback(true, null, applicationRequest.Context);
                }
            }

            #region Accessors

            public QS._core_c_.Base3.Message Message
            {
                get { return applicationRequest.Argument; }
            }

            #endregion
        }

        #endregion

        #region Class Subrequest

        private sealed class Subrequest : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>
        {
            public Subrequest(ReliableGroupSink owner, Request request, Feed feed)
            {
                this.owner = owner;
                this.request = request;
                this.feed = feed;
            }

            private ReliableGroupSink owner;
            private Request request;
            private Feed feed;

            public Request Request
            {
                get { return request; }
            }

            public Feed Feed
            {
                get { return feed; }
            }

            #region IAsynchronous<Message,object> Members

            QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
            {
                get { return request.Message; }
            }

            object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
            {
                get { return this; }
            }

            QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
            {
                get { return owner.completionCallback; }
            }

            #endregion
        }

        #endregion

        #region Callback

        private void CompletionCallback(bool succeeded, Exception exception, object context)
        {
            Subrequest subrequest = (Subrequest) context;
            subrequest.Request.Completed(subrequest.Feed);
        }

        #endregion

        #region GetObjects

        private void GetObjects(
            Feed requestor,
            int numberOfObjectsRequested,
#if UseEnhancedRateControl
            int numberOfBytesRequested, 
#endif
            out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
            out int numberOfBytesReturned,
#endif
            out bool moreObjectsAvailable)
        {
            // TODO: Implement enhanced rate control

#if DEBUG_MeasureGetOverheads
            double tt1 = clock.Time;
#endif

            numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
            numberOfBytesReturned = 0;
#endif

#if OPTION_SynchronizeFeeds                    
            if (!completion_blocked && !feed_blocked)
            {
#endif
                moreObjectsAvailable = true;

                lock (this)
                {
                    RecheckView();

                    while (numberOfObjectsReturned < numberOfObjectsRequested) // && numberOfBytesReturned < numberOfBytesRequested)
                    {
#if OPTION_LimitPendingCompletion
                        completion_blocked = (numberOfPendingCompletion >= maximumNumberOfPendingCompletion);
                        if (completion_blocked)
                        {
                            moreObjectsAvailable = waiting = false;
                            break;
                        }
                        else
                        {
#endif
                            if (bufferedQueue.Count > 0)
                            {
                                Request request = new Request(this, bufferedQueue.Dequeue(), feeds.Count);

#if OPTION_LimitPendingCompletion
                                numberOfPendingCompletion++;
#endif
                                numberOfObjectsReturned++;

                                // numberOfBytesReturned += ....................................................................................HERE

                                foreach (Feed feed in feeds.Values)
                                {
#if OPTION_SynchronizeFeeds                    
                                    bool got_blocked_just_now;
                                    feed.Send(new Subrequest(this, request, feed), out got_blocked_just_now);

                                    if (got_blocked_just_now)
                                    {
                                        feed_blocked = true;
                                        num_feeds_blocked++;
                                    }
#else
                                    feed.Send(new Subrequest(this, request, feed));
#endif
                                }

#if OPTION_SynchronizeFeeds                    
                                if (feed_blocked)
                                {
                                    moreObjectsAvailable = waiting = false;
                                    break;
                                }
#endif
                            }
                            else
                            {
                                while (incomingQueue.Count > 0 && bufferedQueue.Count == 0)
                                {
                                    QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getCallback = incomingQueue.Dequeue();

                                    int nreturned;
#if UseEnhancedRateControl
                                    int nbytesreturned;
#endif
                                    bool more;
                                    getCallback(bufferedQueue,
#if OPTION_SynchronizeFeeds
                                        Math.Min(numberOfObjectsRequested - numberOfObjectsReturned, feed_bufferhigh),
#else
                                        numberOfObjectsRequested - numberOfObjectsReturned,
#endif
#if UseEnhancedRateControl
                                        int.MaxValue, // numberOfBytesRequested - numberOfBytesReturned, 
#endif
 out nreturned,
#if UseEnhancedRateControl                                
                                        out nbytesreturned, 
#endif
                                        out more);

                                    if (more)
                                    {
                                        if (nreturned > 0)
                                            incomingQueue.Enqueue(getCallback);
                                        else
                                            invokedCallbackQueue.Enqueue(getCallback);
                                    }
                                }

                                while (invokedCallbackQueue.Count > 0)
                                    incomingQueue.Enqueue(invokedCallbackQueue.Dequeue());

                                if (bufferedQueue.Count == 0)
                                {
                                    moreObjectsAvailable = waiting = incomingQueue.Count > 0;
                                    break;
                                }
                            }

#if OPTION_LimitPendingCompletion
                        }
#endif
                    }
                }
#if OPTION_SynchronizeFeeds                    
            }
            else
            {
                moreObjectsAvailable = false;
                waiting = false;
            }
#endif

#if DEBUG_MeasureGetOverheads
            double tt2 = clock.Time;
            ts_GetOverheads.Add(tt1, tt2 - tt1);
            ts_GetNumberOfObjectsReturned.Add(tt2, numberOfObjectsReturned);
#endif
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
            List<Feed> toSignal = null;

            lock (this)
            {
                incomingQueue.Enqueue(getObjectsCallback);

                if (!waiting
#if OPTION_SynchronizeFeeds
                    && !completion_blocked && !feed_blocked
#endif                    
                    )
                {
                    waiting = true;
                    toSignal = new List<Feed>(feeds.Values);
                }
            }

            if (toSignal != null)
            {
                foreach (Feed feed in toSignal)
                {
#if OPTION_SynchronizeFeeds
                    bool got_blocked_just_now;
                    feed.Signal(out got_blocked_just_now);

                    if (got_blocked_just_now)
                    {
                        feed_blocked = true;
                        num_feeds_blocked++;
                    }
#else
                    feed.Signal();
#endif
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            disposed = true;
        }

        #endregion
    }
}
