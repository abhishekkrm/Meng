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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Base3_
{
	public class AutomaticCollection<K,C> : IAutomaticCollection<K, C>
	{
		public AutomaticCollection() // : this(null)
		{
		}

		// public AutomaticCollection(object context) : this(context, typeof(C))
		// {
		// }

		public AutomaticCollection(object context, System.Type elementClass)
		{
			if (context == null)
				context = this;
			System.Reflection.ConstructorInfo constructorInfo =
				elementClass.GetConstructor(new Type[] { context.GetType(), typeof(K) });
			if (constructorInfo == null)
				throw new Exception("Class " + elementClass.Name + " does not have a constructor with signature (" +
					context.GetType().Name + ", " + typeof(K).Name + ").");

			constructorCallback = new ConstructorCallback(delegate(K key)
			{
				return (C)constructorInfo.Invoke(new object[] { context, key });
			});			
		}

		public delegate C ConstructorCallback(K key);

		public AutomaticCollection(ConstructorCallback constructorCallback)
		{
			this.constructorCallback = constructorCallback;
		}

		protected System.Collections.Generic.IDictionary<K, C> collection = new System.Collections.Generic.Dictionary<K, C>();
		protected ConstructorCallback constructorCallback;

		public ConstructorCallback Callback
		{
			get { return constructorCallback; }
			set { constructorCallback = value; }
		}

		#region IAutomaticCollection<NetworkAddress,C> Members

		C QS._qss_c_.Base3_.IAutomaticCollection<K, C>.this[K key]
		{
			get 
			{
				lock (this)
				{
					if (collection.ContainsKey(key))
						return collection[key];
					else
					{
						C element = constructorCallback(key);
						collection.Add(key, element);
						return element;
					}
				}
			}
		}

		#endregion
	}
}
