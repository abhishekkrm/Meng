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

namespace QS._core_x_.Unmanaged
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class OutgoingMsg : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public OutgoingMsg(uint channel, IntPtr data, uint size, bool respond, ulong cookie)
        {
            this.channel = channel;
            this.data = data;
            this.size = size;
            this.respond = respond;
            this.cookie = cookie;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        public uint channel;
        [QS.Fx.Printing.Printable]
		public IntPtr data;
        [QS.Fx.Printing.Printable]
		public uint size;
        [QS.Fx.Printing.Printable]
        public bool respond;
        [QS.Fx.Printing.Printable]
        public ulong cookie;
        [QS.Fx.Printing.Printable]
        public uint module;
        // [QS.Fx.Printing.Printable]
        public QS._core_c_.Base6.CompletionCallback<object> callback;

        #endregion

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return new QS._core_c_.Base3.Message(module, this); }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return this; }
        }

        QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return callback; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort) ClassID.Fx_Unmanaged_Msg, 2 * sizeof(uint), 2 * sizeof(uint) + (int) size, 1); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pheader = header.Array)
            {
                *((uint*)(pheader + header.Offset)) = channel;
                *((uint*)(pheader + header.Offset + sizeof(uint))) = size;
            }
            header.consume(2 * sizeof(uint));
            data.Add(new QS.Fx.Base.Block(this.data, 0, size));
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public delegate void OutgoingMsgCallback(OutgoingMsg message);
}
