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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace QS._core_c_.Collections
{
	/// <summary>
	/// This class implements a simple hashtable.
	/// </summary>
	[Serializable]
	public class Hashtable : IDictionary, Base.IBase1Serializable, System.Runtime.Serialization.ISerializable, System.Collections.ICollection
	{
		#region Collections.ISerializable Members

		public ClassID ClassIDAsSerializable
		{
			get
			{
				return ClassID.Collections_Hashtable;
			}
		}

		public void save(Stream memoryStream)
		{
			System.Collections.ICollection collection = this.getAllEntries();
			byte[] buffer = System.BitConverter.GetBytes((uint) capacity);
			memoryStream.Write(buffer, 0, buffer.Length);
			buffer = System.BitConverter.GetBytes((uint) collection.Count);
			memoryStream.Write(buffer, 0, buffer.Length);
			BinaryFormatter formatter = new BinaryFormatter();
			foreach (Element element in collection)
			{
				formatter.Serialize(memoryStream, element.key);
				formatter.Serialize(memoryStream, element.data);
			}
		}

		public void load(Stream memoryStream)
		{
			byte[] buffer = new byte[8];
			memoryStream.Read(buffer, 0, 8);
			capacity = System.BitConverter.ToUInt32(buffer, 0);
			initialize(capacity);
			uint count = System.BitConverter.ToUInt32(buffer, 4);
			BinaryFormatter formatter = new BinaryFormatter();
			for (int ind = 0; ind < count; ind++)
			{
				object key = formatter.Deserialize(memoryStream);
				object data = formatter.Deserialize(memoryStream);
				this[key] = data;
			}
		}

		#endregion

		public Hashtable() : this(10)
		{
		}

		[System.Xml.Serialization.XmlElement("association")]
		public XmlAssociation[] XmlAssociations
		{
			get
			{
				System.Collections.ICollection entries = getAllEntries();
				XmlAssociation[] result = new XmlAssociation[entries.Count];
				int entry_index = 0;
				foreach (Element entry in entries)
					result[entry_index++] = new XmlAssociation(new Base.XmlObject(entry.key), new Base.XmlObject(entry.data));
				return result;
			}

			set
			{
                if (value != null)
                {
                    this.initialize((uint)value.Length);
                    foreach (XmlAssociation assoc in value)
                        this[assoc.Key.Contents] = assoc.Value.Contents;
                }
                else
                    this.initialize(0);
            }
		}

		public Hashtable(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			byte[] bytes = (byte[]) info.GetValue("this", typeof(byte[]));
			MemoryStream memoryStream = new MemoryStream(bytes);
			this.load(memoryStream);
/*
			capacity = info.GetUInt32("capacity");
			initialize(capacity);
			Element[] flattened = (Element []) info.GetValue("elements", typeof(Element[]));
			foreach (Element elem in flattened)
				this[elem.key] = elem.data;
*/				
		}

		#region System.Runtime.Serialization.ISerializable Members

		// Built-in serialization is buggy!
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			MemoryStream memoryStream = new MemoryStream();
			this.save(memoryStream);
			info.AddValue("this", memoryStream.ToArray());
/*
			info.AddValue("capacity", capacity);
			System.Collections.ICollection coll = this.getAllEntries();
			Element[] flattened = new Element[coll.Count];
			int ind = 0;
			foreach (Element elem in coll)
				flattened[ind++] = elem;
			info.AddValue("elements", flattened);
*/			
		}

		#endregion

		private string makeString(bool compact)
		{
			string s = null;
			string separator = compact ? ", " : "\n  ";
			System.Collections.ICollection coll = this.getAllEntries();
			foreach (Element e in coll)
				s = ((s != null) ? (s + separator) : "") + e.ToString();
			return (compact ? "(" : "HashTable\n{\n  ") + ((s != null) ? s : "") + (compact ? ")" : "}\n");			
		}

		public string AsCompactString
		{
			get
			{
				return makeString(true);
			}
		}

		public override string ToString()
		{
			return this.makeString(false);
		}

		public Hashtable(uint capacity)
		{
			initialize(capacity);
		}

		private void initialize(uint capacity)
		{
            if (capacity < 2)
                capacity = 2;

            this.capacity = capacity;
            this.numberOfElements = 0;
            elements = new Element[this.capacity];
			for (int ind = 0; ind < this.capacity; ind++)
				elements[ind] = null;
		}

        [Diagnostics.ComponentContainer]
		[Serializable]
		protected class Element : IDictionaryEntry, QS.Fx.Inspection.IScalarAttribute
		{
			public Element()
			{
			}

			public object key;
            [Diagnostics.Component]
            public object data;

			[NonSerialized] public Element next = null;

			public override string ToString()
			{
				return ((key != null) ? key.ToString() : "null") + " = " + 
					((data != null) ? data.ToString() : "null");
			}

			#region IDictionaryEntry Members

			public object Key
			{
				set
				{
					key = value;
				}

				get
				{
					return key;
				}
			}

			object IDictionaryEntry.Value
			{
				get
				{
					return data;
				}
				set
				{
					data = value;
				}
			}

			#endregion

			#region IScalarAttribute Members

			object QS.Fx.Inspection.IScalarAttribute.Value
			{
				get { return data; }
			}

			#endregion

			#region IAttribute Members

			public string Name
			{
				get { return key.ToString(); }
			}

			public QS.Fx.Inspection.AttributeClass AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
			}

			#endregion
		}

		protected uint capacity, numberOfElements;
		protected Element[] elements;

		protected Element lookupAndOp(
			object key, bool create, bool delete)
		{
			uint index = ((uint) key.GetHashCode()) % capacity;

			// some sanity check, to be removed later
			if (index < 0 || index >= elements.Length)
			{
				throw new Exception("Hashtable corrupted: Capacity = " + capacity.ToString() + 
					", elements.Length = " + elements.Length + ", index = " + index);
			}

			if (elements[index] != null)
			{
				Element oldGrandpa = null;
				for (Element oldElement = elements[index]; 
					oldElement != null; oldElement = oldElement.next)
				{
					if (oldElement.key.Equals(key))
					{
						if (delete)
						{
							if (oldGrandpa != null)
								oldGrandpa.next = oldElement.next;
							else
								elements[index] = oldElement.next;

                            numberOfElements--;
                        }					

						return oldElement;
					}

					oldGrandpa = oldElement;
				}
			}

			if (create && !delete)
			{
				Element oldRoot = elements[index];
				Element newRoot = createNewElement();
				newRoot.next = oldRoot;

				newRoot.key = key;
				newRoot.data = null;

				elements[index] = newRoot;

                numberOfElements++;

                return newRoot;
			}
			else
				return null;
		}

		protected Element createNewElement()
		{
			return new Element();
		}

		private System.Collections.ICollection getAllEntries()
		{
			System.Collections.ArrayList entries = new System.Collections.ArrayList();
			for (int ind = 0; ind < capacity; ind++)
			{
				for (Element first = elements[ind]; first != null; first = first.next)
					entries.Add(first);
			}
			return entries;
		}

		#region IDictionary Members

		public object[] Keys
		{
			get
			{
				System.Collections.ICollection entries = getAllEntries();
				object[] results = new object[entries.Count];
				int entry_index = 0;
				foreach (object entry in entries)
					results[entry_index++] = ((Element) entry).key;
				return results;
			}
		}

		public object[] Values
		{
			get
			{
				System.Collections.ICollection entries = getAllEntries();
				object[] results = new object[entries.Count];
				int entry_index = 0;
				foreach (object entry in entries)
					results[entry_index++] = ((Element) entry).data;
				return results;
			}
		}

		/// <summary>
		/// Find dictionary entry for a given key, returning null if no entry exists.
		/// </summary>
		/// <param name="key">Lookup key.</param>
		/// <returns>The dictionary entry, if found, or null if not found.</returns>
		public IDictionaryEntry lookup(object key)
		{
			return this.lookupAndOp(key, false, false);
		}

		/// <summary>
		/// Find dictionary entry for a given key, or creates a null mapping if no
		/// such entry exists in the dictionary.
		/// </summary>
		/// <param name="key">Lookup key.</param>
		/// <returns>The dictionary entry, either the one found, or if not found,
		/// a new one with a null mapping.</returns>
		public IDictionaryEntry lookupOrCreate(object key)
		{
			return this.lookupAndOp(key, true, false);
		}

		/// <summary>
		/// Gets or sets the object associated with a given key. If asking for an
		/// object for a key for which association does not exist, throws exception.
		/// </summary>
		public object this[object key]
		{
			get
			{
				Element element = this.lookupAndOp(key, false, false);
				if (element == null) 
					throw new NoSuchKeyException();
				
				return element.data;
			}
			set
			{
				Element element = this.lookupAndOp(key, true, false);
				element.data = value;
			}
		}

		/// <summary>
		/// Removes and returns the entry for a given key.
		/// </summary>
		/// <param name="key"></param>
		public IDictionaryEntry remove(object key)
		{
			return this.lookupAndOp(key, false, true);
		}

		#endregion

        #region ICollection Members

        void System.Collections.ICollection.CopyTo(System.Array array, int index)
        {
            foreach (IDictionaryEntry entry in this)
                array.SetValue(entry, index++);
        }

        int System.Collections.ICollection.Count
        {
            get { return (int) numberOfElements; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

/*
        private class Enumerator : System.Collections.IEnumerator
        {
            public Enumerator(Hashtable hashtable)
            {
                this.hashtable = hashtable;
            }

            private Hashtable hashtable;

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { throw new global::System.NotImplementedException(); }
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                throw new NotImplementedException();
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
*/

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
/*
            return new Enumerator(this);
*/
            for (int ind = 0; ind < capacity; ind++)
                for (Element first = elements[ind]; first != null; first = first.next)
                    yield return first;
        }

        #endregion

        public Hashtable CreateCopy
        {
            get
            {
                Hashtable t = new Hashtable(this.numberOfElements);
                foreach (IDictionaryEntry en in this)
                    t[en.Key] = en.Value;
                return t;
            }
		}

		#region IAttributeCollection Members

		System.Collections.Generic.IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get 
			{
				for (int ind = 0; ind < capacity; ind++)
					for (Element first = elements[ind]; first != null; first = first.next)
						yield return first.Name;
			}
		}

		QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get { throw new System.NotSupportedException("Generic hashtables do not support looking up elements by name"); }
		}

		#endregion

		#region IAttribute Members

		string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "Hashtable"; }
		}

		QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion
	}
}
