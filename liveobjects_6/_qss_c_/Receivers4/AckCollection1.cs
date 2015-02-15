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

// #define DEBUG_CollectStatistics
// #define DEBUG_AutomaticChecking

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Receivers4
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class AckCollection1 : QS.Fx.Inspection.Inspectable, IAckCollection
    {
        private const int Default_MinimumWindowSize = 8;

        public AckCollection1() : this(null)
        {
        }

        public AckCollection1(QS.Fx.Clock.IClock clock)
        {
            this.clock = clock;

#if DEBUG_CollectStatistics
            if (clock == null)
                throw new Exception("Clock must not be NULL.");
#endif
        }

        
        private uint windowMin = 1, windowMax, windowSize, maximumUsed, maximumSeen, unacknowledgedCount;
        private System.Collections.BitArray window;
        private QS.Fx.Clock.IClock clock;

#if DEBUG_CollectStatistics
        [QS.CMS.Diagnostics.Component("Maximum Seen (X=time, Y=seqno)")]
        private Statistics.SamplesXY timeSeries_maximumSeen = new QS.CMS.Statistics.SamplesXY();

        // the value of (windowMin - 1)
        [QS.CMS.Diagnostics.Component("Maximum Contiguous (X=time, Y=seqno)")]
        private Statistics.SamplesXY timeSeries_maximumContiguous = new QS.CMS.Statistics.SamplesXY();

        // the value of (maximumUsed)
        [QS.CMS.Diagnostics.Component("Maximum Added (X=time, Y=seqno)")]
        private Statistics.SamplesXY timeSeries_maximumAdded = new QS.CMS.Statistics.SamplesXY();

        // defined as (maximumSeen - windowMin + 1), that is the size of the sequence being acknowledged at the time
        [QS.CMS.Diagnostics.Component("Window Width (X=time, Y=count)")]
        private Statistics.SamplesXY timeSeries_windowWidth = new QS.CMS.Statistics.SamplesXY();

        // defined as (maximumSeen - maximumUsed), i.e. the difference between the maximum seen in the group and seen locally
        [QS.CMS.Diagnostics.Component("Receiver Lag (X=time, Y=count)")]
        private Statistics.SamplesXY timeSeries_receiverLag = new QS.CMS.Statistics.SamplesXY();

        // defined as the number of holes in the range (1..maximumSeen), i.e. the number of known gaps to fill
        [QS.CMS.Diagnostics.Component("Unacknowledged Count (X=time, Y=count)")]
        private Statistics.SamplesXY timeSeries_unacknowledgedCount = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Internal Processing

        bool _Contains(uint sequenceNo)
        {
            if (sequenceNo < windowMin)
                return true;

            if (sequenceNo > windowMax)
                return false;

            return window.Get((int)(sequenceNo % windowSize));
        }

        bool _Add(uint sequenceNo)
        {
#if DEBUG_CollectStatistics
            double now = clock.Time;
#endif

            if (sequenceNo < windowMin)
                return false;

            if (sequenceNo > maximumSeen)
            {
                unacknowledgedCount += sequenceNo - maximumSeen;
                maximumSeen = sequenceNo;

#if DEBUG_CollectStatistics
                if (timeSeries_maximumSeen.Enabled)
                    timeSeries_maximumSeen.addSample(now, maximumSeen);

                if (timeSeries_windowWidth.Enabled)
                    timeSeries_windowWidth.addSample(now, maximumSeen - windowMin + 1);

                if (timeSeries_receiverLag.Enabled)
                    timeSeries_receiverLag.addSample(now, ((double)maximumSeen) - ((double)maximumUsed));

                if (timeSeries_unacknowledgedCount.Enabled)
                    timeSeries_unacknowledgedCount.addSample(now, unacknowledgedCount);
#endif
            }

            if (sequenceNo > maximumUsed)
            {
                maximumUsed = sequenceNo;

#if DEBUG_CollectStatistics
                if (timeSeries_maximumAdded.Enabled)
                    timeSeries_maximumAdded.addSample(now, maximumUsed);

                if (timeSeries_receiverLag.Enabled)
                    timeSeries_receiverLag.addSample(now, ((double)maximumSeen) - ((double)maximumUsed));
#endif
            }

            if (sequenceNo > windowMax)
            {
                if (windowSize == 0)
                {
                    windowSize = sequenceNo + 1;
                    if (windowSize < Default_MinimumWindowSize)
                        windowSize = Default_MinimumWindowSize;
                    windowSize = (uint)(1 << ((int)Math.Ceiling(Math.Log((double)windowSize, 2))));
                    windowMin = 1;
                    windowMax = windowSize;
                    window = new System.Collections.BitArray((int)windowSize, false);

#if DEBUG_CollectStatistics
                    if (timeSeries_maximumContiguous.Enabled)
                        timeSeries_maximumContiguous.addSample(now, windowMin - 1);

                    if (timeSeries_windowWidth.Enabled)
                        timeSeries_windowWidth.addSample(now, maximumSeen - windowMin + 1);
#endif
                }
                else
                {
                    uint newWindowSize = (uint)(((int)windowSize) *
                        (1 << ((int)Math.Ceiling(Math.Log(1 + ((double)(sequenceNo - windowMax)) / ((double)windowSize), 2)))));
                    System.Collections.BitArray newWindow = new System.Collections.BitArray((int)newWindowSize, false);
                    for (uint seqno = windowMin; seqno <= windowMax; seqno++)
                        if (window.Get((int)(seqno % windowSize)))
                            newWindow.Set((int)(seqno % newWindowSize), true);
                    window = newWindow;
                    windowSize = newWindowSize;
                    windowMax = windowMin + windowSize - 1;
                }

                window.Set((int)(sequenceNo % windowSize), true);
                unacknowledgedCount--;

                // new fix
                while (window.Get((int)(windowMin % windowSize)))
                {
                    window.Set((int)(windowMin % windowSize), false);
                    windowMin++;
                    windowMax++;
                }

#if DEBUG_CollectStatistics
                if (timeSeries_unacknowledgedCount.Enabled)
                    timeSeries_unacknowledgedCount.addSample(now, unacknowledgedCount);
#endif

                return true;
            }
            else
            {
                if (!window.Get((int)(sequenceNo % windowSize)))
                {
                    window.Set((int)(sequenceNo % windowSize), true);
                    unacknowledgedCount--;

                    while (window.Get((int)(windowMin % windowSize)))
                    {
                        window.Set((int)(windowMin % windowSize), false);
                        windowMin++;
                        windowMax++;
                    }

#if DEBUG_CollectStatistics
                    if (timeSeries_maximumContiguous.Enabled)
                        timeSeries_maximumContiguous.addSample(now, windowMin - 1);

                    if (timeSeries_windowWidth.Enabled)
                        timeSeries_windowWidth.addSample(now, maximumSeen - windowMin + 1);

                    if (timeSeries_unacknowledgedCount.Enabled)
                        timeSeries_unacknowledgedCount.addSample(now, unacknowledgedCount);
#endif

                    return true;
                }
                else
                    return false;
            }
        }

        #endregion

        #region IAckCollection Members

        uint IAckCollection.MaxContiguous
        {
            get { return windowMin - 1; }
        }

        bool IAckCollection.Add(uint sequenceNo)
        {
#if DEBUG_AutomaticChecking
            bool _result = _Contains(sequenceNo);
#endif

            bool result = _Add(sequenceNo);

#if DEBUG_AutomaticChecking
            if (!_Contains(sequenceNo) || (result != !_result))
                throw new Exception("Adding failed!");
#endif

            return result;
        }

        uint IAckCollection.MaximumSeen
        {
            get { return maximumSeen; }
        }

        void IAckCollection.Seen(uint sequenceNo)
        {
            if (sequenceNo > maximumSeen)
            {
                unacknowledgedCount += sequenceNo - maximumSeen;
                maximumSeen = sequenceNo;

#if DEBUG_CollectStatistics
                double now = clock.Time;

                if (timeSeries_maximumSeen.Enabled)
                    timeSeries_maximumSeen.addSample(now, maximumSeen);

                if (timeSeries_receiverLag.Enabled)
                    timeSeries_receiverLag.addSample(now, ((double)maximumSeen) - ((double)maximumUsed));

                if (timeSeries_unacknowledgedCount.Enabled)
                    timeSeries_unacknowledgedCount.addSample(now, unacknowledgedCount);
#endif
            }
        }

        IEnumerable<Base1_.Range<uint>> IAckCollection.Missing
        {
            get { return new MissingEnumerator(this); }
        }

        #endregion

        #region Class Enumerator

        private class MissingEnumerator : IEnumerable<Base1_.Range<uint>>, IEnumerator<Base1_.Range<uint>>
        {
            public MissingEnumerator(AckCollection1 ackCollection)
            {
                this.ackCollection = ackCollection;
            }

            private AckCollection1 ackCollection;
            private bool running, completed, last;
            private Base1_.Range<uint> current;

            #region IEnumerator<Range<uint>> Members

            QS._qss_c_.Base1_.Range<uint> IEnumerator<QS._qss_c_.Base1_.Range<uint>>.Current
            {
                get { return current; }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    if (!running || completed)
                        throw new Exception("Enumerator not yet started or already finished.");
                    return current; 
                }
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                if (last || completed)
                {
                    completed = true;
                    return false;
                }

                uint seqno;
                if (running)
                    seqno = current.To + 1;
                else
                {
                    seqno = ackCollection.windowMin;
                    running = true;
                }

                while (seqno <= ackCollection.maximumUsed && ackCollection.window.Get((int)(seqno % ackCollection.windowSize)))
                    seqno++;

                if (seqno > ackCollection.maximumUsed)
                {
                    if (ackCollection.maximumUsed < ackCollection.maximumSeen)
                    {
                        current = new QS._qss_c_.Base1_.Range<uint>(ackCollection.maximumUsed + 1, ackCollection.maximumSeen);
                        last = true;
                        return true;
                    }
                    else
                    {
                        completed = true;
                        return false;
                    }
                }
                else
                {
                    uint last_seqno = seqno;
                    while (last_seqno < ackCollection.maximumUsed &&
                        !ackCollection.window.Get((int)((last_seqno + 1) % ackCollection.windowSize)))
                        last_seqno++;

                    if (last_seqno < ackCollection.maximumUsed)
                    {
                        current = new QS._qss_c_.Base1_.Range<uint>(seqno, last_seqno);
                        return true;
                    }
                    else
                    {
                        current = new QS._qss_c_.Base1_.Range<uint>(seqno, ackCollection.maximumSeen);
                        last = true;
                        return true;
                    }
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                running = completed = last = false;
            }

            #endregion

            #region IEnumerable<Range<uint>> Members

            IEnumerator<QS._qss_c_.Base1_.Range<uint>> IEnumerable<QS._qss_c_.Base1_.Range<uint>>.GetEnumerator()
            {
                return this;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this;
            }

            #endregion
        }

        #endregion

        #region ToString

        private const uint ElementsPerRow = 100;
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("max. added: ");
            s.Append(maximumUsed.ToString());
            s.Append(", max. seen: ");
            s.AppendLine(maximumSeen.ToString());
            uint seqno = windowMin;
            while (seqno <= windowMax)
            {
                uint upto = seqno + ElementsPerRow - 1;
                if (upto > windowMax)
                    upto = windowMax;
                s.Append(seqno.ToString("000000000"));
                s.Append("-");
                s.Append(upto.ToString("000000000"));
                s.Append(" ");
                while (seqno <= upto)
                {
                    s.Append(window.Get((int)(seqno % windowSize)) ? "X" : "o");
                    seqno++;
                }
                s.AppendLine();
            }
            return s.ToString();
        }

        #endregion
    }
}
