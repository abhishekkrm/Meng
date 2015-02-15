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
    public sealed class Object_Custom_ : Object_, IObject_Custom_, IDeclaration_
    {
        #region Constructor

        public Object_Custom_(string _identifier, IMetadata_ _metadata, IEnumerable<IParameter_> _parameters, 
            IObjectClass_ _objectclass, IEnumerable<IEndpoint_> _endpoints)
            : base(_identifier, _metadata, Object_.Type_._Custom, _parameters, _objectclass, _endpoints)
        {
        }

        #endregion

        #region Fields

        #endregion

        #region IObject_Custom_ Members

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("object_");
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
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        _printer._Print("#region object \"");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("\"\n\n");
                        Metadata_._Declare(_printer, ((IDeclaration_) this)._Metadata);
                        _printer._Print("[QS.Fx.Reflection.ComponentClass(");
                        ((IDeclaration_) this)._Metadata._Print(_printer);
                        _printer._Print(")]\n[QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]\n");
                        _printer._Print("public sealed class object_");
                        _printer._Print(((IDeclaration_) this)._Identifier);
                        Declaration_._DeclareParameters(_printer, ((IDeclaration_) this)._Parameters, false);
                        _printer._Shift(1);
                        _printer._Print("\n: ");
                        ((IObject_) this)._ObjectClass._Print(_printer);
                        _printer._Print("\n");                        
                        _printer._Shift(-1);
                        _DeclareParameterConstraints(_printer, ((IDeclaration_) this)._Parameters);
                        _printer._Print("{\n");
                        _printer._Shift(1);
                        _printer._Print("#region constructor\n\npublic object_");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Shift(1);
                        _printer._Print("(\nQS.Fx.Object.IContext _context");
                        Declaration_._DeclareParameters(_printer, ((IDeclaration_) this)._Parameters, true, true);
                        _printer._Shift(-1);
                        _printer._Print("\n)\n{\n");
                        _printer._Shift(1);
                        _printer._Print("this._context = _context;\n");
                        if (((IObject_)this)._Endpoints != null)
                        {
                            foreach (IEndpoint_ _endpoint in ((IObject_) this)._Endpoints)
                            {
                                _printer._Print("this._endpoint_");
                                _printer._Print(_endpoint._Identifier);
                                _printer._Print(" = _context.");
                                _endpoint._EndpointClass._PrintCreator(_printer);
                                _printer._Print("(null)"); // .........................................................................................................................................................HERE FIX NEEDED
                                _printer._Print(";\n");
                            }
                        }
                        _printer._Shift(-1);
                        _printer._Print("}\n\n#endregion\n\n#region fields\n\n[QS.Fx.Base.Inspectable]\nprivate QS.Fx.Object.IContext _context;\n");
                        if (((IObject_) this)._Endpoints != null)
                        {
                            foreach (IEndpoint_ _endpoint in ((IObject_) this)._Endpoints)
                            {
                                _printer._Print("[QS.Fx.Base.Inspectable]\nprivate ");
                                _endpoint._EndpointClass._PrintInternal(_printer);
                                _printer._Print(" _endpoint_");
                                _printer._Print(_endpoint._Identifier);
                                _printer._Print(";\n");
                            }
                        }
                        _printer._Print("\n#endregion\n");
                        if (((IObject_) this)._Endpoints != null)
                        {
                            _printer._Print("\n#region endpoint accessors\n");
                            foreach (IEndpoint_ _endpoint in ((IObject_) this)._Endpoints)
                            {
                                _printer._Print("\n");
                                _endpoint._EndpointClass._Print(_printer);
                                _printer._Print(" ");
                                ((IObject_) this)._ObjectClass._Print(_printer);
                                _printer._Print(".");
                                _printer._Print(_endpoint._Identifier);
                                _printer._Print("\n{\n");
                                _printer._Shift(1);
                                _printer._Print("get { return this._endpoint_");
                                _printer._Print(_endpoint._Identifier);
                                _printer._Print("; }\n");
                                _printer._Shift(-1);
                                _printer._Print("}\n");
                            }
                            _printer._Print("\n#endregion\n");
                        }
                        _printer._Shift(-1);
                        _printer._Print("}\n\n#endregion\n");
                    }
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
