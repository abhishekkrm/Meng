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
using System.Linq;
using System.Text;

namespace QS._qss_x_.Azure_
{
    [QS.Fx.Reflection.ComponentClass("4DFF66264DCE4d4eBC683D18B810D53F", "CCStringMatch_")]
    public class CCStringMatch_ :
        QS._qss_x_.Azure_.Classes_.AzureChannelController_<CCStringMatch_.CCStringMatch_Channel, QS.Fx.Value.Classes.IText>
    {

        public CCStringMatch_(QS.Fx.Object.IContext _mycontext,
                        [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, QS._qss_x_.Azure_.Values_.WorkerMessage_>>
                    _transport_reference,
            [QS.Fx.Reflection.Parameter("server addr", QS.Fx.Reflection.ParameterClass.Value)]
            string _server_addr)
            : base(_mycontext, _transport_reference, _server_addr)
        {

        }

        protected override CCStringMatch_Channel _Channel(QS.Fx.Object.IContext _mycontext, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>> _channel, QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<QS.Fx.Value.Classes.IText>, QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> _channelendpoint)
        {
            return new CCStringMatch_Channel(_mycontext, _channel, _channelendpoint);
        }

        public class CCStringMatch_Channel : QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>
        {

            public CCStringMatch_Channel(QS.Fx.Object.IContext _mycontext, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>> _channel, QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<QS.Fx.Value.Classes.IText>, QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> _channelendpoint)
            {

                this._mycontext = _mycontext;
                this._channelendpoint = _channelendpoint;

                QS.Fx.Object.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_> _somechannel =
                     _channel.Dereference(this._mycontext);

                this._connected_channel_endpoint =
                            this._mycontext.DualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>,
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>>(this);

                this._connected_channel_endpoint.Connect(_somechannel.Channel);

            }

            #region Fields
            private QS.Fx.Object.IContext _mycontext;

            private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>,
                QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>> _connected_channel_endpoint;


            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<QS.Fx.Value.Classes.IText>,
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> _channelendpoint;
            #endregion

            int _count = 0;

            #region ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<QS._qss_x_.Azure_.Values_.WorkerMessage_>.Message(
                QS._qss_x_.Azure_.Values_.WorkerMessage_ _msg)
            {
                lock (this)
                {

                    if (_msg.Ready)
                    {
                        if (_channelendpoint.IsConnected)
                        {
                            _mycontext.Platform.Logger.Log("Relaying 'Ready'");
                            _connected_channel_endpoint.Interface.Message(_msg);
                        }
                        else
                        {
                            if (_count > 10)
                                return;
                            QS.Fx.Value.Classes.IText _text = new QS.Fx.Value.UnicodeText("SOME MSG --- " + _count++);
                            _mycontext.Platform.Logger.Log("Server returning :" + _text.Text);
                            _connected_channel_endpoint.Interface.Message(new QS._qss_x_.Azure_.Values_.WorkerMessage_(_text));
                        }
                    }
                    else
                    {
                        _channelendpoint.Interface.Data((QS.Fx.Value.Classes.IText)_msg.Data);
                    }

                }
            }

            #endregion
        }

    }
}

