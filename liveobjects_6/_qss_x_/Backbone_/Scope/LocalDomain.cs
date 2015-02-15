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

#define OPTION_SanityChecking

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Scope
{
    public sealed class LocalDomain : Domain, ILocalDomain
    {
        #region Constructor

        public LocalDomain(Base1_.QualifiedID qualifiedid, string name, LocalScope localscope) : base(qualifiedid, name, localscope)
        {
            this.localscope = localscope;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private LocalScope localscope;
        [QS.Fx.Base.Inspectable]
        private bool modified;
        [QS.Fx.Base.Inspectable]
        private MembershipView view, committed;
        [QS.Fx.Base.Inspectable]
        private IList<Membership> added, removed, changed, unaffected;

        #endregion

        #region ILocalDomain Members

        #region MembershipView

        IMembershipView ILocalDomain.MembershipView
        {
            get { return view; }
        }

        #endregion

        #region Register

        void ILocalDomain.Register(IDomain _subdomain, MembershipType membershiptype)
        {
            Domain subdomain = _subdomain as Domain;
            if (subdomain == null)
                throw new Exception("The subdomain passed as an argument is either null or is of a wrong type.");

            _CopyOnWrite();

            bool found = false;
            Membership existingmembership = null;
            if (view != null)
            {
                foreach (Membership membership in view.Memberships)
                {
                    if (((IEquatable<IDomain>) membership.Member).Equals(subdomain) && membership.State != MembershipState.Retired)
                    {
                        if (membership.State == MembershipState.Associate)
                        {
                            if (found || existingmembership != null)
                                throw new Exception("Two associate memberships for the same domain found!");

                            found = true;
                            existingmembership = membership;
                        }
                        else
                            throw new Exception("Domain " + qualifiedid.ToString() + " already has member " + _subdomain.ToString() + 
                                " registered in a " + membership.State.ToString() + " state; the old registration must be removed first.");
                    }
                }
            }

            if (existingmembership != null)
            {
                if (membershiptype == MembershipType.Passive)
                    throw new Exception("Already registered as associate.");
                else
                {
                    // TODO: Implement promoting associate to a member................

                    System.Diagnostics.Debug.Assert(false, "Not Implemented");
                }

                if (!added.Contains(existingmembership) && 
                    (unaffected.Remove(existingmembership) || !changed.Contains(existingmembership)))
                    changed.Add(existingmembership);
            }
            else
            {
                Membership newmembership =
                    new Membership(++lastinstance, this, subdomain, membershiptype, MembershipState.New);

                view.Memberships.Add(newmembership);

                added.Add(newmembership);
            }
        }

        #endregion

        #region Unregister

        void ILocalDomain.Unregister(IDomain _subdomain)
        {
            Domain subdomain = _subdomain as Domain;
            if (subdomain == null)
                throw new Exception("The subdomain passed as an argument is either null or is of a wrong type.");

            if (view == null)
                throw new Exception("Cannot unregister because the view is null, hence there are currently no members to unregister.");

            _CopyOnWrite();

            bool found = false;
            Membership existingmembership = null;
            foreach (Membership membership in view.Memberships)
            {
                if (((IEquatable<IDomain>) membership.Member).Equals(subdomain) && (membership.State != MembershipState.Retired))
                {
                    if (found || existingmembership != null)
                        throw new Exception("Two non-retired existing memberships for the same domain found!");

                    found = true;
                    existingmembership = membership;
                    break;
                }
            }

            if (!found)
                throw new Exception("Domain " + qualifiedid.ToString() + " does not have member " + _subdomain.ToString() +
                    " registered in a non-retired state, so there's nothing to unregister.");

            switch (existingmembership.State)
            {
                case MembershipState.New:
                case MembershipState.Associate:
                    {
                        if (!view.Memberships.Remove(existingmembership))
                            throw new Exception("Could not remove existing membership.");

                        if (!added.Remove(existingmembership))
                            throw new Exception("Internal error");
                    }
                    break;

                case MembershipState.Candidate:
                case MembershipState.Member:
                case MembershipState.Downgraded:
                case MembershipState.Upgraded:
                    {
                        existingmembership.State = MembershipState.Retired;

                        if (added.Remove(existingmembership) || unaffected.Remove(existingmembership) || 
                            !changed.Contains(existingmembership))
                        {
                            changed.Add(existingmembership);
                        }
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #region Internal Interface

        #region View

        internal MembershipView View
        {
            get { return view; }
        }

        #endregion

        #region ComittedView

        internal MembershipView ComittedView
        {
            get { return committed; }
        }

        #endregion

        #region Commit

        internal void Commit()
        {
            if (modified)
            {
                committed = view;
                modified = false;

                bool membersexist = false;
                foreach (Membership membership in view.Memberships)
                {
                    switch (membership.State)
                    {
                        case MembershipState.New:
                            {
                                switch (membership.Type)
                                {
                                    case MembershipType.Active:
                                        membership.State = MembershipState.Candidate;
                                        break;

                                    case MembershipType.Passive:
                                    default:
                                        membership.State = MembershipState.Associate;
                                        break;
                                }
                            }
                            break;

                        case MembershipState.Upgraded:
                            {
                                membership.State = MembershipState.Member;
                                membersexist = true;
                            }
                            break;

                        case MembershipState.Downgraded:
                            membership.State = MembershipState.Candidate;
                            break;

                        case MembershipState.Member:
                        case MembershipState.Retired:
                            {
                                membersexist = true;
                            }
                            break;

                        case MembershipState.Associate:
                        case MembershipState.Candidate:
                        default:
                            break;
                    }
                }

                if (!membersexist)
                {
                    foreach (Membership membership in view.Memberships)
                    {
                        if (membership.State == MembershipState.Candidate)
                            membership.State = MembershipState.Member;
                    }
                }

                foreach (Membership membership in added)
                    membership.Member.Add(membership, view);
                added = null;

                foreach (Membership membership in changed)
                    membership.Member.Change(membership, view);
                changed = null;

                foreach (Membership membership in unaffected)
                    membership.Member.Notify(membership, view);
                unaffected = null;

                foreach (Membership membership in removed)
                    membership.Member.Remove(membership);
                removed = null;
            }
        }

        #endregion

        #endregion

        // private

        #region _CopyOnWrite

        private void _CopyOnWrite()
        {
            if (!modified)
            {
                modified = true;

                added = new List<Membership>();
                removed = new List<Membership>();
                changed = new List<Membership>();
                unaffected = new List<Membership>();

                if (!((committed != null) && (view != null) && committed.Equals(view) || (committed == null) && (view == null)))
                    throw new Exception("View is marked as unmodified, but it is not the same as the committed view.");

                if (committed != null)
                {
                    view = new MembershipView(this, committed.Number + 1);
                    
                    foreach (Membership membership in committed.Memberships)
                    {
                        Membership copy = new Membership(membership);
                        view.Memberships.Add(copy);
                        unaffected.Add(copy);
                    }
                }
                else
                    view = new MembershipView(this, 1);

                localscope.Modified(this);
            }
        }

        #endregion
    }
}
