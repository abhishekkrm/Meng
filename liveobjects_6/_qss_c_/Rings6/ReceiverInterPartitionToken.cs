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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Rings6_ReceivingAgent1_Receiver_InterPartitionToken)]
    public class ReceiverInterPartitionToken : QS.Fx.Serialization.ISerializable
    {
        public ReceiverInterPartitionToken()
        {
        }

        [QS.Fx.Printing.Printable]
        private uint maximumSeen, cutOff, maxContiguous, maximumToClean;
        [QS.Fx.Printing.Printable]
        private uint[] tableOfCutOffs, tableOfMaxContiguous;
        [QS.Fx.Printing.Printable]
        private double minimumReceiveRate, cumulatedReceiveRate, maximumReceiveRate;
        [QS.Fx.Printing.Printable]
        private int memberCount;

        #region Accessors

        public double MinimumReceiveRate
        {
            get { return minimumReceiveRate; }
            set { minimumReceiveRate = value; }
        }

        public double CumulatedReceiveRate
        {
            get { return cumulatedReceiveRate; }
            set { cumulatedReceiveRate = value; }
        }

        public int MemberCount
        {
            get { return memberCount; }
            set { memberCount = value; }
        }

        public double MaximumReceiveRate
        {
            get { return maximumReceiveRate; }
            set { maximumReceiveRate = value; }
        }

        public uint[] TableOfMaxContiguous
        {
            get { return tableOfMaxContiguous; }
            set { tableOfMaxContiguous = value; }
        }

        public uint[] TableOfCutOffs
        {
            get { return tableOfCutOffs; }
            set { tableOfCutOffs = value; }
        }

        public uint MaximumSeen
        {
            get { return maximumSeen; }
            set { maximumSeen = value; }
        }

        public uint CutOff
        {
            get { return cutOff; }
            set { cutOff = value; }
        }

        public uint MaxContiguous
        {
            get { return maxContiguous; }
            set { maxContiguous = value; }
        }

        public uint MaximumToClean
        {
            get { return maximumToClean; }
            set { maximumToClean = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = ((5 + tableOfCutOffs.Length + tableOfMaxContiguous.Length) * sizeof(uint) + 2 * sizeof(ushort)) + 3 * sizeof(float);
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Rings6_ReceivingAgent1_Receiver_InterPartitionToken, (ushort)size, size, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)pheader) = maximumSeen;
                *((uint*)(pheader + sizeof(uint))) = cutOff;
                *((uint*)(pheader + 2 * sizeof(uint))) = maxContiguous;
                *((uint*)(pheader + 3 * sizeof(uint))) = maximumToClean;
                *((ushort*)(pheader + 4 * sizeof(uint))) = (ushort)tableOfCutOffs.Length;
                *((ushort*)(pheader + 4 * sizeof(uint) + sizeof(ushort))) = (ushort)tableOfMaxContiguous.Length;
                *((float*)(pheader + 4 * sizeof(uint) + 2 * sizeof(ushort))) = (float) minimumReceiveRate;
                *((float*)(pheader + 4 * sizeof(uint) + 2 * sizeof(ushort) + sizeof(float))) = (float) cumulatedReceiveRate;
                *((int*)(pheader + 4 * sizeof(uint) + 2 * sizeof(ushort) + 2 * sizeof(float))) = memberCount;
                *((float*)(pheader + 5 * sizeof(uint) + 2 * sizeof(ushort) + 2 * sizeof(float))) = (float)maximumReceiveRate;
                pheader += 5 * sizeof(uint) + 2 * sizeof(ushort) + 3 * sizeof(float);
                for (int ind = 0; ind < tableOfCutOffs.Length; ind++)
                {
                    *((uint*)pheader) = tableOfCutOffs[ind];
                    pheader += sizeof(uint);
                }
                for (int ind = 0; ind < tableOfMaxContiguous.Length; ind++)
                {
                    *((uint*)pheader) = tableOfMaxContiguous[ind];
                    pheader += sizeof(uint);
                }
            }
            header.consume((5 + tableOfCutOffs.Length + tableOfMaxContiguous.Length) * sizeof(uint) + 2 * sizeof(ushort) + 3 * sizeof(float));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                maximumSeen = *((uint*)pheader);
                cutOff = *((uint*)(pheader + sizeof(uint)));
                maxContiguous = *((uint*)(pheader + 2 * sizeof(uint)));
                maximumToClean = *((uint*)(pheader + 3 * sizeof(uint)));
                tableOfCutOffs = new uint[(int)(*((ushort*)(pheader + 4 * sizeof(uint))))];
                tableOfMaxContiguous = new uint[(int)(*((ushort*)(pheader + 4 * sizeof(uint) + sizeof(ushort))))];
                minimumReceiveRate = (double)(*((float*)(pheader + 4 * sizeof(uint) + 2 * sizeof(ushort))));
                cumulatedReceiveRate = (double)(*((float*)(pheader + 4 * sizeof(uint) + 2 * sizeof(ushort) + sizeof(float))));
                memberCount = (*((int*)(pheader + 4 * sizeof(uint) + 2 * sizeof(ushort) + 2 * sizeof(float))));
                maximumReceiveRate = (double)(*((float*)(pheader + 5 * sizeof(uint) + 2 * sizeof(ushort) + 2 * sizeof(float))));

                pheader += 5 * sizeof(uint) + 2 * sizeof(ushort) + 3 * sizeof(float);
                for (int ind = 0; ind < tableOfCutOffs.Length; ind++)
                {
                    tableOfCutOffs[ind] = *((uint*)pheader);
                    pheader += sizeof(uint);
                }
                for (int ind = 0; ind < tableOfMaxContiguous.Length; ind++)
                {
                    tableOfMaxContiguous[ind] = *((uint*)pheader);
                    pheader += sizeof(uint);
                }
            }
            header.consume((5 + tableOfCutOffs.Length + tableOfMaxContiguous.Length) * sizeof(uint) + 2 * sizeof(ushort) + 3 * sizeof(float));
        }

        #endregion

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }
    }
}
