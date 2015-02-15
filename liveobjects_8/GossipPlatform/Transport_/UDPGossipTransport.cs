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
using System.Net;
using System.Net.Sockets;
using GOTransport.Common;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using GOTransport.Core;
using System.Runtime.Remoting.Messaging;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;

namespace GOTransport.GossipTransport
{
    /// <summary>
    /// UDP oriented propagation mechanism for gossip objects
    /// </summary>
    public class UDPGossipTransport : IGossipTransport
    {
        #region private fields

        private UdpClient udpSender;
        private UdpClient udpReceiver;
        private string receivePort;
        private object localLock;
        private AddIncomingRumorsDelegate addIncomingRumorsDelegate;
        private AutoResetEvent receiveAutoResetEvent;
        private IPEndPoint epSend;
        private IPEndPoint epReceive;
        private Byte[] bytesToSend;
        private NodeAdaptor nodeAdaptor;
        private PlatformWorkerThreadHelper platformWorkerThreadHelper;
        private GroupObjectGraph groupObjectGraph;
        private RumorBuffer rumorBuffer;
        private Queue<IGossip> receivedRumorQueue;

        #endregion

        #region public fields

        public delegate void SendDigestAsyncDelegate(RumorList _digest, String _ip, String _port);
        public static SendDigestAsyncDelegate sendDigestAsyncDelegate;
        public delegate void ReceiveDigestAsyncDelegate();
        public delegate void AddIncomingRumorsDelegate(byte[] _message);
        public ReceiveDigestAsyncDelegate receiveDigestAsyncDelegate;

        public string ReceivePort { get { return receivePort; } }

        #endregion

        #region constructor

        public UDPGossipTransport(string _port, RumorBuffer _rumorBuffer, Queue<IGossip> _receivedRumorQueue, NodeAdaptor _nodeAdaptor)
        {
            try
            {
                receivePort = _port;

                udpSender = new UdpClient();
                udpReceiver = new UdpClient(Int32.Parse(_port));

                epSend = new IPEndPoint(IPAddress.Loopback, 0);
                epReceive = new IPEndPoint(IPAddress.Any, 0);

                addIncomingRumorsDelegate = new AddIncomingRumorsDelegate(this.AddIncomingRumors);
                rumorBuffer = _rumorBuffer;
                receivedRumorQueue = _receivedRumorQueue;
                nodeAdaptor = _nodeAdaptor;
                sendDigestAsyncDelegate = new SendDigestAsyncDelegate(this.Send);
                receiveDigestAsyncDelegate = new ReceiveDigestAsyncDelegate(this.StartReceiver);
                localLock = new object();
            }
            catch (Exception e)
            {
                throw new UseCaseException("480", "UDPGossipTransport::UDPGossipTransport " + e + ", " + e.StackTrace);
            }
        }

        #endregion

        #region IGossipTransport Members

        /// <summary>
        /// Send the RumorDigest to a node
        /// </summary>
        /// <param name="rumor">rumor digest to be sent</param>
        public void Send(RumorList _digest, String _dstIp, String _dstPort)
        {
            try
            {
                epSend.Address = IPAddress.Parse(_dstIp);
                epSend.Port = Int32.Parse(_dstPort);
                bytesToSend = Utils.SerializeDigestBinary(_digest as RumorList);
                udpSender.BeginSend(bytesToSend, bytesToSend.Length, epSend, new AsyncCallback(SendCallback), null);
            }
            catch (Exception e)
            {
                throw new UseCaseException("500", "UDPGossipTransport::Send " + e + ", " + e.StackTrace);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            // nothing to do
        }

        /// <summary>
        /// Receive a RumorDigest object from other nodes
        /// </summary>
        /// <returns>IGossip object that was received</returns>
        public void StartReceiver()
        {
            try
            {
                udpReceiver.BeginReceive(ReceiveCallback, (epReceive));
            }
            catch (Exception e)
            {
                throw new UseCaseException("520", "UDPGossipTransport::StartReceiver " + e + ", " + e.StackTrace);
            }
        }

        public void Initialize(AutoResetEvent ae, PlatformWorkerThreadHelper _platformWorkerThreadHelper, GroupObjectGraph _groupObjectGraph)
        {
            this.receiveAutoResetEvent = ae;
            platformWorkerThreadHelper = _platformWorkerThreadHelper;
            groupObjectGraph = _groupObjectGraph;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Free the resources
        /// </summary>
        public void Close()
        {
            udpSender.Close();
            udpReceiver.Close();
        }

        public void SetReceiveAutoResetEvent(AutoResetEvent ae)
        {
            this.receiveAutoResetEvent = ae;
        }

        public void SetPlatformWorkerThreadHelper(PlatformWorkerThreadHelper _platformWorkerThreadHelper)
        {
            platformWorkerThreadHelper = _platformWorkerThreadHelper;
        }

        public void SetGroupObjectGraph(GroupObjectGraph _groupObjectGraph)
        {
            groupObjectGraph = _groupObjectGraph;
        }

        #endregion

        #region private methods

        private void AddIncomingRumors(byte[] _message)
        {
            try
            {
                byte[] logMessage = Utils.StringToByteArray("Adding incoming rumors");
                RumorList incoming = Utils.DeserializeDigestBinary(_message) as RumorList;

                lock (localLock)
                {
                    foreach (Rumor r in incoming.GetAll())
                    {
                        r.HopCount -= 1;

                        DebuggingContext debuggingContext = new DebuggingContext();
                        debuggingContext.Port = "" + this.receivePort;
                        
                        String localIp = Utils.GetLocalIPAddress();
                        bool flag = false;
                        foreach (IGossip __r in receivedRumorQueue.ToArray())
                        {
                            if (__r.Id == r.Id)
                            {
                                flag = true;
                            }
                        }

                        if (flag == false)
                        {
                            Trace.WriteLine("\r\n" + debuggingContext
                                            + "\t" + r.Id
                                            + "\t" + r.SourceNode.Ip
                                            + "\t" + Utils.GetLocalIPAddress()
                                            + "\t" + r.Age);
                        }

                        // create a clone for putting into the incoming queue
                        Rumor rumorCopy = new Rumor(r.Id, r.PayLoad);
                        rumorCopy.SourceNode = new Node(r.SourceNode.Id);
                        rumorCopy.SourceGroup = new Group(r.SourceGroup.Id,
                            (nodeAdaptor.GetGroupElementByName(r.SourceGroup.Id) as Group).MaxMessageSizePerInterval);
                        rumorCopy.DestinationGroup = new Group(r.DestinationGroup.Id,
                            (nodeAdaptor.GetGroupElementByName(r.DestinationGroup.Id) as Group).MaxMessageSizePerInterval);
                        rumorCopy.DestinationNode = new Node(r.DestinationNode.Id);
                        rumorCopy.HopCount = r.HopCount;

                        platformWorkerThreadHelper.Record(r, nodeAdaptor.GetGraphElementByName(r.SourceNode.Id));

                        if (r.HopCount >= 0 && ! rumorBuffer.HasRumor(r))
                        //if (r.HopCount >= 0)
                        {
                            r.DestinationGroup = nodeAdaptor.GetGroupElementByName(r.DestinationGroup.GetID()) as Group;

                            // add to the pool of rumors in rumorBuffer
                            rumorBuffer.Add(r);
                            rumorBuffer.SetRumorDispatchEvent();

                            // add to the list of received rumors
                            receivedRumorQueue.Enqueue(rumorCopy);
                            receiveAutoResetEvent.Set();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("540", "UDPGossipTransport::AddIncomingRumors " + e + ", " + e.StackTrace);
            }
        }   

        /// <summary>
        /// callback for handling received messages
        /// </summary>
        /// <param name="result">result of asynchronous operation</param>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint ipep = (_result.AsyncState) as IPEndPoint;

                byte[] data = udpReceiver.EndReceive(_result, ref ipep);
                addIncomingRumorsDelegate.BeginInvoke(data, null, null);
                udpReceiver.BeginReceive(ReceiveCallback, _result.AsyncState);
            }
            catch (Exception e)
            {
                throw new UseCaseException("560", "UDPGossipTransport::ReceiveCallback " + e + ", " + e.StackTrace);
            }
        }

        #endregion
    }
}
