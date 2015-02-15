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
/*
    /// <summary>
    /// Measuring throughput of the Multicasting7.PlaceholderGS sender (using a "pull" scheme).
    /// </summary>
    [TMS.Experiments.DefaultExperiment]
    [TMS.Base.Arguments(
        "-nnodes:0 -count:100000 -size:100 -maxsends:1000 -maxbytes:1000000 -maxsockets:100 -prealloc:yes " +
        "-download:no -save:no -stabilize:10 -cooldown:10 -stayon:yes")]
    public class Experiment_258 : Experiment_200
    {
        private const string DefaultRepositoryPath = "C:\\.QuickSilver\\.Repository";

        #region experimentWork

        protected override void experimentWork(QS.CMS.Components.IAttributeSet results)
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

                CMS.Components.AttributeSet applicationStatistics = new QS.CMS.Components.AttributeSet();
                foreach (TMS.Runtime.IApplicationRef application in this.Applications)
                {
                    logger.Log(this, "Downloading from " + application.AppID.ToString());
                    object obj = application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { true });
                    if (obj != null && obj is QS.CMS.Components.Attribute)
                        applicationStatistics.Add((CMS.Components.Attribute)obj);
                }

                results["Application Statistics"] = new CMS.Components.Attribute("Application Statistics", applicationStatistics);
            }
            else
            {
                foreach (TMS.Runtime.IApplicationRef application in this.Applications)
                    application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { false });
            }

            if (arguments.contains("save") && (arguments["save"].Equals("yes") || arguments["save"].Equals("on")))
                QS.TMS.Experiment.Helpers.SaveResults.Save(DefaultRepositoryPath, logger, this, arguments, results);

            logger.Log(this, "Completed.");
        }

        #endregion

        #region Callbacks

        private ManualResetEvent sendingCompleted = new ManualResetEvent(false);
        public void SendingCompleted(QS.CMS.Components.AttributeSet arguments)
        {
            sendingCompleted.Set();
        }

        #endregion

        #region Class Application

        protected new class Application
            : Experiment_200.Application, CMS.Base6.ISource<CMS.Base6.IAsynchronous<CMS.Base3.Message>>
        {
            private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
            private static readonly uint loid = ((uint)QS.ReservedObjectID.User_Min) + 100;

            #region Constructor

            public Application(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args)
                : base(platform, args)
            {
                communicationsDevice =
                    platform.Network[localAddress.HostIPAddress][QS.CMS.Devices3.CommunicationsDevice.Class.UDP];
                demultiplexer = new CMS.Base3.Demultiplexer(platform.Logger, platform.EventLogger);
                rootSender = new QS.CMS.Base3.RootSender(platform.EventLogger, incarnation, platform.Logger,
                    communicationsDevice, localAddress.PortNumber, demultiplexer, platform.Clock);
                ((CMS.Devices3.IMembershipController)rootSender).Join(groupAddress);

                demultiplexer.register(loid, new QS.CMS.Base3.ReceiveCallback(this.ReceiveCallback));

                if (isCoordinator)
                {
                    senderController = new QS.CMS.Devices6.SenderController();
                    blockSender = new CMS.Devices6.Sender(localAddress.HostIPAddress, groupAddress, senderController, 10);
                    sender = new QS.CMS.Base6.SerializingSender(new CMS.QS._core_c_.Base3.InstanceID(localAddress, incarnation), blockSender);

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

                    prealloc = TMS.Experiment.Helpers.Args.BoolOf(args, "prealloc");
                    if (prealloc)
                    {
                        preallocatedObjects = new Request[nmessages];
                        for (int ind = 0; ind < nmessages; ind++)
                            preallocatedObjects[ind] = new Request(this, ind);
                    }

                    channel = CMS.Base6.Channel<CMS.Base6.IAsynchronous<CMS.Base3.Message>>.Connect(this, sender);
                }

                logger.Log(this, "Ready");
            }

            #endregion

            private CMS.Devices3.ICommunicationsDevice communicationsDevice;
            private CMS.Base3.IDemultiplexer demultiplexer;
            private CMS.Base3.RootSender rootSender;
            [QS.CMS.Diagnostics.Component("Sender Controller")]
            private CMS.Devices6.SenderController senderController;
            [QS.CMS.Diagnostics.Component("Block Sender")]
            private CMS.Devices6.Sender blockSender;
            [QS.CMS.Diagnostics.Component("Root Sender")]
            private CMS.Base6.SerializingSender sender;
            private int size, nmessages, nsent;
            private bool prealloc;
            private Request[] preallocatedObjects;
            private CMS.Base6.IChannel channel;

            [QS.CMS.Diagnostics.Component("Creation Times (X = seqno, Y = time)")]
            private QS.CMS.Statistics.Samples creationTimes = new QS.CMS.Statistics.Samples();
            [QS.CMS.Diagnostics.Component("Transmit Times (X = seqno, Y = time)")]
            private QS.CMS.Statistics.Samples transmitTimes = new QS.CMS.Statistics.Samples();
            [QS.CMS.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
            private QS.CMS.Statistics.Samples receiveTimes = new QS.CMS.Statistics.Samples();

            [TMS.Inspection.Inspectable("Collected Statistics", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
            private QS.CMS.Components.Attribute collectedStatistics;

            #region Class Request

            private class Request : CMS.Base6.IAsynchronous<CMS.Base3.Message>
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

                    message = new QS.CMS.Base3.Message(loid, new CMS.Base2.BlockOfData(bytes));
                }

                private Application application;
                private int seqno;
                private CMS.Base3.Message message;

                #region IAsynchronous<Message,object> Members

                QS.CMS.Base3.Message QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message, object>.Argument
                {
                    get { return message; }
                }

                object QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message, object>.Context
                {
                    get { return this; }
                }

                QS.CMS.Base6.Callback<object> QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message, object>.Callback
                {
                    get { return new QS.CMS.Base6.Callback<object>(application.Callback); }
                }

                #endregion

                public int SeqNo
                {
                    get { return seqno; }
                }
            }

            #endregion

            #region ISource<Asynchronous<Message>> Members

            QS.CMS.Base6.IChannel QS.CMS.Base6.ISource<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>.Channel
            {
                get { return channel; }
                set { channel = value; }
            }

            void QS.CMS.Base6.ISource<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>.GetObjects(
                Queue<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>> objectQueue,
                int maximumNumberOfObjects, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
            {
                lock (this)
                {
                    numberOfObjectsReturned = 0;
                    moreObjectsAvailable = true;

                    double time = platform.Clock.Time;

                    while (true)
                    {
                        if (nsent < nmessages)
                        {
                            if (numberOfObjectsReturned < maximumNumberOfObjects)
                            {
                                Request req = prealloc ? preallocatedObjects[nsent] : (new Request(this, nsent));

                                creationTimes.addSample(nsent, time);

                                nsent++;
                                numberOfObjectsReturned++;

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

            private void Callback(bool succeeded, Exception exception, object context)
            {
                double time = platform.Clock.Time;

                Request request = (Request)context;

                lock (this)
                {
                    transmitTimes.addSample(request.SeqNo, time);
                }

                if (request.SeqNo >= (nmessages - 1))
                    applicationController.upcall("SendingCompleted", CMS.Components.AttributeSet.None);
            }

            #endregion

            #region Receive Callback

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                CMS.QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                double time = platform.Clock.Time;

                CMS.Base2.BlockOfData block = (CMS.Base2.BlockOfData)receivedObject;

                int seqno;
                unsafe
                {
                    fixed (byte* pbytes = block.Buffer)
                    {
                        seqno = *((int*)(pbytes + block.OffsetWithinBuffer));
                    }
                }

                lock (this)
                {
                    receiveTimes.addSample(seqno, time);
                }

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

            public CMS.Components.Attribute GetStatistics(bool upload)
            {
                collectedStatistics =
                    new QS.CMS.Components.Attribute(localAddress.ToString(), QS.CMS.Diagnostics.DataCollector.Collect(this));

                return upload ? collectedStatistics : new QS.CMS.Components.Attribute("foo", "bar");
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

        public Experiment_258()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
*/ 
}
