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

// #define OPTION_RequireUniqueOperatorMatching

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace QS._qss_x_.Language_.Structure_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Library
    {
        #region Constructors

        public Library()
        {
            _RegisterPrimitiveTypes();

            foreach (System.Type _underlyingtype in typeof(QS._qss_x_.Language_.Structure_.Library).Assembly.GetTypes())
            {
                if (_underlyingtype.GetCustomAttributes(typeof(ValueTypeAttribute), false).Length > 0)
                    _RegisterType_1(_underlyingtype);

                if (_underlyingtype.GetCustomAttributes(typeof(ValueTypeTemplateAttribute), false).Length > 0)
                    _RegisterTypeTemplate_1(_underlyingtype);
            }

            _VOID = _types3[PredefinedType.VOID];
            _BOOL = _types3[PredefinedType.BOOL];
            _UINT = _types3[PredefinedType.UINT];
            _USET = _types3[PredefinedType.USET];

            _VERSIONED = _templates_3[PredefinedTypeTemplate.VERSIONED];

            foreach (ValueType _valuetype in _types1.Values)
                _RegisterType_2(_valuetype);

            foreach (ValueTypeTemplate _valuetypetemplate in _templates_1.Values)
                _RegisterTypeTemplate_2(_valuetypetemplate);
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded)]
        private IDictionary<string, ValueTypeTemplate> _templates_1 = new Dictionary<string, ValueTypeTemplate>();
        private IDictionary<System.Type, ValueTypeTemplate> _templates_2 = new Dictionary<System.Type, ValueTypeTemplate>();
        private IDictionary<PredefinedTypeTemplate, ValueTypeTemplate> _templates_3 = new Dictionary<PredefinedTypeTemplate, ValueTypeTemplate>();

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded)]
        private IDictionary<string, ValueType> _types1 = new Dictionary<string, ValueType>();
        private IDictionary<System.Type, ValueType> _types2 = new Dictionary<System.Type, ValueType>();
        private IDictionary<PredefinedType, ValueType> _types3 = new Dictionary<PredefinedType, ValueType>();

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded)]
        private IDictionary<string, IList<Operator>> _operators1 = new Dictionary<string, IList<Operator>>();
        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded)]
        private IDictionary<PredefinedOperator, IList<Operator>> _operators2 = new Dictionary<PredefinedOperator, IList<Operator>>();

        private ValueType _system_Boolean, _system_Int32, _system_UInt32, _system_String, _VOID, _BOOL, _UINT, _USET;
        private ValueTypeTemplate _VERSIONED;

        private IDictionary<string, Protocol> _protocols = new Dictionary<string, Protocol>();

        #endregion

        #region _RegisterPrimitiveTypes

        private void _RegisterPrimitiveTypes()
        {
            _system_Boolean = new ValueType(typeof(System.Boolean), "bool", PredefinedType._system_Boolean);
            _system_Int32 = new ValueType(typeof(System.Int32), "int", PredefinedType._system_Int32);
            _system_UInt32 = new ValueType(typeof(System.UInt32), "uint", PredefinedType._system_UInt32);
            _system_String = new ValueType(typeof(System.String), "string", PredefinedType._system_String);
            
            ValueType[] _system_types = new ValueType[] { _system_Boolean, _system_Int32, _system_UInt32 };

            foreach (ValueType _valuetype in _system_types)
                _AddValueType(_valuetype);

            _AddOperator(new OperatorFromPrefixOperator("Minus", PredefinedOperator.Minus, _system_Int32, "-", true));
            _AddOperator(new OperatorFromInfixOperator("Add", PredefinedOperator.Add, _system_Int32, "+", true));
            _AddOperator(new OperatorFromInfixOperator("Substract", PredefinedOperator.Substract, _system_Int32, "-", true));
            _AddOperator(new OperatorFromInfixOperator("Multiply", PredefinedOperator.Multiply, _system_Int32, "*", true));
            _AddOperator(new OperatorFromInfixOperator("Divide", PredefinedOperator.Divide, _system_Int32, "/", true));
            _AddOperator(new OperatorFromInfixOperator("EQ", PredefinedOperator.EQ, _system_Boolean, _system_Int32, "==", true));
            _AddOperator(new OperatorFromInfixOperator("NEQ", PredefinedOperator.NEQ, _system_Boolean, _system_Int32, "!=", true));
            _AddOperator(new OperatorFromInfixOperator("LT", PredefinedOperator.LT, _system_Boolean, _system_Int32, "<", true));
            _AddOperator(new OperatorFromInfixOperator("LTE", PredefinedOperator.LTE, _system_Boolean, _system_Int32, "<=", true));
            _AddOperator(new OperatorFromInfixOperator("GT", PredefinedOperator.GT, _system_Boolean, _system_Int32, ">", true));
            _AddOperator(new OperatorFromInfixOperator("GTE", PredefinedOperator.GTE, _system_Boolean, _system_Int32, ">=", true));

            _AddOperator(new OperatorFromInfixOperator("Add", PredefinedOperator.Add, _system_UInt32, "+", true));
            _AddOperator(new OperatorFromInfixOperator("Substract", PredefinedOperator.Substract, _system_UInt32, "-", true));
            _AddOperator(new OperatorFromInfixOperator("Multiply", PredefinedOperator.Multiply, _system_UInt32, "*", true));
            _AddOperator(new OperatorFromInfixOperator("Divide", PredefinedOperator.Divide, _system_UInt32, "/", true));
            _AddOperator(new OperatorFromInfixOperator("EQ", PredefinedOperator.EQ, _system_Boolean, _system_UInt32, "==", true));
            _AddOperator(new OperatorFromInfixOperator("NEQ", PredefinedOperator.NEQ, _system_Boolean, _system_UInt32, "!=", true));
            _AddOperator(new OperatorFromInfixOperator("LT", PredefinedOperator.LT, _system_Boolean, _system_UInt32, "<", true));
            _AddOperator(new OperatorFromInfixOperator("LTE", PredefinedOperator.LTE, _system_Boolean, _system_UInt32, "<=", true));
            _AddOperator(new OperatorFromInfixOperator("GT", PredefinedOperator.GT, _system_Boolean, _system_UInt32, ">", true));
            _AddOperator(new OperatorFromInfixOperator("GTE", PredefinedOperator.GTE, _system_Boolean, _system_UInt32, ">=", true));

            _AddOperator(new OperatorFromCastingOperator(_system_Int32, _system_UInt32, true));
            _AddOperator(new OperatorFromCastingOperator(_system_UInt32, _system_Int32, true));

            _AddOperator(new OperatorFromPrefixOperator("Not", PredefinedOperator.Not, _system_Boolean, "!", true));
            _AddOperator(new OperatorFromInfixOperator("Or", PredefinedOperator.Or, _system_Boolean, "||", true));
            _AddOperator(new OperatorFromInfixOperator("And", PredefinedOperator.And, _system_Boolean, "&&", true));
        }

        #endregion

        #region _RegisterType_1

        private static readonly Type _genericvaluetype = typeof(QS._qss_x_.Runtime_1_.IValue<QS._qss_x_.Runtime_1_.UINT>).GetGenericTypeDefinition();
        private void _RegisterType_1(System.Type _underlyingtype)
        {
            if (!typeof(QS.Fx.Serialization.ISerializable).IsAssignableFrom(_underlyingtype))
                throw new Exception("The type " + _underlyingtype.ToString() + " is not serializable.");

//            if (_underlyingtype.GetConstructor(Type.EmptyTypes) == null)
//                throw new Exception("The type " + _underlyingtype.ToString() + " does not have a default no-argument constructor.");

            System.Type _valueofitself;
            try
            {
                _valueofitself = _genericvaluetype.MakeGenericType(_underlyingtype);
            }
            catch (Exception exc)
            {
                throw new Exception("The type " + _underlyingtype.ToString() + 
                    " is not compatible with the generic QS.Fx.Runtime.IValue interface.", exc);
            }

            if (!_valueofitself.IsAssignableFrom(_underlyingtype))
                throw new Exception("The type " + _underlyingtype.ToString() +
                    " does not implement the generic QS.Fx.Runtime.IValue interface.");

            object[] _attributes = _underlyingtype.GetCustomAttributes(typeof(ValueTypeAttribute), false);
            if (_attributes == null || _attributes.Length != 1)
                throw new Exception("The type " + _underlyingtype.ToString() + 
                    " has not been decorated with QS.Fx.Language.Structure.ValueTypeAttribute.");
            ValueTypeAttribute _attribute = (ValueTypeAttribute) _attributes[0];

            _AddValueType(new ValueType(_underlyingtype, _attribute.Name, _attribute.PredefinedType));
        }

        #endregion

        #region _RegisterType_2

        private void _RegisterType_2(ValueType _valuetype)
        {
            System.Type _underlyingtype = _valuetype.UnderlyingType;

            foreach (ConstructorInfo _info in _underlyingtype.GetConstructors())
            {
//                if (_info.GetCustomAttributes(typeof(ConstructorAttribute), false).Length > 0)
//                {
//                    System.Type[] _parametertypes;
//                    if (!_AreLegalConstructorParameters(_info.GetParameters(), out _parametertypes))
//                        throw new Exception("The parameters of a constructor " + _info.ToString() + " in type \"" + _valuetype.Name +
//                            "\" are not legal parameters of a constructor.");
//                    _valuetype.AddConstructor(new ConstructorFromConstructor(_valuetype, _parametertypes, _info));
//                }

                object[] _operatorattribs = _info.GetCustomAttributes(typeof(OperatorAttribute), false);
                if (_operatorattribs.Length > 0)
                {
                    ValueType[] _parametertypes;
                    if (!_AreLegalOperatorParameters(_info.GetParameters(), out _parametertypes))
                        throw new Exception("The parameters of a constructor " + _info.ToString() + " in type \"" + _valuetype.Name +
                            "\" are not legal parameters of an operator.");
                    foreach (OperatorAttribute _operatorattrib in _operatorattribs)
                    {
                        Operator _operator = new OperatorFromConstructor(_operatorattrib.Names, _operatorattrib.PredefinedOperators,
                            _valuetype, _parametertypes, _info, _operatorattrib.IsAtom);
                        _AddOperator(_operator);
                    }
                }
            }

            foreach (MethodInfo _info in _underlyingtype.GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
            {
                object[] _operatorattribs = _info.GetCustomAttributes(typeof(OperatorAttribute), false);
                if (_operatorattribs.Length > 0)
                {
                    ValueType _resulttype;
                    if (_info.ReturnType.Equals(typeof(void)))
                        _resulttype = _VOID;
                    else
                        if (!_IsLegalOperatorType(_info.ReturnType, out _resulttype))
                            throw new Exception("The result type of an instance method " + _info.ToString() + " in type \"" + _valuetype.Name +
                                "\" is not a legal result type of an operator.");
                    ValueType[] _parametertypes;
                    if (!_AreLegalOperatorParameters(_info.GetParameters(), out _parametertypes))
                        throw new Exception("The parameters of an instance method " + _info.ToString() + " in type \"" + _valuetype.Name +
                            "\" are not legal parameters of an operator.");
                    ValueType[] _parametertypes_2 = new ValueType[_parametertypes.Length + 1];
                    _parametertypes_2[0] = _valuetype;
                    Array.Copy(_parametertypes, 0, _parametertypes_2, 1, _parametertypes.Length);
                    foreach (OperatorAttribute _operatorattrib in _operatorattribs)
                    {
                        Operator _operator = new OperatorFromInstanceMethod(_operatorattrib.Names, _operatorattrib.PredefinedOperators,
                            _resulttype, _parametertypes_2, _info, _operatorattrib.IsAtom);
                        _AddOperator(_operator);
                    }
                }
            }

            foreach (MethodInfo _info in _underlyingtype.GetMethods(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod))
            {
//                if (_info.GetCustomAttributes(typeof(ConstructorAttribute), false).Length > 0)
//                {
//                    if (!_info.ReturnType.Equals(_underlyingtype))
//                        throw new Exception("The method \"" + _info.Name +
//                            "\" cannot be used as a constructor because it does not return values of the constructed type.");
//                    System.Type[] _parametertypes;
//                    if (!_AreLegalConstructorParameters(_info.GetParameters(), out _parametertypes))
//                        throw new Exception("The parameters of a static method " + _info.ToString() + " in type \"" + _valuetype.Name +
//                            "\" are not legal parameters of a constructor.");
//                    _valuetype.AddConstructor(new ConstructorFromStaticMethod(_valuetype, _parametertypes, _info));
//                }

                object[] _operatorattribs = _info.GetCustomAttributes(typeof(OperatorAttribute), false);
                if (_operatorattribs.Length > 0)
                {                    
                    ValueType _resulttype;
                    if (_info.ReturnType.Equals(typeof(void)))
                        _resulttype = _VOID;
                    else
                        if (!_IsLegalOperatorType(_info.ReturnType, out _resulttype))
                            throw new Exception("The result type of a static method " + _info.ToString() + " in type \"" + _valuetype.Name +
                                "\" is not a legal result type of an operator.");
                    ValueType[] _parametertypes;
                    if (!_AreLegalOperatorParameters(_info.GetParameters(), out _parametertypes))
                        throw new Exception("The parameters of a static method " + _info.ToString() + " in type \"" + _valuetype.Name +
                            "\" are not legal parameters of an operator.");
                    foreach (OperatorAttribute _operatorattrib in _operatorattribs)
                    {
                        Operator _operator = new OperatorFromStaticMethod(_operatorattrib.Names, _operatorattrib.PredefinedOperators,
                            _resulttype, _parametertypes, _info, _operatorattrib.IsAtom);
                        _AddOperator(_operator);
                    }
                }
            }

            foreach (PropertyInfo _info in _underlyingtype.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object[] _operatorattribs = _info.GetCustomAttributes(typeof(OperatorAttribute), false);
                if (_operatorattribs.Length > 0)
                {
                    ValueType _resulttype;
                    if (!_IsLegalOperatorType(_info.PropertyType, out _resulttype))
                        throw new Exception("The type of an instance property " + _info.ToString() + " in type \"" + _valuetype.Name +
                            "\" is not a legal result type of an operator.");
                    foreach (OperatorAttribute _operatorattrib in _operatorattribs)
                    {
                        Operator _operator = new OperatorFromInstanceProperty(_operatorattrib.Names, _operatorattrib.PredefinedOperators,
                            _resulttype, _valuetype, _info, _operatorattrib.IsAtom);
                        _AddOperator(_operator);
                    }
                }
            }

            foreach (PropertyInfo _info in _underlyingtype.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
//                if (_info.GetCustomAttributes(typeof(ConstructorAttribute), false).Length > 0)
//                {
//                    if (!_info.PropertyType.Equals(_underlyingtype))
//                        throw new Exception("The property \"" + _info.Name +
//                            "\" cannot be used as a constructor because its type is different from the constructed type.");
//                    if (!_info.CanRead)
//                        throw new Exception("The property \"" + _info.Name + "\" cannot be used as a constructor because it is not readable.");
//                    _valuetype.AddConstructor(new ConstructorFromStaticProperty(_valuetype, _info));
//                }

                object[] _operatorattribs = _info.GetCustomAttributes(typeof(OperatorAttribute), false);
                if (_operatorattribs.Length > 0)
                {
                    ValueType _resulttype;
                    if (!_IsLegalOperatorType(_info.PropertyType, out _resulttype))
                        throw new Exception("The type of a static property " + _info.ToString() + " in type \"" + _valuetype.Name +
                            "\" is not a legal result type of an operator.");
                    foreach (OperatorAttribute _operatorattrib in _operatorattribs)
                    {
                        Operator _operator = new OperatorFromStaticProperty(
                            _operatorattrib.Names, _operatorattrib.PredefinedOperators, _resulttype, _info, _operatorattrib.IsAtom);
                        _AddOperator(_operator);
                    }
                }
            }
        }

        #endregion

        #region _RegisterTypeTemplate_1

        private void _RegisterTypeTemplate_1(System.Type _underlyingtype)
        {
            if (!_underlyingtype.IsGenericTypeDefinition || !_underlyingtype.ContainsGenericParameters)
                throw new Exception("The type template " + _underlyingtype.ToString() + 
                    " is not a generic type definition or does not contain generic type parameters.");

            Type[] _genericparameters = _underlyingtype.GetGenericArguments();
            List<ValueTypeTemplateParameter> _parameters = new List<ValueTypeTemplateParameter>();

            foreach (Type _genericparameter in _genericparameters)
            {
                if (_genericparameter.IsGenericParameter)
                    _parameters.Add(new ValueTypeTemplateParameter(_genericparameter.Name));
            }

            object[] _attributes = _underlyingtype.GetCustomAttributes(typeof(ValueTypeTemplateAttribute), false);
            if (_attributes == null || _attributes.Length != 1)
                throw new Exception("The type template " + _underlyingtype.ToString() +
                    " has not been decorated with QS.Fx.Language.Structure.ValueTypeTemplateAttribute.");
            ValueTypeTemplateAttribute _attribute = (ValueTypeTemplateAttribute) _attributes[0];

            _AddValueTypeTemplate(
                new ValueTypeTemplate(_underlyingtype, _attribute.Name, _attribute.PredefinedTypeTemplate, _parameters.ToArray()));
        }

        #endregion

        #region _RegisterTypeTemplate_2

        private void _RegisterTypeTemplate_2(ValueTypeTemplate _valuetypetemplate)
        {
/*
            System.Type _underlyingtype = _valuetype.UnderlyingType;
*/
        }

        #endregion

        #region _InstantiateTypeTemplate_1

        private bool _InstantiateTypeTemplate_1(ValueTypeTemplate _template, ValueType[] _arguments, out ValueType _valuetype)
        {
            if (_arguments.Length != _template.Parameters.Length)
                throw new Exception("The number of template type arguments does not match the template type definition.");

            System.Type[] _tt = new System.Type[_arguments.Length];
            StringBuilder _namesb = new StringBuilder();
            _namesb.Append(_template.Name + "<");
            for (int _k = 0; _k < _arguments.Length; _k++)
            {
                _tt[_k] = _arguments[_k].UnderlyingType;
                if (_k > 0)
                    _namesb.Append(", ");
                _namesb.Append(_arguments[_k].Name);
            }
            _namesb.Append(">");
            string _name = _namesb.ToString();
            System.Type _underlyingtype = _template.UnderlyingType.MakeGenericType(_tt);

            if (!_types2.TryGetValue(_underlyingtype, out _valuetype))
            {
                _valuetype = new ValueType(_underlyingtype, _name, PredefinedType.None, _template, _arguments);
                _AddValueType(_valuetype);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region _InstantiateTypeTemplate_2

        private void _InstantiateTypeTemplate_2(ValueType _valuetype)
        {
            _RegisterType_2(_valuetype);
        }

        #endregion

        #region _AddValueType

        private void _AddValueType(ValueType _valuetype)
        {
            _types1.Add(_valuetype.Name, _valuetype);
            _types2.Add(_valuetype.UnderlyingType, _valuetype);
            if (_valuetype.PredefinedType != PredefinedType.None)
                _types3.Add(_valuetype.PredefinedType, _valuetype);
        }

        #endregion

        #region _AddValueTypeTemplate

        private void _AddValueTypeTemplate(ValueTypeTemplate _valuetypetemplate)
        {
            _templates_1.Add(_valuetypetemplate.Name, _valuetypetemplate);
            _templates_2.Add(_valuetypetemplate.UnderlyingType, _valuetypetemplate);
            _templates_3.Add(_valuetypetemplate.PredefinedTypeTemplate, _valuetypetemplate);
        }

        #endregion

        #region _AddOperator

        private void _AddOperator(Operator _operator)
        {
            if (_operator.Names != null)
            {
                foreach (string _name in _operator.Names)
                {
                    if (_name != null)
                    {
                        IList<Operator> _list1;
                        if (!_operators1.TryGetValue(_name, out _list1))
                            _operators1.Add(_name, (_list1 = new List<Operator>()));
                        _list1.Add(_operator);
                    }
                }
            }

            if (_operator.PredefinedOperators != null)
            {
                foreach (PredefinedOperator _predefinedoperator in _operator.PredefinedOperators)
                {
                    if (_predefinedoperator != PredefinedOperator.None)
                    {
                        IList<Operator> _list2;
                        if (!_operators2.TryGetValue(_predefinedoperator, out _list2))
                            _operators2.Add(_predefinedoperator, (_list2 = new List<Operator>()));
                        _list2.Add(_operator);
                    }
                }
            }

            if (_operator.ResultType != null)
                _operator.ResultType.RegisterOperator(_operator);
        }

        #endregion

        #region _AreLegalConstructorParameters

        //        private static readonly ICollection<System.Type> _legalConstructorParameterTypes =
//            new System.Collections.ObjectModel.Collection<System.Type>(new List<System.Type>(new System.Type[] 
//            { 
//                typeof(bool), typeof(int), typeof(uint), typeof(string)
//            }));
//
//        private bool _AreLegalConstructorParameters(ParameterInfo[] _parameters, out System.Type[] _types)
//        {
//            _types = new System.Type[_parameters.Length];
//            for (int _k = 0; _k < _parameters.Length; _k++)
//            {
//                if (!_IsLegalConstructorParameter(_parameters[_k], out _types[_k]))
//                {
//                    _types = null;
//                    return false;
//                }
//            }
//            return true;
//        }
//
//        private bool _IsLegalConstructorParameter(ParameterInfo _parameter, out System.Type _type)
//        {
//            _type = _parameter.ParameterType;
//            if (_parameter.IsOut || _parameter.IsRetval || _parameter.IsOptional || !_legalConstructorParameterTypes.Contains(_type))
//            {
//                _type = null;
//                return false;
//            }
//            return true;
//        }

        #endregion

        #region _AreLegalOperatorParameters

        private bool _AreLegalOperatorParameters(ParameterInfo[] _parameters, out ValueType[] _valuetypes)
        {
            _valuetypes = new ValueType[_parameters.Length];
            for (int _k = 0; _k < _parameters.Length; _k++)
            {
                if (!_IsLegalOperatorParameter(_parameters[_k], out _valuetypes[_k]))
                {
                    _valuetypes = null;
                    return false;
                }
            }
            return true;
        }

        private bool _IsLegalOperatorParameter(ParameterInfo _parameter, out ValueType _valuetype)
        {
            if (_parameter.IsOut || _parameter.IsRetval || _parameter.IsOptional ||
                !_IsLegalOperatorType(_parameter.ParameterType, out _valuetype))
            {
                _valuetype = null;
                return false;
            }
            return true;
        }

        private bool _IsLegalOperatorType(System.Type _type, out ValueType _valuetype)
        {
            return _types2.TryGetValue(_type, out _valuetype);
        }

        #endregion

        #region _MakeOperation

        private Expression _MakeOperation(IEnumerable<Operator> _operators, Expression[] _parameters)
        {
            IList<Operator> _oo = new List<Operator>();
            
            foreach (Operator _o in _operators)
            {
                if (_o.ParameterTypes.Length == _parameters.Length)
                {
                    bool _identical = true;
                    for (int _k = 0; _identical && (_k < _parameters.Length); _k++)
                    {
                        if (!ValueType.Equal(_parameters[_k].ValueType, _o.ParameterTypes[_k]))
                            _identical = false;
                    }
                    if (_identical)
                        return new Operation(_o, _parameters);
                    else
                        _oo.Add(_o);
                }
            }

            int _nc = int.MaxValue;
            IList<Expression> _ee = new List<Expression>();

            foreach (Operator _o in _oo)
            {
                Operator[] _castingoperators = new Operator[_parameters.Length];
                bool _cancast = true;
                for (int _k = 0; _cancast && (_k < _parameters.Length); _k++)
                {
                    if (ValueType.Equal(_parameters[_k].ValueType, _o.ParameterTypes[_k]))
                        _castingoperators[_k] = null;
                    else if (!TryCast(_o.ParameterTypes[_k], _parameters[_k].ValueType, out  _castingoperators[_k]))
                        _cancast = false;
                }
                if (_cancast)
                {
                    Expression[] _ev = new Expression[_parameters.Length];
                    int _my_nc = 0;
                    for (int _k = 0; _k < _parameters.Length; _k++)
                    {
                        if (_castingoperators[_k] == null)
                            _ev[_k] = _parameters[_k];
                        else
                        {
                            _my_nc++;
                            _ev[_k] = new Operation(_castingoperators[_k], new Expression[] { _parameters[_k] });
                        }
                    }

                    if (_my_nc <= _nc)
                    {
                        if (_my_nc < _nc)
                        {
                            _nc = _my_nc;
                            _ee.Clear();
                        }
                        _ee.Add(new Operation(_o, _ev));
                    }
                }
            }

            if (_ee.Count > 0)
            {
#if OPTION_RequireUniqueOperatorMatching
                if (_ee.Count > 1)
                    throw new Exception("More than one operator found that can be applied with implicit casting of arguments.");
#endif

                return _ee[0];
            }
            else
                throw new Exception("Could not match any operators even with implicit casting.");
        }

        #endregion

        #region _FindOperators

        public IEnumerable<Operator> _FindOperators(PredefinedOperator _predefinedoperator, ValueType _resulttype)
        {
            if (_resulttype != null)
                return _resulttype.GetOperators(_predefinedoperator);
            else
            {
                IList<Operator> _oo;
                if (!_operators2.TryGetValue(_predefinedoperator, out _oo))
                    throw new Exception("Predefined operator \"" + _predefinedoperator.ToString() + "\" does not exist in the library.");
                else
                {
                    List<Operator> _ooo = new List<Operator>();
                    foreach (Operator _o in _oo)
                        if ((_resulttype == null) || ValueType.Equal(_resulttype, _o.ResultType))
                            _ooo.Add(_o);
                    return _ooo;
                }
            }
        }

        public IEnumerable<Operator> _FindOperators(string _operatorname, ValueType _resulttype)
        {
            if (_resulttype != null)
                return _resulttype.GetOperators(_operatorname);
            else
            {
                IList<Operator> _oo;
                if (!_operators1.TryGetValue(_operatorname, out _oo))
                    throw new Exception("Operator named \"" + _operatorname + "\" does not exist in the library.");
                else
                {
                    List<Operator> _ooo = new List<Operator>();
                    foreach (Operator _o in _oo)
                        if ((_resulttype == null) || ValueType.Equal(_resulttype, _o.ResultType))
                            _ooo.Add(_o);
                    return _ooo;
                }
            }
        }

        #endregion

        #region _ParameterTypesOk

        private bool _ParameterTypesOk(ValueType[] _parametertypes, ValueType[] _parametertypestemplate)
        {
            if (_parametertypes.Length == _parametertypestemplate.Length)
            {
                for (int _k = 0; _k < _parametertypes.Length; _k++)
                {
                    if ((_parametertypestemplate[_k] != null) &&
                        ((_parametertypes[_k] == null) || (!_parametertypes[_k].Equals(_parametertypestemplate[_k]))))
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Interface

        public IEnumerable<ValueType> ValueTypes
        {
            get { return _types1.Values; }
        }

        public bool TryGetValueType(string _name, out ValueType _valuetype)
        {
            return _types1.TryGetValue(_name, out _valuetype);
        }

        public ValueType GetValueType(string _name)
        {
            ValueType _valuetype;
            if (_types1.TryGetValue(_name, out _valuetype))
                return _valuetype;
            else
                throw new Exception("Cannot find value type named \"" + _name + "\" in the library.");
        }

        public bool TryGetValueType(System.Type _type, out ValueType _valuetype)
        {
            return _types2.TryGetValue(_type, out _valuetype);
        }

        public ValueType GetValueType(System.Type _type)
        {
            ValueType _valuetype;
            if (_types2.TryGetValue(_type, out _valuetype))
                return _valuetype;
            else
                throw new Exception("Cannot find value type corresponding to \"" + _type.ToString() + "\" in the library.");
        }

        public bool TryGetValueType(PredefinedType _type, out ValueType _valuetype)
        {
            return _types3.TryGetValue(_type, out _valuetype);
        }

        public ValueType GetValueType(PredefinedType _type)
        {
            ValueType _valuetype;
            if (_types3.TryGetValue(_type, out _valuetype))
                return _valuetype;
            else
                throw new Exception("Cannot find predefined type \"" + _type.ToString() + "\" in the library.");
        }

        public bool TryGetValueTypeTemplate(string _name, out ValueTypeTemplate _valuetypetemplate)
        {
            return _templates_1.TryGetValue(_name, out _valuetypetemplate);
        }

        public ValueTypeTemplate GetValueTypeTemplate(string _name)
        {
            ValueTypeTemplate _valuetypetemplate;
            if (_templates_1.TryGetValue(_name, out _valuetypetemplate))
                return _valuetypetemplate;
            else
                throw new Exception("Cannot find value type template named \"" + _name + "\" in the library.");
        }

        public bool TryGetValueTypeTemplate(System.Type _type, out ValueTypeTemplate _valuetypetemplate)
        {
            return _templates_2.TryGetValue(_type, out _valuetypetemplate);
        }

        public ValueTypeTemplate GetValueTypeTemplate(System.Type _type)
        {
            ValueTypeTemplate _valuetypetemplate;
            if (_templates_2.TryGetValue(_type, out _valuetypetemplate))
                return _valuetypetemplate;
            else
                throw new Exception("Cannot find value type template corresponding to type \"" + _type.ToString() + "\" in the library.");
        }

        public bool TryGetValueTypeTemplate(PredefinedTypeTemplate _typetemplate, out ValueTypeTemplate _valuetypetemplate)
        {
            return _templates_3.TryGetValue(_typetemplate, out _valuetypetemplate);
        }

        public ValueTypeTemplate GetValueTypeTemplate(PredefinedTypeTemplate _typetemplate)
        {
            ValueTypeTemplate _valuetypetemplate;
            if (_templates_3.TryGetValue(_typetemplate, out _valuetypetemplate))
                return _valuetypetemplate;
            else
                throw new Exception("Cannot find predefined value type template \"" + _typetemplate.ToString() + "\" in the library.");
        }

        public Operator GetOperator(string _operatorname, ValueType _resulttype, ValueType[] _parametertypes)
        {
            Operator _operator;
            if (!TryGetOperator(_operatorname, _resulttype, _parametertypes, out _operator))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Cannot find operator with name \"" + _operatorname + "\", result type " + _resulttype.Name + ", and parameter types ( ");
                for (int _k = 0; _k < _parametertypes.Length; _k++)
                {
                    if (_k > 0)
                        sb.Append(", ");
                    sb.Append(_parametertypes[_k].Name);
                }
                sb.Append(" ).");
                throw new Exception(sb.ToString());
            }
            return _operator;
        }

        public bool TryGetOperator(
            string _operatorname, ValueType _resulttype, ValueType[] _parametertypes, out Operator _operator)
        {
            IList<Operator> _list;
            if (!_operators1.TryGetValue(_operatorname, out _list))
            {
                _operator = null;
                return false;
            }

            foreach (Operator _o in _list)
            {
                if (((_resulttype == null) || ValueType.Equal(_resulttype, _o.ResultType))
                    && ValueType.Equal(_parametertypes, _o.ParameterTypes))
                {
                    _operator = _o;
                    return true;
                }
            }

            _operator = null;
            return false;
        }

        public Operator GetOperator(PredefinedOperator _predefinedoperator, ValueType _resulttype, ValueType[] _parametertypes)
        {
            Operator _operator;
            if (!TryGetOperator(_predefinedoperator, _resulttype, _parametertypes, out _operator))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Cannot find predefined operator \"" + _predefinedoperator + "\" with result type " + 
                    _resulttype.Name + ", and parameter types ( ");
                for (int _k = 0; _k < _parametertypes.Length; _k++)
                {
                    if (_k > 0)
                        sb.Append(", ");
                    sb.Append(_parametertypes[_k].Name);
                }
                sb.Append(" ).");
                throw new Exception(sb.ToString());
            }
            return _operator;
        }

        public bool TryGetOperator(
            PredefinedOperator _predefinedoperator, ValueType _resulttype, ValueType[] _parametertypes, out Operator _operator)
        {
            IList<Operator> _list;
            if (!_operators2.TryGetValue(_predefinedoperator, out _list))
            {
                _operator = null;
                return false;
            }

            foreach (Operator _o in _list)
            {
                if (((_resulttype == null) || ValueType.Equal(_resulttype, _o.ResultType))
                    && ValueType.Equal(_parametertypes, _o.ParameterTypes))
                {
                    _operator = _o;
                    return true;
                }
            }

            _operator = null;
            return false;
        }

        public ValueType InstantiateTypeTemplate(ValueTypeTemplate _template, ValueType[] _arguments)
        {
            ValueType _valuetype;
            if (_InstantiateTypeTemplate_1(_template, _arguments, out _valuetype))
                _InstantiateTypeTemplate_2(_valuetype);
            return _valuetype;
        }

        public IEnumerable<Protocol> Protocols
        {
            get { return _protocols.Values; }
        }

        public void AddProtocol(Protocol protocol)
        {
            protocol.Library = this;
            _protocols.Add(protocol.Name, protocol);
        }

        public Protocol GetProtocol(string _name)
        {
            Protocol _protocol;
            if (!_protocols.TryGetValue(_name, out _protocol))
                throw new Exception("Protocol named \"" + _name + "\" has not been defined.");
            return _protocol;
        }

        public bool TryCast(ValueType _valuetype, ValueType _giventype, out Operator _castingoperator)
        {
            if (_valuetype == null)
                throw new Exception("Cannot cast to a null type.");
            return _valuetype.TryCast(_giventype, out _castingoperator);
        }

        public Operator Cast(ValueType _valuetype, ValueType _giventype)
        {
            Operator _castingoperator;
            if (TryCast(_valuetype, _giventype, out _castingoperator))
                return _castingoperator;
            else
                throw new Exception("Cannot find a suitable operator to cast from type \"" + _giventype.Name + "\" to type \"" + _valuetype.Name + "\".");
        }

        public Expression Cast(ValueType _valuetype, Expression _expression)
        {
            if (ValueType.Equal(_valuetype, _expression.ValueType))
                return _expression;
            else
            {
                Operator _operator = Cast(_valuetype, _expression.ValueType);
                return new Operation(_operator, new Expression[] { _expression });
            }
        }

        public Expression CastConstant(PredefinedType _defaulttype, ValueType _desiredtype, object _valueobject)
        {
            return CastConstant(GetValueType(_defaulttype), _desiredtype, _valueobject);
        }

        public Expression CastConstant(ValueType _defaulttype, ValueType _desiredtype, object _valueobject)
        {
            if ((_desiredtype == null) || ValueType.Equal(_desiredtype, _defaulttype))
                return new Constant(_defaulttype, _valueobject);
            else
            {
                Operator _operator;
                if (_desiredtype.TryCast(_defaulttype, out _operator))
                    return new Operation(_operator, new Expression[] { new Constant(_defaulttype, _valueobject) });
                else
                    throw new Exception(
                        "Cannot find a suitable operator to cast from \"" + _defaulttype.ToString() + "\" to \"" + _desiredtype.Name + "\".");
            }
        }

        public Expression MakeOperation(PredefinedOperator _predefinedoperator, ValueType _resulttype, Expression[] _parameters)
        {
            try
            {
                return _MakeOperation(_FindOperators(_predefinedoperator, _resulttype), _parameters);
            }
            catch (Exception exc)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Could not find predefined operator " + _predefinedoperator.ToString() + " with result type " +
                    ((_resulttype != null) ? _resulttype.Name : "unspecified") + " and parameter types ");
                for (int _k = 0; _k < _parameters.Length; _k++)
                {
                    if (_k > 0)
                        sb.Append(", ");
                    sb.Append(_parameters[_k].ValueType.Name);
                }
                throw new Exception(sb.ToString(), exc);
            }
        }

        public Expression MakeOperation(string _operatorname, ValueType _resulttype, Expression[] _parameters)
        {
            try
            {
                return _MakeOperation(_FindOperators(_operatorname, _resulttype), _parameters);
            }
            catch (Exception exc)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Could not find custom operator " + _operatorname + " with result type " +
                    ((_resulttype != null) ? _resulttype.Name : "unspecified") + " and parameter types ");
                for (int _k = 0; _k < _parameters.Length; _k++)
                {
                    if (_k > 0)
                        sb.Append(", ");
                    sb.Append(_parameters[_k].ValueType.Name);
                }
                throw new Exception(sb.ToString(), exc);
            }
        }

        public Expression MakeOperation(
            PredefinedOperator _predefinedoperator, ValueType _resulttype, ValueType[] _parametertypes, Expression[] _parameters)
        {
            try
            {
                IList<Operator> _oo = new List<Operator>();
                foreach (Operator _o in _FindOperators(_predefinedoperator, _resulttype))
                {
                    if (_ParameterTypesOk(_o.ParameterTypes, _parametertypes))
                        _oo.Add(_o);
                }
                return _MakeOperation(_oo, _parameters);
            }
            catch (Exception exc)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Could not find predefined operator " + _predefinedoperator.ToString() + " with result type " +
                    ((_resulttype != null) ? _resulttype.Name : "unspecified") + " and parameter types ");
                for (int _k = 0; _k < _parameters.Length; _k++)
                {
                    if (_k > 0)
                        sb.Append(", ");
                    sb.Append(_parameters[_k].ValueType.Name);
                }
                throw new Exception(sb.ToString(), exc);
            }
        }

        public Expression MakeOperation(
            string _operatorname, ValueType _resulttype, ValueType[] _parametertypes, Expression[] _parameters)
        {
            try
            {
                IList<Operator> _oo = new List<Operator>();
                foreach (Operator _o in _FindOperators(_operatorname, _resulttype))
                {
                    if (_ParameterTypesOk(_o.ParameterTypes, _parametertypes))
                        _oo.Add(_o);
                }
                return _MakeOperation(_oo, _parameters);
            }
            catch (Exception exc)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Could not find operator named \"" + _operatorname + "\" with result type " +
                    ((_resulttype != null) ? _resulttype.Name : "unspecified") + " and parameter types ");
                for (int _k = 0; _k < _parameters.Length; _k++)
                {
                    if (_k > 0)
                        sb.Append(", ");
                    sb.Append(_parameters[_k].ValueType.Name);
                }
                throw new Exception(sb.ToString(), exc);
            }
        }

        #endregion
    }
}
