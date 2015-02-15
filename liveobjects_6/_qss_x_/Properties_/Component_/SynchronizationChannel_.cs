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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.SynchronizationChannel, "Properties Framework Synchronization Channel")]
    public sealed class SynchronizationChannel_
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.ISynchronizationChannel,
        QS.Fx.Interface.Classes.ISynchronizationChannel
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public SynchronizationChannel_
        (
            QS.Fx.Object.IContext _mycontext,
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
                this._logger.Log("Component_.SynchronizationChannel_.Constructor");
#endif

            this._identifier = _identifier;
            this._delegation_reference = _delegation_reference;
            this._channel_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ISynchronizationChannelClient, QS.Fx.Interface.Classes.ISynchronizationChannel>(this);
            this._channel_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Connect))); });
            this._channel_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Disconnect))); });
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_reference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.IPropertiesClient_ _client;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ISynchronizationChannelClient, QS.Fx.Interface.Classes.ISynchronizationChannel> _channel_endpoint;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ISynchronizationChannel Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ISynchronizationChannelClient, QS.Fx.Interface.Classes.ISynchronizationChannel>
            QS.Fx.Object.Classes.ISynchronizationChannel.Channel
        {
            get { return this._channel_endpoint; }
        }

        #endregion

        #region ISynchronizationChannel Members

        void QS.Fx.Interface.Classes.ISynchronizationChannel.Ready(int _phase)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<int>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Ready), _phase));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                this._client = new PropertiesClient_(
                    _mycontext,
                    this._identifier, this._delegation_reference, new QS._qss_x_.Properties_.Base_.PropertiesCallback_(this._Properties_Incoming), _debug);
                if ((this._platform != null) && (this._client is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._client).Start(this._platform, null);
                
                this._client._Enable();
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Dispose");
#endif

            lock (this)
            {
                if ((this._client != null) && (this._client is IDisposable))
                    ((IDisposable) this._client).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._client != null) && (this._client is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._client).Start(this._platform, null);
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Stop");
#endif

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Channel_Connect

        private void _Channel_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Channel_Connect ");
#endif
        }

        #endregion

        #region _Channel_Disconnect

        private void _Channel_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Channel_Disconnect");
#endif
        }

        #endregion

        #region _Channel_Ready

        private void _Channel_Ready(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            int _phase = ((QS._qss_x_.Properties_.Base_.IEvent_<int>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationClient_._Channel_Ready ( " + _phase.ToString() + " )");
#endif

            lock (this)
            {
                // @@@@@@@@@@@@@@@@@@@@@@@@@@@@
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Properties_Incoming

        private void _Properties_Incoming(QS.Fx.Value.Classes.IPropertyValues _properties)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationChannel_._Properties_Incoming : \n" + QS.Fx.Printing.Printable.ToString(_properties));
#endif

            foreach (QS.Fx.Value.Classes.IPropertyValue _propertyvalue in _properties.Items)
            {
                // _propertyvalue.Index
            }            
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
