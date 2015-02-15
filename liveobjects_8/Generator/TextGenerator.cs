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
    /// Randomly generate UnicodeText messages on the given channel. Message sizes and the amount
    /// of time between when they are sent are chosen uniformly from the given intervals.
    /// </summary>
    [QS.Fx.Reflection.ComponentClass(
        "517B849D9D5B692D7D7112776A2BEC88", "TextGenerator", "Randomly generate text messages")]
    public sealed class TextGenerator :
        QS._qss_x_.Properties_.Component_.Base_,
        Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>
    {
        private QS.Fx.Endpoint.Internal.IDualInterface<Int.ICheckpointedCommunicationChannel<Val.IText, Val.IText>,
            Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>> endpoint;
        private QS.Fx.Endpoint.IConnection connection;

        private IIntervalInterface generator;
        private QS.Fx.Endpoint.Internal.IImportedInterface<IIntervalInterface> gen_iface;
        private QS.Fx.Endpoint.IConnection gen_connection;

        public TextGenerator(
            QS.Fx.Object.IContext context,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                Obj.ICheckpointedCommunicationChannel<
                    QS.Fx.Value.Classes.IText,
                    QS.Fx.Value.Classes.IText>> channel,
            [QS.Fx.Reflection.Parameter("generator", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<IIntervalObject> generator)
            : base(context, true)
        {
            this.gen_iface = context.ImportedInterface<IIntervalInterface>();
            this.gen_connection = gen_iface.Connect(generator.Dereference(context).Generator);
            this.generator = gen_iface.Interface;

            // init endpoint and channel
            this.endpoint = context.DualInterface<Int.ICheckpointedCommunicationChannel<Val.IText, Val.IText>,
                Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>>(this);

            this.endpoint.OnConnected += new QS.Fx.Base.Callback(
                delegate
                {
                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(unused => this.Schedule()));
                });
            
            if (channel != null)
                this.connection = this.endpoint.Connect(channel.Dereference(context).Channel);
        }

        public void Schedule()
        {
            if (!this.generator.Next())
            {
                this._logger.Log("generator ended, not scheduling any more transmissions");
                return;
            }

            this._platform.AlarmClock.Schedule(this.generator.Interval(),
                new QS.Fx.Clock.AlarmCallback
                (
                    delegate(QS.Fx.Clock.IAlarm _alarm)
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(this.SendMessage));
                    }
                ),
                null);
        }

        public void SendMessage(QS._qss_x_.Properties_.Base_.IEvent_ e)
        {
            StringBuilder x = new StringBuilder(this.generator.Length());
            x.Append('a', this.generator.Length());
            //this._logger.Log("sending message of length " + Convert.ToString(x.Length));
            this.endpoint.Interface.Send(new QS.Fx.Value.UnicodeText(x.ToString()));
            Schedule();
        }

        #region ICheckpointedCommunicationChannelClient<IText,IText> Members

        void Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>.Receive(Val.IText _message)
        {
            //this._logger.Log(string.Format("Received message of length {0}", _message.Text.Length));
        }

        void Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>.Initialize(Val.IText _checkpoint)
        {
            // nothing to do...
        }

        Val.IText Int.ICheckpointedCommunicationChannelClient<Val.IText, Val.IText>.Checkpoint()
        {
            return new QS.Fx.Value.UnicodeText("CHECKPOINT???");
        }

        #endregion
    }
}
