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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_Append)]
    public class Append : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public Append(string machineName, QS._qss_x_.Base1_.Address replicaAddress, uint machineIncarnation, uint viewSeqNo,
            uint messageSeqNo, IList<ServiceControl.IServiceControllerOperation> newOperations)
        {
            this.machineName = machineName;
            this.replicaAddress = replicaAddress;
            this.machineIncarnation = machineIncarnation;
            this.viewSeqNo = viewSeqNo;
            this.messageSeqNo = messageSeqNo;
            this.newOperations = newOperations;
        }

        public Append()
        {
        }

        #endregion

        #region Fields

        private string machineName;
        private QS._qss_x_.Base1_.Address replicaAddress;
        private uint machineIncarnation, viewSeqNo, messageSeqNo;
        private IList<ServiceControl.IServiceControllerOperation> newOperations;

        #endregion

        #region Accessors

        [QS.Fx.Printing.Printable]
        public string MachineName
        {
            get { return machineName; }
            set { machineName = value; }
        }

        [QS.Fx.Printing.Printable]
        public QS._qss_x_.Base1_.Address ReplicaAddress
        {
            get { return replicaAddress; }
            set { replicaAddress = value; }
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
        public uint MessageSeqNo
        {
            get { return messageSeqNo; }
            set { messageSeqNo = value; }
        }

        [QS.Fx.Printing.Printable]
        public IList<ServiceControl.IServiceControllerOperation> NewOperations
        {
            get { return newOperations; }
            set { newOperations = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Machine_Components_Append, 
                    sizeof(ushort) * (1 + ((newOperations != null) ? newOperations.Count : 0)));
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, machineName);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)replicaAddress).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                if (newOperations != null)
                {
                    foreach (ServiceControl.IServiceControllerOperation newOperation in newOperations)
                        info.AddAnother(((QS.Fx.Serialization.ISerializable)newOperation).SerializableInfo);
                }
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, machineName);
            ((QS.Fx.Serialization.ISerializable)replicaAddress).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, machineIncarnation);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, viewSeqNo);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, messageSeqNo);
            fixed (byte* pheaderarray = header.Array)
            {
                byte* pheader = pheaderarray + header.Offset;
                *((ushort*) pheader) = (ushort)((newOperations != null) ? newOperations.Count : 0);
                pheader += sizeof(ushort);
                foreach (ServiceControl.IServiceControllerOperation newOperation in newOperations)
                {
                    *((ushort*) pheader) = ((QS.Fx.Serialization.ISerializable)newOperation).SerializableInfo.ClassID;
                    pheader += sizeof(ushort);
                }
            }
            header.consume(sizeof(ushort) * (1 + ((newOperations != null) ? newOperations.Count : 0)));
            if (newOperations != null)
            {
                foreach (ServiceControl.IServiceControllerOperation newOperation in newOperations)
                    ((QS.Fx.Serialization.ISerializable)newOperation).SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            machineName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            replicaAddress = new QS._qss_x_.Base1_.Address();
            ((QS.Fx.Serialization.ISerializable)replicaAddress).DeserializeFrom(ref header, ref data);
            machineIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            viewSeqNo = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            messageSeqNo = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            int noperations;
            fixed (byte* pheaderarray = header.Array)
            {
                byte* pheader = pheaderarray + header.Offset;
                noperations = (int)(*((ushort*)pheader));
                pheader += sizeof(ushort);
                if (noperations > 0)
                {
                    newOperations = new List<ServiceControl.IServiceControllerOperation>(noperations);
                    for (int ind = 0; ind < noperations; ind++)
                    {
                        ServiceControl.IServiceControllerOperation newOperation =
                            (ServiceControl.IServiceControllerOperation)QS._core_c_.Base3.Serializer.CreateObject(*((ushort*)pheader));
                        pheader += sizeof(ushort);
                        newOperations.Add(newOperation);
                    }
                }
                else
                    newOperations = null;
            }
            header.consume(sizeof(ushort) * (1 + ((newOperations != null) ? newOperations.Count : 0)));
            if (newOperations != null)
            {
                foreach (ServiceControl.IServiceControllerOperation newOperation in newOperations)
                    ((QS.Fx.Serialization.ISerializable)newOperation).DeserializeFrom(ref header, ref data);
            }
        }

        #endregion

        #region Overrides from System.Object

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion
    }
}
