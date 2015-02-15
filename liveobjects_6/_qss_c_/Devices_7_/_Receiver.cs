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

namespace QS._qss_c_.Devices_7_
{
/*
    [Diagnostics.ComponentContainer]
    public class Receiver : TMS.Inspection.Inspectable, IReceiver
    {
        public Receiver(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress listeningAddress, ReceiveCallback receiveCallback,
            QS.Fx.Logging.IEventLogger eventLogger, Base6.RemoveCallback<Receiver> removeCallback, int mtu)
        {
            this.interfaceAddress = interfaceAddress;
            this.listeningAddress = listeningAddress;
            this.eventLogger = eventLogger;
            this.removeCallback = removeCallback;
            this.receiveCallback = receiveCallback;
            this.mtu = mtu;
            this.packet = new Reader(mtu);

            socket = Devices6.Sockets.CreateReceiverUDPSocket(interfaceAddress, ref this.listeningAddress);
            myid = "Receiver(" + interfaceAddress.ToString() + ", " + listeningAddress.ToString() + ")";

            receiveAsyncCallback = new AsyncCallback(this.ReceiveCallback);

//            receiverController.Enqueue(ref buffer);
//            this.ReceiveCallback(null); 
        }

        private string myid;
        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress listeningAddress;
        private Base6.RemoveCallback<Receiver> removeCallback;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private Socket socket;
        private AsyncCallback receiveAsyncCallback;
        private bool disconnected;
        private ReceiveCallback receiveCallback;        
        private int mtu;
        private Reader packet;
        private IAsyncResult asynchronousResult;

        private void ReceiveCallback(IAsyncResult asynchronousResult)
        {
        }


        public override string ToString()
        {
            return myid;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (!disconnected)
                {
                    disconnected = true;
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (removeCallback != null)
                removeCallback(this);
        }

        #endregion

        #region IReceiver Members

        IPAddress IReceiver.Interface
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.NetworkAddress IReceiver.Address
        {
            get { return listeningAddress; }
        }

        #endregion
    }
*/
}
