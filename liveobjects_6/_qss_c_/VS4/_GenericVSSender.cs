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

namespace QS._qss_c_.VS4
{
	/// <summary>
	/// Summary description for GenericVSSender.
	/// </summary>
/*	/// 
	public abstract class GenericVSSender : Base.IClient, IVSSender // , IIncomingMessageContainerFactory
	{
		public GenericVSSender(VS4.IViewController viewController, Multicasting2.IMulticastingSender multicastingSender)
		{
			this.viewController = viewController;
			this.multicastingSender = multicastingSender;
		}

		private VS4.IViewController viewController;
		private Multicasting2.IMulticastingSender multicastingSender;

		#region Class MessageWrapper

		private static uint headerOverhead = Base2.SizeOf.Int32 + 2 * Base2.SizeOf.UInt32;

		private class MessageWrapper : Base2.IdentifiableObject	
		{
			public MessageWrapper(Base2.ISerializable wrappedObject, 
				GMS.GroupId groupID, uint viewSeqNo, uint withinViewSeqNo)
			{
				this.wrappedObject = wrappedObject;
				this.groupID = groupID;
				this.viewSeqNo = viewSeqNo;
				this.withinViewSeqNo = withinViewSeqNo;
			}

			private Base2.ISerializable wrappedObject;
			private GMS.GroupId groupID;
			private uint viewSeqNo, withinViewSeqNo;

			#region Base2.IIdentifiableSerializableObject Members

			public override Base2.IIdentifiableKey UniqueID
			{
				get
				{
					return null; // ...........................................................
				}
			}

			#endregion

			#region ISerializable Members

			public override QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.GenericVSSender_MessageWrapper;
				}
			}

			public override uint Size
			{
				get
				{
					return headerOverhead + wrappedObject.Size;
				}
			}

			public override void save(Base2.IBlockOfData blockOfData)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(groupID.ID), 0, blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.Int32);
				blockOfData.consume(Base2.SizeOf.Int32);
				Buffer.BlockCopy(BitConverter.GetBytes(viewSeqNo), 0, blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.UInt32);
				blockOfData.consume(Base2.SizeOf.UInt32);
				Buffer.BlockCopy(BitConverter.GetBytes(withinViewSeqNo), 0, blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.UInt32);
				blockOfData.consume(Base2.SizeOf.UInt32);
				wrappedObject.save(blockOfData);
			}

			public override void load(Base2.IBlockOfData blockOfData)
			{
				// ...........
			}

			#endregion
		}

		#endregion

		#region IVSSender Members

		public void send(GMS.GroupId groupID, QS.CMS.Base2.ISerializable serializableObject)
		{
			IMembershipViewRef membershipViewRef;
			uint withinViewSeqNo;

			viewController.registerOutgoingMessage(groupID, out membershipViewRef, out withinViewSeqNo);

			MessageWrapper messageWrapper = new MessageWrapper(serializableObject, groupID, 
				membershipViewRef.ViewSeqNo, withinViewSeqNo);

			// multicastingSender.multicast(this.LocalObjectID, membershipViewRef, messageWrapper);
		}

		public uint MTU
		{
			get
			{
				return multicastingSender.MTU - headerOverhead;
			}
		}

		#endregion


//		#region IIncomingMessageContainerFactory Members
//
//		public abstract IIncomingMessageContainer createContainer(GMS.GroupId groupID, uint viewSeqNo);
//
//		#endregion
		

		#region Base.IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.VSSender;
			}
		}

		#endregion
	}
*/
}
