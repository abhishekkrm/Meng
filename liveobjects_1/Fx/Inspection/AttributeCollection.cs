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

namespace QS.Fx.Inspection
{
	[Serializable]
	public class AttributeCollection : IAttributeCollection
	{
		#region EmptyCollection

		[NonSerialized]
		private static IAttributeCollection noAttributes = new EmptyCollection();

		[System.Xml.Serialization.XmlIgnore]
		public static IAttributeCollection NoAttributes
		{
			get { return noAttributes; }
		}

		[Serializable]
		private class EmptyCollection : IAttributeCollection
		{
			public EmptyCollection()
			{
			}

			#region IAttributeCollection Members

			IEnumerable<string> IAttributeCollection.AttributeNames
			{
				get { return null; }
			}

			IAttribute IAttributeCollection.this[string attributeName]
			{
				get { throw new IndexOutOfRangeException(); }
			}

			#endregion

			#region IAttribute Members

			string IAttribute.Name
			{
				get { return "NoAttributes"; }
			}

			AttributeClass IAttribute.AttributeClass
			{
				get { return AttributeClass.COLLECTION; }
			}

			#endregion
		}

		#endregion

		public AttributeCollection() : this("unnamed")
		{
		}

		public AttributeCollection(string name)
		{
			this.name = name;
		}

		protected string name;
		protected System.Collections.Generic.IDictionary<string, IAttribute> attributes = new System.Collections.Generic.Dictionary<string, IAttribute>();

		public void Clear()
		{
			attributes.Clear();
		}

		public void Add(IAttribute attribute)
		{
			attributes.Add(attribute.Name, attribute);
		}

		#region IAttribute Members

		public string Name
		{
			get { return name; }
		}

		public AttributeClass AttributeClass
		{
			get { return AttributeClass.COLLECTION; }
		}

		#endregion

		#region IEnumerable<IAttribute> Members

		[System.Xml.Serialization.XmlIgnore]
		public System.Collections.Generic.IEnumerable<string> AttributeNames
		{
			get { return attributes.Keys; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public IAttribute this[string attributeName]
		{
			get { return attributes[attributeName]; }
		}

		#endregion
	}
}
