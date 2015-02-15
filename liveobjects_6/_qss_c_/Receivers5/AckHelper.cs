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
    public static class AckHelper
    {
        public static void AckCollection2Acks(
            Receivers4.IAckCollection ackCollection, int maxAcknowledgementRanges, out IList<Base1_.Range<uint>> acks, out uint maximumCovered)
        {
            maximumCovered = ackCollection.MaxContiguous;
            acks = new List<Base1_.Range<uint>>();
            if (maximumCovered > 0)
                acks.Add(new QS._qss_c_.Base1_.Range<uint>(1, maximumCovered));
            foreach (Base1_.Range<uint> range in ackCollection.Missing)
            {
                if (range.From > (maximumCovered + 1))
                    acks.Add(new QS._qss_c_.Base1_.Range<uint>(maximumCovered + 1, range.From - 1));
                maximumCovered = range.To;
                if (acks.Count >= maxAcknowledgementRanges)
                    break;
            }

            if (acks.Count < maxAcknowledgementRanges && maximumCovered < ackCollection.MaximumSeen)
            {
                acks.Add(new QS._qss_c_.Base1_.Range<uint>(maximumCovered + 1, ackCollection.MaximumSeen));
                maximumCovered = ackCollection.MaximumSeen;
            }
        }

/*
        public static void GetIncrementalAcks(
            IList<Base.Range<uint>> oldAcks, IList<Base.Range<uint>> newAcks, out IList<Base.Range<uint>> deltaAcks)
        {
            IEnumerator<Base.Range<uint>> enumOld = oldAcks.GetEnumerator();
            if (!enumOld.MoveNext())
                deltaAcks = newAcks;
            else
            {
                IEnumerator<Base.Range<uint>> enumNew = newAcks.GetEnumerator();
                if (!enumNew.MoveNext())
                    throw new Exception("Internal error: wrong usage, new acks are not a superset of old acks.");                
                uint maxCovered = 0;
                while (true)
                {
                    if (enumOld.Current.From > (maxCovered + 1))
                    {
                        if (enumNew.Current.From > (maxCovered + 1))
                            maxCovered = Math.Min(enumOld.Current.From, enumNew.Current.From) - 1;
                        else
                        {
                            maxCovered = Math.Min(enumOld.Current.From - 1, enumNew.Current.
                            deltaAcks.Add(new QS.CMS.Base.Range<uint>(enumNew.Current.From, enumOld.Current.From - 1));
                            maxCovered = enumOld.Current.From - 1;
                        }
                    }
                    else
                    {
                    }

                }
            }
        }
*/ 
    }
}
