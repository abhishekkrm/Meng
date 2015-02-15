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

namespace QS._qss_c_.Infrastructure2.Embeddings.ReplicatedObjects
{
    public sealed class Request : QS.Fx.Serialization.ISerializable
    {
        public Request(System.Runtime.Remoting.Messaging.IMethodCallMessage methodCallMessage)
        {
            this.methodCallMessage = methodCallMessage;
        }

        private System.Runtime.Remoting.Messaging.IMethodCallMessage methodCallMessage;
        private bool completed;
        private System.Runtime.Remoting.Messaging.IMethodReturnMessage methodReturnMessage;
        // private Exception exception;
        private ManualResetEvent completionWaitHandle = new ManualResetEvent(false);

        #region Accessors

        public System.Runtime.Remoting.Messaging.IMethodCallMessage MethodCallMessage
        {
            get { return methodCallMessage; }
        }

        #endregion

        #region Completion

        public System.Runtime.Remoting.Messaging.IMethodReturnMessage Wait()
        {
            completionWaitHandle.WaitOne();
            return methodReturnMessage;
        }

        public void Completed(System.Runtime.Remoting.Messaging.IMethodReturnMessage methodReturnMessage)
        {
            lock (this)
            {
                if (completed)
                    throw new Exception("Internal error: already completed.");
                completed = true;

                this.methodReturnMessage = methodReturnMessage;
                completionWaitHandle.Set();
            }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                // ........................................

                throw new Exception("The method or operation is not implemented."); 
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {

            // ........................................





            throw new Exception("The method or operation is not implemented.");
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new Exception("This object was not meant to be deserialized directly, it should always deserialize into " +  typeof(MethodCall).Name + ".");
        }

        #endregion
    }
}
