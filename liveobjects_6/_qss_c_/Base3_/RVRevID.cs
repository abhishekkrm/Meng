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
    public struct RVRevID : System.IComparable<RVRevID>, QS.Fx.Serialization.ISerializable // , QS.Fx.Serialization.IStringSerializable
    {
        public RVRevID(RegionID regionID, uint regionViewSequenceNo, uint regionViewRevisionSequenceNo)
        {
            this.regionID = regionID;
            this.regionViewSequenceNo = regionViewSequenceNo;
            this.regionViewRevisionSequenceNo = regionViewRevisionSequenceNo;
        }

        private RegionID regionID;
        private uint regionViewSequenceNo, regionViewRevisionSequenceNo;

        #region Accessors and Overriden from System.Object

        public RegionID RegionID
        {
            get { return regionID; }
        }

        public uint RegionViewSequenceNo
        {
            get { return regionViewSequenceNo; }
        }

        public uint RegionViewRevisionSequenceNo
        {
            get { return regionViewRevisionSequenceNo; }
        }

        public override bool Equals(object obj)
        {
            if (obj is RVRevID) 
            {
                RVRevID other = (RVRevID)obj;
                return regionID.Equals(other.regionID) && regionViewSequenceNo.Equals(other.regionViewSequenceNo) &&
                    regionViewRevisionSequenceNo.Equals(other.regionViewRevisionSequenceNo);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return regionID.GetHashCode() ^ regionViewSequenceNo.GetHashCode() ^ regionViewRevisionSequenceNo.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + regionID.ToString() + ":" + regionViewSequenceNo.ToString() + "," + RegionViewRevisionSequenceNo.ToString() + ")";
        }

        #endregion

        #region IComparable<RVRevID> Members

        int IComparable<RVRevID>.CompareTo(RVRevID other)
        {
            int result = regionID.CompareTo(other.regionID);
            if (result == 0)
                result = regionViewSequenceNo.CompareTo(other.regionViewSequenceNo);
            if (result == 0)
                result = regionViewRevisionSequenceNo.CompareTo(other.regionViewRevisionSequenceNo);
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
                info.AddAnother(regionID.SerializableInfo);
                return info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)(pheader)) = regionViewSequenceNo;
                *((uint*)(pheader + sizeof(uint))) = regionViewRevisionSequenceNo;
            }
            header.consume(2 * sizeof(uint));
            regionID.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                regionViewSequenceNo = *((uint*)(pheader));
                regionViewRevisionSequenceNo = *((uint*)(pheader + sizeof(uint)));
            }
            header.consume(2 * sizeof(uint));
            regionID.DeserializeFrom(ref header, ref data);
        }

        #endregion

/*
        #region IStringSerializable Members

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
                RegionID regionID = new RegionID();
                regionID.AsString = value.Substring(0, separator_position);
                data = new IDWithSequenceNo<RegionID>(
                    regionID, System.Convert.ToUInt32(value.Substring(separator_position + 1)));
            }
        }

        #endregion

        public static RVRevID FromString(string s)
        {
            RVRevID rvrevid = new RVRevID();
            rvrevid.AsString = s;
            return rvrevid;
        }
*/
    }
}
