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
    internal sealed class EndpointClass : Class<QS.Fx.Reflection.IEndpointClass, EndpointClass>, QS.Fx.Reflection.IEndpointClass
    {
        #region Create(QS.Fx.Reflection.Xml.EndpointClass)

/*
        public static IEndpointClass Create(QS.Fx.Reflection.Xml.EndpointClass _xmlendpointclass)
        {
            if (_xmlendpointclass.ID != null)
            {
                QS.Fx.Base.ID _id = new QS.Fx.Base.ID(_xmlendpointclass.ID);
                IEndpointClass _endpointclass;
                if (!EndpointClasses.GetClass(_id, out _endpointclass))
                    throw new Exception("Coult not find endpoint class with id = \"" + _id.ToString() + "\".");
                List<IParameter> _parameters = new List<IParameter>();
                if (_xmlendpointclass.Parameters != null && _xmlendpointclass.Parameters.Length > 0)
                {
                    foreach (QS.Fx.Reflection.Xml.Parameter _parameter in _xmlendpointclass.Parameters)
                        _parameters.Add(new Parameter(_parameter.ID, _parameter.Value));
                }

                return _endpointclass.Instantiate(_parameters);
            }
            else
                throw new NotImplementedException();
        }
*/

        #endregion

        #region Constructor(QS.Fx.Base.ID,string,string,Type,IDictionary<string,IParameter>)

        internal EndpointClass(
            QS.Fx.Reflection.IEndpointClass _original_base_template_endpointclass,
            Library.Namespace_ _namespace, QS.Fx.Base.ID _id, ulong _incarnation, string _name, string _comment, Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters)
            : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {
            if (_original_base_template_endpointclass == null)
                _original_base_template_endpointclass = this;
            this._original_base_template_endpointclass = _original_base_template_endpointclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IEndpointClass _original_base_template_endpointclass;

        #endregion

        #region _Original_Base_Template_EndpointClass

        internal QS.Fx.Reflection.IEndpointClass _Original_Base_Template_EndpointClass
        {
            get { return this._original_base_template_endpointclass; }
        }

        #endregion

        #region _Instantiate

        protected override EndpointClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            IDictionary<string, QS.Fx.Reflection.IParameter> _new_classparameters, _new_openparameters;
            Library._InstantiateParameters(_parameters, this._classparameters, this._openparameters, out _new_classparameters, out _new_openparameters);
            return new EndpointClass(this._original_base_template_endpointclass,
                this._namespace, this._id, this._incarnation, this._name, this._comment, this._type, _new_classparameters, _new_openparameters);
        }

        #endregion

        #region IEndpointClass Members

        QS.Fx.Reflection.Xml.EndpointClass QS.Fx.Reflection.IEndpointClass.Serialize
        {
            get
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append(this._namespace.uuid_);
                _ss.Append(":");
                _ss.Append(this._uuid);
                return 
                    new QS.Fx.Reflection.Xml.EndpointClass(
                        _ss.ToString(),
                        QS.Fx.Reflection.Parameter.Serialize(this._classparameters.Values));
            }
        }

        private static Type _type_exportedinterface = 
            typeof(QS.Fx.Endpoint.Classes.IExportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();

        private static Type _type_importedinterface =
            typeof(QS.Fx.Endpoint.Classes.IImportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
        
        private static Type _type_dualinterface =
            typeof(QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IInterface, QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();

        bool QS.Fx.Reflection.IEndpointClass.IsSubtypeOf(QS.Fx.Reflection.IEndpointClass other)
        {
            if (ReferenceEquals(this, other))
                return true;

            EndpointClass other_ = other as EndpointClass;
            if (other_ == null)
                throw new Exception("Unsupported endpoint type \"" + other.GetType() + "\".");

            if (this._type.Equals(other_._type))
            {
                if (this._type.Equals(_type_exportedinterface))
                {
                    QS.Fx.Reflection.IInterfaceClass out1 = (QS.Fx.Reflection.IInterfaceClass)this._classparameters["ExportedInterface"].Value;
                    QS.Fx.Reflection.IInterfaceClass out2 = (QS.Fx.Reflection.IInterfaceClass)other.ClassParameters["ExportedInterface"].Value;
                    return out1.IsSubtypeOf(out2);
                }
                else if (this._type.Equals(_type_importedinterface))
                {
                    QS.Fx.Reflection.IInterfaceClass in1 = (QS.Fx.Reflection.IInterfaceClass)this._classparameters["ImportedInterface"].Value;
                    QS.Fx.Reflection.IInterfaceClass in2 = (QS.Fx.Reflection.IInterfaceClass)other.ClassParameters["ImportedInterface"].Value;
                    return in2.IsSubtypeOf(in1);
                }
                else if (this._type.Equals(_type_dualinterface))
                {
                    QS.Fx.Reflection.IInterfaceClass in1 = (QS.Fx.Reflection.IInterfaceClass)this._classparameters["ImportedInterface"].Value;
                    QS.Fx.Reflection.IInterfaceClass in2 = (QS.Fx.Reflection.IInterfaceClass)other.ClassParameters["ImportedInterface"].Value;
                    QS.Fx.Reflection.IInterfaceClass out1 = (QS.Fx.Reflection.IInterfaceClass)this._classparameters["ExportedInterface"].Value;
                    QS.Fx.Reflection.IInterfaceClass out2 = (QS.Fx.Reflection.IInterfaceClass)other.ClassParameters["ExportedInterface"].Value;
                    return in2.IsSubtypeOf(in1) && out1.IsSubtypeOf(out2);
                }
/*
                else if (this._type.Equals(_type_clientof))
                {
                    QS.Fx.Reflection.IEndpointClass c = (QS.Fx.Reflection.IEndpointClass)this._classparameters["EndpointClass"].Value;
                    QS.Fx.Reflection.IEndpointClass co = (QS.Fx.Reflection.IEndpointClass)other.ClassParameters["EndpointClass"].Value;
                    EndpointClass c_ = c as EndpointClass;
                    if (c_ == null)
                        throw new Exception("Unsupported endpoint type \"" + c.GetType() + "\".");
                    EndpointClass co_ = co as EndpointClass;
                    if (co_ == null)
                        throw new Exception("Unsupported endpoint type \"" + co.GetType() + "\".");
                    return c_._type.Equals(co_._type);
                }
*/
                else
                    return true;
            }
            else
                return false;
        }

        bool QS.Fx.Reflection.IEndpointClass.CanConnectTo(QS.Fx.Reflection.IEndpointClass other)
        {
            throw new NotImplementedException();
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
                if ((this._original_base_template_endpointclass != null) && !ReferenceEquals(this, this._original_base_template_endpointclass))
                    return ((EndpointClass)this._original_base_template_endpointclass)._IsInternal;
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
