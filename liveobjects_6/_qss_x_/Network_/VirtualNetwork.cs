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

#define DEBUG_LogSomeStuff
// #define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Network_
{
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public sealed class VirtualNetwork : QS.Fx.Inspection.Inspectable, IVirtualNetwork, QS._qss_e_.Management_.IManagedComponent
    {
        #region Constructor

        /// <param name="bandwidth">Raw network bandwidth in bits per second.</param>
        /// <param name="recoverytime">Minimum time between subsequent frames.</param>
        public VirtualNetwork(QS._qss_c_.Base1_.Subnet subnet, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, 
            double bandwidth, double recoverytime, double lossrate)
        {
            this.subnet = subnet;
            this.clock = clock;
            this.alarmClock = alarmClock;            
            
            this.bandwidth = bandwidth;
            this.recoverytime = recoverytime;
            this.lossrate = lossrate;

            scheduleTransmissionAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.ScheduleTransmissionAlarmCallback);
            transmissionCompleteAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.TransmissionCompleteAlarmCallback);
            readyToTransmitAgainAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.ReadyToTransmitAgainAlarmCallback);

            logger = new QS._qss_c_.Base3_.Logger(clock, true, null);

#if DEBUG_LogSomeStuff
            logger.Log(this, "Initializing the virtual network: bandwidth " + 
                bandwidth.ToString() + " Mb/s, recovery " + recoverytime.ToString() + " s, loss rate " + lossrate.ToString() + ".");
#endif
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet subnet;

        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS._qss_c_.Base3_.Logger logger;

        private const int ETHERNET_FRAME = 1500;

        [QS.Fx.Base.Inspectable]
        private double bandwidth, recoverytime, lossrate;
        
        private IDictionary<System.Net.IPAddress, ICollection<IVirtualNetworkClient>> connections =
            new Dictionary<System.Net.IPAddress, ICollection<IVirtualNetworkClient>>();

        #region Status

        private enum Status
        {
            Ready, Transmitting, Recovering
        }

        #endregion

        private QS.Fx.Clock.AlarmCallback 
            scheduleTransmissionAlarmCallback, transmissionCompleteAlarmCallback, readyToTransmitAgainAlarmCallback;

        [QS.Fx.Base.Inspectable]
        private Status status;
        [QS.Fx.Base.Inspectable]
        private IVirtualPacket transmittedPacket;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.CompletionCallback<IVirtualPacket> transmittedPacketCompletionCallback;
        [QS.Fx.Base.Inspectable]
        private Queue<PacketTransmissionCallback> pendingCallbacks = new Queue<PacketTransmissionCallback>();

        private Random random = new Random();

        #endregion

        #region Updating network statistics

        private void _RegisterDrop(IVirtualPacket packet)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Dropping packet of " + transmittedPacket.Data.Count.ToString() + " bytes from " +
                transmittedPacket.From.ToString() + " to " + transmittedPacket.To.ToString() + ".");
#endif

            // ........................................
        }

        private void _RegisterLoss(IVirtualPacket packet)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Losing packet of " + transmittedPacket.Data.Count.ToString() + " bytes from " +
                transmittedPacket.From.ToString() + " to " + transmittedPacket.To.ToString() + ".");
#endif

            // ........................................
        }

        #endregion

        #region ReadyToTransmitAgainAlarmCallback

        private void ReadyToTransmitAgainAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                if (status != Status.Recovering)
                    logger.Log(this, "ReadyToTransmitAgainAlarmCallback : Internal error, status is not \"Recovering\" as expected.");

                status = Status.Ready;
                _ScheduleTransmission();
            }
        }

        #endregion

        #region TransmissionCompleteAlarmCallback

        private void TransmissionCompleteAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            IVirtualPacket transmittedPacket = null;
            QS.Fx.Base.CompletionCallback<IVirtualPacket> transmittedPacketCompletionCallback = null;

            ICollection<IVirtualNetworkClient> clients = null;

            lock (this)
            {
                if (status == Status.Transmitting)
                {
                    transmittedPacket = this.transmittedPacket;
                    transmittedPacketCompletionCallback = this.transmittedPacketCompletionCallback;

                    this.transmittedPacket = null;
                    this.transmittedPacketCompletionCallback = null;

                    if (random.NextDouble() > lossrate)
                    {
                        if (!connections.TryGetValue(transmittedPacket.To.HostIPAddress, out clients))
                            _RegisterDrop(transmittedPacket);
                    }
                    else
                        _RegisterLoss(transmittedPacket);

                    status = Status.Recovering;
                    alarmClock.Schedule(recoverytime, readyToTransmitAgainAlarmCallback, null);
                }
                else
                {
                    logger.Log(this, "TransmissionCompleteAlarmCallback : Internal error, status is not \"Transmitting\" as expected.");

                    status = Status.Ready;
                    _ScheduleTransmission();
                }
            }

            if (transmittedPacketCompletionCallback != null)
            {
#if DEBUG_LogGenerously
                logger.Log(this, "Invoking transmission completion callback.");
#endif

                transmittedPacketCompletionCallback(true, null, transmittedPacket);
            }

            if (clients != null)
            {
#if DEBUG_LogGenerously
                logger.Log(this, "Delivering packet of " + transmittedPacket.Data.Count.ToString() + " bytes from " + 
                    transmittedPacket.From.ToString() + " to " + transmittedPacket.To.ToString() + ".");
#endif

                IEnumerator<IVirtualNetworkClient> iclients = clients.GetEnumerator();
                if (iclients.MoveNext())
                {
                    IVirtualNetworkClient client = iclients.Current;
                    while (iclients.MoveNext())
                    {
#if DEBUG_LogGenerously
                        logger.Log(this, "Delivering packet of " + transmittedPacket.Data.Count.ToString() + " bytes from " +
                            transmittedPacket.From.ToString() + " to " + transmittedPacket.To.ToString() + " at client " + client.Name + ".");
#endif

                        client.Dispatch(clock.Time, transmittedPacket.Clone());
                        client = iclients.Current;
                    }

#if DEBUG_LogGenerously
                    logger.Log(this, "Delivering packet of " + transmittedPacket.Data.Count.ToString() + " bytes from " +
                        transmittedPacket.From.ToString() + " to " + transmittedPacket.To.ToString() + " at client " + client.Name + ".");
#endif

                    client.Dispatch(clock.Time, transmittedPacket);
                }
            }
        }

        #endregion

        #region _ScheduleTransmission

        private bool _ScheduleTransmission()
        {
            if (pendingCallbacks.Count > 0)
            {
                if (status == Status.Ready)
                {
                    PacketTransmissionCallback transmissionCallback;
                    do
                    {
                        transmissionCallback = pendingCallbacks.Dequeue();
                    }
                    while (!_ScheduleTransmission(transmissionCallback));
                }
                else
                    logger.Log(this, "_ScheduleTransmission : Internal error, status is not \"Ready\" as expected.");
            }

            return false;
        }

        private bool _ScheduleTransmission(PacketTransmissionCallback transmissionCallback)
        {
            if (status == Status.Ready)
            {
                bool more;
                transmissionCallback(out transmittedPacket, out transmittedPacketCompletionCallback, out more);

                if (transmittedPacket != null)
                {
                    if (more)
                        pendingCallbacks.Enqueue(transmissionCallback);

                    status = Status.Transmitting;

                    double transmission_time = Math.Ceiling(((double)transmittedPacket.Data.Count) / ((double)ETHERNET_FRAME)) *
                        ((double)ETHERNET_FRAME) * 8.0 / bandwidth;
                    alarmClock.Schedule(transmission_time, transmissionCompleteAlarmCallback, null);

#if DEBUG_LogGenerously
                    logger.Log(this, "Transmitting " + transmittedPacket.Data.Count.ToString() + " bytes from " + 
                        transmittedPacket.From.ToString() + " to " + transmittedPacket.To.ToString() + " in " + transmission_time.ToString() + " s.");
#endif

                    return true;
                }
                else
                {
                    if (more)
                    {
                        logger.Log(this, "Internal error, callback returned no packets, yet it claims it has more to send.");
                    }
                }
            }
            else
                pendingCallbacks.Enqueue(transmissionCallback);
                
            return false;
        }

        #endregion

        #region ScheduleTransmissionAlarmCallback

        private void ScheduleTransmissionAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                _ScheduleTransmission((PacketTransmissionCallback)alarmRef.Context);
            }
        }

        #endregion

        #region IVirtualNetwork Members

        void IVirtualNetwork.ScheduleTransmission(double time, PacketTransmissionCallback transmissionCallback)
        {
            lock (this)
            {
                double delay = time - clock.Time;
                if (delay > 0)
                    alarmClock.Schedule(
                        delay, new QS.Fx.Clock.AlarmCallback(scheduleTransmissionAlarmCallback), transmissionCallback);
                else
                    _ScheduleTransmission(transmissionCallback);
            }
        }

        System.Net.IPAddress IVirtualNetwork.Connect(IVirtualNetworkClient client)
        {
            lock (this)  
            {
                foreach (KeyValuePair<System.Net.IPAddress, ICollection<IVirtualNetworkClient>> element in connections)
                {
                    if (!QS.Fx.Network.NetworkAddress.IsMulticastIPAddress(element.Key))
                    {
                        foreach (IVirtualNetworkClient other in element.Value)
                        {
                            if (!ReferenceEquals(other, client) && other.Name.Equals(client.Name))
                                throw new Exception("Cannot connect client \"" + client.Name + "\", duplicate name exists on the network.");
                        }
                    }
                }

                foreach (System.Net.IPAddress address in subnet.Addresses)
                {
                    if (!connections.Keys.Contains(address))
                    {
                        ICollection<IVirtualNetworkClient> clients = new System.Collections.ObjectModel.Collection<IVirtualNetworkClient>();
                        clients.Add(client);
                        connections.Add(address, clients);

#if DEBUG_LogSomeStuff
                        logger.Log(this, "Connected client \"" + client.Name + "\" as " + address.ToString());
#endif

                        return address;
                    }
                }

/*
                System.Net.IPAddress address;
                for (int attemptno = 0; attemptno < 100; attemptno++)
                {
                    address = subnet.RandomAddress;
                    if (!connections.Keys.Contains(address))             
                    {
                        ICollection<IVirtualNetworkClient> clients = new System.Collections.ObjectModel.Collection<IVirtualNetworkClient>();
                        clients.Add(client);
                        connections.Add(address, clients);

#if DEBUG_LogGenerously
                        logger.Log(this, "Connected client \"" + client.Name + "\" as " + address.ToString());
#endif

                        return address;
                    }
                }
*/

#if DEBUG_LogSomeStuff
                logger.Log(this, "Cannot connect client \"" + client.Name + "\", unable to find unallocated address.");
#endif

               throw new Exception("Cannot connect client \"" + client.Name + "\", unable to find unallocated address.");
            }
        }

        void IVirtualNetwork.Connect(IVirtualNetworkClient client, System.Net.IPAddress address)
        {
            lock (this)
            {
                ICollection<IVirtualNetworkClient> clients;
                if (!connections.TryGetValue(address, out clients))
                {
                    clients = new System.Collections.ObjectModel.Collection<IVirtualNetworkClient>();
                    connections.Add(address, clients);
                }

                if (clients.Contains(client))
                {
#if DEBUG_LogSomeStuff
                    logger.Log(this, "Cannot register client \"" + client.Name + "\" for multicast address " + address.ToString() + ", it is already registered.");
#endif

                    throw new Exception("Already connected.");
                }

                clients.Add(client);

#if DEBUG_LogSomeStuff
                logger.Log(this, "Registered client \"" + client.Name + "\" for multicast address " + address.ToString());
#endif
            }
        }

        void IVirtualNetwork.Disconnect(IVirtualNetworkClient client, System.Net.IPAddress address)
        {
            lock (this)
            {
                ICollection<IVirtualNetworkClient> clients;
                if (!connections.TryGetValue(address, out clients) || !clients.Remove(client))
                {
#if DEBUG_LogSomeStuff
                    logger.Log(this, "Cannot unregister client \"" + client.Name + "\" from multicast address " + address.ToString() + ", it is not registered.");
#endif

                    throw new Exception("Not connected.");
                }

#if DEBUG_LogSomeStuff
                logger.Log(this, "Unregistered client \"" + client.Name + "\" from multicast address " + address.ToString());
#endif
            }
        }

        System.Net.IPHostEntry IVirtualNetwork.GetHostEntry(string hostname)
        {
            try
            {
                System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(hostname);
                return ((IVirtualNetwork)this).GetHostEntry(ipAddress);
            }
            catch (Exception)
            {
                lock (this)
                {
                    List<System.Net.IPAddress> addresses = new List<System.Net.IPAddress>();

                    foreach (KeyValuePair<System.Net.IPAddress, ICollection<IVirtualNetworkClient>> element in connections)
                    {
                        if (!QS.Fx.Network.NetworkAddress.IsMulticastIPAddress(element.Key))
                        {
                            foreach (IVirtualNetworkClient client in element.Value)
                            {
                                if (client.Name.Equals(hostname))
                                    addresses.Add(element.Key);
                            }
                        }
                    }

                    System.Net.IPHostEntry entry = new System.Net.IPHostEntry();
                    entry.HostName = hostname;
                    entry.AddressList = addresses.ToArray();
                    entry.Aliases = new string[0];
                    return entry;
                }
            }
        }

        System.Net.IPHostEntry IVirtualNetwork.GetHostEntry(System.Net.IPAddress address)
        {
            if (!QS.Fx.Network.NetworkAddress.IsMulticastIPAddress(address))
            {
                lock (this)
                {
                    ICollection<IVirtualNetworkClient> clients;
                    if (connections.TryGetValue(address, out clients) && clients.Count > 0)
                    {
                        IEnumerator<IVirtualNetworkClient> en = clients.GetEnumerator();
                        if (en.MoveNext())                        
                            return ((IVirtualNetwork)this).GetHostEntry(en.Current.Name);
                    }
                    
                    throw new Exception("Cannot resolve address " + address.ToString() + ".");
                }
            }
            else
                throw new Exception("Cannot get host entry for multicast address " + address.ToString() + ".");
        }

        #endregion

        #region IManagedComponent Members

        string QS._qss_e_.Management_.IManagedComponent.Name
        {
            get { return "Network " + subnet.ToString(); }
        }

        QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
        {
            get { return new QS._qss_e_.Management_.IManagedComponent[0]; }
        }

        QS._core_c_.Base.IOutputReader QS._qss_e_.Management_.IManagedComponent.Log
        {
            get { return logger; }
        }

        object QS._qss_e_.Management_.IManagedComponent.Component
        {
            get { return this; }
        }

        #endregion
    }
}
