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

// #define DEBUG_Quiescence

#define OPTION_ProcessingCrashes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    public class AgentCore : IAgentCore, IDisposable
    {
        public AgentCore(IAgentCoreContext context)
        {
            this.context = context;
        }

        private bool disposed;

        private uint lastRound;
        private IAgentCoreContext context;

#if OPTION_ProcessingCrashes
        private event EventHandler resumeCallback;
        private bool quiescence, quiescence_committed;
#endif

        #region IPartitionedTokenRingMember<AgentIntraPartitionToken,AgentInterPartitionToken> Members

        // ***(Case 1)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process()
        {
#if OPTION_ProcessingCrashes
            bool should_quiesce = true;
#endif
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken,
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
            {
                element.Value.Process();
#if OPTION_ProcessingCrashes
                if (element.Value.IsActive)
                    should_quiesce = false;
#endif
            }

#if OPTION_ProcessingCrashes
            if (should_quiesce)
            {
                Quiesce();
                quiescence_committed = true;
            }
#endif
        }

// ***(Case 2)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            out AgentIntraPartitionToken outgoingToken)
        {
            outgoingToken = new AgentIntraPartitionToken(context.LocalAddress, context.LocalAddress, ++lastRound, true, 
#if OPTION_ProcessingCrashes
                quiescence
#else
                false
#endif
                );

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken,
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
            {
                ReceiverIntraPartitionToken token;
                element.Value.Process(out token);
                outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes
                if (element.Value.IsActive)
                {
                    outgoingToken.Quiescence = false;
                    Resume();
                }
#endif
            }
        }

// ***(Case 3)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            out AgentInterPartitionToken outgoingToken)
        {
/*
            Process Partition Singleton.................................................................
*/

            throw new Exception("The method or operation is not implemented.");
        }

// ***(Case 4)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, AgentIntraPartitionToken incomingToken)
        {
            lastRound = incomingToken.Round;

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken> element in incomingToken.ReceiverTokens)
            {
                context.ReceiverAt(element.Key).Process(incomingAddress, element.Value);
            }

#if OPTION_ProcessingCrashes
            bool should_quiesce = true;
#endif

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken,
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
            {
                if (!incomingToken.ReceiverTokens.ContainsKey(element.Key))
                    element.Value.Process(incomingAddress, (ReceiverIntraPartitionToken) null);

#if OPTION_ProcessingCrashes
                if (element.Value.IsActive)
                    should_quiesce = false;
#endif
            }

#if OPTION_ProcessingCrashes
            if (should_quiesce)
            {
                if (incomingToken.Quiescence)
                {
                    Quiesce();

                    if (incomingToken.QuiescenceCommit)
                        quiescence_committed = true;
                }
            }
            else
            {
                Resume();
            }
#endif
        }

// ***(Case 5)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, AgentIntraPartitionToken incomingToken, out AgentIntraPartitionToken outgoingToken)
        {
            lastRound = incomingToken.Round;

            outgoingToken = new AgentIntraPartitionToken(
                incomingToken.InterPartitionCreatorAddress, incomingToken.PartitionCreatorAddress, incomingToken.Round,
#if OPTION_ProcessingCrashes 
                incomingToken.Quiescence, incomingToken.QuiescenceCommit
#else
                false, false
#endif
                );

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken> element in incomingToken.ReceiverTokens)
            {
                ReceiverIntraPartitionToken token;
                IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken> receiver = context.ReceiverAt(element.Key);
                receiver.Process(incomingAddress, element.Value, out token);
                outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes 
                if (receiver.IsActive)
                    outgoingToken.Quiescence = false;
#endif
            }

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken, 
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
                if (!incomingToken.ReceiverTokens.ContainsKey(element.Key))
                {
                    ReceiverIntraPartitionToken token;
                    element.Value.Process(incomingAddress, (ReceiverIntraPartitionToken) null, out token);
                    outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes 
                    if (element.Value.IsActive)
                        outgoingToken.Quiescence = false;
#endif
                }

#if OPTION_ProcessingCrashes 
            if (outgoingToken.Quiescence && outgoingToken.QuiescenceCommit)
            {
                Quiesce();
            }
#endif
        }

// ***(Case 6)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, AgentIntraPartitionToken incomingToken, out AgentInterPartitionToken outgoingToken)
        {
            lastRound = incomingToken.Round;

            outgoingToken = new AgentInterPartitionToken(incomingToken.InterPartitionCreatorAddress, incomingToken.Round,
#if OPTION_ProcessingCrashes 
                incomingToken.Quiescence, incomingToken.QuiescenceCommit
#else
                false, false
#endif                
                );

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken> element in incomingToken.ReceiverTokens)
            {
                ReceiverInterPartitionToken token;
                IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken> receiver = context.ReceiverAt(element.Key);
                receiver.Process(incomingAddress, element.Value, out token);
                outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes 
                if (receiver.IsActive)
                    outgoingToken.Quiescence = false;
#endif
            }

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken, 
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
                if (!incomingToken.ReceiverTokens.ContainsKey(element.Key))
                {
                    ReceiverInterPartitionToken token;
                    element.Value.Process(incomingAddress, (ReceiverIntraPartitionToken) null, out token);
                    outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes 
                    if (element.Value.IsActive)
                        outgoingToken.Quiescence = false;
#endif
                }

#if OPTION_ProcessingCrashes 
            if (outgoingToken.QuiescenceCommit)
            {
                Quiesce();
            }
#endif
        }

// ***(Case 7)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, AgentInterPartitionToken incomingToken)
        {
            lastRound = incomingToken.Round;

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, ReceiverInterPartitionToken> element in incomingToken.ReceiverTokens)
            {
                context.ReceiverAt(element.Key).Process(incomingAddress, element.Value);
            }

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken, 
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
                if (!incomingToken.ReceiverTokens.ContainsKey(element.Key))
                    element.Value.Process(incomingAddress, (ReceiverInterPartitionToken) null);

#if OPTION_ProcessingCrashes 
            if (incomingToken.Quiescence)
            {
                Quiesce();

                if (incomingToken.QuiescenceCommit)
                    quiescence_committed = true;
            }
#endif
        }

// ***(Case 8)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, AgentInterPartitionToken incomingToken, out AgentIntraPartitionToken outgoingToken)
        {
            lastRound = incomingToken.Round;

            outgoingToken = new AgentIntraPartitionToken(incomingToken.CreatorAddress, context.LocalAddress, incomingToken.Round, 
#if OPTION_ProcessingCrashes 
                incomingToken.Quiescence, incomingToken.QuiescenceCommit
#else
                false, false
#endif
                );

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, ReceiverInterPartitionToken> element in incomingToken.ReceiverTokens)
            {
                ReceiverIntraPartitionToken token;
                IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken> receiver = context.ReceiverAt(element.Key);
                receiver.Process(incomingAddress, element.Value, out token);
                outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes 
                if (receiver.IsActive)
                    outgoingToken.Quiescence = false;
#endif
            }

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken, 
                ReceiverInterPartitionToken>> element in context.ReceiverCollection)
                if (!incomingToken.ReceiverTokens.ContainsKey(element.Key))
                {
                    ReceiverIntraPartitionToken token;
                    element.Value.Process(incomingAddress, (ReceiverIntraPartitionToken)null, out token);
                    outgoingToken.ReceiverTokens.Add(element.Key, token);

#if OPTION_ProcessingCrashes 
                    if (element.Value.IsActive)
                        outgoingToken.Quiescence = false;
#endif
                }

#if OPTION_ProcessingCrashes 
            if (outgoingToken.QuiescenceCommit)
            {
                Quiesce();
            }
#endif
        }

// ***(Case 9)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, AgentInterPartitionToken incomingToken, out AgentInterPartitionToken outgoingToken)
        {
            lastRound = incomingToken.Round;

/*
            Process the Passing of Inter-Partition Token through a Single-Node Partition...............................................................
*/

            throw new Exception("The method or operation is not implemented.");
        }

// ************************************************************************************************************************************************************

#if OPTION_ProcessingCrashes 

        bool IPartitionedTokenRingMember<AgentIntraPartitionToken, AgentInterPartitionToken>.IsActive
        {
            get { return !(quiescence && quiescence_committed); }
        }

        public void Quiesce()
        {
            if (!quiescence)
            {
#if DEBUG_Quiescence
                context.logger.Log(this, "__________Quiesce[" + context.ID + "]");
#endif

                quiescence = true;
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken,
                    ReceiverInterPartitionToken>> element in context.ReceiverCollection)
                {
                    element.Value.Quiesce();
                }
            }
        }

        public void Resume()
        {
            if (quiescence)
            {
#if DEBUG_Quiescence
                context.logger.Log(this, "__________Resume[" + context.ID + "]");
#endif

                quiescence = false;
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken,
                    ReceiverInterPartitionToken>> element in context.ReceiverCollection)
                {
                    element.Value.Resume();
                }
            }
            quiescence_committed = false;
        }

        public event EventHandler OnResume
        {
            add { resumeCallback += value; }
            remove { resumeCallback -= value; }
        }
#endif

        #endregion

        #region ProcessingCrashes

        public const bool ProcessingCrashes =
#if OPTION_ProcessingCrashes
            true
#else
            false
#endif
            ;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            disposed = true;
        }

        #endregion
    }
}
