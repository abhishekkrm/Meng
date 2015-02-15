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
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Remoting_.Encryption
{
	public class EncryptingClientSink : BaseChannelSinkWithProperties, IClientChannelSink
	{
		public EncryptingClientSink(IClientChannelSink nextSink, string algorithmName, byte[] symmetricKey)
		{
			encryptor = new QS._qss_c_.Streams_.StreamEncryptor(algorithmName, symmetricKey);
			this.nextSink = nextSink;
		}

		private Streams_.StreamEncryptor encryptor;
		private IClientChannelSink nextSink;

		#region IClientChannelSink Members

		void IClientChannelSink.ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, 
			Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			byte[] initializationVector;
			Stream encryptedStream = encryptor.Encrypt(requestStream, out initializationVector);

			requestHeaders["X-Encrypt"] = "yes";
			requestHeaders["X-EncryptIV"] = Convert.ToBase64String(initializationVector);

			nextSink.ProcessMessage(msg, requestHeaders, encryptedStream, out responseHeaders, out responseStream);

			if (responseHeaders["X-Encrypt"] != null && responseHeaders["X-Encrypt"].Equals("yes"))
			{
				responseStream = encryptor.Decrypt(responseStream, 
					Convert.FromBase64String((string)responseHeaders["X-EncryptIV"]));
			}
		}

		void IClientChannelSink.AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, 
			ITransportHeaders headers, Stream stream)
		{
			byte[] initializationVector;
			Stream encryptedStream = encryptor.Encrypt(stream, out initializationVector);

			headers["X-Encrypt"] = "yes";
			headers["X-EncryptIV"] = Convert.ToBase64String(initializationVector);

			sinkStack.Push(this, null);
			nextSink.AsyncProcessRequest(sinkStack, msg, headers, encryptedStream);
		}

		void IClientChannelSink.AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, 
			ITransportHeaders headers, Stream stream)
		{
			if (headers["X-Encrypt"] != null && headers["X-Encrypt"].Equals("yes"))
			{
				stream = encryptor.Decrypt(stream,
					Convert.FromBase64String((string)headers["X-EncryptIV"]));
			}

			sinkStack.AsyncProcessResponse(headers, stream);
		}

		Stream IClientChannelSink.GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			return null;
		}

		IClientChannelSink IClientChannelSink.NextChannelSink
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
