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

// #define DEBUG_ReceivingAgent1
// #define DEBUG_ReceivingAgent1_ShowReceives
// #define DEBUG_CheckStatus

#define DEBUG_CollectStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    // [QS.CMS.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class ReceivingAgent : QS.Fx.Inspection.Inspectable, Receivers4.IReceivingAgentClass, Receivers4.ICollectingAgentClass, 
        IReceiverConfiguration, IAgentContext, IAgentConfiguration
    {
        #region Constructor

        public ReceivingAgent(QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localAddress,
            QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Base3_.IDemultiplexer demultiplexer,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection,            
            uint numberOfReplicas, double tokenCirculationRate, double minimumTokenCirculationRate, 
            uint maximumNakRangesPerToken, uint maximumWindowWidth, RateSharingAlgorithmClass rateSharingAlgorithm,
            Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinkCollection,
            QS._core_c_.Statistics.IStatisticsController statisticsController)
        {
            this.statisticsController = statisticsController;
            this.eventLogger = eventLogger;
            this.logger = logger;
            this.localAddress = localAddress;
            this.clock = clock;
            this.demultiplexer = demultiplexer;
            this.alarmClock = alarmClock;
            this.senderCollection = senderCollection;
            this.sinkCollection = sinkCollection;
            this.tokenCirculationRate = tokenCirculationRate;
            this.minimumTokenCirculationRate = minimumTokenCirculationRate;
            this.numberOfReplicas = numberOfReplicas;
            this.maximumNakRangesPerToken = maximumNakRangesPerToken;
            this.maximumWindowWidth = maximumWindowWidth;
            this.rateSharingAlgorithm = rateSharingAlgorithm;
        }

        #endregion

        #region Fields

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private RateSharingAlgorithmClass rateSharingAlgorithm;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS.Fx.Clock.IClock clock;
        private Base3_.IDemultiplexer demultiplexer;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS._core_c_.Base3.InstanceID localAddress;
        [QS.Fx.Base.Inspectable]
        private uint numberOfReplicas, maximumNakRangesPerToken, maximumWindowWidth;
        [QS.Fx.Base.Inspectable]
        private double tokenCirculationRate, minimumTokenCirculationRate;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection;
        private Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinkCollection;
        [QS.Fx.Base.Inspectable]
        private bool pullCaching = false, forwardingAllowed = true, naksAllowed = true, acksAllowed = false;

        #endregion

        #region Adjusting Default Configuration

        public bool AcksAllowed
        {
            get { return acksAllowed; }
            set { acksAllowed = value; }
        }

        public bool NaksAllowed
        {
            get { return naksAllowed; }
            set { naksAllowed = value; }
        }

        public bool ForwardingAllowed
        {
            get { return forwardingAllowed; }
            set { forwardingAllowed = value; }
        }

        public uint MaximumWindowWidth
        {
            get { return maximumWindowWidth; }
            set { maximumWindowWidth = value; }
        }

        public bool PullCaching
        {
            get { return pullCaching; }
            set { pullCaching = value; }
        }

        public double TokenRate
        {
            get { return tokenCirculationRate; }
            set { tokenCirculationRate = value; }
        }

        public double MinimumTokenRate
        {
            get { return minimumTokenCirculationRate; }
            set { minimumTokenCirculationRate = value; }
        }

        public uint ReplicationCoefficient
        {
            get { return numberOfReplicas; }
            set { numberOfReplicas = value; }
        }

        public uint MaximumNakRangesPerToken
        {
            get { return maximumNakRangesPerToken; }
            set { maximumNakRangesPerToken = value; }
        }

        #endregion

        #region IReceivingAgentClass Members

        QS._qss_c_.Receivers4.IReceivingAgent QS._qss_c_.Receivers4.IReceivingAgentClass.CreateAgent(QS._qss_c_.Receivers4.IReceivingAgentContext context)
        {
            return new Agent(context, this, this, this, statisticsController);
        }

        #endregion

        #region ICollectingAgentClass Members

        QS._qss_c_.Receivers4.ICollectingAgent QS._qss_c_.Receivers4.ICollectingAgentClass.CreateAgent(QS._qss_c_.Receivers4.ICollectingAgentContext context)
        {
            return new CollectingAgent(context, eventLogger, localAddress, clock, (uint)ReservedObjectID.Rings6_ReceivingAgent1_AckChannel,
                (uint)ReservedObjectID.Rings6_ReceivingAgent1_NakChannel, statisticsController);
        }

        #endregion

        #region IReceiverConfiguration Members

        bool IReceiverConfiguration.PullCaching
        {
            get { return pullCaching; }
        }

        uint IReceiverConfiguration.MaximumNakRangesPerToken
        {
            get { return maximumNakRangesPerToken; }
        }

        bool IReceiverConfiguration.ForwardingAllowed
        {
            get { return forwardingAllowed; }
        }

        bool IReceiverConfiguration.NaksAllowed
        {
            get { return naksAllowed; }
        }

        bool IReceiverConfiguration.AcksAllowed
        {
            get { return acksAllowed; }
        }

        uint IReceiverConfiguration.MaximumWindowWidth
        {
            get { return maximumWindowWidth; }
        }

        RateSharingAlgorithmClass IReceiverConfiguration.RateSharingAlgorithm
        {
            get { return rateSharingAlgorithm; }
        }

        #endregion

        #region IAgentContext Members

        QS.Fx.Logging.ILogger IAgentContext.Logger
        {
            get { return logger; }
        }

        QS.Fx.Logging.IEventLogger IAgentContext.EventLogger
        {
            get { return eventLogger; }
        }

        QS._core_c_.Base3.InstanceID IAgentContext.LocalAddress
        {
            get { return localAddress; }
        }

        QS.Fx.Clock.IClock IAgentContext.Clock
        {
            get { return clock; }
        }

        QS.Fx.Clock.IAlarmClock IAgentContext.AlarmClock
        {
            get { return alarmClock; }
        }

        QS._qss_c_.Base3_.IDemultiplexer IAgentContext.Demultiplexer
        {
            get { return demultiplexer; }
        }

        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> IAgentContext.SenderCollection
        {
            get { return senderCollection; }
        }

        Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> IAgentContext.SinkCollection
        {
            get { return sinkCollection; }
        }

        #endregion

        #region IAgentConfiguration Members

        uint IAgentConfiguration.NumberOfReplicas
        {
            get { return numberOfReplicas; }
        }

        double IAgentConfiguration.TokenCirculationRate
        {
            get { return tokenCirculationRate; }
        }

        double IAgentConfiguration.MinimumTokenCirculationRate
        {
            get { return minimumTokenCirculationRate; }
        }

        #endregion
    }
}
