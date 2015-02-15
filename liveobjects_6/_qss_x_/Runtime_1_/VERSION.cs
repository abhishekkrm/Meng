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
    [QS._qss_x_.Language_.Structure_.ValueType("VERSION", QS._qss_x_.Language_.Structure_.PredefinedType.VERSION)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    public struct VERSION 
        : IComparable<VERSION>, IEquatable<VERSION>, IComparable, QS.Fx.Serialization.ISerializable, IValue<VERSION>
    {
        #region Constructor

        public VERSION(uint major, uint minor)
        {
            this.major = major;
            this.minor = minor;
        }

        #endregion

        #region Fields

        private uint major, minor;

        #endregion

        #region Accessors

        public uint Major
        {
            get { return major; }
            set { major = value; }
        }

        public uint Minor
        {
            get { return minor; }
            set { minor = value; }
        }

        #endregion

        #region IComparable<Version> Members

        public int CompareTo(VERSION other)
        {
            int result = major.CompareTo(other.major);
            if (result == 0)
                result = minor.CompareTo(other.minor);
            return result;
        }

        #endregion

        #region IEquatable<Version> Members

        public bool Equals(VERSION other)
        {
            return major == other.major && minor.Equals(other.minor);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is VERSION)
            {
                VERSION other = (VERSION) obj;
                int result = major.CompareTo(other.major);
                if (result == 0)
                    result = minor.CompareTo(other.minor);
                return result;
            }
            else
                throw new NotSupportedException();
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return major.ToString() + "." + minor.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is VERSION)
            {
                VERSION other = (VERSION)obj;
                return major == other.major && minor.Equals(other.minor);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return major.GetHashCode() ^ minor.GetHashCode();
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Runtime_Version, 2 * sizeof(uint)); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((uint*)pheader) = major;
                *((uint*) (pheader + sizeof(uint))) = minor;
            }
            header.consume(2 * sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                major = (*((uint*)pheader));
                minor = (*((uint*)(pheader + sizeof(uint))));
            }
            header.consume(2 * sizeof(uint));            
        }

        #endregion

        #region IsDefined

        [QS._qss_x_.Language_.Structure_.Operator("IsDefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.IsDefined, true)]
        public bool IsDefined
        {
            get { return major != 0 && minor != 0; }
        }

        #endregion

        #region Undefined

        [QS._qss_x_.Language_.Structure_.Operator("Undefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.Undefined)]
        public static VERSION Undefined
        {
            get { return new VERSION(0, 0); }
        }

        #endregion

        #region IValue<VERSION> Members

        bool IValue<VERSION>.IsDefined
        {
            get { return (major != 0) || (minor != 0); }
        }

        void IValue<VERSION>.SetTo(VERSION value)
        {
            major = value.major;
            minor = value.minor;
        }

        VERSION IValue<VERSION>.Clone()
        {
            return new VERSION(major, minor);
        }

        VERSION IValue<VERSION>.Erase()
        {
            throw new NotSupportedException();
        }

        bool IValue<VERSION>.IsLess(VERSION other)
        {
            if (major < other.major)
                return true;
            else if (major > other.major)
                return false;
            else
                return minor <= other.minor;
        }

        #endregion
    }
}
