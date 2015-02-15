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
using Quilt;
using Quilt.HostDetector.NATCheck;
using Quilt.Transmitter;

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass("633A4944C4E440dc91F5417D09D61C45", "EUID Address")]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.EUID)]
    [XmlType(TypeName = "EUIDAddress")]
    public sealed class EUIDAddress : QS.Fx.Serialization.ISerializable, IComparable<EUIDAddress>, IComparable, IEquatable<EUIDAddress>, QS.Fx.Serialization.IStringSerializable, IEUIDable
    {

        #region Construct

        public EUIDAddress()
        {
            
        }

        public EUIDAddress(string euid)
        {
            this.address = euid;
            ParseAddress();
        }

        #endregion

        #region Function

        // Check whether this is a full EUID
        public bool IsFull()
        {
            if (address_map.ContainsKey("udp") && address_map.ContainsKey("tcp"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Parse the multiple addresses from the string and create a map to store them
        private void ParseAddress()
        {
            if (address == null || address.Length == 0) return;

            if (address_map == null) address_map = new Dictionary<string, ProtocolInfo>();

            if (router_stack == null) router_stack = new List<RouterInfo>();

            try
            {
                // Parse the address string
                string[] protocols = address.Split(';');

                foreach (string protocol in protocols)
                {
                    if (protocol.Contains("://"))
                    {
                        // Connectivity options
                        string[] separator = new string[] { "://", "|" };
                        string[] elems = protocol.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        if (elems.Length < 3)
                        {
                            throw new Exception("Address string is lack of components, should be proto_name://direct|proto_address");
                        }
                        ProtocolInfo info = new ProtocolInfo();
                        info.proto_direct = Direction.StringToDirection(elems[1]);
                        info.proto_addr = elems[2];
                        address_map[elems[0].ToLower()] = info;
                    }
                    else if (protocol.Contains("|"))
                    {
                        string[] elems = protocol.Split('|');
                        this.router_number = int.Parse(elems[0]);
                        this.ipmc_range = int.Parse(elems[1]);
                        if (router_number > 0)
                        {
                            string[] routers = elems[2].Split(',');
                            foreach (string router in routers)
                            {
                                RouterInfo info = new RouterInfo();
                                info.addr = router;
                                router_stack.Add(info);
                            }
                        }
                    }
                    else if (protocol.Contains("UDP") || protocol.Contains("TCP"))
                    {
                        this.preferred_proto = protocol;
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception("EUIDAddress.ParseAddress exception: " + e.Message);
            }
        }

        // Return Connectivity Options and string addresses based on the required protocol 
        // If it is not contained, null is returned
        // Currently it supports TCP and STUN, case insensitive
        public ProtocolInfo GetProtocolInfo(string protocol)
        {
            if (address_map == null)
                return null;
            else
            {
                ProtocolInfo info;
                if (address_map.TryGetValue(protocol.ToLower(), out info))
                    return info;
                else
                    return null;
            }
        }

        /// <summary>
        /// Return the string name of preferred protocol, UDP or TCP
        /// </summary>
        /// <returns></returns>
        public string GetPreferred()
        {
            if (this.preferred_proto == null)
                return null;
            else
                return this.preferred_proto;
        }

        #endregion

        #region Fields

        // EUID Address consists three parts
        // 1) Connectivity Options
        // "[Protocol Name]://[Connectivity Direction]|[Protocol Address];"
        // Each protocol (TCP/UDP) can have one address
        // UDP uses STUNAddress for NAT traverse support
        // Multiple addresses are separated by semicolon
        // 2) Router Stack
        // [Range]|[IPMC Range]|[Router 1],[Router 2],[Router 3]...;
        // 3) Performance
        // TODO
        // 4) Preferred protocol
        // String of protocol name

        [QS.Fx.Printing.Printable]
        private string address;

        // Stores the address string by its protocol name
        public class ProtocolInfo
        {
            public string proto_addr;
            public Direction.DIRECTION proto_direct;
        }
        private Dictionary<string, ProtocolInfo> address_map;

        private string preferred_proto;

        public class RouterInfo : IEquatable<RouterInfo>
        {
            public string addr;
            public int as_number;
            public string domain_name;

            #region IEquatable<RouterInfo> Members

            bool IEquatable<RouterInfo>.Equals(RouterInfo other)
            {
                if (addr == other.addr) return true;
                else return false;
            }

            #endregion
        }

        private List<RouterInfo> router_stack;
        private int router_number;
        private int ipmc_range;

        #endregion

        #region Accessors

        [XmlAttribute("value")]
        public string String
        {
            get { return this.address; }
            set { this.address = value; }
        }

        public int IPMCRange
        {
            get { return this.ipmc_range; }
        }

        public List<RouterInfo> RouterStack
        {
            get { return this.router_stack; }
            set { router_stack = value; }
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
            return (obj is EUIDAddress) && this.address.Equals(((EUIDAddress)obj).address);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if ((obj == null) || (!(obj is EUIDAddress)))
                throw new Exception();
            return this.address.CompareTo(((EUIDAddress)obj).address);
        }

        #endregion

        #region IComparable<EUIDAddress> Members

        int IComparable<EUIDAddress>.CompareTo(EUIDAddress other)
        {
            if (other == null)
                throw new Exception();
            else
                return (address == null) ? ((other.address == null) ? 0 : -1) :
                    ((other.address == null) ? 1 : address.CompareTo(other.address));
        }

        #endregion

        #region IEquatable<EUIDAddress> Members

        bool IEquatable<EUIDAddress>.Equals(EUIDAddress other)
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
                return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.EUID, sizeof(ushort), sizeof(ushort) + length, ((length > 0) ? 1 : 0));
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

                ParseAddress();
            }
            else
                this.address = null;
        }

        #endregion

        #region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
        {
            get { return (ushort)QS.ClassID.EUID; }
        }

        string QS.Fx.Serialization.IStringSerializable.AsString
        {
            get { return this.address; }
            set { this.address = value; }
        }

        #endregion

        #region IEUIDable Members

        EUIDAddress IEUIDable.GetEUID()
        {
            return this;
        }

        #endregion
    }
}

