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

#define DEBUG_CheckSerializedSize

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_c_.Base3_
{
    public static class Root
    {
        public const int HeaderOverhead = 4 * sizeof(uint) + sizeof(ushort);

        public static unsafe void Encode(QS._core_c_.Base3.InstanceID localAddress, uint channel, QS.Fx.Serialization.ISerializable dataObject,
            out IList<QS.Fx.Base.Block> buffers, out uint transmittedSize)
        {
            QS.Fx.Serialization.SerializableInfo info = dataObject.SerializableInfo;

            transmittedSize = (uint) (info.Size + (4 * sizeof(uint) + sizeof(ushort)));

            QS.Fx.Base.ConsumableBlock header =
                new QS.Fx.Base.ConsumableBlock((uint) (info.HeaderSize + (4 * sizeof(uint) + sizeof(ushort))));
            buffers = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
            buffers.Add(header.Block);

            fixed (byte* headerptr = header.Array)
            {
                *((uint*)headerptr) = (uint) localAddress.Address.PortNumber;
                *((uint*)(headerptr + sizeof(uint))) = channel;
                *((ushort*)(headerptr + 2 * sizeof(uint))) = info.ClassID;
                *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort))) = (uint) info.HeaderSize;
                *((uint*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))) = localAddress.Incarnation.SeqNo;
            }
            header.consume(4 * sizeof(uint) + sizeof(ushort));

            dataObject.SerializeTo(ref header, ref buffers);

#if DEBUG_CheckSerializedSize
            try
            {
                uint actualHeaderSize = (uint)((int) buffers[0].size - (4 * sizeof(uint) + sizeof(ushort))), actualBuffersSize = 0;
                for (int ind = 1; ind < buffers.Count; ind++)
                    actualBuffersSize += buffers[ind].size;
                if (actualHeaderSize != info.HeaderSize)
                    throw new Exception("Bad header size, expected " + info.HeaderSize.ToString() + ", but actual " + actualHeaderSize.ToString() + ".");
                if (actualBuffersSize != info.Size - info.HeaderSize)
                    throw new Exception("Bad buffers size, expected " + (info.Size - info.HeaderSize).ToString() +
                        ", but actual " + actualBuffersSize.ToString() + ".");
                if (buffers.Count - 1 != info.NumberOfBuffers)
                    throw new Exception("Bad buffer count, expected " + info.NumberOfBuffers.ToString() +
                        ", but actual " + (buffers.Count - 1).ToString() + ".");
            }
            catch (Exception exc)
            {
                throw new Exception("Error serializing object of type ( " +
                    Helpers_.Enum.ToString<QS.ClassID, ushort>(dataObject.SerializableInfo.ClassID) + 
                    " ), class " + dataObject.GetType().ToString() + ".", exc);
            }
#endif
        }

        public static unsafe void Decode(IPAddress interfaceAddress, QS.Fx.Base.Block packet, 
            out QS._core_c_.Base3.InstanceID senderAddress, out uint channel, out QS.Fx.Serialization.ISerializable receivedObject)
        {
            System.Diagnostics.Debug.Assert(packet.buffer != null && 
                (packet.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed,
                "Unmanaged memory not supported here.");

            uint responsePortNo, headerSize;
            ushort classID;
            QS._core_c_.Base3.Incarnation incarnation;

            fixed (byte* packetptr = packet.buffer)
            {
                byte* headerptr = packetptr + (int) packet.offset;
                responsePortNo = *((uint*)headerptr);
                channel = *((uint*)(headerptr + sizeof(uint)));
                classID = *((ushort*)(headerptr + 2 * sizeof(uint)));
                headerSize = *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort)));
                incarnation = new QS._core_c_.Base3.Incarnation(*((uint*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))));
            }

            receivedObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
            
            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(packet, (uint) (4 * sizeof(uint) + sizeof(ushort)), headerSize);
            QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(packet, (uint)(4 * sizeof(uint) + sizeof(ushort)) + headerSize);

            receivedObject.DeserializeFrom(ref header, ref data);

            senderAddress = new QS._core_c_.Base3.InstanceID(
                new QS.Fx.Network.NetworkAddress(interfaceAddress, (int) responsePortNo), incarnation);
        }
    }
}
