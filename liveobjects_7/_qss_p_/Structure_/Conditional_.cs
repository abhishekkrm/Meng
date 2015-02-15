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

namespace QS._qss_p_.Structure_
{
    public sealed class Conditional_ : Statement_, IConditional_, IElement_
    {
        #region Constructor

        public Conditional_(IExpression_ _expression, IStatement_ _statement1, IStatement_ _statement2) : base(Statement_.Type_._Conditional)
        {
            this._expression = _expression;
            this._statement1 = _statement1;
            this._statement2 = _statement2;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IExpression_ _expression;
        [QS.Fx.Base.Inspectable]
        private IStatement_ _statement1;
        [QS.Fx.Base.Inspectable]
        private IStatement_ _statement2;

        #endregion

        #region IConditional_ Members

        IExpression_ IConditional_._Expression
        {
            get { return this._expression; }
        }

        IStatement_ IConditional_._Statement1
        {
            get { return this._statement1; }
        }

        IStatement_ IConditional_._Statement2
        {
            get { return this._statement2; }
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            _printer._Print("where (");
            if (this._expression != null)
                _expression._Print(_printer);
            else
                _printer._Print(" /* unrecognized */ ");
            _printer._Print(")\n");
            if (this._statement1 != null)
            {
                if (this._statement1._Type == Type_._Code)
                    _statement1._Print(_printer);
                else
                {
                    _printer._Shift(1);
                    _statement1._Print(_printer);
                    _printer._Shift(-1);
                }
            }
            else
            {
                _printer._Shift(1);
                _printer._Print(" /* unrecognized */\n");
                _printer._Shift(-1);
            }
            if (this._statement2 != null)
            {
                _printer._Print("elsewhere\n");
                if (this._statement1._Type == Type_._Code)
                    _statement2._Print(_printer);
                else
                {
                    _printer._Shift(1);
                    _statement2._Print(_printer);
                    _printer._Shift(-1);
                }
            }
        }

        #endregion
    }
}
