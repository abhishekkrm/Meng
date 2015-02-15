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

namespace QS._core_c_.Collections
{
	public class NoSuchKeyException : Exception
	{
		public NoSuchKeyException()
		{
		}
	}

	public interface IDictionaryEntry : QS.Fx.Inspection.IScalarAttribute
	{
		object Key
		{
			get;
		}

		new object Value
		{
			get;
			set;
		}
	}

	/// <summary>
	/// This interface is implemented by all kinds of dictionary structures.
	/// </summary>
	public interface IDictionary : QS.Fx.Inspection.IAttributeCollection
	{
		object[] Keys
		{
			get;
		}

		object[] Values
		{
			get;
		}

		/// <summary>
		/// Find dictionary entry for a given key, returning null if no entry exists.
		/// </summary>
		/// <param name="key">Lookup key.</param>
		/// <returns>The dictionary entry, if found, or null if not found.</returns>
		IDictionaryEntry lookup(object key);

		/// <summary>
		/// Find dictionary entry for a given key, or creates a null mapping if no
		/// such entry exists in the dictionary.
		/// </summary>
		/// <param name="key">Lookup key.</param>
		/// <returns>The dictionary entry, either the one found, or if not found,
		/// a new one with a null mapping.</returns>
		IDictionaryEntry lookupOrCreate(object key);

		/// <summary>
		/// Gets or sets the object associated with a given key. If asking for an
		/// object for a key for which association does not exist, throws exception.
		/// </summary>
		object this[object key]
		{
			get;
			set;
		}

		/// <summary>
		/// Removes and returns the entry for a given key.
		/// </summary>
		/// <param name="key"></param>
		IDictionaryEntry remove(object key);
	}
}
