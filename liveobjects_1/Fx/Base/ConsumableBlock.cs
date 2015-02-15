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

namespace QS.Fx.Base
{
    /// <summary>
    /// The class represents a segment of memory that data can be appended to.
    /// </summary>
    public struct ConsumableBlock
    {
        /// <summary>
        /// Allocates a new segment of memory of a given size.
        /// </summary>
        /// <param name="size">The requested size of the memory segment.</param>
        public ConsumableBlock(uint size)
        {
            this.buffer = new Block(new byte[size], 0, size);
            this.offset = this.buffer.offset;
        }

        /// <summary>
        /// Creates a reference to an existing segment of managed memory.
        /// </summary>
        /// <param name="buffer">The segment of managed memory that data will be appended to.</param>
        public ConsumableBlock(byte[] buffer)
        {
            this.buffer = new Block(buffer);
            this.offset = this.buffer.offset;
        }

        /// <summary>
        /// Creates a reference to an existing segment of managed memory, with the specified starting position and size.
        /// </summary>
        /// <param name="buffer">The segment of managed memory that data will be appended to.</param>
        /// <param name="offset">The position at which data can be appended.</param>
        /// <param name="count">The number of bytes that can be stored at the given position.</param>
        public ConsumableBlock(byte[] buffer, uint offset, uint count)
        {
            this.buffer = new Block(buffer, offset, count);
            this.offset = this.buffer.offset;
        }

        /// <summary>
        /// Creates a reference to an existing segment of managed memory.
        /// </summary>
        /// <param name="buffer">The array segment representing the segment of memory where data can be appended.</param>
        public ConsumableBlock(ArraySegment<byte> buffer)
        {
            this.buffer = new Block(buffer);
            this.offset = this.buffer.offset;
        }

        /// <summary>
        /// Creates a reference to an existing segment of managed or unmanaged memory.
        /// </summary>
        /// <param name="buffer">A reference to a managed or unmanaged memory buffer where data can be appended.</param>
        /// <param name="extraoffset">The position with respect to the beginning of the buffer where data can be appended.</param>
        public ConsumableBlock(Block buffer, uint extraoffset)
        {
            this.buffer = buffer;
            this.buffer.offset += extraoffset;
            this.buffer.size -= extraoffset;
            this.offset = this.buffer.offset;
        }

        /// <summary>
        /// Creates a reference to an existing segment of managed or unmanaged memory.
        /// </summary>
        /// <param name="buffer">A reference to a managed or unmanaged memory buffer where data can be appended.</param>
        /// <param name="extraoffset">The position with respect to the beginning of the buffer where data can be appended.</param>
        /// <param name="fixedcount">The maximum number of bytes that can be appended.</param>
        public ConsumableBlock(Block buffer, uint extraoffset, uint fixedcount)
        {
            this.buffer = buffer;
            this.buffer.offset += extraoffset;
            this.buffer.size -= extraoffset;
            if (fixedcount < this.buffer.size)
                this.buffer.size = fixedcount;
            this.offset = this.buffer.offset;
        }

        private Block buffer;
        private uint offset;

        /// <summary>
        /// Returns the managed address of the memory segment if the segment is managed and throws and exception otherwise.
        /// </summary>
        public byte[] Array
        {
            get 
            {
                if ((buffer.type & Block.Type.Managed) == Block.Type.Managed && buffer.buffer != null)
                    return buffer.buffer;
                else
                    throw new Exception("Cannot return the array, the underlying buffer is not managed.");
            }
        }

        /// <summary>
        /// Returns the managed or unmanaged memory block that contains the data.
        /// </summary>
        public Block Block
        {
            get { return buffer; }
        }

        /// <summary>
        /// Returns the current offset within the memory block where new data can be appended.
        /// </summary>
        public int Offset
        {
            get { return (int) offset; }
        }

        /// <summary>
        /// Returns the number of bytes remaining to be appended to the memory block.
        /// </summary>
        public int Count
        {
            get { return (int) (this.buffer.size + this.buffer.offset - this.offset); }
        }

        /// <summary>
        /// Returns the number of bytes already appended to the memory block.
        /// </summary>
        public int Consumed
        {
            get { return (int) (this.offset - this.buffer.offset); }
        }

        /// <summary>
        /// Adjusts the offset and available space within the memory block to reflect the fact that a number of bytes have been appended to it.
        /// </summary>
        /// <param name="count">The number of bytes appended.</param>
        public void consume(int count)
        {
            this.offset += (uint) count;
        }

        /// <summary>
        /// Resets the memory block to its original size, allowing all existing data to be overwritten.
        /// </summary>
        public void reset()
        {
            this.offset = this.buffer.offset;
        }
    }
}
