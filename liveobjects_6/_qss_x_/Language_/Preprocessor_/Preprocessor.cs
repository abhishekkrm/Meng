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

namespace QS._qss_x_.Language_.Preprocessor_
{
    public sealed class Preprocessor
    {
        #region Preprocess

        public static void Preprocess(QS._qss_x_.Language_.AST_.Protocol protocol)
        {
/*
            (new Preprocessing(protocol)).Preprocess();
*/ 
        }

        #endregion

/*
        #region Class Preprocessing

        private sealed class Preprocessing
        {
            #region Constructor

            public Preprocessing(QS.Fx.Language.AST.Protocol protocol)
            {
                this.protocol = protocol;

                properties = new Dictionary<string, QS.Fx.Language.AST.Property>();
                foreach (QS.Fx.Language.AST.Property property in protocol.Properties)
                    properties.Add(property.Name, property);
            }

            #endregion

            #region Fields

            private QS.Fx.Language.AST.Protocol protocol;
            private IDictionary<string, QS.Fx.Language.AST.Property> properties;

            #endregion

            #region Preprocess

            public void Preprocess()
            {
                _PreprocessRules();
            }

            #endregion

            #region _PreprocessRules

            private void _PreprocessRules()
            {
                // Queue<QS.Fx.Language.Structure.Rule> _rules = new Queue<QS.Fx.Language.Structure.Rule>();
                foreach (QS.Fx.Language.AST.Rule _rule in protocol.Rules)
                {
                    if (_rule.UpdateType != QS.Fx.Language.AST.UpdateType.Assignment)
                        throw new NotSupportedException();

                    QS.Fx.Language.AST.Property _property = _GetProperty(_rule.PropertyName);
                    QS.Fx.Language.AST.Expression _e = _rule.Expression;
                    QS.Fx.Language.AST.Placement _p = _rule.Placement;
                    _PreprocessExpression(ref _e, ref _p, true, _property.Type, null, null);
                    _rule.Expression = _e;
                    _rule.Placement = _p;

                    // _PreprocessExpression(protocol, _rule.Expression, _rule.UpdateType
                    // _rules.Enqueue(_rule);
                }

                //                while (_rules.Count > 0)
                //                {

                /-*
                                QS.Fx.Language.Structure.Rule _rule = _rules.Dequeue();
                                switch (_rule.Expression.ExpressionType)
                                {
                                    case QS.Fx.Language.Structure.ExpressionType.NewValue:
                                    case QS.Fx.Language.Structure.ExpressionType.OldValue:
                                    case QS.Fx.Language.Structure.ExpressionType.FunctionCall:
                                        throw new Exception("Expressions of type \"" + rule.Expression.ExpressionType.ToString() + 
                                            "\" are not supported in the body of a rule.");

                                    case QS.Fx.Language.Structure.ExpressionType.VariableValue:
                                        QS.Fx.Language.Structure.VariableValue _v = (QS.Fx.Language.Structure.VariableValue) _rule.Expression;
                                        _rule.Expression = new QS.Fx.Language.Structure.PropertyValue(_v.Name);
                                        break;

                                    case QS.Fx.Language.Structure.ExpressionType.Boolean:
                                        _rule.Expression.ValueType = new QS.Fx.Language.Structure.ValueType(
                                            QS.Fx.Language.Structure.PrimitiveType.Bool, QS.Fx.Language.Structure.ValueTypeAttributes.None);
                                        break;

                                    case QS.Fx.Language.Structure.ExpressionType.EmptySet:
                                        _rule.Expression.ValueType = new QS.Fx.Language.Structure.ValueType(
                                            QS.Fx.Language.Structure.PrimitiveType., QS.Fx.Language.Structure.ValueTypeAttributes.None);
                                        break;

                                    case QS.Fx.Language.Structure.ExpressionType.Integer:
                                        _rule = null;
                                        break;

                                    case QS.Fx.Language.Structure.ExpressionType.PropertyValue:
                                        protocol._Properties.C
                                        break;

                                    case QS.Fx.Language.Structure.ExpressionType.UnaryOperation:
                                    case QS.Fx.Language.Structure.ExpressionType.BinaryOperation:
                                    case QS.Fx.Language.Structure.ExpressionType.ChildrenValue:
                                    case QS.Fx.Language.Structure.ExpressionType.GroupValue:
                                    case QS.Fx.Language.Structure.ExpressionType.ParentValue:

                                }
                *-/
                //              }
            }

            #endregion

            #region _GetProperty

            private QS.Fx.Language.AST.Property _GetProperty(string propertyname)
            {
                QS.Fx.Language.AST.Property _property;
                if (!properties.TryGetValue(propertyname, out _property))
                    throw new Exception("Property \"" + propertyname + "\" has not been defined in this protocol.");
                return _property;
            }

            #endregion

            #region _PreprocessExpression

            private void _PreprocessExpression(ref QS.Fx.Language.AST.Expression expression, 
                ref QS.Fx.Language.AST.Placement placement, bool inrule, QS.Fx.Language.AST.ValueType expectedtype, 
                IDictionary<string, QS.Fx.Language.AST.ValueType> variabletypes, QS.Fx.Language.AST.ValueType updatedtype)
            {
                if (expectedtype != null && expectedtype.Base != QS.Fx.Language.AST.PrimitiveType.Undefined)
                {
                    if (expression.ValueType != null && expression.ValueType.Base != QS.Fx.Language.AST.PrimitiveType.Undefined)
                    {
                        // TODO: We could perform some sort of implicit casting at this point...
                        if (!expectedtype.Equals(expression.ValueType))
                            throw new Exception("The expected type of the expression (" + expectedtype.ToString() +
                                ") and the type assigned to the expression (" + expression.ValueType.ToString() + ") are incompatible.");
                    }
                    else
                    {
                        expression.ValueType = expectedtype;
                    }
                }

                switch (expression.ExpressionType)
                {
                    case QS.Fx.Language.AST.ExpressionType.NewValue:
                    case QS.Fx.Language.AST.ExpressionType.OldValue:
                        {
                            if (inrule || updatedtype == null)
                                throw new Exception(
                                    "Using keywords \"oldvalue\" and \"newvalue\" in context other than a property update handler is illegal.");
                            _PreprocessExpression(ref expression, expectedtype, updatedtype);
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.Boolean:
                        {
                            _PreprocessExpression(ref expression, expectedtype, new QS.Fx.Language.AST.ValueType(
                                QS.Fx.Language.AST.PrimitiveType.Bool, QS.Fx.Language.AST.ValueTypeAttributes.None));
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.Number:
                        {
                            _PreprocessExpression(ref expression, expectedtype, new QS.Fx.Language.AST.ValueType(
                                QS.Fx.Language.AST.PrimitiveType.Int, QS.Fx.Language.AST.ValueTypeAttributes.None));
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.EmptySet:
                        {
                            _PreprocessExpression(ref expression, expectedtype, new QS.Fx.Language.AST.ValueType(
                                QS.Fx.Language.AST.PrimitiveType.IntSet, QS.Fx.Language.AST.ValueTypeAttributes.None));
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.VariableValue:
                        {
                            QS.Fx.Language.AST.VariableValue _v = (QS.Fx.Language.AST.VariableValue) expression;
                            QS.Fx.Language.AST.ValueType variabletype;
                            if (!inrule && variabletypes.TryGetValue(_v.Name, out variabletype))
                                _PreprocessExpression(ref expression, expectedtype, variabletype);
                            else
                            {
                                expression = new QS.Fx.Language.AST.PropertyValue(_v.Name);
                                QS.Fx.Language.AST.Property property;
                                if (properties.TryGetValue(_v.Name, out property))
                                    _PreprocessExpression(ref expression, expectedtype, property.Type);
                                else
                                    throw new Exception("Neither a variable nor a property named \"" + _v.Name + "\" has been defined.");
                            }
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.PropertyValue:
                        {
                            QS.Fx.Language.AST.PropertyValue _v = (QS.Fx.Language.AST.PropertyValue) expression;
                            QS.Fx.Language.AST.Property property;
                            if (properties.TryGetValue(_v.Name, out property))
                            {
                                _ConstrainPlacementTo(ref placement, property.Placement);
                                _PreprocessExpression(ref expression, expectedtype, property.Type);
                            }
                            else
                                throw new Exception("A property named \"" + _v.Name + "\" has not been defined.");
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.ParentValue:
                        {
                            if (!inrule)
                                throw new Exception("Using the \"parent\" keyword is only permitted in the body of a rule.");
                            QS.Fx.Language.AST.ParentValue _v = (QS.Fx.Language.AST.ParentValue) expression;
                            QS.Fx.Language.AST.Expression _e = _v.Value;
                            QS.Fx.Language.AST.Placement _pp = QS.Fx.Language.AST.Placement.Undefined;
                            _PreprocessExpression(ref _e, ref _pp, inrule, expectedtype, variabletypes, updatedtype);
                            // we might need to do something about this undefined placement
                            _ConstrainPlacementTo(ref placement, QS.Fx.Language.AST.Placement.Child);
                            QS.Fx.Language.AST.PropertyValue _pe =
                                _FlattenToPropertyValue(_e, QS.Fx.Language.AST.Placement.Parent);
                            string _importedname = "_parent_" + _pe.Name;
                            protocol.Properties.Add(
                                new QS.Fx.Language.AST.Property(
                                    _importedname, new QS.Fx.Language.AST.ValueType(_pe.ValueType.Base, 
                                        _pe.ValueType.Attributes | QS.Fx.Language.AST.ValueTypeAttributes.Versioned),
                                    null, QS.Fx.Language.AST.Placement.Child, null));
                            protocol.Flows.Add(new QS.Fx.Language.AST.Dissemination(_pe.Name, _importedname));
                            QS.Fx.Language.AST.PropertyValue _ipe = new QS.Fx.Language.AST.PropertyValue(_importedname);
                            _ipe.ValueType = _pe.ValueType;
                            expression = _ipe;
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.ChildrenValue:
                        {
                            if (!inrule)
                                throw new Exception("Using the \"children\" keyword is only permitted in the body of a rule.");
                            QS.Fx.Language.AST.ChildrenValue _v = (QS.Fx.Language.AST.ChildrenValue)expression;
                            QS.Fx.Language.AST.Expression _e = _v.Value;
                            _PreprocessExpression(ref _e, ref placement, inrule, expectedtype, variabletypes, updatedtype);
                            _v.Value = _e;
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.GroupValue:
                        {
                            if (!inrule)
                                throw new Exception("Using the \"group\" keyword is only permitted in the body of a rule.");
                            QS.Fx.Language.AST.GroupValue _v = (QS.Fx.Language.AST.GroupValue)expression;
                            QS.Fx.Language.AST.Expression _e = _v.Value;
                            _PreprocessExpression(ref _e, ref placement, inrule, expectedtype, variabletypes, updatedtype);
                            _v.Value = _e;
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.BinaryOperation:
                        {
                            QS.Fx.Language.AST.BinaryOperation _bo = (QS.Fx.Language.AST.BinaryOperation)expression;
                            QS.Fx.Language.AST.Expression _e1 = _bo.Expression1;
                            _PreprocessExpression(ref _e1, ref placement, inrule, null, variabletypes, updatedtype);
                            _bo.Expression1 = _e1;
                            QS.Fx.Language.AST.Expression _e2 = _bo.Expression2;
                            _PreprocessExpression(ref _e2, ref placement, inrule, null, variabletypes, updatedtype);
                            _bo.Expression2 = _e2;

                            if (_e1.ValueType == null || _e1.ValueType.Base == QS.Fx.Language.AST.PrimitiveType.Undefined ||
                                _e2.ValueType == null || _e2.ValueType.Base == QS.Fx.Language.AST.PrimitiveType.Undefined)
                            {
                                throw new NotImplementedException();
                            }

                            // ..................
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.UnaryOperation:
                        {
                            QS.Fx.Language.AST.UnaryOperation _uo = (QS.Fx.Language.AST.UnaryOperation)expression;
                            QS.Fx.Language.AST.Expression _e = _uo.Expression;
                            _PreprocessExpression(ref _e, ref placement, inrule, null, variabletypes, updatedtype);
                            _uo.Expression = _e;

                            if (_e.ValueType == null || _e.ValueType.Base == QS.Fx.Language.AST.PrimitiveType.Undefined)
                            {
                                throw new NotImplementedException();
                            }

                            // ..................
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.FunctionCall:
                    default:
                        throw new NotImplementedException(
                            "Cannot perform inference for expressions of type \"" + expression.ExpressionType.ToString() + "\".");
                }
            }

            private void _PreprocessExpression(ref QS.Fx.Language.AST.Expression expression,
                QS.Fx.Language.AST.ValueType expectedtype, QS.Fx.Language.AST.ValueType inferredtype)
            {
                if (expectedtype != null && !expectedtype.Equals(inferredtype))
                    throw new Exception("The expected type of the expression (" + expectedtype.ToString() +
                        ") and the inferred type (" + inferredtype.ToString() + ") are incompatible.");

                if (expression.ValueType != null && !expression.ValueType.Equals(inferredtype))
                    throw new Exception("The type assigned to the expression (" + expression.ValueType.ToString() +
                        ") does not match the inferred type (" + inferredtype.ToString() + ").");

                expression.ValueType = inferredtype;
            }

            #endregion

            #region _UniqueExpressionName

            private static string _UniqueExpressionName(QS.Fx.Language.AST.Expression expression)
            {
                switch (expression.ExpressionType)
                {
                    case QS.Fx.Language.AST.ExpressionType.NewValue:
                        return "newvalue";

                    case QS.Fx.Language.AST.ExpressionType.OldValue:
                        return "oldvalue";

                    case QS.Fx.Language.AST.ExpressionType.Boolean:
                        return ((QS.Fx.Language.AST.Boolean)expression).Value.ToString();

                    case QS.Fx.Language.AST.ExpressionType.Number:
                        return ((QS.Fx.Language.AST.Number)expression).Value.ToString();

                    case QS.Fx.Language.AST.ExpressionType.EmptySet:
                        return "emptyset";

                    case QS.Fx.Language.AST.ExpressionType.VariableValue:
                        return ((QS.Fx.Language.AST.VariableValue)expression).Name;

                    case QS.Fx.Language.AST.ExpressionType.PropertyValue:
                        return ((QS.Fx.Language.AST.PropertyValue)expression).Name;

                    case QS.Fx.Language.AST.ExpressionType.ChildrenValue:
                        {
                            QS.Fx.Language.AST.ChildrenValue _v = (QS.Fx.Language.AST.ChildrenValue)expression;
                            return "children_" + _v.Attributes.ToString().Replace(",", "_") + "_" +
                                _v.AggregationOperator.ToString() + "_" + _UniqueExpressionName(_v.Value);
                        }

                    case QS.Fx.Language.AST.ExpressionType.GroupValue:
                        {
                            QS.Fx.Language.AST.GroupValue _v = (QS.Fx.Language.AST.GroupValue)expression;
                            return "children_" + _v.Attributes.ToString().Replace(",", "_") + "_" +
                                _v.AggregationOperator.ToString() + "_" + _UniqueExpressionName(_v.Value);
                        }

                    case QS.Fx.Language.AST.ExpressionType.ParentValue:
                        return "parent_" + _UniqueExpressionName(((QS.Fx.Language.AST.ParentValue)expression).Value);

                    case QS.Fx.Language.AST.ExpressionType.BinaryOperation:
                        {
                            QS.Fx.Language.AST.BinaryOperation _bo = (QS.Fx.Language.AST.BinaryOperation)expression;
                            return _bo.Operator.ToString() + "_" + _UniqueExpressionName(_bo.Expression1) + "_" +
                                _UniqueExpressionName(_bo.Expression2);
                        }

                    case QS.Fx.Language.AST.ExpressionType.UnaryOperation:
                        {
                            QS.Fx.Language.AST.UnaryOperation _uo = (QS.Fx.Language.AST.UnaryOperation)expression;
                            return _uo.Operator.ToString() + "_" + _UniqueExpressionName(_uo.Expression);
                        }

                    case QS.Fx.Language.AST.ExpressionType.FunctionCall:
                    default:
                        {
                            throw new NotImplementedException(
                                "Cannot generate name for expressions of type \"" + expression.ExpressionType.ToString() + "\".");
                        }
                }
            }

            #endregion

            #region _ConstrainPlacementTo

            private void _ConstrainPlacementTo(
                ref QS.Fx.Language.AST.Placement placement1, QS.Fx.Language.AST.Placement placement2)
            {
                placement1 |= placement2;
                switch (placement1)
                {
                    case QS.Fx.Language.AST.Placement.Local:
                    case QS.Fx.Language.AST.Placement.Global:
                    case QS.Fx.Language.AST.Placement.Child:
                    case QS.Fx.Language.AST.Placement.Parent:
                    case QS.Fx.Language.AST.Placement.Undefined:
                        break;

                    default:
                        throw new Exception("Illegal placement: " + placement1.ToString());
                }
            }

            #endregion

            #region _FlattenToPropertyValue

            private QS.Fx.Language.AST.PropertyValue _FlattenToPropertyValue(
                QS.Fx.Language.AST.Expression expression, QS.Fx.Language.AST.Placement placement)
            {
                QS.Fx.Language.AST.PropertyValue _pe;
                if (expression.ExpressionType == QS.Fx.Language.AST.ExpressionType.PropertyValue)
                    _pe = (QS.Fx.Language.AST.PropertyValue) expression;
                else
                {
                    string _propertyname = _UniqueExpressionName(expression);
                    QS.Fx.Language.AST.Property _property = 
                        new QS.Fx.Language.AST.Property(_propertyname, expression.ValueType, null, placement, null);
                    protocol.Properties.Add(_property);
                    properties.Add(_propertyname, _property);
                    _pe = new QS.Fx.Language.AST.PropertyValue(_propertyname);
                    _pe.ValueType = expression.ValueType;
                    protocol.Rules.Add(
                        new QS.Fx.Language.AST.Rule(
                            placement, QS.Fx.Language.AST.RuleAttributes.None,
                            QS.Fx.Language.AST.UpdateType.Assignment, _propertyname, expression));
                }
                return _pe;
            }

            #endregion
        }

        #endregion
*/
    }
}
