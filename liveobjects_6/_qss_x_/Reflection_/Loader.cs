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
using System.IO;
using System.Xml.Serialization;

namespace QS._qss_x_.Reflection_
{
    public class Loader : QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>, QS._qss_x_.Interface_.Classes_.IDeserializer
    {
        #region Constructor

        public Loader(QS.Fx.Object.IContext _mycontext, QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary> _libraryservice)
        {
            this._mycontext = _mycontext;
            this._connection = QS._qss_x_.Component_.Classes_.Service<QS.Fx.Interface.Classes.ILibrary>.Connect(_mycontext, _libraryservice, out this._library);
        }

        public Loader(QS.Fx.Object.IContext _mycontext, QS.Fx.Interface.Classes.ILibrary _library)
        {
            this._mycontext = _mycontext;
            this._library = _library;
            this._connection = null;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Endpoint.IConnection _connection;
        private QS.Fx.Interface.Classes.ILibrary _library;

        #endregion

        #region ILoader Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>.Load(string _xmlspecification)
        {
            QS.Fx.Reflection.Xml.Object _xmlobject;
            using (StringReader _stringreader = new StringReader(_xmlspecification))
            {
                _xmlobject = ((QS.Fx.Reflection.Xml.Root)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Deserialize(_stringreader)).Object;
            }
            return _LoadObject(_xmlobject);
        }

        #endregion

        #region IDeserializer Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS._qss_x_.Interface_.Classes_.IDeserializer.DeserializeObject(QS.Fx.Reflection.Xml.Object _xmlobject)
        {
            return this._LoadObject(_xmlobject);
        }

        QS.Fx.Reflection.IValueClass QS._qss_x_.Interface_.Classes_.IDeserializer.DeserializeValueClass(QS.Fx.Reflection.Xml.ValueClass _xmlvalueclass)
        {
            return this._LoadValueClass(_xmlvalueclass);
        }

        QS.Fx.Reflection.IInterfaceClass QS._qss_x_.Interface_.Classes_.IDeserializer.DeserializeInterfaceClass(QS.Fx.Reflection.Xml.InterfaceClass _xmlinterfaceclass)
        {
            return this._LoadInterfaceClass(_xmlinterfaceclass);
        }

        QS.Fx.Reflection.IEndpointClass QS._qss_x_.Interface_.Classes_.IDeserializer.DeserializeEndpointClass(QS.Fx.Reflection.Xml.EndpointClass _xmlendpointeclass)
        {
            return this._LoadEndpointClass(_xmlendpointeclass);
        }

        QS.Fx.Reflection.IObjectClass QS._qss_x_.Interface_.Classes_.IDeserializer.DeserializeObjectClass(QS.Fx.Reflection.Xml.ObjectClass _xmlobjectclass)
        {
            return this._LoadObjectClass(_xmlobjectclass);
        }

        #endregion

        #region _LoadObject

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _LoadObject(QS.Fx.Reflection.Xml.Object _xmlobject)
        {
            if (_xmlobject != null)
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _returnedobject;
                if (_xmlobject is QS.Fx.Reflection.Xml.ReferenceObject)
                    _returnedobject = _LoadReferenceObject((QS.Fx.Reflection.Xml.ReferenceObject)_xmlobject);
                else if (_xmlobject is QS.Fx.Reflection.Xml.CompositeObject)
                    _returnedobject = _LoadCompositeObject((QS.Fx.Reflection.Xml.CompositeObject)_xmlobject);
                else
                    throw new NotImplementedException();
                if (_returnedobject != null)
                {
                    if (_xmlobject.Authority != null)
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>> _authority =
                            _LoadObject(_xmlobject.Authority).CastTo<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>>();
                        _returnedobject = QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(_returnedobject, _authority);
                    }
                    else
                    {
                        if (_returnedobject.ObjectClass.AuthenticatingClass != null)
                            throw new UnauthorizedAccessException();
                    }
                }
                return _returnedobject;
            }
            else
                return null;
        }

        #endregion

        #region _LoadCompositeObject

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _LoadCompositeObject(QS.Fx.Reflection.Xml.CompositeObject _xmlobject)
        {
            if (_xmlobject.Components != null)
            {
                IDictionary<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _components =
                    new Dictionary<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(_xmlobject.Components.Length);
                foreach (QS.Fx.Reflection.Xml.CompositeObject.Component _c in _xmlobject.Components)
                {
                    string _id = _c.ID;
                    for (int k = 0; k < _id.Length; k++)
                        if (!(char.IsLetterOrDigit(_id[k]) || (_id[k] == '_')))
                            throw new Exception("Component id \"" + _id + "\" contains an illegal character \'" + _id[k].ToString() + "\' at position " + k.ToString() + ".");
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _obj = _LoadObject(_c.Object);
                    _components.Add(_id, _obj);
                }
                List<QS.Fx.Attributes.IAttribute> _attributes = new List<QS.Fx.Attributes.IAttribute>();
                if (_xmlobject.Attributes != null)
                {
                    foreach (QS.Fx.Reflection.Xml.Attribute _attribute in _xmlobject.Attributes)
                        _attributes.Add(
                            new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.GetAttributeClass(
                                new QS.Fx.Base.ID(_attribute.ID)), _attribute.Value));
                }
                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                (
                    _xmlobject.ID,
                    new QS.Fx.Attributes.Attributes(_attributes.ToArray()),
                    _LoadObjectClass(_xmlobject.ObjectClass),
                    _components
                );
            }
            else
                return null;
        }

        #endregion

        #region _LoadReferenceObject

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _LoadReferenceObject(QS.Fx.Reflection.Xml.ReferenceObject _xmlobject)
        {
            if (_xmlobject.ID != null)
            {
                if (_xmlobject.From != null)
                    return _LoadReferenceObject_FromFolder(_xmlobject);
                else
                    return _LoadReferenceObject_FromLibrary(_xmlobject);
            }
            else
                return null;
        }

        #endregion

        #region _LoadReferenceObject_FromFolder

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _LoadReferenceObject_FromFolder(QS.Fx.Reflection.Xml.ReferenceObject _xmlobject)
        {
            if (_xmlobject.Parameters != null)
                throw new Exception("References to parameterized objects in folders are not supported at this time.");

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _folder_ref_1 = _LoadObject(_xmlobject.From);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder_ref_2 =
                _folder_ref_1.CastTo<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>();

            QS.Fx.Reflection.IObjectClass _objectclass = 
                (_xmlobject.ObjectClass != null) ? _LoadObjectClass(_xmlobject.ObjectClass) : null;

            List<QS.Fx.Attributes.IAttribute> _attributes = new List<QS.Fx.Attributes.IAttribute>();
            if (_xmlobject.Attributes != null)
            {
                foreach (QS.Fx.Reflection.Xml.Attribute _attribute in _xmlobject.Attributes)
                    _attributes.Add(
                        new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.GetAttributeClass(
                            new QS.Fx.Base.ID(_attribute.ID)), _attribute.Value));
            }
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _returnedobject = 
                QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(
                    _folder_ref_2, _xmlobject.ID, _objectclass, new QS.Fx.Attributes.Attributes(_attributes.ToArray()));
            return _returnedobject;
        }

        #endregion

        #region _LoadParameters_1

        private IEnumerable<QS.Fx.Reflection.IParameter> _LoadParameters_1(
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IEnumerable<QS.Fx.Reflection.Xml.Parameter> _xmlparameters)
        {
            List<QS.Fx.Reflection.IParameter> _parameters = new List<QS.Fx.Reflection.IParameter>();
            if (_xmlparameters != null)
            {
                foreach (QS.Fx.Reflection.Xml.Parameter _xmlparameter in _xmlparameters)
                {
                    QS.Fx.Reflection.IParameter _classparameter;
                    if (!_classparameters.TryGetValue(_xmlparameter.ID, out _classparameter))
                        throw new Exception("The parameter \"" + _xmlparameter.ID +
                            "\" in the XML code is not one of the template parameters.");
                    if (_classparameter.ParameterClass != QS.Fx.Reflection.ParameterClass.Value)
                    {
                        object _value;
                        switch (_classparameter.ParameterClass)
                        {
                            case QS.Fx.Reflection.ParameterClass.ValueClass:
                                {
                                    if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ValueClass)
                                        _value = _LoadValueClass((QS.Fx.Reflection.Xml.ValueClass)_xmlparameter.Value);
                                    else
                                        throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting a value class.");
                                }
                                break;

                            case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                                {
                                    if (_xmlparameter.Value is QS.Fx.Reflection.Xml.InterfaceClass)
                                        _value = _LoadInterfaceClass((QS.Fx.Reflection.Xml.InterfaceClass)_xmlparameter.Value);
                                    else
                                        throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting an interface class.");
                                }
                                break;

                            case QS.Fx.Reflection.ParameterClass.EndpointClass:
                                {
                                    if (_xmlparameter.Value is QS.Fx.Reflection.Xml.EndpointClass)
                                        _value = _LoadEndpointClass((QS.Fx.Reflection.Xml.EndpointClass)_xmlparameter.Value);
                                    else
                                        throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting an endpoint class.");
                                }
                                break;

                            case QS.Fx.Reflection.ParameterClass.ObjectClass:
                                {
                                    if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ObjectClass)
                                        _value = _LoadObjectClass((QS.Fx.Reflection.Xml.ObjectClass)_xmlparameter.Value);
                                    else
                                        throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting an object class.");
                                }
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                        QS.Fx.Reflection.IParameter _parameter = new QS.Fx.Reflection.Parameter(
                            _classparameter.ID, _classparameter.Attributes, _classparameter.ParameterClass, _classparameter.ValueClass, null, _value);
                        _parameters.Add(_parameter);
                    }
                }
            }
            return (_parameters.Count > 0) ? _parameters : null;
        }

        #endregion

        #region _LoadParameters_2

        private IEnumerable<QS.Fx.Reflection.IParameter> _LoadParameters_2(
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IEnumerable<QS.Fx.Reflection.Xml.Parameter> _xmlparameters)
        {
            List<QS.Fx.Reflection.IParameter> _parameters = new List<QS.Fx.Reflection.IParameter>();
            if (_xmlparameters != null)
            {
                foreach (QS.Fx.Reflection.Xml.Parameter _xmlparameter in _xmlparameters)
                {
                    QS.Fx.Reflection.IParameter _classparameter = _classparameters[_xmlparameter.ID];
                    if (_classparameter.ParameterClass == QS.Fx.Reflection.ParameterClass.Value)
                    {
                        object _value;
                        Type _parametertype = _classparameter.ValueClass.UnderlyingType;
                        if (_parametertype.IsGenericType && _parametertype.GetGenericTypeDefinition().Equals(_referenceobjectbasetype))
                        {
                            Type[] _parametertypeargs = _parametertype.GetGenericArguments();
                            if (_parametertypeargs.Length != 1)
                                throw new Exception("Internal error: the type \"" + _parametertype.ToString() + "\" was expected to have a single argument.");
                            Type _expectedobjecttype = _parametertypeargs[0];
                            Type _expectedobjectreferencetype = _referenceobjectbasetype_class.MakeGenericType(new Type[] { _expectedobjecttype });
                            System.Reflection.MethodInfo _constructorinfo =
                                _expectedobjectreferencetype.GetMethod("Create", new Type[] { typeof(QS.Fx.Reflection.IObject) });
                            QS.Fx.Reflection.IObject _constructorobj;
                            if (_xmlparameter.Value == null)
                                throw new Exception("Encountered a null value in the document where an object reference was expected.");
                            else if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ReferenceObject)
                                _constructorobj = _LoadObject((QS.Fx.Reflection.Xml.ReferenceObject) _xmlparameter.Value);
                            else if (_xmlparameter.Value is QS.Fx.Reflection.Xml.CompositeObject)
                                _constructorobj = _LoadObject((QS.Fx.Reflection.Xml.ReferenceObject) _xmlparameter.Value);
                            else
                                throw new Exception("Illegal parameter value: expecting a reference object, composite object, or a null object.");
                            _value = (_constructorobj != null) ? _constructorinfo.Invoke(null, new object[] { _constructorobj }) : null;
                        }
                        //else if (_parametertype.IsGenericType && _parametertype.GetGenericTypeDefinition().Equals(_referenceobjectbasetype))
                        //{
                        //}
/*
                        else if (_parametertype.Equals(typeof(IDictionary<string, object>)))
                        {
                            IDictionary<string, object> pp = new Dictionary<string, object>();
                            _value = pp;
                            if (_xmlparameter.Value != null)
                            {
                                if (_xmlparameter.Value is QS.Fx.Reflection.Xml.Parameters)
                                {
                                    QS.Fx.Reflection.Xml.Parameters pp_ = (QS.Fx.Reflection.Xml.Parameters) _xmlparameter.Value;
                                    foreach (QS.Fx.Reflection.Xml.Parameter p_ in pp_.Items)
                                    {
                                        if (p_.Value != null)
                                        {
                                        }
                                        else
                                            pp.Add(p_.ID, 
                                    }
                                }
                                else
                                    throw new Exception("Type mismatch, expecting an array of parameters.");
                            }
                        }
*/
                        else
                        {
                            if (_xmlparameter.Value != null)
                            {
                                Type _xmlparametertype = _xmlparameter.Value.GetType();
                                if (!_parametertype.IsAssignableFrom(_xmlparametertype))
                                    throw new Exception("The type \"" + _xmlparametertype.ToString() +
                                        "\" of the supplied parameter does not match the expected type \"" + _parametertype.ToString() + "\".");
                            }
                            _value = _xmlparameter.Value;
                        }
                        QS.Fx.Reflection.IParameter _parameter = new QS.Fx.Reflection.Parameter(
                            _classparameter.ID, _classparameter.Attributes, _classparameter.ParameterClass, _classparameter.ValueClass, null, _value);
                        _parameters.Add(_parameter);
                    }
                }
            }
            return (_parameters.Count > 0) ? _parameters : null;
        }

        #endregion

        #region _LoadReferenceObject_FromLibrary

        private static readonly Type _referenceobjectbasetype = typeof(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();
        private static readonly Type _referenceobjectbasetype_class = typeof(QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _LoadReferenceObject_FromLibrary(QS.Fx.Reflection.Xml.ReferenceObject _xmlobject)
        {
            try
            {
                QS.Fx.Reflection.IComponentClass _componentclass = _library.GetComponentClass(_xmlobject.ID);
                IEnumerable<QS.Fx.Reflection.IParameter> _parameters;
                _parameters = _LoadParameters_1(
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).ClassParameters, _xmlobject.Parameters);
                if (_parameters != null)
                    _componentclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).Instantiate(_parameters);
                _parameters = _LoadParameters_2(
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).ClassParameters, _xmlobject.Parameters);
                if (_parameters != null)
                    _componentclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).Instantiate(_parameters);
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _o = (QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>)_componentclass;
                if (_xmlobject.ObjectClass != null)
                {
                    QS.Fx.Reflection.IObjectClass _objectclass = _LoadObjectClass(_xmlobject.ObjectClass);
                    _o = QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(_o, _objectclass);
                }
                if (_xmlobject.Attributes != null)
                {
                    IDictionary<QS.Fx.Base.ID, QS.Fx.Attributes.IAttribute> _attributes = new Dictionary<QS.Fx.Base.ID, QS.Fx.Attributes.IAttribute>();
                    foreach (QS.Fx.Reflection.Xml.Attribute _attribute in _xmlobject.Attributes)
                    {
                        QS.Fx.Base.ID _classid = new QS.Fx.Base.ID(_attribute.ID);
                        _attributes.Add(_classid, new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.GetAttributeClass(_classid), _attribute.Value));
                    }
                    if (_o.Attributes != null)
                    {
                        foreach (QS.Fx.Attributes.IAttribute _attribute in _o.Attributes)
                        {
                            if (!_attributes.ContainsKey(_attribute.Class.ID))
                                _attributes.Add(_attribute.Class.ID, _attribute);
                        }
                    }
                    QS.Fx.Attributes.IAttributes _attributes_2 = new QS.Fx.Attributes.Attributes((new List<QS.Fx.Attributes.IAttribute>(_attributes.Values)).ToArray());
                    _o = QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(_o, _attributes_2);
                }
                return  _o;
            }
            catch (Exception _exc)
            {
                throw new Exception("Cannot load reference object \"" + _xmlobject.ID + "\" from the library.", _exc);
            }
        }

        #endregion

        #region _LoadValueClass

        private QS.Fx.Reflection.IValueClass _LoadValueClass(QS.Fx.Reflection.Xml.ValueClass _xmlvalueclass)
        {
            if (_xmlvalueclass.ID != null)
            {
                QS.Fx.Reflection.IValueClass _valueclass = _library.GetValueClass(_xmlvalueclass.ID);
                IEnumerable<QS.Fx.Reflection.IParameter> _parameters;
                _parameters = _LoadParameters_1(
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IValueClass>)_valueclass).ClassParameters, _xmlvalueclass.Parameters);
                if (_parameters != null)
                    _valueclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IValueClass>)_valueclass).Instantiate(_parameters);
                return _valueclass;
            }
            else
                throw new NotImplementedException();
        }

        #endregion

        #region _LoadInterfaceClass

        private QS.Fx.Reflection.IInterfaceClass _LoadInterfaceClass(QS.Fx.Reflection.Xml.InterfaceClass _xmlinterfaceclass)
        {
            if (_xmlinterfaceclass.ID != null)
            {
                QS.Fx.Reflection.IInterfaceClass _interfaceclass = _library.GetInterfaceClass(_xmlinterfaceclass.ID);
                IEnumerable<QS.Fx.Reflection.IParameter> _parameters;
                _parameters = _LoadParameters_1(
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IInterfaceClass>)_interfaceclass).ClassParameters, _xmlinterfaceclass.Parameters);
                if (_parameters != null)
                    _interfaceclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IInterfaceClass>)_interfaceclass).Instantiate(_parameters);
                return _interfaceclass;
            }
            else
                throw new NotImplementedException();
        }

        #endregion

        #region _LoadEndpointClass

        private QS.Fx.Reflection.IEndpointClass _LoadEndpointClass(QS.Fx.Reflection.Xml.EndpointClass _xmlendpointclass)
        {
            if (_xmlendpointclass.ID != null)
            {
                QS.Fx.Reflection.IEndpointClass _endpointclass = _library.GetEndpointClass(_xmlendpointclass.ID);
                IEnumerable<QS.Fx.Reflection.IParameter> _parameters;
                _parameters = _LoadParameters_1(
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IEndpointClass>)_endpointclass).ClassParameters, _xmlendpointclass.Parameters);
                if (_parameters != null)
                    _endpointclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IEndpointClass>)_endpointclass).Instantiate(_parameters);
                return _endpointclass;
            }
            else
                throw new NotImplementedException();
        }

        #endregion

        #region _LoadObjectClass

        private QS.Fx.Reflection.IObjectClass _LoadObjectClass(QS.Fx.Reflection.Xml.ObjectClass _xmlobjectclass)
        {
            if (_xmlobjectclass.ID != null)
            {
                QS.Fx.Reflection.IObjectClass _objectclass = _library.GetObjectClass(_xmlobjectclass.ID);
                IEnumerable<QS.Fx.Reflection.IParameter> _parameters;
                _parameters = _LoadParameters_1(
                    ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObjectClass>) _objectclass).ClassParameters, _xmlobjectclass.Parameters);
                if (_parameters != null)
                    _objectclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObjectClass>) _objectclass).Instantiate(_parameters);
                if ((_xmlobjectclass.EndpointConstraints != null) && (_xmlobjectclass.EndpointConstraints.Length > 0))
                {
                    _objectclass = ((QS._qss_x_.Reflection_.ObjectClass) _objectclass)._DeserializeConstraints(_xmlobjectclass.EndpointConstraints);
                }
                if ((_xmlobjectclass.ObjectConstraints != null) && (_xmlobjectclass.ObjectConstraints.Length > 0))
                {
                    QS._qss_x_.Reflection_.ObjectClass _new_objectclass = ((QS._qss_x_.Reflection_.ObjectClass)_objectclass)._Clone();
                    foreach (QS.Fx.Reflection.Xml.ObjectConstraint _xmlconstraint in _xmlobjectclass.ObjectConstraints)
                    {
                        QS.Fx.Reflection.IObjectConstraintClass _constraintclass = this._library.GetObjectConstraintClass(_xmlconstraint.Class);
                        QS.Fx.Reflection.IObjectConstraint _constraint = _constraintclass.CreateConstraint();
                        _constraint.Initialize(_xmlconstraint.Value, _objectclass);
                        QS._qss_x_.Reflection_.ObjectConstraint _objectconstraint =
                            new QS._qss_x_.Reflection_.ObjectConstraint(
                                (QS._qss_x_.Reflection_.ObjectConstraintClass)_constraintclass, _constraint);
                        string _kind = _xmlconstraint.Kind.ToLower();
                        if (_kind.Equals("provided"))
                            _new_objectclass._AddConstraint(QS.Fx.Reflection.ConstraintKind.Provided, _xmlconstraint.Class, _objectconstraint);
                        else if (_kind.Equals("required"))
                            _new_objectclass._AddConstraint(QS.Fx.Reflection.ConstraintKind.Required, _xmlconstraint.Class, _objectconstraint);
                        else
                            throw new NotImplementedException();
                    }
                    _objectclass = _new_objectclass;
                }
                return _objectclass;
            }
            else
                throw new NotImplementedException();
        }

        #endregion
    }
}
