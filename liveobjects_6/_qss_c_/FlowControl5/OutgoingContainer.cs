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

// #define STATISTICS_TrackWindowContents

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.FlowControl5
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class OutgoingContainer<C> : QS.Fx.Inspection.Inspectable, IOutgoingContainer<C> where C : class
    {
        public const uint DefaultCapacity = 20;
        public const double DefaultGrowthFactor = 2.0;

        public OutgoingContainer(QS.Fx.Clock.IClock clock) : this(clock, DefaultCapacity, DefaultGrowthFactor)
        {
        }

        public OutgoingContainer(QS.Fx.Clock.IClock clock, uint capacity, double growthFactor)
        {
            this.clock = clock;
            this.capacity = capacity;
            this.growthFactor = growthFactor;
            elements = new C[capacity];
        }

        public uint FirstOccupiedSeqNo
        {
            get { return this.firstOccupied; }
        }

        // First free seqno is always firstOccupied + occupiedCount, so when nothing is occupied, firstOccupied is same as first free.
        private uint capacity, firstOccupied = 1, occupiedCount = 0; 
        private C[] elements;
        private double growthFactor;
        private QS.Fx.Clock.IClock clock;

#if STATISTICS_TrackWindowContents
        [QS.CMS.Diagnostics.Component("First Occupied")]
        private Statistics.SamplesXY timeSeries_firstOccupied = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Occupied Count")]
        private Statistics.SamplesXY timeSeries_occupiedCount = new QS.CMS.Statistics.SamplesXY();
#endif

        private const uint ELEMENTS_PER_LINE = 100;
        public string AsString
        {
            get
            {
                StringBuilder s = new StringBuilder("OutgoingContainer");
                for (uint ind = 0; ind < capacity; ind++)
                {
                    uint seqno = firstOccupied + ind;
                    uint slotno = seqno % capacity;

                    if (ind % ELEMENTS_PER_LINE == 0)
                    {
                        s.AppendLine();
                        s.Append(seqno.ToString("000000"));
                        s.Append("-");
                        s.Append((seqno + ELEMENTS_PER_LINE - 1).ToString("000000"));
                        s.Append(" ");
                    }

                    s.Append((elements[slotno] != null) ? "*" : ".");
                }
                s.AppendLine();
                return s.ToString();
            }
        }

        public C FirstOccupied
        {
            get { return (occupiedCount > 0) ? elements[firstOccupied % capacity] : null; }
        }

        #region IOutgoingContainer<C> Members

        uint IOutgoingContainer<C>.Add(C element)
        {
            if (occupiedCount >= capacity)
            {
                uint new_capacity = (uint) Math.Ceiling(((double) capacity) * growthFactor);
                C[] new_elements = new C[new_capacity];
                for (uint ind = 0; ind < occupiedCount; ind++)
                {
                    uint seqno = firstOccupied + ind;
                    new_elements[seqno % new_capacity] = elements[seqno % capacity];
                }

                capacity = new_capacity;
                elements = new_elements;
            }

            uint added_seqno = firstOccupied + occupiedCount;
            occupiedCount++;
            elements[added_seqno % capacity] = element;

#if STATISTICS_TrackWindowContents
            double now = clock.Time;
            timeSeries_firstOccupied.addSample(now, firstOccupied);
            timeSeries_occupiedCount.addSample(now, occupiedCount);
#endif

            return added_seqno;
        }

        C IOutgoingContainer<C>.Remove(uint seqno)
        {
            C result_to_return;

            if (seqno >= firstOccupied && seqno < firstOccupied + occupiedCount)
            {
                uint slotno = seqno % capacity;
                if (elements[slotno] != null)
                {
                    C result = elements[slotno];
                    elements[slotno] = null;
                    if (seqno == firstOccupied) 
                    {
                        while (occupiedCount > 0 && elements[firstOccupied % capacity] == null)
                        {
                            firstOccupied++;
                            occupiedCount--;
                        }
                    }

                    result_to_return = result;
                }
                else
                    result_to_return = null;
            }
            else
                result_to_return = null;

#if STATISTICS_TrackWindowContents
            double now = clock.Time;
            timeSeries_firstOccupied.addSample(now, firstOccupied);
            timeSeries_occupiedCount.addSample(now, occupiedCount);
#endif

            return result_to_return;
        }

        C IOutgoingContainer<C>.this[uint seqno]
        {
            get { return (seqno >= firstOccupied && seqno < firstOccupied + occupiedCount) ? elements[seqno % capacity] : null; }
        }

        #endregion
    }
}
