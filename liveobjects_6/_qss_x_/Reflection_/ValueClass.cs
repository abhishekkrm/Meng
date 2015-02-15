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
    internal sealed class ValueClass : Class<QS.Fx.Reflection.IValueClass, ValueClass>, QS.Fx.Reflection.IValueClass
    {
        #region Constructor

        internal ValueClass(
            QS.Fx.Reflection.IValueClass _original_base_template_valueclass,
            Library.Namespace_ _namespace, QS.Fx.Base.ID _id, ulong _incarnation, string _name, string _comment, Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, 
            IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters)
            : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {
            if (_original_base_template_valueclass == null)
                _original_base_template_valueclass = this;
            this._original_base_template_valueclass = _original_base_template_valueclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IValueClass _original_base_template_valueclass;

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Reflection_.Internal_._internal_info_valueclass _internal_info;

        #endregion

        #region internal_info_

        public QS._qss_x_.Reflection_.Internal_._internal_info_valueclass internal_info_
        {
            get { return this._internal_info; }
            set { this._internal_info = value; }
        }

        #endregion

        #region _Original_Base_Template_ValueClass

        internal QS.Fx.Reflection.IValueClass _Original_Base_Template_ValueClass
        {
            get { return this._original_base_template_valueclass; }
        }

        #endregion

        #region _Instantiate

        protected override ValueClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            if ((this._type == null) && (this._id == null))
            {
                foreach (QS.Fx.Reflection.IParameter _parameter in _parameters)
                {
                    if (_parameter.ID.Equals(this._name))
                        return (QS._qss_x_.Reflection_.ValueClass) _parameter.Value;
                }
                return this;
            }
            else
            {
                IDictionary<string, QS.Fx.Reflection.IParameter> _new_classparameters, _new_openparameters;
                Library._InstantiateParameters(_parameters, this._classparameters, this._openparameters, out _new_classparameters, out _new_openparameters);
                return new ValueClass(this._original_base_template_valueclass, this._namespace,
                    this._id, this._incarnation, this._name, this._comment, this._type, _new_classparameters, _new_openparameters);
            }
        }

        #endregion

        #region IValueClass Members

        // private static readonly Type _objectreftypebase1 = typeof(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();        
        // private static readonly Type _objectreftypebase2 = typeof(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();

        object QS.Fx.Reflection.IValueClass.Create(object _object)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Reflection.Xml.ValueClass QS.Fx.Reflection.IValueClass.Serialize
        {
            get
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append(this._namespace.uuid_);
                _ss.Append(":");
                _ss.Append(this._uuid);
                return new QS.Fx.Reflection.Xml.ValueClass(
                    _ss.ToString(), QS.Fx.Reflection.Parameter.Serialize(this._classparameters.Values));
            }
        }

        bool QS.Fx.Reflection.IValueClass.IsSubtypeOf(QS.Fx.Reflection.IValueClass other)
        {
            if (!ReferenceEquals(this, other) && !other.UnderlyingType.IsAssignableFrom(((QS.Fx.Reflection.IValueClass)this).UnderlyingType))
                return QS._qss_x_.Reflection_.Internal_._internal._can_convert_value(((QS.Fx.Reflection.IValueClass) this).UnderlyingType, other.UnderlyingType);
            else
                return true;
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

        #region _IsInternal

        public bool _IsInternal
        {
            get
            {
                if ((this._original_base_template_valueclass != null) && !ReferenceEquals(this, this._original_base_template_valueclass))
                    return ((ValueClass)this._original_base_template_valueclass)._IsInternal;
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
