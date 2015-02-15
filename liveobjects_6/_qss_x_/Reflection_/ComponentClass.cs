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
    internal sealed class ComponentClass : Class<QS.Fx.Reflection.IComponentClass, ComponentClass>, QS.Fx.Reflection.IComponentClass
    {
        #region Constructor(QS.Fx.Base.ID,string,string,Type,IDictionary<string,IParameter>,IObjectClass,System.Reflection.ConstructorInfo)

        internal ComponentClass(
            QS.Fx.Reflection.IComponentClass _original_base_template_componentclass,
            Library.Namespace_ _namespace, QS.Fx.Base.ID _id, ulong _incarnation, string _name, string _comment, Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters,
            QS.Fx.Reflection.IObjectClass _objectclass, System.Reflection.ConstructorInfo _constructor,
            QS.Fx.Base.SynchronizationOption _synchronizationoption)
            : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {
            this._synchronizationoption = _synchronizationoption;
            this._objectclass = _objectclass;
            this._constructor = _constructor;
            if (_original_base_template_componentclass == null)
                _original_base_template_componentclass = this;
            this._original_base_template_componentclass = _original_base_template_componentclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IComponentClass _original_base_template_componentclass;

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("objectclass")]
#endif
        private QS.Fx.Reflection.IObjectClass _objectclass;

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("constructor")]
#endif
        private System.Reflection.ConstructorInfo _constructor;
        private QS.Fx.Base.SynchronizationOption _synchronizationoption;

        #endregion

        #region _Original_Base_Template_ComponentClass

        internal QS.Fx.Reflection.IComponentClass _Original_Base_Template_ComponentClass
        {
            get { return this._original_base_template_componentclass; }
        }

        #endregion

        #region IClass<IObject> Members

        QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
        {
            get { return this._id; }
        }

        ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
        {
            get { return this._incarnation; }
        }

        QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
        {
            get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)this).Attributes; }
        }

        IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
        {
            get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)this).ClassParameters; }
        }

        IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
        {
            get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)this).OpenParameters; }
        }

        QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)this).Instantiate(_parameters);
        }

        Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
        {
            get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)this).UnderlyingType; }
        }

        #endregion

        #region IObject Members

        string QS.Fx.Reflection.IObject.ID
        {
            get { return this._id.ToString(); }
        }

        QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
        {
            get { return null; }
        }

        QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
        {
            get { return this._objectclass; }
        }

        QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
        {
            return ((QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>) this).Dereference(_mycontext);
        }

        QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
        {
            get
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append(this._namespace.uuid_);
                _ss.Append(":");
                _ss.Append(this._uuid);
                return new QS.Fx.Reflection.Xml.ReferenceObject(_ss.ToString(), 
                    QS.Fx.Attributes.Attributes.Serialize(new QS.Fx.Attributes.Attributes(this)),
                    this._objectclass.Serialize, null, QS.Fx.Reflection.Parameter.Serialize(this._classparameters.Values), null);
            }
        }

        #endregion

        #region IReference<IObject> Members

        QS.Fx.Object.Classes.IObject QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>.Dereference(QS.Fx.Object.IContext _mycontext)
        {
            Type _underlyingtype = Library._InstantiateParameters(this._type, this._classparameters);
            System.Reflection.ConstructorInfo _newconstructor = null;
            foreach (System.Reflection.ConstructorInfo _someconstructor in _underlyingtype.GetConstructors())
            {
                if (_someconstructor.GetParameters().Length > 0)
                {
                    _newconstructor = _someconstructor;
                    break;
                }
            }
            System.Reflection.ParameterInfo[] _constructorparameters = _newconstructor.GetParameters();
            if (!_constructorparameters[0].ParameterType.Equals(typeof(QS.Fx.Object.IContext)))
                throw new Exception("Constructor is missing the context parameter.");
            object[] _args = new object[_constructorparameters.Length];
            QS._qss_x_.Object_.Context_ _newobjectcontext = new QS._qss_x_.Object_.Context_((QS._qss_x_.Object_.Context_) _mycontext, this._synchronizationoption);
            _args[0] = _newobjectcontext;
            for (int _k = 1; _k < _constructorparameters.Length; _k++)
            {
                System.Reflection.ParameterInfo _constructorparameter = _constructorparameters[_k];
                object[] _parameterattributes = _constructorparameter.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), true);
                if (_parameterattributes == null || _parameterattributes.Length != 1)
                    throw new Exception("Cannot create element because the constructor parameters have not been properly annotated with the \"Parameter\" attribute.");
                QS.Fx.Reflection.ParameterAttribute _parameterattribute = (QS.Fx.Reflection.ParameterAttribute)_parameterattributes[0];
                QS.Fx.Reflection.IParameter _parameter = this._classparameters[_parameterattribute.ID];
                _args[_k] = _parameter.Value;
            }
            ((QS._qss_x_.Object_.IInternal_)_newobjectcontext)._RemoteContext = ((QS._qss_x_.Object_.IInternal_)_mycontext)._RemoteContext;
            ((QS._qss_x_.Object_.IInternal_)_newobjectcontext)._Class = this._namespace.uuid_ + ":" + this.uuid_;
            QS.Fx.Object.Classes.IObject _newobject = (QS.Fx.Object.Classes.IObject)_newconstructor.Invoke(_args);
            ((QS._qss_x_.Object_.IInternal_) _newobjectcontext)._Object = _newobject;
            return _newobject;
        }

        QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>.CastTo<AnotherObjectClass>() 
        {
            return QS._qss_x_.Object_.Reference<AnotherObjectClass>.Create((QS.Fx.Reflection.IObject) this);
        }

        #endregion

        #region _Instantiate

        protected override ComponentClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            IDictionary<string, QS.Fx.Reflection.IParameter> _new_classparameters, _new_openparameters;
            Library._InstantiateParameters(_parameters, this._classparameters, this._openparameters, out _new_classparameters, out _new_openparameters);
            return new ComponentClass(
                this._original_base_template_componentclass,
                this._namespace,
                this._id, this._incarnation, this._name, this._comment, this._type, _new_classparameters, _new_openparameters, 
                this._objectclass.Instantiate(_parameters), this._constructor,
                this._synchronizationoption);
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
                if ((this._original_base_template_componentclass != null) && !ReferenceEquals(this, this._original_base_template_componentclass))
                    return ((ComponentClass) this._original_base_template_componentclass)._IsInternal;
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
