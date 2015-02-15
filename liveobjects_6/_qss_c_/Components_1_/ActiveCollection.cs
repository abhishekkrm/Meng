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

namespace QS._qss_c_.Components_1_
{
	public class ActiveCollection<C> : IActiveCollection<C>
	{
		public ActiveCollection()
		{
		}

		private CollectionChanged<C> onChange;

		#region IActiveCollection<C> Members

		event CollectionChanged<C> IActiveCollection<C>.OnChange
		{
			add 
			{
				onChange += value;
			}

			remove 
			{
				onChange -= value;
			}
		}

		#endregion

		#region IEnumerable<C> Members

		IEnumerator<C> IEnumerable<C>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICollection<C> Members

		void ICollection<C>.Add(C item)
		{
			throw new NotImplementedException();
		}

		void ICollection<C>.Clear()
		{
			throw new NotImplementedException();
		}

		bool ICollection<C>.Contains(C item)
		{
			throw new NotImplementedException();
		}

		void ICollection<C>.CopyTo(C[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		int ICollection<C>.Count
		{
			get { throw new System.NotImplementedException(); }
		}

		bool ICollection<C>.IsReadOnly
		{
			get { throw new System.NotImplementedException(); }
		}

		bool ICollection<C>.Remove(C item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
