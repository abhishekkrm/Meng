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

namespace QS._core_x_.Base
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class ParameterInfo : QS.Fx.Base.IParameterInfo
    {
        public ParameterInfo(string name, Type type)
            : this(name, type, QS.Fx.Base.ParameterAccess.Unrestricted)
        {
        }

        public ParameterInfo(string name, Type type, QS.Fx.Base.ParameterAccess access) 
            : this(name, type,
                ((access & QS.Fx.Base.ParameterAccess.Readable) == QS.Fx.Base.ParameterAccess.Readable),
                ((access & QS.Fx.Base.ParameterAccess.Writable) == QS.Fx.Base.ParameterAccess.Writable))
        {
        }

        public ParameterInfo(string name, Type type, bool readable, bool writable)
        {
            this.name = name;
            this.type = type;
            this.readable = readable;
            this.writable = writable;
        }

        private string name;
        private Type type;
        private bool readable, writable;

        #region IParameterInfo Members

        [QS.Fx.Printing.Printable]
        string QS.Fx.Base.IParameterInfo.Name
        {
            get { return name; }
        }

        [QS.Fx.Printing.Printable]
        Type QS.Fx.Base.IParameterInfo.Type
        {
            get { return type; }
        }

        [QS.Fx.Printing.Printable]
        bool QS.Fx.Base.IParameterInfo.Readable
        {
            get { return readable; }
        }

        [QS.Fx.Printing.Printable]
        bool QS.Fx.Base.IParameterInfo.Writable
        {
            get { return writable; }
        }

        #endregion
    }
}
