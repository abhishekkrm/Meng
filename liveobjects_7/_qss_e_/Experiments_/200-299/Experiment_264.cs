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

// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

namespace QS._qss_e_.Experiments_
{
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments(
        " -nnodes:0 -count:10000 -size:1000 -rate:1000 " +
        " -fc_credits:100 -sender_cc:100 -alarm_quantum:0.05 -io_quantum:0.1 " +
        " -stabilize:5 -cooldown:10 -stayon:yes -gui:yes ")]
    public class Experiment_264 : Experiment_200
    {
        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            logger.Log(this, "Waiting for the system to stabilize.");

            sleeper.sleep(Convert.ToDouble((string)arguments["stabilize"]));

            string destinationAddress = (string)ApplicationOf(1).invoke(typeof(Application).GetMethod("GetAddress"), new object[] { });
            logger.Log(this, "Starting to send to { " + destinationAddress + " }");

            Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { destinationAddress });
            
            sendingCompleted.WaitOne();

            logger.Log(this, "Sending completed, cooling down now...");

            sleeper.sleep(Convert.ToDouble((string)arguments["cooldown"]));

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

        protected new class Application : Experiment_200.Application, QS._qss_e_.Runtime_.IControlledApp
        {
            private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                string rootpath = repository_root + "\\" + repository_key;
                core = new QS._core_c_.Core.Core(rootpath + "\\coreworkdir");
                string fsroot = rootpath + "\\filesystem";

                framework = new QS._qss_c_.Framework_1_.FrameworkOnCore(
                    new QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress, platform.Logger, 
                    platform.EventLogger, core, fsroot, false, 0, false);

                if (args.contains("alarm_quantum"))
                    core.MaximumQuantumForAlarms = Convert.ToDouble((string)args["alarm_quantum"]);
                if (args.contains("io_quantum"))
                    core.MaximumQuantumForCompletionPorts = Convert.ToDouble((string)args["io_quantum"]);
                if (args.contains("fc_unicast_credits"))
                    core.DefaultMaximumSenderUnicastCredits = Convert.ToDouble((string)args["fc_unicast_credits"]);
                if (args.contains("fc_multicast_credits"))
                    core.DefaultMaximumSenderMulticastCredits = Convert.ToDouble((string)args["fc_multicast_credits"]);
                if (args.contains("sender_cc"))
                    core.DefaultMaximumSenderConcurrency = Convert.ToInt32((string)args["sender_cc"]);
                double rate;
                if (args.contains("rate"))
                    rate = Convert.ToDouble((string)args["rate"]);
                else
                    rate = double.PositiveInfinity;
                core.DefaultMaximumSenderUnicastRate = rate;
                core.DefaultMaximumSenderMulticastRate = rate;

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "gui"))
                    AppController.Show("Experiment 263 App Controller", this);
                framework.Demultiplexer.register(myloid, new QS._qss_c_.Base3_.ReceiveCallback(ReceiveCallback));
                messagesize = Convert.ToInt32(args["size"]);
                if (messagesize < sizeof(uint))
                    throw new Exception("Message size too small.");
                nmessages = Convert.ToInt32(args["count"]);

                receiveTimes = new double[nmessages];

                if (isCoordinator)
                {
                    sendTimes = new double[nmessages];
                    completionTimes = new double[nmessages];
                }

                core.Start();

                logger.Log(this, "Ready");
            }

            #endregion

            private QS._qss_c_.Framework_1_.FrameworkOnCore framework;

            [QS._core_c_.Diagnostics.Component("Core")]
            [QS._core_c_.Diagnostics2.Module("Core")]
            private QS._core_c_.Core.Core core;

            private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sendingSink;

            [QS._core_c_.Diagnostics.Ignore]
            [QS.Fx.Inspection.Ignore]
            private double[] sendTimes, completionTimes, receiveTimes;
            private int messagesize, nmessages, ncompleted, nreceived, nsent;

            #region Stats

            [QS._core_c_.Diagnostics.Component("Send Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet SendTimes
            {
                get { return new QS._core_e_.Data.DataSeries(sendTimes); }
            }

            [QS._core_c_.Diagnostics.Component("Completion Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet CompletionTimes
            {
                get { return new QS._core_e_.Data.DataSeries(completionTimes); }
            }

            [QS._core_c_.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet ReceiveTimes
            {
                get { return new QS._core_e_.Data.DataSeries(receiveTimes); }
            }

            #endregion

            #region GetAddress

            public string GetAddress()
            {
                return ((QS.Fx.Serialization.IStringSerializable)framework.LocalAddress).AsString;
            }

            #endregion

            #region Sending

            public void Send(string addressAsString)
            {
                QS._core_c_.Base3.InstanceID destinationAddress = new QS._core_c_.Base3.InstanceID();
                ((QS.Fx.Serialization.IStringSerializable)destinationAddress).AsString = addressAsString;

                framework.Platform.Scheduler.Execute(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        sendingSink = ((QS._qss_c_.Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, 
                            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>) 
                                framework.ReliableInstanceSinks)[destinationAddress];
                        sendingSink.Send(
                            new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetCallback));
                    }), null);
            }

            public void GetCallback(
                Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue,
                int maximumNumberOfObjects,
#if UseEnhancedRateControl
                int maximumNumberOfBytes, 
#endif
                out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
                out int numberOfBytesReturned,
#endif
                out bool moreObjectsAvailable)
            {
                // TODO: Implement enhanced rate control

                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                numberOfBytesReturned = 0;
#endif
                moreObjectsAvailable = true;
                while (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                {
                    if (nsent < nmessages)
                    {
                        int seqno = nsent++;
                        sendTimes[seqno] = framework.Clock.Time;

                        objectQueue.Enqueue(new QS._qss_c_.Base6_.AsynchronousMessage(
                            new QS._core_c_.Base3.Message(myloid, new QS._qss_c_.Base3_.SequenceNo((uint)seqno)),
                            new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback)));
                        numberOfObjectsReturned++;
                        // numberOfBytesReturned += ........................................................HERE
                    }
                    else
                    {
                        moreObjectsAvailable = false;
                        break;
                    }
                }
            }

            private void CompletionCallback(bool succeeded, Exception exception, object context)
            {
                QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request =
                    (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>)context;
                QS._qss_c_.Base3_.SequenceNo sequenceNo = (QS._qss_c_.Base3_.SequenceNo)request.Argument.transmittedObject;
                int seqno = (int)sequenceNo.Value;

                completionTimes[seqno] = framework.Clock.Time;
                ncompleted++;
                if (ncompleted >= nmessages)
                    applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
            }

            #endregion

            #region ReceiveCallback

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                QS._core_c_.Base3.InstanceID senderAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                QS._qss_c_.Base3_.SequenceNo sequenceNo = (QS._qss_c_.Base3_.SequenceNo)receivedObject;
                int seqno = (int)sequenceNo.Value;

                receiveTimes[seqno] = framework.Clock.Time;
                nreceived++;

                return null;
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
                get { return core.Running; }
            }

            void QS._qss_e_.Runtime_.IControlledApp.Start()
            {
                core.Start();
            }

            void QS._qss_e_.Runtime_.IControlledApp.Stop()
            {
                core.Stop();
            }

            #endregion
        }

        #endregion

        #region Other Garbage

        public Experiment_264()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
