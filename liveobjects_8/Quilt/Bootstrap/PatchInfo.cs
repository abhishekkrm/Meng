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
using System.Xml.Serialization;

using Quilt.Multicast;
using QS.Fx.Value;
using QS.Fx.Base;

namespace Quilt.Bootstrap
{
    [QS.Fx.Reflection.ValueClass("691AB613CCE94b228D3142637D68FE49", "Patch Info")]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.PatchInfo)]
    [XmlType(TypeName = "EUIDAddress")]
    public sealed class PatchInfo : QS.Fx.Serialization.ISerializable, IEquatable<PatchInfo>
    {
        #region Constructor

        public PatchInfo()
        {
        }

        public PatchInfo(PROTOTYPE _patch_proto, string _patch_description, EUIDAddress _patch_euid)
        {
            this._patch_proto = _patch_proto;
            this._patch_description = new Name(_patch_description);
            this._patch_euid = _patch_euid;
        }

        #endregion

        #region Field

        public Name _patch_description;
        public PROTOTYPE _patch_proto;
        public EUIDAddress _patch_euid;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int length = (_patch_proto != null) ? _patch_proto.ToString().Length : 0;
                if (length > (int)ushort.MaxValue)
                    throw new Exception();
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.PatchInfo, sizeof(ushort), sizeof(ushort) + length, ((length > 0)?1:0));
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_patch_description).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_patch_euid).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            try
            {
                int length = (_patch_proto != null) ? _patch_proto.ToString().Length : 0;
                if (length > (int)ushort.MaxValue)
                    throw new Exception();

                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    *((ushort*)(_pheader)) = (ushort)length;
                }

                header.consume(sizeof(ushort));

                if (this._patch_description != null)
                    ((QS.Fx.Serialization.ISerializable)_patch_description).SerializeTo(ref header, ref data);

                ((QS.Fx.Serialization.ISerializable)_patch_euid).SerializeTo(ref header, ref data);

                if (length > 0)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(_patch_proto.ToString());
                    if (bytes.Length != length)
                        throw new Exception();
                    data.Add(new QS.Fx.Base.Block(bytes));
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
                ushort length;
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    length = *((ushort*)(_pheader));
                }

                header.consume(sizeof(ushort));

                this._patch_description = new Name();
                ((QS.Fx.Serialization.ISerializable)_patch_description).DeserializeFrom(ref header, ref data);
                this._patch_euid = new EUIDAddress();
                ((QS.Fx.Serialization.ISerializable)_patch_euid).DeserializeFrom(ref header, ref data);

                if (length > 0)
                {
                    string proto = Encoding.ASCII.GetString(data.Array, data.Offset, length);
                    data.consume(length);
                    this._patch_proto = ProtoType.StringToType(proto);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("GroupMember SerializeTo exception");
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IEquatable<PatchInfo> Members

        bool IEquatable<PatchInfo>.Equals(PatchInfo other)
        {
            throw new NotImplementedException();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
