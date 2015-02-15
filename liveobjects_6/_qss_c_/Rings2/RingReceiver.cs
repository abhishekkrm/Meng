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

#define DEBUG_RingReceiver

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings2
{
    public class RingReceiver
    {
        public RingReceiver(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID[] receiverAddresses, QS._core_c_.Base3.InstanceID localAddress,
            QS.Fx.Clock.IAlarmClock alarmClock, double frequency, 
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection)
        {
            this.logger = logger;
            this.receiverAddresses = receiverAddresses;
            this.localAddress = localAddress;
            this.alarmClock = alarmClock;
            this.frequency = frequency;
            this.senderCollection = senderCollection;

            Array.Sort<QS._core_c_.Base3.InstanceID>(receiverAddresses);
            position = Array.BinarySearch<QS._core_c_.Base3.InstanceID>(receiverAddresses, localAddress);
            if (position < 0)
                throw new Exception("Local address is not on the list of receivers!");
            leader = (position == 0);
            successor = (position + 1) % receiverAddresses.Length;
            successorSender = senderCollection[receiverAddresses[successor]];

            // TODO: Circulate token only if necessary; for now we just do it all the time.
            if (leader)
                alarmClock.Schedule(1 / frequency, new QS.Fx.Clock.AlarmCallback(this.TokenGenerationCallback), null);
        }

        // TODO: Reconfigure after failures; for now we just ignore failures.

        private QS.Fx.Logging.ILogger logger;
        private QS._core_c_.Base3.InstanceID[] receiverAddresses;
        private QS._core_c_.Base3.InstanceID localAddress;
        private bool leader;
        private int position, successor;
        private double frequency;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private Base3_.IReliableSerializableSender successorSender;

        private uint aggregatedMaxSeqNo;

        #region GetNAKs

        private void GetNAKs(uint cutoffSeqNo, out ICollection<uint> nakCollection, out uint maximumSeqNo)
        {
            // TODO: Implement this part.........................

            throw new NotImplementedException();
        }

        #endregion

        #region Sending

        private void Forward(Token token)
        {
            // TODO: Send the token to successor.............................
        }

        #endregion

        #region Token Receive Callback

        private void TokenReceiveCallback(Token token)
        {
            if (leader)
            {
                aggregatedMaxSeqNo = token.MaximumSeqNo;

                // TODO: Finish this...............................


#if DEBUG_RingReceiver
                logger.Log(this, "Collected token: " + token.ToString());
#endif
            }
            else
            {
                ICollection<uint> nakCollection;
                uint maximumSeqNo;
                GetNAKs(aggregatedMaxSeqNo, out nakCollection, out maximumSeqNo);

                aggregatedMaxSeqNo = token.MaximumSeqNo;
                if (maximumSeqNo > aggregatedMaxSeqNo)
                    aggregatedMaxSeqNo = maximumSeqNo;

                Token forwardedToken = new Token(token.CutoffSeqNo, aggregatedMaxSeqNo);

                // TODO: Finish this...............................

                Forward(forwardedToken);
            }
        }

        #endregion

        #region Token Generation Callback

        private void TokenGenerationCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            ICollection<uint> nakCollection;
            uint maximumSeqNo;
            GetNAKs(aggregatedMaxSeqNo, out nakCollection, out maximumSeqNo);

            Token token = new Token(aggregatedMaxSeqNo, maximumSeqNo);

            // TODO: Finish this........................................................................................................................

            alarmRef.Reschedule();

            Forward(token);
        }

        #endregion

        #region Class Token

        private class Token : QS.Fx.Serialization.ISerializable
        {
            public Token(uint cutoffSeqNo, uint maximumSeqNo)
            {
                this.cutoffSeqNo = cutoffSeqNo;
                this.maximumSeqNo = maximumSeqNo;
            }

            private uint cutoffSeqNo, maximumSeqNo;

            public Token()
            {
            }

            #region Accessors

            public uint MaximumSeqNo
            {
                get { return maximumSeqNo; }
            }

            public uint CutoffSeqNo
            {
                get { return cutoffSeqNo; }
            }

            #endregion

            // TODO: Implement serialization of the token.

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

            #region Printing

            public override string ToString()
            {
                StringBuilder s = new StringBuilder();

                s.AppendLine("MaximumSeqNo = " + maximumSeqNo.ToString());
                s.AppendLine("CutoffSeqNo = " + cutoffSeqNo.ToString());

                return s.ToString();
            }

            #endregion
        }

        #endregion
    }
}
