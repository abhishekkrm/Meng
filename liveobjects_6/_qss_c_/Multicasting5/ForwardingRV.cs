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
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting5_ForwardingRV)]
    public class ForwardingRV : MessageRV
    {
        public ForwardingRV()
        {
        }

        public ForwardingRV(
            Base3_.RVID regionViewID, QS._core_c_.Base3.InstanceID senderAddress, uint messageSeqNo, QS._core_c_.Base3.Message message)
            : base(regionViewID, messageSeqNo, message)
        {
            this.senderAddress = senderAddress;
        }

        private QS._core_c_.Base3.InstanceID senderAddress;

        #region Accessors

        public QS._core_c_.Base3.InstanceID SenderAddress
        {
            get { return senderAddress; }
        }

        #endregion

        #region ISerializable Members

        public override QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                return base.SerializableInfo.CombineWith(senderAddress.SerializableInfo).Extend(
                    (ushort)ClassID.Multicasting5_ForwardingRV, 0, 0, 0);
            }
        }

        public unsafe override void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            senderAddress.SerializeTo(ref header, ref data);
            base.SerializeTo(ref header, ref data);
        }

        public unsafe override void DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            senderAddress = new QS._core_c_.Base3.InstanceID();
            senderAddress.DeserializeFrom(ref header, ref data);
            base.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            return base.ToString() + "-from-" + senderAddress.ToString();
        }
    }
}
