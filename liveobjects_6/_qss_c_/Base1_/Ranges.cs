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

namespace QS._qss_c_.Base1_
{
    public static class Ranges
    {
        #region FromString and ToString

        public static IList<Base1_.Range<uint>> FromString(string s)
        {
            List<Base1_.Range<uint>> ranges = new List<Range<uint>>();
            int left = 0;
            while (left < s.Length)
            {
                int right = s.IndexOf(",", left);
                if (right < left || right > s.Length)
                    right = s.Length;
                string element = s.Substring(left, right - left).Trim();

                if (element.Length > 0)
                {
                    int dash = element.IndexOf("-");
                    if (dash >= 0 && dash < element.Length)
                        ranges.Add(new Range<uint>(Convert.ToUInt32(element.Substring(0, dash)), Convert.ToUInt32(element.Substring(dash + 1))));
                    else
                    {
                        uint num = Convert.ToUInt32(element); 
                        ranges.Add(new Range<uint>(num, num));
                    }
                }
                else
                    break;

                left = right + 1;
            }
            return ranges;
        }

        public static string ToString(IList<Base1_.Range<uint>> ranges)
        {
            return QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base1_.Range<uint>>(ranges, ",");
        }

        #endregion

        #region Adding

        public static void Add(ref IList<Base1_.Range<uint>> existing, uint seqno)
        {
            Builder builder = default(Builder);

            IEnumerator<Base1_.Range<uint>> en = existing.GetEnumerator();           
            bool more = en.MoveNext();

            while (more && en.Current.From <= seqno)
            {
                builder.Add(en.Current);
                more = en.MoveNext();
            }

            builder.Add(seqno);

            while (more)
            {
                builder.Add(en.Current);
                more = en.MoveNext();
            }

            existing = builder.Ranges;
        }

        public static bool Add(ref IList<Base1_.Range<uint>> existing, IList<Base1_.Range<uint>> toadd)
        {
            IList<Base1_.Range<uint>> merged;
            IList<Base1_.Range<uint>> newlyadded;
            Add(existing, toadd, out merged, out newlyadded);
            existing = merged;
            return newlyadded.Count > 0;
        }

        public static void Add(ref IList<Base1_.Range<uint>> existing, IList<Base1_.Range<uint>> toadd, out IList<Base1_.Range<uint>> newlyadded)
        {
            IList<Base1_.Range<uint>> merged;
            Add(existing, toadd, out merged, out newlyadded);
            existing = merged;
        }

        public static void Add(IList<Base1_.Range<uint>> existing, IList<Base1_.Range<uint>> toadd, 
            out IList<Base1_.Range<uint>> merged, out IList<Base1_.Range<uint>> newlyadded)
        {
            IEnumerator<Base1_.Range<uint>> en_existing = existing.GetEnumerator();
            IEnumerator<Base1_.Range<uint>> en_toadd = toadd.GetEnumerator();

            merged = new List<Base1_.Range<uint>>();
            newlyadded = new List<Base1_.Range<uint>>();

            bool done_existing = !en_existing.MoveNext();
            bool done_toadd = !en_toadd.MoveNext();

            bool merging = false;
            Range<uint> tomerge = new Range<uint>(0, 0);

            while (!done_existing || !done_toadd)
            {
                if (!done_existing)
                {
                    if (!done_toadd)
                    {
                        if (en_toadd.Current.From < en_existing.Current.From)
                        {
                            uint from = Math.Max(en_toadd.Current.From, tomerge.To + 1);
                            uint to =  Math.Min(en_toadd.Current.To, en_existing.Current.From - 1);
                            if (from <= to)
                                newlyadded.Add(new Range<uint>(from, to));

                            while (!done_existing && en_existing.Current.To < en_toadd.Current.To)
                            {
                                from = en_existing.Current.To + 1;
                                to = en_toadd.Current.To;

                                done_existing = !en_existing.MoveNext();
                                if (!done_existing)
                                    to = Math.Min(to, en_existing.Current.From - 1);

                                if (from <= to)
                                    newlyadded.Add(new Range<uint>(from, to));
                            }

                            _updatemerged(ref merging, ref tomerge, en_toadd.Current, merged);
                            done_toadd = !en_toadd.MoveNext();
                        }
                        else if (en_toadd.Current.To <= en_existing.Current.From)
                        {
                            done_toadd = !en_toadd.MoveNext(); // redundant
                        }
                        else if (en_toadd.Current.From > en_existing.Current.To)
                        {
                            _updatemerged(ref merging, ref tomerge, en_existing.Current, merged);
                            done_existing = !en_existing.MoveNext();
                        }
                        else // old.from <= new.from <= old.to < new.to
                        {
                            _updatemerged(ref merging, ref tomerge, en_existing.Current, merged);
                            done_existing = !en_existing.MoveNext();
                        }
                    }
                    else
                    {
                        _updatemerged(ref merging, ref tomerge, en_existing.Current, merged);
                        done_existing = !en_existing.MoveNext();
                    }
                }
                else
                {
                    uint from = Math.Max(en_toadd.Current.From, tomerge.To + 1);
                    uint to = en_toadd.Current.To;
                    if (from <= to)
                        newlyadded.Add(new Range<uint>(from, to));

                    _updatemerged(ref merging, ref tomerge, en_toadd.Current, merged);
                    done_toadd = !en_toadd.MoveNext();
                }
            }

            if (merging)
                merged.Add(tomerge);
        }

        private static void _updatemerged(ref bool merging, ref Range<uint> tomerge, Range<uint> range, IList<Base1_.Range<uint>> merged)
        {
            if (merging)
            {
                if (range.From > tomerge.To + 1)
                {
                    merged.Add(tomerge);
                    tomerge = range;
                }
                else
                {
                    if (range.To > tomerge.To)
                        tomerge.To = range.To;
                }
            }
            else
            {
                tomerge = range;
                merging = true;
            }
        }

        #endregion

        #region FromBools

        public static IList<Range<uint>> FromBools(Range<uint> interval, bool[] bools)
        {
            if (bools.Length != (interval.To - interval.From + 1))
                throw new Exception("The supplied bool array is of the wrong size.");

            List<Range<uint>> merged = new List<Range<uint>>();

            bool merging = false;
            Range<uint> tomerge = new Range<uint>(0, 0);

            for (uint ind = 0; ind < bools.Length; ind++)
            {
                if (bools[(int) ind])
                {
                    if (merging)
                        tomerge.To++;
                    else
                    {
                        merging = true;
                        tomerge.From = tomerge.To = interval.From + ind;
                    }
                }
                else
                {
                    if (merging)
                    {
                        merged.Add(tomerge);
                        merging = false;
                    }
                }
            }

            if (merging)
                merged.Add(tomerge);

            return merged;
        }

        #endregion

        #region Generate random (for testing)

        public static IList<Range<uint>> Random(uint from, uint to, double missratio)
        {
            return Random(new Range<uint>(from, to), missratio);
        }

        public static IList<Range<uint>> Random(Range<uint> interval, double missratio)
        {
            bool[] bools = new bool[interval.To - interval.From + 1];
            for (int ind = 0; ind < bools.Length; ind++)
                bools[ind] = QS._qss_c_.Base1_.Random.Common.NextDouble() > missratio;
            return FromBools(interval, bools);
        }

        #endregion

        #region Unit testing

        public static void TestAdding1()
        {
            for (int i = 0; i < 10000; i++)
            {
                QS._qss_c_.Base1_.Range<uint> interval = new QS._qss_c_.Base1_.Range<uint>(1, 20);

                IList<QS._qss_c_.Base1_.Range<uint>> existing = QS._qss_c_.Base1_.Ranges.Random(interval, 0.2);
                IList<QS._qss_c_.Base1_.Range<uint>> toadd = QS._qss_c_.Base1_.Ranges.Random(interval, 0.2);

                IList<QS._qss_c_.Base1_.Range<uint>> merged, newlyadded;
                QS._qss_c_.Base1_.Ranges.Add(existing, toadd, out merged, out newlyadded);

/*
                Console.WriteLine("X:\t" + QS.CMS.Base.Ranges.ToString(existing));
                Console.WriteLine("Y:\t" + QS.CMS.Base.Ranges.ToString(toadd));
                Console.WriteLine("S:\t" + QS.CMS.Base.Ranges.ToString(merged));
                Console.WriteLine("N:\t" + QS.CMS.Base.Ranges.ToString(newlyadded));
*/

                System.Collections.ObjectModel.Collection<uint> c1 = new System.Collections.ObjectModel.Collection<uint>();
                System.Collections.ObjectModel.Collection<uint> c2 = new System.Collections.ObjectModel.Collection<uint>();
                System.Collections.ObjectModel.Collection<uint> c3 = new System.Collections.ObjectModel.Collection<uint>();
                System.Collections.ObjectModel.Collection<uint> c4 = new System.Collections.ObjectModel.Collection<uint>();
                System.Collections.ObjectModel.Collection<uint> c5 = new System.Collections.ObjectModel.Collection<uint>();

                foreach (QS._qss_c_.Base1_.Range<uint> range in existing)
                    for (uint seqno = range.From; seqno <= range.To; seqno++)
                    {
                        c1.Add(seqno);
                        c3.Add(seqno);
                    }

                foreach (QS._qss_c_.Base1_.Range<uint> range in toadd)
                    for (uint seqno = range.From; seqno <= range.To; seqno++)
                    {
                        c1.Add(seqno);
                        c4.Add(seqno);
                    }

                foreach (QS._qss_c_.Base1_.Range<uint> range in merged)
                    for (uint seqno = range.From; seqno <= range.To; seqno++)
                        c2.Add(seqno);

                foreach (QS._qss_c_.Base1_.Range<uint> range in newlyadded)
                    for (uint seqno = range.From; seqno <= range.To; seqno++)
                        c5.Add(seqno);

                foreach (uint seqno in c1)
                    if (!c2.Contains(seqno))
                        throw new Exception();

                foreach (uint seqno in c2)
                    if (!c1.Contains(seqno))
                        throw new Exception();

                foreach (uint seqno in c5)
                    if (!c4.Contains(seqno))
                        throw new Exception();

                foreach (uint seqno in c5)
                    if (c3.Contains(seqno))
                        throw new Exception();
            }

/*
            Console.WriteLine("OK");
*/ 
        }

        public static void TestAdding2()
        {
            for (int ind = 0; ind < 10000; ind++)
            {
                IList<QS._qss_c_.Base1_.Range<uint>> original = QS._qss_c_.Base1_.Ranges.Random(1, 20, 0.2);
                IList<QS._qss_c_.Base1_.Range<uint>> ranges = original;
                uint toadd = (uint)Base1_.Random.Common.Next(1, 20);
                ICollection<uint> rc = QS._qss_c_.Base1_.Ranges.ToCollection(ranges);
                if (!rc.Contains(toadd))
                    rc.Add(toadd);
                QS._qss_c_.Base1_.Ranges.Add(ref ranges, toadd);
                ICollection<uint> rx = QS._qss_c_.Base1_.Ranges.ToCollection(ranges);

                if (!QS._qss_c_.Base1_.Ranges.CollectionsEqual(rc, rx))
                    throw new Exception("Bad: (" + ToString(original) + ") + (" + toadd.ToString() + ") = (" + ToString(ranges) + ")");
            }
        }

        public static void TestRemoving1()
        {
            for (int i = 0; i < 10000; i++)
            {
                QS._qss_c_.Base1_.Range<uint> interval = new QS._qss_c_.Base1_.Range<uint>(1, 20);

                IList<QS._qss_c_.Base1_.Range<uint>> existing = QS._qss_c_.Base1_.Ranges.Random(interval, 0.2);
                IList<QS._qss_c_.Base1_.Range<uint>> toremove = QS._qss_c_.Base1_.Ranges.Random(interval, 0.2);

                IList<QS._qss_c_.Base1_.Range<uint>> result, removed;
                QS._qss_c_.Base1_.Ranges.Remove(existing, toremove, out result, out removed);

                try
                {
                    System.Collections.ObjectModel.Collection<uint> c1 = new System.Collections.ObjectModel.Collection<uint>();
                    System.Collections.ObjectModel.Collection<uint> c2 = new System.Collections.ObjectModel.Collection<uint>();
                    System.Collections.ObjectModel.Collection<uint> c3 = new System.Collections.ObjectModel.Collection<uint>();
                    System.Collections.ObjectModel.Collection<uint> c4 = new System.Collections.ObjectModel.Collection<uint>();
                    System.Collections.ObjectModel.Collection<uint> c5 = new System.Collections.ObjectModel.Collection<uint>();
                    System.Collections.ObjectModel.Collection<uint> c6 = new System.Collections.ObjectModel.Collection<uint>();

                    foreach (QS._qss_c_.Base1_.Range<uint> range in existing)
                        for (uint seqno = range.From; seqno <= range.To; seqno++)
                        {
                            c1.Add(seqno);
                            c5.Add(seqno);
                        }

                    foreach (QS._qss_c_.Base1_.Range<uint> range in toremove)
                        for (uint seqno = range.From; seqno <= range.To; seqno++)
                        {
                            c2.Add(seqno);
                            c5.Remove(seqno);
                            if (c1.Contains(seqno))
                                c6.Add(seqno);
                        }

                    foreach (QS._qss_c_.Base1_.Range<uint> range in result)
                        for (uint seqno = range.From; seqno <= range.To; seqno++)
                            c3.Add(seqno);

                    foreach (QS._qss_c_.Base1_.Range<uint> range in removed)
                        for (uint seqno = range.From; seqno <= range.To; seqno++)
                            c4.Add(seqno);

                    foreach (uint seqno in c5)
                        if (!c3.Contains(seqno))
                            throw new Exception("Result is missing { " + seqno.ToString() + " }.");

                    foreach (uint seqno in c3)
                        if (!c5.Contains(seqno))
                            throw new Exception("Result incorrectly includes { " + seqno.ToString() + " }.");

                    foreach (uint seqno in c6)
                        if (!c4.Contains(seqno))
                            throw new Exception("Removed is missing { " + seqno.ToString() + " }.");

                    foreach (uint seqno in c4)
                        if (!c6.Contains(seqno))
                            throw new Exception("Removed incorrectly includes { " + seqno.ToString() + " }.");
                }
                catch (Exception exc)
                {
                    StringBuilder s = new StringBuilder();
                    s.AppendLine("x\t = \t" + ToString(existing));
                    s.AppendLine("y\t = \t" + ToString(toremove));
                    s.AppendLine("x-y\t = \t" + ToString(result));
                    s.AppendLine("x*y\t = \t" + ToString(removed));

                    throw new Exception("Removing test failed.\n" + s.ToString(), exc);
                }
            }
        }

        public static void TestRemoving2()
        {
            for (int ind = 0; ind < 10000; ind++)
            {
                IList<QS._qss_c_.Base1_.Range<uint>> original = QS._qss_c_.Base1_.Ranges.Random(1, 20, 0.2);
                IList<QS._qss_c_.Base1_.Range<uint>> ranges = original;
                uint toremove = (uint) Base1_.Random.Common.Next(1, 20);
                ICollection<uint> rc = QS._qss_c_.Base1_.Ranges.ToCollection(ranges);
                rc.Remove(toremove);
                QS._qss_c_.Base1_.Ranges.Remove(ref ranges, toremove);
                ICollection<uint> rx = QS._qss_c_.Base1_.Ranges.ToCollection(ranges);

                if (!QS._qss_c_.Base1_.Ranges.CollectionsEqual(rc, rx))
                    throw new Exception("Bad: (" + ToString(original) + ") - (" + toremove.ToString() + ") = (" + ToString(ranges) + ")");
            }
        }

        #endregion

        #region Builder

        public struct Builder
        {
            private bool merging;
            private Range<uint> tomerge;
            private IList<Range<uint>> merged;

            public void Add(uint seqno)
            {
                this.Add(new Range<uint>(seqno, seqno));
            }

            public void Add(uint from, uint to)
            {
                this.Add(new Range<uint>(from, to));
            }

            public void Add(Range<uint> range)
            {
                if (range.To >= range.From)
                {
                    if (merged == null)
                        merged = new List<Range<uint>>();

                    if (merging)
                    {
                        if (range.From > tomerge.To + 1)
                        {
                            merged.Add(tomerge);
                            tomerge = range;
                        }
                        else
                        {
                            if (range.To > tomerge.To)
                                tomerge.To = range.To;
                        }
                    }
                    else
                    {
                        tomerge = range;
                        merging = true;
                    }
                }
            }

            public IList<Range<uint>> Ranges
            {
                get
                {
                    if (merged == null)
                        merged = new List<Range<uint>>();

                    if (merging)
                    {
                        merged.Add(tomerge);
                        merging = false;
                    }

                    return merged;
                }
            }
        }

        #endregion

        #region Trimming

        public static void Trim(ref IList<Range<uint>> ranges, uint maxtotrim)
        {
            ranges = Trim(ranges, maxtotrim);
        }

        public static IList<Range<uint>> Trim(IList<Range<uint>> ranges, uint maxtotrim)
        {
            if (ranges.Count == 0 || maxtotrim < ranges[0].From)
                return ranges;
            else
            {
                List<Range<uint>> trimmed = new List<Range<uint>>();
                foreach (Range<uint> range in ranges)
                    if (range.To > maxtotrim)
                        trimmed.Add(new Range<uint>(Math.Max(range.From, maxtotrim + 1), range.To));
                return trimmed;
            }
        }

        #endregion

        #region Removing

        public static void Remove(ref IList<Base1_.Range<uint>> existing, uint seqno)
        {
            Builder builder = default(Builder);

            IEnumerator<Base1_.Range<uint>> en = existing.GetEnumerator();
            bool more = en.MoveNext();

            while (more && en.Current.To < seqno)
            {
                builder.Add(en.Current);
                more = en.MoveNext();
            }

            if (more)
            {
                if (en.Current.From <= seqno)
                {
                    builder.Add(en.Current.From, seqno - 1);
                    builder.Add(seqno + 1, en.Current.To);
                    more = en.MoveNext();
                }
                
                while (more)
                {
                    builder.Add(en.Current);
                    more = en.MoveNext();
                }
            }

            existing = builder.Ranges;
        }

        public static void Remove(ref IList<Base1_.Range<uint>> existing, uint maxtotrim, IList<Base1_.Range<uint>> toremove)
        {
            IList<Base1_.Range<uint>> result;
            IList<Base1_.Range<uint>> removed;
            Remove(Trim(existing, maxtotrim), toremove, out result, out removed);
            existing = result;
        }

        public static void Remove(ref IList<Base1_.Range<uint>> existing, IList<Base1_.Range<uint>> toremove)
        {
            IList<Base1_.Range<uint>> result;
            IList<Base1_.Range<uint>> removed;
            Remove(existing, toremove, out result, out removed);
            existing = result;
        }

        public static void Remove(IList<Base1_.Range<uint>> existing, IList<Base1_.Range<uint>> toremove,
            out IList<Base1_.Range<uint>> result, out IList<Base1_.Range<uint>> removed)
        {
            IEnumerator<Base1_.Range<uint>> en_existing = existing.GetEnumerator();
            IEnumerator<Base1_.Range<uint>> en_toremove = toremove.GetEnumerator();

            bool done_existing = !en_existing.MoveNext();
            bool done_toremove = !en_toremove.MoveNext();

            Builder result_builder = default(Builder), removed_builder = default(Builder);
            uint covered = 0;

            while (!done_existing || !done_toremove)
            {
                if (!done_existing) // (a <= b)
                {
                    if (!done_toremove) // (c <= d)
                    {
                        if (en_existing.Current.From < en_toremove.Current.From) // a < c
                        {
                            if (en_existing.Current.To < en_toremove.Current.From) // a <= b < c 
                            {
                                result_builder.Add(Math.Max(en_existing.Current.From, covered + 1), en_existing.Current.To);
                                covered = Math.Max(covered, en_existing.Current.To);
                                done_existing = !en_existing.MoveNext();
                            }
                            else // a < c <= b
                            {
                                if (en_toremove.Current.To > en_existing.Current.To) // a < c <= b < d
                                {
                                    result_builder.Add(Math.Max(en_existing.Current.From, covered + 1), en_toremove.Current.From - 1);
                                    removed_builder.Add(Math.Max(en_toremove.Current.From, covered + 1), en_existing.Current.To);
                                    covered = Math.Max(covered, en_existing.Current.To);
                                    done_existing = !en_existing.MoveNext();
                                }
                                else // a < c,d <= b
                                {
                                    result_builder.Add(Math.Max(en_existing.Current.From, covered + 1), en_toremove.Current.From - 1);
                                    removed_builder.Add(Math.Max(en_toremove.Current.From, covered + 1), en_toremove.Current.To);
                                    covered = Math.Max(covered, en_toremove.Current.To);
                                    done_toremove = !en_toremove.MoveNext();
                                }
                            }
                        }
                        else // c <= a <= b
                        {
                            if (en_toremove.Current.To < en_existing.Current.From) // c <= d < a <= b
                            {
                                done_toremove = !en_toremove.MoveNext();
                            }
                            else // c <= a <= b, a <= d
                            {
                                if (en_toremove.Current.To > en_existing.Current.To) // c <= a <= b < d
                                {
                                    removed_builder.Add(Math.Max(en_existing.Current.From, covered + 1), en_existing.Current.To);
                                    covered = Math.Max(covered, en_existing.Current.To);
                                    done_existing = !en_existing.MoveNext();
                                }
                                else // c <= a,d <= b
                                {
                                    removed_builder.Add(Math.Max(en_existing.Current.From, covered + 1), en_toremove.Current.To);
                                    covered = Math.Max(covered, en_toremove.Current.To);
                                    done_toremove = !en_toremove.MoveNext();
                                }
                            }
                        }
                    }
                    else
                    {
                        result_builder.Add(Math.Max(en_existing.Current.From, covered + 1), en_existing.Current.To);
                        covered = Math.Max(covered, en_existing.Current.To);
                        done_existing = !en_existing.MoveNext();
                    }
                }
                else
                {
                    break;
                }
            }

            result = result_builder.Ranges;
            removed = removed_builder.Ranges;
        }

        #endregion

        #region Operaitons on collections

        public static ICollection<uint> ToCollection(IList<Range<uint>> ranges)
        {
            ICollection<uint> collection = new System.Collections.ObjectModel.Collection<uint>();
            foreach (Range<uint> range in ranges)
                for (uint seqno = range.From; seqno <= range.To; seqno++)
                    collection.Add(seqno);
            return collection;
        }

        /// <summary>
        /// Warning: This operation is slow.
        /// </summary>
        public static bool CollectionsEqual(ICollection<uint> c1, ICollection<uint> c2)
        {
            foreach (uint x in c1)
                if (!c2.Contains(x))
                    return false;

            foreach (uint x in c2)
                if (!c1.Contains(x))
                    return false;

            return true;
        }

        #endregion
    }
}
