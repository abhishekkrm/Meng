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

namespace QS._qss_c_.Gossiping5
{
    public class GossipingRRVS : Base4_.ISinkCollection<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
    {
        public GossipingRRVS(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
            Membership2.Controllers.IMembershipController membershipController,
            Base4_.ISinkCollection<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> regionSinks)
        {
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.membershipController = membershipController;
            this.regionSinks = regionSinks;

            ((Membership2.Consumers.IRegionChangeProvider) membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.RegionChangedCallback(this.MembershipChangeCallback);
            demultiplexer.register((uint)ReservedObjectID.Gossiping5_GossipingRRVS_MessageChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base4_.ISinkCollection<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> regionSinks;
        private IDictionary<Base3_.RVID, Sender> senders = new Dictionary<Base3_.RVID, Sender>();
        private IDictionary<Base3_.RVID, Receiver> receivers = new Dictionary<Base3_.RVID, Receiver>();

        #region MembershipChangeCallback

        private void MembershipChangeCallback(QS._qss_c_.Membership2.Consumers.RegionChange change)
        {
            // TODO: process this stuff and manage receivers based on input from the membership controller.........................
        }

        #endregion

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            // TODO: Perform some kind of filtering on region views.............................so we don't get any messages from other views 

            Multicasting5.MessageRV messageRV = receivedObject as Multicasting5.MessageRV;
            if (messageRV != null)
                GetReceiver(messageRV.RVID).ReceiveCallback(
                    sourceAddress, messageRV.SeqNo, messageRV.EncapsulatedMessage);

            return null;
        }

        #endregion

        #region Class Receiver

        private class Receiver
        {
            public Receiver(GossipingRRVS owner, Base3_.RVID rvid)
            {
                this.owner = owner;
                this.rvid = rvid;
            }

            private GossipingRRVS owner;
            private Base3_.RVID rvid;
            private IDictionary<QS._core_c_.Base3.InstanceID, SourceController> sourceControllers = 
                new Dictionary<QS._core_c_.Base3.InstanceID, SourceController>();

            #region Class SourceController

            private class SourceController
            {
                public SourceController(Receiver owner, QS._core_c_.Base3.InstanceID sourceAddress)
                {
                    this.owner = owner;
                    this.sourceAddress = sourceAddress;
                }

                private Receiver owner;
                private QS._core_c_.Base3.InstanceID sourceAddress;
                private IAckCollection ackCollection = new AckCollection();

                #region Receive Callback

                public void ReceiveCallback(uint seqno, QS._core_c_.Base3.Message message)
                {
                    if (ackCollection.Add(seqno))
                        owner.owner.demultiplexer.dispatch(message.destinationLOID, sourceAddress, message.transmittedObject);

                    // TODO: Implement the receive callabck...................................................................
                }

                #endregion
            }

            #endregion

            #region GetController

            private SourceController GetController(QS._core_c_.Base3.InstanceID sourceAddress)
            {
                lock (this)
                {
                    if (sourceControllers.ContainsKey(sourceAddress))
                        return sourceControllers[sourceAddress];
                    else
                    {
                        SourceController sourceController = new SourceController(this, sourceAddress);
                        sourceControllers[sourceAddress] = sourceController;
                        return sourceController;
                    }
                }
            }

            #endregion

            #region Receive Callback

            public void ReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, uint seqno, QS._core_c_.Base3.Message message)
            {
                GetController(sourceAddress).ReceiveCallback(seqno, message);
            }

            #endregion
        }

        #endregion

        #region Class Sender

        private class Sender : Base4_.IAddressedSink<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
        {
            public Sender(GossipingRRVS owner, Base3_.RVID destinationRVID)
            {
                this.owner = owner;
                this.destinationRVID = destinationRVID;

                underlyingSink = owner.regionSinks[destinationRVID.RegionID];
                outgoingChannel = underlyingSink.Register(
                    new Base4_.GetObjectsCallback<Base4_.Asynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback));
            }

            private GossipingRRVS owner;
            private Base3_.RVID destinationRVID;
            private Base4_.IAddressedSink<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> underlyingSink;
            private Base4_.IChannel outgoingChannel;
            private Queue<Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>>> pendingQueue =
                new Queue<Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>>>();
            private bool waiting = false;
            private Queue<Base4_.Asynchronous<QS._core_c_.Base3.Message>> internalWorkingQueue = 
                new Queue<Base4_.Asynchronous<QS._core_c_.Base3.Message>>();
            private uint lastSeqNo = 0;
            private IRequestContainer<Request> requestContainer = new RequestContainer<Request>();

            #region Class Request

            private class Request : QS.Fx.Serialization.ISerializable
            {
                public const uint HeaderOverhead = 0;

                public Request(Sender owner, Base4_.Asynchronous<QS._core_c_.Base3.Message> request, uint seqno)
                {
                    this.owner = owner;
                    this.request = request;
                    this.seqno = seqno;
                }

                private Sender owner;
                private Base4_.Asynchronous<QS._core_c_.Base3.Message> request;
                private uint seqno;

                #region Completion Callback

                public void CompletionCallback(bool succeeded, Exception exception, object context)
                {
                    // TODO: Process sending completion..................
                }

                #endregion

                // TODO: Write all the serialization stuff........................................serialize this crap into Multicasting5.MessageRV

                #region ISerializable Members

                QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
                {
                    get { throw new Exception("The method or operation is not implemented."); }
                }

                void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
                {
                    throw new Exception("The method or operation is not implemented.");
                }

                void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
                {
                    throw new Exception("The method or operation is not implemented.");
                }

                #endregion
            }

            #endregion

            #region GetObjectsCallback

            private bool GetObjectsCallback(ref Queue<Base4_.Asynchronous<QS._core_c_.Base3.Message>> returnedObjects, uint maximumSize)
            {
                lock (this)
                {
                    bool ready = false;
                    while (!ready && pendingQueue.Count > 0)
                    {
                        Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>> source = pendingQueue.Dequeue();
                        if (source.GetObjects(ref internalWorkingQueue, maximumSize - Request.HeaderOverhead))
                        {
                            ready = true;
                            pendingQueue.Enqueue(source);
                        }
                    }

                    if (ready)
                    {
                        foreach (Base4_.Asynchronous<QS._core_c_.Base3.Message> message in internalWorkingQueue)
                        {
                            uint seqno = ++lastSeqNo;
                            Request request = new Request(this, message, seqno);
                            requestContainer.Add(seqno, request);

                            returnedObjects.Enqueue(new Base4_.Asynchronous<QS._core_c_.Base3.Message>(
                                new QS._core_c_.Base3.Message((uint) ReservedObjectID.Gossiping5_GossipingRRVS_MessageChannel, request),
                                new QS._qss_c_.Base4_.CompletionCallback(request.CompletionCallback), null));
                        }
                        internalWorkingQueue.Clear();
                    }

                    return ready;
                }
            }

            #endregion

            #region SourceCallback

            private void SourceCallback(Base4_.ISource<Base4_.Asynchronous<QS._core_c_.Base3.Message>> source)
            {
                bool signaled_now;
                lock (this)
                {
                    signaled_now = !waiting;
                    pendingQueue.Enqueue(source);
                    waiting = true;
                }

                if (signaled_now)
                    outgoingChannel.Signal();
            }

            #endregion

            #region ISink<Asynchronous<Message>> Members

            QS._qss_c_.Base4_.IChannel QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Register(
                QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> getObjectCallback)
            {
                return new Base4_.Channel<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                    new QS._qss_c_.Base4_.SourceCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                        this.SourceCallback), getObjectCallback);
            }

            uint QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.MTU
            {
                get { return underlyingSink.MTU - Request.HeaderOverhead; }
            }

            #endregion

            #region IAddressedSink<RVID,Asynchronous<Message>> Members

            QS._qss_c_.Base3_.RVID QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Address
            {
                get { return destinationRVID; }
            }

            #endregion
        }

        #endregion

        #region GetSender

        private Sender GetSender(Base3_.RVID destinationRVID)
        {
            lock (this)
            {
                if (senders.ContainsKey(destinationRVID))
                    return senders[destinationRVID];
                else
                {
                    Sender sender = new Sender(this, destinationRVID);
                    senders[destinationRVID] = sender;
                    return sender;
                }
            }
        }

        #endregion

        #region GetReceiver

        private Receiver GetReceiver(Base3_.RVID rvid)
        {
            lock (this)
            {
                if (receivers.ContainsKey(rvid))
                    return receivers[rvid];
                else
                {
                    Receiver receiver = new Receiver(this, rvid);
                    receivers[rvid] = receiver;
                    return receiver;
                }
            }
        }

        #endregion

        #region ISinkCollection<RVID,Asynchronous<Message>> Members

        QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> QS._qss_c_.Base4_.ISinkCollection<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.this[QS._qss_c_.Base3_.RVID destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion
    }
}
