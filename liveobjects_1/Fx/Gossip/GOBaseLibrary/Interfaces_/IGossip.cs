/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Threading;
using GOBaseLibrary.Common;

namespace GOBaseLibrary.Interfaces
{
    /// <summary>
    /// Gossip that has to spread to other nodes
    /// </summary>
    [QS.Fx.Reflection.ValueClass("5`1", "IGossip")]
    public interface IGossip
    {
        // unique identifier for this gossip
        String Id { get; set; }

        // type of gossip
        GossipType Type { get; set; }

        // source group of gossip
        Group SourceGroup { get; set; }

        // destination group of gossip
        Group DestinationGroup { get; set; }

        // source node of gossip
        Node SourceNode { get; set; }

        // destination node of gossip
        Node DestinationNode { get; set; }

        // actual message contained in the gossip
        String PayLoad { get; set; }

        double Utility { get; set; }

        void StampTheTime();

        double GetTimestamp();

        void StartTTLTicks(TimerCallback _timerDelegate, long _timeout);
    }
}
