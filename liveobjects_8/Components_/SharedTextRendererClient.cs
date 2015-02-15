/* Copyright (c) 2009 Jared Cantwell. All rights reserved.

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
SUCH DAMAGE. */

#if XNA

using System;
using System.Collections.Generic;

using System.Text;

namespace Demo
{
    [QS.Fx.Reflection.ComponentClass(
    "C363B7E7BB5142a58110D7ABC71C4F67", "SharedTextRendererClient", "Sends text from a channel to the TextRenderer")]
    class SharedTextRendererClient : ITextRendererClient, ITextRendererClientOps,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {

        public SharedTextRendererClient(
            QS.Fx.Object.IContext _mycontext,
        [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> 
                _channel) 
            : base()
        {
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);

            if (_channel != null)
                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint) this._channelendpoint).Connect(_channel.Dereference(_mycontext).Channel);
        
            this.clientendpoint = _mycontext.DualInterface<ITextRendererOps, ITextRendererClientOps>(this);
        }

        // LiveObjects data
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channelendpoint;
        private QS.Fx.Endpoint.IConnection _channelconnection;
        private QS.Fx.Endpoint.Internal.IDualInterface<ITextRendererOps, ITextRendererClientOps> clientendpoint;

        // TextObject
        QS.Fx.Value.Classes.IText text;

        private String[] divide(String text, int maxChars)
        {
            String[] words = text.Split(' ');
            List<String> lines = new List<string>();

            String current = "";
            String word;

            for (int i = 0; i < words.Length; i++)
            {
                word = words[i];
                if (current.Length + word.Length + 1 < maxChars)
                {
                    current += " " + word;
                }
                else
                {
                    lines.Add(current);
                    current = "";
                    if (word.Length > maxChars)
                    {
                        int j = 0;
                        for (; j < word.Length / maxChars; j++)
                            lines.Add(word.Substring(maxChars * j, maxChars));
                        word = word.Substring(maxChars * j);
                    }

                    current = word;
                }
            }

            if (!current.Equals(""))
                lines.Add(current);

            return lines.ToArray();
        }

        #region ITextRendererClientOps Members

        void ITextRendererClientOps.CurrentLocation(MapLibrary.Location loc, MapLibrary.Location topLeft, MapLibrary.Location bottomRight)
        {
            // we don't care about the location for now
            return;
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<IText,IText> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(QS.Fx.Value.Classes.IText _message)
        {
            lock (this)
            {
                text = _message;
                clientendpoint.Interface.FlushContent();
                clientendpoint.Interface.DrawText(new Demo.Xna.Vector3(-73.90f, 40.73f, 0f), divide(text.Text, 20));
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(QS.Fx.Value.Classes.IText _checkpoint)
        {
            if (text == null)
            {
                text = new QS.Fx.Value.UnicodeText("");
                return;
            }

            lock (this)
            {
                text = _checkpoint;
                clientendpoint.Interface.FlushContent();
                clientendpoint.Interface.DrawText(new Demo.Xna.Vector3(-50f, 50f, 0f), divide(text.Text, 20));
            }
        }

        QS.Fx.Value.Classes.IText QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            return text;
        }

        #endregion

        #region ITextRendererClient Members

        QS.Fx.Endpoint.Classes.IDualInterface<ITextRendererOps, ITextRendererClientOps> ITextRendererClient.TextRendererClient
        {
            get { return clientendpoint; }
        }

        #endregion
    }
}

#endif
