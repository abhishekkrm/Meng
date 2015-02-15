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

namespace QS._qss_c_.Membership_3_.Responses
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Responses_Opened)]
    public class Opened : Response
    {
        public Opened()
        {
        }

        public Opened(int sequenceNo, Base3_.GroupID groupID, Interface.IGroupType groupType, Interface.IGroupAttributes groupAttributes)
            : base(sequenceNo)
        {
            this.groupID = groupID;
            this.groupType = groupType;
            this.groupAttributes = groupAttributes;
        }

        [QS.Fx.Printing.Printable]
        private Base3_.GroupID groupID;
        [QS.Fx.Printing.Printable]
        private Interface.IGroupType groupType;
        [QS.Fx.Printing.Printable]
        private Interface.IGroupAttributes groupAttributes;

        public override ResponseType ResponseType
        {
            get { return ResponseType.Opened; }
        }

        #region ISerializable Members

        public override QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Responses_Opened, 2 * sizeof(ushort), 2 * sizeof(ushort), 0);
                info.AddAnother(base.SerializableInfo);
                info.AddAnother(groupID.SerializableInfo);
                if (groupType != null)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)groupType).SerializableInfo);
                if (groupAttributes != null)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)groupAttributes).SerializableInfo);
                return info;
            }
        }

        public unsafe override void SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            base.SerializeTo(ref header, ref data);
            groupID.SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)(pheader)) =
                    (groupType != null) ? ((QS.Fx.Serialization.ISerializable)groupType).SerializableInfo.ClassID : (ushort)ClassID.Nothing;
                *((ushort*)(pheader + sizeof(ushort))) =
                    (groupAttributes != null) ? ((QS.Fx.Serialization.ISerializable)groupAttributes).SerializableInfo.ClassID : (ushort)ClassID.Nothing;
            }
            header.consume(2 * sizeof(ushort));
            if (groupType != null)
                ((QS.Fx.Serialization.ISerializable)groupType).SerializeTo(ref header, ref data);
            if (groupAttributes != null)
                ((QS.Fx.Serialization.ISerializable)groupAttributes).SerializeTo(ref header, ref data);
        }

        public unsafe override void DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            base.DeserializeFrom(ref header, ref data);
            groupID = new QS._qss_c_.Base3_.GroupID();
            groupID.DeserializeFrom(ref header, ref data);
            ushort classID1, classID2;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                classID1 = *((ushort*)(pheader));
                classID2 = *((ushort*)(pheader + sizeof(ushort)));
            }
            header.consume(2 * sizeof(ushort));
            groupType = (Interface.IGroupType)QS._core_c_.Base3.Serializer.CreateObject(classID1);
            if (groupType != null)
                ((QS.Fx.Serialization.ISerializable)groupType).DeserializeFrom(ref header, ref data);
            groupAttributes = (Interface.IGroupAttributes)QS._core_c_.Base3.Serializer.CreateObject(classID2);
            if (groupAttributes != null)
                ((QS.Fx.Serialization.ISerializable)groupAttributes).DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
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
    }
}
