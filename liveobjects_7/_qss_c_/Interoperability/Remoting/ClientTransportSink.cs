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

﻿// #define DEBUG_ClientTransportSink

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Interoperability.Remoting
{
    public class ClientTransportSink : IClientChannelSink, IChannelSinkBase
    {
        public ClientTransportSink(QS.Fx.Logging.ILogger logger, QS._qss_c_.Base3_.ISerializableCaller caller, string objectURI)
        {
            this.logger = logger;
            this.caller = caller;
            this.objectURI = objectURI;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS._qss_c_.Base3_.ISerializableCaller caller;
        private string objectURI;

        #region IClientChannelSink Members

        public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, System.IO.Stream requestStream, out ITransportHeaders responseHeaders, out System.IO.Stream responseStream)
        {
#if DEBUG_ClientTransportSink
            logger.Log("__ProcessMessage...");
#endif

            string url = (string)msg.Properties["__Uri"];
            string objectURI, baseURL = ClientChannel.ParseURL(url, out objectURI);

            RequestWrapper requestWrapper = new RequestWrapper(objectURI, requestHeaders, requestStream);

#if DEBUG_ClientTransportSink
            logger.Log(this, "Sending to " + caller.Address.ToString() + " object " +requestWrapper.ToString());
#endif

            RequestWrapper wrappedResponse = 
                (RequestWrapper) caller.Call((uint)ReservedObjectID.Interoperability_Remoting_ServerObject, requestWrapper);

#if DEBUG_ClientTransportSink
            logger.Log(this, "Response : " + wrappedResponse.ToString());
#endif

            responseHeaders = wrappedResponse.TransportHeaders;
            responseStream = wrappedResponse.Stream;
        }

        public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, System.IO.Stream stream)
        {
            throw new NotSupportedException();
        }

        public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, System.IO.Stream stream)
        {
            throw new NotSupportedException();
        }

        public System.IO.Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        public IClientChannelSink NextChannelSink
        {
            get { return null; }
        }

        #endregion

        #region IChannelSinkBase Members
        
        public System.Collections.IDictionary Properties
        {
            get { return null; }
        }

        #endregion
    }
}
