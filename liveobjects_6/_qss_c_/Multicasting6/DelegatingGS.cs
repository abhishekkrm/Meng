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

// #define DEBUG_DelegatingGS

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Multicasting6
{
    public class DelegatingGS : Base4_.ISinkCollection<Base3_.GroupID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
    {
        public DelegatingGS(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController,
            Base4_.ISinkCollection<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> regionViewSinks)
        {
            this.logger = logger;
            this.membershipController = membershipController;
            this.regionViewSinks = regionViewSinks;
        }

        private QS.Fx.Logging.ILogger logger;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base4_.ISinkCollection<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> regionViewSinks;
        private IDictionary<Base3_.GroupID, Sender> senders = new Dictionary<Base3_.GroupID, Sender>();

        #region GetSender

        private Sender GetSender(Base3_.GroupID groupID)
        {
            lock (this)
            {
                if (senders.ContainsKey(groupID))
                    return senders[groupID];
                else
                {
                    Sender sender = new Sender(this, groupID);
                    senders[groupID] = sender;
                    return sender;
                }
            }
        }

        #endregion

        #region Class Sender

        private class Sender : Base4_.IAddressedSink<Base3_.GroupID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
        {
            public Sender(DelegatingGS owner, Base3_.GroupID groupID)
            {
                this.owner = owner;
                this.groupID = groupID;

                try
                {
                    this.Update();
                }
                catch (Exception)
                {
                }
            }

            private DelegatingGS owner;
            private Base3_.GroupID groupID;
            private Membership2.Controllers.IGroupController groupController;
            private Membership2.Controllers.IGroupViewController groupViewController;
            private uint currentMTU;
            private Queue<Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>>> pendingQueue =
                new Queue<Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>>>();
            private bool waiting = false;
            private Queue<Base4_.Asynchronous<QS._core_c_.Base3.Message>> internalWorkingQueue = 
                new Queue<Base4_.Asynchronous<QS._core_c_.Base3.Message>>();
            private RegionQueue[] regionQueues;

            #region Class Request

            private class Request
            {
                public Request(Base4_.Asynchronous<QS._core_c_.Base3.Message> request, RegionQueue[] regionQueues)
                {
                    this.request = request;

                    this.regionQueues = new List<RegionQueue>(regionQueues.Length);
                    this.regionQueues.AddRange(regionQueues);
                }

                private Base4_.Asynchronous<QS._core_c_.Base3.Message> request;
                private List<RegionQueue> regionQueues;

                public void CompletionCallback(RegionQueue regionQueue, bool succeeded, Exception exception)
                {
                    // TODO: Use the information about errors instead of just throwing it away.

                    bool completed = false;
                    lock (this)
                    {
                        if (regionQueues.Contains(regionQueue))
                        {
                            regionQueues.Remove(regionQueue);
                            if (regionQueues.Count == 0)
                                completed = true;
                        }
                    }

                    if (completed)
                    {
                        if (request.CompletionCallback != null)
                            request.CompletionCallback(true, null, request.AsynchronousState);
                    }
                }

                #region Accessors

                public QS._core_c_.Base3.Message Message
                {
                    get { return request.EncapsulatedObject; }
                }

                #endregion
            }

            #endregion

            #region Class RegionQueue

            private class RegionQueue : Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>>
            {
                public RegionQueue(Sender owner, Base3_.RVID destinationRVID)
                {
                    this.owner = owner;
                    this.destinationRVID = destinationRVID;
                    rvSink = owner.owner.regionViewSinks[destinationRVID];

                    channel = rvSink.Register(
                        new QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                            ((Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>>)this).GetObjects));
                }

                private Sender owner;
                private Base3_.RVID destinationRVID;
                private Base4_.IAddressedSink<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> rvSink;
                private Base4_.IChannel channel;
                private Queue<Request> pendingQueue = new Queue<Request>();

                public Base3_.RVID Address
                {
                    get { return destinationRVID; }
                }

                public void Add(Request request)
                {
                    lock (this)
                    {
                        // TODO: May need to signal a queue if this is a new queue.....
                        pendingQueue.Enqueue(request);
                    }
                }

                public void Signal()
                {
#if DEBUG_DelegatingGS
                    owner.owner.logger.Log(this, "__Signal(" + destinationRVID.ToString() + ")");
#endif

                    channel.Signal();
                }

                #region Callback

                private void CompletionCallback(bool succeeded, Exception exception, object context)
                {
                    ((Request)context).CompletionCallback(this, succeeded, exception);
                }

                #endregion

                #region ISource<Asynchronous<Message>> Members

                bool QS._qss_c_.Base4_.ISource<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.GetObjects(
                    ref Queue<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> returnedObjects, uint maximumSize)
                {
                    lock (this)
                    {
                        if (pendingQueue.Count == 0)
                        {
                            Monitor.Exit(this);
                            owner.RefillQueues(maximumSize);
                            Monitor.Enter(this);

                            if (pendingQueue.Count == 0)
                                return false;
                        }

                        Request request = pendingQueue.Dequeue();
                        returnedObjects.Enqueue(new QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>(request.Message,
                            new QS._qss_c_.Base4_.CompletionCallback(this.CompletionCallback), request));
                        return true;
                    }
                }

                bool QS._qss_c_.Base4_.ISource<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Ready
                {
                    get { throw new NotSupportedException(); }
                }

                #endregion
            }

            #endregion

            #region RefillQueues

            private void RefillQueues(uint maximumSize)
            {
                lock (this)
                {
                    bool ready = false;
                    while (!ready && pendingQueue.Count > 0)
                    {
                        Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>> source = pendingQueue.Dequeue();
                        if (source.GetObjects(ref internalWorkingQueue, maximumSize))
                        {
                            ready = true;
                            pendingQueue.Enqueue(source);
                        }
                    }

                    if (ready)
                    {
                        foreach (Base4_.Asynchronous<QS._core_c_.Base3.Message> element in internalWorkingQueue)
                        {
                            if (regionQueues == null)
                                Update();

                            Request request = new Request(element, regionQueues);
                            foreach (RegionQueue queue in regionQueues)
                                queue.Add(request);
                        }
                        internalWorkingQueue.Clear();
                    }
                }
            }

            #endregion

            #region Update

            private void Update()
            {
#if DEBUG_DelegatingGS
                owner.logger.Log(this, "__Update(" + groupID.ToString() + ")");
#endif

                if (groupController == null)
                    groupController = owner.membershipController[groupID];

                groupViewController = groupController.CurrentView;
                currentMTU = uint.MaxValue; // needs to be changed

                IDictionary<Base3_.RVID, RegionQueue> currentQueues = new Dictionary<Base3_.RVID, RegionQueue>();
                if (regionQueues != null)
                    foreach (RegionQueue queue in regionQueues)
                        currentQueues.Add(queue.Address, queue);

                List<RegionQueue> newQueues = new List<RegionQueue>();
                foreach (Membership2.Controllers.IRegionViewController regionVC in groupViewController.RegionViews)
                {
                    Base3_.RVID rvid = new Base3_.RVID(regionVC.Region.ID, regionVC.SeqNo);
                    if (currentQueues.ContainsKey(rvid))
                    {
                        newQueues.Add(currentQueues[rvid]);
                        currentQueues.Remove(rvid);
                    }
                    else
                    {                        
                        RegionQueue queue = new RegionQueue(this, rvid);
                        newQueues.Add(queue);
                    }

                    uint mtu = owner.regionViewSinks[rvid].MTU;
                    if (mtu < currentMTU)
                        currentMTU = mtu;
                }
                regionQueues = newQueues.ToArray();

                foreach (RegionQueue queue in currentQueues.Values)
                {
                    // TODO: This queue may not be needed any more, maybe we may need to recycle it or something...
                }

#if DEBUG_DelegatingGS
                foreach (RegionQueue queue in regionQueues)
                    owner.logger.Log(this, "RegionQueue : " + queue.Address.ToString());
#endif
            }

            #endregion

            #region SourceCallback

            private void SourceCallback(Base4_.ISource<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> source)
            {
#if DEBUG_DelegatingGS
                owner.logger.Log(this, "__SourceCallback(" + groupID.ToString() + ")");
#endif

                try
                {
                    RegionQueue[] queues = null;
                    lock (this)
                    {
                        if (!waiting)
                        {
                            if (regionQueues == null)
                                Update();

                            queues = regionQueues;
                        }

                        pendingQueue.Enqueue(source);
                        waiting = true;
                    }

                    if (queues != null)
                    {
#if DEBUG_DelegatingGS
                        owner.logger.Log(this, "__SourceCallback(" + groupID.ToString() + ") : Signaling queues");
#endif

                        foreach (RegionQueue queue in queues)
                        {
#if DEBUG_DelegatingGS
                            owner.logger.Log(this, "__SourceCallback(" + groupID.ToString() +
                                ") : Signaling " + queue.Address.ToString());
#endif

                            queue.Signal();
                        }
                    }
                    else
                    {
#if DEBUG_DelegatingGS
                        owner.logger.Log(this, "__SourceCallback(" + groupID.ToString() + ") : No queues");
#endif
                    }
                }
                catch (Exception exc)
                {
                    owner.logger.Log(this, "SourceCallback : " + exc.ToString());
                    throw;
                }
            }

            #endregion

            #region ISink<Asynchronous<Message>> Members

            QS._qss_c_.Base4_.IChannel 
                QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Register(
                    QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> getObjectCallback)
            {
                return new Base4_.Channel<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                    new QS._qss_c_.Base4_.SourceCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                        this.SourceCallback), getObjectCallback);
            }

            uint QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.MTU
            {
                get { return currentMTU; }
            }

            #endregion

            #region IAddressedSink<GroupID,Asynchronous<Message>> Members

            QS._qss_c_.Base3_.GroupID QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.GroupID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Address
            {
                get { return groupID; }
            }

            #endregion
        }

        #endregion

        #region ISinkCollection<GroupID,Asynchronous<Message>> Members

        QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.GroupID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> QS._qss_c_.Base4_.ISinkCollection<QS._qss_c_.Base3_.GroupID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.this[QS._qss_c_.Base3_.GroupID destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion
    }
}
