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

namespace QS._qss_x_.Properties_.Component_
{
    public sealed class PropertiesClient_
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>,
        QS._qss_x_.Properties_.Base_.IPropertiesClient_,
        QS.Fx.Interface.Classes.IProperties
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PropertiesClient_
        (
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Base.Identifier _identifier,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_reference,
            QS._qss_x_.Properties_.Base_.PropertiesCallback_ _propertiescallback, 
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_.Constructor");
#endif

            this._propertiescallback = _propertiescallback;
            this._identifier = _identifier;
            this._delegation_reference = _delegation_reference;
            this._delegation_endpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>,
                    QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>>(this);
            this._delegation_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Connect)));
                    });
            this._delegation_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Disconnect)));
                    });
            this._properties_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IProperties, QS.Fx.Interface.Classes.IProperties>(this);
            this._properties_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Connect)));
                    });
            this._properties_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Disconnect)));
                    });
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties> _delegation_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>,
            QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _delegation_connection;
        [QS.Fx.Base.Inspectable]
        private bool _isdelegated;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IProperties> _properties_reference;
        [QS.Fx.Base.Inspectable]
        private bool _isrequested;
        [QS.Fx.Base.Inspectable]
        private bool _isinitialized;
        [QS.Fx.Base.Inspectable]
        private bool _isconnected;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IProperties _properties_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IProperties, QS.Fx.Interface.Classes.IProperties> _properties_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _properties_connection;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.PropertiesCallback_ _propertiescallback;
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Value.Classes.IPropertyValues> _outgoing = new Queue<QS.Fx.Value.Classes.IPropertyValues>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDelegationChannelClient<Identifier,IProperties> Members

        void QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>.Delegate(
            QS.Fx.Base.Identifier _identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IProperties> _object)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IProperties>>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Delegate), _identifier, _object));            
        }

        void QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>.Undelegate(QS.Fx.Base.Identifier _identifier)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Identifier>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Undelegate), _identifier));            
        }

        #endregion

        #region IProperties Members

        void QS.Fx.Interface.Classes.IProperties.Properties(QS.Fx.Value.Classes.IPropertyValues _properties)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.Classes.IPropertyValues>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Incoming), _properties));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IPropertiesClient_ Members

        void QS._qss_x_.Properties_.Base_.IPropertiesClient_._Enable()
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Enable)));            

        }

        private void _Enable(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Enable ");
#endif

            lock (this)
            {
                if (!this._isrequested)
                {
                    this._isrequested = true;
                    if (this._isdelegated)
                        this._Properties_Initialize();
                }
            }
        }

        void QS._qss_x_.Properties_.Base_.IPropertiesClient_._Disable()
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disable)));
        }

        private void _Disable(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Disable ");
#endif

            if (this._isrequested)
            {
                this._isrequested = false;
                if (this._isdelegated)
                    this._Properties_Dispose();
            }
        }

        void QS._qss_x_.Properties_.Base_.IPropertiesClient_._Properties(QS.Fx.Value.Classes.IPropertyValues _properties)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.Classes.IPropertyValues>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Properties_Outgoing), _properties));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                this._delegation_object = this._delegation_reference.Dereference(_mycontext);
                if (this._delegation_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication) this._delegation_object).Start(this._platform, null);
                this._delegation_connection = this._delegation_endpoint.Connect(this._delegation_object.Delegation);
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Dispose");
#endif

            lock (this)
            {
                if (this._properties_endpoint.IsConnected)
                    this._properties_endpoint.Disconnect();
                if ((this._properties_object != null) && (this._properties_object is IDisposable))
                    ((IDisposable)this._properties_object).Dispose();
                this._properties_object = null;
                this._properties_reference = null;
                this._properties_connection = null;

                if (this._delegation_endpoint.IsConnected)
                    this._delegation_endpoint.Disconnect();
                if ((this._delegation_object != null) && (this._delegation_object is IDisposable))
                    ((IDisposable) this._delegation_object).Dispose();
                this._delegation_object = null;
                this._delegation_reference = null;
                this._delegation_connection = null;
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._platform != null) && (this._delegation_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._delegation_object).Start(this._platform, null);

                if ((this._platform != null) && (this._properties_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._properties_object).Start(this._platform, null);
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Stop");
#endif

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Delegation_Connect

        private void _Delegation_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Delegation_Connect ");
#endif

            lock (this)
            {
                if (this._delegation_endpoint.IsConnected)
                    this._delegation_endpoint.Interface.Register(this._identifier);
            }
        }

        #endregion

        #region _Delegation_Disconnect

        private void _Delegation_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Delegation_Disconnect");
#endif
        }

        #endregion

        #region _Delegation_Delegate

        private void _Delegation_Delegate(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IProperties>> _event_ =
                ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IProperties>>)_event);
            QS.Fx.Base.Identifier _identifier = _event_._Object1;
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IProperties> _properties_reference = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Delegation_Delegate ( " + _identifier.ToString() + " )");
#endif

            lock (this)
            {
                if (this._identifier.Equals(_identifier))
                {
                    if (!this._isdelegated)
                    {
                        this._isdelegated = true;
                        this._properties_reference = _properties_reference;
                        if (this._isrequested)
                            this._Properties_Initialize();
                    }
                }
                else
                    _mycontext.Error("Identifier does not match.");
            }
        }

        #endregion

        #region _Delegation_Undelegate

        private void _Delegation_Undelegate(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier> _event_ =
                ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier>)_event);
            QS.Fx.Base.Identifier _identifier = _event_._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Delegation_Undelegate ( " + _identifier.ToString() + " )");
#endif

            lock (this)
            {
                if (this._isdelegated)
                {
                    this._isdelegated = false;
                    if (this._isrequested)
                        this._Properties_Dispose();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Properties_Initialize

        private void _Properties_Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Properties_Initialize");
#endif

            if (!this._isinitialized)
            {
                this._isinitialized = true;
                this._properties_object = this._properties_reference.Dereference(_mycontext);
                if (this._properties_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication)this._properties_object).Start(this._platform, null);
                this._properties_connection = this._properties_endpoint.Connect(this._properties_object.Properties);
            }
        }

        #endregion

        #region _Properties_Dispose

        private void _Properties_Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Properties_Dispose");
#endif

            this._isinitialized = false;
            this._isconnected = false;
            if (this._properties_endpoint.IsConnected)
                this._properties_endpoint.Disconnect();
            if ((this._properties_object != null) && (this._properties_object is IDisposable))
                ((IDisposable)this._properties_object).Dispose();
            this._properties_object = null;
            this._properties_reference = null;
            this._properties_connection = null;
            this._outgoing.Clear();
        }

        #endregion

        #region _Properties_Connect

        private void _Properties_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Properties_Connect ");
#endif

            lock (this)
            {
                if (this._isinitialized && !this._isconnected)
                {
                    this._isconnected = true;
                    _Properties_Outgoing();
                }
            }
        }

        #endregion

        #region _Properties_Disconnect

        private void _Properties_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Properties_Disconnect");
#endif

            lock (this)
            {
                this._isconnected = false;
            }
        }

        #endregion

        #region _Properties_Incoming

        private void _Properties_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IPropertyValues _properties = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IPropertyValues>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Properties_Incoming\n\n" + QS.Fx.Printing.Printable.ToString(_properties));
#endif

            this._propertiescallback(_properties);
        }

        #endregion

        #region _Properties_Outgoing

        private void _Properties_Outgoing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IPropertyValues _properties = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IPropertyValues>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.PropertiesClient_._Properties_Outgoing\n\n" + QS.Fx.Printing.Printable.ToString(_properties));
#endif

            lock (this)
            {
                _outgoing.Enqueue(_properties);
                if (this._isconnected)
                    this._Properties_Outgoing();
            }
        }

        private void _Properties_Outgoing()
        {
            if (this._properties_endpoint.IsConnected)
            {
                while (this._outgoing.Count > 0)
                    this._properties_endpoint.Interface.Properties(this._outgoing.Dequeue());
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
