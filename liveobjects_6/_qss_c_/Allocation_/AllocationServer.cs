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
using System.Threading;

namespace QS._qss_c_.Allocation_
{
	/// <summary>
	/// Summary description for Server.
	/// </summary>

	public abstract class AllocationServer : Base1_.IClient
	{
		public AllocationServer(Base2_.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;
			subscriptionContainer = new Collections_2_.SynchronizedRBT(new Collections_1_.RawSplayTree(), logger);
			createBTNCallback = new QS._qss_c_.Collections_2_.CreateBinaryTreeNodeCallback(this.my_createCallback);

			demultiplexer.register(this.LocalObjectID, new Base2_.ReceiveCallback(this.receiveCallback));
		}

		private QS.Fx.Logging.ILogger logger;
		private Collections_2_.ISynchronizedRBT subscriptionContainer;
		private Collections_2_.CreateBinaryTreeNodeCallback createBTNCallback;

		protected interface IAllocatedObject : Base2_.IIdentifiableObject
		{
			QS._core_c_.Base2.IBase2Serializable AllocatedObject
			{
				get;
			}
		}

		#region Class GenericAllocatedObject

		[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
		protected abstract class GenericAllocatedObject : Base2_.IdentifiableObject, IAllocatedObject
		{
			public GenericAllocatedObject(Base2_.IIdentifiableKey key)
			{
				this.key = key;
			}

			private Base2_.IIdentifiableKey key;

			public override QS._qss_c_.Base2_.IIdentifiableKey UniqueID
			{
				get
				{
					return key;
				}
			}

			#region IAllocatedObject Members

			public virtual QS._core_c_.Base2.IBase2Serializable AllocatedObject
			{
				get
				{
					return this;
				}
			}

			#endregion
		}

		#endregion

		private QS._core_c_.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable argumentObject)
		{
			try
			{
				Base2_.IIdentifiableKey key = (Base2_.IIdentifiableKey) argumentObject;
				bool createdAnew;
				IAllocatedObject allocatedObject = 
					(IAllocatedObject) subscriptionContainer.lookupOrCreate(key, createBTNCallback, out createdAnew);

				if (allocatedObject != null)
				{
					QS._core_c_.Base2.IBase2Serializable responseObject = allocatedObject.AllocatedObject;
					Monitor.Exit(allocatedObject);

					return responseObject;
				}
				else
					throw new Exception("Could not allocate object.");
			}
			catch (Exception exc)
			{
				logger.Log(this, "__receiveCallback failed : " + exc.ToString());
				return null;
			}
		}

		protected abstract IAllocatedObject createCallback(Base2_.IIdentifiableKey lookupKey);		

		private Collections_1_.IBinaryTreeNode my_createCallback(System.IComparable key)
		{
			return this.createCallback((Base2_.IIdentifiableKey) key);
		}

		#region IClient Members

		public virtual uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.AllocationServer;
			}
		}

		#endregion
	}
}
