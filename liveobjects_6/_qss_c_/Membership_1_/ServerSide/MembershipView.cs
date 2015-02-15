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

namespace QS._qss_c_.Membership_1_.ServerSide
{
    [System.Serializable]
    public class MembershipView : IMembershipView
    {
        public MembershipView()
        {
        }

        public MembershipView CreateCopy
        {
            get
            {
                return new MembershipView(this);
            }
        }

        public MembershipView(MembershipView existingView) : this(existingView.groupID, existingView.seqNo)
        {
            this.numberOfMembers = existingView.numberOfMembers;
            foreach (SubView subview in existingView.subviews)
                subviews.Add(new SubView(subview));
        }

        public MembershipView(GMS.GroupId groupID, uint seqNo)
        {
            this.groupID = groupID;
            this.seqNo = seqNo;
            this.subviews = new System.Collections.Generic.List<SubView>();
        }

        private GMS.GroupId groupID;
        private uint seqNo, numberOfMembers = 0;
        private System.Collections.Generic.List<SubView> subviews;

        public SubView[] SubViews
        {
            get { return subviews.ToArray(); }
            set { subviews = new System.Collections.Generic.List<SubView>(value); }
        }

        private SubView lookupSubView(QS.Fx.Network.NetworkAddress networkAddress)
        {
            SubView subview = null;
            foreach (SubView existingSubView in subviews)
            {
                if (existingSubView.Address.Equals(networkAddress))
                {
                    subview = existingSubView;
                    break;
                }
            }

            return subview;
        }

        #region IMembershipView Members

        public void add(QS.Fx.Network.NetworkAddress networkAddress, uint loid)
        {
            try
            {
                SubView subview = lookupSubView(networkAddress);
                if (subview == null)
                    subviews.Add(subview = new SubView(networkAddress));

                subview.add(loid);
                numberOfMembers++;
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot add " + networkAddress.ToString() + ":" + loid.ToString() +
                    " to view " + groupID.ToString() + ":" + seqNo.ToString(), exc);
            }
        }

        public void remove(QS.Fx.Network.NetworkAddress networkAddress, uint loid)
        {
            try
            {
            SubView subview = lookupSubView(networkAddress);
            if (subview == null)
            {
                throw new Exception("Cannot remove LOID:" + loid.ToString() +
                    " from subview " + networkAddress.ToString() + ", no such subview exists");
            }

            subview.remove(loid);
            numberOfMembers--;
        }
        catch (Exception exc)
        {
            throw new Exception("Cannot remove " + networkAddress.ToString() + ":" + loid.ToString() +
                " from view " + groupID.ToString() + ":" + seqNo.ToString(), exc);
        }
    }

        #endregion

        #region Class SubView

        [System.Serializable]
        public class SubView : GMS.ISubView
        {
            public SubView()
            {
            }

            public SubView(SubView anotherSubView) : this(anotherSubView.networkAddress)
            {
                members.AddRange(anotherSubView.members);
            }

            public SubView(QS.Fx.Network.NetworkAddress networkAddress)
            {
                this.networkAddress = networkAddress;
                members = new List<uint>();
            }

            private QS.Fx.Network.NetworkAddress networkAddress;
            private System.Collections.Generic.List<uint> members;

            public void add(uint loid)
            {
                if (members.Contains(loid))
                    throw new Exception("Subview " + networkAddress.ToString() +
                        " already contains member " + loid.ToString() + ".");

                members.Add(loid);
            }

            public void remove(uint loid)
            {
                if (!members.Remove(loid))
                    throw new Exception("Subview " + networkAddress.ToString() +
                        " does not contain member " + loid.ToString() + ".");
            }

            public QS.Fx.Network.NetworkAddress Address
            {
                get { return networkAddress; }
                set { networkAddress = value; }
            }

            public uint[] Members
            {
                get { return members.ToArray(); }
                set { members = new List<uint>(value); }
            }

            public override int GetHashCode()
            {
                return networkAddress.GetHashCode();
            }

            #region ISubView Members

            System.Net.IPAddress QS.GMS.ISubView.IPAddress
            {
                get { return networkAddress.HostIPAddress; }
            }

            int QS.GMS.ISubView.PortNumber
            {
                get { return networkAddress.PortNumber; }
            }

            uint QS.GMS.ISubView.this[uint index]
            {
                get { return members[(int) index]; }
            }

            int QS.GMS.ISubView.Count
            {
                get { return members.Count; }
            }

            #endregion
        }

        #endregion

        public GMS.GroupId GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }

        #region IView Members

        public uint SeqNo
        {
            get { return seqNo; }
            set { seqNo = value; }
        }

        QS.GMS.ISubView QS.GMS.IView.this[uint index]
        {
            get { return subviews[(int) index]; }
        }

        int QS.GMS.IView.NumberOfSubViews
        {
            get { return subviews.Count; }
        }

        int QS.GMS.IView.NumberOfMembers
        {
            get { return (int) numberOfMembers; }
        }

        #endregion

        public QS.Fx.Network.NetworkAddress[] Addresses
        {
            get
            {
                QS.Fx.Network.NetworkAddress[] addresses = new QS.Fx.Network.NetworkAddress[subviews.Count];
                int index = 0;
                foreach (SubView subview in subviews)
                    addresses[index++] = subview.Address;
                return addresses;
            }
        }
    }
}
