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

namespace QS._qss_x_.Language_.Structure_
{
    public sealed class Aggregation : Flow
    {
        #region Constructors

        public Aggregation(Operator _operator, AggregationAttributes _aggregationattributes, 
            Property _sourceproperty, Property _groupaggregate, Property _parentaggregate) : base()
        {
            this._operator = _operator;
            this._aggregationattributes = _aggregationattributes;
            this._sourceproperty = _sourceproperty;
            this._groupaggregate = _groupaggregate;
            this._parentaggregate = _parentaggregate;
        }

        #endregion

        #region Fields

        private Operator _operator;
        private AggregationAttributes _aggregationattributes;
        private Property _sourceproperty, _groupaggregate, _parentaggregate;

        #endregion

        #region Overridden from Flow

        public override FlowCategory FlowCategory
        {
            get { return FlowCategory.Aggregation; }
        }

        #endregion

        #region Interface

        public Operator Operator
        {
            get { return _operator; }
        }

        public AggregationAttributes AggregationAttributes
        {
            get { return _aggregationattributes; }
        }

        public Property SourceProperty
        {
            get { return _sourceproperty; }
        }

        public Property GroupAggregate
        {
            get { return _groupaggregate; }
            set { _groupaggregate = value; }
        }

        public Property ParentAggregate
        {
            get { return _parentaggregate; }
            set { _parentaggregate = value; }
        }

        #endregion
    }
}
