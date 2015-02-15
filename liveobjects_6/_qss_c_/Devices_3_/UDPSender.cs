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

#define OPTION_DisableScatterGatherForMonoCompatibility

// #define DEBUG_SendSynchronously
// #define DEBUG_AllowLoggingPacketsWhileSending
// #define STATISTICS_TimeToSend

// #define Check_Outgoing_Data_Size
// #define DEBUG_Retries
// #define DEBUG_PacketTooBigErrors
// #define STATISTICS_TrackSocketAllocations
// #define STATISTICS_AllocationErrors
// #define STATISTICS_SendingErrors
// #define STATISTICS_PacketTooBigErrors

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QS._qss_c_.Devices_3_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class UDPSender : QS.Fx.Inspection.Inspectable, ISender, System.IDisposable, QS._qss_d_.Monitoring_.IListener
    {
        private const int MaximumRetryTimeoutInMilliseconds = 100;

        public UDPSender(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, 
            UDPCommunicationsDevice device, QS.Fx.Network.NetworkAddress destinationAddress)
        {
            this.logger = logger;
            this.device = device;
            asyncCallback = new AsyncCallback(asynchronousSendCallback);
            this.destinationAddress = destinationAddress;
            socketQueue = new Queue<Socket>();
            addressClass = QS._qss_c_.Devices_2_.UDPReceiver.ClassOfAddress(destinationAddress.HostIPAddress);
            this.clock = clock;
        }

        private UDPCommunicationsDevice device;
        private AsyncCallback asyncCallback;
        private QS.Fx.Network.NetworkAddress destinationAddress;
        private System.Collections.Generic.Queue<Socket> socketQueue;
        private Devices_2_.UDPReceiver.Class addressClass;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;

        // private bool logPacketsWhileSending = false;

#if DEBUG_AllowLoggingPacketsWhileSending
        private IList<KeyValuePair<double, IList<ArraySegment<byte>>>> loggedPackets = 
            new List<KeyValuePair<double, IList<ArraySegment<byte>>>>();

        private QS.HMS.Monitoring.IAnalyzer analyzer;

        [QS.TMS.Inspection.Inspectable("Analyzer", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
        public QS.HMS.Monitoring.IAnalyzer Analyzer
        {
            get
            {
                lock (this)
                {
                    if (analyzer == null)
                    {
                        analyzer = new QS.HMS.Monitoring.Analyzers.Analyzer1();
                        analyzer.DataSource = this;
                    }
                    return analyzer;
                }
            }
        }
#endif

        #region QS.HMS.Monitoring.IListener Members

        IEnumerable<QS._qss_d_.Monitoring_.IPacket> QS._qss_d_.Monitoring_.IListener.Received
        {
            get 
            {
                lock (this)
                {
                    List<QS._qss_d_.Monitoring_.IPacket> packets = new List<QS._qss_d_.Monitoring_.IPacket>();
#if DEBUG_AllowLoggingPacketsWhileSending
                    foreach (KeyValuePair<double, IList<ArraySegment<byte>>> element in loggedPackets)
                    {
                        int totalsize = 0;
                        foreach (ArraySegment<byte> segment in element.Value)
                            totalsize += segment.Count;
                        byte[] consolidated = new byte[totalsize];
                        int offset = 0;
                        foreach (ArraySegment<byte> segment in element.Value)
                        {
                            Buffer.BlockCopy(segment.Array, segment.Offset, consolidated, offset, segment.Count);
                            offset += segment.Count;
                        }

                        CMS.QS._core_c_.Base3.InstanceID senderAddress;
                        uint channel;
                        QS.Fx.Serialization.ISerializable receivedObject;
                        CMS.Base3.Root.Decode(
                            IPAddress.Any, new ArraySegment<byte>(consolidated), out senderAddress, out channel, out receivedObject);

                        packets.Add(new QS.HMS.Monitoring.Packet(
                            senderAddress, destinationAddress, channel, element.Key, receivedObject));
                    }
#endif                    
                    return packets;
                }
            }
        }

        #endregion

#if STATISTICS_TrackSocketAllocations
        //        [TMS.Inspection.Inspectable("Socket Queue Sizes")]
//        private QS.CMS.Statistics.SamplesXY timeSeries_socketQueueSizes = new QS.CMS.Statistics.SamplesXY();
        [TMS.Inspection.Inspectable("Total Number of Allocated Sockets")]
        private uint totalAllocatedSockets;
        [TMS.Inspection.Inspectable("Number of Sockets Currently Used")]
        private uint currentlyUsedSockets;

        [QS.CMS.Diagnostics.Component("History of the Total Number of Allocated Sockets")]
        private QS.CMS.Statistics.SamplesXY timeSeries_totalAllocatedSockets = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("History of the Number of Sockets Currently Used")]
        private QS.CMS.Statistics.SamplesXY timeSeries_currentlyUsedSockets = new QS.CMS.Statistics.SamplesXY();
#endif

#if STATISTICS_AllocationErrors
        [QS.CMS.Diagnostics.Component("Allocation Error Times")]
        private QS.CMS.Statistics.Samples timeSeries_allocationErrorTimes = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_SendingErrors
        [QS.CMS.Diagnostics.Component("Sending Error Times")]
        private QS.CMS.Statistics.Samples timeSeries_sendingErrorTimes = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_PacketTooBigErrors
        [QS.CMS.Diagnostics.Component("Packet Too Big Errors")]
        private QS.CMS.Statistics.SamplesXY timeSeries_packetTooBigErrors = new QS.CMS.Statistics.SamplesXY();
#endif

#if STATISTICS_TimeToSend
        [QS.CMS.Diagnostics.Component("Time To Send")]
        private QS.CMS.Statistics.Samples timeSeries_timeToSend = new QS.CMS.Statistics.Samples();
#endif

//        public override IComparable Contents
//        {
//            get { return destinationAddress; }
//        }

        private Socket AllocateSocket
        {
            get
            {
                int retry_timeout = 1;
                while (true)
                {
                    lock (socketQueue)
                    {
                        if (socketQueue.Count > 0)
                        {
#if STATISTICS_TrackSocketAllocations
                            currentlyUsedSockets++;
                            timeSeries_currentlyUsedSockets.addSample(clock.Time, currentlyUsedSockets);
#endif
                            return socketQueue.Dequeue();
                        }
                    }

                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        IPEndPoint endpoint = new IPEndPoint(device.Address, 0);

                        socket.Bind(endpoint);

                        if (destinationAddress.HostIPAddress.Equals(IPAddress.Broadcast))
                            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                        else
                        {
                            byte firstbyte = (destinationAddress.HostIPAddress.GetAddressBytes())[0];
                            if (firstbyte > 223 && firstbyte < 240)
                            {
                                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);                                
                            }
                        }

                        socket.Connect(new IPEndPoint(destinationAddress.HostIPAddress, destinationAddress.PortNumber));

#if STATISTICS_TrackSocketAllocations
                        lock (socketQueue)
                        {
                            totalAllocatedSockets++;
                            currentlyUsedSockets++;
                            double now = clock.Time;
                            timeSeries_totalAllocatedSockets.addSample(now, totalAllocatedSockets);
                            timeSeries_currentlyUsedSockets.addSample(now, currentlyUsedSockets);
                        }
#endif

                        return socket;
                    }
                    catch (System.Net.Sockets.SocketException socketException)
                    {
                        if (socketException.ErrorCode == (int)WINSOCK2.ErrorCode.WSAENOBUFS)
                        {
#if STATISTICS_AllocationErrors                        
                            timeSeries_allocationErrorTimes.addSample(clock.Time);
#endif

#if DEBUG_Retries
                            logger.Log(this, "UDPSender(" + destinationAddress.ToString() +
                                ").AllocateSocket : Retrying socket allocation after a short while...");
#endif

                            System.Threading.Thread.Sleep(retry_timeout);
                            retry_timeout = retry_timeout * 2;
                            if (retry_timeout > MaximumRetryTimeoutInMilliseconds)
                                retry_timeout = MaximumRetryTimeoutInMilliseconds;
                        }
                        else
                            throw;
                    }
                }
            }
        }

        private void recycleSocket(Socket socket)
        {
            lock (socketQueue)
            {
                socketQueue.Enqueue(socket);

#if STATISTICS_TrackSocketAllocations
                currentlyUsedSockets--;
                timeSeries_currentlyUsedSockets.addSample(clock.Time, currentlyUsedSockets);
#endif
            }
        }

        private void asynchronousSendCallback(System.IAsyncResult asyncResult)
        {
            Socket socket = (Socket) asyncResult.AsyncState;
            socket.EndSend(asyncResult);
            recycleSocket(socket);
        }

        #region ISender Members

        public ICommunicationsDevice CommunicationsDevice
        {
            get { return device; }
        }

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return destinationAddress; }
        }

        public void send(IList<QS.Fx.Base.Block> buffers)
        {
#if Check_Outgoing_Data_Size
			int total_size = 0;
			foreach (ArraySegment<byte> buffer in buffers)
				total_size += buffer.Count;
            if (total_size > device.MTU)
            {
#if STATISTICS_PacketTooBigErrors
                timeSeries_packetTooBigErrors.addSample(clock.Time, total_size);
#endif

#if DEBUG_PacketTooBigErrors
                throw new ArgumentException("Cannot send " + total_size.ToString() + " bytes, device MTU = " + device.MTU.ToString() + ".");
#else
                return;
#endif
            }
#endif
            
            bool should_retry;
            int retry_timeout = 1;
            do
            {
                try
                {
                    Socket socket = this.AllocateSocket;

#if STATISTICS_TimeToSend || DEBUG_AllowLoggingPacketsWhileSending
                    double t1 = clock.Time;
#endif

#if DEBUG_SendSynchronously
                    socket.Send(buffers, SocketFlags.None);
                    recycleSocket(socket);
#else
#if OPTION_DisableScatterGatherForMonoCompatibility
                    byte[] flattened_buffers = Base3_.BufferHelper.FlattenBuffers(buffers);
                    socket.BeginSend(flattened_buffers, 0, flattened_buffers.Length, SocketFlags.None, asyncCallback, socket);                    
#else
                    socket.BeginSend(buffers, SocketFlags.None, asyncCallback, socket);                    
#endif
#endif

#if STATISTICS_TimeToSend
                    double t2 = clock.Time;
                    lock (timeSeries_timeToSend)
                    {
                        timeSeries_timeToSend.addSample(t2 - t1);
                    }
#endif

#if DEBUG_AllowLoggingPacketsWhileSending
                    lock (loggedPackets)
                    {
                        loggedPackets.Add(new KeyValuePair<double, IList<ArraySegment<byte>>>(t1, buffers));
                    }
#endif

                    should_retry = false;
                }
                catch (System.Net.Sockets.SocketException socketException)
                {
                    if (socketException.ErrorCode == (int)WINSOCK2.ErrorCode.WSAENOBUFS)
                    {
#if STATISTICS_SendingErrors
                        timeSeries_sendingErrorTimes.addSample(clock.Time);
#endif

#if DEBUG_Retries
                        logger.Log(this, "UDPSender(" + destinationAddress.ToString() + ") : Retrying...");
#endif

                        should_retry = true;
                        System.Threading.Thread.Sleep(retry_timeout);
                        retry_timeout = retry_timeout * 2;
                        if (retry_timeout > MaximumRetryTimeoutInMilliseconds)
                            retry_timeout = MaximumRetryTimeoutInMilliseconds;
                    }
                    else
                        throw new Exception("__send(" + destinationAddress.ToString() +
#if Check_Outgoing_Data_Size
                            ", " + total_size.ToString() + " bytes" +
#endif
                            ") : Socket exception", socketException);
                }
                catch (System.Exception exception)
                {
                    throw new Exception("__send(" + destinationAddress.ToString() + 
#if Check_Outgoing_Data_Size
                        ", " + total_size.ToString() + " bytes" +
#endif
                        ") : System exception", exception);
                }
            }
            while (should_retry);
        }

        public IAsyncResult BeginSend(IList<QS.Fx.Base.Block> buffers, AsyncCallback callback, object asyncState)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region System.IDisposable Members

        public void Dispose()
        {
            lock (socketQueue)
            {
                while (socketQueue.Count > 0)
                    (socketQueue.Dequeue()).Close();
            }
        }

        #endregion
    }
}
