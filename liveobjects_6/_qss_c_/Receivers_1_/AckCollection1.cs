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

namespace QS._qss_c_.Receivers_1_
{
    [QS.Fx.Base.Inspectable]
    public class AckCollection1 : QS.Fx.Inspection.Inspectable, IAckCollection    
    {
        private const uint DefaultWindowSize = 100;

        public AckCollection1() : this(DefaultWindowSize)
        {
        }

        public AckCollection1(uint windowSize)
        {
            this.windowSize = windowSize;
            window = new bool[windowSize];
        }

        private uint windowSize, minimumSeqNo = 1, maximumSeqNo = 0, lastChecked = 0; 
        private bool[] window;
        private ICollection<uint> missedCollection = new System.Collections.ObjectModel.Collection<uint>();

        #region Internal Processing

        private void adjustMaximum(uint sequenceNo)
        {
            while (minimumSeqNo + windowSize <= sequenceNo)
            {
                if (minimumSeqNo > maximumSeqNo || !window[minimumSeqNo % windowSize])
                    missedCollection.Add(minimumSeqNo);
                minimumSeqNo++;
            }

            while (maximumSeqNo < sequenceNo)
            {
                maximumSeqNo++;
                window[maximumSeqNo % windowSize] = false;
            }
        }

        #endregion

        #region IAckCollection Members

        bool IAckCollection.Add(uint sequenceNo)
        {
            if (sequenceNo < minimumSeqNo)
            {
                if (sequenceNo > maximumSeqNo)
                    maximumSeqNo = sequenceNo;
                return missedCollection.Remove(sequenceNo);
            }
            else if (sequenceNo > maximumSeqNo)
            {
                adjustMaximum(sequenceNo);

                if (sequenceNo == minimumSeqNo)
                    minimumSeqNo++;
                else
                    window[sequenceNo % windowSize] = true;
                return true;
            }
            else
            {
                bool added_now = !window[sequenceNo % windowSize];
                window[sequenceNo % windowSize] = true;

                if (added_now && sequenceNo == minimumSeqNo)
                {
                    do
                    {
                        minimumSeqNo++;
                    }
                    while (minimumSeqNo <= maximumSeqNo && window[minimumSeqNo % windowSize] == true);                        
                }

                return added_now;
            }
        }

        uint IAckCollection.Maximum
        {
            set
            {
                if (value > maximumSeqNo)
                    adjustMaximum(value);
            }

            get { return maximumSeqNo; }
        }

        // ............................................................................................................................................................................

        IEnumerable<uint> IAckCollection.Missing
        {
            get 
            {
                while (minimumSeqNo <= lastChecked)
                {
                    if (!window[minimumSeqNo % windowSize])
                        missedCollection.Add(minimumSeqNo);
                    minimumSeqNo++;
                }
                lastChecked = maximumSeqNo;

                return missedCollection; 
            }
        }

        IEnumerable<uint> IAckCollection.CutOff(uint cutoffSeqNo)
        {
            while (minimumSeqNo <= cutoffSeqNo)
            {
                if (!window[minimumSeqNo % windowSize])
                    missedCollection.Add(minimumSeqNo);
                minimumSeqNo++;
            }

            return missedCollection; 
        }

        #endregion

        #region Printing

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("(min:");
            s.Append(minimumSeqNo.ToString());
            s.Append(", \"");
            for (uint seqno = minimumSeqNo; seqno <= maximumSeqNo; seqno++)
                s.Append(window[seqno % windowSize] ? "x" : ".");
            s.Append("\", max:");
            s.Append(maximumSeqNo.ToString());
            s.Append(", miss:");
            bool first = true;
            foreach (uint x in missedCollection)
            {
                if (first)
                    first = false;
                else
                    s.Append(",");
                s.Append(x.ToString());
            }
            s.Append(")");
            return s.ToString();
        }

        #endregion
    }
}
