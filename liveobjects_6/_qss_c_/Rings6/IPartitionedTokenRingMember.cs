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

#define OPTION_ProcessingCrashes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    /// <summary>
    /// The interface for a module that handles the multiple different cases of token generation, receipt and processing.
    /// </summary>
    /// <typeparam name="InternalClass">The class used to implement the token circulated inside the partitions.</typeparam>
    /// <typeparam name="ExternalClass">The class used to implement the token passed between partitions.</typeparam>
    public interface IPartitionedTokenRingMember<InternalClass, ExternalClass>
    {
#if OPTION_ProcessingCrashes

        bool IsActive
        {
            get;
        }

        void Quiesce();
        void Resume();

        event EventHandler OnResume;

#endif

        /// <summary>
        /// The case invoked when the node is the only element of the region, i.e. there is a single partition with a single member.
        /// It will be triggered by token callback and just talk to sender without circulating any tokens at all.
        /// </summary>
        void Process();

        /// <summary>
        /// The case invoked when the node is the region leader and the first node in its multi-node partition. 
        /// It will be triggered by callback and generate a partition token.
        /// </summary>
        /// <param name="outgoingToken"></param>
        void Process(out InternalClass outgoingToken);

        /// <summary>
        /// The case invoked whtn the node is the region leader and the only node in its partition, but there are also other partitions.
        /// It will collect its own partition info locally and generate inter-partition token.
        /// </summary>
        /// <param name="outgoingToken"></param>
        void Process(out ExternalClass outgoingToken);

        /// <summary>
        /// The case invoked when the node, a region and partition leader in the only multi-node partition, collects a partition token,
        /// and there are no other partitions to talk to, hence the info gathered within this partition is communicated to the sender
        /// directly.
        /// </summary>
        void Process(QS._core_c_.Base3.InstanceID incomingAddress, InternalClass incomingToken);

        /// <summary>
        /// The case invoked when the node receives a partition token and just passes it further, hence it is not a partition nor a region
        /// leader and nothing gets collected.
        /// </summary>
        /// <param name="incomingToken"></param>
        /// <param name="outgoingToken"></param>
        void Process(QS._core_c_.Base3.InstanceID incomingAddress, InternalClass incomingToken, out InternalClass outgoingToken);

        /// <summary>
        /// The case invoked when the node, a partition leader, receives partition token, finalizing its partition round, and passes the
        /// control to the next partition.
        /// </summary>
        void Process(QS._core_c_.Base3.InstanceID incomingAddress, InternalClass incomingToken, out ExternalClass outgoingToken);

        /// <summary>
        /// The case invoked when the node is a region leader, just receives a token from another partion, which implies that the
        /// full regional round is complete, info needs to be communicated to the sender and the next round initiated.
        /// </summary>
        void Process(QS._core_c_.Base3.InstanceID incomingAddress, ExternalClass incomingToken);

        /// <summary>
        /// The case invoked when the node is a partition leader, receives a token from another partition and starts a round in its own
        /// partition in response.
        /// </summary>
        void Process(QS._core_c_.Base3.InstanceID incomingAddress, ExternalClass incomingToken, out InternalClass outgoingToken);

        /// <summary>
        /// The case invoked when the node is a partition leader in a singleton partition, but not a region leader, it receives a token
        /// from another partition, should process it locally and pass to the next partition.
        /// </summary>
        void Process(QS._core_c_.Base3.InstanceID incomingAddress, ExternalClass incomingToken, out ExternalClass outgoingToken);
    }
}
