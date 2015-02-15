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

// #define DEBUG_CollectingAgent

// #define DEBUG_STATISTICS_CollectTimes
#define DEBUG_STATISTICS_AcknakTimes
#define DEBUG_STATISTICS_Resends
#define DEBUG_STATISTICS_Resends_Collect_In_Memory

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class CollectingAgent : QS.Fx.Inspection.Inspectable, Receivers4.ICollectingAgent, QS._core_c_.Diagnostics2.IModule, IDisposable
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public CollectingAgent(Receivers4.ICollectingAgentContext context, QS.Fx.Logging.IEventLogger eventLogger, 
            QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IClock clock, uint ackChannel, uint nakChannel, 
            QS._core_c_.Statistics.IStatisticsController statisticsController)
        {
            this.clock = clock;
            this.context = context;
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;

            context.Demultiplexer.register(ackChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.AckReceiveCallback));
            context.Demultiplexer.register(nakChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.NakReceiveCallback));

#if DEBUG_STATISTICS_Resends
#if DEBUG_STATISTICS_Resends_Collect_In_Memory
            timeSeries_resends = new QS._qss_c_.Statistics_.Samples2D("resends", "time", "", "seqno", "");
#else
            timeSeries_resends = statisticsController.Allocate2D("resends", "", "time", "", "", "seqno", "", "");
#endif
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private bool disposed;
        private QS.Fx.Clock.IClock clock;
        private Receivers4.ICollectingAgentContext context;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS._core_c_.Base3.InstanceID localAddress;
        private event Receivers4.RateCallback onRateCollected;

#if DEBUG_STATISTICS_CollectTimes
        [QS.CMS.Diagnostics.Component("Collect Times (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("CollectTimes")]
        private Statistics.SamplesXY timeSeries_collectTimes = new QS.CMS.Statistics.SamplesXY("Rings6.CollectingAgent.CollectTimes");
#endif

#if DEBUG_STATISTICS_AcknakTimes
        [QS._core_c_.Diagnostics.Component("Acks Received (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("AcksReceived")]
        private Statistics_.Samples2D timeSeries_acksReceived = new QS._qss_c_.Statistics_.Samples2D("Rings6.CollectingAgent.AcksReceived");
        [QS._core_c_.Diagnostics.Component("Nak Count (X=time, Y=count)")]
        [QS._core_c_.Diagnostics2.Property("NakCount")]
        private Statistics_.Samples2D timeSeries_nakCounts = new QS._qss_c_.Statistics_.Samples2D("Rings6.CollectingAgent.NakCounts");
#endif

#if DEBUG_STATISTICS_Resends
        [QS._core_c_.Diagnostics.Component("Resends (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("Resends")]
        private QS._core_c_.Statistics.ISamples2D timeSeries_resends;
#endif

        private Receivers4.IMessageRepository<Receivers4.IAcknowledgeable> requests =
            new Receivers4.MessageRepository1<Receivers4.IAcknowledgeable>();

#if DEBUG_CollectingAgent
        private void log(string description, string details)
        {
            if (eventLogger.Enabled)
                eventLogger.Log(new MyEvent(clock.Time, localAddress, this, description, details));
        }
#endif

        #region ICollectingAgent Members

        event QS._qss_c_.Receivers4.RateCallback QS._qss_c_.Receivers4.ICollectingAgent.OnRateCollected
        {
            add { onRateCollected += value; }
            remove { onRateCollected -= value; }
        }

        void QS._qss_c_.Receivers4.ICollectingAgent.Collect(QS._qss_c_.Receivers4.IAcknowledgeable request)
        {
/*
#if DEBUG_ReceivingAgent1
            log("Collect", QS.Fx.Printing.Printable.ToString(request));
#endif
*/

            // DEADLOCK (1)
            lock (this)
            {
#if DEBUG_STATISTICS_CollectTimes
                if (timeSeries_collectTimes.Enabled)
                    timeSeries_collectTimes.addSample(clock.Time, request.SequenceNo);
#endif

                requests.Add(request.SequenceNo, request);

            }
        }

        #endregion

        #region CleanupCallback

//        private void CleanupCallback(uint sequenceNo, Receivers4.IAcknowledgeable request, object cleanupContext)
//        {
//            request.Acknowledged();
//        }

        #endregion

        #region AckReceiveCallback

        private QS.Fx.Serialization.ISerializable AckReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_CollectingAgent
            log("Received ACK from " + sourceAddress.ToString(), QS.Fx.Printing.Printable.ToString(receivedObject));
#endif

            Acknowledgement ack = receivedObject as Acknowledgement;
            if (ack != null)
            {
                IEnumerable<KeyValuePair<uint, Receivers4.IAcknowledgeable>> toCleanup;
                lock (this)
                {
#if DEBUG_STATISTICS_AcknakTimes
                    if (timeSeries_acksReceived.Enabled)
                        timeSeries_acksReceived.Add(clock.Time, ack.MaximumClean);
#endif

                    toCleanup = requests.CleanUp(ack.MaximumClean);
                        // new QS.CMS.Receivers4.CleanupCallback<Receivers4.IAcknowledgeable>(this.CleanupCallback), null);
                }

                if (toCleanup != null)
                {
                    foreach (KeyValuePair<uint, Receivers4.IAcknowledgeable> element in toCleanup)
                        element.Value.Acknowledged();
                }
            }

            onRateCollected(ack.MinimumReceiveRate, ack.AverageReceiveRate, ack.MaximumReceiveRate);

            return null;
        }

        #endregion

        #region NakReceiveCallback

        private QS.Fx.Serialization.ISerializable NakReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_CollectingAgent
            log("Received NAK from " + sourceAddress.ToString(), QS.Fx.Printing.Printable.ToString(receivedObject));
#endif

            PartitionAcknowledgement partitionAck = receivedObject as PartitionAcknowledgement;
            if (partitionAck != null)
            {
#if DEBUG_STATISTICS_AcknakTimes
                if (timeSeries_nakCounts.Enabled)
                {
                    int nakCount = 0;
                    foreach (Base1_.Range<uint> nak in partitionAck.IsolatedNaks)
                        nakCount += ((int)nak.To - (int)nak.From + 1);

                    timeSeries_nakCounts.Add(clock.Time, nakCount);
                }
#endif

                foreach (Base1_.Range<uint> nak in partitionAck.IsolatedNaks)
                {
                    for (uint partitionSeqNo = ((nak.From > 0) ? nak.From : 1); partitionSeqNo <= nak.To; partitionSeqNo++)
                    {
                        uint seqno = (partitionSeqNo - 1) * partitionAck.PartitionCount + partitionAck.PartitionIndex + 1;
                        Receivers4.IAcknowledgeable request = null;
                        if (requests.Get(seqno, ref request))
                        {
#if DEBUG_STATISTICS_Resends
                            timeSeries_resends.Add(clock.Time, seqno);
#endif

                            request.Resend();

#if DEBUG_CollectingAgent
                            log("Resending " + request.SequenceNo.ToString(), "");
#endif
                        }
                        else
                        {
#if DEBUG_CollectingAgent
                            log("Cannot resend request " + seqno.ToString(), "");
#endif
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        public override string ToString()
        {
            return "Collector(" + context.ID.ToString() + ")";
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            disposed = true;
        }

        #endregion
    }        
}
