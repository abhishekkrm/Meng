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

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class SenderAckNakClient : QS.Fx.Inspection.Inspectable, ISenderAckNakClient, QS._core_c_.Diagnostics2.IModule
    {
        public SenderAckNakClient()
        {
            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        // private IList<Base.Range<uint>> sumnaks;
        private uint maxclean; // , sumcovered;
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region ISenderAckNakClient Members

        void ISenderAckNakClient.Update(IList<QS._qss_c_.Base1_.Range<uint>> naks, uint covered, out IList<QS._qss_c_.Base1_.Range<uint>> newacks, out IList<QS._qss_c_.Base1_.Range<uint>> newnaks)
        {
            // Receivers4.AckHelpers.IntersectNaks(sumnaks, sumcovered, naks, covered, 
            
            throw new NotImplementedException();   
        }

        void ISenderAckNakClient.Cleanup(uint maxclean)
        {
            if (maxclean > this.maxclean)
            {
                this.maxclean = maxclean;

                // ......................................................................................................................................................
            }
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion
    }
}
