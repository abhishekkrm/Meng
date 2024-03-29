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

namespace QS._qss_x_.Language_.AST_
{
    public sealed class PredefinedOperator : Operator
    {
        #region Constructors

        public PredefinedOperator(PredefinedOperatorCategory _predefinedoperatorcategory, ValueType _valuetype, List<ValueType> _parametertypes)
            : base(_valuetype, _parametertypes)
        {
            this._predefinedoperatorcategory = _predefinedoperatorcategory;
        }

        public PredefinedOperator()
        {
        }

        #endregion

        #region Fields

        private PredefinedOperatorCategory _predefinedoperatorcategory;

        #endregion

        #region Accessors

        public PredefinedOperatorCategory PredefinedOperatorCategory
        {
            get { return _predefinedoperatorcategory; }
            set { _predefinedoperatorcategory = value; }
        }

        #endregion

        #region Overridden from Operator

        public override OperatorCategory OperatorCategory
        {
            get { return OperatorCategory.Predefined; }
        }

        #endregion

        public override string ToString()
        {
            return PredefinedOperatorCategory.ToString();
        }

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is PredefinedOperator)
            {
                PredefinedOperator other = (PredefinedOperator) obj;
                return base.Equals(obj) && (_predefinedoperatorcategory.Equals(other._predefinedoperatorcategory));
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _predefinedoperatorcategory.GetHashCode();
        }

        #endregion
*/ 
    }
}
