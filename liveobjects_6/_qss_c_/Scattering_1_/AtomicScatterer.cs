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

namespace QS._qss_c_.Scattering_1_
{
	/// <summary>
	/// Summary description for AtomicScatterer.
	/// </summary>
	public class AtomicScatterer : IAtomicScatterer, Base1_.IClient
	{
		public AtomicScatterer(IRetransmittingScatterer retransmittingScatterer, Base2_.IDemultiplexer demultiplexer,
			Base2_.IMasterIOC objectContainer)
		{
			this.retransmittingScatterer = retransmittingScatterer;
			this.objectContainer = objectContainer;

			demultiplexer.register(this.LocalObjectID, new Base2_.ReceiveCallback(this.receiveCallback));
		}

		private Base2_.IMasterIOC objectContainer;
		private IRetransmittingScatterer retransmittingScatterer;
		private uint lastused_seqno = 0;

		private QS._core_c_.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable receivedObject)
		{

			return null;

		}

		#region Class MessageID

		[QS.Fx.Serialization.ClassID(QS.ClassID.AtomicScatterer_MessageID)]
		private class MessageID : QS._qss_c_.Components_1_.SeqNo
		{
			public MessageID()
			{
			}

			public MessageID(uint seqno, bool distributed)
			{
				this.distributed = distributed;
			}

			public bool distributed;

			#region Base2.ISerializable, System.IComparable and System.Object Overrides

			public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				base.save(blockOfData);
				QS._core_c_.Base2.Serializer.saveBool(distributed, blockOfData);
			}

			public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				base.load(blockOfData);
				distributed = QS._core_c_.Base2.Serializer.loadBool(blockOfData);
			}

			public override uint Size
			{
				get
				{
					return base.Size + QS._core_c_.Base2.SizeOf.Bool;
				}
			}

			public override ClassID ClassID
			{
				get
				{
					return QS.ClassID.AtomicScatterer_MessageID;
				}
			}

			public override int CompareTo(object obj)
			{
				int result = base.CompareTo(obj);
				return (result == 0) ? this.distributed.CompareTo(((MessageID) obj).distributed) : result;
			}

			public override bool Equals(object obj)
			{
				return base.Equals(obj) && distributed.Equals(((MessageID) obj).distributed);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode() ^ distributed.GetHashCode();
			}

			public override string ToString()
			{
				return base.ToString() + (distributed ? "/2" : "/1");
			}

			#endregion
		}

		#endregion

		#region Class Wrapper

		[QS.Fx.Serialization.ClassID(QS.ClassID.AtomicScatterer_Wrapper)]
		private class Wrapper : Base2_.IdentifiableObject
		{
			public Wrapper()
			{
			}

			public Wrapper(MessageID messageID, uint destinationLOID, QS._core_c_.Base2.IBase2Serializable transmittedObject)
			{
				this.messageID = messageID;
				this.transmittedObject = transmittedObject;
				this.destinationLOID = destinationLOID;
			}

			public MessageID messageID;
			public QS._core_c_.Base2.IBase2Serializable transmittedObject;
			public uint destinationLOID;

			#region Overrides

			public override QS._qss_c_.Base2_.IIdentifiableKey UniqueID
			{
				get
				{
					return messageID;
				}
			}

			public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				messageID.save(blockOfData);
				QS._core_c_.Base2.Serializer.saveUInt32(destinationLOID, blockOfData);
				if (!messageID.distributed)
				{
					QS._core_c_.Base2.Serializer.saveUInt16((ushort) transmittedObject.ClassID, blockOfData);
					transmittedObject.save(blockOfData);					
				}
			}

			public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				messageID = new MessageID();
				messageID.load(blockOfData);				
				destinationLOID = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
				if (!messageID.distributed)
				{
					transmittedObject = QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(
						(QS.ClassID) QS._core_c_.Base2.Serializer.loadUInt16(blockOfData));
					transmittedObject.load(blockOfData);
				}
			}

			public override uint Size
			{
				get
				{
					return messageID.Size + QS._core_c_.Base2.SizeOf.UInt32 + 
						(messageID.distributed ? 0 : (QS._core_c_.Base2.SizeOf.UInt16 + transmittedObject.Size));
				}
			}

			public override ClassID ClassID
			{
				get
				{
					return QS.ClassID.AtomicScatterer_Wrapper;
				}
			}

			#endregion
		}

		#endregion

		#region Class Request

		[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
		private class Request : RetransmittingScatterer.GenericRequest
		{
			public Request(uint destinationLOID, uint seqno, QS._core_c_.Base2.IBase2Serializable transmittedObject,
				QS._qss_c_.Scattering_1_.CompletionCallback completionCallback, IScatterSet addressSet, 
				AtomicScatterer associatedScatterer) 
				: base(associatedScatterer.LocalObjectID, addressSet, new Wrapper(new MessageID(seqno, false), 
					destinationLOID, transmittedObject), associatedScatterer.objectContainer, completionCallback)
			{
				this.associatedScatterer = associatedScatterer;
			}
			
			public AtomicScatterer associatedScatterer;

			protected override void completionCallback(bool succeeded, Exception exception)
			{
				

				base.completionCallback(succeeded, exception);
			}
		}

		#endregion

		#region IAtomicScatterer Members

		public void multicast(uint destinationLOID, IScatterSet addressSet, QS._core_c_.Base2.IBase2Serializable message, 
			QS._qss_c_.Scattering_1_.CompletionCallback completionCallback)
		{
			uint seqno;
			lock (this)
			{
				seqno = ++lastused_seqno;				
			}

			Request request = new Request(destinationLOID, seqno, message, completionCallback, addressSet, this);
			objectContainer.insert(request);

			retransmittingScatterer.multicast(request);
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.AtomicScatterer;
			}
		}

		#endregion
	}
}
