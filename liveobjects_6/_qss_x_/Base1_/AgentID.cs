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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_AgentID)]    
    public sealed class AgentID : QS.Fx.Serialization.ISerializable, IComparable<AgentID>, IEquatable<AgentID>, IComparable
    {
        #region Constructor

        public AgentID(QS._qss_x_.Base1_.QualifiedID subdomain, QS._qss_x_.Base1_.QualifiedID superdomain)
        {
            this.subdomain = subdomain;
            this.superdomain = superdomain;
        }

        public AgentID()
        {
        }

        #endregion

        #region Fields

        private QS._qss_x_.Base1_.QualifiedID subdomain, superdomain;

        #endregion

        #region IComparable<AgentID> Members

        int IComparable<AgentID>.CompareTo(AgentID other)
        {
            if (other == null)
                throw new ArgumentNullException();
            int result = ((IComparable<QualifiedID>) subdomain).CompareTo(other.subdomain);
            if (result == 0)
                result = ((IComparable<QualifiedID>) superdomain).CompareTo(other.superdomain);
            return result;
        }

        #endregion

        #region IEquatable<AgentID> Members

        bool IEquatable<AgentID>.Equals(AgentID other)
        {            
            return (other != null) && ((IEquatable<QualifiedID>) subdomain).Equals(other.subdomain)
                && ((IEquatable<QualifiedID>) superdomain).Equals(other.superdomain);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<AgentID>) this).CompareTo((AgentID) obj);
        }

        #endregion

        #region Overrides from System.Object

        public override bool Equals(object obj)
        {
            return ((IEquatable<AgentID>) this).Equals(obj as AgentID);
        }

        public override int GetHashCode()
        {
            return subdomain.GetHashCode() ^ superdomain.GetHashCode();
        }

        public override string ToString()
        {
            return subdomain.ToString() + "@" + superdomain.ToString();
        }

        #endregion

        #region FromString

        public static AgentID FromString(string s)
        {
            int ind = s.IndexOf('@');
            QualifiedID n1 = QualifiedID.FromString(s.Substring(0, ind));
            QualifiedID n2 = QualifiedID.FromString(s.Substring(ind + 1));
            return new AgentID(n1, n2);
        }

        #endregion

        #region Accessors

        public QualifiedID Subdomain
        {
            get { return subdomain; }
            set
            {
                if (value != null)
                    subdomain = value;
                else
                    throw new Exception("Name cannot be null.");
            }
        }

        public QualifiedID Superdomain
        {
            get { return superdomain; }
            set
            {
                if (value != null)
                    superdomain = value;
                else
                    throw new Exception("Name cannot be null.");
            }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Base_AgentID, 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) subdomain).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) superdomain).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) subdomain).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) superdomain).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            subdomain = new QualifiedID();
            ((QS.Fx.Serialization.ISerializable) subdomain).DeserializeFrom(ref header, ref data);
            superdomain = new QualifiedID();
            ((QS.Fx.Serialization.ISerializable) superdomain).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
