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
    [QS.Fx.Printing.Printable("Member", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Member_<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : IMember_<IdentifierClass, IncarnationClass, AddressClass>, QS.Fx.Serialization.ISerializable
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, new()
        where IncarnationClass : class, QS.Fx.Serialization.ISerializable, new()
        where AddressClass : class, QS.Fx.Serialization.ISerializable, new()
    {
        #region Constructor

        public Member_(IdentifierClass _identifier, IncarnationClass _incarnation, AddressClass _address)
        {
            this._identifier = _identifier;
            this._incarnation = _incarnation;
            this._address = _address;
        }

        public Member_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("identifier")]
        private IdentifierClass _identifier;
        [QS.Fx.Printing.Printable("incarnation")]
        private IncarnationClass _incarnation;
        [QS.Fx.Printing.Printable("address")]
        private AddressClass _address;

        #endregion

        #region IMember_<IdentifierClass,IncarnationClass,AddressClass> Members

        IdentifierClass IMember_<IdentifierClass, IncarnationClass, AddressClass>._Identifier
        {
            get { return this._identifier; }
        }

        IncarnationClass IMember_<IdentifierClass, IncarnationClass, AddressClass>._Incarnation
        {
            get { return this._incarnation; }
        }

        AddressClass IMember_<IdentifierClass, IncarnationClass, AddressClass>._Address
        {
            get { return this._address; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Nothing, 0);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable) this._identifier).SerializableInfo);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable) this._incarnation).SerializableInfo);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable) this._address).SerializableInfo);
                return _info; 
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) this._identifier).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) this._incarnation).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) this._address).SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this._identifier = new IdentifierClass();
            this._incarnation = new IncarnationClass();
            this._address = new AddressClass();
            ((QS.Fx.Serialization.ISerializable) this._identifier).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) this._incarnation).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) this._address).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
*/ 
}
