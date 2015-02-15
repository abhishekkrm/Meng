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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Backbone_Node_Message)]
    public sealed class Incoming : QS.Fx.Serialization.ISerializable, IIncoming, IChannel
    {
        #region Constructors

        public Incoming()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable] private QS.Fx.Base.ID id1, id2;
        [QS.Fx.Printing.Printable] private ulong endpoint1, endpoint2;
        [QS.Fx.Printing.Printable] private QS.Fx.Serialization.ISerializable body;
        [QS.Fx.Printing.Printable] private uint number, cookie;
        [QS.Fx.Printing.Printable] private MessageOptions options;
                
        private ResponseCallback responsecallback;

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
        }

        public ulong Endpoint1
        {
            get { return endpoint1; }
        }

        public QS.Fx.Base.ID ID2
        {
            get { return id2; }
        }

        public ulong Endpoint2
        {
            get { return endpoint2; }
        }

        public uint Number
        {
            get { return number; }
        }

        public uint Cookie
        {
            get { return cookie; }
        }

        public QS.Fx.Serialization.ISerializable Body
        {
            get { return body; }
        }

        public MessageOptions Options
        {
            get { return options; }
        }

        public ResponseCallback ResponseCallback
        {
            get { return responsecallback; }
            set { responsecallback = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Backbone_Node_Message, 0); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            throw new NotSupportedException("Messages are not supposed to be serialized: register class \"Request\" for this purpose.");
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            id1 = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) id1).DeserializeFrom(ref header, ref data);
            endpoint1 = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt64(ref header, ref data);
            id2 = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) id2).DeserializeFrom(ref header, ref data);
            endpoint2 = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt64(ref header, ref data);
            number = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            cookie = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            options = (MessageOptions) QS._qss_c_.Base3_.SerializationHelper.Deserialize_Byte(ref header, ref data);
            ushort classid = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
            body = QS._core_c_.Base3.Serializer.CreateObject(classid);
            body.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IIncoming Members

        QS.Fx.Serialization.ISerializable IIncoming.Message
        {
            get { return body; }
        }

        bool IIncoming.Respond
        {
            get { return (options & MessageOptions.Respond) == MessageOptions.Respond; }
        }

        IChannel IIncoming.ResponseChannel
        {
            get { return this; }
        }

        #endregion

        #region IChannel Members

        IOutgoing IChannel.Submit(QS.Fx.Serialization.ISerializable response, MessageOptions options, 
            QS.Fx.Base.ContextCallback<IOutgoing> callback, object context)
        {
            if ((this.options & MessageOptions.Respond) == MessageOptions.Respond)
            {
                if (responsecallback != null)
                    return responsecallback(this, response, options, callback, context);
                else
                    throw new Exception("Response cannot be delivered, response callback was not set.");
            }
            else
                throw new Exception("Response was not expected, message options are : " + this.options.ToString() + ".");
        }

        #endregion
    }
}
