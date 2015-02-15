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
	public class DictionaryWrapper : QS.Fx.Inspection.IAttributeCollection
	{
		public delegate object ConversionCallback(string s);

		public DictionaryWrapper(string name, QS._core_c_.Collections.IDictionary dictionary, ConversionCallback callback)
		{
			this.name = name;
			this.dictionary = dictionary;
			this.callback = callback;
		}

		private string name;
		private QS._core_c_.Collections.IDictionary dictionary;
		private ConversionCallback callback;

		#region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get { return dictionary.AttributeNames; }
		}

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get 
			{
				object key = callback(attributeName);
				QS._core_c_.Collections.IDictionaryEntry entry = dictionary.lookup(key);
				if (entry == null)
					throw new Exception("Cannot find attribute \"" + attributeName + "\" as " + QS._core_c_.Helpers.ToString.ObjectRef(key) + " in this collection.");
                return (QS.Fx.Inspection.IAttribute)entry;
			}
		}

		#endregion

		#region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
		{
			get { return name; }
		}

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion
	}

    public class DictionaryWrapper0<V> : QS.Fx.Inspection.IAttributeCollection
    {
        public DictionaryWrapper0(string name, System.Collections.Generic.IDictionary<string, V> dictionary)
        {
            this.name = name;
            this.dictionary = dictionary;
        }

        private string name;
        private System.Collections.Generic.IDictionary<string, V> dictionary;

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { return dictionary.Keys; }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get
            {
                V _result;
                if (dictionary.TryGetValue(attributeName, out _result))
                    return new QS.Fx.Inspection.ScalarAttribute(attributeName, _result);
                else
                    throw new Exception("Cannot find attribute \"" + attributeName + "\" in this collection.");
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return name; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }

	public class DictionaryWrapper1<K,V> : QS.Fx.Inspection.IAttributeCollection
	{
		public delegate K ConversionCallback(string s);

		public DictionaryWrapper1(string name, System.Collections.Generic.IDictionary<K,V> dictionary, ConversionCallback callback)
		{
			this.name = name;
			this.dictionary = dictionary;
			this.callback = callback;
		}

		private string name;
		private System.Collections.Generic.IDictionary<K, V> dictionary;
		private ConversionCallback callback;

		#region IAttributeCollection Members

		IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get 
			{
				foreach (K key in dictionary.Keys)
					yield return key.ToString();
			}
		}

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get
			{
				K key = callback(attributeName);
				if (dictionary.ContainsKey(key))
                    return new QS.Fx.Inspection.ScalarAttribute(attributeName, dictionary[key]);
				else
					throw new Exception("Cannot find attribute \"" + attributeName + "\" as " + QS._core_c_.Helpers.ToString.ObjectRef(key) + " in this collection.");
			}
		}

		#endregion

		#region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
		{
			get { return name; }
		}

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion
	}

	public class DictionaryWrapper2<K, V>
		: QS.Fx.Inspection.IAttributeCollection where K : QS.Fx.Serialization.IStringSerializable
	{
		public DictionaryWrapper2(string name, System.Collections.Generic.IDictionary<K, V> dictionary)
		{
			this.name = name;
			this.dictionary = dictionary;
		}

		private string name;
		private System.Collections.Generic.IDictionary<K, V> dictionary;

		#region IAttributeCollection Members

		IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get
			{
				foreach (K key in dictionary.Keys)
					yield return QS._core_c_.Base3.Serializer.ToString(key);
			}
		}

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get
			{
				K key = (K)QS._core_c_.Base3.Serializer.FromString(attributeName);
				if (dictionary.ContainsKey(key))
                    return new QS.Fx.Inspection.ScalarAttribute(attributeName, dictionary[key]);
				else
					throw new Exception("Cannot find attribute \"" + attributeName + "\" as " + QS._core_c_.Helpers.ToString.ObjectRef(key) + " in this collection.");
			}
		}

		#endregion

		#region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
		{
			get { return name; }
		}

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion
	}

    public class DictionaryWrapper3<K, V>
        : QS.Fx.Inspection.IAttributeCollection where K : QS.Fx.Serialization.IStringSerializable, new()
    {
        public DictionaryWrapper3(string name, System.Collections.Generic.IDictionary<K, V> dictionary)
        {
            this.name = name;
            this.dictionary = dictionary;
        }

        private string name;
        private System.Collections.Generic.IDictionary<K, V> dictionary;

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get
            {
                foreach (K key in dictionary.Keys)
                    yield return key.AsString;
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get
            {
                K key = new K();
                key.AsString = attributeName;
                if (dictionary.ContainsKey(key))
                    return new QS.Fx.Inspection.ScalarAttribute(attributeName, dictionary[key]);
                else
                    throw new Exception("Cannot find attribute \"" + attributeName + "\" as " + QS._core_c_.Helpers.ToString.ObjectRef(key) + " in this collection.");
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return name; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }

    public delegate string ObjectToStringCallback<C>(C argument);
    public delegate C StringToObjectCallback<C>(string argument);

    public class DictionaryWrapper4<K, V> : QS.Fx.Inspection.IAttributeCollection
    {
        public DictionaryWrapper4(string name, System.Collections.Generic.IDictionary<K, V> dictionary,
            ObjectToStringCallback<K> key2string, StringToObjectCallback<K> string2key)
        {
            this.name = name;
            this.dictionary = dictionary;
            this.key2string = key2string;
            this.string2key = string2key;
        }

        private string name;
        private System.Collections.Generic.IDictionary<K, V> dictionary;
        private ObjectToStringCallback<K> key2string;
        private StringToObjectCallback<K> string2key;

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get
            {
                foreach (K key in dictionary.Keys)
                    yield return key2string(key);
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get
            {
                K key = string2key(attributeName);
                if (dictionary.ContainsKey(key))
                    return new QS.Fx.Inspection.ScalarAttribute(attributeName, dictionary[key]);
                else
                    throw new Exception("Cannot find attribute \"" + attributeName + "\" as " + QS._core_c_.Helpers.ToString.ObjectRef(key) + " in this collection.");
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return name; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
}
