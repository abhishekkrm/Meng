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

﻿// #define DEBUG_CompressingServerSink

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Remoting_.Compression
{
	public class CompressingServerSink : BaseChannelSinkWithProperties, IServerChannelSink
	{
		public CompressingServerSink(IServerChannelSink nextSink)
		{
			this.nextSink = nextSink;
		}

		private IServerChannelSink nextSink;

		#region IServerChannelSink Members

		ServerProcessing IServerChannelSink.ProcessMessage(IServerChannelSinkStack sinkStack,
			IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream,
			out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
#if DEBUG_CompressingServerSink
				Console.WriteLine("CompressingServerSink.ProcessMessage");
#endif

			string xcompress = (string) requestHeaders["X-Compress"];
			bool isCompressed = xcompress != null && xcompress == "yes";
			if (isCompressed)
			{
				requestStream = Streams_.StreamCompressor.Uncompress(requestStream);
			}

			sinkStack.Push(this, isCompressed);

			ServerProcessing serverProcessing = nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders,
				requestStream, out responseMsg, out responseHeaders,
				out responseStream);

			if (serverProcessing == ServerProcessing.Complete && isCompressed)
			{
				responseStream = Streams_.StreamCompressor.Compress(responseStream);
				responseHeaders["X-Compress"] = "yes";
			}

			return serverProcessing;
		}

		void IServerChannelSink.AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, 
			object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			bool isCompressed = (bool)state;
			if (isCompressed)
			{
				stream = Streams_.StreamCompressor.Compress(stream);
				headers["X-Compress"] = "yes";
			}

			sinkStack.AsyncProcessResponse(msg, headers, stream);
		}

		Stream IServerChannelSink.GetResponseStream(IServerResponseChannelSinkStack sinkStack, 
			object state, IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		IServerChannelSink IServerChannelSink.NextChannelSink
		{
			get { return nextSink; }
		}

		#endregion

		#region IChannelSinkBase Members

		System.Collections.IDictionary IChannelSinkBase.Properties
		{
			get { return null; }
		}

		#endregion
	}
}
