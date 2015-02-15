/*

Copyright (c) 2004-2009 Jared Cantwell, Krzysztof Ostrowski. All rights reserved.

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
using System.Xml.Serialization;

namespace QS.Fx.Base
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_midentifier)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.MessageIdentifier)]
    [XmlType(TypeName = "MessageIdentifier")]
    public sealed class MessageIdentifier : QS.Fx.Serialization.ISerializable, IComparable<MessageIdentifier>, IComparable, IEquatable<MessageIdentifier>
    {
        #region Constructor

        public MessageIdentifier(QS.Fx.Base.Identifier id, QS.Fx.Base.Incarnation incarnation, QS.Fx.Base.Index index)
        {
            this.id = id;
            this.incarnation = incarnation;
            this.index = index;
        }

        public MessageIdentifier()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Base.Identifier id;
        private QS.Fx.Base.Incarnation incarnation;
        private QS.Fx.Base.Index index;

        #endregion

        #region Accessors

        public QS.Fx.Base.Identifier Id
        {
            get { return id; }
        }

        public QS.Fx.Base.Incarnation Incarnation
        {
            get { return incarnation; }
        }

        public QS.Fx.Base.Index Index
        {
            get { return Index; }
        }

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return "(" + this.id.String + "," + this.incarnation.String + "," + this.index.String + ")";
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageIdentifier)
            {
                MessageIdentifier other = (MessageIdentifier)obj;
                return id.Equals(other.id) && incarnation.Equals(other.incarnation) && index.Equals(other.index);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() + incarnation.GetHashCode() + index.GetHashCode();
        }

        #endregion

        #region IComparable<MessageIdentifier> Members

        public int CompareTo(MessageIdentifier other)
        {
            int result1 = id.CompareTo(other.id);
            int result2 = incarnation.CompareTo(other.incarnation);
            int result3 = index.CompareTo(other.index);

            return (result1 != 0) ? result1 : (result2 != 0) ? result2 : result3;
        }

        #endregion

        #region IEquatable<MessageIdentifier> Members

        public bool Equals(MessageIdentifier other)
        {
            return id.Equals(other.id) && incarnation.Equals(other.incarnation) && index.Equals(other.index);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj is MessageIdentifier)
            {
                MessageIdentifier other = (MessageIdentifier)obj;
                int result1 = id.CompareTo(other.id);
                int result2 = incarnation.CompareTo(other.incarnation);
                int result3 = index.CompareTo(other.index);

                return (result1 != 0) ? result1 : (result2 != 0) ? result2 : result3;
            }
            else
                throw new Exception("Cannot compare to something that is not a MessageIdentifier");
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.MessageIdentifier, 0); 
                _info.AddAnother(this.id.SerializableInfo);
                _info.AddAnother(this.incarnation.SerializableInfo);
                _info.AddAnother(this.index.SerializableInfo);
                return _info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            this.id.SerializeTo(ref header, ref data);
            this.incarnation.SerializeTo(ref header, ref data);
            this.index.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this.id = new QS.Fx.Base.Identifier();
            this.id.DeserializeFrom(ref header, ref data);
            this.incarnation = new QS.Fx.Base.Incarnation();
            this.incarnation.DeserializeFrom(ref header, ref data);
            this.index = new QS.Fx.Base.Index();
            this.index.DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
