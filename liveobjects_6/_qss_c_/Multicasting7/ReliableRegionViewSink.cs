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

// #define DEBUG_AllowCollectingOfStatistics
#define DEBUG_AllowCollectingOfRateStatistics
#define DEBUG_CollectPeriodicStatistics
// #define DEBUG_LogGenerously
#define DEBUG_MeasureGetOverheads

#define OPTION_ChangesToLoopbackHandling

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Multicasting7
{
    [QS.Fx.Printing.Printable("RRVS7", QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public sealed class ReliableRegionViewSink 
        : QS.Fx.Inspection.Inspectable, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>, QS._core_c_.Diagnostics2.IModule
    {
        public static bool DisableSoftwareLoopback = false;

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public ReliableRegionViewSink(QS._core_c_.Statistics.IStatisticsController statisticsController,
            QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, QS._core_c_.Base3.InstanceID localAddress,
            QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Base3_.RVID destinationAddress, uint dataChannel, uint retransmissionChannel,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink,
            Receivers4.IRegionalController regionalController, Receivers4.ICollectingAgent collectingAgent,
            QS._core_c_.FlowControl3.IRateControlled rateControlled, double retransmissionTimeout, FlowControl7.IRateController rateController,
            bool useloopback, bool usenetwork, double warmup_delay) 
        {
            this.retransmissionTimeout = retransmissionTimeout;
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.destinationAddress = destinationAddress;
            this.dataChannel = dataChannel;
            this.retransmissionChannel = retransmissionChannel;
            this.retransmissionAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.RetransmissionAlarmCallback);
            this.regionalController = regionalController;
            this.collectingAgent = collectingAgent;
            this.rateControlled = rateControlled;
            this.rateController = rateController;
            this.useloopback = useloopback && !DisableSoftwareLoopback;
            this.usenetwork = usenetwork;
            this.consumingSink = downstreamSink;
            this.warmup_delay = warmup_delay;

            myCallback = new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjects);
            myRetransmissionCallback = new RequestRV.Callback(this.RetransmissionCallback);
            myCompletionCallback = new RequestRV.Callback(this.CompletionCallback);

            if (regionalController == null || collectingAgent == null)
                throw new Exception("Internal error");

            collectingAgent.OnRateCollected += new QS._qss_c_.Receivers4.RateCallback(RateCollectedCallback);            
            sendingRateCalculator = new FlowControl3.RateCalculator3(clock, TimeSpan.FromSeconds(1));

#if DEBUG_MeasureGetOverheads
            ts_GetOverheads = statisticsController.Allocate2D("get overheads", "", "time", "s", "", "overhead", "s", "");
            ts_GetNumberOfObjectsReturned = statisticsController.Allocate2D(
                "number of objects returned by get", "", "time", "s", "", "number of objects returned", "", "");
            ts_Detailed_GetOverheads_Sending = statisticsController.Allocate2D("detailed get overheads (sending)", "", "time", "s", "", "overhead", "s", "");
            ts_Detailed_GetOverheads_SoftwareLoopback = statisticsController.Allocate2D("detailed get overheads (loopback)", "", "time", "s", "", "overhead", "s", "");
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

            alarmClock.Schedule(warmup_delay,
                new QS.Fx.Clock.AlarmCallback(this.WarmupCallback), null);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        [QS.Fx.Printing.Printable]
        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        [QS.Fx.Base.Inspectable]
        private double warmup_delay, retransmissionTimeout = 10.0;
        [QS.Fx.Printing.Printable]
        private Base3_.RVID destinationAddress;
        private uint seqno, dataChannel, retransmissionChannel;
        private QS.Fx.Clock.AlarmCallback retransmissionAlarmCallback;
        private Receivers4.IRegionalController regionalController;
        private Receivers4.ICollectingAgent collectingAgent;
        private Queue<RequestRV> retransmissionQueue = new Queue<RequestRV>();
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> pendingQueue =
            new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();
        private Queue<RequestRV> transmittedQueue = new Queue<RequestRV>();
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.FlowControl3.IRateControlled rateControlled;
        [QS._core_c_.Diagnostics.Component]
        private FlowControl3.IRateCalculator sendingRateCalculator;
        [QS._core_c_.Diagnostics.Component]
        [QS._core_c_.Diagnostics2.Module("RateController")]
        private FlowControl7.IRateController rateController;
        private bool useloopback, usenetwork;
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> consumingSink;
        private bool registered, warmingup = true;
        private QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> myCallback;
        private RequestRV.Callback myRetransmissionCallback, myCompletionCallback;

        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> incomingQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();

#if DEBUG_MeasureGetOverheads
        [QS._core_c_.Diagnostics2.Property("GetOverheads")]
        private QS._core_c_.Statistics.ISamples2D ts_GetOverheads;

        [QS._core_c_.Diagnostics2.Property("Detailed_GetOverheads_Sending")]
        private QS._core_c_.Statistics.ISamples2D ts_Detailed_GetOverheads_Sending;

        [QS._core_c_.Diagnostics2.Property("Detailed_GetOverheads_SoftwareLoopback")]
        private QS._core_c_.Statistics.ISamples2D ts_Detailed_GetOverheads_SoftwareLoopback;

        [QS._core_c_.Diagnostics2.Property("GetNumberOfObjectsReturned")]
        private QS._core_c_.Statistics.ISamples2D ts_GetNumberOfObjectsReturned;
#endif

        #region WarmupCallback

        private void WarmupCallback(QS.Fx.Clock.IAlarm alarm)
        {
            warmingup = false;
            if (usenetwork && !registered && (incomingQueue.Count > 0 || pendingQueue.Count > 0 || retransmissionQueue.Count > 0))
            { 
                registered = true;
                consumingSink.Send(myCallback);
            }
        }

        #endregion

        #region Collecting Statistics

#if DEBUG_AllowCollectingOfStatistics
        [Diagnostics.Component("Creation Times (X = seqno, Y = time)")]
        [QS._core_c_.Diagnostics2.Property("CreationTimes")]
        private Statistics.Samples timeSeries_creationTimes = new QS.CMS.Statistics.Samples();

        [Diagnostics.Component("Acknowledgement Times (X = seqno, Y = time)")]
        [QS._core_c_.Diagnostics2.Property("AcknowledgementTimes")]
        private Statistics.Samples timeSeries_acknowledgementTimes = new QS.CMS.Statistics.Samples();

        [Diagnostics.Component("Retransmission Times (X = time, Y = seqno)")]
        [QS._core_c_.Diagnostics2.Property("RetransmissionTimes")]
        private Statistics.SamplesXY timeSeries_retransmissionTimes = new QS.CMS.Statistics.SamplesXY();            
#endif

#if DEBUG_CollectPeriodicStatistics
        private const double DefaultSamplingPeriod = 0.1;
        private double lastlogged, cumulativelatency, samplingperiod = DefaultSamplingPeriod;
        private int nnewsamples, nnewretransmits;

        [QS._core_c_.Diagnostics.Component("Time To Acknowledgement (X = measurement time, Y = latency)")]
        [QS._core_c_.Diagnostics2.Property("TimeToAcknowledgement")]
        private Statistics_.Samples2D timeSeries_timeToAcknowledgement = new QS._qss_c_.Statistics_.Samples2D();

        [QS._core_c_.Diagnostics.Component("Retransmission Rates (X = time, Y = retransmissions/s)")]
        [QS._core_c_.Diagnostics2.Property("RetransmissionRates")]
        private Statistics_.Samples2D timeSeries_retransmissionRates = new QS._qss_c_.Statistics_.Samples2D();

        private void _PeriodicCheck(double now)
        {
            if (now > lastlogged + samplingperiod)
            {
                double timeelapsed = now - lastlogged;

                timeSeries_timeToAcknowledgement.Add(now, cumulativelatency / ((double)nnewsamples));
                timeSeries_retransmissionRates.Add(now, ((double)nnewretransmits) / timeelapsed);

                lastlogged = now;
                cumulativelatency = 0;
                nnewsamples = 0;
                nnewretransmits = 0;
            }
        }

#endif

#if DEBUG_AllowCollectingOfRateStatistics
        [QS._core_c_.Diagnostics.Component("Minimum Receive Rates Collected (X = time, Y = rate)")]
        [QS._core_c_.Diagnostics2.Property("MinimumReceiveRates")]
        private Statistics_.Samples2D timeSeries_minimumReceiveRatesCollected = new QS._qss_c_.Statistics_.Samples2D();

        [QS._core_c_.Diagnostics.Component("Average Receive Rates Collected (X = time, Y = rate)")]
        [QS._core_c_.Diagnostics2.Property("AverageReceiveRates")]
        private Statistics_.Samples2D timeSeries_averageReceiveRatesCollected = new QS._qss_c_.Statistics_.Samples2D();

        [QS._core_c_.Diagnostics.Component("Maximum Receive Rates Collected (X = time, Y = rate)")]
        [QS._core_c_.Diagnostics2.Property("MaximumReceiveRates")]
        private Statistics_.Samples2D timeSeries_maximumReceiveRatesCollected = new QS._qss_c_.Statistics_.Samples2D();

        [QS._core_c_.Diagnostics.Component("Sending Rates Collected (X = time, Y = rate)")]
        [QS._core_c_.Diagnostics2.Property("SendingRates")]
        private Statistics_.Samples2D timeSeries_sendingRatesCollected = new QS._qss_c_.Statistics_.Samples2D();

        [QS._core_c_.Diagnostics.Component("Rates Set by a Controller (X = time, Y = rate)")]
        [QS._core_c_.Diagnostics2.Property("RatesSetByController")]
        private Statistics_.Samples2D timeSeries_ratesSetByAController = new QS._qss_c_.Statistics_.Samples2D();
#endif

        #endregion

        #region RateCollectedCallback

        private void RateCollectedCallback(double minrate, double avgrate, double maxrate)
        {
#if DEBUG_AllowCollectingOfRateStatistics
            double now = clock.Time;
            if (timeSeries_minimumReceiveRatesCollected.Enabled)
                timeSeries_minimumReceiveRatesCollected.Add(now, minrate);
            if (timeSeries_averageReceiveRatesCollected.Enabled)
                timeSeries_averageReceiveRatesCollected.Add(now, avgrate);
            if (timeSeries_maximumReceiveRatesCollected.Enabled)
                timeSeries_maximumReceiveRatesCollected.Add(now, maxrate);
#endif

            if (rateController != null)          
            {
                double sendingRate = sendingRateCalculator.Rate;
#if DEBUG_AllowCollectingOfRateStatistics
                if (timeSeries_sendingRatesCollected.Enabled)
                    timeSeries_sendingRatesCollected.Add(now, sendingRate);
#endif

                double calculatedRate = rateController.Calculate(sendingRate,minrate, avgrate, maxrate);
                if (rateControlled != null)
                    rateControlled.MaximumRate = calculatedRate;

#if DEBUG_AllowCollectingOfRateStatistics
                if (timeSeries_ratesSetByAController.Enabled)
                    timeSeries_ratesSetByAController.Add(now, calculatedRate);
#endif
            }
        }

        #endregion

        #region Callback

        private void CompletionCallback(RequestRV request)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "_CompletionCallback : " + destinationAddress.ToString() + " : " + request.SequenceNo.ToString());
#endif

            bool completed_now = false;
            lock (request)
            {
                if (!request.Completed)
                {
                    request.Completed = completed_now = true;

#if DEBUG_CollectPeriodicStatistics
                    double now = clock.Time;
                    double latency = now - request.CreationTime;
                    cumulativelatency += latency;
                    nnewsamples++;

                    _PeriodicCheck(now);
#endif

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
                            timeSeries_acknowledgementTimes.addSample((int)request.SequenceNo, clock.Time);
                    }
                }
#endif

                if (request.Message.CompletionCallback != null)
                    request.Message.CompletionCallback(true, null, request.Message.Context);
            }
        }

        #endregion

        #region RetransmissionCallback

        private void RetransmissionCallback(RequestRV request)
        {
            bool retransmit_now;
            lock (request)
            {
                retransmit_now = !request.Completed && !request.Retransmitting;
                // we might need to add restriction not to retransmit yet if we just did this a short while ago

                if (retransmit_now)
                {
                    request.Retransmitting = true;

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
            this.RetransmissionCallback((RequestRV)alarmRef.Context);
        }

        #endregion

        #region RegisterObject

        public void RegisterObject(QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message, out uint sequenceNo)
        {
            sequenceNo = ++seqno;
            RequestRV request = new RequestRV(
                destinationAddress, dataChannel, sequenceNo, message, null, myRetransmissionCallback, myCompletionCallback);
            request.CreationTime = clock.Time;
            collectingAgent.Collect(request);

            request.Alarm = alarmClock.Schedule(retransmissionTimeout, retransmissionAlarmCallback, request);

            if (useloopback)
            {
                regionalController.Receive(
                    localAddress, destinationAddress, request.SequenceNo, request.Message.Argument, false, false);
            }
        }

        #endregion

        #region GetObjects

        private void GetObjects(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoingQueue,
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

#if DEBUG_MeasureGetOverheads
            double tt1 = clock.Time;
#endif

            lock (this)
            {
                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                    numberOfBytesReturned = 0;
#endif
                moreObjectsAvailable = true;

                #region Processing retransmissions

                while (retransmissionQueue.Count > 0 
                    && numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                {
                    RequestRV request = retransmissionQueue.Dequeue();
                    if (!request.Completed)
                    {
                        if (!request.Retransmitting)
                            throw new Exception("Internal error: request not marked as \"retransmitting\".");
                        request.Retransmitting = false;
                        request.Retransmitted = true;
                        request.Channel = retransmissionChannel;
                        double now = clock.Time;
                        request.LastRetransmitted = now;                        

                        transmittedQueue.Enqueue(request);

#if DEBUG_CollectPeriodicStatistics
                        nnewretransmits++;
                        _PeriodicCheck(now);
#endif

#if DEBUG_AllowCollectingOfStatistics
                        if (timeSeries_retransmissionTimes.Enabled)
                            timeSeries_retransmissionTimes.addSample(clock.Time, (int)request.SequenceNo);
#endif

                        outgoingQueue.Enqueue(request);
                        sendingRateCalculator.sample();
                        numberOfObjectsReturned++;
                        // numberOfBytesReturned += ..................................................HERE
                    }
                }

                #endregion

                #region Processing regular multicast

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
                                    RequestRV request = new RequestRV(
                                        destinationAddress, dataChannel, ++seqno, message, null, myRetransmissionCallback, myCompletionCallback);
                                    request.CreationTime = clock.Time;

#if DEBUG_AllowCollectingOfStatistics
                                    if (timeSeries_creationTimes.Enabled)
                                        timeSeries_creationTimes.addSample((int)request.SequenceNo, clock.Time);
#endif

                                    transmittedQueue.Enqueue(request);
                                    
                                    outgoingQueue.Enqueue(request);
                                    sendingRateCalculator.sample();
                                    numberOfObjectsReturned++;
                                    // numberOfBytesReturned += ....................................................HERE

                                    collectingAgent.Collect(request);                                    
                                }

                                pendingQueue.Clear();
                            }
                            else
                                break;
                        }
                        else
                        {
                            moreObjectsAvailable = registered = false;
                            break;
                        }
                    }
                }

                #endregion
            }

#if DEBUG_MeasureGetOverheads
            double tt2 = clock.Time;
#endif

            foreach (RequestRV request in transmittedQueue)
            {
                lock (request)
                {
                    if (!request.Completed)
                        request.Alarm = alarmClock.Schedule(retransmissionTimeout, retransmissionAlarmCallback, request);
                }

                if (!request.Retransmitted
#if OPTION_ChangesToLoopbackHandling
                    && useloopback
#endif
                    )
                {
                    regionalController.Receive(
                        localAddress, destinationAddress, request.SequenceNo, request.Message.Argument, false, false);
                }
            }

            transmittedQueue.Clear();

#if DEBUG_MeasureGetOverheads
            double tt3 = clock.Time;
            ts_GetOverheads.Add(tt1, tt3 - tt1);
            ts_GetNumberOfObjectsReturned.Add(tt3, numberOfObjectsReturned);
            ts_Detailed_GetOverheads_Sending.Add(tt1, tt2 - tt1);
            ts_Detailed_GetOverheads_SoftwareLoopback.Add(tt2, tt3- tt2);
#endif
        }

        #endregion

        #region Base6.ISink<Base6.IAsynchronous<Base3.Message>> Members

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
#if OPTION_ChangesToLoopbackHandling
            if (usenetwork)
            {
#endif

                bool signal_now = false;
                lock (this)
                {
                    incomingQueue.Enqueue(getObjectsCallback);
                    if (!registered && !warmingup)
                    {
                        signal_now = true;
                        registered = true;
                    }
                }

                if (signal_now)
                    consumingSink.Send(myCallback);

#if OPTION_ChangesToLoopbackHandling
            }
            else
            {
                if (useloopback)
                {
                    bool moreObjectsAvailable;
                    do
                    {
                        int numberOfObjectsReturned;
                        getObjectsCallback(pendingQueue, int.MaxValue, out numberOfObjectsReturned, out moreObjectsAvailable);

                        foreach (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message in pendingQueue)
                        {
                            regionalController.Receive(localAddress, destinationAddress, ++seqno, message.Argument, false, false);
                            if (message.CompletionCallback != null)
                                message.CompletionCallback(true, null, message.Context);
                        }

                        pendingQueue.Clear();
                    }
                    while (moreObjectsAvailable);
                }
                else
                    throw new Exception("Cannot send to an empty region.");
            }
#endif
        }

        #endregion
    }
}
