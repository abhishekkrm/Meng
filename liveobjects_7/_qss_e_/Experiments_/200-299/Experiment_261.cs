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

namespace QS._qss_e_.Experiments_
{
    /// <summary>
    /// Measuring throughput vs. loss rate for sender/receiver pair in Base3/Devices3.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments(
        "-nnodes:0 -count:20000 -size:1000 -rate:1000 -window:100 " +
        "-download:yes -save:yes -stabilize:5 -cooldown:10 -stayon:yes")]
    public class Experiment_261 : Experiment_200
    {
        private const string DefaultRepositoryPath = "C:\\.QuickSilver\\.Repository";

        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            logger.Log(this, "Waiting for the system to stabilize.");

            sleeper.sleep(Convert.ToDouble((string)arguments["stabilize"]));

            logger.Log(this, "Starting to multicast.");

            Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { });
            sendingCompleted.WaitOne();

            logger.Log(this, "Multicasting completed, cooling down now...");

            sleeper.sleep(Convert.ToDouble((string)arguments["cooldown"]));

            if (arguments.contains("download") && (arguments["download"].Equals("on") || arguments["download"].Equals("yes")))
            {
                logger.Log(this, "Downloading application statistics...");

                QS._core_c_.Components.AttributeSet applicationStatistics = new QS._core_c_.Components.AttributeSet();
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                {
                    logger.Log(this, "Downloading from " + application.AppID.ToString());
                    object obj = application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { true });
                    if (obj != null && obj is QS._core_c_.Components.Attribute)
                        applicationStatistics.Add((QS._core_c_.Components.Attribute)obj);
                }

                results["Application Statistics"] = new QS._core_c_.Components.Attribute("Application Statistics", applicationStatistics);
            }
            else
            {
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                    application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { false });
            }

            if (arguments.contains("save") && (arguments["save"].Equals("yes") || arguments["save"].Equals("on")))
                QS._qss_e_.Experiment_.Helpers.SaveResults.Save(DefaultRepositoryPath, logger, this, arguments, results);

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
            private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
            private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(platform.Logger, platform.EventLogger);
                ((QS._qss_c_.Base3_.IDemultiplexer)demultiplexer).register(
                    myloid, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));

                root3 = new QS._qss_c_.Base3_.RootSender(platform.EventLogger, platform.Logger,
                    platform.Network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP],
                    localAddress.PortNumber, demultiplexer, platform.Clock, false);
                ((QS._qss_c_.Devices_3_.IMembershipController)root3).Join(groupAddress);

                size = Convert.ToInt32(args["size"]);
                if (size < sizeof(uint))
                    throw new Exception("Message size too small.");
                nmessages = Convert.ToInt32(args["count"]);
                window = Convert.ToInt32(args["window"]);

                receiveTimes = new double[nmessages];

                if (isCoordinator)
                {
                    sendTimes = new double[nmessages];
                    transmitTimes = new double[nmessages];

                    bufferingUNS = new QS._qss_c_.Senders9.BufferingUNS(platform.Logger, root3.SenderCollection,
                        QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass, platform.AlarmClock, platform.Clock);
                    if (args.contains("rate"))
                    {
                        bufferingUNS.FlowControlEnabledByDefault = true;
                        bufferingUNS.DefaultRate = Convert.ToDouble((string)args["rate"]);
                    }
                    else
                    {
                        bufferingUNS.FlowControlEnabledByDefault = false;
                        bufferingUNS.DefaultRate = 0.00000000001;
                    }
                    bufferingUNS.BatchingEnabledByDefault = false;                    
                    sender = ((QS._qss_c_.Base3_.ISenderCollection<QS.Fx.Network.NetworkAddress, 
                        QS._qss_c_.Base3_.IReliableSerializableSender>)bufferingUNS)[groupAddress];
                }

                logger.Log(this, "Ready");
            }

            #endregion

            [QS._core_c_.Diagnostics.Component]
            private QS._qss_c_.Base3_.Demultiplexer demultiplexer;
            [QS._core_c_.Diagnostics.Component]
            private QS._qss_c_.Base3_.RootSender root3;
            [QS._core_c_.Diagnostics.Component]
            private QS._qss_c_.Senders9.BufferingUNS bufferingUNS;
            [QS._core_c_.Diagnostics.Component]
            private QS._qss_c_.Base3_.IReliableSerializableSender sender;
            private int size, nmessages, nsent, window;

            [QS.Fx.Inspection.Ignore]
            [QS._core_c_.Diagnostics.Ignore]
            private double[] sendTimes, transmitTimes, receiveTimes;

            [QS._core_c_.Diagnostics.Component("Send Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet SendTimes
            {
                get { return new QS._core_e_.Data.DataSeries(sendTimes); }
            }

            [QS._core_c_.Diagnostics.Component("Transmit Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet TransmitTimes
            {
                get { return new QS._core_e_.Data.DataSeries(transmitTimes); }
            }

            [QS._core_c_.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet ReceiveTimes
            {
                get { return new QS._core_e_.Data.DataSeries(receiveTimes); }
            }

            [QS.Fx.Base.Inspectable("Collected Statistics", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private QS._core_c_.Components.Attribute collectedStatistics;

            #region Callback

            private void CompletionCallback(QS._qss_c_.Base3_.IAsynchronousOperation argument)
            {
                transmitTimes[(int)argument.AsyncState] = platform.Clock.Time;
                SendOne();                
            }

            #endregion

            #region Sending

            public void Send()
            {
                int burstsize = window;
                if (nmessages < burstsize)
                    burstsize = nmessages;

                while (burstsize-- > 0 && SendOne())
                    ;
            }

            private bool SendOne()
            {
                int seqno = Interlocked.Increment(ref nsent);
                if (seqno < nmessages)
                {
                    sendTimes[seqno] = platform.Clock.Time;
                    sender.BeginSend((uint)myloid, new QS._qss_c_.Base3_.SequenceNo((uint)seqno),
                        new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.CompletionCallback), seqno);

                    return true;
                }
                else
                {
                    applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
                    return false;
                }
            }

            #endregion

            #region ReceiveCallback

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                receiveTimes[(int)((QS._qss_c_.Base3_.SequenceNo)receivedObject).Value] = platform.Clock.Time;

                return null;
            }

            #endregion

            #region GetStatistics

            public QS._core_c_.Components.Attribute GetStatistics(bool upload)
            {
                collectedStatistics =
                    new QS._core_c_.Components.Attribute(localAddress.ToString(), QS._qss_c_.Diagnostics_1_.DataCollector.Collect(this));

                return upload ? collectedStatistics : new QS._core_c_.Components.Attribute("foo", "bar");
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

        public Experiment_261()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
