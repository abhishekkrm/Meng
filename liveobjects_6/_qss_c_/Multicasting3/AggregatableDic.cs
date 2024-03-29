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

namespace QS._qss_c_.Multicasting3
{
	public abstract class AggregatableDic<K,C> : Aggregation3_.IAggregatable, Base3_.IKnownClass 
		where K : QS.Fx.Serialization.ISerializable, new() where C : Aggregation3_.IAggregatable, new()
	{
		public AggregatableDic()
		{
		}

		public AggregatableDic(K key, C element) : this()
		{
			add(key, element);
		}

		private System.Collections.Generic.Dictionary<K, C> dictionary = new Dictionary<K,C>();

		public System.Collections.Generic.IDictionary<K, C> Dictionary
		{
			get { return dictionary; }
		}

		public void add(K key, C data)
		{
			lock (this)
			{
				_add(key, data);
			}
		}

		private void _add(K key, C data)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key].aggregateWith(data);
//				C current_obj = dictionary[key];
//				object aggregated_obj = current_obj.aggregateWith(data);
//				if (aggregated_obj is C)
//					dictionary[key] = (C)aggregated_obj;
//				else
//					throw new Exception("Cast error: aggregate(" + Helpers.ToString.ObjectRef(current_obj) + ", " +
//						Helpers.ToString.ObjectRef(data) + ") returned " + Helpers.ToString.ObjectRef(aggregated_obj));
			}
			else
			{
				dictionary[key] = data;
			}
		}

		#region IAggregatable Members

		void QS._qss_c_.Aggregation3_.IAggregatable.aggregateWith(QS._qss_c_.Aggregation3_.IAggregatable anotherObject)
		{
			lock (this)
			{
				AggregatableDic<K, C> another_dic = anotherObject as AggregatableDic<K, C>;
				if (another_dic == null)
					throw new ArgumentException("Cannot aggregate with this object.");

//				object[] empty_objects = { };
//				AggregatableDic<K, C> result = (AggregatableDic<K, C>) 
//					this.GetType().GetConstructor(Type.EmptyTypes).Invoke(empty_objects);
//				foreach (System.Collections.Generic.KeyValuePair<K, C> entry in dictionary)
//					result._add(entry.Key, entry.Value);

				foreach (System.Collections.Generic.KeyValuePair<K, C> entry in another_dic.Dictionary)
					_add(entry.Key, entry.Value);

//				return result;
			}
		}

		#endregion

		#region ISerializable Members

		QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
		{
			get 
			{
                QS.Fx.Serialization.SerializableInfo info = 
					new QS.Fx.Serialization.SerializableInfo((ushort) this.ClassID, (ushort)sizeof(ushort), sizeof(ushort), 0);
				foreach (System.Collections.Generic.KeyValuePair<K, C> entry in dictionary)
				{
					info.AddAnother(entry.Key.SerializableInfo);
					info.AddAnother(entry.Value.SerializableInfo);
				}
				return info;
			}
		}

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
		{
			fixed (byte* arrayptr = header.Array)
			{
				*((ushort*)(arrayptr + header.Offset)) = (ushort) dictionary.Count;
			}
			header.consume(sizeof(ushort));

			foreach (System.Collections.Generic.KeyValuePair<K, C> entry in dictionary)
			{
				entry.Key.SerializeTo(ref header, ref data);
				entry.Value.SerializeTo(ref header, ref data);
			}
		}

		unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
		{
			ushort count;
			fixed (byte* arrayptr = header.Array)
			{
				count = *((ushort*)(arrayptr + header.Offset));
			}
			header.consume(sizeof(ushort));

			while (count-- > 0)
			{
				K key = new K();
				key.DeserializeFrom(ref header, ref data);
				C element_data = new C();
				element_data.DeserializeFrom(ref header, ref data);

				dictionary.Add(key, element_data);
			}
		}

		#endregion

		#region IKnownClass Members

		public abstract ClassID ClassID
		{
			get;
		}

		#endregion
	}
}
