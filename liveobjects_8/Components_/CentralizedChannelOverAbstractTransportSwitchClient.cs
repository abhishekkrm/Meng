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
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CentralizedChannelOverAsbtractTransportSwitchClient,
        "CentralizedChannelOverAsbtractTransportSwitchClient", "")]
    public sealed class CentralizedChannelOverAsbtractTransportSwitchClient<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public CentralizedChannelOverAsbtractTransportSwitchClient(
            QS.Fx.Object.IContext _mycontext,
                        [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>>
                    _transport_reference,
            [QS.Fx.Reflection.Parameter("server_address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _server_address,
            [QS.Fx.Reflection.Parameter("Switch_After", QS.Fx.Reflection.ParameterClass.Value)]
            int SwitchAfter)
        {
            this._mycontext = _mycontext;
            this._server_address = _server_address;
            this._initialized = false;
            this._transport_reference = _transport_reference;
            this._lastsequenceno = 0;
            this._msgID = 0;
            this._switch_after = SwitchAfter;
            this._switched_at = 0;
            this._noofmessagesent = 0;
           // this._prevtime = 0.0;

            this._channelendpoint = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>>(this);
            this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._ChannelConnectCallback);
            this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ChannelDisconnectCallback);

            this._transport_object = this._transport_reference.Dereference(_mycontext);
            if (_transport_object != null)
            {
                this._transport_endpoint =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>>(this);

                this._transport_connection = this._transport_endpoint.Connect(this._transport_object.Transport);
                string ser_add = this._server_address.ToString();
                string address = ser_add.Split('/')[1].Split(':')[0];
                int port = Convert.ToInt32(ser_add.Split('/')[1].Split(':')[1]);
                _mycontext.Platform.Logger.Log("connecting to port:" + port);
                if (address.Equals("0"))
                    this._transport_endpoint.Interface.Connect(new QS.Fx.Base.Address("127.0.0.1", port));
                else
                    this._transport_endpoint.Interface.Connect(new QS.Fx.Base.Address(address, port));          
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("context")]
        public QS.Fx.Object.IContext _mycontext;
        
        [QS.Fx.Base.Inspectable("channel endpoint")]
        public QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>> _channelendpoint;

        [QS.Fx.Base.Inspectable("transport reference")]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>> _transport_reference;

        [QS.Fx.Base.Inspectable("transport object")]
        private QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1> _transport_object;

        [QS.Fx.Base.Inspectable("transport endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>> _transport_endpoint;

        [QS.Fx.Base.Inspectable("transport connection")]
        private QS.Fx.Endpoint.IConnection _transport_connection;

        private QS.Fx.Base.Address _myaddress;
        public QS.Fx.Base.Address _server_address;
        
        private Queue<QS._qss_x_.Values_.TransportMessage_1> _msgqueue = new Queue<QS._qss_x_.Values_.TransportMessage_1>();
        private CentralizedChannelOverAsbtractTransportSwitchClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Communication_channel1<MessageClass, CheckpointClass> comm;
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>>
                    _somechannel_endpoint;
        private bool _initialized;
        public int _lastsequenceno;
        private int _msgID;
        public Thread MyNewThread;
        private int _switch_after;
        private int _switched_at;
        private int _noofmessagesent;
        //private double _prevtime;
        #endregion

        #region ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>>
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>.Channel
        {
            get { return this._channelendpoint; }
        }

        #endregion

        void OnDisconnect()
        {
        }

        public CheckpointClass _checkpoint()
        {
            return (this._channelendpoint.Interface.Checkpoint());
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

        /* Code that can be referred to collect dynamic parameters (like time in below example) for sending the switch message
        public void sendswitch()
        {
            double _current_time = _mycontext.Platform.Clock.Time;
            while (true)
            {
                while ((_current_time - _prevtime) == _switch_after_time)
                {
                    _prevtime = _mycontext.Platform.Clock.Time;
                    MessageClass msg = null;
                    _mycontext.Platform.Logger.Log("_server_address " + _server_address + " sending switch message server:" + _server_address);
                    QS._qss_x_.Values_.SwitchMessage_ _switch_message = new QS._qss_x_.Values_.SwitchMessage_(msg, 0, 2, 0);
                    this._channelendpoint.Interface.Receive(_switch_message);
                    _mycontext.Platform.Logger.Log("Sent switch message from :" + _server_address);
                }
                _current_time = _mycontext.Platform.Clock.Time;
                Thread.Sleep(1);
            } 
        }*/

        public void receive(MessageClass msg, int _msgID)
        {
            // Type
            // 1. Update message
            // 2. Switch message            
            QS._qss_x_.Values_.SwitchMessage_ _message = new QS._qss_x_.Values_.SwitchMessage_(msg, _msgID, 1, 0);
            this._channelendpoint.Interface.Receive(_message);
            _noofmessagesent++;

            if ((_noofmessagesent - _switched_at) == _switch_after)
            {
                _switched_at = _noofmessagesent;
                MessageClass message = null;
                _mycontext.Platform.Logger.Log("_server_address " + _server_address + " sending switch message server:" + _server_address);
                QS._qss_x_.Values_.SwitchMessage_ _switch_message = new QS._qss_x_.Values_.SwitchMessage_(message, 0, 2, 0);
                this._channelendpoint.Interface.Receive(_switch_message);                
             }
        }

        public void initialize(CheckpointClass msg)
        {
            this._channelendpoint.Interface.Initialize(msg);
            this._initialized = true;
        }        

        #region ITransportClient<NetworkAddress,ISerializable> Members

        void QS.Fx.Interface.Classes.ITransportClient<
            QS.Fx.Base.Address,
            QS._qss_x_.Values_.TransportMessage_1>.Address(QS.Fx.Base.Address _address)
        {
            this._myaddress = _address;
        }

        void QS.Fx.Interface.Classes.ITransportClient<
            QS.Fx.Base.Address,
            QS._qss_x_.Values_.TransportMessage_1>.Connected(
                QS.Fx.Base.Address _address,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _channel)
        {
            QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1> _somechannel =
                 _channel.Dereference(this._mycontext);

            comm = new CentralizedChannelOverAsbtractTransportSwitchClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Communication_channel1<MessageClass, CheckpointClass>(_channel, _address, this);

            _somechannel_endpoint =
                this._mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>>(comm);
            _somechannel_endpoint.OnConnected += new QS.Fx.Base.Callback(_somechannel_endpoint_OnConnected);
            _somechannel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(_somechannel_endpoint_OnDisconnect);
            _somechannel_endpoint.Connect(_somechannel.Channel);
            comm.setEndpoint(_somechannel_endpoint);

            // send a message to server indicating that the connection is from a swicth client
            // -2 indicates that it is message from switch connection
            QS._qss_x_.Values_.TransportMessage_1 switch_connection_msg = new QS._qss_x_.Values_.TransportMessage_1(null, -2, 0, false);
            _somechannel_endpoint.Interface.Message(switch_connection_msg);

            for (int i = 0; i < _msgqueue.Count; i++)
            {
                QS._qss_x_.Values_.TransportMessage_1 temp_msg = _msgqueue.Dequeue();
                _somechannel_endpoint.Interface.Message(temp_msg);
                _msgqueue.Enqueue(temp_msg);
            }

            // Thread code to collect the dynamic parameters to send switch message
            //this.MyNewThread = new Thread(new ThreadStart(sendswitch));
            //MyNewThread.Start();
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
        #region ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Receive(QS.Fx.Serialization.ISerializable _message)
        {
            throw new NotImplementedException();
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
        {
            throw new NotImplementedException();
        }

        CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Checkpoint()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>.Send(QS.Fx.Serialization.ISerializable _message)
        {
            int flag = 0;
            QS._qss_x_.Values_.SwitchMessage_ msg = (QS._qss_x_.Values_.SwitchMessage_)_message;            
            if (_lastsequenceno >= msg._msgID)
            {
                // older (or duplicate) message was received. Discard the message and log the error
                _mycontext.Platform.Logger.Log("Older Message with messageID:" + msg._msgID + " already received. Message discarded");
            }
            else if ((_lastsequenceno + 1) < msg._msgID)
            {
                // missed 1 or more messages in between the last received message and the current message. Apply the update and indicate in the log
                _mycontext.Platform.Logger.Log("MessageID received:" + msg._msgID + " Expected messageID:" + (_lastsequenceno + 1));
                _lastsequenceno = msg._msgID;
                flag = 1;                
            }
            else
            {
                _mycontext.Platform.Logger.Log("MessageID received:" + msg._msgID);
                _lastsequenceno = msg._msgID;
                flag = 1;                
            }

            if (flag == 1)
            {                
                _msgID++;                
                QS._qss_x_.Values_.TransportMessage_1 _transport_message = new QS._qss_x_.Values_.TransportMessage_1(msg._o, _msgID, 0, false);
                _msgqueue.Enqueue(_transport_message);

                comm._connected_channel_endpoint.Interface.Message(_transport_message);            
            }
        }

        #endregion

        class Communication_channel1<
                [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
                [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
                 : QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>
            where MessageClass : class, QS.Fx.Serialization.ISerializable
            where CheckpointClass : class, QS.Fx.Serialization.ISerializable
        {
            public Communication_channel1(
                 [QS.Fx.Reflection.Parameter("Channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _communicator,
                [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _remote_addr,
                QS._qss_x_.Component_.Classes_.CentralizedChannelOverAsbtractTransportSwitchClient<MessageClass, CheckpointClass> _parent)
            {
                this._connection_addr = _remote_addr;  //the remote address to which you are connected
                this._communicator = _communicator;
                this._parent = _parent;
                this._lastsequenceno = 0;
            }
            #region Fields

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _communicator;
            public QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _connected_channel_endpoint;
            QS.Fx.Base.Address _connection_addr;
            private QS._qss_x_.Component_.Classes_.CentralizedChannelOverAsbtractTransportSwitchClient<MessageClass, CheckpointClass> _parent;
            private int _lastsequenceno;
            #endregion

            #region ICommunicationChannel<ISerializable> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>.Message(QS._qss_x_.Values_.TransportMessage_1 message)
            {
                if (message._is_chkpoint == true && message._msgID != -1)
                {
                    // Its a checkpoint message, intialize your state
                    CheckpointClass msg = (CheckpointClass)message.o;
                    _parent._lastsequenceno = message._msgID;                    
                    _parent.initialize(msg);
                }
                else if (message._is_chkpoint == true && message._msgID == -1)
                {
                    // got request for a checkpoint message
                    CheckpointClass msg = _parent._checkpoint();                    
                    QS._qss_x_.Values_.TransportMessage_1 _transport_message = new QS._qss_x_.Values_.TransportMessage_1(msg, _parent._lastsequenceno, 0, true);
                    //test:  QS._qss_x_.Values_.TransportMessage_1 _transport_message = new QS._qss_x_.Values_.TransportMessage_1(msg, 1, true);
                    this._connected_channel_endpoint.Interface.Message(_transport_message);
                }
                else
                {
                    // _pending_msg = 1 indicates that the message received is the pending message after the checkpoint. This component can ignore 
                    // this message.                    
                    if (message._pending_msg != 1)
                    {
                        MessageClass msg = (MessageClass)message.o;                        
                        if (_lastsequenceno >= message._msgID)
                        {
                            // older (or duplicate) message was received. Discard the message and log the error
                            _parent._mycontext.Platform.Logger.Log("Older Message with messageID:" + message._msgID + " already received. Message discarded");
                        }
                        else if ((_lastsequenceno + 1) < message._msgID)
                        {
                            // missed 1 or more messages in between the last received message and the current message. Apply the update and indicate in the log
                            _parent._mycontext.Platform.Logger.Log("MessageID received:" + message._msgID + " Expected messageID:" + (_parent._lastsequenceno + 1));
                            _lastsequenceno = message._msgID;
                            _parent.receive(msg, message._msgID);
                        }
                        else
                        {
                            _parent._mycontext.Platform.Logger.Log("MessageID received:" + message._msgID);
                            _lastsequenceno = message._msgID;
                            _parent.receive(msg, message._msgID);
                        }
                    }
                }
            }
            #endregion

            #region Methods
            public void setEndpoint(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _channel_endpoint)
            {
                this._connected_channel_endpoint = _channel_endpoint;
            }
            #endregion
        }
    }   
}
