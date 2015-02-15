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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Protocol
    {
        #region Constructors

        public Protocol(string _name)
        {
            this._name = _name;
            this._interface = new Interface(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private Library _library;
        [QS.Fx.Printing.Printable]
        private string _name;
        [QS.Fx.Printing.Printable]
        private Interface _interface;
        [QS.Fx.Printing.Printable]
        private IDictionary<string, Property> _properties = new Dictionary<string, Property>();
        [QS.Fx.Printing.Printable]
        private IList<Rule> _rules= new List<Rule>();
        [QS.Fx.Printing.Printable]
        private IDictionary<string, Protocol> _baseprotocols = new Dictionary<string, Protocol>();
        [QS.Fx.Printing.Printable]
        private IList<Flow> _flows = new List<Flow>();
        [QS.Fx.Printing.Printable]
        private IList<Condition> _conditions = new List<Condition>();
        [QS.Fx.Printing.Printable]
        private IList<Binding> _bindings = new List<Binding>();
        [QS.Fx.Printing.Printable]
        private IList<OnInitialization> _initializers = new List<OnInitialization>();
        [QS.Fx.Printing.Printable]
        private IList<Property> _autos = new List<Property>();

        #endregion

        #region Interface

        public Library Library
        {
            get { return _library; }
            set { _library = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Interface Interface
        {
            get { return _interface; }
        }

        public IEnumerable<Property> Properties
        {
            get { return _properties.Values; }
        }

        public void AddBaseProtocol(Protocol _protocol)
        {
            if (_baseprotocols.ContainsKey(_protocol.Name))
                throw new Exception("Protocol named \"" + _protocol.Name + "\" has already been declared as a base protocol for \"" + _name + "\".");
            _baseprotocols.Add(_protocol.Name, _protocol);
        }

        public void AddProperty(Property _property)
        {
            if (_properties.ContainsKey(_property.Name))
                throw new Exception("Property named \"" + _property.Name + "\" has already been defined in protocol \"" + _name + "\".");
            _properties.Add(_property.Name, _property);
        }

        public void AddRule(Rule _rule)
        {
            _rules.Add(_rule);
        }

        public void AddFlow(Flow _flow)
        {
            _flows.Add(_flow);
        }

        public void AddCondition(Condition _condition)
        {
            _conditions.Add(_condition);
        }

        public void AddBinding(Binding _binding)
        {
            _bindings.Add(_binding);
        }

        public IEnumerable<Rule> Rules
        {
            get { return _rules; }
        }

        public IEnumerable<Flow> Flows
        {
            get { return _flows; }
        }

        public IEnumerable<Condition> Conditions
        {
            get { return _conditions; }
        }

        public IEnumerable<Binding> Bindings
        {
            get { return _bindings; }
        }

        public bool TryGetBaseProtocol(string _name, out Protocol _protocol)
        {
            return _baseprotocols.TryGetValue(_name, out _protocol);
        }

        public Protocol GetProtocol(string _name)
        {
            if (_name.Equals(this._name))
                return this;
            else
                return GetBaseProtocol(_name);
        }

        public Protocol GetBaseProtocol(string _name)
        {
            Protocol _protocol;
            if (!TryGetBaseProtocol(_name, out _protocol))
                throw new Exception("Protocol \"" + this._name + "\" does not extend protocol \"" + _name + "\".");
            return _protocol;
        }

        public bool TryGetProperty(string _name, out Property _property)
        {
            return _properties.TryGetValue(_name, out _property);
        }

        public Property GetProperty(string _name)
        {
            Property _property;
            if (!TryGetProperty(_name, out _property))
                throw new Exception("Cannot find property \"" + _name + "\" in protocol \"" + this._name + "\".");
            return _property;
        }

        public void AddInitializer(OnInitialization _initializer)
        {
            _initializers.Add(_initializer);
        }

        public IEnumerable<OnInitialization> Initializers
        {
            get { return _initializers; }
        }

        public ValueOf Flatten(Expression _expression, Placement _placement)
        {
            Property _property = null;
            bool _isconstant = _expression.IsConstant;
            if (_isconstant)
            {
                foreach (Property _p in _properties.Values)
                {
                    if (_p.IsConstant && _expression.Equals(_p.InitialValue))
                    {
                        _property = _p;
                        break;
                    }
                }
            }
            if (_property == null)
            {
                foreach (Rule _rule in _rules)
                {
                    if (_rule.Update.UpcateCategory == UpdateCategory.Assignment && ((Assignment) _rule.Update).Value.Equals(_expression))
                    {
                        Variable _v = ((Assignment) _rule.Update).Variable;
                        if (_v is Property)
                        {
                            _property = (Property)_v;
                            break;
                        }
                    }
                }
            }
            if (_property == null)
            {
                string _name;
                switch (_expression.ExpressionCategory)
                {
                    case ExpressionCategory.Constant:
                    case ExpressionCategory.ValueOf:
                    case ExpressionCategory.Operation:
                    case ExpressionCategory.FunctionCall:
                        _name = "_e_" + (_autos.Count + 1).ToString();
                        break;

                    default:
                        throw new NotImplementedException();
                }
                if (_isconstant)
                    _property = new Property(this, _expression.ValueType, _name, _expression, _placement, PropertyAttributes.Alias, null);
                else
                {
                    _property = new Property(this, _expression.ValueType, _name, null, _placement, PropertyAttributes.Alias, null);
                    Rule _rule = new Rule(new Assignment(_property, _expression), _placement, RuleAttributes.None, null);
                    _rules.Add(_rule);
                }
                _properties.Add(_name, _property);
                _autos.Add(_property);
            }
            return new ValueOf(_property);
        }

        public Expression Disseminate(Expression _expression, ref Placement _placement)
        {
            if (_expression.IsConstant)
                return _expression;

            ValueOf _valueof = (_expression is ValueOf) ? ((ValueOf)_expression) : Flatten(_expression, _placement);
            return Disseminate(_valueof.Variable, ref _placement);
        }

        public Expression Disseminate(Variable _variable, ref Placement _placement)
        {
            if (_variable.VariableCategory != VariableCategory.Property)
                throw new Exception("Cannot create a data flow for something that is not a property.");
            
            Property _property = (Property)_variable;

            if (_placement != Placement.Child)
            {
                if (_placement == Placement.Undefined)
                    _placement = Placement.Child;
                else
                    throw new Exception("References to parent values are only allowed in rules that have the \"child\" placement.");
            }

            if ((_property.Placement != Placement.Parent) && (_property.Placement != Placement.Undefined))
                throw new Exception("References to parent values are only legal for properties with the \"parent\" or unspecified placement.");

            Dissemination _diss = null;

            foreach (Flow _flow in _flows)
            {
                if ((_flow.FlowCategory == FlowCategory.Dissemination) && ReferenceEquals(((Dissemination)_flow).SourceProperty, _property))
                {
                    _diss = (Dissemination) _flow;
                    break;
                }
            }

            if (_diss == null)
            {                
                ValueType _aggregatedtype = _library.InstantiateTypeTemplate(
                    _library.GetValueTypeTemplate(PredefinedTypeTemplate.VERSIONED), new ValueType[] { _property.ValueType });

                Property _newproperty = new Property(this, _aggregatedtype,
                    "_e_" + (_autos.Count + 1).ToString(), null, _placement, PropertyAttributes.Alias, null);
                this.AddProperty(_newproperty);
                _autos.Add(_newproperty);

                _diss = new Dissemination(_property, _newproperty);
                this.AddFlow(_diss);
            }

            return new Operation(
                _library.GetOperator(PredefinedOperator.Value, _property.ValueType, new ValueType[] { _diss.TargetProperty.ValueType }),
                   new Expression[] { new ValueOf(_diss.TargetProperty) });
        }

        public Expression Aggregate(
            bool _ischildren, Expression _expression, Operator _operator, AggregationAttributes _attributes, ref Placement _placement)
        {
            if (_expression.IsConstant)
                return _expression;

            ValueOf _valueof = (_expression is ValueOf) ? ((ValueOf)_expression) : Flatten(_expression, _placement);
            return Aggregate(_ischildren, _valueof.Variable, _operator, _attributes, ref _placement);
        }

        public Expression Aggregate(
            bool _ischildren, Variable _variable, Operator _operator, AggregationAttributes _attributes, ref Placement _placement)
        {
            if (_variable.VariableCategory != VariableCategory.Property)
                throw new Exception("Cannot create a data flow for something that is not a property.");

            Property _property = (Property)_variable;

            if (_placement != Placement.Parent)
            {
                if (_placement == Placement.Undefined)
                    _placement = Placement.Parent;
                else
                    throw new Exception("References to parent values are only allowed in rules that have the \"parent\" placement.");
            }

            if ((_property.Placement != Placement.Child) && (_property.Placement != Placement.Undefined))
                throw new Exception("References to parent values are only legal for properties with the \"child\" or unspecified placement.");

            if (_operator.IsUnknown)
            {
                ValueType _resulttype = _operator.ResultType;
                ValueType[] _parametertypes = _operator.ParameterTypes;

                if (_resulttype == null)
                    _resulttype = _property.ValueType;
                else
                {
                    if (!_resulttype.Equals(_property.ValueType))
                        throw new Exception("Aggregation operator result type must be same as the type of the aggregated property.");
                }

                if (_parametertypes == null)
                    _parametertypes = new ValueType[] { _resulttype, _resulttype };
                else
                {
                    if (_parametertypes.Length != 2)
                        throw new Exception("Aggregation operator must be a binary operator.");

                    if (!_parametertypes[0].Equals(_property.ValueType) || !_parametertypes[1].Equals(_property.ValueType))
                        throw new Exception("Aggregation operator parameter types must be the same as the result type.");
                }

                if (_operator.Names != null)
                    _operator = _library.GetOperator(_operator.Names[0], _resulttype, _parametertypes);
                else
                    _operator = _library.GetOperator(_operator.PredefinedOperators[0], _resulttype, _parametertypes);
            }
            else
            {
                ValueType _resulttype = _operator.ResultType;
                ValueType[] _parametertypes = _operator.ParameterTypes;

                if (!_resulttype.Equals(_property.ValueType))
                    throw new Exception("Aggregation operator result type must be same as the type of the aggregated property.");

                if (_parametertypes.Length != 2)
                    throw new Exception("Aggregation operator must be a binary operator.");

                if (!_parametertypes[0].Equals(_property.ValueType) || !_parametertypes[1].Equals(_property.ValueType))
                    throw new Exception("Aggregation operator parameter types must be the same as the result type.");
            }

            Aggregation _aggr = null;

            foreach (Flow _flow in _flows)
            {
                if ((_flow.FlowCategory == FlowCategory.Aggregation) && ReferenceEquals(((Aggregation) _flow).SourceProperty, _property) &&
                    ((Aggregation) _flow).Operator.Equals(_operator))
                {
                    _aggr = (Aggregation) _flow;
                    break;
                }
            }

            if (_aggr == null)
            {
                ValueType _aggregatedtype = _library.InstantiateTypeTemplate(
                    _library.GetValueTypeTemplate(PredefinedTypeTemplate.VERSIONED), new ValueType[] { _property.ValueType });

                Property _groupaggregate = new Property(this, _aggregatedtype,
                    "_e_" + (_autos.Count + 1).ToString(), null, _property.Placement | Placement.Child, PropertyAttributes.Alias, null);
                this.AddProperty(_groupaggregate);
                _autos.Add(_groupaggregate);

                Property _parentaggregate = new Property(this, _aggregatedtype,
                    "_e_" + (_autos.Count + 1).ToString(), null, _placement, PropertyAttributes.Alias, null);
                this.AddProperty(_parentaggregate);
                _autos.Add(_parentaggregate);

                _aggr = new Aggregation(_operator, _attributes, _property, _groupaggregate, _parentaggregate);
                this.AddFlow(_aggr);
            }

            return new Operation(
                _library.GetOperator(PredefinedOperator.Value, _property.ValueType, new ValueType[] { _aggr.GroupAggregate.ValueType }),
                   new Expression[] { new ValueOf(_ischildren ?_aggr.ParentAggregate : _aggr.GroupAggregate) });
        }

        #endregion
    }
}
