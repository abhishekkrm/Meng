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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Multicasting7
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class ReliableRegionViewSinks
        : QS.Fx.Inspection.Inspectable, 
        Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>, 
        Base6_.ICollectionOf<Base3_.RVID, ReliableRegionViewSink>,
        QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        // private const double Default_RetransmissionTimeout = 30;

        public ReliableRegionViewSinks(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger,
            QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock,  uint dataChannel, uint retransmissionChannel,
            Base6_.ICollectionOf<Base3_.RegionID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks,
            Receivers4.IRegionalController regionalController,
            Receivers4.ICollectingAgentCollection<Base3_.RVID> collectingAgentCollection,
            Membership2.Controllers.IMembershipController membershipController,
            Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.FlowControl3.IRateControlled> rateControlled,
            double default_retransmissionTimeout, Base1_.IFactory<FlowControl7.IRateController> rateControllers, double default_warmup_delay)
        {
            this.statisticsController = statisticsController;
            this.default_retransmissionTimeout = default_retransmissionTimeout;
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.retransmissionChannel = retransmissionChannel;
            this.dataChannel = dataChannel;
            this.downstreamSinks = downstreamSinks;
            this.regionalController = regionalController;
            this.collectingAgentCollection = collectingAgentCollection;
            this.membershipController = membershipController;
            this.rateControlled = rateControlled;
            this.rateControllers = rateControllers;
            this.default_warmup_delay = default_warmup_delay;

            ((Membership2.Consumers.IRegionChangeProvider)membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.RegionChangedCallback(this._RegionChangedCallback);
        }

        private void _RegionChangedCallback(QS._qss_c_.Membership2.Consumers.RegionChange change)
        {
            List<Base3_.RVID> _toremove = new List<QS._qss_c_.Base3_.RVID>();
            foreach (Membership2.ClientState.IRegion region in change.RegionsRemoved)
            {
                foreach (Base3_.RVID _rvid in sinks.Keys)
                {
                    if (_rvid.RegionID.Equals(region.ID))
                        _toremove.Add(_rvid);
                }
            }

            foreach (Base3_.RVID _rvid in _toremove)
            {
                logger.Log("removing region " + _rvid.ToString());
                sinks.Remove(_rvid);
            }
        }

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private double default_retransmissionTimeout;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private uint dataChannel, retransmissionChannel;
        private Base6_.ICollectionOf<Base3_.RegionID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        [QS._core_c_.Diagnostics.ComponentCollection("Sinks")]
        private IDictionary<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinks =
            new Dictionary<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private Receivers4.IRegionalController regionalController;
        private Receivers4.ICollectingAgentCollection<Base3_.RVID> collectingAgentCollection;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.FlowControl3.IRateControlled> rateControlled;
        private Base1_.IFactory<FlowControl7.IRateController> rateControllers;
        private double default_warmup_delay;

        public double RetransmissionTimeout
        {
            get { return default_retransmissionTimeout; }
            set { default_retransmissionTimeout = value; }
        }

        #region ICollectionOf<RVID,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS._qss_c_.Base3_.RVID address]
        {
            get 
            {
                lock (this)
                {
                    if (sinks.ContainsKey(address))
                        return sinks[address];
                    else
                    {
                        Membership2.Controllers.IRegionController regionController = membershipController.lookupRegion(address.RegionID);
                        Membership2.Controllers.IRegionViewController regionVC = (Membership2.Controllers.IRegionViewController) regionController[address.SeqNo];

                        bool useloopback = regionVC.Members.Contains(localAddress);
                        bool usenetwork = !(useloopback && (regionVC.Members.Count == 1));

                        ReliableRegionViewSink sink = new ReliableRegionViewSink(statisticsController, logger, eventLogger,
                            localAddress, alarmClock, clock, address, dataChannel, retransmissionChannel, downstreamSinks[address.RegionID],
                            regionalController, collectingAgentCollection[address],
                            (rateControlled != null) ? rateControlled[regionController.Address] : null,
                            default_retransmissionTimeout, (rateControllers != null) ? rateControllers.Create() : null, useloopback, usenetwork,
                            default_warmup_delay);
                        sinks[address] = sink;

                        ((QS._core_c_.Diagnostics2.IContainer) diagnosticsContainer).Register(
                            address.ToString(), ((QS._core_c_.Diagnostics2.IModule) sink).Component, QS._core_c_.Diagnostics2.RegisteringMode.Force);

                        return sink;
                    }
                }                
            }
        }

        #endregion

        #region ICollectionOf<RVID,ReliableRegionViewSink> Members

        public ReliableRegionViewSink this[QS._qss_c_.Base3_.RVID address]
        {
            get 
            { 
                return (ReliableRegionViewSink)(((Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>)this)[address]); 
            }
        }

        #endregion
    }
}
