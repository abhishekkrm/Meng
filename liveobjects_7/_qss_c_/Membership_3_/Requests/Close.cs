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

namespace QS._qss_c_.Membership_3_.Requests
{
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Requests_Close)]
    public sealed class Close : Request
    {
        public Close()
        {
        }

        public Close(int sequenceNo, Base3_.GroupID groupID) : base(sequenceNo)
        {
            this.groupID = groupID;
        }

        private Base3_.GroupID groupID;

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public override RequestType RequestType
        {
            get { return RequestType.Close; }
        }

        #region ISerializable Members

        public override QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            { 
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Requests_Close, 0, 0, 0);
                info.AddAnother(base.SerializableInfo);
                info.AddAnother(groupID.SerializableInfo);
                return info;
            }
        }

        public override void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            base.SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) groupID).SerializeTo(ref header, ref data);
        }

        public override void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            base.DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)groupID).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
