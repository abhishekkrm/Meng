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

namespace QS._qss_c_.Rings4
{
    [QS.Fx.Base.Inspectable]
    public abstract class RequestRV : Base3_.AsynchronousOperation, QS.Fx.Serialization.ISerializable, QS.Fx.Inspection.IInspectable
    {
        public RequestRV(Base3_.RVID regionViewID, QS._core_c_.Base3.Message message,
            Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            : base(completionCallback, asynchronousState)
        {
            this.regionViewID = regionViewID;
            this.message = message;
        }

        private Base3_.RVID regionViewID;
        private QS._core_c_.Base3.Message message;
        private uint seqno;
        private QS.Fx.Clock.IAlarm retransmissionAlarm;

        #region Cleanup

        public override void Unregister()
        {
            lock (this)
            {
                if (retransmissionAlarm != null)
                    retransmissionAlarm.Cancel();
                retransmissionAlarm = null;
            }
        }

        #endregion

        #region Accessors

        public QS.Fx.Clock.IAlarm RetransmissionAlarm
        {
            get { return retransmissionAlarm; }
            set { retransmissionAlarm = value; }
        }

        public QS._core_c_.Base3.Message Message
        {
            get { return message; }
        }

        public uint SeqNo
        {
            get { return seqno; }
            set { seqno = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return regionViewID.SerializableInfo.CombineWith(message.SerializableInfo).Extend(
                    (ushort)ClassID.Multicasting5_MessageRV, (ushort)(sizeof(uint)), 0, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            regionViewID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((uint*)headerptr) = seqno;
            }
            header.consume(sizeof(uint));
            message.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Printing

        public override string ToString()
        {
            return "(" + regionViewID.ToString() + ":" + seqno.ToString() + " " + message.ToString() + ")";
        }

        #endregion

        #region IInspectable Members

        private QS.Fx.Inspection.IAttributeCollection attributeCollection;
        QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
        {
            get
            {
                lock (this)
                {
                    if (attributeCollection == null)
                        attributeCollection = new QS.Fx.Inspection.AttributesOf(this);
                    return attributeCollection;
                }
            }
        }

        #endregion
    }
}
