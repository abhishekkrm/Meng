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

namespace QS._qss_c_.Base3_
{
	[QS.Fx.Serialization.ClassID(ClassID.Base3_Bytes)]
	public class Bytes : System.IO.Stream, QS.Fx.Serialization.ISerializable
	{
		public Bytes()
		{
		}

		private WritableArraySegment<byte> data;

		#region Overridden methods of System.Stream

		public override int Read(byte[] buffer, int offset, int count)
		{
			int toRead = count;
			if (data.Count < toRead)
				toRead = data.Count;

			if (toRead > 0)
			{
				Buffer.BlockCopy(data.Array, data.Offset, buffer, offset, toRead);
				data.consume(toRead);
			}

			return toRead;
		}

		public override long Length
		{
			get { return data.Count; }
		}

		public override long Position
		{
			get { throw new System.NotImplementedException(); }
			set { throw new System.NotImplementedException(); }
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
		{
			get { throw new NotSupportedException("This object is not supposed to ever be serialized."); }
		}

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
		{
			throw new NotSupportedException("This object is not supposed to ever be serialized.");
		}

		public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
		{
			int dataSize;
			fixed (byte* arrayptr = header.Array)
			{
				dataSize = *((int*)(arrayptr + header.Offset));
			}
			header.consume(sizeof(int));
			if (dataSize > data.Count)
				throw new ArgumentException("Not enough data to deserialize");

			this.data = new WritableArraySegment<byte>(data.Array, data.Offset, dataSize);
		}

		#endregion
	}
}
