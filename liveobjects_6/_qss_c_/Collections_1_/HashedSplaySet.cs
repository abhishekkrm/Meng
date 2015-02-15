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

namespace QS._qss_c_.Collections_1_
{
	/// <summary>
	/// Summary description for HashedSplay.
	/// </summary>
	public class HashedSplaySet : IRawBinaryTree
	{
		public HashedSplaySet(uint numberOfBuckets)
		{
			this.numberOfBuckets = numberOfBuckets;
			this.buckets = new IRawBinaryTree[numberOfBuckets];
			for (uint ind = 0; ind < numberOfBuckets; ind++)
				buckets[ind] = null;
			numberOfElements = 0;
		}

		private uint numberOfBuckets, numberOfElements;
		private IRawBinaryTree[] buckets;

		#region IRawBinaryTree Members

		public void insert(IBinaryTreeNode node)
		{
			uint code = (uint) node.GetHashCode();
			uint bucketno = code % numberOfBuckets;
			if (buckets[bucketno] == null)
				buckets[bucketno] = new RawSplayTree();
			
			buckets[bucketno].insert(node);			
			numberOfElements++;
		}

		public IBinaryTreeNode lookup(IComparable key)
		{
			uint code = (uint) key.GetHashCode();
			uint bucketno = code % numberOfBuckets;
			if (buckets[bucketno] == null)
				return null;
			else
				return buckets[bucketno].lookup(key);
		}

		public IBinaryTreeNode remove(IComparable key)
		{
			uint bucketno = ((uint) key.GetHashCode()) % numberOfBuckets;
			if (buckets[bucketno] == null)
				return null;
			else
			{
				IBinaryTreeNode result = buckets[bucketno].remove(key);
				numberOfElements--;				
				return result;
			}
		}

		public uint Count
		{
			get
			{
				return numberOfElements;
			}
		}

		#endregion
	}
}
