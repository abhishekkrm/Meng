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

namespace QS._qss_c_.Base1_
{
	[QS.Fx.Serialization.ClassID(ClassID.Message)]
	public class Message : QS._core_c_.Base.IMessage
	{
		public Message()
		{
		}

		public Message(uint destinationLOID, QS._core_c_.Base.IBase1Serializable message)
		{
			this.destinationLOID = destinationLOID;
			this.message = message;
		}

		private uint destinationLOID;
		private QS._core_c_.Base.IBase1Serializable message;

		#region Accessors

		public uint DestinationLOID 
		{
			get { return destinationLOID; }
			set { destinationLOID = value; }
		}

		public QS._core_c_.Base.IBase1Serializable TheMessage
		{
			get { return message; }
			set { message = value; }
		}

		#endregion

		#region ISerializable Members

		ClassID QS._core_c_.Base.IBase1Serializable.ClassIDAsSerializable
		{
			get { return ClassID.Message; }
		}

		void QS._core_c_.Base.IBase1Serializable.save(System.IO.Stream stream)
		{
			byte[] buffer;
			buffer = System.BitConverter.GetBytes(destinationLOID);
			stream.Write(buffer, 0, buffer.Length);
			buffer = System.BitConverter.GetBytes((ushort) message.ClassIDAsSerializable);
			stream.Write(buffer, 0, buffer.Length);
			message.save(stream);
		}

		void QS._core_c_.Base.IBase1Serializable.load(System.IO.Stream stream)
		{
			byte[] buffer = new byte[System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint))];
			stream.Read(buffer, 0, buffer.Length);
			destinationLOID = System.BitConverter.ToUInt32(buffer, 0);
			buffer = new byte[System.Runtime.InteropServices.Marshal.SizeOf(typeof(ushort))];
			stream.Read(buffer, 0, buffer.Length);
			ushort classID = System.BitConverter.ToUInt16(buffer, 0);
			message = Serializer.Get.createObject((ClassID)classID);
			message.load(stream);
		}

		#endregion
	}
}
