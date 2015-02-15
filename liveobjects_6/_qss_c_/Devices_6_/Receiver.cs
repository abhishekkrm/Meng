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
using System.Net;
using System.Net.Sockets;

namespace QS._qss_c_.Devices_6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Receiver : QS.Fx.Inspection.Inspectable, IDisposable
    {
        public Receiver(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress listeningAddress, 
            IReceiverController receiverController, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.interfaceAddress = interfaceAddress;
            this.listeningAddress = listeningAddress;
            this.receiverController = receiverController;
            this.eventLogger = eventLogger;

            socket = Sockets.CreateReceiverUDPSocket(interfaceAddress, ref this.listeningAddress);
            myid = "Receiver(" + interfaceAddress.ToString() + ", " + listeningAddress.ToString() + ")";

            receiveAsyncCallback = new AsyncCallback(this.ReceiveCallback);
            receiverController.Enqueue(ref buffer);
            this.ReceiveCallback(null);
        }

        private string myid;
        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress listeningAddress;
        private IReceiverController receiverController;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private Socket socket;
        private AsyncCallback receiveAsyncCallback;
        private EndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
        private ReceiveBuffer buffer;        
        private IAsyncResult asynchronousResult;

        private void ReceiveCallback(IAsyncResult asynchronousResult)
        {
            if (asynchronousResult != null)
            {
                if ((buffer.NReceived = socket.EndReceiveFrom(asynchronousResult, ref buffer.EndPoint)) > 0)
                {
                    buffer.NIC = interfaceAddress;
                    buffer.ReceiverAddress = listeningAddress;
                    receiverController.Enqueue(ref buffer);
                }
                else
                    this.Problem("Could not finalize asynchronous receive.");
            }

            while (socket.Available > 0)
            {
                try
                {
                    buffer.EndPoint = endpoint;

                    if ((buffer.NReceived = socket.ReceiveFrom(buffer.Buffer.Array, buffer.Buffer.Offset, buffer.Buffer.Count,
                        SocketFlags.None, ref buffer.EndPoint)) > 0)
                    {
                        buffer.NIC = interfaceAddress;
                        buffer.ReceiverAddress = listeningAddress;
                        receiverController.Enqueue(ref buffer);
                    }
                    else
                        this.Problem("Could not complete synchronous receive.");
                }
                catch (SocketException exc)
                {
                    this.Problem("Could not complete synchronous receive: \"" + exc.SocketErrorCode.ToString() + "\".");
                }
            }

            bool listening;
            do
            {
                try
                {
                    buffer.EndPoint = endpoint;

                    this.asynchronousResult = socket.BeginReceiveFrom(buffer.Buffer.Array, buffer.Buffer.Offset, 
                        buffer.Buffer.Count, SocketFlags.None, ref buffer.EndPoint, receiveAsyncCallback, null);
                    listening = true;
                }
                catch (SocketException exc)
                {
                    listening = false;
                    this.Problem("Could not initiate asynchronous receive: \"" + exc.SocketErrorCode.ToString() + "\".");
                }
            }
            while (!listening);

            receiverController.Process();
        }

        private void Problem(string description)
        {
            try
            {
                if (eventLogger != null && eventLogger.Enabled)
                    eventLogger.Log(new Logging_1_.Events.Error(double.NaN, interfaceAddress, this, description));
            }
            catch (Exception)
            {
            }
        }

        public override string ToString()
        {
            return myid;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO: Implement cleanup................
        }

        #endregion
    }
}
