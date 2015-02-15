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
using System.Diagnostics;

namespace QS._qss_c_.Collections_1_
{
	/// <summary>
	/// Summary description for RawSplayTree.
	/// </summary>
	public class RawSplayTree : IRawBinaryTree
	{
		public RawSplayTree()
		{
		}

		private IBinaryTreeNode root = null;
		private uint numberOfNodes = 0;

		public IBinaryTreeNode Root
		{
			get
			{
				return root;
			}
		}

        public System.Collections.Generic.IList<IBinaryTreeNode> SortedListOfNodes            
        {
            get
            {
                System.Collections.Generic.IList<IBinaryTreeNode> listOfNodes =
                    new System.Collections.Generic.List<IBinaryTreeNode>((int) this.Count);
                this.ListNodes(ref listOfNodes, this.root);
                return listOfNodes;
            }
        }

        private void ListNodes(ref System.Collections.Generic.IList<IBinaryTreeNode> listOfNodes, IBinaryTreeNode subtreeRoot)
        {
            if (subtreeRoot != null)
            {
                this.ListNodes(ref listOfNodes, subtreeRoot.LChildNode);
                listOfNodes.Add(subtreeRoot);
                this.ListNodes(ref listOfNodes, subtreeRoot.RChildNode);
            }
        }

		#region Rotate and Splay Operations 

		private static void splay(IBinaryTreeNode node)
		{
			while (node.ParentNode != null)
			{
				if (node.ParentNode.ParentNode != null)
				{
					bool b1 = node.ParentNode.LChildNode == node;
					bool b2 = node.ParentNode.ParentNode.LChildNode == node.ParentNode;

					rotate(((b1 && !b2) || (!b1 && b2)) ? node : node.ParentNode);
				}

				rotate(node);
			}
		}

		private static void rotate(IBinaryTreeNode node)
		{
			if (node.ParentNode == null)
				throw new Exception("Cannot rotate at this node becase it has no parent.");

			IBinaryTreeNode parentNode = node.ParentNode;
			IBinaryTreeNode grandparentNode = parentNode.ParentNode;

			if (node == parentNode.LChildNode)
			{
				parentNode.LChildNode = node.RChildNode;
				if (node.RChildNode != null)
					node.RChildNode.ParentNode = parentNode;
				node.RChildNode = parentNode;
			}
			else
			{
				parentNode.RChildNode = node.LChildNode;
				if (node.LChildNode != null)
					node.LChildNode.ParentNode = parentNode;
				node.LChildNode = parentNode;
			}
			parentNode.ParentNode = node;
			node.ParentNode = grandparentNode;

			if (grandparentNode != null)
			{
				if (parentNode == grandparentNode.LChildNode)
					grandparentNode.LChildNode = node;
				else
					grandparentNode.RChildNode = node;
			}
		}

		#endregion

		#region Binary Search

		private static bool binarySearch(IBinaryTreeNode root, object key, out IBinaryTreeNode foundNode)
		{
			if (root != null)
				return searchWithin(root, key, out foundNode);
			else
			{
				foundNode = null;
				return false;
			}
		}

		private static bool searchWithin(IBinaryTreeNode root, object key, out IBinaryTreeNode foundNode)
		{
			int comparison = root.Contents.CompareTo(key);

			if (comparison == 0)
			{
				foundNode = root;
				return true;
			}
			else
			{
				IBinaryTreeNode childNode = (comparison < 0) ? root.RChildNode : root.LChildNode;

				if (childNode != null)
					return searchWithin(childNode, key, out foundNode);
				else
				{
					foundNode = root;
					return false;
				}
			}
		}

		#endregion

		#region Merging Subtrees

		private static IBinaryTreeNode findmax(IBinaryTreeNode root)
		{
			IBinaryTreeNode maxnode = root;
			while (maxnode.RChildNode != null)
				maxnode = maxnode.RChildNode;
			return maxnode;
		}

		private static IBinaryTreeNode internalJoin(IBinaryTreeNode lnode, IBinaryTreeNode rnode)
		{
			IBinaryTreeNode lmaxnode = findmax(lnode);
			splay(lmaxnode);

			Debug.Assert(lmaxnode.RChildNode == null);

			lmaxnode.RChildNode = rnode;
			rnode.ParentNode = lmaxnode;

			return lmaxnode;
		}

		#endregion

		#region Elementary Operations

		public IBinaryTreeNode lookup(System.IComparable key)
		{
			IBinaryTreeNode result;
			bool found = binarySearch(root, key, out result);

			if (result != null)
			{
				splay(result);
				root = result;
			}
			
			return found ? result : null;
		}

		public void insert(IBinaryTreeNode node)
		{
			if (root != null)
			{
				IBinaryTreeNode parent;
				if (binarySearch(root, node.Contents, out parent))
					throw new Exception("Node is already in the tree");
				
				if (parent.Contents.CompareTo(node.Contents) > 0)
				{
					Debug.Assert(parent.LChildNode == null);
					parent.LChildNode = node;				
				}
				else
				{
					Debug.Assert(parent.RChildNode == null);
					parent.RChildNode = node;
				}
				
				node.ParentNode = parent;
				node.LChildNode = node.RChildNode = null;

				splay(node);
			}
			else
			{
				node.LChildNode = node.RChildNode = node.ParentNode = null;
			}

			numberOfNodes++;

			root = node;
		}

		public IBinaryTreeNode remove(System.IComparable key)
		{
			IBinaryTreeNode node;
			if (binarySearch(root, key, out node))
			{
				splay(node);

				if (node.LChildNode != null)
				{
					node.LChildNode.ParentNode = null;

					if (node.RChildNode != null)
					{
						node.RChildNode.ParentNode = null;
						root = internalJoin(node.LChildNode, node.RChildNode);
					}
					else
						root = node.LChildNode;
				}
				else
				{
					if (node.RChildNode != null)
					{
						node.RChildNode.ParentNode = null;
						root = node.RChildNode;
					}
					else
						root = null;
				}

				numberOfNodes--;
				
				node.LChildNode = null;
				node.RChildNode = null;				

				return node;
			}
			else
				return null;
		}

		public uint Count
		{
			get
			{
				return numberOfNodes;
			}
		}

		#endregion

		#region Some stuff for testing purposes...

		/*
				public static IBinaryTreeNode randomTree(double height)
				{
					uint nodeNo = 1;
					Node node = new Node(nodeNo++);
					node.ParentNode = null;
					propagate(node, ref nodeNo, 1.0 - 1.0 / height);
					return node;
				}

				private static System.Random random = new System.Random();
				private static void propagate(IBinaryTreeNode node, ref uint nodeNo, double allowance)
				{
					uint nnodes = 0;

					if (random.NextDouble() < allowance)
					{
						node.LChildNode = new Node(nodeNo++);
						node.LChildNode.ParentNode = node;
						nnodes++;
					}
			
					if (random.NextDouble() < allowance)
					{
						node.RChildNode = new Node(nodeNo++);
						node.RChildNode.ParentNode = node;
						nnodes++;
					}

					if (node.LChildNode != null)
						propagate(node.LChildNode, ref nodeNo, allowance / nnodes);

					if (node.RChildNode != null)
						propagate(node.RChildNode, ref nodeNo, allowance / nnodes);
				}
		*/ 

		#endregion
	}
}
