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
using System.Xml.Serialization;

namespace QS._core_e_.Repository
{
    [XmlType("collection")]
    [XmlInclude(typeof(AttributeCollection))]
    [XmlInclude(typeof(ScalarAttribute))]
    [Serializable]
    public class AttributeCollection : Attribute, QS.Fx.Inspection.IAttributeCollection, QS.Fx.Inspection.IAttribute, IAttribute
    {
        public AttributeCollection(IRepository repository, string key, QS.Fx.Inspection.IAttributeCollection attributeCollection) 
            : this(repository, key, attributeCollection.Name)
        {
            foreach (string attributeName in attributeCollection.AttributeNames)
            {
                QS.Fx.Inspection.IAttribute attribute = attributeCollection[attributeName];
                switch (attribute.AttributeClass)
                {
                    case QS.Fx.Inspection.AttributeClass.COLLECTION:
                        attributes.Add(attributeName, new AttributeCollection(repository, key, (QS.Fx.Inspection.IAttributeCollection)attribute));
                        break;

                    case QS.Fx.Inspection.AttributeClass.SCALAR:
                        attributes.Add(attributeName, new ScalarAttribute(repository, key, (QS.Fx.Inspection.IScalarAttribute)attribute));
                        break;
                }
            }
        }

        public AttributeCollection(IRepository repository, string key, string name) : base(repository, key, name)
        {
        }

        public AttributeCollection()
        {
        }

        private Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

        public bool TryGet(string name, out Attribute attribute)
        {
            return attributes.TryGetValue(name, out attribute);
        }

        public void Add(string name, Attribute attribute)
        {
            attributes.Add(name, attribute);
        }

        public void Add(string itemName, string attributeName, object valueObject)
        {
            attributes.Add(itemName, 
                new ScalarAttribute(repository, key, new QS.Fx.Inspection.ScalarAttribute(attributeName, valueObject)));
        }

        public override IRepository Repository
        {
            set
            {
                base.Repository = value;
                foreach (Attribute attribute in attributes.Values)
                    attribute.Repository = value;
            }
        }

        #region Accessors

        [XmlElement("element")]
        public Attribute[] Attributes
        {
            get 
            { 
                Attribute[] result = new Attribute[attributes.Count];
                int ind = 0;
                foreach (Attribute attribute in attributes.Values)
                    result[ind++] = attribute;
                return result; 
            }

            set 
            {
                attributes.Clear();
                if (value != null)
                    foreach (Attribute attribute in value)
                    {
                        attributes.Add(((QS.Fx.Inspection.IAttribute)attribute).Name, attribute);
                        attribute.Key = key;
                    }
            }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { return attributes.Keys; }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { return attributes[attributeName]; }
        }

        #endregion

        #region IAttribute Members

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion

        #region IAttribute Members

        string IAttribute.Ref
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public override string Key
        {
            set
            {
                base.Key = value;
                foreach (Attribute attribute in attributes.Values)
                    attribute.Key = value;
            }
        }
    }
}
