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
using System.Linq;
using System.Text;

using QS.Fx.Value;
using QS.Fx.Serialization;

/// Transmitter is the network module residing in Quilt taking care 
/// of network communication.
/// 
/// It is a client of EUIDTransport, maintaining all channels inside,
/// while helping sending out message and dispatching received messages
/// to upper modules
/// 
namespace Quilt.Transmitter
{
    public class Transmitter :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, TransmitterMsg>
    {
        #region Delegates

        // Delegate function to handle received message
        public delegate void UpperMessageHandler(EUIDAddress remote_euid, ISerializable message);

        #endregion

        #region Constructor

        public Transmitter(
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>>
            _transport_object_reference,
            UpperMessageHandler 
            _upper_message_handler)
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Transmitter.Transmitter constructor");
#endif

            this._mycontext = _mycontext;

            // Connect EUIDTransport
            this._transport_endpt = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ITransport<EUIDAddress, TransmitterMsg>,
                QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, TransmitterMsg>>(this);

            this._transport_endpt.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
            this._transport_endpt.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });

            this._transport_conn = this._transport_endpt.Connect(_transport_object_reference.Dereference(_mycontext).Transport);

            // Initialize containers
            this._channel_dict = new Dictionary<string, ChannelHandler>();
            this._waiting_dict = new Dictionary<string, Queue<ISerializable>>();
            this._outgoing_queue = new Queue<Tuple_<EUIDAddress, ISerializable>>();
            this._incoming_queue = new Queue<Tuple_<EUIDAddress, ISerializable>>();

            this._message_handler = MessageHandler;
            this._channel_disconnect_handler = ChannelBreakHandler;

            this._upper_message_handler = _upper_message_handler;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<EUIDAddress, TransmitterMsg>,
            QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, TransmitterMsg>> _transport_endpt;

        private QS.Fx.Endpoint.IConnection _transport_conn;

        private EUIDAddress _self_euid;

        private Dictionary<string, ChannelHandler> _channel_dict;
        private Dictionary<string, Queue<ISerializable>> _waiting_dict;
        private Queue<Tuple_<EUIDAddress, ISerializable>> _outgoing_queue;
        private Queue<Tuple_<EUIDAddress, ISerializable>> _incoming_queue;

        private ChannelHandler.MessageHandler _message_handler;
        private ChannelHandler.ChannelBreakHandler _channel_disconnect_handler;

        private UpperMessageHandler _upper_message_handler;

        #endregion

#if VERBOSE

        // Debug code for logging transmitter message flow
        private System.IO.StreamWriter _peer_log = null;
        public void SetLogTarget(System.IO.StreamWriter _peer_log)
        {
            this._peer_log = _peer_log;
        }
#endif

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect for EUIDTransport

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Transmitter.Transmitter EUIDTransport Endpoint Connect");
#endif

        }

        #endregion

        #region _Disconnect for EUIDTransport

        private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Transmitter.Transmitter EUIDTransport Endpoint Disconnect");
#endif
            // Clear channel dictionary
            this._channel_dict.Clear();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransportClient<EUIDAddress,TransmitterMsg> Members

        void QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, TransmitterMsg>.Address(EUIDAddress address)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Quilt.Transmitter.Transmitter.ITransportClient.Address" + address.String);
#endif
            this._self_euid = address;
            MessageHandler(_self_euid, null);
        }

        void QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, TransmitterMsg>.Connected(EUIDAddress address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<TransmitterMsg>> channel)
        {
          
            try
            {
                if (!_channel_dict.ContainsKey(address.String))
                {
                    ChannelHandler channel_handler = new ChannelHandler(_mycontext, address, channel, _message_handler, _channel_disconnect_handler);
                    _channel_dict.Add(address.String, channel_handler);

#if VERBOSE
                    //if (this._logger != null)
                    //    this._logger.Log("Quilt.Transmitter.Transmitter.ITransportClient.Connected new " + address.String);
#endif  

                    if (_waiting_dict.ContainsKey(address.String))
                    {
                        // Need to clear the waiting messages
                        this._Enqueue(
                            new QS._qss_x_.Properties_.Base_.Event_<EUIDAddress>(
                            new QS._qss_x_.Properties_.Base_.EventCallback_(this.ClearMessage), address));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Quilt.Transmitter. ITransportClient.Connected function, dictionary add exception: " + e.Message);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Message Process

        private void MessageProcess(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            // Processing messages
            while (_incoming_queue.Count > 0)
            {
                Tuple_<EUIDAddress, ISerializable> tuple = _incoming_queue.Dequeue();

                // Call Upper layer process functions
                _upper_message_handler(tuple.x, tuple.y);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MessageHandler and ChannelBreakHandler for ChannelHandler

        // These two functions would be called by Enqueueed messages

        private void MessageHandler(EUIDAddress remote_euid, TransmitterMsg message)
        {
            bool isProcessing = _incoming_queue.Count > 0;

            if (message == null)
            {
                _incoming_queue.Enqueue(new Tuple_<EUIDAddress, ISerializable>(remote_euid, null, 0));
            }
            else if (remote_euid.IsFull())
            {
                _incoming_queue.Enqueue(new Tuple_<EUIDAddress, ISerializable>(remote_euid, message.Message, 0));
            }
            else
            {

                EUIDAddress new_remote_euid = ((IEUIDable)message).GetEUID();

#if VERBOSE
                //if (this._logger != null)
                //    this._logger.Log("Quilt.Transmitter.Transmitter.Update EUID " + remote_euid.String + " to " + new_remote_euid.String);
#endif

                ///
                /// If the message comes from a incoming connection
                /// need to fix the remote EUID
                /// 
                if (new_remote_euid.IsFull())
                {
                    // Change the key of channel_dict
                    ChannelHandler new_channel;
                    if (_channel_dict.TryGetValue(remote_euid.String, out new_channel))
                    {
                        bool isReplacable = true;

                        ChannelHandler duplicated_channel;
                        // Check if there a duplicated outgoing connection to remote host
                        if (_channel_dict.TryGetValue(new_remote_euid.String, out duplicated_channel))
                        {
                            // Keep the connection starting from a smaller EUID, just for coherent selection
                            if (((IComparable<EUIDAddress>)_self_euid).CompareTo(new_remote_euid) <= 0)
                            {
                                isReplacable = false;
                            }
                            else
                            {
                                // Remove the duplicated channel
                                duplicated_channel.Disconnect();
                            }
                        }

                        if (isReplacable)
                        {
                            // Set the ChannelHandler
                            new_channel.RemoteEUID = new_remote_euid;

                            // Set the _channel_dict
                            _channel_dict[new_remote_euid.String] = new_channel;
                            _channel_dict.Remove(remote_euid.String);
                        }
                        else
                        {
                            // Keep the old one
                            new_channel.Disconnect();
                        }
                    }
                    _incoming_queue.Enqueue(new Tuple_<EUIDAddress, ISerializable>(new_remote_euid, message.Message, 0));
                }
            }
            

            if (!isProcessing)
            {
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.MessageProcess)));

            }
        }

        private void ChannelBreakHandler(EUIDAddress remote_euid)
        {
#if VERBOSE

            if (this._logger != null)
                this._logger.Log("Component_.Transmitter.ChannelBreakHandler Channel removed " + remote_euid.String);

#endif
            // Remove the channel from dictionary
            _channel_dict.Remove(remote_euid.String);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region SendMessage API

        public void SendMessage(EUIDAddress remote_euid, ISerializable message)
        {
            bool isSending = this._outgoing_queue.Count > 0;
            _outgoing_queue.Enqueue(new Tuple_<EUIDAddress, ISerializable>(remote_euid, message, 0));

            if (!isSending)
            {
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.SendMessage)));

            }
        }

        #endregion

        #region SendMessage, called by _Enqueue

        private void SendMessage(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            while (_outgoing_queue.Count > 0)
            {
                Tuple_<EUIDAddress, ISerializable> tuple = _outgoing_queue.Dequeue();
                EUIDAddress remote_euid = tuple.x;
                ISerializable message = tuple.y;

                ChannelHandler channel_handler;
                if (_channel_dict.TryGetValue(remote_euid.String, out channel_handler))
                {
#if VERBOSE
                    //if (this._logger != null)
                        //this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " Quilt.Transmitter.Transmitter.SendMessage begin");

                    if (_peer_log != null)
                    {
                        double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        _peer_log.WriteLine(cur + "\tTransmitter\t" + message.SerializableInfo.Size + "\t" + message.SerializableInfo.ClassID + "\t" + "\tPushed To Channel");
                        _peer_log.Flush();
                    }
#endif

                    // Wrap message with self EUID
                    TransmitterMsg tran_msg = new TransmitterMsg(this._self_euid, message);
                    channel_handler.Send(tran_msg);

#if VERBOSE
                    //if (this._logger != null)
                    //    this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " Quilt.Transmitter.Transmitter.SendMessage end");
#endif
                }
                else
                {
#if VERBOSE
                    //if (this._logger != null)
                    //    this._logger.Log("Quilt.Transmitter.Transmitter.SendMessage new channel for " + remote_euid.String);
#endif
                    // Set up waiting queue
                    Queue<ISerializable> waiting_queue;
                    if (!_waiting_dict.TryGetValue(remote_euid.String, out waiting_queue))
                    {
                        waiting_queue = new Queue<ISerializable>();
                        _waiting_dict.Add(remote_euid.String, waiting_queue);

                        // Connect remote end-host
                        _transport_endpt.Interface.Connect(remote_euid);
                    }
                    waiting_queue.Enqueue(message);
                   
                }
            }
        }

        #endregion

        #region Clear wating messages, called by _Enqueue

        private void ClearMessage(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<EUIDAddress> _event_ = (QS._qss_x_.Properties_.Base_.IEvent_<EUIDAddress>)_event;
            EUIDAddress remote_euid = _event_._Object;
            Queue<ISerializable> waiting_queue = _waiting_dict[remote_euid.String];
            ChannelHandler channel_handler = _channel_dict[remote_euid.String];

            while (waiting_queue.Count > 0)
            {
                // Wrap message with self EUID
                ISerializable message = waiting_queue.Dequeue();
                channel_handler.Send(new TransmitterMsg(_self_euid, message));

#if VERBOSE
                //if (this._logger != null)
                //this._logger.Log(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " Quilt.Transmitter.Transmitter.SendMessage begin");

                if (_peer_log != null)
                {
                    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    _peer_log.WriteLine(cur + "\tTransmitter\t" + message.SerializableInfo.Size + "\t" + message.SerializableInfo.ClassID + "\t" + "\tPushed To Channel");
                    _peer_log.Flush();
                }
#endif
            }

            _waiting_dict.Remove(remote_euid.String);
        }

        #endregion
    }
}
