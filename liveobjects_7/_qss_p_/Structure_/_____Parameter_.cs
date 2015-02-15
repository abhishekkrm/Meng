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

/*
using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_p_.Structure_
{

    public sealed class Parameter_ : Declaration_, IParameter_
    {
        #region Constructor

        public Parameter_(string _identifier, Type_ _type, IElement_ _value)
        {
            this._identifier = _identifier;
            this._type = _type;
            this._value = _value;
        }

        public Parameter_(string _identifier, Type_ _type)
        {
            this._identifier = _identifier;
            this._type = _type;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _identifier;
        [QS.Fx.Base.Inspectable]
        private Type_ _type;
        [QS.Fx.Base.Inspectable]
        private IElement_ _value;

        #endregion

        #region Type_

        public enum Type_
        {
            _Value, _ValueClass, _InterfaceClass, _EndpointClass, _ObjectClass
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    {
                        switch (this._type)
                        {
                            case Type_._ValueClass:
                                _printer._Print("value class");
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        _printer._Print(" ");
                        _printer._Print(this._identifier);
                        if (this._value != null)
                        {
                            _printer._Print(" = ");
                            if (this._value is IElement_)
                                ((IElement_)this._value)._Print(_printer);
                            else
                                throw new NotImplementedException();
                        }
                    }
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        _printer._Print("[QS.Fx.Reflection.Parameter(\"");
                        _printer._Print(this._identifier);
                        _printer._Print("\", QS.Fx.Reflection.ParameterClass.");
                        switch (this._type)
                        {
                            case Type_._ValueClass:
                                _printer._Print("ValueClass");
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                        _printer._Print(")] ");
                        _printer._Print(this._identifier);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IParameter_ Members

        string IParameter_._Identifier
        {
            get { return this._identifier; }
        }

        Parameter_.Type_ IParameter_._Type
        {
            get { return this._type; }
        }

        IElement_ IParameter_._Value
        {
            get { return this._value; }
        }

        #endregion

        #region _PrintDeclarations

        public static void _PrintDeclarations(QS._qss_p_.Printing_.IPrinter_ _printer, IEnumerable<IParameter_> _parameters)
        {
            if (_parameters != null)
            {
                switch (_printer._Type)
                {
                    case QS._qss_p_.Printing_.Printer_.Type_._Source:
                        {
                            _printer._Print("<");
                            bool _isfirst = true;
                            foreach (IParameter_ _parameter in _parameters)
                            {
                                if (_isfirst)
                                    _isfirst = false;
                                else
                                    _printer._Print(", ");
                                _parameter._PrintDeclaration(_printer);
                            }
                            _printer._Print(">");
                        }
                        break;

                    case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                        {
                            _printer._Print("<\n");
                            _printer._Shift(1);
                            bool _isfirst = true;
                            foreach (IParameter_ _parameter in _parameters)
                            {
                                if (_isfirst)
                                    _isfirst = false;
                                else
                                    _printer._Print(",\n");
                                _parameter._Print(_printer);
                            }
                            _printer._Shift(-1);
                            _printer._Print(">");
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion
    }
}
*/
