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

namespace QS._qss_c_.Batching_
{
    public class Unpacker
    {
        public Unpacker(Base3_.IDemultiplexer demultiplexer)
        {
            this.demultiplexer = demultiplexer;
            demultiplexer.register((uint)ReservedObjectID.Batching_Unpacker, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
        }

        private Base3_.IDemultiplexer demultiplexer;

        #region Class Request

        [QS.Fx.Serialization.ClassID(ClassID.Batching_Unpacker_Request)]
        public class Request : QS.Fx.Serialization.ISerializable
        {
            public Request()
            {
            }

            private IList<QS._core_c_.Base3.Message> messages;

            public IList<QS._core_c_.Base3.Message> Messages
            {
                get { return messages; }
            }

            #region ISerializable Members

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                int nelements;
                fixed (byte* parray = header.Array)
                {
                    byte* pheader = parray + header.Offset;
                    nelements = (int)(*((ushort*)pheader));
                    header.consume(sizeof(ushort));

                    messages = new List<QS._core_c_.Base3.Message>(nelements);

                    while (nelements-- > 0)
                    {
                        uint destinationLOID;
                        ushort classid;

                        pheader = parray + header.Offset;
                        destinationLOID = *((uint*)pheader);
                        classid = *((ushort*)(pheader + sizeof(uint)));
                        header.consume(sizeof(uint) + sizeof(ushort));

                        QS.Fx.Serialization.ISerializable deserialized_obj = QS._core_c_.Base3.Serializer.CreateObject(classid);
                        deserialized_obj.DeserializeFrom(ref header, ref data);

                        messages.Add(new QS._core_c_.Base3.Message(destinationLOID, deserialized_obj));
                    }
                }
            }

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get { throw new NotSupportedException(); }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable obj)
        {
            Request req = obj as Request;
            if (req != null)
            {
                foreach (QS._core_c_.Base3.Message message in req.Messages)
                    demultiplexer.dispatch(message.destinationLOID, address, message.transmittedObject);
            }
            else
            {
                BatchingSink.Request req2 = obj as BatchingSink.Request;
                if (req2 != null)
                {
                    foreach (BatchingSink.Chunk chunk in req2.chunks)
                    {
                        QS._core_c_.Base3.Message message = chunk.request.Argument;
                        demultiplexer.dispatch(message.destinationLOID, address, message.transmittedObject);
                    }
                }
                else
                    throw new Exception("Unpacker.ReceiveCallback expected Unpacker.Request or BatchingSink.Request, but found " +
                        ((obj != null) ? (obj.GetType().ToString()) : ("(null)")) + ".");
            }
                
            return null;
        }

        #endregion
    }
}
