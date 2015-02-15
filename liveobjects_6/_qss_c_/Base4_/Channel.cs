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

namespace QS._qss_c_.Base4_
{
    public class Channel<C> : IChannel, ISource<C>
    {
        public Channel(SourceCallback<C> sourceCallback, GetObjectsCallback<C> getObjectsCallback)
        {
            this.sourceCallback = sourceCallback;
            this.getObjectsCallback = getObjectsCallback;
        }

        private SourceCallback<C> sourceCallback;
        private GetObjectsCallback<C> getObjectsCallback;
        private bool signaled = false;

        #region ISource<C> Members

        bool ISource<C>.Ready
        {
            get { return signaled; }
        }

        bool ISource<C>.GetObjects(ref Queue<C> returnedObjects, uint maximumSize)
        {
            if (getObjectsCallback(ref returnedObjects, maximumSize))
                return true;
            else
            {
                lock (this)
                {
                    if (getObjectsCallback(ref returnedObjects, maximumSize))
                        return true;
                    else
                    {
                        signaled = false;
                        return false;
                    }
                }
            }
        }

        #endregion

        #region IChannel Members

        void IChannel.Signal()
        {
            bool signaled_now;
            lock (this)
            {
                signaled_now = !signaled;
                signaled = true;
            }

            if (signaled_now)
                sourceCallback(this);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
