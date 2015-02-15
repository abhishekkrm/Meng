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
    public abstract class OperatorClass_ : Declaration_, IOperatorClass_
    {
        #region Constructor

        protected OperatorClass_(string _identifier, IMetadata_ _metadata, Type_ _type, IEnumerable<IParameter_> _parameters,
            IEnumerable<IValue_> _arguments, IEnumerable<IValue_> _results)
            : base(_identifier, _metadata, Declaration_.Type_._OperatorClass, _parameters)
        {
            this._type = _type;
            if (_arguments != null)
                this._arguments = (new List<IValue_>(_arguments)).ToArray();
            if (_results != null)
                this._results = (new List<IValue_>(_results)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Type_ _type;
        [QS.Fx.Base.Inspectable]
        private IValue_[] _arguments;
        [QS.Fx.Base.Inspectable]
        private IValue_[] _results;

        #endregion

        #region Type_

        public enum Type_
        {
            _Primitive, _Parameter, _Custom, _Reference
        }

        #endregion

        #region IOperatorClass_ Members

        Type_ IOperatorClass_._Type
        {
            get { return this._type; }
        }

        IValue_[] IOperatorClass_._Arguments
        {
            get { return this._arguments; }
        }

        IValue_[] IOperatorClass_._Results
        {
            get { return this._results; }
        }

        #endregion
    }
}
