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

namespace QS._qss_p_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Protocol, "Protocol", "Properties Framework Protocol Object")]
    public sealed class Protocol_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        #region Constructor

        public Protocol_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("code", QS.Fx.Reflection.ParameterClass.Value)] string _code)
        {
            this._code = _code;

            try
            {
                this._library = QS._qss_p_.Parser_.Parser._Parse(_code);
                QS._qss_p_.Printing_.Printer_ _printer = new QS._qss_p_.Printing_.Printer_(QS._qss_p_.Printing_.Printer_.Type_._Source);
                this._library._Print(_printer);
                this._output = _printer.ToString();
            }
            catch (Exception _exc)
            {
                this._output = _exc.ToString();
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _code;
        [QS.Fx.Base.Inspectable]
        private QS._qss_p_.Structure_.ILibrary_ _library;
        [QS.Fx.Base.Inspectable]
        private string _output;

        #endregion
    }
}
