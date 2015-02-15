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

namespace QS._qss_c_.Membership_3_.ServerState
{
/*
    public sealed class Group 
    {
        public Group(string name, Expressions.Expression expression, Base3.GroupID id, 
            Interface.IGroupType type, Interface.IGroupAttributes attributes)
        {
            this.name = name;
            this.expression = expression;
            this.id = id;
            this.type = type;
            this.attributes = attributes;
        }

        private string name;
        private Expressions.Expression expression;
        private Base3.GroupID id;
        private Interface.IGroupType type;
        private Interface.IGroupAttributes attributes;
        private IDictionary<int, GroupView> membershipViews = new Dictionary<int, GroupView>();
        private IDictionary<int, ClientView> clientViews = new Dictionary<int, ClientView>();
        private GroupView currentMembershipView;
        private ClientView currentClientView;

        // additional stuff, for efficient updates
        private ICollection<QS._core_c_.Base3.InstanceID> memberAddresses = 
            new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>(),
            clientAddresses = new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();

        #region Accessors

        public string Name
        {
            get { return name; }
        }

        public Expressions.Expression Expression
        {
            get { return expression; }
        }

        public Base3.GroupID ID
        {
            get { return id; }
        }

        public Interface.IGroupType Type
        {
            get { return type; }
        }

        public Interface.IGroupAttributes Attributes
        {
            get { return attributes; }
        }

        public IDictionary<int, GroupView> MembershipViews
        {
            get { return membershipViews; }
        }

        public GroupView CurrentMembershipView
        {
            get { return currentMembershipView; }
        }

        public IDictionary<int, ClientView> ClientViews
        {
            get { return clientViews; }
        }

        public ClientView CurrentClientView
        {
            get { return currentClientView; }
        }

        #endregion
        
        #region Additional interface used by the code that processes updates

        public ICollection<QS._core_c_.Base3.InstanceID> AddressesOfMembers
        {
            get { return memberAddresses; }
        }

        public ICollection<QS._core_c_.Base3.InstanceID> AddressesOfClients
        {
            get { return clientAddresses; }
        }

        #endregion
    }
*/ 
}
