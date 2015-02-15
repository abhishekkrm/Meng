/* Copyright (c) 2004-2009 Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
SUCH DAMAGE. */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Timers;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Demo_.Component_
{

    [QS.Fx.Reflection.ComponentClass("68D18401389145da83417EDB5657F31F", "TransportDemo_")]
    public sealed class TransportDemo_ :
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
        QS.Fx.Object.Classes.IObject
    {


        public TransportDemo_(QS.Fx.Object.IContext _mycontext,
           [QS.Fx.Reflection.Parameter("Transport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>
                    _transport_object_reference,
            [QS.Fx.Reflection.Parameter("Is Server?", QS.Fx.Reflection.ParameterClass.Value)]
            bool _is_server)
        {
            this._is_server = _is_server;
            this._mycontext = _mycontext;

            this._transport_endpt =
                     _mycontext.DualInterface<
                         QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
                         QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>(this);

            _transport_proxy = _transport_object_reference.Dereference(_mycontext);


            this._transport_endpt.Connect(_transport_proxy.Transport);

            if(!_is_server)
                this._transport_endpt.Interface.Connect(new QS.Fx.Base.Address(IPAddress.Parse("127.0.0.1"), 42424));

        }




        private bool _is_server;
        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText> _transport_proxy;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_endpt;

        private QS.Fx.Endpoint.Internal.IDualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel;

        #region ITransportClient<Address,IText> Members

        private QS.Fx.Base.Address _address;

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Address(QS.Fx.Base.Address address)
        {
            this._address = address;
        }

        private QS.Fx.Base.Address _peer_address;

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> channel)
        {
            this._peer_address = address;
            _channel = _mycontext.DualInterface<
                                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>(this);
            _channel.Connect(channel.Dereference(_mycontext).Channel);

            if(!_is_server)
                _channel.Interface.Message(new QS.Fx.Value.UnicodeText("0"));
        }

        #endregion

        #region ICommunicationChannel<IText> Members

        private int _message_index = 0;

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {
            if (message.Text == _message_index.ToString())
            {
                if (_is_server)
                {
                    _mycontext.Platform.Logger.Log("Server got: "+message.Text);
                    _channel.Interface.Message(message);
                    _message_index++;
                    
                }
                else
                {
                    _mycontext.Platform.Logger.Log("Client got: "+message.Text);
                    _channel.Interface.Message(new QS.Fx.Value.UnicodeText((++_message_index).ToString()));
                }
            }
        }

        #endregion
    }
}
