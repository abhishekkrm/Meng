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
	public class CollectionUnroller : Unroller
	{
		private static readonly System.Type genericCollection =
			typeof(System.Collections.Generic.ICollection<object>).GetGenericTypeDefinition();
		public static bool IsGenericCollection(System.Type type)
		{
			if (type.IsGenericType)
			{
				Type[] arguments = type.GetGenericArguments();
				if (arguments.Length != genericCollection.GetGenericArguments().Length)
					return false;
				else
				{
					System.Type basetype = genericCollection.MakeGenericType(arguments);
					return basetype != null && basetype.IsAssignableFrom(type);
				}
			}
			else
				return false;
		}

		private static readonly System.Type genericUnroller = typeof(CollectionUnroller<object>).GetGenericTypeDefinition();
		private static readonly System.Type[] constructorSignature = new System.Type[] { typeof(IScalarAttribute) };
		public static Unroller UnrollGeneric(System.Type type, IScalarAttribute attribute)
		{
			if (IsGenericCollection(type))
			{
				System.Type unrollerClass = genericUnroller.MakeGenericType(type.GetGenericArguments());
				return (Unroller) unrollerClass.GetConstructor(constructorSignature).Invoke(new object[] { attribute });
			}
			else
				throw new Exception("Type " + type.FullName + " is not a generic collection.");
		}

		public CollectionUnroller(IScalarAttribute attribute) : base(attribute)
		{
		}

		public override IEnumerable<string> AttributeNames
		{
			get
			{
				object obj = attribute.Value;
				System.Collections.ICollection collection = obj as System.Collections.ICollection;
				if (collection != null)
				{
					int nelements = collection.Count;
					for (int ind = 0; ind < nelements; ind++)
						yield return ind.ToString();
				}
				else
					throw new Exception("Cannot unroll: the underlying attribute is not a collection.");
			}
		}

		public override IAttribute this[string attributeName]
		{
			get
			{
				object obj = attribute.Value;
				System.Collections.ICollection collection = obj as System.Collections.ICollection;
				if (collection != null)
				{
					int elementno = System.Convert.ToInt32(attributeName);
					foreach (object o in collection)
						if (elementno-- == 0)
							return new ScalarAttribute(attributeName, o);
					
					return null;
				}
				else
					throw new Exception("Cannot unroll: the underlying attribute is not a collection.");
			}
		}
	}

	public class CollectionUnroller<C> : Unroller
	{
		public CollectionUnroller(IScalarAttribute attribute) : base(attribute)
		{
		}

		public override IEnumerable<string> AttributeNames
		{
			get
			{
				object obj = attribute.Value;
				System.Collections.Generic.ICollection<C> collection = obj as System.Collections.Generic.ICollection<C>;
				if (collection != null)
				{
					int nelements = collection.Count;
					for (int ind = 0; ind < nelements; ind++)
						yield return ind.ToString();
				}
				else
					throw new Exception("Cannot unroll: the underlying attribute is not a collection.");
			}
		}

		public override IAttribute this[string attributeName]
		{
			get
			{
				object obj = attribute.Value;
				System.Collections.Generic.ICollection<C> collection = obj as System.Collections.Generic.ICollection<C>;
				if (collection != null)
				{
					int elementno = System.Convert.ToInt32(attributeName);
					foreach (C o in collection)
						if (elementno-- == 0)
							return new ScalarAttribute(attributeName, o);

					return null;
				}
				else
					throw new Exception("Cannot unroll: the underlying attribute is not a collection.");
			}
		}
	}
}
