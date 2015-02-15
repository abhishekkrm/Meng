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
using System.Diagnostics;

namespace QS._qss_c_.Gossiping4
{
/*
    public class Token
    {
        /// <summary>
        /// Create from local ack sets, used by the token creator.
        /// </summary>
        /// <param name="localAckSets"></param>
        public Token(IDictionary<QS._core_c_.Base3.InstanceID, AckSet> localAckSets)
        {
            receiverStates = new Dictionary<QS.CMS.QS._core_c_.Base3.InstanceID, ReceiverState>(localAckSets.Count);
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, AckSet> element in localAckSets)
                receiverStates.Add(element.Key, new ReceiverState(element.Value));
        }

        private Dictionary<QS._core_c_.Base3.InstanceID, ReceiverState> receiverStates;

        #region Accessors

        public IDictionary<QS._core_c_.Base3.InstanceID, ReceiverState> ReceiverStates
        {
            get { return receiverStates; }
        }

        #endregion

        #region Struct ReceiverState

        public struct ReceiverState
        {
            public ReceiverState(AckSet ackSet)
            {
                numberOfVotes = 1;
                maximumContiguousAck = ackSet.MaxContiguousAck;
                minimumContiguousNak = ackSet.MinContiguousNak;
                isolatedNaks = new Dictionary<uint, Nak>(ackSet.IsolatedNaks.Count);
                foreach (uint seqno in ackSet.IsolatedNaks)
                    isolatedNaks.Add(seqno, new Nak(0, 1));
            }

            /// <summary>
            /// The resulting "maximumContiguousAck" should be the minimum of the old and local values.
            /// The resulting "minimumContiguousNak" should be the maximum.
            /// The resulting set of isolated NAKs should be for seqnos representing the sum of the two sets of isolated NAK seqnos.
            /// </summary>
            private uint numberOfVotes, maximumContiguousAck, minimumContiguousNak;
            private Dictionary<uint, Nak> isolatedNaks;

            public void CombineWithAckSet(AckSet localAckSet)
            {
                Dictionary<uint, Base3.Pair<bool>> combinedNackCollection = 
                    new Dictionary<uint, Base3.Pair<bool>>(isolatedNaks.Count + localAckSet.IsolatedNaks.Count);
                foreach (uint seqno in isolatedNaks.Keys)
                    combinedNackCollection[seqno] = new Base3.Pair<bool>(true, false);
                foreach (uint seqno in localAckSet.IsolatedNaks)
                    combinedNackCollection[seqno] = new Base3.Pair<bool>(combinedNackCollection.ContainsKey(seqno), true);

                foreach (KeyValuePair<uint, Base3.Pair<bool>> element in combinedNackCollection)
                {
                    uint seqno = element.Key;
                    bool inOld = element.Value.Element1, inLocal = element.Value.Element2;

                    if (inOld) // nack in old token isolated nack list
                    {
                        Nak nak = isolatedNaks[seqno];
                        if (inLocal || seqno >= localAckSet.MinContiguousNak) // local does not have this guy
                        {
                            nak.WantingVotes++;
                            // schedule to pull this guy from host "nak.WantingVotes" hops away backwards
                        }
                        else // local has the guy
                        {
                            // schedule to push thus guy to all of the "nak.WantingVotes" preceding hosts
                            nak.WantingVotes = 0;
                            nak.ReceivedVotes++;
                        }
                        isolatedNaks[seqno] = nak;
                    }
                    else
                    {
                        Debug.Assert(inLocal);




                    }
                }



/--*
                foreach (KeyValuePair<uint, Nak> element in isolatedNaks)
                {
                    if (element.Key <= localAckSet.MaxContiguousAck) // this local host is the first to have it
                    {

                        element.Value.ReceivedVotes = element.Value.ReceivedVotes + 1;
                        // schedule to push this guy back to all of the "element.Value.WantingVotes" preceding nodes on the ring
                        element.Value.WantingVotes = 0;                        
                    }
                    else if (element.Key >= localAckSet.MinContiguousNak) // this local host does not have it
                    {                        
                        element.Value.

                    }


                }
                foreach (uint seqno in localAckSet.IsolatedNaks)
                {
                    if (seqno <= maximumContiguousAck) // this local host is missing it
                    {
                        if (isolatedNaks.ContainsKey(seqno))
                        {
                            isolatedNaks[seqno].WantingVotes++;
                            // schedule to pull this guy from some node "element.Value.WantingVotes" away backwards along the ring   
                        }
                        else
                        {
                            isolatedNaks[seqno] = new Nak(
                        }


                    }

                }



                if (localAckSet.MaxContiguousAck < maximumContiguousAck)
                    maximumContiguousAck = localAckSet.MaxContiguousAck;

                if (localAckSet.MinContiguousNak > minimumContiguousNak)
                    minimumContiguousNak = localAckSet.MinContiguousNak;  
*--/ 
            }

            #region Accessors
            #endregion

            #region Struct Nak

            public struct Nak
            {
                public Nak(uint receivedVotes, uint wantingVotes)
                {
                    this.receivedVotes = receivedVotes;
                    this.wantingVotes = wantingVotes;
                }

                private uint receivedVotes, wantingVotes;

                #region Accessors

                public uint ReceivedVotes
                {
                    get { return receivedVotes; }
                    set { receivedVotes = value; }
                }

                public uint WantingVotes
                {
                    get { return wantingVotes; }
                    set { wantingVotes = value; }
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
*/ 
}
