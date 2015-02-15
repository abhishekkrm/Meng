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

﻿#define DEBUG_ClientChannel

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Interoperability.Remoting
{
    public class ClientChannel : IChannelSender
    {
        public const string ProtocolName = "quicksilver";

        public ClientChannel(string name, QS.Fx.Logging.ILogger logger, IClientChannelSinkProvider clientSinkProvider,
            Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableCaller> callerCollection)
        {
            this.name = name;
            this.clientSinkProvider = clientSinkProvider;
            this.logger = logger;
            this.callerCollection = callerCollection;
        }

        private QS.Fx.Logging.ILogger logger;
        private IClientChannelSinkProvider clientSinkProvider;
        private string name;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableCaller> callerCollection;

        public static string ParseURL(string url, out string objectURI)
        {
            if (url.StartsWith(ProtocolName))
            {
                int pos = url.IndexOf("/");
                if (pos > 0)
                {
                    objectURI = url.Substring(pos);
                    return url.Substring(ProtocolName.Length + 1, pos - (ProtocolName.Length + 1));
                }
            }

            objectURI = null;
            return null;
        }

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
            return ClientChannel.ParseURL(url, out objectURI);
        }

        #endregion

        #region IChannelSender Members

        public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
        {
            if (url == null)
            {
                IChannelDataStore ds = remoteChannelData as IChannelDataStore;
                if (ds != null)
                    url = ds.ChannelUris[0];
            }

            if (url != null && url.ToLower().StartsWith(ProtocolName + ":"))
            {
                IClientChannelSinkProvider provider = clientSinkProvider;
                while (provider.Next != null)
                    provider = provider.Next;
                provider.Next = new ClientTransportSinkProvider(logger, callerCollection);

                this.Parse(url, out objectURI);

                return (IMessageSink)clientSinkProvider.CreateSink(this, url, remoteChannelData);
            }
            else
            {
                objectURI = null;
                return null;
            }
        }

        #endregion
    }
}
