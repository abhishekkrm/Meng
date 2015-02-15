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

namespace QS._qss_c_.Multicasting5
{
    /// <summary>
    /// This is an unreliable region sender that just multicasts messages via IP multicast, relying on whatever node sender is given to it.
    /// Node sender should invoke callback when it puts the request on the wire. Node sender is responsibe for flow control and message batching.
    /// </summary>
    public class MulticastingURS : Base3_.SenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender>,
        Base3_.ISenderCollection<Base3_.RegionID, QS._qss_c_.Base3_.ISerializableSender>
    {
        public MulticastingURS(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController,
            Base3_.ISenderCollection<QS.Fx.Network.NetworkAddress, Base3_.IReliableSerializableSender> underlyingSenderCollection)
        {
            this.logger = logger;
            this.membershipController = membershipController;
            this.underlyingSenderCollection = underlyingSenderCollection;
        }

        private QS.Fx.Logging.ILogger logger;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base3_.ISenderCollection<QS.Fx.Network.NetworkAddress, Base3_.IReliableSerializableSender> underlyingSenderCollection;

        #region Class Sender

        [QS.Fx.Base.Inspectable]
        private class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender
        {
            public Sender(MulticastingURS owner, Base3_.RegionID regionID)
            {
                this.owner = owner;
                this.regionID = regionID;
                regionController = owner.membershipController.lookupRegion(regionID);
                if (regionController == null)
                    throw new ArgumentException("This region is not recognized locally, we need to be a member of the groups in region signature!");

                multicastAddress = regionController.Address;
                underlyingSender = owner.underlyingSenderCollection[multicastAddress];
            }

            private MulticastingURS owner;
            private Base3_.RegionID regionID;
            private Membership2.Controllers.IRegionController regionController;
            private QS.Fx.Network.NetworkAddress multicastAddress;
            private Base3_.IReliableSerializableSender underlyingSender;

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
#if DEBUG_MulticastingURS
                owner.logger.Log(this, "__BeginSend: " + regionID.ToString() + ", " + destinationLOID.ToString() + ", " + data.ToString());
#endif

                return underlyingSender.BeginSend(destinationLOID, data, completionCallback, asynchronousState);
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
                underlyingSender.EndSend(asynchronousOperation);
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return multicastAddress; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        protected override QS._qss_c_.Base3_.IReliableSerializableSender CreateSender(QS._qss_c_.Base3_.RegionID address)
        {
            return new Sender(this, address);
        }

        #region ISenderCollection<ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<Base3_.RegionID, QS._qss_c_.Base3_.ISerializableSender>.this[Base3_.RegionID destinationAddress]
        {
            get { return ((Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender>)this)[destinationAddress]; }
        }

        #endregion
    }
}
