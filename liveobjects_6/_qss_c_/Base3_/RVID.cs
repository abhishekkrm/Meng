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
    public struct RVID : System.IComparable<RVID>, QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.IStringSerializable
    {
        public static RVID Undefined
        {
            get { return undefined; }
        }

        private static readonly RVID undefined = new RVID(new RegionID(0UL), 0U);

        public RVID(RegionID regionID, uint seqNo) : this(new IDWithSequenceNo<RegionID>(regionID, seqNo))
        {
        }

        public RVID(Base3_.IDWithSequenceNo<Base3_.RegionID> value)
        {
            data = value;
        }

        private IDWithSequenceNo<RegionID> data;

        #region Accessors and Overriden from System.Object

        public RegionID RegionID
        {
            get { return data.ID; }
        }

        public uint SeqNo
        {
            get { return data.SeqNo; }
        }

        public override bool Equals(object obj)
        {
            if (obj is RVID) 
            {
                RVID other = (RVID) obj;
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

        public static implicit operator Base3_.IDWithSequenceNo<Base3_.RegionID>(RVID rvid)
        {
            return rvid.data;
        }

        #region IComparable<RVID> Members

        int IComparable<RVID>.CompareTo(RVID other)
        {
            int result = data.ID.CompareTo(other.data.ID);
            return (result != 0) ? result : data.SeqNo.CompareTo(other.data.SeqNo);
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return this.data.SerializableInfo; }
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

        #region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
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

        public static RVID FromString(string s)
        {
            RVID rvid = new RVID();
            rvid.AsString = s;
            return rvid;
        }
    }
}
