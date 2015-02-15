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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
/*
    public class SimpleSender : GroupSenderClass<IGroupSender, SimpleSender.Sender>
    {
        public SimpleSender(VS6.IMembershipController membershipController)
        {
            this.membershipController = membershipController;
        }

        private VS6.IMembershipController membershipController;

        protected override Sender createSender(Base3.GroupID groupID)
        {
            return new Sender(this, groupID);
        }

        #region Class Sender

        public class Sender : Base3.IndexedByGroup, IGroupSender
        {
            public Sender(SimpleSender owner, Base3.GroupID groupID) : base(groupID)
            {
                this.owner = owner;
                groupController = owner.membershipController[groupID];
            }

            private SimpleSender owner;
            private VS6.IGroupController groupController;

            #region IGroupSender Members

            public void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {

                // ..............................................................................................................

                throw new NotImplementedException();
            }

            public int MTU
            {
                get { throw new global::System.NotImplementedException(); }
            }

            #endregion
        }

        #endregion
    }
*/
}
