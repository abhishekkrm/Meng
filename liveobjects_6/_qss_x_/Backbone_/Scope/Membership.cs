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
    public sealed class Membership : QS.Fx.Inspection.Inspectable, IMembership
    {
        #region Constructor

        public Membership(Membership other) : this(other.instance, other.container, other.member, other.type, other.state)
        {
        }

        public Membership(ulong instance, Domain container, Domain member, MembershipType type, MembershipState state)
        {
            this.instance = instance;
            this.container = container;
            this.member = member;
            this.type = type;
            this.state = state;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private ulong instance;
        [QS.Fx.Base.Inspectable]
        private Domain container, member;
        [QS.Fx.Base.Inspectable]
        private MembershipType type;
        [QS.Fx.Base.Inspectable]
        private MembershipState state;

        #endregion

        #region IMembership Members

        IDomain IMembership.Container
        {
            get { return container; }
        }

        IDomain IMembership.Member
        {
            get { return member; }
        }

        MembershipType IMembership.Type
        {
            get { return type; }
        }

        MembershipState IMembership.State
        {
            get { return state; }
        }

        #endregion

        #region Internal Interface

        internal ulong Instance
        {
            get { return instance; }
        }

        internal Domain Container
        {
            get { return container; }
        }

        internal Domain Member
        {
            get { return member; }
        }

        internal MembershipType Type
        {
            get { return type; }
        }

        internal MembershipState State
        {
            get { return state; }
            set { state = value; }
        }

        #endregion
    }
}
