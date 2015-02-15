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

using QS.Fx.Value.Classes;
using QS.Fx.Base;
using QS.Fx.Value;

using Quilt.Bootstrap;

namespace Quilt.Multicast
{
    [QS.Fx.Reflection.ValueClass("96B48D7024194693872815C4C45FDB45", "HeartbeatMember")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.HeatbeatMember)]
    public class HeartbeatMember:
        QS.Fx.Serialization.ISerializable
    {
        #region Fields

        public double _heartbeat;
        public BootstrapMember _member;

        #endregion

        #region Constructor

        public HeartbeatMember()
        {
        }

        public HeartbeatMember(BootstrapMember member, double heartbeat) 
        {
            _heartbeat = heartbeat;
            _member = member;
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.HeatbeatMember), sizeof(double));
                info.AddAnother(((QS.Fx.Serialization.ISerializable)_member).SerializableInfo);

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
                    *((double*)(_pheader)) = _heartbeat;
                }

                header.consume(sizeof(double));

                if (this._member != null)
                    ((QS.Fx.Serialization.ISerializable)_member).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("HeartbeatMember SerializeTo exception");
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref ConsumableBlock header, ref ConsumableBlock data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    _heartbeat = *((double*)(_pheader));
                }

                header.consume(sizeof(double));
                this._member = new BootstrapMember();
                ((QS.Fx.Serialization.ISerializable)_member).DeserializeFrom(ref header, ref data);
               
            }
            catch (Exception exc)
            {
                throw new Exception("HeartbeatMember DeserializeFrom exception");
            }
        }

        #endregion
    }
}
