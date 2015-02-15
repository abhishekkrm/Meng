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

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass("04CB769F2F834e40B481D6E08611EA85", "BootstrapLeave")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.BootstrapLeave)]
    public sealed class BootstrapLeave
        : QS.Fx.Serialization.ISerializable
    {

        #region Constructor

        public BootstrapLeave(Name _group_name, Name _member_id)
        {
            this._group_name = _group_name;
            this._member_id = _member_id;
        }

        public BootstrapLeave()
        {
        }

        #endregion

        #region Fields

        private Name _group_name;
        private Name _member_id;

        #endregion

        #region Acess

        public Name GroupID
        {
            get { return this._group_name; }
        }

        public Name MemberID
        {
            get { return this._member_id; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo(QS.ClassID.BootstrapLeave);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_group_name).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_member_id).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            try
            {
                if (this._group_name != null)
                    ((QS.Fx.Serialization.ISerializable)_group_name).SerializeTo(ref header, ref data);
                if (this._member_id != null)
                    ((QS.Fx.Serialization.ISerializable)_member_id).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("BootstrapLeave SerializeTo Exception! " + exc.Message);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                this._group_name = new Name();
                ((QS.Fx.Serialization.ISerializable)_group_name).DeserializeFrom(ref header, ref data);
                this._member_id = new Name();
                ((QS.Fx.Serialization.ISerializable)_member_id).DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("BootstrapLeave DeserializeFrom Exception! " + exc.Message);
            }
        }

        #endregion
    }
}
