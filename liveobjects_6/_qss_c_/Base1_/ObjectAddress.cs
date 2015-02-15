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
using System.Net;
using System.IO;

namespace QS._qss_c_.Base1_
{	 
	[QS.Fx.Serialization.ClassID(QS.ClassID.ObjectAddress)]
	public sealed class ObjectAddress : IAddress
	{
        public static ObjectAddress ParseURI(string uri)
        {
            IPAddress _ipaddress;
            int _port;
            uint _loid;
            ParseURI(uri, out _ipaddress, out _port, out _loid);
            return new ObjectAddress(new QS.Fx.Network.NetworkAddress(_ipaddress, _port), _loid);
        }

        public string ToURI()
        {
            return CreateURI(networkaddress.HostIPAddress, networkaddress.PortNumber, loid);
        }

        // QuickSilver://172.23.79.101:12000/956

        public static string ProtocolName
        {
            get { return protocol_name; }
        }

        private const string protocol_name = "QuickSilver";
        private const string protocol_prefix = protocol_name + "://";
        private static void ParseURI(string uri, out IPAddress ipAddress, out int portno, out uint loid) 
        {
            if (!uri.StartsWith(protocol_prefix))
                throw new Exception("Bad format: must start with \"" + protocol_prefix + "\".");
            int colon_position = uri.IndexOf(':', protocol_prefix.Length);
            if (colon_position < 0)
                throw new Exception("Bad format: must specify port number.");
            int slash_position = uri.IndexOf('/', colon_position);
            if (slash_position < 0)
                throw new Exception("Bad format: must specify registered objectid (channel).");

            ipAddress = IPAddress.Parse(uri.Substring(protocol_prefix.Length, colon_position - protocol_prefix.Length));
            portno = Convert.ToInt32(uri.Substring(colon_position + 1, slash_position - colon_position - 1));
            loid = Convert.ToUInt32(uri.Substring(slash_position + 1));
        }

        private static string CreateURI(IPAddress ipAddress, int portno, uint loid)
        {
            return protocol_prefix + ipAddress.ToString() + ":" + portno.ToString() + "/" + loid.ToString();
        }

        public ObjectAddress(QS.Fx.Network.NetworkAddress networkAddress, uint localObjectID) 
		{
            this.networkaddress = new QS.Fx.Network.NetworkAddress(networkaddress);
			this.loid = localObjectID;
		}

		public ObjectAddress(IPAddress hostIPAddress, int port, uint localObjectID) 
		{
            this.networkaddress = new QS.Fx.Network.NetworkAddress(hostIPAddress, port);
			this.loid = localObjectID;
		}

		public ObjectAddress() 
		{
            this.networkaddress = new QS.Fx.Network.NetworkAddress();
		}

        public QS.Fx.Network.NetworkAddress NetworkAddress
        {
            get { return networkaddress; }
            set { networkaddress = value; }
        }

		/// <summary>
		/// A local identifier of the object with respect to the 
		/// host and (a protocol stack sitting on) a given port.
		/// </summary>
		public uint LocalObjectID
		{
			get
			{
				return loid;
			}

			set
			{
				loid = value;
			}
		}

        private QS.Fx.Network.NetworkAddress networkaddress; 
		private uint loid;

		public override string ToString()
		{
			return  networkaddress.ToString() + "." + loid.ToString();
		}

		#region IAddress Members

		public QS._qss_c_.Base1_.ObjectAddress[] Destinations
		{
			get
			{
				return new ObjectAddress[] { this };
			}
		}

		#endregion

/*
		#region ISerializable Members

		public override ClassID ClassIDAsSerializable
		{
			get
			{
				return ClassID.ObjectAddress;
			}
		}

		public override void save(System.IO.Stream memoryStream)
		{
			byte[] buffer;
			buffer = System.BitConverter.GetBytes(loid);
			memoryStream.Write(buffer, 0, buffer.Length);			
			base.save(memoryStream);
		}

		public override void load(System.IO.Stream memoryStream)
		{
			byte[] buffer = new byte[4];
			memoryStream.Read(buffer, 0, 4);
			loid = System.BitConverter.ToUInt32(buffer, 0);
			base.load(memoryStream);
		}

		#endregion
*/
	}
}
