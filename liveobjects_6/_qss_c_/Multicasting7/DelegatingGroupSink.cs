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

namespace QS._qss_c_.Multicasting7
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class DelegatingGroupSink : QS.Fx.Inspection.Inspectable, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>
    {
        public DelegatingGroupSink(Base3_.GroupID groupID, Membership2.Controllers.IMembershipController membershipController,
            Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks,
            int maximumNumberOfPendingCompletion)
        {
            this.membershipController = membershipController;
            this.groupID = groupID;
            this.downstreamSinks = downstreamSinks;
            this.maximumNumberOfPendingCompletion = maximumNumberOfPendingCompletion;
/*
            this.maximumConcurrency = maximumConcurrency;
*/ 
        }

        private Base3_.GroupID groupID;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        private int maximumNumberOfPendingCompletion;

/* 
        private RegionQueue[] regionQueues;

        private int concurrency, maximumConcurrency;
        private bool registered; // true iff all region queues are in a signaled state and will keep pulling messages from the group
        private Queue<Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>>> pendingQueue =
            new Queue<QS.CMS.Base6.GetObjectsCallback<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>>();

        #region

        private void RefillQueues()
        {

            while (true)
            {
                if (pendingQueue.Count > 0)
                {
                    Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>> getCallback = pendingQueue.Dequeue();

                    getCallback(


                }
                else
                {
                    registered = false;
                    break;
                }
            }
        }

        #endregion

        #region Class RegionQueue

        private class RegionQueue : Base6.IChannel
        {
            public RegionQueue(DelegatingGS owner, Base3.RVID rvid, Base6.ISink<Base6.IAsynchronous<Base3.Message>> sink)
            {
                this.owner = owner;
                this.rvid = rvid;
                this.sink = sink;
            }

            private DelegatingGS owner;
            private Base3.RVID rvid;
            private Base6.ISink<Base6.IAsynchronous<Base3.Message>> sink;
            private bool registered, destroyed;
            private Queue<Subrequest> pendingQueue = new Queue<Subrequest>();

            #region GetCallback

            private void GetCallback(Queue<Base6.IAsynchronous<Base3.Message>> outgoingQueue,
                int maximumNumberOfObjects, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
            {
                lock (this)
                {
                    numberOfObjectsReturned = 0;
                    moreObjectsAvailable = true;

                    bool refilled = false;
                    while (true)
                    {
                        if (pendingQueue.Count > 0)
                        {
                            if (numberOfObjectsReturned < maximumNumberOfObjects)
                            {
                                outgoingQueue.Enqueue(pendingQueue.Dequeue());
                                numberOfObjectsReturned++;
                            }
                            else
                                break;
                        }
                        else
                        {
                            if (destroyed)
                            {
                                moreObjectsAvailable = false;
                                break;
                            }
                            else
                                owner.RefillQueues();
                        }
                    }
                }
            }

            #endregion

            #region IChannel Members

            void QS.CMS.Base6.IChannel.Signal()
            {
                bool signal_now;
                lock (this)
                {
                    signal_now = !registered;
                    registered = true;
                }

                if (signal_now)
                    sink.Send(
                        new QS.CMS.Base6.GetObjectsCallback<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>(
                            this.GetCallback));
            }

            #endregion
        }

        #endregion

        #region Class Request

        private class Request
        {
            public Request(Base6.IAsynchronous<Base3.Message> argument, int numberOfSubrequests)
            {
                this.argument = argument;
                this.subrequestsToGo = numberOfSubrequests;
            }

            private Base6.IAsynchronous<Base3.Message> argument;
            private int subrequestsToGo;

            public Base6.IAsynchronous<Base3.Message> Argument
            {
                get { return argument; }
            }

            #region SubrequestCompleted

            public void SubrequestCompleted(bool succeeded, Exception exception, object context)
            {
                bool completed_now;
                lock (this)
                {
                    // TODO: We should properly report failures of subrequests..................

                    subrequestsToGo--;
                    completed_now = subrequestsToGo == 0;
                }

                if (completed_now && argument.Callback != null)
                    argument.Callback(true, null, argument.Context);
            }

            #endregion
        }

        #endregion

        #region Class Subrequest

        public class Subrequest : Base6.IAsynchronous<Base3.Message>
        {
            public Subrequest(Request request)
            {
                this.request = request;
            }

            private Request request;

            #region IAsynchronous<Message,object> Members

            QS.CMS.Base3.Message QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message, object>.Argument
            {
                get { return request.Argument; }
            }

            object QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message, object>.Context
            {
                get { return this; }
            }

            QS.CMS.Base6.Callback<object> QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message, object>.Callback
            {
                get { return new QS.CMS.Base6.Callback<object>(request.SubrequestCompleted); }
            }

            #endregion
        }

        #endregion
*/

        #region ISink<IAsynchronous<Message>> Members

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)        
        {            
/*
            lock (this)
            {
                pendingQueue.Enqueue(getObjectsCallback);
                if (!registered)
                {
                    registered = true;
                    RefillQueues();
                }
            }
*/ 
        }

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
