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
    public sealed class Declaration_Parameter_ : Declaration_, IDeclaration_, IParameter_, IUnresolved_, 
        IValueClass_, IValueClass_Parameter_, IInterfaceClass_, IInterfaceClass_Parameter_
    {
        #region Constructor

        public Declaration_Parameter_(string _identifier, Type_ _type, IDeclaration_ _constraint)
            : base(_identifier, null, _type, null)
        {
            this._constraint = _constraint;
        }

        public Declaration_Parameter_(string _identifier, IDeclaration_ _constraint)
            : this(_identifier, Type_._Unresolved, _constraint)
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IDeclaration_ _constraint;

        #endregion

        #region IParameter_ Members

        IDeclaration_ IParameter_._Constraint
        {
            get { return this._constraint; }
        }

        void IParameter_._DeclareConstraint(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    throw new NotImplementedException();

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        if (this._constraint != null)
                        {
                            _printer._Print("where ");
                            _printer._Print(((IDeclaration_) this)._Identifier);
                            _printer._Print(" : ");
                            this._constraint._Print(_printer);
                            _printer._Print("\n");
                        }
                    }
                    break;

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
                    throw new NotImplementedException();

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        _printer._Print("[QS.Fx.Reflection.Parameter(\"");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("\", \"\", QS.Fx.Reflection.ParameterClass.");
                        switch (((IDeclaration_) this)._Type)
                        {
                            case Type_._Value:
                                _printer._Print("Value");
                                break;

                            case Type_._ValueClass:
                                _printer._Print("ValueClass");
                                break;

                            case Type_._EndpointClass:
                                _printer._Print("EndpointClass");
                                break;

                            case Type_._InterfaceClass:
                                _printer._Print("InterfaceClass");
                                break;

                            case Type_._ObjectClass:
                                _printer._Print("ObjectClass");
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                        _printer._Print(", ");
                        switch (((IDeclaration_)this)._Type)
                        {
                            case Type_._Value:
                                {
                                    _printer._Print("null");
                                }
                                break;

                            default:
                                {
                                    if (this._constraint != null)
                                    {
                                        _printer._Print("typeof(");
                                        this._constraint._Print(_printer);
                                        _printer._Print(")");
                                    }
                                    else
                                        _printer._Print("null");
                                }
                                break;
                        }
                        _printer._Print(")]");
                        switch (((IDeclaration_)this)._Type)
                        {
                            case Type_._Value:
                                {
                                    _printer._Print("\n");
                                    this._constraint._Print(_printer);                                    
                                }
                                break;

                            default:
                                {
                                }
                                break;
                        }
                        _printer._Print(" ");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IUnresolved_ Members

        void IUnresolved_._Resolve()
        {
            if (((IDeclaration_) this)._Type == Type_._Unresolved)
            {
                if (this._constraint != null)
                {
                    if (this._constraint is IUnresolved_)
                        ((IUnresolved_) this._constraint)._Resolve();
                    this._Type(this._constraint._Type);
                }
                else
                    throw new Exception("Cannot resolve the type of parameter \"" + ((IDeclaration_) this)._Identifier + "\".");
            }
        }

        #endregion

        #region IValueClass_ Members

        ValueClass_.Type_ IValueClass_._Type
        {
            get { return ValueClass_.Type_._Parameter; }
        }

        #endregion

        #region IValueClass_Parameter_ Members

        IValueClass_ IValueClass_Parameter_._Constraint
        {
            get { return (IValueClass_) this._constraint; }
        }

        #endregion

        #region IInterfaceClass_ Members

        InterfaceClass_.Type_ IInterfaceClass_._Type
        {
            get { return InterfaceClass_.Type_._Parameter; }
        }

        #endregion

        #region IInterfaceClass_Parameter_ Members

        IInterfaceClass_ IInterfaceClass_Parameter_._Constraint
        {
            get { return (IInterfaceClass_) this._constraint; }
        }

        #endregion
    }
}
