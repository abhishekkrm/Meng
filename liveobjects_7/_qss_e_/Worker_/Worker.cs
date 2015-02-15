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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_e_.Worker_
{
    public class Worker : IWorker
    {
        public const int DefaultConsolePortNo = 65226;
        public const int DefaultControllerPortNo = 65227;

        public Worker(Configuration configuration, string[] addresses, bool coordinator, QS.Fx.Logging.ILogger logger, string resultsdir)
        {
            this.configuration = configuration;
            this.addresses = addresses;
            this.coordinator = coordinator;
            this.logger = logger;
            this.resultsdir = resultsdir;

#if DEBUG_LogGenerously
            logger.Log(this, "__________Creating(coordinator = " + coordinator.ToString() + ")");
            logger.Log(this, "Directory: " + resultsdir);
            logger.Log(this, "Configuration:\n" + QS.Fx.Printing.Printable.ToString(configuration));
#endif

            bool found = false;
            foreach (IPAddress addr in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (configuration.Subnet.contains(addr))
                {
                    found = true;
                    ipaddress = addr;
                    break;
                }
            }

            if (!found)
                throw new Exception("Could not find any address for the local node on the requested subnet { " + configuration.Subnet.ToString() + " }.");

#if DEBUG_LogGenerously
            logger.Log(this, "Local address : " + ipaddress.ToString());
#endif

            ipaddresses = new IPAddress[addresses.Length];
            for (int ind = 0; ind < addresses.Length; ind++)
            {
                found = false;
                foreach (IPAddress addr in Dns.GetHostAddresses(addresses[ind]))
                {
                    if (configuration.Subnet.contains(addr))
                    {
                        found = true;
                        ipaddresses[ind] = addr;
                        break;
                    }
                }

                if (!found)
                    throw new Exception("Could not find any address for \"" + addresses[ind] + "\" on the requested subnet { " + configuration.Subnet.ToString() + " }.");

#if DEBUG_LogGenerously
                logger.Log(this, "Node[" + ind.ToString("000") +"] : " + ipaddresses[ind].ToString());
#endif
            }

            if (!coordinator)
                logger.Log(this, "Coordinator is " + ipaddresses[0].ToString());

            applications = new Application[configuration.Concurrency];

            if (coordinator)
            {
                // .................................................................................................................................................................................................................

            }
            else
            {
                // .................................................................................................................................................................................................................

            }
        }

        private Configuration configuration;
        private string[] addresses;
        private bool coordinator;
        private QS.Fx.Logging.ILogger logger;
        private string resultsdir;
        private IPAddress[] ipaddresses;
        private IPAddress ipaddress;

        private Application[] applications;

        #region IWorker Members

        void IWorker.Work()
        {
#if DEBUG_LogGenerously
            logger.Log(this, "__________Working");
#endif

/*
            for (int ind = 0; ind < applications.Length; ind++)
                applications[ind] = new Application("QuickSilver_RunApp.exe", resultsdir + "\\Process_" + ind.ToString(), ipaddress,
                    ipaddresses[0], DefaultConsolePortNo, DefaultControllerPortNo, logger);
*/

        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
#if DEBUG_LogGenerously
            logger.Log(this, "__________Disposing");
#endif
        }

        #endregion
    }
}
