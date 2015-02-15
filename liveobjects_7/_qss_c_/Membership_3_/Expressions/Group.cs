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

namespace QS._qss_c_.Membership_3_.Expressions
{
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Expressions_Group)]
    public class Group : Expression, QS.Fx.Serialization.ISerializable
    {
        public Group()
        {
        }

        public Group(string name)
        {
            this.name = name;
        }

        private string name; 

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Group; }
        }

        public override string ToString()
        {
            return "Group(" + name + ")";
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Expressions_Group, sizeof(ushort), sizeof(ushort), 1); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            byte[] nameBytes = System.Text.Encoding.Unicode.GetBytes(name);
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer + header.Offset)) = (ushort) nameBytes.Length;
            }
            data.Add(new QS.Fx.Base.Block(nameBytes));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int nbytes;
            fixed (byte* pbuffer = header.Array)
            {
                nbytes = (int)(*((ushort*)(pbuffer + header.Offset)));
            }
            name = System.Text.Encoding.Unicode.GetString(data.Array, data.Offset, nbytes);
            data.consume(nbytes);            
        }

        #endregion
    }
}
