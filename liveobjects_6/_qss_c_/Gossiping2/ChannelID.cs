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

namespace QS._qss_c_.Gossiping2
{
    [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_ChannelID)]
    public class ChannelID : QS.Fx.Serialization.ISerializable, IComparable<ChannelID>
    {
        public enum ReservedIDs : ushort
        {
            Base3_RootSender_ReceiveRate                                                                     = 1000
        }

        public ChannelID()
        {
        }

        public ChannelID(ReservedIDs reservedID) : this((ushort) reservedID)
        {
        }

        public ChannelID(ushort reservedID)
        {
            this.reservedID = reservedID;
        }

        private ushort reservedID;

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Gossiping2_ChannelID, (ushort)sizeof(ushort), sizeof(ushort), 0); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = reservedID;
            }
            header.consume(sizeof(ushort));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                reservedID = * ((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
        }

        #endregion

        #region IComparable<ChannelID> Members

        int IComparable<ChannelID>.CompareTo(ChannelID other)
        {
            return reservedID.CompareTo(other.reservedID);
        }

        #endregion

        public override string ToString()
        {
            return reservedID.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj is ChannelID) && (reservedID == ((ChannelID)obj).reservedID);
        }

        public override int GetHashCode()
        {
            return reservedID.GetHashCode();
        }
    }
}
