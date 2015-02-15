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

namespace QS.Fx.Serialization
{
    /// <summary>
    /// This interface is implemented by classes that can be serialized or deserialized using the QuickSilver's custom serialization scheme.
    /// </summary>
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)]
    public interface ISerializable
    {
        /// <summary>
        /// This structure stores serialization-related information about the given object.
        /// </summary>
        SerializableInfo SerializableInfo
        {
            get;
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="header">Points to the preallocated header. Data can be appended to the header by copying it.</param>
        /// <param name="data">Points to a list of memory buffers. Data can be appended by adding buffer addresses to the list.</param>
        void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data);

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="header">Points to the portion of the received data that represents the header. Data can be copied from it.</param>
        /// <param name="data">Points to the portion of the received data that represents the flattened list of buffers that the sender
        /// appended to the buffer list during serialization. Data can be copied from it (the memory can be reused unless specifically locked
        /// during deserialization by increasing the reference count).</param>
        void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data);
    }
}
