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

namespace QS._qss_c_.Allocation_
{
/*
	/// <summary>
	/// Summary description for Client.
	/// </summary>
	/// 

	public class AllocationClient : IAllocationClient, Base.IClient
	{
		public AllocationClient(Base.ObjectAddress serverAddress, RPC2.IRPCProxy rpcProxy, 
			Base2.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger)
		{
			this.serverAddress = serverAddress;
			this.rpcProxy = rpcProxy;
			rpcCallback = new QS.CMS.RPC2.RPCCallback(this.completionCallback);
			this.logger = logger;

			demultiplexer.register(this.LocalObjectID, new Base2.ReceiveCallback(this.receiveCallback));
		}

		private Base.ObjectAddress serverAddress;
		private RPC2.IRPCProxy rpcProxy;
		private RPC2.RPCCallback rpcCallback;
		private QS.Fx.Logging.ILogger logger;

		private class Subscription : Base2.GenericSharedObject, IAllocatedObject
		{
			public Subscription(Base2.IIdentifiableKey key, AllocationCallback allocationCallback,
				AllocationClient allocationClient)
			{
				this.key = key;
				this.allocationCallback = allocationCallback;
				this.allocationClient = allocationClient;
			}

			public Base2.IIdentifiableKey key;
			public AllocationCallback allocationCallback;
			public Base2.IBase2Serializable result;
			public AllocationClient allocationClient;

			#region IAllocatedObject Members
			
			public Base2.IBase2Serializable AllocatedObject
			{
				get
				{
					return this.result;
				}
			}

			#endregion

			#region GenericSharedObject Overrides

			protected override void ObjectReleased()
			{
				allocationClient.logger.Log(this, "__ObjectReleased : We should do some cleanup here...");

				// ...................
			}

			#endregion

			public override string ToString()
			{
				return "Allocated:" + key.ToString() + "->" + result.ToString();
			}
		}

		private Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBase2Serializable argumentObject)
		{
			throw new Exception("Operation not permitted in this context.");

			// ....................later we should implement something here, but for now we leave it as is

		}

		private void completionCallback(object contextObject, Base2.IBase2Serializable result)
		{
			Subscription subscription = (Subscription) contextObject;
			subscription.result = result;
			subscription.allocationCallback(subscription.key, subscription);
		}

		#region IAllocationClient Members

		public void allocate(Base2.IIdentifiableKey key, AllocationCallback allocationCallback)
		{
			Subscription subscription = new Subscription(key, allocationCallback, this);
			rpcProxy.call(serverAddress, key, rpcCallback, subscription);
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.AllocationClient;
			}
		}

		#endregion
	}
*/ 
}
