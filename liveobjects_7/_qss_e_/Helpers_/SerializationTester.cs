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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_e_.Helpers_
{
    public static class SerializationTester
    {
        public static void TestObject<T>(QS.Fx.Logging.ILogger logger, T someObject) where T : QS.Fx.Serialization.ISerializable, new()
        {
            logger.Log("\n\nTesting Object\n\n" + someObject.ToString() + "\n\n");

            QS.Fx.Serialization.SerializableInfo info = someObject.SerializableInfo;
            logger.Log("SerializableInfo : " + info.ToString());

            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) (info.HeaderSize + 1000));
            System.Collections.Generic.IList<QS.Fx.Base.Block> buffers = new List<QS.Fx.Base.Block>(info.NumberOfBuffers);

            someObject.SerializeTo(ref header, ref buffers);

            int dataSize = 0;
            foreach (QS.Fx.Base.Block segment in buffers)
                dataSize += (int) segment.size;

            logger.Log("Consumed\t\t\t   HeaderSize = " + header.Offset.ToString() + ", Size = " + (header.Offset + dataSize).ToString() +
                ", NumberOfBuffers = " + buffers.Count.ToString() + "\n");

            if (header.Offset != info.HeaderSize || dataSize != (info.Size - info.HeaderSize) || buffers.Count != info.NumberOfBuffers)
                throw new Exception("Amount of resources consumed during serialization differs from the amount declared in SerializableInfo.");

            header.consume(-header.Offset);

            QS.Fx.Base.ConsumableBlock newdata = new QS.Fx.Base.ConsumableBlock((uint)(dataSize + 1000));
            foreach (QS.Fx.Base.Block segment in buffers)
            {
                if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                    Buffer.BlockCopy(segment.buffer, (int) segment.offset, newdata.Array, newdata.Offset, (int) segment.size);
                else
                    throw new Exception("Unmanaged memory is not supported here.");
                newdata.consume((int) segment.size);
            }

            newdata.consume(-dataSize);

            T anotherObject = new T();

            anotherObject.DeserializeFrom(ref header, ref newdata);

            logger.Log("After deserialization:\n\n" + anotherObject.ToString() + "\n\n");

            logger.Log("Consumed\t\t\t   HeaderSize = " + header.Offset.ToString() + ", Size = " + (header.Offset + newdata.Offset).ToString() + "\n");

            if (header.Offset != info.HeaderSize || newdata.Offset != dataSize)
                throw new Exception("Amount of resources consumed during deserialization differs from that generated during serialization.");

            logger.Log("Succeeded.\n\n");
        }
    }
}
