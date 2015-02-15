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

namespace QS._qss_x_.Base1_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_Address)]
    public sealed class Address : QS.Fx.Serialization.ISerializable, IComparable<Address>, IComparable, IEquatable<Address>
    {
        #region Constructors

        public Address()
        {
        }

        public Address(Uri address)
        {
            this.address = address;
        }

        public Address(string address) : this(new Uri(address))
        {
        }

        public Address(string protocol_name, string host_address, int port_number)
            : this(new Uri(protocol_name + "://" + host_address + ":" + port_number.ToString()))
        {
        }

        public Address(string protocol_name, System.Net.IPAddress host_address, int port_number)
            : this(protocol_name, host_address.ToString(), port_number)
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private Uri address;

        #endregion

        #region Constants

        private const string QuickSilverScheme = "quicksilver";

        #endregion

        #region Accessors

        public Uri Uri
        {
            get { return address; }
        }

        #endregion

        #region Helpers

        public static Address QuickSilver(string host_address, int port_number)
        {
            return new Address(QuickSilverScheme, host_address, port_number);
        }

        public static Address QuickSilver(System.Net.IPAddress host_address, int port_number)
        {
            return new Address(QuickSilverScheme, host_address, port_number);
        }

        public static Address QuickSilver(QS.Fx.Network.NetworkAddress address)
        {
            return new Address(QuickSilverScheme, address.HostIPAddress, address.PortNumber);
        }

        public bool IsNull
        {
            get { return address == null; }
        }

        public bool IsQuickSilver
        {
            get { return (address != null) && address.Scheme.Equals(QuickSilverScheme); }
        }

        public bool ToQuickSilver(out string host, out int port)
        {
            if (IsQuickSilver)
            {
                host = address.Host;
                port = address.Port;
                return true;
            }
            else
            {
                host = null;
                port = 0;
                return false;
            }
        }

        private static readonly Address nullAddress = new Address();

        public static Address Null
        {
            get { return nullAddress; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Fx_Base_Address,
                    sizeof(ushort), 
                    sizeof(ushort) + ((address != null) ? Encoding.ASCII.GetBytes(address.OriginalString).Length : 0), 
                    (address != null) ? 1 : 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            byte[] address_bytes = (address != null) ? Encoding.ASCII.GetBytes(address.OriginalString) : null;
            fixed (byte* parray = header.Array)
            {
                *((ushort*)(parray + header.Offset)) = (ushort) ((address_bytes != null) ? address_bytes.Length : 0);
            }
            header.consume(sizeof(ushort));
            if (address_bytes != null)
                data.Add(new QS.Fx.Base.Block(address_bytes));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int address_length;
            fixed (byte* parray = header.Array)
            {
                address_length = (int)(*((ushort*)(parray + header.Offset)));
            }
            header.consume(sizeof(ushort));
            if (address_length > 0)
            {
                address = new Uri(Encoding.ASCII.GetString(data.Array, data.Offset, address_length));
                data.consume(address_length);
            }
            else
                address = null;
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return (address != null) ? address.ToString() : "";
        }

        public override int GetHashCode()
        {
            return (address != null) ? address.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<Address>)this).Equals(obj as Address);
        }

        #endregion

        #region IComparable<Address> Members

        int IComparable<Address>.CompareTo(Address other)
        {
            if (other != null)
            {
                return (address == null) ? ((other.address == null) ? 0 : -1) : 
                    ((other.address == null) ? 1 : address.OriginalString.CompareTo(other.address.OriginalString));
            }
            else
                throw new Exception();
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<Address>)this).CompareTo(obj as Address);
        }

        #endregion

        #region IEquatable<Address> Members

        bool IEquatable<Address>.Equals(Address other)
        {
            return (other != null) && ((address != null) ? ((other.address != null) && address.Equals(other.address)) : (other.address == null));
        }

        #endregion
    }
}
