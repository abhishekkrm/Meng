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
    public sealed class Endpoint_Internal_ : Endpoint_, IEndpoint_Internal_, IDeclaration_
    {
        #region Constructor

        public Endpoint_Internal_(string _identifier, IMetadata_ _metadata, IEndpointClass_ _endpointclass)
            //, ICode_ _code)
            : base(_identifier, _metadata, Endpoint_.Type_._Internal, _endpointclass)
        {
            //this._code = _code;
        }

        #endregion

        #region Fields

        //[QS.Fx.Base.Inspectable]
        //private ICode_ _code;

        #endregion

        #region IEndpoint_Internal_ Members

        //ICode_ IEndpoint_._Code
        //{
        //    get { return this._code; }
        //}

        #endregion

        #region IDeclaration_ Members

        void IDeclaration_._Declare(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
