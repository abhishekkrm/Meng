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
using System.Net;
using System.Threading;

namespace QS._qss_e_.Components_
{
    public class RemoteController1 : QS.Fx.Inspection.Inspectable, IExperimentController, Base_1_.IApplicationController
    {
        public RemoteController1
        (
            string experiment_path,
            IEnumerable<QS._qss_c_.Base1_.Subnet> localSubnets,
            string cryptographicKeyPath,
            string experimentNodeName,
            string[] workingNodeNames,
            int processesPerNode,
            string[] deployedFiles,
            string deploymentPath,
            string executablePath,
            bool debugging,
            string username,
            string password,
            string domain,
            bool restartWorkers,
            bool uploadFiles
        )
            : this
            (
                experiment_path, 
                localSubnets, 
                cryptographicKeyPath, 
                experimentNodeName,
                workingNodeNames, 
                processesPerNode, 
                deployedFiles, 
                deploymentPath, 
                executablePath, 
                debugging, 
                username,
                password, 
                domain, 
                restartWorkers, 
                uploadFiles,
                null
            )
        {
        }

        public RemoteController1
        (
            string experiment_path,
            IEnumerable<QS._qss_c_.Base1_.Subnet> localSubnets, 
            string cryptographicKeyPath, 
            string experimentNodeName, 
            string[] workingNodeNames, 
            int processesPerNode, 
            string[] deployedFiles, 
            string deploymentPath, 
            string executablePath, 
            bool debugging, 
            string username, 
            string password, 
            string domain, 
            bool restartWorkers, 
            bool uploadFiles,
            QS.Fx.Logging.ILogger _logger
        )
            : this
            (
                experiment_path, 
                localSubnets, 
                QS._qss_d_.Service_2_.ServiceHelper.LoadCryptographicKey(cryptographicKeyPath), 
                experimentNodeName,
                workingNodeNames, 
                processesPerNode, 
                deployedFiles, 
                deploymentPath, 
                executablePath, 
                debugging, 
                username,
                password, 
                domain, 
                restartWorkers, 
                uploadFiles,
                _logger
            )
        {
        }

        public RemoteController1
        (
            string experiment_path,
            IEnumerable<QS._qss_c_.Base1_.Subnet> localSubnets,
            byte[] cryptographicKey,
            string experimentNodeName,
            string[] workingNodeNames,
            int processesPerNode,
            string[] deployedFiles,
            string deploymentPath,
            string executablePath,
            bool debugging,
            string username,
            string password,
            string domain,
            bool restartWorkers,
            bool uploadFiles
        )
            : this
            (
                experiment_path,
                localSubnets,
                cryptographicKey,
                experimentNodeName,
                workingNodeNames,
                processesPerNode,
                deployedFiles,
                deploymentPath,
                executablePath,
                debugging,
                username,
                password,
                domain,
                restartWorkers,
                uploadFiles,
                null
            )
        {
        }

        public RemoteController1
        (
            string experiment_path, 
            IEnumerable<QS._qss_c_.Base1_.Subnet> localSubnets, 
            byte[] cryptographicKey,
            string experimentNodeName, 
            string[] workingNodeNames, 
            int processesPerNode, 
            string[] deployedFiles, 
            string deploymentPath, 
            string executablePath, 
            bool debugging, 
            string username, 
            string password, 
            string domain,
            bool restartWorkers, 
            bool uploadFiles,
            QS.Fx.Logging.ILogger _logger
        )
        {
            this.experimentPath = experiment_path;
            this.localSubnets = localSubnets;
            this.cryptographicKey = cryptographicKey;
            this.experimentNodeName = experimentNodeName;
            this.deploymentPath = deploymentPath;
            this.executablePath = executablePath;
            this.workingNodeNames = workingNodeNames;
            this.deployedFiles = deployedFiles; 
            this.processesPerNode = processesPerNode;
            this.debugging = debugging;
            this.username = username;
            this.password = password;
            this.domain = domain;
            this.restartWorkers = restartWorkers;
            this.uploadFiles = uploadFiles;

            if (_logger == null)
            {
                mainLogger = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true);
                experimentLogger = new QS._core_c_.Base.Logger(null, true);
            }
            else
            {
                mainLogger = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, false, _logger, false, "[RemoteController.Main]");
                experimentLogger = new QS._core_c_.Base.Logger(null, false, _logger, false, "[RemoteController.Experiment]");    
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.Initialize), null);

            if (default_sendmail)
                this.EnableNotification(default_mailaccount, default_mailpassword, default_maildomain, default_mailserver,
                    default_mailfrom, default_mailto, default_mailtoname);
        }

        #region Cut out from constructor, some old stuff

        /*
                IPAddress[] workerAddresses = new IPAddress[MyConfiguration.WorkNodes.Length];
                for (int ind = 0; ind < MyConfiguration.WorkNodes.Length; ind++)
                    workerAddresses[ind] = System.Net.Dns.Resolve(MyConfiguration.WorkNodes[ind]).AddressList[0];

                IPAddress[] addresses = new IPAddress[workerAddresses.Length + 1];
                addresses[0] = experimentNodeAddress;
                for (int ind = 0; ind < workerAddresses.Length; ind++)
                    addresses[ind + 1] = workerAddresses[ind];

                // restarting host manager on all nodes

                QS.HMS.Scheduler.SchedulerService.restartServices("QuickSilver", addresses, mainLogger, true, true, 10);

                IPAddress[] all_addresses = new IPAddress[MyConfiguration.WorkNodes.Length + 1];
                all_addresses[0] = experimentNodeAddress;
                for (int ind = 0; ind < MyConfiguration.WorkNodes.Length; ind++)
                    all_addresses[ind + 1] = Dns.Resolve(MyConfiguration.WorkNodes[ind]).AddressList[0];

                try
                {
                    QS.HMS.Scheduler.SchedulerService.restartServices("QuickSilver", all_addresses, mainLogger, true, true, 10);
                }
                catch (Exception exc)
                {
                    mainlogger.Log(this, exc.ToString() + "\n" + exc.StackTrace + "\n" + exc.Source + "\n" + exc.Message);
                    throw new Exception("Cannot restart services.", exc);
                }

                mainLogger.writeLine("");

                using (QS.TMS.Deployment.IUploader uploader = new QS.TMS.Deployment.ServiceUploader(
                    serviceClient, mainLogger, TimeSpan.FromSeconds(60)))
                {
                    for (int ind = 0; ind < MyConfiguration.DeployedFiles.Length; ind++)
                        uploader.schedule(MyConfiguration.LocalPath + MyConfiguration.DeployedFiles[ind], experimentNodeAddress,
                            MyConfiguration.RemotePath + MyConfiguration.DeployedFiles[ind]);
                }

                using (experimentServiceRef = serviceClient.connectTo(experimentNodeNetworkAddress, mainLogger, TimeSpan.FromSeconds(30)))
                {
                    mainLogger.writeLine("");
                    mainLogger.writeLine(experimentServiceRef.CompleteLog);
                    mainLogger.writeLine("");

                    mainLogger.writeLine(
                        experimentServiceRef.restartServices("QuickSilver_HostAdministrator", MyConfiguration.WorkNodes, true, true, 60,
                        MyConfiguration.UserName, MyConfiguration.Password, MyConfiguration.DomainName));
                }

                QS.HMS.Service2.DeploymentRequest[] deploymentRequests = new QS.HMS.Service2.DeploymentRequest[MyConfiguration.DeployedFiles.Length];
                for (int ind = 0; ind < MyConfiguration.DeployedFiles.Length; ind++)
                    deploymentRequests[ind] = new QS.HMS.Service2.DeploymentRequest(
                        MyConfiguration.LocalPath + MyConfiguration.DeployedFiles[ind], MyConfiguration.RemotePath + MyConfiguration.DeployedFiles[ind]);

                try
                {
                    mainLogger.writeLine(experimentServiceRef.deploy(deploymentRequests, addresses, TimeSpan.FromSeconds(25)));
                }
                catch (Exception exc)
                {
                    mainLogger.writeLine(exc.ToString());

                    // reconnect and show the log
                    experimentServiceRef.Dispose();
                    experimentServiceRef = serviceClient.connectTo(new QS.Fx.Network.NetworkAddress(
                        experimentNodeAddress, (int)QS.HMS.Base.Win32Config.DefaultMainTCPServicePortNo), mainLogger, TimeSpan.FromSeconds(30));
                    mainLogger.writeLine(experimentServiceRef.CompleteLog);
                }

                // starting up remote nodes

                QS.TMS.Runtime.IRemoteNode[] remoteNodes = new QS.TMS.Runtime.IRemoteNode[nodeAddresses.Length];
                for (uint ind = 0; ind < nodeAddresses.Length; ind++)
                {
                    remoteNodes[ind] = new QS.TMS.Runtime.ServiceControlledNode(subcollections[ind], localAddress,
                        0, 0, serviceClient, new QS.Fx.Network.NetworkAddress(nodeAddresses[ind],
                        (int)QS.HMS.Base.Win32Config.DefaultMainTCPServicePortNo), TimeSpan.FromSeconds(10),
                        req.executablePath);
                }
*/

        #endregion

        private string experimentPath;
        private IEnumerable<QS._qss_c_.Base1_.Subnet> localSubnets;
        private byte[] cryptographicKey;
        private string experimentNodeName, deploymentPath, executablePath;
        private string[] workingNodeNames, deployedFiles; 
        private int processesPerNode;
        private bool debugging;
        private string username, password, domain;
        private bool restartWorkers, uploadFiles;
        private bool sendmail;
        private string mailaccount, mailpassword, maildomain, mailserver, mailfrom, mailto, mailtoname;

        #region EnableNotification

        public void EnableNotification(string mailaccount, string mailpassword, string maildomain, string mailserver, string mailfrom, 
            string mailto, string mailtoname)
        {
            sendmail = true;
            this.mailaccount = mailaccount;
            this.mailpassword = mailpassword;
            this.maildomain = maildomain;
            this.mailserver = mailserver;
            this.mailfrom = mailfrom;
            this.mailto = mailto;
            this.mailtoname = mailtoname;
        }

        private static bool default_sendmail;
        private static string default_mailaccount, default_mailpassword, default_maildomain, default_mailserver, default_mailfrom,
            default_mailto, default_mailtoname;

        public static void EnableNotificationByDefault(string mailaccount, string mailpassword, string maildomain, string mailserver, 
            string mailfrom, string mailto, string mailtoname)
        {
            default_sendmail = true;
            default_mailaccount = mailaccount;
            default_mailpassword = mailpassword;
            default_maildomain = maildomain;
            default_mailserver = mailserver;
            default_mailfrom = mailfrom;
            default_mailto = mailto;
            default_mailtoname = mailtoname;
        }

        #endregion

        private ManualResetEvent initializationComplete = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable("Initialization Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Base.Logger mainLogger;
        [QS.Fx.Base.Inspectable("Experiment Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Base.Logger experimentLogger;
        private bool started = false;
        [QS.Fx.Base.Inspectable("Class", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private Type experimentClass;
        [QS.Fx.Base.Inspectable("Arguments", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._core_c_.Components.AttributeSet experimentArgs;
        private QS._qss_e_.Runtime_.IRemoteNode experimentNode;
        [QS.Fx.Base.Inspectable("Experiment App", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Runtime_.IApplicationRef experimentApp;

        private IPAddress localAddress;
        private IPAddress experimentNodeAddress;
        private IPAddress[] experimentNodeAddress_AsArray;
        private QS._qss_d_.Service_2_.IClient serviceClient;

        public event EventHandler OnStarted, OnCompleted, OnDestroyed;

        public QS._core_c_.Base.IReadableLogger MainLogger
        {
            get { return mainLogger; }
        }
            
        public QS._core_c_.Base.IReadableLogger ExperimentLogger
        {
            get { return experimentLogger; }
        }

        #region Initialization

        private void Initialize(object o)
        {
            mainLogger.Log("Initializing the remote experiment controller...");

            try
            {
                localAddress = QS._qss_c_.Devices_2_.Network.AnyAddressOn(localSubnets);
                serviceClient = new QS._qss_d_.Service_2_.Client(localAddress, null, cryptographicKey);
                experimentNodeAddress = System.Net.Dns.GetHostAddresses(experimentNodeName)[0];
                experimentNodeAddress_AsArray = new System.Net.IPAddress[] { experimentNodeAddress };

                try
                {
                    QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                        "QuickSilver_Scheduler", experimentNodeAddress_AsArray, mainLogger, true, false, 30);
                }
                catch (Exception)
                {
                }

                QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                    "QuickSilver_HostAdministrator", experimentNodeAddress_AsArray, mainLogger, true, true, 30);

                QS.Fx.Network.NetworkAddress experimentNodeNetworkAddress = new QS.Fx.Network.NetworkAddress(
                    experimentNodeAddress, (int)QS._qss_d_.Base_.Win32Config.DefaultMainTCPServicePortNo);

                mainLogger.Log("Connecting to controller node at " + experimentNodeNetworkAddress.ToString());

                experimentNode = new QS._qss_e_.Runtime_.ServiceControlledNode(
                    experimentLogger, localAddress, 0, 0, serviceClient, experimentNodeNetworkAddress,
                    TimeSpan.FromSeconds(300), executablePath);
                experimentNode.connect();

                QS._core_c_.Components.AttributeSet launchAttributes = new QS._core_c_.Components.AttributeSet(7);
                launchAttributes["experimentPath"] = experimentPath;
                launchAttributes["workerAddresses"] = workingNodeNames;
                launchAttributes["processesPerNode"] = processesPerNode;
                launchAttributes["deploymentPath"] = deploymentPath;
                launchAttributes["deploymentFiles"] = deployedFiles;
                launchAttributes["localAddress"] = experimentNodeAddress.ToString();
                launchAttributes["executablePath"] = executablePath;
                launchAttributes["debugging"] = debugging;
                launchAttributes["restartWorkers"] = restartWorkers;
                launchAttributes["uploadFiles"] = uploadFiles;
                launchAttributes["username"] = username;
                launchAttributes["password"] = password;
                launchAttributes["domain"] = domain;
                launchAttributes["sendmail"] = sendmail;
                if (sendmail)
                {
                    launchAttributes["mail-account"] = mailaccount;
                    launchAttributes["mail-password"] = mailpassword;
                    launchAttributes["mail-domain"] = maildomain;
                    launchAttributes["mail-hostname"] = mailserver;
                    launchAttributes["mail-from"] = mailfrom;
                    launchAttributes["mail-to"] = mailto;
                    launchAttributes["mail-toname"] = mailtoname;
                }

                mainLogger.Log("Launching application at the controller node.");

                experimentApp = experimentNode.launch(typeof(QS._qss_e_.Launchers_.RunExperiment).FullName, launchAttributes);
                if (experimentApp == null)
                    throw new Exception("ExperimentApp is NULL for \"" + launchAttributes + "\".");

                mainLogger.Log("Application launched successfully at the controller node.");

                experimentApp.Controller = this;
            }
            catch (Exception exc)
            {
                mainLogger.Log(this, "__Initialize: Could not complete initialization.\n" + exc.ToString());
            }

            initializationComplete.Set();
        }

        #endregion

        #region IExperimentController Members

        void IExperimentController.Run(Type experimentClass, QS._core_c_.Components.AttributeSet experimentArgs)
        {
            lock (this)
            {
                if (started)
                    throw new Exception("Already started.");
                started = true;

                this.experimentClass = experimentClass;
                this.experimentArgs = experimentArgs;

                mainLogger.Log(this, "Waiting for initialization to complete...");

                initializationComplete.WaitOne();

                mainLogger.Log(this, "Initialization complete.");

                if (!initializationComplete.WaitOne(0, false))
                    mainLogger.Log(this, "Cannot start experiment, not connected yet.");
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(
                        delegate(object obj) 
                        {
                            QS._core_c_.Components.AttributeSet args = new QS._core_c_.Components.AttributeSet();
                            args["experimentClass"] = experimentClass.FullName;
                            args["experimentArguments"] = experimentArgs.AsString;
                            System.Reflection.MethodInfo methodInfo = typeof(QS._qss_e_.Launchers_.RunExperiment).GetMethod(
                                "runExperiment", new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) });

                            mainLogger.Log(this, "Calling the experiment app to start.");

                            experimentApp.invoke(methodInfo, new object[] { args });
                        }), null);
                }
            }
        }

        void IExperimentController.Shutdown()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                delegate(object obj) 
                {
                    System.Reflection.MethodInfo methodInfo = 
                        typeof(QS._qss_e_.Launchers_.RunExperiment).GetMethod("stopExperiment", System.Type.EmptyTypes);
                    experimentApp.invoke(methodInfo, new object[] { });
                    Thread.Sleep(5);
                    
                    try
                    {
                        experimentApp.Dispose();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        experimentNode.Dispose();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        QS._qss_d_.Scheduler_1_.SchedulerService.restartServices(
                            "QuickSilver_HostAdministrator", experimentNodeAddress_AsArray, mainLogger, true, false, 30);
                    }
                    catch (Exception)
                    {
                    }

                    experimentApp = null;
                    experimentNode = null;
                }), null);
        }

        Type IExperimentController.Class
        {
            get { return started ? experimentClass : null; }
        }

        QS._core_c_.Components.AttributeSet IExperimentController.Arguments
        {
            get { return started ? experimentArgs : null; }
        }

        QS._core_c_.Components.AttributeSet IExperimentController.Results
        {
            get 
            {
                return (QS._core_c_.Components.AttributeSet) (((QS.Fx.Inspection.IScalarAttribute)((QS.Fx.Inspection.IInspectable)
                    experimentApp).Attributes["Result Attributes"]).Value);
            }
        }

        #endregion

        #region IApplicationController Members

        QS._core_c_.Components.AttributeSet QS._qss_e_.Base_1_.IApplicationController.upcall(string operation, QS._core_c_.Components.AttributeSet arguments)
        {
            EventHandler handler = null;

            if (operation.Equals("Experiment_Started"))
            {
                handler = this.OnStarted;
            }
            else if (operation.Equals("Experiment_Completed"))
            {
                handler = this.OnCompleted;
            }
            else if (operation.Equals("Experiment_Destroyed"))
            {
                handler = this.OnDestroyed;
            }

            if (handler != null)
                handler(this, null);
            else
                experimentLogger.Log(this, "Unhandled Upcall: " + operation);

            return QS._core_c_.Components.AttributeSet.None;
        }

        #endregion
    }
}
