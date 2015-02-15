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

namespace QS._qss_x_.Uplink_2_
{
/*
    public sealed class Channel_<MessageClass>
        : QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<MessageClass>,
        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constants

        private const int _c_channel_qsm = 1;

        #endregion

        #region Constructor

        public Channel_(Uplink_ _uplink, int _id)
        {
            this._uplink = _uplink;
            this._id = _id;
            this.endpoint = _mycontext.DualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>(this);
            this.endpoint.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);
            this.endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>> endpoint;
        private Uplink_ _uplink;
        private int _id;

        #endregion

        #region ICommunicationChannel<MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>
                QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<MessageClass>.Channel
        {
            get { return this.endpoint; }
        }

        #endregion

        #region _ConnectCallback

        private void _ConnectCallback()
        {
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback()
        {
        }

        #endregion

        #region ICommunicationChannel<MessageClass> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>.Send(MessageClass _message)
        {
            QS.Fx.Serialization.ISerializable _m = (QS.Fx.Serialization.ISerializable) _message;

        }

        #endregion
    }
*/ 
}
