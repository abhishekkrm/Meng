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

namespace QS._qss_c_.Membership_3_.Requests
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Requests_Open)]
    public sealed class Open : Request
    {
        public Open()
        {
        }

        public Open(int sequenceNo, Expressions.Expression groupSpecification, Interface.OpeningMode openingMode, 
            Interface.AccessMode accessMode, Interface.IGroupType groupType, Interface.IGroupAttributes groupAttributes) : base(sequenceNo)
        {
            // a little bit of validation
            if (groupSpecification.Type == QS._qss_c_.Membership_3_.Expressions.ExpressionType.Group)
            {
                if (openingMode != QS._qss_c_.Membership_3_.Interface.OpeningMode.Open && groupType == null)
                    throw new System.Exception("Opening mode allows the group to be created on demand, but group type has not been specified.");

                if (groupType == null && groupAttributes != null)
                    throw new System.Exception("Cannot specify attributes if group type is not specified, attributes make sense only in a specific context.");
            }
            else
            {
                if (openingMode != QS._qss_c_.Membership_3_.Interface.OpeningMode.Open)
                    throw new System.Exception("Derived groups cannot be explicitly created, only opened provided that the underlying base group exist.");

                if ((accessMode & QS._qss_c_.Membership_3_.Interface.AccessMode.Member) == QS._qss_c_.Membership_3_.Interface.AccessMode.Member)
                    throw new System.Exception("Cannot explicitly join a derived group as a member, only as a client or a passive observer.");
            }

            this.groupSpecification = groupSpecification;
            this.openingMode = openingMode;
            this.accessMode = accessMode;
            this.groupType = groupType;
            this.groupAttributes = groupAttributes;
        }

        [QS.Fx.Printing.Printable]
        private Expressions.Expression groupSpecification;
        [QS.Fx.Printing.Printable]
        private Interface.OpeningMode openingMode;
        [QS.Fx.Printing.Printable]
        private Interface.AccessMode accessMode;
        [QS.Fx.Printing.Printable]
        private Interface.IGroupType groupType;
        [QS.Fx.Printing.Printable]
        private Interface.IGroupAttributes groupAttributes;

        #region Accessors

        public override RequestType RequestType
        {
            get { return RequestType.Open; }
        }

        public Expressions.Expression GroupSpecification
        {
            get { return groupSpecification; }
        }

        public Interface.OpeningMode OpeningMode
        {
            get { return openingMode; }
        }

        public Interface.AccessMode AccessMode
        {
            get { return accessMode; }
        }

        public Interface.IGroupType GroupType
        {
            get { return groupType; }
        }

        public Interface.IGroupAttributes GroupAttributes
        {
            get { return groupAttributes; }
        }

        #endregion

        #region ISerializable Members

        public override QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Requests_Open, 3 * sizeof(ushort) + 2 * sizeof(byte), 3 * sizeof(ushort) + 2 * sizeof(byte), 0);                    
                info.AddAnother(base.SerializableInfo);                
                info.AddAnother(((QS.Fx.Serialization.ISerializable)groupSpecification).SerializableInfo);
                if (groupType != null)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)groupType).SerializableInfo);
                if (groupAttributes != null)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)groupAttributes).SerializableInfo);
                return info;
            }
        }

        public override unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            base.SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)pheader) = ((QS.Fx.Serialization.ISerializable)groupSpecification).SerializableInfo.ClassID;
                *(pheader + sizeof(ushort)) = (byte)openingMode;
                *(pheader + sizeof(ushort) + sizeof(byte)) = (byte)accessMode;
                *((ushort*)(pheader + sizeof(ushort) + 2 * sizeof(byte))) =
                    (groupType != null) ? ((QS.Fx.Serialization.ISerializable)groupType).SerializableInfo.ClassID : (ushort) ClassID.Nothing;
                *((ushort*)(pheader + 2 * sizeof(ushort) + 2 * sizeof(byte))) = 
                    (groupAttributes != null) ? ((QS.Fx.Serialization.ISerializable)groupAttributes).SerializableInfo.ClassID : (ushort) ClassID.Nothing;
            }
            header.consume(3 * sizeof(ushort) + 2 * sizeof(byte));
            ((QS.Fx.Serialization.ISerializable)groupSpecification).SerializeTo(ref header, ref data);
            if (groupType != null)
                ((QS.Fx.Serialization.ISerializable)groupType).SerializeTo(ref header, ref data);
            if (groupAttributes != null)
                ((QS.Fx.Serialization.ISerializable)groupAttributes).SerializeTo(ref header, ref data);
        }

        public override unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            base.DeserializeFrom(ref header, ref data);
            ushort classID1, classID2, classID3;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                classID1 = *((ushort*)pheader);
                openingMode = (QS._qss_c_.Membership_3_.Interface.OpeningMode)(* (pheader + sizeof(ushort)));
                accessMode = (QS._qss_c_.Membership_3_.Interface.AccessMode)(*(pheader + sizeof(ushort) + sizeof(byte)));
                classID2 = *((ushort*)(pheader + sizeof(ushort) + 2 * sizeof(byte)));
                classID3 = *((ushort*)(pheader + 2 * sizeof(ushort) + 2 * sizeof(byte)));
            }
            header.consume(3 * sizeof(ushort) + 2 * sizeof(byte));
            groupSpecification = (Expressions.Expression) QS._core_c_.Base3.Serializer.CreateObject(classID1);
            ((QS.Fx.Serialization.ISerializable)groupSpecification).DeserializeFrom(ref header, ref data);
            groupType = (Interface.IGroupType)QS._core_c_.Base3.Serializer.CreateObject(classID2);
            if (groupType != null)
                ((QS.Fx.Serialization.ISerializable)groupType).DeserializeFrom(ref header, ref data);
            groupAttributes = (Interface.IGroupAttributes)QS._core_c_.Base3.Serializer.CreateObject(classID3);
            if (groupAttributes != null)
                ((QS.Fx.Serialization.ISerializable)groupAttributes).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
