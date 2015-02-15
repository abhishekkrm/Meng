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
    public sealed class ObjectClass_Custom_ : ObjectClass_, IObjectClass_Custom_, IDeclaration_
    {
        #region Constructor

        public ObjectClass_Custom_(string _identifier, IMetadata_ _metadata, IEnumerable<IParameter_> _parameters, IEnumerable<IEndpoint_> _endpoints)
            : base(_identifier, _metadata, ObjectClass_.Type_._Custom, _parameters, _endpoints)
        {
        }

        #endregion

        #region Fields

        #endregion

        #region IObjectClass_Custom_ Members

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("objectclass_");
                    _printer._Print(((IDeclaration_)this)._Identifier);
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IDeclaration_ Members

        void IDeclaration_._Declare(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        _printer._Print("#region object class \"");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("\"\n\n");
                        Metadata_._Declare(_printer, ((IDeclaration_) this)._Metadata);
                        _printer._Print("[QS.Fx.Reflection.ObjectClass(");
                        ((IDeclaration_)this)._Metadata._Print(_printer);
                        _printer._Print(")]\npublic interface objectclass_");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _DeclareParameters(_printer, ((IDeclaration_) this)._Parameters, false);
                        _printer._Shift(1);
                        _printer._Print("\n: QS.Fx.Object.Classes.IObject\n");
                        _printer._Shift(-1);
                        _DeclareParameterConstraints(_printer, ((IDeclaration_)this)._Parameters);
                        _printer._Print("{\n");
                        _printer._Shift(1);
                        if (((IObjectClass_)this)._Endpoints != null)
                        {
                            bool _isfirst = true;
                            foreach (IEndpoint_ _endpoint in ((IObjectClass_)this)._Endpoints)
                            {
                                if (_isfirst)
                                    _isfirst = false;
                                else
                                    _printer._Print("\n");
                                _endpoint._Declare(_printer);
                            }
                        }
                        _printer._Shift(-1);
                        _printer._Print("}\n\n#endregion\n");
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
