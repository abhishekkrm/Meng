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
	/// Summary description for HashSet.
	/// </summary>
	public class HashSet : ISet
	{
		public HashSet(uint capacity)
		{
			hashSet = new LinkableHashSet(capacity);
		}

		private class Element : GenericLinkable
		{
			public Element(object element) : base()
			{
				this.element = element;
			}

			public object element;

			public override object Contents
			{
				get
				{
					return element;
				}
			}

			public override string ToString()
			{
				return element.ToString();
			}

			public override bool Equals(object obj)
			{
				return element.Equals(obj);
			}

			public override int GetHashCode()
			{
				return element.GetHashCode();
			}
		}

		private ILinkableHashSet hashSet;

		public override string ToString()
		{
			return hashSet.ToString();
		}

		#region ISet Members

		public bool contains(object lookupKey)
		{
			return hashSet.contains(lookupKey);
		}

		public void insert(object element)
		{
			hashSet.insert(new Element(element));
		}

		public object lookup(object lookupKey)
		{
			Element element = (Element) hashSet.lookup(lookupKey);
			return (element != null) ? element.element : null;
		}

		public object remove(object lookupKey)
		{
			Element element = (Element) hashSet.remove(lookupKey);
			return (element != null) ? element.element : null;
		}

		public uint Count
		{
			get
			{
				return (uint) hashSet.Count;
			}
		}

		public object[] Elements
		{
			get
			{
				ILinkable[] elements = hashSet.Elements;
				object[] result = new object[elements.Length];
				for (uint ind = 0; ind < elements.Length; ind++)
					result[ind] = ((Element) elements[ind]).element;
				return result;
			}
		}

		public System.Array ToArray(System.Type typeOfArrayElements)
		{
			return hashSet.ToArray(typeOfArrayElements);

//			ILinkable[] elements = hashSet.Elements;
//			System.Array resultingArray = System.Array.CreateInstance(typeOfArrayElements, elements.Length);
//			for (uint ind = 0; ind < elements.Length; ind++)
//				resultingArray.SetValue(((Element) elements[ind]).element, ind);
//			return resultingArray;
		}

		#endregion
	}
}
