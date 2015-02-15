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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Constant : Expression
    {
        #region Constructors

        public Constant(ValueType _valuetype, object _value) : base(_valuetype)
        {
            this._value = _value;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private object _value;

        #endregion

        #region Overridden from Expression

        public override ExpressionCategory ExpressionCategory
        {
            get { return ExpressionCategory.Constant; }
        }

        public override bool IsConstant
        {
            get { return true; }
        }

        public override bool IsAtom
        {
            get { return true; }
        }

        #endregion

        #region Interface

        public object Value
        {
            get { return _value; }
        }

        #endregion

        #region Overridden from System.Object

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj) && (obj is Constant))
            {
                Constant other = (Constant) obj;
                return _value.Equals(other._value);
            }
            else
                return false;
        }

        #endregion
    }
}
