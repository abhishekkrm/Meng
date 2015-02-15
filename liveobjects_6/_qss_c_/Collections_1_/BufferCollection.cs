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

namespace QS._qss_c_.Collections_1_
{
	public interface IBufferCollection<C>
	{
		C[] Buffers
		{
			get;
		}

		int Allocate
		{
			get;
		}

		void Release(int index);
	}

	public class BufferCollection<C>
	{
		private const int default_initialCapacity = 5;

		public BufferCollection() : this(default_initialCapacity)
		{
		}

		public BufferCollection(int initialCapacity)
		{
			this.capacity = initialCapacity;
			buffers = new C[capacity];
			links = new int[capacity];
			for (int ind = 0; ind < (capacity - 1); ind++)
				links[ind] = ind + 1;
			links[capacity - 1] = -1;
			firstFree = 0;
		}

		private int capacity, firstFree;
		private C[] buffers;
		private int[] links;

		private void increaseCapacity()
		{
			int new_capacity = capacity * 2;
			C[] new_buffers = new C[new_capacity];
			int[] new_links = new int[new_capacity];
			for (int ind = 0; ind < capacity; ind++)
			{
				new_buffers[ind] = buffers[ind];
				new_links[ind] = links[ind];
			}
			buffers = new_buffers;
			links = new_links;
			for (int ind = capacity; ind < (new_capacity - 1); ind++)
				links[ind] = ind + 1;
			links[new_capacity - 1] = firstFree;
			firstFree = capacity;
		}

		public C[] Buffers
		{
			get { return buffers; }
		}

		public int Allocate
		{
			get 
			{
				if (firstFree < 0)
					increaseCapacity();
				int result = firstFree;
				firstFree = links[firstFree];
				return result;
			}
		}

		public void Release(int index)
		{
			links[index] = firstFree;
			firstFree = index;
		}
	}
}
