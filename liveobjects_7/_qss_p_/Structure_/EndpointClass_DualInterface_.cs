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
    public sealed class EndpointClass_DualInterface_ : EndpointClass_, IEndpointClass_DualInterface_, IElement_
    {
        #region Constructor

        public EndpointClass_DualInterface_(IInterfaceClass_ _imported, IInterfaceClass_ _exported)
            : base(string.Empty, null, EndpointClass_.Type_._DualInterface, null)
        {
            this._imported = _imported;
            this._exported = _exported;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IInterfaceClass_ _imported;
        [QS.Fx.Base.Inspectable]
        private IInterfaceClass_ _exported;

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("QS.Fx.Endpoint.Classes.IDualInterface<");
                    _imported._Print(_printer);
                    _printer._Print(", ");
                    _exported._Print(_printer);
                    _printer._Print(">");
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IEndpointClass_ Members

        void IEndpointClass_._PrintInternal(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("QS.Fx.Endpoint.Internal.IDualInterface<");
                    _imported._Print(_printer);
                    _printer._Print(", ");
                    _exported._Print(_printer);
                    _printer._Print(">");
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                default:
                    throw new NotImplementedException();
            }
        }

        void IEndpointClass_._PrintCreator(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("DualInterface<");
                    _imported._Print(_printer);
                    _printer._Print(", ");
                    _exported._Print(_printer);
                    _printer._Print(">");
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IEndpointClass_DualInterface_ Members

        IInterfaceClass_ IEndpointClass_DualInterface_._Imported
        {
            get { return this._imported; }
        }

        IInterfaceClass_ IEndpointClass_DualInterface_._Exported
        {
            get { return this._exported; }
        }

        #endregion
    }
}
