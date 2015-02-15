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
using System.IO;
using System.Net;
using System.Timers;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Properties_
{
    //[QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TcpTransportClient_eval, "TcpTransportClient")]
    [QS.Fx.Reflection.ComponentClass("71B5A167EC924E8F930C1926223507B5", "TcpTransportClient_eval")]
    public sealed class TcpTransportClient_eval_ :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>,
        QS.Fx.Object.Classes.IObject
    {

        public TcpTransportClient_eval_(
            QS.Fx.Object.IContext _mycontext,
           [QS.Fx.Reflection.Parameter("Transport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>>
                    _transport_object_reference, [QS.Fx.Reflection.Parameter("PartnerAddress", QS.Fx.Reflection.ParameterClass.Value)]  string _test_partner_addr)
            :base(_mycontext, true)
        {
            this._mycontext = _mycontext;

            if (_test_partner_addr != null && _test_partner_addr != "")
            {
                this._test_partner_addr = _test_partner_addr;
            }

            if (_transport_object_reference != null)
            {
                this._transport_endpt =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>>(this);

                this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._Transport_ConnectedCallback);
                this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._Transport_DisconnectCallback);

                this._transport_conn = this._transport_endpt.Connect(_transport_object_reference.Dereference(_mycontext).Transport);

                if (_test_partner_addr != null && _test_partner_addr != "")
                {
                    QS.Fx.Base.Address _partner_addr = new QS.Fx.Base.Address(_test_partner_addr);
                    this._transport_endpt.Interface.Connect(_partner_addr);
                }
            }
            currMsg = 0;


        }

        #region _Transport_ConnectedCallback

        private void _Transport_ConnectedCallback()
        {
        }

        #endregion

        #region _Transport_DisconnectCallback

        private void _Transport_DisconnectCallback()
        {
        }
        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>> _transport_endpt;
        private QS.Fx.Endpoint.IConnection _transport_conn;
        private string _transport_addr;
        private string _test_partner_addr;


        public IDictionary<string, TcpChannelClient>
                            _channels = new Dictionary<string, TcpChannelClient>();

        private Queue<string> _incoming = new Queue<string>();


        private System.Timers.Timer msgTimer;
        private int currMsg;

        #endregion

        #region ITransportClient<QS.Fx.Base.Address,string> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>.Address(QS.Fx.Base.Address address)
        {
            _transport_addr = address.String;
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Serialization.ISerializable>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> channel)
        {
            if (!_channels.ContainsKey(address.String))
            {
                TcpChannelClient _channel = new TcpChannelClient(_mycontext, this, address.String, channel);

                if (msgTimer == null)
                {
                    msgTimer = new System.Timers.Timer();
                    msgTimer.Elapsed += new ElapsedEventHandler(sendMsg);
                    msgTimer.Interval = 25;
                    msgTimer.AutoReset = true;
                    msgTimer.Start();
                }
            }
            else
            {
                throw new Exception("Connected to same channel twice!");
                // been connected to same channel twice,.
                // failed asseration?
            }
        }
        #endregion

        public class TcpChannelClient :
            QS._qss_x_.Properties_.Component_.Base_,
            QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>
        {
            public TcpChannelClient(
                QS.Fx.Object.IContext _mycontext,
                TcpTransportClient_eval_ _transport,
                string _address,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _channel)
                :base(_mycontext, true)
            {
                this._transport = _transport;
                this._address = _address;
                this._tcp_endpt = _mycontext.DualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>>(this);

                this._tcp_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);

                this._connection = _tcp_endpt.Connect(_channel.Dereference(_mycontext).Channel);
                lock (this._transport._channels)
                {
                    this._transport._channels.Add(_address, this);
                }
            }

            public string _address;
            public QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> _tcp_endpt;
            public TcpTransportClient_eval_ _transport;
            QS.Fx.Endpoint.IConnection _connection;
            Queue<QS.Fx.Serialization.ISerializable> _incoming = new Queue<QS.Fx.Serialization.ISerializable>();

            private void _OnDisconnect()
            {
                lock (this._transport._channels)
                {
                    if (this._transport._channels.Count == 1)
                    {
                        this._transport.msgTimer.Stop();
                        this._transport.msgTimer = null;
                    }
                    this._transport._channels.Remove(this._address);
                    _tcp_endpt = null;
                }
            }

            void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>.Message(QS.Fx.Serialization.ISerializable message)
            {
                lock (this._incoming)
                {
                    // Not store it
                    //this._incoming.Enqueue(message);
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Received message " + _incoming.Count);
#endif
                }
            }



            #region ICommunicationChannel<string> Members

            QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>, QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>> QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Serialization.ISerializable>.Channel
            {
                get { return this._tcp_endpt; }
            }

            #endregion
        }

        private void sendMsg(object source, ElapsedEventArgs e)
        {


            bool sent = false;
            lock (_channels)
            {
                foreach (KeyValuePair<string, TcpChannelClient> kvp in _channels)
                {
                    sent = true;
                    kvp.Value._tcp_endpt.Interface.Message(new QS.Fx.Value.UnicodeText(currMsg.ToString()));
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Send message " + currMsg);
#endif
                }
                if (sent)
                    currMsg++;
            }


        }

    }
}
