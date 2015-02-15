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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Multicasting5_Message)]
    public class Message : QS.Fx.Serialization.ISerializable
    {
        public Message()
        {
        }

        public Message(Base3_.GroupID groupID, uint viewSeqNo, uint messageSeqNo, uint destinationLOID, QS.Fx.Serialization.ISerializable transmittedObject)
        {
            this.groupID = groupID;
            this.viewSeqNo = viewSeqNo;
            this.messageSeqNo = messageSeqNo;
            this.destinationLOID = destinationLOID;
            this.transmittedObject = transmittedObject;
        }

        private Base3_.GroupID groupID;
        private uint viewSeqNo, messageSeqNo, destinationLOID;
        private QS.Fx.Serialization.ISerializable transmittedObject;

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

        public uint DestinationLOID
        {
            get { return destinationLOID; }
        }

        public QS.Fx.Serialization.ISerializable TransmittedObject
        {
            get { return transmittedObject; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                return transmittedObject.SerializableInfo.CombineWith(groupID.SerializableInfo).Extend(
                    (ushort)QS.ClassID.Multicasting5_Message, 3 * sizeof(uint) + sizeof(ushort), 0, 0);
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
                *((uint*)(headerptr + 2 * sizeof(uint))) = destinationLOID;
                *((ushort*)(headerptr + 3 * sizeof(uint))) = transmittedObject.SerializableInfo.ClassID;
            }
            header.consume(3 * sizeof(uint) + sizeof(ushort));
            transmittedObject.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            groupID = new QS._qss_c_.Base3_.GroupID();
            groupID.DeserializeFrom(ref header, ref data);
            ushort classID;
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                viewSeqNo = *((uint*)headerptr);
                messageSeqNo = *((uint*)(headerptr + sizeof(uint)));
                destinationLOID = *((uint*)(headerptr + 2 * sizeof(uint)));
                classID = *((ushort*)(headerptr + 3 * sizeof(uint)));
            }
            header.consume(3 * sizeof(uint) + sizeof(ushort));
            transmittedObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
            transmittedObject.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public Acknowledgement AcknowledgementTo
        {
            get { return new Acknowledgement(groupID, viewSeqNo, messageSeqNo); }
        }
    }
}
