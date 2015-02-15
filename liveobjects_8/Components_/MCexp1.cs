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
using System.ComponentModel;
using System.Text;
using System.Timers;

namespace QS._qss_x_.Properties_
{
    [QS.Fx.Reflection.ComponentClass("C4052A4D30824919BA252FC0F90803D0", "MCexp1")]
    public sealed class MCexp1
        : QS.Fx.Object.Classes.IObject,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        #region Constructor

        public MCexp1(QS.Fx.Object.IContext _mycontext, [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channel, [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)] int _rate,
            [QS.Fx.Reflection.Parameter("id", QS.Fx.Reflection.ParameterClass.Value)] int _id)
        {

            this._mycontext = _mycontext;
            this._rate = _rate;
            this._id = _id;
            this._currEvt = 0;

            // rate in msgs per second
            this._msgTimer = new Timer();
            this._msgTimer.Elapsed += new ElapsedEventHandler(sendMsg);
            this._msgTimer.Interval = 1000;
            this._msgTimer.AutoReset = true;

            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);
            this._channelendpoint.OnConnected += new QS.Fx.Base.Callback(this.Connected_);
            this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this.Disconnect_);

            if (_channel != null)
                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channel.Dereference(_mycontext).Channel);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channelendpoint;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        private Timer _msgTimer;
        private int _currEvt;
        private int _id;
        private int _rate; // msgs per second
        private QS.Fx.Object.IContext _mycontext;

        #endregion

        private void Disconnect_()
        {
            this._msgTimer.Stop();
        }

        private void Connected_()
        {
            this._msgTimer.Start();
        }

        private void sendMsg(object source, ElapsedEventArgs e) {
            for (int i = 0; i < _rate; i++)
            {
                _channelendpoint.Interface.Send(new QS.Fx.Value.UnicodeText("ID: " + this._id + " | Event #: " + this._currEvt));
                this._currEvt++;
            }
        }

        #region ICheckpointedCommunicationChannelClient<IText,IText> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(QS.Fx.Value.Classes.IText _message)
        {
            return;
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(QS.Fx.Value.Classes.IText _checkpoint)
        {
            return;
        }

        QS.Fx.Value.Classes.IText QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            return new QS.Fx.Value.UnicodeText("ID: "+this._id+" | Event #: "+this._currEvt);
        }

        #endregion
    }
}
