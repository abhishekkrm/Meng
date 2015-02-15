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

namespace QS._qss_c_.Membership2.Consumers
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    public class RegionChange
    {
        public enum KindOf
        {
            ENTERED_REGION, SWITCHED_REGION, LEFT_REGION, MEMBERSHIP_CHANGED, NONE
        }

        public RegionChange()
        {
        }

        private KindOf localChange = KindOf.NONE;
        private ClientState.IRegionView currentView;
        private List<ClientState.IRegion> neighborsToAdd = new List<ClientState.IRegion>();
        private List<ClientState.IRegion> neighborsToRemove = new List<ClientState.IRegion>();

        public KindOf LocalChange
        {
            get { return localChange; }
            set { localChange = value; }
        }

        public ClientState.IRegionView CurrentView
        {
            get { return currentView; }
            set { currentView = value; }
        }

        public IList<ClientState.IRegion> RegionsAdded
        {
            get { return neighborsToAdd; }
        }

        public IList<ClientState.IRegion> RegionsRemoved
        {
            get { return neighborsToRemove; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("LocalChange: " + localChange.ToString() + ", CurrentView: " + 
                currentView.Region.ID.ToString() + ":" + currentView.SeqNo.ToString() + ", RegionsToAdd: ");
            foreach (ClientState.IRegion region in neighborsToAdd)
                s.Append(region.ID.ToString() + " ");
            s.Append("RegionsToRemove");
            foreach (ClientState.IRegion region in neighborsToRemove)
                s.Append(region.ID.ToString() + " ");
            return s.ToString();
        }
    }
}
