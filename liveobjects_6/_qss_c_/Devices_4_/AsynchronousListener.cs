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

// #define DEBUG_LogAllExceptions

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_c_.Devices_4_
{
    public class AsynchronousListener : IListener
    {
        public AsynchronousListener(QS.Fx.Logging.ILogger logger, INetworkConnection networkConnection, IPAddress interfaceAddress, 
            QS.Fx.Network.NetworkAddress receivingAddress, ReceiveCallback receiveCallback, uint maximumTransmissionUnit)
        {
            this.interfaceAddress = interfaceAddress;
            this.logger = logger;
            this.receivingAddress = receivingAddress;
            this.receiveCallback = receiveCallback;
            this.networkConnection = networkConnection;
            this.maximumTransmissionUnit = maximumTransmissionUnit;

            socket = QS._qss_c_.Devices_2_.UDPReceiver.createSocket(interfaceAddress, ref this.receivingAddress);
            receiveBuffer = new byte[maximumTransmissionUnit];
            sender = new IPEndPoint(IPAddress.Any, 0);
            senderRemote = (EndPoint)sender;

            socket.BeginReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ref senderRemote, 
                new AsyncCallback(this.ReceiveCallback), this);
        }

        private INetworkConnection networkConnection;
        private QS.Fx.Logging.ILogger logger;
        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress receivingAddress;
        private ReceiveCallback receiveCallback;
        private uint maximumTransmissionUnit;

        private Socket socket;
        private byte[] receiveBuffer;
        private IPEndPoint sender;
        private EndPoint senderRemote;
        private bool shutdown = false;

        #region ReceiveCallback

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (!shutdown)
                {
                    int nreceived = socket.EndReceiveFrom(result, ref senderRemote);
                    if (nreceived > 0)
                    {
                        receiveCallback(this, ((IPEndPoint)senderRemote).Address, ((IPEndPoint)senderRemote).Port,
                            new ArraySegment<byte>(receiveBuffer, 0, nreceived));
                    }
                    socket.BeginReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ref senderRemote,
                        new AsyncCallback(this.ReceiveCallback), this);
                }
            }
#if DEBUG_LogAllExceptions
            catch (Exception exc)
            {
                logger.Log(this, "__ReceiveCallback : " + exc.ToString());
#else
            catch (Exception)
            {
#endif
            }
        }

        #endregion 

        #region IListener Members

        INetworkConnection IListener.NetworkConnection
        {
            get { return networkConnection; }
        }

        QS.Fx.Network.NetworkAddress IListener.Address
        {
            get { return receivingAddress; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                shutdown = true;
                socket.Close();
                networkConnection.Unregister(this);
            }
#if DEBUG_LogAllExceptions
            catch (Exception exc)
            {
                logger.Log(this, "__Dispose : " + exc.ToString());
#else
            catch (Exception)
            {
#endif
            }
        }

        #endregion
    }
}
