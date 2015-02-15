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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_AgentIncarnation)]
    public sealed class AgentIncarnation 
        : QS.Fx.Serialization.ISerializable, IComparable<AgentIncarnation>, IEquatable<AgentIncarnation>, IComparable
    {
        #region Constructor

        public AgentIncarnation(uint[] versions)
        {
            if (versions == null || versions.Length < 1)
                throw new Exception("Bad version sequence.");
            this.versions = versions;
        }

        public AgentIncarnation()
        {
        }

        #endregion

        #region Fields

        private uint[] versions;

        #endregion

        #region IComparable<AgentIncarnation> Members

        int IComparable<AgentIncarnation>.CompareTo(AgentIncarnation other)
        {
            if (other == null)
                throw new ArgumentNullException();

            int ind = 0;
            while (true)
            {
                if (ind < versions.Length)
                {
                    if (ind < other.versions.Length)
                    {
                        int result = versions[ind].CompareTo(other.versions[ind]);
                        if (result != 0)
                            return result;
                    }
                    else
                        throw new Exception("Agent versions are incomparable: one is a prefix of the other.");
                }
                else
                {
                    if (ind < other.versions.Length)
                        throw new Exception("Agent versions are incomparable: one is a prefix of the other.");
                    else
                        return 0;
                }
            }
        }

        #endregion

        #region IEquatable<AgentIncarnation> Members

        bool IEquatable<AgentIncarnation>.Equals(AgentIncarnation other)
        {
            if (other != null && other.versions.Length == versions.Length)
            {
                for (int ind = 0; ind < versions.Length; ind++)
                    if (other.versions[ind] != versions[ind])
                        return false;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<AgentIncarnation>)this).CompareTo((AgentIncarnation) obj);
        }

        #endregion

        #region Overrides from System.Object

        public override bool Equals(object obj)
        {
            return ((IEquatable<AgentIncarnation>) this).Equals(obj as AgentIncarnation);
        }

        public override int GetHashCode()
        {
            int _hashcode = versions.Length;
            for (int ind = 0; ind < versions.Length; ind++)
                _hashcode ^= (int) versions[ind];
            return _hashcode;
        }

        public override string ToString()
        {
            return QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<uint>(versions, ".");
        }

        #endregion

        #region FromString

        public static AgentIncarnation FromString(string s)
        {
            string[] _s = s.Split('.');
            uint[] _versions = new uint[_s.Length];
            for (int ind = 0; ind < _s.Length; ind++)
                _versions[ind] = Convert.ToUInt32(_s[ind]);
            return new AgentIncarnation(_versions);
        }

        #endregion

        #region Accessors

        public uint[] Versions
        {
            get { return versions; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort) QS.ClassID.Fx_Base_AgentIncarnation, sizeof(ushort) + versions.Length * sizeof(uint));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((ushort*)pheader) = (ushort) versions.Length;
                pheader += sizeof(ushort);
                for (int ind = 0; ind < versions.Length; ind++)
                {
                    *((uint*)pheader) = versions[ind];
                    pheader += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + versions.Length * sizeof(uint));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                versions = new uint[(int)(*((ushort*)pheader))];
                pheader += sizeof(ushort);
                for (int ind = 0; ind < versions.Length; ind++)
                {
                    versions[ind]  = *((uint*)pheader);
                    pheader += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + versions.Length * sizeof(uint));
        }

        #endregion
    }
}
