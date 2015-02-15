/*

Copyright (c) 2009 Rinku Agarwal. All rights reserved.

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

namespace QS._qss_x_.Component_.Classes_
{


    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CentralizedChannelOverAsbtractTransportClient1_,
        "CentralizedChannelOverAsbtractTransportClient", "")]
    public sealed class CentralizedChannelOverAsbtractTransportClient1_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {




        #region Constructor

        public CentralizedChannelOverAsbtractTransportClient1_(
            QS.Fx.Object.IContext _mycontext,
                        [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>>
                    _transport_reference,
            [QS.Fx.Reflection.Parameter("server_address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _server_address)
        {

            this._mycontext = _mycontext;
            this._server_address = _server_address;

            this._transport_reference = _transport_reference;

            this._channelendpoint = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
            this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._ChannelConnectCallback);
            this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ChannelDisconnectCallback);

            this._transport_object = this._transport_reference.Dereference(_mycontext);
            if (_transport_object != null)
            {
                this._transport_endpoint =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>>(this);

                this._transport_connection = this._transport_endpoint.Connect(this._transport_object.Transport);
                string ser_add = this._server_address.ToString();
                string address = ser_add.Split('/')[1].Split(':')[0];
                int port = Convert.ToInt32(ser_add.Split('/')[1].Split(':')[1]);
                if (address.Equals("0"))                
                    this._transport_endpoint.Interface.Connect(new QS.Fx.Base.Address("127.0.0.1", port));
                else
                    this._transport_endpoint.Interface.Connect(new QS.Fx.Base.Address(address, port));               
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("context")]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable("channel endpoint")]
        public QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channelendpoint;

        [QS.Fx.Base.Inspectable("transport reference")]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>> _transport_reference;

        [QS.Fx.Base.Inspectable("transport object")]
        private QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_> _transport_object;

        [QS.Fx.Base.Inspectable("transport endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_>> _transport_endpoint;

        [QS.Fx.Base.Inspectable("transport connection")]
        private QS.Fx.Endpoint.IConnection _transport_connection;

        [QS.Fx.Base.Inspectable]

        private QS.Fx.Base.Address _myaddress;
        private QS.Fx.Base.Address _server_address;

        // your own data structures
        private Queue<QS._qss_x_.Values_.TransportMessage_> _msgqueue = new Queue<QS._qss_x_.Values_.TransportMessage_>();
        private CentralizedChannelOverAsbtractTransportClient1_<MessageClass, CheckpointClass>.Communication_channel1<MessageClass, CheckpointClass> comm;
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>>
                    _somechannel_endpoint;

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._channelendpoint; }
        }

        #endregion

        void OnDisconnect()
        {
        }

        #region _ChannelConnectCallback

        private void _ChannelConnectCallback()
        {
        }

        #endregion

        #region _ChannelDisconnectCallback

        private void _ChannelDisconnectCallback()
        {
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(
            MessageClass _message)
        {
            QS._qss_x_.Values_.TransportMessage_ _transport_message = new QS._qss_x_.Values_.TransportMessage_(_message, false);
            _msgqueue.Enqueue(_transport_message);
            _somechannel_endpoint.Interface.Message(_transport_message);
        }

        public void receive(MessageClass msg)
        {
            this._channelendpoint.Interface.Receive(msg);
        }

        public void initialize(CheckpointClass msg)
        {
            this._channelendpoint.Interface.Initialize(msg);
        }

        #endregion

        #region ITransportClient<NetworkAddress,ISerializable> Members

        void QS.Fx.Interface.Classes.ITransportClient<
            QS.Fx.Base.Address,
            QS._qss_x_.Values_.TransportMessage_>.Address(QS.Fx.Base.Address _address)
        {
            this._myaddress = _address;
        }

        void QS.Fx.Interface.Classes.ITransportClient<
            QS.Fx.Base.Address,
            QS._qss_x_.Values_.TransportMessage_>.Connected(
                QS.Fx.Base.Address _address,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>> _channel)
        {
            QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_> _somechannel =
                 _channel.Dereference(this._mycontext);

            comm = new CentralizedChannelOverAsbtractTransportClient1_<MessageClass, CheckpointClass>.Communication_channel1<MessageClass, CheckpointClass>(_channel, _address, this);

            _somechannel_endpoint =
                this._mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>>(comm);
            _somechannel_endpoint.OnConnected += new QS.Fx.Base.Callback(_somechannel_endpoint_OnConnected);
            _somechannel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(_somechannel_endpoint_OnDisconnect);
            _somechannel_endpoint.Connect(_somechannel.Channel);
            comm.setEndpoint(_somechannel_endpoint);

            for (int i = 0; i < _msgqueue.Count; i++)
            {
                QS._qss_x_.Values_.TransportMessage_ temp_msg = _msgqueue.Dequeue();
                _somechannel_endpoint.Interface.Message(temp_msg);
                _msgqueue.Enqueue(temp_msg);
            }
        }

        void _somechannel_endpoint_OnDisconnect()
        {
            //throw new NotImplementedException();
        }

        void _somechannel_endpoint_OnConnected()
        {
            //throw new NotImplementedException();
        }
        #endregion

        class Communication_channel1<
                [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
                [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
                 : QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>
            where MessageClass : class, QS.Fx.Serialization.ISerializable
            where CheckpointClass : class, QS.Fx.Serialization.ISerializable
        {
            public Communication_channel1(
                 [QS.Fx.Reflection.Parameter("Channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>> _communicator,
                [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _remote_addr,
                QS._qss_x_.Component_.Classes_.CentralizedChannelOverAsbtractTransportClient1_<MessageClass, CheckpointClass> _parent)
            {
                this._connection_addr = _remote_addr;  //the remote address to which you are connected
                this._communicator = _communicator;
                this._parent = _parent;
            }
            #region Fields

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>> _communicator;
            private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>> _connected_channel_endpoint;
            QS.Fx.Base.Address _connection_addr;
            private QS._qss_x_.Component_.Classes_.CentralizedChannelOverAsbtractTransportClient1_<MessageClass, CheckpointClass> _parent;
            #endregion

            #region ICommunicationChannel<ISerializable> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>.Message(QS._qss_x_.Values_.TransportMessage_ message)
            {
                if (message._is_chkpoint)
                {
                    CheckpointClass msg = (CheckpointClass)message.o;
                    _parent.initialize(msg);
                }
                else
                {
                    MessageClass msg = (MessageClass)message.o;
                    _parent.receive(msg);
                }
            }
            #endregion

            #region Methods
            public void setEndpoint(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_>> _channel_endpoint)
            {
                this._connected_channel_endpoint = _channel_endpoint;
            }
            #endregion
        }
    }
}
