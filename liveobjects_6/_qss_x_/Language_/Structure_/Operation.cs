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
    public sealed class Operation : Expression
    {
        #region Constructors

        public Operation(Operator _operator, Expression[] _arguments) : base(_operator.ResultType)
        {
            this._operator = _operator;
            this._arguments = _arguments;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private Operator _operator;
        [QS.Fx.Printing.Printable]
        private Expression[] _arguments;

        #endregion

        #region Overridden from Expression

        public override ExpressionCategory ExpressionCategory
        {
            get { return ExpressionCategory.Operation; }
        }

        public override bool IsConstant
        {
            get 
            {
                foreach (Expression _e in _arguments)
                {
                    if (!_e.IsConstant)
                        return false;
                }
                return true; 
            }
        }

        public override bool IsAtom
        {
            get 
            {
                if (_operator.IsAtom)
                {
                    foreach (Expression _e in _arguments)
                    {
                        if (!_e.IsAtom)
                            return false;
                    }
                    return true;
                }
                else
                    return false; 
            }
        }

        #endregion

        #region Interface

        public Operator Operator
        {
            get { return _operator; }
        }

        public IEnumerable<Expression> Arguments
        {
            get { return _arguments; }
        }

        #endregion

        #region Overridden from System.Object

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _operator.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj) && (obj is Operation))
            {
                Operation other = (Operation)obj;
                if (_operator.Equals(other._operator) && _arguments.Length.Equals(other._arguments.Length))
                {
                    for (int _k = 0; _k < _arguments.Length; _k++)
                    {
                        if (!_arguments[_k].Equals(other._arguments[_k]))
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        #endregion
    }
}
