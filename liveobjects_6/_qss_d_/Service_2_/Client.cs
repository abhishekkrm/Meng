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
using System.Net;

namespace QS._qss_d_.Service_2_
{
	/// <summary>
	/// Summary description for Client.
	/// </summary>
	public class Client : IClient
	{
		public Client(IPAddress localAddress, QS.Fx.Network.NetworkAddress groupAddress, string cryptographicKeyPath)
			: this(localAddress, groupAddress, ServiceHelper.LoadCryptographicKey(cryptographicKeyPath))
		{
		}

		public Client(IPAddress localAddress, QS.Fx.Network.NetworkAddress groupAddress, byte[] cryptographicKey)
		{
			this.localAddress = localAddress;
			this.groupAddress = groupAddress;
			this.cryptographicKey = cryptographicKey;
		}

		private IPAddress localAddress;
		private QS.Fx.Network.NetworkAddress groupAddress;
		private byte[] cryptographicKey;

		#region IClient Members

		public QS.Fx.Network.NetworkAddress[] scan(TimeSpan timeoutOnScanning)
		{
			throw new Exception("not implemented");
		}

		public IServiceRef connectTo(QS.Fx.Network.NetworkAddress serviceAddress, QS.Fx.Logging.ILogger logger, TimeSpan timeoutOnRequests)
		{
			return new ServiceRef(localAddress, serviceAddress, logger, timeoutOnRequests, cryptographicKey);
		}

		#endregion
	}
}
