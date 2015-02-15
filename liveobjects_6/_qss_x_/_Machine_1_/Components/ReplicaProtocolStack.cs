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

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Base.Inspectable]
    public class ReplicaProtocolStack : QS.Fx.Inspection.Inspectable, IDisposable
    {
        public ReplicaProtocolStack(QS.Fx.Platform.IPlatform platform, int portno, string machineName, bool master,
            QS.Fx.Network.NetworkAddress[] multicastAddresses)
        {
            demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(platform.Logger, platform.EventLogger);

            localAddress = new QS._core_c_.Base3.InstanceID(
                new QS.Fx.Network.NetworkAddress(platform.Network.Interfaces[0].InterfaceAddress, portno), DateTime.Now);

            root = new QS._qss_c_.Base8_.Root(statisticsController, platform.Logger, platform.EventLogger, platform.Clock,
                platform.Network, localAddress, demultiplexer);

            List<QS._qss_x_.Base1_.Address> addresses = new List<QS._qss_x_.Base1_.Address>();
            foreach (QS.Fx.Network.NetworkAddress multicastAddress in multicastAddresses)
            {
                multicastListeners.Add(((QS._qss_c_.Devices_3_.IMembershipController)root).Join(multicastAddress));
                addresses.Add(QS._qss_x_.Base1_.Address.QuickSilver(multicastAddress));
            }

            unreliableSenders = new QS._qss_x_.Senders_.UnreliableSenders(platform.Network, root);
            unreliableSinks = new QS._qss_x_.Senders_.UnreliableSinks(platform.Network, root);

            replica = new _Machine_1_.Components.Replica(platform.Logger, demultiplexer,
                new Persistence_.Persistent<QS.Fx.Serialization.ISerializable, _Machine_1_.Components.IReplicaPersistentState,
                    _Machine_1_.Components.ReplicaPersistentState, _Machine_1_.Components.ReplicaPersistentState.Operation>(
                    platform.Filesystem.Root, platform.AlarmClock, QS._core_c_.Base3.Serializer.Global, platform.Logger),
                platform.Network.GetHostName(), QS._qss_x_.Base1_.Address.QuickSilver(localAddress.Address),
                machineName, master, addresses, platform.Clock, platform.AlarmClock, unreliableSenders, unreliableSinks);
        }

        private QS._qss_c_.Statistics_.MemoryController statisticsController = new QS._qss_c_.Statistics_.MemoryController();

        [QS.Fx.Base.Inspectable("Demultiplexer")]
        private QS._qss_c_.Base3_.Demultiplexer demultiplexer;

        [QS.Fx.Base.Inspectable("Address")]
        private QS._core_c_.Base3.InstanceID localAddress;

        [QS.Fx.Base.Inspectable("Root")]
        private QS._qss_c_.Base8_.Root root;

        private List<QS._qss_c_.Devices_3_.IListener> multicastListeners = new List<QS._qss_c_.Devices_3_.IListener>();

        [QS.Fx.Base.Inspectable("Unreliable Sender")]
        private Senders_.UnreliableSenders unreliableSenders;

        [QS.Fx.Base.Inspectable("Unreliable Sinks")]
        private Senders_.UnreliableSinks unreliableSinks;

        [QS.Fx.Base.Inspectable("Replica")]
        private _Machine_1_.Components.Replica replica;

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IDisposable)replica).Dispose();
        }

        #endregion
    }
}
