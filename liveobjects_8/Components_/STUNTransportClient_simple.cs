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
using System.Text;
using System.IO;
using System.Net;
using System.Timers;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Properties_
{
    [QS.Fx.Reflection.ComponentClass("5483B8543E6D4dbf8BD8BCDE2E580DF5", "STUNTransportClient_simple")]
    public sealed class STUNTransportClient_simple
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>
    {

        public STUNTransportClient_simple(
            QS.Fx.Object.IContext _mycontext, 
           [QS.Fx.Reflection.Parameter("STUNTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>>
                    _transport_object_reference, [QS.Fx.Reflection.Parameter("PartnerAddress", QS.Fx.Reflection.ParameterClass.Value)]  string _test_partner_addr)
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNTransportClient_simple.Constructor");
#endif

            this._mycontext = _mycontext;

            if (_test_partner_addr != null && _test_partner_addr!="")
            {
                this._test_partner_addr = _test_partner_addr;
            }

            if (_transport_object_reference != null)
            {
                this._transport_endpt =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>>(this);

                this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._Transport_ConnectedCallback);
                this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._Transport_DisconnectCallback);

                this._transport_conn = this._transport_endpt.Connect(_transport_object_reference.Dereference(_mycontext).Transport);

            }
           
        }

        #region _Transport_ConnectedCallback

        private void _Transport_ConnectedCallback()
        {
            int a = 1;
        }

        #endregion

        #region _Transport_DisconnectCallback

        private void _Transport_DisconnectCallback()
        {
        }
        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>> _transport_endpt;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _transport_conn;
        
        [QS.Fx.Base.Inspectable]
        private string _transport_addr;

        [QS.Fx.Base.Inspectable]
        private string _test_partner_addr;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> _channel;
        
        [QS.Fx.Base.Inspectable]
        private List<string> _incoming = new List<string>();

        [QS.Fx.Base.Inspectable]
        private System.Timers.Timer msgTimer;

        [QS.Fx.Base.Inspectable]
        private bool timerCreate = false;

        [QS.Fx.Base.Inspectable]
        private int currMsg;

        [QS.Fx.Base.Inspectable]
        private int currRcvMsg;

        [QS.Fx.Base.Inspectable]
        private int timerRun;

        #endregion

        #region ITransportClient<QS.Fx.Value.STUNAddress,QS.Fx.Value.Classes.IText> Members

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>.Address(QS.Fx.Value.STUNAddress address)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.STUNTransportClient_simple.Address " + address.String);
#endif

            _transport_addr = address.String;

            if (this._test_partner_addr != null)
            {
                this._transport_endpt.Interface.Connect(new QS.Fx.Value.STUNAddress(this._test_partner_addr));
            }
        }

        void QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Value.STUNAddress, QS.Fx.Value.Classes.IText>.Connected(QS.Fx.Value.STUNAddress address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>> channel)
        {

            if(_channel==null) 
            {
                currMsg = 1;
                _channel = _mycontext.DualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>,
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>>(this);

                // add on disconnect event
                _channel.OnDisconnect += new QS.Fx.Base.Callback(_comm_endpt_OnDisconnect);

                QS.Fx.Endpoint.IConnection _comm_conn = _channel.Connect(channel.Dereference(_mycontext).Channel);

                if (_incoming.Count == 0)
                {
                    _incoming.Add("test msg");
                }
                else
                {
                    throw new Exception("incoming queue not empty");
                }

                try
                {

                    if (msgTimer == null)
                    {
                        timerRun = 0;
                        msgTimer = new System.Timers.Timer();
                        msgTimer.Elapsed += new ElapsedEventHandler(sendMsg);
                        msgTimer.Interval = 1000;
                        msgTimer.AutoReset = true;
                        msgTimer.Start();
                    }
                    //if (timerCreate == false)
                    //{
                    //    _mycontext.Platform.AlarmClock.Schedule(25, new QS.Fx.Clock.AlarmCallback(sendMsg), _mycontext);
                    //    timerCreate = true;
                    //    //_mycontext.Platform.
                    //}
                    else
                    {
                        throw new Exception("msgTimer ought to be null");
                    }
                }
                catch (Exception exc)
                {
                }
            }
            else if(_channel != null && address.String == _test_partner_addr) 
            {
                throw new Exception("Client got connected to same channel twice!");
                // been connected to same channel twice,.
                // failed assertion?
            }
        }

        void _comm_endpt_OnDisconnect()
        {
            msgTimer.Stop();
            msgTimer = null;
            _channel = null;
            _incoming.Clear();

            // only for test
            throw new Exception("Channel Disconnect.\r\n");
        }

        #endregion

        #region sendMsg

        private void sendMsg(object source, ElapsedEventArgs e)
        //private void sendMsg(QS.Fx.Clock.IAlarm alarm)
        {
            try
            {

                //alarm.
                timerRun++;

                if (_channel != null)
                {
                    //test
                    //_mycontext.Platform.AlarmClock.Schedule(25, new QS.Fx.Clock.AlarmCallback(sendMsg), _mycontext);

                    _channel.Interface.Message(new QS.Fx.Value.UnicodeText(currMsg.ToString()));
                    currMsg++;
                }
                else
                {
                    msgTimer.Stop();
                    msgTimer = null;
                    //timerCreate = false;
                }
            }
            catch (Exception exc)
            {
            }

        }

        #endregion

        #region ICommunicationChannel<string> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Value.Classes.IText>.Message(QS.Fx.Value.Classes.IText message)
        {
            lock (_incoming)
            {

                //Debug.Assert(Convert.ToInt32(message.Text) == Convert.ToInt32(_incoming[_incoming.Count - 1]) + 1);
                _incoming.Add(message.Text);

#if VERBOSE

                if (this._logger != null)
                    this._logger.Log("STUNTransportClient_simple.Channel.Message " + message.Text);
#endif

                currRcvMsg = _incoming.Count;
            }
        }

        #endregion
    }
}
