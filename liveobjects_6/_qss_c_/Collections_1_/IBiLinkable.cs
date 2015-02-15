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
	/// Summary description for IBiLinkable.
	/// </summary>
	public interface IBiLinkable
	{
		IBiLinkable NextBiLinkable
		{
			set;
			get;
		}

		IBiLinkable PrevBiLinkable
		{
			set;
			get;
		}

		object Contents
		{
			get;
		}
	}

	public class GenericBiLinkable : IBiLinkable
	{
		public GenericBiLinkable()
		{
			this.prevBiLinkable = null;
			this.nextBiLinkable = null;
		}

		public GenericBiLinkable(IBiLinkable prev, IBiLinkable next)
		{
			this.prevBiLinkable = prev;
			this.nextBiLinkable = next;
		}

		private IBiLinkable prevBiLinkable, nextBiLinkable;

		public IBiLinkable NextBiLinkable
		{
			set
			{
				nextBiLinkable = value;
			}

			get
			{
				return nextBiLinkable;
			}
		}					

		public IBiLinkable PrevBiLinkable
		{
			set
			{
				prevBiLinkable = value;
			}

			get
			{
				return prevBiLinkable;
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
				return this;
			}
		}	
	}
}
