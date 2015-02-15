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
    [XmlType("AttributedType")]
    public sealed class AttributedType : ValueType
    {
        #region Constructors

        public AttributedType(ValueType _basetype, AttributedTypeCategory _category)
        {
            this._basetype = _basetype;
            this._category = _category;
        }

        public AttributedType()
        {
        }

        #endregion

        #region Fields

        private ValueType _basetype;
        private AttributedTypeCategory _category;

        #endregion

        #region Accessors

        [XmlElement("BaseType")]
        public ValueType BaseType
        {
            get { return this._basetype; }
            set { this._basetype = value; }
        }

        [XmlAttribute("AttributedTypeCategory")]
        public AttributedTypeCategory AttributedTypeCategory
        {
            get { return this._category; }
            set { this._category = value; }
        }

        #endregion

        #region Overrides from ValueType

        public override ValueTypeCategory ValueTypeCategory
        {
            get { return ValueTypeCategory.Attributed; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is AttributedType)
            {
                AttributedType other = (AttributedType)obj;
                return _basetype.Equals(other._basetype) && _category.Equals(other._category);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _basetype.GetHashCode() ^ _category.GetHashCode();
        }

        #endregion
*/ 
    }
}
