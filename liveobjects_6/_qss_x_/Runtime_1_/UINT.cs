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

namespace QS._qss_x_.Runtime_1_
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._c_uint)]
    [QS._qss_x_.Language_.Structure_.ValueType("UINT", QS._qss_x_.Language_.Structure_.PredefinedType.UINT)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    // [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Runtime_UInt)]
    public sealed class UINT : IValue<UINT>, QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public UINT()
        {
        }

        [QS._qss_x_.Language_.Structure_.Operator(QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, true)]
        public UINT(int number) : this(true, (uint) number)
        {
        }

        [QS._qss_x_.Language_.Structure_.Operator(QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, true)]
        public UINT(uint number) : this(true, number)
        {
        }

        public UINT(bool isdefined, uint number)
        {
            this.isdefined = isdefined;
            this.number = number;
        }

        private static readonly UINT undefined = new UINT(false, 0);
        private static readonly UINT number0 = new UINT(true, 0);
        private static readonly UINT number1 = new UINT(true, 1);

        [QS._qss_x_.Language_.Structure_.Operator("Undefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.Undefined, true)]
        public static UINT Undefined
        {
            get { return undefined; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("Zero", true)]
        public static UINT Zero
        {
            get { return number0; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("One", true)]
        public static UINT One
        {
            get { return number1; }
        }

        #endregion

        #region Fields

        private bool isdefined;
        private uint number;

        #endregion

        #region Accessors

        [QS._qss_x_.Language_.Structure_.Operator("Value", new QS._qss_x_.Language_.Structure_.PredefinedOperator[] { 
            QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, QS._qss_x_.Language_.Structure_.PredefinedOperator.Value }, true)]
        public uint Number
        {
            get { return number; }
            set { number = value; }
        }

        [QS._qss_x_.Language_.Structure_.Operator(QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, true)]
        public uint ToUInt32
        {
            get { return number; }
        }

        [QS._qss_x_.Language_.Structure_.Operator(QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, true)]
        public int ToInt32
        {
            get { return (int) number; }
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Runtime_UInt);
                int headersize = sizeof(uint) + sizeof(bool);
                info.HeaderSize += headersize;
                info.Size += headersize;
                return info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((uint*)pheader) = number;
                pheader += sizeof(uint);
                *((bool*)pheader) = isdefined;
            }
            header.consume(sizeof(uint) + sizeof(bool));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                number = *((uint*)pheader);
                pheader += sizeof(uint);
                isdefined = *((bool*)pheader);
            }
            header.consume(sizeof(uint) + sizeof(bool));
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            if (isdefined)
                return number.ToString();
            else
                return "undefined";
        }

        #endregion

        #region IValue<UInt>.IsDefined

        [QS._qss_x_.Language_.Structure_.Operator("IsDefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.IsDefined, true)]
        public bool IsDefined
        {
            get { return isdefined; }
            set { isdefined = value; }
        }

        #endregion

        #region IValue<UInt>.SetTo

        public void SetTo(UINT other)
        {
            isdefined = other.isdefined;
            number = other.number;
        }

        #endregion

        #region IValue<UInt>.Clone

        public UINT Clone()
        {
            return new UINT(isdefined, number);
        }

        #endregion

        #region IValue<UInt>.Erase

        public UINT Erase()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<UInt> Members

        public int CompareTo(UINT other)
        {
            if (isdefined)
            {
                if (other.isdefined)
                    return number.CompareTo(other.number);
                else
                    return +1;
            }
            else
            {
                if (other.isdefined)
                    return -1;
                else
                    return 0;
            }
        }

        #endregion

        #region IsLess

        [QS._qss_x_.Language_.Structure_.Operator("LTE", QS._qss_x_.Language_.Structure_.PredefinedOperator.LTE, true)]
        public bool IsLess(UINT other)
        {
            return !isdefined || (other.isdefined && (number <= other.number));
        }

        #endregion

        #region IsStrictlyLess

        [QS._qss_x_.Language_.Structure_.Operator("LT", QS._qss_x_.Language_.Structure_.PredefinedOperator.LT, true)]
        public bool IsStrictlyLess(UINT other)
        {
            return other.isdefined && ((!isdefined) || (number < other.number));
        }

        #endregion

        #region IsEqual

        [QS._qss_x_.Language_.Structure_.Operator("EQ", QS._qss_x_.Language_.Structure_.PredefinedOperator.EQ, true)]
        public bool IsEqual(UINT other)
        {
            return isdefined ? (other.isdefined && (number == other.number)) : !other.isdefined;
        }

        #endregion

        #region IsMore

        [QS._qss_x_.Language_.Structure_.Operator("GTE", QS._qss_x_.Language_.Structure_.PredefinedOperator.GTE, true)]
        public bool IsMore(UINT other)
        {
            return !other.isdefined || (isdefined && (other.number <= number));
        }

        #endregion

        #region IsStrictlyMore

        [QS._qss_x_.Language_.Structure_.Operator("GT", QS._qss_x_.Language_.Structure_.PredefinedOperator.GT, true)]
        public bool IsStrictlyMore(UINT other)
        {
            return isdefined && ((!other.isdefined) || (other.number < number));
        }

        #endregion

        #region IsNotEqual

        [QS._qss_x_.Language_.Structure_.Operator("NEQ", QS._qss_x_.Language_.Structure_.PredefinedOperator.NEQ, true)]
        public bool IsNotEqual(UINT other)
        {
            return (isdefined) ? (!other.isdefined || (number != other.number)) : other.isdefined;
        }

        #endregion

        #region Min

        [QS._qss_x_.Language_.Structure_.Operator("Min", QS._qss_x_.Language_.Structure_.PredefinedOperator.Min)]
        public static UINT Min(UINT x, UINT y)
        {
            return (x.isdefined && y.isdefined) ? (new UINT(Math.Min(x.number, y.number))) : UINT.Undefined.Clone();
        }

        #endregion

        #region Min2

        [QS._qss_x_.Language_.Structure_.Operator("Min2")]
        public static UINT Min2(UINT x, UINT y)
        {
            if (x.isdefined)
            {
                if (y.isdefined)
                    return new UINT(Math.Min(x.number, y.number));
                else
                    return x.Clone();
            }
            else
            {
                if (y.isdefined)
                    return y.Clone();
                else
                    return undefined.Clone();
            }
        }

        #endregion

        #region Max

        [QS._qss_x_.Language_.Structure_.Operator("Max", QS._qss_x_.Language_.Structure_.PredefinedOperator.Max)]
        public static UINT Max(UINT x, UINT y)
        {
            return (x.isdefined && y.isdefined) ? (new UINT(Math.Max(x.number, y.number))) : UINT.Undefined.Clone();
        }

        #endregion

        #region Sum

        [QS._qss_x_.Language_.Structure_.Operator("Add", QS._qss_x_.Language_.Structure_.PredefinedOperator.Add)]
        public static UINT Sum(UINT x, UINT y)
        {
            return (x.isdefined && y.isdefined) ? (new UINT(x.number + y.number)) : UINT.Undefined.Clone();
        }

        #endregion
    }
}
