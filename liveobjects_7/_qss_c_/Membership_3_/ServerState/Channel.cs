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

namespace QS._qss_c_.Membership_3_.ServerState
{
    /// <summary>
    /// Channel represents a connection between a particular client node and a particular group view. Channels are created when a node joins a 
    /// particular group as a client, a channel is then added to the current group view. After a group view enters the flushing stage, no new channels
    /// can be added (but existing channels can still be removed, this happens when a given client node leaves a group, whether active or not,
    /// before flushing in that group is complete). 
    /// The state of the channel includes the information about the maximum bandwidth allocated for a given client within the given group view. The
    /// client should not exceed this bandwidth limit. The limit will change as other clients come and go or capacity of the region views changes.
    /// Clients in such case receive updates (but versioning is not used, it is not necessary).  
    /// Channels live in the perspective of a region view member, the member .
    /// </summary>
    public sealed class Channel
    {
        public Channel()
        {
        }

/*
        private GroupView groupView;
        private Node client;
        private double bandwidthAllocated;
*/

    }
}
