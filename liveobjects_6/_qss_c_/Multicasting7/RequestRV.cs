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
    public sealed class RequestRV : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, QS.Fx.Serialization.ISerializable, Receivers4.IAcknowledgeable
    {
        public delegate void Callback(RequestRV request);

        public RequestRV(Base3_.RVID destinationAddress, uint channel, uint sequenceNo,
            QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message, QS._core_c_.Base6.CompletionCallback<object> transmissionCallback,             
            Callback retransmissionCallback, Callback acknowledgementCallback)
        {
            this.channel = channel;
            this.destinationAddress = destinationAddress;
            this.sequenceNo = sequenceNo;
            this.message = message;
            this.transmissionCallback = transmissionCallback;
            this.retransmissionCallback = retransmissionCallback;
            this.acknowledgementCallback = acknowledgementCallback;
        }

        [QS.Fx.Printing.Printable] private Base3_.RVID destinationAddress;
        [QS.Fx.Printing.Printable] private uint channel, sequenceNo;
        [QS.Fx.Printing.Printable] private bool completed, retransmitting, retransmitted;
        [QS.Fx.Printing.Printable] private double timestamp, lastretransmitted;

        private QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message;
        private QS._core_c_.Base6.CompletionCallback<object> transmissionCallback;
        private Callback retransmissionCallback, acknowledgementCallback;
        private QS.Fx.Clock.IAlarm alarm;

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #region Accessors

        public double CreationTime
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public uint SequenceNo
        {
            get { return sequenceNo; }
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

        public QS.Fx.Clock.IAlarm Alarm
        {
            get { return alarm; }
            set { alarm = value; }
        }

        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }

        public bool Retransmitting
        {
            get { return retransmitting; }
            set { retransmitting = value; }
        }

        public bool Retransmitted
        {
            get { return retransmitted; }
            set { retransmitted = value; }
        }

        public double LastRetransmitted
        {
            get { return lastretransmitted; }
            set { lastretransmitted = value; }
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
            get { return transmissionCallback; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Multicasting5_MessageRV, sizeof(uint), sizeof(uint), 0);
                info.AddAnother(message.Argument.SerializableInfo);
                info.AddAnother(destinationAddress.SerializableInfo);
                return info;                
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            destinationAddress.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((uint*)headerptr) = sequenceNo;
            }
            header.consume(sizeof(uint));
            message.Argument.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IAcknowledgeable Members

        uint QS._qss_c_.Receivers4.IAcknowledgeable.SequenceNo
        {
            get { return sequenceNo; }
        }

        QS._core_c_.Base3.Message QS._qss_c_.Receivers4.IAcknowledgeable.Message
        {
            get { return new QS._core_c_.Base3.Message(channel, this); }
        }

        void QS._qss_c_.Receivers4.IAcknowledgeable.Acknowledged()
        {
            acknowledgementCallback(this);
        }

        void QS._qss_c_.Receivers4.IAcknowledgeable.Resend()
        {
            retransmissionCallback(this);
        }

        #endregion
    }
}
