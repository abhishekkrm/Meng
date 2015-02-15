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
    public sealed class Code_ : Statement_, ICode_, IElement_
    {
        #region Constructor

        public Code_(IEnumerable<IEmbedded_> _embedded, IEnumerable<IStatement_> _statements) : base(Statement_.Type_._Code)
        {
            this._embedded = (new List<IEmbedded_>(_embedded)).ToArray();
            this._statements = (new List<IStatement_>(_statements)).ToArray();
        }

        #endregion

        #region Fields

        private IEmbedded_[] _embedded;
        private IStatement_[] _statements;

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            if (this._embedded != null)
                foreach (IEmbedded_ _embedded in this._embedded)
                {
                    if (_embedded != null)
                        _embedded._Print(_printer);
                    else
                        _printer._Print("/* unrecognized */\n");
                }
            if (this._statements != null)
                foreach (IStatement_ _statement in this._statements)
                {
                    if (_statement != null)
                        _statement._Print(_printer);
                    else
                        _printer._Print("/* unrecognized */\n");
                }
        }

        #endregion

        #region ICode_ Members

        IEmbedded_[] ICode_._Embedded
        {
            get { return this._embedded; }
        }

        IStatement_[] ICode_._Statements
        {
            get { return this._statements; }
        }

        #endregion
    }
}
