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
	/// Summary description for AVLTree.
	/// </summary>
	public class SplayTree : IOrderedSet
	{
		public SplayTree()
		{
			this.underlyingTree = new RawSplayTree();
		}

		private RawSplayTree underlyingTree;

		public RawSplayTree UnderlyingTree
		{
			get
			{
				return underlyingTree;
			}
		}

		#region Class Node

		private class Node : GenericBinaryTreeNode
		{
			public Node(System.IComparable contents) : base()
			{
				this.contents = contents;
			}

			private System.IComparable contents;

			public override System.IComparable Contents
			{
				get
				{
					return contents;
				}
			}

			public override int CompareTo(object obj)
			{
				return contents.CompareTo(obj);
			}
		}

		#endregion

		#region IOrderedSet Members

		public void insert(System.IComparable element)
		{
			if (element == null)
				throw new Exception("Cannot insert a null element into a splay tree.");

			underlyingTree.insert(new Node(element));
		}

		public System.IComparable lookup(System.IComparable lookupKey)
		{
			IBinaryTreeNode node = underlyingTree.lookup(lookupKey);
			return (node != null) ? node.Contents : null;
		}

		public System.IComparable remove(System.IComparable lookupKey)
		{
			IBinaryTreeNode node = underlyingTree.remove(lookupKey);
			return (node != null) ? node.Contents : null;
		}

		public bool contains(System.IComparable lookupKey)
		{
			return underlyingTree.lookup(lookupKey) != null;
		}

		public uint Count
		{
			get
			{
				return underlyingTree.Count;
			}
		}

		#endregion
	}
}
