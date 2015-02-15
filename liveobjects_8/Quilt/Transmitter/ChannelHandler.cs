/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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

//#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QS.Fx.Value;
using QS.Fx.Serialization;

/// ChannelHandler is the objects maintained by Transmitter regarding
/// to each ICommunicationChannel reported by EUIDTransport
/// 
/// It is a client of ICommunicationChannel, reporting messages
/// 
namespace Quilt.Transmitter
{
    public sealed class ChannelHandler
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ICommunicationChannel<TransmitterMsg>
    {
        #region Delegates

        // Delegate function to handle received message
        public delegate void MessageHandler(EUIDAddress remote_euid, TransmitterMsg message);

        // Delegate function to handle disconnected channel
        public delegate void ChannelBreakHandler(EUIDAddress remote_euid);

        #endregion

        #region Constructor

        public ChannelHandler(
            QS.Fx.Object.IContext _mycontext,
            EUIDAddress _remote_euid,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<TransmitterMsg>>
            _channel_ref,
            MessageHandler _message_handler,
            ChannelBreakHandler _disconnect_handler)
            :base(_mycontext, true)
        {
            this._mycontext = _mycontext;

            if (_channel_ref != null)
            {
                this._channel_endpt = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<TransmitterMsg>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<TransmitterMsg>>(this);

                this._channel_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);

                _channel_conn = _channel_endpt.Connect(_channel_ref.Dereference(_mycontext).Channel);

                this._message_handler = _message_handler;
                this._disconnect_handler = _disconnect_handler;
            }

            this._remote_euid = _remote_euid;
        }

        #endregion

        #region Field

        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<TransmitterMsg>,
                QS.Fx.Interface.Classes.ICommunicationChannel<TransmitterMsg>> _channel_endpt;

        private EUIDAddress _remote_euid;
        private QS.Fx.Endpoint.IConnection _channel_conn;
        private QS.Fx.Object.IContext _mycontext;

        private MessageHandler _message_handler;
        private ChannelBreakHandler _disconnect_handler;

        //private 

        #endregion

        #region Accessor

        public EUIDAddress RemoteEUID
        {
            get { return _remote_euid; }
            set { _remote_euid = value; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _OnDisconnect

        private void _OnDisconnect()
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.Disconnect)));
        }

        #endregion

        #region Disconnect, callback for _Enqueue

        private void Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            this._disconnect_handler(_remote_euid);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICommunicationChannel<TransmitterMsg> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<TransmitterMsg>.Message(TransmitterMsg message)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<TransmitterMsg>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.Message), message));
        }

        #endregion

        #region Message, callback for _Enqueue

        private void Message(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Transmitter.Transmitter.Message from " + _remote_euid.String);
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<TransmitterMsg> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<TransmitterMsg>)_event;

            TransmitterMsg message = _event_._Object;

            this._message_handler(_remote_euid, message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Send API

        public void Send(TransmitterMsg message)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<TransmitterMsg>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(Outgoing), message));
        }

        #endregion

        #region Outgoing, callback fro _Enqueue

        private void Outgoing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<TransmitterMsg> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<TransmitterMsg>)_event;

            TransmitterMsg message = _event_._Object;

            if (this._channel_endpt.IsConnected)
            {
                this._channel_endpt.Interface.Message(message);
            }
        }

        #endregion

        #region Disconnect API

        public void Disconnect()
        {
            this._channel_endpt.Disconnect();
        }

        #endregion
    }
}
