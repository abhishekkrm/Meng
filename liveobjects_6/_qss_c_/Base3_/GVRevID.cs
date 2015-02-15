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
    public struct GVRevID : System.IComparable<GVRevID>, QS.Fx.Serialization.ISerializable // , QS.Fx.Serialization.IStringSerializable
    {
        public GVRevID(GroupID groupID, uint groupViewSequenceNo, uint groupViewRevisionSequenceNo)
        {
            this.groupID = groupID;
            this.groupViewSequenceNo = groupViewSequenceNo;
            this.groupViewRevisionSequenceNo = groupViewRevisionSequenceNo;
        }

        private GroupID groupID;
        private uint groupViewSequenceNo, groupViewRevisionSequenceNo;

        #region Accessors and Overriden from System.Object

        public GroupID GroupID
        {
            get { return groupID; }
        }

        public uint GroupViewSequenceNo
        {
            get { return groupViewSequenceNo; }
        }

        public uint GroupViewRevisionSequenceNo
        {
            get { return groupViewRevisionSequenceNo; }
        }

        public override bool Equals(object obj)
        {
            if (obj is GVRevID)
            {
                GVRevID other = (GVRevID)obj;
                return groupID.Equals(other.groupID) && groupViewSequenceNo.Equals(other.groupViewSequenceNo) &&
                    groupViewRevisionSequenceNo.Equals(other.groupViewRevisionSequenceNo);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return groupID.GetHashCode() ^ groupViewSequenceNo.GetHashCode() ^ groupViewRevisionSequenceNo.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + groupID.ToString() + ":" + groupViewSequenceNo.ToString() + "," + groupViewRevisionSequenceNo.ToString() + ")";
        }

        #endregion

        #region IComparable<GVRevID> Members

        int IComparable<GVRevID>.CompareTo(GVRevID other)
        {
            int result = groupID.CompareTo(other.groupID);
            if (result == 0)
                result = groupViewSequenceNo.CompareTo(other.groupViewSequenceNo);
            if (result == 0)
                result = groupViewRevisionSequenceNo.CompareTo(other.groupViewRevisionSequenceNo);
            return result;
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Nothing, 2 * sizeof(uint), 2 * sizeof(uint), 0);
                info.AddAnother(groupID.SerializableInfo);
                return info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)(pheader)) = groupViewSequenceNo;
                *((uint*)(pheader + sizeof(uint))) = groupViewRevisionSequenceNo;
            }
            header.consume(2 * sizeof(uint));
            groupID.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                groupViewSequenceNo = *((uint*)(pheader));
                groupViewRevisionSequenceNo = *((uint*)(pheader + sizeof(uint)));
            }
            header.consume(2 * sizeof(uint));
            groupID.DeserializeFrom(ref header, ref data);
        }

        #endregion

        /*
        #group IStringSerializable Members

        ushort IStringSerializable.ClassID
        {
            get { return (ushort) QS.ClassID.Nothing; }
        }

        public string AsString
        {
            get { return ((QS.Fx.Serialization.IStringSerializable) data.ID).AsString + ":" + data.SeqNo.ToString(); }
            set
            {                
                int separator_position = value.LastIndexOf(":");
                groupID groupID = new groupID();
                groupID.AsString = value.Substring(0, separator_position);
                data = new IDWithSequenceNo<groupID>(
                    groupID, System.Convert.ToUInt32(value.Substring(separator_position + 1)));
            }
        }

        #endgroup

        public static RVRevID FromString(string s)
        {
            RVRevID rvrevid = new RVRevID();
            rvrevid.AsString = s;
            return rvrevid;
        }
*/
    }
}
