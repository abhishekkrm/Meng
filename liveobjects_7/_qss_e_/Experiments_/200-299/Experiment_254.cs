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
    /// Measure multicast throughput for a combination of root sender in QS.CMS.Base4 and a root/demultiplexer in QS.CMS.Base3.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments(
        "-nnodes:0 -stayon:yes -stabilize:20 -cooldown:10 " +
        "-count:10000 -size:100 -concurrency:1")]
    public class Experiment_254 : Experiment_200
    {
        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
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

            logger.Log(this, "Collecting results of the experiment.");

            double[] sendTimes = (double[])Coordinator.invoke(typeof(Application).GetMethod("GetSendTimes"), new object[] { });
            results["SendTimes"] = new QS._core_e_.Data.DataSeries(sendTimes);

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
            private const uint MyLOID = (uint)ReservedObjectID.User_Min;

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                groupAddress = new QS.Fx.Network.NetworkAddress(GroupAddressString);
                messageCount = Convert.ToInt32(args["count"]);
                messageSize = Convert.ToInt32(args["size"]);
                unsafe
                {
                    if (messageSize < sizeof(int))
                        throw new ArgumentException("Message size too small.");
                }
                messageConcurrency = Convert.ToInt32(args["concurrency"]);

                framework = new QS._qss_c_.Framework_1_.SimpleFramework(incarnation, platform, localAddress, coordinatorAddress);

                framework.Demultiplexer.register(MyLOID, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));

                framework.MembershipAgent.ChangeMembership(
                   QS._qss_c_.Helpers_.ListOf<QS._qss_c_.Base3_.GroupID>.Singleton(groupID), QS._qss_c_.Helpers_.ListOf<QS._qss_c_.Base3_.GroupID>.Nothing);
/*
                platform.AlarmClock.scheduleAnAlarm(2000, new QS.CMS.Base.AlarmCallback(delegate(CMS.Base.IAlarmRef alarmRef)
                {
                    framework.MembershipAgent.ChangeMembership(
                        CMS.Helpers.ListOf<CMS.Base3.GroupID>.Singleton(groupID), CMS.Helpers.ListOf<CMS.Base3.GroupID>.Nothing);
                }), null);
*/

                if (isCoordinator)
                {
                    groupSink = ((QS._qss_c_.Base4_.ISinkCollection<QS._qss_c_.Base3_.GroupID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>)
                        framework.NewDelegatingGS)[groupID];
                }

                receivingTimes = new double[messageCount];

                logger.Log(this, "Ready");
            }

            #endregion

            private QS.Fx.Network.NetworkAddress groupAddress;
            private int messageCount, messageSize, messageConcurrency;
            private QS._qss_c_.Framework_1_.SimpleFramework framework;
            private QS._qss_c_.Base3_.GroupID groupID = new QS._qss_c_.Base3_.GroupID(1000);
            private QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.GroupID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> groupSink;
            // private bool receiving;
            private int sentCount = 0, receivedCount = 0;
            private double[] sendingTimes, receivingTimes;
            private QS._core_c_.Base2.BlockOfData[] blocks;
            private QS._qss_c_.Base4_.IChannel outgoingChannel;

            #region Receive Callback

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
            {
                double time = platform.Clock.Time;
                QS._core_c_.Base2.BlockOfData block = (QS._core_c_.Base2.BlockOfData)receivedObject;
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

                return null;
            }

            #endregion

            #region Sending

            public void Send()
            {
                sendingTimes = new double[messageCount];
                blocks = new QS._core_c_.Base2.BlockOfData[messageCount];
                unsafe
                {
                    for (int ind = 0; ind < messageCount; ind++)
                    {
                        byte[] buffer = new byte[messageSize];
                        fixed (byte* pbuffer = buffer)
                        {
                            *((int*)pbuffer) = ind;
                        }
                        blocks[ind] = new QS._core_c_.Base2.BlockOfData(buffer);
                    }
                }

                outgoingChannel = groupSink.Register(
                        new QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>(
                            this.GetObjectsCallback));

                outgoingChannel.Signal();
            }

            #endregion

            #region GetObjectsCallback

            private bool GetObjectsCallback(
                ref Queue<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> returnedObjects, uint maximumSize)
            {
                int message_no = Interlocked.Increment(ref sentCount) - 1;
                if (message_no < messageCount)
                {
                    sendingTimes[message_no] = platform.Clock.Time;
                    returnedObjects.Enqueue(new QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>(
                        new QS._core_c_.Base3.Message(MyLOID, blocks[message_no]), null, null));
                    return true;
                }
                else
                {
                    applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
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

        public Experiment_254()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
