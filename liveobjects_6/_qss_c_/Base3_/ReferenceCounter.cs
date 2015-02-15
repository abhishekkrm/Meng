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

namespace QS._qss_c_.Base3_
{
    public sealed class ReferenceCounter<C> where C : class
    {
        public ReferenceCounter(C target, QS.Fx.Base.ContextCallback callback, object context)
        {
            if (target == null)
                throw new Exception("Target cannot be null.");
            if (callback == null)
                throw new Exception("Callback cannot be null.");

            this.target = target;
            this.callback = callback;
            this.context = context;
        }

        private int nreferences;
        private C target;
        private bool disposed;
        private QS.Fx.Base.ContextCallback callback;        
        private object context;

        public bool Disposed
        {
            get { return disposed; }
        }

        /// <summary>
        /// Warning: must hold a lock on the target before calling this method.
        /// </summary>
        /// <returns></returns>
        public IDisposableRef<C> GetReference()
        {
            nreferences++;
            return new DisposableRef<C>(target, new QS.Fx.Base.ContextCallback(this.ReleaseCallback), null);
        }

        private void ReleaseCallback(object context)
        {
            bool disposed_now = false;
            lock (target)
            {
                if (disposed)
                    throw new Exception("Internal error: release callback called for an already disposed reference counter.");
                if (nreferences == 0)
                    throw new Exception("Internal error: release callback called for a reference counter without references.");
                
                nreferences--;
                if (nreferences == 0)
                    disposed = disposed_now = true;
            }

            if (disposed_now)
                callback(this.context);
        }
    }
}
