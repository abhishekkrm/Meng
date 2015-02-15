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

// #define DEBUG_AsynchronousSocketReceiver

using System;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_c_.Devices_2_
{
	/// <summary>
	/// Summary description for SocketReceiver.
	/// </summary>
	public abstract class AsynchronousSocketReceiver
	{
		public AsynchronousSocketReceiver(Socket socket, uint bufferSize, QS.Fx.Logging.ILogger logger, bool processAsynchronously)
		{
			this.socket = socket;
			this.buffer = new byte[bufferSize];
			this.bufferSize = bufferSize;
			this.sender = new IPEndPoint(IPAddress.Any, 0);
			this.senderRemote = (EndPoint) sender;
			this.logger = logger;
			this.processAsynchronously = processAsynchronously;
			this.shuttingDown = false;

			initiateAsynchronousReceive();
		}

		public void shutdown()
		{
			shuttingDown = true;
			socket.Close();
		}

		private void initiateAsynchronousReceive()
		{
			socket.BeginReceiveFrom(buffer, 0, (int) bufferSize, SocketFlags.None, ref senderRemote, new AsyncCallback(receiveCallback), this);
		}

		protected abstract void process(byte[] bufferWithData, uint bytesReceived, IPAddress sourceAddress, uint sourcePort);

		private void receiveCallback(System.IAsyncResult asyncResult)
		{
#if DEBUG_AsynchronousSocketReceiver
            try
            {
                logger.Log(this, "__ReceiveCallback : We just received a packet.");
#endif

                if (!shuttingDown)
                {
                    if (processAsynchronously)
                    {
//                        logger.Log(this, "__ReceiveCallback : Processing asynchronously.");

                        uint bytesReceived = 0;
                        byte[] bufferWithData = null;
                        IPAddress sourceIPAddress;
                        uint sourcePortNo = 0;

                        lock (this)
                        {
                            bytesReceived = (uint)socket.EndReceiveFrom(asyncResult, ref senderRemote);
                            bufferWithData = buffer;
                            buffer = new byte[bufferSize];
                            sourceIPAddress = ((IPEndPoint)senderRemote).Address;
                            sourcePortNo = (uint)((IPEndPoint)senderRemote).Port;
                        }

#if DEBUG_AsynchronousSocketReceiver
                        logger.Log(this, "__ReceiveCallback : Source is " + sourceIPAddress.ToString() + ":" +
                            sourcePortNo.ToString() + ", data size is " + bytesReceived.ToString() + " bytes.");
#endif

                        initiateAsynchronousReceive();

                        if (bytesReceived > 0)
                            process(bufferWithData, bytesReceived, sourceIPAddress, sourcePortNo);
                    }
                    else
                    {
//                        logger.Log(this, "__ReceiveCallback : Processing synchronously.");

                        uint bytesReceived = 0;

                        lock (this)
                        {
                            bytesReceived = (uint)socket.EndReceiveFrom(asyncResult, ref senderRemote);

                            if (bytesReceived > 0)
                                process(buffer, bytesReceived, ((IPEndPoint)senderRemote).Address, (uint)((IPEndPoint)senderRemote).Port);
                        }

                        initiateAsynchronousReceive();
                    }
                }

#if DEBUG_AsynchronousSocketReceiver
            }
            catch (Exception exc)
            {
                try
                {
                    logger.Log(this, "__ReceiveCallback : Exception thrown, " + exc.ToString() + ".");
                }
                catch (Exception)
                {
                }

                throw new Exception("Could not receive.", exc);
            }
#endif
		}

		protected QS.Fx.Logging.ILogger logger;

		private Socket socket;
		private bool shuttingDown;
		private byte[] buffer;
		private uint bufferSize;
		private IPEndPoint sender;
		private EndPoint senderRemote;
		private bool processAsynchronously;
	}
}
