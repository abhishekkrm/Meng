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

namespace QS._qss_c_.Senders6
{
    public class UnreliableAnycastingRegionSender : Base3_.SenderCollection<Base3_.RegionID, QS._qss_c_.Base3_.ISerializableSender>
    {
        public UnreliableAnycastingRegionSender(Membership2.Controllers.IMembershipController membershipController,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection)
        {
            this.membershipController = membershipController;
            this.underlyingSenderCollection = underlyingSenderCollection;
        }

        private Membership2.Controllers.IMembershipController membershipController;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;

        #region Class Sender

        [QS.Fx.Base.Inspectable]
        private class Sender : QS.Fx.Inspection.Inspectable, QS._qss_c_.Base3_.ISerializableSender
        {
            public Sender(UnreliableAnycastingRegionSender owner, Base3_.RegionID regionID)
            {
                this.owner = owner;
                this.regionID = regionID;
                this.regionController = owner.membershipController.lookupRegion(regionID);
            }

            private UnreliableAnycastingRegionSender owner;
            private Base3_.RegionID regionID;
            private Membership2.Controllers.IRegionController regionController;

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return regionController.Address; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                IEnumerator<QS._core_c_.Base3.InstanceID> en = regionController.CurrentView.Members.GetEnumerator();
                if (en.MoveNext())                
                    ((QS._qss_c_.Base3_.ISerializableSender)owner.underlyingSenderCollection[en.Current]).send(destinationLOID, data);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotSupportedException(); }
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

        protected override QS._qss_c_.Base3_.ISerializableSender CreateSender(QS._qss_c_.Base3_.RegionID address)
        {
            return new Sender(this, address);
        }
    }
}
