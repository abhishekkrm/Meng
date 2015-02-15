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

namespace QS._qss_c_.Base3_
{
    public struct GVID : System.IComparable<GVID>, QS.Fx.Serialization.ISerializable, IEquatable<GVID>
    {
        public GVID(GroupID groupID, uint seqNo) : this(new IDWithSequenceNo<Base3_.GroupID>(groupID, seqNo))
        {
        }

        public GVID(Base3_.IDWithSequenceNo<Base3_.GroupID> value)
        {
            data = value;
        }

        private IDWithSequenceNo<GroupID> data;

        #region Accessors and Overriden from System.Object

        public GroupID GroupID
        {
            get { return data.ID; }
            set { data.ID = value; }
        }

        public uint SeqNo
        {
            get { return data.SeqNo; }
            set { data.SeqNo = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is GVID)
            {
                GVID other = (GVID)obj;
                return data.ID.Equals(other.data.ID) && data.SeqNo.Equals(other.data.SeqNo);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override string ToString()
        {
            return data.ToString();
        }

        #endregion

        public static implicit operator Base3_.IDWithSequenceNo<Base3_.GroupID>(GVID gvid)
        {
            return gvid.data;
        }

        #region IComparable<RVID> Members

        public int CompareTo(GVID other)
        {
            int result = data.ID.CompareTo(other.data.ID);
            return (result != 0) ? result : data.SeqNo.CompareTo(other.data.SeqNo);
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return data.SerializableInfo; }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            this.data.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this.data.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IEquatable<GVID> Members

        public bool Equals(GVID other)
        {
            return data.Equals(other.data);
        }

        #endregion
    }
}
