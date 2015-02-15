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
    public interface IReplicaNonpersistentState
    {
        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Status
        // ------------------------------------------------------------------------------------------------------------------------------------------------

        ReplicaStatus Status
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Views
        // ------------------------------------------------------------------------------------------------------------------------------------------------

        bool HasView
        {
            get;
        }

        Base.MembershipView CurrentView
        {
            get;
            set;
        }

        bool IsInView
        {
            get;
        }

        bool IsCoordinator
        {
            get;
        }

        bool IsSingleton
        {
            get;
        }

        uint PositionInView
        {
            get;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Information about other nodes in the current view
        // ------------------------------------------------------------------------------------------------------------------------------------------------

        IPeerInfo PeerInfo(int index);

        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Related to the discovery process
        // ------------------------------------------------------------------------------------------------------------------------------------------------

        bool RunningDiscovery
        {
            get;
            set;
        }

        double DiscoveryTimestamp
        {
            get;
            set;
        }

        QS.Fx.Clock.IAlarm DiscoveryAlarm
        {
            get;
            set;
        }

        QS.Fx.Clock.IAlarm DiscoveryResendAlarm
        {
            get;
            set;
        }

        double DiscoveredWeight
        {
            get;
            set;
        }

        bool DiscoveredReadQuorum
        {
            get;
        }

        bool DiscoveredWriteQuorum
        {
            get;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Related to the synchronizing of local replica
        // ------------------------------------------------------------------------------------------------------------------------------------------------

        bool Synchronizing
        {
            get;
            set;
        }

        bool Synchronized
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Related to the job of coordinator
        // ------------------------------------------------------------------------------------------------------------------------------------------------

        Queue<Submission> PendingOperations
        {
            get;
        }

        bool PendingOperationsRegistered
        {
            get;
            set;
        }

        QS.Fx.Clock.IAlarm PendingOperationsAlarm
        {
            get;
            set;
        }

        uint LastSubmittedMessageNumber
        {
            get;
            set;
        }

        IDictionary<uint, Request> SubmittedOperations
        {
            get;
        }

        bool TokenCirculationIsActive
        {
            get;
            set;
        }

        double TokenTimestamp
        {
            get;
            set;
        }

        QS.Fx.Clock.IAlarm TokenCirculationAlarm
        {
            get;
            set;
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------
        // Related to operations
        // ------------------------------------------------------------------------------------------------------------------------------------------------


        

        // ------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
