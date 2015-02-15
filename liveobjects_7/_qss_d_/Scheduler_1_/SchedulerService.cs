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
using System.Threading;
using System.Runtime.Remoting;

#endregion

namespace QS._qss_d_.Scheduler_1_
{
    public static class SchedulerService 
    {
        #region Restarting Services

        public static void restartServices(System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout)
        {
            restartServices(nodeAddresses, logger, shouldStop, shouldStart, timeout, null, null, null);
        }

        public static void restartServices(System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout, string impersonatedUser, string impersonatedDomain, string impersonatedPassword)
        {
            restartServices(QS._qss_d_.Base_.Win32Config.OurServiceName, nodeAddresses, logger, shouldStop, shouldStart, timeout,
                impersonatedUser, impersonatedDomain, impersonatedPassword);
        }

        public static void restartServices(string serviceName, System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout)
        {
            restartServices(serviceName, nodeAddresses, logger, shouldStop, shouldStart, timeout, null, null, null);
        }

        public static void restartServices(string serviceName, System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout, string impersonatedUser, string impersonatedDomain, string impersonatedPassword)
        {
            if (impersonatedUser != null)
                logger.Log(typeof(SchedulerService), "__RestartServices: Impersonating " +
                    ((impersonatedDomain != null) ? (impersonatedDomain + "\\") : "") + impersonatedUser + ".");

            System.ServiceProcess.ServiceController[] serviceControllers = new System.ServiceProcess.ServiceController[nodeAddresses.Length];

            for (uint ind = 0; ind < nodeAddresses.Length; ind++)
            {
                logger.Log("Creating controller for " + nodeAddresses[ind].ToString());
                serviceControllers[ind] = new System.ServiceProcess.ServiceController(serviceName, nodeAddresses[ind].ToString());
            }

            if (shouldStop)
            {
                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security_.Impersonation impersonation =
                        new Security_.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        if (serviceControllers[ind].Status != System.ServiceProcess.ServiceControllerStatus.Stopped)
                        {
                            logger.Log("Stopping " + nodeAddresses[ind].ToString());
                            serviceControllers[ind].Stop();
                        }
                    }
                }

                DateTime operation_deadline = DateTime.Now + TimeSpan.FromSeconds(timeout);

                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security_.Impersonation impersonation =
                        new Security_.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        try
                        {
                            TimeSpan time_to_deadline = operation_deadline - DateTime.Now;
                            serviceControllers[ind].WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped,
                                (time_to_deadline > TimeSpan.Zero ? time_to_deadline : TimeSpan.Zero));
                        }
                        catch (Exception exc)
                        {
                            logger.Log("Cound not change status on " + nodeAddresses[ind].ToString() + ", " + exc.ToString());
                        }

                        logger.Log("Status " + serviceControllers[ind].Status.ToString() + " on " + nodeAddresses[ind].ToString());
                    }
                }
            }

            if (shouldStart)
            {
                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security_.Impersonation impersonation =
                        new Security_.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        if (serviceControllers[ind].Status != System.ServiceProcess.ServiceControllerStatus.Running)
                        {
                            logger.Log("Starting " + nodeAddresses[ind].ToString());
                            serviceControllers[ind].Start();
                        }
                    }
                }

                DateTime operation_deadline = DateTime.Now + TimeSpan.FromSeconds(timeout);

                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security_.Impersonation impersonation =
                        new Security_.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        try
                        {
                            TimeSpan time_to_deadline = operation_deadline - DateTime.Now;
                            serviceControllers[ind].WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running,
                                (time_to_deadline > TimeSpan.Zero ? time_to_deadline : TimeSpan.Zero));
                        }
                        catch (Exception exc)
                        {
                            logger.Log("Cound not change status on " + nodeAddresses[ind].ToString() + ", " + exc.ToString());
                        }

                        logger.Log("Status " + serviceControllers[ind].Status.ToString() + " on " + nodeAddresses[ind].ToString());
                    }
                }
            }
        }

        #endregion
    }

/*
    public class SchedulerService : MarshalByRefObject, QS.TMS.Inspection.IInspectable, System.IDisposable, CMS.Base.IClient, IScheduler 
    {
        public const string ServiceName = "QuickSilver_Scheduler";

        public const uint DefaultSchedulerServicePortNo                 = 10665;
        public const uint DefaultSchedulerServiceRemotingPortNo  = 10664;
        public const string URL = "SchedulerService.soap";
        public const string DefaultSchedulerDirectory = "C:\\.QuickSilver\\.QuickSilver_Scheduler\\.Scheduler_Database";
        public const string DefaultCryptographicKeyPath = "C:\\.QuickSilver\\.QuickSilver_Scheduler\\Secret_Key.dat";

        public static string GenerateURL(string hostname)
        {
            return "http://" + hostname + ":" + DefaultSchedulerServiceRemotingPortNo + "/" + URL;
        }

        public SchedulerService(QS.CMS.Base.IReadableLogger logger, string pathToConfigFile)
        {
            try
            {
                this.schedulerLogger = logger;

                logger.Log(this, "Version: Build " + QS.BuildNo.SeqNo.ToString() + ", " + QS.BuildNo.Date.ToString());

                configuration = Configuration.load(this.pathToConfigFile = pathToConfigFile, logger);
                localAddress = CMS.Devices2.Network.AnyAddressOn(configuration.requestedSubnet);

                logger.Log(null, "Local Address : " + localAddress.ToString() + ":" + configuration.mainPortNo.ToString() +
                    " on subnet " + configuration.requestedSubnet.ToString());

                inspectableGuy = new QS.TMS.Inspection.AttributesOf(this);

                serviceClient = new QS.HMS.Service2.Client(localAddress, null, configuration.cryptographicKeyFile);

                this.tcpDevice = new QS.CMS.Devices.TCPCommunicationsDevice(
                    "SchedulerService_TCP", localAddress, logger, true, (int)configuration.mainPortNo, 10);

                QS.CMS.Base.Serializer.Get.register(QS.ClassID.AnyMessage, QS.CMS.Base.AnyMessage.Factory);
                QS.CMS.Base.Serializer.Get.register(QS.ClassID.XmlMessage, QS.CMS.Base.XmlObject.Factory);

                QS.CMS.Base.Serializer.Get.register(QS.ClassID.CompressedObject, QS.CMS.Base.Serializable<QS.CMS.Base.CompressedObject>.CreateSerializable);
                QS.CMS.Base.Serializer.Get.register(QS.ClassID.Message, QS.CMS.Base.Serializable<QS.CMS.Base.Message>.CreateSerializable);

                demultiplexer = new CMS.Base.SimpleDemultiplexer(3);
                demultiplexer.register(this, new CMS.Dispatchers.MultithreadedDispatcher(new CMS.Base.OnReceive(this.receiveCallback)));
                baseSender = new CMS.Senders.BaseSender(tcpDevice, null, new CMS.Devices.IReceivingDevice[] { tcpDevice }, demultiplexer, logger);

                byte[] cryptographicKey = Service2.ServiceHelper.LoadCryptographicKey(configuration.cryptographicKeyFile);
                cryptographicSender = new CMS.Senders.CryptoSender(baseSender, demultiplexer, logger, System.Security.Cryptography.SymmetricAlgorithm.Create(), cryptographicKey);
                sender = new QS.CMS.Senders.CompressingSender(cryptographicSender, demultiplexer);

                processing_thread = new System.Threading.Thread(new System.Threading.ThreadStart(processing_mainloop));
                processing_thread.Start();

                QS.CMS.Remoting.Channels.CustomizableChannels.Initialize((int) DefaultSchedulerServiceRemotingPortNo, DefaultCryptographicKeyPath);

                RemotingServices.Marshal(this, URL);

                logger.Log(this, "Initialization completed");

                processRequests();
            }
            catch (Exception exc)
            {
                logger.Log(this, "Could not start.\n" + exc.ToString());
                throw new Exception("Could not start.", exc);
            }
        }

        private string pathToConfigFile;
        private QS.CMS.Base.IReadableLogger schedulerLogger;
        private System.Net.IPAddress localAddress;
        private Configuration configuration;
        private QS.HMS.Service2.IClient serviceClient;
        private CMS.Devices.TCPCommunicationsDevice tcpDevice;
        private CMS.Base.IDemultiplexer demultiplexer;
        private CMS.Base.ISender baseSender, cryptographicSender, sender;

		#region Inspection

		[TMS.Inspection.Inspectable]
		public string CurrentLogContents
		{
			get { return schedulerLogger.CurrentContents; }
		}

		private CMS.Components.AttributeSet inspectableProxyCall(QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
		{
			return new QS.CMS.Components.AttributeSet("resultObject", QS.TMS.Inspection.RemotingProxy.DispatchCall(this, arguments));
		}

		#endregion

        #region Methods called by the Clients

        private CMS.Components.AttributeSet GetCurrentLog(QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            return new QS.CMS.Components.AttributeSet("log", this.schedulerLogger.CurrentContents);
        }

        private CMS.Components.AttributeSet ListRequests(QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            ScheduledRequest_Summary[] requests = null;
            lock (configuration)
            {
                requests = new ScheduledRequest_Summary[configuration.scheduledRequests.Count];
                uint index = 0;
                foreach (ScheduledRequest_Summary req in configuration.scheduledRequests)
                    requests[index++] = new ScheduledRequest_Summary(req);
            }

            return new QS.CMS.Components.AttributeSet("requests", requests);
        }

        private CMS.Components.AttributeSet GetRequest(QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            TimeStamp timestamp = (TimeStamp) arguments["timestamp"];
            ScheduledRequest req = null;
            lock (configuration)
            {
                req = configuration.findRequest(timestamp);
            }

            // some race conditions, but we don't care
            return new QS.CMS.Components.AttributeSet("request", req);
        }

/-*
        private CMS.Components.AttributeSet GetRequestResults(QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            TimeStamp timestamp = (TimeStamp)arguments["timestamp"];
            ScheduledRequest req = null;
            lock (configuration)
            {
                req = configuration.findRequest(timestamp);
            }

            // some race conditions, but we don't care
            return new QS.CMS.Components.AttributeSet("request_results", new RequestResults(req));
        }
*-/

        private CMS.Components.AttributeSet Schedule(QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            QS.HMS.Scheduler.Request request = (QS.HMS.Scheduler.Request)arguments["request"];

            TimeStamp timestamp = TimeStamp.Now;

            string directoryName = DefaultSchedulerDirectory + "\\" + timestamp.ToString();
            System.IO.Directory.CreateDirectory(directoryName);

            Service2.DeploymentRequest[] deploymentRequests = 
                new QS.HMS.Service2.DeploymentRequest[request.DeploymentRequests.Length];
            for (uint ind = 0; ind < request.DeploymentRequests.Length; ind++)
            {
                QS.HMS.Service2.ServiceDeploymentRequest dreq = request.DeploymentRequests[ind];

                string localPath = directoryName + "\\File_" + ind.ToString("00") + "." + 
                    dreq.destinationPath.Substring(dreq.destinationPath.LastIndexOf("\\") + 1);
                 
                using (System.IO.FileStream fileStream = 
                    new System.IO.FileStream(localPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    fileStream.Write(dreq.fileAsByteArray, 0, dreq.fileAsByteArray.Length);
                }

                deploymentRequests[ind] = new QS.HMS.Service2.DeploymentRequest(localPath, dreq.destinationPath);
            }

            ScheduledRequest.Experiment[] experimentSpecifications =
                new ScheduledRequest.Experiment[request.ExperimentSpecifications.Length];
            for (uint ind = 0; ind < request.ExperimentSpecifications.Length; ind++)
            {
                experimentSpecifications[ind] = new ScheduledRequest.Experiment(
                    request.ExperimentSpecifications[ind].ExperimentClass, request.ExperimentSpecifications[ind].ExperimentArguments, (ind + 1),
                    directoryName + "\\Experiment_" + (ind + 1).ToString("000000") + "_LogCollection.xml",
                    directoryName + "\\Experiment_" + (ind + 1).ToString("000000") + "_ResultAttributes.xml");
            }

            ScheduledRequest scheduledRequest = new ScheduledRequest(timestamp, deploymentRequests,
                request.HostAddresses, request.ExecutablePath, request.RestartServicesOnStart,
                request.RestartServicesBetweenExperiments, experimentSpecifications);

            lock (configuration)
            {
                configuration.schedule(scheduledRequest);
                configuration.save(pathToConfigFile);
            }

            processRequests();

            return CMS.Components.AttributeSet.None;
        }

        private CMS.Components.AttributeSet GetExperimentLog(
            QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            TimeStamp timestamp = (TimeStamp) arguments["timestamp"];
            uint experiment_seqno = (uint)arguments["experiment_seqno"];

            LogCollection logCollection = null;
            lock (configuration)
            {
                ScheduledRequest req = configuration.findRequest(timestamp);
                ScheduledRequest.Experiment exp = req.findExperiment(experiment_seqno);
                logCollection = exp.CurrentLog;
            }

            return new QS.CMS.Components.AttributeSet("log_collection", logCollection);
        }

        private CMS.Components.AttributeSet GetExperimentResultAttributes(
            QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            TimeStamp timestamp = (TimeStamp)arguments["timestamp"];
            uint experiment_seqno = (uint)arguments["experiment_seqno"];

            CMS.Components.AttributeSet resultAttributes = null;
            lock (configuration)
            {
                ScheduledRequest req = configuration.findRequest(timestamp);
                ScheduledRequest.Experiment exp = req.findExperiment(experiment_seqno);
                resultAttributes = exp.ResultAttributes;
            }

            return new QS.CMS.Components.AttributeSet("result_attributes", resultAttributes);
        }

        private CMS.Components.AttributeSet InterruptExperiment(
            QS.Fx.Network.NetworkAddress clientAddress, CMS.Components.AttributeSet arguments)
        {
            TimeStamp timestamp = (TimeStamp)arguments["timestamp"];
            uint experiment_seqno = (uint)arguments["experiment_seqno"];

            schedulerLogger.Log(this, "Requested Interruption of Experiment " +
                timestamp.ToString() + " / " + experiment_seqno.ToString());

            lock (configuration)
            {
                ScheduledRequest req = configuration.findRequest(timestamp);
                ScheduledRequest.Experiment exp = req.findExperiment(experiment_seqno);

                if (exp.currentStatus.Equals(ScheduledRequest.Status.RUNNING))
                {
                    schedulerLogger.Log(this, "Experiment running, aborting main processing thread.");

                    exp.logCollection.Log(this, 
                        "\n\n********** INTERRUPTING EXPERIMENT UPON USER REQUEST **********\n\n");
                    exp.currentStatus = ScheduledRequest.Status.FAILED;

                    processing_thread.Abort();
                    processing_thread.Join(TimeSpan.FromSeconds(10));

                    if (processing_thread.IsAlive)
                        throw new Exception("Could not abort processing thread.");

                    schedulerLogger.Log(this, "Processing thread aborted sucessfully, saving configuration.");

                    req.verifyStatus();

                    configuration.save(pathToConfigFile);

                    schedulerLogger.Log(this, "Configuration saved, restarting processing thread.");

                    processing_thread = new System.Threading.Thread(
                        new System.Threading.ThreadStart(processing_mainloop));
                    processing_thread.Start();

                    schedulerLogger.Log(this, "Processing thread restarted sucessfully, now ready to run.");
                }
            }

            processRequests();

            return QS.CMS.Components.AttributeSet.None;
        }

        #endregion

        #region Class ScheduledRequest

        #region Class ScheduledRequest_Summary

        [System.Serializable]
        public class ScheduledRequest_Summary : System.IComparable
        {
            public ScheduledRequest_Summary()
            {
            }

            public ScheduledRequest_Summary(TimeStamp timestamp)
            {
                this.timestamp = timestamp;
                this.currentStatus = Status.PENDING;
            }

            public ScheduledRequest_Summary(ScheduledRequest_Summary anotherGuy)
            {
                this.timestamp = anotherGuy.timestamp;
                this.currentStatus = anotherGuy.currentStatus;
            }

            public enum Status
            {
                PENDING, RUNNING, COMPLETED, FAILED
            }

            public TimeStamp timestamp;
            public Status currentStatus;

            public override string ToString()
            {
                return timestamp.ToString();
            }

            public override bool Equals(object obj)
            {
                return (obj != null) && (obj is ScheduledRequest_Summary) &&
                    timestamp.Equals(((ScheduledRequest_Summary)obj).timestamp);
            }

            public override int GetHashCode()
            {
                return timestamp.GetHashCode();
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                    throw new Exception("Cannot compare with a null object.");
                else
                    if (obj is ScheduledRequest_Summary)
                        return timestamp.CompareTo(((ScheduledRequest_Summary)obj).timestamp);
                    else
                        throw new Exception("Cannot compare with object of type " + obj.GetType().FullName);
            }
		}

        #endregion

        [System.Serializable]
        public class ScheduledRequest : ScheduledRequest_Summary
        {
            public ScheduledRequest()
            {
            }

            public ScheduledRequest(TimeStamp timestamp, Service2.DeploymentRequest[] deploymentRequests,
                string[] hostAddresses, string executablePath, bool restartServicesOnStart, bool restartServicesBetween,
                Experiment[] experimentSpecifications) : base(timestamp)
            {
                this.timestamp = timestamp;
                this.deploymentRequests = deploymentRequests;
                this.hostAddresses = hostAddresses;
                this.executablePath = executablePath;
                this.restartServicesBetween = restartServicesBetween;
                this.restartServicesOnStart = restartServicesOnStart;
                
                ExperimentSpecifications = experimentSpecifications;
            }

            #region Class Experiment

            [System.Serializable]
            public class Experiment
            {
                public Experiment()
                {
                }

                public Experiment(string experimentClass, CMS.Components.AttributeSet arguments, uint seqNo,
                    string logCollectionPath, string resultAttributesPath)
                {
                    this.seqNo = seqNo;
                    this.experimentClass = experimentClass;
                    this.arguments = arguments;
                    this.currentStatus = Status.PENDING;
                    this.logCollection = null;
                    this.logCollectionPath = logCollectionPath;
                    this.resultAttributes = null;
                    this.resultAttributesPath = resultAttributesPath;
                }

                public uint seqNo;
                public string experimentClass, logCollectionPath, resultAttributesPath;
                public CMS.Components.AttributeSet arguments;
                public Status currentStatus;

                [System.Xml.Serialization.XmlIgnore]
                public LogCollection logCollection;

                [System.Xml.Serialization.XmlIgnore]
                public CMS.Components.AttributeSet resultAttributes;

                public override string ToString()
                {
                    return seqNo.ToString("00000") + " " + experimentClass.ToString() + " " + arguments.ToString();
                }

                public QS.CMS.Components.AttributeSet ResultAttributes
                {
                    get
                    {
                        if (resultAttributes != null)
                        {
                            return resultAttributes;
                        }
                        else
                        {
                            try
                            {
                                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(CMS.Components.AttributeSet));
                                CMS.Components.AttributeSet attributes = null;
                                using (System.IO.TextReader reader = new System.IO.StreamReader(resultAttributesPath, System.Text.Encoding.Unicode))
                                {
                                    attributes = (CMS.Components.AttributeSet)serializer.Deserialize(reader);
                                    reader.Close();
                                }

                                return attributes;
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Cannot load a saved result attribute set for experiment " + this.ToString() + ".", exc);
                            }
                        }
                    }
                }

                public LogCollection CurrentLog
                {
                    get
                    {
                        if (logCollection != null)
                        {
                            return logCollection.Clone;
                        }
                        else
                        {
                            try
                            {
                                System.Xml.Serialization.XmlSerializer configurationSerializer =
                                    new System.Xml.Serialization.XmlSerializer(typeof(LogCollection));
                                LogCollection collection = null;
                                using (System.IO.TextReader r =
                                    new System.IO.StreamReader(logCollectionPath, System.Text.Encoding.Unicode))
                                {
                                    collection = (LogCollection)configurationSerializer.Deserialize(r);
                                    r.Close();
                                }

                                return collection;
                            }
                            catch (Exception exc)
                            {
                                throw new Exception(
                                    "Cannot load a saved log collection for experiment " + this.ToString() + ".", exc);
                            }
                        }
                    }
                }
            }

            #endregion

            public Service2.DeploymentRequest[] deploymentRequests;
            public string[] hostAddresses;
            public string executablePath;
            public bool restartServicesOnStart, restartServicesBetween;

            [System.Xml.Serialization.XmlIgnore]
            public System.Collections.Generic.Queue<Experiment> experimentSpecifications;
            public Experiment[] ExperimentSpecifications
            {
                get { return experimentSpecifications.ToArray(); }
                set 
                { 
                    experimentSpecifications = new Queue<Experiment>(value.Length); 
                    for (uint ind = 0; ind < value.Length; ind++)
                        experimentSpecifications.Enqueue(value[ind]);
                }
            }

            public Experiment findExperiment(uint seqno)
            {
                foreach (Experiment exp in experimentSpecifications)
                {
                    if (exp.seqNo == seqno)
                        return exp;
                }

                throw new Exception("No such experiment!");
            }

            public void verifyStatus()
            {
                if (currentStatus == Status.RUNNING)
                {
                    bool all_pending = true;
                    bool all_completed = true;
                    bool some_failed = false;
                    foreach (Experiment exp in experimentSpecifications)
                    {
                        if (!exp.currentStatus.Equals(ScheduledRequest.Status.PENDING))
                            all_pending = false;
                        if (!(exp.currentStatus.Equals(ScheduledRequest.Status.COMPLETED) ||
                            exp.currentStatus.Equals(ScheduledRequest.Status.FAILED)))
                            all_completed = false;
                        if (exp.currentStatus.Equals(ScheduledRequest.Status.FAILED))
                            some_failed = true;
                    }

                    if (all_completed)
                        currentStatus = (some_failed ? Status.FAILED : Status.COMPLETED);
                    else if (all_pending)
                        currentStatus = Status.PENDING;
                }
            }

            public void resetStatus()
            {
                if (currentStatus == Status.RUNNING)
                {
                    foreach (Experiment exp in experimentSpecifications)
                    {
                        if (exp.currentStatus == ScheduledRequest.Status.RUNNING)
                            exp.currentStatus = ScheduledRequest.Status.FAILED; // Used to be: ScheduledRequest.Status.PENDING;
                    }

                    verifyStatus();
                }
            }
        }

        #endregion

        #region TimeStamp

        [System.Serializable]
        public class TimeStamp : System.IComparable
        {
            private static object mainlock = new System.Object();
            private static System.DateTime lastTime = System.DateTime.MinValue;
            private static uint lastSeqNo = 0;

            private System.DateTime time;
            private uint seqno;

            public TimeStamp()
            {
            }

            public static TimeStamp Now
            {
                get
                {
                    TimeStamp ts = new TimeStamp();
                    ts.time = System.DateTime.Now;
                    ts.seqno = 0;

                    lock (mainlock)
                    {
                        if (!ts.time.Equals(lastTime))
                        {
                            lastTime = ts.time;
                            lastSeqNo = 0;
                        }

                        ts.seqno = ++lastSeqNo;
                    }

                    return ts;
                }
            }

            public System.DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            public uint SeqNo
            {
                get { return seqno; }
                set { seqno = value; }
            }

            public override string ToString()
            {
                return time.ToString("yyyyMMdd.HHmmss.") + seqno.ToString("0000");
            }

            public override bool Equals(object obj)
            {
                return (obj is TimeStamp) && (((TimeStamp) obj).time.Equals(time)) &&
                    (((TimeStamp)obj).seqno.Equals(seqno));
            }

            public override int GetHashCode()
            {
                return time.GetHashCode() ^ seqno.GetHashCode();
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                    throw new Exception("Cannot compare with a null object.");
                else if (obj is TimeStamp)
                {
                    TimeStamp anotherGuy = obj as TimeStamp;
                    int result = time.CompareTo(anotherGuy.time);
                    return (result == 0) ? (seqno.CompareTo(anotherGuy.seqno)) : result;
                }
                else
                    throw new Exception("Cannot compare with object of type " + obj.GetType().FullName);

            }
        }

        #endregion

        #region Restarting Services

        public static void restartServices(System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout)
        {
            restartServices(nodeAddresses, logger, shouldStop, shouldStart, timeout, null, null, null);
        }

        public static void restartServices(System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger, 
            bool shouldStop, bool shouldStart, double timeout, string impersonatedUser, string impersonatedDomain, string impersonatedPassword)
        {
            restartServices(QS.HMS.Base.Win32Config.OurServiceName, nodeAddresses, logger, shouldStop, shouldStart, timeout,
                impersonatedUser, impersonatedDomain, impersonatedPassword);
        }

        public static void restartServices(string serviceName, System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout)
        {
            restartServices(serviceName, nodeAddresses, logger, shouldStop, shouldStart, timeout, null, null, null);
        }

        public static void restartServices(string serviceName, System.Net.IPAddress[] nodeAddresses, QS.Fx.Logging.ILogger logger,
            bool shouldStop, bool shouldStart, double timeout, string impersonatedUser, string impersonatedDomain, string impersonatedPassword)
        {
            if (impersonatedUser != null)
                logger.Log(typeof(SchedulerService), "__RestartServices: Impersonating " + 
                    ((impersonatedDomain != null) ? (impersonatedDomain + "\\") : "") + impersonatedUser + ".");

            System.ServiceProcess.ServiceController[] serviceControllers = new System.ServiceProcess.ServiceController[nodeAddresses.Length];

            for (uint ind = 0; ind < nodeAddresses.Length; ind++)
            {
                logger.Log("Creating controller for " + nodeAddresses[ind].ToString());
                serviceControllers[ind] = new System.ServiceProcess.ServiceController(serviceName, nodeAddresses[ind].ToString());
            }

            if (shouldStop)
            {
                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security.Impersonation impersonation = 
                        new Security.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        if (serviceControllers[ind].Status != System.ServiceProcess.ServiceControllerStatus.Stopped)
                        {
                            logger.Log("Stopping " + nodeAddresses[ind].ToString());
                            serviceControllers[ind].Stop();
                        }
                    }
                }

                DateTime operation_deadline = DateTime.Now + TimeSpan.FromSeconds(timeout);

                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security.Impersonation impersonation =
                        new Security.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        try
                        {
                            TimeSpan time_to_deadline = operation_deadline - DateTime.Now;
                            serviceControllers[ind].WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped,
                                (time_to_deadline > TimeSpan.Zero ? time_to_deadline : TimeSpan.Zero));
                        }
                        catch (Exception exc)
                        {
                            logger.Log("Cound not change status on " + nodeAddresses[ind].ToString() + ", " + exc.ToString());
                        }

                        logger.Log("Status " + serviceControllers[ind].Status.ToString() + " on " + nodeAddresses[ind].ToString());
                    }
                }
            }

            if (shouldStart)
            {
                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security.Impersonation impersonation =
                        new Security.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        if (serviceControllers[ind].Status != System.ServiceProcess.ServiceControllerStatus.Running)
                        {
                            logger.Log("Starting " + nodeAddresses[ind].ToString());
                            serviceControllers[ind].Start();
                        }
                    }
                }

                DateTime operation_deadline = DateTime.Now + TimeSpan.FromSeconds(timeout);

                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    using (Security.Impersonation impersonation =
                        new Security.Impersonation(impersonatedUser, impersonatedPassword, impersonatedDomain))
                    {
                        try
                        {
                            TimeSpan time_to_deadline = operation_deadline - DateTime.Now;
                            serviceControllers[ind].WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running,
                                (time_to_deadline > TimeSpan.Zero ? time_to_deadline : TimeSpan.Zero));
                        }
                        catch (Exception exc)
                        {
                            logger.Log("Cound not change status on " + nodeAddresses[ind].ToString() + ", " + exc.ToString());
                        }

                        logger.Log("Status " + serviceControllers[ind].Status.ToString() + " on " + nodeAddresses[ind].ToString());
                    }
                }
            }
        }

        #endregion

        #region Processing of Requests

        private void processRequests()
        {
            requestsChanged.Set();
        }

        private bool finishing = false;
        private System.Threading.Thread processing_thread;
        private System.Threading.AutoResetEvent requestsChanged = new System.Threading.AutoResetEvent(false);
        private void processing_mainloop()
        {
            while (!finishing)
            {
                requestsChanged.WaitOne();
                while (!finishing && int_processRequests())
                    ;
            }
        }

        private bool int_processRequests()
        {
            bool processed_something = false;

            lock (configuration)
            {
                ScheduledRequest req = null;
                if (configuration.scheduledRequests.Count > 0)
                {
                    Queue<ScheduledRequest>.Enumerator en = configuration.scheduledRequests.GetEnumerator();
                    while (req == null && en.MoveNext())
                    {
                        ScheduledRequest some_req = en.Current;
                        some_req.verifyStatus();
                        if (!(some_req.currentStatus.Equals(ScheduledRequest.Status.COMPLETED)
                            || some_req.currentStatus.Equals(ScheduledRequest.Status.FAILED)))
                            req = some_req;
                    }
                }

                if (req != null)
                {
                    if (req.currentStatus.Equals(ScheduledRequest.Status.PENDING))
                        req.currentStatus = ScheduledRequest.Status.RUNNING;

                    ScheduledRequest.Experiment exp = null;
                    Queue<ScheduledRequest.Experiment>.Enumerator ien = req.experimentSpecifications.GetEnumerator();
                    while (exp == null && ien.MoveNext())
                    {
                        ScheduledRequest.Experiment some_exp = ien.Current;
                        if (!(some_exp.currentStatus.Equals(ScheduledRequest_Summary.Status.COMPLETED)
                            || some_exp.currentStatus.Equals(ScheduledRequest_Summary.Status.FAILED)))
                            exp = some_exp;
                    }

                    if (exp != null)
                    {
                        if (exp.currentStatus == ScheduledRequest_Summary.Status.PENDING)
                        {
                            processed_something = true;
                            processExperiment(req, exp);
                        }
                    }
                }
            }

            return processed_something;
        }

		[QS.TMS.Inspection.Inspectable("Currently Running", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private QS.TMS.Inspection.AttributeCollection currentlyRunning = 
			new QS.TMS.Inspection.AttributeCollection("Attributes of the Currently Running Experiment");

		[QS.TMS.Inspection.Inspectable("Currently Running Log", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		LogCollection logCollection;

		private void processExperiment(ScheduledRequest req, ScheduledRequest.Experiment exp)
        {
            schedulerLogger.Log(this, "Running Experiment: " + req.ToString() + " / " + exp.ToString());

            exp.currentStatus = ScheduledRequest_Summary.Status.RUNNING;

            req.verifyStatus();
            configuration.save(pathToConfigFile);

            System.Net.IPAddress[] nodeAddresses = new System.Net.IPAddress[req.hostAddresses.Length];
            LogCollection[] subcollections = new LogCollection[req.hostAddresses.Length];
            for (uint ind = 0; ind < nodeAddresses.Length; ind++)
            {
                nodeAddresses[ind] = System.Net.Dns.GetHostAddresses(req.hostAddresses[ind])[0];
                subcollections[ind] = new LogCollection(nodeAddresses[ind].ToString());
            }

			logCollection = new LogCollection("experiment_log", subcollections);
			exp.logCollection = logCollection;

			exp.resultAttributes = new QS.CMS.Components.AttributeSet(20);

            Monitor.Exit(configuration);

            bool experiment_failed = false;
            try
            {
                System.Type experimentType = System.Type.GetType(exp.experimentClass);

                if (req.restartServicesOnStart)
                    restartServices(nodeAddresses, logCollection, true, true, 10.0, 
                        configuration.impersonatedUser, configuration.impersonatedDomain, configuration.impersonatedPassword);

                using (QS.HMS.Service2.IServiceRef serviceRef = serviceClient.connectTo(
                    new QS.Fx.Network.NetworkAddress(nodeAddresses[0],
                    (int)QS.HMS.Base.Win32Config.DefaultMainTCPServicePortNo), 
                    logCollection, TimeSpan.FromSeconds(30)))
                {
                    logCollection.Log(serviceRef.deploy(
                        req.deploymentRequests, nodeAddresses, TimeSpan.FromSeconds(20)));
                }

                logCollection.Log(this, "Starting up remote nodes.");

                QS.TMS.Runtime.IRemoteNode[] remoteNodes = new QS.TMS.Runtime.IRemoteNode[nodeAddresses.Length];
                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    remoteNodes[ind] = new QS.TMS.Runtime.ServiceControlledNode(subcollections[ind], localAddress, 
                        0, 0, serviceClient, new QS.Fx.Network.NetworkAddress(nodeAddresses[ind], 
                        (int)QS.HMS.Base.Win32Config.DefaultMainTCPServicePortNo), TimeSpan.FromSeconds(10), 
                        req.executablePath);
                }

                logCollection.Log(this, "Creating the environment.");

				currentlyRunning.Clear();

				using (QS.TMS.Runtime.IEnvironment environment = 
                    new QS.TMS.Environments.DistributedEnvironment(remoteNodes))
                {
					currentlyRunning.Add(new QS.TMS.Inspection.ScalarAttribute("Environment", environment.Attributes));

					using (QS.TMS.Experiments.IExperiment experiment = 
                        (QS.TMS.Experiments.IExperiment) experimentType.GetConstructor(System.Type.EmptyTypes).Invoke(new object[] {}))
                    {
						currentlyRunning.Add(new QS.TMS.Inspection.ScalarAttribute("Experiment", experiment));

						logCollection.Log(this, "Experiment_Starting...");

                        experiment.run(environment, logCollection, exp.arguments, exp.resultAttributes);

                        logCollection.Log(this, "Experiment_Complete.");

						currentlyRunning.Clear();
					}
                }
            }
            catch (Exception exc)
            {
                experiment_failed = true;
                logCollection.Log(this, "\nExperiment Failed :\n" + exc.ToString() + "\n" + exc.StackTrace+ "\n");
            }

            Monitor.Enter(configuration);

            exp.currentStatus = experiment_failed ? ScheduledRequest_Summary.Status.FAILED : 
                ScheduledRequest_Summary.Status.COMPLETED;

            req.verifyStatus();
            configuration.save(pathToConfigFile);

            try
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(LogCollection));
                using (System.IO.TextWriter w =
                    new System.IO.StreamWriter(exp.logCollectionPath, false, System.Text.Encoding.Unicode))
                {
                    s.Serialize(w, exp.logCollection);
                    w.Close();
                }
            }
            catch (Exception exc)
            {
                schedulerLogger.Log(this, "__processExperiment, while saving logCollection: " + exc.ToString());
            }
            finally
            {
                exp.logCollection = null;
            }

            try
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(CMS.Components.AttributeSet));
                using (System.IO.TextWriter w = new System.IO.StreamWriter(exp.resultAttributesPath, false, System.Text.Encoding.Unicode))
                {
                    s.Serialize(w, exp.resultAttributes);
                    w.Close();
                }
            }
            catch (Exception exc)
            {
                schedulerLogger.Log(this, "__processExperiment, while saving resultAttributes: " + exc.ToString());
            }
            finally
            {
                exp.resultAttributes = null;
            }
        }

        #endregion

        #region Receive Callback

        private void receiveCallback(CMS.Base.IAddress source, CMS.Base.IMessage message)
        {
            try
            {
                if (!(source is CMS.Base.ObjectAddress))
                    throw new Exception("Received a message from an unrecognizable address.");

                if ((message is CMS.Base.AnyMessage) && ((CMS.Base.AnyMessage) message).Contents is Service2.ServiceRequest)
                {
                    object response = null;
                    Service2.ServiceRequest request = (Service2.ServiceRequest) ((CMS.Base.AnyMessage) message).Contents;

                    try
                    {
                        System.Reflection.MethodInfo methodInfo = this.GetType().GetMethod(request.methodName,
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null,
                            new System.Type[] { typeof(QS.Fx.Network.NetworkAddress), typeof(CMS.Components.AttributeSet) }, null);
                        if (methodInfo == null)
                            throw new Exception("The requested method could not be found.");

                        response = methodInfo.Invoke(this, new object[] { ((CMS.Base.ObjectAddress) source), request.argument });

                        if (!(response is CMS.Components.AttributeSet))
                            throw new Exception("Method returned an argument of incompatible type.");
                    }
                    catch (Exception exc)
                    {
                        response = exc;
                    }

                    sender.send(this, source, new CMS.Base.AnyMessage(response), null);
                }
                else
                    throw new Exception("Receive message of an incorrect type.");
            }
            catch (Exception exc)
            {
                schedulerLogger.Log(this, "Receive Callback : " + exc.ToString());
            }
        }

        #endregion

        #region Class Configuration

        [Serializable]
        [System.Xml.Serialization.XmlType("SchedulerServiceConfiguration")]
        public class Configuration
        {
            public Configuration()
            {
            }

	        private void overrideWithDefaultSettings()
			{
				this.requestedSubnet = new CMS.Base.Subnet("192.168.x.x");
				this.mainPortNo = DefaultSchedulerServicePortNo;
				this.cryptographicKeyFile = QS.HMS.Base.Win32Config.DefaultCryptographicKeyFile;
                this.scheduledRequests = new Queue<ScheduledRequest>();
                this.reqindex = new Dictionary<TimeStamp,ScheduledRequest>();
                this.impersonatedUser = "Administrator";
                this.impersonatedDomain = string.Empty;
                this.impersonatedPassword = string.Empty;
            }

            public CMS.Base.Subnet requestedSubnet;
            public uint mainPortNo;
            public string cryptographicKeyFile, impersonatedUser, impersonatedDomain, impersonatedPassword;
            
            [System.Xml.Serialization.XmlIgnore]
            public System.Collections.Generic.Queue<ScheduledRequest> scheduledRequests;
            private System.Collections.Generic.Dictionary<TimeStamp,ScheduledRequest> reqindex;

            public void schedule(ScheduledRequest req)
            {
                scheduledRequests.Enqueue(req);
                reqindex.Add(req.timestamp, req);
            }

            private void resetStatus()
            {
                foreach (ScheduledRequest req in scheduledRequests)
                    req.resetStatus();
            }

            public ScheduledRequest findRequest(TimeStamp timestamp)
            {
                try
                {
                    return reqindex[timestamp];
                }
                catch (Exception exc)
                {
                    System.Text.StringBuilder output = new StringBuilder();
                    output.Append("Could not find request \"" + timestamp.ToString() + "\", DateTime:" + 
                        timestamp.Time.ToLongTimeString() + ", SeqNo:" + timestamp.SeqNo.ToString() + " in the dictionary.\n");
                    foreach (TimeStamp ts in reqindex.Keys)
                        output.Append("Key(DateTime:" + ts.Time.ToLongTimeString() + ", SeqNo:" + ts.SeqNo.ToString() + ")\n");
                    throw new Exception(output.ToString(), exc);
                }
            }

            public ScheduledRequest[] ScheduledRequests
            {
                get { return scheduledRequests.ToArray(); }
                set 
                {
                    scheduledRequests = new Queue<ScheduledRequest>(value.Length);
                    reqindex = new Dictionary<TimeStamp,ScheduledRequest>(value.Length);
                    for (uint ind = 0; ind < value.Length; ind++)
                        schedule(value[ind]);
                }
            }

            public static Configuration load(string localConfigurationFileName, QS.Fx.Logging.ILogger logger)
            {
				Configuration localConfiguration = null;

				try
				{
					System.Xml.Serialization.XmlSerializer configurationSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
					System.IO.TextReader configurationReader = new System.IO.StreamReader(localConfigurationFileName, System.Text.Encoding.Unicode);
					localConfiguration = (Configuration) configurationSerializer.Deserialize(configurationReader);
					configurationReader.Close();
				}
				catch (Exception exc)
				{
					logger.Log(null, "Could not load configuration, overriding with defaults; the exception caught : " + exc.ToString());

					localConfiguration = new Configuration();
					localConfiguration.overrideWithDefaultSettings();
					localConfiguration.save(localConfigurationFileName);
				}

                localConfiguration.resetStatus();

                return localConfiguration;
			}

			public void save(string localConfigurationFileName)
			{
				lock (this)
				{
					System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
					System.IO.TextWriter w = new System.IO.StreamWriter(localConfigurationFileName, false, System.Text.Encoding.Unicode);
					s.Serialize(w, this);
					w.Close();
				}
			}
        }

        #endregion

        public void Dispose()
        {
            this.finishing = true;
            processRequests();

            tcpDevice.shutdown();
        }

        public uint LocalObjectID
        {
            get
            {
                return (uint) ReservedObjectID.HostManagementObject;
            }
        }

        #region IInspectable Members

        private QS.TMS.Inspection.IAttributeCollection inspectableGuy;
        QS.TMS.Inspection.IAttributeCollection QS.TMS.Inspection.IInspectable.Attributes
        {
            get { return inspectableGuy; }
        }

        #endregion

        #region IScheduler Members

        string IScheduler.CurrentLogContents
        {
            get { return schedulerLogger.CurrentContents; }
        }

        #endregion
    }
*/
}
