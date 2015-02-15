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

namespace QS._qss_c_.IPMulticast_
{
/*
	/// <summary>
	/// Summary description for AllocationServer.
	/// </summary>
	public class AllocationServer : Allocation.AllocationServer
	{
		private const string defaultSubnet	= "224.0.66.0:255.255.255.240"; // "224.0.6.0:255.255.255.128";
		private const uint defaultMinPortNo	= 15000;
		private const uint defaultMaxPortNo	= 15050;

		private const uint defaultNumberOfAllocationAttempts = 20;

		private static Base.Subnet defaultAllocationSubnet = new Base.Subnet(defaultSubnet);
		public AllocationServer(Base2.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger)
			: this(demultiplexer, logger, defaultAllocationSubnet, defaultMinPortNo, defaultMaxPortNo)
		{
		}

		public AllocationServer(Base2.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger, 
			Base.Subnet allocationSubnet, uint allocationMinimumPortNo, uint allocationMaximumPortNo)
			: base(demultiplexer, logger)
		{
			this.allocationSubnet = allocationSubnet;
			this.allocationMinimumPortNo = allocationMinimumPortNo;
			this.allocationMaximumPortNo = allocationMaximumPortNo;

			allocatedAddresses = new System.Collections.ArrayList();
			allocatedPorts = new System.Collections.ArrayList();
		}

		private Base.Subnet allocationSubnet;
		private uint allocationMinimumPortNo, allocationMaximumPortNo;
		private static System.Random random = new System.Random();

		private System.Collections.ArrayList allocatedAddresses, allocatedPorts;		
		
		#region Class AllocatedAddress

		[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
		protected class AllocatedAddress : Allocation.AllocationServer.GenericAllocatedObject
		{
			public AllocatedAddress(QS.CMS.Base2.IIdentifiableKey lookupKey, QS.Fx.Network.NetworkAddress networkAddress)
				: base(lookupKey)
			{
				this.networkAddress = networkAddress;
			}

			public QS.Fx.Network.NetworkAddress networkAddress;
			
			public override Base2.IBase2Serializable AllocatedObject
			{
				get
				{
					return networkAddress;
				}
			}	
		}

		#endregion

		protected override QS.CMS.Allocation.AllocationServer.IAllocatedObject createCallback(
			QS.CMS.Base2.IIdentifiableKey lookupKey)
		{
			QS.CMS.Allocation.AllocationServer.IAllocatedObject allocatedObject = null;

			lock (this)
			{
				IPAddress ipAddress = IPAddress.None;
				uint portno = 0;

				uint countdown = defaultNumberOfAllocationAttempts;
				bool succeeded = false;
				while (!succeeded && countdown-- > 0)
				{
					ipAddress = allocationSubnet.RandomAddress;
					succeeded = !allocatedAddresses.Contains(ipAddress);
				}

				if (!succeeded)
					throw new Exception("Could not find a free address on the assigned subnet.");

				countdown = defaultNumberOfAllocationAttempts;
				succeeded = false;
				while (!succeeded && countdown-- > 0)
				{
					portno = (uint) random.Next((int) allocationMinimumPortNo, (int) allocationMaximumPortNo);
					succeeded = !allocatedPorts.Contains(portno);
				}

				if (!succeeded)
					throw new Exception("Could not find a free port number in the assigned range.");

				allocatedAddresses.Add(ipAddress);
				allocatedPorts.Add(portno);

				allocatedObject = new AllocatedAddress(lookupKey, new QS.Fx.Network.NetworkAddress(ipAddress, (int) portno));
			}

			return allocatedObject;
		}

		public override uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.IPMulticast_AllocationServer;
			}
		}
	}
*/ 
}
