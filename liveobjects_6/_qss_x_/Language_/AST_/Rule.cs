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
    [XmlType("Rule")]
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
    [XmlInclude(typeof(UpdatingOperation))]
    [XmlInclude(typeof(Assignment))]
    public sealed class Rule
    {
        #region Constructors

        public Rule(Placement _placement, RuleAttributes _ruleattributes, Update _update)
        {
            this._placement = _placement;
            this._ruleattributes = _ruleattributes;
            this._update = _update;
        }

        public Rule()
        {
        }

        #endregion

        #region Fields

        private Placement _placement;
        private RuleAttributes _ruleattributes;
        private Update _update;
        private string _comment;

        #endregion

        #region Accessors

        public Placement Placement
        {
            get { return _placement; }
            set { _placement = value; }
        }

        public RuleAttributes RuleAttributes
        {
            get { return _ruleattributes; }
            set { _ruleattributes = value; }
        }

        public Update Update
        {
            get { return _update; }
            set { _update = value; }
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
            if (obj is Rule)
            {
                Rule other = (Rule)obj;
                return _placement.Equals(other._placement)
                    && _ruleattributes.Equals(other._ruleattributes)
                    && _propertyname.Equals(other._propertyname)
                    && _expression.Equals(other._expression);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _placement.GetHashCode() ^ _ruleattributes.GetHashCode() ^
                _propertyname.GetHashCode() ^ _expression.GetHashCode();
        }

        #endregion
*/
    }
}
