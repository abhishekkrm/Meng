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

#define DEBUG_AllowCollectingOfStatistics
// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders11
{
    [QS.Fx.Printing.Printable("ReliableInstanceSink", QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class ReliableInstanceSink : Base6_.ProcessingSink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>,
        QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>, FlowControl3.IRetransmittingSender, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public ReliableInstanceSink(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, uint dataChannel,
            QS._core_c_.Base3.InstanceID localAddress, QS._core_c_.Base3.InstanceID destinationAddress, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock,            
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink) : base(downstreamSink)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;
            this.destinationAddress = destinationAddress;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.dataChannel = dataChannel;
            this.retransmissionAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.RetransmissionAlarmCallback);

            retransmissionController = new FlowControl3.RetransmissionController1(this,
                new QS._qss_c_.FlowControl3.RetransmissionController1.Configuration(1, 0.99, 2, 0.1, 1.0));

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        [QS.Fx.Printing.Printable]
        private QS._core_c_.Base3.InstanceID localAddress, destinationAddress;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        [QS.Fx.Base.Inspectable]
        private double retransmissionTimeout = 1.0;
        private uint seqno, dataChannel;
        private QS.Fx.Clock.AlarmCallback retransmissionAlarmCallback;

        [QS._core_c_.Diagnostics2.Module("RetransmissionController")]
        private FlowControl3.IRetransmissionController retransmissionController;

        [QS.Fx.Base.Inspectable]
        private Queue<ReliableInstanceRequest> retransmissionQueue = new Queue<ReliableInstanceRequest>();
        [QS.Fx.Base.Inspectable]
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> pendingQueue = new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();

#if DEBUG_AllowCollectingOfStatistics
        [QS._core_c_.Diagnostics.Component("Creation Times")]
        [QS._core_c_.Diagnostics2.Property("CreationTimes")]
        private Statistics_.Samples1D timeSeries_creationTimes = new QS._qss_c_.Statistics_.Samples1D("creation times", "seqno", "packet", "time", "s");
        [QS._core_c_.Diagnostics.Component("Acknowledgement Times")]
        [QS._core_c_.Diagnostics2.Property("AcknowledgementTimes")]
        private Statistics_.Samples1D timeSeries_acknowledgementTimes = new QS._qss_c_.Statistics_.Samples1D("ack times", "seqno", "packet", "time", "s");
        [QS._core_c_.Diagnostics.Component("Retransmission Times")]
        [QS._core_c_.Diagnostics2.Property("RetransmissionTimes")]
        private Statistics_.Samples2D timeSeries_retransmissionTimes = 
            new QS._qss_c_.Statistics_.Samples2D("retransmission times", "seqno", "packet", "time", "s");
        [QS._core_c_.Diagnostics2.Property("RemovalTimes")]
        [QS._core_c_.Diagnostics.Component("Removal Times")]
        private Statistics_.Samples2D timeSeries_removalTimes = new QS._qss_c_.Statistics_.Samples2D();
#endif

        [QS.Fx.Base.Inspectable]
        private Receivers5.IPendingCollection<ReliableInstanceRequest> requests = 
            new Receivers5.PendingCollection<ReliableInstanceRequest>();

        #region Collect and Acknowledge

        private void Collect(ReliableInstanceRequest request)
        {
            if (!requests.Add(request.SequenceNo, request))
                throw new Exception("Internal error: collect failed, request already there.");
        }

        public void Acknowledge(IList<Base1_.Range<uint>> acknowledgements)
        {
            double time_now = clock.Time;

            IEnumerable<ReliableInstanceRequest> newlyAcked;
            lock (this)
            {
                requests.Remove(acknowledgements, out newlyAcked);
            }
            foreach (ReliableInstanceRequest request in newlyAcked)
            {
#if DEBUG_AllowCollectingOfStatistics
                timeSeries_removalTimes.Add(request.SequenceNo, clock.Time);
#endif

                double sample_rtt = time_now - request.CreationTime;
                retransmissionController.completed(sample_rtt, request.NumberOfRetransmissions);
                
                // retransmissionController.completed(completion_time, request.NRetransmissions);    
                ((Receivers4.IAcknowledgeable)request).Acknowledged();
            }
        }

        #endregion

        #region Callback

        private void CompletionCallback(ReliableInstanceRequest request)
        {
            bool completed_now = false;
            lock (request)
            {
                if (!request.Completed)
                {
                    request.Completed = completed_now = true;

                    if (request.Alarm != null)
                    {
                        try
                        {
                            request.Alarm.Cancel();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    request.Alarm = null;
                }
            }

            if (completed_now)
            {
#if DEBUG_AllowCollectingOfStatistics
                if (timeSeries_acknowledgementTimes.Enabled)
                {
                    lock (this)
                    {
                        if (timeSeries_acknowledgementTimes.Enabled)
                            timeSeries_acknowledgementTimes.Add((int)request.SequenceNo, clock.Time);
                    }
                }
#endif

                if (request.Message.CompletionCallback != null)
                    request.Message.CompletionCallback(true, null, request.Message.Context);
            }
        }

        #endregion

        #region RetransmissionCallback

        private void RetransmissionCallback(ReliableInstanceRequest request)
        {
            bool retransmit_now;
            lock (request)
            {
                retransmit_now = !request.Completed && !request.Retransmitted;
                if (retransmit_now)
                {
                    request.Retransmitted = true;

                    if (request.Alarm != null)
                    {
                        try
                        {
                            request.Alarm.Cancel();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    request.Alarm = null;
                }
            }

            if (retransmit_now)
            {
                bool signal_now;
                lock (this)
                {
                    retransmissionQueue.Enqueue(request);
                    signal_now = !registered;
                    registered = true;
                }

                if (signal_now)
                    consumingSink.Send(myCallback);
            }
        }

        private void RetransmissionAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            this.RetransmissionCallback((ReliableInstanceRequest)alarmRef.Context);
        }

        #endregion

        #region GetObjects Callback

        protected override void GetObjects(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoingQueue,
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

            List<ReliableInstanceRequest> transmitted = new List<ReliableInstanceRequest>();
            lock (this)
            {
                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                    numberOfBytesReturned = 0;
#endif
                moreObjectsAvailable = true;

                while (retransmissionQueue.Count > 0 
                    && numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                {                    
                    ReliableInstanceRequest request = retransmissionQueue.Dequeue();
                    if (!request.Completed)
                    {
                        request.NumberOfRetransmissions++;
                        transmitted.Add(request);

#if DEBUG_AllowCollectingOfStatistics
                        if (timeSeries_retransmissionTimes.Enabled)
                            timeSeries_retransmissionTimes.Add((int)request.SequenceNo, clock.Time);
#endif

                        outgoingQueue.Enqueue(request);
                        numberOfObjectsReturned++;
                        // numberOfBytesReturned += ......................................................................................HERE
                    }
                }

                if (retransmissionQueue.Count == 0)
                {
                    while (true)
                    {
                        if (incomingQueue.Count > 0)
                        {
                            if (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                            {
                                QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getCallback = incomingQueue.Dequeue();

                                int objectsReturned;
#if UseEnhancedRateControl
                                int bytesReturned;
#endif
                                bool moreAvailable;
                                getCallback(pendingQueue,
                                    maximumNumberOfObjects - numberOfObjectsReturned, 
#if UseEnhancedRateControl
                                    int.MaxValue, // maximumNumberOfBytes - numberOfBytesReturned, 
#endif
                                    out objectsReturned, 
#if UseEnhancedRateControl
                                    out bytesReturned, 
#endif                                    
                                    out moreAvailable);
                                if (moreAvailable)
                                    incomingQueue.Enqueue(getCallback);

                                foreach (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message in pendingQueue)
                                {
                                    ReliableInstanceRequest request = 
                                        new ReliableInstanceRequest(destinationAddress, dataChannel, ++seqno, message, null, 
                                        new ReliableInstanceRequest.Callback(this.RetransmissionCallback), 
                                        new ReliableInstanceRequest.Callback(this.CompletionCallback));
                                    request.CreationTime = clock.Time;

#if DEBUG_AllowCollectingOfStatistics
                                    if (timeSeries_creationTimes.Enabled)
                                        timeSeries_creationTimes.Add((int)request.SequenceNo, clock.Time);
#endif

                                    transmitted.Add(request);
                                    
                                    outgoingQueue.Enqueue(request);
                                    numberOfObjectsReturned++;
                                    // numberOfBytesReturned += ....................................................HERE

                                    this.Collect(request);
                                }

                                pendingQueue.Clear();
                            }
                            else
                                break;
                        }
                        else
                        {
                            moreObjectsAvailable = false;
                            this.Done();
                            break;
                        }
                    }
                }
            }

            foreach (ReliableInstanceRequest request in transmitted)
            {
                lock (request)
                {
                    if (!request.Completed)
                    {
                        request.Retransmitted = false;
                        request.Alarm = alarmClock.Schedule(retransmissionTimeout, retransmissionAlarmCallback, request);
                    }
                }

                // regionalController.Receive(
                // localAddress, destinationAddress, request.SequenceNo, request.Message.Argument);
            }
        }

        #endregion

        #region IRetransmittingSender Members

        double QS._qss_c_.FlowControl3.IRetransmittingSender.RetransmissionTimeout
        {
            get { return retransmissionTimeout; }
            set { retransmissionTimeout = value; }
        }

        #endregion
    }
}
