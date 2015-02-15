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

namespace QS._qss_x_.Base1_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_QualifiedID)]
    public sealed class QualifiedID : QS.Fx.Serialization.ISerializable, IComparable<QualifiedID>, IComparable, IEquatable<QualifiedID>
    {
        #region Constructors

        public static QualifiedID Undefined
        {
            get { return _undefined; }
        }

        private static readonly QualifiedID _undefined = new QualifiedID(QS.Fx.Base.ID.Undefined, QS.Fx.Base.ID.Undefined);

        public QualifiedID(QS.Fx.Base.ID context, QS.Fx.Base.ID obj)
        {
            if (context == null || obj == null)
                throw new Exception("Neither of the components of the qualified name can be null.");
            this.context = context;
            this.obj = obj;
        }

        public QualifiedID()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Base.ID context, obj;

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Base_QualifiedID, 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) context).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) obj).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) context).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) obj).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            context = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) context).DeserializeFrom(ref header, ref data);
            obj = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable) obj).DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return context.ToString() + ":" + obj.ToString();
        }

        public override int GetHashCode()
        {
            return context.GetHashCode() ^ obj.GetHashCode();
        }

        public override bool Equals(object _obj)
        {
            QualifiedID other = _obj as QualifiedID;
            return (other != null) && this.context.Equals(other.context) && this.obj.Equals(other.obj);
        }

        #endregion

        #region IComparable<QualifiedName> Members

        int IComparable<QualifiedID>.CompareTo(QualifiedID other)
        {
            if (other != null)
            {
                int result = ((IComparable<QS.Fx.Base.ID>) context).CompareTo(other.context);
                if (result == 0)
                    result = ((IComparable<QS.Fx.Base.ID>) obj).CompareTo(other.obj);
                return result;
            }
            else
                throw new Exception("The argument for comparison is null.");
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            QualifiedID other = obj as QualifiedID;
            if (other != null)
                return ((IComparable<QualifiedID>) this).CompareTo(other);
            else
                throw new Exception("The argument for comparison is either null or is not a" + GetType().FullName + ".");
        }

        #endregion

        #region IEquatable<Name> Members

        bool IEquatable<QualifiedID>.Equals(QualifiedID other)
        {
            return context.Equals(other.context) && obj.Equals(other.obj);
        }

        #endregion

        #region Accessors

        public QS.Fx.Base.ID Context
        {
            get { return context; }
            set
            {
                if (value != null)
                    context = value;
                else
                    throw new Exception("Name cannot be null.");
            }
        }

        public QS.Fx.Base.ID Object
        {
            get { return obj; }
            set
            {
                if (value != null)
                    obj = value;
                else
                    throw new Exception("Name cannot be null.");
            }
        }

        #endregion

        #region FromString

        public static QualifiedID FromString(string s)
        {
            int ind = s.IndexOf(':');
            QS.Fx.Base.ID n1 = QS.Fx.Base.ID.FromString(s.Substring(0, ind));
            QS.Fx.Base.ID n2 = QS.Fx.Base.ID.FromString(s.Substring(ind + 1));
            return new QualifiedID(n1, n2);
        }

        #endregion
    }
}
