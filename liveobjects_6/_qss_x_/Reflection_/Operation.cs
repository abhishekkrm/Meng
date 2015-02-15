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

namespace QS._qss_x_.Reflection_
{
    [QS.Fx.Printing.Printable("Operation", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Operation : QS.Fx.Inspection.Inspectable, QS.Fx.Reflection.IOperation
    {
        #region Constructor

        public Operation(string _id, QS.Fx.Reflection.IOperationClass _operationclass, System.Reflection.MemberInfo _memberinfo)
        {
            this._id = _id;
            this._operationclass = _operationclass;
            this._memberinfo = _memberinfo;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("id")]
        private string _id;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("operationclass")]
        private QS.Fx.Reflection.IOperationClass _operationclass;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("memberinfo")]
        private System.Reflection.MemberInfo _memberinfo;

        #endregion

        #region IOperation Members

        string QS.Fx.Reflection.IOperation.ID
        {
            get { return _id; }
        }

        QS.Fx.Reflection.IOperationClass QS.Fx.Reflection.IOperation.OperationClass
        {
            get { return _operationclass; }
        }

        QS.Fx.Reflection.IOperation QS.Fx.Reflection.IOperation.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            return new Operation(this._id, this._operationclass.Instantiate(_parameters), this._memberinfo);
        }

        #endregion
    }
}
