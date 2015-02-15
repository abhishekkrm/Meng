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

#define DEBUG_FaultyDevice

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Devices_3_
{
    public class FaultyDevice : ICommunicationsDevice
    {
        public FaultyDevice(INetworkInterface networkInterface, ICommunicationsDevice underlyingDevice, 
            QS.Fx.Logging.ILogger logger, double lossRate, double duplicationRate)
        {
            this.networkInterface = networkInterface;
            this.logger = logger;
            this.underlyingDevice = underlyingDevice;
            if (lossRate + duplicationRate > 1)
                throw new ArgumentException("Loss and duplication rate added together cannot be bigger than 1.");
            this.lossRate = lossRate;
            this.duplicationRate = duplicationRate;
        }

        private INetworkInterface networkInterface;
        private QS.Fx.Logging.ILogger logger;
        private ICommunicationsDevice underlyingDevice;
        private double lossRate, duplicationRate;

        private System.Random random = new System.Random();

        #region Class Sender

        private class Sender : ISender
        {
            public Sender(FaultyDevice owner, ISender underlyingSender)
            {
                this.owner = owner;
                this.underlyingSender = underlyingSender;
            }

            private FaultyDevice owner;
            private ISender underlyingSender;

            #region ISender Members

            ICommunicationsDevice ISender.CommunicationsDevice
            {
                get { return owner; }
            }

            QS.Fx.Network.NetworkAddress ISender.Address
            {
                get { return underlyingSender.Address; }
            }

            void ISender.send(IList<QS.Fx.Base.Block> buffers)
            {
                double decision = owner.random.NextDouble() - owner.lossRate;
                if (decision < 0)
                {                    
#if DEBUG_FaultyDevice
                    owner.logger.Log(this, "__send: packet from " + owner.underlyingDevice.Address.ToString() +
                        " to " + underlyingSender.Address.ToString() + " has been lost in the network");
#endif
                }
                else if (decision < owner.duplicationRate)
                {
#if DEBUG_FaultyDevice
                    owner.logger.Log(this, "__send: packet from " + owner.underlyingDevice.Address.ToString() +
                        " to " + underlyingSender.Address.ToString() + " has been duplicated in the network");
#endif
                    underlyingSender.send(buffers);
                    underlyingSender.send(buffers);
                }
                else
                {
#if DEBUG_FaultyDevice
                    owner.logger.Log(this, "__send: packet from " + owner.underlyingDevice.Address.ToString() +
                        " to " + underlyingSender.Address.ToString() + " has been delivered successfully");
#endif
                    underlyingSender.send(buffers);
                }
            }

            IAsyncResult ISender.BeginSend(IList<QS.Fx.Base.Block> buffers, AsyncCallback callback, object state)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        #region ICommunicationsDevice Members

        System.Net.IPAddress ICommunicationsDevice.Address
        {
            get { return underlyingDevice.Address; }
        }

        INetworkInterface ICommunicationsDevice.NetworkInterface
        {
            get { return networkInterface; }
        }

        ISender ICommunicationsDevice.GetSender(QS.Fx.Network.NetworkAddress destinationAddress)
        {
            return new Sender(this, underlyingDevice.GetSender(destinationAddress));
        }

        IListener ICommunicationsDevice.ListenAt(QS.Fx.Network.NetworkAddress receivingAddress, IReceiver asynchronousReceiver)
        {
            return underlyingDevice.ListenAt(receivingAddress, asynchronousReceiver);
        }

        int ICommunicationsDevice.MTU
        {
            get { return underlyingDevice.MTU; }
            set { underlyingDevice.MTU = value; }
        }

        CommunicationsDevice.Class ICommunicationsDevice.Class
        {
            get { return underlyingDevice.Class; }
        }

        #endregion
    }
}
