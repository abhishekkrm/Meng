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
    public abstract class Object_ : Declaration_, IObject_
    {
        #region Constructor

        protected Object_(string _identifier, IMetadata_ _metadata, Type_ _type, IEnumerable<IParameter_> _parameters, 
            IObjectClass_ _objectclass, IEnumerable<IEndpoint_> _endpoints)
            // , ICode_ _code) 
            : base(_identifier, _metadata, Declaration_.Type_._Object, _parameters)
        {
            this._type = _type;
            this._objectclass = _objectclass;
            if (_endpoints != null)
                this._endpoints = (new List<IEndpoint_>(_endpoints)).ToArray();
            //this._code = _code;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Type_ _type;
        [QS.Fx.Base.Inspectable]
        private IObjectClass_ _objectclass;
        [QS.Fx.Base.Inspectable]
        private IEndpoint_[] _endpoints;

        //[QS.Fx.Base.Inspectable]
        //private ICode_ _code;

        #endregion

        #region Type_

        public enum Type_
        {
            _Parameter, _Custom, _Reference
        }

        #endregion

        #region IObject_ Members

        Type_ IObject_._Type
        {
            get { return this._type; }
        }

        IObjectClass_ IObject_._ObjectClass
        {
            get { return this._objectclass; }
        }

        IEndpoint_[] IObject_._Endpoints
        {
            get { return this._endpoints; }
        }

        //ICode_ IObject_._Code
        //{
        //    get { return this._code; }
        //}

        #endregion
    }
}
