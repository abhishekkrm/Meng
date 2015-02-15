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

namespace QS._qss_x_.Backbone_.Node
{
    public sealed class ProtocolStack : IProtocolStack
    {
        #region Constructor

        public ProtocolStack(QS.Fx.Platform.IPlatform platform, QS._qss_c_.Base1_.Subnet subnet, int portno)
        {
            this.platform = platform;
            
            demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(platform.Logger, platform.EventLogger);

            QS.Fx.Network.INetworkInterface netinterface = null;
            foreach (QS.Fx.Network.INetworkInterface i in platform.Network.Interfaces)
            {
                if (subnet.contains(i.InterfaceAddress))
                {
                    netinterface = i;
                    break;
                }
            }

            if (netinterface == null)
                throw new Exception("Could not find any network interface on subnet " + subnet.ToString() + ".");

            localAddress = new QS._core_c_.Base3.InstanceID(
                new QS.Fx.Network.NetworkAddress(netinterface.InterfaceAddress, portno), DateTime.Now);

            root = new QS._qss_c_.Base8_.Root(statisticsController, platform.Logger, platform.EventLogger, platform.Clock,
                platform.Network, localAddress, demultiplexer);

            unreliableSenders = new QS._qss_x_.Senders_.UnreliableSenders(platform.Network, root);

            unreliableSinks = new QS._qss_x_.Senders_.UnreliableSinks(platform.Network, root);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("Platform")]
        private QS.Fx.Platform.IPlatform platform;

        [QS.Fx.Base.Inspectable("Demultiplexer")]
        private QS._qss_c_.Base3_.Demultiplexer demultiplexer;

        [QS.Fx.Base.Inspectable("Address")]
        private QS._core_c_.Base3.InstanceID localAddress;

        private QS._qss_c_.Statistics_.MemoryController statisticsController = new QS._qss_c_.Statistics_.MemoryController();

        [QS.Fx.Base.Inspectable("Root")]
        private QS._qss_c_.Base8_.Root root;

        [QS.Fx.Base.Inspectable("Unreliable Senders")]
        private Senders_.UnreliableSenders unreliableSenders;

        [QS.Fx.Base.Inspectable("Unreliable Sinks")]
        private Senders_.UnreliableSinks unreliableSinks;

        #endregion

        #region IProtocolStack Members

        QS._qss_c_.Base3_.IDemultiplexer IProtocolStack.Demultiplexer
        {
            get { return demultiplexer; }
        }

//        public QS.CMS.QS._core_c_.Base3.InstanceID IProtocolStack.InstanceID
//        {
//            get { return localAddress; }
//        }
//
//        public QS.CMS.Base8.Root IProtocolStack.Root
//        {
//            get { return root; }
//        }

        QS._qss_c_.Base3_.ISenderCollection<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender> IProtocolStack.UnreliableSenders
        {
            get { return unreliableSenders; }
        }

        QS._qss_c_.Base6_.ICollectionOf<QS._qss_x_.Base1_.Address, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> IProtocolStack.UnreliableSinks
        {
            get { return unreliableSinks; }
        }

        #endregion
    }
}
