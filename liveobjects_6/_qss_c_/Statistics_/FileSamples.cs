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

namespace QS._qss_c_.Statistics_
{
    public sealed class FileSamples : IDisposable
    {
        public FileSamples(QS._core_c_.Core.IFile outputFile, int batchsize)
        {
            this.outputFile = outputFile;
            this.batchsize = batchsize;
        }

        private QS._core_c_.Core.IFile outputFile;
        private long offset;
        private int batchsize;
        private Block block;

        #region Class Block

        private class Block
        {
            public Block(int nsamples)
            {
                this.nsamples = nsamples;
                this.samples = new double[nsamples];
                this.bufsize = sizeof(double) * nsamples;
            }

            public int nsamples, nwritten, bufsize;
            public double[] samples;
            public GCHandle gchandle;
            public IntPtr address;

            public void Pin()
            {
                gchandle = GCHandle.Alloc(samples, GCHandleType.Pinned);
                address = gchandle.AddrOfPinnedObject();
            }

            public void Unpin()
            {
                address = IntPtr.Zero;
                gchandle.Free();
                samples = null;
            }
        }

        #endregion

        #region Adding

        public void Add(double sample)
        {
            if (block == null)
            {
                block = new Block(batchsize);
            }

            block.samples[block.nwritten++] = sample;

            if (block.nwritten == block.nsamples)
            {
                block.Pin();
                outputFile.Write(offset, block.address, block.bufsize, new QS.Fx.Base.IOCompletionCallback(this.WriteCallback), block);
                offset += block.bufsize;
                block = null;
            }
        }

        #endregion

        #region Callbacks

        private void WriteCallback(bool succeeded, uint ntransmitted, Exception errror, object context)
        {
            ((Block)context).Unpin();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (block != null)
            {
                int actualsize = block.nwritten * sizeof(double);

                block.Pin();
                outputFile.Write(offset, block.address, actualsize, new QS.Fx.Base.IOCompletionCallback(this.WriteCallback), block);
                block = null;
                offset += actualsize;
            }
        }

        #endregion
    }
}
