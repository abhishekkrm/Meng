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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Agents_Base_AggregationToken)]
    public sealed class AggregationToken : QS.Fx.Serialization.ISerializable
    {
        public AggregationToken()
        {
        }

        [QS.Fx.Printing.Printable]
        internal uint fingerprint, disseminationround, maximumround;
        [QS.Fx.Printing.Printable]
        internal bool isvalid;
        [QS.Fx.Printing.Printable]
        internal ParentInfo[] parents;
        [QS.Fx.Printing.Printable]
        internal QS.Fx.Serialization.ISerializable[] tokens;

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Agents_Base_AggregationToken);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortArrayOf<ParentInfo>(ref info, parents);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                if (tokens != null)
                {
                    for (int ind = 0; ind < tokens.Length; ind++)
                    {
                        QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                        info.AddAnother(tokens[ind].SerializableInfo);
                    }
                }
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, maximumround);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, disseminationround);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, fingerprint);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, isvalid);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortArrayOf<ParentInfo>(ref header, ref data, parents);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, (ushort)((tokens != null) ? tokens.Length : 0));
            if (tokens != null)
            {
                for (int ind = 0; ind < tokens.Length; ind++)
                {
                    QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, tokens[ind].SerializableInfo.ClassID);
                    tokens[ind].SerializeTo(ref header, ref data);
                }
            }
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            maximumround = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            disseminationround = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            fingerprint = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            isvalid = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            parents = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortArrayOf<ParentInfo>(ref header, ref data);
            int ntokens = (int) QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
            if (ntokens > 0)
            {
                tokens = new QS.Fx.Serialization.ISerializable[ntokens];
                for (int ind = 0; ind < ntokens; ind++)
                {
                    ushort classid = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
                    tokens[ind] = QS._core_c_.Base3.Serializer.CreateObject(classid);
                    tokens[ind].DeserializeFrom(ref header, ref data);
                }
            }
            else
                tokens = null;
        }

        #endregion
    }
}
