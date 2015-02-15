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

// #define Using_VS2005Beta1

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    // [QS.Fx.Serialization.ClassID(ClassID.Base3_RegionID)]
    public struct RegionID 
		: QS.Fx.Serialization.ISerializable, System.IComparable, System.IComparable<RegionID>, QS.Fx.Serialization.IStringSerializable
	{
		#region Delegates

		public static RegionID FromString(string s)
		{
			return new RegionID(System.Convert.ToUInt64(s));
		}

		public static object String2Object(string s)
		{
			return FromString(s);
		}

		#endregion

		public RegionID(System.UInt64 regionID)
        {
            this.regionID = regionID;
        }

        private System.UInt64 regionID;

        #region System.IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is RegionID)
                return regionID.CompareTo(((RegionID)obj).regionID);
            else
                throw new ArgumentException();
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
			if (obj is RegionID)
				return regionID.Equals(((RegionID)obj).regionID);
			else
				throw new ArgumentException();
        }

        public override int GetHashCode()
        {
            return regionID.GetHashCode();
        }

        public override string ToString()
        {
            // return "RID[" + regionID.ToString("x8") + "]";
			return regionID.ToString();
		}

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Base3_RegionID, sizeof(System.UInt64), sizeof(System.UInt64), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ulong*)(arrayptr + header.Offset)) = regionID;
            }
            header.consume(sizeof(System.UInt64));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                regionID = *((ulong*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(System.UInt64));
        }

        #endregion

        #region IComparable<RegionID> Members

        int IComparable<RegionID>.CompareTo(RegionID other)
        {
            return regionID.CompareTo(other.regionID);
        }

#if Using_VS2005Beta1
        bool IComparable<RegionID>.Equals(RegionID other)
        {
            return regionID.Equals(other.regionID);
        }
#endif

        #endregion

		#region IStringSerializable Members

		public ushort ClassID
		{
			get { return (ushort) QS.ClassID.Base3_RegionID; }
		}

		public string AsString
		{
			get { return regionID.ToString(); }
			set { regionID = System.Convert.ToUInt64(value); }
		}

		#endregion
	}
}
