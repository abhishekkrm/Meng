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

namespace QS._qss_x_._Machine_2_.Replicated
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Machine_Persisted)]
    public sealed class Persisted : IPersisted, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Persisted()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Base.ID replicaID = new QS.Fx.Base.ID(), machineID = new QS.Fx.Base.ID();
        private uint replicaIncarnation;
        private MembershipView view = new MembershipView(0, 0, new MemberInfo[0], 0, 0);
        private string applicationID = string.Empty, replicaName = string.Empty, machineName = string.Empty;

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Machine_Persisted_Operation, sizeof(uint));
                info.AddAnother(((QS.Fx.Serialization.ISerializable) replicaID).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) machineID).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, applicationID);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) view).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, replicaName);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, machineName);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((uint*) pheader) = replicaIncarnation;
            }
            header.consume(sizeof(uint));
            ((QS.Fx.Serialization.ISerializable)replicaID).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)machineID).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, applicationID);
            ((QS.Fx.Serialization.ISerializable) view).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, replicaName);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, machineName);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                replicaIncarnation  = *((uint*)pheader);
            }
            header.consume(sizeof(uint));
            ((QS.Fx.Serialization.ISerializable)replicaID).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)machineID).DeserializeFrom(ref header, ref data);
            applicationID = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)view).DeserializeFrom(ref header, ref data);
            replicaName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            machineName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
        }

        #endregion

        #region IPersisted Members

        QS.Fx.Base.ID IPersisted.ReplicaID
        {
            get { return replicaID; }
        }

        string IPersisted.ReplicaName
        {
            get { return replicaName; }
        }

        uint IPersisted.ReplicaIncarnation
        {
            get { return replicaIncarnation; }
        }

        QS.Fx.Base.ID IPersisted.MachineID
        {
            get { return machineID; }
        }

        string IPersisted.MachineName
        {
            get { return machineName; }
        }

        MembershipView IPersisted.View
        {
            get { return view; }
        }

        string IPersisted.ApplicationID
        {
            get { return applicationID; }
        }

        #endregion

        #region IOperation

        public interface IOperation : QS._qss_x_.Persistence_.IOperation<Persisted>, QS.Fx.Serialization.ISerializable
        {
        }

        #endregion

        #region Operation

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Machine_Persisted_Operation)]
        public sealed class Operation : IOperation, IEnumerable<IOperation>
        {
            #region Constructor

            public Operation()
            {
            }

            #endregion

            #region Fields

            private IList<IOperation> operations = new List<IOperation>();

            #endregion

            #region IOperation<Persisted> Members

            void QS._qss_x_.Persistence_.IOperation<Persisted>.Execute(Persisted target)
            {
                foreach (IOperation operation in operations)
                    operation.Execute(target);
            }

            #endregion

            #region ISerializable Members

            unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = 
                        new QS.Fx.Serialization.SerializableInfo(
                            (ushort) QS.ClassID.Machine_Persisted, sizeof(int) + sizeof(ushort) * operations.Count);
                    foreach (IOperation operation in operations)
                        info.AddAnother(operation.SerializableInfo);
                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* _pheader = header.Array)
                {
                    byte* pheader = _pheader + header.Offset;
                    *((int*) pheader) = operations.Count;
                    pheader += sizeof(int);
                    foreach (IOperation operation in operations)
                    {
                        *((ushort*)pheader) = (ushort) operation.SerializableInfo.ClassID;
                        pheader += sizeof(ushort);
                    }
                }
                header.consume(sizeof(int) + sizeof(ushort) * operations.Count);
                foreach (IOperation operation in operations)
                    operation.SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* _pheader = header.Array)
                {
                    byte* pheader = _pheader + header.Offset;
                    int count = *((int *) pheader);
                    pheader += sizeof(int);
                    while (count-- > 0)
                    {
                        operations.Add((IOperation) QS._core_c_.Base3.Serializer.CreateObject(*((ushort*)pheader)));
                        pheader += sizeof(ushort);
                    }
                }
                header.consume(sizeof(int) + sizeof(ushort) * operations.Count);
                foreach (IOperation operation in operations)
                    operation.DeserializeFrom(ref header, ref data);
            }

            #endregion

            #region Accessors

            public void Add(IOperation operation)
            {
                operations.Add(operation);
            }

            #endregion

            #region IEnumerable<IOperation> Members

            IEnumerator<IOperation> IEnumerable<IOperation>.GetEnumerator()
            {
                return operations.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return operations.GetEnumerator();
            }

            #endregion

            #region Operations

            #region SetMachineID

            [QS.Fx.Serialization.ClassID(QS.ClassID.Machine_Persisted_Operation_SetMachineID)]
            public sealed class SetMachineID : IOperation
            {
                public SetMachineID(QS.Fx.Base.ID id)
                {
                    this.id = id;
                }

                public SetMachineID()
                {
                }

                private QS.Fx.Base.ID id;

                #region IOperation<Persisted> Members

                void QS._qss_x_.Persistence_.IOperation<Persisted>.Execute(Persisted target)
                {
                    target.machineID = id;
                }

                #endregion

                #region ISerializable Members

                QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
                {
                    get
                    {
                        QS.Fx.Serialization.SerializableInfo info =
                            new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Machine_Persisted_Operation_SetMachineID);
                        info.AddAnother(((QS.Fx.Serialization.ISerializable)id).SerializableInfo);
                        return info;
                    }
                }

                void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
                {
                    ((QS.Fx.Serialization.ISerializable)id).SerializeTo(ref header, ref data);
                }

                void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
                {
                    id = new QS.Fx.Base.ID();
                    ((QS.Fx.Serialization.ISerializable)id).DeserializeFrom(ref header, ref data);
                }

                #endregion
            }

            #endregion

            #endregion
        }

        #endregion
    }
}
