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

namespace QS._qss_x_.Backbone_.Node
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Outgoing : IOutgoing, QS.Fx.Serialization.ISerializable, QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>
    {
        #region Constructors

        public Outgoing(QS.Fx.Base.ID id1, ulong endpoint1, QS.Fx.Base.ID id2, ulong endpoint2, uint number, uint cookie,
            QS.Fx.Serialization.ISerializable body, MessageOptions options, QS.Fx.Base.ContextCallback<IOutgoing> callback, object context)
        {
            this.id1 = id1;
            this.endpoint1 = endpoint1;
            this.id2 = id2;
            this.endpoint2 = endpoint2;
            this.number = number;
            this.cookie = cookie;
            this.body = body;
            this.options = options;
            this.callback = callback;
            this.context = context;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable] private QS.Fx.Base.ID id1, id2;
        [QS.Fx.Printing.Printable] private ulong endpoint1, endpoint2;
        [QS.Fx.Printing.Printable] private uint number, cookie;
        [QS.Fx.Printing.Printable] private QS.Fx.Serialization.ISerializable body;
        [QS.Fx.Printing.Printable] private MessageOptions options;
        [QS.Fx.Printing.Printable] private MessageState state = MessageState.Pending;
        [QS.Fx.Printing.Printable] private bool retransmitted;
        [QS.Fx.Printing.Printable] private Incoming response;

        private QS.Fx.Base.ContextCallback<IOutgoing> callback;
        private object context;
        private QS.Fx.Clock.IAlarm alarm;

        #endregion

        #region Overrides from System.Object

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region Accessors

        public QS.Fx.Base.ID ID1
        {
            get { return id1; }
            set { id1 = value; }
        }

        public ulong Endpoint1
        {
            get { return endpoint1; }
            set { endpoint1 = value; }
        }

        public QS.Fx.Base.ID ID2
        {
            get { return id2; }
            set { id2 = value; }
        }

        public ulong Endpoint2
        {
            get { return endpoint2; }
            set { endpoint2 = value; }
        }

        public uint Number
        {
            get { return number; }
            set { number = value; }
        }

        public uint Cookie
        {
            get { return cookie; }
            set { cookie = value; }
        }

        public QS.Fx.Serialization.ISerializable Body
        {
            get { return body; }
            set { body = value; }
        }

        public Incoming Response
        {
            get { return response; }
            set { response = value; }
        }
        
        public MessageOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        public QS.Fx.Base.ContextCallback<IOutgoing> Callback
        {
            get { return callback; }
            set { callback = value; }
        }

        public object Context
        {
            get { return context; }
            set { context = value; }
        }

        public MessageState State
        {
            get { return state; }
            set { state = value; }
        }

        public QS.Fx.Clock.IAlarm Alarm
        {
            get { return alarm; }
            set { alarm = value; }
        }

        public bool Retransmitted
        {
            get { return retransmitted; }
            set { retransmitted = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Backbone_Node_Message, 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)id1).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt64(ref info);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)id2).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt64(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Byte(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                info.AddAnother(body.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)id1).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt64(ref header, ref data, endpoint1);
            ((QS.Fx.Serialization.ISerializable)id2).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt64(ref header, ref data, endpoint2);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, number);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, cookie);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Byte(ref header, ref data, (byte) options);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, body.SerializableInfo.ClassID);
            body.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException("Requests are not supposed to be deserialized: register class \"Message\" for this purpose.");
        }

        #endregion

        #region IRequest Members

        QS.Fx.Serialization.ISerializable IOutgoing.Message
        {
            get { return body; }
        }

        MessageOptions IOutgoing.Options
        {
            get { return options; }
        }

        object IOutgoing.Context
        {
            get { return context; }
        }

        MessageState IOutgoing.State
        {
            get { return state; }
        }

        IIncoming IOutgoing.Response
        {
            get { return response; }
        }

        #endregion

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return new QS._core_c_.Base3.Message((uint) ReservedObjectID.Fx_Backbone_Node, this); }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return null; }
        }

        QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return null; }
        }

        #endregion
    }
}
