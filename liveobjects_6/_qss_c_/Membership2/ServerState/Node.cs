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

namespace QS._qss_c_.Membership2.ServerState
{
    public class Node : System.IComparable<Node> // : Collections.GenericBinaryTreeNode
    {
        public Node(QS._core_c_.Base3.InstanceID instanceID)
        {
			this.instanceID = instanceID;
            this.region = null; // when null, it marks the fact the region is new
        }

		private QS._core_c_.Base3.InstanceID instanceID;
        private Region region;
        private uint notificationsSent = 0;

        public QS._core_c_.Base3.InstanceID InstanceID       
        {
            get { return instanceID; }
        }

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return instanceID.Address; }
        }

        public Region Region
        {
            set { region = value; }
            get { return region; }
        }

//        public override IComparable Contents
//        {
//            get { return instanceID; }
//        }

        public override string ToString()
        {
            return "(Node " + instanceID.ToString() + ")"; // +" in region " + ((region != null) ? region.RegionID.ToString() : "(null)");
        }

        public uint NotificationsSent
        {
            get { return notificationsSent; }
            set { notificationsSent = value; }
        }

        #region IComparable<Node> Members

        int IComparable<Node>.CompareTo(Node other)
        {
            return ((IComparable<QS._core_c_.Base3.InstanceID>)instanceID).CompareTo(other.instanceID);
        }

        #endregion
}
}
