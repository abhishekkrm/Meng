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

// #define DEBUG_NetworkConsole

using System;
using System.Text;
using System.Net;

namespace QS._qss_c_.Components_1_
{
	/// <summary>
	/// Summary description for NetworkConsole.
	/// </summary>
	public class NetworkConsole : QS.Fx.Logging.IConsole, IDisposable
	{
		public NetworkConsole(QS.Fx.Logging.ILogger logger, IPAddress localIPAddress, QS.Fx.Network.NetworkAddress networkAddress)
		{
			this.networkAddress = networkAddress;
			this.logger = logger;
			tcpDevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice("NetworkConsole_TCP", localIPAddress, logger, false, 0, 2);
			
// #if DEBUG_NetworkConsole
			this.Log("Starting a network console at " + tcpDevice.IPAddress.ToString() + ":" + tcpDevice.PortNumber.ToString() + ".");
// #endif
		}

		private QS.Fx.Network.NetworkAddress networkAddress;
		private QS.Fx.Logging.ILogger logger;
		private Devices_1_.TCPCommunicationsDevice tcpDevice;

		#region IConsole Members

        public void Log(string s)
		{
			try
			{
				byte[] encodedBytes = Encoding.ASCII.GetBytes(s);			
				tcpDevice.unicast(networkAddress.HostIPAddress, networkAddress.PortNumber, encodedBytes, 0, encodedBytes.Length);
			}
			catch (Exception exc)
			{
				try
				{
					logger.Log(this, exc.ToString());
				}
				catch (Exception)
				{
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
#if DEBUG_NetworkConsole
				logger.Log(null, "NetworkConsole.Dispose_enter");
#endif

				try
				{
					this.Log("Shutting down network console at " + tcpDevice.IPAddress.ToString() + ":" + tcpDevice.PortNumber.ToString() + ".");
					tcpDevice.shutdown();
				}
				catch (Exception exc)
				{
					logger.Log(this, "Exception catched during NetworkConsole.Dispose : " + exc.ToString());
				}

#if DEBUG_NetworkConsole
				logger.Log(null, "NetworkConsole.Dispose_leave");
#endif
			}
			catch (Exception)
			{
			}
		}

		#endregion
	}
}
