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
	/// Aaa...
	/// </summary>
	public class RawQueue : IRawQueue
	{
		public RawQueue()
		{
			firstOne = lastOne = null;
		}

		private ILinkable firstOne, lastOne;

		#region IRawQueue Members

		public uint size()
		{
			uint count = 0;
			for (ILinkable element = firstOne; element != null; element = element.Next)
			{
				count++;
			}

			return count;
		}

		public void enqueue(ILinkable element)
		{
			element.Next = null;
			if (lastOne != null)
				lastOne.Next = element;
			else
				firstOne = element;
			lastOne = element;
		}

		public ILinkable dequeue()
		{
			ILinkable element = firstOne;
			if (element != null)
			{
				firstOne = element.Next;
				if (firstOne == null)
					lastOne = null;
			}
			return element;
		}

		#endregion
	}
}
