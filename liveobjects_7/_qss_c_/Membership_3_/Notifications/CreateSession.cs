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

namespace QS._qss_c_.Membership_3_.Notifications
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateSession)]
    public sealed class CreateSession : QS.Fx.Serialization.ISerializable
    {
        public CreateSession()
        {
        }

        public CreateSession(Base3_.RVID regionViewID, Base3_.GVID groupViewID, 
            IEnumerable<QS._core_c_.Base3.InstanceID> synchronizedMemberAddresses)
        {
            this.regionViewID = regionViewID;
            this.groupViewID = groupViewID;
            this.synchronizedMemberAddresses = new List<QS._core_c_.Base3.InstanceID>(synchronizedMemberAddresses);
        }

        [QS.Fx.Printing.Printable]
        private Base3_.RVID regionViewID;
        [QS.Fx.Printing.Printable]
        private Base3_.GVID groupViewID;
        [QS.Fx.Printing.Printable]
        private List<QS._core_c_.Base3.InstanceID> synchronizedMemberAddresses;

        #region Accessors

        public Base3_.RVID RegionViewID
        {
            get { return regionViewID; }
        }

        public Base3_.GVID GroupViewID
        {
            get { return groupViewID; }
        }

        public IList<QS._core_c_.Base3.InstanceID> SynchronizedMemberAddresses
        {
            get { return synchronizedMemberAddresses; }
        }

        #endregion

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
}
