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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;

namespace QS._qss_e_.Experiments_
{
    [QS._qss_e_.Base_1_.Arguments("-count:100 -size:200 -mtu:1500")]
    public class Experiment_008 : IExperiment
    {
        public Experiment_008()
        {
        }

        public void run(Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS._core_c_.Components.IAttributeSet args, QS._core_c_.Components.IAttributeSet results)
        {
            Runtime_.INodeRef node1 = environment.Nodes[0];
            Runtime_.INodeRef node2 = environment.Nodes[1];

            System.Type testClass = typeof(TestApp);
            app1 = node1.launch(testClass.FullName, QS._core_c_.Components.AttributeSet.None);
            app2 = node2.launch(testClass.FullName, QS._core_c_.Components.AttributeSet.None);

            app2.invoke(testClass.GetMethod("send"), new object[] { node1.NICs[0].ToString(), Convert.ToInt32(args["count"]), 
                Convert.ToInt32(args["size"]), Convert.ToInt32(args["mtu"]) });
        }

        private Runtime_.IApplicationRef app1, app2;

        #region IDisposable Members

        public void Dispose()
        {
            app2.Dispose();
            app1.Dispose();
        }

        #endregion

        public class TestApp : System.IDisposable
        {
            private const int portno = 10000;
            private const uint loid = 666;

            public TestApp(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
            {
                this.platform = platform;
                platform.Logger.Log(this, "Local Address : " + platform.Network.NICs[0].Address.ToString());
                device = platform.Network.NICs[0][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];
                demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(platform.Logger, platform.EventLogger);
                rootSender = new QS._qss_c_.Base3_.RootSender(
                    platform.EventLogger, platform.Logger, device, portno, demultiplexer, platform.Clock, false);
                unwrapper = new QS._qss_c_.Buffering_3_.Unwrapper(
                    2000, demultiplexer, QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass, platform.Logger);

                demultiplexer.register(loid, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));
            }

            private QS._qss_c_.Platform_.IPlatform platform;
            private QS._qss_c_.Devices_3_.ICommunicationsDevice device;
            private QS._qss_c_.Base3_.IDemultiplexer demultiplexer; 
            private QS._qss_c_.Base3_.RootSender rootSender;
            private QS._qss_c_.Buffering_3_.Unwrapper unwrapper;
            private QS._qss_c_.Buffering_3_.IBufferingSender bufferingSender;

            public void send(string address, int numberOfMessages, int messageSize, int maximumSize, double interval)
            {
                isCoordinator = true;
                messageObject = new QS._core_c_.Base2.BlockOfData((uint)messageSize);

                platform.Logger.Log(this, "Sending " + numberOfMessages.ToString() + " of " + messageSize.ToString() + 
                    "-byte messages in " + maximumSize.ToString() + "-byte packets to " + address);

                QS.Fx.Network.NetworkAddress destinationAddress = new QS.Fx.Network.NetworkAddress(IPAddress.Parse(address), portno);

                QS._qss_c_.Buffering_3_.LazySender lazySender = new QS._qss_c_.Buffering_3_.LazySender(2000, rootSender.SenderCollection, 
                    QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass, platform.AlarmClock, TimeSpan.FromSeconds(1), platform.Logger);

                bufferingSender = lazySender.SenderCollection[destinationAddress];

/*
                bufferingSender = new QS.CMS.Buffering3.ThresholdSender(
                    2000, rootSender.CreateSender(destinationAddress), QS.CMS.Buffering3.AccumulatingController.ControllerClass,
                    platform.AlarmClock, platform.Clock, 1, 10, platform.Logger);
*/

                platform.Logger.Log(this, "Using " + bufferingSender.GetType().FullName + ".");

                messagesToGo = numberOfMessages;

                for (int ind = 0; ind < numberOfMessages; ind++)
                {
                    if (ind > 0 && interval > 0)
                        System.Threading.Thread.Sleep((int) Math.Floor(1000 * interval));

                    bufferingSender.send(666, new QS._core_c_.Base2.BlockOfData((uint)messageSize));
                }
            }

            private bool isCoordinator = false;
            private int messagesToGo;
            private QS.Fx.Serialization.ISerializable messageObject;

            private void sendOne()
            {

            }

            private System.Threading.AutoResetEvent completed = new System.Threading.AutoResetEvent(false);
            private QS._qss_c_.Base3_.ISerializableSender responseSender = null;

			private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
            {
                platform.Logger.Log(this, "Received " + receivedObject.ToString() + " from " + sourceIID.Address.ToString() + ".");

                if (isCoordinator)
                {

                }
                else
                {
                    if (responseSender == null)
                        responseSender = rootSender.CreateSender(sourceIID.Address);

                    responseSender.send(666, QS._qss_c_.Base2_.NullObject.Object);
                }

                return null;
            }

            #region IDisposable Members

            public void Dispose()
            {
                rootSender.Dispose();
            }

            #endregion
        }
    }
}
