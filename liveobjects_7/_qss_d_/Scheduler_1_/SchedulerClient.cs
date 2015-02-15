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

// #define CONNECT_VIA_REMOTING

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_d_.Scheduler_1_
{
/*
    public class SchedulerClient : TMS.Inspection.RemotingProxy, ISchedulerClient, CMS.Base.IClient
    {
        public SchedulerClient(CMS.Base.Subnet subnet, System.Net.IPAddress serviceAddress) : this(subnet, serviceAddress, null)
        {
        }

        public SchedulerClient(CMS.Base.Subnet subnet, System.Net.IPAddress serviceAddress, string remotingPathToSecretKey)
            : this(new CMS.Base.Subnet[] { subnet }, serviceAddress, remotingPathToSecretKey)
		{
		}

        public SchedulerClient(System.Collections.Generic.IEnumerable<CMS.Base.Subnet> subnets, System.Net.IPAddress serviceAddress)
            : this(subnets, serviceAddress, null)
        {
        }

		public SchedulerClient(System.Collections.Generic.IEnumerable<CMS.Base.Subnet> subnets,
            System.Net.IPAddress serviceAddress, string remotingPathToSecretKey) 
            : this(subnets, new QS.Fx.Network.NetworkAddress(serviceAddress, (int) SchedulerService.DefaultSchedulerServicePortNo),
                QS.HMS.Base.Win32Config.DefaultCryptographicKeyFile, remotingPathToSecretKey)
        {
        }

        public SchedulerClient(System.Collections.Generic.IEnumerable<CMS.Base.Subnet> subnets, 
			QS.Fx.Network.NetworkAddress serviceAddress, string pathToCryptographicKey, string remotingPathToSecretKey)
        {
            if (remotingPathToSecretKey != null)
            {
                QS.CMS.Remoting.Channels.CustomizableChannels.Initialize(remotingPathToSecretKey);
                schedulerService = (IScheduler)Activator.GetObject(typeof(IScheduler),
                    SchedulerService.GenerateURL(serviceAddress.HostIPAddress.ToString()));
                if (schedulerService == null)
                    throw new Exception("Cannot connect to scheduler.");
            }

            logger = new QS.CMS.Base.Logger(QS.CMS.Base2.PreciseClock.Clock, true);

            this.localAddress = CMS.Devices2.Network.AnyAddressOn(subnets);
            this.serviceAddress = new CMS.Base.ObjectAddress(serviceAddress, (uint) ReservedObjectID.HostManagementObject);

            this.tcpDevice = new QS.CMS.Devices.TCPCommunicationsDevice("SchedulerClient_TCP", localAddress, logger, false, 0, 2);

            QS.CMS.Base.Serializer.Get.register(QS.ClassID.AnyMessage, QS.CMS.Base.AnyMessage.Factory);
            QS.CMS.Base.Serializer.Get.register(QS.ClassID.XmlMessage, QS.CMS.Base.XmlObject.Factory);

			QS.CMS.Base.Serializer.Get.register(QS.ClassID.CompressedObject, QS.CMS.Base.Serializable<QS.CMS.Base.CompressedObject>.CreateSerializable);
			QS.CMS.Base.Serializer.Get.register(QS.ClassID.Message, QS.CMS.Base.Serializable<QS.CMS.Base.Message>.CreateSerializable);

			demultiplexer = new CMS.Base.SimpleDemultiplexer(3);
            demultiplexer.register(this, new CMS.Dispatchers.DirectDispatcher(new CMS.Base.OnReceive(this.receiveCallback)));

            baseSender = new CMS.Senders.BaseSender(tcpDevice, null, new CMS.Devices.IReceivingDevice[] { tcpDevice }, demultiplexer, logger);
			cryptographicSender = new CMS.Senders.CryptoSender(
				baseSender, demultiplexer, logger, System.Security.Cryptography.SymmetricAlgorithm.Create(), 
                Service2.ServiceHelper.LoadCryptographicKey(pathToCryptographicKey));

			sender = new CMS.Senders.CompressingSender(cryptographicSender, demultiplexer);

			this.responseArrived = new System.Threading.AutoResetEvent(false);
        }

        private IScheduler schedulerService;

        private CMS.Base.Logger logger;
        private System.Net.IPAddress localAddress;
        private CMS.Base.ObjectAddress serviceAddress;
        private CMS.Devices.TCPCommunicationsDevice tcpDevice = null;
        private CMS.Base.IDemultiplexer demultiplexer;
        private CMS.Base.ISender baseSender, cryptographicSender, sender;
        private object response = null;
        private System.Threading.AutoResetEvent responseArrived;

		protected override object MakeCall(QS.CMS.Components.AttributeSet arguments)
		{
			return invoke("inspectableProxyCall", arguments)["resultObject"];
		}

		public QS.Fx.Network.NetworkAddress ServiceAddress
        {
            get
            {
                return serviceAddress;
            }
        }

        public CMS.Base.IReadableLogger Logger
        {
            get { return logger; }
        }

        public string CurrentLogContents
        {
            get
            {
                return (string)(invoke("GetCurrentLog", QS.CMS.Components.AttributeSet.None))["log"];
            }
        }

        public SchedulerService.ScheduledRequest_Summary[] ListRequests
        {
            get
            {
                return (SchedulerService.ScheduledRequest_Summary[])
                    (invoke("ListRequests", QS.CMS.Components.AttributeSet.None))["requests"];
            }
        }

        public SchedulerService.ScheduledRequest getRequest(SchedulerService.TimeStamp timestamp)
        {
            return (SchedulerService.ScheduledRequest)
                    (invoke("GetRequest", new QS.CMS.Components.AttributeSet("timestamp", timestamp)))["request"];
        }

/-*
        public SchedulerService.RequestResults gerRequestResults(SchedulerService.TimeStamp timestamp)
        {
            return (SchedulerService.RequestResults)
                    (invoke("GetRequestResults", new QS.CMS.Components.AttributeSet("timestamp", timestamp)))["request_results"];
        }
*-/

        public void schedule(Request experimentRequest)
        {
            this.invoke("Schedule", new QS.CMS.Components.AttributeSet("request", experimentRequest));
        }

        public LogCollection getExperimentLog(SchedulerService.TimeStamp timestamp, uint experimentSeqNo)
        {
            QS.CMS.Components.AttributeSet args = new QS.CMS.Components.AttributeSet(2);
            args["timestamp"] = timestamp;
            args["experiment_seqno"] = experimentSeqNo;
            return (LogCollection) (invoke("GetExperimentLog", args))["log_collection"];
        }

        public CMS.Components.AttributeSet getExperimentResultAttributes(SchedulerService.TimeStamp timestamp, uint experimentSeqNo)
        {
            QS.CMS.Components.AttributeSet args = new QS.CMS.Components.AttributeSet(2);
            args["timestamp"] = timestamp;
            args["experiment_seqno"] = experimentSeqNo;
            return (CMS.Components.AttributeSet)(invoke("GetExperimentResultAttributes", args))["result_attributes"];
        }

        public void interruptExperiment(SchedulerService.TimeStamp timestamp, uint experimentSeqNo)
        {
            QS.CMS.Components.AttributeSet args = new QS.CMS.Components.AttributeSet(2);
            args["timestamp"] = timestamp;
            args["experiment_seqno"] = experimentSeqNo;
            invoke("InterruptExperiment", args);
        }

        #region Sending and Receiving

        private CMS.Components.AttributeSet invoke(string methodName, CMS.Components.AttributeSet argument)
        {
            HMS.Service2.ServiceRequest request = new QS.HMS.Service2.ServiceRequest(methodName, argument);
            object responseObject = null;

            lock (this)
            {
                sender.send(this, serviceAddress, new CMS.Base.AnyMessage(request), null);

                responseArrived.WaitOne();

                responseObject = this.response;
                this.response = null;
            }

            if (responseObject is CMS.Components.AttributeSet)
                return (CMS.Components.AttributeSet) responseObject;
            else if (responseObject is System.Exception)
                throw (System.Exception)responseObject;
            else
                throw new Exception("Unrecognizable response.");
        }

        private void receiveCallback(CMS.Base.IAddress source, CMS.Base.IMessage message)
        {
            if (message is CMS.Base.AnyMessage)
            {
                this.response = ((CMS.Base.AnyMessage) message).Contents;
                responseArrived.Set();
            }
            else
                logger.Log(this, "Unrecognizable response.");
        }

        #endregion

        public override string ToString()
        {
            return serviceAddress.ToString();
        }

        public void Dispose()
        {
            if (tcpDevice != null)
                tcpDevice.shutdown();
        }

        public uint LocalObjectID
        {
            get
            {
                return (uint) ReservedObjectID.HostManagementObject;
            }
        }
    }
*/
}
