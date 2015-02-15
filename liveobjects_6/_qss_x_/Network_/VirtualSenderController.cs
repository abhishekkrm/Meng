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
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public sealed class VirtualSenderController : QS.Fx.Inspection.Inspectable, IVirtualSenderController
    {
        public VirtualSenderController(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, 
            QS._core_c_.Statistics.IStatisticsController statisticsController, IVirtualNetwork network)
        {
            this.logger = logger;
            this.statisticsController = statisticsController;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.network = network;

            packetTransmissionCallback = new PacketTransmissionCallback(this.PacketTransmissionCallback);
            transmissionCompletionCallback = new QS.Fx.Base.CompletionCallback<IVirtualPacket>(this.TransmissionCompletionCallback);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private IVirtualNetwork network;

        [QS.Fx.Base.Inspectable]
        private int bufferedPackets, maximumBufferedPackets = 100;
        [QS.Fx.Base.Inspectable]
        private int bufferedBytes, maximumBufferedBytes = 1000000;

        [QS.Fx.Base.Inspectable]
        private Queue<IVirtualSender> senders = new Queue<IVirtualSender>();
        [QS.Fx.Base.Inspectable]
        private bool registeredToSend;
        [QS.Fx.Base.Inspectable]
        private Queue<Request> requests = new Queue<Request>();

        private Queue<QS.Fx.Network.AsynchronousSend> temp = new Queue<QS.Fx.Network.AsynchronousSend>();
        private PacketTransmissionCallback packetTransmissionCallback;
        private QS.Fx.Base.CompletionCallback<IVirtualPacket> transmissionCompletionCallback;

        #region Request

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private struct Request
        {
            public Request(IVirtualPacket packet, QS.Fx.Base.ContextCallback callback, object context)
            {
                this.packet = packet;
                this.callback = callback;
                this.context = context;
            }

            [QS.Fx.Printing.Printable]
            private IVirtualPacket packet;
            [QS.Fx.Printing.Printable]
            private QS.Fx.Base.ContextCallback callback;
            [QS.Fx.Printing.Printable]
            private object context;

            public IVirtualPacket Packet
            {
                get { return packet; }
            }

            public void Completed()
            {
                if (callback != null)
                    callback(context);
            }

            public override string ToString()
            {
                return QS.Fx.Printing.Printable.ToString(this);
            }
        }

        #endregion

        #region IVirtualNetworkInterface Members

        void IVirtualSenderController.ScheduleSender(IVirtualSender sender)
        {
            bool register_now = false;
            lock (this)
            {
                senders.Enqueue(sender);
                if (!registeredToSend)
                    register_now = registeredToSend = true;                   
            }

            if (register_now)
                network.ScheduleTransmission(clock.Time, packetTransmissionCallback);
        }

        #endregion

        #region PacketTransmissionCallback

        private void PacketTransmissionCallback(
            out IVirtualPacket packet, out QS.Fx.Base.CompletionCallback<IVirtualPacket> callback, out bool hasmore)
        {
            Request request = new Request();

            lock (this)
            {
                while (true)
                {
                    if (requests.Count > 0)
                    {
                        request = requests.Dequeue();
                        packet = request.Packet;
                        callback = transmissionCompletionCallback;
                        
                        break;
                    }
                    else
                    {
                        if (senders.Count > 0)
                        {
                            IVirtualSender sender = senders.Dequeue();

                            int packetsConsumed, bytesConsumed;
                            bool more;
                            sender.TransmissionCallback(
                                temp, maximumBufferedPackets - bufferedPackets, maximumBufferedBytes - bufferedBytes,
                                out packetsConsumed, out bytesConsumed, out more);

                            bufferedPackets += packetsConsumed;
                            bufferedBytes += bytesConsumed;

                            while (temp.Count > 0)
                            {
                                QS.Fx.Network.AsynchronousSend send = temp.Dequeue();

                                // sanity check
                                // send.Data._CheckSize();

                                byte[] block = new byte[send.Data.Size];
                                int blockoffset = 0;
                                foreach (QS.Fx.Base.Block segment in send.Data.Segments)
                                {
                                    if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                                        Buffer.BlockCopy(segment.buffer, (int) segment.offset, block, blockoffset, (int) segment.size);
                                    else
                                        throw new Exception("Unmanaged memory is not supported here.");
                                    blockoffset += (int) segment.size;
                                }

                                requests.Enqueue(
                                    new Request(
                                        new VirtualPacket(sender.SourceAddress, sender.DestinationAddress, new ArraySegment<byte>(block)), 
                                        send.Callback, send.Context));
                            }
                        }
                        else
                        {
                            packet = null;
                            callback = null; 
                            break;
                        }
                    }
                }
                
                hasmore = requests.Count > 0 || senders.Count > 0;

                if (!hasmore)
                    registeredToSend = false;
            }

            request.Completed();
        }

        #endregion

        #region TransmissionCompletionCallback

        private void TransmissionCompletionCallback(bool succeeded, Exception error, IVirtualPacket packet)
        {
            lock (this)
            {
                bufferedPackets--;
                bufferedBytes -= packet.Data.Count;
            }
        }

        #endregion

        #region Reset

        public void Reset()
        {
#if DEBUG_LogGenerously
            logger.Log("Resetting sender controller.");
#endif

            lock (this)
            {
#if DEBUG_LogGenerously
                logger.Log("Sender controller : canceling " + requests.Count.ToString() + 
                    " requests and unregistering " + senders.Count.ToString() + " senders.");
#endif

                registeredToSend = false;
                bufferedBytes = 0;
                bufferedPackets = 0;
                temp.Clear();
                requests.Clear();
                senders.Clear();
            }
        }

        #endregion
    }
}
