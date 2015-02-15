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

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace QS._qss_c_.Remoting_.Channels
{
	public static class CustomizableChannels
	{
		public const QS._qss_c_.Remoting_.Channels.CustomizableChannels.ChannelOption DefaultChannelOption =
			QS._qss_c_.Remoting_.Channels.CustomizableChannels.ChannelOption.Compressed |
			QS._qss_c_.Remoting_.Channels.CustomizableChannels.ChannelOption.Encrypted;

		public const QS._qss_c_.Remoting_.Encryption.Keys.AlgorithmClass DefaultEncryptionAlgorithm =
			QS._qss_c_.Remoting_.Encryption.Keys.AlgorithmClass.TripleDES;

		public static void Initialize(string cryptographicKeyPath)
		{
			Initialize(0, cryptographicKeyPath);
		}

		public static void Initialize(int portno, string cryptographicKeyPath)
		{
			Initialize(portno, DefaultChannelOption, DefaultEncryptionAlgorithm, cryptographicKeyPath);
		}

		public static void Initialize(int portno, ChannelOption channelOption, 
			Encryption.Keys.AlgorithmClass encryptionAlgorithm, string cryptographicKeyPath)
		{
			Initialize(portno, channelOption, encryptionAlgorithm, Encryption.Keys.Load(cryptographicKeyPath));
		}

		public static void Initialize(int portno, ChannelOption channelOption, 
			Encryption.Keys.AlgorithmClass encryptionAlgorithm, byte[] cryptographicKey)
		{
			lock (typeof(CustomizableChannels))
			{
				if (channel != null)
					throw new Exception("Already initialized.");

#pragma warning disable 0618
                ChannelServices.RegisterChannel(
					channel = QS._qss_c_.Remoting_.Channels.CustomizableChannels.CreateChannel(
					portno, channelOption, Encryption.Keys.AlgorithmName(encryptionAlgorithm),
					cryptographicKey));
#pragma warning restore 0618
            }
		}

		private static HttpChannel channel = null;

		public static void Cleanup()
		{
			ChannelServices.UnregisterChannel(channel);
		}

		// -----------------------------------------------------------------------------------------------

		[System.Flags]
		public enum ChannelOption : byte
		{
			None				= 0x00,
			Compressed		= 0x01, 
			Encrypted			= 0x02,
			Authenticated	= 0x04,
			Default				= Compressed | Encrypted
		}

//		public const ChannelOption DefaultChannelOption = 
//			ChannelOption.Compressed | ChannelOption.Encrypted | ChannelOption.Authenticated;

//		public static HttpChannel CreateChannel()
//		{
//			return CreateChannel(DefaultChannelOption);
//		}
//
//		public static HttpChannel CreateChannel(ChannelOption channelOption)
//		{
//			return CreateChannel(channelOption, 0);
//		}
//
//		public static HttpChannel CreateChannel(int portno)
//		{
//			return CreateChannel(DefaultChannelOption, portno);
//		}

		public static HttpChannel CreateChannel(int portno, ChannelOption channelOption, 
			string encryptionAlgorithm, byte[] cryptographicKey)
		{
			IDictionary properties = new Hashtable();
			properties["port"] = portno;

			Queue<IClientChannelSinkProvider> clientProviders = new Queue<IClientChannelSinkProvider>();
			Queue<IServerChannelSinkProvider> serverProviders = new Queue<IServerChannelSinkProvider>();

			if ((channelOption & ChannelOption.Compressed) == ChannelOption.Compressed)
			{
				clientProviders.Enqueue(new QS._qss_c_.Remoting_.Compression.CompressionClientSinkProvider());
				serverProviders.Enqueue(new QS._qss_c_.Remoting_.Compression.CompressionServerSinkProvider());
			}

			if ((channelOption & ChannelOption.Encrypted) == ChannelOption.Encrypted)
			{
				IDictionary encryptionProperties = new Hashtable();
				encryptionProperties["algorithmName"] = encryptionAlgorithm;
				encryptionProperties["symmetricKey"] = cryptographicKey;

				clientProviders.Enqueue(
					new QS._qss_c_.Remoting_.Encryption.EncryptionClientSinkProvider(encryptionProperties, null));
				serverProviders.Enqueue(
					new QS._qss_c_.Remoting_.Encryption.EncryptionServerSinkProvider(encryptionProperties, null));
			}

			SoapClientFormatterSinkProvider clientSoap = new SoapClientFormatterSinkProvider();
			SoapServerFormatterSinkProvider serverSoap = new SoapServerFormatterSinkProvider();

			IClientChannelSinkProvider clientProvider = clientSoap;
			IServerChannelSinkProvider serverProvider = serverSoap;

			while (clientProviders.Count > 0)
			{
				IClientChannelSinkProvider nextClientProvider = clientProviders.Dequeue();
				clientProvider.Next = nextClientProvider;
				clientProvider = nextClientProvider;

				IServerChannelSinkProvider nextServerProvider = serverProviders.Dequeue();
				nextServerProvider.Next = serverProvider;
				serverProvider = nextServerProvider;
			}

			HttpChannel channel = new HttpChannel(properties, clientSoap, serverProvider);

			return channel;
		}
	}
}
