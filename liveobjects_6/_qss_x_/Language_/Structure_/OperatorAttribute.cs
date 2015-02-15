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
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class OperatorAttribute : System.Attribute
    {
        #region Constructors

        public OperatorAttribute(string _name) 
            : this(_name, new PredefinedOperator[0], false)
        {
        }

        public OperatorAttribute(PredefinedOperator _predefinedoperator)
            : this(new string[0], _predefinedoperator, false)
        {
        }

        public OperatorAttribute(string _name, PredefinedOperator _predefinedoperator)
            : this(_name, _predefinedoperator, false)
        {
        }

        public OperatorAttribute(string _name, bool _isatom)
            : this(_name, new PredefinedOperator[0], _isatom)
        {
        }

        public OperatorAttribute(PredefinedOperator _predefinedoperator, bool _isatom)
            : this(new string[0], _predefinedoperator, _isatom)
        {
        }

        public OperatorAttribute(string _name, PredefinedOperator _predefinedoperator, bool _isatom)
            : this(new string[] { _name }, new PredefinedOperator[] { _predefinedoperator }, _isatom)
        {
        }

        public OperatorAttribute(string[] _names)
            : this(_names, new PredefinedOperator[0], false)
        {
        }

        public OperatorAttribute(PredefinedOperator[] _predefinedoperators)
            : this(new string[0], _predefinedoperators, false)
        {
        }

        public OperatorAttribute(string[] _names, PredefinedOperator[] _predefinedoperators)
            : this(_names, _predefinedoperators, false)
        {
        }

        public OperatorAttribute(string[] _names, bool _isatom)
            : this(_names, new PredefinedOperator[0], _isatom)
        {
        }

        public OperatorAttribute(PredefinedOperator[] _predefinedoperators, bool _isatom)
            : this(new string[0], _predefinedoperators, _isatom)
        {
        }

        public OperatorAttribute(string _name, PredefinedOperator[] _predefinedoperators)
            : this(new string[] { _name }, _predefinedoperators, false)
        {
        }

        public OperatorAttribute(string[] _names, PredefinedOperator _predefinedoperator)
            : this(_names, new PredefinedOperator[] { _predefinedoperator }, false)
        {
        }

        public OperatorAttribute(string _name, PredefinedOperator[] _predefinedoperators, bool _isatom)
            : this(new string[] { _name }, _predefinedoperators, _isatom)
        {
        }

        public OperatorAttribute(string[] _names, PredefinedOperator _predefinedoperator, bool _isatom)
            : this(_names, new PredefinedOperator[] { _predefinedoperator }, _isatom)
        {
        }

        public OperatorAttribute(string[] _names, PredefinedOperator[] _predefinedoperators, bool _isatom)
        {
            this._names = _names;
            this._predefinedoperators = _predefinedoperators;
            this._isatom = _isatom;
        }

        #endregion

        #region Fields

        private string[] _names;
        private PredefinedOperator[] _predefinedoperators;
        private bool _isatom;

        #endregion

        #region Accessors

        public string[] Names
        {
            get { return _names; }
        }

        public PredefinedOperator[] PredefinedOperators
        {
            get { return _predefinedoperators; }
        }

        public bool IsAtom
        {
            get { return _isatom; }
        }

        #endregion
    }
}
