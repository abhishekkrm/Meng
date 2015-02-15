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

// #define DEBUG_AggregationClass

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
    public class AggregationClass : Aggregation1_.IAggregationClass
    {
        public AggregationClass(QS.Fx.Logging.ILogger logger, 
            Membership2.Controllers.IMembershipController membershipController, Routing_1_.IRoutingAlgorithm routingAlgorithm)
        {
            this.logger = logger;
            this.membershipController = membershipController;
            this.routingAlgorithm = routingAlgorithm;
        }

        private QS.Fx.Logging.ILogger logger;
        private Membership2.Controllers.IMembershipController membershipController;
        private Routing_1_.IRoutingAlgorithm routingAlgorithm;

        #region IAggregationClass Members

        ClassID QS._qss_c_.Aggregation1_.IAggregationClass.AssociatedClass
        {
            get { return ClassID.Multicasting3_MessageID; }
        }

        bool QS._qss_c_.Aggregation1_.IAggregationClass.resolve(Aggregation1_.IAggregationKey aggregationKey, 
            out Routing_2_.IStructure<QS._core_c_.Base3.InstanceID> routingStructure, out Failure_.ISource failureSource)
        {
            MessageID messageID = aggregationKey as MessageID;
            if (messageID != null)
            {
#if DEBUG_AggregationClass
                string reason = null;
#endif
                try
                {
#if DEBUG_AggregationClass
                    reason = "cannot find group";
#endif
                    
                    Membership2.Controllers.IGroupController groupController = 
                        (Membership2.Controllers.IGroupController)membershipController[messageID.GroupID];

#if DEBUG_AggregationClass
                    reason = "cannot find view in a group";
#endif
                    
                    Membership2.Controllers.IGroupViewController viewController =
                        (Membership2.Controllers.IGroupViewController)groupController[messageID.ViewSeqNo];

#if DEBUG_AggregationClass
                    reason = "cannot instantiate routing structure";
#endif

                    Routing_2_.IStructure<QS._core_c_.Base3.InstanceID> immutableStructure =
                        new Routing_2_.ImmutableStructure<QS._core_c_.Base3.InstanceID>(viewController.Members, routingAlgorithm,
                        messageID.GroupID.GetHashCode());

                    routingStructure = immutableStructure;
                    failureSource = viewController;

                    return true;
                }
                catch (Exception)
                {
                    routingStructure = null;
                    failureSource = null;

#if DEBUG_AggregationClass
                    logger.Log(this, "Cannot resolve: " + aggregationKey.ToString() + ", reason: " + reason + ".");
#endif

                    return false;
                }
            }
            else
            {
                throw new ArgumentException("The aggregation key was expected to be of class " +
                    ((Aggregation1_.IAggregationClass)this).AssociatedClass.ToString() + ", but it is of an incompatible class " +
                    ((Base3_.IKnownClass) aggregationKey).ClassID.ToString());
            }
        }

        #endregion
    }
}
