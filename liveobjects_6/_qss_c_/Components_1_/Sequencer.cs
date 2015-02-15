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

namespace QS._qss_c_.Components_1_
{
	/// <summary>
	/// Summary description for Sequencer.
	/// </summary>
	public class Sequencer
	{
		private static System.Object mylock = new System.Object();
		private static uint seqno = 0;

		private static bool dummy = register_serializable();
		public static bool register_serializable()
		{
			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(QS.ClassID.Sequencer_Wrapper, typeof(Wrapper));
			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(QS.ClassID.Sequencer_Wrapper_SeqNo, typeof(SeqNo));
			return true;
		}

		#region Interface IWrappedObject

		public interface IWrappedObject : Base2_.IIdentifiableObject
		{
			QS._core_c_.Base2.IBase2Serializable SerializableObject
			{
				get;
				set;
			}
		}

		#endregion

		#region Class Wrapper

		[QS.Fx.Serialization.ClassID(QS.ClassID.Sequencer_Wrapper)]
		private class Wrapper : Base2_.IdentifiableObject, IWrappedObject
		{
			public Wrapper()
			{
			}

			public Wrapper(QS._core_c_.Base2.IBase2Serializable serializableObject, uint seqno)
			{
				this.serializableObject = serializableObject;
				this.sequenceNo = new SeqNo(seqno);
			}

			public Wrapper(QS._core_c_.Base2.IBase2Serializable serializableObject, SeqNo sequenceNo)
			{
				this.serializableObject = serializableObject;
				this.sequenceNo = sequenceNo;
			}

			private QS._core_c_.Base2.IBase2Serializable serializableObject;
			private SeqNo sequenceNo;

			public override string ToString()
			{
				return sequenceNo.ToString() + ":" + serializableObject.ToString();
			}

			#region IWrappedObject Members

			public QS._core_c_.Base2.IBase2Serializable SerializableObject
			{
				get
				{
					return serializableObject;
				}

				set
				{
					serializableObject = value;
				}
			}

			#endregion

			#region IIdentifiableObject Members

			public override Base2_.IIdentifiableKey UniqueID
			{
				get
				{
					return sequenceNo;
				}
			}

			#endregion

			#region ISerializable Members

			public override QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.Sequencer_Wrapper;
				}
			}

			public override uint Size
			{
				get
				{
					return sequenceNo.Size + QS._core_c_.Base2.SizeOf.UInt16 + serializableObject.Size;
				}
			}

			public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				sequenceNo.save(blockOfData);
				Buffer.BlockCopy(BitConverter.GetBytes((ushort) serializableObject.ClassID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) QS._core_c_.Base2.SizeOf.UInt16);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
				serializableObject.save(blockOfData);
			}

			public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				sequenceNo = new SeqNo();
				sequenceNo.load(blockOfData);
				QS.ClassID classID = (QS.ClassID) BitConverter.ToUInt16(
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
				serializableObject = QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(classID);
				serializableObject.load(blockOfData);				
			}

			#endregion
		}

		#endregion

		public static IWrappedObject wrap(QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			lock (mylock)
			{				
				return new Wrapper(serializableObject, ++seqno);
			}
		}
	}
}
