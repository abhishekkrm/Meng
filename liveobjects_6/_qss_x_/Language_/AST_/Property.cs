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

namespace QS._qss_x_.Language_.AST_
{
    [XmlType("Property")]
    public sealed class Property
    {
        #region Constructors

        public Property(string _name, ValueType _valuetype, List<ValueType> _indexes, Placement _placement, 
            PropertyAttributes _attributes, Expression _initialvalue)
        {
            this._name = _name;
            this._valuetype = _valuetype;
            this._indexes = _indexes;
            this._placement = _placement;
            this._initialvalue = _initialvalue;
            this._attributes = _attributes;
        }

        public Property()
        {
        }

        #endregion

        #region Fields

        private string _name;
        private ValueType _valuetype;
        private List<ValueType> _indexes;
        private Placement _placement;
        private Expression _initialvalue;
        private string _comment;
        private PropertyAttributes _attributes;

        #endregion

        #region Accessors

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Placement")]
        public Placement Placement
        {
            get { return _placement; }
            set { _placement = value; }
        }

        [XmlAttribute("Attributes")]
        public PropertyAttributes Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        [XmlElement("ValueType")]
        public ValueType ValueType
        {
            get { return _valuetype; }
            set { _valuetype = value; }
        }

        [XmlElement("Index")]
        public ValueType[] Indexes
        {
            get { return (_indexes != null) ? _indexes.ToArray() : null; }
            set { _indexes = (value != null) ? new List<ValueType>(value) : new List<ValueType>(); }
        }

        /*
                [XmlIgnore]
                public List<ValueType> Indexes
                {
                    get { return _indexes; }
                    set { _indexes = value; }
                }
        */

        [XmlElement("InitialValue")]
        public Expression InitialValue
        {
            get { return _initialvalue; }
            set { _initialvalue = value; }
        }

        [XmlElement("Comment")]
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Property)
            {
                Property other = (Property)obj;
                return _name.Equals(other._name) && _valuetype.Equals(other._valuetype) && 
                    (((_indexes == null) && (other._indexes == null)) || (_indexes != null) && (other._indexes != null) && 
                    _indexes.Equals(other._indexes)) && _placement.Equals(other._placement) && 
                    (((_initialvalue == null) && (other._initialvalue == null)) || (_initialvalue != null) && (other._initialvalue != null) && 
                    _initialvalue.Equals(other._initialvalue));
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode() ^ _valuetype.GetHashCode() ^ ((_indexes != null) ? _indexes.GetHashCode() : 0) ^ 
                _placement.GetHashCode() ^ ((_initialvalue != null) ? _initialvalue.GetHashCode() : 0);
        }

        #endregion
 */
    }
}
