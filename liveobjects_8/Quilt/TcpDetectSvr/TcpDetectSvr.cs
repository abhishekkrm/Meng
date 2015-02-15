/*

Copyright (c) 2004-2009 Bo Peng. All rights reserved.

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
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Quilt.TcpDetectSvr
{
    [QS.Fx.Reflection.ComponentClass("1B9B6771FCDB4e1bA486409A71790C3D", "TcpDetectSvr")]
    public sealed class TcpDetectSvr:
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>
    {
        #region constructor

        public TcpDetectSvr(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>
                    _transport_object_reference)
            : base(_mycontext, true)
        {
            //should be tcptransport2
            this._mycontext = _mycontext;

            if (_transport_object_reference != null)
            {
                //construct transport endpoint
                this._transport_endpt =
                    this._mycontext.DualInterface<
                             QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
                             QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>(this);

                // To be connected or disconnected as EUIDTransport
                // Set callback
                this._transport_endpt.OnConnected +=
                    new QS.Fx.Base.Callback(
                        delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
                this._transport_endpt.OnDisconnect +=
                    new QS.Fx.Base.Callback(
                        delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });
                
                //connect to the transport object from this transport client object
                this._transport_endpt.Connect(_transport_object_reference.Dereference(this._mycontext).Transport);
            }
        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region public fields

        static int _remote_port = 3479;

        #endregion

        #region private fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
                    QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
                    QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_endpt;

        private QS.Fx.Object.IContext _mycontext;

        private QS.Fx.Base.Address _listen_addr;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.TcpDetectSvr.TcpDetectSvr Endpoint Connect");
#endif

        }

        #endregion

        #region _Disconnect

        private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.TcpDetectSvr.TcpDetectSvr Endpoint Disconnect");
#endif

        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransportClient<Address,IText> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Address(QS.Fx.Base.Address address)
        {
            //once listening is done, ITransport will call Address(), notify you the address that ITransport is listening for new connections
            this._listen_addr = address;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.TcpDetectSvr.Address: " + _listen_addr.String);
#endif
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> channel)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.TcpDetectSvr.Connected Connection Set up to/from: " + address.String);
#endif
            if (address.String.Split(':')[1] == _remote_port.ToString())
            {
                // This is a conection started from remote host
                // need to connect back as a test
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Address>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this.ConnectBack), address));
                
            }
            else
            {
                // This is a connection to remote host
                // Dispose the channel (connect and then disconnect might work)
            }
        }

        #endregion

        private void ConnectBack(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.TcpDetectSvr.TcpDetectSvr ConnectBack");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Address> _event_ = (QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Address>)_event;
            _transport_endpt.Interface.Connect(_event_._Object);
        }
    }

    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    public sealed class MyChannel:
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>
    {
        #region constructor

        public MyChannel(QS.Fx.Object.IContext _mycontext,
                 QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>
                 _channel_ref):base(_mycontext, true)
        {
            this._mycontext = _mycontext;

            if (_channel_ref != null)
            {
                this._channel_endpt = this._mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>(this);

                this._channel_endpt.Connect(_channel_ref.Dereference(this._mycontext).Channel);
            }
        }

        #endregion

        #region private fields

        private QS.Fx.Object.IContext _mycontext;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel_endpt;

        #endregion

        #region ICommunicationChannel<IText> Members

        void  QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {
 	        //get a message from client, do nothing
        } 

        #endregion
    }
}
