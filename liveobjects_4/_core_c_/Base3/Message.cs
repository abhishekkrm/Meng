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

namespace QS._core_c_.Base3
{
    public struct Message : QS.Fx.Serialization.ISerializable
    {
        public const int HeaderOverhead = sizeof(ushort) + sizeof(uint);

        #region Class Wrapper

        [QS.Fx.Serialization.ClassID(ClassID.Base3_Message)]
        public class Wrapper : SerializableWrapper<Message>
        {
            public Wrapper()
            {
            }

            public Wrapper(Message message) : base(message)
            {                
            }
        }

        #endregion

        public Wrapper Wrap
        {
            get { return new Wrapper(this); }
        }

        public Message(uint destinationLOID, QS.Fx.Serialization.ISerializable transmittedObject)
        {
            this.destinationLOID = destinationLOID;
            this.transmittedObject = transmittedObject;
        }

        public uint destinationLOID;
        public QS.Fx.Serialization.ISerializable transmittedObject;

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = transmittedObject.SerializableInfo;
                return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Base3_Message, (ushort)(info.HeaderSize + sizeof(ushort) + sizeof(uint)),
                    info.Size + sizeof(ushort) + sizeof(uint), info.NumberOfBuffers);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte *headerptr = arrayptr + header.Offset;
                *((ushort*)headerptr) = (ushort)transmittedObject.SerializableInfo.ClassID;
                *((uint*)(headerptr + sizeof(ushort))) = destinationLOID;
            }
            header.consume(sizeof(ushort) + sizeof(uint));
            transmittedObject.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID;
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                classID = *((ushort*)headerptr);
                destinationLOID = *((uint*)(headerptr + sizeof(ushort)));
            }            
            header.consume(sizeof(ushort) + sizeof(uint));
            transmittedObject = Serializer.CreateObject(classID);
            transmittedObject.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            return "(LOID: " + destinationLOID.ToString() + ", Object: " + transmittedObject.ToString() + ")";
        }
    }
}
