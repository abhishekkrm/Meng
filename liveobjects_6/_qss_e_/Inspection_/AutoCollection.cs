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

namespace QS._qss_e_.Inspection_
{
	public class AutoCollection<K,C> 
		: QS.Fx.Inspection.IAttributeCollection where K : QS.Fx.Serialization.IStringSerializable where C : class, new()
	{
		public AutoCollection()
		{
		}

		private System.Collections.Generic.IDictionary<K, C> my_elements = null;

		#region Dictionary Operations

		public C this[K key]
		{
			get
			{
				lock (this)
				{
					System.Collections.Generic.IDictionary<K, C> elements = this.Elements;
					if (elements.ContainsKey(key))
						return elements[key];
					else
					{
						C element = new C();
						elements[key] = element;
						return element;
					}
				}
			}
		}

		private System.Collections.Generic.IDictionary<K, C> Elements
		{
			get
			{
				if (my_elements == null)
					my_elements = new System.Collections.Generic.Dictionary<K, C>();
				return my_elements;
			}
		}

		#endregion

		#region IAttributeCollection Members

		IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get 
			{
				System.Collections.Generic.List<string> names = new List<string>();
				lock (this)
				{
					foreach (K key in this.Elements.Keys)
						names.Add(QS._core_c_.Base3.Serializer.ToString(key));
				}
				return names;
			}
		}

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get 
			{ 
				K key = (K) QS._core_c_.Base3.Serializer.FromString(attributeName);
                return new QS.Fx.Inspection.ScalarAttribute(key.AsString, this.Elements[key]);
			}
		}

		#endregion

		#region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "Collection of " + typeof(C).FullName; }
		}

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion
	}
}
