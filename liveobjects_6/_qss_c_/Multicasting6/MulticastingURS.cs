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

// #define DEBUG_MulticastingURS

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Multicasting6
{
    /// <summary>
    /// Unreliable sender fitting into the "pull" protocol stack.
    /// </summary>
    public class MulticastingURS : Base4_.ISinkCollection<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
    {
        public MulticastingURS(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController,
            Base4_.ISinkCollection<QS.Fx.Network.NetworkAddress, Base4_.Asynchronous<QS._core_c_.Base3.Message>> underlyingSinkCollection)
        {
            this.logger = logger;
            this.membershipController = membershipController;
            this.underlyingSinkCollection = underlyingSinkCollection;
        }

        private QS.Fx.Logging.ILogger logger;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base4_.ISinkCollection<QS.Fx.Network.NetworkAddress, Base4_.Asynchronous<QS._core_c_.Base3.Message>> underlyingSinkCollection;
        private IDictionary<Base3_.RegionID, Sender> senders = new Dictionary<Base3_.RegionID, Sender>();

        #region GetSender

        private Sender GetSender(Base3_.RegionID regionID)
        {
            lock (this)
            {
                if (senders.ContainsKey(regionID))
                    return senders[regionID];
                else
                {
                    Sender sender = new Sender(this, regionID);
                    senders[regionID] = sender;
                    return sender;
                }
            }            
        }

        #endregion

        #region Class Sender

        private class Sender : Base4_.IAddressedSink<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> 
        {
            public Sender(MulticastingURS owner, Base3_.RegionID regionID)
            {
                this.owner = owner;
                this.regionID = regionID;

                regionController = owner.membershipController.lookupRegion(regionID);
                if (regionController == null)
                    throw new ArgumentException("Region " + regionID.ToString() + " is not recognized locally.");
                multicastAddress = regionController.Address;
                underlyingSink = owner.underlyingSinkCollection[multicastAddress];
            }

            private MulticastingURS owner;
            private Base3_.RegionID regionID;
            private Membership2.Controllers.IRegionController regionController;
            private QS.Fx.Network.NetworkAddress multicastAddress;
            private Base4_.IAddressedSink<QS.Fx.Network.NetworkAddress, Base4_.Asynchronous<QS._core_c_.Base3.Message>> underlyingSink;

            #region ISink<Asynchronous> Members

            QS._qss_c_.Base4_.IChannel QS._qss_c_.Base4_.ISink<Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Register(
                QS._qss_c_.Base4_.GetObjectsCallback<Base4_.Asynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
            {
                 return underlyingSink.Register(
                    new QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                        getObjectsCallback));
            }

            uint QS._qss_c_.Base4_.ISink<Base4_.Asynchronous<QS._core_c_.Base3.Message>>.MTU
            {
                get { return underlyingSink.MTU; }
            }

            #endregion

            #region IAddressedSink<RVID,Asynchronous> Members

            QS._qss_c_.Base3_.RegionID 
                QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Address
            {
                get { return regionID; }
            }

            #endregion
        }

        #endregion

        #region ISinkCollection<RegionID,Asynchronous<Message>> Members

        QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RegionID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> 
            QS._qss_c_.Base4_.ISinkCollection<QS._qss_c_.Base3_.RegionID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.this[QS._qss_c_.Base3_.RegionID destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion
    }
}
