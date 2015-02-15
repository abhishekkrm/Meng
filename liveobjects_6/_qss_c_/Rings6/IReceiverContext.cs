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

namespace QS._qss_c_.Rings6
{
    public interface IReceiverContext
    {
        QS.Fx.Logging.ILogger Logger
        {
            get;
        }

        QS.Fx.Logging.IEventLogger EventLogger
        {
            get;
        }

        QS._core_c_.Base3.InstanceID LocalAddress
        {
            get;
        }

        string Name
        {
            get;
        }

        uint NumberOfPartitions
        {
            get;
        }

        uint PartitionNumber
        {
            get;
        }

        Base3_.IDemultiplexer Demultiplexer
        {
            get;
        }

        QS.Fx.Clock.IClock Clock
        {
            get;
        }

        QS._core_c_.Base3.InstanceID[] ReceiverAddresses
        {
            get;
        }

        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> SenderCollection
        {
            get;
        }

        Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> SinkCollection
        {
            get;
        }

        Receivers4.IReceivingAgentContext AgentContext
        {
            get;
        }

        // extra stuff for the paper

        void RateSample();

        double ReceiveRate
        {
            get;
        }


    }
}
