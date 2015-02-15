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
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Properties_
{
    //[QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TcpTransportClient_eval, "TcpTransportClient")]
    [QS.Fx.Reflection.ComponentClass("2CC1CC18342942f4834795ECFD3709A2", "AzureTest_2_")]
    public sealed class AzureTest_2_ :
        QS.Fx.Object.Classes.IObject
    {

        public AzureTest_2_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Server Address", QS.Fx.Reflection.ParameterClass.Value)]  
                string _server_addr)
        {
            this._mycontext = _mycontext;

            this._server_addr = _server_addr;

            _c = new TcpClient();
            string[] _split = _server_addr.Split(':');
            _c.Connect(new IPEndPoint(IPAddress.Parse(_split[0]), Convert.ToInt32(_split[1])));
            string _msg = "CONNECTED";
            Array.Clear(_buf, 0, _buf.Length);
            Array.Copy(ASCIIEncoding.ASCII.GetBytes(_msg), _buf, ASCIIEncoding.ASCII.GetByteCount(_msg));
            _s = _c.GetStream();
            _s.Write(_buf, 0, _buf.Length);

            currMsg = 1;
            msgTimer = new System.Timers.Timer();
            msgTimer.Elapsed += new ElapsedEventHandler(sendMsg);
            msgTimer.Interval = 1000;
            msgTimer.AutoReset = true;
            msgTimer.Start();
            
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

        void _comm_endpt_OnDisconnect()
        {
            msgTimer.Stop();
            msgTimer = null;
            _channel = null;
        }

        TcpClient _c;
        NetworkStream _s;
        byte[] _buf = new byte[1024];
        private void sendMsg(object source, ElapsedEventArgs e)
        {
            lock (_s)
            {
                string _msg = "I'm alive! Count: " + currMsg.ToString();
                Array.Clear(_buf, 0, _buf.Length);
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_msg), _buf, ASCIIEncoding.ASCII.GetByteCount(_msg));

                _s.Write(_buf, 0, _buf.Length);

                currMsg++;
            }
        }

    }
}
