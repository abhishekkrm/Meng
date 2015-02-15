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

namespace QS._qss_c_.Gossiping3
{
    public class RegionalAgent
    {
        public RegionalAgent(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, QS.Fx.Clock.IAlarmClock alarmClock, double tokensPerSecond)
        {
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.tokensPerSecond = tokensPerSecond;
            this.localIID = localIID;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private double tokensPerSecond;
        private QS._core_c_.Base3.InstanceID localIID;
        private Agent agent = null;

        #region Accessors

        public double TokensPerSecond
        {
            get { return tokensPerSecond; }
            set
            {
                Agent agent;
                lock (this)
                {
                    tokensPerSecond = value;
                    agent = this.agent;
                }

                if (agent != null)
                {
                    lock (agent)
                    {
                        agent.RecalculateTokens();
                    }
                }
            }
        }

        #endregion

        #region Class Token

        private class Token : QS.Fx.Serialization.ISerializable
        {
            public Token()
            {
            }

            public Token(QS._core_c_.Base3.InstanceID originAddress, bool forwardGoing)
            {
                this.originAddress = originAddress;
                this.forwardGoing = forwardGoing;
            }

            private QS._core_c_.Base3.InstanceID originAddress;
            private bool forwardGoing;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        #region Class Agent

        private class Agent : System.IDisposable
        {
            public Agent(RegionalAgent owner, Membership2.ClientState.IRegionView regionView)
            {
                this.owner = owner;
                this.regionView = regionView;

                RecalculateTokens();
                alarmRef = owner.alarmClock.Schedule(tokenInterval, new QS.Fx.Clock.AlarmCallback(this.GenerateTokenCallback), null);
            }

            private RegionalAgent owner;
            private Membership2.ClientState.IRegionView regionView;
            private double tokenInterval;
            private QS.Fx.Clock.IAlarm alarmRef;
            private bool forwardGoing = true;

            #region 

            // private void 

            #endregion

            #region Configuration Changes

            public void RecalculateTokens()
            {
                tokenInterval = 1.0 / (owner.tokensPerSecond * ((double)regionView.Members.Count));
            }

            #endregion

            #region Token Generation

            private void GenerateTokenCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (this)
                {
                    Token token = new Token(owner.localIID, forwardGoing);
                    forwardGoing = !forwardGoing;


                    // ........................

                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                lock (this)
                {
                    alarmRef.Cancel();
                    alarmRef = null;
                }
            }

            #endregion
        }

        #endregion
    }
}
