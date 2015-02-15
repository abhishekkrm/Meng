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

namespace QS._qss_c_.Senders3
{
    public interface IChoppable
    {
        QS.Fx.Serialization.ISerializable ChopOff(int maximumSize);
    }

    public class Choppable : IChoppable
    {
        public Choppable(QS.Fx.Serialization.ISerializable objectToChop)
        {
            segments = QS._core_c_.Base3.Serializer.FlattenObject(objectToChop);
            segment_no = consumed_inseg = 0;
        }

        private IList<QS.Fx.Base.Block> segments;
        private int segment_no, consumed_inseg;

        #region IChoppable Members

        QS.Fx.Serialization.ISerializable IChoppable.ChopOff(int maximumSize)
        {
            return this.ChopOff(maximumSize);
        }

        #endregion

        public Base3_.Segments ChopOff(int maximumSize)
        {
            maximumSize -= Base3_.Segments.HeaderOverhead;
            if (maximumSize < 0)
                throw new Exception("Cannot chop off, too small a bite.");

            if (segment_no < segments.Count)
            {
                List<QS.Fx.Base.Block> segmentsChoppedOff = new List<QS.Fx.Base.Block>();

                do
                {
                    QS.Fx.Base.Block this_segment = segments[segment_no];
                    this_segment.offset += (uint) consumed_inseg;
                    this_segment.size -= (uint) consumed_inseg;

                    if (this_segment.size <= maximumSize)
                    {
                        segmentsChoppedOff.Add(this_segment);
                        maximumSize -= (int) this_segment.size;
                        segment_no++;
                        consumed_inseg = 0;
                    }
                    else
                    {
                        this_segment.size = (uint) maximumSize;
                        segmentsChoppedOff.Add(this_segment);
                        consumed_inseg += maximumSize;
                        maximumSize = 0;
                    }
                }
                while (maximumSize > 0 && segment_no < segments.Count);

                return new QS._qss_c_.Base3_.Segments(segmentsChoppedOff);
            }
            else
                return null;            
        }
    }
}
