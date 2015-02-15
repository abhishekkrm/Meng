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
    /// Measure multicast throughput for raw devices in QS.CMS.Devices3.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments("-nnodes:0 -stayon:yes -count:1000 -size:100 -batchsize:10 -interval:0.001 -sendertoo:no")]
    public class Experiment_251 : Experiment_200
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

        protected new class Application : Experiment_200.Application, QS._qss_c_.Devices_3_.IReceiver
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
                interval = Convert.ToDouble(args["interval"]);
                batchSize = Convert.ToInt32(args["batchsize"]);

                QS._qss_c_.Devices_3_.ICommunicationsDevice device =
                    platform.Network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];

                if (isCoordinator)
                {
                    sender = device.GetSender(groupAddress);
                }

                receiving = (!isCoordinator || args.contains("sendertoo") && args["sendertoo"].Equals("yes"));
                if (receiving)
                {
                    logger.Log(this, "Receiving");

                    receivingTimes = new double[messageCount];

                    listener = device.ListenAt(groupAddress, this);
                }

                logger.Log(this, "Ready");
            }

            #endregion            

            private QS.Fx.Network.NetworkAddress groupAddress;
            private int messageCount, messageSize, batchSize;
            private double interval;
            private QS._qss_c_.Devices_3_.ISender sender;
            private QS._qss_c_.Devices_3_.IListener listener;
            private bool receiving;
            private int sentCount = 0, receivedCount = 0;
            private double[] sendingTimes, receivingTimes;

            private List<QS.Fx.Base.Block> buffersToSend;
            private byte[][] byteArrays;
            private List<QS.Fx.Base.Block>[] segments;

            #region IReceiver Members

            void QS._qss_c_.Devices_3_.IReceiver.receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
            {
                double time = platform.Clock.Time;
                int messageSeqNo;
                unsafe
                {
                    fixed (byte* pbuffer = data.Array)
                    {
                        messageSeqNo = *((int*) (pbuffer + data.Offset));
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
            }

            #endregion

            #region Sending

            public void Send()
            {
                byte[] dataToSend = new byte[messageSize];
                buffersToSend = new List<QS.Fx.Base.Block>();
                buffersToSend.Add(new QS.Fx.Base.Block(dataToSend));

                sendingTimes = new double[messageCount];
                byteArrays = new byte[messageCount][];
                segments = new List<QS.Fx.Base.Block>[messageCount];
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
                        segments[ind] = new List<QS.Fx.Base.Block>();
                        segments[ind].Add(new QS.Fx.Base.Block(buffer));
                    }
                }

                for (ind = 0; ind < batchSize; ind++)
                {
                    int message_no = Interlocked.Increment(ref sentCount) - 1;
                    if (message_no < messageCount)
                    {
                        sendingTimes[message_no] = platform.Clock.Time;
                        sender.send(segments[message_no]);
                    }
                }
                platform.AlarmClock.Schedule(interval, new QS.Fx.Clock.AlarmCallback(this.SendCallback), null);
            }

            #endregion

            #region SendCallback

            private void SendCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                for (int ind = 0; ind < batchSize; ind++)
                {
                    int message_no = Interlocked.Increment(ref sentCount) - 1;
                    if (message_no < messageCount)
                    {
                        sendingTimes[message_no] = platform.Clock.Time;
                        sender.send(segments[message_no]);
                    }
                    else
                    {
                        applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
                        return;
                    }
                }
                alarmRef.Reschedule();
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

        public Experiment_251()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
