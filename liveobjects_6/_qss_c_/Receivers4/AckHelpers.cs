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
    public static class AckHelpers
    {
        #region CompareNaks

        public static void CompareNaks(uint covered1, IList<Base1_.Range<uint>> missing1, uint covered2,
            IList<Base1_.Range<uint>> missing2, IList<Base1_.Range<uint>> forward1to2, IList<Base1_.Range<uint>> forward2to1)
        {
            /*
            #if DEBUG_ReceivingAgent1
                        StringBuilder ss = new StringBuilder();
                        ss.AppendLine("covered1 = " + covered1.ToString());
                        ss.AppendLine("missing1 = " + Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(missing1, ", "));
                        ss.AppendLine("covered2 = " + covered2.ToString());
                        ss.AppendLine("missing2 = " + Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(missing2, ", "));
                        ss.AppendLine("forward1to2 = " + Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(forward1to2, ", "));
                        ss.AppendLine("forward2to1 = " + Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(forward2to1, ", "));
                        log("CompareNaks", ss.ToString());
            #endif
            */

            IEnumerator<Base1_.Range<uint>> naks1 = missing1.GetEnumerator(), naks2 = missing2.GetEnumerator();
            bool finished1 = !naks1.MoveNext(), finished2 = !naks2.MoveNext();
            uint maximumCovered = covered1;
            if (covered2 < maximumCovered)
                maximumCovered = covered2;

            uint bothCovered = 0;
            while (bothCovered < maximumCovered)
            {
                while (!finished1 && naks1.Current.To <= bothCovered)
                    finished1 = !naks1.MoveNext();

                while (!finished2 && naks2.Current.To <= bothCovered)
                    finished2 = !naks2.MoveNext();

                if (finished1)
                {
                    if (finished2)
                    {
                        bothCovered = maximumCovered;
                        break;
                    }
                    else
                    {
                        if (naks2.Current.From > maximumCovered)
                            break;
                        else
                        {
                            uint min = bothCovered + 1;
                            if (naks2.Current.From > min)
                                min = naks2.Current.From;

                            if (naks2.Current.To < maximumCovered)
                            {
                                forward1to2.Add(new Base1_.Range<uint>(min, naks2.Current.To));
                                bothCovered = naks2.Current.To;
                                finished2 = !naks2.MoveNext();
                            }
                            else
                            {
                                forward1to2.Add(new Base1_.Range<uint>(min, maximumCovered));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (finished2)
                    {
                        if (naks1.Current.From > maximumCovered)
                            break;
                        else
                        {
                            uint min = bothCovered + 1;
                            if (naks1.Current.From > min)
                                min = naks1.Current.From;

                            if (naks1.Current.To < maximumCovered)
                            {
                                forward2to1.Add(new Base1_.Range<uint>(min, naks1.Current.To));
                                bothCovered = naks1.Current.To;
                                finished1 = !naks1.MoveNext();
                            }
                            else
                            {
                                forward2to1.Add(new Base1_.Range<uint>(min, maximumCovered));
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (naks1.Current.From > bothCovered + 1)
                        {
                            if (naks2.Current.From > bothCovered + 1)
                            {
                                bothCovered = naks1.Current.From - 1;
                                if (naks2.Current.From - 1 < bothCovered)
                                    bothCovered = naks2.Current.From - 1;
                            }
                            else
                            {
                                uint newCovered = naks1.Current.From - 1;
                                if (naks2.Current.To < newCovered)
                                    newCovered = naks2.Current.To;

                                forward1to2.Add(new Base1_.Range<uint>(bothCovered + 1, newCovered));
                                bothCovered = newCovered;
                            }
                        }
                        else
                        {
                            if (naks2.Current.From > bothCovered + 1)
                            {
                                uint newCovered = naks2.Current.From - 1;
                                if (naks1.Current.To < newCovered)
                                    newCovered = naks1.Current.To;

                                forward2to1.Add(new Base1_.Range<uint>(bothCovered + 1, newCovered));
                                bothCovered = newCovered;
                            }
                            else
                            {
                                if (naks1.Current.To < naks2.Current.To)
                                {
                                    bothCovered = naks1.Current.To;
                                    finished1 = !naks1.MoveNext();
                                }
                                else if (naks2.Current.To < naks1.Current.To)
                                {
                                    bothCovered = naks2.Current.To;
                                    finished2 = !naks2.MoveNext();
                                }
                                else
                                {
                                    bothCovered = naks1.Current.To;
                                    finished1 = !naks1.MoveNext();
                                    finished2 = !naks2.MoveNext();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region PartitionNaks

        public static IList<Base1_.Range<uint>>[] PartitionNaks(
            IEnumerable<Base1_.Range<uint>> naksToSplit, uint cutoff, uint[] tableOfCutOffs, uint npartitions, uint toskip)
        {
            IList<Base1_.Range<uint>>[] requests = new IList<QS._qss_c_.Base1_.Range<uint>>[npartitions];
            foreach (Base1_.Range<uint> nak in naksToSplit)
            {
                uint from = nak.From;
                if (from > cutoff)
                    break;
                uint to = nak.To > cutoff ? cutoff : nak.To;

                for (uint partno = 0; partno < npartitions; partno++)
                {
                    if (partno != toskip)
                    {
                        uint part_from = SeqNo.ToPartitionLowerBound(from, partno, npartitions);
                        if (part_from <= tableOfCutOffs[partno])
                        {
                            uint part_to = SeqNo.ToPartitionUpperBound(to, partno, npartitions);
                            if (part_to >= part_from)
                            {
                                if (part_to > tableOfCutOffs[partno])
                                    part_to = tableOfCutOffs[partno];

                                if (requests[partno] == null)
                                    requests[partno] = new List<Base1_.Range<uint>>();
                                requests[partno].Add(new Base1_.Range<uint>(part_from, part_to));
                            }
                        }
                    }
                }
            }
            return requests;
        }

        #endregion

        #region Class SeqNo

        public static class SeqNo
        {
            /// <summary>
            /// Converts a global sequence lower bound into a lower bound within the given partition.
            /// </summary>
            /// <param name="bound">Global bound.</param>
            /// <param name="partno">Number of the partition to convert the bound to.</param>
            /// <param name="npartitions">Number of partitions.</param>
            /// <returns>The lower bound within the partition.</returns>
            public static uint ToPartitionLowerBound(uint bound, uint partno, uint npartitions)
            {
                return (bound > partno) ? ((uint)(Math.Ceiling(((double)(bound - (partno + 1))) / ((double)npartitions)) + 1)) : 1;
            }

            /// <summary>
            /// Converts a global sequence upper bound into an upper bound within the given partition.
            /// </summary>
            /// <param name="bound">Global bound.</param>
            /// <param name="partno">Number of the partition to convert the bound to.</param>
            /// <param name="npartitions">Number of partitions.</param>
            /// <returns>The upper bound within the partition.</returns>
            public static uint ToPartitionUpperBound(uint bound, uint partno, uint npartitions)
            {
                return (bound > partno) ? ((uint)Math.Floor(((double)(bound - (partno + 1))) / ((double)npartitions) + 1)) : 0;
            }

            /// <summary>
            /// Converts a sequence number within partition into the global sequence number.
            /// </summary>
            /// <param name="bound">Sequence number within the partition.</param>
            /// <param name="partno">Number of the partition within which the sequence number if given.</param>
            /// <param name="npartitions">Number of partitions.</param>
            /// <returns>Global sequence number.</returns>
            public static uint FromPartition(uint bound, uint partno, uint npartitions)
            {
                return (bound > 0) ? (npartitions * (bound - 1) + partno + 1) : 0;
            }
        }

        #endregion

        #region GenerateNaks

        public static void GetTrimmedNaks(IEnumerable<Base1_.Range<uint>> completeNakCollection, uint partitionCutOff, 
            uint maximumNakRangesPerToken, IList<Base1_.Range<uint>> trimmedNakCollection, out uint partitionCovered)
        {
            IEnumerator<Base1_.Range<uint>> naks = completeNakCollection.GetEnumerator();
            uint nakCount = 0;
            while (true)
            {
                if (nakCount < maximumNakRangesPerToken)
                {
                    if (naks.MoveNext() && naks.Current.From <= partitionCutOff)
                    {
                        if (naks.Current.To <= partitionCutOff)
                        {
                            trimmedNakCollection.Add(naks.Current);
                            nakCount++;
                        }
                        else
                        {
                            trimmedNakCollection.Add(new QS._qss_c_.Base1_.Range<uint>(naks.Current.From, partitionCutOff));
                            partitionCovered = partitionCutOff;
                            break;
                        }
                    }
                    else
                    {
                        partitionCovered = partitionCutOff;
                        break;
                    }
                }
                else
                {
                    if (naks.MoveNext())
                    {
                        if (naks.Current.From <= partitionCutOff)
                            partitionCovered = naks.Current.From - 1;
                        else
                            partitionCovered = partitionCutOff;
                        break;
                    }
                    else
                    {
                        partitionCovered = partitionCutOff;
                        break;
                    }
                }
            }
        }

        #endregion

        #region CompressNaks

        public static IList<Base1_.Range<uint>> CompressNaks(IList<Base1_.Range<uint>> naks)
        {
            List<Base1_.Range<uint>> result = new List<QS._qss_c_.Base1_.Range<uint>>();
            bool first = true;
            Base1_.Range<uint> lastNak = new QS._qss_c_.Base1_.Range<uint>();
            foreach (Base1_.Range<uint> nak in naks)
            {
                if (first)
                {
                    lastNak = nak;
                    first = false;
                }
                else
                {
                    if (nak.From == lastNak.To + 1)
                        lastNak.To = nak.To;
                    else
                    {
                        result.Add(lastNak);
                        lastNak = nak;
                    }
                }
            }
            result.Add(lastNak);
            return result;
        }

        #endregion

        #region IntersectNaks

        public static void IntersectNaks(
            IEnumerable<Base1_.Range<uint>> missing1, uint covered1, IEnumerable<Base1_.Range<uint>> missing2, uint covered2,
            IList<Base1_.Range<uint>> commonNaks, out uint commonCovered, uint maximumNaksAllowed)
        {
            IEnumerator<Base1_.Range<uint>> naks1 = missing1.GetEnumerator(), naks2 = missing2.GetEnumerator();
            bool finished1 = !naks1.MoveNext(), finished2 = !naks2.MoveNext();
            uint maximumCovered = covered1;
            if (covered2 < maximumCovered)
                maximumCovered = covered2;

            uint bothCovered = 0;
            while (bothCovered < maximumCovered)
            {
                while (!finished1 && naks1.Current.To <= bothCovered)
                    finished1 = !naks1.MoveNext();

                while (!finished2 && naks2.Current.To <= bothCovered)
                    finished2 = !naks2.MoveNext();

                if (finished1 || finished2)
                {
                    bothCovered = maximumCovered;
                    break;
                }
                else
                {
                    uint newCovered = naks1.Current.From - 1;
                    if (naks2.Current.From - 1 > newCovered)
                        newCovered = naks2.Current.From - 1;

                    if (newCovered > bothCovered)
                    {
                        bothCovered = newCovered;
                    }
                    else
                    {
                        if (commonNaks.Count < maximumNaksAllowed)
                        {
                            newCovered = naks1.Current.To;
                            if (naks2.Current.To < newCovered)
                                newCovered = naks2.Current.To;

                            commonNaks.Add(new Base1_.Range<uint>(bothCovered + 1, newCovered));
                            bothCovered = newCovered;
                        }
                        else
                            break;
                    }
                }
            }

            commonCovered = bothCovered;
        }

        #endregion
    }
}
