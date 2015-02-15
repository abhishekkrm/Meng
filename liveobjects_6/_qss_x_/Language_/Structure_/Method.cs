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

namespace QS._qss_x_.Language_.Structure_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public abstract class Method
    {
        #region Constructors

        protected Method(Interface _interface, string _name, ValueType _resulttype, ValueType[] _parametertypes)
        {
            this._interface = _interface;
            this._name = _name;
            this._resulttype = _resulttype;
            this._parametertypes = _parametertypes;
        }

        #endregion

        #region Fields

        private Interface _interface;
        [QS.Fx.Printing.Printable]
        private string _name;
        
        private ValueType _resulttype;
        private ValueType[] _parametertypes;

        #endregion

        #region Accesors

        public Interface Interface
        {
            get { return _interface; }
        }

        public string Name
        {
            get { return _name; }
        }

        public ValueType ResultType
        {
            get { return _resulttype; }
        }

        public ValueType[] ParameterTypes
        {
            get { return _parametertypes; }
        }

        public abstract MethodCategory MethodCategory
        {
            get;
        }

        #endregion
    }
}
