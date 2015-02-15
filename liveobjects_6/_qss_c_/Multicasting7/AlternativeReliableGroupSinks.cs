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
    public class AlternativeReliableGroupSinks : QS.Fx.Inspection.Inspectable,
        Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Constructor

        public AlternativeReliableGroupSinks(
            QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Clock.IClock clock,
            Membership2.Controllers.IMembershipController membershipController, QS.Fx.Logging.ILogger logger,
            Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> nodesinks,
            Base6_.ICollectionOf<Base3_.RVID, ReliableRegionViewSink> rrvsinks, uint transmissionChannel,
            Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.FlowControl3.IRateControlled> rateControlled,
            Base1_.IFactory<FlowControl7.IRateController> rateControllers, double initialrate)
        {
            this.statisticsController = statisticsController;
            this.clock = clock;
            this.membershipController = membershipController;
            this.logger = logger;
            this.transmissionChannel = transmissionChannel;
            this.rrvsinks = rrvsinks;
            this.nodesinks = nodesinks;
            this.rateControlled = rateControlled;
            this.rateControllers = rateControllers;
            this.initialrate = initialrate;

            ((Membership2.Consumers.IGroupChangeProvider)membershipController).OnChange +=
                new QS._qss_c_.Membership2.Consumers.GroupChangedCallback(this.GroupChangeCallback);
        }

        #endregion

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private QS.Fx.Clock.IClock clock;
        private Membership2.Controllers.IMembershipController membershipController;
        private QS.Fx.Logging.ILogger logger;
        private uint transmissionChannel;
        private Base6_.ICollectionOf<Base3_.RVID, ReliableRegionViewSink> rrvsinks;
        private Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> nodesinks;
        private Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.FlowControl3.IRateControlled> rateControlled;
        private Base1_.IFactory<FlowControl7.IRateController> rateControllers;
        private double initialrate;

        [QS._core_c_.Diagnostics.ComponentCollection("Sinks")]
        private IDictionary<Base3_.GroupID, AlternativeReliableGroupSink> sinks = new Dictionary<Base3_.GroupID, AlternativeReliableGroupSink>();

        #region GroupChangeCallback

        private void GroupChangeCallback(IEnumerable<QS._qss_c_.Membership2.Consumers.GroupChange> changes)
        {
            lock (this)
            {
                foreach (Membership2.Consumers.GroupChange change in changes)
                {
                    AlternativeReliableGroupSink sink;
                    if (sinks.TryGetValue(change.CurrentView.Group.ID, out sink))
                        sink.MembershipChanged((Membership2.Controllers.IGroupViewController)change.CurrentView);
                }
            }
        }

        #endregion

        #region ICollectionOf<GroupID,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>
            QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS._qss_c_.Base3_.GroupID address]
        {
            get
            {
                lock (this)
                {
                    AlternativeReliableGroupSink sink;
                    if (!sinks.TryGetValue(address, out sink))
                    {
                        QS.Fx.Network.NetworkAddress multicastAddress = 
                            ((Membership2.Controllers.IGroupController) membershipController.lookupGroup(address)).Address;

                        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> downstreamSink = nodesinks[multicastAddress];

                        sinks.Add(address, sink = new AlternativeReliableGroupSink(
                            statisticsController, clock, address, membershipController, logger, downstreamSink, transmissionChannel, rrvsinks,
                            ((rateControlled != null) ? rateControlled[multicastAddress] : null), 
                            ((rateControllers != null) ? rateControllers.Create() : null), initialrate));
                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(address.ToString(), ((QS._core_c_.Diagnostics2.IModule)sink).Component);
                    }

                    return sink;
                }
            }
        }

        #endregion
    }
}
