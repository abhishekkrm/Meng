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

// #define OPTION_DisallowAmbiguityInCastingOperators

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace QS._qss_x_.Language_.Structure_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class ValueType
    {
        #region Constructors

        public ValueType(System.Type _underlyingtype, string _name, PredefinedType _predefinedtype)
            : this(_underlyingtype, _name, _predefinedtype, null, null)
        {
        }

        public ValueType(System.Type _underlyingtype, string _name, PredefinedType _predefinedtype,
            ValueTypeTemplate _typetemplate, ValueType[] _typetemplatearguments)
        {
            this._underlyingtype = _underlyingtype;
            this._name = _name;
            this._predefinedtype = _predefinedtype;
            this._typetemplate = _typetemplate;
            this._typetemplatearguments = _typetemplatearguments;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private string _name;
        [QS.Fx.Printing.Printable]
        private PredefinedType _predefinedtype;
        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
        private System.Type _underlyingtype;
        private ValueTypeTemplate _typetemplate;
        private ValueType[] _typetemplatearguments;

        private IDictionary<string, IList<Operator>> _operators_1 = new Dictionary<string, IList<Operator>>();
        private IDictionary<PredefinedOperator, IList<Operator>> _operators_2 = new Dictionary<PredefinedOperator, IList<Operator>>();
        private IDictionary<ValueType, Operator> _operators_3 = new Dictionary<ValueType, Operator>();

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return _name;
        }

        #endregion

        #region Interface

        public string Name
        {
            get { return _name; }
        }

        public PredefinedType PredefinedType
        {
            get { return _predefinedtype; }
        }

        public System.Type UnderlyingType
        {
            get { return _underlyingtype; }
        }

        public ValueTypeTemplate ValueTypeTemplate
        {
            get { return _typetemplate; }
        }

        public ValueType[] ValueTypeTemplateArguments
        {
            get { return _typetemplatearguments; }
        }

        public void RegisterOperator(Operator _operator)
        {
            if (!ReferenceEquals(_operator.ResultType, this))
                throw new Exception("Cannot register operator.");

            foreach (string _name in _operator.Names)
            {
                IList<Operator> _o_1;
                if (!_operators_1.TryGetValue(_name, out _o_1))
                {
                    _o_1 = new List<Operator>();
                    _operators_1.Add(_name, _o_1);
                }
                _o_1.Add(_operator);
            }

            bool _does_create = false;
            foreach (PredefinedOperator _predefinedoperator in _operator.PredefinedOperators)
            {
                if (_predefinedoperator != PredefinedOperator.None)
                {
                    IList<Operator> _o_2;
                    if (!_operators_2.TryGetValue(_predefinedoperator, out _o_2))
                    {
                        _o_2 = new List<Operator>();
                        _operators_2.Add(_predefinedoperator, _o_2);
                    }
                    _o_2.Add(_operator);
                }

                if (_predefinedoperator == PredefinedOperator.Create)
                    _does_create = true;
            }

            if (_does_create && (_operator.ParameterTypes.Length == 1))
            {
                if (_operators_3.ContainsKey(_operator.ParameterTypes[0]))
                {
#if OPTION_DisallowAmbiguityInCastingOperators
                    throw new Exception("More than one casting operator has been found to cast from \"" +
                        _operator.ParameterTypes[0].Name + "\" to \"" + _name + "\".");
#endif
                }
                else
                    _operators_3.Add(_operator.ParameterTypes[0], _operator);
            }
        }

        public IEnumerable<Operator> GetOperators(string _name)
        {
            IList<Operator> _o_1;
            if (_operators_1.TryGetValue(_name, out _o_1))
                return _o_1;
            else
                return new Operator[0];
        }

        public IEnumerable<Operator> GetOperators(PredefinedOperator _predefinedoperator)
        {
            IList<Operator> _o_2;
            if (_operators_2.TryGetValue(_predefinedoperator, out _o_2))
                return _o_2;
            else
                return new Operator[0];
        }

        public bool TryCast(ValueType _sourcetype, out Operator _castingoperator)
        {
            return _operators_3.TryGetValue(_sourcetype, out _castingoperator);
        }

        public Operator Cast(ValueType _sourcetype)
        {
            Operator _castingoperator;
            if (!TryCast(_sourcetype, out _castingoperator))
                throw new Exception("Cannot cast from type \"" + _sourcetype.Name + "\" to type \"" + _name + "\".");
            return _castingoperator;
        }

        public static bool Equal(ValueType t1, ValueType t2)
        {
            return (t1 == null) ? (t2 == null) : ((t2 != null) && 
                t1.Name.Equals(t2.Name) && ReferenceEquals(t1.UnderlyingType, t2.UnderlyingType));
        }

        public static bool Equal(ValueType[] t1, ValueType[] t2)
        {
            if (t1 == null)
            {
                return (t2 == null);
            }
            else
            {
                if ((t2 == null) || (t2.Length != t1.Length))
                    return false;
                for (int _k = 0; _k < t1.Length; _k++)
                {
                    if (!Equal(t1[_k], t2[_k]))
                        return false;
                }
                return true;
            }
        }

        #endregion
    }
}
