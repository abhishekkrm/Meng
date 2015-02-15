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
using System.Net;

namespace QS._qss_c_.Base3_
{
    public struct Address : IEquatable<Address>, IComparable<Address>, IComparable, QS.Fx.Serialization.ISerializable
    {
        public Address(IPAddress ipAddress, int port)
        {
            address = MakeAddress(ipAddress, port);
        }

        public Address(QS.Fx.Network.NetworkAddress networkAddress)
            : this(networkAddress.HostIPAddress, networkAddress.PortNumber)
        {
        }

        public Address(string address)
        {
            QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(address);
            this.address = MakeAddress(networkAddress.HostIPAddress, networkAddress.PortNumber);
        }

        private ulong address;

        #region Helpers

        private unsafe static ulong MakeAddress(IPAddress ipAddress, int port)
        {
            ulong address;

#pragma warning disable 618
            long n = ipAddress.Address;
#pragma warning restore 618

            ((byte*)&address)[4] = ((byte*)&n)[3];
            ((byte*)&address)[5] = ((byte*)&n)[2];
            ((byte*)&address)[6] = ((byte*)&n)[1];
            ((byte*)&address)[7] = ((byte*)&n)[0];
            ((int*)&address)[0] = port;
            return address;
        }

        #endregion

        #region Conversion Operators

        public static explicit operator Address(QS.Fx.Network.NetworkAddress networkAddress)
        {
            return new Address(networkAddress);
        }

        public static explicit operator QS.Fx.Network.NetworkAddress(Address address)
        {
            return new QS.Fx.Network.NetworkAddress(address.IPAddress, address.Port);
        }

/*
        public static explicit operator Address(ulong address)
        {
            Address result = new Address();
            result.address = address;
            return result;
        }

        public static explicit operator ulong(Address address)
        {
            return address.address;
        }
*/

        #endregion

        #region Accessors

        public unsafe IPAddress IPAddress
        {
            get 
            {
                long n;
                fixed (ulong* paddress = &address)
                {
                    ((byte*)&n)[0] = ((byte*)paddress)[7];
                    ((byte*)&n)[1] = ((byte*)paddress)[6];
                    ((byte*)&n)[2] = ((byte*)paddress)[5];
                    ((byte*)&n)[3] = ((byte*)paddress)[4];
                    ((int*)&n)[1] = 0;
                }
                return new IPAddress(n);
            }

#pragma warning disable 618
            set 
            {
                long n = value.Address;

                fixed (ulong* paddress = &address)
                {
                    ((byte*)paddress)[4] = ((byte*)&n)[3];
                    ((byte*)paddress)[5] = ((byte*)&n)[2];
                    ((byte*)paddress)[6] = ((byte*)&n)[1];
                    ((byte*)paddress)[7] = ((byte*)&n)[0];
                }
            }
#pragma warning restore 618
        }

        public unsafe int Port
        {
            get 
            { 
                int port;
                fixed (ulong* paddress = &address)
                {
                    port = ((int*)paddress)[0];
                }
                return port; 
            }

            set 
            {
                fixed (ulong* paddress = &address)
                {
                    ((int*)paddress)[0] = value;
                }
            }
        }

        #endregion

        #region Overrides from System.Object

        public override int GetHashCode()
        {
            return address.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is Address) ? address.Equals(((Address)obj).address) : false;
        }

        public unsafe override string ToString()
        {
            StringBuilder s = new StringBuilder();
            fixed (ulong* paddress = &address)
            {
                s.Append(((byte*)paddress)[7].ToString());
                s.Append(".");
                s.Append(((byte*)paddress)[6].ToString());
                s.Append(".");
                s.Append(((byte*)paddress)[5].ToString());
                s.Append(".");
                s.Append(((byte*)paddress)[4].ToString());
                s.Append(":");
                s.Append(((int*)paddress)[0].ToString());
            }
            return s.ToString();
        }

        #endregion

        #region IEquatable<NetworkAddress> Members

        public bool Equals(Address other)
        {
            return address.Equals(other.address);
        }

        #endregion

        #region IComparable<NetworkAddress> Members

        public int CompareTo(Address other)
        {            
            return address.CompareTo(other.address);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj is Address)
                return address.CompareTo(((Address)obj).address);
            else
                throw new Exception("Cannot compare with object of a different type.");
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)sizeof(ulong), sizeof(ulong), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ulong*)(pbuffer + header.Offset)) = address;
            }
            header.consume(sizeof(ulong));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                address = *((ulong*)(pbuffer + header.Offset));
            }
            header.consume(sizeof(ulong));
        }

        #endregion
    }
}
