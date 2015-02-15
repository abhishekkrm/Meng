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
using System.Net;

using QS._qss_e_.Parameters_.Specifications;

namespace QS._qss_e_.Experiments_
{
    public class Experiment_298 : Experiment_200
    {
        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
        }

        protected new class Application : Experiment_200.Application
        {
            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args) : base(platform, args)
            {
                string rootpath = repository_root + "\\" + repository_key;
                core = new QS._core_c_.Core.Core(rootpath + "\\coreworkdir");
                string fsroot = rootpath + "\\filesystem";

                framework = new QS._qss_c_.Framework_1_.FrameworkOnCore(new QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress,
                    logger, platform.EventLogger, core, fsroot, false, 20000, false);
                performanceLog = new QS._qss_c_.Diagnostics_3_.PerformanceLog(framework.Clock, framework.AlarmClock, 1);

                performanceLog.AddCounter("Processor", "_Total", "% Processor Time");
                performanceLog.AddCounter("Processor", "_Total", "Interrupts/sec");
                performanceLog.AddCounter("System", "", "System Calls/sec");
                performanceLog.AddCounter("System", "", "File Data Operations/sec");
                performanceLog.AddCounter("System", "", "Context Switches/sec");
                performanceLog.AddCounter("UDPv4", "", "Datagrams Received Errors");
                performanceLog.AddCounter("PhysicalDisk", "_Total", "Disk Transfers/sec");
                performanceLog.AddCounter("Network Interface", null, "Packets Received Discarded");
                performanceLog.AddCounter("Network Interface", null, "Packets Received Errors");
                performanceLog.AddCounter("Memory", "", "Page Faults/sec");
                performanceLog.AddCounter("IPv4", "", "Datagrams Received Discarded");
                performanceLog.AddCounter(".NET CLR Exceptions", "_Global_", "# of Exceps Thrown / sec");


                core.Start();

                logger.Log(this, "Ready");
            }

            private QS._qss_c_.Framework_1_.FrameworkOnCore framework;
            private QS._qss_c_.Diagnostics_3_.PerformanceLog performanceLog;

            [QS._core_c_.Diagnostics.Component("Core")]
            [QS._core_c_.Diagnostics2.Module("Core")]
            private QS._core_c_.Core.Core core;

            public override void TerminateApplication(bool smoothly)
            {
            }

            public override void Dispose()
            {
            }
        }

        public Experiment_298()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }
    }
}
