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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;

namespace QS._qss_c_.Devices_3_
{
    // [Diagnostics.ComponentContainer]
    public class UDPReceiver : Devices_2_.AsynchronousSocketReceiver, IListener
    {
        public enum ProcessingMode
        {
            SYNCHRONOUS, ASYNCHRONOUS
        }

        public UDPReceiver(QS.Fx.Logging.ILogger logger, IPAddress interfaceAddress, 
            QS.Fx.Network.NetworkAddress receivingAddress, IReceiver asynchronousReceiver)
            : base(Devices_2_.UDPReceiver.createSocket(interfaceAddress, ref receivingAddress), 20000, logger, true)
        {
            this.asynchronousReceiver = asynchronousReceiver;
            this.device = null;
            this.receivingAddress = receivingAddress;
        }

        public UDPReceiver(UDPCommunicationsDevice device, QS.Fx.Logging.ILogger logger, QS.Fx.Network.NetworkAddress receivingAddress, 
            IReceiver asynchronousReceiver, ProcessingMode processingMode) 
            : base(Devices_2_.UDPReceiver.createSocket(device.Address, ref receivingAddress), 
                (device != null) ? ((uint) device.MTU) : 20000, logger, 
                processingMode.Equals(ProcessingMode.ASYNCHRONOUS))
        {
            this.asynchronousReceiver = asynchronousReceiver;
            this.device = device;
            this.receivingAddress = receivingAddress;
        }

        private QS.Fx.Network.NetworkAddress receivingAddress;
        private IReceiver asynchronousReceiver;
        private UDPCommunicationsDevice device;
        private bool destroyed = false;

        protected override void process(byte[] bufferWithData, uint bytesReceived, IPAddress sourceAddress, uint sourcePort)
        {
            asynchronousReceiver.receive(new QS.Fx.Network.NetworkAddress(sourceAddress, (int) sourcePort), 
                new ArraySegment<byte>(bufferWithData, 0, (int) bytesReceived));
        }

        #region IListener Members

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return receivingAddress; }
        }

//        public void shutdown()
//        {
//            this.Dispose();
//        }

        #endregion

        public bool IsDestroyed
        {
            get { return destroyed; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            destroyed = true;
            this.shutdown();
        }

        #endregion

//        public override IComparable Contents
//        {
//            get { return receivingAddress; }
//        }
    }
}
