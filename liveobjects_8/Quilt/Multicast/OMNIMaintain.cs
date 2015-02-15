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

namespace Quilt.Multicast
{
    //[QS.Fx.Reflection.ValueClass("", "OMNIMaintain")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.OmniMaintain)]
    public class OMNIMaintain:ISerializable
    {
        #region Field

        private double _local_time;
        private Name _id;

        #endregion

        #region Constructor

        public OMNIMaintain(double local_time, Name id)
        {
            this._local_time = local_time;
            this._id = id;
        }

        public OMNIMaintain()
        {
        }

        #endregion

        #region Access

        public double LocalTime
        {
            get { return _local_time; }
        }

        public Name Id
        {
            get { return _id; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ISerializable Members

        SerializableInfo ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                   new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.OmniMaintain, sizeof(double));
                info.AddAnother(((ISerializable)_id).SerializableInfo);
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
                    *((double*)(_pheader)) = _local_time;
                }

                header.consume(sizeof(double));

                ((QS.Fx.Serialization.ISerializable)_id).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("OmniMaintain SerializeTo Exception! " + exc.Message);
            }
        }

        unsafe void ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    _local_time = *((double*)(_pheader));
                }

                header.consume(sizeof(double));

                this._id = new Name();
                ((QS.Fx.Serialization.ISerializable)_id).DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("OmniMaintain DeserializeFrom Exception! " + exc.Message);
            }
        }

        #endregion
    }
}
