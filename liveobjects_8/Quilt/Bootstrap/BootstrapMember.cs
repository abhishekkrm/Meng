/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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

//using live objects namespaces
using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS.Fx.Base;

namespace Quilt.Bootstrap
{
    [QS.Fx.Reflection.ValueClass("E122E564C26E4205ABABAC452BD857D4", "BootstrapMember")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.BootstrapMember)]
    public sealed class BootstrapMember
        :
        IMember<Name, Incarnation, Name, EUIDAddress>,
        QS.Fx.Serialization.ISerializable, IDisposable, ICloneable
    {
        #region Field

        private Incarnation incarnation;
        private Name identifier;
        private Name name;
        private List<EUIDAddress> addresses;

        #endregion

        #region Constructor

        public BootstrapMember()
        {
        }

        public BootstrapMember(IMember<Name, Incarnation, Name, EUIDAddress> member) :
            this(member.Identifier, member.Incarnation, member.Name, member.Addresses)
        {            
        }

        public BootstrapMember(Name identifier, Incarnation incarnation, Name name, IEnumerable<EUIDAddress> addresses)
        {
            this.identifier = identifier;
            this.incarnation = incarnation;
            this.name = name;
            this.addresses = new List<EUIDAddress>();
            foreach (EUIDAddress addr in addresses)
            {
                this.addresses.Add(addr);
            }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.BootstrapMember), sizeof(ushort));
                info.AddAnother(((QS.Fx.Serialization.ISerializable)identifier).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)incarnation).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)name).SerializableInfo);

                if (addresses != null)
                {
                    foreach (EUIDAddress addr in addresses)
                        info.AddAnother(((QS.Fx.Serialization.ISerializable)addr).SerializableInfo);
                }

                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    *((ushort*)(_pheader)) = (ushort)((addresses != null) ? addresses.Count : 0);
                }

                header.consume(sizeof(ushort));

                if (this.identifier != null)
                    ((QS.Fx.Serialization.ISerializable)identifier).SerializeTo(ref header, ref data);
                if (this.incarnation != null)
                    ((QS.Fx.Serialization.ISerializable)incarnation).SerializeTo(ref header, ref data);
                if (this.name != null)
                    ((QS.Fx.Serialization.ISerializable)name).SerializeTo(ref header, ref data);

                if (addresses != null)
                {
                    foreach (EUIDAddress addr in addresses)
                    {
                        ((QS.Fx.Serialization.ISerializable)addr).SerializeTo(ref header, ref data);
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Exception("GroupMember SerializeTo exception");
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                ushort addresscount;

                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    addresscount = *((ushort*)(_pheader));
                }

                header.consume(sizeof(ushort));
                this.identifier = new Name();
                ((QS.Fx.Serialization.ISerializable)identifier).DeserializeFrom(ref header, ref data);
                this.incarnation = new Incarnation();
                ((QS.Fx.Serialization.ISerializable)incarnation).DeserializeFrom(ref header, ref data);
                this.name = new Name();
                ((QS.Fx.Serialization.ISerializable)name).DeserializeFrom(ref header, ref data);

                if (addresscount > 0)
                {
                    this.addresses = new List<EUIDAddress>();
                    while (addresscount > 0)
                    {

                        EUIDAddress addr = (EUIDAddress)(QS._core_c_.Base3.Serializer.CreateObject((ushort)QS.ClassID.EUID));
                        ((QS.Fx.Serialization.ISerializable)addr).DeserializeFrom(ref header, ref data);
                        this.addresses.Add(addr);
                        addresscount--;
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Exception("GroupMember DeserializeFrom exception");
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.addresses.Clear();
        }

        #endregion

        #region IMember<Name,Incarnation,Name,EUIDAddress> Members

        bool IMember<Name, Incarnation, Name, EUIDAddress>.Operational
        {
            get { return true; }
        }

        IEnumerable<EUIDAddress> IMember<Name, Incarnation, Name, EUIDAddress>.Addresses
        {
            get { return this.addresses; }
        }

        #endregion

        #region IMember<Name,Incarnation,Name> Members

        Name IMember<Name, Incarnation, Name>.Identifier
        {
            get { return this.identifier; }
        }

        Incarnation IMember<Name, Incarnation, Name>.Incarnation
        {
            get { return this.incarnation; }
        }

        Name IMember<Name, Incarnation, Name>.Name
        {
            get { return this.name; }
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return new BootstrapMember(this);
        }

        #endregion
    }
}
