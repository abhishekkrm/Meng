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
	public interface ILinkable
	{
		ILinkable Next
		{
			set;
			get;
		}

		object Contents
		{
			get;
		}
	}

	public abstract class GenericLinkable : ILinkable
	{
		public GenericLinkable()
		{
			this.next = null;
		}

		public GenericLinkable(ILinkable next)
		{
			this.next = next;
		}

		private ILinkable next;

		public ILinkable Next
		{
			set
			{
				next = value;
			}

			get
			{
				return next;
			}
		}		
		
		public override bool Equals(object obj)
		{
			return this.Contents.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Contents.GetHashCode();
		}

		public override string ToString()
		{
			return this.Contents.ToString();
		}

		public virtual object Contents
		{
			get
			{
				throw new Exception("This method needs to be overridden!");
			}
		}
	}
}
