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

namespace QS._qss_x_.Language_.Formatter_
{
    public static class Formatter
    {
        #region Format

        public static string Format(QS._qss_x_.Language_.Structure_.Protocol x)
        {
            QS._qss_x_.Language_.Output_.Output o = new QS._qss_x_.Language_.Output_.Output();
            _FormatProtocol(o, x);
            return o.ToString();
        }

        public static string Format(QS._qss_x_.Language_.Structure_.Expression x)
        {
            QS._qss_x_.Language_.Output_.Output o = new QS._qss_x_.Language_.Output_.Output();
            _FormatExpression(o, x);
            return o.ToString();
        }

/*
        public static string Format(QS.Fx.Language.AST.Update x)
        {
            QS.Fx.Language.Output.Output o = new QS.Fx.Language.Output.Output();
            _FormatUpdate(o, x);
            return o.ToString();
        }
*/

        #endregion

        #region _FormatProtocol

        private static void _FormatProtocol(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Protocol x)
        {
            o.AppendLine("protocol " + x.Name);
            o.AppendLine("{");
            o.Indent(+1);
            _FormatInterface(o, x.Interface);
            o.AppendLine("properties");
            o.AppendLine("{");
            if (x.Properties != null)
            {
                o.Indent(+1);
                foreach (QS._qss_x_.Language_.Structure_.Property _property in x.Properties)
                    _FormatProperty(o, _property);
                o.Indent(-1);
            }
            o.AppendLine("}");
            o.AppendLine("bindings");
            o.AppendLine("{");
            if (x.Bindings != null)
            {
                o.Indent(+1);
                foreach (QS._qss_x_.Language_.Structure_.Binding _binding in x.Bindings)
                    _FormatBinding(o, _binding);
                o.Indent(-1);
            }
            o.AppendLine("}");
            o.AppendLine("flows");
            o.AppendLine("{");
            if (x.Flows != null)
            {
                o.Indent(+1);
                foreach (QS._qss_x_.Language_.Structure_.Flow _flow in x.Flows)
                    _FormatFlow(o, _flow);
                o.Indent(-1);
            }
            o.AppendLine("}");
            o.AppendLine("rules");
            o.AppendLine("{");
            if (x.Rules != null)
            {
                o.Indent(+1);
                foreach (QS._qss_x_.Language_.Structure_.Rule _rule in x.Rules)
                    _FormatRule(o, _rule);
                o.Indent(-1);
            }
            o.AppendLine("}");
            o.AppendLine("conditions");
            o.AppendLine("{");
            if (x.Conditions != null)
            {
                o.Indent(+1);
                foreach (QS._qss_x_.Language_.Structure_.Condition _condition in x.Conditions)
                    _FormatCondition(o, _condition);
                o.Indent(-1);
            }
            o.AppendLine("}");
            o.Indent(-1);
            o.AppendLine("}");
        }

        #endregion

        #region _FormatInterface

        private static void _FormatInterface(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Interface x)
        {
            o.AppendLine("interface");
            o.AppendLine("{");
            if (x != null)
            {
                o.Indent(+1);
                foreach (QS._qss_x_.Language_.Structure_.Method _method in x.Methods)
                    _FormatMethod(o, _method);
                o.Indent(-1);
            }
            o.AppendLine("}");
        }

        #endregion

        #region _FormatMethod

        private static void _FormatMethod(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Method x)
        {
            if (x.MethodCategory == QS._qss_x_.Language_.Structure_.MethodCategory.IncomingCall)
                o.Append("callback ");
            if (x.ResultType != null)
            {
                _FormatValueType(o, x.ResultType);
                o.Append(" ");
            }
            o.Append(x.Name + "(");
            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.ValueType _parametertype in x.ParameterTypes)
            {
                if (isfirst)
                    isfirst = false;
                else
                    o.Append(", ");
                _FormatValueType(o, _parametertype);
            }
            o.AppendLine(");");
        }

        #endregion

        #region _FormatValueType

        private static void _FormatValueType(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.ValueType x)
        {
            if (x.ValueTypeTemplate != null)
            {
                if (x.ValueTypeTemplateArguments == null || x.ValueTypeTemplateArguments.Length < 1)
                    throw new Exception("Invalid template type.");

                switch (x.ValueTypeTemplate.PredefinedTypeTemplate)
                {
                    case QS._qss_x_.Language_.Structure_.PredefinedTypeTemplate.VERSIONED:
                        o.Append("VERSIONED");
                        break;

                    default:
                        throw new NotImplementedException();
                }

                o.Append("<");

                bool isfirst = true;
                foreach (QS._qss_x_.Language_.Structure_.ValueType _valuetype in x.ValueTypeTemplateArguments)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        o.Append(", ");
                    _FormatValueType(o, _valuetype);
                }

                o.Append(">");
            }
            else
            {
                switch (x.PredefinedType)
                {
                    case QS._qss_x_.Language_.Structure_.PredefinedType._system_Boolean:
                        o.Append("bool");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType._system_Int32:
                        o.Append("int");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType._system_String:
                        o.Append("string");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType._system_UInt32:
                        o.Append("uint");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType.BOOL:
                        o.Append("BOOL");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType.UINT:
                        o.Append("UINT");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType.USET:
                        o.Append("USET");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType.VERSION:
                        o.Append("VERSION");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType.VOID:
                        o.Append("VOID");
                        break;
                    case QS._qss_x_.Language_.Structure_.PredefinedType.None:
                        {

                            throw new NotImplementedException();

                        }
                }
            }
        }

        #endregion

        #region _FormatProperty

        private static void _FormatProperty(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Property x)
        {
            if (x.Placement != QS._qss_x_.Language_.Structure_.Placement.Undefined)
            {
                _FormatPlacement(o, x.Placement, false);
                o.Append(" ");
            }

            if (x.Attributes != QS._qss_x_.Language_.Structure_.PropertyAttributes.None)
            {
                _FormatPropertyAttributes(o, x.Attributes);
                o.Append(" ");
            }

            _FormatValueType(o, x.ValueType);

            o.Append(" " + x.Name);

            if (x.InitialValue != null)
            {
                o.Append(" = ");
                _FormatExpression(o, x.InitialValue);
            }

            o.Append(";");

            if (x.Comment != null)
                o.Append(" // " + x.Comment);

            o.AppendLine();
        }

        #endregion

        #region _FormatPropertyAttributes

        private static void _FormatPropertyAttributes(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.PropertyAttributes x)
        {
            QS._qss_x_.Language_.Structure_.PropertyAttributes[] _a = 
            { 
                QS._qss_x_.Language_.Structure_.PropertyAttributes.Const, 
                QS._qss_x_.Language_.Structure_.PropertyAttributes.Alias
            };

            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.PropertyAttributes a in _a)
            {
                if ((x & a) == a)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        o.Append(", ");

                    switch (a)
                    {
                        case QS._qss_x_.Language_.Structure_.PropertyAttributes.Const:
                            o.Append("const");
                            break;

                        case QS._qss_x_.Language_.Structure_.PropertyAttributes.Alias:
                            o.Append("alias");
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region _FormatPlacement

        private static void _FormatPlacement(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Placement x, bool inrule)
        {
            switch (x)
            {
                case QS._qss_x_.Language_.Structure_.Placement.Local:
                    o.Append(inrule ? "{local}" : "{local}");
                    break;

                case QS._qss_x_.Language_.Structure_.Placement.Global:
                    o.Append(inrule ? "{global}" : "{global}");
                    break;

                case QS._qss_x_.Language_.Structure_.Placement.Child:
                    o.Append(inrule ? "{children}" : "{children}");
                    break;

                case QS._qss_x_.Language_.Structure_.Placement.Parent:
                    o.Append(inrule ? "{parent}" : "{parent}");
                    break;

                case QS._qss_x_.Language_.Structure_.Placement.Undefined:
                    break;

                default:
                    throw new NotImplementedException("Unrecognized placement: " + x.ToString());
            }
        }

        #endregion

        #region _FormatFlow

        private const string FLOW_ARROW = "<<<";

        private static void _FormatFlow(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Flow x)
        {
            switch (x.FlowCategory)
            {
                case QS._qss_x_.Language_.Structure_.FlowCategory.Aggregation:
                    _FormatAggregation(o, (QS._qss_x_.Language_.Structure_.Aggregation)x);
                    break;

                case QS._qss_x_.Language_.Structure_.FlowCategory.Dissemination:
                    _FormatDissemination(o, (QS._qss_x_.Language_.Structure_.Dissemination)x);
                    break;

/*
                case QS.Fx.Language.Structure.FlowCategory.:
                    _FormatGossip(o, (QS.Fx.Language.AST.Gossip)x);
                    break;
*/

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _FormatAggregation

        private static void _FormatAggregation(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Aggregation x)
        {
            if (x.GroupAggregate != null)
            {
                if (x.ParentAggregate != null)
                    o.Append(x.GroupAggregate.Protocol.Name + "." + x.GroupAggregate.Name + 
                        ", parent." + x.ParentAggregate.Protocol.Name + "." + x.ParentAggregate.Name);
                else
                    o.Append(x.GroupAggregate.Protocol.Name + "." + x.GroupAggregate.Name);
            }
            else
            {
                if (x.ParentAggregate != null)
                    o.Append("parent." + x.ParentAggregate.Protocol.Name + "." + x.ParentAggregate.Name);
                else
                    throw new Exception("Invalid aggregation.");
            }

            o.Append(" " + FLOW_ARROW + " ");

            if (x.AggregationAttributes != QS._qss_x_.Language_.Structure_.AggregationAttributes.None)
            {
                o.Append("[");
                _FormatAggregationAttributes(o, x.AggregationAttributes);
                o.Append("] ");
            }

            o.Append("group(");
            _FormatOperator(o, x.Operator);
            o.AppendLine(")." + x.SourceProperty.Name + ";");
        }

        #endregion

        #region _FormatDissemination

        private static void _FormatDissemination(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Dissemination x)
        {
            o.AppendLine(x.TargetProperty.Protocol.Name + "." + x.TargetProperty.Name + " " + FLOW_ARROW +
                " parent." + x.SourceProperty.Protocol.Name + "." + x.SourceProperty.Name + ";");
        }

        #endregion

/*
        #region _FormatGossip

        private static void _FormatGossip(QS.Fx.Language.Output.Output o, QS.Fx.Language.Structure.Gossip x)
        {
            throw new NotImplementedException();
        }

        #endregion
*/

        #region _FormatUpdate *****

        private static void _FormatUpdate(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Update x)
        {
            switch (x.UpcateCategory)
            {
                case QS._qss_x_.Language_.Structure_.UpdateCategory.Assignment:
                    {
                        QS._qss_x_.Language_.Structure_.Assignment _assignment = (QS._qss_x_.Language_.Structure_.Assignment) x;
                        switch (_assignment.Variable.VariableCategory)
                        {
                            case QS._qss_x_.Language_.Structure_.VariableCategory.Parameter:
                                o.Append(_assignment.Variable.Name);
                                break;

                            case QS._qss_x_.Language_.Structure_.VariableCategory.Property:
                                if (((QS._qss_x_.Language_.Structure_.Property) _assignment.Variable).Protocol != null)
                                    o.Append(((QS._qss_x_.Language_.Structure_.Property)_assignment.Variable).Protocol.Name + ".");
                                o.Append(_assignment.Variable.Name);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                        o.Append(" := ");
                        _FormatExpression(o, _assignment.Value);
                    }
                    break;

/*
                case QS.Fx.Language.AST.UpdateCategory.UpdatingOperation:
                    {
                        QS.Fx.Language.AST.UpdatingOperation _operation = (QS.Fx.Language.AST.UpdatingOperation)x;
                        o.Append(_operation.Target + " ");
                        _FormatOperator(o, _operation.Operator);
                        o.Append("= ");
                        if (_operation.Arguments == null || _operation.Arguments.Length < 2)
                            throw new Exception("Malformed updating operation.");
                        if (_operation.Arguments.Length > 2)
                        {
                            o.Append("(");
                            bool isfirst = true;
                            for (int _k = 1; _k < _operation.Arguments.Length; _k++)
                            {
                                if (isfirst)
                                    isfirst = false;
                                else
                                    o.Append(", ");
                                _FormatExpression(o, _operation.Arguments[_k]);
                            }
                            o.Append(")");
                        }
                        else
                        {
                            _FormatExpression(o, _operation.Arguments[1]);
                        }
                    }
                    break;
*/

                default:
                    throw new NotImplementedException();
            }

        }

        #endregion

        #region _FormatRule

        private static void _FormatRule(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Rule x)
        {
            if (x.Attributes != QS._qss_x_.Language_.Structure_.RuleAttributes.None)
            {
                o.Append("[");
                _FormatRuleAttributes(o, x.Attributes);
                o.Append("] ");
            }

            if (x.Placement != QS._qss_x_.Language_.Structure_.Placement.Undefined)
            {
                _FormatPlacement(o, x.Placement, true);
                o.Append(" ");
            }

            _FormatUpdate(o, x.Update);

            o.Append(";");

            if (x.Comment != null)
                o.Append(" // " + x.Comment);

            o.AppendLine();
        }

        #endregion

        #region _FormatRuleAttributes

        private static void _FormatRuleAttributes(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.RuleAttributes x)
        {
            QS._qss_x_.Language_.Structure_.RuleAttributes[] _a = 
            { 
                QS._qss_x_.Language_.Structure_.RuleAttributes.Mono, 
                QS._qss_x_.Language_.Structure_.RuleAttributes.Once, 
                QS._qss_x_.Language_.Structure_.RuleAttributes.Bootstrap,
                QS._qss_x_.Language_.Structure_.RuleAttributes.Strict
            };

            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.RuleAttributes a in _a)
            {
                if ((x & a) == a)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        o.Append(", ");

                    switch (a)
                    {
                        case QS._qss_x_.Language_.Structure_.RuleAttributes.Mono:
                            o.Append("mono");
                            break;

                        case QS._qss_x_.Language_.Structure_.RuleAttributes.Once:
                            o.Append("once");
                            break;

                        case QS._qss_x_.Language_.Structure_.RuleAttributes.Strict:
                            o.Append("strict");
                            break;

                        case QS._qss_x_.Language_.Structure_.RuleAttributes.Bootstrap:
                            o.Append("bootstrap");
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region _FormatCondition

        private static void _FormatCondition(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Condition x)
        {
            if (x.Attributes != QS._qss_x_.Language_.Structure_.ConditionAttributes.None)
            {
                o.Append("[");
                _FormatConditionAttributes(o, x.Attributes);
                o.Append("] ");
            }

            if (x.Placement != QS._qss_x_.Language_.Structure_.Placement.Undefined)
            {
                _FormatPlacement(o, x.Placement, true);
                o.Append(" ");
            }

            _FormatExpression(o, x.Expression);

            o.Append(";");

            if (x.Comment != null)
                o.Append(" // " + x.Comment);

            o.AppendLine();
        }

        #endregion

        #region _FormatConditionAttributes

        private static void _FormatConditionAttributes(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.ConditionAttributes x)
        {
            QS._qss_x_.Language_.Structure_.ConditionAttributes[] _a = 
            { 
                QS._qss_x_.Language_.Structure_.ConditionAttributes.Bootstrap,
                QS._qss_x_.Language_.Structure_.ConditionAttributes.Strict
            };

            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.ConditionAttributes a in _a)
            {
                if ((x & a) == a)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        o.Append(", ");

                    switch (a)
                    {
                        case QS._qss_x_.Language_.Structure_.ConditionAttributes.Strict:
                            o.Append("strict");
                            break;

                        case QS._qss_x_.Language_.Structure_.ConditionAttributes.Bootstrap:
                            o.Append("bootstrap");
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region _FormatExpression

        private static void _FormatExpression(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Expression x)
        {
            switch (x.ExpressionCategory)
            {
                case QS._qss_x_.Language_.Structure_.ExpressionCategory.Constant:
                    _FormatConstant(o, (QS._qss_x_.Language_.Structure_.Constant)x);
                    break;

                case QS._qss_x_.Language_.Structure_.ExpressionCategory.FunctionCall:
                    _FormatFunctionCall(o, (QS._qss_x_.Language_.Structure_.FunctionCall)x);
                    break;

                case QS._qss_x_.Language_.Structure_.ExpressionCategory.Operation:
                    _FormatOperation(o, (QS._qss_x_.Language_.Structure_.Operation)x);
                    break;

                case QS._qss_x_.Language_.Structure_.ExpressionCategory.ValueOf:
                    _FormatValueOf(o, (QS._qss_x_.Language_.Structure_.ValueOf)x);
                    break;
                
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _FormatConstant

        private static void _FormatConstant(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Constant x)
        {
            switch (x.ValueType.PredefinedType)
            {
                case QS._qss_x_.Language_.Structure_.PredefinedType._system_Boolean:
                    o.Append(((bool)x.Value) ? "true" : "false");
                    break;

                case QS._qss_x_.Language_.Structure_.PredefinedType._system_Int32:
                case QS._qss_x_.Language_.Structure_.PredefinedType.UINT:
                case QS._qss_x_.Language_.Structure_.PredefinedType.BOOL:
                    o.Append(x.Value.ToString());
                    break;

                case QS._qss_x_.Language_.Structure_.PredefinedType._system_UInt32:
                    o.Append(x.Value.ToString() + "u");
                    break;

                case QS._qss_x_.Language_.Structure_.PredefinedType.USET:
                    o.Append(x.Value.ToString() + "U");
                    break;

                default:
                    throw new NotImplementedException();
            }            
        }

        #endregion

        #region _FormatOperation

        private static void _FormatOperation(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Operation x)
        {
            _FormatOperator(o, x.Operator);
            if (x.Arguments != null)
            {
                o.Append("(");
                bool isfirst = true;
                foreach (QS._qss_x_.Language_.Structure_.Expression _expression in x.Arguments)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        o.Append(", ");
                    _FormatExpression(o, _expression);
                }
                o.Append(")");
            }
        }

        #endregion

        #region _FormatOperator

        private static void _FormatOperator(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Operator x)
        {
            if (x.Names != null && x.Names.Length > 0)
                o.Append(x.Names[0]);
            else
            {
                if (x.PredefinedOperators != null && x.PredefinedOperators.Length > 0)
                {
                    string _s = null;
                    foreach (QS._qss_x_.Language_.Structure_.PredefinedOperator _predefinedoperator in x.PredefinedOperators)
                    {
                        switch (_predefinedoperator)
                        {
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Add:
                                _s = "add";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.And:
                                _s = "and";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Complement:
                                _s = "complement";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Contains:
                                _s = "contains";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Create:
                                _s = "create";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Diff:
                                _s = "diff";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Divide:
                                _s = "divide";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Empty:
                                _s = "empty";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.EQ:
                                _s = "EQ";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.False:
                                _s = "false";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.GT:
                                _s = "gt";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.GTE:
                                _s = "gte";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.In:
                                _s = "in";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Insert:
                                _s = "insert";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Intersect:
                                _s = "intersect";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.IsDefined:
                                _s = "isdefined";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.LT:
                                _s = "LT";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.LTE:
                                _s = "LTE";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Max:
                                _s = "max";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Min:
                                _s = "min";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Minus:
                                _s = "minus";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Multiply:
                                _s = "multiply";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.NEQ:
                                _s = "NEQ";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Not:
                                _s = "not";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Or:
                                _s = "or";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Remove:
                                _s = "remove";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.SubsetOf:
                                _s = "subsetof";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Substract:
                                _s = "substract";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.SupersetOf:
                                _s = "supersetof";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.True:
                                _s = "true";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Undefined:
                                _s = "undefined";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Union:
                                _s = "union";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Value:
                                _s = "value";
                                break;
                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.Version:
                                _s = "version";
                                break;

                            case QS._qss_x_.Language_.Structure_.PredefinedOperator.None:
                            default:
                                break;
                        }
                        if (_s != null)
                            break;
                    }
                    if (_s == null)
                        throw new Exception("The operator is invalid, it does not have any names or predefined types associated with it.");
                    else
                        o.Append(_s);
                }
                else
                    throw new Exception("The operator is invalid, it does not have any names or predefined types associated with it.");
            }
            o.Append("{");
            _FormatValueType(o, x.ResultType);
            o.Append(" : ");
            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.ValueType _valuetype in x.ParameterTypes)
            {
                if (isfirst)
                    isfirst = false;
                else
                    o.Append(", ");
                _FormatValueType(o, _valuetype);
            }
            o.Append("}");
        }

        #endregion

        #region _FormatValueOf

        private static void _FormatValueOf(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.ValueOf x)
        {
            switch (x.Variable.VariableCategory)
            {
                case QS._qss_x_.Language_.Structure_.VariableCategory.Parameter:
                    o.Append(x.Variable.Name);
                    break;

                case QS._qss_x_.Language_.Structure_.VariableCategory.Property:
                    {
                        QS._qss_x_.Language_.Structure_.Property _property = (QS._qss_x_.Language_.Structure_.Property) x.Variable;
                        o.Append(_property.Protocol.Name + "." + _property.Name);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _FormatAggregationAttributes

        private static void _FormatAggregationAttributes(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.AggregationAttributes x)
        {
            QS._qss_x_.Language_.Structure_.AggregationAttributes[] _a = 
            { 
                QS._qss_x_.Language_.Structure_.AggregationAttributes.All, 
                QS._qss_x_.Language_.Structure_.AggregationAttributes.Strict, 
                QS._qss_x_.Language_.Structure_.AggregationAttributes.Mono
            };
            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.AggregationAttributes a in _a)
            {
                if ((x & a) == a)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        o.Append(", ");

                    switch (a)
                    {
                        case QS._qss_x_.Language_.Structure_.AggregationAttributes.All:
                            o.Append("all");
                            break;

                        case QS._qss_x_.Language_.Structure_.AggregationAttributes.Strict:
                            o.Append("strict");
                            break;

                        case QS._qss_x_.Language_.Structure_.AggregationAttributes.Mono:
                            o.Append("mono");
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region _FormatFunctionCall

        private static void _FormatFunctionCall(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.FunctionCall x)
        {
            o.Append("call " + x.OutgoingCall.Interface.Protocol.Name + "." + x.OutgoingCall.Name + "(");
            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.Expression _expression in x.Arguments)
            {
                if (isfirst)
                    isfirst = false;
                else
                    o.Append(", ");
                _FormatExpression(o, _expression);
            }
            o.Append(")");
        }

        #endregion

        #region _FormatBinding

        private static void _FormatBinding(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Binding x)
        {
            o.Append("on ");
            switch (x.BindingCategory)
            {
                case QS._qss_x_.Language_.Structure_.BindingCategory.OnInitialization:
                    o.Append("initialization");
                    break;

                case QS._qss_x_.Language_.Structure_.BindingCategory.OnUpdate:
                    {
                        QS._qss_x_.Language_.Structure_.OnUpdate _onupdate = (QS._qss_x_.Language_.Structure_.OnUpdate) x;
                        o.Append("update " + _onupdate.Property.Protocol.Name + "." + _onupdate.Property.Name + "(" +
                            _onupdate.OldValue.Name + ", " + _onupdate.NewValue.Name + ")");
                    }
                    break;

                case QS._qss_x_.Language_.Structure_.BindingCategory.OnIncomingCall:
                    {
                        QS._qss_x_.Language_.Structure_.OnIncomingCall _oncallback = (QS._qss_x_.Language_.Structure_.OnIncomingCall) x;
                        o.Append(_oncallback.IncomingCall.Interface.Protocol.Name + "." + _oncallback.IncomingCall.Name + "(");
                        bool isfirst = true;
                        foreach (QS._qss_x_.Language_.Structure_.Parameter _parameter in _oncallback.Parameters)
                        {
                            if (isfirst)
                                isfirst = false;
                            else
                                o.Append(", ");
                            _FormatParameter(o, _parameter);
                        }
                        o.Append(")");
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            o.AppendLine(" : ");
            if (x.Action.ActionCategory == QS._qss_x_.Language_.Structure_.ActionCategory.Sequence)
                _FormatAction(o, x.Action);
            else
            {
                o.Indent(+1);
                _FormatAction(o, x.Action);
                o.Indent(-1);
            }
        }

        #endregion

        #region _FormatParameter

        private static void _FormatParameter(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Parameter x)
        {
            _FormatValueType(o, x.ValueType);
            o.Append(" " + x.Name);
        }

        #endregion

        #region _FormatAction

        private static void _FormatAction(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Action x)
        {
            switch (x.ActionCategory)
            {
                case QS._qss_x_.Language_.Structure_.ActionCategory.Update:
                    _FormatUpdate(o, (QS._qss_x_.Language_.Structure_.Update )x);
                    o.AppendLine(";");
                    break;

                case QS._qss_x_.Language_.Structure_.ActionCategory.MethodCall:
                    _FormatMethodCall(o, (QS._qss_x_.Language_.Structure_.MethodCall) x);
                    break;

                case QS._qss_x_.Language_.Structure_.ActionCategory.Sequence:
                    _FormatSequence(o, (QS._qss_x_.Language_.Structure_.Sequence) x);
                    break;

                case QS._qss_x_.Language_.Structure_.ActionCategory.Loop:
                    _FormatLoop(o, (QS._qss_x_.Language_.Structure_.Loop)x);
                    break;

                case QS._qss_x_.Language_.Structure_.ActionCategory.Choice:
                    _FormatChoice(o, (QS._qss_x_.Language_.Structure_.Choice)x);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _FormatMethodCall

        private static void _FormatMethodCall(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.MethodCall x)
        {
            o.Append("call " + x.OutgoingCall.Name + "(");
            bool isfirst = true;
            foreach (QS._qss_x_.Language_.Structure_.Expression _expression in x.Arguments)
            {
                if (isfirst)
                    isfirst = false;
                else
                    o.Append(", ");
                _FormatExpression(o, _expression);
            }
            o.AppendLine(");");
        }

        #endregion

        #region _FormatSequence

        private static void _FormatSequence(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Sequence x)
        {
            o.AppendLine("{");
            o.Indent(+1);
            foreach (QS._qss_x_.Language_.Structure_.Action _action in x.Actions)
                _FormatAction(o, _action);
            o.Indent(-1);
            o.AppendLine("}");
        }

        #endregion

        #region _FormatLoop

        private static void _FormatLoop(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Loop x)
        {
            o.Append("foreach (");
            _FormatParameter(o, x.Parameter);
            o.Append(" in ");
            _FormatExpression(o, x.Collection);
            o.AppendLine(")");
            if (x.Action.ActionCategory == QS._qss_x_.Language_.Structure_.ActionCategory.Sequence)
                _FormatAction(o, x.Action);
            else
            {
                o.Indent(+1);
                _FormatAction(o, x.Action);
                o.Indent(-1);
            }
        }

        #endregion

        #region _FormatChoice

        private static void _FormatChoice(QS._qss_x_.Language_.Output_.Output o, QS._qss_x_.Language_.Structure_.Choice x)
        {
            o.Append("if (");
            _FormatExpression(o, x.Expression);
            o.AppendLine(")");
            if (x.IfAction.ActionCategory == QS._qss_x_.Language_.Structure_.ActionCategory.Sequence)
                _FormatAction(o, x.IfAction);
            else
            {
                o.Indent(+1);
                _FormatAction(o, x.IfAction);
                o.Indent(-1);
            }
            if (x.ElseAction != null)
            {
                o.AppendLine("else");
                if (x.ElseAction.ActionCategory == QS._qss_x_.Language_.Structure_.ActionCategory.Sequence)
                    _FormatAction(o, x.ElseAction);
                else
                {
                    o.Indent(+1);
                    _FormatAction(o, x.ElseAction);
                    o.Indent(-1);
                }
            }
        }

        #endregion

/*
        #region _FormatSet

        private static void _FormatSet(QS.Fx.Language.Output.Output o, QS.Fx.Language.AST.Set x)
        {
            o.Append("{");
            bool isfirst = true;
            foreach (QS.Fx.Language.AST.Elements _elements in x.Elements)
            {
                if (isfirst)
                    isfirst = false;
                else
                    o.Append(",");
                o.Append(" ");
                switch (_elements.ElementsCategory)
                {
                    case QS.Fx.Language.AST.ElementsCategory.Element:
                        _FormatExpression(o, ((QS.Fx.Language.AST.Element)_elements).Value);
                        break;

                    case QS.Fx.Language.AST.ElementsCategory.Range:
                        _FormatRange(o, (QS.Fx.Language.AST.Range)_elements);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            o.Append(" }");
        }

        #endregion

        #region _FormatRange

        private static void _FormatRange(QS.Fx.Language.Output.Output o, QS.Fx.Language.AST.Range x)
        {
            _FormatExpression(o, x.From);
            o.Append("..");
            _FormatExpression(o, x.To);
        }

        #endregion

        #region _FormatTypeCast

        private static void _FormatTypeCast(QS.Fx.Language.Output.Output o, QS.Fx.Language.AST.TypeCast x)
        {
            if (x.ValueType != null)
            {
                o.Append("(");
                _FormatValueType(o, x.ValueType);
                o.Append(") ");
            }

            _FormatExpression(o, x.Value);
        }

        #endregion
 */ 

    }
}
