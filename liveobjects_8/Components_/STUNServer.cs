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
    [QS.Fx.Reflection.ComponentClass("931BABE755AE4f5e91C83BA40A4BF53D", "STUNServer")]
    public sealed class STUNServer
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>
    {

        #region Constructor

        public STUNServer(
            QS.Fx.Object.IContext _mycontext,
           [QS.Fx.Reflection.Parameter("UDPTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>>
                    _transport_object_reference)
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNServer.Constructor");
#endif
            this._mycontext = _mycontext;

            this._channels = new Dictionary<string, UdpChannelClient>();

            if (_transport_object_reference != null)
            {
                this._transport_endpt =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>>(this);

                this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._Transport_ConnectedCallback);
                this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._Transport_DisconnectCallback);

                this._transport_conn = this._transport_endpt.Connect(_transport_object_reference.Dereference(_mycontext).Transport);

            }

        }

        #endregion

        #region _Transport_ConnectedCallback

        private void _Transport_ConnectedCallback()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNServer.Transport Connect");
#endif
        }

        #endregion

        #region _Transport_DisconnectCallback

        private void _Transport_DisconnectCallback()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNServer.Transport Disconnect");
#endif
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Address _address;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>> _transport_endpt;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _transport_conn;

        [QS.Fx.Base.Inspectable]
        public Dictionary<string, UdpChannelClient> _channels;

        #endregion

        #region ITransportClient<Address,BounceMessage> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>.Address(QS.Fx.Base.Address address)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNServer.Transport Address" + address.String);
#endif
            this._address = address;
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.BounceMessage>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>> channel)
        {
            try
            {
                lock (this._channels)
                {
                    UdpChannelClient udp_channel;
                    if (!this._channels.TryGetValue(address.String, out udp_channel))
                    {
                        udp_channel = new UdpChannelClient(_mycontext, this, address.String, channel);
                        _channels.Add(address.String, udp_channel);
                    }
                    else
                    {
                        throw new Exception("Existed channel connected!!");
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Exception("STUNServer.Connected Exception: " + exc.Message);
            }
        }

        #endregion

        #region UdpChannelClient

        public class UdpChannelClient
            : QS._qss_x_.Properties_.Component_.Base_,
            QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>
        {
            #region Constructor

            public UdpChannelClient(
                QS.Fx.Object.IContext _mycontext,
                STUNServer _transport,
                string _address,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>> _channel)
                : base(_mycontext, true)
            {
                this._mycontext = _mycontext;

                this._transport = _transport;
                this._address = _address;
                this._channel_endpt = _mycontext.DualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>,
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>>(this);

                this._channel_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._OnDisconnect);

                this._connection = _channel_endpt.Connect(_channel.Dereference(_mycontext).Channel);
            }

            #endregion

            #region Fields

            public string _address;
            public QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>> _channel_endpt;
            public STUNServer _transport;
            QS.Fx.Endpoint.IConnection _connection;
            Queue<QS.Fx.Value.BounceMessage> _incoming = new Queue<QS.Fx.Value.BounceMessage>();

            #endregion

            #region _OnDisconnect

            private void _OnDisconnect()
            {
                lock (this._transport._channels)
                {
                    this._transport._channels.Remove(this._address);
                    _channel_endpt = null;
                }
            }

            #endregion

            #region ICommunicationChannel<QS.Fx.Value.BounceMessage> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>.Message(QS.Fx.Value.BounceMessage message)
            {
#if VERBOSE
                //if (this._logger != null)
                //    this._logger.Log("Component_.STUNServer.UdpChannelClient Receive Message");
#endif
                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.BounceMessage>(new QS._qss_x_.Properties_.Base_.EventCallback_(Incoming), message));
            }

            #endregion

            #region ICommunicationChannel<QS.Fx.Value.BounceMessage> Members

            QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>, QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>> QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.BounceMessage>.Channel
            {
                get { return this._channel_endpt; }
            }

            #endregion

            #region Incoming

            private void Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
                QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.BounceMessage> _event_
                    = (QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.BounceMessage>)_event;

                QS.Fx.Value.BounceMessage message = _event_._Object;

                lock (this._incoming)
                {
                    this._incoming.Enqueue(message);

                    while (this._incoming.Count > 0)
                    {
                        Incoming();
                    }
                }
            }

            private void Incoming()
            {
                QS.Fx.Value.BounceMessage message = this._incoming.Dequeue();

                switch (message.TYPE)
                {
                    case QS.Fx.Value.BounceMessage.Type.TEST:
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.STUNServer.UdpChannelClient Test Message" + this._address);
#endif

                            string pri_addr = message.Addr.PriAddr;
                            string pub_addr = this._address;
                            QS.Fx.Value.STUNAddress stun_addr = new QS.Fx.Value.STUNAddress(pub_addr + "/" + pri_addr);

                            QS.Fx.Value.BounceMessage bounce = new QS.Fx.Value.BounceMessage(QS.Fx.Value.BounceMessage.Type.BOUNCE, stun_addr);
                            this._channel_endpt.Interface.Message(bounce);
                        }
                        break;

                    case QS.Fx.Value.BounceMessage.Type.ALIVE:
                        {
#if VERBOSE
                            //if (this._logger != null)
                            //    this._logger.Log("Component_.STUNServer.UdpChannelClient Alive Message" + this._address + message.Addr.String);
#endif

                            // TODO maintain the channel dict
                        }
                        break;

                    case QS.Fx.Value.BounceMessage.Type.ACTIVE:
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.STUNServer.UdpChannelClient Active Message from " + message.Addr.String + " to " + message.TargetAddr.String);
#endif
                            string target_pub_addr = message.TargetAddr.PubAddr;

                            lock (this._transport._channels)
                            {
                                UdpChannelClient target_channel;
                                if (this._transport._channels.TryGetValue(target_pub_addr, out target_channel))
                                {
                                    target_channel.Outgoing(message);
                                }
                            }
                        }
                        break;

                    case QS.Fx.Value.BounceMessage.Type.PASSIVE:
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("Component_.STUNServer.UdpChannelClient Passive Message from " + message.Addr.String + " to " + message.TargetAddr.String);
#endif
                            string target_pub_addr = message.TargetAddr.PubAddr;

                            lock (this._transport._channels)
                            {
                                UdpChannelClient target_channel;
                                if (this._transport._channels.TryGetValue(target_pub_addr, out target_channel))
                                {
                                    target_channel.Outgoing(message);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            #endregion

            #region Outgoing

            public void Outgoing(QS.Fx.Value.BounceMessage message)
            {
                this._channel_endpt.Interface.Message(message);
            }

            #endregion
        }

        #endregion
    }
}
