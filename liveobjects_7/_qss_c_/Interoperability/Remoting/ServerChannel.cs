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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;

namespace QS._qss_c_.Interoperability.Remoting
{
    public class ServerChannel : IChannelReceiver, IChannel
    {
        public ServerChannel(string name, QS.Fx.Logging.ILogger logger, IServerChannelSinkProvider serverSinkProvider,
            QS.Fx.Network.NetworkAddress localAddress, Base3_.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            this.serverSinkProvider = serverSinkProvider;
            this.name = name;
            this.localAddress = localAddress;

            this.channelData = new ChannelDataStore(new string[] { this.GetURLBase() });

            IServerChannelSinkProvider provider = serverSinkProvider;
            while (provider != null)
            {
                provider.GetChannelData(channelData);
                provider = provider.Next;
            }

            transportSink = new ServerTransportSink(logger, ChannelServices.CreateServerChannelSinkChain(serverSinkProvider, this), demultiplexer);

            this.StartListening(null);
        }

        private IServerChannelSinkProvider serverSinkProvider;
        private ServerTransportSink transportSink;

        private QS.Fx.Logging.ILogger logger;
        private string name;
        private QS.Fx.Network.NetworkAddress localAddress;

        private string GetURLBase()
        {
            return ClientChannel.ProtocolName + ":" + localAddress.ToString();
        }

        private ChannelDataStore channelData;

        #region IChannel Members

        public string ChannelName
        {
            get { return name ; }
        }

        public int ChannelPriority
        {
            get { return 0; }
        }

        public string Parse(string url, out string objectURI)
        {
            return ClientChannel.ParseURL(url, out objectURI);
        }

        #endregion

        #region IChannelReceiver Members

        public object ChannelData
        {
            get { return channelData; }
        }

        public string[] GetUrlsForUri(string objectURI)
        {
            if (!(objectURI.StartsWith("/")))
                objectURI = "/" + objectURI;

            return new string[] { this.GetURLBase() + objectURI };
        }

        public void StartListening(object data)
        {
        }

        public void StopListening(object data)
        {
        }

        #endregion
    }
}
