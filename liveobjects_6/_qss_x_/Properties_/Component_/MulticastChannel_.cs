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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.MulticastChannel, "Properties Framework Multicast Channel")]
    public sealed class MulticastChannel_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass> 
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>        
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MulticastChannel_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("identifier", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Identifier _identifier,
            [QS.Fx.Reflection.Parameter("delegation", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IProperties>> _delegation_reference,
            [QS.Fx.Reflection.Parameter("batching", QS.Fx.Reflection.ParameterClass.Value)] 
            double _batching,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_.Constructor");
#endif

            this._identifier = _identifier;
            this._index = new QS.Fx.Base.Index(0);
            this._delegation_reference = _delegation_reference;
            this._batching = _batching;
            this._channel_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
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
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel_endpoint;
        [QS.Fx.Base.Inspectable]
        private uint _outgoing_min;
        [QS.Fx.Base.Inspectable]
        private uint _outgoing_max;
        [QS.Fx.Base.Inspectable]
        private Queue<MessageClass> _outgoing = new Queue<MessageClass>();
        [QS.Fx.Base.Inspectable]
        private bool _isoutdated;
        [QS.Fx.Base.Inspectable]
        private bool _isbatching;
        [QS.Fx.Base.Inspectable]
        private double _batching;
        [QS.Fx.Base.Inspectable]
        private double _timestamp;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.Property_ _property_sending;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index _index;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._channel_endpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<MessageClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Send), _message));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                this._client = new PropertiesClient_(_mycontext,
                    this._identifier, this._delegation_reference, new QS._qss_x_.Properties_.Base_.PropertiesCallback_(this._Properties_Incoming), _debug);
                if ((this._platform != null) && (this._client is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._client).Start(this._platform, null);

                this._client._Enable();

                this._Properties_Updated(false);
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Dispose");
#endif

            lock (this)
            {
                if ((this._client != null) && (this._client is IDisposable))
                    ((IDisposable)this._client).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Start");
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
                this._logger.Log("Component_.MulticastChannel_._Stop");
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
                this._logger.Log("Component_.MulticastChannel_._Channel_Connect ");
#endif

            lock (this)
            {
                if (this._channel_endpoint.IsConnected)
                    this._channel_endpoint.Interface.Initialize(null);
            }
        }

        #endregion

        #region _Channel_Disconnect

        private void _Channel_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Channel_Disconnect");
#endif
        }

        #endregion

        #region _Channel_Send

        private void _Channel_Send(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            MessageClass _message = ((QS._qss_x_.Properties_.Base_.IEvent_<MessageClass>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Channel_Send\n\n" + QS.Fx.Printing.Printable.ToString(_message) + "\n\n");
#endif

            lock (this)
            {
                this._outgoing.Enqueue(_message);
                if (this._outgoing_max > 0)
                    this._outgoing_max++;
                else
                    this._outgoing_min = this._outgoing_max = 1;
                this._Properties_Updated(true);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Properties_Connect

        private void _Properties_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Properties_Connect ");
#endif
        }

        #endregion

        #region _Properties_Disconnect

        private void _Properties_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Properties_Disconnect");
#endif
        }

        #endregion

        #region _Properties_Updated

        private void _Properties_Updated(bool _updated)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Properties_Updated ");
#endif

            if (_updated)
                this._isoutdated = true;

            if (this._isoutdated && !this._isbatching)
            {
                double _time = this._platform.Clock.Time;
                if (_time >= this._timestamp + this._batching)
                {
                    this._isoutdated = false;
                    this._timestamp = _time;
                    this._Properties_Outgoing();
                }
                else if (this._platform != null)
                {
                    this._isbatching = true;
                    this._alarm = this._platform.AlarmClock.Schedule
                    (
                        (this._timestamp + this._batching - _time),
                        new QS.Fx.Clock.AlarmCallback
                        (
                            delegate(QS.Fx.Clock.IAlarm _alarm)
                            {
                                if ((_alarm != null) && !_alarm.Cancelled && ReferenceEquals(_alarm, this._alarm))
                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this._Properties_Updated));
                            }
                        ),
                        null
                    );
                }
            }
        }

        private void _Properties_Updated(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            lock (this)
            {
                this._isbatching = false;
                this._Properties_Updated(false);
            }
        }

        #endregion

        #region _Properties_Outgoing

        private void _Properties_Outgoing()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Properties_Outgoing ");
#endif

            this._index = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>) this._index).Incremented;

            this._client._Properties
            (
                new QS.Fx.Value.PropertyValues
                (
                    new QS.Fx.Value.PropertyValue[]
                    {
                        new QS.Fx.Value.PropertyValue
                        (
                            new QS.Fx.Base.Index(1),
                            new QS.Fx.Value.PropertyVersion
                            (
                                new QS.Fx.Base.Incarnation(1),
                                this._index
                            ),
                            null
                        )
                    }
                )
            );
        }

        #endregion

        #region _Properties_Incoming

        private void _Properties_Incoming(QS.Fx.Value.Classes.IPropertyValues _properties)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.MulticastChannel_._Properties_Incoming : \n\n" + QS.Fx.Printing.Printable.ToString(_properties));
#endif

            foreach (QS.Fx.Value.Classes.IPropertyValue _propertyvalue in _properties.Items)
            {
                // _propertyvalue.Index
            }            

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
