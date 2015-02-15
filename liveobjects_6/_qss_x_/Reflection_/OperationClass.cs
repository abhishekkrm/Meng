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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class OperationClass : QS.Fx.Inspection.Inspectable, QS.Fx.Reflection.IOperationClass
    {
        #region Constructor

        public OperationClass(QS.Fx.Reflection.IMessageClass _incoming, QS.Fx.Reflection.IMessageClass _outgoing)
        {
            this._incoming = _incoming;
            this._outgoing = _outgoing;
        }

        #endregion

        #region Fields

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Printing.Printable("incoming")]
        [QS.Fx.Base.Inspectable("incoming")]
#endif
        private QS.Fx.Reflection.IMessageClass _incoming;

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Printing.Printable("outgoing")]
        [QS.Fx.Base.Inspectable("outgoing")]
#endif
        private QS.Fx.Reflection.IMessageClass _outgoing;

        #endregion

        #region IOperationClass Members

        QS.Fx.Reflection.IMessageClass QS.Fx.Reflection.IOperationClass.Incoming
        {
            get { return _incoming; }
        }

        QS.Fx.Reflection.IMessageClass QS.Fx.Reflection.IOperationClass.Outgoing
        {
            get { return _outgoing; }
        }

        QS.Fx.Reflection.IOperationClass QS.Fx.Reflection.IOperationClass.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            return new OperationClass(this._incoming.Instantiate(_parameters), this._outgoing.Instantiate(_parameters));
        }

        bool QS.Fx.Reflection.IOperationClass.IsSubtypeOf(QS.Fx.Reflection.IOperationClass other)
        {
            if (ReferenceEquals(this, other))
                return true;
            return other.Incoming.IsSubtypeOf(this._incoming) && this._outgoing.IsSubtypeOf(other.Outgoing);
        }

        #endregion
    }
}
