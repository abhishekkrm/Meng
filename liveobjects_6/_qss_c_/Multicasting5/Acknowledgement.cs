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

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting5_Acknowledgement)]
    public class Acknowledgement : QS.Fx.Serialization.ISerializable
    {
        public Acknowledgement()
        {
        }

        public Acknowledgement(Base3_.GroupID groupID, uint viewSeqNo, uint messageSeqNo)
        {
            this.groupID = groupID;
            this.viewSeqNo = viewSeqNo;
            this.messageSeqNo = messageSeqNo;
        }

        private Base3_.GroupID groupID;
        private uint viewSeqNo, messageSeqNo;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public uint ViewSeqNo
        {
            get { return viewSeqNo; }
        }

        public uint MessageSeqNo
        {
            get { return messageSeqNo; }
        }

        #endregion

      #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                return groupID.SerializableInfo.Extend((ushort)QS.ClassID.Multicasting5_Acknowledgement, 2 * sizeof(uint), 0, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            groupID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((uint*)headerptr) = viewSeqNo;
                *((uint*)(headerptr + sizeof(uint))) = messageSeqNo;
            }
            header.consume(2 * sizeof(uint));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            groupID = new QS._qss_c_.Base3_.GroupID();
            groupID.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                viewSeqNo = *((uint*)headerptr);
                messageSeqNo = *((uint*)(headerptr + sizeof(uint)));
            }
            header.consume(2 * sizeof(uint));
        }

        #endregion
    }
}
