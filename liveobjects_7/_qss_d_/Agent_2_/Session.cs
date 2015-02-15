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

namespace QS._qss_d_.Agent_2_
{
    public class Session : ISession
    {
        public Session(QS.Fx.Network.NetworkAddress address, QS.Fx.Logging.ILogger logger, IChannel channel)
        {
            this.address = address;
            this.logger = logger;
            this.channel = channel;

            logger.Log(this, "__________Connected(" + address.ToString() + ")");
        }

        private QS.Fx.Network.NetworkAddress address;
        private QS.Fx.Logging.ILogger logger;
        private IChannel channel;

        #region Accessors

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return address; }
        }

        #endregion

        #region ISession Members

        void ISession.Receive(Message message)
        {
            logger.Log(this, "__________Received(" + address.ToString() + ") : " + QS.Fx.Printing.Printable.ToString(message));

            channel.Send(new Message("Ack(" + message.S + ")"));
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            logger.Log(this, "__________Disposing(" + address.ToString() + ")");
        }

        #endregion
    }
}
