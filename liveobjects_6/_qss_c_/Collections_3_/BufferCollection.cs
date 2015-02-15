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

namespace QS._qss_c_.Collections_3_
{
	public class BufferCollection<C> : IBufferCollection<C>
	{
		private const int default_initialSize = 10;
		private const double growth_factor = 2;

		public BufferCollection()
		{
		}

		private C[] buffers;
		private System.Collections.Generic.Queue<int> free = new Queue<int>();

		#region Internal Processing

		private void IncreaseSize()
		{
			if (buffers == null)
			{
				buffers = new C[default_initialSize];
				for (int ind = 0; ind < buffers.Length; ind++)
					free.Enqueue(ind);
			}
			else
			{
				C[] new_buffers = new C[(int)Math.Ceiling(((double)buffers.Length) * growth_factor)];
				Array.Copy(buffers, new_buffers, buffers.Length);
				for (int ind = buffers.Length; ind < new_buffers.Length; ind++)
					free.Enqueue(ind);
				buffers = new_buffers;
			}
		}

		#endregion

		#region IBufferCollection<C> Members

		C[] IBufferCollection<C>.Buffers
		{
			get 
			{
				if (buffers != null)
					return buffers;
				else
					throw new Exception("Collection is empty.");
			}
		}

		int IBufferCollection<C>.Allocate
		{
			get 
			{
				if (free.Count == 0)
					IncreaseSize();
				return free.Dequeue();
			}
		}

		void IBufferCollection<C>.Release(int index)
		{
			free.Enqueue(index);
		}

		int IBufferCollection<C>.Count
		{
			get { return buffers.Length - free.Count; }
		}

		#endregion
	}
}
