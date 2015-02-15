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

namespace QS._qss_c_.Embeddings2
{
    public class GroupServices
    {
        public static GroupServices Shared
        {
            get
            {
                if (shared != null)
                    return shared;
                else
                    throw new Exception("Not connected yet!");
            }
        }

        private static GroupServices shared;
        public static void Connect(Membership_3_.Interface.IMembershipService membershipService, QS.Fx.Logging.ILogger logger)
        {
            shared = new GroupServices(membershipService, logger);
        }

        private GroupServices(Membership_3_.Interface.IMembershipService membershipService, QS.Fx.Logging.ILogger logger)
        {
            this.membershipService = membershipService;
            this.logger = logger;
        }

        private Membership_3_.Interface.IMembershipService membershipService;
        private QS.Fx.Logging.ILogger logger;

        public IAsyncResult BeginOpen<C>(string name, C localObject, AsyncCallback asyncCallback, object asyncState)
        {
            ReplicationGroupType2 groupType = new ReplicationGroupType2(typeof(C));
            return membershipService.BeginOpen(
                new Membership_3_.Expressions.Group(name), QS._qss_c_.Membership_3_.Interface.OpeningMode.OpenOrCreate,
                QS._qss_c_.Membership_3_.Interface.AccessMode.Member, groupType, null, asyncCallback, asyncState);
        }

        public C EndOpen<C>(IAsyncResult asyncResult)
        {
            Membership_3_.Interface.IGroupRef groupRef = membershipService.EndOpen(asyncResult);
            ReplicationGroup2 replicationGroup = new ReplicationGroup2(groupRef, typeof(C), logger);
            return (C)replicationGroup.GetTransparentProxy();
        }
    }
}
