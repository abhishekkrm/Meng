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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Agents_Base_ParentInfo)]
    public sealed class ParentInfo : QS.Fx.Serialization.ISerializable
    {
        public ParentInfo(QS._qss_x_.Base1_.SessionID sessionid, QS._qss_x_.Base1_.AgentID id, QS._qss_x_.Base1_.AgentIncarnation incarnation)
        {
            this.sessionid = sessionid;
            this.id = id;
            this.incarnation = incarnation;
        }

        public ParentInfo()
        {
        }

        [QS.Fx.Printing.Printable]
        internal QS._qss_x_.Base1_.SessionID sessionid;
        [QS.Fx.Printing.Printable]
        internal QS._qss_x_.Base1_.AgentID id;
        [QS.Fx.Printing.Printable]
        internal QS._qss_x_.Base1_.AgentIncarnation incarnation;

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Agents_Base_ParentInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) sessionid).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) id).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) incarnation).SerializableInfo);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) sessionid).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) id).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) incarnation).SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            sessionid = new QS._qss_x_.Base1_.SessionID();
            ((QS.Fx.Serialization.ISerializable) sessionid).DeserializeFrom(ref header, ref data);
            id = new QS._qss_x_.Base1_.AgentID();
            ((QS.Fx.Serialization.ISerializable) id).DeserializeFrom(ref header, ref data);
            incarnation = new QS._qss_x_.Base1_.AgentIncarnation();
            ((QS.Fx.Serialization.ISerializable) incarnation).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
