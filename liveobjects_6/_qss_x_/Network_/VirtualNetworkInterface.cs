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
    public sealed class VirtualNetworkInterface : QS.Fx.Inspection.Inspectable, QS.Fx.Network.INetworkInterface, IVirtualNetworkClient
    {
        public VirtualNetworkInterface(string name, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock,
            QS.Fx.Scheduling.IScheduler scheduler, QS._core_c_.Statistics.IStatisticsController statisticsController, IVirtualNetwork network)
        {
            this.name = name;
            this.statisticsController = statisticsController;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.network = network;
            this.scheduler = scheduler;

            logger = new QS._core_c_.Base.Logger(clock, true);

            senderController = new VirtualSenderController(logger, clock, alarmClock, statisticsController, network);
            
            dispatchCallback = new AsyncCallback(this.DispatchCallback);
            deliveryCallback = new QS.Fx.Clock.AlarmCallback(this.DeliveryCallback);

            interfaceAddress = network.Connect(this);
        }

        #region Class PortInfo

        private class PortInfo
        {
            public PortInfo(PortType type, params System.Net.IPAddress[] addresses)
            {
                this.type = type;

                if (addresses.Length > 0)
                {
                    this.addresses = new System.Collections.ObjectModel.Collection<System.Net.IPAddress>();
                    foreach (System.Net.IPAddress address in addresses)
                        this.addresses.Add(address);
                }
            }

            public enum PortType
            {
                Sending, Receiving
            }

            private PortType type;
            private ICollection<System.Net.IPAddress> addresses;

            public PortType Type
            {
                get { return type; }
            }

            public ICollection<System.Net.IPAddress> Addresses
            {
                get { return addresses; }
            }
        }

        #endregion

        #region Class AddressInfo

        private class AddressInfo
        {
            public AddressInfo()
            {
            }

            private ICollection<int> ports = new System.Collections.ObjectModel.Collection<int>();

            public ICollection<int> Ports
            {
                get { return ports; }
            }
        }

        #endregion

        #region Struct DispatchInfo

        private struct DispatchInfo
        {
            public DispatchInfo(double time, IVirtualPacket packet)
            {
                this.time = time;
                this.packet = packet;
            }

            private double time;
            private IVirtualPacket packet;

            public double Time
            {
                get { return time; }
            }

            public IVirtualPacket Packet
            {
                get { return packet; }
            }
        }

        #endregion

        private const int MinimumRandomPortNumber = 55000, MaximumRandomPortNumber = 60000;

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private IVirtualNetwork network;
        private string name;
        private QS.Fx.Scheduling.IScheduler scheduler;

        [QS.Fx.Base.Inspectable]
        private QS._core_c_.Base.Logger logger;
        [QS.Fx.Base.Inspectable]
        private System.Net.IPAddress interfaceAddress;
        [QS.Fx.Base.Inspectable]
        private VirtualSenderController senderController;
        [QS.Fx.Base.Inspectable]
        private IDictionary<QS.Fx.Network.NetworkAddress, VirtualSender> senders =
            new Dictionary<QS.Fx.Network.NetworkAddress, VirtualSender>();
        [QS.Fx.Base.Inspectable]
        private IDictionary<QS.Fx.Network.NetworkAddress, VirtualListener> listeners =
            new Dictionary<QS.Fx.Network.NetworkAddress, VirtualListener>();

        private static readonly Random random = new Random();
        private IDictionary<int, PortInfo> ports = new Dictionary<int, PortInfo>();
        private IDictionary<System.Net.IPAddress, AddressInfo> addresses = new Dictionary<System.Net.IPAddress, AddressInfo>();

        private Queue<DispatchInfo> toDispatch = new Queue<DispatchInfo>();
        private AsyncCallback dispatchCallback;
        private QS.Fx.Clock.AlarmCallback deliveryCallback;

        #region INetworkInterface Members

        System.Net.IPAddress QS.Fx.Network.INetworkInterface.InterfaceAddress
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.ISender QS.Fx.Network.INetworkInterface.GetSender(QS.Fx.Network.NetworkAddress address, params QS.Fx.Base.IParameter[] parameters)
        {
            lock (this)
            {
                int portno = 0;
                bool portok = false;
                for (int countdown = 1000; countdown > 0 &&
                    !(portok = !ports.ContainsKey(portno = random.Next(MinimumRandomPortNumber, MaximumRandomPortNumber))); countdown--)
                    ;

                if (portok)
                {
                    ports.Add(portno, new PortInfo(PortInfo.PortType.Sending));

                    QS._core_c_.RateControl.IRateController rateController = 
                        new QS._core_c_.RateControl.RateController1(clock, alarmClock, (address.IsMulticastAddress ? 10000 : 300), 
                        (address.IsMulticastAddress ? 500 : 100), statisticsController);

                    VirtualSender sender = new VirtualSender(interfaceAddress, portno, address, senderController, rateController, logger);
                    senders.Add(address, sender);

                    return sender;
                }
                else
                    throw new Exception("Cannot connect, unable to find an unused port number.");
            }
        }

        QS.Fx.Network.IListener QS.Fx.Network.INetworkInterface.Listen(QS.Fx.Network.NetworkAddress address, QS.Fx.Network.ReceiveCallback callback, object context, 
            params QS.Fx.Base.IParameter[] parameters)
        {
            lock (this)
            {
                bool ismulticast = address.IsMulticastAddress;
                if (!ismulticast && !address.HostIPAddress.Equals(interfaceAddress))
                    throw new Exception("Cannot listen, the address to listen at is neither a multicast address nor the local interface address.");

                PortInfo portinfo;
                if (address.PortNumber == 0)
                {
                    int portno = 0;
                    bool portok = false;
                    for (int countdown = 1000; countdown > 0 &&
                        !(portok = !ports.ContainsKey(portno = random.Next(MinimumRandomPortNumber, MaximumRandomPortNumber))); countdown--)
                        ;

                    if (portok)
                    {
                        address.PortNumber = portno;
                        portinfo = new PortInfo(PortInfo.PortType.Receiving, address.HostIPAddress);
                        ports.Add(address.PortNumber, portinfo);
                    }
                    else
                        throw new Exception("Cannot connect, unable to find an unused port number.");
                }
                else
                {
                    if (ports.TryGetValue(address.PortNumber, out portinfo))
                    {
                        if (portinfo.Type == PortInfo.PortType.Receiving)
                        {
                            if (portinfo.Addresses.Contains(address.HostIPAddress))
                                throw new Exception("Already listening at " + address.ToString() + ".");
                            portinfo.Addresses.Add(address.HostIPAddress);
                        }
                        else
                            throw new Exception("Cannot listen at this port, this port is already in use by a sending socket.");
                    }
                    else
                    {
                        portinfo = new PortInfo(PortInfo.PortType.Receiving, address.HostIPAddress);
                        ports.Add(address.PortNumber, portinfo);
                    }
                }

                if (listeners.ContainsKey(address))
                    throw new Exception("Internal error: address " + address.ToString() + " is in use (althought the port is not registered as in use).");

                VirtualListener listener = new VirtualListener(interfaceAddress, address, callback, context, logger);
                listeners.Add(address, listener);

                if (address.IsMulticastAddress)
                {
                    AddressInfo addressinfo;
                    if (addresses.TryGetValue(address.HostIPAddress, out addressinfo))
                    {
                        if (addressinfo.Ports.Contains(address.PortNumber))
                            throw new Exception("Internal error: address " + address.ToString() + " is in use.");
                    }
                    else
                    {
                        addresses.Add(address.HostIPAddress, addressinfo = new AddressInfo());
                        network.Connect(this, address.HostIPAddress);
                    }

                    addressinfo.Ports.Add(address.PortNumber);
                }

                return listener;
            }
        }

        #endregion

        #region IVirtualNetworkClient Members

        string IVirtualNetworkClient.Name
        {
            get { return name; }
        }

        void IVirtualNetworkClient.Dispatch(double time, IVirtualPacket packet)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "_Dispatch(" + time.ToString() + ")\n" + QS.Fx.Printing.Printable.ToString(packet));
#endif

            lock (this)
            {
                toDispatch.Enqueue(new DispatchInfo(time, packet));
                scheduler.Execute(dispatchCallback, null);
            }
        }

        #endregion

        #region DispatchCallback

        private void DispatchCallback(IAsyncResult result)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "_DispatchCallback");
#endif

            lock (this)
            {
                // .................perhaps add some packet dropping ?????????????????????????

                while (toDispatch.Count > 0)
                {
                    DispatchInfo dispatchInfo = toDispatch.Dequeue();
                    double delay = dispatchInfo.Time - clock.Time;
                    alarmClock.Schedule((delay > 0) ? delay : 0, deliveryCallback, dispatchInfo.Packet);
                }
            }
        }

        #endregion
        
        #region DeliveryCallback

        private void DeliveryCallback(QS.Fx.Clock.IAlarm alarm)
        {
            _Deliver((IVirtualPacket) alarm.Context);
        }

        #endregion

        #region _Deliver

        private void _Deliver(IVirtualPacket packet)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "_Deliver : " + QS.Fx.Printing.Printable.ToString(packet));
#endif

            VirtualListener listener;
            lock (this)
            {
                if (!listeners.TryGetValue(packet.To, out listener))
                    listener = null;
            }

            if (listener != null)
                ((IVirtualListener)listener).Dispatch(packet);
        }

        #endregion

        #region Reset

        public void Reset()
        {
            lock (this)
            {
#if DEBUG_LogGenerously
                logger.Log("Resetting network interface " + interfaceAddress.ToString() + " at \"" + name + "\".");
#endif

                foreach (VirtualSender sender in senders.Values)
                {
                    sender.Reset();
                }

                foreach (VirtualListener listener in listeners.Values)
                {
                    listener.Reset();
                }

                foreach (System.Net.IPAddress address in addresses.Keys)
                {
#if DEBUG_LogGenerously
                    logger.Log("Disconnecting interface " + interfaceAddress.ToString() + " from address " + address.ToString() + ".");
#endif

                    network.Disconnect(this, address);
                }

                senders.Clear();
                listeners.Clear();
                ports.Clear();
                addresses.Clear();
                toDispatch.Clear();

                senderController.Reset();
            }
        }

        #endregion
    }
}
