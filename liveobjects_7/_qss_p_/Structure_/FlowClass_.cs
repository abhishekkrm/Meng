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
    public sealed class FlowClass_ : QS.Fx.Inspection.Inspectable, IFlowClass_, IElement_
    {
        #region Constructor

        public FlowClass_(IValueClass_ _valueclass, Properties_ _properties)
        {
            this._valueclass = _valueclass;
            this._properties = _properties;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IValueClass_ _valueclass;
        [QS.Fx.Base.Inspectable]
        private Properties_ _properties;

        #endregion

        #region IFlowClass_ Members

        IValueClass_ IFlowClass_._ValueClass
        {
            get { return this._valueclass; }
        }

        Properties_ IFlowClass_._Properties
        {
            get { return this._properties; }
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            if ((this._properties & Properties_._Consistent) == Properties_._Consistent)
                _printer._Print("same ");
            if ((this._properties & Properties_._Constant) == Properties_._Constant)
                _printer._Print("const ");
            if (((this._properties & Properties_._Increasing) == Properties_._Increasing) || ((this._properties & Properties_._Decreasing) == Properties_._Decreasing))
            {
                if ((this._properties & Properties_._Strongly) == Properties_._Strongly)
                    _printer._Print("strong ");
                else
                    _printer._Print("weak ");
                if ((this._properties & Properties_._Increasing) == Properties_._Increasing)
                    _printer._Print("up ");
                if ((this._properties & Properties_._Decreasing) == Properties_._Decreasing)
                    _printer._Print("down ");
            }
            this._valueclass._Print(_printer);
        }

        #endregion
    }
}
