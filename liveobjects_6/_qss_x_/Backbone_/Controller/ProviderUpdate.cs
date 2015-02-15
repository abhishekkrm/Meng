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

namespace QS._qss_x_.Backbone_.Controller
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Backbone_Controller_ProviderUpdate)]
    public sealed class ProviderUpdate : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public ProviderUpdate(string name, QS.Fx.Base.ID id, UpdateType updatetype, TopicUpdate[] topics)
        {
            this.name = name;
            this.id = id;
            this.updatetype = updatetype;
            this.topics = topics;
        }

        public ProviderUpdate()
        {
        }

        #endregion

        #region Overrides from System.Object

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private string name;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Base.ID id;
        [QS.Fx.Printing.Printable]
        UpdateType updatetype;
        [QS.Fx.Printing.Printable]
        private TopicUpdate[] topics;

        #endregion

        #region Accessors

        public string Name
        {
            get { return name; }
        }

        public QS.Fx.Base.ID ID
        {
            get { return id; }
        }

        public UpdateType UpdateType
        {
            get { return updatetype; }
        }

        public TopicUpdate[] Topics
        {
            get { return topics; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Backbone_Controller_ProviderUpdate, 0);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, name);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) id).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Byte(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortArrayOf<TopicUpdate>(ref info, topics);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, name);
            ((QS.Fx.Serialization.ISerializable) id).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Byte(ref header, ref data, (byte) updatetype);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortArrayOf<TopicUpdate>(ref header, ref data, topics);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            name = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            id = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) id).DeserializeFrom(ref header, ref data);
            updatetype = (UpdateType) QS._qss_c_.Base3_.SerializationHelper.Deserialize_Byte(ref header, ref data);
            topics = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortArrayOf<TopicUpdate>(ref header, ref data);
        }

        #endregion
    }
}
