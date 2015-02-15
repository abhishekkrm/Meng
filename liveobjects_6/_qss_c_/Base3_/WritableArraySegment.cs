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
    public struct WritableArraySegment<T>
    {
        public WritableArraySegment(int size) : this(new ArraySegment<T>(new T[size], 0, size))
        {
        }

        public WritableArraySegment(T[] buffer) : this(new ArraySegment<T>(buffer))
        {
        }

        public WritableArraySegment(T[] buffer, int offset, int count) : this(new ArraySegment<T>(buffer, offset, count))
        {
        }

        public WritableArraySegment(ArraySegment<T> buffer)
        {
            this.buffer = buffer;
            offset = buffer.Offset;
        }

        private ArraySegment<T> buffer;
        private int offset;

        public ArraySegment<T> ArraySegment
        {
            get
            {
                return buffer;
            }
        }

        public T[] Array
        {
            get { return buffer.Array; }
        }

        public int Offset 
        {
            get { return offset; }
        }

        public int Count
        {
            get { return buffer.Count + buffer.Offset - offset; }
        }

        public int Consumed
        {
            get { return offset - buffer.Offset; }
        }

        public void consume(int count)
        {
            this.offset += count;
        }

        public void reset()
        {
            this.offset = buffer.Offset; // 0;
        }
    }
}
