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

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("2BE65E44690C4f5c864ACDF9AEBD2936", "Lookup_Root")]
    public sealed class Lookup_Root :
           QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
        /*  QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,*/
           QS.Fx.Object.Classes.IObject
    {

        public Lookup_Root(
            QS.Fx.Object.IContext _my_context,
            [QS.Fx.Reflection.Parameter("Transport", QS.Fx.Reflection.ParameterClass.Value)]
           QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_obj /* ,
          [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)] 
        QS.Fx.Base.Address _myaddr*/
                                     )
        {
            this._my_context = _my_context;
            this._transport_obj = _transport_obj;
            this._transport_endpt = this._my_context.DualInterface<
              QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
              QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>(this);
            this._transport_conn = this._transport_endpt.Connect(this._transport_obj.Dereference(this._my_context).Transport);
            this._addresses = new LinkedList<string>();
            this._addresses.AddLast("");
            this._myaddr = _myaddr;



        }
        #region Fields
        private QS.Fx.Object.IContext _my_context;
        private QS.Fx.Endpoint.IConnection _transport_conn;
        private QS.Fx.Base.Address _myaddr;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_obj;
        private QS.Fx.Endpoint.Internal.IDualInterface<
           QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
           QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_endpt;
        private LinkedList<string> _addresses;   //string which has addresses of clients

        #endregion

        #region ITransportClient<Address,IText> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Address(QS.Fx.Base.Address address)
        {

            //throw new NotImplementedException();
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> channel)
        {
            this._addresses.AddLast(address.String);
            Root_Communicator comm = new Root_Communicator(channel, address, this);
            QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel_endpoint =
                this._my_context.DualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>(comm);
            QS.Fx.Endpoint.IConnection _comm_conn = _channel_endpoint.Connect(channel.Dereference(_my_context).Channel);
            comm.setEndpoint(_channel_endpoint);
            // throw new NotImplementedException();
        }

        #endregion
        /*
        #region ICommunicationChannel<IText> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {
            
            //   throw new NotImplementedException();
        }

        #endregion */

        #region Methods

        public LinkedList<string> GetAddress()
        {
            return this._addresses;
        }

        #endregion
    }
    class Root_Communicator : QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>, QS.Fx.Object.Classes.IObject
    {
        public Root_Communicator(
            [QS.Fx.Reflection.Parameter("Channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _communicator,
            [QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _remote_addr,
           Lookup_Root _parent)
        {
            this._parent = _parent;
            this._connection_addr = _remote_addr;


        }

        #region Fields
        //need to maintain list to whom connected.
        // private QS._qss_x_.Properties_.TcpTransport_<QS.Fx.Value.Classes.IText> _transport_obj;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _communicator;
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _connected_channel_endpoint;
        private Lookup_Root _parent;
        QS.Fx.Base.Address _connection_addr;
        LinkedList<string> _all_connected_hosts;


        #endregion

        #region Methods
        public void setEndpoint(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
           QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel_endpoint)
        {
            this._connected_channel_endpoint = _channel_endpoint;
        }
        #endregion


        #region ICommunicationChannel<IText> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {
            // throw new NotImplementedException();
            this._all_connected_hosts = _parent.GetAddress();
            foreach (string tempadd in _all_connected_hosts)
            {
                this._connected_channel_endpoint.Interface.Message(new QS.Fx.Value.UnicodeText(tempadd));
            }
        }

        #endregion
    }
}
