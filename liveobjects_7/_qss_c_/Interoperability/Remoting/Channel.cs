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

namespace QS._qss_c_.Interoperability.Remoting
{
    public class Channel : IChannelSender, IChannelReceiver
    {
        public Channel(string name, QS.Fx.Logging.ILogger logger, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableCaller> callerCollection, 
            QS.Fx.Network.NetworkAddress localAddress, Base3_.IDemultiplexer demultiplexer)
        {
            clientChannel = new ClientChannel(name, logger, new SoapClientFormatterSinkProvider(), callerCollection);
            serverChannel = new ServerChannel(name, logger, new SoapServerFormatterSinkProvider(), localAddress, demultiplexer);
            this.name = name;
        }

        private ClientChannel clientChannel;
        private ServerChannel serverChannel;
        private string name;

        #region IChannelSender Members

        public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
        {
            return clientChannel.CreateMessageSink(url, remoteChannelData, out objectURI);
        }

        #endregion

        #region IChannel Members

        public string ChannelName
        {
            get { return name; }
        }

        public int ChannelPriority
        {
            get { return 0; }
        }

        public string Parse(string url, out string objectURI)
        {
            return clientChannel.Parse(url, out objectURI);
        }

        #endregion

        #region IChannelReceiver Members

        public object ChannelData
        {
            get { return (serverChannel != null) ? serverChannel.ChannelData : null; }
        }

        public string[] GetUrlsForUri(string objectURI)
        {
            return (serverChannel != null) ? serverChannel.GetUrlsForUri(objectURI) : null;            
        }

        public void StartListening(object data)
        {
            if (serverChannel != null)
                serverChannel.StartListening(data);
        }

        public void StopListening(object data)
        {
            if (serverChannel != null)
                serverChannel.StopListening(data);
        }

        #endregion
    }
}
