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

namespace QS._qss_x_.Properties_
{
/*
    [QS.Fx.Printing.Printable("Membership", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class MembershipView_<
        [QS.Fx.Reflection.Parameter("VersionClass", QS.Fx.Reflection.ParameterClass.ValueClass)] VersionClass,
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : IMembershipView_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>, QS.Fx.Serialization.ISerializable
        where VersionClass : class, QS.Fx.Serialization.ISerializable, new()
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, new()
        where IncarnationClass : class, QS.Fx.Serialization.ISerializable, new()
        where AddressClass : class, QS.Fx.Serialization.ISerializable, new()
    {
        #region Constructor

        public MembershipView_(VersionClass _version, IMember_<IdentifierClass, IncarnationClass, AddressClass>[] _members)
        {
            this._version = _version;
            this._members = (IMember_<IdentifierClass, IncarnationClass, AddressClass>[]) _members.Clone();
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("version")]
        private VersionClass _version;
        [QS.Fx.Printing.Printable("members")]
        private IMember_<IdentifierClass, IncarnationClass, AddressClass>[] _members;

        #endregion

        #region IMembershipView_<VersionClass,IdentifierClass,IncarnationClass,AddressClass> Members

        VersionClass IMembershipView_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>._Version
        {
            get { return this._version; }
        }

        int IMembershipView_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>._Count
        {
            get { return this._members.Length; }
        }

        IMember_<IdentifierClass, IncarnationClass, AddressClass> 
            IMembershipView_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>._Member(int _index)
        {
            return this._members[_index];
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Nothing, sizeof(uint));
                _info.AddAnother(((QS.Fx.Serialization.ISerializable) this._version).SerializableInfo);
                foreach (IMember_<IdentifierClass, IncarnationClass, AddressClass> _member in this._members)
                    _info.AddAnother(((QS.Fx.Serialization.ISerializable) _member).SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) this._version).SerializeTo(ref header, ref data);
            fixed (byte* pheader = header.Array)
            {
                *((uint*)(pheader + header.Offset)) = (uint) this._members.Length;
            }
            header.consume(sizeof(uint));
            foreach (IMember_<IdentifierClass, IncarnationClass, AddressClass> _member in this._members)
                ((QS.Fx.Serialization.ISerializable) _member).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this._version = new VersionClass();
            ((QS.Fx.Serialization.ISerializable) this._version).DeserializeFrom(ref header, ref data);
            int _count;
            fixed (byte* pheader = header.Array)
            {
                _count = (int)(*((uint*)(pheader + header.Offset)));
            }
            header.consume(sizeof(uint));
            this._members = new IMember_<IdentifierClass, IncarnationClass, AddressClass>[_count];
            for (int _index = 0; _index < _count; _index++)
            {
                Member_<IdentifierClass, IncarnationClass, AddressClass> _member = new Member_<IdentifierClass, IncarnationClass, AddressClass>();
                ((QS.Fx.Serialization.ISerializable) _member).DeserializeFrom(ref header, ref data);
                this._members[_index] = _member;                
            }
        }

        #endregion
    }
*/ 
}
