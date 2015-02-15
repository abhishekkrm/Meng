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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    public abstract class InSequence<Class> : Base3_.KnownClass, QS.Fx.Serialization.ISerializable where Class : QS.Fx.Serialization.ISerializable
    {
        public const int HeaderOverhead = sizeof(uint);
        public InSequence()
        {
        }

        public InSequence(Class dataObject)
        {
            this.dataObject = dataObject;
        }

        public InSequence(uint seqno, Class dataObject)
        {
            this.seqno = seqno;
            this.dataObject = dataObject;
        }

        private uint seqno;
        private Class dataObject;

        public uint SeqNo
        {
            get { return seqno; }
            set { seqno = value; }
        }

        public Class Object
        {
            get { return dataObject; }
        }

        #region QS.Fx.Serialization.ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = dataObject.SerializableInfo;
                return new QS.Fx.Serialization.SerializableInfo((ushort)this.ClassID, 
                    (ushort)(info.HeaderSize + sizeof(uint)), info.Size + sizeof(uint), info.NumberOfBuffers);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((uint*)(arrayptr + header.Offset)) = seqno;
            }
            header.consume(sizeof(uint));
            dataObject.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                seqno = *((uint*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(uint));
            dataObject.DeserializeFrom(ref header, ref data);
        }

        #endregion

		public override string ToString()
		{
			return GetType().Name + "(" + seqno.ToString() + ", " + QS._core_c_.Helpers.ToString.Object(dataObject) + ")";
		}
	}
}
