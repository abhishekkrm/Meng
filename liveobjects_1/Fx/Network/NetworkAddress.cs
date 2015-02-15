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
#define DEBUG_CheckAddressesOnDeserialization

using System;
using System.Net;

namespace QS.Fx.Network
{
	/// <summary>
	/// This class represents a full IPv4 network address including an IP address and a port number.
	/// </summary>
	[Serializable]
	[QS.Fx.Serialization.ClassID(QS.ClassID.NetworkAddress)]
    public sealed class NetworkAddress : System.IComparable, System.IComparable<QS.Fx.Network.NetworkAddress>,
        QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.IStringSerializable
/*
        , QS.CMS.Base.IBase1Serializable, QS.CMS.Base2.IMessage, QS.CMS.Scattering.IScatterAddress, QS.CMS.Base2.IBase2Serializable
*/
    {
        #region Helpers

        /// <summary>
        /// Tests whether the given IP address is a multicast address.
        /// </summary>
        /// <param name="addr">The address to test.</param>
        /// <returns>True if the address is a multicast address.</returns>
        public static bool IsMulticastIPAddress(IPAddress addr)
        {
            byte firstByte = (addr.GetAddressBytes())[0];
            return firstByte >= 224 && firstByte <= 239;
        }

        /// <summary>
        /// True if this address is a multicast address.
        /// </summary>
        public bool IsMulticastAddress
        {
            get { return IsMulticastIPAddress(this.HostIPAddress); }
        }

        #endregion

        #region Constants

        /// <summary>
        /// Represents an uninitialized network address.
        /// </summary>
        public static NetworkAddress Any
        {
            get
            {
                return new NetworkAddress(IPAddress.None, 0);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a network address from a string. 
        /// </summary>
        /// <param name="addressString">The address string. It must be in the format "IPAddress:PortNo", e.g. 192.168.0.0:20000.</param>
        public NetworkAddress(string addressString)
		{
			int separator_position = addressString.IndexOf(":");
			this.host = IPAddress.Parse(addressString.Substring(0, separator_position));
			this.port = (int) Convert.ToUInt32(addressString.Substring(separator_position + 1));
		}

        /// <summary>
        /// Create a network address from a given IP address and port number.
        /// </summary>
        /// <param name="hostIPAddress">IP address</param>
        /// <param name="port">port number</param>
		public NetworkAddress(IPAddress hostIPAddress, int port)
		{
			this.host = hostIPAddress;
			this.port = port;
		}

        /// <summary>
        /// The copy constructor.
        /// </summary>
        /// <param name="anotherAddress">The address to duplicate.</param>
		public NetworkAddress(NetworkAddress anotherAddress)
		{
			this.host = anotherAddress.host;
			this.port = anotherAddress.port;
		}

        /// <summary>
        /// The default constructor that creates an uninitialized address.
        /// </summary>
		public NetworkAddress()
		{
		}

/*
        public NetworkAddress(QS.CMS.Base2.IBlockOfData blockOfData)
        {
            byte[] bytes = new byte[4];
            Buffer.BlockCopy(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, bytes, 0, 4);
            host = new IPAddress(bytes);
            port = BitConverter.ToInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer + 4);
        }
*/

        #endregion

        #region Accessors

        /// <summary>
		/// The IP address of the host.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public IPAddress HostIPAddress
		{
			get
			{
				return host;
			}

			set
			{
				host = value;
			}		
		}

        /// <summary>
        /// The string representation of the IP address of the host.
        /// </summary>
		[System.Xml.Serialization.XmlAttribute("IPAddress")]
		public string HostIPAddressAsString
		{
			get
			{
				return host.ToString();
			}

			set
			{
				host = IPAddress.Parse(value);
			}
		}

		/// <summary>
		/// The port number.
		/// </summary>
		[System.Xml.Serialization.XmlAttribute("PortNo")]
		public int PortNumber
		{
			get
			{
				return port;
			}

			set
			{
				port = value;
			}
        }

        #endregion

        #region Fields

        private IPAddress host;
		private int port;

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
		{
			return (obj is QS.Fx.Network.NetworkAddress) && 
				((QS.Fx.Network.NetworkAddress) obj).host.Equals(this.host) &&
				((QS.Fx.Network.NetworkAddress) obj).port.Equals(this.port);
		}

		public override int GetHashCode()
		{
			return host.GetHashCode() ^ port.GetHashCode();
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
//			bool isfirst = true;
//			foreach (byte b in host.GetAddressBytes())
//			{
//				if (isfirst)
//					isfirst = false;
//				else
//					sb.Append(".");
//				sb.Append(b.ToString("000"));
//			}
			sb.Append(host.ToString());
			sb.Append(":");
			sb.Append(port.ToString());
			return sb.ToString();
		}

        #endregion

/*
		#region ISerializable Members

        [System.Xml.Serialization.XmlIgnore]
        public virtual ClassID ClassIDAsSerializable
        {
            get
            {
                return ClassID.NetworkAddress;
            }
        }

		public virtual void save(System.IO.Stream memoryStream)
		{
			byte[] buffer;
			buffer = System.BitConverter.GetBytes(port);
			memoryStream.Write(buffer, 0, buffer.Length);
			buffer = host.GetAddressBytes();
			memoryStream.Write(buffer, 0, buffer.Length);			
		}

		public virtual void load(System.IO.Stream memoryStream)
		{
			byte[] buffer = new byte[4];
			memoryStream.Read(buffer, 0, 4);
			port = System.BitConverter.ToInt32(buffer, 0);
			buffer = new byte[4];
			memoryStream.Read(buffer, 0, 4);
			host = IPAddress.Parse(
				buffer[0] + "." + buffer[1] + "." + buffer[2] + "." + buffer[3]);			
		}

		#endregion
*/

/*
        [System.Xml.Serialization.XmlIgnore]
		public static uint SizeAsBlockOfData
		{
			get
			{				
				return 4 + (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(int));
			}
		}
*/

/*
		#region Base2.IMessage Members

		[System.Xml.Serialization.XmlIgnore]
		public QS.CMS.Base2.IOutgoingData AsOutgoingData
		{
			get
			{
				return new QS.CMS.Base2.OutgoingVector(new QS.CMS.Base2.IBlockOfData[] { new QS.CMS.Base2.BlockOfData(host.GetAddressBytes()), 
					new QS.CMS.Base2.BlockOfData(BitConverter.GetBytes(port)) });
			}
		}

		// public uint Size
		// {
		//	get
		//	{				
		//		return NetworkAddress.SizeAsBlockOfData;
		//	}
		// }
		//
		// public void serializeTo(byte[] bufferForData, uint offsetWithinBuffer, uint spaceWithinBuffer)
		// {
		//	Buffer.BlockCopy(, 0, bufferForData, (int) offsetWithinBuffer, 4);
		//	byte[] bytes = ;
		//	Buffer.BlockCopy(bytes , 0, bufferForData, (int) offsetWithinBuffer + 4, bytes.Length);
		// }

		#endregion
*/

/*
		#region IScatterAddress Members

		public QS.CMS.Scattering.AddressClass AddressClass
		{
			get
			{
				return QS.CMS.Scattering.AddressClass.INDIVIDUAL_ADDRESS;
			}
		}

		#endregion
*/

/*
		#region Base2.ISerializable Members

		public uint Size
		{
			get
			{
                return QS.CMS.Base2.SizeOf.IPAddress + QS.CMS.Base2.SizeOf.UInt32;
			}
		}

		void QS.CMS.Base2.IBase2Serializable.load(QS.CMS.Base2.IBlockOfData blockOfData)
		{
			host = QS.CMS.Base2.Serializer.loadIPAddress(blockOfData);
			port = QS.CMS.Base2.Serializer.loadInt32(blockOfData);
		}

		void QS.CMS.Base2.IBase2Serializable.save(QS.CMS.Base2.IBlockOfData blockOfData)
		{
            QS.CMS.Base2.Serializer.saveIPAddress(host, blockOfData);
            QS.CMS.Base2.Serializer.saveInt32(port, blockOfData);
		}

		public QS.ClassID ClassID
		{
			get
			{
				return ClassID.NetworkAddress;
			}
		}

		#endregion
*/

/*
		public unsafe void CopyTo(System.ArraySegment<byte> segment)
		{
			if (segment.Count < 6)
				throw new ArgumentException("Not enough space.");
			fixed (byte* arrayptr = segment.Array)
			{
				byte* dataptr = arrayptr + segment.Offset;
				*((int*)dataptr) = port;
#pragma warning disable 0618
				*((long*)(dataptr + sizeof(int))) = host.Address;
#pragma warning restore 0618
			}
		}
*/

        #region QS.Fx.Serialization.ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                int total_size = sizeof(int) + sizeof(long);
                return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.NetworkAddress, (ushort)total_size, total_size, 0);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte *headerptr = arrayptr + header.Offset;
                *((int*) headerptr) = port;

#pragma warning disable 0618

                *((long*)(headerptr + sizeof(int))) = host.Address;

#pragma warning restore 0618

            }
            header.consume(sizeof(int) + sizeof(long));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            long host_address = 0;
#if DEBUG_CheckAddressesOnDeserialization
            try
            {
#endif                
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    port = *((int*)headerptr);
                    host_address = *((long*)(headerptr + sizeof(int)));
                }
                header.consume(sizeof(int) + sizeof(long));

                host = new IPAddress(host_address);
#if DEBUG_CheckAddressesOnDeserialization
            }
            catch (Exception exc)
            {
                throw new Exception("Error while deserializing address {" + host_address.ToString("x") + "}.", exc);
            }
#endif
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
		{
			if (obj is QS.Fx.Network.NetworkAddress)
                return ((IComparable<NetworkAddress>) this).CompareTo(obj as NetworkAddress);
            else
                throw new ArgumentException();
        }

        #endregion

        #region CompareIPAddresses

        /// <summary>
        /// Tests whether the two given IP addresses are identical.
        /// </summary>
        /// <param name="address1">Address 1.</param>
        /// <param name="address2">Address 2.</param>
        /// <returns>True if Address1 == Address2.</returns>
        public unsafe static int CompareIPAddresses(IPAddress address1, IPAddress address2)
        {
#pragma warning disable 618
            long n1 = address1.Address;
            long n2 = address2.Address;
#pragma warning restore 618

            int result = ((byte*)&n1)[0] - ((byte*)&n2)[0];
            if (result == 0)
            {
                result = ((byte*)&n1)[1] - ((byte*)&n2)[1];
                if (result == 0)
                {
                    result = ((byte*)&n1)[2] - ((byte*)&n2)[2];
                    if (result == 0)
                        result = ((byte*)&n1)[3] - ((byte*)&n2)[3];
                }
            }
            return result;
        }

        #endregion

        #region IComparable<NetworkAddress> Members

        int IComparable<NetworkAddress>.CompareTo(NetworkAddress other)
        {            
            int result = CompareIPAddresses(this.HostIPAddress, other.HostIPAddress);
            return (result == 0) ? (this.PortNumber.CompareTo(other.PortNumber)) : result;
        }

#if Using_VS2005Beta1
        bool IComparable<NetworkAddress>.Equals(NetworkAddress other)
        {
            return ((IComparable<NetworkAddress>)this).CompareTo(other) == 0;
        }
#endif

        #endregion

		#region IStringSerializable Members

		ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort)ClassID.NetworkAddress; }
		}

		string QS.Fx.Serialization.IStringSerializable.AsString
		{
			get { return host.ToString() + ":" + port.ToString(); }
			set
			{
				int separator = value.IndexOf(":");
				host = IPAddress.Parse(value.Substring(0, separator));
				port = System.Convert.ToInt32(value.Substring(separator + 1));
			}
		}

		#endregion
	}
}
