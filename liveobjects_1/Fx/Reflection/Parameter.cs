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

namespace QS.Fx.Reflection
{
    [QS.Fx.Printing.Printable("Parameter", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Parameter : QS.Fx.Inspection.Inspectable, QS.Fx.Reflection.IParameter
    {
        #region Constructors

        public Parameter(string _id, QS.Fx.Attributes.IAttributes _attributes,
            QS.Fx.Reflection.ParameterClass _parameterclass, QS.Fx.Reflection.IValueClass _valueclass, object _defaultvalue, object _value)
        {
            this._id = _id;
            this._attributes = _attributes;
            this._parameterclass = _parameterclass;
            this._valueclass = _valueclass;
            this._defaultvalue = _defaultvalue;
            this._value = _value;
        }

        public static Parameter ValueClass(string _id, QS.Fx.Reflection.IValueClass _valueclass)
        {
            return new Parameter(
                _id, null, ParameterClass.ValueClass, null, null, _valueclass);
        }

        //public static Parameter ValueClass<C>(string _id) where C : class
        //{
        //    return new Parameter(
        //        _id, null, ParameterClass.ValueClass, null, null, QS.Fx.Reflection.Library.LocalLibrary.ValueClass<C>());
        //}

        public static Parameter InterfaceClass(string _id, QS.Fx.Reflection.IInterfaceClass _interfaceclass)
        {
            return new Parameter(
                _id, null, ParameterClass.InterfaceClass, null, null, _interfaceclass);
        }

        //public static Parameter InterfaceClass<C>(string _id) where C : class, QS.Fx.Interface.Classes.IInterface
        //{
        //    return new Parameter(
        //        _id, null, ParameterClass.InterfaceClass, null, null, QS.Fx.Reflection.Library.LocalLibrary.InterfaceClass<C>());
        //}

        public static Parameter ObjectClass(string _id, QS.Fx.Reflection.IObjectClass _objectclass)
        {
            return new Parameter(
                _id, null, ParameterClass.ObjectClass, null, null, _objectclass);
        }

        //public static Parameter ObjectClass<C>(string _id) where C : class, QS.Fx.Object.Classes.IObject
        //{
        //    return new Parameter(
        //        _id, null, ParameterClass.ObjectClass, null, null, QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<C>());
        //}

        public static Parameter Value<C>(string _id, object _value)
        {
            return new Parameter(
                _id, null, ParameterClass.Value, QS.Fx.Reflection.Library.LocalLibrary.ValueClass<C>(), null, _value);
        }

        public static Parameter Object(string _id, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            return new Parameter(
                _id, null, ParameterClass.Value, QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(), null, _object);
        }

        public static Parameter Object<C>(string _id, QS.Fx.Object.IReference<C> _object) where C : class, QS.Fx.Object.Classes.IObject
        {
            return new Parameter(
                _id, null, ParameterClass.Value, QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Object.IReference<C>>(), null, _object);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("id")]
        private string _id;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("attributes")]
        private QS.Fx.Attributes.IAttributes _attributes;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("parameterclass")]
        private QS.Fx.Reflection.ParameterClass _parameterclass;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("valueclass")]
        private QS.Fx.Reflection.IValueClass _valueclass;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("defaultvalue")]
        private object _defaultvalue;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("value")]
        private object _value;

        #endregion

        #region IParameter Members

        string QS.Fx.Reflection.IParameter.ID
        {
            get { return _id; }
        }

        QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IParameter.Attributes
        {
            get { return (this._attributes != null) ? this._attributes : (new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[0])); }
        }

        QS.Fx.Reflection.ParameterClass QS.Fx.Reflection.IParameter.ParameterClass
        {
            get { return _parameterclass; }
        }

        QS.Fx.Reflection.IValueClass QS.Fx.Reflection.IParameter.ValueClass
        {
            get { return _valueclass; }
        }

        object QS.Fx.Reflection.IParameter.DefaultValue
        {
            get { return _defaultvalue; }
        }

        object QS.Fx.Reflection.IParameter.Value
        {
            get { return _value; }
        }

        QS.Fx.Reflection.Xml.Parameter QS.Fx.Reflection.IParameter.Serialize
        {
            get
            {
                object _value;

                switch (this._parameterclass)
                {
                    case QS.Fx.Reflection.ParameterClass.ValueClass:
                        {
                            if (this._value == null)
                                throw new Exception("Value class parameter is null.");
                            if (!(this._value is QS.Fx.Reflection.IValueClass))
                                throw new Exception("Value class parameter assigned object of a wrong type \"" + this._value.GetType().ToString() + "\".");
                            _value = ((QS.Fx.Reflection.IValueClass)this._value).Serialize;
                        }
                        break;

                    case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                        {
                            if (this._value == null)
                                throw new Exception("Interface class parameter is null.");
                            if (!(this._value is QS.Fx.Reflection.IInterfaceClass))
                                throw new Exception("Interface class parameter assigned object of a wrong type \"" + this._value.GetType().ToString() + "\".");
                            _value = ((QS.Fx.Reflection.IInterfaceClass)this._value).Serialize;
                        }
                        break;

                    case QS.Fx.Reflection.ParameterClass.EndpointClass:
                        {
                            if (this._value == null)
                                throw new Exception("Endpoint class parameter is null.");
                            if (!(this._value is QS.Fx.Reflection.IEndpointClass))
                                throw new Exception("Endpoint class parameter assigned object of a wrong type \"" + this._value.GetType().ToString() + "\".");
                            _value = ((QS.Fx.Reflection.IEndpointClass)this._value).Serialize;
                        }
                        break;

                    case QS.Fx.Reflection.ParameterClass.ObjectClass:
                        {
                            if (this._value == null)
                                throw new Exception("Object class parameter is null.");
                            if (!(this._value is QS.Fx.Reflection.IObjectClass))
                                throw new Exception("Object class parameter assigned object of a wrong type \"" + this._value.GetType().ToString() + "\".");
                            _value = ((QS.Fx.Reflection.IObjectClass)this._value).Serialize;
                        }
                        break;

                    case QS.Fx.Reflection.ParameterClass.Value:
                        {
                            if (this._valueclass == null)
                                throw new Exception("Value parameter has a null value class.");
                            if (this._valueclass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ValueClasses._Object)))
                            {
                                if (this._value != null)
                                {
                                    QS.Fx.Reflection.IObject _object = (QS.Fx.Reflection.IObject) this._value;
                                    _value = _object.Serialize;
                                }
                                else
                                {
                                    _value = new QS.Fx.Reflection.Xml.ReferenceObject(null, null, null, null, null, null);
                                }
                            }
                            else
                            {
                                _value = this._value;
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return new QS.Fx.Reflection.Xml.Parameter(this._id, _value);
            }
        }

        #endregion

        #region Serialize

        public static QS.Fx.Reflection.Xml.Parameter[] Serialize(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            List<QS.Fx.Reflection.Xml.Parameter> _xmlparameters = new List<QS.Fx.Reflection.Xml.Parameter>();
            foreach (QS.Fx.Reflection.IParameter _parameter in _parameters)
                _xmlparameters.Add(_parameter.Serialize);
            return _xmlparameters.ToArray();
        }

        #endregion
    }
}
