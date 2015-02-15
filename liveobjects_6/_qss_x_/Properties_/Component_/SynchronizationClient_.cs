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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.SynchronizationClient, "Properties Framework Synchronization Client")]
    public sealed class SynchronizationClient_
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ISynchronizationChannelClient
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public SynchronizationClient_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ISynchronizationChannel> _channel_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationClient_.Constructor");
#endif

            if (_channel_reference == null)
                _mycontext.Error("Channel reference cannot be null.");
            this._channel_reference = _channel_reference;

            this._channel_endpoint =
                _mycontext.DualInterface<QS.Fx.Interface.Classes.ISynchronizationChannel, QS.Fx.Interface.Classes.ISynchronizationChannelClient>(this);
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
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ISynchronizationChannel> _channel_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ISynchronizationChannel _channel_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ISynchronizationChannel, QS.Fx.Interface.Classes.ISynchronizationChannelClient> _channel_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channel_connection;
        [QS.Fx.Base.Inspectable]
        private int _phase;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ISynchronizationChannelClient Members

        void QS.Fx.Interface.Classes.ISynchronizationChannelClient.Phase(int _phase)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<int>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Channel_Phase), _phase));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationClient_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                this._channel_object = this._channel_reference.Dereference(_mycontext);
                if (this._channel_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication) this._channel_object).Start(this._platform, null);
                this._channel_connection = this._channel_endpoint.Connect(this._channel_object.Channel);
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.SynchronizationClient_._Dispose");
#endif

            lock (this)
            {
                if (this._channel_connection != null)
                    this._channel_connection.Dispose();
                if (this._channel_endpoint.IsConnected)
                    this._channel_endpoint.Disconnect();
                if ((this._channel_object != null) && (this._channel_object is IDisposable))
                    ((IDisposable) this._channel_object).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.SynchronizationClient_._Start");
#endif

            base._Start();
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.SynchronizationClient_._Stop");
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
                this._logger.Log("Component_.SynchronizationClient_._Channel_Connect ");
#endif
        }

        #endregion

        #region _Channel_Disconnect

        private void _Channel_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationClient_._Channel_Disconnect");
#endif
        }

        #endregion

        #region _Channel_Phase

        private void _Channel_Phase(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            int _phase = ((QS._qss_x_.Properties_.Base_.IEvent_<int>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.SynchronizationClient_._Channel_Phase ( " + _phase.ToString() + " )");
#endif

            lock (this)
            {
                if (_phase < this._phase)
                    _mycontext.Error("Phase received is smaller than the current one.");
                if (_phase > this._phase)
                {
                    this._phase = _phase;
                    if (this._channel_endpoint.IsConnected)
                        this._channel_endpoint.Interface.Ready(this._phase);
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
