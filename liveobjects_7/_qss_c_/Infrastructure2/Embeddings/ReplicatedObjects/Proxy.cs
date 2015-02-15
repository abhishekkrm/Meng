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
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_.Infrastructure2.Embeddings.ReplicatedObjects
{
    public sealed class Proxy : RealProxy
    {
        public Proxy(Type interfaceType, Interfaces.Subscription.IGroupReference groupReference) : base(interfaceType)
        {
            this.groupReference = groupReference;
            responseCallback = new AsyncCallback(this.ResponseCallback);
        }

        private Interfaces.Subscription.IGroupReference groupReference;
        private AsyncCallback responseCallback;

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;
            Request request = new Request(methodCall);

            bool one_way = (methodCall.MethodBase.GetCustomAttributes(typeof(OneWayAttribute), true).Length > 0);
            if (one_way)
            {
                groupReference.BeginSend(new QS._core_c_.Base3.Message(0, request), responseCallback, request);
                return new ReturnMessage(null, null, 0, request.MethodCallMessage.LogicalCallContext, request.MethodCallMessage);
            }
            else
            {
                groupReference.BeginCall(new QS._core_c_.Base3.Message(0, request), responseCallback, request);
                return request.Wait();
            }
        }

        private void ResponseCallback(IAsyncResult result)
        {
            ((Request) result.AsyncState).Completed(((Response) groupReference.EndCall(result)).MethodReturnMessage);
        }
    }
}
