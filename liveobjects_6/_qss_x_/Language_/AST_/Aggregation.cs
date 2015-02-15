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
    [XmlType("Aggregation")]
    public sealed class Aggregation : Flow
    {
        #region Constructors

        public Aggregation(string _sourceproperty, string _targetgroupproperty, string _targetparentproperty, 
            Operator _aggregationoperator, AggregationAttributes _aggregationattributes)
        {
            this._sourceproperty = _sourceproperty;
            this._targetgroupproperty = _targetgroupproperty;
            this._targetparentproperty = _targetparentproperty;
            this._aggregationoperator = _aggregationoperator;
            this._aggregationattributes = _aggregationattributes;
        }

        public Aggregation()
        {
        }

        #endregion

        #region Fields

        private string _sourceproperty, _targetgroupproperty, _targetparentproperty;
        private Operator _aggregationoperator;
        private AggregationAttributes _aggregationattributes;

        #endregion

        #region Accessors

        [XmlAttribute("SourceProperty")]
        public string SourceProperty
        {
            get { return _sourceproperty; }
            set { _sourceproperty = value; }
        }

        [XmlAttribute("TargetGroupProperty")]
        public string TargetGroupProperty
        {
            get { return _targetgroupproperty; }
            set { _targetgroupproperty = value; }
        }

        [XmlAttribute("TargetParentProperty")]
        public string TargetParentProperty
        {
            get { return _targetparentproperty; }
            set { _targetparentproperty = value; }
        }

        [XmlElement("AggregationOperator")]
        public Operator AggregationOperator
        {
            get { return _aggregationoperator; }
            set { _aggregationoperator = value; }
        }

        [XmlAttribute("AggregationAttributes")]
        public AggregationAttributes AggregationAttributes
        {
            get { return _aggregationattributes; }
            set { _aggregationattributes = value; }
        }

        #endregion

        #region Overridden from Flow

        public override FlowCategory FlowCategory
        {
            get { return FlowCategory.Aggregation; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Aggregation)
            {
                Aggregation other = (Aggregation) obj;
                return _sourceproperty.Equals(other._sourceproperty) && _targetgroupproperty.Equals(other._targetgroupproperty)
                    && _targetparentproperty.Equals(other._targetparentproperty) && _aggregationoperator.Equals(other._aggregationoperator)
                    && _aggregationattributes.Equals(other._aggregationattributes);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _sourceproperty.GetHashCode() ^ _targetgroupproperty.GetHashCode() ^ _targetparentproperty.GetHashCode() ^
                _aggregationoperator.GetHashCode() ^ _aggregationattributes.GetHashCode();
        }

        #endregion
*/
    }
}
