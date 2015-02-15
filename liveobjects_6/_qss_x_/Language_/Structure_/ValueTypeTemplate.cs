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
using System.Reflection;

namespace QS._qss_x_.Language_.Structure_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class ValueTypeTemplate
    {
        #region Constructors

        public ValueTypeTemplate(System.Type _underlyingtype, string _name, PredefinedTypeTemplate _predefinedtypetemplate,
            ValueTypeTemplateParameter[] _parameters)
        {
            this._underlyingtype = _underlyingtype;
            this._name = _name;
            this._predefinedtypetemplate = _predefinedtypetemplate;
            this._parameters = _parameters;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
        private System.Type _underlyingtype;
        [QS.Fx.Printing.Printable]
        private string _name;
        [QS.Fx.Printing.Printable]
        private PredefinedTypeTemplate _predefinedtypetemplate;
        [QS.Fx.Printing.Printable]
        private ValueTypeTemplateParameter[] _parameters;

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return _name;
        }

        #endregion

        #region Interface

        public string Name
        {
            get { return _name; }
        }

        public PredefinedTypeTemplate PredefinedTypeTemplate
        {
            get { return _predefinedtypetemplate; }
        }

        public System.Type UnderlyingType
        {
            get { return _underlyingtype; }
        }

        public ValueTypeTemplateParameter[] Parameters
        {
            get { return _parameters; }
        }

        #endregion
    }
}
