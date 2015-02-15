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

namespace QS._qss_x_.Applications_
{
    public class Application_001 : Platform_.IApplication
    {
        public Application_001()
        {
        }

        private QS.Fx.Platform.IPlatform platform;
        private QS._qss_x_.Platform_.IApplicationContext context;
        private QS.Fx.Network.ISender sender;
        private QS.Fx.Network.IListener listener;
        private int seqno;

        #region IApplication Members

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            this.platform = platform;
            this.context = context;

            if (context.NodeNames[0].Equals(platform.Network.GetHostName()))
            {
                IPAddress address = platform.Network.GetHostEntry(context.NodeNames[1]).AddressList[0];
                platform.Logger.Log(this, "SendingTo : " + address.ToString());

                sender = platform.Network.Interfaces[0].GetSender(
                    new QS.Fx.Network.NetworkAddress(address, 10000));
                platform.AlarmClock.Schedule(1, new QS.Fx.Clock.AlarmCallback(this.SendingCallback), null);
            }
            else
            {
                listener = platform.Network.Interfaces[0].Listen(
                    new QS.Fx.Network.NetworkAddress(platform.Network.Interfaces[0].InterfaceAddress, 10000), 
                    new QS.Fx.Network.ReceiveCallback(this.ReceiveCallback), null);
            }
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
        }

        #endregion

        #region SendingCallback

        private void SendingCallback(QS.Fx.Clock.IAlarm alarm)
        {
            string message = "Message(" + (++seqno).ToString() + ")";
            byte[] data = Encoding.ASCII.GetBytes(message);
            List<QS.Fx.Base.Block> segments = new List<QS.Fx.Base.Block>(1);
            segments.Add(new QS.Fx.Base.Block(data));

            platform.Logger.Log(this, "SendingCallback : \"" + message + "\".");
            sender.Send(new QS.Fx.Network.Data(segments));

            if (seqno < 10)
                alarm.Reschedule();
        }

        #endregion

        #region ReceiveCallback

        private void ReceiveCallback(IPAddress ipaddr, int port, QS.Fx.Base.Block data, object context)
        {
            platform.Logger.Log(this, "ReceiveCallback : \"" + Encoding.ASCII.GetString(data.buffer, (int) data.offset, (int) data.size) + "\".");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (listener != null)
                listener.Dispose();
        }

        #endregion
    }
}
