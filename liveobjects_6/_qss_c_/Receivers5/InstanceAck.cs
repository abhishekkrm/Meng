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

namespace QS._qss_c_.Receivers5
{
    [QS.Fx.Serialization.ClassID(ClassID.Receivers5_InstanceAck)]
    public class InstanceAck : QS.Fx.Serialization.ISerializable, QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>
    {
        public InstanceAck(QS._core_c_.Base3.InstanceID destinationAddress, IList<Base1_.Range<uint>> acknowledgementRanges, uint ackChannel)
        {
            this.ackChannel = ackChannel;
            this.destinationAddress = destinationAddress;
            this.ranges = new SeqNoRanges(acknowledgementRanges);
        }

        public InstanceAck()
        {
        }

        private uint ackChannel;
        private QS._core_c_.Base3.InstanceID destinationAddress;
        private SeqNoRanges ranges;

        public QS._core_c_.Base3.InstanceID DestinationAddress
        {
            get { return destinationAddress; }
        }

        public IList<Base1_.Range<uint>> Acks
        {
            get { return ranges.Ranges; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Receivers5_InstanceAck, 0, 0, 0);
                info.AddAnother(ranges.SerializableInfo);
                info.AddAnother(destinationAddress.SerializableInfo);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ranges.SerializeTo(ref header, ref data);
            destinationAddress.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ranges = new SeqNoRanges();
            ranges.DeserializeFrom(ref header, ref data);
            destinationAddress = new QS._core_c_.Base3.InstanceID();
            destinationAddress.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "InstanceAck(" + ranges.ToString() + ")";
        }

        #endregion

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return new QS._core_c_.Base3.Message(ackChannel, this); }
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
