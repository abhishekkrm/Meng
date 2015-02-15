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
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Expressions_Minus)]
    public class Minus : Expression, QS.Fx.Serialization.ISerializable
    {
        public Minus()
        {
        }

        public Minus(Expression left, Expression right)
        {
            this.left = left;
            this.right = right;
        }

        private Expression left, right;

        public Expression Left
        {
            get { return left; }
            set { left = value; }
        }

        public Expression Right
        {
            get { return right; }
            set { right = value; }
        }

        public override string ToString()
        {
            return "Minus(" + left.ToString() + ", " + right.ToString() + ")";
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Minus; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Expressions_Minus, 2 * sizeof(ushort), 2 * sizeof(ushort), 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)left).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)right).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)pbuffer) = ((QS.Fx.Serialization.ISerializable)left).SerializableInfo.ClassID;
                *((ushort*)pbuffer) = ((QS.Fx.Serialization.ISerializable)right).SerializableInfo.ClassID;
            }
            header.consume(2 * sizeof(ushort));
            ((QS.Fx.Serialization.ISerializable)left).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)right).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID1, classID2;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                classID1 = *((ushort*)pbuffer);
                classID2 = *((ushort*)pbuffer);
            }
            header.consume(2 * sizeof(ushort));
            left = (Expression)QS._core_c_.Base3.Serializer.CreateObject(classID1);
            ((QS.Fx.Serialization.ISerializable)left).DeserializeFrom(ref header, ref data);
            right = (Expression)QS._core_c_.Base3.Serializer.CreateObject(classID2);
            ((QS.Fx.Serialization.ISerializable)right).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
