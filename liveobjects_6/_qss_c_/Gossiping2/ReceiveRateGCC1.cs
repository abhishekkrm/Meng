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

// #define DEBUG_ReceiveRateGCC1

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Gossiping2
{    
    public class ReceiveRateGCC1 : QS.Fx.Inspection.Inspectable, IGossipChannelController, QS._qss_e_.Base_1_.IStatisticsCollector
    {
        public ReceiveRateGCC1(QS.Fx.Logging.ILogger logger, Base3_.RootSender rootSender)
        {
            this.logger = logger;
            this.rootSender = rootSender;
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.RootSender rootSender;
        private IDictionary<Base3_.RegionID, FlowControl3.IEstimator> estimators =
            new Dictionary<Base3_.RegionID, FlowControl3.IEstimator>();                        

        #region IGossipChannelController Members

        ChannelID IGossipChannelController.ChannelID
        {
            get { return new ChannelID(ChannelID.ReservedIDs.Base3_RootSender_ReceiveRate); }
        }

        QS._qss_c_.Aggregation3_.IAggregatable IGossipChannelController.GetAggregatable
        {
            get { return new Multicasting3.Minimum(rootSender.ReceiveRate); }
        }

        private FlowControl3.IEstimator GetEstimator(Base3_.RegionID regionID)
        {
            lock (this)
            {
                if (estimators.ContainsKey(regionID))
                    return estimators[regionID];
                else
                {
                    FlowControl3.IEstimator estimator = new FlowControl3.SmoothingEstimator();
                    estimators[regionID] = estimator;
                    return estimator;
                }
            }
        }

        void IGossipChannelController.SetAggregatable(QS._qss_c_.Base3_.RegionID regionID, QS._qss_c_.Aggregation3_.IAggregatable aggregatedObject)
        {
#if DEBUG_ReceiveRateGCC1
            logger.Log(this, "__SetAggregatable : " + regionID.ToString() + ", " + aggregatedObject.ToString());
#endif

            GetEstimator(regionID).AddSample(((Multicasting3.Minimum)aggregatedObject).Value);
        }

        #endregion

        public FlowControl3.IEstimator this[Base3_.RegionID regionID]
        {
            get { return GetEstimator(regionID); }
        }

        #region IStatisticsCollector Members
        
        IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
        {
            get 
            {
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();

                foreach (KeyValuePair<Base3_.RegionID, FlowControl3.IEstimator> element in estimators)
                {
                    QS._qss_e_.Base_1_.IStatisticsCollector statisticsCollector = element.Value as QS._qss_e_.Base_1_.IStatisticsCollector;
                    if (statisticsCollector != null)
                    {
                        s.Add(new QS._core_c_.Components.Attribute(element.Key.ToString(), 
                            new QS._core_c_.Components.AttributeSet(statisticsCollector.Statistics)));
                    }
                }

                return s;
            }
        }

        #endregion

        [QS.Fx.Base.Inspectable]
        public QS._core_c_.Components.AttributeSet Statistics
        {
            get { return new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector)this).Statistics); } 
        }
    }
}
