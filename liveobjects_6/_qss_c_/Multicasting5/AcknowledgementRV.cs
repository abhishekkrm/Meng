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

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting5_AcknowledgementRV)]
    public class AcknowledgementRV : QS.Fx.Serialization.ISerializable
    {
        public AcknowledgementRV()
        {
        }

        public AcknowledgementRV(Base3_.RVID regionViewID, CompressedAckSet ackCollection)
        {
            this.regionViewID = regionViewID;
            this.ackCollection = ackCollection;
        }

        private Base3_.RVID regionViewID;
        private CompressedAckSet ackCollection;

        #region Accessors

        public Base3_.RVID RVID
        {
            get { return regionViewID; }
        }

        public CompressedAckSet AckCollection
        {
            get { return ackCollection; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return regionViewID.SerializableInfo.CombineWith(((QS.Fx.Serialization.ISerializable)ackCollection).SerializableInfo).Extend(
                    (ushort)ClassID.Multicasting5_AcknowledgementRV, 0, 0, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            regionViewID.SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)ackCollection).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            regionViewID = new QS._qss_c_.Base3_.RVID();
            regionViewID.DeserializeFrom(ref header, ref data);
            ackCollection = new CompressedAckSet();
            ((QS.Fx.Serialization.ISerializable)ackCollection).DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("(");
            s.Append(regionViewID.ToString());
            s.Append(" ");
            s.Append(ackCollection.ToString());
            s.Append(")");
            return s.ToString();
        }
    }
}
