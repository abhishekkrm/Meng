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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateRegion)]
    public sealed class CreateRegion : QS.Fx.Serialization.ISerializable
    {
        public CreateRegion()
        {
        }

        public CreateRegion(Base3_.RegionID regionID, Base3_.RegionSig regionSignature, 
            QS.Fx.Serialization.ISerializable regionAttributes)
        {
            this.regionID = regionID;
            this.regionSignature = regionSignature;
            this.regionAttributes = regionAttributes;
        }

        [QS.Fx.Printing.Printable]
        private Base3_.RegionID regionID;
        [QS.Fx.Printing.Printable]
        private Base3_.RegionSig regionSignature;
        [QS.Fx.Printing.Printable]        
        private QS.Fx.Serialization.ISerializable regionAttributes;

        #region Accessors

        public Base3_.RegionID RegionID
        {
            get { return regionID; }
        }

        public Base3_.RegionSig RegionSignature
        {
            get { return regionSignature; }
        }

        public QS.Fx.Serialization.ISerializable RegionAttributes
        {
            get { return regionAttributes; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Notifications_CreateRegion, 
                    sizeof(bool) + sizeof(ushort), sizeof(bool) + sizeof(ushort), 0);
                info.AddAnother(regionID.SerializableInfo);
                if (regionSignature != null)
                    info.AddAnother(regionSignature.SerializableInfo);
                if (regionAttributes != null)
                    info.AddAnother(regionAttributes.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer)) = (regionAttributes != null) ? regionAttributes.SerializableInfo.ClassID : (ushort) ClassID.Nothing;
                *((bool*)(pbuffer + sizeof(ushort))) = (regionSignature != null);
            }
            header.consume(sizeof(bool) + sizeof(ushort));
            regionID.SerializeTo(ref header, ref data);
            if (regionSignature != null)
                regionSignature.SerializeTo(ref header, ref data);
            if (regionAttributes != null)
                regionAttributes.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID;
            bool included_signature;
            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer));
                included_signature = *((bool*)(pbuffer + sizeof(ushort)));
            }
            header.consume(sizeof(bool) + sizeof(ushort));
            regionID.DeserializeFrom(ref header, ref data);
            if (included_signature)
            {
                regionSignature = new QS._qss_c_.Base3_.RegionSig();
                regionSignature.DeserializeFrom(ref header, ref data);
            }
            regionAttributes = QS._core_c_.Base3.Serializer.CreateObject(classID);
            if (regionAttributes != null)
                regionAttributes.DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
