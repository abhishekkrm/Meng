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

﻿// #define DEBUG_ServerTransportSink

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Interoperability.Remoting
{
    public class ServerTransportSink : IServerChannelSink
    {
        public ServerTransportSink(QS.Fx.Logging.ILogger logger, IServerChannelSink nextSink, Base3_.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            this.nextSink = nextSink;

#if DEBUG_ServerTransportSink
            logger.Log(this, "Registering!");
#endif

            demultiplexer.register((uint)ReservedObjectID.Interoperability_Remoting_ServerObject,
                new Base3_.ReceiveCallback(receiveCallback));
        }

        private IServerChannelSink nextSink;
        private QS.Fx.Logging.ILogger logger;

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            RequestWrapper requestWrapper = (RequestWrapper)receivedObject;

#if DEBUG_ServerTransportSink
            logger.Log(this, "Received from " + clientAddress.ToString() + " object " + receivedObject.ToString());
#endif

            ITransportHeaders requestHeaders = requestWrapper.TransportHeaders;
            System.IO.Stream requestStream = requestWrapper.Stream;
            
            requestHeaders["__RequestUri"] = requestWrapper.ObjectURI;

            ServerChannelSinkStack stack = new ServerChannelSinkStack();
            stack.Push(this, sourceIID.Address);

            IMessage responseMsg;
            System.IO.Stream responseStream;
            ITransportHeaders responseHeaders;

            ServerProcessing proc = nextSink.ProcessMessage(stack, null, requestHeaders, requestStream, out responseMsg, out responseHeaders,
                out responseStream);

            switch (proc)
            {
                case ServerProcessing.Complete:
                {
#if DEBUG_ServerTransportSink
                    logger.Log(this, "Request completed, now we should send a response somehow!");
#endif

                    RequestWrapper wrappedResponse = new RequestWrapper(responseHeaders, responseStream);

#if DEBUG_ServerTransportSink
                    logger.Log(this, "Wrapped response : " + wrappedResponse.ToString());
#endif

                    return wrappedResponse;
                }
                // break;

                case ServerProcessing.Async:
                case ServerProcessing.OneWay:
                break;
            }

            return null;
        }    

        #region IServerChannelSink Members

        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, System.IO.Stream stream)
        {
            QS.Fx.Network.NetworkAddress clientAddress = (QS.Fx.Network.NetworkAddress)state;

#if DEBUG_ServerTransportSink
            logger.Log(this, "Request completed, now we should send a response somehow!");
#endif

            throw new NotImplementedException();

            // sends a response message
        }

        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, System.IO.Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out System.IO.Stream responseStream)
        {
            throw new NotSupportedException();
        }

        public System.IO.Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        public IServerChannelSink NextChannelSink
        {
            get { return nextSink; }
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
