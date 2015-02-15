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

namespace QS._qss_e_.Tests_.Test107
{
/*
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		private const uint port_number = 12233;
		private const uint local_objid = 33333;

		public MainApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args)
		{
			this.platform = platform;

			QS.CMS.Base2.Serializer.CommonSerializer.registerClasses(platform.Logger);

			IPAddress localIPAddress;
			if (args.contains("base"))
				localIPAddress = IPAddress.Parse((string) args["base"]);
			else if (args.contains("subnet"))
				localIPAddress = CMS.Devices2.Network.AnyAddressOn(
					new CMS.Base.Subnet((string) args["subnet"]), platform);
			else 
				localIPAddress = platform.NICs[0];
			localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, (int) port_number);			

			platform.Logger.Log(null, "Base Address Chosen : " + localAddress.ToString());

			demultiplexer = new CMS.Base2.Demultiplexer(platform.Logger);
			rootSender = new QS.CMS.Base2.RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);
			masterContainer = new CMS.Base2.MasterIOC();

			demultiplexer.register(local_objid, new CMS.Base2.ReceiveCallback(this.receiveCallback));
		
			rpcProxy = new CMS.RPC2.RPCProxy(demultiplexer, rootSender, platform.Logger, masterContainer, platform.AlarmClock);

			if (args.contains("allocationserver"))
			{
				platform.Logger.Log(this, "Connecting to alocation server on " + args["allocationserver"] + ".");
				allocationClient = new CMS.Allocation.AllocationClient(
					new CMS.Base.ObjectAddress(IPAddress.Parse((string) args["allocationserver"]), 
					(int) port_number, (uint) QS.ReservedObjectID.IPMulticast_AllocationServer), rpcProxy, 
					demultiplexer, platform.Logger);

			}
			else
			{
				platform.Logger.Log(this, "Starting up alocation server on this node.");
				allocationServer = new CMS.IPMulticast.AllocationServer(demultiplexer, platform.Logger);
			}
		}

		private CMS.Platform.IPlatform platform;
		private QS.Fx.Network.NetworkAddress localAddress;
		private CMS.Base2.IDemultiplexer demultiplexer;
		private CMS.Base2.RootSender rootSender;
		private CMS.Base2.IMasterIOC masterContainer;
		private CMS.RPC2.RPCProxy rpcProxy;

		private CMS.Allocation.IAllocationClient allocationClient;
		private CMS.IPMulticast.AllocationServer allocationServer;

		private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, CMS.Base2.IBase2Serializable serializableObject)
		{
			string req = serializableObject.ToString();
			platform.Logger.Log(this, "__________ReceiveCallback : " + req);
			return new QS.CMS.Base2.StringWrapper("Res(" + req + ")");
		}

		public void call(GMS.GroupId groupID)
		{
			allocationClient.allocate(groupID, new CMS.Allocation.AllocationCallback(this.allocationCallback));


//			rpcProxy.call(new CMS.Base.ObjectAddress(IPAddress.Parse(address), (int) port_number, local_objid), 
//				new CMS.Base2.StringWrapper("calling:" + arg), new CMS.RPC2.RPCCallback(this.rpcCallback), "dupa");
		}

		private void allocationCallback(
			CMS.Base2.IIdentifiableKey key, CMS.Allocation.IAllocatedObject allocatedObject)
		{
			platform.Logger.Log(this, "__allocationCallback : " + 
				key.ToString() + " -> " + allocatedObject.ToString());
		}

		private void rpcCallback(object contextObject, CMS.Base2.IBase2Serializable serializableObject)
		{
			platform.Logger.Log(this, 
				"__________RPCCallback : " + contextObject.ToString() + ", " + serializableObject.ToString());
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
*/
}
