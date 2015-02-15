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

namespace QS._qss_c_.Devices_6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Connections : QS.Fx.Inspection.Inspectable, Base6_.ICollectionOf<IPAddress, INetworkConnection>, INetwork
    {
        public Connections(QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.eventLogger = eventLogger;
            receiverController = new ReceiverController(QS._core_c_.Base2.PreciseClock.Clock, 5000, eventLogger);

            foreach (IPAddress interfaceAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                connections.Add(interfaceAddress, 
                    new Connection(senderController, receiverController, interfaceAddress, eventLogger));
        }

        private QS.Fx.Logging.IEventLogger eventLogger;

        [QS._core_c_.Diagnostics.Component("Concurrency Controller")]
        private SenderController senderController = new SenderController();
        [QS._core_c_.Diagnostics.ComponentCollection("Connections")]
        private IDictionary<IPAddress, Connection> connections = new Dictionary<IPAddress, Connection>();
        [QS._core_c_.Diagnostics.Component("Receiver Controller")]
        private ReceiverController receiverController;

        #region Class Connection

        [QS._core_c_.Diagnostics.ComponentContainer]
        private class Connection : QS.Fx.Inspection.Inspectable, INetworkConnection
        {
            public Connection(SenderController senderController, ReceiverController receiverController, IPAddress interfaceAddress,
                QS.Fx.Logging.IEventLogger eventLogger)
            {
                this.senderController = senderController;
                this.interfaceAddress = interfaceAddress;
                this.receiverController = receiverController;
                this.eventLogger = eventLogger;
            }

            private SenderController senderController;
            private ReceiverController receiverController;
            private QS.Fx.Logging.IEventLogger eventLogger;
            private IPAddress interfaceAddress;
            [QS._core_c_.Diagnostics.ComponentCollection("Senders")]
            private IDictionary<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._qss_c_.Base6_.Asynchronous<Block>>> senders =
                new Dictionary<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._qss_c_.Base6_.Asynchronous<Block>>>();
            [QS._core_c_.Diagnostics.ComponentCollection("Receivers")]
            private IDictionary<QS.Fx.Network.NetworkAddress, Receiver> receivers = new Dictionary<QS.Fx.Network.NetworkAddress, Receiver>();

            #region ICollectionOf<NetworkAddress,ISink<Asynchronous<Block>>> Members

            QS._core_c_.Base6.ISink<QS._qss_c_.Base6_.Asynchronous<Block>> 
                QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, 
                    QS._core_c_.Base6.ISink<QS._qss_c_.Base6_.Asynchronous<Block>>>.this[QS.Fx.Network.NetworkAddress address]
            {
                get 
                {
                    lock (this)
                    {
                        if (senders.ContainsKey(address))
                            return senders[address];
                        else
                        {
                            Sender sender = new Sender(interfaceAddress, address, senderController);
                            senders[address] = sender;
                            return sender;
                        }
                    }
                }
            }

            #endregion

            #region INetworkConnection Members

            void INetworkConnection.Start(QS.Fx.Network.NetworkAddress listeningAddress)
            {
                lock (this)
                {
                    Receiver receiver;
                    if (!receivers.TryGetValue(listeningAddress, out receiver))
                        receivers.Add(listeningAddress, 
                            receiver = new Receiver(interfaceAddress, listeningAddress, receiverController, eventLogger));
                }                
            }

            void INetworkConnection.Stop(QS.Fx.Network.NetworkAddress listeningAddress)
            {
                lock (this)
                {
                    Receiver receiver;
                    if (!receivers.TryGetValue(listeningAddress, out receiver))
                        throw new Exception("Not listening at the specified address.");
                    ((IDisposable) receiver).Dispose();
                }
            }

            #endregion
        }

        #endregion

        #region ICollectionOf<IPAddress,INetworkConnection> Members

        INetworkConnection QS._qss_c_.Base6_.ICollectionOf<IPAddress, INetworkConnection>.this[IPAddress address]
        {
            get { return connections[address]; }
        }

        #endregion

        #region INetwork Members

        QS._qss_c_.Base6_.ICollectionOf<IPAddress, INetworkConnection> INetwork.Connections
        {
            get { return this; }
        }

        ReceiveCallback INetwork.ReceiveCallback
        {
            get { return receiverController.Callback; }
            set { receiverController.Callback = value; }
        }

        #endregion
    }
}
