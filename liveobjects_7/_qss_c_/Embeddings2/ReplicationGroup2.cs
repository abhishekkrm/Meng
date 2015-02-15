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
    public class ReplicationGroup2 : RealProxy, IDisposable
    {
        public ReplicationGroup2(Membership_3_.Interface.IGroupRef groupRef, System.Type interfaceType, QS.Fx.Logging.ILogger logger) 
            : base(interfaceType)
        {
            this.groupRef = groupRef;
            this.interfaceType = interfaceType;
            this.logger = logger;
        }

        private Membership_3_.Interface.IGroupRef groupRef;
        private System.Type interfaceType;
        private QS.Fx.Logging.ILogger logger;

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;

            StringBuilder s = new StringBuilder();
            s.AppendLine("Object(\"" + groupRef.Name + "\", " + interfaceType.Name + ").Invoke\n{");
            foreach (System.Collections.DictionaryEntry entry in msg.Properties)
                s.AppendLine("\t" + entry.Key + " = " + entry.Value);
            s.AppendLine("}");

            logger.Log(this, "__________Invoke___\n" + s.ToString());

            return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            groupRef.Dispose();
            groupRef = null;
        }

        #endregion
    }
}
