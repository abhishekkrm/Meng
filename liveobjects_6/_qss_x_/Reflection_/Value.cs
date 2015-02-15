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

#define DEBUG_INCLUDE_INSPECTION_CODE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Reflection_
{
    [QS.Fx.Printing.Printable("Value", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Value : QS.Fx.Inspection.Inspectable, QS.Fx.Reflection.IValue
    {
        #region Constructor

        public Value(string _id, QS.Fx.Reflection.IValueClass _valueclass, System.Reflection.ParameterInfo _parameterinfo)
        {
            this._id = _id;
            this._valueclass = _valueclass;
            this._parameterinfo = _parameterinfo;
        }

        #endregion

        #region Fields

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("id")]
#endif
        private string _id;
#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("valueclass")]
#endif
        private QS.Fx.Reflection.IValueClass _valueclass;
#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("parameterinfo")]
#endif
        private System.Reflection.ParameterInfo _parameterinfo;

        #endregion

        #region IValue Members

        string QS.Fx.Reflection.IValue.ID
        {
            get { return _id; }
        }

        QS.Fx.Reflection.IValueClass QS.Fx.Reflection.IValue.ValueClass
        {
            get { return _valueclass; }
        }

        QS.Fx.Reflection.IValue QS.Fx.Reflection.IValue.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            return new Value(this._id, this._valueclass.Instantiate(_parameters), this._parameterinfo);
        }

        #endregion
    }
}
