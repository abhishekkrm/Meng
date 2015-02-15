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

// #define DEBUG_Inspector

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Connections_
{
    public class Inspector : QS._qss_e_.Inspection_.RemotingProxy
    {
        public static readonly QS.Fx.Network.NetworkAddress MulticastAddress =
            new QS.Fx.Network.NetworkAddress("224.99.99.99:65050");

        public Inspector(QS.Fx.Logging.ILogger logger, IAsynchronousRef asynchronousRef)
        {
            this.logger = logger;
            agent = Invocator.ConnectedObject<Agent>(asynchronousRef);
        }

        private QS.Fx.Logging.ILogger logger;
        private Agent agent;

        protected override object MakeCall(QS._core_c_.Components.AttributeSet arguments)
        {
#if DEBUG_Inspector
            logger.Log(this, "__MakeCall : Arguments = " + arguments.ToString());
#endif

            object result = agent.Inspect(arguments);

#if DEBUG_Inspector
            logger.Log(this, "__MakeCall : Response = " + Helpers.ToString.Object(result));
#endif

            return result;
        }

        #region Class Agent

        public class Agent : MarshalByRefObject
        {
            public Agent(QS.Fx.Inspection.IInspectable inspectableObject)
            {
                this.inspectableObject = inspectableObject;
            }

            private QS.Fx.Inspection.IInspectable inspectableObject;

            public object Inspect(QS._core_c_.Components.AttributeSet arguments)
            {
                return QS._qss_e_.Inspection_.RemotingProxy.DispatchCall(inspectableObject, arguments);
            }
        }

        #endregion
    }
}
