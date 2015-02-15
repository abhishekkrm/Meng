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
    public sealed class Operator_Primitive_ : Operator_, IOperator_Primitive_, IElement_
    {
        #region Constructor

        public Operator_Primitive_(Type_ _type)
            : base(_IdentifierOf(_type), null, Operator_.Type_._Primitive, null, null) // this null value needs to be fixed
        {
            throw new NotImplementedException();
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
        //    _Union,
        //    _Intersection,
        //    _Minimum,
        //    _Maximum,
        //    _Conjunction,
        //    _Disjunction,
        //    _Addition,
        //    _Substraction,
        //    _Multiplication,
        //    _Division,
        //    _Equals,
        //    _NotEquals,
        //    _LessThan,
        //    _LessThanOrEquals,
        //    _GreaterThan,
        //    _GreaterThanOrEquals,
        //    _Range,
        //    _Tuple,
        //    _Set,
        //    _Element
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
                //case Type_._Object:
                //    return "object";
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
                //case Type_._Object:
                //    return "QS.Fx.Object.Classes.IObject";
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IOperator_Primitive_ Members

        Operator_Primitive_.Type_ IOperator_Primitive_._Type
        {
            get { return this._type; }
        }

        #endregion
    }
}
