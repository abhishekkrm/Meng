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

namespace QS._qss_x_.Backbone_.Node
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Backbone_Node_Open)]
    public sealed class Open : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public Open(string name1, QS.Fx.Base.ID id1, ulong endpoint1, string name2, QS.Fx.Base.ID id2, ulong endpoint2, Phase phase)
        {
            this.name1 = name1;
            this.id1 = id1;
            this.endpoint1 = endpoint1;
            this.name2 = name2;
            this.id2 = id2;
            this.endpoint2 = endpoint2;
            this.phase = phase;
        }

        public Open()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private string name1, name2;

        [QS.Fx.Printing.Printable]
        private QS.Fx.Base.ID id1, id2;

        [QS.Fx.Printing.Printable]
        private ulong endpoint1, endpoint2;

        [QS.Fx.Printing.Printable]
        private Phase phase;

        #endregion

        #region Overrides from System.Object

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region Accessors

        public string Name1
        {
            get { return name1; }
            set { name1 = value; }
        }

        public QS.Fx.Base.ID ID1
        {
            get { return id1; }
            set { id1 = value; }
        }

        public ulong Endpoint1
        {
            get { return endpoint1; }
            set { endpoint1 = value; }
        }

        public string Name2
        {
            get { return name2; }
            set { name2 = value; }
        }

        public QS.Fx.Base.ID ID2
        {
            get { return id2; }
            set { id2 = value; }
        }

        public ulong Endpoint2
        {
            get { return endpoint2; }
            set { endpoint2 = value; }
        }

        public Phase Phase
        {
            get { return phase; }
            set { phase = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Backbone_Node_Open, 0);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, name1);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) id1).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt64(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, name2);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) id2).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt64(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Byte(ref info);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, name1);
            ((QS.Fx.Serialization.ISerializable) id1).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt64(ref header, ref data, endpoint1); 
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, name2);
            ((QS.Fx.Serialization.ISerializable) id2).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt64(ref header, ref data, endpoint2);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Byte(ref header, ref data, (byte) phase); 
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            name1 = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            id1 = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) id1).DeserializeFrom(ref header, ref data);
            endpoint1 = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt64(ref header, ref data);
            name2 = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            id2 = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) id2).DeserializeFrom(ref header, ref data);
            endpoint2 = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt64(ref header, ref data);
            phase = (Phase) QS._qss_c_.Base3_.SerializationHelper.Deserialize_Byte(ref header, ref data); 
        }

        #endregion
    }
}
