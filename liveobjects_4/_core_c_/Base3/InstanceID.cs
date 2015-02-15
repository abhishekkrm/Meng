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

namespace QS._core_c_.Base3
{
    [System.Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.InstanceID)]
    public sealed class InstanceID : QS.Fx.Serialization.ISerializable, IComparable, IComparable<InstanceID>, QS.Fx.Serialization.IStringSerializable, IEquatable<InstanceID>
    {
		public InstanceID(QS.Fx.Network.NetworkAddress address, System.DateTime time) : this(address, new Incarnation(time))
		{
		}

		public InstanceID()
        {
        }

		public InstanceID(QS.Fx.Network.NetworkAddress address, Incarnation incarnation) : this(address, incarnation.SeqNo)
		{
		}

		public InstanceID(QS.Fx.Network.NetworkAddress address, uint incarnationSeqNo)
        {
            this.address = address;
            this.incarnationSeqNo = incarnationSeqNo;
        }

        public InstanceID(string address)
        {
            ((QS.Fx.Serialization.IStringSerializable)this).AsString = address;
        }

        private QS.Fx.Network.NetworkAddress address;
        private uint incarnationSeqNo;

        public static implicit operator InstanceID(string address)
        {
            return new InstanceID(address);
        }

        public static implicit operator string(InstanceID instanceID)
        {
            return instanceID.ToString();
        }

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return address; }
            set { address = value; }
        }

//        public uint IncarnationSeqNo
//        {
//            get { return incarnationSeqNo; }
//        }

		public Incarnation Incarnation
		{
			get { return new Incarnation(incarnationSeqNo); }
            set { incarnationSeqNo = value.SeqNo; }
		}

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return address.SerializableInfo.Extend((ushort) QS.ClassID.InstanceID, sizeof(uint), 0, 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            address.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                *((uint*)(arrayptr + header.Offset)) = incarnationSeqNo;
            }
            header.consume(sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            address = new QS.Fx.Network.NetworkAddress();
            address.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                incarnationSeqNo = *((uint*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(uint));
        }

        #endregion

        public override string ToString()
        {
            return address.ToString() + "," + (new Incarnation(incarnationSeqNo)).ToString();
        }

        public override bool Equals(object obj)
        {
            InstanceID anotherGuy = obj as InstanceID;
            return (anotherGuy != null) ? (address.Equals(anotherGuy.address) && incarnationSeqNo.Equals(anotherGuy.incarnationSeqNo)) : false;
        }

        public override int GetHashCode()
        {
            return address.GetHashCode() ^ incarnationSeqNo.GetHashCode();
        }

        #region IComparable<InstanceID> Members

        int IComparable<InstanceID>.CompareTo(InstanceID other)
        {
            int result = address.CompareTo(other.address);
            return (result != 0) ? result : incarnationSeqNo.CompareTo(other.incarnationSeqNo);
        }

#if Using_VS2005Beta1
        bool IComparable<InstanceID>.Equals(InstanceID other)
        {
            return ((IComparable<InstanceID>)this).CompareTo(other) == 0;
        }
#endif

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            InstanceID anotherGuy = obj as InstanceID;
            if (anotherGuy != null)
                return ((IComparable<InstanceID>)this).CompareTo(anotherGuy);
            else
                throw new ArgumentException();
        }

        #endregion

		#region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort)QS.ClassID.InstanceID; }
		}

        [System.Xml.Serialization.XmlIgnore]
        string QS.Fx.Serialization.IStringSerializable.AsString
		{
			get
			{
				StringBuilder s = new StringBuilder(address.ToString());
				s.Append(":");
				s.Append(incarnationSeqNo.ToString());
				return s.ToString();
			}

			set
			{
				int separator = value.LastIndexOf(":");
				address = new QS.Fx.Network.NetworkAddress(value.Substring(0, separator));
				incarnationSeqNo = System.Convert.ToUInt32(value.Substring(separator + 1));
			}
		}

		#endregion

        #region IEquatable<InstanceID> Members

        bool IEquatable<InstanceID>.Equals(InstanceID other)
        {
            return address.Equals(other.address) && incarnationSeqNo.Equals(other.incarnationSeqNo);
        }

        #endregion
    }
}
