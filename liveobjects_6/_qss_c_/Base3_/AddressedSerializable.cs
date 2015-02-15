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

namespace QS._qss_c_.Base3_
{
    public abstract class AddressedSerializable<AddressClass, MessageClass> : QS.Fx.Serialization.ISerializable, Base3_.IKnownClass
        where AddressClass : QS.Fx.Serialization.ISerializable, new()
        where MessageClass : QS.Fx.Serialization.ISerializable, new()
    {
        public AddressedSerializable()
        {
        }

        public AddressedSerializable(AddressClass address, MessageClass message)
        {
            this.address = address;
            this.message = message;
        }

        private AddressClass address;
        private MessageClass message;

        #region Accessors

        public AddressClass Address
        {
            get { return address; }
        }

        public MessageClass Message
        {
            get { return message; }
        }

        #endregion

        public override string ToString()
        {
            return "(" + address.ToString() + ", " + message.ToString() + ")";
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return address.SerializableInfo.CombineWith(message.SerializableInfo).Extend((ushort)this.ClassID, 0, 0, 0);
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            address.SerializeTo(ref header, ref data);
            message.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            address = new AddressClass();
            address.DeserializeFrom(ref header, ref data);
            message = new MessageClass();
            message.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IKnownClass Members

        public abstract ClassID ClassID
        {
            get;
        }

        #endregion
    }

    public abstract class AddressedSerializable<AddressClass> 
        : QS.Fx.Serialization.ISerializable, Base3_.IKnownClass where AddressClass : QS.Fx.Serialization.ISerializable, new()
    {
        public AddressedSerializable()
        {
        }

        public AddressedSerializable(AddressClass address, QS.Fx.Serialization.ISerializable message)
        {
            this.address = address;
            this.message = message;
        }

        private AddressClass address;
        private QS.Fx.Serialization.ISerializable message;

        #region Accessors

        public AddressClass Address
        {
            get { return address; }
        }

        public QS.Fx.Serialization.ISerializable Message
        {
            get { return message; }
        }

        #endregion

        public override string ToString()
        {
            return "(" + address.ToString() + ", " + message.ToString() + ")";
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return address.SerializableInfo.CombineWith(message.SerializableInfo).Extend(
                    (ushort)this.ClassID, sizeof(ushort), 0, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer + header.Offset)) = message.SerializableInfo.ClassID;
            }
            header.consume(sizeof(ushort));
            address.SerializeTo(ref header, ref data);
            message.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID;
            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer + header.Offset));
            }
            header.consume(sizeof(ushort));
            address = new AddressClass();
            address.DeserializeFrom(ref header, ref data);
            message = QS._core_c_.Base3.Serializer.CreateObject(classID);
            message.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IKnownClass Members

        public abstract ClassID ClassID
        {
            get;
        }

        #endregion
    }
}
