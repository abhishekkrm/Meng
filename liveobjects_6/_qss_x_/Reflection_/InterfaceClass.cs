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
    internal sealed class InterfaceClass : Class<QS.Fx.Reflection.IInterfaceClass, InterfaceClass>, QS.Fx.Reflection.IInterfaceClass
    {
        #region Create(QS.Fx.Reflection.Xml.InterfaceClass)

/*
        public static IInterfaceClass Create(QS.Fx.Reflection.Xml.InterfaceClass _xmlinterfaceclass)
        {
            if (_xmlinterfaceclass.ID != null)
            {
                QS.Fx.Base.ID _id = new QS.Fx.Base.ID(_xmlinterfaceclass.ID);
                IInterfaceClass _interfaceclass;
                if (!InterfaceClasses.GetClass(_id, out _interfaceclass))
                    throw new Exception("Coult not find interface class with id = \"" + _id.ToString() + "\".");
                List<IParameter> _parameters = new List<IParameter>();
                if (_xmlinterfaceclass.Parameters != null && _xmlinterfaceclass.Parameters.Length > 0)
                {
                    foreach (QS.Fx.Reflection.Xml.Parameter _parameter in _xmlinterfaceclass.Parameters)
                        _parameters.Add(new Parameter(_parameter.ID, _parameter.Value));
                }
                return _interfaceclass.Instantiate(_parameters);
            }
            else
                throw new NotImplementedException();
        }
*/

        #endregion

        #region Constructor(QS.Fx.Base.ID,string,string,Type,IDictionary<string,IParameter>)

        internal InterfaceClass(
            QS.Fx.Reflection.IInterfaceClass _original_base_template_interfaceclass, 
            Library.Namespace_ _namespace, QS.Fx.Base.ID _id, ulong _incarnation, string _name, string _comment, Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters,
            IDictionary<string, QS.Fx.Reflection.IOperation> _operations)
            : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {
            this._operations = _operations;
            if (_original_base_template_interfaceclass == null)
                _original_base_template_interfaceclass = this;
            this._original_base_template_interfaceclass = _original_base_template_interfaceclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IInterfaceClass _original_base_template_interfaceclass;

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Printing.Printable("operations")]
#endif
        private IDictionary<string, QS.Fx.Reflection.IOperation> _operations;

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Reflection_.Internal_._internal_info_interfaceclass _internal_info;

        #endregion

        public QS._qss_x_.Reflection_.Internal_._internal_info_interfaceclass internal_info_
        {
            get { return this._internal_info; }
            set { this._internal_info = value; }
        }

        #region _Original_Base_Template_InterfaceClass

        internal QS.Fx.Reflection.IInterfaceClass _Original_Base_Template_InterfaceClass
        {
            get { return this._original_base_template_interfaceclass; }
        }

        #endregion

        #region Inspection

#if DEBUG_INCLUDE_INSPECTION_CODE

        [QS.Fx.Base.Inspectable("operations")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IOperation> __inspectable_operations
        {
            get
            {
                return new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IOperation>("operations", _operations,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IOperation>.ConversionCallback(
                        delegate(string s) { return s; }));
            }
        }

#endif

        #endregion

        #region IInterfaceClass Members

        IDictionary<string, QS.Fx.Reflection.IOperation> QS.Fx.Reflection.IInterfaceClass.Operations
        {
            get { return new QS._qss_x_.Base1_.ReadonlyDictionaryOf<string, QS.Fx.Reflection.IOperation>(_operations); }
        }

        QS.Fx.Reflection.Xml.InterfaceClass QS.Fx.Reflection.IInterfaceClass.Serialize
        {
            get
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append(this._namespace.uuid_);
                _ss.Append(":");
                _ss.Append(this._uuid);
                return new QS.Fx.Reflection.Xml.InterfaceClass(_ss.ToString(), QS.Fx.Reflection.Parameter.Serialize(this._classparameters.Values));
            }
        }

        bool QS.Fx.Reflection.IInterfaceClass.IsSubtypeOf(QS.Fx.Reflection.IInterfaceClass other)
        {
            if (ReferenceEquals(this, other))
                return true;
            foreach (KeyValuePair<string, QS.Fx.Reflection.IOperation> element in other.Operations)
            {
                QS.Fx.Reflection.IOperation myoperation, otheroperation;
                if (!this._operations.TryGetValue(element.Key, out myoperation))
                    return false;
                otheroperation = element.Value;
                if (!myoperation.OperationClass.IsSubtypeOf(otheroperation.OperationClass))
                    return false;
            }
            return true;
        }

        #endregion

        #region _Instantiate

        protected override InterfaceClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            IDictionary<string, QS.Fx.Reflection.IParameter> _new_classparameters, _new_openparameters;
            Library._InstantiateParameters(_parameters, this._classparameters, this._openparameters, out _new_classparameters, out _new_openparameters);
            IDictionary<string, QS.Fx.Reflection.IOperation> _new_operations = new Dictionary<string, QS.Fx.Reflection.IOperation>();
            foreach (QS.Fx.Reflection.IOperation _operation in this._operations.Values)
                _new_operations.Add(_operation.ID, _operation.Instantiate(_parameters));
            return new InterfaceClass(this._original_base_template_interfaceclass,
                this._namespace, this._id, this._incarnation, this._name, this._comment, this._type, _new_classparameters, _new_openparameters, _new_operations);
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
                if ((this._original_base_template_interfaceclass != null) && !ReferenceEquals(this, this._original_base_template_interfaceclass))
                    return ((InterfaceClass)this._original_base_template_interfaceclass)._IsInternal;
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
