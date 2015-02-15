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
    [XmlType("ChildrenValue")]
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
    public sealed class ChildrenValue : Expression
    {
        #region Constructors

        public ChildrenValue(Expression _expression, Operator _operator, AggregationAttributes _attributes)
        {
            this._expression = _expression;
            this._operator = _operator;
            this._attributes = _attributes;
        }

        public ChildrenValue()
        {
        }

        #endregion

        #region Fields

        private Expression _expression;
        private Operator _operator;
        private AggregationAttributes _attributes;

        #endregion

        #region Accessors

        [XmlElement("AggregatedValue")]
        public Expression AggregatedValue
        {
            get { return _expression; }
            set { _expression = value; }
        }

        [XmlElement("AggregationOperator")]
        public Operator AggregationOperator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        [XmlAttribute("AggregationAttributes")]
        public AggregationAttributes AggregationAttributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        #endregion

        #region Overridden from Expression

        public override ExpressionCategory ExpressionCategory
        {
            get { return ExpressionCategory.ChildrenValue; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is ChildrenValue)
            {
                ChildrenValue other = (ChildrenValue)obj;
                return _expression.Equals(other._expression) && _operator.Equals(other._operator) && _attributes.Equals(other._attributes);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _expression.GetHashCode() ^ _operator.GetHashCode() ^ _attributes.GetHashCode();
        }

        #endregion
*/
    }
}
