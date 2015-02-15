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

namespace QS._qss_c_.Receivers4
{
    public class MessageRepository1<C> : IMessageRepository<C>
    {
        public MessageRepository1()
        {
        }

        private IDictionary<uint, C> messages = new Dictionary<uint, C>();
        private uint maximumCleaned;

        #region IMessageRepository<C> Members

        void IMessageRepository<C>.Add(uint sequenceNo, C message)
        {
            messages[sequenceNo] = message;
        }

        bool IMessageRepository<C>.Get(uint sequenceNo, ref C message)
        {
            return messages.TryGetValue(sequenceNo, out message);
        }

/*
        void IMessageRepository<C>.CleanUp(uint sequenceNo)
        {
            if (sequenceNo > maximumCleaned)
            {
                List<uint> toCleanUp = new List<uint>();
                foreach (uint seqno in messages.Keys)
                    if (seqno <= sequenceNo)
                        toCleanUp.Add(seqno);

                foreach (uint seqno in toCleanUp)
                    messages.Remove(seqno);

                maximumCleaned = sequenceNo;
            }
        }
*/

        IEnumerable<KeyValuePair<uint, C>> IMessageRepository<C>.CleanUp(uint sequenceNo)
        // CleanupCallback<C> cleanupCallback, object cleanupContext
        {
            if (sequenceNo > maximumCleaned)
            {
                List<KeyValuePair<uint, C>> toCleanUp = new List<KeyValuePair<uint, C>>();

                foreach (uint seqno in messages.Keys)
                {
                    if (seqno <= sequenceNo)
                        toCleanUp.Add(new KeyValuePair<uint, C>(seqno, messages[seqno]));
                }

                foreach (KeyValuePair<uint, C> element in toCleanUp)
                {
                    // cleanupCallback(seqno, messages[seqno], cleanupContext);
                    messages[element.Key] = default(C);
                    messages.Remove(element.Key);
                }

                maximumCleaned = sequenceNo;

                return toCleanUp;
            }
            else
                return null;
        }

        uint IMessageRepository<C>.MaximumCleaned
        {
            get { return maximumCleaned; }
        }

        IEnumerable<Base1_.Range<uint>> IMessageRepository<C>._Missing(uint maximumToCheck)
        {
            List<Base1_.Range<uint>> missing = new List<QS._qss_c_.Base1_.Range<uint>>();
            List<uint> acks = new List<uint>(messages.Keys);
            acks.Sort();
            uint current = maximumCleaned + 1;
            foreach (uint seqno in acks)
            {
                if (seqno > current)
                {
                    uint to = seqno - 1;
                    if (to > maximumToCheck)
                        to = maximumToCheck;
                    missing.Add(new Base1_.Range<uint>(current, to));
                    current = seqno + 1;
                }
                else
                    current++;

                if (current > maximumToCheck)
                    break;
            }
            if (current <= maximumToCheck)
                missing.Add(new Base1_.Range<uint>(current, maximumToCheck));
            return missing;
        }

        void IMessageRepository<C>.Find(IList<Base1_.Range<uint>> tofind, out IList<Base1_.Range<uint>> found, out IList<Base1_.Range<uint>> missing)
        {
            Base1_.Ranges.Builder foundbuilder = default(Base1_.Ranges.Builder), missingbuilder = default(Base1_.Ranges.Builder);
            foreach (Base1_.Range<uint> range in tofind)
                for (uint seqno = Math.Max(range.From, maximumCleaned + 1); seqno < range.To; seqno++)
                    (messages.ContainsKey(seqno) ? foundbuilder : missingbuilder).Add(seqno);

            found = foundbuilder.Ranges;
            missing = missingbuilder.Ranges;
        }

        #endregion

        #region ToString

        private const uint LineSize = 10;
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("count: ");
            s.Append(messages.Count.ToString());
            List<uint> seqnoCollection = new List<uint>(messages.Keys);
            seqnoCollection.Sort();
            bool newline = true, isfirst = true;
            uint lastused = 0, lastnewlineseqno = 0;
            foreach (uint element in seqnoCollection)
            {
                if (!isfirst)
                    for (uint seqno = lastused + 1; seqno < element; seqno++)
                        _ToString(s, seqno, ref newline, ref lastnewlineseqno, false);
                else
                    isfirst = false;

                _ToString(s, element, ref newline, ref lastnewlineseqno, true);
                lastused = element;
            }
            return s.ToString();
        }

        private void _ToString(StringBuilder s, uint seqno, ref bool newline, ref uint lastnewlineseqno, bool exists)
        {
            if (newline)
            {
                lastnewlineseqno = seqno + LineSize - 1;
                s.AppendLine();
                s.Append(seqno.ToString("000000000"));
                s.Append("-");
                s.Append(lastnewlineseqno.ToString("000000000"));
                s.Append(" ");
                newline = false;
            }

            s.Append(exists ? "X" : "o");

            if (seqno == lastnewlineseqno)
                newline = true;
        }

        #endregion
    }
}
