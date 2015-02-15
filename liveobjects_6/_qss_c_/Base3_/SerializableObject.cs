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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Serialization.Formatters.Binary;

namespace QS._qss_c_.Base3_
{
/*
	public interface ISerializableObject : QS.Fx.Serialization.ISerializable
	{
		object SerializedObject
		{
			get;
			set;
		}

		SerializationClass SerializationClass
		{
			get;
			set;
		}

		void Synchronize();
	}

	public enum SerializationClass
	{
		BINARY, SOAP
	}

	[QS.Fx.Serialization.ClassID(ClassID.Base3_SerializableObject)]
	public class SerializableObject : QS.Fx.Serialization.ISerializableObject
	{
		public SerializableObject() : this(null)
		{
		}

		public SerializableObject(object serializedObject) : this(serializedObject, SerializationClass.BINARY)
		{
		}

		public SerializableObject(object serializedObject, SerializationClass serializationClass)
		{
			this.serializedObject = serializedObject;
		}

		private object serializedObject;
		private SerializationClass serializationClass;
		private System.Nullable<Buffers> buffers;

		#region Formatters

		private static System.Runtime.Serialization.Formatter binaryFormatter = new BinaryFormatter();
		private static System.Runtime.Serialization.Formatter Formatter(SerializationClass serializationClass)
		{
			switch (serializationClass)
			{
				case SerializationClass.BINARY:
					return binaryFormatter;

				default:
					throw new NotSupportedException();
			}
		}

		#endregion

		#region ISerializableObject Members

		object ISerializableObject.SerializedObject
		{
			get { return serializedObject; }
			set { serializedObject = value; }
		}

		SerializationClass ISerializableObject.SerializationClass
		{
			get { return serializationClass; }
			set { serializationClass = value; }
		}

		void ISerializableObject.Synchronize()
		{
			lock (this)
			{
				if (serializedObject == null)
					buffers = null;
				else
				{
					Base3.Buffers stream = new Base3.Buffers();
					System.Runtime.Serialization.Formatter formatter = Formatter(serializationClass);
					lock (formatter)
					{
						formatter.Serialize(stream, serializedObject);
					}
					this.buffers = stream;
				}
			}
		}

		#endregion

		#region ISerializable Members

		public SerializableInfo SerializableInfo
		{
			get 
			{ 
				if (!stream.HasValue)
					Synchronize();
				SerializableInfo info = buffers.Value.SerializableInfo; 
				info.ClassID = ClassID.Base3_SerializableObject;
				return info;
			}
		}

		private static BinaryFormatter formatter = new BinaryFormatter();

		public unsafe void SerializeTo(ref WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data)
		{
			buffers.Value.SerializeTo(ref header, ref data);
			buffers = null;
		}

		public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
		{
	// ...........
		}

		#endregion
	}
*/
}
