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

#define DEBUG_PermitLogging

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Framework_2_
{
    public sealed class Group : IDisposable
    {
        #region Constructor

        public Group(QS._qss_c_.Base3_.GroupID groupID, QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
            Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> underlyingSinkCollection,
            Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> hybridUnderlyingSinkCollection,
            QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent> mainWorker,
            QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent> completionWorker,
            QS.Fx.Base.ContextCallback<Group> subscribeCallback, QS.Fx.Base.ContextCallback<Group> cancelSubscribeCallback,
            QS.Fx.Base.ContextCallback<Group> unsubscribeCallback, QS.Fx.Base.ContextCallback<Group> cancelUnsubscribeCallback,
            QS.Fx.Base.ContextCallback<Group> removeCallback)
        {
            this.groupID = groupID;
            this.logger = logger;
            this.hybridUnderlyingSinkCollection = hybridUnderlyingSinkCollection;
            this.subscribeCallback = subscribeCallback;
            this.cancelSubscribeCallback = cancelSubscribeCallback;
            this.unsubscribeCallback = unsubscribeCallback;
            this.cancelUnsubscribeCallback = cancelUnsubscribeCallback;
            this.removeCallback = removeCallback;
            this.mainWorker = mainWorker;
            this.completionWorker = completionWorker;
            this.completionCallback = new QS._core_c_.Base6.CompletionCallback<SendRequest>(this.CompletionCallback);

            this.underlyingSinkCollection = underlyingSinkCollection;
            this.sendingCallback = new QS.Fx.Base.ContextCallback<SendRequest>(this.SendingCallback);
            this.hybridSendingCallback = new QS.Fx.Base.ContextCallback<SendRequest>(this.HybridSendingCallback);
            this.registerFeed = new QS.Fx.Base.ContextCallback<Feed>(this.RegisterFeed);
            this.hybridRegisterFeed = new QS.Fx.Base.ContextCallback<Feed>(this.HybridRegisterFeed);
            this.getObjectsCallback = new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback);
            this.hybridGetObjectsCallback = 
                new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.HybridGetObjectsCallback);
        }

        #endregion

        #region Fields

        private QS._qss_c_.Base3_.GroupID groupID;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Base.ContextCallback<Group> subscribeCallback, cancelSubscribeCallback, 
            unsubscribeCallback, cancelUnsubscribeCallback, removeCallback;
        private State state = State.New;
        private Queue<OpenRequest> pendingOpening = new Queue<OpenRequest>();
        private IList<GroupRef> groupRefs = new List<GroupRef>();
        private QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent> mainWorker, completionWorker;
        private QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback;

        private Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> underlyingSinkCollection,
            hybridUnderlyingSinkCollection;
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> underlyingSink, hybridUnderlyingSink;

        private QS.Fx.Base.ContextCallback<SendRequest> sendingCallback, hybridSendingCallback;
        private QS.Fx.Base.ContextCallback<Feed> registerFeed, hybridRegisterFeed;
        private bool waiting_for_send, hybrid_waiting_for_send;
        private QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback, hybridGetObjectsCallback;
        private Queue<SendRequest> pendingSendRequests = new Queue<SendRequest>(),
            hybridPendingSendRequests = new Queue<SendRequest>();
        private Queue<Feed> pendingFeeds = new Queue<Feed>(), hybridPendingFeeds = new Queue<Feed>();
        private Queue<Feed> pendingFeedsDrained = new Queue<Feed>(), hybridPendingFeedsDrained = new Queue<Feed>();

        #endregion

        #region State

        private enum State
        {
            New,                        // freshly created
            Subscribing1,         // scheduled to send subscribe request       
            Subscribing2,         // subscribe request sent
            Active,                     // subscribed
            Unsubscribing1,     // scheduled to send ubsubscribe request
            Unsubscribing2,     // unsubscribe request sent still requested
            Unsubscribing3      // unsubscribe sent, but will resubscribe
        }

        #endregion

        #region Accessors

        public Base3_.GroupID ID
        {
            get { return groupID; }
        }

        #endregion

        #region Process Open Request

        public void ProcessOpenRequest(OpenRequest openreq)
        {
#if DEBUG_PermitLogging
            if (logger != null)
                logger.Log(this, "ProcessOpenRequest(" + groupID.ToString() + ")");
#endif

            switch (state)
            {
                case State.New:
                    {
                        state = State.Subscribing1;
                        pendingOpening.Enqueue(openreq);
                        subscribeCallback(this);
                    }
                    break;

                case State.Subscribing1:
                case State.Subscribing2:
                    {
                        pendingOpening.Enqueue(openreq);
                    }
                    break;

                case State.Active:
                    {
                        SatisfyOpenRequest(openreq);
                    }
                    break;

                case State.Unsubscribing1:
                    {
                        state = State.Active;
                        cancelUnsubscribeCallback(this);
                        SatisfyOpenRequest(openreq);
                    }
                    break;

                case State.Unsubscribing2:
                    {
                        state = State.Unsubscribing3;
                        pendingOpening.Enqueue(openreq);
                    }
                    break;

                case State.Unsubscribing3:
                    {
                        pendingOpening.Enqueue(openreq);
                    }
                    break;
            }
        }

        #endregion

        #region ReallyWantsToSubscribe

        public bool ReallyWantsToSubscribe()
        {
            if (state == State.Subscribing1)
            {
                state = State.Subscribing2;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region ReallyWantsToUnsubscribe

        public bool ReallyWantsToUnsubscribe()
        {
            if (state == State.Unsubscribing1)
            {
                state = State.Unsubscribing2;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region SatisfyOpenRequest

        private void SatisfyOpenRequest(OpenRequest openreq)
        {
#if DEBUG_PermitLogging
            if (logger != null)
                logger.Log(this, "SatisfyOpenRequest(" + groupID.ToString() + ")");
#endif

            bool is_hybrid = ((openreq.Options & GroupOptions.Hybrid) == GroupOptions.Hybrid);
            GroupRef groupRef = new GroupRef(this, openreq.Options, logger, is_hybrid ? hybridSendingCallback : sendingCallback, 
                completionCallback, is_hybrid ? hybridRegisterFeed : registerFeed, mainWorker, completionWorker);
            groupRefs.Add(groupRef);

            openreq.GroupRef = groupRef;
            openreq.IsCompleted = true;

            completionWorker.Process(openreq);
        }

        #endregion

        #region MembershipChanged

        public void MembershipChanged(bool creation)
        {
#if DEBUG_PermitLogging
            if (logger != null)
                logger.Log(this, "MembershipChanged(" + groupID.ToString() + ", creation = " + creation.ToString() + ")");
#endif

            if (creation)
            {
                if (state != State.Subscribing2)
                    throw new Exception("Internal error: should have been waiting for the GMS to process join request.");

                underlyingSink = underlyingSinkCollection[groupID];
                if (hybridUnderlyingSinkCollection != null)
                    hybridUnderlyingSink = hybridUnderlyingSinkCollection[groupID];
                state = State.Active;

                foreach (OpenRequest openreq in pendingOpening)
                    SatisfyOpenRequest(openreq);

                pendingOpening.Clear();
            }
            else
            {
                switch (state)
                {
                    case State.Unsubscribing2:
                    case State.Unsubscribing3:
                        {
                            if (pendingOpening.Count > 0)
                            {
                                state = State.Subscribing1;
                                subscribeCallback(this);
                            }
                            else
                            {
                                if (groupRefs.Count > 0)
                                    throw new Exception("Internal error: unsubscribed, but there are still registered refs around.");

                                removeCallback(this);
                            }
                        }
                        break;

                    default:
                        throw new Exception("Internal error: should have been waiting for the GMS to process leave request.");
                }
            }
        }

        #endregion

        #region Remove Reference

        public void RemoveReference(GroupRef groupRef, bool safe)
        {
#if DEBUG_PermitLogging
            if (logger != null)
                logger.Log(this, "RemoveReference(" + groupID.ToString() + ")");
#endif

            if (safe)
            {
                groupRefs.Remove(groupRef);

                if (groupRefs.Count == 0)
                {
                    if (state != State.Active)
                        throw new Exception("Internal error: removing references in a state other than active.");

#if DEBUG_PermitLogging
                    if (logger != null)
                        logger.Log(this, "RemoveReference(" + groupID.ToString() + ") - Requesting Unsubscribe");
#endif

                    state = State.Unsubscribing1;
                    unsubscribeCallback(this);
                }
            }
            else
            {
#if DEBUG_PermitLogging
                if (logger != null)
                    logger.Log(this, "RemoveReference(" + groupID.ToString() + ") - Unsafe, Deferring to Core");
#endif

                mainWorker.Process(new CloseRequest(this, groupRef));
            }
        }

        #endregion

        #region Sending Callbacks

        private void SendingCallback(SendRequest request)
        {
            pendingSendRequests.Enqueue(request);
            _Signal();
        }

        private void HybridSendingCallback(SendRequest request)
        {
            hybridPendingSendRequests.Enqueue(request);
            _HybridSignal();
        }

        #endregion

        #region Register Feed Callbacks

        private void RegisterFeed(Feed feed)
        {
            pendingFeeds.Enqueue(feed);
            _Signal();
        }

        private void HybridRegisterFeed(Feed feed)
        {
            hybridPendingFeeds.Enqueue(feed);
            _HybridSignal();
        }

        #endregion

        #region Signal

        private void _Signal()
        {
            if (!waiting_for_send)
            {
                waiting_for_send = true;
                underlyingSink.Send(getObjectsCallback);
            }
        }

        private void _HybridSignal()
        {
            if (!hybrid_waiting_for_send)
            {
                hybrid_waiting_for_send = true;
                hybridUnderlyingSink.Send(hybridGetObjectsCallback);
            }
        }

        #endregion

        #region Get Objects Callbacks

        private void GetObjectsCallback(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue,
            int maximumNumberOfObjects, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
        {
            numberOfObjectsReturned = 0;
            moreObjectsAvailable = true;

            while (numberOfObjectsReturned < maximumNumberOfObjects)
            {
                if (pendingSendRequests.Count > 0)
                {
                    objectQueue.Enqueue(pendingSendRequests.Dequeue());
                    numberOfObjectsReturned++;
                }
                else
                {
                    bool gotsomething = false;
                    while (pendingFeeds.Count > 0)
                    {
                        Feed feed = pendingFeeds.Dequeue();

                        bool hasmore;
                        feed.GetObjects(((uint)(maximumNumberOfObjects - numberOfObjectsReturned)), out hasmore);

                        if (pendingSendRequests.Count > 0)
                        {
                            if (hasmore)
                                pendingFeeds.Enqueue(feed);

                            gotsomething = true;
                            break;
                        }
                        else
                        {
                            if (hasmore)
                                pendingFeedsDrained.Enqueue(feed);
                        }
                    }

                    if (!gotsomething)
                    {
                        if (pendingFeedsDrained.Count == 0)
                        {
                            moreObjectsAvailable = false;
                            waiting_for_send = false;
                        }

                        break;
                    }
                }
            }

            while (pendingFeedsDrained.Count > 0)
                pendingFeeds.Enqueue(pendingFeedsDrained.Dequeue());
        }

        private void HybridGetObjectsCallback(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue,
            int maximumNumberOfObjects, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
        {
            numberOfObjectsReturned = 0;
            moreObjectsAvailable = true;

            while (numberOfObjectsReturned < maximumNumberOfObjects)
            {
                if (hybridPendingSendRequests.Count > 0)
                {
                    objectQueue.Enqueue(hybridPendingSendRequests.Dequeue());
                    numberOfObjectsReturned++;
                }
                else
                {
                    bool gotsomething = false;
                    while (hybridPendingFeeds.Count > 0)
                    {
                        Feed feed = hybridPendingFeeds.Dequeue();

                        bool hasmore;
                        feed.GetObjects(((uint)(maximumNumberOfObjects - numberOfObjectsReturned)), out hasmore);

                        if (hybridPendingSendRequests.Count > 0)
                        {
                            if (hasmore)
                                hybridPendingFeeds.Enqueue(feed);

                            gotsomething = true;
                            break;
                        }
                        else
                        {
                            if (hasmore)
                                hybridPendingFeedsDrained.Enqueue(feed);
                        }
                    }

                    if (!gotsomething)
                    {
                        if (hybridPendingFeedsDrained.Count == 0)
                        {
                            moreObjectsAvailable = false;
                            hybrid_waiting_for_send = false;
                        }

                        break;
                    }
                }
            }

            while (hybridPendingFeedsDrained.Count > 0)
                hybridPendingFeeds.Enqueue(hybridPendingFeedsDrained.Dequeue());
        }

        #endregion

        #region Sending

        public void Send(SendRequest request)
        {
            pendingSendRequests.Enqueue(request);
        }

        public void HybridSend(SendRequest request)
        {
            hybridPendingSendRequests.Enqueue(request);
        }

        #endregion

        #region Completion Callback

        private void CompletionCallback(bool succeeded, Exception exception, SendRequest request)
        {
            CompletionCallback applicationCallback = request.ApplicationCallback;
            if (applicationCallback != null)
            {
                if ((request.GroupRef.Options & GroupOptions.FastCompletionCallback) == GroupOptions.FastCompletionCallback)
                {
                    applicationCallback(succeeded, exception, request.ApplicationContext);
                }
                else
                {
                    request.Completed = true;
                    request.Succeeded = succeeded;
                    request.Exception = exception;
                    completionWorker.Process(request);
                }
            }
        }

        #endregion

        #region ReceiveCallback

        public QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sender, QS.Fx.Serialization.ISerializable message)
        {
            foreach (GroupRef groupRef in groupRefs)
                groupRef.DispatchReceivedMessage(sender, message, false);
            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
