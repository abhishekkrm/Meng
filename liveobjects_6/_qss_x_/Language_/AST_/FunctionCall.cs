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
    [XmlType("FunctionCall")]
    [XmlInclude(typeof(SpecialValue))]
    [XmlInclude(typeof(Number))]
    [XmlInclude(typeof(VariableValue))]
    [XmlInclude(typeof(PropertyValue))]
    [XmlInclude(typeof(ParentValue))]
    [XmlInclude(typeof(GroupValue))]
    [XmlInclude(typeof(ChildrenValue))]
    [XmlInclude(typeof(FunctionCall))]
    [XmlInclude(typeof(Operation))]
    [XmlInclude(typeof(Set))]
    [XmlInclude(typeof(Boolean))]
    public sealed class FunctionCall : Expression
    {
        #region Constructors

        public FunctionCall(string _method, List<Expression> _arguments)
        {
            this._method = _method;
            this._arguments = _arguments;
        }

        public FunctionCall()
        {
        }

        #endregion

        #region Fields

        private string _method;
        private List<Expression> _arguments;

        #endregion

        #region Accessors

        [XmlAttribute("Method")]
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        [XmlElement("Argument")]
        public Expression[] Arguments
        {
            get { return (_arguments != null) ? _arguments.ToArray() : null; }
            set { _arguments = (value != null) ? new List<Expression>(value) : new List<Expression>(); }
        }

/*
        [XmlIgnore]
        public List<Expression> Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }
*/

        #endregion

        #region Overridden from Expression

        public override ExpressionCategory ExpressionCategory
        {
            get { return ExpressionCategory.FunctionCall; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is FunctionCall)
            {
                FunctionCall other = (FunctionCall) obj;
                return _method.Equals(other._method) && _arguments.Equals(other._arguments);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _method.GetHashCode() ^ _arguments.GetHashCode();
        }

        #endregion
*/
    }
}
