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

namespace QS._qss_x_.Backbone_.Scope
{
    public interface IExternalControl
    {
        // Define the hierarchy controller for this local scope.
        IController HierarchyController
        {
            get;
            set;
        }

        // Interface for controlling the local endpoint
        IEndpointControl EndpointControl
        {
            get;
        }

        void RegisterProvider(QS.Fx.Base.ID providerid, string name);

        void UnregisterProvider(QS.Fx.Base.ID providerid);

        void RegisterTopic(Base1_.QualifiedID topicid, string name);

        void UnregisterTopic(Base1_.QualifiedID topicid);

        void RegisterSubscope(QS.Fx.Base.ID subscopeid, string name);

        void UnregisterSubscope(QS.Fx.Base.ID subscopeid);

        /// <summary>
        /// Register the given domain at the given child scope for the given topic.
        /// </summary>
        /// <param name="scopeid">Child scope.</param>
        /// <param name="domainid">Domain at the child scope.</param>
        /// <param name="topicid">Topic to register for.</param>
        /// <param name="registrationtype">Whether the domain is going to be an active member or just a passive client.</param>
        void RegisterSubdomain(
            Base1_.QualifiedID domainid, string domainname, Base1_.QualifiedID topicid, MembershipType registrationtype);

        /// <summary>
        /// Gracefully unregister the given domain at the given child scope from the given topic.
        /// </summary>
        /// <param name="scopeid">Child scope.</param>
        /// <param name="domainid">Domain at the child scope.</param>
        /// <param name="topicid">Topic to unregister from.</param>
        void UnregisterSubdomain(Base1_.QualifiedID domainid, Base1_.QualifiedID topicid);
    }
}
