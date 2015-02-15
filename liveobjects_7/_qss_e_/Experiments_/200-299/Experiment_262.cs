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
// #define DEBUG_TrackDisturbances

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

namespace QS._qss_e_.Experiments_
{
    // Unreliable sender on core that uses core sender directly.

    // [TMS.Experiments.DefaultExperiment]
/*
  <argument name="nnodes"                   value="0"               />
  <argument name="count"                    value="50000"           />
  <argument name="size"                     value="1000"            />
  <argument name="rc_rate"                  value="5000"            />
  <argument name="rc_type"                  value="3"               />
  <argument name="rc_window_size"           value="100"             />
  <argument name="rc_burst_size"            value="20"              />
  <argument name="rc_burst_rate"            value="100000"          />
  <argument name="sender_concurrency"       value="100"             />
  <argument name="sender_too"               value="no"              />
  <argument name="verbose"                  value="no"              />
  <argument name="download"                 value="no"              />
  <argument name="save"                     value="no"              />
  <argument name="stabilize"                value="2"               />
  <argument name="cooldown"                 value="5"               />
  <argument name="stayon"                   value="yes"             />
  <argument name="gui"                      value="yes"             />
*/
    public class Experiment_262 : Experiment_200
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
                logger.Log(this, "About to save statistics.");

                Queue<IAsyncResult> waitHandles = new Queue<IAsyncResult>();
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                {
                    logger.Log(this, "[" + (Interlocked.Increment(ref nsaving) + 1).ToString() + "] saving statistics at " + application.Address);
                    waitHandles.Enqueue(
                        application.BeginInvoke(
                            typeof(Application).GetMethod("GetStatistics"), new object[] { false }, new AsyncCallback(
                                delegate(IAsyncResult result)
                                {
                                    logger.Log(this, "[" + (Interlocked.Increment(ref nsaved) + 1).ToString() + "] statistics saved at " + ((QS._qss_e_.Runtime_.IApplicationRef)result.AsyncState).Address);
                                }), application));
                }

                while (waitHandles.Count > 0)
                    waitHandles.Dequeue().AsyncWaitHandle.WaitOne();

                logger.Log(this, "All statistics saved.");
            }

            if (arguments.contains("save") && (arguments["save"].Equals("yes") || arguments["save"].Equals("on")))
                QS._qss_e_.Experiment_.Helpers.SaveResults.Save(DefaultRepositoryPath, logger, this, arguments, results);

            logger.Log(this, "Completed.");
        }

        #endregion

        private int nsaving, nsaved;

        #region Callbacks

        private ManualResetEvent sendingCompleted = new ManualResetEvent(false);
        public void SendingCompleted(QS._core_c_.Components.AttributeSet arguments)
        {
            sendingCompleted.Set();
        }

        #endregion

        #region Class Application

        protected new class Application : Experiment_200.Application, QS.Fx.Network.ISource, QS._qss_e_.Runtime_.IControlledApp
        {
            private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
            private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "gui"))
                    AppController.Show("Experiment 262 App Controller", this);

                core = new QS._core_c_.Core.Core("C:\\.QuickSilver");
                core.OnError += new QS._core_c_.Core.ErrorCallback(this.ErrorCallback);

                performanceLog = new QS._qss_c_.Diagnostics_3_.PerformanceLog(core, core, 0);
                // performanceLog.AddCounter("Processor", "_Total", "% Processor Time");

                if (args.contains("comaxcc"))
                    core.MaximumConcurrency = Convert.ToInt32((string)args["comaxcc"]);
                
                if (args.contains("comintx"))
                    core.MinimumTransmitted = Convert.ToInt32((string)args["comintx"]);

                if (args.contains("comaxtx"))
                    core.MaximumTransmitted = Convert.ToInt32((string)args["comaxtx"]);

                if (args.contains("alarm_quantum"))
                    core.MaximumQuantumForAlarms = Convert.ToDouble((string)args["alarm_quantum"]);

                if (args.contains("io_quantum"))
                    core.MaximumQuantumForCompletionPorts = Convert.ToDouble((string)args["io_quantum"]);

                if (args.contains("default_unicast_rate"))
                    core.DefaultMaximumSenderUnicastRate = Convert.ToDouble((string)args["default_unicast_rate"]);

                if (args.contains("default_multicast_rate"))
                    core.DefaultMaximumSenderMulticastRate = Convert.ToDouble((string)args["default_multicast_rate"]);

                if (args.contains("fc_unicast_credits"))
                    core.DefaultMaximumSenderUnicastCredits = Convert.ToDouble((string)args["fc_unicast_credits"]);

                if (args.contains("fc_multicast_credits"))
                    core.DefaultMaximumSenderMulticastCredits = Convert.ToDouble((string)args["fc_multicast_credits"]);

                if (args.contains("sender_cc"))
                    core.DefaultMaximumSenderConcurrency = Convert.ToInt32((string)args["sender_cc"]);

                core.ContinueIOOnTimeWarps = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "timewarps_continueio");

                messagesize = Convert.ToInt32(args["size"]);
                if (messagesize < sizeof(uint))
                    throw new Exception("Message size too small.");
                nmessages = Convert.ToInt32(args["count"]);
                isVerbose = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "verbose");

                address = new QS._core_c_.Core.Address(
                    localAddress.HostIPAddress, groupAddress.HostIPAddress, groupAddress.PortNumber);

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "sender_too") || !isCoordinator)
                {
                    receiveTimes = new double[nmessages];
                    // for (int ind = 0; ind < nmessages; ind++)
                    //    receiveTimes[ind] = double.NaN;

                    bool drain = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "drain_socket");
                    int num_buffers = args.contains("num_buffers") ? Convert.ToInt32((string)args["num_buffers"]) : 1;
                    int adf_bufsize = args.contains("adf_bufsize") ? Convert.ToInt32((string)args["adf_bufsize"]) : 1048576;
                    int max_bufsize = args.contains("max_bufsize") ? Convert.ToInt32((string)args["max_bufsize"]) : 65535;

                    listener = ((QS._core_c_.Core.ICore)core).Listen(
                        address, new QS.Fx.Network.ReceiveCallback(this.ReceiveCallback), null, max_bufsize, num_buffers, drain, adf_bufsize, true);
                }

                if (isCoordinator)
                {
                    sender = ((QS._core_c_.Core.ICore) core).GetSender(address, 0, 0, true);

                    if (args.contains("sender_concurrency"))
                        sender.MaximumConcurrency = Convert.ToInt32((string)args["sender_concurrency"]);                    

                    int rateControllerType = 0;
                    if (args.contains("rc_type"))
                        rateControllerType = Convert.ToInt32((string)args["rc_type"]);
                    
                    switch (rateControllerType)
                    {
                        case 0:
                        case 1:
                            {
                                // sender.RateController = new QS.CMS.RateControl.RateController1(core, core.Clock 

                                if (args.contains("snmaxcr") && sender.RateController is QS._core_c_.RateControl.RateController1)
                                {
                                    ((QS._core_c_.RateControl.RateController1)sender.RateController).MaximumCredits =
                                        Convert.ToInt32((string)args["snmaxcr"]);
                                }

                                if (args.contains("snlowcr") && sender.RateController is QS._core_c_.RateControl.RateController1)
                                {
                                    ((QS._core_c_.RateControl.RateController1)sender.RateController).LowCredits =
                                        Convert.ToInt32((string)args["snlowcr"]);
                                }

                                if (args.contains("snhighcr") && sender.RateController is QS._core_c_.RateControl.RateController1)
                                {
                                    ((QS._core_c_.RateControl.RateController1)sender.RateController).HighCredits =
                                        Convert.ToInt32((string)args["snhighcr"]);
                                }

                                if (args.contains("sncrrmc") && sender.RateController is QS._core_c_.RateControl.RateController1)
                                {
                                    ((QS._core_c_.RateControl.RateController1)sender.RateController).RecoveryMaxCount =
                                        Convert.ToInt32((string)args["sncrrmc"]);
                                }
                            }
                            break;

                        case 2:
                            {
                                QS._qss_c_.RateControl_1_.RateController2 rateController = 
                                    new QS._qss_c_.RateControl_1_.RateController2(core.PhysicalClock, core, platform.Logger, 100000, core.StatisticsController);
                                sender.RateController = rateController;

                                if (args.contains("rc_window_size"))
                                    rateController.WindowSize = Convert.ToInt32((string)args["rc_window_size"]);
                            }
                            break;

                        case 3:
                            {
                                QS._qss_c_.RateControl_1_.RateController3 rateController =
                                    new QS._qss_c_.RateControl_1_.RateController3(core.PhysicalClock, core, platform.Logger, 100000, core.StatisticsController);
                                sender.RateController = rateController;

                                if (args.contains("rc_window_size"))
                                    rateController.WindowSize = Convert.ToInt32((string)args["rc_window_size"]);
                                if (args.contains("rc_burst_size"))
                                    rateController.BurstSize = Convert.ToInt32((string)args["rc_burst_size"]);
                                if (args.contains("rc_burst_rate"))
                                    rateController.BurstRate = Convert.ToDouble((string)args["rc_burst_rate"]);
                            }
                            break;

                        default:
                            throw new Exception("Unknown rate controller type.");
                    }

                    sendTimes = new double[nmessages];

                    if (args.contains("rate"))
                        rate = Convert.ToDouble((string)args["rate"]);
                    else
                        rate = double.PositiveInfinity;

                    sender.RateController.MaximumRate = rate;
                }

                if (isCoordinator)
                {
                    if (args.contains("alarm_rate_sender"))
                        alarm_rate = Convert.ToDouble((string)args["alarm_rate_sender"]);
                    else
                        alarm_rate = 0;
                }
                else
                {
                    if (args.contains("alarm_rate_receiver"))
                        alarm_rate = Convert.ToDouble((string)args["alarm_rate_receiver"]);
                    else
                        alarm_rate = 0;
                }

                logger.Log(this, "Disturbing alarm rate on this node is " + alarm_rate.ToString());

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

                core.Start();

                if (alarm_rate > 0)
                {
                    logger.Log(this, "Scheduling disturbances at the rate of " + alarm_rate.ToString() + " per second.");
                    ((QS.Fx.Scheduling.IScheduler) core).Execute(new AsyncCallback(
                        delegate(IAsyncResult result)
                        {
                            disturbingAlarm = ((QS.Fx.Clock.IAlarmClock)core).Schedule(
                                (1 / alarm_rate), new QS.Fx.Clock.AlarmCallback(this.DisturbingAlarmCallback), null);
                        }), null);
                }
                else
                    logger.Log(this, "Transmission undisturbed.");

                logger.Log(this, "Ready");
            }

            #endregion

            #region Fields and Properties

            [QS._core_c_.Diagnostics.Component("Core")]
            [QS._core_c_.Diagnostics2.Module("Core")]
            private QS._core_c_.Core.Core core;

            private QS._core_c_.Core.Address address;
            private QS._core_c_.Core.ISender sender;
            private QS._core_c_.Core.IListener listener;
            [QS.Fx.Inspection.Ignore]
            [QS._core_c_.Diagnostics.Ignore]
            private double[] sendTimes, receiveTimes;
            private double rate, alarm_rate;
            private int messagesize, nmessages, nsent, nreceived;
            private bool isVerbose;
            private QS.Fx.Clock.IAlarm disturbingAlarm;

            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            [QS._core_c_.Diagnostics.Component("Performance Log")]
            [QS._core_c_.Diagnostics2.Module("PerformanceLog")]
            private QS._qss_c_.Diagnostics_3_.PerformanceLog performanceLog;

#if DEBUG_EnableStatistics
            [QS.CMS.Diagnostics.Component("Disturbance Times (X = time)")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("DisturbanceTimes")]
            private QS.CMS.Statistics.Samples disturbanceTimes = new QS.CMS.Statistics.Samples();
#endif

            private double last_disturbance = -1;
            private double disturbance_correction = 0;

            [QS.Fx.Base.Inspectable("Collected Statistics", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private QS._core_c_.Components.AttributeSet collected_statistics;

            [QS._core_c_.Diagnostics.Component("Send Times (X = seqno, Y = time)")]
            [QS._core_c_.Diagnostics2.Property("SendTimes")]
            public QS._core_e_.Data.IDataSet SendTimes
            {
                get { return new QS._core_e_.Data.DataSeries(sendTimes); }
            }

            [QS._core_c_.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
            [QS._core_c_.Diagnostics2.Property("ReceiveTimes")]
            public QS._core_e_.Data.IDataSet ReceiveTimes
            {
                get { return new QS._core_e_.Data.DataSeries(receiveTimes); }
            }

            #endregion

            #region DisturbingAlarmCallback

            private void DisturbingAlarmCallback(QS.Fx.Clock.IAlarm alarm)
            {
                double time_now = core.PhysicalClock.Time;
#if DEBUG_TrackDisturbances
                disturbanceTimes.addSample(time_now);
#endif
                if (last_disturbance >= 0)
                    disturbance_correction += (time_now - last_disturbance - 1 / alarm_rate);
                last_disturbance = time_now; 

                double next_delay = (1 / alarm_rate) - disturbance_correction;
                alarm.Reschedule(next_delay > 0 ? next_delay : 0.000000001);
            }

            #endregion

            #region Error Callback

            private void ErrorCallback(string description)
            {
                platform.Logger.Log(this, "Core: " + description);
            }

            #endregion

            #region Sending

            public void Send()
            {
                ((QS.Fx.Scheduling.IScheduler)core).Execute(new AsyncCallback(delegate(IAsyncResult result) { sender.Send(this); }), null);
            }

            #endregion

            #region ISource Members

            bool  QS.Fx.Network.ISource.Get(int maximumSize, out QS.Fx.Network.Data data, 
                out QS.Fx.Base.ContextCallback callback, out object context, out bool moreAvailable)
            {
 	            int seqno = nsent++;
                moreAvailable = nsent < nmessages;

                byte[] message = new byte[messagesize];
                List<QS.Fx.Base.Block> segments = new List<QS.Fx.Base.Block>(1);
                segments.Add(new QS.Fx.Base.Block(message));
                unsafe 
                {
                    fixed (byte* pmessage = message)
                    {
                        *((int*)pmessage) = seqno;
                    }
                }

                data = new QS.Fx.Network.Data(segments, messagesize);
                callback = null;
                context = null;

                sendTimes[seqno] = core.PhysicalClock.Time;

                if (!moreAvailable)                   
                    applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);

                if (isVerbose)
                    platform.Logger.Log(this, "Sending: " + seqno.ToString());

                return true;
            }

            #endregion

            #region ReceiveCallback

            private void ReceiveCallback(IPAddress ipAddress, int portNo, QS.Fx.Base.Block data, object context)
            {
                int seqno;
                unsafe
                {
                    fixed (byte* pbuffer = data.buffer)
                    {
                        seqno = *((int*)(pbuffer + (int) data.offset));
                    }
                }

                receiveTimes[seqno] = core.Time;
                nreceived++;
            }

            #endregion

            #region GetStatistics

            public QS._core_c_.Components.Attribute GetStatistics(bool upload)
            {
                logger.Log(this, "_____GetStatistics : Enter");

                logger.Log(this, "_____GetStatistics : Collecting all statistics");

                // collected_statistics = QS.CMS.Diagnostics.DataCollector.Collect(this);
                collected_statistics = QS._core_c_.Diagnostics2.Helper.Collect(diagnosticsContainer);

                logger.Log(this, "_____GetStatistics : Dumping to hard drive");

                QS._qss_e_.Experiment_.Helpers.DumpResults.Dump(logger, collected_statistics, experimentPath, "Incarnation_" + incarnation.ToString());

                logger.Log(this, "_____GetStatistics : Preparing return value");

                QS._core_c_.Components.Attribute returned_statistics =
                    upload ? (new QS._core_c_.Components.Attribute(localAddress.ToString(), collected_statistics))
                        : (new QS._core_c_.Components.Attribute("foo", "bar"));

                logger.Log(this, "_____GetStatistics : Leave");

                return returned_statistics;
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

            #region IControlledApp Members

            bool QS._qss_e_.Runtime_.IControlledApp.Running
            {
                get { return core != null && core.Running; }
            }

            void QS._qss_e_.Runtime_.IControlledApp.Start()
            {
                if (core != null)
                    core.Start();
            }

            void QS._qss_e_.Runtime_.IControlledApp.Stop()
            {
                if (core != null)
                    core.Stop();
            }

            #endregion
        }

        #endregion

        #region Other Garbage

        public Experiment_262()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
