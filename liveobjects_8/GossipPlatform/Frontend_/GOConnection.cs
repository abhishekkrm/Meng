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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Ipc;
using System.Collections.Generic;
using System.Text;
using GOTransport.Core;
using GOTransport.Common;
using System.Diagnostics;
using System.Threading;
using GOTransport.GossipTransport;
using System.Security.Permissions;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Frontend
{
    /// <summary>
    /// Provides all the services that Gossip platform is to offer (a facade)
    /// </summary>
    public class GOConnection : MarshalByRefObject, AbstractServer, IGOConnection
    {
        #region private fields

        private static Dictionary<string, ServiceContext> serviceContextMap
                 = new Dictionary<string, ServiceContext>();

        SubscriptionHandler subscriptionHandler = new SubscriptionHandler();

        private GroupObjectGraph groupObjectGraph;
        private GroupOverlapGraph groupOverlapGraph;
        private RumorBuffer rumorBuffer;
        private NodeAdaptor nodeAdaptor;
        private UDPGossipTransport udpGossipTransport;
        private Queue<IGossip> receivedRumorQueue;
        private AutoResetEvent receiveCallback;
        private string name;
        private Queue<IGossip> interestedIncomingQueue;
        private AutoResetEvent interestedIncomingEvent;
        private RumorBuffer interestedRumorBuffer;
        private AutoResetEvent feedEventClientSide;
        private AutoResetEvent feedEventServiceSide;
        private List<Rumor> fedRumor;
        private ServiceContext serviceContext;
        private Dictionary<string, string> loConfig;
        private static Boolean isReady = false;
        private static Boolean hasGraphData = false;

        #endregion


        #region constructors

        /// <summary>
        /// when a GOConnection is created, start the actual platform instances
        /// </summary>
        public GOConnection()
        {

        }

        #endregion

        #region public methods

        public event RumorFeedEventHandler AppCallbackHandler;
        public delegate string myfuncDelegate(string what);

        public string AppCallback(string _message)
        {
            FireNewBroadcastedMessageEvent("Event: " + _message + " was said");
            return "done";
        }


        public event RumorFeedEventHandler AppCallbackEvent
        {
            add
            {
                Console.WriteLine("in event myevent + add");

                AppCallbackHandler = value;
            }

            remove
            {
                Console.WriteLine("in event myevent + remove");
            }
        }

        protected void FireNewBroadcastedMessageEvent(string text)
        {
            AppCallbackHandler("hai");
        }

        public void SetHasGraphData(Boolean _flag)
        {
            hasGraphData = _flag;
        }

        public Boolean HasGraphData()
        {
            return hasGraphData;
        }

        /// <summary>
        /// server would activate the object after the first method call
        /// this method is used to get the first (and only) instance 'activated'
        /// </summary>
        public void Touch(Dictionary<string, string> _loConfig)
        {
            this.loConfig = _loConfig;
            this.subscriptionHandler.SetConfig(_loConfig);
            this.StartPlatformInstances();
        }

        public void SetWorkingContext(string _port)
        {
            InstanceContainer.SetWorkingContext(_port,
                    out groupObjectGraph,
                    out groupOverlapGraph,
                    out udpGossipTransport,
                    out name,
                    out rumorBuffer,
                    out nodeAdaptor,
                    out receivedRumorQueue,
                    out receiveCallback,
                    out feedEventClientSide,
                    out feedEventServiceSide,
                    out fedRumor);

            serviceContext = InstanceContainer.GetContext(_port);
            serviceContext.GetReceiveAutoResetEvent(out interestedIncomingEvent);
            serviceContext.GetReceivedRumorQueue(out interestedIncomingQueue);
        }

        /// <summary>
        /// start a new platform instance, one per platform definition, as defined in the configuration file
        /// </summary>
        public void StartPlatformInstances()
        {
            try
            {
                PlatformWorkerThread worker = null;
                PlatformConfigReader config = new PlatformConfigReader(loConfig["OUT_PORTS"],
                                                                        loConfig["GOSSIP_INTERVALS"],
                                                                        loConfig["RUMOR_TIMEOUTS"],
                                                                        loConfig["NUMBER_OF_ROUNDS_TO_GOSSIP"]);

                if (config.HasDefinitions() == true)
                {
                    foreach (InstanceContainer definition in config.GetDefinitions())
                    {
                        worker = new PlatformWorkerThread(definition.OutPort,
                                                          definition.GossipInterval,
                                                          definition.RumorTimeout,
                                                          definition.RoundsToGossip,
                                                          this);

                        InstanceContainer.CreateContext(definition.OutPort, loConfig);
                        worker.Start();
                        
                        String message = "\r\n" + DateTime.Now
                                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                    + "Started platform on [" + definition.OutPort + "]"
                                    + ", Gossip Interval: " + definition.GossipInterval
                                    + ", Rumor Timeout: " + definition.RumorTimeout;

                        //Utils.outLogFile.Write(Utils.StringToByteArray(message), 0, Utils.StringToByteArray(message).Length);
                        //Utils.outLogFile.Flush();
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    message);
                    }
                }
                else
                {
                    Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    "\r\n" + DateTime.Now
                                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                                    + "GossipService: Could not initialize, No definitions");
                }

                isReady = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + ", " + e.StackTrace);
                Thread.Sleep(50000);
            }
        }

        public bool Subscribe(ISubscription _subscription, RumorFeedEventHandler _handler)
        {
            if (subscriptionHandler.Subscribe(_subscription) == true)
            {
                AppCallbackEvent += _handler;
                return true;
            }

            return false;
        }

        public void Unsubscribe(ISubscription _subscription)
        {
            subscriptionHandler.Unsubscribe(_subscription);
        }

        /// <summary>
        /// add node to a group
        /// </summary>
        /// <param name="group">group to which the node has to be added</param>
        /// <param name="_node">node to add</param>
        public void AddNode(Group _group, Node _node)
        {
            // Create new copy of group
            Group group = _group.Clone() as Group;

            // Create new copy of node
            Node node = _node.Clone() as Node;
            this.nodeAdaptor.AddNode(group, node);
        }

        /// <summary>
        /// add a list of nodes to a group
        /// </summary>
        /// <param name="group">group to which the node has to be added</param>
        /// <param name="nodeList">list of nodes to be added</param>
        public void AddNodeList(IGraphElement _group, NodeList _nodeList)
        {
            this.nodeAdaptor.AddNodeList(_group, _nodeList);
        }

        /// <summary>
        /// remove node from a group
        /// </summary>
        /// <param name="group">group from which the node has to be removed from</param>
        /// <param name="node">node to be removed</param>
        public void RemoveNode(IGraphElement _group, IGraphElement _node)
        {
            this.nodeAdaptor.RemoveNode(_group, _node);
        }

        /// <summary>
        /// remove a list of nodes from a group
        /// </summary>
        /// <param name="group">group from which the node has to be removed from</param>
        /// <param name="nodeList">list of nodes to be removed</param>
        public void RemoveNodeList(IGraphElement _group, NodeList _nodeList)
        {
            this.nodeAdaptor.RemoveNodeList(_group, _nodeList);
        }

        /// <summary>
        /// send a gossip message
        /// </summary>
        /// <param name="rumorID">unique identifier for the rumor</param>
        /// <param name="payload">actual message that has to be gossiped</param>
        /// <param name="destNode">destination node</param>
        /// <param name="destGroup">destination group</param>
        /// <param name="hopCount">hop count for the rumor</param>
        public void Send(Rumor _rumor)
        {
            Rumor rumor = _rumor.Clone() as Rumor;
            Console.WriteLine("Platform received rumor: " + rumor.Id + " with payload: " + rumor.PayLoad);

            if (rumor.DestinationGroup == null || rumor.SourceGroup == null
                || rumor.DestinationNode == null || rumor.SourceNode == null)
            {
                throw new UseCaseException("", "one of the following parameters for the rumor [" + rumor.Id + "] is not set: "
                                + "rumor.DestinationGroup: " + rumor.DestinationGroup
                                + ", rumor.SourceGroup: " + rumor.SourceGroup
                                + ", rumor.DestinationNode: " + rumor.DestinationNode
                                + ", rumor.SourceNode: " + rumor.SourceNode);
            }

            // destination node and groups should point to the instances on the server end
            rumor.DestinationNode = nodeAdaptor.GetGraphElementByName(_rumor.DestinationNode.Id) as Node;
            rumor.DestinationGroup = nodeAdaptor.GetGroupElementByName(_rumor.DestinationGroup.Id) as Group;

            rumor.SourceGroup = nodeAdaptor.GetGroupElementByName(_rumor.SourceGroup.Id) as Group;
            rumor.SourceNode = nodeAdaptor.GetGraphElementByName(_rumor.SourceNode.Id) as Node;

            rumorBuffer.Add(rumor);
        }

        public IGossip Receive()
        {
            //return rumorBuffer.GetAll();
            if (receivedRumorQueue == null)
            {
                Debug.WriteLineIf(Utils.debugSwitch.Verbose, "receivedRumorQueue is null, exiting");
                return null;
            }

            Debug.WriteLineIf(Utils.debugSwitch.Verbose, "checking receivedRumorQueue: " + receivedRumorQueue.GetHashCode());
            return this.receivedRumorQueue == null || this.receivedRumorQueue.Count == 0
                                    ? null
                                    : this.receivedRumorQueue.Dequeue();
        }

        public Boolean IsReady()
        {
            return isReady;
        }

        public void Clear()
        {
            InstanceContainer.Clear();
            this.StartPlatformInstances();
        }

        /// <summary>
        /// connect two nodes by establishing an edge between them
        /// </summary>
        /// <param name="node1">node to connect</param>
        /// <param name="node2">node to connect</param>
        /// <param name="cost">cost of edge between the connected nodes</param>
        public void Connect(Node _node1, Node _node2, double _cost)
        {
            this.nodeAdaptor.Connect(_node1, _node2, _cost);
        }

        /// <summary>
        /// disconnect two nodes by removing the edge between them
        /// </summary>
        /// <param name="node1">node to disconnect</param>
        /// <param name="node2">node to disconnect</param>
        public void DisConnect(Node _node1, Node _node2)
        {
            this.nodeAdaptor.DisConnect(_node1, _node2);
        }


        /// <summary>
        /// indicate that initialization is complete
        /// </summary>
        public void InitializationComplete()
        {
            this.nodeAdaptor.InitializationComplete();
        }

        public void SetReceiveContext(String _context,
                                    out AutoResetEvent _ae,
                                    out AutoResetEvent _fec,
                                    out AutoResetEvent _fes,
                                    out List<Rumor> _fedRumor)
        {
            ServiceContext sc = InstanceContainer.GetContext(_context);
            AutoResetEvent _event;

            sc.GetReceivedRumorQueue(out interestedIncomingQueue);
            sc.GetReceiveAutoResetEvent(out _event);
            sc.GetFeedContext(out _fec, out _fes, out _fedRumor);

            _ae = _event;
            interestedIncomingEvent = _event;
            sc.GetRumorBuffer(out interestedRumorBuffer);


            feedEventClientSide = _fec;
            feedEventServiceSide = _fes;
            fedRumor = _fedRumor;
        }

        public Rumor BeginToFetch()
        {
            if (interestedIncomingQueue != null && interestedIncomingQueue.Count > 0)
            {
                return interestedIncomingQueue.Dequeue() as Rumor;
            }
            else
            {
                //Console.WriteLine("Waiting for incoming rumors.............");
                //interestedIncomingEvent.WaitOne();
            }

            return null;
        }

        public void SetFedRumor(ref List<Rumor> rumor)
        {
            fedRumor = rumor;
            serviceContext.SetFedRumor(ref fedRumor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Object InitializeLifetimeService()
        {
            return null;
        }

        public void SetReceiveEvent(AutoResetEvent _ae)
        {
            udpGossipTransport.SetReceiveAutoResetEvent(_ae);
        }

        #endregion

        #region IGOConnection Members


        public void Close(string port)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
