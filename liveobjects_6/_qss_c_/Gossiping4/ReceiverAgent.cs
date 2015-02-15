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

namespace QS._qss_c_.Gossiping4
{
/*
    public class ReceiverAgent
    {
        public ReceiverAgent(AckSetsCallback ackSetsCallback)
        {
            this.ackSetsCallback = ackSetsCallback;
        }

        private AckSetsCallback ackSetsCallback;

        #region Internal Processing

        private Token NewToken
        {
            get
            {
                IDictionary<QS._core_c_.Base3.InstanceID, AckSet> localAckSets = ackSetsCallback();
                return new Token(localAckSets);
            }
        }

        private Token ProcessToken(Token receivedToken)
        {
            IDictionary<QS._core_c_.Base3.InstanceID, AckSet> localAckSets = ackSetsCallback();
            
            List<QS._core_c_.Base3.InstanceID> addresses = 
                new List<QS.CMS.QS._core_c_.Base3.InstanceID>(receivedToken.ReceiverStates.Count + localAckSets.Count); 
            addresses.AddRange(receivedToken.ReceiverStates.Keys);
            addresses.AddRange(localAckSets.Keys);

            foreach (QS._core_c_.Base3.InstanceID address in addresses)
            {
                if (receivedToken.ReceiverStates.ContainsKey(address))
                {
                    if (localAckSets.ContainsKey(address))
                    {
                        Token.ReceiverState receiverState = receivedToken.ReceiverStates[address];
                        AckSet localAckSet = localAckSets[address];

                        receiverState.CombineWithAckSet(localAckSet);


                        // .................................................................................................
                    }
                    else
                    {
                        // .................................................................................................
                    }
                }
                else
                {
                    if (localAckSets.ContainsKey(address))
                    {
                        // .................................................................................................
                    }
                    else
                        throw new Exception("Internal error: shouldn't be here!");
                }
            }

            return null;
        }

        #endregion
    }
*/ 
}
