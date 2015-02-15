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

namespace QS._qss_c_.Multicasting7
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class PlaceholderGS : QS.Fx.Inspection.Inspectable, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>
    {
        public PlaceholderGS(Base3_.GroupID groupID, Membership2.Controllers.IMembershipController membershipController,
            Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks)
        {
            this.membershipController = membershipController;
            this.groupID = groupID;
            this.downstreamSinks = downstreamSinks;

            groupController = membershipController[groupID];

            ViewChange(groupController.CurrentView);
        }

        private Base3_.GroupID groupID;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        private bool waiting;
        private Membership2.Controllers.IGroupController groupController;
        [QS._core_c_.Diagnostics.Component("Feed")]
        private Feed feed;        
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> incomingQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();

        #region ViewChange

        public void ViewChange(Membership2.Controllers.IGroupViewController groupViewController)
        {
            IEnumerator<Membership2.ClientState.IRegionView> rvc = groupViewController.RegionViews.GetEnumerator();
            if (!rvc.MoveNext())
                throw new Exception("Internal error");

            Membership2.Controllers.IRegionViewController regionViewController = 
                (Membership2.Controllers.IRegionViewController) rvc.Current;
            if (rvc.MoveNext())
                throw new Exception("This placeholder group sink cannot handle group views consisting of more than one region view.");

            Feed feed_to_signal = null;
            lock (this)
            {
                if (feed != null)
                    feed.Deactivate();
                
                feed = new Feed(this, // regionViewController, 
                    downstreamSinks[new Base3_.RVID(regionViewController.Region.ID, regionViewController.SeqNo)]);
                if (waiting)
                    feed_to_signal = feed;
            }

            if (feed_to_signal != null)
                feed_to_signal.Signal();
        }

        #endregion

        #region Class Feed

        [QS._core_c_.Diagnostics.ComponentContainer]
        private class Feed : QS.Fx.Inspection.Inspectable
        {
            public Feed(PlaceholderGS owner, // Membership2.Controllers.IRegionViewController regionViewController,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink)
            {
                this.owner = owner;
                // this.regionViewController = regionViewController;
                this.downstreamSink = downstreamSink;
            }

            private PlaceholderGS owner;
            // private Membership2.Controllers.IRegionViewController regionViewController;
            [QS._core_c_.Diagnostics.Component]
            private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink;
            private bool deactivated;

            public void Signal()
            {
                downstreamSink.Send(
                    new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(
                        this.GetObjectsCallback));
            }

            public void Deactivate()
            {
                deactivated = true;
            }

            private void GetObjectsCallback(
                Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoingQueue,
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
                lock (owner)
                {
                    if (!deactivated && ReferenceEquals(owner.feed, this) && owner.waiting)
                    {
                        owner.GetObjectsCallback(
                            outgoingQueue, maximumNumberOfObjects, 
#if UseEnhancedRateControl    
                            maximumNumberOfBytes, 
#endif                            
                            out numberOfObjectsReturned, 
#if UseEnhancedRateControl                                
                            out numberOfBytesReturned, 
#endif                            
                            out moreObjectsAvailable);
                    }
                    else
                    {
                        numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                        numberOfBytesReturned = 0;
#endif
                        moreObjectsAvailable = false;                        
                    }
                }
            }
        }

        #endregion

        #region GetObjectsCallback

        private void GetObjectsCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoingQueue,
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

            while (true)
            {
                if (incomingQueue.Count > 0)
                {
                    if (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                    {
                        QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getCallback = incomingQueue.Dequeue();

                        int nobjects;
#if UseEnhancedRateControl
                        int nbytes;
#endif
                        bool more;
                        getCallback(outgoingQueue, maximumNumberOfObjects - numberOfObjectsReturned, 
#if UseEnhancedRateControl
                            int.MaxValue, // maximumNumberOfBytes - numberOfBytesReturned,
#endif
                            out nobjects, 
#if UseEnhancedRateControl
                            out nbytes, 
#endif
                            out more);
                        if (more)
                            incomingQueue.Enqueue(getCallback);

                        numberOfObjectsReturned += nobjects;
                        // numberOfBytesReturned += .............................................................HERE
                    }
                    else
                        break;
                }
                else
                {
                    moreObjectsAvailable = waiting = false;
                    break;
                }
            }
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
            Feed feed_to_signal = null;
            lock (this)
            {
                incomingQueue.Enqueue(getObjectsCallback);
                if (!waiting)
                {
                    waiting = true;
                    feed_to_signal = feed;
                }
            }

            if (feed_to_signal != null)
                feed_to_signal.Signal();
        }

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
