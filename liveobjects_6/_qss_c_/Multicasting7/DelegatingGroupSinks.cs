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
    public class DelegatingGroupSinks
        : QS.Fx.Inspection.Inspectable, Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>
    {
        public DelegatingGroupSinks(Membership2.Controllers.IMembershipController membershipController,
            Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks,
            int defaultMaximumNumberOfPendingCompletion)
        {
            this.membershipController = membershipController;
            this.downstreamSinks = downstreamSinks;
            this.defaultMaximumNumberOfPendingCompletion = defaultMaximumNumberOfPendingCompletion;
        }

        private Membership2.Controllers.IMembershipController membershipController;
        private Base6_.ICollectionOf<Base3_.RVID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        [QS._core_c_.Diagnostics.ComponentCollection("Sinks")]
        private IDictionary<Base3_.GroupID, DelegatingGroupSink> sinks = new Dictionary<Base3_.GroupID, DelegatingGroupSink>();
        private int defaultMaximumNumberOfPendingCompletion;

        #region ICollectionOf<GroupID,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS._qss_c_.Base3_.GroupID address]
        {
            get 
            {
                lock (this)
                {
                    if (sinks.ContainsKey(address))
                        return sinks[address];
                    else
                    {
                        DelegatingGroupSink sink = new DelegatingGroupSink(address, membershipController, downstreamSinks, 
                            defaultMaximumNumberOfPendingCompletion);
                        sinks[address] = sink;
                        return sink;
                    }
                }
            }
        }

        #endregion
    }
}
