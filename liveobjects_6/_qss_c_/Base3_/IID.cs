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
    public struct IID : IEquatable<IID>, IComparable<IID>, IComparable, QS.Fx.Serialization.ISerializable
    {
        public IID(Address address, QS._core_c_.Base3.Incarnation incarnation)
        {
            this.address = address;
            this.incarnation = incarnation;
        }

        public IID(QS.Fx.Network.NetworkAddress address, QS._core_c_.Base3.Incarnation incarnation)
        {
            this.address = (Address) address;
            this.incarnation = incarnation;
        }

        public IID(QS._core_c_.Base3.InstanceID instanceID) : this(instanceID.Address, instanceID.Incarnation)
        {
        }

        public IID(string address) : this(new QS._core_c_.Base3.InstanceID(address))
        {
        }

        private Address address;
        private QS._core_c_.Base3.Incarnation incarnation;

        #region Accessors

        public Address Address
        {
            get { return address; }
            set { address = value; }
        }

        public QS._core_c_.Base3.Incarnation Incarnation
        {
            get { return incarnation; }
            set { incarnation = value; }
        }

        #endregion

        #region Overrides from System.Object

        public override bool Equals(object obj)
        {
            return (obj is IID) && Equals((IID)obj);
        }

        public override int GetHashCode()
        {
            return address.GetHashCode() ^ incarnation.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(address.ToString());
            s.Append(":");
            s.Append(incarnation.ToString());
            return s.ToString();
        }

        #endregion

        #region IEquatable<IID> Members

        public bool Equals(IID other)
        {
            return address.Equals(other.address) && incarnation.Equals(other.incarnation);
        }

        #endregion

        #region IComparable<IID> Members

        public int CompareTo(IID other)
        {
            int result = address.CompareTo(other.address);
            return (result != 0) ? result : incarnation.CompareTo(other.incarnation);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj is IID)
                return CompareTo((IID)obj);
            else
                throw new Exception("Cannot compare to object of different type.");
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return address.SerializableInfo.CombineWith(incarnation.SerializableInfo); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            address.SerializeTo(ref header, ref data);
            incarnation.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            address.DeserializeFrom(ref header, ref data);
            incarnation.DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
