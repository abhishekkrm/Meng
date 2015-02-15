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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace QS._qss_c_.Base1_
{
	/// <summary>
	/// Represents an arbitrary message that will be serialized and deserialized 
	/// using a binary formatter.
	/// </summary>

	[Serializable]
	public class AnyMessage : QS._core_c_.Base.IMessage
	{
		public static QS._core_c_.Base.CreateSerializable Factory = new QS._core_c_.Base.CreateSerializable(AnyMessage.createSerializable);

		private static QS._core_c_.Base.IBase1Serializable createSerializable()
		{
			return new AnyMessage();
		}

		public AnyMessage()
		{
			this.contents = null;
		}

		public AnyMessage(object contents)
		{
			this.contents = contents;
		}

		public object Contents
		{
			get
			{
				return contents;
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
				return ClassID.AnyMessage;
			}
		}

		public void save(Stream memoryStream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(memoryStream, contents);
		}

		public void load(Stream memoryStream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			contents = formatter.Deserialize(memoryStream);
		}

		#endregion
	}
}
