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

namespace QS.Fx.Serialization
{
    /// <summary>
    /// This class represents serialization-related information about the serialized object.
    /// </summary>
    public struct SerializableInfo
    {
        /// <summary>
        /// Creates serialization-related information.
        /// </summary>
        /// <param name="classID">The unique class identifier.</param>
        public SerializableInfo(QS.ClassID classID) : this((ushort)classID, 0, 0, 0)
        {
        }

        /// <summary>
        /// Creates serialization-related information.
        /// </summary>
        /// <param name="headerSize">The number of bytes occupied by the serialized object in the preallocated header.</param>
        public SerializableInfo(int headerSize) : this(headerSize, headerSize, 0)
        {
        }

        /// <summary>
        /// Creates serialization-related information.
        /// </summary>
        /// <param name="headerSize">The number of bytes occupied by the serialized object in the preallocated header.</param>
        /// <param name="size">The total number of bytes occupied by the serialized object in the header and in the appended buffers.</param>
        /// <param name="numberOfBuffers">The number of buffers appended by the serialized obejct.</param>
        public SerializableInfo(int headerSize, int size, int numberOfBuffers)
        {
            this.ClassID = (ushort)QS.ClassID.Nothing;
            this.HeaderSize = headerSize;
            this.Size = size;
            this.NumberOfBuffers = numberOfBuffers;
        }

        /// <summary>
        /// Creates serialization-related information.
        /// </summary>
        /// <param name="classID">The unique class identifier.</param>
        /// <param name="headerSize">The number of bytes occupied by the serialized object in the preallocated header.</param>
        public SerializableInfo(ushort classID, int headerSize) : this(classID, headerSize, headerSize, 0)
        {
        }

        /// <summary>
        /// Creates serialization-related information.
        /// </summary>
        /// <param name="classID">The unique class identifier.</param>
        /// <param name="headerSize">The number of bytes occupied by the serialized object in the preallocated header.</param>
        /// <param name="size">The total number of bytes occupied by the serialized object in the header and in the appended buffers.</param>
        /// <param name="numberOfBuffers">The number of buffers appended by the serialized obejct.</param>
        public SerializableInfo(ushort classID, int headerSize, int size, int numberOfBuffers)
        {
            this.ClassID = classID;
            this.HeaderSize = headerSize;
            this.Size = size;
            this.NumberOfBuffers = numberOfBuffers;
        }

        /// <summary>
        /// The unique class identifier of the serialized object.
        /// </summary>
        public ushort ClassID;

        /// <summary>
        /// The number of buffers appended by the serialized obejct.
        /// </summary>
        public int NumberOfBuffers;
        
        /// <summary>
        /// The number of bytes occupied by the serialized object in the preallocated header.
        /// </summary>
        public int HeaderSize;
        
        /// <summary>
        /// The total number of bytes occupied by the serialized object in the header and in the appended buffers.
        /// </summary>
        public int Size;

        /// <summary>
        /// Updates the serialization information to account for additional data stored in the preallocated header.
        /// </summary>
        /// <param name="size">The number of additional bytes stored in the preallocated header.</param>
        public void ExtendHeader(int size)
        {
            this.HeaderSize += size;
            this.Size += size;
        }

        /// <summary>
        /// Updates the serialization information to account for additional data and a new unique class identifier.
        /// </summary>
        /// <param name="extensionClassID">The new unique class identifier.</param>
        /// <param name="increaseInHeaderSize">The number of additional bytes stored in the preallocated header.</param>
        /// <param name="increaseInDataSize">The number of additional bytes stored in the appended buffers.</param>
        /// <param name="increaseInNumberOfBuffers">Tje number of additional buffers to append.</param>
        /// <returns></returns>
        public SerializableInfo Extend(ushort extensionClassID, int increaseInHeaderSize,
            int increaseInDataSize, int increaseInNumberOfBuffers)
        {
            return new SerializableInfo(extensionClassID, (this.HeaderSize + increaseInHeaderSize),
                this.Size + increaseInHeaderSize + increaseInDataSize, this.NumberOfBuffers + increaseInNumberOfBuffers);
        }

        /// <summary>
        /// Returns the serialization information to account for additional data occupied by this and another object.
        /// </summary>
        /// <param name="other">The serialization information of another object.</param>
        /// <returns>The combined serialization information for this and another obejct.</returns>
        public SerializableInfo CombineWith(SerializableInfo other)
        {
            return new SerializableInfo(this.ClassID,
                (this.HeaderSize + other.HeaderSize), this.Size + other.Size, this.NumberOfBuffers + other.NumberOfBuffers);
        }

        /// <summary>
        /// Updates the serialization information to account for additional data occupied by another object.
        /// </summary>
        /// <param name="info">The serialization information of another object.</param>
        public void AddAnother(SerializableInfo info)
        {
            this.Size += info.Size;
            this.HeaderSize += info.HeaderSize;
            this.NumberOfBuffers += info.NumberOfBuffers;
        }

        public override string ToString()
        {
            return "ClassID = " + ClassID.ToString() + ", HeaderSize = " + HeaderSize.ToString() + ", Size = " + Size.ToString() +
                ", NumberOfBuffers = " + NumberOfBuffers.ToString();
        }
    }
}
