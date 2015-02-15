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
using System.Xml.Serialization;

namespace QS._qss_x_.Language_.AST_
{
/*
    [XmlType("UnaryOperation")]
    public sealed class UnaryOperation : Expression
    {
        public UnaryOperation(UnaryOperator _operator, Expression _expression)
        {
            this._operator = _operator;
            this._expression = _expression;
        }

        public UnaryOperation()
        {
        }

        private UnaryOperator _operator;
        private Expression _expression;

        public override bool Equals(object obj)
        {
            if (obj is UnaryOperation)
            {
                UnaryOperation other = (UnaryOperation)obj;
                return base.Equals(obj)
                    && _operator.Equals(other._operator)
                    && _expression.Equals(other._expression);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _operator.GetHashCode() ^ _expression.GetHashCode();
        }

        [XmlAttribute("Operator")]
        public UnaryOperator Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        [XmlElement("Value")]
        public Expression Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        public override ExpressionType ExpressionType
        {
            get { return ExpressionType.UnaryOperation; }
        }

        public override void Format(QS.Fx.Language.Output.Output o)
        {
            base.Format(o);
            FormatOperator(_operator, o);
            o.Append("(");
            _expression.Format(o);
            o.Append(")");
        }

        public static void FormatOperator(UnaryOperator x, QS.Fx.Language.Output.Output o)
        {
            switch (x)
            {
                case UnaryOperator.Minus:
                    o.Append("minus");
                    break;

                case UnaryOperator.Complement:
                    o.Append("complement");
                    break;

                case UnaryOperator.Not:
                    o.Append("not");
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
*/ 
}
