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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_Token)]
    public sealed class Token : QS.Fx.Serialization.ISerializable
    {
        // .............................TO FIX!

        public Token(string machineName, uint machineIncarnation, uint viewSeqNo, uint maximumSent)
        {
            this.machineName = machineName;
            this.machineIncarnation = machineIncarnation;
            this.viewSeqNo = viewSeqNo;
            this.maximumSent = maximumSent;
        }

        public Token()
        {
        }

        private string machineName;
        private uint machineIncarnation, viewSeqNo, maximumSent;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public string MachineName
        {
            get { return machineName; }
            set { machineName = value; }
        }

        [QS.Fx.Printing.Printable]
        public uint MachineIncarnation
        {
            get { return machineIncarnation; }
            set { machineIncarnation = value; }
        }

        [QS.Fx.Printing.Printable]
        public uint ViewSeqNo
        {
            get { return viewSeqNo; }
            set { viewSeqNo = value; }
        }

        [QS.Fx.Printing.Printable]
        public uint MaximumSent
        {
            get { return maximumSent; }
            set { maximumSent = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Machine_Components_Token, 0);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, machineName);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, machineName);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, machineIncarnation);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, viewSeqNo);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, maximumSent);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            machineName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            machineIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            viewSeqNo = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            maximumSent = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }
    }
}
