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
    /// Measuring throughput of the Devices6.Sender sender using a "pull" scheme from Base6/Devices6.
    /// </summary>
    // [TMS.Experiments.DefaultExperiment]
    [QS._qss_e_.Base_1_.Arguments(
        "-nnodes:0 -count:500000 -size:1000 -maxsends:1000 -maxbytes:1000000 -maxsockets:100 -prealloc:yes " +
        "-download:no -save:no -stabilize:10 -cooldown:25 -stayon:yes -wrapper:yes")]
    public class Experiment_256 : Experiment_200
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

        protected new class Application : Experiment_200.Application, 
            QS._qss_c_.Devices_3_.IReceiver, QS._qss_c_.Base6_.ISource<QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>>
        {
            private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                communicationsDevice = 
                    platform.Network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];
                listener = communicationsDevice.ListenAt(groupAddress, this);

                if (isCoordinator)
                {
                    size = Convert.ToInt32(args["size"]);
                    if (size < sizeof(uint))
                        throw new Exception("Message size too small.");
                    nmessages = Convert.ToInt32(args["count"]);

                    prealloc = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "prealloc");
                    if (prealloc)
                    {
                        preallocatedObjects = new QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>[nmessages];
                        for (int ind = 0; ind < nmessages; ind++)
                            preallocatedObjects[ind] = Allocate(ind);
                    }

                    if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "wrapper"))
                    {
                        sender4 = platform.NetworkConnections[localAddress.HostIPAddress][groupAddress];
                        adapter2AS = new QS._qss_c_.Devices_6_.Adapter2AS(sender4);

                        channel = QS._qss_c_.Base6_.Channel<QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>>.Connect(this, adapter2AS);                        
                    }
                    else
                    {
                        senderController = new QS._qss_c_.Devices_6_.SenderController();
                        sender = new QS._qss_c_.Devices_6_.Sender(localAddress.HostIPAddress, groupAddress, senderController, 10);

                        if (args.contains("maxsends"))
                            senderController.MaximumSends = Convert.ToInt32(args["maxsends"]);
                        if (args.contains("maxbytes"))
                            senderController.MaximumBytes = Convert.ToInt32(args["maxbytes"]);
                        if (args.contains("maxsockets"))
                            sender.MaximumConcurrency = Convert.ToInt32(args["maxsockets"]);

                        channel = QS._qss_c_.Base6_.Channel<QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>>.Connect(this, sender);
                    }
                }

                logger.Log(this, "Ready");
            }

            #endregion

            private QS._qss_c_.Devices_3_.ICommunicationsDevice communicationsDevice;
            private QS._qss_c_.Devices_3_.IListener listener;
            [QS._core_c_.Diagnostics.Component("Devices6.SenderController")]
            private QS._qss_c_.Devices_6_.SenderController senderController;
            [QS._core_c_.Diagnostics.Component("Devices6.Sender")]
            private QS._qss_c_.Devices_6_.Sender sender;
            [QS._core_c_.Diagnostics.Component("Devices4.Sender")]
            private QS._qss_c_.Devices_4_.ISender sender4;
            [QS._core_c_.Diagnostics.Component("Devices6.Adapter2AS")]
            private QS._qss_c_.Devices_6_.Adapter2AS adapter2AS;
            private int size, nmessages, nsent;
            private bool prealloc;
            private QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>[] preallocatedObjects;
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

            #region Allocate

            private QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block> Allocate(int seqno)
            {
                byte[] bytes = new byte[size];
                unsafe
                {
                    fixed (byte* pbytes = bytes)
                    {
                        *((int*)pbytes) = seqno;
                    }
                }

                return new QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>(new QS._qss_c_.Devices_6_.Block(
                    new List<QS.Fx.Base.Block>(new QS.Fx.Base.Block[] { new QS.Fx.Base.Block(bytes) })),
                    bytes, new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback));
            }

            #endregion

            #region ISource<Asynchronous<Block>> Members

            QS._qss_c_.Base6_.IChannel QS._qss_c_.Base6_.ISource<QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>>.Channel
            {
                get { return channel; }
                set { channel = value; }
            }

            void QS._qss_c_.Base6_.ISource<QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>>.GetObjects(
                Queue<QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block>> objectQueue,
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
                                QS._qss_c_.Base6_.Asynchronous<QS._qss_c_.Devices_6_.Block> req = prealloc ? preallocatedObjects[nsent] : Allocate(nsent);

#if DEBUG_EnableStatistics
                                creationTimes.addSample(nsent, time);
#endif

                                nsent++;
                                numberOfObjectsReturned++;
                                // numberOfBytesReturned += ................................................HERE

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

                byte[] bytes = (byte[])context;
                int seqno;
                unsafe
                {
                    fixed (byte* pbytes = bytes)
                    {
                        seqno = *((int*)pbytes);
                    }
                }

#if DEBUG_EnableStatistics
                lock (this)
                {
                    transmitTimes.addSample(seqno, time);
                }
#endif

                if (seqno >= (nmessages - 1))
                    applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
            }

            #endregion

            #region IReceiver Members

            void QS._qss_c_.Devices_3_.IReceiver.receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
            {
                double time = platform.Clock.Time;

                int seqno;
                unsafe
                {
                    fixed (byte* pbytes = data.Array)
                    {
                        seqno = *((int*)(pbytes + data.Offset));
                    }
                }

#if DEBUG_EnableStatistics
                lock (this)
                {
                    receiveTimes.addSample(seqno, time);
                }
#endif
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

        public Experiment_256()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
