/*
 
Copyright (c) 2009 Hari Shreedharan. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Threading;
namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("E8B44D3D50354882AA9E4FECC6BA9235", "Lookup_")]
    public sealed class Lookup_ :
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
        /*  QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,*/
        QS.Fx.Object.Classes.IObject
    {

        public Lookup_(
            QS.Fx.Object.IContext _my_context,
            [QS.Fx.Reflection.Parameter("Transport", QS.Fx.Reflection.ParameterClass.Value)]
           QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_obj,
            [QS.Fx.Reflection.Parameter("Root Address", QS.Fx.Reflection.ParameterClass.Value)]
            string _root_addr)
        {
            this._my_context = _my_context;
            this._hostaddr = new LinkedList<string>();
            if (_root_addr != null && !_root_addr.Equals(""))
                this._root_addr = _root_addr;
            if (_transport_obj != null)
            {
                this._transport_obj = _transport_obj;
                //define transport connected callback
            }
            this._transport_endpt = this._my_context.DualInterface<
           QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
           QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>(this);
            this._transport_conn = this._transport_endpt.Connect(this._transport_obj.Dereference(this._my_context).Transport);
            //       this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._transport_endpt_callback);
            //     this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._transport_endpt_callback);



        }
        #region Fields
        private string _root_addr;
        private LinkedList<string> _hostaddr;
        private LinkedList<string> _incoming_data;
        private QS.Fx.Base.Address _myaddr;
        private QS.Fx.Object.IContext _my_context;
        private QS.Fx.Endpoint.IConnection _transport_conn;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_obj;
        private QS.Fx.Endpoint.Internal.IDualInterface<
           QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
           QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_endpt;



        #endregion

        #region ITransportClient<Address,IText> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Address(QS.Fx.Base.Address address)
        {
            //throw new NotImplementedException();
            this._myaddr = address;
        }



        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> channel)
        {
            Lookup_Communicator comm = new Lookup_Communicator(channel, address, this);
            QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel_endpoint =
                this._my_context.DualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>(comm);
            QS.Fx.Endpoint.IConnection _comm_conn = _channel_endpoint.Connect(channel.Dereference(_my_context).Channel);
            comm.setEndpoint(_channel_endpoint);

            //create a new object who will handle the new connection
        }

        #endregion

        #region Methods
        public string GetRootAddr()
        {
            return this._root_addr;
        }
        public void Connecttoremote(QS.Fx.Base.Address _connect_addr)
        {
            this._transport_endpt.Interface.Connect(_connect_addr);
        }

        public QS.Fx.Base.Address GetMyAddress()
        {
            return this._myaddr;
        }
        #endregion

        /*
        #region ICommunicationChannel<IText> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {
            throw new NotImplementedException();
        }

        #endregion */
    }
    class Lookup_Communicator : QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>, QS.Fx.Object.Classes.IObject
    {
        public Lookup_Communicator(
            [QS.Fx.Reflection.Parameter("Channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _communicator,
            [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _remote_addr,
           Lookup_ _parent)
        {
            // this._connected_channel_endpoint = _connected_channel_endpoint; //get channel endpoint
            this._parent = _parent;  //get the channel so that you can use it if needed -- like when you get info about a new member, 
            //ask parent to contact him using his transport endpoint.
            this._connection_addr = _remote_addr;  //the remote address to which you are connected
            this._communicator = _communicator;
            this._address_received = false;
            this._root_addr = _parent.GetRootAddr();
            this._all_connected_hosts = new LinkedList<string>();
        }


        #region Fields
        //need to maintain list to whom connected.
        // private QS._qss_x_.Properties_.TcpTransport_<QS.Fx.Value.Classes.IText> _transport_obj;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _communicator;
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _connected_channel_endpoint;
        private Lookup_ _parent;
        QS.Fx.Base.Address _connection_addr;
        LinkedList<string> _all_connected_hosts;
        //      private LinkedList<string> _incoming_data = new LinkedList<string>();
        bool _address_received;
        private string _root_addr;

        #endregion


        #region ICommunicationChannel<IText> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {

            if (this._connection_addr.Equals(_root_addr))
            {
                if (message.ToString() != "")
                {
                    if (!this._all_connected_hosts.Contains(message.ToString()))
                    { //check if you already connected to it, if not go ahead
                        if (_parent.GetMyAddress().String != message.ToString())
                            this._parent.Connecttoremote(new QS.Fx.Base.Address(message.ToString()));//assumes list sent by root is current
                        //we can add additional state to keep track of
                        //node failures.
                        this._all_connected_hosts.AddLast(message.ToString());
                    }
                    this._address_received = true;
                }
                else
                {
                    while (_address_received == false)
                    {
                        //Thread.Sleep(10000);
                        this._connected_channel_endpoint.Interface.Message(new QS.Fx.Value.UnicodeText("Send"));
                    }
                }
            }
            else
                _connected_channel_endpoint.Interface.Message(new QS.Fx.Value.UnicodeText("Hello World")); //the connected components will keep sending Hello world to each other, as of now..no condition for them to stop communicating.

        }
        #endregion

        #region Methods
        public void setEndpoint(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel_endpoint)
        {
            this._connected_channel_endpoint = _channel_endpoint;
        }
        #endregion

    }

}
