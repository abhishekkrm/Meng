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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CentralizedChannelOverAsbtractTransport_2,
        "CentralizedChannelOverAsbtractTransport_2", "")]
    public sealed class CentralizedChannelOverAsbtractTransport_2<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public CentralizedChannelOverAsbtractTransport_2(
            QS.Fx.Object.IContext _mycontext,
                        [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>>
                    _transport_reference)
        {
            this._mycontext = _mycontext;

            this._transport_reference = _transport_reference;

            this.pending_cc = new List<Communication_channel>();
            this.intialized_cc = new List<Communication_channel>();           

            this.lastmessagereceived = 0;
            this.lastcheckpointreceived = 0;            
            this._noofconnections = 0;
            this.channel_id = 0;

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
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Values_.TransportMessage_1>>(this);

                this._transport_connection = this._transport_endpoint.Connect(this._transport_object.Transport);
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("context")]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable("channel endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channelendpoint;

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

        private List<Communication_channel> pending_cc;
        private List<Communication_channel> intialized_cc;

        private QS.Fx.Base.Address _myaddress; 

        // Queue to hold all the messages received at the server
        private Queue<QS._qss_x_.Values_.TransportMessage_1> _msgqueue = new Queue<QS._qss_x_.Values_.TransportMessage_1>();        

        private int _noofconnections;
        private int channel_id; 
        public int lastmessagereceived;
        public int lastcheckpointreceived;        

        #endregion

        public void receive(MessageClass msg)
        {
            this._channelendpoint.Interface.Receive(msg);
        }

        public CheckpointClass checkpoint()
        {
            return (this._channelendpoint.Interface.Checkpoint());
        }

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._channelendpoint; }
        }

        #endregion

        #region _ChannelConnectCallback

        private void _ChannelConnectCallback()
        {
        }

        #endregion

        #region _ChannelDisconnectCallback

        void Disconnect(Communication_channel comm)
        {            
            lock(this)
            {
                pending_cc.Remove(comm);
                intialized_cc.Remove(comm);
                _noofconnections--;
                if (intialized_cc.Count == 0)
                {                    
                    // clear the msgqueue containing old messages
                    foreach (QS._qss_x_.Values_.TransportMessage_1 msg in _msgqueue)
                    {
                        _msgqueue.Dequeue();
                        lastmessagereceived = 0;
                    }
                    foreach (Communication_channel comm_new in pending_cc)
                    {
                        pending_cc.Remove(comm_new);
                        intialized_cc.Add(comm_new);
                    }
                }
                // checkpoint was requested from this connection but it got disconnected before checkpoint was received from him
                // ask checkpoint from next connection
                if (comm._ckhpointrequested == true && comm._ckhpointrereceived == false)
                {
                    // No initialized connections left to ask checkpoint from and there are still connections that are pending intialization,
                    // so clear the old messages from the queue and move pending connections to initialized connections                    
                    foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm1 in intialized_cc)
                    {
                        _mycontext.Platform.Logger.Log(" going to next connection for checkpoint");
                        CheckpointClass cp = null;
                        QS._qss_x_.Values_.TransportMessage_1 _transport_message = new QS._qss_x_.Values_.TransportMessage_1(cp, -1, 0, true);
                        comm1._connected_channel_endpoint.Interface.Message(_transport_message);
                        comm1._ckhpointrequested = true;
                        comm1._ckhpointrereceived = false;
                        break;
                    }
                }
            }        
        }

        void switch_channel(Communication_channel comm)
        {
            foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm1 in pending_cc)
            {
                if (comm == comm1)
                {                    
                    intialized_cc.Add(comm1);
                }
            }
              
        }
        public void _ChannelDisconnectCallback()
        {
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(
            MessageClass _message)
        {
        }

        #endregion

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

            channel_id++;
            Communication_channel comm = new Communication_channel(_mycontext, _channel, _address, this, channel_id);


            QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>>
                    _somechannel_endpoint =
                        this._mycontext.DualInterface<
                            QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                            QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>>(comm);
            
            _somechannel_endpoint.Connect(_somechannel.Channel);
            comm.setEndpoint(_somechannel_endpoint);

            // create a checkpoint request message
            // -1 in msgID is requesting a checkpoint message
            CheckpointClass cp = null;
            // CheckpointClass cp = checkpoint();            
            QS._qss_x_.Values_.TransportMessage_1 _transport_message = new QS._qss_x_.Values_.TransportMessage_1(cp, -1, 0, true);
            
            foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm1 in intialized_cc)
            {
                // Ask the checkpoint from switch component only if it is the only one connected to the server
                if (comm1._switch_connection == true && intialized_cc.Count == 1)
                {
                    comm1._connected_channel_endpoint.Interface.Message(_transport_message);
                    comm1._ckhpointrequested = true;
                    comm1._ckhpointrereceived = false;                    
                    break;
                }
                else
                {
                    if (comm1._switch_connection == false)
                    {
                        comm1._connected_channel_endpoint.Interface.Message(_transport_message);
                        comm1._ckhpointrequested = true;
                        comm1._ckhpointrereceived = false;                        
                        break;
                    }                    
                }
            }
            lock (this)
            {
                _noofconnections++;

                // first connection
                if (_noofconnections == 1 || intialized_cc.Count == 0)
                {
                    foreach (QS._qss_x_.Values_.TransportMessage_1 msg in _msgqueue)
                    {
                        _msgqueue.Dequeue();
                        lastmessagereceived = 0;
                    }
                    intialized_cc.Add(comm);
                }
                else
                {
                    pending_cc.Add(comm);
                }
            }
        }

        void _somechannel_endpoint_OnDisconnect()
        {
            throw new NotImplementedException();
        }

        #endregion

        class Communication_channel
         : QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>
        {
            public Communication_channel() { }
            public Communication_channel(
                QS.Fx.Object.IContext _mycontext,
                [QS.Fx.Reflection.Parameter("Channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _communicator,
                [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _remote_addr,
                QS._qss_x_.Component_.Classes_.CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass> _parent,
                int identifier)
            {
                this._mycontext = _mycontext;
                this._connection_addr = _remote_addr;  //the remote address to which you are connected
                this._communicator = _communicator;
                this._parent = _parent;
                this._ckhpointrequested = false;
                this._ckhpointrereceived = false;
                this.identifier = identifier;
                this.lastmsgreceived = 0;
                this._switch_connection = false;
            }

            #region Fields
            private QS.Fx.Object.IContext _mycontext;
            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _communicator;
            public QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _connected_channel_endpoint;
            QS.Fx.Base.Address _connection_addr;

            private QS._qss_x_.Component_.Classes_.CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass> _parent;
            public bool _ckhpointrequested;
            public bool _ckhpointrereceived;
            public int identifier;
            private int lastmsgreceived;
            public bool _switch_connection;
            #endregion

            #region ICommunicationChannel<ISerializable> Members
 
            void QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>.Message(QS._qss_x_.Values_.TransportMessage_1 message)
            {
                // message ID of -2 indicates that this connection is from a client connected to switch component
                if (message._msgID == -2)
                {
                    _switch_connection = true;
                }
                else
                {
                    if (message._is_chkpoint)
                    {
                        lock (this)
                        {
                            this._ckhpointrereceived = true;
                            this._ckhpointrequested = false;
                            // send the checkpoint message to the switch component
                            foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm in _parent.intialized_cc)
                            {
                                if (comm._switch_connection == true)
                                {
                                    comm._connected_channel_endpoint.Interface.Message(message);
                                }
                            }
                            // got a checkpoint message, initialize the new connection.
                            int checkpointmsgID = message._msgID;                            
                            // send the checkpoint message and all messages after that to all the pending channels
                            foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm in _parent.pending_cc)
                            {
                                if (!(_parent.intialized_cc.Contains(comm)))
                                {
                                    // send the checkpoint message to all
                                    comm._connected_channel_endpoint.Interface.Message(message);

                                    // and all the messages pending after the checkpoint message (except the switch client)
                                    foreach (QS._qss_x_.Values_.TransportMessage_1 msg in _parent._msgqueue)
                                    {
                                        if (msg._msgID > checkpointmsgID && comm._switch_connection != true)
                                        {
                                            // Indicate that this is a pending message after the checkpoint message
                                            // Switch component will ignore this message based on _pending_msg value
                                            QS._qss_x_.Values_.TransportMessage_1 tempmsg = new QS._qss_x_.Values_.TransportMessage_1(msg.o, msg._msgID, 1, false);
                                            comm._connected_channel_endpoint.Interface.Message(tempmsg);
                                        }
                                    }

                                    // Add the connection in intialized_cc
                                    _parent.intialized_cc.Add(comm);
                                }
                            }
                        }
                    }
                    else
                    {
                        int flag = 0;
                        MessageClass msg = (MessageClass)message.o;

                        int msgID = message._msgID;

                        if (lastmsgreceived >= msgID)
                        {
                            // older (or duplicate) message was received. Discard the message and log the error
                            _parent._mycontext.Platform.Logger.Log("identifier:" + identifier + " Older Message with messageID:" + msgID + " already received. Message discarded");
                        }
                        else if ((lastmsgreceived + 1) < msgID)
                        {
                            // missed 1 or more messages in between the last received message and the current message. Apply the update and indicate in the log
                            _parent._mycontext.Platform.Logger.Log("identifier:" + identifier + " MessageID received:" + msgID + " Expected messageID:" + (lastmsgreceived + 1));
                            lastmsgreceived = msgID;
                            _parent.receive(msg);
                            flag = 1;
                        }
                        else
                        {
                            _parent._mycontext.Platform.Logger.Log("identifier:" + identifier + " Proper MessageID received:" + msgID);
                            lastmsgreceived = msgID;
                            _parent.receive(msg);
                            flag = 1;
                        }

                        if (flag == 1)
                        {
                            QS._qss_x_.Values_.TransportMessage_1 tempMsg;

                            double _t1 = _parent._mycontext.Platform.Clock.Time;
                            _parent._mycontext.Platform.Logger.Log("Going to acquire lock at:" + _t1);
                            lock (this)
                            {
                                double _t2 = _parent._mycontext.Platform.Clock.Time;
                                _parent._mycontext.Platform.Logger.Log("Got lock at:" + _t2 + " time taken (_t2 - _t1):" + (_t2 - _t1));

                                _parent.lastmessagereceived++;
                                tempMsg = message;
                                tempMsg._msgID = _parent.lastmessagereceived;
                                _parent._msgqueue.Enqueue(tempMsg);
                                
                                foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm in _parent.intialized_cc)
                                {
                                    // Remove the connection from the pending list as it has been already initialized
                                    if (_parent.pending_cc.Contains(comm))
                                        _parent.pending_cc.Remove(comm);
                                    if (comm != this || comm._switch_connection != true)
                                    {                                        
                                        // not sending to the connection from which it received
                                        comm._connected_channel_endpoint.Interface.Message(tempMsg);
                                    }
                                }
                            }
                            /*test: int i = 1;
                            foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm in _parent.intialized_cc)
                            {
                                // Remove the connection from the pending list as it has been already initialized
                                if (_parent.pending_cc.Contains(comm))
                                    _parent.pending_cc.Remove(comm);

                                if (i == 1)
                                {
                                    QS._qss_x_.Values_.TransportMessage_1 tempMsg1 = new QS._qss_x_.Values_.TransportMessage_1(message.o, 3, 0, false);
                                
                                    //tempMsg1._msgID = 3;
                                    comm._connected_channel_endpoint.Interface.Message(tempMsg1);
                                    _parent._mycontext.Platform.Logger.Log("sent message with ID 3 to connection 1");
                                }
                                else
                                    comm._connected_channel_endpoint.Interface.Message(tempMsg);
                                i++;
                            }*/
                        }
                        // }
                    }
                }
            }
            #endregion          

            void Disconnect()
            {
                this._parent.Disconnect(this);
            }            

            #region Methods
            public void setEndpoint(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Values_.TransportMessage_1>> _channel_endpoint)
            {
                this._connected_channel_endpoint = _channel_endpoint;
               
                this._connected_channel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(Disconnect);
            }
            #endregion
        }
    }     
}
