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

namespace QS._qss_c_.Collections_1_
{
    public sealed class HashingDictionary<KeyClass,ValueClass> : IDictionary<KeyClass, ValueClass>
    {
        private const int DefaultInitialCapacity = 5;

        public HashingDictionary(IEnumerable<KeyValuePair<KeyClass, ValueClass>> items) : this()
        {
            foreach (KeyValuePair<KeyClass, ValueClass> item in items)
                ((ICollection<KeyValuePair<KeyClass, ValueClass>>) this).Add(item);
        }

        public HashingDictionary(ICollection<KeyValuePair<KeyClass, ValueClass>> items) : this(items.Count)
        {
            foreach (KeyValuePair<KeyClass, ValueClass> item in items)
                ((ICollection<KeyValuePair<KeyClass, ValueClass>>)this).Add(item);
        }

        public HashingDictionary() : this(DefaultInitialCapacity)
        {
        }

        public HashingDictionary(int initialCapacity)
        {
            hashtable_capacity = initialCapacity;
            hashtable = new int[hashtable_capacity];
            for (int ind = 0; ind < hashtable_capacity; ind++)
                hashtable[ind] = -1;

            num_entries = initialCapacity;
            entries = new DictionaryEntry<KeyClass, ValueClass>[num_entries];
            pointers = new int[num_entries];
            for (int ind = 0; ind < num_entries; ind++)
                pointers[ind] = ind + 1;
            pointers[num_entries - 1] = -1;

            first_free = count = 0;
        }

        private int[] hashtable, pointers;
        private int first_free, count, hashtable_capacity, num_entries;
        private DictionaryEntry<KeyClass, ValueClass>[] entries;

        #region Allocating and Deallocating Entries

        private int Allocate()
        {
            if (first_free < 0)
            {
                int old_num_entries = num_entries;
                num_entries = 2 * old_num_entries;

                DictionaryEntry<KeyClass, ValueClass>[] old_entries = entries;
                entries = new DictionaryEntry<KeyClass, ValueClass>[num_entries];
                Array.Copy(old_entries, entries, old_num_entries);

                int[] old_pointers = pointers;
                pointers = new int[num_entries];
                Array.Copy(old_pointers, pointers, old_num_entries);

                for (int ind = old_num_entries; ind < num_entries; ind++)
                    pointers[ind] = ind + 1;
                pointers[num_entries - 1] = -1;

                first_free = old_num_entries;
            }

            int result = first_free;
            first_free = pointers[first_free];
            return result;
        }

        private void Deallocate(int index)
        {
            pointers[index] = first_free;
            first_free = index;
        }

        #endregion

        #region Rehashing

        private void Rehash()
        {
            int old_hashtable_capacity = hashtable_capacity;
            hashtable_capacity = 2 * old_hashtable_capacity;

            int[] old_hashtable = hashtable;
            hashtable = new int[hashtable_capacity];
            for (int hash = 0; hash < hashtable_capacity; hash++)
                hashtable[hash] = -1;

            for (int hash = 0; hash < old_hashtable_capacity; hash++)
            {
                int pointer = old_hashtable[hash];
                while (pointer >= 0)
                {
                    int temp = pointer;
                    pointer = pointers[pointer];

                    int new_hash = (int)(((uint)entries[temp].Key.GetHashCode()) % hashtable_capacity);

                    pointers[temp] = hashtable[new_hash];
                    hashtable[new_hash] = temp;
                }
            }
        }

        #endregion

        #region Adding and Replacing

        private bool TryAdd(DictionaryEntry<KeyClass, ValueClass> entry, bool replace)
        {
            int hash = (int) (((uint)entry.Key.GetHashCode()) % hashtable_capacity);

            if (hashtable[hash] < 0)
            {
                int pointer = Allocate();
                entries[pointer] = entry;
                pointers[pointer] = -1;
                hashtable[hash] = pointer;
            }
            else
            {
                int parent_pointer = -1, pointer = hashtable[hash];
                while (pointer >= 0)
                {
                    if (entries[pointer].Key.Equals(entry.Key))
                    {
                        if (replace)
                        {
                            entries[pointer] = entry;
                            return true;
                        }
                        else
                            return false;
                    }

                    parent_pointer = pointer;
                    pointer = pointers[pointer];
                }

                pointer = Allocate();
                entries[pointer] = entry;
                pointers[pointer] = -1;
                pointers[parent_pointer] = pointer;
            }

            count++;
            if (count > hashtable_capacity)
                Rehash();

            return true;
        }

        #endregion

        #region Searching

        private int Lookup(KeyClass key)
        {
            int hash = (int) (((uint) key.GetHashCode()) % hashtable_capacity);

            int pointer = hashtable[hash];
            while (pointer >= 0)
            {
                if (entries[pointer].Key.Equals(key))
                    return pointer;
                pointer = pointers[pointer];
            }

            return -1;
        }

        #endregion

        #region Removing and Clearing

        private int Remove(KeyClass key)
        {
            int hash = (int)(((uint)key.GetHashCode()) % hashtable_capacity);

            int parent_pointer = -1, pointer = hashtable[hash];
            while (pointer >= 0)
            {
                if (entries[pointer].Key.Equals(key))
                {
                    if (parent_pointer < 0)
                        hashtable[hash] = pointers[pointer];
                    else
                        pointers[parent_pointer] = pointers[pointer];

                    Deallocate(pointer);

                    count--;
                    return pointer;
                }

                parent_pointer = pointer;
                pointer = pointers[pointer];
            }

            return -1;
        }

        private void Clear()
        {
            for (int hash = 0; hash < hashtable_capacity; hash++)
            {
                int pointer = hashtable[hash];
                while (pointer >= 0)
                {
                    int temp = pointer;
                    pointer = pointers[pointer];
                    Deallocate(temp);
                    entries[temp] = new DictionaryEntry<KeyClass, ValueClass>(default(KeyClass), default(ValueClass));
                    count--;
                }

                hashtable[hash] = -1;
            }
        }

        #endregion

        #region IDictionary<KeyClass,ValueClass> Members

        void IDictionary<KeyClass, ValueClass>.Add(KeyClass key, ValueClass value)
        {
            if (!TryAdd(new DictionaryEntry<KeyClass, ValueClass>(key, value), false))
                throw new Exception("Cannot add, entry with the same key already exists in the dictionary.");
        }

        bool IDictionary<KeyClass, ValueClass>.ContainsKey(KeyClass key)
        {
            return Lookup(key) >= 0;
        }

        bool IDictionary<KeyClass, ValueClass>.TryGetValue(KeyClass key, out ValueClass value)
        {
            int pointer = Lookup(key);
            if (pointer < 0)
            {
                value = default(ValueClass);
                return false;
            }
            else
            {
                value = entries[pointer].Value;
                return true;
            }
        }

        ValueClass IDictionary<KeyClass, ValueClass>.this[KeyClass key]
        {
            get 
            {
                int pointer = Lookup(key);
                if (pointer < 0)
                    throw new Exception("No entry with such key exists in the dictionary.");
                return entries[pointer].Value;
            }

            set 
            { 
                TryAdd(new DictionaryEntry<KeyClass,ValueClass>(key, value), true); 
            }
        }

        bool IDictionary<KeyClass, ValueClass>.Remove(KeyClass key)
        {
            int pointer = Remove(key);
            if (pointer < 0)
                return false;
            else
            {
                entries[pointer] = new DictionaryEntry<KeyClass, ValueClass>(default(KeyClass), default(ValueClass));
                return true;
            }
        }

        ICollection<KeyClass> IDictionary<KeyClass, ValueClass>.Keys
        {
            get 
            {
                KeyClass[] keys = new KeyClass[count];
                int index = 0;
                for (int hash = 0; hash < hashtable_capacity; hash++)
                {
                    for (int pointer = hashtable[hash]; pointer >= 0; pointer = pointers[pointer])
                    {
                        if (index < keys.Length)
                            keys[index++] = entries[pointer].Key;
                        else
                            throw new Exception("Error: dictionary changed.");
                    }
                }
                return keys;
            }
        }

        ICollection<ValueClass> IDictionary<KeyClass, ValueClass>.Values
        {
            get
            {
                ValueClass[] values = new ValueClass[count];
                int index = 0;
                for (int hash = 0; hash < hashtable_capacity; hash++)
                {
                    for (int pointer = hashtable[hash]; pointer >= 0; pointer = pointers[pointer])
                    {
                        if (index < values.Length)
                            values[index++] = entries[pointer].Value;
                        else
                            throw new Exception("Error: dictionary changed.");
                    }
                }
                return values;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<KeyClass,ValueClass>> Members

        void ICollection<KeyValuePair<KeyClass, ValueClass>>.Add(KeyValuePair<KeyClass, ValueClass> item)
        {
            if (!TryAdd(new DictionaryEntry<KeyClass, ValueClass>(item.Key, item.Value), false))
                throw new Exception("Cannot add, entry with the same key already exists in the dictionary.");
        }

        bool ICollection<KeyValuePair<KeyClass, ValueClass>>.Contains(KeyValuePair<KeyClass, ValueClass> item)
        {
            int pointer = Lookup(item.Key);
            if (pointer < 0)
                return false;
            else
                return entries[pointer].Value.Equals(item.Value);
        }

        bool ICollection<KeyValuePair<KeyClass, ValueClass>>.IsReadOnly
        {
            get { return false; }
        }

        int ICollection<KeyValuePair<KeyClass, ValueClass>>.Count
        {
            get { return count; }
        }

        bool ICollection<KeyValuePair<KeyClass, ValueClass>>.Remove(KeyValuePair<KeyClass, ValueClass> item)
        {
            throw new NotSupportedException("Currently this functionality is not supported.");
        }

        void ICollection<KeyValuePair<KeyClass, ValueClass>>.Clear()
        {
            Clear();
        }

        void ICollection<KeyValuePair<KeyClass, ValueClass>>.CopyTo(KeyValuePair<KeyClass, ValueClass>[] array, int arrayIndex)
        {
            if (arrayIndex + count > array.Length)
                throw new Exception("Cannot copy, the array supplied is too small to hold all elements.");

            for (int hash = 0; hash < hashtable_capacity; hash++)
            {
                for (int pointer = hashtable[hash]; pointer >= 0; pointer = pointers[pointer])
                {
                    if (arrayIndex < array.Length)
                        array[arrayIndex++] = new KeyValuePair<KeyClass,ValueClass>(entries[pointer].Key, entries[pointer].Value);
                    else
                        throw new Exception("Coult not copy, past the end of the array."); 
                }
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<KeyClass,ValueClass>> Members

        IEnumerator<KeyValuePair<KeyClass, ValueClass>> IEnumerable<KeyValuePair<KeyClass, ValueClass>>.GetEnumerator()
        {
            for (int hash = 0; hash < hashtable_capacity; hash++)
                for (int pointer = hashtable[hash]; pointer >= 0; pointer = pointers[pointer])
                    yield return new KeyValuePair<KeyClass, ValueClass>(entries[pointer].Key, entries[pointer].Value);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<KeyClass, ValueClass>>)this).GetEnumerator();
        }

        #endregion
    }
}
