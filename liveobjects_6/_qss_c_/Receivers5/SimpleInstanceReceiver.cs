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

// #define UseEnhancedRateControl

#define DEBUG_CollectStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Receivers5
{    
    public class SimpleInstanceReceiver : QS.Fx.Inspection.Inspectable, IInstanceReceiverClass
    {
        private const double DefaultMaximumAcknowledgementRate = 100;

        public SimpleInstanceReceiver(uint acknowledgementChannel,
            Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks)
            : this(acknowledgementChannel, downstreamSinks, DefaultMaximumAcknowledgementRate, 100)
        {
        }

        public SimpleInstanceReceiver(uint acknowledgementChannel, 
            Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks,
            double maximumAckRate, int maximumNumberOfAckRangesPerSingleAck)
        {
            this.downstreamSinks = downstreamSinks;
            this.acknowledgementChannel = acknowledgementChannel;
            this.maximumAckRate = maximumAckRate;
            this.maximumNumberOfAckRangesPerSingleAck = maximumNumberOfAckRangesPerSingleAck;
        }

        private uint acknowledgementChannel;
        private Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        private double maximumAckRate;
        private int maximumNumberOfAckRangesPerSingleAck;

        #region IInstanceReceiverClass Members

        IInstanceReceiver IInstanceReceiverClass.Create(IInstanceReceiverContext context)
        {
            Receiver receiver = new Receiver(context, acknowledgementChannel, downstreamSinks[context.SourceAddress.Address],
                maximumAckRate, maximumNumberOfAckRangesPerSingleAck);
            return receiver;
        }

        #endregion

        #region Class Receiver

        [QS.Fx.Base.Inspectable]
        [QS._core_c_.Diagnostics.ComponentContainer]
        private class Receiver : QS.Fx.Inspection.Inspectable, IInstanceReceiver, QS._core_c_.Diagnostics2.IModule
        {
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            #region IModule Members

            QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
            {
                get { return diagnosticsContainer; }
            }

            #endregion

            public Receiver(IInstanceReceiverContext context, 
                uint acknowledgementChannel, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink,
                double maximumAckRate, int maximumNumberOfAckRangesPerSingleAck)
            {
                this.downstreamSink = downstreamSink;
                this.acknowledgementChannel = acknowledgementChannel;
                this.context = context;
                this.maximumAckRate = maximumAckRate;
                this.maximumNumberOfAckRangesPerSingleAck = maximumNumberOfAckRangesPerSingleAck;

                ackCollection = new Receivers4.AckCollection1(context.Clock);

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
            }

            private IInstanceReceiverContext context;
            private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink;
            private uint acknowledgementChannel;
            [QS._core_c_.Diagnostics.Component]
            private Receivers4.IAckCollection ackCollection;
            private double maximumAckRate, nextAcknowledgement;            
            private bool waitingToAcknowledge = false;
            private QS.Fx.Clock.IAlarm acknowledgementAlarm;
            private int maximumNumberOfAckRangesPerSingleAck;

#if DEBUG_CollectStatistics
            [QS._core_c_.Diagnostics.Component("Receives")]
            [QS._core_c_.Diagnostics2.Property("Receives")]
            private Statistics_.Samples2D timeSeries_receiveTimes = 
                new QS._qss_c_.Statistics_.Samples2D("receive times", "time", "s", "sequence numbers", "packet");
            [QS._core_c_.Diagnostics.Component("Ack Request Times")]
            [QS._core_c_.Diagnostics2.Property("AckRequestTimes")]
            private Statistics_.Samples1D timeSeries_ackRequestTimes = new QS._qss_c_.Statistics_.Samples1D();
            [QS._core_c_.Diagnostics.Component("Ack Transmit Times")]
            [QS._core_c_.Diagnostics2.Property("AckTransmitTimes")]
            private Statistics_.Samples1D timeSeries_ackTransmitTimes = new QS._qss_c_.Statistics_.Samples1D();
            [QS._core_c_.Diagnostics.Component("Acknowledged MaxCovered")]
            [QS._core_c_.Diagnostics2.Property("AcknowledgedMaxCovered")]
            private Statistics_.Samples2D timeSeries_acknowledgedMaxCovered = 
                new QS._qss_c_.Statistics_.Samples2D("acknowledged maxcovered", "time", "s", "maximum covered", "packet");
#endif

            #region GetAckCallback

            private void GetAckCallback(
                Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue,
                int maximumNumberOfObjects,
#if UseEnhancedRateControl
                int maximumNumberOfBytes, 
#endif
                out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
                out int numberOfBytesReturned,
#endif
                out bool moreObjectsAvailable)
            {
                // TODO: Implement enhanced rate control

                double time_now = context.Clock.Time;
                waitingToAcknowledge = false;
                nextAcknowledgement = time_now + 1 / maximumAckRate;

#if DEBUG_CollectStatistics
                if (timeSeries_ackTransmitTimes.Enabled)
                    timeSeries_ackTransmitTimes.Add(time_now);
#endif

                IList<Base1_.Range<uint>> acks;
                uint maximumCovered;
                AckHelper.AckCollection2Acks(ackCollection, maximumNumberOfAckRangesPerSingleAck, out acks, out maximumCovered);

#if DEBUG_CollectStatistics
                if (timeSeries_acknowledgedMaxCovered.Enabled)
                    timeSeries_acknowledgedMaxCovered.Add(time_now, maximumCovered);
#endif

                objectQueue.Enqueue(new InstanceAck(context.SourceAddress, acks, acknowledgementChannel));
                numberOfObjectsReturned = 1;
#if UseEnhancedRateControl    
                numberOfBytesReturned = 0; // FIX THIS ZERO.............................................................................................HERE
#endif
                moreObjectsAvailable = false;
            }

            #endregion

            #region Acknowledge

            private void Acknowledge()
            {
#if DEBUG_CollectStatistics
                if (timeSeries_ackRequestTimes.Enabled)
                    timeSeries_ackRequestTimes.Add(context.Clock.Time);
#endif

                downstreamSink.Send(
                    new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetAckCallback));
            }

            #endregion

            #region AcknowledgementCallback

            private void AcknowledgementCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (this)
                {
                    Acknowledge();
                }
            }

            #endregion

            #region IInstanceReceiver Members

            void IInstanceReceiver.Receive(uint sequenceNo, QS._core_c_.Base3.Message message)
            {
                lock (this)
                {
                    double time_now = context.Clock.Time;

#if DEBUG_CollectStatistics
                    if (timeSeries_receiveTimes.Enabled)
                        timeSeries_receiveTimes.Add(time_now, sequenceNo);
#endif

                    if (ackCollection.Add(sequenceNo))
                        context.Demultiplexer.dispatch(message.destinationLOID, context.SourceAddress, message.transmittedObject);
                        
                    if (!waitingToAcknowledge)
                    {
                        waitingToAcknowledge = true;
                        if (time_now >= nextAcknowledgement)
                            Acknowledge();
                        else
                        {
                            double timeout = nextAcknowledgement - time_now;
                            if (acknowledgementAlarm == null)
                                acknowledgementAlarm = context.AlarmClock.Schedule(
                                    timeout, new QS.Fx.Clock.AlarmCallback(this.AcknowledgementCallback), null);
                            else
                                acknowledgementAlarm.Reschedule(timeout);
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
