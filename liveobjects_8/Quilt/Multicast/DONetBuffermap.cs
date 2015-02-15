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

using QS.Fx.Value.Classes;
using QS.Fx.Base;
using QS.Fx.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Quilt.Bootstrap;

namespace Quilt.Multicast
{
    [QS.Fx.Reflection.ValueClass("AD064160F31E4f5c98A0F756D0A92478", "DONetBuffermap")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.DonetBuffermap)]
    public sealed class DONetBuffermap:
        QS.Fx.Serialization.ISerializable
    {
        #region Fields

        //public Name _group_name;
        public List<double> _snapshot;
        public BootstrapMember _self;

        #endregion

        #region Constructor

        public DONetBuffermap()
        {
        }

        public DONetBuffermap(List<double> _snapshot, BootstrapMember _self)
        {
            this._snapshot = _snapshot;
            this._self = _self;
            //this._group_name = _group_name;
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.DonetBuffermap, sizeof(ushort) + (_snapshot == null? 0:_snapshot.Count * sizeof(double)));
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_self).SerializableInfo);
                //info.AddAnother(((QS.Fx.Serialization.ISerializable)_group_name).SerializableInfo);

                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref ConsumableBlock header, ref IList<Block> data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    *((ushort*)(_pheader)) = (ushort)((_snapshot != null) ? _snapshot.Count : 0);
                    
                    for (int i = 0; i < _snapshot.Count; i++)
                    {
                        *((double*)(_pheader + sizeof(ushort) + i * sizeof(double))) = _snapshot[i];
                    }
                }

                header.consume(sizeof(ushort) + _snapshot.Count * sizeof(double));
                ((QS.Fx.Serialization.ISerializable)_self).SerializeTo(ref header, ref data);

                //if (this._group_name != null)
                //    ((QS.Fx.Serialization.ISerializable)_group_name).SerializeTo(ref header, ref data);

            }
            catch (Exception exc)
            {
                throw new Exception("DONetBuffermap SerializeTo Exception " + exc.Message);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref ConsumableBlock header, ref ConsumableBlock data)
        {
            try
            {
                ushort membercount;

                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    membercount = *((ushort*)(_pheader));

                    if (membercount > 0)
                    {
                        _snapshot = new List<double>();
                    }

                    for (int i = 0; i < membercount; i++)
                    {
                        _snapshot.Add( *((double*)(_pheader + sizeof(ushort) + i * sizeof(double))));
                    }
                }

                header.consume(sizeof(ushort) + membercount * sizeof(double));
                _self = new BootstrapMember();
                ((QS.Fx.Serialization.ISerializable)_self).DeserializeFrom(ref header, ref data);

                //this._group_name = new Name();
                //((QS.Fx.Serialization.ISerializable)_group_name).DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("DONetBuffermap Deserialization Exception " + exc.Message);
            }
        }

        #endregion
    }
}
