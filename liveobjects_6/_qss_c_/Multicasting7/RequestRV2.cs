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

namespace QS._qss_c_.Multicasting7
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Base.Inspectable]
    public sealed class RequestRV2 : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, QS.Fx.Serialization.ISerializable, IMessageRV2
    {
        static RequestRV2()
        {
            myCompletionCallback = new QS._core_c_.Base6.CompletionCallback<object>(RequestRV2.CompletionCallback);
        }

        private static QS._core_c_.Base6.CompletionCallback<object> myCompletionCallback;

        public delegate void Callback(RequestRV2 request);

        public RequestRV2(Base3_.RVID[] destinationAddresses, uint[] sequenceNos, uint channel, 
            QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message, Callback completionCallback)
        {
            this.destinationAddresses = destinationAddresses;
            this.sequenceNos = sequenceNos;
            this.channel = channel;
            this.message = message;
            this.countdown = sequenceNos.Length;
            this.completionCallback = completionCallback;
        }

        [QS.Fx.Printing.Printable]
        private Base3_.RVID[] destinationAddresses;
        [QS.Fx.Printing.Printable]
        private uint[] sequenceNos;
        [QS.Fx.Printing.Printable]
        private uint channel;
        [QS.Fx.Printing.Printable]
        private double timestamp;

        private QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message;
        private Callback completionCallback;
        private int countdown;

        #region ToString

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region Accessors

        public double CreationTime
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public Base3_.RVID[] RVIDs
        {
            get { return destinationAddresses; }
        }

        public uint[] SeqNos
        {
            get { return sequenceNos; }
        }

        public uint Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> Message
        {
            get { return message; }
        }

        public QS._core_c_.Base3.Message EncapsulatedMessage
        {
            get { return message.Argument; }
        }

        #endregion

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return new QS._core_c_.Base3.Message(channel, this); }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return this; }
        }

        QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return myCompletionCallback; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Multicasting7_MessageRV2, sizeof(ushort) + sequenceNos.Length * sizeof(uint));
                for (int ind = 0; ind < destinationAddresses.Length; ind++)
                    info.AddAnother(destinationAddresses[ind].SerializableInfo);
                info.AddAnother(message.Argument.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((ushort*)headerptr) = (ushort)sequenceNos.Length;
                headerptr += sizeof(ushort);
                for (int ind = 0; ind < sequenceNos.Length; ind++)
                {
                    *((uint*)headerptr) = sequenceNos[ind];
                    headerptr += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + sequenceNos.Length * sizeof(uint));
            for (int ind = 0; ind < sequenceNos.Length; ind++)
                destinationAddresses[ind].SerializeTo(ref header, ref data);
            message.Argument.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Callback

        private static void CompletionCallback(bool succeeded, System.Exception exception, object obj)
        {
            RequestRV2 request = (RequestRV2) obj;
            request.countdown--;
            if (request.countdown == 0)
            {
                if (request.completionCallback != null)
                    request.completionCallback(request);

                QS._core_c_.Base6.CompletionCallback<object> callback = request.message.CompletionCallback;
                if (callback != null)
                    callback(true, null, request.message.Context);
            }
        }

        #endregion
    }
}
