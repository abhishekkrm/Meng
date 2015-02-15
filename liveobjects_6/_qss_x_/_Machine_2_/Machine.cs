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

namespace QS._qss_x_._Machine_2_
{
    [QS._qss_x_.Platform_.Application("Machine")]
    public sealed class Machine : QS._qss_x_.Platform_.IApplication
    {
        public Machine()
        {
        }

        private QS.Fx.Platform.IPlatform platform;
        private QS._qss_x_.Platform_.IApplicationContext context;
        private QS._qss_x_._Machine_2_.Replicated.Replica replica;

        #region IApplication Members

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            this.platform = platform;
            this.context = context;

            QS._qss_x_._Machine_2_.Replicated.BootOption bootoption = QS._qss_x_._Machine_2_.Replicated.BootOption.None;
            if (platform.Network.GetHostName().Equals(context.NodeNames[0]))
                bootoption |= QS._qss_x_._Machine_2_.Replicated.BootOption.Master;

            this.replica = new QS._qss_x_._Machine_2_.Replicated.Replica(
                platform, null, bootoption, new QS.Fx.Base.ID(new Guid("{392188D1-7E90-410c-BEA1-F1DCA34B6EFF}")));
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
            ((IDisposable)replica).Dispose();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
