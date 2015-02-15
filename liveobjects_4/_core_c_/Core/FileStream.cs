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

namespace QS._core_c_.Core
{
    public abstract class FileStream : System.IO.Stream
    {
        public FileStream()
        {
        }

        public override bool CanRead
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool CanSeek
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool CanWrite
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override void Flush()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override long Length
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override long Position
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool CanTimeout
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void Dispose(bool disposing)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int ReadByte()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteByte(byte value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int WriteTimeout
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }

            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override int ReadTimeout
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }

            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
}
