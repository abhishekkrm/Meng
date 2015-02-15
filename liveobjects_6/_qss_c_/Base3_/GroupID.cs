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

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QS._qss_c_.Base3_
{
	[Serializable]
    // [DataContract]
    public struct GroupID : QS.Fx.Serialization.ISerializable, System.IComparable, System.IComparable<GroupID>, QS.Fx.Serialization.IStringSerializable
    {
		public static GroupID FromString(string s)
		{
			return new GroupID(s);
		}

		public GroupID(System.UInt32 groupID)
        {
            this.groupID = groupID;
        }

        public GroupID(string s)
        {
			this.groupID = System.Convert.ToUInt32(s);
        }

        public static implicit operator GroupID(uint group_id)
        {
            return new GroupID(group_id);
        }

        // [DataMember]
        private System.UInt32 groupID;

        #region System.IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is GroupID)
                return groupID.CompareTo(((GroupID)obj).groupID);
            else
                throw new ArgumentException();
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            return (obj is GroupID) && groupID.Equals(((GroupID) obj).groupID);
        }

        public override int GetHashCode()
        {
            return groupID.GetHashCode();
        }

        public override string ToString()
        {
			return groupID.ToString();
			// return "GID[" + groupID.ToString("x4") + "]";
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Base3_GroupID, sizeof(uint), sizeof(uint), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((uint*)(arrayptr + header.Offset)) = groupID;
            }
            header.consume(sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                groupID = *((uint*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(uint));
        }

        #endregion

        #region IComparable<GroupID> Members

        int IComparable<GroupID>.CompareTo(GroupID other)
        {
            return groupID.CompareTo(other.groupID);
        }

#if Using_VS2005Beta1
        bool IComparable<GroupID>.Equals(GroupID other)
        {
            return groupID.Equals(other.groupID);
        }
#endif

        #endregion

        public System.UInt32 ToUInt32
        {
            get { return groupID; }
        }

		#region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort)QS.ClassID.Base3_GroupID; }
		}

		public string AsString
		{
			get { return groupID.ToString(); }
			set  { groupID = System.Convert.ToUInt32(value); }
		}

		#endregion
	}
}
