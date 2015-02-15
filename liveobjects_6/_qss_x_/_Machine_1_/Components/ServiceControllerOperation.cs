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

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ServiceControllerOperation)]
    public class ServiceControllerOperation : ServiceControl.IServiceControllerOperation, QS.Fx.Serialization.ISerializable
    {
        public ServiceControllerOperation(bool persistent, string comment)
        {
            this.persistent = persistent;
        }

        public ServiceControllerOperation()
        {
        }

        private bool persistent;
        private string comment;

        #region IServiceControllerOperation Members

        bool QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerOperation.Persistent
        {
            get { return persistent; }
        }

        string QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerOperation.Comment
        {
            get { return comment; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Machine_Components_ServiceControllerOperation);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, comment);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, persistent);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, comment);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            persistent = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            comment = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
        }

        #endregion
    }
}
