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

// #define DEBUG_AllowCollectingOfStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Base7_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Root : QS.Fx.Inspection.Inspectable, Devices_3_.IMembershipController
    {
        public Root(QS.Fx.Logging.IEventLogger eventLogger, QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IClock clock, 
            Devices_7_.IConnections connections, Scheduling_1_.IScheduler scheduler, Base3_.IDemultiplexer demultiplexer)
        {
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;
            this.clock = clock;
            this.scheduler = scheduler;
            this.demultiplexer = demultiplexer;

            connection = connections.Connections[localAddress.Address.HostIPAddress];

            mainListener = connection.Listeners[localAddress.Address];
            listeners.Add(localAddress.Address, mainListener);

            mainListener.OnArrival += new QS._qss_c_.Devices_7_.ListenerCallback(this.ArrivalCallback);
            mainListener.Start();

            iw_listeners =
                new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, QS._qss_c_.Devices_7_.IListener>(
                    "Listeners", listeners);
        }

        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Clock.IClock clock;
        private Devices_7_.IConnection connection;        
        private Scheduling_1_.IScheduler scheduler;
        private Base3_.IDemultiplexer demultiplexer;
        [QS._core_c_.Diagnostics.Component("Root Listener")]
        private Devices_7_.IListener mainListener;
        [QS._core_c_.Diagnostics.ComponentCollection("Registered Listeners")]
        private IDictionary<QS.Fx.Network.NetworkAddress, Devices_7_.IListener> listeners = 
            new Dictionary<QS.Fx.Network.NetworkAddress, Devices_7_.IListener>();
        [QS.Fx.Base.Inspectable("Listeners")]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, Devices_7_.IListener> iw_listeners;

#if DEBUG_AllowCollectingOfStatistics
        [Diagnostics.Component("Deserialization Times and Overheads")]
        private Statistics.SamplesXY timeSeries_deserializationTimesAndOverheads = new QS.CMS.Statistics.SamplesXY();
#endif

        public void Start(QS.Fx.Network.NetworkAddress address)
        {
            lock (this)
            {
                if (listeners.ContainsKey(address))
                    throw new Exception("Already listening at " + address.ToString());

                Devices_7_.IListener listener = connection.Listeners[address];
                listeners.Add(address, listener);

                listener.OnArrival += new QS._qss_c_.Devices_7_.ListenerCallback(this.ArrivalCallback);
                listener.Start();
            }
        }

        public void Stop(QS.Fx.Network.NetworkAddress address)
        {
            lock (this)
            {
                if (address.Equals(localAddress.Address))
                    throw new Exception("Cannot stop listening at the main app address.");

                Devices_7_.IListener listener;
                if (!listeners.TryGetValue(address, out listener))
                    throw new Exception("Not listening at " + address.ToString());

                listeners.Remove(address);
                listener.Stop();
            }
        }

        #region Dispatch

        private void Dispatch(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.Message message)
        {
            demultiplexer.dispatch(message.destinationLOID, sourceAddress, message.transmittedObject);
        }

        #endregion

        #region Class MyEvent

        private class MyEvent : Scheduling_1_.Event
        {
            public MyEvent(Root owner, QS._core_c_.Base3.InstanceID sender, QS._core_c_.Base3.Message message)
            {
                this.owner = owner;
                this.sender = sender;
                this.message = message;
            }

            private Root owner;
            private QS._core_c_.Base3.InstanceID sender;
            private QS._core_c_.Base3.Message message;

            protected override void Process()
            {
                owner.Dispatch(sender, message);                
            }
        }

        #endregion

        #region ArrivalCallback

        private void ArrivalCallback(QS._qss_c_.Devices_7_.IListener listener)
        {
            Synchronization_1_.ChainOf<Scheduling_1_.IEvent> eventChain = new Synchronization_1_.ChainOf<Scheduling_1_.IEvent>();

            foreach (Devices_7_.IPacket packet in listener.Received)            
            {
                try
                {
#if DEBUG_AllowCollectingOfStatistics
                    double t1 = clock.Time;
#endif

                    QS._core_c_.Base3.InstanceID sender;
                    uint channel;
                    QS.Fx.Serialization.ISerializable data;

                    Base3_.Root.Decode(listener.NIC, new QS.Fx.Base.Block(packet.Data), out sender, out channel, out data);

#if DEBUG_AllowCollectingOfStatistics
                    double t2 = clock.Time;
                    if (timeSeries_deserializationTimesAndOverheads.Enabled)
                        timeSeries_deserializationTimesAndOverheads.addSample(t1, t2 - t1);
#endif

                    eventChain.Add(new MyEvent(this, sender, new QS._core_c_.Base3.Message(channel, data)));
                }
                catch (Exception exc)
                {
                    if (eventLogger.Enabled)
                        eventLogger.Log(new Logging_1_.Events.ExceptionCaught(clock.Time, listener.NIC.ToString(), exc));
                }
            }

            scheduler.Schedule(eventChain);
            listener.Start();

            scheduler.Work();
        }

        #endregion

        #region IMembershipController Members

        QS._qss_c_.Devices_3_.IListener QS._qss_c_.Devices_3_.IMembershipController.Join(QS.Fx.Network.NetworkAddress multicastAddress)
        {
            this.Start(multicastAddress);
            return new D3L(this, multicastAddress);
        }

        private class D3L : Devices_3_.IListener
        {
            public D3L(Root owner, QS.Fx.Network.NetworkAddress address)
            {
                this.owner = owner;
                this.address = address;
            }

            private Root owner;
            private QS.Fx.Network.NetworkAddress address;

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                owner.Stop(address);
            }

            #endregion

            #region IListener Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Devices_2_.IListener.Address
            {
                get { return address; }
            }

            void QS._qss_c_.Devices_2_.IListener.shutdown()
            {
                owner.Stop(address);
            }

            #endregion
        }

        #endregion
    }
}
