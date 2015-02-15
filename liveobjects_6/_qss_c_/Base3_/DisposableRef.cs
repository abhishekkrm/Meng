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
using System.Threading;

namespace QS._qss_c_.Base3_
{
    /// <summary>
    /// A disposable reference to an object, used for reference counting. Methods of this class are thread-safe and nonblocking.
    /// </summary>
    /// <typeparam name="C">Class of the object that the reference will point to.</typeparam>
    public sealed class DisposableRef<C> : IDisposableRef<C> where C : class
    {
        /// <summary>
        /// Creates a disposable reference.
        /// </summary>
        /// <param name="target">The target object that this reference is pointing to.</param>
        /// <param name="callback">Callback to invoke when this reference is disposed.</param>
        /// <param name="context">Context object that will be passed to the callback. </param>
        public DisposableRef(C target, QS.Fx.Base.ContextCallback callback, object context)
        {
            if (target == null)
                throw new Exception("The target object given as argument is null; it does not make sense to create a disposable reference for a null object.");
            if (callback == null)
                throw new Exception("Callback given as argument is null; cannot use disposable a reference without a callback.");

            this.target = target;
            this.callback = callback;
            this.context = context;
        }

        private int disposed;
        private C target;
        private QS.Fx.Base.ContextCallback callback;
        private object context;

        #region IDisposableRef<C> Members

        /// <summary>
        /// Returns the target object that this reference is pointing to. Reading this property is nonblocking. 
        /// This property cannot be used after the object has been disposed. It is legal to read this property from the callback. 
        /// </summary>
        public C Target
        {
            get 
            {
                C mytarget = target;
                
                if (disposed > 0 || mytarget == null)
                    throw new Exception("Cannot use this reference, it has already been disposed.");

                return mytarget;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the reference and invokes the callback given in the constructor. This call is nonblocking. The callback can access the target object.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 1)
                throw new Exception("This reference has already been disposed; must not call Dispose twice on the same reference.");

            if (target == null)
                throw new Exception("Internal error: the target reference is already null even though disposing has not yet completed.");

            callback(context);

            // in case some dumb programmer keeps this unused reference around
            target = null;
            callback = null;
            context = null;
        }

        #endregion
    }
}
