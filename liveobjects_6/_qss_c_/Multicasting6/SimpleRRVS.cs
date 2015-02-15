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

namespace QS._qss_c_.Multicasting6
{
    public class SimpleRRVS
    {
        public SimpleRRVS(QS.Fx.Logging.ILogger logger, uint loid, Base3_.IDemultiplexer demultiplexer,
            Membership2.Controllers.IMembershipController membershipController, Receivers_1_.IReceiverClass receiverClass)
        {
            this.logger = logger;
            this.loid = loid;
            this.demultiplexer = demultiplexer;
            this.receiverClass = receiverClass;
            this.membershipController = membershipController;

            demultiplexer.register(loid, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
            ((Membership2.Consumers.IRegionChangeProvider) membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.RegionChangedCallback(this.MembershipCallback);
        }

        private QS.Fx.Logging.ILogger logger;
        private uint loid;
        private Base3_.IDemultiplexer demultiplexer;
        private Membership2.Controllers.IMembershipController membershipController;
        private Receivers_1_.IReceiverClass receiverClass;
        private IDictionary<Base3_.RVID, ReceiverCollection> receiverCollections = 
            new Dictionary<Base3_.RVID, ReceiverCollection>();

        #region Membership Callback

        private void MembershipCallback(QS._qss_c_.Membership2.Consumers.RegionChange change)
        {
            lock (this)
            {
                Base3_.RVID rvid = new QS._qss_c_.Base3_.RVID(change.CurrentView.Region.ID, change.CurrentView.SeqNo);
                switch (change.LocalChange)
                {
                    case QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.ENTERED_REGION:
                    case QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.SWITCHED_REGION:
                    {
                        if (!receiverCollections.ContainsKey(rvid))
                            receiverCollections.Add(rvid, new ReceiverCollection(this, rvid, 
                                (Membership2.Controllers.IRegionViewController) 
                                    membershipController.lookupRegion(rvid.RegionID)[rvid.SeqNo]));                            
                    }
                    break;

                    // TODO: Process recycling of receiver collections for old region views and receivers for crashed nodes.
                }
            }
        }

        #endregion

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Multicasting5.MessageRV message = receivedObject as Multicasting5.MessageRV;
            if (message != null)
            {
                ReceiverCollection receiverCollection;
                lock (this)
                {
                    receiverCollection = receiverCollections.ContainsKey(message.RVID) ? receiverCollections[message.RVID] : null;
                }

                if (receiverCollection != null)
                    receiverCollection.ReceiveCallback(sourceAddress, message);
                else
                    logger.Log(this, "Could not deliver message from " + sourceAddress.ToString() +
                        " addressed at a locally nonexistent region view " + message.RVID.ToString());
            }
            else
                logger.Log(this, "Unrecognizable message of type " + 
                    ((receivedObject != null) ? receivedObject.GetType().FullName : "null") + " received from " + 
                    sourceAddress.ToString());

            return null;
        }

        #endregion

        #region Class ReceiverCollection

        private class ReceiverCollection
        {
            public ReceiverCollection(
                SimpleRRVS owner, Base3_.RVID rvid, Membership2.Controllers.IRegionViewController regionVC)
            {
                this.owner = owner;
                this.rvid = rvid;
                this.regionVC = regionVC;
                this.receiverClass = owner.receiverClass;
            }

            private SimpleRRVS owner;
            private Base3_.RVID rvid;
            private Membership2.Controllers.IRegionViewController regionVC;
            private Receivers_1_.IReceiverClass receiverClass;

            private IDictionary<QS._core_c_.Base3.InstanceID, Receivers_1_.IReceiver> receivers = 
                new Dictionary<QS._core_c_.Base3.InstanceID, Receivers_1_.IReceiver>();

            #region ReceiveCallback

            public void ReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, Multicasting5.MessageRV message)
            {
                Receivers_1_.IReceiver receiver;
                lock (this)
                {
                    if (receivers.ContainsKey(sourceAddress))
                        receiver = receivers[sourceAddress];
                    else
                    {
                        Base3_.IAddressCollection addressCollection = null; // TODO: Create a proper address collection for this receiver.
                        receiver = receiverClass.Create(sourceAddress, addressCollection);
                        receivers.Add(sourceAddress, receiver);
                    }
                }

                receiver.Receive(message.SeqNo, message.EncapsulatedMessage);
            }

            #endregion
        }

        #endregion
    }
}
