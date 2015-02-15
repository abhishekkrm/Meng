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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for XmlObject.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.Base2_XmlObject)]
	public class XmlWrapper : QS._core_c_.Base2.BlockOfData
	{
		public XmlWrapper()
		{
		}

		public XmlWrapper(object encapsulatedObject) : base(encode(encapsulatedObject))
		{
			this.compressedType = new QS._core_c_.Base2.StringWrapper(encapsulatedObject.GetType().FullName);
		}

		private QS._core_c_.Base2.StringWrapper compressedType;

		private static System.IO.MemoryStream encode(object encapsulatedObject)
		{
			MemoryStream memoryStream = new MemoryStream();
			(new System.Xml.Serialization.XmlSerializer(encapsulatedObject.GetType())).Serialize(
				memoryStream, encapsulatedObject);
			return memoryStream;
		}

		public object Object
		{
			get
			{
                System.Type type = System.Type.GetType(compressedType.String);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
                MemoryStream stream = this.AsStream;
				return serializer.Deserialize(stream);
			}
		}

		public override string ToString()
		{
			return this.Object.ToString();
		}

		#region ISerializable Members

		public override QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.Base2_XmlObject;
			}
		}

		public override uint Size
		{
			get
			{
				return base.Size + compressedType.Size;
			}
		}

		public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			compressedType = new QS._core_c_.Base2.StringWrapper();
			compressedType.load(blockOfData);
			base.load(blockOfData);
		}

		public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			compressedType.save(blockOfData);
			base.save(blockOfData);
		}

		#endregion
	}
}
