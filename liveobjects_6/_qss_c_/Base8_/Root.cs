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

// #define DEBUG_UseOldInterfacesToCore
// #define DEBUG_DisplayAllReceivedObjects
#define DEBUG_LogJoiningMulticastAddresses

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_c_.Base8_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Root : QS.Fx.Inspection.Inspectable, 
        Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>,
        Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>, Base3_.ISenderCollection<Base3_.IReliableSerializableSender>,
        Devices_3_.IMembershipController,
        Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.FlowControl3.IRateControlled>, QS._core_c_.Diagnostics2.IModule,
        IDisposable
    {
        private const int KILOBYTE = 1024;
        private const int MEGABYTE = KILOBYTE * KILOBYTE;

        public const int default_NumberOfReceiveBuffersForUnicastAddresses = 10;
        public const int default_DefaultNumberOfReceiveBuffersForMulticastAddresses = 100;
        public const bool default_DefaultDrainSynchronouslyForUnicastAddresses = false;
        public const bool default_DefaultDrainSynchronouslyForMulticastAddresses = false;
        public const int default_AfdReceiveBufferSizeForUnicastAddresses = 1 * MEGABYTE;
        public const int default_AfdReceiveBufferSizeForMulticastAddresses = 1 * MEGABYTE;
        public const int default_AfdSendBufferSizeForUnicastAddresses = 8 * KILOBYTE;
        public const int default_AfdSendBufferSizeForMulticastAddresses = 8 * KILOBYTE;
        public const int default_UnicastSenderConcurrency = 100;
        public const int default_MulticastSenderConcurrency = 100;

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsSenderContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Constructors

        public Root(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger,
#if DEBUG_UseOldInterfacesToCore
            Core.ICore core, 
#else
            QS.Fx.Clock.IClock clock,
            QS.Fx.Network.INetworkConnection networkConnection,
#endif
            QS._core_c_.Base3.InstanceID localAddress, Base3_.IDemultiplexer demultiplexer)
            : this(statisticsController, logger, eventLogger, 
#if DEBUG_UseOldInterfacesToCore
                core, 
#else
                clock, networkConnection,
#endif
                localAddress, demultiplexer, 65535)
        {
        }

        public Root(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger,
#if DEBUG_UseOldInterfacesToCore
            Core.ICore core, 
#else
            QS.Fx.Clock.IClock clock,
            QS.Fx.Network.INetworkConnection networkConnection,
#endif
            QS._core_c_.Base3.InstanceID localAddress, Base3_.IDemultiplexer demultiplexer, int mtu)
        {
#if DEBUG_UseOldInterfacesToCore
            this.core = core;
#else
            this.clock = clock;
            this.networkConnection = networkConnection;
#endif

            this.statisticsController = statisticsController;
            this.defaultNumberOfReceiveBuffersForUnicastAddresses = default_NumberOfReceiveBuffersForUnicastAddresses;
            this.defaultNumberOfReceiveBuffersForMulticastAddresses = default_DefaultNumberOfReceiveBuffersForMulticastAddresses;
            this.defaultDrainSynchronouslyForUnicastAddresses = default_DefaultDrainSynchronouslyForUnicastAddresses;
            this.defaultDrainSynchronouslyForMulticastAddresses = default_DefaultDrainSynchronouslyForMulticastAddresses;
            this.defaultAfdReceiveBufferSizeForUnicastAddresses = default_AfdReceiveBufferSizeForUnicastAddresses;
            this.defaultAfdReceiveBufferSizeForMulticastAddresses = default_AfdReceiveBufferSizeForMulticastAddresses;
            this.defaultAfdSendBufferSizeForUnicastAddresses = default_AfdSendBufferSizeForUnicastAddresses;
            this.defaultAfdSendBufferSizeForMulticastAddresses = default_AfdSendBufferSizeForMulticastAddresses;
            this.defaultUnicastSenderConcurrency = default_UnicastSenderConcurrency;
            this.defaultMulticastSenderConcurrency = default_MulticastSenderConcurrency;            

            this.logger = logger;
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;
            this.mtu = mtu;
            this.demultiplexer = demultiplexer;

            ListenerContext context = new ListenerContext(this, localAddress.Address);
            listenerContexts.Add(localAddress.Address, context);

#if DEBUG_UseOldInterfacesToCore
            context.Listener = core.Listen(new QS.CMS.Core.Address(
                localAddress.Address.HostIPAddress, localAddress.Address.HostIPAddress, localAddress.Address.PortNumber),
                new QS.CMS.Core.ReceiveCallback(this.ReceiveCallback), context, mtu,
                defaultNumberOfReceiveBuffersForUnicastAddresses, defaultDrainSynchronouslyForUnicastAddresses,
                defaultAfdReceiveBufferSizeForUnicastAddresses, true);
#else
            localNetworkInterface = networkConnection.GetInterface(localAddress.Address.HostIPAddress);
            context.Listener = localNetworkInterface.Listen(
                localAddress.Address, new QS.Fx.Network.ReceiveCallback(this.ReceiveCallback), context,
                new QS._core_x_.Base.Parameter<int>(
                    QS._core_c_.Core.ListenerInfo.Parameters.BufferSize, mtu),
                new QS._core_x_.Base.Parameter<int>(
                    QS._core_c_.Core.ListenerInfo.Parameters.NumberOfBuffers, defaultNumberOfReceiveBuffersForUnicastAddresses),
                new QS._core_x_.Base.Parameter<bool>(
                    QS._core_c_.Core.ListenerInfo.Parameters.DrainSynchronously, defaultDrainSynchronouslyForUnicastAddresses),
                new QS._core_x_.Base.Parameter<int>(
                    QS._core_c_.Core.ListenerInfo.Parameters.AdfBufferSize, defaultAfdReceiveBufferSizeForUnicastAddresses),
                new QS._core_x_.Base.Parameter<bool>(
                    QS._core_c_.Core.ListenerInfo.Parameters.HighPriority, true));
#endif

            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("Senders", diagnosticsSenderContainer);
        }

        #endregion

        #region Fields

#if DEBUG_UseOldInterfacesToCore
        private Core.ICore core;
#else
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Network.INetworkConnection networkConnection;
        private QS.Fx.Network.INetworkInterface localNetworkInterface;
#endif

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private int defaultNumberOfReceiveBuffersForUnicastAddresses, defaultNumberOfReceiveBuffersForMulticastAddresses,
            defaultAfdReceiveBufferSizeForUnicastAddresses, defaultAfdReceiveBufferSizeForMulticastAddresses,
            defaultAfdSendBufferSizeForUnicastAddresses, defaultAfdSendBufferSizeForMulticastAddresses,
            defaultUnicastSenderConcurrency, defaultMulticastSenderConcurrency;
        private bool defaultDrainSynchronouslyForUnicastAddresses, defaultDrainSynchronouslyForMulticastAddresses;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private int mtu;
        private Base3_.IDemultiplexer demultiplexer;
        private QS._core_c_.Base3.InstanceID localAddress;
        private IDictionary<QS.Fx.Network.NetworkAddress, ListenerContext> listenerContexts =
            new Dictionary<QS.Fx.Network.NetworkAddress, ListenerContext>();

        [QS._core_c_.Diagnostics.ComponentCollection("Senders")]
        private IDictionary<QS.Fx.Network.NetworkAddress, SerializingSender> senders =
            new Dictionary<QS.Fx.Network.NetworkAddress, SerializingSender>();

        #endregion

        #region Adjusting Configuration

        public int DefaultNumberOfReceiveBuffersForUnicastAddresses
        {
            get { return defaultNumberOfReceiveBuffersForUnicastAddresses; }
            set { defaultNumberOfReceiveBuffersForUnicastAddresses = value;  }
        }

        public int DefaultNumberOfReceiveBuffersForMulticastAddresses
        {
            get { return defaultNumberOfReceiveBuffersForMulticastAddresses; }
            set { defaultNumberOfReceiveBuffersForMulticastAddresses = value; }
        }

        public bool DefaultDrainSynchronouslyForUnicastAddresses
        {
            get { return defaultDrainSynchronouslyForUnicastAddresses; }
            set { defaultDrainSynchronouslyForUnicastAddresses = value; }
        }

        public bool DefaultDrainSynchronouslyForMulticastAddresses
        {
            get { return defaultDrainSynchronouslyForMulticastAddresses; }
            set { defaultDrainSynchronouslyForMulticastAddresses = value; }
        }

        public int DefaultAfdReceiveBufferSizeForUnicastAddresses
        {
            get { return defaultAfdReceiveBufferSizeForUnicastAddresses; }
            set { defaultAfdReceiveBufferSizeForUnicastAddresses = value; }
        }

        public int DefaultAfdReceiveBufferSizeForMulticastAddresses
        {
            get { return defaultAfdReceiveBufferSizeForMulticastAddresses; }
            set { defaultAfdReceiveBufferSizeForMulticastAddresses = value; }
        }

        public int DefaultAfdSendBufferSizeForUnicastAddresses
        {
            get { return defaultAfdSendBufferSizeForUnicastAddresses; }
            set { defaultAfdSendBufferSizeForUnicastAddresses = value; }
        }

        public int DefaultAfdSendBufferSizeForMulticastAddresses
        {
            get { return defaultAfdSendBufferSizeForMulticastAddresses; }
            set { defaultAfdSendBufferSizeForMulticastAddresses = value; }
        }

        public int DefaultUnicastSenderConcurrency
        {
            get { return defaultUnicastSenderConcurrency; }
            set { defaultUnicastSenderConcurrency = value; }
        }

        public int DefaultMulticastSenderConcurrency
        {
            get { return defaultMulticastSenderConcurrency; }
            set { defaultMulticastSenderConcurrency = value; }
        }
        
        #endregion

        #region ReceiveCallback

        private void ReceiveCallback(IPAddress ipAddress, int port, QS.Fx.Base.Block data, object callbackContext)
        {
            ListenerContext context = callbackContext as ListenerContext;
            if (context == null)
                throw new Exception("Internal error: Wrong context.");

            QS._core_c_.Base3.InstanceID senderAddress;
            uint destinationLOID;
            QS.Fx.Serialization.ISerializable receivedObject;

            QS._qss_c_.Base3_.Root.Decode(ipAddress, data, out senderAddress, out destinationLOID, out receivedObject);

#if DEBUG_DisplayAllReceivedObjects
            logger.Log("_____QS.CMS.Base8.Root : ReceiveCallback : " + QS.Fx.Printing.Printable.ToString(receivedObject));
#endif

            demultiplexer.dispatch(destinationLOID, senderAddress, receivedObject);
        }

        #endregion

        #region Class ListenerContext

        private class ListenerContext : Devices_3_.IListener
        {
            public ListenerContext(Root owner, QS.Fx.Network.NetworkAddress address)
            {
                this.owner = owner;
                this.address = address;
            }

            private QS.Fx.Network.NetworkAddress address;
            private Root owner;

#if DEBUG_UseOldInterfacesToCore
            private Core.IListener listener;
            public Core.IListener Listener
#else
            private QS.Fx.Network.IListener listener;
            public QS.Fx.Network.IListener Listener
#endif
            {
                get { return listener; }
                set { listener = value; }
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                try
                {
#if DEBUG_LogJoiningMulticastAddresses
                    owner.logger.Log("_____QS.CMS.Base8.Root.ListenerContext : QS.CMS.Devices3.IListener.Dispose : " + address.ToString());
#endif

                    owner.listenerContexts.Remove(address);

                    listener.Dispose();
                }
                catch (Exception exc)
                {
                    owner.logger.Log("While disposing of listener " + address.ToString() + " :\n" + exc.ToString());
                }
            }

            #endregion

            #region IListener Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Devices_2_.IListener.Address
            {
                get { return address; }
            }

            void QS._qss_c_.Devices_2_.IListener.shutdown()
            {
                ((IDisposable)this).Dispose();
            }

            #endregion
        }

        #endregion

        #region GetSender

        private SerializingSender GetSender(QS.Fx.Network.NetworkAddress address)
        {
            if (senders.ContainsKey(address))
                return senders[address];
            else
            {
#if DEBUG_UseOldInterfacesToCore                
                QS.CMS.Core.ISender underlyingSender = core.GetSender(new Core.Address(
                    localAddress.Address.HostIPAddress, address.HostIPAddress, address.PortNumber),
                    (address.IsMulticastAddress ? defaultAfdSendBufferSizeForMulticastAddresses : defaultAfdSendBufferSizeForUnicastAddresses),
                    (address.IsMulticastAddress ? defaultMulticastSenderConcurrency : defaultUnicastSenderConcurrency),
                    !address.IsMulticastAddress);
#else
                QS.Fx.Network.ISender underlyingSender = localNetworkInterface.GetSender(address,
                    new QS._core_x_.Base.Parameter<int>(QS._core_c_.Core.SenderInfo.Parameters.AdfBufferSize, (address.IsMulticastAddress ? 
                        defaultAfdSendBufferSizeForMulticastAddresses : defaultAfdSendBufferSizeForUnicastAddresses)),
                    new QS._core_x_.Base.Parameter<int>(QS._core_c_.Core.SenderInfo.Parameters.MaximumConcurrency, (address.IsMulticastAddress ? 
                        defaultMulticastSenderConcurrency : defaultUnicastSenderConcurrency)),
                    new QS._core_x_.Base.Parameter<bool>(QS._core_c_.Core.SenderInfo.Parameters.HighPriority, !address.IsMulticastAddress));
#endif

                SerializingSender sender = 
                    new SerializingSender(statisticsController, clock, logger, eventLogger, address, underlyingSender, localAddress);
                senders[address] = sender;

                ((QS._core_c_.Diagnostics2.IContainer)diagnosticsSenderContainer).Register(address.ToString(), ((QS._core_c_.Diagnostics2.IModule)sender).Component);

                return sender;
            }
        }

        #endregion

        #region ICollectionOf<NetworkAddress,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> 
            QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, 
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS.Fx.Network.NetworkAddress address]
        {
            get { return GetSender(address); }
        }

        #endregion

        #region ISenderCollection<IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender 
            Base3_.ISenderCollection<Base3_.IReliableSerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get 
            {
                foreach (QS.Fx.Network.NetworkAddress address in senders.Keys)
                    yield return ((QS.Fx.Serialization.IStringSerializable)address).AsString;
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get 
            {
                QS.Fx.Network.NetworkAddress address = new QS.Fx.Network.NetworkAddress();
                ((QS.Fx.Serialization.IStringSerializable)address).AsString = attributeName;
                return new QS.Fx.Inspection.ScalarAttribute(attributeName, senders[address]);
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Serializing Senders"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion

        #region ISenderCollection<ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region IMembershipController Members

        Devices_3_.IListener Devices_3_.IMembershipController.Join(QS.Fx.Network.NetworkAddress multicastAddress)
        {
#if DEBUG_LogJoiningMulticastAddresses
            logger.Log("_____QS.CMS.Base8.Root : QS.CMS.Devices3.IMembershipController.Join : " + multicastAddress.ToString());
#endif

            if (listenerContexts.ContainsKey(multicastAddress))
                throw new Exception("Already listening at " + multicastAddress.ToString());

            ListenerContext context = new ListenerContext(this, multicastAddress);
            listenerContexts.Add(multicastAddress, context);

#if DEBUG_UseOldInterfacesToCore
            context.Listener = core.Listen(new QS.CMS.Core.Address(
                localAddress.Address.HostIPAddress, multicastAddress.HostIPAddress, multicastAddress.PortNumber),
                new QS.CMS.Core.ReceiveCallback(this.ReceiveCallback), context, mtu,
                defaultNumberOfReceiveBuffersForMulticastAddresses, defaultDrainSynchronouslyForMulticastAddresses,
                defaultAfdReceiveBufferSizeForMulticastAddresses, false);
#else
            context.Listener = localNetworkInterface.Listen(
                multicastAddress, new QS.Fx.Network.ReceiveCallback(this.ReceiveCallback), context,
                new QS._core_x_.Base.Parameter<int>(
                    QS._core_c_.Core.ListenerInfo.Parameters.BufferSize, mtu),
                new QS._core_x_.Base.Parameter<int>(
                    QS._core_c_.Core.ListenerInfo.Parameters.NumberOfBuffers, defaultNumberOfReceiveBuffersForMulticastAddresses),
                new QS._core_x_.Base.Parameter<bool>(
                    QS._core_c_.Core.ListenerInfo.Parameters.DrainSynchronously, defaultDrainSynchronouslyForMulticastAddresses),
                new QS._core_x_.Base.Parameter<int>(
                    QS._core_c_.Core.ListenerInfo.Parameters.AdfBufferSize, defaultAfdReceiveBufferSizeForMulticastAddresses),
                new QS._core_x_.Base.Parameter<bool>(
                    QS._core_c_.Core.ListenerInfo.Parameters.HighPriority, false));
#endif

            return context;
        }

        #endregion

        #region ICollectionOf<NetworkAddress,IRateControlled> Members

        QS._core_c_.FlowControl3.IRateControlled QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.FlowControl3.IRateControlled>.this[QS.Fx.Network.NetworkAddress address]
        {
            get { return GetSender(address); }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (this.listenerContexts != null)
                {
                    foreach (ListenerContext _listener in this.listenerContexts.Values)
                    {
                        if ((_listener.Listener != null) && (_listener.Listener is IDisposable))
                            ((IDisposable)_listener.Listener).Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
