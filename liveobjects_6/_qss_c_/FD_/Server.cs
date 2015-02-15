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

using System.Threading;

namespace QS._qss_c_.FD_
{
    public class Server : IServer
    {
        public Server(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            demultiplexer.register((uint)ReservedObjectID.FailureDetector_ServerAgent, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));

            monitoredNodes = new QS._qss_c_.Collections_2_.RefCollection<QS.Fx.Network.NetworkAddress,NodeRef>(
                new QS._qss_c_.Collections_2_.RefCollection<QS.Fx.Network.NetworkAddress,NodeRef>.InitializeCallback(this.initializeCallback));
        }

        private QS.Fx.Logging.ILogger logger;

        private readonly Collections_2_.RefCollection<QS.Fx.Network.NetworkAddress, NodeRef> monitoredNodes;

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Beacon beacon = receivedObject as Beacon;
            // beacon.

            return null;
        }

        private void initializeCallback(NodeRef nodeRef)
        {
            nodeRef.lastHeardOf = DateTime.Now;
        }

        private class NodeRef : Collections_2_.RefCollection<QS.Fx.Network.NetworkAddress,NodeRef>.Element, INodeRef
        {
            public NodeRef()
            {
            }

            public DateTime lastHeardOf;

            protected override void disposingOf()
            {
                // .........................................................................................
            }
        }

        #region IServer Members

        public INodeRef Monitor(QS.Fx.Network.NetworkAddress nodeAddress)
        {
            return monitoredNodes[nodeAddress];
        }

        #endregion
    }


}
