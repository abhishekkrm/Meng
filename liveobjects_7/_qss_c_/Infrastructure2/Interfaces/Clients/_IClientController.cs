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
using System.ServiceModel;

namespace QS._qss_c_.Infrastructure2.Interfaces.Clients
{
/*
    /// <summary>
    /// This interface is exposed by the service that creates agents on behalf of a particular client node that will send in
    /// a paricular group view.
    /// </summary>
    [ServiceContract] 
    public interface IClientController
    {
        /// <summary>
        /// Creates a client agent that will handle sending in a particular session, cooperating with other \
        /// clients for this purpose.
        /// </summary>
        /// <param name="session_id">Identifier of the session on behalf of which the new agent will work.</param>
        /// <param name="algorithm_id">Identifier of the algorithm that the agents in this session will employ to communicate
        /// with each other, ensure ordering and whatever other properties they desire.</param>
        [OperationContract]
        void Start(string session_id, string algorithm_id);

        /// <summary>
        /// Removes the agent (either because the given node is no longer interested in sending, or because a view change
        /// has occurred that requires all client nodes to switch to a different session).
        /// </summary>
        /// <param name="session_id"></param>
        [OperationContract]
        void Stop(string session_id);


    }
*/ 
}
