/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

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
using System.IO;
using System.Net;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Properties_
{
    //[QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TcpTransportClient_eval, "TcpTransportClient")]
    [QS.Fx.Reflection.ComponentClass("3960244B216649e8A0B034E007BE3F62", "AzureTest_1_")]
    public sealed class AzureTest_1_ :
        QS.Fx.Inspection.Inspectable,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
        QS.Fx.Object.Classes.IObject
    {

        public AzureTest_1_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Transport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_object_reference,
            [QS.Fx.Reflection.Parameter("Server Address", QS.Fx.Reflection.ParameterClass.Value)]  
                string _server_addr)
        {
            this._mycontext = _mycontext;

            this._server_addr = _server_addr;

            if (_transport_object_reference != null)
            {
                this._transport_endpt =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>>(this);

                this._transport_conn = this._transport_endpt.Connect(_transport_object_reference.Dereference(_mycontext).Transport);

                this._transport_endpt.Interface.Connect(new QS.Fx.Base.Address(_server_addr));
            }

        }

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>> _transport_endpt;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _transport_conn;
        [QS.Fx.Base.Inspectable]
        private string _transport_addr;
        [QS.Fx.Base.Inspectable]
        private string _server_addr;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel;


        private System.Timers.Timer msgTimer;

        [QS.Fx.Base.Inspectable]
        private int currMsg;



        #endregion

        #region ITransportClient<string,QS.Fx.Value.Classes.IText> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Address(QS.Fx.Base.Address address)
        {
            _transport_addr = address.String;
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, QS.Fx.Value.Classes.IText>.Connected(QS.Fx.Base.Address address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> channel)
        {

            if (_channel == null && address.String == _server_addr)
            {
                currMsg = 1;
                _channel = _mycontext.DualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>(this);

                // add on disconnect event
                _channel.OnDisconnect += new QS.Fx.Base.Callback(_comm_endpt_OnDisconnect);

                QS.Fx.Endpoint.IConnection _comm_conn = _channel.Connect(channel.Dereference(_mycontext).Channel);


                msgTimer = new System.Timers.Timer();
                msgTimer.Elapsed += new ElapsedEventHandler(sendMsg);
                msgTimer.Interval = 1000;
                msgTimer.AutoReset = true;
                msgTimer.Start();

            }
        }

        void _comm_endpt_OnDisconnect()
        {
            msgTimer.Stop();
            msgTimer = null;
            _channel = null;
        }

        #endregion

        private void sendMsg(object source, ElapsedEventArgs e)
        {
            if (_channel != null)
            {
                _channel.Interface.Message(new QS.Fx.Value.UnicodeText("I'm alive! Count: " + currMsg.ToString()));
                currMsg++;
            }
            else
            {
                msgTimer.Stop();
                msgTimer = null;

            }
        }

        #region ICommunicationChannel<string> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {

        }

        #endregion
    }
}
