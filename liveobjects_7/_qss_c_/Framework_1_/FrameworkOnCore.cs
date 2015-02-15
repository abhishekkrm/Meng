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

namespace QS._qss_c_.Framework_1_
{
    public class FrameworkOnCore : QS._qss_c_.Framework_1_.Framework
    {
/*
        public FrameworkOnCore(QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Network.NetworkAddress coordinatorAddress, QS.Fx.Logging.ILogger logger, 
            QS.Fx.Logging.IEventLogger eventLogger, string rootpath, bool activate_fd, int mtu, bool allocateMulticastAddressPerGroup) 
            : this(localAddress, coordinatorAddress, logger, eventLogger, new QS.CMS.Core.Core(rootpath + "\\results"), 
                rootpath + "\\filesystem", activate_fd, mtu, allocateMulticastAddressPerGroup)
        {
        }
*/

        public FrameworkOnCore(QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Network.NetworkAddress coordinatorAddress, QS.Fx.Logging.ILogger logger,
            QS.Fx.Logging.IEventLogger eventLogger, QS._core_c_.Core.Core core, string fsroot, bool activate_fd, int mtu, bool allocateMulticastAddressPerGroup)
            : base(localAddress, coordinatorAddress, new QS._qss_x_.Platform_.PhysicalPlatform(logger, eventLogger, core, fsroot),
                core.StatisticsController, activate_fd, mtu, allocateMulticastAddressPerGroup)
        {
            if (mtu > 0)
                core.DefaultMTU = mtu;
        }

/*
        private Core.Core core;

        public Core.Core Core
        {
            get { return core; }
        }
*/ 
    }
}
