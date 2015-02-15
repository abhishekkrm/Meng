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

namespace QS._qss_x_.Network_
{
    public sealed class PhysicalNetworkInterface : QS.Fx.Network.INetworkInterface
    {
        public PhysicalNetworkInterface(QS._core_c_.Core.ICore core, System.Net.IPAddress interfaceAddress)
        {
            this.core = core;
            this.interfaceAddress = interfaceAddress;
        }

        private QS._core_c_.Core.ICore core;
        private System.Net.IPAddress interfaceAddress;

        #region INetworkInterface Members

        System.Net.IPAddress QS.Fx.Network.INetworkInterface.InterfaceAddress
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.IListener QS.Fx.Network.INetworkInterface.Listen(QS.Fx.Network.NetworkAddress address, QS.Fx.Network.ReceiveCallback callback, object context, 
            params QS.Fx.Base.IParameter[] parameters)
        {
            QS.Fx.Base.IParameters p = new QS._core_x_.Base.Parameters(parameters);
            return core.Listen(
                new QS._core_c_.Core.Address(interfaceAddress, address.HostIPAddress, address.PortNumber), callback, context,
                QS._core_x_.Base.Parameters.Get<int>(p, QS._core_c_.Core.ListenerInfo.Parameters.BufferSize, 20 * 1024),
                QS._core_x_.Base.Parameters.Get<int>(p, QS._core_c_.Core.ListenerInfo.Parameters.NumberOfBuffers, 100),
                QS._core_x_.Base.Parameters.Get<bool>(p, QS._core_c_.Core.ListenerInfo.Parameters.DrainSynchronously, !address.IsMulticastAddress),
                QS._core_x_.Base.Parameters.Get<int>(p, QS._core_c_.Core.ListenerInfo.Parameters.AdfBufferSize, 4 * 1024 * 1024),
                QS._core_x_.Base.Parameters.Get<bool>(p, QS._core_c_.Core.ListenerInfo.Parameters.HighPriority, !address.IsMulticastAddress));
        }

        QS.Fx.Network.ISender QS.Fx.Network.INetworkInterface.GetSender(QS.Fx.Network.NetworkAddress address, params QS.Fx.Base.IParameter[] parameters)
        {
            QS.Fx.Base.IParameters p = new QS._core_x_.Base.Parameters(parameters);
            return core.GetSender(
                new QS._core_c_.Core.Address(interfaceAddress, address.HostIPAddress, address.PortNumber),
                QS._core_x_.Base.Parameters.Get<int>(p, 
                    QS._core_c_.Core.SenderInfo.Parameters.AdfBufferSize, address.IsMulticastAddress ? 1024 * 1024 : 8192),
                QS._core_x_.Base.Parameters.Get<int>(p, 
                    QS._core_c_.Core.SenderInfo.Parameters.MaximumConcurrency, address.IsMulticastAddress ? 500 : 50),
                QS._core_x_.Base.Parameters.Get<bool>(p, QS._core_c_.Core.ListenerInfo.Parameters.HighPriority, !address.IsMulticastAddress));
        }

        #endregion
    }
}
