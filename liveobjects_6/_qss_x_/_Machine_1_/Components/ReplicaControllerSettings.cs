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

namespace QS._qss_x_._Machine_1_.Components
{
    public sealed class ReplicaControllerSettings : IReplicaControllerSettings
    {
        public const double DefaultDiscoveryTimeout = 20;
        public const double DefaultDiscoveryRate = 5;
        public const double DefaultDiscoveryBroadcastTimeout = 1;
        public const double DefaultTokenRate = 5;
        public const double DefaultPendingOperationsBatchingInterval = 0.001;

        public ReplicaControllerSettings()
        {
            discoveryTimeout = DefaultDiscoveryTimeout;
            discoveryRate = DefaultDiscoveryRate;
            discoveryBroadcastTimeout = DefaultDiscoveryBroadcastTimeout;
            tokenRate = DefaultTokenRate;
            pendingOperationsBatchingInterval = DefaultPendingOperationsBatchingInterval;
        }

        private double discoveryTimeout, tokenRate, discoveryRate, discoveryBroadcastTimeout, pendingOperationsBatchingInterval;

        #region IReplicaControllerSettings Members

        double IReplicaControllerSettings.DiscoveryTimeout
        {
            get { return discoveryTimeout; }
        }

        double IReplicaControllerSettings.TokenRate
        {
            get { return tokenRate; }
        }

        double IReplicaControllerSettings.DiscoveryRate
        {
            get { return discoveryRate; }
        }

        double IReplicaControllerSettings.DiscoveryBroadcastTimeout
        {
            get { return discoveryBroadcastTimeout; }
        }

        double IReplicaControllerSettings.PendingOperationsBatchingInterval
        {
            get { return pendingOperationsBatchingInterval; }
        }

        #endregion
    }
}
