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

namespace QS._qss_x_.Agents_.Base
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Plan
    {
        internal Plan(uint configuration_incarnation, uint roundno, bool confirmed, Session[] sessions)
        {
            this.configuration_incarnation = configuration_incarnation;
            this.roundno = roundno;
            this.confirmed = confirmed;
            this.sessions = sessions;

            QS._qss_x_.Base1_.SessionID[] _sessionids = new QS._qss_x_.Base1_.SessionID[sessions.Length];
            int _fingerprint = configuration_incarnation.GetHashCode() ^ roundno.GetHashCode() ^ sessions.Length;
            for (int ind = 0; ind < sessions.Length; ind++)
            {
                _sessionids[ind] = sessions[ind].id;
                _fingerprint ^= sessions[ind].id.GetHashCode();
            }
            this.fingerprint = (uint) _fingerprint;

            this.info = new Info(roundno, _sessionids);
        }

        [QS.Fx.Printing.Printable]
        internal uint configuration_incarnation;
        [QS.Fx.Printing.Printable]
        internal bool confirmed;
        [QS.Fx.Printing.Printable]
        internal uint roundno;
        [QS.Fx.Printing.Printable]
        internal uint fingerprint;
        [QS.Fx.Printing.Printable]
        internal Info info;

        internal Session[] sessions;

        #region Class Info

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Agents_Base_Plan_Info)]
        public sealed class Info : QS.Fx.Serialization.ISerializable
        {
            #region Constructors

            public Info()
            {
            }

            internal Info(/* uint fingerprint, */ uint round, QS._qss_x_.Base1_.SessionID[] sessions)
            {
                // this.fingerprint = fingerprint;
                this.round = round;
                this.sessions = sessions;
            }

            #endregion

            // [QS.Fx.Printing.Printable]
            // internal uint fingerprint;
            [QS.Fx.Printing.Printable]
            internal uint round;
            [QS.Fx.Printing.Printable]
            internal QS._qss_x_.Base1_.SessionID[] sessions;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Agents_Base_Plan_Info);
                    // QS.CMS.Base3.SerializationHelper.ExtendSerializableInfo_UInt32(ref info, fingerprint);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortArrayOf<QS._qss_x_.Base1_.SessionID>(ref info, sessions);
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                // QS.CMS.Base3.SerializationHelper.Serialize_UInt32(ref info, fingerprint);
                QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, round);
                QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortArrayOf<QS._qss_x_.Base1_.SessionID>(ref header, ref data, sessions);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                // fingerprint = QS.CMS.Base3.SerializationHelper.Deserialize_UInt32(ref info);
                round = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
                sessions = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortArrayOf<QS._qss_x_.Base1_.SessionID>(ref header, ref data);
            }

            #endregion
        }

        #endregion
    }
}
