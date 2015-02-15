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
    [XmlType("BinaryOperation")]
    [XmlInclude(typeof(PropertyValue))]
    [XmlInclude(typeof(VariableValue))]
    [XmlInclude(typeof(ParentValue))]
    [XmlInclude(typeof(ChildrenValue))]
    [XmlInclude(typeof(GroupValue))]
    [XmlInclude(typeof(BinaryOperation))]
    [XmlInclude(typeof(UnaryOperation))]
    [XmlInclude(typeof(Number))]
    [XmlInclude(typeof(EmptySet))]
    [XmlInclude(typeof(Boolean))]
    public sealed class BinaryOperation : Expression
    {
        public BinaryOperation(BinaryOperator _operator, Expression _expression1, Expression _expression2)
        {
            this._operator = _operator;
            this._expression1 = _expression1;
            this._expression2 = _expression2;
        }

        public BinaryOperation()
        {
        }

        private BinaryOperator _operator;
        private Expression _expression1, _expression2;

        public override bool Equals(object obj)
        {
            if (obj is BinaryOperation)
            {
                BinaryOperation other = (BinaryOperation) obj;
                return base.Equals(obj) 
                    && _operator.Equals(other._operator)
                    && _expression1.Equals(other._expression1)
                    && _expression2.Equals(other._expression2);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ _operator.GetHashCode() ^ _expression1.GetHashCode() ^ _expression2.GetHashCode();
        }

        [XmlAttribute("Operator")]
        public BinaryOperator Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        [XmlElement("Value1")]
        public Expression Expression1
        {
            get { return _expression1; }
            set { _expression1 = value; }
        }
            
        [XmlElement("Value2")]
        public Expression Expression2
        {
            get { return _expression2; }
            set { _expression2 = value; }
        }

        public override ExpressionType ExpressionType
        {
            get { return ExpressionType.BinaryOperation; }
        }

        public override void Format(QS.Fx.Language.Output.Output o)
        {
            base.Format(o);
            FormatOperator(_operator, o);
            o.Append("(");
            _expression1.Format(o);
            o.Append(", ");
            _expression2.Format(o);
            o.Append(")");            
        }

        public static void FormatOperator(BinaryOperator x, QS.Fx.Language.Output.Output o)
        {
            switch (x)
            {
                case BinaryOperator.Add:
                    o.Append("add");
                    break;

                case BinaryOperator.Substract:
                    o.Append("substract");
                    break;

                case BinaryOperator.Multiply:
                    o.Append("multiply");
                    break;

                case BinaryOperator.Divide:
                    o.Append("divide");
                    break;

                case BinaryOperator.Union:
                    o.Append("union");
                    break;

                case BinaryOperator.Diff:
                    o.Append("diff");
                    break;

                case BinaryOperator.Intersect:
                    o.Append("intersect");
                    break;

                case BinaryOperator.Insert:
                    o.Append("insert");
                    break;

                case BinaryOperator.Remove:
                    o.Append("remove");
                    break;

                case BinaryOperator.Min:
                    o.Append("min");
                    break;

                case BinaryOperator.Max:
                    o.Append("max");
                    break;

                case BinaryOperator.Or:
                    o.Append("or");
                    break;

                case BinaryOperator.And:
                    o.Append("and");
                    break;

                case BinaryOperator.EQ:
                    o.Append("eq");
                    break;

                case BinaryOperator.NEQ:
                    o.Append("neq");
                    break;

                case BinaryOperator.LT:
                    o.Append("lt");
                    break;

                case BinaryOperator.LTE:
                    o.Append("lte");
                    break;

                case BinaryOperator.GT:
                    o.Append("gt");
                    break;

                case BinaryOperator.GTE:
                    o.Append("gte");
                    break;

                case BinaryOperator.SubsetOf:
                    o.Append("subsetof");
                    break;

                case BinaryOperator.SupersetOf:
                    o.Append("supersetof");
                    break;

                case BinaryOperator.In:
                    o.Append("in");
                    break;

                case BinaryOperator.Contains:
                    o.Append("contains");
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
*/ 
}
