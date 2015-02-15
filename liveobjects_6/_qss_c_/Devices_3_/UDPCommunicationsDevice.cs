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
using System.Net.Sockets;

namespace QS._qss_c_.Devices_3_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class UDPCommunicationsDevice 
        : QS.Fx.Inspection.Inspectable, ICommunicationsDevice, IDisposable, Devices_2_.ICommunicationsDevice
    {
        private const int defaultAnticipatedNumberOfSenders = 20;
        private const int defaultAnticipatedNumberOfListeners = 20;

        private const UDPReceiver.ProcessingMode defaultIncomingPacketProcessingMode = UDPReceiver.ProcessingMode.ASYNCHRONOUS;

        public UDPCommunicationsDevice(INetworkInterface networkInterface, QS.Fx.Logging.ILogger logger, int maximumTransmissionUnit) 
            : this(networkInterface, logger, defaultIncomingPacketProcessingMode, maximumTransmissionUnit)
        {
        }

        public UDPCommunicationsDevice(INetworkInterface networkInterface, QS.Fx.Logging.ILogger logger, 
            UDPReceiver.ProcessingMode incomingPacketProcessingMode, int maximumTransmissionUnit) 
            : this(networkInterface, logger, defaultAnticipatedNumberOfSenders, defaultAnticipatedNumberOfListeners, 
                incomingPacketProcessingMode, maximumTransmissionUnit)
        {
        }
        
        public UDPCommunicationsDevice(INetworkInterface networkInterface, QS.Fx.Logging.ILogger logger, int anticipatedNumberOfSenders,
            int anticipatedNumberOfListeners, UDPReceiver.ProcessingMode incomingPacketProcessingMode, int maximumTransmissionUnit)
        {
            this.clock = QS._core_c_.Base2.PreciseClock.Clock;

            this.networkInterface = networkInterface;
            this.incomingPacketProcessingMode = incomingPacketProcessingMode;
            this.logger = logger;
            this.senderCollection = new SortedDictionary<QS.Fx.Network.NetworkAddress, UDPSender>(); // anticipatedNumberOfSenders);
            this.listenerCollection = new SortedDictionary<QS.Fx.Network.NetworkAddress, UDPReceiver>(); // anticipatedNumberOfListeners);
            this.maximumTransmissionUnit = maximumTransmissionUnit;

            this.senderCollection_insWrapper =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, UDPSender>(
                    "Senders", senderCollection);

            this.receiverCollection_insWrapper =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, UDPReceiver>(
                    "Listeners", listenerCollection);

/*
            _senders_diagnostics =
                new QS.CMS.Diagnostics.ComponentContainer<QS.Fx.Network.NetworkAddress, UDPSender>(senderCollection);

            _receivers_diagnostics =
                new QS.CMS.Diagnostics.ComponentContainer<QS.Fx.Network.NetworkAddress, UDPSender>(listenerCollection);
*/
        }

        private INetworkInterface networkInterface;
        private UDPReceiver.ProcessingMode incomingPacketProcessingMode;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private System.Collections.Generic.SortedDictionary<QS.Fx.Network.NetworkAddress, UDPSender> senderCollection;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private System.Collections.Generic.SortedDictionary<QS.Fx.Network.NetworkAddress, UDPReceiver> listenerCollection;
        private int maximumTransmissionUnit;

        [QS.Fx.Base.Inspectable("Senders", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, UDPSender> senderCollection_insWrapper;
        [QS.Fx.Base.Inspectable("Receivers", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, UDPReceiver> receiverCollection_insWrapper;

/*
        [Diagnostics.Component("Senders")]
        private Diagnostics.ComponentContainer<QS.Fx.Network.NetworkAddress, UDPSender> _senders_diagnostics;
        [Diagnostics.Component("Receivers")]
        private Diagnostics.ComponentContainer<QS.Fx.Network.NetworkAddress, UDPSender> _receivers_diagnostics;
*/

        #region ICommunicationsDevice Members

        public IPAddress Address
        {
            get { return networkInterface.Address; }
        }

        public INetworkInterface NetworkInterface
        {
            get { return networkInterface; }
        }

        public ISender GetSender(QS.Fx.Network.NetworkAddress destinationAddress)
        {
            UDPSender sender;
            lock (senderCollection)
            {
                if (!senderCollection.TryGetValue(destinationAddress, out sender))
                    senderCollection.Add(destinationAddress, sender = new UDPSender(logger, clock, this, destinationAddress));
            }

            return sender;
        }

        public IListener ListenAt(QS.Fx.Network.NetworkAddress receivingAddress, IReceiver asynchronousReceiver)
        {
            UDPReceiver receiver;
            lock (listenerCollection)
            {
                if (listenerCollection.TryGetValue(receivingAddress, out receiver))
                {
                    if (!receiver.IsDestroyed)
                        throw new Exception("Already listening at address " + receivingAddress.ToString() + ".");

                    // disposed, we can recycle it and make space for a new one
                    listenerCollection.Remove(receivingAddress);
                }

                receiver = new UDPReceiver(this, logger, receivingAddress, asynchronousReceiver, incomingPacketProcessingMode);
                listenerCollection.Add(receiver.Address, receiver);
            }

            return receiver;
        }

        public int MTU
        {
            get { return maximumTransmissionUnit; }
            set 
            {
                maximumTransmissionUnit = value;
                logger.Log(this, "Maximum transmission unit for the device changed to " + maximumTransmissionUnit.ToString() + ".");
            }
        }

        public CommunicationsDevice.Class Class
        {
            get { return CommunicationsDevice.Class.UDP; }
        }

        #endregion

        public void ReleaseResources()
        {
            try
            {
                lock (listenerCollection)
                {
                    foreach (System.Collections.Generic.KeyValuePair<QS.Fx.Network.NetworkAddress, UDPReceiver> receiver in listenerCollection)
                    {
                        logger.Log(this, "__Dispose: Releasing Rcv @ " + receiver.Value.Address.ToString());
                        receiver.Value.Dispose();
                    }

                    listenerCollection.Clear();
                }

                lock (senderCollection)
                {
                    foreach (System.Collections.Generic.KeyValuePair<QS.Fx.Network.NetworkAddress, UDPSender> sender in senderCollection)
                    {
                        logger.Log(this, "__Dispose: Releasing Snd @ " + sender.Value.Address.ToString());
                        sender.Value.Dispose();
                    }

                    senderCollection.Clear();
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, "__ReleaseResources: " + exc.ToString());
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            ReleaseResources();
        }

        #endregion

        #region Devices2.ISendingDevice Members

        uint QS._qss_c_.Devices_2_.ISendingDevice.MTU
        {
            get { return (uint) this.MTU; }
        }

        void QS._qss_c_.Devices_2_.ISendingDevice.sendto(
            QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBlockOfData blockOfData)
        {
            if (!sourceAddress.HostIPAddress.Equals(this.Address) || sourceAddress.PortNumber != 0)
                throw new Exception("Incorrect source address: " + sourceAddress.ToString());

            System.Collections.Generic.List<QS.Fx.Base.Block> buffers = new System.Collections.Generic.List<QS.Fx.Base.Block>(1);
            buffers.Add(new QS.Fx.Base.Block(blockOfData.Buffer, blockOfData.OffsetWithinBuffer, blockOfData.SizeOfData));
            (this.GetSender(destinationAddress)).send(buffers);
        }

        public IAsyncResult BeginSendTo(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            QS._core_c_.Base2.IBlockOfData blockOfData, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Devices2.IReceivingDevice Members

        QS._qss_c_.Devices_2_.IListener QS._qss_c_.Devices_2_.IReceivingDevice.listenAt(IPAddress localAddress, QS.Fx.Network.NetworkAddress receivingAddress, QS._qss_c_.Devices_2_.OnReceiveCallback receiveCallback)
        {
            if (!localAddress.Equals(this.Address))
                throw new Exception("Incorrect local address: " + localAddress.ToString());

            return this.ListenAt(receivingAddress, new CompatibilityWrapper(new QS.Fx.Network.NetworkAddress(this.Address, 0), receiveCallback)); 
        }

        void QS._qss_c_.Devices_2_.IReceivingDevice.shutdown()
        {
            // this.Dispose();
        }

        #region CompatibilityWrapper

        private class CompatibilityWrapper : IReceiver
        {
            public CompatibilityWrapper(QS.Fx.Network.NetworkAddress localAddress, QS._qss_c_.Devices_2_.OnReceiveCallback receiveCallback)
            {
                this.localAddress = localAddress;
                this.receiveCallback = receiveCallback;
            }

            private QS.Fx.Network.NetworkAddress localAddress;
            private QS._qss_c_.Devices_2_.OnReceiveCallback receiveCallback;

            #region IReceiver Members

            public void receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
            {
                receiveCallback(sourceAddress, localAddress, new QS._core_c_.Base2.BlockOfData(data.Array, (uint) data.Offset, (uint) data.Count));
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
