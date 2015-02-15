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
    public sealed class Operator_Custom_ : Operator_, IOperator_Custom_, IDeclaration_
    {
        #region Constructor

        public Operator_Custom_(string _identifier, IMetadata_ _metadata, IEnumerable<IParameter_> _parameters,
            IOperatorClass_ _operatorclass, IEnumerable<IValue_> _arguments, IEnumerable<IValue_> _results, IEnumerable<IValue_> _internals)
            : base(_identifier, _metadata, Operator_.Type_._Custom, _parameters, _operatorclass)
        {
            if (_arguments != null)
                this._arguments = (new List<IValue_>(_arguments)).ToArray();
            if (_results != null)
                this._results = (new List<IValue_>(_results)).ToArray();
            if (_internals != null)
                this._internals = (new List<IValue_>(_internals)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IValue_[] _arguments;
        [QS.Fx.Base.Inspectable]
        private IValue_[] _results;
        [QS.Fx.Base.Inspectable]
        private IValue_[] _internals;

        #endregion

        #region IOperator_Custom_ Members

        IValue_[] IOperator_Custom_._Arguments
        {
            get { return this._arguments; }
        }

        IValue_[] IOperator_Custom_._Results
        {
            get { return this._results; }
        }

        IValue_[] IOperator_Custom_._Internals
        {
            get { return this._internals; }
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("operator_");
                    _printer._Print(((IDeclaration_) this)._Identifier);
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
                        _printer._Print("#region operator \"");
                        _printer._Print(((IDeclaration_) this)._Identifier);
                        _printer._Print("\"\n\n");
                        Metadata_._Declare(_printer, ((IDeclaration_) this)._Metadata);
                        _printer._Print("[QS.Fx.Reflection.Operator(");
                        ((IDeclaration_) this)._Metadata._Print(_printer);
                        _printer._Print(")]\npublic sealed class operator_");
                        _printer._Print(((IDeclaration_) this)._Identifier);
                        _DeclareParameters(_printer, ((IDeclaration_) this)._Parameters, false);
                        _printer._Shift(1);
                        _printer._Print("\n: ");
                        ((IOperator_)this)._OperatorClass._Print(_printer);
                        _printer._Print("\n");
                        _printer._Shift(-1);
                        _DeclareParameterConstraints(_printer, ((IDeclaration_)this)._Parameters);
                        _printer._Print("{\n");
                        _printer._Shift(1);
                        _printer._Print("#region constructor\n\npublic operator_");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("\n(\n");
                        _printer._Shift(1);
                        _DeclareParameters(_printer, ((IDeclaration_) this)._Parameters, true);
                        _printer._Shift(-1);
                        _printer._Print("\n)\n{\n");
                        _printer._Shift(1);
                        if (((IDeclaration_) this)._Parameters != null)
                        {
                            foreach (IParameter_ _parameter in ((IDeclaration_) this)._Parameters)
                            {
                                if (((IDeclaration_) _parameter)._Type == Declaration_.Type_._Value)
                                {
                                    _printer._Print("this.");
                                    _parameter._Print(_printer);
                                    _printer._Print(" = ");
                                    _parameter._Print(_printer);
                                    _printer._Print(";\n");
                                }
                            }
                        }
                        _printer._Shift(-1);
                        _printer._Print("}\n\n#endregion\n\n#region fields\n\n");
                        if (((IDeclaration_) this)._Parameters != null)
                        {
                            foreach (IParameter_ _parameter in ((IDeclaration_) this)._Parameters)
                            {
                                if (((IDeclaration_) _parameter)._Type == Declaration_.Type_._Value)
                                {
                                    _printer._Print("[QS.Fx.Base.Inspectable]\nprivate ");
                                    _parameter._Constraint._Print(_printer);
                                    _printer._Print(" ");
                                    _parameter._Print(_printer);
                                    _printer._Print(";\n");
                                }
                            }
                        }
                        _printer._Print("\n#endregion\n\n#region ");
                        ((IOperator_)this)._OperatorClass._Print(_printer);
                        _printer._Print(" members\n\n");
                        _printer._Print("void ");
                        ((IOperator_)this)._OperatorClass._Print(_printer);
                        _printer._Print(".compute");
                        IValue_[] _arguments = this._arguments;
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
                        IValue_[] _results = this._results;
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
                            _printer._Print("()\n");
                        else
                        {
                            _printer._Shift(-1);
                            _printer._Print("\n)\n");
                        }
                        _printer._Print("{\n");
                        _printer._Shift(1);
                        if (this._internals != null)
                        {
                            foreach (IValue_ _internal in this._internals)
                                _internal._Declare(_printer);
                            _printer._Print("\n");
                        }
                        _printer._Print("throw new NotImplementedException();\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\n#endregion\n");
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
