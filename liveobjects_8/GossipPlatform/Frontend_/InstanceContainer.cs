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
using GOTransport.Core;
using GOTransport.GossipTransport;
using GOTransport.Common;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Frontend
{
    /// <summary>
    /// container for state of each instance of the gossip service
    /// </summary>
    public class InstanceContainer
    {
        #region private fields

        // map for maintaining the state of each instance, indexed by the port
        private static Dictionary<string, ServiceContext> serviceContextMap
                 = new Dictionary<string, ServiceContext>();

        private string outPort;
        private string gossipInterval;
        private string rumorTimeout;
        private string roundsToGossip;

        #endregion

        #region public fields

        // public getters
        public string OutPort { get { return outPort; } }
        public int GossipInterval { get { return Int32.Parse(gossipInterval); } }
        public int RumorTimeout { get { return Int32.Parse(rumorTimeout); } }
        public int RoundsToGossip { get { return Int32.Parse(roundsToGossip); } }

        #endregion

        #region constructors

        public InstanceContainer(string _outPort, string _gosssipInterval, string _rumorTimeout, string _rounds)
        {
            outPort = _outPort;
            gossipInterval = _gosssipInterval;
            rumorTimeout = _rumorTimeout;
            roundsToGossip = _rounds;
        }

        #endregion

        #region public methods

        /// <summary>
        /// read the context corresponding to the port and set the state in passed parameters
        /// </summary>
        /// <param name="port">port for which state is requested</param>
        /// <param name="groupObjectGraph">groupObjectGraph corresponding to 'port' instance of the platform</param>
        /// <param name="groupOverlapGraph">groupOverlapGraph corresponding to 'port' instance of the platform</param>
        /// <param name="udpGossipTransport">udpGossipTransport corresponding to 'port' instance of the platform</param>
        /// <param name="name">name corresponding to 'port' instance of the platform</param>
        /// <param name="rumorBuffer">rumorBuffer corresponding to 'port' instance of the platform</param>
        /// <param name="nodeAdaptor">nodeAdaptor corresponding to 'port' instance of the platform</param>
        /// <param name="receivedRumorQueue">receivedRumorQueue corresponding to 'port' instance of the platform</param>
        public static void SetWorkingContext(string port, out GroupObjectGraph _groupObjectGraph,
                                        out GroupOverlapGraph _groupOverlapGraph,
                                        out UDPGossipTransport _udpGossipTransport,
                                        out string _name,
                                        out RumorBuffer _rumorBuffer,
                                        out NodeAdaptor _nodeAdaptor,
                                        out Queue<IGossip> _receivedRumorQueue,
                                        out AutoResetEvent _receiveCallback,
                                        out AutoResetEvent _feedEventClientSide,
                                        out AutoResetEvent _feedEventServiceSide,
                                        out List<Rumor> _fedRumor)
        {
            ServiceContext sc = GetContext(port);

            sc.GetGraphs(out _groupObjectGraph, out _groupOverlapGraph);
            sc.GetTransport(out _udpGossipTransport);
            sc.GetName(out _name);
            sc.GetRumorBuffer(out _rumorBuffer);
            sc.GetAdaptors(out _nodeAdaptor);
            sc.GetReceivedRumorQueue(out _receivedRumorQueue);
            sc.GetReceiveAutoResetEvent(out _receiveCallback);
            sc.GetFeedContext(out _feedEventClientSide, out _feedEventServiceSide, out _fedRumor);
        }

        public static void Clear()
        {
            serviceContextMap.Clear();
        }

        /// <summary>
        /// return the service context for the requested port
        /// </summary>
        /// <param name="port">port for which context is requested</param>
        /// <returns>service context of the corresponding to the port</returns>
        public static ServiceContext GetContext(string _port)
        {
            Console.WriteLine("Getting context for port [" + _port + "] from contextMap: " + serviceContextMap.GetHashCode());
            if (serviceContextMap == null || serviceContextMap.Count == 0 || !serviceContextMap.ContainsKey(_port))
            {
                Console.WriteLine("Getting context for port [" + _port + "]: not found");
                return null;
            }

            Console.WriteLine("Getting context for port [" + _port + "], returning: " + serviceContextMap[_port]);
            return serviceContextMap[_port];
        }

        /// <summary>
        /// create a new state object (ServiceContext) to maintain the state of a platform instance
        /// </summary>
        /// <param name="_outPort">port of the instance for which state has to be created (and maintained)</param>
        public static void CreateContext(string _outPort, Dictionary<string,string> _config)
        {
            if (serviceContextMap.ContainsKey(_outPort))
            {
                throw new Exception("The specified port already exists");
            }

            string name = _outPort;
            GroupObjectGraph groupObjectGraph = new GroupObjectGraph(_config);
            GroupOverlapGraph groupOverlapGraph = new GroupOverlapGraph(ref groupObjectGraph);
            RumorBuffer rumorBuffer = new RumorBuffer();
            NodeAdaptor nodeAdaptor = new NodeAdaptor(ref groupObjectGraph, ref groupOverlapGraph);
            Queue<IGossip> receivedRumorQueue = new Queue<IGossip>();
            UDPGossipTransport udpGossipTransport = new UDPGossipTransport(_outPort, rumorBuffer, receivedRumorQueue, nodeAdaptor);
            AutoResetEvent receiveAutoResetEvent = new AutoResetEvent(false);
            AutoResetEvent feedEventClientSide = new AutoResetEvent(false);
            AutoResetEvent feedEventServiceSide = new AutoResetEvent(false);
            List<Rumor> fedRumor = null;

            // create a context for the new platform instance and save its reference in ServiceContextMap
            ServiceContext sc = new ServiceContext(ref name,
                                                   ref groupObjectGraph, 
                                                   ref groupOverlapGraph, 
                                                   ref rumorBuffer,
                                                   ref nodeAdaptor, 
                                                   ref udpGossipTransport,
                                                   ref receivedRumorQueue,
                                                   ref receiveAutoResetEvent,
                                                   ref feedEventClientSide,
                                                   ref feedEventServiceSide,
                                                   ref fedRumor);
            serviceContextMap[_outPort] = sc;

            Console.WriteLine("GOServiceFacade::AddContext:Added to contextMap: "
                            + "name: " + name + ", _outPort: " + _outPort + ", ServiceContext: " + sc.GetHashCode()
                            + ", groupObjectGraph: " + groupObjectGraph.GetHashCode()
                            + ", groupOverlapGraph: " + groupOverlapGraph.GetHashCode()
                            + ", rumorBuffer: " + rumorBuffer.GetHashCode()
                            + ", udpGossipTransport: " + udpGossipTransport.GetHashCode()
                            + ", nodeAdaptor: " + nodeAdaptor.GetHashCode());

        }

        #endregion
    }
}
