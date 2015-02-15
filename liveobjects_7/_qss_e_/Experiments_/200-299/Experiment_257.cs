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
// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_e_.Experiments_
{
    /// <summary>
    /// Measuring throughput of the Base6.RootSender sender using a "pull" scheme from Base6/Devices6.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments(
        "-nnodes:0 -count:100000 -size:100 -maxsends:1000 -maxbytes:1000000 -maxsockets:100 -prealloc:yes " +
        "-download:no -save:no -stabilize:10 -cooldown:10 -stayon:yes")]
    public class Experiment_257 : Experiment_200
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

        protected new class Application 
            : Experiment_200.Application, QS._qss_c_.Base6_.ISource<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>
        {
            private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
            private static readonly uint loid = ((uint) QS.ReservedObjectID.User_Min) + 100;

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                communicationsDevice =
                    platform.Network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];
                demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(platform.Logger, platform.EventLogger);
                rootSender = new QS._qss_c_.Base3_.RootSender(platform.EventLogger, incarnation, platform.Logger,
                    communicationsDevice, localAddress.PortNumber, demultiplexer, platform.Clock, false);
                ((QS._qss_c_.Devices_3_.IMembershipController)rootSender).Join(groupAddress);

                demultiplexer.register(loid, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));

                if (isCoordinator)
                {
                    senderController = new QS._qss_c_.Devices_6_.SenderController();
                    blockSender = new QS._qss_c_.Devices_6_.Sender(localAddress.HostIPAddress, groupAddress, senderController, 10);
                    sender = new QS._qss_c_.Base6_.SerializingSender(new QS._core_c_.Base3.InstanceID(localAddress, incarnation), platform.Clock, blockSender);

                    if (args.contains("maxsends"))
                        senderController.MaximumSends = Convert.ToInt32(args["maxsends"]);
                    if (args.contains("maxbytes"))
                        senderController.MaximumBytes = Convert.ToInt32(args["maxbytes"]);
                    if (args.contains("maxsockets"))
                        blockSender.MaximumConcurrency = Convert.ToInt32(args["maxsockets"]);

                    size = Convert.ToInt32(args["size"]);
                    if (size < sizeof(uint))
                        throw new Exception("Message size too small.");
                    nmessages = Convert.ToInt32(args["count"]);

                    prealloc = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "prealloc");
                    if (prealloc)
                    {
                        preallocatedObjects = new Request[nmessages];
                        for (int ind = 0; ind < nmessages; ind++)
                            preallocatedObjects[ind] = new Request(this, ind);
                    }

                    channel = QS._qss_c_.Base6_.Channel<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Connect(this, sender);
                }

                logger.Log(this, "Ready");
            }

            #endregion

            private QS._qss_c_.Devices_3_.ICommunicationsDevice communicationsDevice;
            private QS._qss_c_.Base3_.IDemultiplexer demultiplexer;
            private QS._qss_c_.Base3_.RootSender rootSender;
            [QS._core_c_.Diagnostics.Component("Sender Controller")]
            private QS._qss_c_.Devices_6_.SenderController senderController;
            [QS._core_c_.Diagnostics.Component("Block Sender")]
            private QS._qss_c_.Devices_6_.Sender blockSender;
            [QS._core_c_.Diagnostics.Component("Root Sender")]
            private QS._qss_c_.Base6_.SerializingSender sender;
            private int size, nmessages, nsent;
            private bool prealloc;
            private Request[] preallocatedObjects;
            private QS._qss_c_.Base6_.IChannel channel;

#if DEBUG_EnableStatistics
            [QS.CMS.Diagnostics.Component("Creation Times (X = seqno, Y = time)")]
            private QS.CMS.Statistics.Samples creationTimes = new QS.CMS.Statistics.Samples();
            [QS.CMS.Diagnostics.Component("Transmit Times (X = seqno, Y = time)")]
            private QS.CMS.Statistics.Samples transmitTimes = new QS.CMS.Statistics.Samples();
            [QS.CMS.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
            private QS.CMS.Statistics.Samples receiveTimes = new QS.CMS.Statistics.Samples();
#endif

            [QS.Fx.Base.Inspectable("Collected Statistics", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private QS._core_c_.Components.Attribute collectedStatistics;

            #region Class Request

            private class Request : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>
            {
                public Request(Application application, int seqno)
                {
                    this.application = application;
                    this.seqno = seqno;

                    byte[] bytes = new byte[application.size];
                    unsafe
                    {
                        fixed (byte* pbytes = bytes)
                        {
                            *((int*)pbytes) = seqno;
                        }
                    }

                    message = new QS._core_c_.Base3.Message(loid, new QS._core_c_.Base2.BlockOfData(bytes));
                }

                private Application application;
                private int seqno;
                private QS._core_c_.Base3.Message message;

                #region IAsynchronous<Message,object> Members

                QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
                {
                    get { return message; }
                }

                object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
                {
                    get { return this; }
                }

                QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
                {
                    get { return new QS._core_c_.Base6.CompletionCallback<object>(application.CompletionCallback); }
                }

                #endregion

                public int SeqNo
                {
                    get { return seqno; }
                }
            }

            #endregion

            #region ISource<Asynchronous<Message>> Members

            QS._qss_c_.Base6_.IChannel QS._qss_c_.Base6_.ISource<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Channel
            {
                get { return channel; }
                set { channel = value; }
            }

            void QS._qss_c_.Base6_.ISource<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.GetObjects(
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

                lock (this)
                {
                    numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                    numberOfBytesReturned = 0;
#endif
                    moreObjectsAvailable = true;

                    double time = platform.Clock.Time;

                    while (true)
                    {
                        if (nsent < nmessages)
                        {
                            if (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                            {
                                Request req = prealloc ? preallocatedObjects[nsent] : (new Request(this, nsent));

#if DEBUG_EnableStatistics
                                creationTimes.addSample(nsent, time);
#endif

                                nsent++;
                                numberOfObjectsReturned++;
                                // numberOfBytesReturned += .....................................HERE

                                objectQueue.Enqueue(req);
                            }
                            else
                                break;
                        }
                        else
                        {
                            moreObjectsAvailable = false;
                            break;
                        }
                    }
                }
            }

            #endregion

            #region Callback

            private void CompletionCallback(bool succeeded, Exception exception, object context)
            {
                double time = platform.Clock.Time;

                Request request = (Request) context;

#if DEBUG_EnableStatistics
                lock (this)
                {
                    transmitTimes.addSample(request.SeqNo, time);
                }
#endif

                if (request.SeqNo >= (nmessages - 1))
                    applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
            }

            #endregion

            #region Receive Callback

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                double time = platform.Clock.Time;

                QS._core_c_.Base2.BlockOfData block = (QS._core_c_.Base2.BlockOfData)receivedObject;

                int seqno;
                unsafe
                {
                    fixed (byte* pbytes = block.Buffer)
                    {
                        seqno = *((int*)(pbytes + block.OffsetWithinBuffer));
                    }
                }

#if DEBUG_EnableStatistics
                lock (this)
                {
                    receiveTimes.addSample(seqno, time);
                }
#endif

                return null;
            }

            #endregion

            #region Sending

            public void Send()
            {
                channel.Signal();
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

        public Experiment_257()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
