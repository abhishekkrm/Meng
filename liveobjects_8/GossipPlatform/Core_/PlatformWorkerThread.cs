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
using System.Text;
using System.Threading;
using GOTransport.Common;
using GOTransport.Core;
using GOTransport.GossipTransport;
using System.Diagnostics;
using GOTransport.Frontend;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;
using System.Configuration;

namespace GOTransport.Core
{
    /// <summary>
    /// the actual execution thread of the core platform instance
    /// </summary>
    public class PlatformWorkerThread
    {
        #region private fields

        private object syncObj;
        private string outPort;
        private int gossipInterval;
        private int rumorTimeout;
        private int roundsToGossip;
        private PlatformWorkerThreadHelper platformWorkerThreadHelper;
        private AutoResetEvent nodeInfoEvent;
        private Thread platformThread;
        private DebuggingContext debuggingContext;
        private AutoResetEvent receiveAutoResetEvent;
        private IGOConnection goConnection;

        #endregion

        #region public methods

        /// <summary>
        /// initialize the platform instance, indexed by port
        /// </summary>
        /// <param name="_outPort">port for this instance</param>
        /// <param name="_gossipInterval"></param>
        /// <param name="_rumorTimeout"></param>
        public PlatformWorkerThread(string _outPort, int _gossipInterval, int _rumorTimeout, int _rounds, IGOConnection _goConnection)
        {
            this.outPort = _outPort;
            this.gossipInterval = _gossipInterval;
            this.rumorTimeout = _rumorTimeout;
            this.roundsToGossip = _rounds;
            this.nodeInfoEvent = new AutoResetEvent(false);
            this.goConnection = _goConnection;
            platformThread = new Thread(this.Execute);
            platformThread.Priority = ThreadPriority.Highest;
            syncObj = new object();
            debuggingContext = new DebuggingContext();
        }

        /// <summary>
        /// method to start the thread corresponding to the platform instance
        /// </summary>
        internal void Start()
        {
            platformThread.Start(this.outPort);
        }

        /// <summary>
        /// execute the platform instance
        /// do forever
        ///     pick a destination
        ///     pick rumors and pack them into messages
        ///     gossip the messages to chosen destination
        /// done
        /// </summary>
        /// <param name="port"></param>
        public void Execute(object _port)
        {
            string name;
            GroupObjectGraph groupObjectGraph;
            GroupOverlapGraph groupOverlapGraph;
            RumorBuffer rumorBuffer;
            UDPGossipTransport gossipTransport;
            NodeAdaptor nodeAdaptor;
            Queue<IGossip> incomingRumorQueue;
            AutoResetEvent feedEventClientSide;
            AutoResetEvent feedEventServiceSide;
            List<Rumor> fedRumor;
            RumorList digest = new RumorList();
            IGraphElement destination = null;
            Node currentNode = null;
            ServiceContext sc = InstanceContainer.GetContext(_port.ToString());
            Rumor rumor = null;
            RumorList message = new RumorList();
            Group commonGroup;
            int MAX_RUMORBUFFER_SIZE = 5;

            debuggingContext.Port = this.outPort;

            sc.GetAdaptors(out  nodeAdaptor);
            sc.GetGraphs(out groupObjectGraph, out groupOverlapGraph);
            sc.GetName(out name);
            sc.GetTransport(out gossipTransport);
            sc.GetRumorBuffer(out rumorBuffer);
            sc.GetReceivedRumorQueue(out incomingRumorQueue);
            sc.GetReceiveAutoResetEvent(out receiveAutoResetEvent);
            sc.GetFeedContext(out feedEventClientSide, out feedEventServiceSide, out fedRumor);
            
            groupObjectGraph.SetNodeInfoNotifier(this.nodeInfoEvent);

            groupOverlapGraph.SetDebuggingContext(debuggingContext);
            groupOverlapGraph.WaitOne();
            groupOverlapGraph.Update(groupObjectGraph);
            
            currentNode = nodeAdaptor.GetGraphElementByName(name) as Node;
            platformWorkerThreadHelper = new PlatformWorkerThreadHelper(groupObjectGraph, currentNode, 
                                                                        syncObj, debuggingContext);

            rumorBuffer.SetLocker(syncObj);
            rumorBuffer.SetTimeout(rumorTimeout);
            rumorBuffer.SetPlatformWorkerThreadHelper(platformWorkerThreadHelper);
            
            gossipTransport.StartReceiver();
            gossipTransport.Initialize(receiveAutoResetEvent, platformWorkerThreadHelper, groupObjectGraph);

            // content selector
            GravitationalContentSelector gselector = new GravitationalContentSelector
                                                    (ref rumorBuffer, ref groupObjectGraph, ref groupOverlapGraph,
                                                     ref platformWorkerThreadHelper, ref debuggingContext);

            RandomContentSelector rcselector = new RandomContentSelector(ref rumorBuffer, ref platformWorkerThreadHelper);
            ContentSelectorContext contentContext = null;
            if (ConfigurationManager.AppSettings["CONTENT_SELECTION"].Equals("ALGO", StringComparison.CurrentCultureIgnoreCase))
            {
                contentContext = new ContentSelectorContext(gselector);
            }
            else if (ConfigurationManager.AppSettings["CONTENT_SELECTION"].Equals("RANDOM", StringComparison.CurrentCultureIgnoreCase))
            {
                contentContext = new ContentSelectorContext(rcselector);
            }
            else
            {
                Trace.WriteLine("Invalid CONTENT_SELECTION algorithm selected, exiting");
                return;
            }

            String maxRumorBufferSize = ConfigurationManager.AppSettings["MAX_RUMORBUFFER_SIZE"];
            if (maxRumorBufferSize != null)
            {
                MAX_RUMORBUFFER_SIZE = Int32.Parse(maxRumorBufferSize);
            }
            

            // recepient selector
            RandomRecepientSelector rSelector = new RandomRecepientSelector(ref name, ref groupObjectGraph, ref groupOverlapGraph,
                                                                            ref nodeAdaptor);
            RecepientSelectorContext recepientContext = new RecepientSelectorContext(rSelector);
            rSelector.SetWorkingGraph();

            int count = rumorBuffer.GetCount();
            int round = 0;

            try
            {
                while (true)
                {
                    lock (rumorBuffer)
                    {
                        round++;
                        //if (round > this.roundsToGossip)
                        //{
                        //    //Trace.WriteLine(debuggingContext + "Platform finished " + (round - 1) + " rounds of gossip, exiting");
                        //    //return;
                        //}

                        (goConnection as GOConnection).AppCallback("feed");

                        count = rumorBuffer.GetCount();

                        if (count > 0)
                        {

                            //Trace.WriteLine(debuggingContext + "Count: " + count);

                            destination = recepientContext.Select();
                            commonGroup = groupObjectGraph.SelectCmnGroup(currentNode, destination);

                            if (commonGroup != null)
                            {
                                //Trace.WriteLine(debuggingContext
                                //                + "common group between " + currentNode.GetID()
                                //                + " and " + destination.GetID()
                                //                + " is " + commonGroup.GetID());
                                message.SetMaxSize(commonGroup.MaxMessageSizePerInterval);
                            }

                            message.SetSource(currentNode);

                            if (rumor == null || platformWorkerThreadHelper.IsSent(destination, rumor))
                            {
                                rumor = contentContext.Select(destination) as Rumor;
                            }

                            while ( !  platformWorkerThreadHelper.IsSent(destination, rumor) 
                                    && message.TryAdd(rumor) == true)
                            {
                                platformWorkerThreadHelper.Record(rumor, destination);

                               

                                rumor = contentContext.Select(destination) as Rumor;
                                
                                if (rumor == null) break;
                            }

                            //foreach (PriorityQueueItem<IGossip, DateTime> r in rumorBuffer.GetAll())
                            //{
                            //    Trace.WriteLine(debuggingContext + "Buffer contains: " + r.Value.Id);
                            //}

                            //foreach (Rumor r in message.GetAll())
                            //{
                            //    Trace.WriteLine(debuggingContext + "Transmitting now...[" + r.Id + "] to "
                            //                        + (destination as Node).Ip + ":" + (destination as Node).Port);
                            //}

                            // let the message go asynchronously, the refence is passed to asycn delegate
                            if (message != null && message.rumorList != null && message.rumorList.Count > 0)
                            {
                                gossipTransport.Send(message,
                                                    (destination as Node).Ip,
                                                    (destination as Node).Port);
                            }

                            while (count >= MAX_RUMORBUFFER_SIZE)
                            {
                                bool retVal = rumorBuffer.DiscardOldest(destination);
                                if (retVal == false)
                                    break;
                            }

                            Thread.Sleep(this.gossipInterval);
                            message = new RumorList();
                        }
                        else
                        {
                            Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                                debuggingContext
                                                + "Rumorbuffer is emtpy, waiting for RumorDispatchEvent");

                            rumorBuffer.WaitForRumorDispatchEvent();

                            Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                                debuggingContext
                                                + "RumorDispatchEvent is true, continuing");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("PlatformWorkerThread::Execute: Exception " + e.ToString() + e.StackTrace);
                Console.WriteLine("PlatformWorkerThread::Execute: Exception " + e.ToString() + e.StackTrace);
                throw new UseCaseException("900", "PlatformWorkerThread::Execute " + e + ", " + e.StackTrace);
            }
        }

        #endregion
    }
}
