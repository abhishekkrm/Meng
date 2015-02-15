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
using System.Threading;

namespace QS._qss_c_.Multicasting7
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class AlternativeReliableGroupSink : 
        QS.Fx.Inspection.Inspectable, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Constructor

        public AlternativeReliableGroupSink(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Clock.IClock clock,
            Base3_.GroupID groupID, Membership2.Controllers.IMembershipController membershipController, QS.Fx.Logging.ILogger logger,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink, uint transmissionChannel,
            Base6_.ICollectionOf<Base3_.RVID, ReliableRegionViewSink> reliableRegionViewSinks,
            QS._core_c_.FlowControl3.IRateControlled rateControlled, FlowControl7.IRateController rateController, double initialrate)
        {
            this.clock = clock;
            this.membershipController = membershipController;
            this.groupID = groupID;
            this.downstreamSink = downstreamSink;
            this.logger = logger;
            this.reliableRegionViewSinks = reliableRegionViewSinks;
            this.transmissionChannel = transmissionChannel;
            this.rateControlled = rateControlled;
            this.rateController = rateController;

            rateControlled.MaximumRate = initialrate;

            myGetCallback = new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback);
            completionCallback = new RequestRV2.Callback(this.CompletionCallback);

            InitializeView();

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        #endregion

        private QS._core_c_.FlowControl3.IRateControlled rateControlled;
        private FlowControl7.IRateController rateController;
        private QS.Fx.Clock.IClock clock;
        private Base3_.GroupID groupID;
        private Membership2.Controllers.IMembershipController membershipController;
        private QS.Fx.Logging.ILogger logger;
        private bool waiting;
        private Base6_.ICollectionOf<Base3_.RVID, ReliableRegionViewSink> reliableRegionViewSinks;
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink;
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> incomingQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> invokedCallbackQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> myGetCallback;
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> bufferedQueue = new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();
        private RequestRV2.Callback completionCallback;
        private Membership2.Controllers.IGroupController groupController;
        private Membership2.Controllers.IGroupViewController groupViewController;
        private Base3_.RVID[] rvids;
        private ReliableRegionViewSink[] rvsinks;
        private uint transmissionChannel;

        #region Initialize View

        private void InitializeView()
        {
            groupController = membershipController[groupID];
            groupViewController = groupController.CurrentView;

            RecheckView(true);
        }

        #endregion

        #region Membership change callback

        public void MembershipChanged(Membership2.Controllers.IGroupViewController groupVC)
        {
            logger.Log(this, "__________MembershipChanged: [new view = " + groupVC.SeqNo.ToString() + "]");

            RecheckView(false);
        }

        #endregion

        #region RecheckView

        private void RecheckView(bool force)
        {
            Membership2.Controllers.IGroupViewController currentGroupViewController = groupController.CurrentView;

            bool changed = !ReferenceEquals(currentGroupViewController, groupViewController);
            if (force || changed)
            {
                logger.Log(this, "__________GV(changed = " + changed.ToString() + ") : " +
                    groupViewController.SeqNo.ToString() + " -> " + currentGroupViewController.SeqNo.ToString());
                groupViewController = currentGroupViewController;

                List<Base3_.RVID> new_rvids = new List<QS._qss_c_.Base3_.RVID>();
                List<ReliableRegionViewSink> new_rvsinks = new List<ReliableRegionViewSink>();

                foreach (Membership2.ClientState.IRegionView element in groupViewController.RegionViews)
                {
                    Membership2.Controllers.IRegionViewController regionViewController = (Membership2.Controllers.IRegionViewController)element;
                    Base3_.RVID rvid = new QS._qss_c_.Base3_.RVID(regionViewController.Region.ID, regionViewController.SeqNo);
                    new_rvids.Add(rvid);
                    new_rvsinks.Add(reliableRegionViewSinks[rvid]);
                }

                rvids = new_rvids.ToArray();
                rvsinks = new_rvsinks.ToArray();
            }
        }

        #endregion

        #region Callback

        private void CompletionCallback(RequestRV2 request)
        {
            // do nothing
        }

        #endregion

        #region GetObjectsCallback

        private void GetObjectsCallback(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue, 
            int maximumNumberOfObjects, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
        {
            numberOfObjectsReturned = 0;
            moreObjectsAvailable = true;

            RecheckView(false);

            while (numberOfObjectsReturned < maximumNumberOfObjects)
            {
                if (bufferedQueue.Count > 0)
                {
                    uint[] seqnos = new uint[rvids.Length];
                    RequestRV2 request = new RequestRV2(
                        rvids, seqnos, transmissionChannel, bufferedQueue.Dequeue(), null); // completionCallback);
                    for (int ind = 0; ind < seqnos.Length; ind++)
                        rvsinks[ind].RegisterObject(request, out seqnos[ind]);

                    objectQueue.Enqueue(request);
                    numberOfObjectsReturned++;
                }
                else
                {
                    while (incomingQueue.Count > 0 && bufferedQueue.Count == 0)
                    {
                        QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getCallback = incomingQueue.Dequeue();

                        int nreturned;
                        bool more;
                        getCallback(bufferedQueue, maximumNumberOfObjects - numberOfObjectsReturned, out nreturned, out more);

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
            }
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
            bool signaled_now = false;
            lock (this)
            {
                incomingQueue.Enqueue(getObjectsCallback);
                if (!waiting)
                    waiting = signaled_now = true;
            }

            if (signaled_now)
                downstreamSink.Send(myGetCallback);
        }

        #endregion
    }
}
