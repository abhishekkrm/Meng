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

namespace QS._qss_x_.Agents_.Components
{
    /// <summary>
    /// This driver implements a simple token ring.
    /// </summary>
    public sealed class Driver1 : QS._qss_x_.Agents_.Base.IDriver
    {
        #region Constructor

        public Driver1()
        {
        }

        #endregion

        #region Fields

        private QS._qss_x_.Agents_.Base.IDriverContext context;
        private uint configuration_incarnation, my_index, predecessor_index, successor_index, num_members;
        private bool is_leader;
        private QS._qss_x_.Agents_.Base.IMember successor;
        private QS.Fx.Clock.IAlarm tokenalarm;

        #endregion

        #region IDriver Members

        void QS._qss_x_.Agents_.Base.IDriver.Initialize(QS._qss_x_.Agents_.Base.IDriverContext context)
        {
            lock (this)
            {
                this.context = context;

                System.Diagnostics.Debug.Assert(
                    context.Configuration.Members.Length > 1, "The token ring driver currently does not support singleton rings.");

                configuration_incarnation = context.Configuration.Incarnation;
                num_members = (uint) context.Configuration.Members.Length;
                my_index = context.Configuration.LocalIndex;
                is_leader = my_index == 0;
                predecessor_index = (uint)((((int) my_index) + (((int) num_members) - 1)) % num_members);
                successor_index = (uint)((((int)my_index) + 1) % num_members);
                successor = context.Configuration.Members[successor_index];

                if (is_leader)
                {
                    Token token;
                    _ProcessToken(out token);
                    successor.Send(token);
                    tokenalarm = context.AlarmClock.Schedule(((double)1.0) / context.Rate, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), null);
                }
            }
        }

        void QS._qss_x_.Agents_.Base.IDriver.Receive(
            uint configuration_incarnation, uint sender_member_index, QS.Fx.Serialization.ISerializable message)
        {
            lock (this)
            {
                Token token = message as Token;
                System.Diagnostics.Debug.Assert(token != null);

                if (configuration_incarnation == this.configuration_incarnation)
                {
                    System.Diagnostics.Debug.Assert(sender_member_index == this.predecessor_index,
                        "\nid = " + this.context.ID.ToString() +
                        "\nincarnation = " + this.context.Incarnation.ToString() +
                        "\nconfiguration_incarnation = " + this.configuration_incarnation.ToString() + 
                        "\nmy_index = " + this.my_index.ToString() +
                        "\nnum_members = " + this.num_members.ToString() +
                        "\npredecessor_index = " + this.predecessor_index.ToString() +
                        "\nsuccessor_index = " + this.successor_index.ToString() +
                        "\nround = " + this.context.Round.ToString() +
                        "\nsender_member_index = " + sender_member_index.ToString() +
                        "\nmessage = \n" + QS.Fx.Printing.Printable.ToString(message));

                    if (is_leader)
                    {
                        _ProcessToken(token);
                    }
                    else
                    {
                        Token _token = new Token();
                        _ProcessToken(token, out _token, my_index > 1, my_index < (num_members - 1));
                        successor.Send(_token);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (tokenalarm != null)
                {
                    if (!tokenalarm.Cancelled)
                        tokenalarm.Cancel();
                    tokenalarm = null;
                }
            }
        }

        #endregion

        #region _AlarmCallback

        private void _AlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (!alarm.Cancelled)
                {
                    Token token;
                    _ProcessToken(out token);
                    successor.Send(token);
                    alarm.Reschedule();
                }
            }
        }

        #endregion

        #region _ProcessToken : Case #1

        private void _ProcessToken(out Token outgoing)
        {
            outgoing = new Token();

            outgoing.round = this.context.Round + 1;

            outgoing.toaggregate = null;

            QS.Fx.Serialization.ISerializable[] todisseminate;
            context.Disseminate(outgoing.round, 1, out todisseminate);
            if (todisseminate.Length != 1)
                throw new Exception("Wrong number of dissemination records.");
            outgoing.todisseminate = todisseminate[0];
        }

        #endregion

        #region _ProcessToken : Case #2

        private void _ProcessToken(Token incoming, out Token outgoing, bool aggregate_predecessor, bool disseminate_successor)
        {
            outgoing = new Token();
            outgoing.round = incoming.round;

            if (disseminate_successor)
            {
                QS.Fx.Serialization.ISerializable[] todisseminate;
                context.Disseminate(outgoing.round, incoming.todisseminate, 1, out todisseminate);
                if (todisseminate.Length != 1)
                    throw new Exception("Wrong number of dissemination records.");
                outgoing.todisseminate = todisseminate[0];
            }
            else
            {
                context.Disseminate(incoming.round, incoming.todisseminate);
                outgoing.todisseminate = null;
            }

            if (aggregate_predecessor)
            {
                System.Diagnostics.Debug.Assert(incoming.toaggregate != null);
                context.Aggregate(outgoing.round, new QS.Fx.Serialization.ISerializable[] { incoming.toaggregate }, out outgoing.toaggregate);
            }
            else
            {
                System.Diagnostics.Debug.Assert(incoming.toaggregate == null);
                context.Aggregate(outgoing.round, out outgoing.toaggregate);
            }
        }

        #endregion

        #region _ProcessToken : Case #3

        private void _ProcessToken(Token incoming)
        {
            context.Aggregate(incoming.round, new QS.Fx.Serialization.ISerializable[] { incoming.toaggregate });

//            context.Logger.Log("Agent[" + context.ID.ToString() + " # " + context.Incarnation.ToString() + 
//                "] completed round : " + incoming.round.ToString() + ".");
        }

        #endregion

        #region Class Token

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Agents_Components_Driver1_Token)]
        public class Token : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public Token()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Printing.Printable]
            internal uint round;
            [QS.Fx.Printing.Printable]
            internal QS.Fx.Serialization.ISerializable toaggregate;
            [QS.Fx.Printing.Printable]
            internal QS.Fx.Serialization.ISerializable todisseminate;

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = 
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Agents_Components_Driver1_Token);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                    if (toaggregate != null)
                        info.AddAnother(toaggregate.SerializableInfo);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                    if (todisseminate != null)
                        info.AddAnother(todisseminate.SerializableInfo);
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, round);
                if (toaggregate != null)
                {
                    QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, toaggregate.SerializableInfo.ClassID);
                    toaggregate.SerializeTo(ref header, ref data);
                }
                else
                    QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, 0);
                if (todisseminate != null)
                {
                    QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, todisseminate.SerializableInfo.ClassID);
                    todisseminate.SerializeTo(ref header, ref data);
                }
                else
                    QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, 0);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                round = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
                ushort classid;
                classid = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
                if (classid > 0)
                {
                    toaggregate = QS._core_c_.Base3.Serializer.CreateObject(classid);
                    toaggregate.DeserializeFrom(ref header, ref data);
                }
                else
                    toaggregate = null;
                classid = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
                if (classid > 0)
                {
                    todisseminate = QS._core_c_.Base3.Serializer.CreateObject(classid);
                    todisseminate.DeserializeFrom(ref header, ref data);
                }
            }

            #endregion
        }

        #endregion
    }
}
