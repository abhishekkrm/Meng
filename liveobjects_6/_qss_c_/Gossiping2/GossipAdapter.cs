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

// #define DEBUG_GossipAdapter

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Gossiping2
{
    public class GossipAdapter : IGossipAdapter
    {
        public GossipAdapter(QS.Fx.Logging.ILogger logger, IRegionalAgent regionalAgent)
        {
            this.logger = logger;
            regionalAgent.GetAggregatableCallback = new QS._qss_c_.Gossiping2.GetAggregatableCallback(this.GetAggregatableCallback);
            regionalAgent.SetAggregatableCallback = new SetAggregatableCallback(this.SetAggregatableCallback);
        }

        private QS.Fx.Logging.ILogger logger;
        private IDictionary<ChannelID, Channel> registeredChannels = new Dictionary<ChannelID, Channel>();

        #region Class Channel

        private class Channel
        {
            public Channel(GetAggregatableCallback getAggregatableCallback, SetAggregatableCallback setAggregatableCallback)
            {
                this.getAggregatableCallback = getAggregatableCallback;
                this.setAggregatableCallback = setAggregatableCallback;
            }

            private GetAggregatableCallback getAggregatableCallback;
            private SetAggregatableCallback setAggregatableCallback;

            public Aggregation3_.IAggregatable GetAggregatable
            {
                get { return getAggregatableCallback(); }
            }

            public void SetAggregatable(Base3_.RegionID regionID, Aggregation3_.IAggregatable aggregatedObject)
            {
                setAggregatableCallback(regionID, aggregatedObject);
            }
        }

        #endregion

        #region ChannelDic

        [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_GossipAdapter_ChannelDic)]
        private class ChannelDic : AggregatableDic<ChannelID>
        {
            public ChannelDic()
            {
            }

            public override ClassID ClassID
            {
                get { return ClassID.Gossiping2_GossipAdapter_ChannelDic; }
            }
        }

        #endregion

        #region GetAggregatableCallback

        private Aggregation3_.IAggregatable GetAggregatableCallback()
        {
            lock (this)
            {
#if DEBUG_GossipAdapter
                logger.Log(this, "__GetAggregatableCallback");
#endif

                ChannelDic channelDic = new ChannelDic();
                foreach (KeyValuePair<ChannelID, Channel> element in registeredChannels)
                    channelDic.add(element.Key, element.Value.GetAggregatable);
                return channelDic;
            }
        }

        #endregion

        #region SetAggregatableCallback

        private void SetAggregatableCallback(Base3_.RegionID regionID, Aggregation3_.IAggregatable aggregatableObject)
        {
#if DEBUG_GossipAdapter
            logger.Log(this, "__SetAggregatableCallback : " + regionID.ToString() + ", " + aggregatableObject.ToString());                
#endif

            ChannelDic channelDic = aggregatableObject as ChannelDic;
            if (channelDic == null)
                throw new Exception("Wrong object type received.");
            AggregatableDic<ChannelID> dictionary = (AggregatableDic<ChannelID>)channelDic;

            foreach (KeyValuePair<ChannelID, Aggregation3_.IAggregatable> element in ((AggregatableDic<ChannelID>)channelDic).Dictionary)
            {
                if (registeredChannels.ContainsKey(element.Key))
                    registeredChannels[element.Key].SetAggregatable(regionID, element.Value);
                else
                    logger.Log(this, "__SetAggregatableCallback: Channel for " + element.Key.ToString() + " not registered, cannot receive.");                    
            }
        }

        #endregion

        #region IGossipAdapter Members

        void IGossipAdapter.Register(
            ChannelID channelID, GetAggregatableCallback getAggregatableCallback, SetAggregatableCallback setAggregatableCallback)
        {
            lock (this)
            {
                if (registeredChannels.ContainsKey(channelID))
                    throw new ArgumentException("Channel " + channelID.ToString() + " is already taken.");

#if DEBUG_GossipAdapter
                logger.Log(this, "__Register : Registering Channel " + channelID.ToString());
#endif

                registeredChannels[channelID] = new Channel(getAggregatableCallback, setAggregatableCallback);
            }
        }

        void IGossipAdapter.Register(IGossipChannelController gossipChannelController)
        {
            ((IGossipAdapter)this).Register(gossipChannelController.ChannelID,
                new GetAggregatableCallback(delegate { return gossipChannelController.GetAggregatable; }),
                new SetAggregatableCallback(gossipChannelController.SetAggregatable));
        }

        #endregion
    }
}
