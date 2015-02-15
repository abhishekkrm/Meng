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
    public abstract class Domain : QS.Fx.Inspection.Inspectable, IDomain
    {
        #region Constructor

        protected Domain(Base1_.QualifiedID qualifiedid, string name, Scope scope)
        {
            this.qualifiedid = qualifiedid;
            this.name = name;
            this.scope = scope;

            _InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        protected Base1_.QualifiedID qualifiedid;
        [QS.Fx.Base.Inspectable]
        protected string name;
        [QS.Fx.Base.Inspectable]
        protected Scope scope;
        [QS.Fx.Base.Inspectable]
        protected ulong lastinstance;

        protected Dictionary<Base1_.QualifiedID, Registration> registrations = new Dictionary<Base1_.QualifiedID, Registration>();
        protected Dictionary<MembershipID, Agent> agents = new Dictionary<MembershipID, Agent>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_registrations")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, Registration> __inspectable_registrations;

        [QS.Fx.Base.Inspectable("_agents")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<MembershipID, Agent> __inspectable_agents;

        private void _InitializeInspection()
        {
            __inspectable_registrations =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, Registration>("_registrations", registrations,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, Registration>.ConversionCallback(
                        delegate(string s) { return Base1_.QualifiedID.FromString(s); }));

            __inspectable_agents =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<MembershipID, Agent>("_agents", agents,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<MembershipID, Agent>.ConversionCallback(
                        delegate(string s) { return MembershipID.FromString(s); }));
        }

        #endregion

        #region IComparable<IDomain> Members

        int IComparable<IDomain>.CompareTo(IDomain other)
        {
            return ((IComparable<Base1_.QualifiedID>) qualifiedid).CompareTo(other.ID);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            IDomain other = obj as IDomain;
            if (other != null)
                return ((IComparable<IDomain>)this).CompareTo(other);
            else
                throw new Exception("Argument of the comparison is not a domain.");
        }

        #endregion

        #region IEquatable<IDomain> Members

        bool IEquatable<IDomain>.Equals(IDomain other)
        {
            return (other != null) && ((IEquatable<Base1_.QualifiedID>) qualifiedid).Equals(other.ID);
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            IDomain other = obj as IDomain;
            return (other != null) && ((IEquatable<IDomain>)this).Equals(other);
        }

        public override int GetHashCode()
        {
            return qualifiedid.GetHashCode();
        }

        public override string ToString()
        {
            return qualifiedid.ToString();
        }

        #endregion

        #region IDomain Members

        QS._qss_x_.Base1_.QualifiedID IDomain.ID
        {
            get { return qualifiedid; }
        }

        string IDomain.Name
        {
            get { return name; }
        }

        IScope IDomain.Scope
        {
            get { return scope; }
        }

        #endregion

        #region Internal Interface

        #region ID

        internal QS._qss_x_.Base1_.QualifiedID ID
        {
            get { return qualifiedid; }
        }

        #endregion

        #region Name

        internal string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion

        #region Scope

        internal Scope Scope
        {
            get { return scope; }
        }

        #endregion

        #region Registrations

        internal IDictionary<Base1_.QualifiedID, Registration> Registrations
        {
            get { return registrations; }
        }

        #endregion

        #region Agents

        internal IDictionary<MembershipID, Agent> Agents
        {
            get { return agents; }
        }

        #endregion

        #region Add

        internal void Add(Membership membership, MembershipView view)
        {
            MembershipID membershipid = new MembershipID(membership.Member.ID, membership.Container.ID, membership.Instance);
            if (agents.ContainsKey(membershipid))
                throw new Exception("Agent cannot be added because one with such name already exists.");

            Agent agent = new Agent(false, membershipid, membership, view);
            agents.Add(membershipid, agent);

            _Delegate(agent);
        }

        #endregion

        #region Change

        internal void Change(Membership membership, MembershipView view)
        {
            MembershipID membershipid = new MembershipID(membership.Member.ID, membership.Container.ID, membership.Instance);
            Agent agent;
            if (!agents.TryGetValue(membershipid, out agent))
                throw new Exception("Agent cannot be changed because no agent with such name exists.");

            System.Diagnostics.Debug.Assert(false, "Not Implemented");
            // TODO: Implement changing agent........................................................................................
        }

        #endregion

        #region Notify

        internal void Notify(Membership membership, MembershipView view)
        {
            MembershipID membershipid = new MembershipID(membership.Member.ID, membership.Container.ID, membership.Instance);
            Agent agent;
            if (!agents.TryGetValue(membershipid, out agent))
                throw new Exception("Agent cannot be notified because no agent with such name exists.");

            System.Diagnostics.Debug.Assert(false, "Not Implemented");
            // TODO: Implement notifying agent........................................................................................
        }

        #endregion

        #region Remove

        internal void Remove(Membership membership)
        {
            MembershipID membershipid = new MembershipID(membership.Member.ID, membership.Container.ID, membership.Instance);
            Agent agent;
            if (!agents.TryGetValue(membershipid, out agent))
                throw new Exception("Agent cannot be removed because no agent with such name exists.");

            System.Diagnostics.Debug.Assert(false, "Not Implemented");
            // TODO: Implement removing agent........................................................................................
        }

        #endregion

        #region StartRoot

        internal void StartRoot(Topic topic)
        {
            MembershipID membershipid = new MembershipID(qualifiedid, topic.ID, ++lastinstance);
            Agent agent = new Agent(true, membershipid, null, null);
            agents.Add(membershipid, agent);

            _StartRoot(agent);
        }

        #endregion

        #region StopRoot

        internal void StopRoot(Topic topic)
        {
            System.Diagnostics.Debug.Assert(false, "Not Implemented");
        }

        #endregion

        #endregion

        // private

        #region _StartRoot

        private void _StartRoot(Agent agent)
        {
            // TODO: Start root agent
        }

        #endregion

        #region _StopRoot

        private void _StopRoot(Agent agent)
        {
            // TODO: Stop root agent
        }

        #endregion

        #region _Delegate

        private void _Delegate(Agent agent)
        {
            // TODO: Delegate new agent
        }

        #endregion

        // ..................................................................................................................................................................
    }
}
