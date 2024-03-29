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
    public sealed class ValueClass_Primitive_ : ValueClass_, IValueClass_Primitive_, IElement_
    {
        #region Constructor

        public ValueClass_Primitive_(Type_ _type) : base(_IdentifierOf(_type), null, ValueClass_.Type_._Primitive)
        {
            this._type = _type;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Type_ _type;

        #endregion

        #region Type_

        public new enum Type_
        {
            _Value, _Boolean, _Integer
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    _printer._Print(_IdentifierOf(this._type));
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    _printer._Print(_ImplementationOf(this._type));
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _IdentifierOf

        private static string _IdentifierOf(Type_ _type)
        {
            switch (_type)
            {
                case Type_._Value:
                    return "value";
                case Type_._Boolean:
                    return "bool";
                case Type_._Integer:
                    return "int";
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _ImplementationOf

        private static string _ImplementationOf(Type_ _type)
        {
            switch (_type)
            {
                case Type_._Value:
                    return "QS.Fx.Value.Classes.IValue";
                case Type_._Boolean:
                    return "bool";
                case Type_._Integer:
                    return "int";
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IValueClass_Primitive_ Members

        ValueClass_Primitive_.Type_ IValueClass_Primitive_._Type
        {
            get { return this._type; }
        }

        #endregion

        #region Globals

        public static ValueClass_Primitive_ _Value
        {
            get { return new ValueClass_Primitive_(Type_._Value); }
        }

        public static ValueClass_Primitive_ _Boolean
        {
            get { return new ValueClass_Primitive_(Type_._Boolean); }
        }

        public static ValueClass_Primitive_ _Integer
        {
            get { return new ValueClass_Primitive_(Type_._Integer); }
        }

        #endregion
    }
}
