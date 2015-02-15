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
    [XmlType("Method")]
    public sealed class Method
    {
        #region Constructor

        public Method(MethodCategory _methodtype, string _name, List<Parameter> _parameters, ValueType _resulttype)
        {
            this._methodtype = _methodtype;
            this._name = _name;
            this._parameters = _parameters;
            this._resulttype = _resulttype;
        }

        public Method()
        {
        }

        #endregion

        #region Fields

        private MethodCategory _methodtype;
        private string _name;
        private List<Parameter> _parameters;
        private ValueType _resulttype;

        #endregion

        #region Accessors

        [XmlAttribute("MethodCategory")]
        public MethodCategory MethodCategory
        {
            get { return _methodtype; }
            set { _methodtype = value; }
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement("Parameter")]
        public Parameter[] Parameters
        {
            get { return (_parameters != null) ? _parameters.ToArray() : null; }
            set { _parameters = (value != null) ? new List<Parameter>(value) : new List<Parameter>(); }
        }

/*
        [XmlIgnore]
        public List<Parameter> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
*/

        [XmlElement("ResultType")]
        public ValueType ResultType
        {
            get { return _resulttype; }
            set { _resulttype = value; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Method)
            {
                Method other = (Method)obj;
                return _methodtype.Equals(other._methodtype)
                    && _name.Equals(other._name)
                    && _parameters.Equals(other._parameters)
                    && _resulttype.Equals(other._resulttype);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _methodtype.GetHashCode() ^ _name.GetHashCode() ^ _parameters.GetHashCode() ^ _resulttype.GetHashCode();
        }

        #endregion
*/ 
    }
}
