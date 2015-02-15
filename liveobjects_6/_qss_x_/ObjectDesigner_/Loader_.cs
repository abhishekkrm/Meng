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

namespace QS._qss_x_.ObjectDesigner_
{
    public sealed class Loader_
    {
        #region Constructor

        public Loader_(QS.Fx.Interface.Classes.ILibrary _library)
        {
            this._library = _library;
        }

        #endregion

        #region Fields

        private QS.Fx.Interface.Classes.ILibrary _library;

        #endregion

        #region Static Fields

        private static readonly Type _objectreferencetype = 
            typeof(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadValueClass

        #region _LoadValueClass(QS.Fx.Reflection.Xml.ValueClass _xmlvalueclass, Elements_.Element_Environment_ _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_ _LoadValueClass(
            QS.Fx.Reflection.Xml.ValueClass _xmlvalueclass, Elements_.Element_Environment_ _context)
        {
            lock (this)
            {
                if (_xmlvalueclass.ID != null)
                {
                    QS.Fx.Reflection.IValueClass _valueclass = _library.GetValueClass(_xmlvalueclass.ID);
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IValueClass>)_valueclass).ClassParameters,
                            _xmlvalueclass.Parameters, _context, true, false, false);
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_(
                        ((QS._qss_x_.Reflection_.ValueClass)_valueclass)._ID, _valueclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, false, _valueclass);
                }
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _LoadValueClass(QS.Fx.Reflection.IValueClass _valueclass, Elements_.Element_Environment_ _environment)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_ _LoadValueClass(
            QS.Fx.Reflection.IValueClass _valueclass, Elements_.Element_Environment_ _context, bool _automatic)
        {
            lock (this)
            {
                string _id = ((QS._qss_x_.Reflection_.ValueClass)_valueclass)._ID;
                if (_id != null)
                {
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IValueClass>)_valueclass).ClassParameters,
                            null, _context, true, false, _automatic);
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_(_id, _valueclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, _automatic, 
                        ((QS._qss_x_.Reflection_.ValueClass)_valueclass)._Original_Base_Template_ValueClass);
                }
                else
                {
                    QS.Fx.Attributes.IAttribute _nameattribute;
                    string _name;
                    if (_valueclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute) &&
                        ((_name = _nameattribute.Value) != null))
                    {
                        Elements_.Element_Environment_ _environment =
                            new QS._qss_x_.ObjectDesigner_.Elements_.Element_Environment_(_context, null, true, null);
                        return new QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_(_name, _valueclass.Attributes,
                             QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Parameter_,
                             null, _environment, _automatic, null);
                    }
                    else
                        throw new Exception("Cannot load value class because neither an id nor a name is defined.");
                }
            }
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadInterfaceClass

        #region _LoadInterfaceClass(QS.Fx.Reflection.Xml.InterfaceClass _xmlinterfaceclass, Elements_.Element_Environment_ _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_InterfaceClass_ _LoadInterfaceClass(
            QS.Fx.Reflection.Xml.InterfaceClass _xmlinterfaceclass, Elements_.Element_Environment_ _context)
        {
            lock (this)
            {
                if (_xmlinterfaceclass.ID != null)
                {
                    QS.Fx.Reflection.IInterfaceClass _interfaceclass = _library.GetInterfaceClass(_xmlinterfaceclass.ID);
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IInterfaceClass>)_interfaceclass).ClassParameters,
                            _xmlinterfaceclass.Parameters, _context, true, false, false);
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_InterfaceClass_(
                        ((QS._qss_x_.Reflection_.InterfaceClass)_interfaceclass)._ID, _interfaceclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, false, _interfaceclass);
                }
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _LoadInterfaceClass(QS.Fx.Reflection.IInterfaceClass _interfaceclass, Elements_.Element_Environment_ _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_InterfaceClass_ _LoadInterfaceClass(
            QS.Fx.Reflection.IInterfaceClass _interfaceclass, Elements_.Element_Environment_ _context, bool _automatic)
        {
            lock (this)
            {
                string _id = ((QS._qss_x_.Reflection_.InterfaceClass)_interfaceclass)._ID;
                if (_id != null)
                {
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IInterfaceClass>)_interfaceclass).ClassParameters,
                            null, _context, true, false, _automatic);
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_InterfaceClass_(_id, _interfaceclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, _automatic, 
                        ((QS._qss_x_.Reflection_.InterfaceClass)_interfaceclass)._Original_Base_Template_InterfaceClass);
                }
                else
                {
                    QS.Fx.Attributes.IAttribute _nameattribute;
                    string _name;
                    if (_interfaceclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute) &&
                        ((_name = _nameattribute.Value) != null))
                    {
                        Elements_.Element_Environment_ _environment =
                            new QS._qss_x_.ObjectDesigner_.Elements_.Element_Environment_(_context, null, true, null);
                        return new QS._qss_x_.ObjectDesigner_.Elements_.Element_InterfaceClass_(_name, _interfaceclass.Attributes,
                             QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Parameter_,
                             null, _environment, _automatic, null);
                    }
                    else
                        throw new Exception("Cannot load interface class because neither an id nor a name is defined.");
                }
            }
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadEndpointClass

        #region _LoadEndpointClass(QS.Fx.Reflection.Xml.EndpointClass _xmlendpointclass, Elements_.Element_Environment_ _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_EndpointClass_ _LoadEndpointClass(
            QS.Fx.Reflection.Xml.EndpointClass _xmlendpointclass, Elements_.Element_Environment_ _context)
        {
            lock (this)
            {
                if (_xmlendpointclass.ID != null)
                {
                    QS.Fx.Reflection.IEndpointClass _endpointclass = _library.GetEndpointClass(_xmlendpointclass.ID);
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IEndpointClass>)_endpointclass).ClassParameters,
                            _xmlendpointclass.Parameters, _context, true, false, false);
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_EndpointClass_(
                        ((QS._qss_x_.Reflection_.EndpointClass)_endpointclass)._ID, _endpointclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, false, _endpointclass);
                }
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _LoadEndpointClass(QS.Fx.Reflection.IEndpointClass _endpointclass, Elements_.Element_Environment_ _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_EndpointClass_ _LoadEndpointClass(
            QS.Fx.Reflection.IEndpointClass _endpointclass, Elements_.Element_Environment_ _context, bool _automatic)
        {
            lock (this)
            {
                string _id = ((QS._qss_x_.Reflection_.EndpointClass)_endpointclass)._ID;
                if (_id != null)
                {
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IEndpointClass>)_endpointclass).ClassParameters,
                            null, _context, true, false, _automatic);
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_EndpointClass_(_id, _endpointclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, _automatic,
                        ((QS._qss_x_.Reflection_.EndpointClass)_endpointclass)._Original_Base_Template_EndpointClass);
                }
                else
                {
                    QS.Fx.Attributes.IAttribute _nameattribute;
                    string _name;
                    if (_endpointclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute) &&
                        ((_name = _nameattribute.Value) != null))
                    {
                        Elements_.Element_Environment_ _environment =
                            new QS._qss_x_.ObjectDesigner_.Elements_.Element_Environment_(_context, null, true, null);
                        return new QS._qss_x_.ObjectDesigner_.Elements_.Element_EndpointClass_(_name, _endpointclass.Attributes,
                             QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Parameter_, 
                             null, _environment, _automatic, null);
                    }
                    else
                        throw new Exception("Cannot load endpoint class because neither an id nor a name is defined.");
                }
            }
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadObjectClass

        #region _LoadObjectClass(QS.Fx.Reflection.Xml.ObjectClass _xmlobjectclass, Elements_.Element_Environment_ _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_ _LoadObjectClass(
            QS.Fx.Reflection.Xml.ObjectClass _xmlobjectclass, Elements_.Element_Environment_ _context)
        {
            lock (this)
            {
                if (_xmlobjectclass.ID != null)
                {
                    QS.Fx.Reflection.IObjectClass _objectclass = _library.GetObjectClass(_xmlobjectclass.ID);
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObjectClass>)_objectclass).ClassParameters,
                            _xmlobjectclass.Parameters, _context, true, false, false);
                    IDictionary<string, Elements_.Element_EndpointClass_> _endpoints = 
                        new Dictionary<string, Elements_.Element_EndpointClass_>();
                    foreach (QS.Fx.Reflection.IEndpoint _endpoint in _objectclass.Endpoints.Values)
                        _endpoints.Add(_endpoint.ID, _LoadEndpointClass(_endpoint.EndpointClass, _environment, true));
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_(
                        ((QS._qss_x_.Reflection_.ObjectClass)_objectclass)._ID, _objectclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, false, _objectclass, _endpoints);
                }
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _LoadObjectClass(QS.Fx.Reflection.IObjectClass _objectclass, IDictionary<string, Elements_.Element_Parameter_> _context)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_ _LoadObjectClass(
            QS.Fx.Reflection.IObjectClass _objectclass, Elements_.Element_Environment_ _context, bool _automatic)
        {
            lock (this)
            {
                string _id = ((QS._qss_x_.Reflection_.ObjectClass)_objectclass)._ID;
                if (_id != null)
                {
                    Elements_.Element_Environment_ _environment =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObjectClass>)_objectclass).ClassParameters, 
                            null, _context, true, false, _automatic);
                    IDictionary<string, Elements_.Element_EndpointClass_> _endpoints =
                        new Dictionary<string, Elements_.Element_EndpointClass_>();
                    foreach (QS.Fx.Reflection.IEndpoint _endpoint in _objectclass.Endpoints.Values)
                        _endpoints.Add(_endpoint.ID, _LoadEndpointClass(_endpoint.EndpointClass, _environment, true));
                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_(_id, _objectclass.Attributes,
                         QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Predefined_,
                        null, _environment, _automatic,
                        ((QS._qss_x_.Reflection_.ObjectClass)_objectclass)._Original_Base_Template_ObjectClass, _endpoints);
                }
                else
                {
                    QS.Fx.Attributes.IAttribute _nameattribute;
                    string _name;
                    if (_objectclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute) &&
                        ((_name = _nameattribute.Value) != null))
                    {
                        Elements_.Element_Environment_ _environment =
                            new QS._qss_x_.ObjectDesigner_.Elements_.Element_Environment_(_context, null, true, null);
                        return new QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_(_name, _objectclass.Attributes,
                             QS._qss_x_.ObjectDesigner_.Elements_.Element_Class_.Category_.Parameter_, 
                             null, _environment, _automatic, null, null);
                    }
                    else
                        throw new Exception("Cannot load object class because neither an id nor a name is defined.");
                }
            }
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadObject

        #region _LoadObject(QS.Fx.Reflection.Xml.Object _xmlobject, Elements_.Element_Environment_ _environment)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _LoadObject(
            QS.Fx.Reflection.Xml.Object _xmlobject, Elements_.Element_Environment_ _context)
        {
            lock (this)
            {
                string _id = _xmlobject.ID;

                QS.Fx.Attributes.IAttributes _attributes;
                if (_xmlobject.Attributes != null)
                {
                    List<QS.Fx.Attributes.IAttribute> _a = new List<QS.Fx.Attributes.IAttribute>();
                    foreach (QS.Fx.Reflection.Xml.Attribute _attribute in _xmlobject.Attributes)
                        _a.Add(new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.GetAttributeClass(
                            new QS.Fx.Base.ID(_attribute.ID)), _attribute.Value));
                    _attributes = new QS.Fx.Attributes.Attributes(_a.ToArray());
                }
                else
                    _attributes = null;

                QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_ _objectclass =
                    (_xmlobject.ObjectClass != null) ? this._LoadObjectClass(_xmlobject.ObjectClass, _context) : null;

                if (_xmlobject is QS.Fx.Reflection.Xml.ReferenceObject)
                {
                    #region Reference Object

                    if (_id != null)
                    {
                        QS.Fx.Reflection.Xml.Object _from = ((QS.Fx.Reflection.Xml.ReferenceObject)_xmlobject).From;                        

                        if (_from != null)
                        {
                            #region Repository Object

                            QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _folder = _LoadObject(_from, _context);

                            return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                                _id,
                                _attributes,
                                QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Repository_,
                                null,
                                null,
                                null,
                                null,
                                _objectclass,
                                new Elements_.Element_From_(_folder),
                                null,
                                null,
                                null);

/*
                            QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _object =
                                new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                                    _environment,
                                    QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Repository_,
                                    _id,
                                    null, 
                                    _objectclass, 
                                     
                                    null, 
                                    null, 
                                    null, 
                                    _attributes, 
                                    null, 
                                    null);

                            _object._InitializeOrReinitializeAfterChange();

                            return _object;
*/

                            #endregion
                        }
                        else
                        {
                            if (_id.StartsWith("@"))
                            {
                                throw new NotImplementedException();

                                #region Parameter Object

/*
                                string _actual_id = _id.Substring(1);
                                Elements_.Element_Parameter_ _parameter;
                                if (!_environment.TryGetValue(_actual_id, out _parameter))
                                    throw new Exception("No parameter \"" + _actual_id + "\" has been defined in this environment.");
                                if (_parameter._Class != QS.Fx.Reflection.ParameterClass.Value)
                                    throw new Exception("Parameter \"" + _actual_id + "\" is not a value.");
                                Elements_.Element_Value_ _value = (Elements_.Element_Value_)_parameter._Value;
                                if (_value._Category != QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Object_)
                                    throw new Exception("Parameter \"" + _actual_id + "\" is not an object reference.");
                                if (_value._Object != null)
                                {
                                    Elements_.Element_Object_ _referenced_object = (Elements_.Element_Object_)_value._Object;
                                    _objectclass = new Elements_.Element_ObjectClass_(_referenced_object._ObjectClass);
                                    IDictionary<string, Elements_.Element_Endpoint_> _endpoints = new Dictionary<string, Elements_.Element_Endpoint_>();
                                    Elements_.Element_Object_ _object = new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Parameter_,
                                        _actual_id, null, _objectclass, null, _parameter, null, null, _attributes, null, _endpoints);
                                    foreach (Elements_.Element_Endpoint_ _endpoint in _objectclass._Endpoints.Values)
                                        _endpoints.Add(_endpoint._ID, 
                                            new Elements_.Element_Endpoint_(_object, _endpoint._ID, 
                                                new Elements_.Element_EndpointClass_(_endpoint._EndpointClass)));
                                    return _object;
                                }
                                else
                                {
                                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Parameter_,
                                        _actual_id, null, null, null, _parameter, null, null, _attributes, null, null);
                                }
*/

                                #endregion
                            }
                            else
                            {
                                #region Predefined Object

                                QS.Fx.Reflection.IComponentClass _componentclass = _library.GetComponentClass(_id);

                                Elements_.Element_Environment_ _environment_1 =
                                    this._LoadEnvironment(
                                        ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).ClassParameters,
                                        _xmlobject.Parameters, _context, true, false, false);

                                Elements_.Element_Environment_ _environment_2 =
                                    this._LoadEnvironment(
                                        ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).ClassParameters,
                                        _xmlobject.Parameters, _environment_1, false, true, false);

                                Elements_.Element_ObjectClass_ _component_objectclass = 
                                    this._LoadObjectClass(_componentclass.ObjectClass, _environment_1, true);

                                IDictionary<QS.Fx.Base.ID, QS.Fx.Attributes.IAttribute> _new_attributes = 
                                    new Dictionary<QS.Fx.Base.ID, QS.Fx.Attributes.IAttribute>();
                                if (_attributes != null)
                                {
                                    foreach (QS.Fx.Attributes.IAttribute _attribute in _attributes)
                                        _new_attributes.Add(_attribute.Class.ID, _attribute);
                                }
                                if (((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>) _componentclass).Attributes != null)
                                {
                                    foreach (QS.Fx.Attributes.IAttribute _attribute in (((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).Attributes))
                                    {
                                        if (!_new_attributes.ContainsKey(_attribute.Class.ID))
                                            _new_attributes.Add(_attribute.Class.ID, _attribute);
                                    }
                                }
                                QS.Fx.Attributes.IAttribute[] _aaa = new QS.Fx.Attributes.IAttribute[_new_attributes.Values.Count];
                                _new_attributes.Values.CopyTo(_aaa, 0);
                                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                                    ((QS._qss_x_.Reflection_.ComponentClass)_componentclass)._ID,
                                    new QS.Fx.Attributes.Attributes(_aaa),
                                    QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Predefined_,
                                    null,
                                    _environment_1,
                                    _environment_2,
                                    ((QS._qss_x_.Reflection_.ComponentClass)_componentclass)._Original_Base_Template_ComponentClass,
                                    _component_objectclass,
                                    null,
                                    null,
                                    null,
                                    null);

/*
                                _objectclass = this._LoadObjectClass(_componentclass.ObjectClass, _environment);
                                _objectclass._Cloned = true;
                                IDictionary<string, Elements_.Element_Endpoint_> _endpoints = new Dictionary<string, Elements_.Element_Endpoint_>();
                                foreach (Elements_.Element_Endpoint_ _endpoint in _objectclass._Endpoints.Values)
                                {
                                    Elements_.Element_Endpoint_ _new_endpoint = new Elements_.Element_Endpoint_(
                                        _object, _endpoint._ID, new Elements_.Element_EndpointClass_(_endpoint._EndpointClass));
                                    // _new_endpoint._Cloned = true;
                                    _endpoints.Add(_endpoint._ID, _new_endpoint);
                                }
                                return _object;
*/

                                #endregion
                            }
                        }
                    }
                    else
                        return null;

                    #endregion
                }
                else if (_xmlobject is QS.Fx.Reflection.Xml.CompositeObject)
                {
                    throw new NotImplementedException();

                    #region Composite Object

/*
                    IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_> _parameters =
                        _LoadParameters(_xmlobject.Parameters, _environment);
                    IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_> _new_environment = _parameters;                        
                    QS._qss_x_.ObjectDesigner_.Elements_.Element_ObjectClass_ _objectclass =
                        (_xmlobject.ObjectClass != null) ? this._LoadObjectClass(_xmlobject.ObjectClass, _new_environment) : null;
                    IDictionary<string, Elements_.Element_Component_> _components = new Dictionary<string, Elements_.Element_Component_>();
                    IDictionary<string, Elements_.Element_Endpoint_> _endpoints = new Dictionary<string, Elements_.Element_Endpoint_>();
                    IList<Elements_.Element_Connection_> _connections = new List<Elements_.Element_Connection_>();
                    Elements_.Element_Object_ _object = 
                        new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                            QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Composite_,
                            null, 
                            null,
                            _objectclass, 
                            null,
                            null,
                            _components,
                            _connections,
                            _attributes, 
                            _parameters, 
                            _endpoints);
                    foreach (Elements_.Element_Endpoint_ _endpoint in _objectclass._Endpoints.Values)
                        _endpoints.Add(_endpoint._ID, new Elements_.Element_Endpoint_(_object, _endpoint._ID, 
                            new Elements_.Element_EndpointClass_(_endpoint._EndpointClass)));
                    QS.Fx.Reflection.Xml.CompositeObject _xmlcompositeobject = (QS.Fx.Reflection.Xml.CompositeObject)_xmlobject;
                    if (_xmlcompositeobject.Components != null)
                    {
                        foreach (QS.Fx.Reflection.Xml.CompositeObject.Component _component in _xmlcompositeobject.Components)
                        {
                            Elements_.Element_Object_ _oo = _LoadObject(_component.Object, _new_environment);
                            _components.Add(_component.ID, new Elements_.Element_Component_(_component.ID, _oo));
                        }
                    }
                    if (_xmlcompositeobject.Endpoints != null)
                    {
                        foreach (QS.Fx.Reflection.Xml.CompositeObject.Endpoint _endpoint in _xmlcompositeobject.Endpoints)
                        {
                            Elements_.Element_Endpoint_ _frontend;
                            if (!_endpoints.TryGetValue(_endpoint.ID, out _frontend))
                                throw new Exception("Composite object defines an endpoint \"" + _endpoint.ID + "\" that is not defined in the object class.");
                            string[] _from = _endpoint.From.Split('.');
                            if (_from.Length != 2)
                                throw new Exception("Incorrect format of the endpoint specification \"" + _endpoint.From + "\".");
                            Elements_.Element_Component_ _c_oo;
                            if (!_components.TryGetValue(_from[0], out _c_oo))
                                throw new Exception("Incorrect parameter \"" + _from[0] + "\" in the endpoint specification \"" + _endpoint.From + "\".");
                            Elements_.Element_Object_ _oo = _c_oo._Object;
//                            Elements_.Element_Parameter_ _parameter;
//                            if (!_parameters.TryGetValue(_from[0], out _parameter))
//                                throw new Exception("Incorrect parameter \"" + _from[0] + "\" in the endpoint specification \"" + _endpoint.From + "\".");
//                            if (_parameter._Class != QS.Fx.Reflection.ParameterClass.Value)
//                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + _endpoint.From + "\" is not a value.");
//                            Elements_.Element_Value_ _value = (Elements_.Element_Value_)_parameter._Value;
//                            if (_value._Category != QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Object_)
//                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + _endpoint.From + "\" is not an object reference.");
//                            if (_value._Object == null)
//                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + _endpoint.From + "\" is undefined.");                            
//                            Elements_.Element_Object_ _oo = (Elements_.Element_Object_)_value._Object;
                            Elements_.Element_Endpoint_ _backend;                            
                            if (!_oo._Endpoints.TryGetValue(_from[1], out _backend))
                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + 
                                    _endpoint.From + "\" does not have endpoint \"" + _from[1] + "\".");
                            _frontend._Backend = _backend;
                            _backend._Frontend = _frontend;
                        }
                    }
                    if (_xmlcompositeobject.Connections != null)
                    {
                        foreach (QS.Fx.Reflection.Xml.CompositeObject.Connection _connection in _xmlcompositeobject.Connections)
                        {
                            string[] _from = _connection.From.Split('.');
                            if (_from.Length != 2)
                                throw new Exception("Incorrect format of the endpoint specification \"" + _connection.From + "\".");
                            Elements_.Element_Component_ _c_oo1;
                            if (!_components.TryGetValue(_from[0], out _c_oo1))
                                throw new Exception("Incorrect parameter \"" + _from[0] + "\" in the endpoint specification \"" + _connection.From + "\".");
                            Elements_.Element_Object_ _oo1 = _c_oo1._Object;
//                            Elements_.Element_Parameter_ _parameter;
//                            if (!_parameters.TryGetValue(_from[0], out _parameter))
//                                throw new Exception("Incorrect parameter \"" + _from[0] + "\" in the endpoint specification \"" + _connection.From + "\".");
//                            if (_parameter._Class != QS.Fx.Reflection.ParameterClass.Value)
//                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + _connection.From + "\" is not a value.");
//                            Elements_.Element_Value_ _value = (Elements_.Element_Value_)_parameter._Value;
//                            if (_value._Category != QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Object_)
//                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + _connection.From + "\" is not an object reference.");
//                            if (_value._Object == null)
//                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + _connection.From + "\" is undefined.");
//                            Elements_.Element_Object_ _oo1 = (Elements_.Element_Object_)_value._Object;
                            Elements_.Element_Endpoint_ _ee1;
                            if (!_oo1._Endpoints.TryGetValue(_from[1], out _ee1))
                                throw new Exception("Parameter \"" + _from[0] + "\" in the endpoint specification \"" + 
                                    _connection.From + "\" does not have endpoint \"" + _from[1] + "\".");
                            string[] _to = _connection.To.Split('.');
                            if (_to.Length != 2)
                                throw new Exception("Incorrect format of the endpoint specification \"" + _connection.To + "\".");
                            Elements_.Element_Component_ _c_oo2;
                            if (!_components.TryGetValue(_to[0], out _c_oo2))
                                throw new Exception("Incorrect parameter \"" + _to[0] + "\" in the endpoint specification \"" + _connection.To + "\".");
                            Elements_.Element_Object_ _oo2 = _c_oo2._Object;
//                            if (!_parameters.TryGetValue(_to[0], out _parameter))
//                                throw new Exception("Incorrect parameter \"" + _to[0] + "\" in the endpoint specification \"" + _connection.To + "\".");
//                            if (_parameter._Class != QS.Fx.Reflection.ParameterClass.Value)
//                                throw new Exception("Parameter \"" + _to[0] + "\" in the endpoint specification \"" + _connection.To + "\" is not a value.");
//                            _value = (Elements_.Element_Value_) _parameter._Value;
//                            if (_value._Category != QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Object_)
//                                throw new Exception("Parameter \"" + _to[0] + "\" in the endpoint specification \"" + _connection.To + "\" is not an object reference.");
//                            if (_value._Object == null)
//                                throw new Exception("Parameter \"" + _to[0] + "\" in the endpoint specification \"" + _connection.To + "\" is undefined.");
//                            Elements_.Element_Object_ _oo2 = (Elements_.Element_Object_)_value._Object;
                            Elements_.Element_Endpoint_ _ee2;
                            if (!_oo2._Endpoints.TryGetValue(_to[1], out _ee2))
                                throw new Exception("Parameter \"" + _to[0] + "\" in the endpoint specification \"" + _connection.To + "\" does not have endpoint \"" + _to[1] + "\".");
                            Elements_.Element_Connection_ _ccc = 
                                new QS._qss_x_.ObjectDesigner_.Elements_.Element_Connection_(_ee1, _ee2);
                            _ee1._Connection = _ccc;
                            _ee2._Connection = _ccc;
                            _connections.Add(_ccc);
                        }
                    }
                    return _object;
*/

                    #endregion
                }
                else
                    throw new NotImplementedException();

            }
        }

        #endregion

        #region _LoadObject(QS.Fx.Reflection.IObject _o)

        public QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _LoadObject(QS.Fx.Reflection.IObject _o)
        {
            lock (this)
            {
                if (_o is QS.Fx.Reflection.IComponentClass)
                {
                    QS.Fx.Reflection.IComponentClass _componentclass = (QS.Fx.Reflection.IComponentClass) _o;

                    Elements_.Element_Environment_ _environment_1 =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).ClassParameters,
                            null, null, true, false, false);

                    Elements_.Element_Environment_ _environment_2 =
                        this._LoadEnvironment(
                            ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).ClassParameters,
                            null, _environment_1, false, true, false);

                    Elements_.Element_ObjectClass_ _component_objectclass =
                        this._LoadObjectClass(_componentclass.ObjectClass, _environment_1, true);

                    return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_(
                        ((QS._qss_x_.Reflection_.ComponentClass)_componentclass)._ID,
                        _o.Attributes,
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_.Category_.Predefined_,
                        null,
                        _environment_1,
                        _environment_2,
                        ((QS._qss_x_.Reflection_.ComponentClass)_componentclass)._Original_Base_Template_ComponentClass,
                        _component_objectclass,
                        null,
                        null,
                        null,
                        null);
                }
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadEnvironment

        #region _LoadEnvironment(IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IEnumerable<QS.Fx.Reflection.Xml.Parameter> _xmlparameters, Elements_.Element_Environment_ _context,bool _include_type_parameters,bool _include_data_parameters)

        private Elements_.Element_Environment_ _LoadEnvironment(
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, IEnumerable<QS.Fx.Reflection.Xml.Parameter> _xmlparameters,
            Elements_.Element_Environment_ _context, bool _include_type_parameters, bool _include_data_parameters, bool _automatic)
        {
            IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_> _parameters = 
                new Dictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_>();
            if (_xmlparameters != null)
            {
                foreach (QS.Fx.Reflection.Xml.Parameter _xmlparameter in _xmlparameters)
                {
                    QS.Fx.Reflection.IParameter _classparameter;
                    if (!_classparameters.TryGetValue(_xmlparameter.ID, out _classparameter))
                        throw new Exception("The parameter \"" + _xmlparameter.ID + "\" is not one of the template parameters.");                    
                    if (((_classparameter.ParameterClass != QS.Fx.Reflection.ParameterClass.Value) && _include_type_parameters) ||
                        ((_classparameter.ParameterClass == QS.Fx.Reflection.ParameterClass.Value) && _include_data_parameters))
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _p =
                            this._LoadParameter(_classparameter, _xmlparameter, _context);
                        _parameters.Add(_classparameter.ID, _p);
                    }
                }
            }
            foreach (KeyValuePair<string, QS.Fx.Reflection.IParameter> _k in _classparameters)
            {
                QS.Fx.Reflection.IParameter _classparameter = _k.Value;
                if (((_classparameter.ParameterClass != QS.Fx.Reflection.ParameterClass.Value) && _include_type_parameters) ||
                    ((_classparameter.ParameterClass == QS.Fx.Reflection.ParameterClass.Value) && _include_data_parameters))
                {
                    if (!_parameters.ContainsKey(_k.Key))
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _p =
                            this._LoadParameter(_classparameter, _context, _automatic);
                        _parameters.Add(_classparameter.ID, _p);
                    }
                }
            }
            return new Elements_.Element_Environment_(_context, _parameters, _automatic, _classparameters);
        }

        #endregion

/*
        #region _LoadParameters(IEnumerable<QS.Fx.Reflection.Xml.Parameter> _xmlparameters,IDictionary<string, Elements_.Element_Parameter_> _environment)

        private IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_> _LoadParameters(
            IEnumerable<QS.Fx.Reflection.Xml.Parameter> _xmlparameters, 
            IDictionary<string, Elements_.Element_Parameter_> _environment)
        {
            IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_> _parameters =
                new Dictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_>();
            if (_xmlparameters != null)
            {
                foreach (QS.Fx.Reflection.Xml.Parameter _xmlparameter in _xmlparameters)
                {
                    QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _p = this._LoadParameter(_xmlparameter, _environment);
                    _parameters.Add(_p._ID, _p);
                }
            }
            return (_parameters.Count > 0) ? _parameters : null;
        }

        #endregion
*/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _LoadParameter

        #region _LoadParameter(QS.Fx.Reflection.IParameter _classparameter, QS.Fx.Reflection.Xml.Parameter _xmlparameter, Elements_.Element_Environment_ _context)

        private QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _LoadParameter(
            QS.Fx.Reflection.IParameter _classparameter, QS.Fx.Reflection.Xml.Parameter _xmlparameter, 
            Elements_.Element_Environment_ _context)
        {
            switch (_classparameter.ParameterClass)
            {
                case QS.Fx.Reflection.ParameterClass.ValueClass:
                    {
                        if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ValueClass)
                        {
                            QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue = 
                                this._LoadValueClass((QS.Fx.Reflection.Xml.ValueClass)_xmlparameter.Value, _context);
                            return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                                _classparameter.Attributes, _parametervalue, _context, false, _classparameter);
                        }
                        else
                            throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting a value class.");
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                    {
                        if (_xmlparameter.Value is QS.Fx.Reflection.Xml.InterfaceClass)
                        {
                            QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue = 
                                this._LoadInterfaceClass((QS.Fx.Reflection.Xml.InterfaceClass)_xmlparameter.Value, _context);
                            return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                                _classparameter.Attributes, _parametervalue, _context, false, _classparameter);
                        }
                        else
                            throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting an interface class.");
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                    {
                        if (_xmlparameter.Value is QS.Fx.Reflection.Xml.EndpointClass)
                        {
                            QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue =
                                this._LoadEndpointClass((QS.Fx.Reflection.Xml.EndpointClass)_xmlparameter.Value, _context);
                            return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                                _classparameter.Attributes, _parametervalue, _context, false, _classparameter);
                        }
                        else
                            throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting an endpoint class.");
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                    {
                        if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ObjectClass)
                        {
                            QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue = 
                                this._LoadObjectClass((QS.Fx.Reflection.Xml.ObjectClass)_xmlparameter.Value, _context);
                            return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                                _classparameter.Attributes, _parametervalue, _context, false, _classparameter);
                        }
                        else
                            throw new Exception("Illegal value of parameter \"" + _xmlparameter.ID + "\": expecting an object class.");
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.Value:
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_ _parametervalueclass =
                            this._LoadValueClass(_classparameter.ValueClass, _context, true);
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue;
                        if (_xmlparameter.Value is QS.Fx.Reflection.Xml.Object)
                            _parametervalue = this._LoadObject((QS.Fx.Reflection.Xml.Object)_xmlparameter.Value, _context);
                        else
                            _parametervalue = new Elements_.Element_ValueObject_(_xmlparameter.Value);
                        return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, 
                            _parametervalueclass, _classparameter.Attributes, _parametervalue, _context, false, _classparameter);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _LoadParameter(QS.Fx.Reflection.IParameter _classparameter, Elements_.Element_Environment_ _context)

        private QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _LoadParameter(
            QS.Fx.Reflection.IParameter _classparameter, Elements_.Element_Environment_ _context, bool _automatic)
        {
            switch (_classparameter.ParameterClass)
            {
                case QS.Fx.Reflection.ParameterClass.ValueClass:
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue =
                            this._LoadValueClass((QS.Fx.Reflection.IValueClass)_classparameter.Value, _context, _automatic);
                        return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                            _classparameter.Attributes, _parametervalue, _context, _automatic, _classparameter);
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue =
                            this._LoadInterfaceClass((QS.Fx.Reflection.IInterfaceClass)_classparameter.Value, _context, _automatic);
                        return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                            _classparameter.Attributes, _parametervalue, _context, _automatic, _classparameter);
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue =
                            this._LoadEndpointClass((QS.Fx.Reflection.IEndpointClass)_classparameter.Value, _context, _automatic);
                        return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                            _classparameter.Attributes, _parametervalue, _context, _automatic, _classparameter);
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ _parametervalue =
                            this._LoadObjectClass((QS.Fx.Reflection.IObjectClass)_classparameter.Value, _context, _automatic);
                        return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass, null,
                            _classparameter.Attributes, _parametervalue, _context, _automatic, _classparameter);
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.Value:
                    {
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_ _parametervalueclass =
                            this._LoadValueClass((QS.Fx.Reflection.IValueClass)_classparameter.ValueClass, _context, true);
                        return new Elements_.Element_Parameter_(_classparameter.ID, _classparameter.ParameterClass,
                            _parametervalueclass, _classparameter.Attributes, null, _context, _automatic, _classparameter);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

/*
        #region _LoadParameter(QS.Fx.Reflection.Xml.Parameter _xmlparameter, IDictionary<string, Elements_.Element_Parameter_> _environment)

        private QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _LoadParameter(
            QS.Fx.Reflection.Xml.Parameter _xmlparameter, IDictionary<string, Elements_.Element_Parameter_> _environment)
        {
            if (_xmlparameter.Value == null)
                throw new Exception("A parameter \"" + _xmlparameter.ID + "\" cannot be left unassigned wehn used in this context.");
            if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ValueClass)
            {
                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                    _xmlparameter.ID, 
                    QS.Fx.Reflection.ParameterClass.ValueClass, 
                    null, 
                    false,
                    this._LoadValueClass((QS.Fx.Reflection.Xml.ValueClass)_xmlparameter.Value, _environment));
            }
            else if (_xmlparameter.Value is QS.Fx.Reflection.Xml.InterfaceClass)
            {
                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                    _xmlparameter.ID, 
                    QS.Fx.Reflection.ParameterClass.InterfaceClass, 
                    null, 
                    false,
                    this._LoadInterfaceClass((QS.Fx.Reflection.Xml.InterfaceClass)_xmlparameter.Value, _environment));
            }
            else if (_xmlparameter.Value is QS.Fx.Reflection.Xml.EndpointClass)
            {
                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                    _xmlparameter.ID, 
                    QS.Fx.Reflection.ParameterClass.EndpointClass, 
                    null, 
                    false,
                    this._LoadEndpointClass((QS.Fx.Reflection.Xml.EndpointClass)_xmlparameter.Value, _environment));
            }
            else if (_xmlparameter.Value is QS.Fx.Reflection.Xml.ObjectClass)
            {
                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                    _xmlparameter.ID, 
                    QS.Fx.Reflection.ParameterClass.ObjectClass, 
                    null, 
                    false,
                    this._LoadObjectClass((QS.Fx.Reflection.Xml.ObjectClass)_xmlparameter.Value, _environment));
            }
            else if (_xmlparameter.Value is QS.Fx.Reflection.Xml.Object)
            {
                Elements_.Element_Object_ _o =
                    this._LoadObject((QS.Fx.Reflection.Xml.Object)_xmlparameter.Value, _environment);                
                QS.Fx.Reflection.IValueClass _vc1 = 
                    QS._qss_x_.Reflection_.Library.LocalLibrary.GetValueClass(QS.Fx.Reflection.ValueClasses._Object);
                QS._qss_x_.Reflection_.ValueClass _vc2 = (QS._qss_x_.Reflection_.ValueClass) _vc1;
                IDictionary<string, Elements_.Element_Parameter_> _parameters = 
                    new Dictionary<string, Elements_.Element_Parameter_>();
                _parameters.Add(
                    "ObjectClass", 
                    new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                        "ObjectClass", 
                        QS.Fx.Reflection.ParameterClass.ObjectClass,
                        null,
                        true,
                        new Elements_.Element_ObjectClass_(_o._ObjectClass)));
                Elements_.Element_ValueClass_ _valueclass = 
                    new QS._qss_x_.ObjectDesigner_.Elements_.Element_ValueClass_(_vc2._ID, _vc1, _parameters);
                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                    _xmlparameter.ID, 
                    QS.Fx.Reflection.ParameterClass.Value,
                    null, 
                    false, 
                    new QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_(
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Object_,
                        _valueclass, 
                        null, 
                        _o));
            }
            else
            {
                return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                    _xmlparameter.ID,
                    QS.Fx.Reflection.ParameterClass.Value,
                    null,
                    false,
                    new QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_(
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Value_,
                        this._LoadValueClass(QS._qss_x_.Reflection_.Library.ValueClassOf(_xmlparameter.Value.GetType()), _environment),
                        _xmlparameter.Value.GetType(),
                        _xmlparameter.Value));
            }
        }


        private QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _LoadParameter(
            QS.Fx.Reflection.IParameter _classparameter, out QS.Fx.Reflection.IParameter _reflection_parameter)
        {
            _reflection_parameter = null;
            QS._qss_x_.ObjectDesigner_.Elements_.Element_ _value;
            switch (_classparameter.ParameterClass)
            {
                case QS.Fx.Reflection.ParameterClass.ValueClass:
                    {
                        QS.Fx.Reflection.IValueClass _valueclass;
                        if ((_classparameter.Value is QS.Fx.Reflection.IValueClass) && 
                            (((QS.Fx.Reflection.IValueClass) _classparameter.Value).ID != null))
                            _valueclass = (QS.Fx.Reflection.IValueClass)_classparameter.Value;
                        else
                            _valueclass = this._library.GetValueClass(QS.Fx.Reflection.ValueClasses.ISerializable);
                        _reflection_parameter = new QS.Fx.Reflection.Parameter(_classparameter.ID,
                            _classparameter.Attributes, QS.Fx.Reflection.ParameterClass.ValueClass, null, null, _valueclass);
                        _value = this._LoadValueClass(_valueclass);
                    }
                    break;
                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                    {
                        QS.Fx.Reflection.IInterfaceClass _interfaceclass;
                        if ((_classparameter.Value is QS.Fx.Reflection.IInterfaceClass) &&
                            (((QS.Fx.Reflection.IInterfaceClass)_classparameter.Value).ID != null))
                            _interfaceclass = (QS.Fx.Reflection.IInterfaceClass)_classparameter.Value;
                        else
                            _interfaceclass = this._library.GetInterfaceClass(QS.Fx.Reflection.InterfaceClasses.Interface);
                        _reflection_parameter = new QS.Fx.Reflection.Parameter(_classparameter.ID,
                            _classparameter.Attributes, QS.Fx.Reflection.ParameterClass.InterfaceClass, null, null, _interfaceclass);
                        _value = this._LoadInterfaceClass(_interfaceclass);
                    }
                    break;
                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                    {
                        QS.Fx.Reflection.IEndpointClass _endpointclass;
                        if ((_classparameter.Value is QS.Fx.Reflection.IEndpointClass) &&
                            (((QS.Fx.Reflection.IEndpointClass)_classparameter.Value).ID != null))
                            _endpointclass = (QS.Fx.Reflection.IEndpointClass)_classparameter.Value;
                        else
                            _endpointclass = this._library.GetEndpointClass(QS.Fx.Reflection.EndpointClasses.Endpoint);
                        _reflection_parameter = new QS.Fx.Reflection.Parameter(_classparameter.ID,
                            _classparameter.Attributes, QS.Fx.Reflection.ParameterClass.EndpointClass, null, null, _endpointclass);
                        _value = this._LoadEndpointClass(_endpointclass);
                    }
                    break;
                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                    {
                        QS.Fx.Reflection.IObjectClass _objectclass;
                        if ((_classparameter.Value is QS.Fx.Reflection.IObjectClass) &&
                            (((QS.Fx.Reflection.IObjectClass)_classparameter.Value).ID != null))
                            _objectclass = (QS.Fx.Reflection.IObjectClass)_classparameter.Value;
                        else
                            _objectclass = this._library.GetObjectClass(QS.Fx.Reflection.ObjectClasses.Object);
                        _reflection_parameter = new QS.Fx.Reflection.Parameter(_classparameter.ID,
                            _classparameter.Attributes, QS.Fx.Reflection.ParameterClass.ObjectClass, null, null, _objectclass);
                        _value = this._LoadObjectClass(_objectclass);
                    }
                    break;
                case QS.Fx.Reflection.ParameterClass.Value:
                    {
                        _reflection_parameter = new QS.Fx.Reflection.Parameter(_classparameter.ID,
                            _classparameter.Attributes, QS.Fx.Reflection.ParameterClass.Value, _classparameter.ValueClass, null, null);
                        Type _parametertype = _classparameter.ValueClass.UnderlyingType;
                        if (_parametertype.IsGenericType && _parametertype.GetGenericTypeDefinition().Equals(_objectreferencetype))
                        {
                            _value = new QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_(
                                QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Object_,
                                this._LoadValueClass(_classparameter.ValueClass),
                                null,
                                null);
                        }
                        else
                        {
                            _value = new QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_(
                                QS._qss_x_.ObjectDesigner_.Elements_.Element_Value_.Category_.Value_,
                                this._LoadValueClass(_classparameter.ValueClass),
                                _parametertype,
                                null);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_(
                _classparameter.ID, _classparameter.ParameterClass, _classparameter.Attributes, true, _value);
        }
 * 
        #endregion
*/

         #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
