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

using QS.Fx.Serialization;
using QS.Fx.Base;

using Quilt.Bootstrap;

namespace Quilt.Multicast
{
    //[QS.Fx.Reflection.ValueClass("", "OmniAck")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.OmniAck)]
    public class OMNIJoinAck : ISerializable
    {
        #region Field

        private BootstrapMember _parent;

        #endregion

        #region Access

        public BootstrapMember Parent
        {
            get { return _parent; }
        }

        #endregion

        #region Constructor

        public OMNIJoinAck(BootstrapMember parent)
        {
            this._parent = parent;
        }

        public OMNIJoinAck()
        {
        }

        #endregion

        #region ISerializable Members

        SerializableInfo ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo(QS.ClassID.OmniAck);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_parent).SerializableInfo);
                return info;
            }
        }

        unsafe void ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                }

                if (this._parent != null)
                    ((QS.Fx.Serialization.ISerializable)_parent).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("OMNIJoinAck SerializeTo Exception!");
            }
        }

        unsafe void ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                }

                this._parent = new BootstrapMember();
                ((QS.Fx.Serialization.ISerializable)_parent).DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("OMNIJoinAck DeserializeFrom Exception!");
            }
        }

        #endregion
    }
}
