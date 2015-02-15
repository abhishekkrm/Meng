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

namespace QS._qss_c_.Embeddings2.Types
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Embeddings2_Types_ReferencedType)]
    public class ReferencedType : QS.Fx.Serialization.ISerializable, IEquatable<ReferencedType>
    {
        public ReferencedType()
        {
        }

        public ReferencedType(System.Type type)
        {
            // if (!type.IsPrimitive)
            //    throw new Exception("Currently only primitive types are supported, cannot reference \"" + type.Name + "\".");
            name = type.Name; 
        }

        [QS.Fx.Printing.Printable]
        private string name;

        public System.Type Type
        {
            get { return System.Type.GetType(name); }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {                
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Embeddings2_Types_ReferencedType, sizeof(ushort), sizeof(ushort), 1);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)(pheader + sizeof(ushort))) = (ushort) nameBytes.Length;
            }
            header.consume(sizeof(ushort));
            data.Add(new QS.Fx.Base.Block(nameBytes));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int nbytes;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                nbytes = (int)(*((ushort*)(pheader + sizeof(ushort))));
            }
            header.consume(sizeof(ushort));
            name = System.Text.Encoding.ASCII.GetString(data.Array, data.Offset, nbytes);
            data.consume(nbytes);
        }

        #endregion

        #region IEquatable<ReferencedType> Members

        bool IEquatable<ReferencedType>.Equals(ReferencedType other)
        {
            return name.Equals(other.name);
        }

        #endregion
    }
}
