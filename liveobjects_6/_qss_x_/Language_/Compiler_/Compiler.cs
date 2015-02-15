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

namespace QS._qss_x_.Language_.Compiler_
{
    public static class Compiler
    {
        #region Compile

        public static string Compile(QS._qss_x_.Language_.AST_.Protocol x)
        {
            return string.Empty;
/*
            return (new Compilation(x)).ToString();
*/ 
        }

        #endregion

/*
        #region Class Compilation

        private class Compilation
        {
            #region Constructor

            public Compilation(QS.Fx.Language.AST.Protocol _protocol)
            {
                this._protocol = _protocol;

                this._interface_o = new Output.Output();
                this._callback_declarations_o = new Output.Output();
                this._aggregationtoken_fields_o = new Output.Output();
                this._aggregationtoken_serializableinfo_o = new Output.Output();
                this._aggregationtoken_serializeto_o = new Output.Output();
                this._aggregationtoken_deserializefrom_o = new Output.Output();
                this._disseminationtoken_fields_o = new Output.Output();
                this._disseminationtoken_serializableinfo_o = new Output.Output();
                this._disseminationtoken_serializeto_o = new Output.Output();
                this._disseminationtoken_deserializefrom_o = new Output.Output();
                this._control_additional_fields_o = new Output.Output();
                this._bind_additional_fields_o = new Output.Output();
                this._bind_initialization_o = new Output.Output();
                this._bind_register_callbacks_o = new Output.Output();
                this._bind_callbacks_o = new Output.Output();
                this._bind_updated_upper_o = new Output.Output();
                this._peer_additional_fields_o = new Output.Output();
                this._peer_initialization_o = new Output.Output();
                this._peer_updated_lower_bind_o = new Output.Output();
                this._peer_updated_lower_peer_o = new Output.Output();
                this._peer_upper_consume_1_o = new Output.Output();
                this._peer_upper_consume_2_arguments_o = new Output.Output();
                this._peer_upper_consume_2_parameters_o = new Output.Output();
                this._peer_upper_consume_2_o = new Output.Output();
                this._peer_upper_consume_3_o = new Output.Output();
                this._check_conditions_o = new Output.Output();
                this._peer_aggregate_1_o = new Output.Output();
                this._peer_aggregate_2_o = new Output.Output();
                this._peer_aggregate_3_o = new Output.Output();
                this._peer_disseminate_1_o = new Output.Output();
                this._peer_disseminate_2_and_3_o = new Output.Output();
                this._root_additional_fields_o = new Output.Output();
                this._root_initialization_o = new Output.Output();
                this._root_updated_lower_o = new Output.Output();

                _CompileProtocol();
            }

            #endregion

            #region Fields

            private QS.Fx.Language.AST.Protocol _protocol;

            private Output.Output _interface_o;
            private Output.Output _callback_declarations_o;
            private Output.Output _aggregationtoken_fields_o;
            private Output.Output _aggregationtoken_serializableinfo_o;
            private Output.Output _aggregationtoken_serializeto_o;
            private Output.Output _aggregationtoken_deserializefrom_o;
            private Output.Output _disseminationtoken_fields_o;
            private Output.Output _disseminationtoken_serializableinfo_o;
            private Output.Output _disseminationtoken_serializeto_o;
            private Output.Output _disseminationtoken_deserializefrom_o;
            private Output.Output _control_additional_fields_o;
            private Output.Output _bind_additional_fields_o;
            private Output.Output _bind_initialization_o;
            private Output.Output _bind_register_callbacks_o;
            private Output.Output _bind_callbacks_o;
            private Output.Output _bind_updated_upper_o;
            private Output.Output _peer_additional_fields_o;
            private Output.Output _peer_initialization_o;
            private Output.Output _peer_updated_lower_bind_o;
            private Output.Output _peer_updated_lower_peer_o;
            private Output.Output _peer_upper_consume_1_o;
            private Output.Output _peer_upper_consume_2_arguments_o;
            private Output.Output _peer_upper_consume_2_parameters_o;
            private Output.Output _peer_upper_consume_2_o;
            private Output.Output _peer_upper_consume_3_o;
            private Output.Output _peer_aggregate_1_o;
            private Output.Output _peer_aggregate_2_o;
            private Output.Output _peer_aggregate_3_o;
            private Output.Output _peer_disseminate_1_o;
            private Output.Output _peer_disseminate_2_and_3_o;
            private Output.Output _check_conditions_o;
            private Output.Output _root_additional_fields_o;
            private Output.Output _root_initialization_o;
            private Output.Output _root_updated_lower_o;

            #endregion

            #region ToString

            public override string ToString()
            {
                Dictionary<string, string> substitutions = new Dictionary<string, string>();

                substitutions.Add("// BIND_INITIALIZATION", _bind_initialization_o.ToString());
                substitutions.Add("// PEER_INITIALIZATION", _peer_initialization_o.ToString());
                substitutions.Add("// ROOT_INITIALIZATION", _root_initialization_o.ToString());

                substitutions.Add("// PEER_AGGREGATE_1", _peer_aggregate_1_o.ToString());
                substitutions.Add("// PEER_AGGREGATE_2", _peer_aggregate_2_o.ToString());
                substitutions.Add("// PEER_AGGREGATE_3", _peer_aggregate_3_o.ToString());
                substitutions.Add("// PEER_DISSEMINATE_1", _peer_disseminate_1_o.ToString());
                substitutions.Add("// PEER_DISSEMINATE_2_AND_3", _peer_disseminate_2_and_3_o.ToString());

                substitutions.Add("// BIND_UPDATED_UPPER", _bind_updated_upper_o.ToString());
                substitutions.Add("// PEER_UPDATED_LOWER_BIND", _peer_updated_lower_bind_o.ToString());
                substitutions.Add("// PEER_UPDATED_LOWER_PEER", _peer_updated_lower_peer_o.ToString());
                substitutions.Add("// PEER_UPPER_CONSUME_1", _peer_upper_consume_1_o.ToString());
                substitutions.Add("// PEER_UPPER_CONSUME_2_ARGUMENTS", _peer_upper_consume_2_arguments_o.ToString());
                substitutions.Add("// PEER_UPPER_CONSUME_2_PARAMETERS", _peer_upper_consume_2_parameters_o.ToString());
                substitutions.Add("// PEER_UPPER_CONSUME_2", _peer_upper_consume_2_o.ToString());
                substitutions.Add("// PEER_UPPER_CONSUME_3", _peer_upper_consume_3_o.ToString());
                substitutions.Add("// ROOT_UPDATED_LOWER", _root_updated_lower_o.ToString());

                substitutions.Add("// CHECK_CONDITIONS", _check_conditions_o.ToString());

                // ********************************************************************************************************************************************

                substitutions.Add("PROTOCOL", _protocol.Name);
                substitutions.Add("// INTERFACE", _interface_o.ToString());
                substitutions.Add("// CALLBACK_DECLARATIONS", _callback_declarations_o.ToString());
                substitutions.Add("// AGGREGATIONTOKEN_FIELDS", _aggregationtoken_fields_o.ToString());
                substitutions.Add("// AGGREGATIONTOKEN_SERIALIZABLEINFO", _aggregationtoken_serializableinfo_o.ToString());
                substitutions.Add("// AGGREGATIONTOKEN_SERIALIZETO", _aggregationtoken_serializeto_o.ToString());
                substitutions.Add("// AGGREGATIONTOKEN_DESERIALIZEFROM", _aggregationtoken_deserializefrom_o.ToString());
                substitutions.Add("// DISSEMINATIONTOKEN_FIELDS", _disseminationtoken_fields_o.ToString());
                substitutions.Add("// DISSEMINATIONTOKEN_SERIALIZABLEINFO", _disseminationtoken_serializableinfo_o.ToString());
                substitutions.Add("// DISSEMINATIONTOKEN_SERIALIZETO", _disseminationtoken_serializeto_o.ToString());
                substitutions.Add("// DISSEMINATIONTOKEN_DESERIALIZEFROM", _disseminationtoken_deserializefrom_o.ToString());                
                substitutions.Add("// CONTROL_ADDITIONAL_FIELDS", _control_additional_fields_o.ToString());
                substitutions.Add("// BIND_ADDITIONAL_FIELDS", _bind_additional_fields_o.ToString());
                substitutions.Add("// PEER_ADDITIONAL_FIELDS", _peer_additional_fields_o.ToString());
                substitutions.Add("// ROOT_ADDITIONAL_FIELDS", _root_additional_fields_o.ToString());
                substitutions.Add("// BIND_REGISTER_CALLBACKS", _bind_register_callbacks_o.ToString());
                substitutions.Add("// BIND_CALLBACKS", _bind_callbacks_o.ToString());

                StringBuilder _o = new StringBuilder();
                foreach (KeyValuePair<string, string> _s in substitutions)
                    _o.AppendLine("// " + (new string('-', 80)) + "\n// ***** " + _s.Key + "\n#region\n/" + "*\n" + _s.Value + "\n*" + "/\n#endregion\n");
                _o.AppendLine("// " + new string('-', 80));
                return _o.ToString();

                // return Output.Output2.Format(Template.TEMPLATE, substitutions);
            }

            #endregion

            #region _CompileProtocol

            private void _CompileProtocol()
            {
                _CompileInterface();
                _CompileProperties();
                _CompileBindings();
                _CompileFlows();

                // ...........................................................................................................................................................................................
            }

            #endregion

            #region _CompileInterface

            private void _CompileInterface()
            {
                foreach (QS.Fx.Language.AST.Method m in _protocol.Interface.Methods)
                {
                    if (m.MethodCategory == QS.Fx.Language.AST.MethodCategory.Callback)
                    {
                        _interface_o.AppendLine("event _" + m.Name + "Callback On" + m.Name + ";");
                        _callback_declarations_o.Append(
                            "public delegate " + _CompileInterfaceValueType(m.ResultType) + " _" + m.Name + "Callback(");
                        _CompileInterfaceParameters(m.Parameters, _callback_declarations_o);
                        _callback_declarations_o.AppendLine(");");
                        _bind_register_callbacks_o.AppendLine(
                            "endpoint.On" + m.Name + " += new _" + m.Name + "Callback(this._On" + m.Name + "Callback);");
                    }
                    else
                    {
                        _interface_o.Append(_CompileInterfaceValueType(m.ResultType) + " " + m.Name + "(");
                        _CompileInterfaceParameters(m.Parameters, _interface_o);
                        _interface_o.AppendLine(");");
                    }
                }
            }

            #endregion

            #region _CompileInterfaceParameters

            private void _CompileInterfaceParameters(IEnumerable<QS.Fx.Language.AST.Parameter> _parameters, Output.Output _o)
            {
                bool isfirst = true;
                foreach (QS.Fx.Language.AST.Parameter p in _parameters)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        _o.Append(", ");
                    _o.Append(_CompileInterfaceParameter(p));
                }
            }

            #endregion

            #region _CompileInterfaceParameter

            private string _CompileInterfaceParameter(QS.Fx.Language.AST.Parameter _parameter)
            {
                return _CompileInterfaceValueType(_parameter.ValueType) + " " + _parameter.Name;
            }

            #endregion

            #region _CompileInterfaceValueType

            private string _CompileInterfaceValueType(QS.Fx.Language.AST.ValueType _valuetype)
            {
                if (_valuetype.ValueTypeCategory != QS.Fx.Language.AST.ValueTypeAttributes.None)
                    throw new Exception("Attributes are not supported in this version of the compiler.");
                return _CompileInterfacePrimitiveType(_valuetype.Base);
            }

            #endregion

            #region _CompileInterfacePrimitiveType

            private string _CompileInterfacePrimitiveType(QS.Fx.Language.AST.PrimitiveType _type)
            {
                switch (_type)
                {
                    case QS.Fx.Language.AST.PrimitiveType.Void:
                        return "void";

                    case QS.Fx.Language.AST.PrimitiveType.Int:
                        return "uint";

                    case QS.Fx.Language.AST.PrimitiveType.Bool:
                    case QS.Fx.Language.AST.PrimitiveType.Id:
                    case QS.Fx.Language.AST.PrimitiveType.IntSet:
                    default:
                        throw new Exception("Type \"" + _type.ToString() + "\" as a part of the interface is not supported in this version of the compiler.");
                }
            }

            #endregion

            #region _CompileProperties

            private void _CompileProperties()
            {
                foreach (QS.Fx.Language.AST.Property _p in _protocol.Properties)
                {
                    string _property_name = _p.Name;
                    string _property_value_type_name = _CompileValueType(_p.Type);

                    switch (_p.Placement)
                    {
                        case QS.Fx.Language.AST.Placement.Undefined:
                            {
                                _control_additional_fields_o.AppendLine("[QS.TMS.Inspection.Inspectable]");
                                _control_additional_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _control_additional_fields_o.AppendLine(
                                    "public " + _property_value_type_name + " " + _property_name + ";");

                                // .......................................................................................................................................................................................
                            }
                            break;

                        case QS.Fx.Language.AST.Placement.Local:
                            {
                                _bind_additional_fields_o.AppendLine("[QS.TMS.Inspection.Inspectable]");
                                _bind_additional_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _bind_additional_fields_o.AppendLine(
                                    "public " + _property_value_type_name + " " + _property_name + ";");

                                // .......................................................................................................................................................................................
                            }
                            break;

                        case QS.Fx.Language.AST.Placement.Global:
                            {
                                _root_additional_fields_o.AppendLine("[QS.TMS.Inspection.Inspectable]");
                                _root_additional_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _root_additional_fields_o.AppendLine(
                                    "public " + _property_value_type_name + " " + _property_name + ";");

                                // .......................................................................................................................................................................................
                            }
                            break;

                        case QS.Fx.Language.AST.Placement.Child:
                            {
                                _peer_additional_fields_o.AppendLine("[QS.TMS.Inspection.Inspectable]");
                                _peer_additional_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _peer_additional_fields_o.AppendLine(
                                    "public " + _property_value_type_name + " " + _property_name + ";");

                                // .......................................................................................................................................................................................
                            }
                            break;

                        case QS.Fx.Language.AST.Placement.Parent:
                            {
                                _peer_additional_fields_o.AppendLine("// this property is unused on the leaf-level peers");
                                _peer_additional_fields_o.AppendLine("[QS.TMS.Inspection.Inspectable]");
                                _peer_additional_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _peer_additional_fields_o.AppendLine(
                                    "public " + _property_value_type_name + " " + _property_name + ";");

                                _root_additional_fields_o.AppendLine("[QS.TMS.Inspection.Inspectable]");
                                _root_additional_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _root_additional_fields_o.AppendLine(
                                    "public " + _property_value_type_name + " " + _property_name + ";");

                                // .......................................................................................................................................................................................
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            #endregion

            #region _CompileValueType

            private string _CompileValueType(QS.Fx.Language.AST.ValueType _valuetype)
            {
                string _result = _CompilePrimitiveType(_valuetype.Base);
                switch (_valuetype.Attributes)
                {
                    case QS.Fx.Language.AST.ValueTypeAttributes.None:
                        break;

                    case QS.Fx.Language.AST.ValueTypeAttributes.Versioned:
                        _result = "QS.Fx.Runtime.Versioned<" + _result + ">";
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return _result;
            }

            #endregion

            #region _CompilePrimitiveType

            private static string _CompilePrimitiveType(QS.Fx.Language.AST.PrimitiveType _type)
            {
                switch (_type)
                {
                    case QS.Fx.Language.AST.PrimitiveType.Int:
                        return "UInt";

                    case QS.Fx.Language.AST.PrimitiveType.IntSet:
                        return "UIntSet";

                    default:
                        throw new NotImplementedException();
                }
            }

            #endregion

            #region _CompileBindings

            private void _CompileBindings()
            {
                foreach (QS.Fx.Language.AST.Binding _b in _protocol.Bindings)
                {
                    switch (_b.Trigger.TriggerType)
                    {
                        #region compiling a callback

                        case QS.Fx.Language.AST.TriggerType.Callback:
                            {
                                QS.Fx.Language.AST.OnCallback _c = (QS.Fx.Language.AST.OnCallback) _b.Trigger;
                                QS.Fx.Language.AST.Method _method = _GetMethod(_c.Callback);
                                if (_method.MethodType != QS.Fx.Language.AST.MethodType.Callback)
                                    throw new Exception("Method " + _method.Name + " is not a callback.");
                                if (_method.Parameters.Count != _c.Arguments.Count)
                                    throw new Exception("The number of arguments in the binding does not match the callback definition.");
                                _bind_callbacks_o.AppendLine("#region _On" + _method.Name + "Callback");
                                _bind_callbacks_o.AppendLine();
                                _bind_callbacks_o.Append("private void _On" + _method.Name + "Callback(");
                                bool isfirst = true;
                                for (int _k = 0; _k < _method.Parameters.Count; _k++)
                                {
                                    if (isfirst)
                                        isfirst = false;
                                    else
                                        _bind_callbacks_o.Append(", ");
                                    _bind_callbacks_o.Append(_CompileInterfaceValueType(_method.Parameters[_k].Type) + " " + _c.Arguments[_k]);
                                }
                                _bind_callbacks_o.AppendLine(")");
                                _bind_callbacks_o.AppendLine("{");
                                _bind_callbacks_o.Indent(+1);
                                _bind_callbacks_o.AppendLine("lock (this)");
                                _bind_callbacks_o.AppendLine("{");
                                _bind_callbacks_o.Indent(+1);
                                _CompileAction(_b.Action, _bind_callbacks_o);
                                _bind_callbacks_o.AppendLine("if (uppercontrol != null)");
                                _bind_callbacks_o.Indent(+1);
                                _bind_callbacks_o.AppendLine("uppercontrol._UpdatedLower();");
                                _bind_callbacks_o.Indent(-1);
                                _bind_callbacks_o.Indent(-1);
                                _bind_callbacks_o.AppendLine("}");
                                _bind_callbacks_o.Indent(-1);
                                _bind_callbacks_o.AppendLine("}");
                                _bind_callbacks_o.AppendLine();
                                _bind_callbacks_o.AppendLine("#endregion");
                            }
                            break;

                        #endregion

                        #region compiling an initializer

                        case QS.Fx.Language.AST.TriggerType.Initialization:
                            {
                            }
                            break;

                        #endregion

                        #region compiling update trigger

                        case QS.Fx.Language.AST.TriggerType.Update:
                            {
                            }
                            break;

                        #endregion

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            #endregion

            #region _CompileAction

            private void _CompileAction(QS.Fx.Language.AST.Action _action, QS.Fx.Language.Output.Output _o)
            {
                switch (_action.ActionType)
                {
                    case QS.Fx.Language.AST.ActionType.Sequence:
                        {
                            QS.Fx.Language.AST.Sequence _s = (QS.Fx.Language.AST.Sequence) _action;
                            _o.AppendLine("{");
                            _o.Indent(+1);
                            foreach (QS.Fx.Language.AST.Action _a in _s.Actions)
                                _CompileAction(_a, _o);
                            _o.Indent(-1);
                            _o.AppendLine("}");
                        }
                        break;

                    case QS.Fx.Language.AST.ActionType.Update:
                        {
                            QS.Fx.Language.AST.Update _u = (QS.Fx.Language.AST.Update) _action;
                            // QS.Fx.Language.Structure.Property _p = _GetProperty(_u.Target);
                            if (_u.Type != QS.Fx.Language.AST.UpdateType.Assignment)
                                throw new NotImplementedException();                            
                            _o.Append(_u.Target + ".SetTo(");
                            _CompileExpression(_u.Value, _o);
                            _o.Append(");");
                        }
                        break;

                    case QS.Fx.Language.AST.ActionType.Choice:
                    case QS.Fx.Language.AST.ActionType.Loop:
                    case QS.Fx.Language.AST.ActionType.MethodCall:
                    default:
                        throw new NotImplementedException();
                }
            }

            #endregion

            #region _CompileExpression

            private void _CompileExpression(QS.Fx.Language.AST.Expression _expression, QS.Fx.Language.Output.Output _o)
            {
                switch (_expression.ExpressionType)
                {
                    case QS.Fx.Language.AST.ExpressionType.Number:
                        {
                            QS.Fx.Language.AST.Number _i = (QS.Fx.Language.AST.Number) _expression;
                            _o.Append("new UInt(" + _i.Value.ToString() + ")");
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.EmptySet:
                        {
                            _o.Append("UIntSet.EmptySet");
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.PropertyValue:
                        {
                            QS.Fx.Language.AST.PropertyValue _pv = (QS.Fx.Language.AST.PropertyValue) _expression;
                            _o.Append(_pv.Name);
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.VariableValue:
                        {
                            QS.Fx.Language.AST.VariableValue _vv = (QS.Fx.Language.AST.VariableValue)_expression;
                            _o.Append(_vv.Name);
                        }
                        break;

                    case QS.Fx.Language.AST.ExpressionType.BinaryOperation:
                    case QS.Fx.Language.AST.ExpressionType.ChildrenValue:
                    case QS.Fx.Language.AST.ExpressionType.GroupValue:
                    case QS.Fx.Language.AST.ExpressionType.FunctionCall:
                    case QS.Fx.Language.AST.ExpressionType.NewValue:
                    case QS.Fx.Language.AST.ExpressionType.OldValue:
                    case QS.Fx.Language.AST.ExpressionType.ParentValue:
                    case QS.Fx.Language.AST.ExpressionType.UnaryOperation:
                    case QS.Fx.Language.AST.ExpressionType.Boolean:
                    default:
                        throw new NotImplementedException();
                }
            }

            #endregion

            #region _GetProperty

            QS.Fx.Language.AST.Property _GetProperty(string _propertyname)
            {
                QS.Fx.Language.AST.Property _property = null;
                foreach (QS.Fx.Language.AST.Property _p in _protocol.Properties)
                {
                    if (_p.Name.Equals(_propertyname))
                    {
                        _property = _p;
                        break;
                    }
                }
                if (_property == null)
                    throw new Exception("Property " + _propertyname + " was not declared.");
                return _property;
            }

            #endregion

            #region _GetMethod

            QS.Fx.Language.AST.Method _GetMethod(string _methodname)
            {
                QS.Fx.Language.AST.Method _method = null;
                foreach (QS.Fx.Language.AST.Method _m in _protocol.Interface.Methods)
                {
                    if (_m.Name.Equals(_methodname))
                    {
                        _method = _m;
                        break;
                    }
                }
                if (_method == null)
                    throw new Exception("Method " + _methodname + " was not declared.");
                return _method;
            }

            #endregion

            #region _CompileFlows

            private void _CompileFlows()
            {
                foreach (QS.Fx.Language.AST.Flow _f in _protocol.Flows)
                {
                    switch (_f.FlowType)
                    {
                        #region compiling aggregations

                        case QS.Fx.Language.AST.FlowType.Aggregation:
                            {
                                QS.Fx.Language.AST.Aggregation _a = (QS.Fx.Language.AST.Aggregation)_f;
                                if (_a.TargetGroupProperty == null)
                                    throw new Exception("Aggregation does not have the target group property defined.");

                                string _aggregating = "_aggregating" + _a.TargetGroupProperty;
                                string _aggregating_isset = "_isset_aggregating" + _a.TargetGroupProperty;
                                string _aggregated = _a.TargetGroupProperty;
                                QS.Fx.Language.AST.Property _target_group_property = _GetProperty(_a.TargetGroupProperty);
                                string _target_group_property_value_type_name = _CompileValueType(_target_group_property.Type);

                                #region populating code for the aggregation token

                                _aggregationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _aggregationtoken_fields_o.AppendLine("public bool " + _aggregating_isset + ";");
                                _aggregationtoken_serializableinfo_o.AppendLine("QS.CMS.Base3.SerializationHelper.ExtendSerializableInfo_Bool(ref info);");
                                _aggregationtoken_serializeto_o.AppendLine(
                                    "QS.CMS.Base3.SerializationHelper.Serialize_Bool(ref header, ref data, " + _aggregating_isset + ");");
                                _aggregationtoken_deserializefrom_o.AppendLine(
                                    _aggregating_isset + " = QS.CMS.Base3.SerializationHelper.Deserialize_Bool(ref header, ref data);");

                                _aggregationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _aggregationtoken_fields_o.AppendLine(
                                    "public QS.Fx.Runtime.Versioned<" + _target_group_property_value_type_name + "> " + _aggregating + ";");
                                _aggregationtoken_serializableinfo_o.AppendLine("info.AddAnother(" + _aggregating + ".SerializableInfo);");
                                _aggregationtoken_serializeto_o.AppendLine(_aggregating + ".SerializeTo(ref header, ref data);");
                                _aggregationtoken_deserializefrom_o.AppendLine(_aggregating + ".DeserializeFrom(ref header, ref data);");

                                _aggregationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _aggregationtoken_fields_o.AppendLine(
                                    "public QS.Fx.Runtime.Versioned<" + _target_group_property_value_type_name + "> " + _aggregated + ";");
                                _aggregationtoken_serializableinfo_o.AppendLine("info.AddAnother(" + _aggregated + ".SerializableInfo);");
                                _aggregationtoken_serializeto_o.AppendLine(_aggregated + ".SerializeTo(ref header, ref data);");
                                _aggregationtoken_deserializefrom_o.AppendLine(_aggregated + ".DeserializeFrom(ref header, ref data);");

                                #endregion

                                #region populating code for the dissemination token

/-*
                            _disseminationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                            _disseminationtoken_fields_o.AppendLine(
                                "public Versioned<" + _target_group_property_value_type_name + ">" + _aggregating + ";");
                            _disseminationtoken_serializableinfo_o.AppendLine("info.AddAnother(" + _aggregating + ".SerializableInfo);");
                            _disseminationtoken_serializeto_o.AppendLine(_aggregating + ".SerializeTo(ref header, ref data);");
                            _disseminationtoken_deserializefrom_o.AppendLine(_aggregating + ".DeserializeFrom(ref header, ref data);");
*-/

                                _disseminationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _disseminationtoken_fields_o.AppendLine(
                                    "public QS.Fx.Runtime.Versioned<" + _target_group_property_value_type_name + "> " + _aggregated + ";");
                                _disseminationtoken_serializableinfo_o.AppendLine("info.AddAnother(" + _aggregated + ".SerializableInfo);");
                                _disseminationtoken_serializeto_o.AppendLine(_aggregated + ".SerializeTo(ref header, ref data);");
                                _disseminationtoken_deserializefrom_o.AppendLine(_aggregated + ".DeserializeFrom(ref header, ref data);");

                                #endregion

                                // .........................................................................................................................................................................
                            }
                            break;

                        #endregion

                        #region compiling disseminations

                        case QS.Fx.Language.AST.FlowType.Dissemination:
                            {
                                QS.Fx.Language.AST.Dissemination _d = (QS.Fx.Language.AST.Dissemination)_f;

                                string _disseminated = _d.TargetProperty;
                                QS.Fx.Language.AST.Property _target_property = _GetProperty(_d.TargetProperty);
                                string _target_property_value_type_name = _CompileValueType(_target_property.Type);

                                #region populating code for the aggregation token

                                _aggregationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _aggregationtoken_fields_o.AppendLine(
                                    "public QS.Fx.Runtime.Versioned<" + _target_property_value_type_name + "> " + _disseminated + ";");
                                _aggregationtoken_serializableinfo_o.AppendLine("info.AddAnother(" + _disseminated + ".SerializableInfo);");
                                _aggregationtoken_serializeto_o.AppendLine(_disseminated + ".SerializeTo(ref header, ref data);");
                                _aggregationtoken_deserializefrom_o.AppendLine(_disseminated + ".DeserializeFrom(ref header, ref data);");

                                #endregion

                                #region populating code for the dissemination token

                                _disseminationtoken_fields_o.AppendLine("[QS.Fx.Printing.Printable]");
                                _disseminationtoken_fields_o.AppendLine(
                                    "public QS.Fx.Runtime.Versioned<" + _target_property_value_type_name + "> " + _disseminated + ";");
                                _disseminationtoken_serializableinfo_o.AppendLine("info.AddAnother(" + _disseminated + ".SerializableInfo);");
                                _disseminationtoken_serializeto_o.AppendLine(_disseminated + ".SerializeTo(ref header, ref data);");
                                _disseminationtoken_deserializefrom_o.AppendLine(_disseminated + ".DeserializeFrom(ref header, ref data);");

                                #endregion

                                // .........................................................................................................................................................................
                            }
                            break;

                        #endregion

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            #endregion

            // .........................................................................................................................................................................
        }

        #endregion
*/
    }
}
