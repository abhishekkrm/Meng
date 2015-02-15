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

namespace QS._qss_x_.Live_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Live_InterfaceID)]
    public sealed class InterfaceID 
        : QS.Fx.Serialization.ISerializable, IComparable<InterfaceID>, IComparable, IEquatable<InterfaceID>
    {
        #region Constructors

        public InterfaceID(Guid guid)
        {
            this.num = new QS.Fx.Base.Int128(guid);
        }

        public InterfaceID()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Base.Int128 num;

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Live_InterfaceID);
                info.AddAnother(num.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            num.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            num.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return num.ToString();
        }

        public override int GetHashCode()
        {
            return num.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            InterfaceID other = obj as InterfaceID;
            return (other != null) && num.Equals(other.num);
        }

        #endregion

        #region IComparable<InterfaceID> Members

        int IComparable<InterfaceID>.CompareTo(InterfaceID other)
        {
            if (other != null)
                return num.CompareTo(other.num);
            else
                throw new Exception("The argument for comparison is null.");
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            InterfaceID other = obj as InterfaceID;
            if (other != null)
                return num.CompareTo(other.num);
            else
                throw new Exception("The argument for comparison is either null or is not of type \"" + typeof(InterfaceID).FullName + "\".");
        }

        #endregion

        #region IEquatable<InterfaceID> Members

        bool IEquatable<InterfaceID>.Equals(InterfaceID other)
        {
            return num.Equals(other.num);
        }

        #endregion
    }
}
