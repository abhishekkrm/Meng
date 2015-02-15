/*

Copyright (c) 2009 Chuck Sakoda. All rights reserved.

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

namespace QS._qss_x_.Azure_.Classes_
{
    
    public abstract class AzureChannelController_<
        CCClass,
         DataClass>
         : QS.Fx.Inspection.Inspectable,
            QS._qss_x_.Azure_.Object_.IAzureWorkerChannel_<DataClass>,
            QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>
        where CCClass : class, QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>
        where DataClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public AzureChannelController_(
            QS.Fx.Object.IContext _mycontext,
                        
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>>
                    _transport_reference,
            
            string _server_addr)
        {
            this._mycontext = _mycontext;


            this._connections = new List<CCClass>();


            this._channelendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>,
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_>(this);
            //this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._ChannelConnectCallback);
            //this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ChannelDisconnectCallback);


            this._transport_object = _transport_reference.Dereference(_mycontext);
            this._transport_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>,
                    QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>>(this);

            this._transport_connection = this._transport_endpoint.Connect(this._transport_object.Transport);
            if (_server_addr != null)
            {
                this._transport_endpoint.Interface.Connect(new QS.Fx.Base.Address(_server_addr));
            }
        }

        #endregion

        #region Fields
        [QS.Fx.Base.Inspectable]
        private IList<CCClass> _connections;
        [QS.Fx.Base.Inspectable("context")]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable("transport object")]
        private QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_> _transport_object;
        [QS.Fx.Base.Inspectable("transport endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>> _transport_endpoint;
        [QS.Fx.Base.Inspectable("transport connection")]
        private QS.Fx.Endpoint.IConnection _transport_connection;

        private QS.Fx.Base.Address _myaddress;

        [QS.Fx.Base.Inspectable("channel endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>,
            QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> _channelendpoint;



        #endregion

        #region _ChannelDisconnectCallback

        //void Disconnect(Communication_channel comm)
        //{
        //    lock (this)
        //    {
        //        pending_cc.Remove(comm);
        //        intialized_cc.Remove(comm);
        //        _noofconnections--;
        //        if (intialized_cc.Count == 0)
        //        {
        //            // clear the msgqueue containing old messages
        //            foreach (QS._qss_x_.Values_.TransportMessage_1 msg in _msgqueue)
        //            {
        //                _msgqueue.Dequeue();
        //                lastmessagereceived = 0;
        //            }
        //            foreach (Communication_channel comm_new in pending_cc)
        //            {
        //                pending_cc.Remove(comm_new);
        //                intialized_cc.Add(comm_new);
        //            }
        //        }
        //        // checkpoint was requested from this connection but it got disconnected before checkpoint was received from him
        //        // ask checkpoint from next connection
        //        if (comm._ckhpointrequested == true && comm._ckhpointrereceived == false)
        //        {
        //            // No initialized connections left to ask checkpoint from and there are still connections that are pending intialization,
        //            // so clear the old messages from the queue and move pending connections to initialized connections                    
        //            foreach (CentralizedChannelOverAsbtractTransport_2<MessageClass, CheckpointClass>.Communication_channel comm1 in intialized_cc)
        //            {
        //                _mycontext.Platform.Logger.Log(" going to next connection for checkpoint");
        //                CheckpointClass cp = null;
        //                QS._qss_x_.Values_.TransportMessage_1 _transport_message = new QS._qss_x_.Values_.TransportMessage_1(cp, -1, 0, true);
        //                comm1._connected_channel_endpoint.Interface.Message(_transport_message);
        //                comm1._ckhpointrequested = true;
        //                comm1._ckhpointrereceived = false;
        //                break;
        //            }
        //        }
        //    }
        //}

        #endregion

        #region ITransportClient<NetworkAddress,ISerializable> Members

        void QS.Fx.Interface.Classes.ITransportClient<
            QS.Fx.Base.Address,
            QS._qss_x_.Azure_.Values_.WorkerMessage_>.Address(QS.Fx.Base.Address _address)
        {
            this._myaddress = _address;
        }

        void QS.Fx.Interface.Classes.ITransportClient<
            QS.Fx.Base.Address,
            QS._qss_x_.Azure_.Values_.WorkerMessage_>.Connected(
                QS.Fx.Base.Address _address,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>> _channel)
        {

            CCClass comm = _Channel(this._mycontext, _channel, _channelendpoint);


            lock (this._connections)
            {
                this._connections.Add(comm);
            }
            if (_send_ready)
            {
                _send_ready = false;
                ((QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_)this).Ready();
            }

        }


        #endregion

        protected abstract CCClass _Channel(QS.Fx.Object.IContext _mycontext, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>> _channel, QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>, QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> _channelendpoint);

        private bool _send_ready = false;

        #region IAzureWorkerChannel_ Members

        void QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_.Ready()
        {
            if(this._connections.Count==0) {
                _send_ready = true;
                return;
            }
            if (this._connections.Count != 1)
                throw new Exception("Need to have exactly one connection");

            _mycontext.Platform.Logger.Log("Sending a ready");
            _connections[0].Message(new QS._qss_x_.Azure_.Values_.WorkerMessage_(true));
        }

        #endregion

        #region IAzureWorkerChannel_<DataClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>, QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> QS._qss_x_.Azure_.Object_.IAzureWorkerChannel_<DataClass>._Channel
        {
            get { return this._channelendpoint; }
        }

        #endregion
    }
}
