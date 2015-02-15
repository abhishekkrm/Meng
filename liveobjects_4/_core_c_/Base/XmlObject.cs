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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace QS._core_c_.Base
{
	/// <summary>
	/// Summary description for XmlMessage.
	/// </summary>
	[Serializable]
	public class XmlObject : IMessage
	{
		public static CreateSerializable Factory = new CreateSerializable(XmlObject.createSerializable);

		private static IBase1Serializable createSerializable()
		{
			return new XmlObject();
		}

		public XmlObject()
		{
			this.contents = null;
		}

		public XmlObject(System.IO.Stream stream)
		{
			this.contents = null;
			this.load(stream);
		}

		public XmlObject(object contents)
		{
			this.contents = contents;
		}

		public object Contents
		{
			get
			{
				return contents;
			}

			set
			{
				contents = value;
			}
		}

		public override string ToString()
		{
			return contents.ToString();
		}

		private object contents;

		#region IMessage Members

		public ClassID ClassIDAsSerializable
		{
			get
			{
				return ClassID.XmlMessage;
			}
		}

		[Serializable]
		public class ObjectWrapper
		{
			public ObjectWrapper()
			{
			}

			public ObjectWrapper(string type, string data)
			{
				this.type = type;
				this.data = data;
			}

			public string type;
			public string data;
		}

		public void save(Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			(new System.Xml.Serialization.XmlSerializer(contents.GetType())).Serialize(memoryStream, this.contents);
			(new System.Xml.Serialization.XmlSerializer(typeof(ObjectWrapper))).Serialize(stream, new ObjectWrapper(
				contents.GetType().FullName, System.Text.Encoding.ASCII.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length)));
		}

		public void load(System.IO.Stream stream)
		{
			ObjectWrapper objectWrapper = 
				(ObjectWrapper) (new System.Xml.Serialization.XmlSerializer(typeof(ObjectWrapper))).Deserialize(stream);
			this.contents = (new System.Xml.Serialization.XmlSerializer(System.Type.GetType(objectWrapper.type))).Deserialize(
				new MemoryStream(System.Text.Encoding.ASCII.GetBytes(objectWrapper.data)));
		}

/*
		private static int sizeOfUInt32 = System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));

		private static void object2Stream(System.Type type, object obj, Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			(new System.Xml.Serialization.XmlSerializer(type)).Serialize(memoryStream, obj);
			stream.Write(BitConverter.GetBytes((uint) memoryStream.Length), 0, sizeOfUInt32);
			stream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
		}

		private static object stream2Object(System.Type type, Stream stream)
		{
			byte[] buffer = new byte[sizeOfUInt32];
			stream.Read(buffer, 0, sizeOfUInt32);
			uint numberOfBytes = BitConverter.ToUInt32(buffer, 0);
			buffer = new byte[numberOfBytes];
			stream.Read(buffer, 0, (int) numberOfBytes);
			return (new System.Xml.Serialization.XmlSerializer(type)).Deserialize(new MemoryStream(buffer));
		}

		public void save(Stream stream)
		{
			object2Stream(typeof(string), contents.GetType().FullName, stream);
			object2Stream(contents.GetType(), contents, stream);
		}

		public void load(System.IO.Stream stream)
		{
			System.Type type = System.Type.GetType((string) stream2Object(typeof(string), stream));
			this.contents = stream2Object(type, stream);
		}
*/

		#endregion
	}
}
