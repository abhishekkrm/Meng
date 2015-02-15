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
    public abstract class InterfaceClass_ : Declaration_, IInterfaceClass_
    {
        #region Constructor

        protected InterfaceClass_(string _identifier, IMetadata_ _metadata, Type_ _type, IEnumerable<IParameter_> _parameters) // , IEnumerable<IFlow_> _flows)
            : base(_identifier, _metadata, Declaration_.Type_._InterfaceClass, _parameters)
        {
            this._type = _type;

            //if (_flows != null)
            //    this._flows = (new List<IFlow_>(_flows)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Type_ _type;

        //[QS.Fx.Base.Inspectable]
        //private IFlow_[] _flows;

        #endregion

        #region Type_

        public enum Type_
        {
            _Parameter, _Custom, _Reference, _Primitive
        }

        #endregion

        #region IInterfaceClass_ Members

        Type_ IInterfaceClass_._Type
        {
            get { return this._type; }
        }

        //IFlow_[] IInterfaceClass_._Flows
        //{
        //    get { return this._flows; }
        //}

        #endregion
    }
}
