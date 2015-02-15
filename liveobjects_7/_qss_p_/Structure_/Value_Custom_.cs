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
    public sealed class Value_Custom_ : Value_, IValue_Custom_, IDeclaration_
    {
        #region Constructor

        public Value_Custom_(string _identifier, IMetadata_ _metadata, IEnumerable<IParameter_> _parameters, IValueClass_ _valueclass, IEnumerable<IValue_> _values)
            : base(_identifier, _metadata, Value_.Type_._Custom, _parameters, _valueclass)
        {
            if (_values != null)
                this._values = (new List<IValue_>(_values)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IValue_[] _values;

        #endregion

        #region IValue_Custom_ Members

        IValue_[] IValue_Custom_._Values
        {
            get { return this._values; }
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print("value_");
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
                    throw new NotImplementedException();

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        bool _isfirst;
                        _printer._Print("#region value \"");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("\"\n\n");
                        Metadata_._Declare(_printer, ((IDeclaration_)this)._Metadata);
                        _printer._Print("[QS.Fx.Reflection.Value(");
                        ((IDeclaration_)this)._Metadata._Print(_printer);
                        _printer._Print(")]\n");
                        _printer._Print("[QS.Fx.Printing.Printable(\"");
                        _printer._Print(((IDeclaration_) this)._Identifier);
                        _printer._Print("\", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]\n");
                        _printer._Print("[System.Xml.Serialization.XmlType(\"");
                        _printer._Print(((IDeclaration_)this)._Metadata._Identifier);
                        _printer._Print("\")]\n");
                        //_printer._Print("// [QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]\n");
                        _printer._Print("public sealed class value_");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        Declaration_._DeclareParameters(_printer, ((IDeclaration_)this)._Parameters, false);
                        _printer._Shift(1);
                        _printer._Print("\n: QS.Fx.Inspection.Inspectable, ");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(", System.IComparable, System.IEquatable<");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(">, System.IComparable<");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(">");
                        _printer._Print("\n");
                        _printer._Shift(-1);
                        _DeclareParameterConstraints(_printer, ((IDeclaration_)this)._Parameters);
                        _printer._Print("{\n");
                        _printer._Shift(1);
                        _printer._Print("#region constructors\n\npublic value_");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("()\n{\n}\n\npublic value_");
                        _printer._Print(((IDeclaration_)this)._Identifier);
                        _printer._Print("(");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(" _other)\n{\n");
                        _printer._Shift(1);
                        if (this._values != null)
                        {
                            foreach (IValue_ _value in this._values)
                            {
                                _printer._Print("this._");
                                _printer._Print(_value._Identifier);
                                _printer._Print(" = _other.");
                                _printer._Print(_value._Identifier);
                                _printer._Print(";\n");
                            }
                        }
                        _printer._Shift(-1);
                        _printer._Print("}\n\n");
                        if (this._values != null)
                        {
                            _printer._Print("public value_");
                            _printer._Print(((IDeclaration_)this)._Identifier);
                            _printer._Print("(\n");
                            _printer._Shift(1);
                            _isfirst = true;
                            foreach (IValue_ _value in this._values)
                            {
                                if (_isfirst)
                                    _isfirst = false;
                                else
                                    _printer._Print(",\n");
                                _value._ValueClass._Print(_printer);
                                _printer._Print(" _");
                                _printer._Print(_value._Identifier);
                            }
                            _printer._Shift(-1);
                            _printer._Print(")\n{\n");
                            _printer._Shift(1);
                            foreach (IValue_ _value in this._values)
                            {
                                _printer._Print("this._");
                                _printer._Print(_value._Identifier);
                                _printer._Print(" = _");
                                _printer._Print(_value._Identifier);
                                _printer._Print(";\n");
                            }
                            _printer._Shift(-1);
                            _printer._Print("}\n\n");
                        }
                        //_printer._Print("public value_");
                        //_printer._Print(((IDeclaration_)this)._Identifier);
                        //_printer._Print("\n(\n");
                        //_printer._Shift(1);
                        //Declaration_._DeclareParameters(_printer, ((IDeclaration_)this)._Parameters, true, false);
                        //_printer._Shift(-1);
                        //if (!_printer._Blank)
                        //    _printer._Print("\n");
                        //_printer._Print(")\n{\n");
                        //_printer._Shift(1);
                        //_printer._Print("// do something here...\n");
                        //_printer._Shift(-1);
                        //_printer._Print("}\n\n");
                        //_printer._Print("public static implicit operator ");
                        //_printer._Print(((IDeclaration_) this)._Identifier);
                        //_printer._Print("(");
                        //((IValue_)this)._ValueClass._Print(_printer);
                        //_printer._Print("_other)\n{\n");
                        //_printer._Shift(1);
                        //_printer._Print("return new ");
                        //_printer._Print(((IDeclaration_)this)._Identifier);
                        //_printer._Print("(_other);\n");
                        //_printer._Shift(-1);        
                        //_printer._Print("}\n\n");
                        _printer._Print("#endregion\n\n");
                        if (this._values != null)
                        {
                            _printer._Print("#region fields\n\n");
                            foreach (IValue_ _value in this._values)
                            {
                                _printer._Print("[QS.Fx.Printing.Printable(\"");
                                _printer._Print(_value._Identifier);
                                _printer._Print("\")]\n[QS.Fx.Base.Inspectable(\"");
                                _printer._Print(_value._Identifier);
                                _printer._Print("\")]\nprivate ");
                                _value._ValueClass._Print(_printer);
                                _printer._Print(" _");
                                _printer._Print(_value._Identifier);
                                _printer._Print(";\n");
                            }
                            _printer._Print("\n#endregion\n\n#region accessors\n\n");
                            foreach (IValue_ _value in this._values)
                            {
                                _printer._Print("[System.Xml.Serialization.XmlIgnore]\n");
                                _value._ValueClass._Print(_printer);
                                _printer._Print(" ");
                                ((IValue_) this)._ValueClass._Print(_printer);
                                _printer._Print(".");
                                _printer._Print(_value._Identifier);
                                _printer._Print("\n{\n");
                                _printer._Shift(1);
                                _printer._Print("get { return this._");
                                _printer._Print(_value._Identifier);
                                _printer._Print("; }\n");
                                _printer._Shift(-1);
                                _printer._Print("}\n\n[System.Xml.Serialization.XmlElement(\"");
                                _printer._Print(_value._Identifier);                                
                                _printer._Print("\")]\npublic ");
                                _value._ValueClass._Print(_printer);
                                _printer._Print(" ");
                                _printer._Print(_value._Identifier);
                                _printer._Print("\n{\n");
                                _printer._Shift(1);
                                _printer._Print("get { return this._");
                                _printer._Print(_value._Identifier);
                                _printer._Print("; }\nset { this._");
                                _printer._Print(_value._Identifier);
                                _printer._Print(" = value; }\n");
                                _printer._Shift(-1);
                                _printer._Print("}\n\n");
                            }
                            _printer._Print("#endregion\n\n");
                        }
                        _printer._Print("#region plumbing\n\npublic override string ToString()\n{\n");
                        _printer._Shift(1);
                        _printer._Print("return QS.Fx.Printing.Printable.ToString(this);\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\npublic override bool Equals(object _object)\n{\n");
                        _printer._Shift(1);
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(" _other = _object as ");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(";\nreturn ((IEquatable<");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(">) this).Equals(_other);\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\npublic override int GetHashCode()\n{\n");
                        _printer._Shift(1);
                        _printer._Print("return ");
                        _printer._Shift(1);
                        _isfirst = true;
                        foreach (IValue_ _value in this._values)
                        {
                            if (_isfirst)
                                _isfirst = false;
                            else
                                _printer._Print("\n^ ");
                            _printer._Print("this._");
                            _printer._Print(_value._Identifier);
                            _printer._Print(".GetHashCode()");
                        }
                        _printer._Shift(-2);
                        _printer._Print(";\n}\n\nint System.IComparable.CompareTo(object _object)\n{\n");
                        _printer._Shift(1);
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(" _other = _object as ");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(";\nif (_other == null)\n");
                        _printer._Shift(1);
                        _printer._Print("throw new ArgumentException();\n");
                        _printer._Shift(-1);
                        _printer._Print("return ((System.IComparable<");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(">) this).CompareTo(_other);\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\nbool System.IEquatable<");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(">.Equals(");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(" _other)\n{\n");
                        _printer._Shift(1);
                        _printer._Print("return (_other != null)");
                        _printer._Shift(1);
                        foreach (IValue_ _value in this._values)
                        {
                            _printer._Print("\n&& (this._");
                            _printer._Print(_value._Identifier);
                            _printer._Print(".Equals(_other.");
                            _printer._Print(_value._Identifier);
                            _printer._Print("))");
                        }
                        _printer._Shift(-2);
                        _printer._Print(";\n}\n\nint System.IComparable<");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(">.CompareTo(");
                        ((IValue_)this)._ValueClass._Print(_printer);
                        _printer._Print(" _other)\n{\n");
                        _printer._Shift(1);
                        _printer._Print("int _result = ");
                        _isfirst = true;
                        foreach (IValue_ _value in this._values)
                        {
                            if (_isfirst)
                                _isfirst = false;
                            else
                            {
                                _printer._Print("if (_result != 0)\n");
                                _printer._Shift(1);
                                _printer._Print("return _result;\n");                            
                                _printer._Shift(-1);
                                _printer._Print("_result = ");
                            }
                            _printer._Print("((System.IComparable) this._");
                            _printer._Print(_value._Identifier);
                            _printer._Print(").CompareTo(_other.");
                            _printer._Print(_value._Identifier);
                            _printer._Print(");\n");
                        }
                        _printer._Print("return _result;\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\n");
                        _printer._Print("#endregion\n\n#region serialization\n\nunsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo\n{\n");
                        _printer._Shift(1);
                        _printer._Print("get\n{\n");
                        _printer._Shift(1);
                        _printer._Print("throw new NotImplementedException();\n");
                        //_printer._Print("QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Nothing);\n");
                        //_printer._Print("return _info;\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\nunsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)\n{\n");
                        _printer._Shift(1);
                        _printer._Print("throw new NotImplementedException();\n");
                        _printer._Shift(-1);
                        _printer._Print("}\n\nunsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)\n{\n");
                        _printer._Shift(1);
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
