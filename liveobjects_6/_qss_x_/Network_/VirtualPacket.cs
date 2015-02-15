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

namespace QS._qss_x_.Network_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class VirtualPacket : IVirtualPacket
    {
        public VirtualPacket(QS.Fx.Network.NetworkAddress from, QS.Fx.Network.NetworkAddress to, ArraySegment<byte> data)
        {
            this.from = from;
            this.to = to;
            this.data = data;
        }

        private QS.Fx.Network.NetworkAddress from, to;
        private ArraySegment<byte> data;

        #region IVirtualPacket Members

        [QS.Fx.Printing.Printable]
        QS.Fx.Network.NetworkAddress IVirtualPacket.From
        {
            get { return from; }
        }

        [QS.Fx.Printing.Printable]
        QS.Fx.Network.NetworkAddress IVirtualPacket.To
        {
            get { return to; }
        }

        [QS.Fx.Printing.Printable]
        ArraySegment<byte> IVirtualPacket.Data
        {
            get { return data; }
        }

        IVirtualPacket IVirtualPacket.Clone()
        {
            byte[] copy = new byte[data.Count];
            Buffer.BlockCopy(data.Array, data.Offset, copy, 0, data.Count);
            return new VirtualPacket(from, to, new ArraySegment<byte>(copy));
        }

        #endregion

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }
    }
}
