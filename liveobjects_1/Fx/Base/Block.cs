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
using System.Runtime.InteropServices;

namespace QS.Fx.Base
{
    /// <summary>
    /// This structure represents a single contiguous block of managed or unmanaged memory.
    /// </summary>
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses.Block)]
    public struct Block
    {
        /// <summary>
        /// The type of the memory block represented by this structure.
        /// </summary>
        [Flags]
        public enum Type : byte
        {
            /// <summary>
            /// The structure represents an invalid or uninitialized reference.
            /// </summary>
            None = 0x0, 
            /// <summary>
            /// The structure points to a managed memory location.
            /// </summary>
            Managed = 0x1, 
            /// <summary>
            /// The structure points to an unmanaged memory location.
            /// </summary>
            Unmanaged = 0x2, 
            /// <summary>
            /// The structure points to a managed location that has been pinned and for which a fixed physical address is provided
            /// </summary>
            Pinned = Managed | Unmanaged
        }

        /// <summary>
        /// Creates a reference to a managed byte array.
        /// </summary>
        /// <param name="buffer"></param>
        public Block(byte[] buffer) : this(Type.Managed, buffer, IntPtr.Zero, 0, (uint) buffer.Length)
        {
        }

        /// <summary>
        /// Creates a reference to a portion of a managed byte array.
        /// </summary>
        /// <param name="buffer">The address of the byte array.</param>
        /// <param name="offset">The place in the array where the data represented by this structure starts.</param>
        /// <param name="size">The size in bytes of the data represented by this structure.</param>
        public Block(byte[] buffer, uint offset, uint size) : this(Type.Managed, buffer, IntPtr.Zero, offset, size)
        {
        }

        /// <summary>
        /// Creates a reference to a portion of a managed byte array represented as an array segment.
        /// </summary>
        /// <param name="segment">The array segment representing the data.</param>
        public Block(ArraySegment<byte> segment) : this(Type.Managed, segment.Array, IntPtr.Zero, (uint) segment.Offset, (uint) segment.Count)
        {
        }

        /// <summary>
        /// Creates a reference to a block of unmanaged memory or managed memory that has been previously pinned.
        /// </summary>
        /// <param name="address">The physical unmanaged address of the segment of memory containing the data.</param>
        /// <param name="offset">The place in the memory segment where the data starts.</param>
        /// <param name="size">The size in bytes of the data represented by this structure.</param>
        public Block(IntPtr address, uint offset, uint size) : this(Type.Unmanaged, null, address, offset, size)
        {
        }

        /// <summary>
        /// Creates a reference to managed or unmanaged memory.
        /// </summary>
        /// <param name="type">The type of memory to reference.</param>
        /// <param name="buffer">Managed address of a memory segment.</param>
        /// <param name="address">Unmanaged address of a memory segment.</param>
        /// <param name="offset">Position with respect to the provided address of a memory segment where the data starts.</param>
        /// <param name="size">The size of the referenced data in bytes.</param>
        public Block(Type type, byte[] buffer, IntPtr address, uint offset, uint size)
        {
            this.type = type;
            this.buffer = buffer;
            this.address = address;
            this.offset = offset;
            this.size = size;
            this.control = null;
        }

        /// <summary>
        /// Pin the block of managed memory, return a GCHandle representing it, and update the reference with the unmanaged address.
        /// </summary>
        /// <param name="handle"></param>
        public void Pin(out GCHandle handle)
        {
            if (type == Type.Managed)
            {
                handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                address = handle.AddrOfPinnedObject();
                type = Type.Pinned;
            }
            else
                throw new Exception("Cannot pin this block, the type of the block is " + type.ToString() + ".");
        }

        /// <summary>
        /// The type of The type of memory to reference.
        /// </summary>
        public Type type;

        /// <summary>
        /// Managed address of a memory segment.
        /// </summary>
        public byte[] buffer;

        /// <summary>
        /// Unmanaged address of a memory segment.
        /// </summary>
        public IntPtr address;

        /// <summary>
        /// Position with respect to the provided address of a memory segment where the data starts.
        /// </summary>
        public uint offset;

        /// <summary>
        /// The size of the referenced data in bytes.
        /// </summary>
        public uint size;

        /// <summary>
        /// The structure that counts references to the memory block, used for explicit garbage collection.
        /// </summary>
        public IBlockControl control;
    }
}
