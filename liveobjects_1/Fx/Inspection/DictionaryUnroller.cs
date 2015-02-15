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
	public static class DictionaryUnroller
	{
		public static bool InstantiatesToDictionary2(System.Type type)
		{
			if (!type.IsGenericType)
				return false;

			Type[] arguments = type.GetGenericArguments();
			if (arguments.Length != 2)
				return false;

			if (!typeof(QS.Fx.Serialization.IStringSerializable).IsAssignableFrom(arguments[0]))
				return false;

			System.Type generic = typeof(System.Collections.Generic.IDictionary<object, object>).GetGenericTypeDefinition();
			System.Type reconstructedType = generic.MakeGenericType(arguments);
			return reconstructedType != null && reconstructedType.IsAssignableFrom(type);
		}

		public static Unroller UnrollToDictionary2(System.Type type, IScalarAttribute attribute)
		{
			Type genericUnroller = typeof(Dictionary2Unroller<QS.Fx.Serialization.IStringSerializable, object>).GetGenericTypeDefinition();
			Type reconstructedClass = genericUnroller.MakeGenericType(type.GetGenericArguments());
			return (Unroller)reconstructedClass.GetConstructor(new Type[] { typeof(IScalarAttribute) }).Invoke(new object[] { attribute });
		}
	}

	public class Dictionary2Unroller<K,C> : Unroller where K : QS.Fx.Serialization.IStringSerializable
	{
		public Dictionary2Unroller(IScalarAttribute attribute) : base(attribute)
		{
		}

		public override IEnumerable<string> AttributeNames
		{
			get
			{
				IDictionary<K, C> dictionary = attribute.Value as IDictionary<K, C>;
				if (dictionary != null)
				{
                    foreach (K key in dictionary.Keys)
                        yield return key.AsString;  
                    // QS.CMS.Base3.Serializer.ToString(key);
				}
				else
					throw new Exception("Cannot unroll: the underlying attribute is not a dictionary.");
			}
		}

		public override IAttribute this[string attributeName]
		{
			get
			{
				IDictionary<K, C> dictionary = attribute.Value as IDictionary<K, C>;
				if (dictionary != null)
				{
                    K key = default(K);
                    if (typeof(K).IsClass)
                    {
                        System.Reflection.ConstructorInfo info = typeof(K).GetConstructor(Type.EmptyTypes);
                        if (info == null)
                            throw new Exception("Cannot instantiate type " + typeof(K).ToString());
                        key = (K) info.Invoke(new object[] { });
                    }
                    key.AsString = attributeName;
                    // (K)QS.CMS.Base3.Serializer.FromString(attributeName);
					return new ScalarAttribute(attributeName, dictionary[key]);
				}
				else
					throw new Exception("Cannot unroll: the underlying attribute is not a dictionary.");
			}
		}
	}
}
