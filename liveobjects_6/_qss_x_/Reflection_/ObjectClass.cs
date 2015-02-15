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

#define DEBUG_INCLUDE_INSPECTION_CODE
// #define DEBUG_STACK_OVERFLOW

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Reflection_
{
    internal sealed class ObjectClass : Class<QS.Fx.Reflection.IObjectClass, ObjectClass>, QS.Fx.Reflection.IObjectClass
    {
        #region Constructor(QS.Fx.Base.ID,string,string,Type,IDictionary<string,IParameter>,IDictionary<string,IEndpoint>)

        internal ObjectClass(
            QS.Fx.Reflection.IObjectClass _original_base_template_objectclass, 
            Library.Namespace_ _namespace, QS.Fx.Base.ID _id, ulong _incarnation, string _name, string _comment, Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters,
            IDictionary<string, QS.Fx.Reflection.IEndpoint> _endpoints,
            QS.Fx.Reflection.IObjectClass _authenticating_objectclass)
            : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {
            this._endpoints = _endpoints;
            if (_original_base_template_objectclass == null)
                _original_base_template_objectclass = this;
            this._original_base_template_objectclass = _original_base_template_objectclass;
            this._authenticating_objectclass = _authenticating_objectclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IObjectClass _original_base_template_objectclass, _authenticating_objectclass;

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Printing.Printable("endpoints")]
#endif
        private IDictionary<string, QS.Fx.Reflection.IEndpoint> _endpoints;

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Reflection_.Internal_._internal_info_objectclass _internal_info;

#if DEBUG_STACK_OVERFLOW
        private static readonly QS._qss_x_.Base1_.StackOverflow_ _stackoverflow = new QS._qss_x_.Base1_.StackOverflow_(20);
#endif

        #endregion

        #region internal_info_

        public QS._qss_x_.Reflection_.Internal_._internal_info_objectclass internal_info_
        {
            get { return this._internal_info; }
            set { this._internal_info = value; }
        }

        #endregion

        #region _Original_Base_Template_ObjectClass

        internal QS.Fx.Reflection.IObjectClass _Original_Base_Template_ObjectClass
        {
            get { return this._original_base_template_objectclass; }
        }

        #endregion

        #region Inspection

#if DEBUG_INCLUDE_INSPECTION_CODE

        [QS.Fx.Base.Inspectable("endpoints")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IEndpoint> __inspectable_endpoints
        {
            get
            {
                return new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IEndpoint>("_endpoints", _endpoints,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IEndpoint>.ConversionCallback(
                        delegate(string s) { return s; }));
            }
        }

#endif

        #endregion

        #region IObjectClass Members

        IDictionary<string, QS.Fx.Reflection.IEndpoint> QS.Fx.Reflection.IObjectClass.Endpoints
        {
            get { return new QS._qss_x_.Base1_.ReadonlyDictionaryOf<string, QS.Fx.Reflection.IEndpoint>(_endpoints); }
        }

        QS.Fx.Reflection.Xml.ObjectClass QS.Fx.Reflection.IObjectClass.Serialize
        {
            get
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append(this._namespace.uuid_);
                _ss.Append(":");
                _ss.Append(this._uuid);
                List<QS.Fx.Reflection.Xml.ObjectConstraint> _objectconstraints = new List<QS.Fx.Reflection.Xml.ObjectConstraint>();
                if (_constraints_provided != null)
                    foreach (ObjectConstraint _constraint in this._constraints_provided.Values)
                        _objectconstraints.Add(
                            new QS.Fx.Reflection.Xml.ObjectConstraint(
                                "provided",
                                _constraint.ConstraintClass.uuid_,
                                _constraint.Constraint.ToString()));
                if (_constraints_required != null)
                    foreach (ObjectConstraint _constraint in this._constraints_required.Values)
                        _objectconstraints.Add(
                            new QS.Fx.Reflection.Xml.ObjectConstraint(
                                "required",
                                _constraint.ConstraintClass.uuid_,
                                _constraint.Constraint.ToString()));
                List<QS.Fx.Reflection.Xml.EndpointConstraint> _endpointconstraints = new List<QS.Fx.Reflection.Xml.EndpointConstraint>();
                if (this._endpoints != null)
                    foreach (Endpoint _endpoint in this._endpoints.Values)
                        _endpointconstraints.AddRange(_endpoint._SerializeConstraints());
                return 
                    new QS.Fx.Reflection.Xml.ObjectClass(
                        _ss.ToString(),
                        QS.Fx.Reflection.Parameter.Serialize(this._classparameters.Values),
                        _objectconstraints.ToArray(),
                        _endpointconstraints.ToArray());
            }
        }

        bool QS.Fx.Reflection.IObjectClass.IsSubtypeOf(QS.Fx.Reflection.IObjectClass other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!(other is ObjectClass))
                throw new NotImplementedException();

            ObjectClass _m_other = (ObjectClass) other;

            if (_m_other._constraints_provided != null)
            {
                if (_constraints_provided == null)
                    return false;
                foreach (KeyValuePair<string, ObjectConstraint> _element in _m_other._constraints_provided)
                {
                    ObjectConstraint _constraint1 = _element.Value;
                    ObjectConstraint _constraint2;
                    if (!_constraints_provided.TryGetValue(_element.Key, out _constraint2))
                        return false;

                    if (!_constraint1.WeakerThan(_constraint2))
                        return false;
                }
            }

            if (_constraints_required != null)
            {
                if (_m_other._constraints_required == null)
                    return false;
                foreach (KeyValuePair<string, ObjectConstraint> _element in _constraints_required)
                {
                    ObjectConstraint _constraint1 = _element.Value;
                    ObjectConstraint _constraint2;
                    if (!_m_other._constraints_required.TryGetValue(_element.Key, out _constraint2))
                        return false;

                    if (!_constraint1.WeakerThan(_constraint2))
                        return false;
                }
            }

            foreach (KeyValuePair<string, QS.Fx.Reflection.IEndpoint> element in other.Endpoints)
            {
                QS.Fx.Reflection.IEndpoint myendpoint, otherendpoint;
                if (!this._endpoints.TryGetValue(element.Key, out myendpoint))
                    return false;

                otherendpoint = element.Value;
                if (!myendpoint.IsSubtypeOf(otherendpoint))
                    return false;
            }

            if (other.AuthenticatingClass != null)
            {
                if (this._authenticating_objectclass != null)
                {
                    if (!this._authenticating_objectclass.IsSubtypeOf(other.AuthenticatingClass))
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObjectClass.AuthenticatingClass
        {
            get { return this._authenticating_objectclass; }
        }

        #endregion

        #region _Instantiate

        protected override ObjectClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
#if DEBUG_STACK_OVERFLOW
            _stackoverflow._Enter();
#endif

            IDictionary<string, QS.Fx.Reflection.IParameter> _new_classparameters, _new_openparameters;
            Library._InstantiateParameters(_parameters, this._classparameters, this._openparameters, out _new_classparameters, out _new_openparameters);
            IDictionary<string, QS.Fx.Reflection.IEndpoint> _new_endpoints = new Dictionary<string, QS.Fx.Reflection.IEndpoint>();
            if (this._endpoints != null)
                foreach (QS.Fx.Reflection.IEndpoint _endpoint in this._endpoints.Values)
                    _new_endpoints.Add(_endpoint.ID, _endpoint.Instantiate(_parameters));
            ObjectClass _result = new ObjectClass(this._original_base_template_objectclass,
                this._namespace, this._id, this._incarnation, this._name, this._comment, this._type, _new_classparameters, _new_openparameters, _new_endpoints,
                ((this._authenticating_objectclass != null) ? this._authenticating_objectclass.Instantiate(_parameters) : null));

#if DEBUG_STACK_OVERFLOW
            _stackoverflow._Leave();
#endif

            return _result;
        }

        #endregion

        #region _ID

        public string _ID
        {
            get
            {
                if ((this._uuid != null) && (this._namespace != null) && (this._namespace.uuid_ != null))
                    return this._Namespace.uuid_ + ":" + this.uuid_;
                else
                    return null;
            }
        }

        #endregion

        #region Handling Constraints

        private IDictionary<string, ObjectConstraint> _constraints_provided, _constraints_required;

        public void _AddConstraint(QS.Fx.Reflection.ConstraintKind _constraintkind, string _constraintclass, ObjectConstraint _constraint)
        {
            switch (_constraintkind)
            {
                case QS.Fx.Reflection.ConstraintKind.Provided:
                    {
                        if (_constraints_provided == null)
                            _constraints_provided = new Dictionary<string, ObjectConstraint>();
                        _constraints_provided.Add(_constraintclass, _constraint);
                    }
                    break;

                case QS.Fx.Reflection.ConstraintKind.Required:
                    {
                        if (_constraints_required == null)
                            _constraints_required = new Dictionary<string, ObjectConstraint>();
                        _constraints_required.Add(_constraintclass, _constraint);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public ObjectClass _Clone()
        {
            return 
                new ObjectClass(
                    this._original_base_template_objectclass,
                    this._namespace, 
                    this._id, 
                    this._incarnation, 
                    this._name, 
                    this._comment, 
                    this._type, 
                    this._classparameters,
                    this._openparameters,
                    this._endpoints, 
                    this._authenticating_objectclass);
        }

        public ObjectClass _DeserializeConstraints(IEnumerable<QS.Fx.Reflection.Xml.EndpointConstraint> _xmlendpointconstraints)
        {
/*
                if (_xmlobjectclass.EndpointConstraints != null)
                {
                    IDictionary<string, IList<QS._qss_x_.Reflection_.EndpointConstraint>>
                        _endpointconstraints =
                            new Dictionary<string, IList<QS._qss_x_.Reflection_.EndpointConstraint>>();
                    foreach (QS.Fx.Reflection.Xml.EndpointConstraint _xmlconstraint in _xmlobjectclass.EndpointConstraints)
                    {
/-*
                        QS.Fx.Reflection.IEndpointConstraintClass _constraintclass = this._library.GetEndpointConstraintClass(_xmlconstraint.Class);
                        QS.Fx.Reflection.IEndpointConstraint _constraint = _constraintclass.CreateConstraint();
                        _constraint.Initialize(_xmlconstraint.Value, _endpointclass);
                        QS._qss_x_.Reflection_.ObjectConstraint _objectconstraint =
                            new QS._qss_x_.Reflection_.ObjectConstraint(
                                (QS._qss_x_.Reflection_.ObjectConstraintClass)_constraintclass, _constraint);
                        _objectconstraints.Add(_objectconstraint);
*-/
                    }
                }
                
*/

/*
            IDictionary<string, QS.Fx.Reflection.IEndpoint> _new_endpoints = new Dictionary<string, QS.Fx.Reflection.IEndpoint>();
            if (this._endpoints != null)
                foreach (QS.Fx.Reflection.IEndpoint _endpoint in this._endpoints.Values)
                    _new_endpoints.Add(_endpoint.ID, _endpoint.Instantiate(_parameters));
            ObjectClass _result = new ObjectClass(this._original_base_template_objectclass,
                this._namespace, this._id, this._incarnation, this._name, this._comment, this._type, _new_classparameters, _new_openparameters, _new_endpoints,
                ((this._authenticating_objectclass != null) ? this._authenticating_objectclass.Instantiate(_parameters) : null));
*/

            return this;
        }

        #endregion

        #region Authentication

        public void _SetAuthenticatingClass(QS.Fx.Reflection.IObjectClass _authenticatingclass)
        {
            if (this._authenticating_objectclass != null)
                throw new Exception("Cannot set the authenticating class twice.");
            this._authenticating_objectclass = _authenticatingclass;
        }

        #endregion

        #region _IsInternal

        public bool _IsInternal
        {
            get
            {
                if ((this._original_base_template_objectclass != null) && !ReferenceEquals(this, this._original_base_template_objectclass))
                    return ((ObjectClass) this._original_base_template_objectclass)._IsInternal;
                else
                {
                    if (this._type != null)
                    {
                        object[] _internalattributes = this._type.GetCustomAttributes(typeof(QS._qss_x_.Reflection_.InternalAttribute), true);
                        return ((_internalattributes != null) && (_internalattributes.Length > 0));
                    }
                    else
                        return false;
                }
            }
        }

        #endregion
    }
}
