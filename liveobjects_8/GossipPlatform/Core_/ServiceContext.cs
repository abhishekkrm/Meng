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
using System.Collections.Generic;
using System.Threading;
using System.Text;
using GOTransport.Common;
using GOTransport.Core;
using GOTransport.GossipTransport;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    public class ServiceContext
    {
        #region private fields

        private string name { get; set; }
        private GroupObjectGraph groupObjectGraph { get; set; }
        private GroupOverlapGraph groupOverlapGraph { get; set; }
        private RumorBuffer rumorBuffer { get; set; }
        private NodeAdaptor nodeAdaptor { get; set; }
        private UDPGossipTransport udpGossipTransport { get; set; }
        private Queue<IGossip> receivedRumorQueue { get; set; }
        private AutoResetEvent receiveAutoResetEvent { get; set; }
        private AutoResetEvent feedEventClientSide;
        private AutoResetEvent feedEventServiceSide;
        private List<Rumor> fedRumor;

        #endregion

        #region constructors

        public ServiceContext(ref string _name,
                              ref GroupObjectGraph _groupObjectGraph, 
                              ref GroupOverlapGraph _groupOverlapGraph, 
                              ref RumorBuffer _rumorBuffer,
                              ref NodeAdaptor _nodeAdaptor,
                              ref UDPGossipTransport _udpGossipTransport,
                              ref Queue<IGossip> _receivedRumorQueue,
                              ref AutoResetEvent _receiveAutoResetEvent,
                              ref AutoResetEvent _feedEventClientSide,
                              ref AutoResetEvent _feedEventServiceSide,
                              ref List<Rumor> _fedRumor)
        {
            name = _name;
            groupObjectGraph = _groupObjectGraph;
            groupOverlapGraph = _groupOverlapGraph;
            rumorBuffer = _rumorBuffer;
            nodeAdaptor = _nodeAdaptor;
            udpGossipTransport = _udpGossipTransport;
            receivedRumorQueue = _receivedRumorQueue;
            receiveAutoResetEvent = _receiveAutoResetEvent;
            feedEventClientSide = _feedEventClientSide;
            feedEventServiceSide = _feedEventServiceSide;
            fedRumor = _fedRumor;
        }

        #endregion

        #region public methods

        public void GetName(out string _name)
        {
            _name = name;
        }

        public void GetRumorBuffer(out RumorBuffer _rumorBuffer)
        {
            _rumorBuffer = rumorBuffer;
        }

        public void GetGraphs(out GroupObjectGraph _groupObjectGraph, out GroupOverlapGraph _groupOverlapGraph)
        {
            _groupObjectGraph = groupObjectGraph;
            _groupOverlapGraph = groupOverlapGraph;
        }

        public void GetAdaptors(out NodeAdaptor _nodeAdaptor)
        {
            _nodeAdaptor = nodeAdaptor;
        }

        public void GetTransport(out UDPGossipTransport _udpGossipTransport)
        {
            _udpGossipTransport = udpGossipTransport;
        }

        public void GetReceivedRumorQueue(out Queue<IGossip> _receivedRumorQueue)
        {
            _receivedRumorQueue = receivedRumorQueue;
        }

        public void GetReceiveAutoResetEvent(out AutoResetEvent _receiveAutoResetEvent)
        {
            _receiveAutoResetEvent = receiveAutoResetEvent;
        }

        public void GetFeedContext(out AutoResetEvent _feedEventClientSide, 
                                   out AutoResetEvent _feedEventServiceSide, 
                                   out List<Rumor> _fedRumor)
        {
            _feedEventClientSide = feedEventClientSide;
            _feedEventServiceSide = feedEventServiceSide;
            _fedRumor = fedRumor;
        }

        public void GetFedRumor(out List<Rumor> _fedRumor)
        {
            _fedRumor = fedRumor;
        }

        public void SetFedRumor(ref List<Rumor> _fedRumor)
        {
            this.fedRumor = _fedRumor;
        }

        #endregion
    }
}
