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

// #define DEBUG_EnableStatistics

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_e_.Experiments_
{
    /// <summary>
    /// Three hosts: (a) is sending, (b) receiving and occasionally senging ACKs to (c).
    /// We want to reproduce the receiver resource depletion phenomena.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments("-nnodes:0 -stayon:yes -stabilize:20 -cooldown:10 -count:1000 -size:100")]
    public class Experiment_255 : Experiment_200
    {
        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            for (int ind = 0; ind < NumberOfApplications; ind++)
                ApplicationOf(ind).invoke(typeof(Application).GetMethod("Prepare"), 
                    new object[] { (ind == 1), Node(1).NICs[0].ToString() });

            double stabilize_time = arguments.contains("stabilize") ? Convert.ToDouble((string)arguments["stabilize"]) : 5;
            logger.Log(this, "Waiting " + stabilize_time.ToString() + " seconds for the system to stabilize.");
            sleeper.sleep(stabilize_time);

            logger.Log(this, "Starting to multicast.");

            Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { });
            sendingCompleted.WaitOne();

            if (arguments.contains("cooldown"))
            {
                double cooldown_time = Convert.ToDouble((string)arguments["cooldown"]);
                logger.Log(this, "Cooling down for " + cooldown_time.ToString() + " seconds....");
                sleeper.sleep(cooldown_time);
            }

/*
            logger.Log(this, "Collecting results of the experiment.");

            double[] sendTimes = (double[])Coordinator.invoke(typeof(Application).GetMethod("GetSendTimes"), new object[] { });
            results["SendTimes"] = new QS.TMS.Data.DataSeries(sendTimes);

            QS.CMS.Components.AttributeSet receiveTimesCollection = new QS.CMS.Components.AttributeSet();
            QS.CMS.Components.AttributeSet nonzeroReceiveTimesCollection = new QS.CMS.Components.AttributeSet();
            QS.CMS.Components.AttributeSet lossRateCollection = new QS.CMS.Components.AttributeSet();

            for (int ind1 = 0; ind1 < this.NumberOfApplications; ind1++)
            {
                Runtime.IApplicationRef application = this.ApplicationOf(ind1);
                double[] receiveTimes = (double[])application.invoke(typeof(Application).GetMethod("GetReceiveTimes"), new object[] { });
                if (receiveTimes != null)
                {
                    string nodeName = ind1.ToString() + " (" + this.Node(ind1).NICs[0].ToString() + ")";
                    receiveTimesCollection[nodeName] = new QS.TMS.Data.DataSeries(receiveTimes);
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
                    nonzeroReceiveTimesCollection[nodeName] = new QS.TMS.Data.DataSeries(nonzeroReceiveTimes);
                    lossRateCollection[nodeName] = ((double)(receiveTimes.Length - nonzeroCount)) / ((double)receiveTimes.Length);
                }
            }
            results["ReceiveTimes"] = receiveTimesCollection;
            results["Nonzero ReceiveTimes"] = nonzeroReceiveTimesCollection;
            results["Loss Rates"] = lossRateCollection;
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
            private const uint MyLOID = (uint)ReservedObjectID.User_Min;
            private static readonly QS._qss_c_.Base3_.GroupID groupID = new QS._qss_c_.Base3_.GroupID();

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                messageCount = Convert.ToInt32(args["count"]);
                messageSize = Convert.ToInt32(args["size"]);
                unsafe
                {
                    if (messageSize < sizeof(int))
                        throw new ArgumentException("Message size too small.");
                }

                framework = new QS._qss_c_.Framework_1_.SimpleFramework(incarnation, platform, localAddress, coordinatorAddress);
                framework.Demultiplexer.register(MyLOID, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));

                if (isCoordinator)
                {
/*
                    groupSink = ((CMS.Base4.ISinkCollection<CMS.Base3.GroupID, CMS.Base4.Asynchronous<CMS.Base3.Message>>)
                        framework.NewDelegatingGS)[groupID];
*/ 
                }

                logger.Log(this, "Ready");
            }

            #endregion

            #region Prepare

            public void Prepare(bool isCollector, string collectorAddress)
            {
                if (isCollector)
                    logger.Log(this, "Acting as a collector.");
                {
                    if (!isCoordinator)
                        logger.Log(this, "Collector at " + collectorAddress.ToString());

                    framework.MembershipAgent.ChangeMembership(
                        QS._qss_c_.Helpers_.ListOf<QS._qss_c_.Base3_.GroupID>.Singleton(groupID), QS._qss_c_.Helpers_.ListOf<QS._qss_c_.Base3_.GroupID>.Nothing);
                }


            }

            #endregion

            private int messageCount, messageSize;
            private QS._qss_c_.Framework_1_.SimpleFramework framework;

/*
            private CMS.Base4.IAddressedSink<CMS.Base3.GroupID, CMS.Base4.Asynchronous<CMS.Base3.Message>> groupSink;
            private int sentCount = 0, receivedCount = 0;
            private double[] sendingTimes, receivingTimes;
            private QS.CMS.Base2.BlockOfData[] blocks;
            private QS.CMS.Base4.IChannel outgoingChannel;
*/
            #region Receive Callback

#if DEBUG_EnableStatistics
            [QS.CMS.Diagnostics.Component]
            private QS.CMS.Statistics.Samples receiveTimes = new QS.CMS.Statistics.Samples();
#endif

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
            {
                lock (this)
                {
                }
/*
                double time = platform.Clock.Time;
                CMS.Base2.BlockOfData block = (CMS.Base2.BlockOfData)receivedObject;
                int messageSeqNo;
                unsafe
                {
                    fixed (byte* pbuffer = block.Buffer)
                    {
                        messageSeqNo = *((int*)(pbuffer + block.OffsetWithinBuffer));
                    }
                }
                if (messageSeqNo < messageCount)
                {
                    receivingTimes[messageSeqNo] = time;
                    Interlocked.Increment(ref receivedCount);
                    if (receivedCount >= messageCount)
                    {
                        logger.Log(this, "Received all messages.");
                    }
                }
*/

                return null;
            }

            #endregion

            #region Sending

            public void Send()
            {
/*
                sendingTimes = new double[messageCount];
                blocks = new QS.CMS.Base2.BlockOfData[messageCount];
                unsafe
                {
                    for (int ind = 0; ind < messageCount; ind++)
                    {
                        byte[] buffer = new byte[messageSize];
                        fixed (byte* pbuffer = buffer)
                        {
                            *((int*)pbuffer) = ind;
                        }
                        blocks[ind] = new QS.CMS.Base2.BlockOfData(buffer);
                    }
                }

                outgoingChannel = groupSink.Register(
                        new QS.CMS.Base4.GetObjectsCallback<QS.CMS.Base4.Asynchronous<QS.CMS.Base3.Message>>(
                            this.GetObjectsCallback));

                outgoingChannel.Signal();
*/ 
            }

            #endregion

/*
            #region GetObjectsCallback

            private bool GetObjectsCallback(
                ref Queue<QS.CMS.Base4.Asynchronous<QS.CMS.Base3.Message>> returnedObjects, uint maximumSize)
            {
                int message_no = Interlocked.Increment(ref sentCount) - 1;
                if (message_no < messageCount)
                {
                    sendingTimes[message_no] = platform.Clock.Time;
                    returnedObjects.Enqueue(new CMS.Base4.Asynchronous<CMS.Base3.Message>(
                        new CMS.Base3.Message(MyLOID, blocks[message_no]), null, null));
                    return true;
                }
                else
                {
                    applicationController.upcall("SendingCompleted", QS.CMS.Components.AttributeSet.None);
                    return false;
                }
            }

            #endregion

            #region Uploading Statistics

            public double[] GetSendTimes()
            {
                logger.Log(this, "Uploading send times");
                return sendingTimes;
            }

            public double[] GetReceiveTimes()
            {
                logger.Log(this, "Uploading receive times");
                return receivingTimes;
            }

            #endregion
*/

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

        public Experiment_255()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
