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

namespace QS._qss_x_.Language_.Structure_
{
    public static class Compiler
    {
        /*
                    #region _CompileExpression
                            #region TypeCast

                            case QS.Fx.Language.AST.ExpressionCategory.TypeCast:
                                {
                                    QS.Fx.Language.AST.TypeCast _tc = (QS.Fx.Language.AST.TypeCast) _x;
                                    return _CompileExpression(_tc.Value, _CompileValueType(_tc.ValueType), _context);
                                }

                            #endregion

                            #region ParentValue

                            case QS.Fx.Language.AST.ExpressionCategory.ParentValue:
                                {
                                    QS.Fx.Language.AST.ParentValue _parentvalue = (QS.Fx.Language.AST.ParentValue)_x;
                                    return _CompileExpression_Disseminate(_parentvalue.Value, _valuetype);                            
                                }

                            #endregion

                            #region ChildrenValue

                            case QS.Fx.Language.AST.ExpressionCategory.ChildrenValue:
                                {
                                    QS.Fx.Language.AST.ChildrenValue _childrenvalue = (QS.Fx.Language.AST.ChildrenValue) _x;
                                    return _CompileExpression_Aggregate(
                                        _childrenvalue.AggregatedValue, _childrenvalue.AggregationOperator, _valuetype, true);                            
                                }

                            #endregion

                            #region GroupValue

                            case QS.Fx.Language.AST.ExpressionCategory.GroupValue:
                                {
                                    QS.Fx.Language.AST.GroupValue _groupvalue = (QS.Fx.Language.AST.GroupValue)_x;
                                    return _CompileExpression_Aggregate(
                                        _groupvalue.AggregatedValue, _groupvalue.AggregationOperator, _valuetype, false);                            
                                }
                                break;

                            #endregion

                            #region FunctionCall

                            case QS.Fx.Language.AST.ExpressionCategory.FunctionCall:
                                {
                                    QS.Fx.Language.AST.FunctionCall _functioncall = (QS.Fx.Language.AST.FunctionCall)_x;
                                    Method _method;
                                    if (!protocol.Interface.TryGetMethod(_functioncall.Method, out _method))
                                        throw new Exception("Cannot find method \"" + _functioncall.Method + "\".");
                                    OutgoingCall _outgoingcall = _method as OutgoingCall;
                                    if (_outgoingcall == null)
                                        throw new Exception("Method \"" + _outgoingcall.Name + "\" is not an outgoing call.");
                                    if (_functioncall.Arguments.Length != _outgoingcall.ParameterTypes.Length)
                                        throw new Exception("Parameter mismatch.");
                                    Expression[] _arguments = new Expression[_functioncall.Arguments.Length];
                                    for (int _k = 0; _k < _arguments.Length; _k++)
                                        _arguments[_k] = _CompileExpression(_functioncall.Arguments[_k], _outgoingcall.ParameterTypes[_k], _context);
                                    return new FunctionCall(_outgoingcall, _arguments);
                                }

                            #endregion

                            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                            #region Set *****

                            case QS.Fx.Language.AST.ExpressionCategory.Set:
                                {
                                    throw new NotImplementedException();
                                }
                                break;

                            #endregion

                            default:
                                throw new NotImplementedException();
                        }
                    }

                    #endregion

         



              




                    #region _CompileBinding

                    private Binding _CompileBinding(QS.Fx.Language.AST.Binding _b)
                    {
                        switch (_b.Trigger.TriggerCategory)
                        {
                            #region Initialization

                            case QS.Fx.Language.AST.TriggerCategory.Initialization:
                                {
                                    _Context _context = new _Context();                            
                                    Action _action = _CompileAction(_b.Action, _context);
                                    OnInitialization _oninitialization = new OnInitialization(_action);
                                    protocol.AddInitializer(_oninitialization);
                                    return _oninitialization;
                                }

                            #endregion

                            #region Callback

                            case QS.Fx.Language.AST.TriggerCategory.Callback:
                                {
                                    QS.Fx.Language.AST.OnCallback _oncallback = (QS.Fx.Language.AST.OnCallback) _b.Trigger;
                            
                                    Method _method;
                                    if (!protocol.Interface.TryGetMethod(_oncallback.Callback, out _method))
                                        throw new Exception("Cannot find method \"" + _oncallback.Callback + "\" in protocol " + protocol.Name + ".");
                            
                                    if (_method.MethodCategory != MethodCategory.IncomingCall)
                                        throw new Exception("Method \"" + _oncallback.Callback + "\" is not a callback.");

                                    if (_oncallback.Arguments.Length != _method.ParameterTypes.Length)
                                        throw new Exception(
                                            "The number of formal parameters of the callback handler does not match that in the callback declaration.");

                                    Parameter[] _parameters = new Parameter[_oncallback.Arguments.Length];
                                    for (int _k = 0; _k < _parameters.Length; _k++)
                                        _parameters[_k] = new Parameter(_method.ParameterTypes[_k], _oncallback.Arguments[_k], false);

                                    _Context _context = new _Context();

                                    foreach (Parameter _parameter in _parameters)
                                        _context.parameters.Add(_parameter.Name, _parameter);

                                    Action _action = _CompileAction(_b.Action, _context);

                                    IncomingCall _incomingcall = (IncomingCall) _method;
                            
                                    OnIncomingCall _onincomingcall = new OnIncomingCall(_incomingcall, _parameters, _action);
                                    _incomingcall.AddHandler(_onincomingcall);

                                    return _onincomingcall;
                                }

                            #endregion

                            #region Update

                            case QS.Fx.Language.AST.TriggerCategory.Update:
                                {
                                    QS.Fx.Language.AST.OnUpdate _onupdate = (QS.Fx.Language.AST.OnUpdate) _b.Trigger;

                                    Property _property;
                                    if (!protocol.TryGetProperty(_onupdate.Property, out _property))
                                        throw new Exception("Cannot find property \"" + _onupdate.Property + "\" in protocol " + protocol.Name + ".");

                                    string _fqn = "_" + _property.Protocol.Name + "_" + _property.Name;
                                    Parameter _oldvalue = new Parameter(_property.ValueType, "_old" + _fqn, false);
                                    Parameter _newvalue = new Parameter(_property.ValueType, "_new" + _fqn, false);
                                    Dictionary<string, Variable> _d = new Dictionary<string, Variable>();
                                    _d.Add(_OLDVALUE, _oldvalue);
                                    _d.Add(_NEWVALUE, _newvalue);                            
                                    _Context _context = new _Context(_d);
                            
                                    Action _action = _CompileAction(_b.Action, _context);

                                    OnUpdate __onupdate = new OnUpdate(_property, _action, _oldvalue, _newvalue);
                                    _property.AddHandler(__onupdate);

                                    return __onupdate;
                                }

                            #endregion

                            default:
                                throw new NotImplementedException();
                        }
                    }

                    #endregion

                    #region _CompileFlow

                    private Flow _CompileFlow(QS.Fx.Language.AST.Flow _f)
                    {
                        throw new NotImplementedException();
                    }

                    #endregion

                    #region _CompileAction

                    private Action _CompileAction(QS.Fx.Language.AST.Action _a, _Context _context)
                    {
                        switch (_a.ActionCategory)
                        {
                            #region Update

                            case QS.Fx.Language.AST.ActionCategory.Update:
                                return _CompileUpdate((QS.Fx.Language.AST.Update) _a, _context);

                            #endregion

                            #region Sequence

                            case QS.Fx.Language.AST.ActionCategory.Sequence:
                                {
                                    QS.Fx.Language.AST.Sequence _sequence = (QS.Fx.Language.AST.Sequence) _a;
                                    List<Action> _actions = new List<Action>();
                                    foreach (QS.Fx.Language.AST.Action _aa in _sequence.Actions)
                                        _actions.Add(_CompileAction(_aa, _context));
                                    return new Sequence(_actions);
                                }

                            #endregion

                            #region MethodCall

                            case QS.Fx.Language.AST.ActionCategory.MethodCall:
                                {
                                    QS.Fx.Language.AST.MethodCall _methodcall = (QS.Fx.Language.AST.MethodCall) _a;
                                    Method _method;
                                    if (!protocol.Interface.TryGetMethod(_methodcall.Method, out _method))
                                        throw new Exception("Cannot find method \"" + _methodcall.Method + "\" in this protocol.");
                                    OutgoingCall _outgoingcall = _method as OutgoingCall;
                                    if (_outgoingcall == null)
                                        throw new Exception("Method \"" + _method.Name + "\" is not an outgoing call.");
                                    if (_outgoingcall.ParameterTypes.Length != _methodcall.Arguments.Length)
                                        throw new Exception("The numbers of arguments to the method call and parameters in the method declaration mismatch.");
                                    Expression[] _expressions = new Expression[_outgoingcall.ParameterTypes.Length];
                                    for (int _k = 0; _k < _outgoingcall.ParameterTypes.Length; _k++)
                                        _expressions[_k] = _CompileExpression(_methodcall.Arguments[_k], _outgoingcall.ParameterTypes[_k], _context);
                                    return new MethodCall(_outgoingcall, _expressions);
                                }

                            #endregion

                            #region Choice

                            case QS.Fx.Language.AST.ActionCategory.Choice:
                                {
                                    QS.Fx.Language.AST.Choice _choice = (QS.Fx.Language.AST.Choice)_a;
                                    // ......................................................................................................................................................
                                    throw new NotImplementedException();
                                }

                            #endregion

                            #region Loop

                            case QS.Fx.Language.AST.ActionCategory.Loop:
                                {
                                    QS.Fx.Language.AST.Loop _loop = (QS.Fx.Language.AST.Loop) _a;
                                    // ......................................................................................................................................................                            
                                    throw new NotImplementedException();
                                }

                            #endregion

                            default:
                                throw new NotImplementedException();
                        }
                    }

                    #endregion

        */
    }
}
