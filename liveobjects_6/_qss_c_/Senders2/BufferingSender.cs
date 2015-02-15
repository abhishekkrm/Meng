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

namespace QS._qss_c_.Senders2
{
	/// <summary>
	/// Summary description for BufferingSender.
	/// </summary>
	public class BufferingSender : Base2_.IBlockSender, Base1_.IClient
	{
		public BufferingSender(uint loid, Base2_.IBlockSender underlyingBlockSender, Base2_.IDemultiplexer demultiplexer, 
			Buffering_1_.IBufferController bufferController, QS.Fx.Logging.ILogger logger)
		{
			this.demultiplexer = demultiplexer;
			this.logger = logger;
			this.underlyingBlockSender = underlyingBlockSender;
			this.bufferController = bufferController;
			this.loid = loid;

			destinationStates = new Collections_2_.SynchronizedRBT(new QS._qss_c_.Collections_1_.HashedSplaySet(30), logger);
			destinationStateCreateCallback = 
				new QS._qss_c_.Collections_2_.CreateBinaryTreeNodeCallback(this.createCallback);

			demultiplexer.register(loid, new Base2_.ReceiveCallback(this.receiveCallback));
		}

		private Base2_.IBlockSender underlyingBlockSender;
		private QS.Fx.Logging.ILogger logger;
		private Buffering_1_.IBufferController bufferController;
		private Base2_.IDemultiplexer demultiplexer;
		private Collections_2_.SynchronizedRBT destinationStates;
		private Collections_2_.CreateBinaryTreeNodeCallback destinationStateCreateCallback;
		private uint loid;

		private QS._core_c_.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			Buffering_1_.IBufferedObjects bufferedObjects = (Buffering_1_.IBufferedObjects) serializableObject;
			uint destinationLOID;
			QS._core_c_.Base2.IBase2Serializable wrappedObject;
			while (bufferedObjects.remove(out destinationLOID, out wrappedObject))
				demultiplexer.demultiplex(destinationLOID, sourceAddress, destinationAddress, wrappedObject);

			return null;
		}

		#region Class DestinationState

		private class DestinationState : Collections_1_.GenericBinaryTreeNode
		{
			public DestinationState(QS.Fx.Network.NetworkAddress address, BufferingSender associatedBufferingSender)
			{
				this.address = address;
				this.associatedBufferingSender = associatedBufferingSender;
			}

			private QS.Fx.Network.NetworkAddress address;
			private BufferingSender associatedBufferingSender;
			private Buffering_1_.IOutgoingBuffer buffer = null;

			public QS._core_c_.Base2.IBase2Serializable append(uint destinationLOID, 
				QS._core_c_.Base2.IBase2Serializable serializableObject)
			{
				if (buffer == null)
					buffer = associatedBufferingSender.bufferController.CreateOutgoingBuffer(
						associatedBufferingSender.underlyingBlockSender.MTU);
				
				return buffer.append(destinationLOID, serializableObject);
			}

			#region Overrides

			public override IComparable Contents
			{
				get
				{
					return address;
				}
			}

			#endregion
		}

		private Collections_1_.IBinaryTreeNode createCallback(System.IComparable key)
		{
			return new DestinationState((QS.Fx.Network.NetworkAddress) key, this);
		}

		#endregion

		#region IBlockSender Members

		public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			bool createdAnew;
			DestinationState destinationState = (DestinationState) destinationStates.lookupOrCreate(
				destinationAddress, destinationStateCreateCallback, out createdAnew);

			QS._core_c_.Base2.IBase2Serializable packet = destinationState.append(destinationLOID, serializableObject);

			Monitor.Exit(destinationState);

			if (packet != null)
				this.underlyingBlockSender.send(this.LocalObjectID, destinationAddress, packet);
		}

        public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress[] destinationAddresses, QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            throw new NotImplementedException();
        }

        public void send(uint destinationLOID, System.Collections.ICollection destinationAddresses, QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            throw new NotImplementedException();
        }

        public void send(uint destinationLOID, 
            System.Collections.Generic.ICollection<QS.Fx.Network.NetworkAddress> destinationAddresses, QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            throw new NotImplementedException();
        }

        public uint MTU
        {
			get
			{
				return underlyingBlockSender.MTU - bufferController.MinimumOverhead;
			}
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return this.loid;
			}
		}

		#endregion
	}
}
