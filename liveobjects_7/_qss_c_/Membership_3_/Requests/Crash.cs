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
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Requests_Crash)]
    public sealed class Crash : Request
    {
        public Crash()
        {
        }

        public Crash(int sequenceNo, params QS._core_c_.Base3.InstanceID[] deadAddresses) : base(sequenceNo)
        {
            this.deadAddresses = deadAddresses;
        }

        private QS._core_c_.Base3.InstanceID[] deadAddresses;

        public QS._core_c_.Base3.InstanceID[] DeadAddresses
        {
            get { return deadAddresses; }
            set { deadAddresses = value; }
        }

        public override RequestType RequestType
        {
            get { return RequestType.Crash; }
        }

        #region ISerializable Members

        public override QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Requests_Crash, sizeof(ushort), sizeof(ushort), 0);
                info.AddAnother(base.SerializableInfo);
                foreach (QS._core_c_.Base3.InstanceID address in deadAddresses)
                    info.AddAnother(address.SerializableInfo);
                return info;
            }
        }

        public override unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            base.SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)pheader) = (ushort) deadAddresses.Length;
            }
            header.consume(sizeof(ushort));
            foreach (QS._core_c_.Base3.InstanceID address in deadAddresses)
                address.SerializeTo(ref header, ref data);
        }

        public override unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            base.DeserializeFrom(ref header, ref data);
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                count = (int)(*((ushort*)pheader));
            }
            header.consume(sizeof(ushort));
            deadAddresses = new QS._core_c_.Base3.InstanceID[count];
            for (int ind = 0; ind < count; ind++)
            {
                deadAddresses[ind] = new QS._core_c_.Base3.InstanceID();
                deadAddresses[ind].DeserializeFrom(ref header, ref data);
            }
        }

        #endregion
    }
}
