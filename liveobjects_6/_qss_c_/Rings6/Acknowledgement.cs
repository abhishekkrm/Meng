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
    [QS.Fx.Serialization.ClassID(ClassID.Rings6_ReceivingAgent_Agent_Receiver_Ack)]
    [QS.Fx.Printing.Printable("Ack", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public class Acknowledgement : QS.Fx.Serialization.ISerializable
    {
        public Acknowledgement(uint maximumClean, IList<Base1_.Range<uint>> isolatedAcks, 
            double minimumReceiveRate, double averageReceiveRate, double maximumReceiveRate)
        {
            this.maximumClean = maximumClean;
            this.isolatedAcks = isolatedAcks;
            this.minimumReceiveRate = minimumReceiveRate;
            this.averageReceiveRate = averageReceiveRate;
            this.maximumReceiveRate = maximumReceiveRate;
        }

        public Acknowledgement()
        {
        }

        [QS.Fx.Printing.Printable]
        private uint maximumClean;
        [QS.Fx.Printing.Printable]
        private IList<Base1_.Range<uint>> isolatedAcks;
        [QS.Fx.Printing.Printable]
        private double minimumReceiveRate, averageReceiveRate, maximumReceiveRate;

        public uint MaximumClean
        {
            get { return maximumClean; }
        }

        public IList<Base1_.Range<uint>> IsolatedAcks
        {
            get { return isolatedAcks; }
        }

        public double MinimumReceiveRate
        {
            get { return minimumReceiveRate; }
        }

        public double AverageReceiveRate
        {
            get { return averageReceiveRate; }
        }

        public double MaximumReceiveRate
        {
            get { return maximumReceiveRate; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = sizeof(ushort) + sizeof(uint) + 2 * sizeof(uint) * (isolatedAcks != null ? isolatedAcks.Count : 0) + 3 * sizeof(float);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Rings6_ReceivingAgent_Agent_Receiver_Ack, (ushort)size, size, 0);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            int count = isolatedAcks != null ? isolatedAcks.Count : 0;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)pheader) = maximumClean;
                *((ushort*)(pheader + sizeof(uint))) = (ushort)count;
                *((float*)(pheader + sizeof(uint) + sizeof(ushort))) = (float) minimumReceiveRate;
                *((float*)(pheader + sizeof(uint) + sizeof(ushort) + sizeof(float))) = (float) averageReceiveRate;
                *((float*)(pheader + sizeof(uint) + sizeof(ushort) + 2 * sizeof(float))) = (float) maximumReceiveRate;
                pheader += sizeof(ushort) + sizeof(uint) + 3 * sizeof(float);
                if (count > 0)
                {
                    foreach (Base1_.Range<uint> ack in isolatedAcks)
                    {
                        *((uint*)pheader) = ack.From;
                        *((uint*)(pheader + sizeof(uint))) = ack.To;
                        pheader += 2 * sizeof(uint);
                    }
                }
            }
            header.consume(sizeof(ushort) + sizeof(uint) + 2 * sizeof(uint) * count + 3 * sizeof(float));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                maximumClean = *((uint*)pheader);
                count = (int)(*((ushort*)(pheader + sizeof(uint))));
                minimumReceiveRate = (double)(*((float*)(pheader + sizeof(uint) + sizeof(ushort))));
                averageReceiveRate = (double)(*((float*)(pheader + sizeof(uint) + sizeof(ushort) + sizeof(float))));
                maximumReceiveRate = (double)(*((float*)(pheader + sizeof(uint) + sizeof(ushort) + 2 * sizeof(float))));
                if (count > 0)
                    isolatedAcks = new List<Base1_.Range<uint>>();
                pheader += sizeof(ushort) + sizeof(uint) + 3 * sizeof(float);
                for (int ind = 0; ind < count; ind++)
                {
                    isolatedAcks.Add(new QS._qss_c_.Base1_.Range<uint>(*((uint*)pheader), *((uint*)(pheader + sizeof(uint)))));
                    pheader += 2 * sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + sizeof(uint) + 2 * sizeof(uint) * count + 3 * sizeof(float));
        }

        #endregion
    }
}
