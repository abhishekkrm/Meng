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
    public abstract class Declaration_ : QS.Fx.Inspection.Inspectable, IDeclaration_
    {
        #region Constructor

        protected Declaration_(string _identifier, IMetadata_ _metadata, Type_ _type, IEnumerable<IParameter_> _parameters)
        {
            this._identifier = _identifier;
            this._metadata = _metadata;
            this._type = _type;
            if (_parameters != null)
                this._parameters = (new List<IParameter_>(_parameters)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _identifier;
        [QS.Fx.Base.Inspectable]
        private IMetadata_ _metadata;
        [QS.Fx.Base.Inspectable]
        private Type_ _type;
        [QS.Fx.Base.Inspectable]
        private IParameter_[] _parameters;

        #endregion

        #region Type_

        public enum Type_
        {
            _Unresolved, 
            _ValueClass,
            _OperatorClass,
            _FlowClass, 
            _InterfaceClass, 
            _EndpointClass, 
            _ObjectClass, 
            _Value, 
            _Operator, 
            _Flow, 
            _Interface, 
            _Endpoint, 
            _Object
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    _printer._Print(this._identifier);
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        //switch (this._type)
                        //{
                        //    case Type_._Value:
                        //        _printer._Print("value_");
                        //        break;
                        //    case Type_._ValueClass:
                        //        _printer._Print("valueclass_");
                        //        break;
                        //    case Type_._Operator:
                        //        _printer._Print("operator_");
                        //        break;
                        //    case Type_._OperatorClass:
                        //        _printer._Print("operatorclass_");
                        //        break;
                        //    case Type_._Interface:
                        //        _printer._Print("interface_");
                        //        break;
                        //    case Type_._InterfaceClass:
                        //        _printer._Print("interfaceclass_");
                        //        break;
                        //    case Type_._Endpoint:
                        //        _printer._Print("endpoint_");
                        //        break;
                        //    case Type_._EndpointClass:
                        //        _printer._Print("endpointclass_");
                        //        break;
                        //    case Type_._Object:
                        //        _printer._Print("object_");
                        //        break;
                        //    case Type_._ObjectClass:
                        //        _printer._Print("objectclass_");
                        //        break;
                        //    case Type_._Flow:
                        //        _printer._Print("flow_");
                        //        break;
                        //    case Type_._FlowClass:
                        //        _printer._Print("flowclass_");
                        //        break;
                        //}
                        _printer._Print(this._identifier);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IDeclaration_ Members

        string IDeclaration_._Identifier
        {
            get { return this._identifier; }
        }

        IMetadata_ IDeclaration_._Metadata
        {
            get { return this._metadata; }
        }

        Declaration_.Type_ IDeclaration_._Type
        {
            get { return this._type; }
        }

        IParameter_[] IDeclaration_._Parameters
        {
            get { return this._parameters; }
        }

        void IDeclaration_._Declare(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region _Type

        protected void _Type(Type_ _type)
        {
            this._type = _type;
        }

        #endregion

        #region _PrintParameters

        protected static void _PrintParameters(QS._qss_p_.Printing_.IPrinter_ _printer, IDeclaration_[] _parameters)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    throw new NotImplementedException();

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        if (_parameters != null)
                        {
                            _printer._Print("<");
                            bool _isfirst = true;
                            foreach (IDeclaration_ _parameter in _parameters)
                            {
                                if (_isfirst)
                                    _isfirst = false;
                                else
                                    _printer._Print(", ");
                                _parameter._Print(_printer);
                            }
                            _printer._Print(">");
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _DeclareParameters

        protected static void _DeclareParameters(QS._qss_p_.Printing_.IPrinter_ _printer, IParameter_[] _parameters, bool _values)
        {
            _DeclareParameters(_printer, _parameters, _values, false);
        }

        protected static void _DeclareParameters(QS._qss_p_.Printing_.IPrinter_ _printer, IParameter_[] _parameters, bool _values, bool _notfirst)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    throw new NotImplementedException();

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        if (_parameters != null)
                        {
                            if (!_values)
                            {
                                _printer._Print("<\n");
                                _printer._Shift(1);
                            }
                            bool _isfirst = !_notfirst;
                            foreach (IDeclaration_ _parameter in _parameters)
                            {
                                bool _isvalue;
                                switch (_parameter._Type)
                                {
                                    case Type_._Object:
                                    case Type_._Operator:
                                    case Type_._Value:
                                        _isvalue = true;
                                        break;

                                    case Type_._EndpointClass:
                                    case Type_._FlowClass:
                                    case Type_._InterfaceClass:
                                    case Type_._ObjectClass:
                                    case Type_._OperatorClass:
                                    case Type_._ValueClass:
                                        _isvalue = false;
                                        break;

                                    case Type_._Endpoint:
                                    case Type_._Flow:
                                    case Type_._Interface:
                                    case Type_._Unresolved:
                                    default:
                                        throw new NotImplementedException();
                                }
                                if (_isvalue == _values)
                                {
                                    if (_isfirst)
                                        _isfirst = false;
                                    else
                                        _printer._Print(",\n");
                                    _parameter._Declare(_printer);
                                }
                            }
                            if (!_values)
                            {
                                _printer._Shift(-1);
                                _printer._Print(">");
                            }
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _DeclareParameterConstraints

        protected static void _DeclareParameterConstraints(QS._qss_p_.Printing_.IPrinter_ _printer, IParameter_[] _parameters)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    throw new NotImplementedException();

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        if (_parameters != null)
                        {
                            _printer._Shift(1);
                            foreach (IParameter_ _parameter in _parameters)
                            {
                                bool _isvalue;
                                switch (_parameter._Type)
                                {
                                    case Type_._Object:
                                    case Type_._Operator:
                                    case Type_._Value:
                                        _isvalue = true;
                                        break;

                                    case Type_._EndpointClass:
                                    case Type_._FlowClass:
                                    case Type_._InterfaceClass:
                                    case Type_._ObjectClass:
                                    case Type_._OperatorClass:
                                    case Type_._ValueClass:
                                        _isvalue = false;
                                        break;

                                    case Type_._Endpoint:
                                    case Type_._Flow:
                                    case Type_._Interface:
                                    case Type_._Unresolved:
                                    default:
                                        throw new NotImplementedException();
                                }
                                if (!_isvalue)
                                    _parameter._DeclareConstraint(_printer);
                            }
                            _printer._Shift(-1);
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
