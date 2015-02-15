/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass("D1FDEE02ABCA42c0922E69A5D9CC538D", "STUN Address")]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.STUNAddress)]
    [XmlType(TypeName = "STUNAddress")]
    public sealed class STUNAddress : QS.Fx.Serialization.ISerializable, IComparable<STUNAddress>, IComparable, IEquatable<STUNAddress>, QS.Fx.Serialization.IStringSerializable
    {

        #region Construct

        public STUNAddress()
        {

        }

        public STUNAddress(string pub_hostname, int pub_port, string pri_hostname, int pri_port)
        {
            this.address = pub_hostname + ":" + pub_port.ToString()
                + "/" + pri_hostname + ":" + pri_port.ToString();
        }

        public STUNAddress(System.Net.IPAddress pub_ipaddress, int pub_port, System.Net.IPAddress pri_ipaddress, int pri_port)
        {
            this.address = pub_ipaddress.ToString() + ":" + pub_port.ToString()
                + "/" + pri_ipaddress.ToString() + ":" + pri_port.ToString();
        }

        public STUNAddress(string stun_addr)
        {
            this.address = stun_addr;
        }

        #endregion

        #region Function

        public bool IsNAT()
        {
            return (PubAddr != PriAddr);
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private string address;

        #endregion

        #region Accessors

        [XmlAttribute("value")]
        public string String
        {
            get { return this.address; }
            set { this.address = value; }
        }

        public string PubAddr
        {
            get { return this.address.Split('/')[0]; }
        }

        public string PriAddr
        {
            get { return this.address.Split('/')[1]; }
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return (this.address != null) ? this.address : string.Empty;
        }

        public override int GetHashCode()
        {
            return (this.address != null) ? this.address.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            return (obj is STUNAddress) && this.address.Equals(((STUNAddress)obj).address);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if ((obj == null) || (!(obj is STUNAddress)))
                throw new Exception();
            return this.address.CompareTo(((STUNAddress)obj).address);
        }

        #endregion

        #region IComparable<STUNAddress> Members

        int IComparable<STUNAddress>.CompareTo(STUNAddress other)
        {
            if (other == null)
                throw new Exception();
            else
                return (address == null) ? ((other.address == null) ? 0 : -1) :
                    ((other.address == null) ? 1 : address.CompareTo(other.address));
        }

        #endregion

        #region IEquatable<STUNAddress> Members

        bool IEquatable<STUNAddress>.Equals(STUNAddress other)
        {
            return (other != null) && ((address != null) ? ((other.address != null) && address.Equals(other.address)) : (other.address == null));
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int length = (address != null) ? address.Length : 0;
                if (length > (int)ushort.MaxValue)
                    throw new Exception();
                return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.STUNAddress, sizeof(ushort), sizeof(ushort) + length, ((length > 0) ? 1 : 0));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            int length = (address != null) ? address.Length : 0;
            if (length > (int)ushort.MaxValue)
                throw new Exception();
            fixed (byte* pheader0 = header.Array)
            {
                byte* pheader = pheader0 + header.Offset;
                *((ushort*)pheader) = (ushort)length;
            }
            header.consume(sizeof(ushort));
            if (length > 0)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(address);
                if (bytes.Length != length)
                    throw new Exception();
                data.Add(new QS.Fx.Base.Block(bytes));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int length;
            fixed (byte* pheader0 = header.Array)
            {
                byte* pheader = pheader0 + header.Offset;
                length = (int)(*((ushort*)pheader));
            }
            header.consume(sizeof(ushort));
            if (length > 0)
            {
                this.address = Encoding.ASCII.GetString(data.Array, data.Offset, length);
                data.consume(length);
            }
            else
                this.address = null;
        }

        #endregion

        #region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
        {
            get { return (ushort)QS.ClassID.STUNAddress; }
        }

        string QS.Fx.Serialization.IStringSerializable.AsString
        {
            get { return this.address; }
            set { this.address = value; }
        }

        #endregion
    }
}
