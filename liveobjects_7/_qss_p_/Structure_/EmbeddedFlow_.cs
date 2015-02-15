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
    public sealed class EmbeddedFlow_ : Embedded_, IEmbeddedFlow_, IElement_
    {
        #region Constructor

        public EmbeddedFlow_(IFlow_ _flow, IFlow_ _initializer) : base(Embedded_.Type_._Flow)
        {
            this._flow = _flow;
            this._initializer = _initializer;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IFlow_ _flow;
        [QS.Fx.Base.Inspectable]
        private IFlow_ _initializer;

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            this._flow._Print(_printer);
            if (this._initializer != null)
            {
                _printer._Print(" = ");
                _initializer._Print(_printer);
            }
            _printer._Print(";\n");
        }

        #endregion

        #region IEmbeddedFlow_ Members

        IFlow_ IEmbeddedFlow_._Flow
        {
            get { return this._flow; }
        }

        IFlow_ IEmbeddedFlow_._Initializer
        {
            get { return this._initializer; }
        }

        #endregion
    }
}
