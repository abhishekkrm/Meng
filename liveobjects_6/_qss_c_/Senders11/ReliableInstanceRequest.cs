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

namespace QS._qss_c_.Senders11
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Base.Inspectable]
    public class ReliableInstanceRequest 
        : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, QS.Fx.Serialization.ISerializable, Receivers4.IAcknowledgeable, QS.Fx.Inspection.IInspectable
    {
        public delegate void Callback(ReliableInstanceRequest request);

        public ReliableInstanceRequest(QS._core_c_.Base3.InstanceID destinationAddress, uint channel, uint sequenceNo,
            QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message, QS._core_c_.Base6.CompletionCallback<object> transmissionCallback,
            Callback retransmissionCallback, Callback acknowledgementCallback) 
        {
            this.destinationAddress = destinationAddress;
            this.channel = channel;
            this.sequenceNo = sequenceNo;
            this.message = message;
            this.transmissionCallback = transmissionCallback;
            this.retransmissionCallback = retransmissionCallback;
            this.acknowledgementCallback = acknowledgementCallback;
        }

        [QS.Fx.Printing.Printable]
        protected QS._core_c_.Base3.InstanceID destinationAddress;
        [QS.Fx.Printing.Printable]
        protected uint channel, sequenceNo;
        protected QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message;
        protected QS._core_c_.Base6.CompletionCallback<object> transmissionCallback;
        protected Callback retransmissionCallback, acknowledgementCallback;
        protected QS.Fx.Clock.IAlarm alarm;
        [QS.Fx.Printing.Printable]
        protected bool completed, retransmitted;
        
        public double CreationTime;
        public int NumberOfRetransmissions;

        #region _debugging

/*
        [QS.Fx.Printing.Printable]
        private string _alarminfo
        {
            get { return alarm. }
        }
*/

        #endregion

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #region Accessors

        public uint SequenceNo
        {
            get { return sequenceNo; }
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

        public bool Retransmitted
        {
            get { return retransmitted; }
            set { retransmitted = value; }
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
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Senders11_InstanceMessage, sizeof(uint), sizeof(uint), 0);
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

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException("This type of obejct is not meant to be deserialized.");
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

        #region IInspectable Members

        [QS.Fx.Inspection.Ignore]
        private QS.Fx.Inspection.IAttributeCollection _attributes;
        [QS.Fx.Inspection.Ignore]
        QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
        {
            get 
            { 
                if (_attributes == null)
                    _attributes = new QS.Fx.Inspection.AttributesOf(this);
                return _attributes;
            }
        }

        #endregion
    }
}
