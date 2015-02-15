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

namespace QS._qss_c_.Embeddings2
{
    public class ReplicationGroup : RealProxy
    {
        public ReplicationGroup(string name, System.Type interfaceType, ReplicationGroupType groupType) : base(interfaceType)
        {
            this.name = name;
            this.interfaceType = interfaceType;
            this.groupType = groupType;

            methodArray = new CallingInfo[groupType.InterfaceType.Methods.Length];
            for (int ind = 0; ind < methodArray.Length; ind++)
            {
                CallingInfo callingInfo = new CallingInfo(ind, groupType.InterfaceType.Methods[ind]);
                methodArray[ind] = callingInfo;
                methodDictionary.Add(callingInfo.Method.Name, callingInfo);
            }
        }

        private string name;
        private System.Type interfaceType;
        private ReplicationGroupType groupType;
        private IDictionary<string, CallingInfo> methodDictionary = new Dictionary<string, CallingInfo>();
        private CallingInfo[] methodArray;

        #region Processing Requests

        private MethodCallResponse Submit(MethodCallRequest request)
        {
            // TODO: Fix this; here we would have some communication, but instead we just call the local replica.............

            return Handle(request);
        }

        private MethodCallResponse Handle(MethodCallRequest request)
        {
            // new System.Runtime.Remoting.Messaging.MethodCall(
            // CallingInfo callingInfo = methodArray[request.MethodIndex];

            throw new NotImplementedException();
        }

        #endregion

        #region Class CallingInfo

        private class CallingInfo
        {
            public CallingInfo(int index, Embeddings2.Types.Method method)
            {
                this.index = index;
                this.method = method;
            }

            private int index;
            private Embeddings2.Types.Method method;

            public int Index
            {
                get { return index; }
            }

            public Embeddings2.Types.Method Method
            {
                get { return method; }
            }
        }

        #endregion

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCall = (IMethodCallMessage) msg;

/*
            Console.WriteLine("Object(\"" + name + "\", " + interfaceType.Name + ").Invoke\n{");
            foreach (System.Collections.DictionaryEntry entry in msg.Properties)
                Console.WriteLine("\t" + entry.Key + " = " + entry.Value);
            Console.WriteLine("}");
*/

            CallingInfo callingInfo;
            if (!methodDictionary.TryGetValue(methodCall.MethodName, out callingInfo))
                throw new Exception("Method \"" + methodCall.MethodName + "\" is not supported by this proxy.");

            MethodCallResponse response = Submit(new MethodCallRequest(callingInfo.Index, methodCall.InArgs));

                
            return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
        }
    }
}
