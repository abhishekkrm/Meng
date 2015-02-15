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
    [XmlType("CustomType")]
    public sealed class CustomType : ValueType
    {
        #region Constructors

        public CustomType(string _typename)
        {
            this._typename = _typename;
        }

        public CustomType()
        {
        }

        #endregion

        #region Fields

        private string _typename;

        #endregion

        #region Accessors

        [XmlAttribute("Name")]
        public string Name
        {
            get { return this._typename; }
            set { this._typename = value; }
        }

        #endregion

        #region Overrides from ValueType

        public override ValueTypeCategory ValueTypeCategory
        {
            get { return ValueTypeCategory.Custom; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is CustomType)
            {
                CustomType other = (CustomType)obj;
                return _typename.Equals(other._typename);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _typename.GetHashCode();
        }

        #endregion
*/ 
    }
}
