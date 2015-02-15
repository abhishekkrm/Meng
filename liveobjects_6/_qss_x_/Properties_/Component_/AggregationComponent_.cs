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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

#if NO

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.AggregationComponent, "Properties Framework Aggregation Component")]
    public sealed class AggregationComponent_
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IProperties,
        QS.Fx.Interface.Classes.IProperties,
        QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public AggregationComponent_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("properties", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Value.Properties _properties,
            [QS.Fx.Reflection.Parameter("aggregator", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>> _aggregator_reference,
            [QS.Fx.Reflection.Parameter("identifier", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Identifier _identifier,
            [QS.Fx.Reflection.Parameter("delegation", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_.Constructor");
#endif

            this._identifier = _identifier;
            this._delegation_reference = _delegation_reference;
            this._aggregator_reference = _aggregator_reference;

            if (this._identifier != null)
                if (this._delegation_reference != null)
                    this._isroot = false;
                else
                    _mycontext.Error("Delegation reference cannot be NULL if identifier is non-NULL.");
            else
                if (this._delegation_reference != null)
                    _mycontext.Error("Identifier cannot be NULL if delegation reference is non-NULL.");
                else
                    this._isroot = true;

            this._properties_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IProperties, QS.Fx.Interface.Classes.IProperties>(this);
            this._properties_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Connect))); });
            this._properties_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Disconnect))); });

            this._aggregator_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS._qss_x_.Properties_.Value_.PropertiesControl_>,
                    QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS._qss_x_.Properties_.Value_.PropertiesControl_>>(this);
            this._aggregator_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Aggregator_Connect)));
                    });
            this._aggregator_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Aggregator_Disconnect)));
                    });

            this._Properties_Initialize(_properties);

            this._InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private bool _isroot;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>> _aggregator_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_> _aggregator_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>,
            QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>> 
                _aggregator_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _aggregator_connection;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.IPropertiesClient_ _client;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IProperties, QS.Fx.Interface.Classes.IProperties> _properties_endpoint;

        private IDictionary<QS.Fx.Base.Index, IList<QS._qss_x_.Properties_.Base_.IProperty_>> _map_pred =
            new Dictionary<QS.Fx.Base.Index, IList<QS._qss_x_.Properties_.Base_.IProperty_>>();

        private IDictionary<QS.Fx.Base.Index, IList<QS._qss_x_.Properties_.Base_.IProperty_>> _map_succ =
            new Dictionary<QS.Fx.Base.Index, IList<QS._qss_x_.Properties_.Base_.IProperty_>>();
        
        private IDictionary<QS.Fx.Base.Index, QS._qss_x_.Properties_.Base_.IProperty_> _properties =
            new Dictionary<QS.Fx.Base.Index, QS._qss_x_.Properties_.Base_.IProperty_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Inspection

        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Base.Index, QS._qss_x_.Properties_.Base_.IProperty_> __inspectable_properties;

        private void _InitializeInspection()
        {
            __inspectable_properties =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Base.Index, QS._qss_x_.Properties_.Base_.IProperty_>(
                    "__inspectable_properties", 
                    _properties);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IProperties Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IProperties, QS.Fx.Interface.Classes.IProperties> QS.Fx.Object.Classes.IProperties.Properties
        {
            get { return this._properties_endpoint; }
        }

        #endregion

        #region IProperties Members

        void QS.Fx.Interface.Classes.IProperties.Properties(QS.Fx.Value.Classes.IPropertyValues _properties)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.Classes.IPropertyValues>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Incoming_Pred), _properties));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                if (!this._isroot)
                {
                    this._client =
                        new PropertiesClient_
                        (
                            _mycontext,
                            this._identifier,
                            this._delegation_reference,
                            new QS._qss_x_.Properties_.Base_.PropertiesCallback_(this._PropertiesClient_Properties),
                            _debug
                        );

                    if ((this._platform != null) && (this._client is QS._qss_x_.Platform_.IApplication))
                        ((QS._qss_x_.Platform_.IApplication)this._client).Start(this._platform, null);
                }

                if (this._aggregator_reference != null)
                {
                    this._aggregator_object = _aggregator_reference.Dereference(_mycontext);
                    if ((this._platform != null) && (this._aggregator_object is QS._qss_x_.Platform_.IApplication))
                        ((QS._qss_x_.Platform_.IApplication) this._aggregator_object).Start(this._platform, null);
                    this._aggregator_connection = this._aggregator_endpoint.Connect(this._aggregator_object.Aggregator);
                }
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Dispose");
#endif

            lock (this)
            {
                if ((this._client != null) && (this._client is IDisposable))
                    ((IDisposable)this._client).Dispose();

                if (this._properties_endpoint.IsConnected)
                    this._properties_endpoint.Disconnect();

                base._Dispose();
            }
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Start");
#endif

            base._Start();
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Stop");
#endif

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _PropertiesClient_Properties

        private void _PropertiesClient_Properties(QS.Fx.Value.Classes.IPropertyValues _properties)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.Classes.IPropertyValues>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Incoming_Succ), _properties));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Properties_Initialize

        private void _Properties_Initialize(QS.Fx.Value.Properties _properties)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Properties_Initialize");
#endif

            if (_properties.Items != null)
            {
                QS._qss_x_.Interface_.Classes_.IDeserializer _deserializer = new QS._qss_x_.Reflection_.Loader(_mycontext, QS._qss_x_.Reflection_.Library.LocalLibrary);

                foreach (QS.Fx.Value.Property _property_def in _properties.Items)
                {
                    QS.Fx.Reflection.IValueClass _valueclass = _deserializer.DeserializeValueClass(_property_def.ValueClass);
                    QS.Fx.Value.Classes.IPropertyVersion _version = null;
                    QS.Fx.Serialization.ISerializable _value = null;
                    if (_property_def.InitialValue != null)
                    {
                        Type _underlyingtype = _valueclass.UnderlyingType;
                        if (!typeof(QS.Fx.Serialization.ISerializable).IsAssignableFrom(_underlyingtype))
                            _mycontext.Error("Cannot initialize property; the specified value class is not serializable.");
                        bool _initialized = false;
                        if (!_initialized)
                        {
                            System.Reflection.ConstructorInfo _constructorinfo = _underlyingtype.GetConstructor(new Type[] { typeof(string) });
                            if (_constructorinfo != null)
                            {
                                _value = (QS.Fx.Serialization.ISerializable)_constructorinfo.Invoke(new object[] { _property_def.InitialValue });
                                _initialized = true;
                            }
                        }
                        if (!_initialized)
                        {
                            System.Reflection.ConstructorInfo _constructorinfo = _underlyingtype.GetConstructor(Type.EmptyTypes);
                            if (_constructorinfo != null)
                            {
                                if (typeof(QS.Fx.Serialization.IStringSerializable).IsAssignableFrom(_underlyingtype))
                                {
                                    _value = (QS.Fx.Serialization.ISerializable)_constructorinfo.Invoke(new object[0]);
                                    ((QS.Fx.Serialization.IStringSerializable)_value).AsString = _property_def.InitialValue;
                                    _initialized = true;
                                }
                            }
                        }
                        if (!_initialized)
                        {
                            System.Reflection.MethodInfo _operatorinfo = null;
                            foreach (System.Reflection.MethodInfo _methodinfo in
                                _underlyingtype.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                            {
                                if ((_methodinfo.Name.Equals("op_Explicit") || _methodinfo.Name.Equals("op_Implicit")) && _methodinfo.ReturnType.Equals(_underlyingtype))
                                {
                                    bool _found = false;
                                    foreach (System.Reflection.ParameterInfo _parameterinfo in _methodinfo.GetParameters())
                                    {
                                        if (!_parameterinfo.IsRetval)
                                        {
                                            if (!_found && _parameterinfo.ParameterType.Equals(typeof(string)))
                                                _found = true;
                                            else
                                            {
                                                _found = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (_found)
                                    {
                                        _operatorinfo = _methodinfo;
                                        break;
                                    }
                                }
                            }
                            if (_operatorinfo != null)
                            {
                                _value = (QS.Fx.Serialization.ISerializable)_operatorinfo.Invoke(null, new object[] { _property_def.InitialValue });
                                _initialized = true;
                            }
                        }
                        if (!_initialized)
                            _mycontext.Error("Cannot initialize the property value; cannot find a suitable constructor and/or initializer method or casting operator.");
                        _version = new QS.Fx.Value.PropertyVersion(0UL, 0U);
                    }
                    QS._qss_x_.Properties_.Base_.IProperty_ _property =
                        new QS._qss_x_.Properties_.Base_.Property_
                        (
                            new QS.Fx.Base.Index(_property_def.Id),
                            null,
                            _property_def.Name,
                            _property_def.Comment,
                            _valueclass,
                            _version,
                            _value
                        );

                    this._properties.Add(_property._Index, _property);

                    foreach (QS.Fx.Value.PropertyBinding _binding in _property_def.Sources)
                    {
                        QS.Fx.Base.Index _index = new QS.Fx.Base.Index(_binding.Id);
                        switch (_binding.Location)
                        {
                            case QS.Fx.Value.PropertyLocation.Pred:
                                {
                                    IList<QS._qss_x_.Properties_.Base_.IProperty_> _destinations;
                                    if (!_map_pred.TryGetValue(_index, out _destinations))
                                    {
                                        _destinations = new List<QS._qss_x_.Properties_.Base_.IProperty_>();
                                        _map_pred.Add(_index, _destinations);
                                    }
                                    _destinations.Add(_property);
                                }
                                break;

                            case QS.Fx.Value.PropertyLocation.Succ:
                                {
                                    IList<QS._qss_x_.Properties_.Base_.IProperty_> _destinations;
                                    if (!_map_succ.TryGetValue(_index, out _destinations))
                                    {
                                        _destinations = new List<QS._qss_x_.Properties_.Base_.IProperty_>();
                                        _map_succ.Add(_index, _destinations);
                                    }
                                    _destinations.Add(_property);
                                }
                                break;

                            case QS.Fx.Value.PropertyLocation.Here:
                                {
                                    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ add updates from one to another.....
                                }
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        #endregion

        #region _Properties_Connect

        private void _Properties_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Properties_Connect ");
#endif
        }

        #endregion

        #region _Properties_Disconnect

        private void _Properties_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Properties_Disconnect");
#endif
        }

        #endregion

        #region _Properties_Incoming_Pred

        private void _Properties_Incoming_Pred(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IPropertyValues _propertyvalues = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IPropertyValues>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Properties_Incoming_Pred\n\n" + QS.Fx.Printing.Printable.ToString(_propertyvalues));
#endif

            lock (this)
            {
                foreach (QS.Fx.Value.Classes.IPropertyValue _propertyvalue in _propertyvalues.Items)
                {
                    IList<QS._qss_x_.Properties_.Base_.IProperty_> _mapped;
                    if (this._map_pred.TryGetValue(_propertyvalue.Index, out _mapped))
                    {
                        foreach (QS._qss_x_.Properties_.Base_.IProperty_ _property in _mapped)
                            _Properties_Update(_property, _propertyvalue.Version, _propertyvalue.Value);
                    }
                    else
                        _mycontext.Error("Unknown property with index " + _propertyvalue.Index.ToString() + " received at the bottom endpoint.");
                }
            }
        }

        #endregion

        #region _Properties_Incoming_Succ

        private void _Properties_Incoming_Succ(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IPropertyValues _propertyvalues = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IPropertyValues>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Properties_Incoming_Succ\n\n" + QS.Fx.Printing.Printable.ToString(_propertyvalues));
#endif

            lock (this)
            {
                foreach (QS.Fx.Value.Classes.IPropertyValue _propertyvalue in _propertyvalues.Items)
                {
                    IList<QS._qss_x_.Properties_.Base_.IProperty_> _mapped;
                    if (this._map_succ.TryGetValue(_propertyvalue.Index, out _mapped))
                    {
                        foreach (QS._qss_x_.Properties_.Base_.IProperty_ _property in _mapped)
                            _Properties_Update(_property, _propertyvalue.Version, _propertyvalue.Value);
                    }
                    else
                        _mycontext.Error("Unknown property with index " + _propertyvalue.Index.ToString() + " received at the top endpoint.");
                }
            }

        }

        #endregion

        #region _Properties_Update

        private void _Properties_Update
        (
            QS._qss_x_.Properties_.Base_.IProperty_ _property, 
            QS.Fx.Value.Classes.IPropertyVersion _version, 
            QS.Fx.Serialization.ISerializable _value
        )
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Properties_Update ( " + 
                    _property._Index.ToString() + ":" + _property._Name + ", " +
                    QS.Fx.Printing.Printable.ToString(_version.Incarnation) + ":" + 
                    QS.Fx.Printing.Printable.ToString(_version.Index) + " ) : \n\n" + 
                    QS.Fx.Printing.Printable.ToString(_value) + "\n");
#endif

            if (_property._Update(_version, _value))
            {

            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Aggregator_Connect

        private void _Aggregator_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Aggregator_Connect ");
#endif
        }

        #endregion

        #region _Aggregator_Disconnect

        private void _Aggregator_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.AggregationComponent_._Aggregator_Disconnect");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        #region IAggregatorClient<Incarnation,Index,Identifier,PropertiesControl_> Members

        void QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>.Phase(QS.Fx.Base.Identifier round)
        {
            throw new NotImplementedException();
        }

        void QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>.Disseminating(QS.Fx.Base.Identifier round, QS._qss_x_.Properties_.Value_.PropertiesControl_ message)
        {
            throw new NotImplementedException();
        }

        void QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Base.Identifier, QS._qss_x_.Properties_.Value_.PropertiesControl_>.Aggregate(QS.Fx.Base.Identifier round, IList<QS._qss_x_.Properties_.Value_.PropertiesControl_> messages)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

#endif
