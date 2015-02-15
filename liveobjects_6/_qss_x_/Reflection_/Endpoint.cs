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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Reflection_
{
    [QS.Fx.Printing.Printable("Endpoint", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    internal sealed class Endpoint : QS.Fx.Inspection.Inspectable, QS.Fx.Reflection.IEndpoint
    {
        #region Create(QS.Fx.Reflection.Xml.Endpoint)

/*
        public static Endpoint Create(QS.Fx.Reflection.Xml.Endpoint _xmlendpoint)
        {
            string _id = _xmlendpoint.ID;
            IEndpointClass _endpointclass = EndpointClass.Create(_xmlendpoint.EndpointClass);
            return new Endpoint(_id, _endpointclass, null);
        }
*/

        #endregion

        #region Constructor

        public Endpoint(string _id, QS.Fx.Reflection.IEndpointClass _endpointclass, System.Reflection.MemberInfo _memberinfo)
        {
            this._id = _id;
            this._endpointclass = _endpointclass;
            this._memberinfo = _memberinfo;
        }

        #endregion

        #region Fields

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("id")]
#endif
        private string _id;
#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("endpointclass")]
#endif
        private QS.Fx.Reflection.IEndpointClass _endpointclass;
#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("memberinfo")]
#endif
        private System.Reflection.MemberInfo _memberinfo;

        #endregion

        #region IEndpoint Members

        string QS.Fx.Reflection.IEndpoint.ID
        {
            get { return _id; }
        }

        QS.Fx.Reflection.IEndpointClass QS.Fx.Reflection.IEndpoint.EndpointClass
        {
            get { return _endpointclass; }
        }

        QS.Fx.Reflection.IEndpoint QS.Fx.Reflection.IEndpoint.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            return new Endpoint(this._id, this._endpointclass.Instantiate(_parameters), this._memberinfo);
        }

        bool QS.Fx.Reflection.IEndpoint.IsSubtypeOf(QS.Fx.Reflection.IEndpoint other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!_endpointclass.IsSubtypeOf(other.EndpointClass))
                return false;

            if (!(other is Endpoint))
                throw new NotImplementedException();

            Endpoint _m_other = (Endpoint) other;

            if (_m_other._constraints_provided != null)
            {
                if (_constraints_provided == null)
                    return false;
                foreach (KeyValuePair<string, EndpointConstraint> _element in _m_other._constraints_provided)
                {
                    EndpointConstraint _constraint1 = _element.Value;
                    EndpointConstraint _constraint2;
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
                foreach (KeyValuePair<string, EndpointConstraint> _element in _constraints_required)
                {
                    EndpointConstraint _constraint1 = _element.Value;
                    EndpointConstraint _constraint2;
                    if (!_m_other._constraints_required.TryGetValue(_element.Key, out _constraint2))
                        return false;

                    if (!_constraint1.WeakerThan(_constraint2))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Handling Constraints

        private IDictionary<string, EndpointConstraint> _constraints_provided, _constraints_required;

        public void _AddConstraint(QS.Fx.Reflection.ConstraintKind _constraintkind, string _constraintclass, EndpointConstraint _constraint)
        {
            switch (_constraintkind)
            {
                case QS.Fx.Reflection.ConstraintKind.Provided:
                    {
                        if (_constraints_provided == null)
                            _constraints_provided = new Dictionary<string, EndpointConstraint>();
                        _constraints_provided.Add(_constraintclass, _constraint);
                    }
                    break;

                case QS.Fx.Reflection.ConstraintKind.Required:
                    {
                        if (_constraints_required == null)
                            _constraints_required = new Dictionary<string, EndpointConstraint>();
                        _constraints_required.Add(_constraintclass, _constraint);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<QS.Fx.Reflection.Xml.EndpointConstraint> _SerializeConstraints()
        {
            List<QS.Fx.Reflection.Xml.EndpointConstraint> _endpointconstraints = new List<QS.Fx.Reflection.Xml.EndpointConstraint>();
            if (this._constraints_provided != null)
                foreach (EndpointConstraint _constraint in this._constraints_provided.Values)
                    _endpointconstraints.Add(
                        new QS.Fx.Reflection.Xml.EndpointConstraint(
                            this._id,
                            "provided",
                            _constraint.ConstraintClass.uuid_,
                            _constraint.Constraint.ToString()));
            if (this._constraints_required != null)
                foreach (EndpointConstraint _constraint in this._constraints_required.Values)
                    _endpointconstraints.Add(
                        new QS.Fx.Reflection.Xml.EndpointConstraint(
                            this._id,
                            "required",
                            _constraint.ConstraintClass.uuid_,
                            _constraint.Constraint.ToString()));
            return _endpointconstraints;
        }

        #endregion
    }
}
