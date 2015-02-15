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

// #define DO_LOCKING
// #define DO_STATISTICS

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Memory_
{
	public class ObjectPool<C> : IAllocator<C> where C : class, new()
	{
		public const int DefaultAllocationBatchSize = 20;

		public ObjectPool() : this(DefaultAllocationBatchSize)
		{
		}

		public ObjectPool(int allocationBatchSize)
		{
			this.allocationBatchSize = allocationBatchSize;
		}

		private System.Collections.Generic.Queue<C> freeQueue = new Queue<C>();
		private int allocationBatchSize;
		
#if DO_STATISTICS
		private int allocatedCount = 0;

		public int AllocatedCount
		{
			get { return allocatedCount; }
		}

		public int AvailableCount
		{
			get { return freeQueue.Count; }
		}
#endif

		#region IAllocator<C> Members

		C IAllocator<C>.Allocate
		{
			get 
			{
#if DO_LOCKING
				lock (this)
				{
#endif

					if (freeQueue.Count > 0)
						return freeQueue.Dequeue();
					else
					{
#if DO_STATISTICS
						allocatedCount += allocationBatchSize;
#endif
						for (int ind = 1; ind < allocationBatchSize; ind++)
							freeQueue.Enqueue(new C());
						return new C();
					}

#if DO_LOCKING
				}				
#endif
			}
		}

		void IAllocator<C>.Release(C element)
		{
#if DO_LOCKING
			lock (this)
			{
#endif

				freeQueue.Enqueue(element);

#if DO_LOCKING
			}
#endif
		}

		#endregion
	}
}
