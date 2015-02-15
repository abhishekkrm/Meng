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

namespace QS._qss_c_.Receivers5
{
    [QS.Fx.Base.Inspectable]
    public class PendingCollection<C> : QS.Fx.Inspection.Inspectable, IPendingCollection<C> where C : class
    {
        public PendingCollection()
        {
        }

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Implicit)]
        [QS.Fx.Base.Inspectable]
        private struct Entry : QS.Fx.Inspection.IInspectable
        {
            public bool removed;
            public C data;

            public override string ToString()
            {
                return QS.Fx.Printing.Printable.ToString(this);
            }

            #region IInspectable Members

            QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
            {
                get 
                {
                    QS.Fx.Inspection.AttributeCollection collection = new QS.Fx.Inspection.AttributeCollection("attributes");
                    collection.Add(new QS.Fx.Inspection.ScalarAttribute("removed", removed));
                    collection.Add(new QS.Fx.Inspection.ScalarAttribute("data", data));
                    return collection;
                }
            }

            #endregion
        }

        private Entry[] entries;
        private uint minimum = 1;

        #region ToString

        private const int width = 100;
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            uint from = minimum;
            while (from < minimum + entries.Length)
            {
                uint to = (uint) Math.Min((int) from + width - 1, (int) minimum + entries.Length - 1);
                s.Append(from.ToString("0000000"));
                s.Append("-");
                s.Append(to.ToString("0000000"));
                s.Append(" ");
                for (int ind = (int) from; ind <= (int) to; ind++)
                    s.Append(entries[ind % entries.Length].removed ? "x" : ((entries[ind % entries.Length].data != null) ? "o" : "_"));
                from = to + 1;
            }
            return s.ToString();
        }

        #endregion

        #region IPendingCollection<C> Members

        bool IPendingCollection<C>.Add(uint seqno, C data)
        {
            if (entries == null)
                entries = new Entry[Math.Max(seqno, 10)];
            else
            {
                int min_length = (int) (seqno - minimum + 1);
                if (entries.Length < min_length)
                {
                    Entry[] new_entries = new Entry[Math.Max(min_length, entries.Length * 2)];
                    for (int ind = (int)minimum; ind < minimum + entries.Length; ind++)
                        new_entries[ind % new_entries.Length] = entries[ind % entries.Length];
                    entries = new_entries;
                }                
            }
            bool added_new = entries[((int)seqno) % entries.Length].data == null;
            entries[((int)seqno) % entries.Length].data = data;
            return added_new;
        }

        bool IPendingCollection<C>.Remove(uint seqno, out C data)
        {
            if (seqno < minimum || seqno >= minimum + entries.Length)
            {
                data = null;
                return false;
            }
            else
            {
                if (entries[seqno % entries.Length].removed)
                {
                    data = null;
                    return false;
                }
                else
                {
                    data = entries[seqno % entries.Length].data;

                    entries[seqno % entries.Length].removed = true;
                    entries[seqno % entries.Length].data = null;

                    while (entries[minimum % entries.Length].removed)
                    {
                        entries[minimum % entries.Length] = default(Entry);
                        minimum++;
                    }
                    return true;
                }
            }
        }

        void IPendingCollection<C>.Remove(IEnumerable<Base1_.Range<uint>> ranges, out IEnumerable<C> data)
        {
            List<C> returned_data = new List<C>();
            foreach (Base1_.Range<uint> range in ranges)
            {
                for (uint seqno = Math.Max(range.From, minimum); seqno <= range.To; seqno++)
                {
                    if (!entries[seqno % entries.Length].removed)
                    {
                        C element = entries[seqno % entries.Length].data;
                        if (element != null)
                            returned_data.Add(element);

                        entries[seqno % entries.Length].removed = true;
                        entries[seqno % entries.Length].data = null;
                    }
                }
            }
            data = returned_data;
            while (entries[minimum % entries.Length].removed)
            {
                entries[minimum % entries.Length] = default(Entry);
                minimum++;
            }
        }

        #endregion
    }
}
