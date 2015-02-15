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
	/// Summary description for IBinaryTreeNode.
	/// </summary>
	public interface IBinaryTreeNode
	{
		IBinaryTreeNode ParentNode
		{
			set;
			get;
		}

		IBinaryTreeNode LChildNode
		{
			set;
			get;
		}

		IBinaryTreeNode RChildNode
		{
			set;
			get;
		}

		System.IComparable Contents
		{
			get;
		}
	}

	public abstract class GenericBinaryTreeNode : IBinaryTreeNode, System.IComparable
	{
		public GenericBinaryTreeNode()
		{
		}

		private IBinaryTreeNode parentNode, leftChildNode, rightChildNode;

		public override int GetHashCode()
		{
			return Contents.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Contents.Equals(obj);
		}

		public override string ToString()
		{
			return Contents.ToString();
		}

		#region IBinaryTreeNode Members

		public IBinaryTreeNode ParentNode
		{
			get
			{
				return parentNode;
			}
			set
			{
				parentNode = value;
			}
		}

		public IBinaryTreeNode LChildNode
		{
			get
			{
				return leftChildNode;
			}
			set
			{
				leftChildNode = value;
			}
		}

		public IBinaryTreeNode RChildNode
		{
			get
			{
				return rightChildNode;
			}
			set
			{
				rightChildNode = value;
			}
		}

		public abstract IComparable Contents
		{
			get;
		}

		#endregion

		#region IComparable Members

		public virtual int CompareTo(object obj)
		{
			return Contents.CompareTo(obj);
		}

		#endregion
	}
}
