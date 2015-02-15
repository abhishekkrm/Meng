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

namespace QS._qss_c_.Membership_3_.Notifications
{
    /// <summary>
    /// Sent to request the client to create a new empty group without any view installed.
    /// </summary>
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateGroup)]
    public sealed class CreateGroup : QS.Fx.Serialization.ISerializable
    {
        public CreateGroup()
        {
        }

        public CreateGroup(Base3_.GroupID groupID, QS.Fx.Serialization.ISerializable groupAttributes)
        {
            this.groupID = groupID;
            this.groupAttributes = groupAttributes;
        }

        [QS.Fx.Printing.Printable]
        private Base3_.GroupID groupID;
        [QS.Fx.Printing.Printable]        
        private QS.Fx.Serialization.ISerializable groupAttributes;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public QS.Fx.Serialization.ISerializable GroupAttributes
        {
            get { return groupAttributes; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Notifications_CreateGroup, sizeof(ushort), sizeof(ushort), 0);
                info.AddAnother(groupID.SerializableInfo);
                if (groupAttributes != null)
                    info.AddAnother(groupAttributes.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer)) = (groupAttributes != null) ? groupAttributes.SerializableInfo.ClassID : (ushort) ClassID.Nothing;
            }
            header.consume(sizeof(ushort));
            groupID.SerializeTo(ref header, ref data);
            if (groupAttributes != null)
                groupAttributes.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID;
            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer));
            }
            header.consume(sizeof(ushort));
            groupID.DeserializeFrom(ref header, ref data);
            groupAttributes = QS._core_c_.Base3.Serializer.CreateObject(classID);
            if (groupAttributes != null)
                groupAttributes.DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
