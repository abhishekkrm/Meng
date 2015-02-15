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

using System.Threading;

namespace QS._qss_c_.Base3_
{
	public class LogOf<C> 
		: QS.Fx.Inspection.Inspectable, ILogOf<C>, System.IDisposable where C : System.IDisposable
	{
		public LogOf()
		{		
		}

		private Collections_3_.IBufferCollection<C> bufferCollection = new Collections_3_.BufferCollection<C>();
		private List<int> buffers = new List<int>();

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			lock (this)
			{
				foreach (int ind in buffers)		
					s.AppendLine(bufferCollection.Buffers[ind].ToString());
			}
			return s.ToString();
		}

		[QS.Fx.Base.Inspectable]
		public string AsString
		{
			get { return ToString(); }
		}

		#region ILogOf<C> Members

		void ILogOf<C>.Add(C element)
		{
			lock (this)
			{
				int ind = bufferCollection.Allocate;
				buffers.Add(ind);
				bufferCollection.Buffers[ind] = element;
			}
		}

		void ILogOf<C>.Clear()
		{
			lock (this)
			{
				foreach (int ind in buffers)
				{
					bufferCollection.Buffers[ind].Dispose();
					bufferCollection.Release(ind);
				}
				buffers.Clear();
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region System.Collections.Generic.IEnumerable<C> Members

		IEnumerator<C> System.Collections.Generic.IEnumerable<C>.GetEnumerator()
		{
			lock (this)
			{
				foreach (int ind in buffers)
				{
					C element = bufferCollection.Buffers[ind];
					
					Monitor.Exit(this);
					yield return element;
					Monitor.Enter(this);
				}
			}
		}

		#endregion

		#region System.IDisposable Members

		void System.IDisposable.Dispose()
		{
			((ILogOf<C>) this).Clear();
			buffers = null;
			bufferCollection = null;
		}

		#endregion
	}
}
