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
    public sealed class OperatorClass_Custom_ : OperatorClass_, IOperatorClass_Custom_, IDeclaration_
    {
        #region Constructor

        public OperatorClass_Custom_(string _identifier, IMetadata_ _metadata, IEnumerable<IParameter_> _parameters, 
            IEnumerable<IValue_> _arguments, IEnumerable<IValue_> _results)
            : base(_identifier, _metadata, OperatorClass_.Type_._Custom, _parameters, _arguments, _results)
        {
        }

        #endregion

        #region Fields

        #endregion

        #region IOperatorClass_Custom_ Members

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("operatorclass_");
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
                        _printer._Print("#region operator class \"");
                        _printer._Print(((IDeclaration_) this)._Identifier);
                        _printer._Print("\"\n\n");
                        Metadata_._Declare(_printer, ((IDeclaration_) this)._Metadata);
                        _printer._Print("[QS.Fx.Reflection.OperatorClass(");
                        if (((IDeclaration_) this)._Metadata != null)
                            ((IDeclaration_) this)._Metadata._Print(_printer);
                        _printer._Print(")]\npublic interface operatorclass_");
                        _printer._Print(((IDeclaration_) this)._Identifier);
                        _DeclareParameters(_printer, ((IDeclaration_) this)._Parameters, false);
                        _printer._Shift(1);
                        _printer._Print("\n: QS.Fx.Operator.Classes.IOperator\n");
                        _printer._Shift(-1);
                        _DeclareParameterConstraints(_printer, ((IDeclaration_)this)._Parameters);
                        _printer._Print("{\n");
                        _printer._Shift(1);
                        _printer._Print("void compute");
                        IValue_[] _arguments = ((IOperatorClass_)this)._Arguments;
                        bool _isfirst = true;
                        if (_arguments != null)
                        {
                            foreach (IValue_ _argument in _arguments)
                            {
                                if (_isfirst)
                                {
                                    _printer._Print("\n(\n");
                                    _printer._Shift(1);
                                    _isfirst = false;
                                }
                                else
                                    _printer._Print(",\n");
                                _argument._Declare(_printer);
                            }
                        }
                        IValue_[] _results = ((IOperatorClass_) this)._Results;
                        if (_results != null)
                        {
                            foreach (IValue_ _result in _results)
                            {
                                if (_isfirst)
                                {
                                    _printer._Print("\n(\n");
                                    _printer._Shift(1);
                                    _isfirst = false;
                                }
                                else
                                    _printer._Print(",\n");
                                _result._Declare(_printer);
                            }
                        }
                        if (_isfirst)
                            _printer._Print("();\n");
                        else
                        {
                            _printer._Shift(-1);
                            _printer._Print("\n);\n");
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
