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

namespace QS._qss_c_.Membership2.ClientState
{
    public abstract class RegionView<RegionClass> : QS.Fx.Inspection.Inspectable, IRegionView where RegionClass : IRegion
    {
        public RegionView(RegionClass region, uint seqNo, QS._core_c_.Base3.InstanceID[] members)
        {
            this.region = region;
            this.seqno = seqNo;
            this.members = members;
			inspectable_membersProxy = new QS._qss_e_.Inspection_.Array("Members", members);
		}

		[QS.Fx.Base.Inspectable("Region", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected RegionClass region;
		[QS.Fx.Base.Inspectable("SeqNo", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint seqno;
		[QS.Fx.Base.Inspectable("Members", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS._qss_e_.Inspection_.Array inspectable_membersProxy;
		protected QS._core_c_.Base3.InstanceID[] members;

        public RegionClass Region
        {
            get { return region; }
        }

        public abstract void ProcessCrashed(IEnumerable<QS._core_c_.Base3.InstanceID> crashedAddresses);

        #region IRegionView Members
        
        IRegion IRegionView.Region
        {
            get { return region; }
        }

        public uint SeqNo
        {
            get { return seqno; }
        }

        public ICollection<QS._core_c_.Base3.InstanceID> Members
        {
            get { return members; }
        }

        #endregion
    }
}
