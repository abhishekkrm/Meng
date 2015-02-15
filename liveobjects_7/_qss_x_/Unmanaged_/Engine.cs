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

// #define LogToFileInsteadOfGUI

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;

namespace QS._qss_x_.Unmanaged_
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class Engine : QS.Fx.Inspection.Inspectable, IEngine, IDisposable
    {
        #region Constants

        private const string TEMPDIR = "C:\\QuickSilver\\Temp";
        private const string FSROOTDIR = "C:\\QuickSilver\\Temp\\Root";

        #endregion

        #region Configuration

        public sealed class Configuration
        {
            public static Configuration Load(string filename)
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    return (Configuration)((new XmlSerializer(typeof(Configuration))).Deserialize(reader));
                }
            }

            public Configuration()
            {
            }

            [XmlElement("AdmSubnet")]
            public string adminsubnet;

            [XmlElement("DataSubnet")]
            public string subnet;

            [XmlElement("AdmPort")]
            public int adminport;

            [XmlElement("DataPort")]
            public int port;

            [XmlElement("GMSAddress")]
            public string gms;

            [XmlElement("GMSBatching")]
            public double gmsbatching;

            [XmlElement("EnableFD")]
            public bool failuredetection;

            [XmlElement("MTU")]
            public int mtu;

            [XmlElement("MaximumRate")]
            public int rate;

            [XmlElement("Loopback")]
            public bool loopback;

            [XmlElement("Channel")]
            public Channel[] channels;

            public sealed class Channel
            {
                public Channel()
                {
                }

                [XmlAttribute("id")]
                public uint id;
            }
        }

        #endregion

        #region Constructor

        public Engine()
        {
        }

        #endregion

        #region Fields

        private Thread uiThread;
        private EngineUI ui;
        private IntPtr terminateeventhandle, initializedeventhandle;
        private Configuration configuration;
        private ManualResetEvent uiready = new ManualResetEvent(false);
        private string applicationname;
        private QS._qss_c_.Base1_.Subnet subnet;
        private QS.Fx.Network.NetworkAddress localAddress, gmsAddress;
        private QS._core_c_.Base3.InstanceID instanceID;
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        private QS._qss_c_.Rings6.ReceivingAgent receivingAgentClass;
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RegionID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionSinkCollection;
        private IDictionary<QS._qss_c_.Base3_.GroupID, Channel> channels = new Dictionary<QS._qss_c_.Base3_.GroupID, Channel>();

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("Logger")]
        [QS.Fx.Base.Inspectable("Logger")]
        private QS._qss_c_.Base3_.Logger logger;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("EventLogger")]
        [QS.Fx.Base.Inspectable("EventLogger")]
        private QS._qss_c_.Logging_1_.EventLogger eventLogger;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("Core")]
        [QS.Fx.Base.Inspectable("Core")]
        private QS._core_c_.Core.Core core;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("Platform")]
        [QS.Fx.Base.Inspectable("Platform")]
        private QS._qss_x_.Platform_.PhysicalPlatform platform;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("Framework")]
        [QS.Fx.Base.Inspectable("Framework")]
        private QS._qss_c_.Framework_1_.Framework framework;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("RegionalController")]
        [QS.Fx.Base.Inspectable("RegionalController")]
        private QS._qss_c_.Receivers4.RegionalController regionalController;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("RegionalSenders")]
        [QS.Fx.Base.Inspectable("RegionalSenders")]
        private QS._qss_c_.Senders10.RegionalSenders regionalSenders;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("ReliableRegionViewSinks")]
        [QS.Fx.Base.Inspectable("ReliableRegionViewSinks")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RVID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionViewSinkCollection;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("ReliableGroupSinks")]
        [QS.Fx.Base.Inspectable("ReliableGroupSinks")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> groupSinkCollection;

        [QS.Fx.Base.Inspectable]
        private ICollection<QS._qss_c_.Base3_.GroupID> pendingchannels = new System.Collections.ObjectModel.Collection<QS._qss_c_.Base3_.GroupID>();

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (core != null)
            {
                core.Stop();
                core.Dispose();
            }

            if (ui != null)
            {
                ui.Close();
            }

            if (uiThread != null)
            {
                if (!uiThread.Join(TimeSpan.FromSeconds(1)))
                    uiThread.Abort();
            }
        }

        #endregion

        #region Win32

        [DllImport("kernel32.dll")]
        static extern bool SetEvent(IntPtr hEvent);

        #endregion

        #region _UIThreadCallback

        private void _UIThreadCallback()
        {
            ui = new EngineUI();
#if !LogToFileInsteadOfGUI
            ((QS._core_c_.Base.IOutputReader)logger).Console = ((IEngineUI)ui).Console;
#endif
            ((IEngineUI)ui).OnClosing += new EventHandler(this._ClosingCallback);
            ((IEngineUI)ui).Inspector.Add(this);
            ((IEngineUI)ui).Application = applicationname;
            ((IEngineUI)ui).Status = "Initializing: 1/2";
            uiready.Set();
            System.Windows.Forms.Application.Run(ui);
        }

        #endregion

        #region _ClosingCallback

        private void _ClosingCallback(object sender, EventArgs e)
        {
            SetEvent(terminateeventhandle);
        }

        #endregion

        #region IEngine Members

        void IEngine.Initialize([In] string configfilename, [In] string applicationname, [In] IntPtr initializedeventhandle, [In] IntPtr terminateeventhandle,
            [Out] out IntPtr incomingchannel, [Out] out IntPtr outgoingchannel)
        {
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);

                configuration = Configuration.Load(configfilename);
                this.applicationname = applicationname;

                logger = new QS._qss_c_.Base3_.Logger(QS._qss_x_.Clock_.PhysicalClock.Clock, true, null);

#if LogToFileInsteadOfGUI
                ((QS.CMS.Base.IOutputReader)logger).Console = new QS.HMS.Components.FileLogger("C:\\Lars.txt");
#endif

                eventLogger = new QS._qss_c_.Logging_1_.EventLogger(QS._qss_x_.Clock_.PhysicalClock.Clock, true);

                subnet = new QS._qss_c_.Base1_.Subnet(configuration.subnet);

                foreach (System.Net.IPAddress ipaddress in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && subnet.contains(ipaddress))
                    {
                        localAddress = new QS.Fx.Network.NetworkAddress(ipaddress, configuration.port);
                        break;
                    }
                }

                if (localAddress == null)
                    throw new Exception("Could not find any interface attached to the requested subnet " + subnet.ToString());

                instanceID = new QS._core_c_.Base3.InstanceID(localAddress, DateTime.Now);

                ((QS.Fx.Logging.ILogger)logger).Log(this, "The full address of this QuickSilver instance is : \"quicksilver://" + instanceID.ToString() + "\"");

                if (configuration.gms != null && configuration.gms.Trim().Length > 0)
                    gmsAddress = new QS.Fx.Network.NetworkAddress(configuration.gms);
                else
                    gmsAddress = localAddress;

                if (gmsAddress.Equals(localAddress))
                    ((QS.Fx.Logging.ILogger)logger).Log(this, "This node is hosting the GMS.");
                else
                    ((QS.Fx.Logging.ILogger)logger).Log(this, "The address of the GMS is : \"quicksilver://" + gmsAddress.ToString() + "\"");

                Directory.CreateDirectory(TEMPDIR);
                core = new QS._core_c_.Core.Core(TEMPDIR);

#if OPTION_SUPPORTING_UNMANAGED_APPLICATIONS
                core.OutgoingMsgCallback = new QS._core_x_.Unmanaged.OutgoingMsgCallback(this._OutgoingMsgCallback);
#endif
                Directory.CreateDirectory(FSROOTDIR);
                platform = new QS._qss_x_.Platform_.PhysicalPlatform(logger, eventLogger, core, FSROOTDIR);

                framework = new QS._qss_c_.Framework_1_.Framework(
                    instanceID, gmsAddress, platform, core.StatisticsController, configuration.failuredetection, configuration.mtu, false);

                receivingAgentClass = new QS._qss_c_.Rings6.ReceivingAgent(
                    framework.EventLogger, framework.Logger, framework.LocalAddress, framework.AlarmClock, framework.Clock,
                    framework.Demultiplexer, framework.BufferedReliableInstanceSinks, 5, 1, 0.1, 10, 5000,
                    QS._qss_c_.Rings6.RateSharingAlgorithmClass.Compete, framework.ReliableInstanceSinks, framework.StatisticsController);

                regionalController = new QS._qss_c_.Receivers4.RegionalController(
                    framework.LocalAddress, framework.Logger, framework.Demultiplexer, framework.AlarmClock, framework.Clock,
                    framework.MembershipController, receivingAgentClass, receivingAgentClass);

                regionalController.IsDisabled = false;

                regionalSenders = new QS._qss_c_.Senders10.RegionalSenders(
                    framework.EventLogger, framework.LocalAddress, framework.Logger, framework.AlarmClock, framework.Clock,
                    framework.Demultiplexer, null, // this null argument needs to be fixed ,
                    regionalController, regionalController, configuration.mtu, regionalController,
                    true, 30000, 3);

                QS._qss_c_.Base1_.IFactory<QS._qss_c_.FlowControl7.IRateController> rateControllers =
                    new QS._qss_c_.FlowControl7.DummyController2(framework.Clock, (configuration.rate > 0) ? configuration.rate : double.PositiveInfinity);

                regionSinkCollection = new QS._qss_c_.Multicasting7.RegionSinks(framework.MembershipController, framework.Root);

                regionViewSinkCollection = new QS._qss_c_.Multicasting7.ReliableRegionViewSinks(framework.StatisticsController,
                    framework.Logger, framework.EventLogger, framework.LocalAddress,
                    framework.AlarmClock, framework.Clock, (uint)QS.ReservedObjectID.Rings6_SenderController1_DataChannel,
                    (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel,
                    regionSinkCollection, regionalController, regionalController, framework.MembershipController, framework.Root,
                    60, rateControllers, 3);

                groupSinkCollection =
                    new QS._qss_c_.Multicasting7.ReliableGroupSinks(
                        framework.StatisticsController, framework.Clock, framework.MembershipController, regionViewSinkCollection, logger,
                        100000, 50, 150);

                ((QS._qss_c_.Membership2.Consumers.IGroupCreationAndRemovalProvider)framework.MembershipController).OnChange +=
                    new QS._qss_c_.Membership2.Consumers.GroupCreationOrRemovalCallback(this._GroupCreationOrRemovalCallback);

                framework.Demultiplexer.register(
                    (uint)QS.ReservedObjectID.Fx_Unmanaged_Engine, new QS._qss_c_.Base3_.ReceiveCallback(this._ReceiveCallback));

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

                uiThread = new Thread(new ThreadStart(this._UIThreadCallback));
                uiThread.Start();

                uiready.WaitOne();

                this.terminateeventhandle = terminateeventhandle;
                this.initializedeventhandle = initializedeventhandle;

                incomingchannel = core.IncomingChannel;
                outgoingchannel = core.OutgoingChannel;

                if (framework.MembershipServer != null)
                    framework.MembershipServer.RequestAggregationInterval = configuration.gmsbatching;

                core.Start();

                ((IEngineUI)ui).CanStop = true;

                ((IEngineUI)ui).OnStart += new EventHandler(this._OnStartCallback);
                ((IEngineUI)ui).OnStop += new EventHandler(this._OnStopCallback);

                if (configuration.channels != null && configuration.channels.Length > 0)
                {
                    foreach (Configuration.Channel _channel in configuration.channels)
                        pendingchannels.Add(new QS._qss_c_.Base3_.GroupID(_channel.id));

                    framework.Platform.Scheduler.Execute(new AsyncCallback(this._ChangeMembershipCallback), null);
                }
                else
                    _Ready();
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.Assert(false, exc.ToString() + "\n\n\n" + exc.StackTrace);
                throw new Exception("The engine could not initialize", exc);
            }
        }

        void _OnStartCallback(object sender, EventArgs e)
        {
            core.Start();
            ((IEngineUI)ui).CanStop = true;
            ((IEngineUI)ui).CanStart = false;
        }

        void _OnStopCallback(object sender, EventArgs e)
        {
            core.Stop();
            ((IEngineUI)ui).CanStart = true;
            ((IEngineUI)ui).CanStop = false;
        }

        void IEngine.Log([In] string message)
        {
            ((QS.Fx.Logging.ILogger)logger).Log(message);
        }

        #endregion

        #region _ChangeMembershipCallback

        private void _ChangeMembershipCallback(IAsyncResult result)
        {
            ((IEngineUI)ui).Status = "Initializing: 2/2";

            List<QS._qss_c_.Base3_.GroupID> groupids = new List<QS._qss_c_.Base3_.GroupID>(pendingchannels);

            foreach (QS._qss_c_.Base3_.GroupID groupid in groupids)
                ((QS.Fx.Logging.ILogger)logger).Log("requesting join : " + groupid.ToString());

            framework.MembershipAgent.ChangeMembership(groupids, new List<QS._qss_c_.Base3_.GroupID>());
        }

        #endregion

        #region _GroupCreationOrRemovalCallback

        private void _GroupCreationOrRemovalCallback(IEnumerable<QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval> notifications)
        {
            foreach (QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval notification in notifications)
            {
                if (notification.Creation)
                    _CreateChannel(notification.ID);
            }
        }

        #endregion

        #region _CreateChannel

        private void _CreateChannel(QS._qss_c_.Base3_.GroupID groupid)
        {
            if (pendingchannels.Remove(groupid))
            {
                ((QS.Fx.Logging.ILogger)logger).Log("joined : " + groupid.ToString());
                channels.Add(groupid, new Channel(groupid, groupSinkCollection[groupid]));
            }

            if (pendingchannels.Count == 0)
                _Ready();
        }

        #endregion

        #region _Ready

        private void _Ready()
        {
            ((IEngineUI)ui).Status = "Ready.";

            ((QS.Fx.Logging.ILogger)logger).Log("All channels have been created and the multicast engine has been initialized.");
            SetEvent(initializedeventhandle);
        }

        #endregion

        #region _OutgoingMsgCallback

        private void _OutgoingMsgCallback(QS._core_x_.Unmanaged.OutgoingMsg message)
        {
            QS._qss_c_.Base3_.GroupID groupid = new QS._qss_c_.Base3_.GroupID(message.channel);
            Channel channel;
            if (!(channels.TryGetValue(groupid, out channel)))
                throw new Exception("Could not send message, channel with id = " + message.channel.ToString() + " does not exist.");

            message.module = (uint)QS.ReservedObjectID.Fx_Unmanaged_Engine;
            message.callback = new QS._core_c_.Base6.CompletionCallback<object>(this._CompletionCallback);

            ((IChannel)channel).Send(message);
        }

        #endregion

        #region _CompletionCallback

        private void _CompletionCallback(bool succeeded, System.Exception exception, object context)
        {
#if OPTION_SUPPORTING_UNMANAGED_APPLICATIONS
            core.AcknowledgeOutgoingMsg((QS._core_x_.Unmanaged.OutgoingMsg) context);
#endif
        }

        #endregion

        #region _ReceiveCallback

        private QS.Fx.Serialization.ISerializable _ReceiveCallback(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable message)
        {
            //            ((QS.Fx.Logging.ILogger) logger).Log("QS.Fx.Unmanaged.Engine._ReceiveCallback :\n" + 
            //                QS.Fx.Printing.Printable.ToString(message));
#if OPTION_SUPPORTING_UNMANAGED_APPLICATIONS
            if (message is QS._core_x_.Unmanaged.IncomingMsg)
                core.ProcessIncomingMsg((QS._core_x_.Unmanaged.IncomingMsg) message);
            else if (message is QS._core_x_.Unmanaged.OutgoingMsg)
            {
                if (configuration.loopback)
                    core.UnmanagedLoopback((QS._core_x_.Unmanaged.OutgoingMsg)message);
            }
            else
                throw new Exception("Unknown message type.");
#endif
            return null;
        }

        #endregion
    }
}
