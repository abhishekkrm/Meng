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

namespace QS._qss_c_.VS5
{
	/// <summary>
	/// Summary description for GenericVSMessage.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.VSMessage)]
	public class GenericVSMessage : Base2_.IdentifiableObject, IVSMessage
	{
		public static void register_serializable()
		{
			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(ClassID.VSMessage, typeof(GenericVSMessage));
		}

		public GenericVSMessage()
		{
		}

		public GenericVSMessage(GenericVSMessageID messageID, QS._core_c_.Base2.IBase2Serializable serializableObject) : base()
		{
			this.messageID = messageID;
			this.serializableObject = serializableObject;
		}

		private GenericVSMessageID messageID;
		private QS._core_c_.Base2.IBase2Serializable serializableObject;

		#region IVSMessage Members

		public IVSMessageID ID
		{
			get
			{
				return this.messageID;
			}
		}

		public QS._core_c_.Base2.IBase2Serializable WrappedObject
		{
			get
			{
				return this.serializableObject;
			}
		}

		#endregion

		#region IdentifiableObject Overrides

		public override QS._qss_c_.Base2_.IIdentifiableKey UniqueID
		{
			get
			{
				return messageID;
			}
		}

		#endregion

		#region ISerializable Members

		public override uint Size
		{
			get
			{
				return messageID.Size + 2 * QS._core_c_.Base2.SizeOf.UInt16 + 
					((serializableObject != null) ? serializableObject.Size : 0);
			}
		}

		public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS.ClassID classID = (QS.ClassID) QS._core_c_.Base2.Serializer.loadUInt16(blockOfData);
			messageID = (GenericVSMessageID) QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(classID);
			messageID.load(blockOfData);
			classID = (QS.ClassID) QS._core_c_.Base2.Serializer.loadUInt16(blockOfData);
			if (classID == QS.ClassID.Nothing)
			{
				serializableObject = null;
			}
			else
			{
				serializableObject = QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(classID);
				serializableObject.load(blockOfData);
			}
		}

		public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS._core_c_.Base2.Serializer.saveUInt16((ushort) messageID.ClassID, blockOfData);
			messageID.save(blockOfData);
			if (serializableObject != null)
			{
				QS._core_c_.Base2.Serializer.saveUInt16((ushort) serializableObject.ClassID, blockOfData);
				serializableObject.save(blockOfData);
			}
			else
				QS._core_c_.Base2.Serializer.saveUInt16((ushort) QS.ClassID.Nothing, blockOfData);
		}

		public override QS.ClassID ClassID
		{
			get
			{
				return ClassID.VSMessage;
			}
		}

		#endregion
	}
}
