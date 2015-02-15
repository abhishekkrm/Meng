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

namespace QS._core_c_.Components
{
    [QS.Fx.Serialization.ClassID(ClassID.AttributeSet)]
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(QS._core_e_.Data.DataSeries))]
	[System.Xml.Serialization.XmlInclude(typeof(QS._core_e_.Data.XYSeries))]
    [System.Xml.Serialization.XmlInclude(typeof(Attribute))]
    [System.Xml.Serialization.XmlInclude(typeof(AttributeSet))]
	[System.Xml.Serialization.XmlInclude(typeof(QS.Fx.Inspection.AttributeCollection))]
	[System.Xml.Serialization.XmlInclude(typeof(QS.Fx.Inspection.ScalarAttribute))]
	public class AttributeSet : IAttributeSet, QS.Fx.Inspection.IAttributeCollection, QS.Fx.Serialization.ISerializable
	{
        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.AttributeSet, (ushort)sizeof(int), sizeof(int), 0);
                foreach (QS._core_c_.Collections.IDictionaryEntry element in attributes)
                    info.AddAnother((new Attribute((string) (element.Key), element.Value)).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((int*)(pbuffer + header.Offset)) = ((System.Collections.ICollection)attributes).Count;
            }
            header.consume(sizeof(int));
            foreach (QS._core_c_.Collections.IDictionaryEntry element in attributes)
            {
                QS._core_c_.Components.Attribute a = new QS._core_c_.Components.Attribute((string)(element.Key), element.Value);
                ((QS.Fx.Serialization.ISerializable)a).SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                count = *((int*)(pbuffer + header.Offset));
            }
            header.consume(sizeof(int));
            while (count-- > 0)
            {
                Attribute attribute = new Attribute();
                attribute.DeserializeFrom(ref header, ref data);
                attributes[attribute.Name] = attribute.Value;
            }
        }

        #endregion

        public string AsString
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string name in this.Attributes.Keys)
                {
                    sb.Append("-");
                    sb.Append(name);
                    if (this[name] != null)
                    {
                        sb.Append(":");
                        string ss = this[name].ToString();
                        bool has_spaces = ss.Contains(" ");
                        if (has_spaces)
                            sb.Append("\"");
                        sb.Append(ss);
                        if (has_spaces)
                            sb.Append("\"");
                    }
                    sb.Append(" ");
                }
                return sb.ToString();
            }
        }

        public static explicit operator AttributeSet(string s)
        {
            return new AttributeSet(s);
        }

        public static explicit operator string(AttributeSet s)
        {
            return s.ToString();
        }

		private static AttributeSet none = new AttributeSet(2);

		public static AttributeSet None
		{
			get
			{
				return none;
			}
		}

        public AttributeSet(Attribute[] attributes) : this(attributes.Length)
        {
            foreach (Attribute attribute in attributes)
                this[attribute.Name] = attribute.Value;
        }

		public AttributeSet(System.Collections.Generic.IEnumerable<Attribute> attributes) : this(10)
		{
			AddRange(attributes);
		}

		public AttributeSet(string commandLineString) : this(extractArguments(commandLineString))
		{
		}

		private static string[] extractArguments(string commandLine)
		{
			System.Collections.ArrayList arguments = new System.Collections.ArrayList();
			int starting_pos, current_pos;
			starting_pos = current_pos = 0;
			bool in_parentheses = false;
			while (current_pos < commandLine.Length)
			{
				if (commandLine[current_pos] == ' ' && !in_parentheses)
				{
					if (current_pos - starting_pos > 0)
					{
						arguments.Add(commandLine.Substring(starting_pos, current_pos - starting_pos));
					}

					while (current_pos < commandLine.Length && commandLine[current_pos] == ' ')
						current_pos++;
					starting_pos = current_pos;				
				}
				else
				{
					if (commandLine[current_pos] == '"')
						in_parentheses = !in_parentheses;
					current_pos++;
				}
			}

			if (current_pos - starting_pos > 0)
			{
				arguments.Add(commandLine.Substring(starting_pos, current_pos - starting_pos));
			}

			return (string[]) arguments.ToArray(typeof(string));
		}

		private static char[] leading_chars = new char[] { '-', '/' };
		public AttributeSet(string[] commandLineArguments) : this(commandLineArguments.Length)
		{
			foreach (string arg in commandLineArguments)
			{
				string cleaned = (arg.TrimStart(leading_chars)).Replace("\"", "");
				int colons_ind = cleaned.IndexOf(":");
				if (colons_ind > 0)
					attributes[cleaned.Substring(0, colons_ind)] = cleaned.Substring(colons_ind + 1);
				else
					attributes[cleaned] = null;
			}
		}

		public AttributeSet() : this(2)
		{
		}

		public AttributeSet(string name, object data) : this(2)
		{
			this[name] = data;
		}

		public AttributeSet(int numberOfAttributes)
		{
            if (numberOfAttributes <= 0)
                numberOfAttributes = 2;

            attributes = new QS._core_c_.Collections.Hashtable((uint) numberOfAttributes);
		}

        public AttributeSet(AttributeSet anotherSet)
        {
            this.attributes = anotherSet.attributes.CreateCopy;
        }

        public AttributeSet(IAttributeSet anotherSet) : this(anotherSet.Attributes)
        {
        }

        public AttributeSet(System.Collections.Generic.IEnumerator<Attribute> iteratorOverAttributes) : this(2)
        {
            // iteratorOverAttributes.Reset();
            while (iteratorOverAttributes.MoveNext())
                Add(iteratorOverAttributes.Current);
        }

        public bool contains(string attributeName)
		{
			return attributes.lookup(attributeName) != null;
		}

		public void remove(string attributeName)
		{
			attributes.remove(attributeName);
		}

        [System.Xml.Serialization.XmlIgnore]
        public object this[string attributeName]
		{
			get
			{
				try
				{
					return attributes[attributeName];
				}
				catch (Exception)
				{
					throw new Exception(
						"Attribute \"" + attributeName + "\" has not been defined for this object.");
				}
			}

			set
			{
				attributes[attributeName] = value;
			}
		}
		
		public override string ToString()
		{
			return attributes.AsCompactString;
		}

        // [System.Xml.Serialization.XmlIgnore]
        [System.Xml.Serialization.XmlElement("Attribute")]
        public QS._core_c_.Collections.XmlAssociation[] AttributesAsXmlAssociations
        {
            get { return attributes.XmlAssociations; }
            set { attributes.XmlAssociations = value; }
        }

        [System.Xml.Serialization.XmlIgnore]
		public QS._core_c_.Collections.Hashtable Attributes
		{
			get
			{
				return attributes;
			}

			set
			{
				attributes = value;
			}
		}

		private QS._core_c_.Collections.Hashtable attributes;

		#region IAttributeCollection Members

		[System.Xml.Serialization.XmlIgnore]
		System.Collections.Generic.IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get 
			{
				foreach (string key in attributes.Keys)
					yield return key;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			// nasty, inefficient hack, just for now...
			get { return new QS.Fx.Inspection.ScalarAttribute(attributeName, attributes[attributeName]); }
		}

		#endregion

		#region IAttribute Members

		[System.Xml.Serialization.XmlIgnore]
		string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "Attributes"; }
		}

		[System.Xml.Serialization.XmlIgnore]
		QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion

		public void Add(Attribute attribute)
		{
			this[attribute.Name] = attribute.Value;
		}

		public void AddRange(System.Collections.Generic.IEnumerable<Attribute> attributes)
		{
			foreach (Attribute a in attributes)
				Add(a);
		}

        public void AddRange(System.Collections.Generic.IEnumerator<Attribute> attributes)
        {
            while (attributes.MoveNext())
                Add(attributes.Current);
        }

        System.Collections.Generic.IEnumerator<Attribute> IAttributeSet.Attributes
        {
            get
            {
                if (attributes != null && attributes.Keys != null)
                {
                    foreach (string key in attributes.Keys)
                        yield return new Attribute(key, attributes[key]);
                }
            }
        }

/*
		#region IEnumerable<Attribute> Members

		// [System.Xml.Serialization.XmlIgnore]
		System.Collections.Generic.IEnumerator<Attribute> System.Collections.Generic.IEnumerable<Attribute>.GetEnumerator()
		{
			if (attributes != null && attributes.Keys != null)
			{
				foreach (string key in attributes.Keys)
					yield return new Attribute(key, attributes[key]);
			}
		}

		#endregion
 * 
		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
            yield break;
		}

		#endregion

        // required by XML serialization
        public void Add(System.Object dataObject)
        {
        }
*/
    }
}
