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
    /// This driver implements a simple binary tree.
    /// </summary>
    public sealed class Driver2 : QS._qss_x_.Agents_.Base.IDriver
    {
        #region Constructor

        public Driver2()
        {
        }

        #endregion

        #region Fields

        private QS._qss_x_.Agents_.Base.IDriverContext context;
        private uint configuration_incarnation, my_index, parent_index, child1_index, child2_index, num_members;
        private bool is_root;
        private QS._qss_x_.Agents_.Base.IMember parent, child1, child2;
        private QS.Fx.Clock.IAlarm tokenalarm;
        private uint round;

        #endregion

        #region IDriver Members

        void QS._qss_x_.Agents_.Base.IDriver.Initialize(QS._qss_x_.Agents_.Base.IDriverContext context)
        {
            lock (this)
            {
                this.context = context;

                configuration_incarnation = context.Configuration.Incarnation;
                num_members = (uint)context.Configuration.Members.Length;
                my_index = context.Configuration.LocalIndex;
                is_root = my_index == 0;
                parent_index = is_root ? 0 : ((uint) (((int) Math.Ceiling(((double) my_index) / 2)) - 1));
                child1_index = 2 * my_index + 1;
                child2_index = 2 * my_index + 2;
                parent = is_root ? null : context.Configuration.Members[parent_index];
                child1 = (child1_index < num_members) ? context.Configuration.Members[child1_index] : null;
                child2 = (child2_index < num_members) ? context.Configuration.Members[child2_index] : null;

                if (is_root)
                {
                    // .........................................................................................................................................................................................................                    

                    tokenalarm = context.AlarmClock.Schedule(1, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), null);
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
                    // .........................................................................................................................................................................................................                    
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
                    // .........................................................................................................................................................................................................                    

                    alarm.Reschedule();
                }
            }
        }

        #endregion

        #region Class Token

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Agents_Components_Driver2_Token)]
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
            internal QS.Fx.Serialization.ISerializable[] toaggregate;
            [QS.Fx.Printing.Printable]
            internal QS.Fx.Serialization.ISerializable[] todisseminate;

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info =
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Agents_Components_Driver1_Token);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);

                    // .........................................................................................................................................................................................................                    

                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, round);

                // .........................................................................................................................................................................................................                    
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                round = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);

                // .........................................................................................................................................................................................................                    
            }

            #endregion
        }

        #endregion
    }
}
