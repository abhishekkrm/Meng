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
using System.Linq;
using System.Text;

using QS.Fx.Base;
using QS.Fx.Value;

namespace Quilt.Bootstrap
{
    [QS.Fx.Reflection.ValueClass("0E860A3C0E304b4eB769FE6A08BE6706", "BootstrapAlive")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.BootstrapAlive)]
    public sealed class BootstrapAlive
        : QS.Fx.Serialization.ISerializable//, IDisposable
    {
        #region Constructor

        public BootstrapAlive(Name group_name, BootstrapMember self)
        {
            this.group_name = group_name;
            this.self = self;
        }

        public BootstrapAlive()
        {
        }

        #endregion

        #region Fields

        private Name group_name;
        private BootstrapMember self;

        #endregion

        #region Acess

        public Name GroupName
        {
            get { return this.group_name; }
        }

        public BootstrapMember BootstrapMember
        {
            get { return this.self; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo(QS.ClassID.BootstrapAlive);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)group_name).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)self).SerializableInfo);
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
                }

                if (this.group_name != null)
                    ((QS.Fx.Serialization.ISerializable)group_name).SerializeTo(ref header, ref data);
                if (this.self != null)
                    ((QS.Fx.Serialization.ISerializable)self).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("BootstrapAlive SerializeTo Exception!");
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                }

                this.group_name = new Name();
                ((QS.Fx.Serialization.ISerializable)group_name).DeserializeFrom(ref header, ref data);
                this.self = (BootstrapMember)(QS._core_c_.Base3.Serializer.CreateObject((ushort)QS.ClassID.BootstrapMember));
                ((QS.Fx.Serialization.ISerializable)self).DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("BootstrapAlive DeserializeFrom Exception!");
            }
        }

        #endregion

        //#region IDisposable Members

        //void IDisposable.Dispose()
        //{
        //    ((IDisposable)self).Dispose();
        //}

        //#endregion
    }
}
