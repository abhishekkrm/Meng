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
    public class Region : Collections_1_.GenericBinaryTreeNode
    {
        public Region(Base3_.RegionID regionID, Base3_.RegionSig regionSig, QS.Fx.Network.NetworkAddress multicastAddress)
        {
            this.multicastAddress = multicastAddress;
            this.regionID = regionID;
            this.regionSig = regionSig;
            this.currentView = null; // when null, it marks the fact that the region is new
        }

        private Base3_.RegionID regionID;
        private Base3_.RegionSig regionSig;
        private RegionView currentView;
        private QS.Fx.Network.NetworkAddress multicastAddress;

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return multicastAddress; }
        }

        public Base3_.RegionID RegionID
        {
            get { return regionID; }
        }

        public Base3_.RegionSig RegionSig
        {
            get { return regionSig; }
        }

        public RegionView CurrentView
        {
            get { return currentView; }
            set { currentView = value; }
        }

/*
        public System.Collections.Generic.ICollection<Node> Nodes
        {
            get { return nodes; }
        }
*/

        public override IComparable Contents
        {
            get { return this.RegionSig; }
        }

        public override string ToString()
        {
            return "(Region " + regionID.ToString() + ")"; 
                // " ( " + regionSig.ToString() + " )\n__currentView: " + ((currentView != null) ? currentView.ToString() : "(null)");
        }
    }
}
