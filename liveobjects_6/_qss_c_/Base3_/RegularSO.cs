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
    [System.Serializable]
    public class RegularSO : System.Runtime.Serialization.ISerializable
    {
        public RegularSO()
        {
        }

        public RegularSO(object encapsulatedObject)
        {
            this.encapsulatedObject = encapsulatedObject;
        }

        public RegularSO(byte[] bytes)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            encapsulatedObject = ((QS._core_c_.Serialization.ISerializer)QS._core_c_.Serialization.Serializer1.Serializer).Deserialize(stream);
        }

        private object encapsulatedObject;

        public object EncapsulatedObject
        {
            get { return encapsulatedObject; }
            set { encapsulatedObject = value; }
        }

        protected RegularSO(System.Runtime.Serialization.SerializationInfo information, System.Runtime.Serialization.StreamingContext context)
        {
            byte[] bytes = (byte[])information.GetValue("bytes", typeof(byte[]));
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            encapsulatedObject = ((QS._core_c_.Serialization.ISerializer)QS._core_c_.Serialization.Serializer1.Serializer).Deserialize(stream);
        }

        public byte[] BytesOf()
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            ((QS._core_c_.Serialization.ISerializer)QS._core_c_.Serialization.Serializer1.Serializer).Serialize(encapsulatedObject, stream);
            byte[] bytes = new byte[(int)stream.Length];
            Buffer.BlockCopy(stream.GetBuffer(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        #region ISerializable Members

        void System.Runtime.Serialization.ISerializable.GetObjectData(
            System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("bytes", this.BytesOf(), typeof(byte[]));
        }

        #endregion
    }
}
