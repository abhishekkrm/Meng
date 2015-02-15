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

﻿// #define DEBUG_ClientTransportSinkProvider

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Interoperability.Remoting
{
    public class ClientTransportSinkProvider : IClientChannelSinkProvider
    {
        public ClientTransportSinkProvider(QS.Fx.Logging.ILogger logger, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableCaller> callerCollection)
        {
            this.logger = logger;
            this.callerCollection = callerCollection;
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableCaller> callerCollection;

        #region IClientChannelSinkProvider Members

        IClientChannelSink IClientChannelSinkProvider.CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            string objectURI, baseURL = ClientChannel.ParseURL(url, out objectURI);
            QS.Fx.Network.NetworkAddress serverAddress = new QS.Fx.Network.NetworkAddress(baseURL) ;

#if DEBUG_ClientTransportSinkProvider
            logger.Log(this, "__CreateSink : Address is " + serverAddress.ToString());
#endif

            return new ClientTransportSink(logger, callerCollection[serverAddress], objectURI);
        }

        IClientChannelSinkProvider IClientChannelSinkProvider.Next
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}
