/*

Copyright (c) 2010 Matt Pearson. All rights reserved.

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
using System.Linq;
using System.Text;

using Val = QS.Fx.Value.Classes;
using Obj = QS.Fx.Object.Classes;
using Int = QS.Fx.Interface.Classes;

namespace Generator
{
    /// <summary>
    /// A channel that takes text messages from an upper layer and packages them
    /// with a timestamp, sender, and other information, and logs that data when
    /// it is received.
    /// </summary>
    [QS.Fx.Reflection.ComponentClass(
        "BB7AD27E888163364133F2C091DADD05", "TimedChannel",
        "Timestamp outgoing messages and print propagation delays")]
    public sealed class TimedChannel :
        QS._qss_x_.Properties_.Component_.Base_,
        Obj.ICheckpointedCommunicationChannel<Val.IText, Val.IText>,
        Int.ICheckpointedCommunicationChannelClient<TimedMessage, Val.IText>,
        Int.ICheckpointedCommunicationChannel<Val.IText, Val.IText>
    {
        private QS.Fx.Endpoint.Internal.IDualInterface<Int.ICheckpointedCommunicationChannel<TimedMessage, Val.IText>,
            Int.ICheckpointedCommunicationChannelClient<TimedMessage, Val.IText>> lower;
        private QS.Fx.Endpoint.Internal.IDualInterface<Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>,
            Int.ICheckpointedCommunicationChannel<Val.IText, Val.IText>> upper;
        private QS.Fx.Endpoint.IConnection connection;
        private QS.Fx.Object.IReference<Obj.ICheckpointedCommunicationChannel<TimedMessage, Val.IText>> channel;

        private uint seqno = 0;

        public TimedChannel(
            QS.Fx.Object.IContext context,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<Obj.ICheckpointedCommunicationChannel<TimedMessage, Val.IText>> channel)
            : base(context, true)
        {
            this.channel = channel;

            this.lower = context.DualInterface<Int.ICheckpointedCommunicationChannel<TimedMessage, Val.IText>,
                Int.ICheckpointedCommunicationChannelClient<TimedMessage, Val.IText>>(this);

            this.upper = context.DualInterface<Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>,
                Int.ICheckpointedCommunicationChannel<Val.IText, Val.IText>>(this);
            this.upper.OnConnect += new QS.Fx.Base.Callback(connectCallback);
            this.upper.OnDisconnect += new QS.Fx.Base.Callback(disconnectCallback);
        }

        #region connect/disconnect

        void connectCallback()
        {
            if (this.channel != null)
                this.connection = this.lower.Connect(this.channel.Dereference(_mycontext).Channel);
            else
                throw new Exception("channel not given, but connect attempted");
        }

        void disconnectCallback()
        {
            this.connection.Dispose();
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<TimedMessage,IText> Members

        public void Receive(TimedMessage _message)
        {
            string self = System.Net.Dns.GetHostName();
            //double currtime = this._mycontext.Platform.Clock.Time;
            double currtime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            this._logger.Log(string.Format("msg {0} {1} {2} {3} {4} {5} {6}",
                _message.FromId, self, _message.Sequence, _message.Timestamp,
                currtime, _message.SerializableInfo.Size, _message.Payload.Length));
            this.upper.Interface.Receive(new QS.Fx.Value.UnicodeText(_message.Payload));
        }

        public void Initialize(Val.IText _checkpoint)
        {
            this.upper.Interface.Initialize(_checkpoint);
        }

        public Val.IText Checkpoint()
        {
            return this.upper.Interface.Checkpoint();
        }

        #endregion

        #region ICheckpointedCommunicationChannel<IText,IText> Members

        public void Send(Val.IText _message)
        {
            // package message with timestamp and other metadata
 	        //double ts = this._mycontext.Platform.Clock.Time;
            double ts = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            string from = System.Net.Dns.GetHostName();
            TimedMessage msg = new TimedMessage(this.seqno++, ts, from, _message.Text);
            this.lower.Interface.Send(msg);
        }

        #endregion

        #region ICheckpointedCommunicationChannel<IText,IText> Members

        public QS.Fx.Endpoint.Classes.IDualInterface<
            Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>,
            Int.ICheckpointedCommunicationChannel<Val.IText, Val.IText>> Channel
        {
            get { return this.upper; }
        }

        #endregion
    }
}
