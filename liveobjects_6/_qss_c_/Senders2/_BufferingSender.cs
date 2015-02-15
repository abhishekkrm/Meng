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
using System.Diagnostics;

namespace QS._qss_c_.Senders2
{
/*
	public class SequentialBufferingSender : Base2.ISender, Base2.IBufferingSender, Base.IClient
	{
		private const uint defaultAnticipatedNumberOfPeers = 20;

		public SequentialBufferingSender(Base2.IBlockSender underlyingBlockSender, Base2.IDemultiplexer demultiplexer, 
			Buffering.IBufferController bufferController)
			: this(underlyingBlockSender, demultiplexer, bufferController, (uint) ReservedObjectID.SequentialBufferingSender)
		{
		}

		public SequentialBufferingSender(Base2.IBlockSender underlyingBlockSender, Base2.IDemultiplexer demultiplexer, 
			Buffering.IBufferController bufferController, uint localObjectID)
		{
			this.underlyingBlockSender = underlyingBlockSender;
			demultiplexer.register(localObjectID, new Devices2.OnReceiveCallback(this.receiveCallback));

			destinationStates = new Collections.HashSet(defaultAnticipatedNumberOfPeers);
			sourceStates = new Collections.HashSet(defaultAnticipatedNumberOfPeers);

			this.localObjectID = localObjectID;

			this.bufferController = bufferController;
			bufferController.BufferSize = underlyingBlockSender.MTU;
		}

		private Base2.IBlockSender underlyingBlockSender;
		private Collections.ISet destinationStates, sourceStates;
		private uint localObjectID;
		private Buffering.IBufferController bufferController;

		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			Base2.IBlockOfData blockOfData)
		{
			SourceState sourceState = null;
			lock (sourceStates)
			{
				sourceState = (SourceState) sourceStates.lookup(sourceAddress);
				if (sourceState == null)
				{
					sourceState = new SourceState(sourceAddress);
					sourceStates.insert(sourceState);
				}

				Monitor.Enter(sourceState);
			}





			// ..................................................

			Monitor.Exit(sourceState);
		}

		#region IBufferingSender Members

		void QS.CMS.Base2.IBufferingSender.send(uint destinationLOID, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS.CMS.Base2.IOutgoingData outgoingData)
		{
			DestinationState destinationState = null;
			lock (destinationStates)
			{
				destinationState = (DestinationState) destinationStates.lookup(destinationAddress);
				if (destinationState == null)
				{
					destinationState = new DestinationState(destinationAddress);
					destinationState.outgoingBuffer = bufferController.CreateOutgoingBuffer;

					destinationStates.insert(destinationState);
				}

				Monitor.Enter(destinationState);
			}

			bool completed = false;
			while (!completed)
			{
				 completed = destinationState.outgoingBuffer.append(destinationLOID, ref outgoingData);

				if (destinationState.outgoingBuffer.Full)
				{
					underlyingBlockSender.send(this.localObjectID, destinationAddress, destinationState.outgoingBuffer);
					destinationState.outgoingBuffer = bufferController.CreateOutgoingBuffer;
				}
			}
	
			Monitor.Exit(destinationState);
		}

		#endregion

		#region Class DestinationState

		protected class DestinationState : PeerState
		{
			public DestinationState(QS.Fx.Network.NetworkAddress destinationAddress) : base(destinationAddress)
			{
			}

			public Buffering.IOutgoingBuffer outgoingBuffer;
		}

		#endregion

		#region Class SourceState

		protected class SourceState : PeerState
		{
			public SourceState(QS.Fx.Network.NetworkAddress sourceAddress) : base(sourceAddress)
			{

			}
		}

		#endregion

		#region Class PeerState

		protected abstract class PeerState
		{
			public PeerState(QS.Fx.Network.NetworkAddress networkAddress)
			{
				this.networkAddress = networkAddress;
			}

			protected QS.Fx.Network.NetworkAddress networkAddress;

			public override int GetHashCode()
			{
				return networkAddress.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return networkAddress.Equals(obj);
			}
		}

		#endregion

		#region ISender Members

		public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress destinationAddress, QS.CMS.Base2.IMessage message)
		{
			((Base2.IBufferingSender) this).send(destinationLOID, destinationAddress, message.AsOutgoingData);
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return localObjectID;
			}
		}

		#endregion
	}
*/	
}
