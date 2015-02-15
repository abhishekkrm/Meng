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

using QS._qss_c_._OldFx.Reliability.Peers.Base;

namespace QS._qss_c_._OldFx.Reliability.Components.Peers
{
/*
    public class Ring1 : IPeer
    {
        public Ring1()
        {
            // .................................................
        }

        private IContext context;
        private bool isLeader;
        private IPeerReference successor;
        private Core.IAlarm alarm;
        private int leaderIndex, successorIndex;

        #region Tokens

        private void ReleaseToken()
        {
            // releasing token

            if (successor != null)
                successor.Send(new QS.CMS.Base3.Message(0, new Base2.StringWrapper("foo")));

            // .................................................
        }

        #endregion

        #region Alarms

        private void AlarmCallback(Core.IAlarm alarm)
        {
            lock (this)
            {
                ReleaseToken();
                alarm.Reschedule();
            }
        }

        private void BecomeLeader()
        {
            isLeader = true;
            context.AlarmClock.Schedule(1, new QS.CMS.Core.AlarmCallback(this.AlarmCallback), null);
        }

        #endregion

        #region IPeer Members

        void IPeer.Initialize(IContext context)
        {
            lock (this)
            {
                this.context = context;
                if (context.Peers.Length > 1)
                {
                    successorIndex = (context.LocalIndex + 1) % context.Peers.Length;
                    successor = context.Peers[successorIndex];
                }

                if (context.LocalIndex == 0)
                    BecomeLeader();
            }
        }

        void IPeer.MembershipChanged()
        {
            lock (this)
            {
                while (!context.Peers[leaderIndex].IsAlive && leaderIndex != context.LocalIndex)
                    leaderIndex++;

                while (!context.Peers[successorIndex].IsAlive && successorIndex != context.LocalIndex)
                    successorIndex = (successorIndex + 1) % context.Peers.Length;

                if (successorIndex != context.LocalIndex)
                    successor = context.Peers[successorIndex];
                else
                    successor = null;

                if (leaderIndex == context.LocalIndex)
                    BecomeLeader();
            }
        }

        #endregion
    }
*/
}
