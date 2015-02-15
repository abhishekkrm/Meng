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

// #define DEBUG_SynchronizedRBT

using System;
using System.Threading;

namespace QS._qss_c_.Collections_2_
{
	/// <summary>
	/// Summary description for SynchronizedSet.
	/// </summary>
	public class SynchronizedRBT : ISynchronizedRBT
	{
//		public SynchronizedRBT(Collections.IRawBinaryTree underlyingBinaryTree) 
//			: this(underlyingBinaryTree, null)
//		{
//		}

		public SynchronizedRBT(Collections_1_.IRawBinaryTree underlyingBinaryTree, QS.Fx.Logging.ILogger logger)
		{
			this.underlyingBinaryTree = underlyingBinaryTree;
			this.logger = logger;
		}

		private QS.Fx.Logging.ILogger logger;
		private Collections_1_.IRawBinaryTree underlyingBinaryTree;

		#region IRawBinaryTree Members

		public void insert(QS._qss_c_.Collections_1_.IBinaryTreeNode node)
		{
			lock (underlyingBinaryTree)
			{
				underlyingBinaryTree.insert(node);
			}
		}

#if DEBUG_SynchronizedRBT
		private uint lastused_lookup_seqno = 0;
#endif

		public QS._qss_c_.Collections_1_.IBinaryTreeNode lookup(IComparable key)
		{
			Collections_1_.IBinaryTreeNode node = null;

#if DEBUG_SynchronizedRBT
			uint lookup_seqno;
			lock (this)
			{
				lookup_seqno = ++lastused_lookup_seqno;
			}

			logger.Log(this, "lookup_enter #" + lookup_seqno.ToString() + " : key = " + 
				key.GetType().Name + "(" + key.ToString() + ")");
#endif

			lock (underlyingBinaryTree)
			{
#if DEBUG_SynchronizedRBT
				logger.Log(this, "finding_node #" + lookup_seqno.ToString() + " : key = " + 
					key.GetType().Name + "(" + key.ToString() + ")");
#endif

				node = underlyingBinaryTree.lookup(key);

				if (node != null)
				{
#if DEBUG_SynchronizedRBT
					logger.Log(this, "locking_node #" + lookup_seqno.ToString() + " : key = " + 
						key.GetType().Name + "(" + key.ToString() + ")");
#endif

					Monitor.Enter(node);
				}
			}

#if DEBUG_SynchronizedRBT
			logger.Log(this, "lookup_leave #" + lookup_seqno.ToString() + " : key = " + 
				key.GetType().Name + "(" + key.ToString() + ")");
#endif

			return node;
		}

		public QS._qss_c_.Collections_1_.IBinaryTreeNode remove(IComparable key)
		{
			Collections_1_.IBinaryTreeNode node = null;
			lock (underlyingBinaryTree)
			{
				node = underlyingBinaryTree.remove(key);
				if (node != null)
					Monitor.Enter(node);
			}
			return node;
		}

		public uint Count
		{
			get
			{
				return underlyingBinaryTree.Count;
			}
		}

		#endregion

		#region ISynchronizedRBT Members

		public Collections_1_.IBinaryTreeNode lookupOrCreate(
			System.IComparable key, CreateBinaryTreeNodeCallback createCallback, out bool createdAnew)
		{
			Collections_1_.IBinaryTreeNode node = null;
			lock (underlyingBinaryTree)
			{
				node = underlyingBinaryTree.lookup(key);				
				if (node == null)
				{
					node = createCallback(key);
					underlyingBinaryTree.insert(node);
					createdAnew = true;
				}
				else
					createdAnew = false;
				Monitor.Enter(node);
			}

			return node;
		}

		#endregion
	}
}
