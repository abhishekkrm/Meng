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
// #define STATISTICS_RecordRoundtripTimes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Multicasting5
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class DelegatingGS : QS.Fx.Inspection.Inspectable, Multicasting3.ISimpleSender
    {
        public DelegatingGS(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, Membership2.Controllers.IMembershipController membershipController,
            Base3_.ISenderCollection<Base3_.RVID, Base3_.IReliableSerializableSender> underlyingSenderCollection, int bufferSize)
        {
            this.logger = logger;
            this.clock = clock;
            this.membershipController = membershipController;
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.bufferSize = bufferSize;

            inspectableWrapper_SenderCollection = new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.GroupID, Sender>("Sender Collection", 
                senders, new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.GroupID,Sender>.ConversionCallback(Base3_.GroupID.FromString));
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;
        private Base3_.ISenderCollection<Base3_.RVID, Base3_.IReliableSerializableSender> underlyingSenderCollection;
        private Membership2.Controllers.IMembershipController membershipController;
        private int bufferSize;

        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<Base3_.GroupID, Sender> senders = new Dictionary<Base3_.GroupID, Sender>();
        [QS.Fx.Base.Inspectable("Sender Collection", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.GroupID, Sender> inspectableWrapper_SenderCollection;

        #region Class Sender

        // [TMS.Inspection.Inspectable]
        private class Sender : Multicasting3.GroupSender, QS._qss_e_.Base_1_.IStatisticsCollector
        {
            public Sender(DelegatingGS owner, Base3_.GroupID groupID) : base(groupID)
            {
                this.owner = owner;
                this.groupController = owner.membershipController[groupID];

                outgoingController = new FlowControl2.OutgoingController<Request>(owner.bufferSize, owner.bufferSize,
                    new QS._qss_c_.FlowControl2.ReadyCallback<Request>(this.ReadyCallback), owner.clock);
            }

            private DelegatingGS owner;
            private Membership2.Controllers.IGroupController groupController;
            [QS.Fx.Base.Inspectable]
            private FlowControl2.IOutgoingController<Request> outgoingController;
            [QS.Fx.Base.Inspectable]
            private uint lastused_seqno = 0;
            private Membership2.Controllers.IGroupViewController groupViewController;
            [QS.Fx.Base.Inspectable]
            private uint viewSeqNo;
            [QS.Fx.Base.Inspectable]
            private List<Base3_.IReliableSerializableSender> regionSenderCollection;

#if STATISTICS_RecordRoundtripTimes
            [QS.CMS.Diagnostics.Component("Roundtrip Times")]
            private QS.CMS.Statistics.Samples roundtripTimes = new QS.CMS.Statistics.Samples();               
#endif

            #region Internal Processing

            private void ReadyCallback(IEnumerable<Base3_.Seq<Request>> readyQueue)
            {
                // throw new NotSupportedException("This callback should never be called, queue should be always consumed synchronously.");

                foreach (Base3_.Seq<Request> element in readyQueue)
                {
                    if (element.SeqNo != element.Object.Message.ID.MessageSeqNo)
                        owner.logger.Log(this, "__ReadyCallback: Internal error, sequence numbers do not match.");
                    else
                        element.Object.Submit();
                }
            }

            private void RemoveComplete(Request request)
            {
#if STATISTICS_RecordRoundtripTimes
                roundtripTimes.addSample(((int)request.Message.ID.MessageSeqNo) - 1, request.RTT);  
#endif

                outgoingController.removeCompleted((int) request.Message.ID.MessageSeqNo);
            }

            #endregion

            #region Overrides from Multicasting3.GroupSender

            public override QS._qss_c_.Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
#if DEBUG_DelegatingGS
                owner.logger.Log(this, "__BeginSend: " + groupID.ToString() + ", " + destinationLOID.ToString() + ", " + data.ToString());
#endif

                Request request;

                // DEADLOCK (2)
                lock (this)
                {
                    Membership2.Controllers.IGroupViewController groupVC = groupController.CurrentView;
                    if (groupVC != groupViewController)
                    {
                        groupViewController = groupVC;
                        viewSeqNo = groupVC.SeqNo;

                        regionSenderCollection = new List<Base3_.IReliableSerializableSender>(groupViewController.RegionViewControllers.Length);
                        foreach (Membership2.Controllers.IRegionViewController regionVC in groupViewController.RegionViews)
                            regionSenderCollection.Add(owner.underlyingSenderCollection[new Base3_.RVID(regionVC.Region.ID, regionVC.SeqNo)]);                        
                    }
                    
                    request = new Request(this, new Multicasting3.MulticastMessage(new Multicasting3.MessageID(groupID, viewSeqNo, 
                        ++lastused_seqno), new QS._core_c_.Base3.Message(destinationLOID, data)), regionSenderCollection, completionCallback, asynchronousState);

                    outgoingController.schedule(request);
                }

                return request;
            }

            public override void EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            public override int MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region Request

            private class Request : Multicasting3.MulticastRequest
            {
                public Request(Sender owner, Multicasting3.MulticastMessage message, IList<Base3_.IReliableSerializableSender> regionSenders,
                    QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) 
                    : base(message, completionCallback, asynchronousState)
                {
                    this.owner = owner;
                    this.regionSenders = regionSenders;

                    npending = regionSenders.Count;
                }

                private Sender owner;
                private IList<Base3_.IReliableSerializableSender> regionSenders;
                private int npending;

#if STATISTICS_RecordRoundtripTimes
                private double submissionTime, completionTime;

                #region Statistics

                public double RTT
                {
                    get { return completionTime - submissionTime; }
                }

                #endregion
#endif

                #region Internal Processing

                public void Submit()
                {
#if DEBUG_DelegatingGS
                    owner.owner.logger.Log(this, "__Submit: " + message.ToString());
#endif

#if STATISTICS_RecordRoundtripTimes
                    submissionTime = owner.owner.clock.Time;
#endif

                    foreach (Base3_.IReliableSerializableSender regionSender in regionSenders)
                    {
                        regionSender.BeginSend(message.Message.destinationLOID, message.Message.transmittedObject,
                            new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.RegionCompletionCallback), null);
                    }
                }

                private void RegionCompletionCallback(Base3_.IAsynchronousOperation asynchronousOperation)
                {
#if DEBUG_DelegatingGS
                    owner.owner.logger.Log(this, "__RegionCompletionCallback: " + message.ToString());
#endif

                    lock (this)
                    {
                        npending--;
                        if (npending == 0)
                        {
#if DEBUG_DelegatingGS
                            owner.owner.logger.Log(this, "__RegionCompletionCallback: " + message.ToString() + ", Completed.");
#endif

#if STATISTICS_RecordRoundtripTimes
                            completionTime = owner.owner.clock.Time;
#endif

                            this.IsCompleted = true;
                        }
                    }
                }

                public override void Unregister()
                {
                    owner.RemoveComplete(this);
                }

                #endregion

                #region Accessors

                public Multicasting3.MulticastMessage Message
                {
                    get { return message; }
                }

//                public IList<Base3.IReliableSerializableSender> RegionSenders
//                {
//                    get { return regionSenders; }
//                }

                #endregion
            }

            #endregion

            #region IStatisticsCollector Members

            IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
            {
                get
                {
                    List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();

#if STATISTICS_RecordRoundtripTimes
                    statistics.Add(new QS.CMS.Components.Attribute("RTT", roundtripTimes.DataSet));
#endif

                    return statistics; 
                }
            }

            #endregion
        }

        #endregion

        #region IGroupSenderClass<IGroupSender> Members

        QS._qss_c_.Multicasting3.IGroupSender QS._qss_c_.Multicasting3.IGroupSenderClass<QS._qss_c_.Multicasting3.IGroupSender>.this[QS._qss_c_.Base3_.GroupID groupID]
        {
            get 
            {
                Sender sender;
                lock (this)
                {
                    if (senders.ContainsKey(groupID))
                        sender = senders[groupID];
                    else
                        senders[groupID] = sender = new Sender(this, groupID);
                }
                return sender;
            }
        }

        #endregion

        #region IStatisticsCollector Members

        IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
        {
            get 
            {
                List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>(senders.Count);
                foreach (Sender sender in senders.Values)
                {
                    statistics.Add(new QS._core_c_.Components.Attribute(sender.GroupID.ToString(),
                        new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector)sender).Statistics)));
                }
                return statistics;
            }
        }

        #endregion
    }  
}
