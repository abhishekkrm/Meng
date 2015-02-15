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
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Expressions_Intersection)]
    public class Intersection : Expression, QS.Fx.Serialization.ISerializable
    {
        public Intersection()
        {
        }

        public Intersection(params Expression[] e)
        {
            this.e = e;
        }

        private Expression[] e;

        public Expression[] Expressions
        {
            get { return e; }
            set { e = value; }
        }

        public override string ToString()
        {
            return "Intersection(" + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Expression>(e, ", ") + ")";
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Intersection; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int size = sizeof(ushort) * (1 + e.Length);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Expressions_Intersection, size, size, 0);
                foreach (Expression expression in e)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)expression).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)pbuffer) = (ushort)e.Length;
                pheader += sizeof(ushort);
                foreach (Expression expression in e)
                {
                    *((ushort*)pbuffer) = ((QS.Fx.Serialization.ISerializable)expression).SerializableInfo.ClassID;
                    pheader += sizeof(ushort);
                }
            }
            header.consume(sizeof(ushort) * (1 + e.Length));
            foreach (Expression expression in e)
            {
                ((QS.Fx.Serialization.ISerializable)expression).SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                count = (int)(*((ushort*)pbuffer));
                pheader += sizeof(ushort);
                e = new Expression[count];
                for (int ind = 0; ind < count; ind++)
                {
                    ushort classID = *((ushort*)pbuffer);
                    pheader += sizeof(ushort);
                    e[ind] = (Expression) QS._core_c_.Base3.Serializer.CreateObject(classID);
                }
            }
            header.consume(sizeof(ushort) * (1 + count));
            for (int ind = 0; ind < count; ind++)
            {
                ((QS.Fx.Serialization.ISerializable) e[ind]).DeserializeFrom(ref header, ref data);
            }
        }

        #endregion
    }
}
