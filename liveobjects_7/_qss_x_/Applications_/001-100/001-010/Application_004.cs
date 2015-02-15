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

namespace QS._qss_x_.Applications_
{
    [QS.Fx.Base.Inspectable]
    public class Application_004 : QS.Fx.Inspection.Inspectable, Platform_.IApplication
    {
        public Application_004()
        {
        }

        private QS._qss_x_._Machine_1_.Components.ReplicaProtocolStack replicaProtocolStack;

        #region IApplication Members

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            replicaProtocolStack = new QS._qss_x_._Machine_1_.Components.ReplicaProtocolStack(
                platform, 12000, "Moo", platform.Network.GetHostName().Equals(context.NodeNames[0]),
                new QS.Fx.Network.NetworkAddress[] { new QS.Fx.Network.NetworkAddress("224.55.55.55:13000") });
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
            ((IDisposable) replicaProtocolStack).Dispose();
            replicaProtocolStack = null;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
