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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Network_
{
    public sealed class VirtualListener : Base1_.Parametrized, QS.Fx.Network.IListener, IVirtualListener
    {
        public VirtualListener(System.Net.IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress address,
            QS.Fx.Network.ReceiveCallback callback, object context, QS.Fx.Logging.ILogger logger)
        {
            this.interfaceAddress = interfaceAddress;
            this.address = address;
            this.callback = callback;
            this.context = context;
            this.logger = logger;
        }

        [QS.Fx.Base.Inspectable]
        private System.Net.IPAddress interfaceAddress;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.NetworkAddress address;

        private QS.Fx.Network.ReceiveCallback callback;
        private object context;
        private QS.Fx.Logging.ILogger logger;

        #region IListener Members

        System.Net.IPAddress QS.Fx.Network.IListener.InterfaceAddress
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.NetworkAddress QS.Fx.Network.IListener.Address
        {
            get { return address; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // ........................................................................................................................................................
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IVirtualListener Members

        void IVirtualListener.Dispatch(IVirtualPacket packet)
        {
            QS.Fx.Network.ReceiveCallback tocall;
            object ctx;

            lock (this)
            {
                tocall = this.callback;
                ctx = this.context;
            }

            if (tocall != null)
                tocall(packet.From.HostIPAddress, packet.From.PortNumber, new QS.Fx.Base.Block(packet.Data), ctx);
        }

        #endregion

        #region Reset

        public void Reset()
        {
            lock (this)
            {
#if DEBUG_LogGenerously
                logger.Log("Resetting listener " + address.ToString() + " at interface " + interfaceAddress.ToString() + ".");
#endif

                callback = null;
                context = null;
            }
        }

        #endregion
    }
}
