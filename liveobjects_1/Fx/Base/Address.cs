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
using System.Xml.Serialization;

namespace QS.Fx.Base
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_address)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Address)]
    [XmlType(TypeName = "Address")]
    public sealed class Address : QS.Fx.Serialization.ISerializable, IComparable<Address>, IComparable, IEquatable<Address>, QS.Fx.Serialization.IStringSerializable
    {
        #region Constructors

        public Address(string address)
        {
            this.address = address;
        }

        public Address(Uri address)
        {
            this.address = address.ToString();
        }

        public Address(string hostname, int port)
        {
            this.address = hostname + ":" + port.ToString();
        }

        public Address(System.Net.IPAddress ipaddress, int port)
        {
            this.address = ipaddress.ToString() + ":" + port.ToString();
        }

        public Address(string protocol, string hostname, int port)            
        {
            this.address = protocol + "://" + hostname + ":" + port.ToString();
        }

        public Address(string protocol, System.Net.IPAddress ipaddress, int port)
        {
            this.address = protocol + "://" + ipaddress.ToString() + ":" + port.ToString();
        }

        public Address()
        {
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
            return (obj is Address) && this.address.Equals(((Address) obj).address);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if ((obj == null) || (!(obj is Address)))
                throw new Exception();
            return this.address.CompareTo(((Address)obj).address);
        }

        #endregion

        #region IComparable<Address> Members

        int IComparable<Address>.CompareTo(Address other)
        {
            if (other == null)
                throw new Exception();
            else
                return (address == null) ? ((other.address == null) ? 0 : -1) :
                    ((other.address == null) ? 1 : address.CompareTo(other.address));
        }

        #endregion

        #region IEquatable<Address> Members

        bool IEquatable<Address>.Equals(Address other)
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
                if (length > (int) ushort.MaxValue)
                    throw new Exception();
                return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Address, sizeof(ushort), sizeof(ushort) + length, ((length > 0) ? 1 : 0));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            int length = (address != null) ? address.Length : 0;
            if (length > (int) ushort.MaxValue)
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
                data.Add(new  QS.Fx.Base.Block(bytes));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int length;
            fixed (byte* pheader0 = header.Array)
            {
                byte* pheader = pheader0 + header.Offset;
                length = (int)(*((ushort*) pheader));
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
            get { return (ushort)QS.ClassID.Address; }
        }

        string QS.Fx.Serialization.IStringSerializable.AsString
        {
            get { return this.address; }
            set { this.address = value; }
        }

        #endregion
    }
}
