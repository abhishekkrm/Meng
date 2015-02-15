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

namespace QS._qss_c_.Multicasting5
{
    public class AckCollection : IAckCollection
    {
        private const uint DefaultCapacity = 10;

        public AckCollection() : this(DefaultCapacity)
        {
        }

        public AckCollection(uint initialCapacity)
        {
            this.capacity = initialCapacity;
            window = new bool[capacity];
            for (uint ind = 0; ind < capacity; ind++)
                window[ind] = false;
        }

        private uint capacity, firstSeqNo = 1;
        private bool[] window;

        private CompressedAckSet AsCompressedAckSet
        {
            get
            {
                List<Components_2_.Range<uint>> ranges = new List<QS._qss_c_.Components_2_.Range<uint>>();
                if (firstSeqNo > 1)
                    ranges.Add(new Components_2_.Range<uint>(1, firstSeqNo - 1));

                uint ind = 0;
                while (ind < capacity)
                {
                    while (!window[(firstSeqNo + ind) % capacity] && ind < capacity)
                        ind++;

                    if (ind < capacity)
                    {
                        uint from = firstSeqNo + ind;
                        while (window[(firstSeqNo + ind) % capacity] && ind < capacity)
                            ind++;

                        uint to = firstSeqNo + ind - 1;

                        ranges.Add(new QS._qss_c_.Components_2_.Range<uint>(from, to));
                    }
                }

                return new CompressedAckSet(ranges);
            }
        }

        #region IAckCollection Members

        QS.Fx.Serialization.ISerializable IAckCollection.AsCompressed
        {
            get { return this.AsCompressedAckSet; }
        }

        bool IAckCollection.Add(uint seqno)
        {
            if (seqno < firstSeqNo)
                return false;

            if (seqno < firstSeqNo + capacity)
            {
                uint slotno = seqno % capacity;
                if (window[slotno])
                    return false;

                window[slotno] = true;
                if (seqno == firstSeqNo)
                {
                    while (true)
                    {
                        uint first_slotno = firstSeqNo % capacity;
                        if (!window[first_slotno])
                            break;
                        window[first_slotno] = false;
                        firstSeqNo++;
                    }
                }

                return true;
            }
            else
            {
                uint new_capacity = (uint)Math.Ceiling(((double)capacity) * 2);
                uint minimum_new_capacity = seqno - firstSeqNo + 1;
                if (new_capacity <  minimum_new_capacity)
                    new_capacity = minimum_new_capacity;

                bool[] new_window = new bool[new_capacity];
                for (uint ind = 0; ind < capacity; ind++)
                {
                    uint some_seqno = firstSeqNo + ind;
                    new_window[some_seqno % new_capacity] = window[some_seqno % capacity];
                }

                for (uint ind = capacity; ind < new_capacity; ind++)
                    new_window[(firstSeqNo + ind) % new_capacity] = false;

                capacity = new_capacity;
                window = new_window;

                window[seqno % capacity] = true;
                return true;
            }
        }

        #endregion

        #region Printing

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

/*
            s.Append(firstSeqNo.ToString());
            s.Append(":");
            for (uint ind = 0; ind < capacity; ind++)
            {
                if (window[ind])
                    s.Append("*");
                else
                    s.Append("_");
            }
*/

            bool comma = false;
            if (firstSeqNo > 1)
            {
                s.Append("1-");
                s.Append((firstSeqNo - 1).ToString());
                comma = true;
            }

            for (uint ind = 0; ind < capacity; ind++)
            {
                if (window[(firstSeqNo + ind) % capacity])
                {
                    if (comma)
                        s.Append(",");
                    else
                        comma = true;
                    s.Append((firstSeqNo + ind).ToString());
                }
            }

            return s.ToString();
        }

        #endregion
    }
}
