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
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_e_.Experiments_
{
    /// <summary>
    /// Measure multicast throughput for raw sockets.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments("-nnodes:0 -stayon:yes -count:50000 -size:1472 -concurrency:1 -sendertoo:no")]
    public class Experiment_250 : Experiment_200
    {
        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            logger.Log(this, "Waiting for the system to stabilize.");

            sleeper.sleep(5);

            logger.Log(this, "Starting to multicast.");

            Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { });
            sendingCompleted.WaitOne();

            logger.Log(this, "Multicasting completed, cooling down now...");

            sleeper.sleep(5);

            logger.Log(this, "Collecting results of the experiment.");

            double[] sendTimes = (double[])Coordinator.invoke(typeof(Application).GetMethod("GetSendTimes"), new object[] { });
            results["SendTimes"] = new QS._core_e_.Data.DataSeries(sendTimes);

            double[] sendCompletionTimes = (double[])Coordinator.invoke(typeof(Application).GetMethod("GetSendCompletionTimes"), new object[] { });
            results["SendCompletionTimes"] = new QS._core_e_.Data.DataSeries(sendCompletionTimes);

            double[] sendSynchronous = (double[])Coordinator.invoke(typeof(Application).GetMethod("GetSendSynchronous"), new object[] { });
            results["SendSynchronous"] = new QS._core_e_.Data.DataSeries(sendSynchronous);

            QS._core_c_.Components.AttributeSet receiveTimesCollection = new QS._core_c_.Components.AttributeSet(); 
            QS._core_c_.Components.AttributeSet nonzeroReceiveTimesCollection = new QS._core_c_.Components.AttributeSet(); 
            QS._core_c_.Components.AttributeSet lossRateCollection = new QS._core_c_.Components.AttributeSet();

            for (int ind1 = 0; ind1 < this.NumberOfApplications; ind1++)
            {
                Runtime_.IApplicationRef application = this.ApplicationOf(ind1);
                double[] receiveTimes = (double[])application.invoke(typeof(Application).GetMethod("GetReceiveTimes"), new object[] { });
                if (receiveTimes != null)
                {
                    string nodeName = ind1.ToString() + " (" + this.Node(ind1).NICs[0].ToString() + ")";
                    receiveTimesCollection[nodeName] = new QS._core_e_.Data.DataSeries(receiveTimes);
                    int nonzeroCount = 0;
                    int ind2;
                    for (ind2 = 0; ind2 < receiveTimes.Length; ind2++)
                    {
                        if (receiveTimes[ind2] != 0)
                            nonzeroCount++;
                    }
                    double[] nonzeroReceiveTimes = new double[nonzeroCount];
                    nonzeroCount = 0;
                    for (ind2 = 0; ind2 < receiveTimes.Length; ind2++)
                    {
                        if (receiveTimes[ind2] != 0)
                            nonzeroReceiveTimes[nonzeroCount++] = receiveTimes[ind2];
                    }
                    nonzeroReceiveTimesCollection[nodeName] = new QS._core_e_.Data.DataSeries(nonzeroReceiveTimes);
                    lossRateCollection[nodeName] = ((double)(receiveTimes.Length - nonzeroCount)) / ((double)receiveTimes.Length);
                }
            }
            results["ReceiveTimes"] = receiveTimesCollection;
            results["Nonzero ReceiveTimes"] = nonzeroReceiveTimesCollection;
            results["Loss Rates"] = lossRateCollection;

/*
            logger.Log(this, "Results collected, now waiting 20 seconds before releasing memory");

            sleeper.sleep(20);

            logger.Log(this, "Release all of the preallocated structures.");

            for (int ind1 = 0; ind1 < this.NumberOfApplications; ind1++)
            {
                Runtime.IApplicationRef application = this.ApplicationOf(ind1);
                application.invoke(typeof(Application).GetMethod("ReleaseMemory"), new object[] { });
            }
*/

            logger.Log(this, "Completed.");
        }

        #endregion

        #region Callbacks

        private ManualResetEvent sendingCompleted = new ManualResetEvent(false);
        public void SendingCompleted(QS._core_c_.Components.AttributeSet arguments)
        {
            sendingCompleted.Set();
        }

        #endregion

        #region Class Application

        protected new class Application : Experiment_200.Application
        {
            private const string GroupAddressString = "224.12.34.56:12000";

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                groupAddress = new QS.Fx.Network.NetworkAddress(GroupAddressString);
                messageCount = Convert.ToInt32(args["count"]);
                messageSize = Convert.ToInt32(args["size"]);
                unsafe
                {
                    if (messageSize < sizeof(int))
                        throw new ArgumentException("Message size too small.");
                }
                sendConcurrency = Convert.ToInt32(args["concurrency"]);

                if (isCoordinator)
                {
                    senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint endpoint = new IPEndPoint(localAddress.HostIPAddress, 0);
                    senderSocket.Bind(endpoint);
                    senderSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                    senderSocket.Connect(groupAddress.HostIPAddress, groupAddress.PortNumber);
                }

                receiving = (!isCoordinator || args.contains("sendertoo") && args["sendertoo"].Equals("yes"));
                if (receiving)
                {
                    logger.Log(this, "Receiving");

                    receivingTimes = new double[messageCount];
                    receiverSocket = QS._qss_c_.Devices_2_.UDPReceiver.createSocket(localAddress.HostIPAddress, ref groupAddress);

                    receivingBuffer = new byte[messageSize];
                    goodSenderRemote = new IPEndPoint(IPAddress.Any, 0);

                    receivingCallback = new AsyncCallback(this.ReceiveCallback);

                    senderRemote = (EndPoint)(goodSenderRemote);
                    receiverSocket.BeginReceiveFrom(
                        receivingBuffer, 0, receivingBuffer.Length, SocketFlags.None, ref senderRemote, receivingCallback, null);
                }

                logger.Log(this, "Ready");
            }

            #endregion

            private QS.Fx.Network.NetworkAddress groupAddress;
            private bool receiving;
            private Socket senderSocket, receiverSocket;
            private int sentCount = 0, receivedCount = 0;
            private int messageCount, messageSize, sendConcurrency, sendCallbacksCalled;
            private double[] initiateSendTimes, sendCallbackTimes, receivingTimes, sendSynchronous;
            private byte[] receivingBuffer;
            private IPEndPoint goodSenderRemote;
            private EndPoint senderRemote;
            private AsyncCallback sendingCallback, receivingCallback;
            private List<ArraySegment<byte>> buffersToSend;
            private byte[][] byteArrays;
            private List<ArraySegment<byte>>[] segments;

            #region Callbacks

            private void SendCallback(IAsyncResult result)
            {
                try
                {
                    if (!result.CompletedSynchronously)
                    {
                        int callback_no = Interlocked.Increment(ref sendCallbacksCalled) - 1;
                        if (callback_no < messageCount)
                            sendCallbackTimes[callback_no] = platform.Clock.Time;

                        while (true)
                        {
                            senderSocket.EndSend(result);
                            int message_no = Interlocked.Increment(ref sentCount) - 1;
                            if (message_no < messageCount)
                            {
                                initiateSendTimes[message_no] = platform.Clock.Time;
                                result = senderSocket.BeginSend(segments[message_no], SocketFlags.None, sendingCallback, null);
                                if (result.CompletedSynchronously)
                                {
                                    sendSynchronous[message_no] = 1;
                                    callback_no = Interlocked.Increment(ref sendCallbacksCalled) - 1;
                                    if (callback_no < messageCount)
                                        sendCallbackTimes[callback_no] = platform.Clock.Time;
                                }
                                else
                                {
                                    sendSynchronous[message_no] = 0;
                                    break;
                                }
                            }
                            else
                            {
                                applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
                                break;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    logger.Log(this, "__SendCallback : " + exc.ToString());
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                receiverSocket.EndReceiveFrom(result, ref senderRemote);
                double time = platform.Clock.Time;
                int messageSeqNo;
                unsafe
                {
                    fixed (byte* pbuffer = receivingBuffer)
                    {
                        messageSeqNo = *((int*)pbuffer);
                    }
                }
                if (messageSeqNo < messageCount)
                {
                    receivingTimes[messageSeqNo] = time;
                    Interlocked.Increment(ref receivedCount);
                    if (receivedCount < messageCount)
                    {
                        senderRemote = (EndPoint)(goodSenderRemote);
                        receiverSocket.BeginReceiveFrom(
                            receivingBuffer, 0, receivingBuffer.Length, SocketFlags.None, ref senderRemote, receivingCallback, null);
                    }
                    else
                    {
                        logger.Log(this, "Received all messages.");
                    }
                }
            }

            #endregion

            #region Sending

            public void Send()
            {
                byte[] dataToSend = new byte[messageSize];
                buffersToSend = new List<ArraySegment<byte>>();
                buffersToSend.Add(new ArraySegment<byte>(dataToSend));

                initiateSendTimes = new double[messageCount];
                sendCallbackTimes = new double[messageCount];
                sendSynchronous = new double[messageCount];

                byteArrays = new byte[messageCount][]; 
                segments = new List<ArraySegment<byte>>[messageCount];
                int ind;
                unsafe
                {
                    for (ind = 0; ind < messageCount; ind++)
                    {
                        byte[] buffer = new byte[messageSize];
                        byteArrays[ind] = buffer;
                        fixed (byte* pbuffer = buffer)
                        {
                            *((int*)pbuffer) = ind;
                        }
                        segments[ind] = new List<ArraySegment<byte>>();
                        segments[ind].Add(new ArraySegment<byte>(buffer));
                    }
                }
                
                sendingCallback = new AsyncCallback(this.SendCallback);

                int concurrentSends = 0;
                while (concurrentSends < sendConcurrency)
                {
                    int message_no = Interlocked.Increment(ref sentCount) - 1;
                    if (message_no < messageCount)
                    {
                        initiateSendTimes[message_no] = platform.Clock.Time;
                        IAsyncResult asyncResult =
                            senderSocket.BeginSend(segments[message_no], SocketFlags.None, sendingCallback, null);
                        if (asyncResult.CompletedSynchronously)
                        {
                            sendSynchronous[message_no] = 1;
                            int callback_no = Interlocked.Increment(ref sendCallbacksCalled) - 1;
                            if (callback_no < messageCount)
                                sendCallbackTimes[callback_no] = platform.Clock.Time;

                            senderSocket.EndSend(asyncResult);
                        }
                        else
                        {
                            sendSynchronous[message_no] = 0;
                            concurrentSends++;
                        }
                    }
                    else
                    {
                        applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
                        break;
                    }
                }
            }

            #endregion

            #region Uploading Statistics

            public double[] GetSendTimes()
            {
                logger.Log(this, "Uploading send times");
                return initiateSendTimes;
            }

            public double[] GetSendCompletionTimes()
            {
                logger.Log(this, "Uploading send completion times");
                return sendCallbackTimes;
            }

            public double[] GetSendSynchronous()
            {
                logger.Log(this, "Uploading send synchronous flags");
                return sendSynchronous;
            }

            public double[] GetReceiveTimes()
            {
                logger.Log(this, "Uploading receive times");
                return receivingTimes;
            }

            #endregion

            #region Releasing Resources

            public void ReleaseMemory()
            {
                initiateSendTimes = null;
                sendCallbackTimes = null;
                sendSynchronous = null;
                receivingTimes = null;
                receivingBuffer = null;
                buffersToSend = null;
                byteArrays = null;
                segments = null;
                GC.Collect();
            }

            #endregion

            #region Terminating and Disposing

            public override void TerminateApplication(bool smoothly)
            {
                platform.ReleaseResources();
            }

            public override void Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region Other Garbage

        public Experiment_250()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
