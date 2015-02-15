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
   [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Switch,
       "Switch", "")]
    public sealed class Switch<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,       
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Switch(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel1", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>>
                _channel1,
            [QS.Fx.Reflection.Parameter("channel2", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>>
                _channel2)
        {
            this._mycontext = _mycontext;
          
            this.connections = new List<_Connection>();
            this._msgID = 0;           

            if (_channel1 != null)
            {               
                this._channel1 = _channel1;
                _channelendpoint1_OnConnected();               
            }
            if (_channel2 != null)
            {                
                this._channel2 = _channel2;
                _channelendpoint2_OnConnected();               
            }
        }
       
        #endregion

        void _channelendpoint1_OnConnected()
        {
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass> _somechannel =
                 _channel1.Dereference(this._mycontext);
            
            _Connection con = new _Connection(_mycontext, this,0);
            _connection_lastmsgIdbeforecheckpoint[0] = 0;
            QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>>
                _somechannel_endpoint =
                this._mycontext.DualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>>(con);

            _somechannel_endpoint.Connect(_somechannel.Channel);
            con.setEndpoint(_somechannel_endpoint);

            connections.Add(con);
        }

        void _channelendpoint2_OnConnected()
        {
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass> _somechannel =
                 _channel2.Dereference(this._mycontext);

            _Connection con = new _Connection(_mycontext, this,1);
            _connection_lastmsgIdbeforecheckpoint[1] = 0;
            QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>>
                _somechannel_endpoint =
                this._mycontext.DualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>>(con);

            _somechannel_endpoint.Connect(_somechannel.Channel);
            con.setEndpoint(_somechannel_endpoint);

            connections.Add(con);
        }

        #region Fields
        private QS.Fx.Object.IContext _mycontext;
        public List<_Connection> connections;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>> _channel1;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>> _channel2;
        private int _msgID;
        public Queue<QS._qss_x_.Values_.SwitchMessage_> _msgqueue = new Queue<QS._qss_x_.Values_.SwitchMessage_>();        
        public int[] _connection_lastmsgIdbeforecheckpoint = new int[2];
        public CheckpointClass[] _connectionCheckpoints = new CheckpointClass[2];
        #endregion

        public void Initialize(CheckpointClass _checkpoint, int _connectionId)
        {
            _connectionCheckpoints[_connectionId] =  _checkpoint;
        }

        public CheckpointClass Checkpoint(int _connectionId)
        {
            return (_connectionCheckpoints[_connectionId]);
        }

        public void Receive(QS.Fx.Serialization.ISerializable _message, _Connection con1)
        {
            lock (this)
            {
                QS._qss_x_.Values_.SwitchMessage_ msg = (QS._qss_x_.Values_.SwitchMessage_)_message;
                _msgID++;
                msg._msgID = _msgID;            
             
                _msgqueue.Enqueue(msg);
                foreach (_Connection con in connections)
                {
                    if (con != con1 && con._switch != 1)
                    {
                        foreach (QS._qss_x_.Values_.SwitchMessage_ temp_msg in _msgqueue)
                        {
                            if (temp_msg._msgID > _connection_lastmsgIdbeforecheckpoint[con._connectionId])
                            {
                                con.send(temp_msg);
                            }
                        }
                    }
                    if (con._switch != 1)
                        _connection_lastmsgIdbeforecheckpoint[con._connectionId] = _msgID;
                }                
            }
        }

        #region ICheckpointedCommunicationChannel<ISerializable,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>.Channel
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
        #region ICheckpointedCommunicationChannel<ISerializable,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>.Send(QS.Fx.Serialization.ISerializable _message)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region ICheckpointedCommunicationChannelClient<ISerializable,CheckpointClass> Members

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

        public class _Connection             
              : QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>
            {
            public _Connection() { }
            public _Connection(QS.Fx.Object.IContext _mycontext,
                QS._qss_x_.Component_.Classes_.Switch<MessageClass, CheckpointClass> _parent, int _connectionId)
            {
                this._mycontext = _mycontext;
                this._parent = _parent;
                this._lastsequenceno = 0;
                this._switch = 0;                
                this._connectionId = _connectionId;
            }

            #region Fields

            private QS.Fx.Object.IContext _mycontext;            
            public QS.Fx.Endpoint.Internal.IDualInterface<
                 QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
                 QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>>
                     _channelendpoint;

            private QS._qss_x_.Component_.Classes_.Switch<MessageClass, CheckpointClass> _parent;
            private int _lastsequenceno;
            public int _switch;           
            public int _connectionId;
            #endregion

            #region ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable,CheckpointClass> Members

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Receive(QS.Fx.Serialization.ISerializable _message)
            {

                QS._qss_x_.Values_.SwitchMessage_ _switch_message = (QS._qss_x_.Values_.SwitchMessage_)_message;                

                // switch msg (type = 2)
                if (_switch_message._type == 2)
                {
                    lock (this)
                    {
                        _parent._mycontext.Platform.Logger.Log("received switch message setting lastmsgbeforeswitch:" + _parent._msgID);
                        foreach (_Connection con in _parent.connections)
                        {
                            if (con != this && con._switch == 1)
                            {
                                con._switch = 0;
                                // send all the pending messages
                                foreach (QS._qss_x_.Values_.SwitchMessage_ temp_msg in _parent._msgqueue)
                                {
                                    if (temp_msg._msgID > _parent._connection_lastmsgIdbeforecheckpoint[con._connectionId])
                                    {
                                        con.send(temp_msg);
                                    }
                                }
                            }
                        }
                        this._switch = 1;                        
                    }
                }
                else
                {
                    // If message is received from a connection which has sent a switch message earlier, ignore messages from that connection
                    if (_switch != 1)
                    {
                        if (_lastsequenceno >= _switch_message._msgID)
                        {
                            // older (or duplicate) message was received. Discard the message and log the error
                            _parent._mycontext.Platform.Logger.Log("Older Message with messageID:" + _switch_message._msgID + " already received. Message discarded");
                        }
                        else if ((_lastsequenceno + 1) < _switch_message._msgID)
                        {
                            // missed 1 or more messages in between the last received message and the current message. Apply the update and indicate in the log
                            _parent._mycontext.Platform.Logger.Log("MessageID received:" + _switch_message._msgID + " Expected messageID:" + (_lastsequenceno + 1));
                            _lastsequenceno = _switch_message._msgID;
                            this._parent.Receive(_switch_message, this);
                        }
                        else
                        {
                            _parent._mycontext.Platform.Logger.Log("MessageID received:" + _switch_message._msgID);
                            _lastsequenceno = _switch_message._msgID;
                            this._parent.Receive(_switch_message, this);
                        }
                    }
                } 
            }

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
            {
                this._parent.Initialize(_checkpoint, _connectionId);
            }

            CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>.Checkpoint()
            {
                return _parent.Checkpoint(_connectionId);
            }

            #endregion

            public void send(QS.Fx.Serialization.ISerializable _message)
            {               
                _lastsequenceno = _parent._msgID;
                _channelendpoint.Interface.Send(_message);
            }

            #region Methods
            public void setEndpoint(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, CheckpointClass>> _channel_endpoint)
            {
                this._channelendpoint = _channel_endpoint;                
            }
            #endregion
        }
    }
}
