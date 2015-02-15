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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for IWrappedIO.
	/// </summary>
	public interface IWrappedIO : Base2_.IIdentifiableObject
	{
		QS._core_c_.Base2.IBase2Serializable WrappedObject
		{
			get;
		}
	}

	[QS.Fx.Serialization.ClassID(QS.ClassID.WrappedIO)]
	public class WrappedIO : IdentifiableObject, IWrappedIO
	{
		public static void register_serializable()
		{
			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(QS.ClassID.WrappedIO, typeof(WrappedIO));
		}

		public WrappedIO()
		{
		}

		public WrappedIO(Base2_.IIdentifiableKey key, QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			this.key = key;
			this.serializableObject = serializableObject;
		}

		private Base2_.IIdentifiableKey key;
		private QS._core_c_.Base2.IBase2Serializable serializableObject;

		#region IWrappedIO Members

		public QS._core_c_.Base2.IBase2Serializable WrappedObject
		{
			get
			{
				return serializableObject;
			}
		}

		#endregion

		#region IIdentifiableObject Members

		public override IIdentifiableKey UniqueID
		{
			get
			{
				return key;
			}
		}

		#endregion

		#region ISerializable Members

		public override uint Size
		{
			get
			{
				return key.Size + serializableObject.Size + 2 * QS._core_c_.Base2.SizeOf.UInt16;
			}
		}

		public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			key = (Base2_.IIdentifiableKey) QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(
				(QS.ClassID) QS._core_c_.Base2.Serializer.loadUInt16(blockOfData));
			key.load(blockOfData);
			serializableObject = QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(
				(QS.ClassID) QS._core_c_.Base2.Serializer.loadUInt16(blockOfData));
			serializableObject.load(blockOfData);		
		}

		public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS._core_c_.Base2.Serializer.saveUInt16((ushort) key.ClassID, blockOfData);
			key.save(blockOfData);
			QS._core_c_.Base2.Serializer.saveUInt16((ushort) serializableObject.ClassID, blockOfData);
			serializableObject.save(blockOfData);		
		}

		public override QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.WrappedIO;
			}
		}

		#endregion
	}
}
