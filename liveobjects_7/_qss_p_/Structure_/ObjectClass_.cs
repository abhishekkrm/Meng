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
    public abstract class ObjectClass_ : Declaration_, IObjectClass_
    {
        #region Constructor

        protected ObjectClass_(string _identifier, IMetadata_ _metadata, Type_ _type, IEnumerable<IParameter_> _parameters, IEnumerable<IEndpoint_> _endpoints)
            : base(_identifier, _metadata, Declaration_.Type_._ObjectClass, _parameters)
        {
            this._type = _type;
            if (_endpoints != null)
                this._endpoints = (new List<IEndpoint_>(_endpoints)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Type_ _type;
        [QS.Fx.Base.Inspectable]
        private IEndpoint_[] _endpoints;

        #endregion

        #region Type_

        public enum Type_
        {
            _Primitive, _Parameter, _Custom, _Reference
        }

        #endregion

        #region IObjectClass_ Members

        Type_ IObjectClass_._Type
        {
            get { return this._type; }
        }

        IEndpoint_[] IObjectClass_._Endpoints
        {
            get { return this._endpoints; }
        }

        #endregion
    }
}
