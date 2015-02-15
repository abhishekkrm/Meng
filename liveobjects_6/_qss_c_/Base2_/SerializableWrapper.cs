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
	/// Summary description for AckWrapper.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
	public abstract class SerializableWrapper : QS._core_c_.Base2.IBase2Serializable
	{
		public SerializableWrapper(QS._core_c_.Base2.IBase2Serializable wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}

		public SerializableWrapper()
		{
		}

		public QS._core_c_.Base2.IBase2Serializable WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}

		protected QS._core_c_.Base2.IBase2Serializable wrappedObject = null;

		public override string ToString()
		{
			return "(" + this.GetType().Name + ": " + wrappedObject.ToString() + ")";
		}

		#region ISerializable Members

		public virtual uint Size
		{
			get
			{
				return QS._core_c_.Base2.SizeOf.UInt16 + wrappedObject.Size;
			}
		}

		public virtual void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			Buffer.BlockCopy(BitConverter.GetBytes((ushort) wrappedObject.ClassID), 0, 
				blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) QS._core_c_.Base2.SizeOf.UInt16);
			blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
			wrappedObject.save(blockOfData);
		}

		public virtual void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS.ClassID classID = 
				(QS.ClassID) BitConverter.ToUInt16(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
			this.wrappedObject = (QS._core_c_.Base2.IBase2Serializable) QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(classID);
			wrappedObject.load(blockOfData);
		}

		public virtual QS.ClassID ClassID
		{
			get
			{
                return (QS.ClassID)((QS.Fx.Serialization.ClassIDAttribute)(this.GetType().GetCustomAttributes(typeof(QS.Fx.Serialization.ClassIDAttribute), false))[0]).ClassID;
			}
		}

		#endregion
	}
}
