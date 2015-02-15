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
    public class ReliableGroupSinks : QS.Fx.Inspection.Inspectable,
        Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public ReliableGroupSinks(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Clock.IClock clock,
            Membership2.Controllers.IMembershipController membershipController,
            Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks, QS.Fx.Logging.ILogger logger,
            int defaultMaximumNumberOfPendingCompletion, int defaultFeedBufferMin, int defaultFeedBufferMax)
        {
            this.statisticsController = statisticsController;
            this.clock = clock;
            this.membershipController = membershipController;
            this.downstreamSinks = downstreamSinks;
            this.logger = logger;
            this.defaultMaximumNumberOfPendingCompletion = defaultMaximumNumberOfPendingCompletion;
            this.defaultFeedBufferMin = defaultFeedBufferMin;
            this.defaultFeedBufferMax = defaultFeedBufferMax;

            ((Membership2.Consumers.IGroupChangeProvider)membershipController).OnChange +=
                new QS._qss_c_.Membership2.Consumers.GroupChangedCallback(this.GroupChangeCallback);

            ((Membership2.Consumers.IGroupCreationAndRemovalProvider)membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.GroupCreationOrRemovalCallback(this._GroupCreationOrRemovalCallback);
        }

        private void _GroupCreationOrRemovalCallback(IEnumerable<QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval> notifications)
        {
            lock (this)
            {
                foreach (QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval notification in notifications)
                {
                    if (!notification.Creation)
                    {
                        logger.Log("ReliableGroupSinks : Removing Group " + notification.ID.ToString());
                        ((IDisposable) (sinks[notification.ID])).Dispose();
                        sinks.Remove(notification.ID);
                    }
                }
            }
        }

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private QS.Fx.Clock.IClock clock;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        private QS.Fx.Logging.ILogger logger;
        [QS._core_c_.Diagnostics.ComponentCollection("Sinks")]
        private IDictionary<Base3_.GroupID, ReliableGroupSink> sinks = new Dictionary<Base3_.GroupID, ReliableGroupSink>();
        private int defaultMaximumNumberOfPendingCompletion, defaultFeedBufferMin, defaultFeedBufferMax;

        private void GroupChangeCallback(IEnumerable<QS._qss_c_.Membership2.Consumers.GroupChange> changes)
        {
            lock (this)
            {
                foreach (Membership2.Consumers.GroupChange change in changes)
                {
                    ReliableGroupSink sink;
                    if (sinks.TryGetValue(change.CurrentView.Group.ID, out sink))
                        sink.MembershipChanged((Membership2.Controllers.IGroupViewController)change.CurrentView);
                }
            }
        }

        #region ICollectionOf<GroupID,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> 
            QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID, 
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS._qss_c_.Base3_.GroupID address]
        {
            get 
            {
                lock (this)
                {
                    if (sinks.ContainsKey(address))
                        return sinks[address];
                    else
                    {
                        ReliableGroupSink sink = new ReliableGroupSink(
                            statisticsController, clock, address, membershipController, downstreamSinks, logger, 
                            defaultMaximumNumberOfPendingCompletion, defaultFeedBufferMin, defaultFeedBufferMax);
                        sinks[address] = sink;

                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(
                            address.ToString(), ((QS._core_c_.Diagnostics2.IModule)sink).Component, QS._core_c_.Diagnostics2.RegisteringMode.Force);

                        return sink;
                    }
                }                
            }
        }

        #endregion
    }
}
