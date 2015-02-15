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

// #define OPTION_EnableGUI

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.ServiceModel;

namespace QS._qss_x_.Service_Old_
{
    public sealed class SystemService : IDisposable
    {
        #region Constructor

        private const string DefaultConfigurationFileName = "C:\\QuickSilver\\Configuration.xml";
        private const int DefaultManagementPort = 65000;

        public SystemService() : this(DefaultConfigurationFileName, DefaultManagementPort)
        {
        }

        public SystemService(int managementport) : this(DefaultConfigurationFileName, managementport)
        {
        }

        public SystemService(string configurationfilename) : this(configurationfilename, DefaultManagementPort)
        {
        }

        public SystemService(string configurationfilename, int managementport)
            : this(Configuration.Load((configurationfilename != null) ? configurationfilename : DefaultConfigurationFileName), managementport)
        {
        }

        public SystemService(Configuration configuration, int managementport)
        {
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);

                logger = new QS._core_c_.Base.Logger(QS._qss_x_.Clock_.PhysicalClock.Clock, true);
                eventLogger = new QS._qss_c_.Logging_1_.EventLogger(QS._qss_x_.Clock_.PhysicalClock.Clock, true);
                Directory.CreateDirectory(TEMPDIR);
                core = new QS._core_c_.Core.Core(TEMPDIR);
                platform = new QS._qss_x_.Platform_.PhysicalPlatform(logger, eventLogger, core, FSROOTDIR);
                service = new Service(logger, platform, configuration);
                servicehost = new ServiceHost(service);

                string serviceendpointaddr = "http://" + ((QS.Fx.Platform.IPlatform)platform).Network.GetHostName() + ":" + managementport.ToString();
                servicehost.AddServiceEndpoint(typeof(IService), new WSHttpBinding(), serviceendpointaddr);

                logger.Log("hosting administrative interface at: " + serviceendpointaddr);

                /*
                            foreach (QS.Fx.Network.INetworkInterface netinterface in ((QS.Fx.Platform.IPlatform)platform).Network.Interfaces)
                            {
                                if (netinterface.InterfaceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    string serviceendpointaddr = "http://" + netinterface.InterfaceAddress.ToString() + ":" + managementport.ToString();
                                    servicehost.AddServiceEndpoint(typeof(IService), new WSHttpBinding(), serviceendpointaddr);

                                    logger.Log("hosting administrative interface at: " + serviceendpointaddr);
                                }
                            }
                */

                servicehost.Open();

#if OPTION_EnableGUI
            windowThread = new Thread(new ThreadStart(this.WindowMain));
            windowThread.Start();
#endif

                core.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Constants

        private const string TEMPDIR = "C:\\QuickSilver\\Temp";
        private const string FSROOTDIR = "C:\\QuickSilver\\Temp\\Root";

        #endregion

        #region Fields

        private QS._core_c_.Base.Logger logger;
        private QS._qss_c_.Logging_1_.EventLogger eventLogger;
        private QS._core_c_.Core.Core core;
        private QS._qss_x_.Platform_.PhysicalPlatform platform;
        private Service service;
        private ServiceHost servicehost;

#if OPTION_EnableGUI
        private Thread windowThread;
        private SystemServiceWindow window;
#endif

        #endregion

        #region Accessors and public stuff

        public IServiceControl ServiceControl
        {
            get { return service; }
        }

        public QS._qss_x_.Service_Old_.IService Service
        {
            get { return service; }
        }

        public void Log(string message)
        {
            logger.Log(message);
        }

        #endregion

        #region WindowMain

#if OPTION_EnableGUI
        private void WindowMain()
        {
            window = new SystemServiceWindow();
            logger.Console = window.LogConsole;
            Application.Run(window);
        }
#endif

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            servicehost.Close();
            ((IDisposable) service).Dispose();
            core.Stop();
            core.Dispose();

#if OPTION_EnableGUI
            window.Close();
            if (!windowThread.Join(TimeSpan.FromSeconds(1)))
                windowThread.Abort();
#endif
        }

        #endregion
    }
}
