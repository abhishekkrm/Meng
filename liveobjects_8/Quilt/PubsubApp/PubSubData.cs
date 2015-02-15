/*

Copyright (c) 2010 Bo Peng. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the
following conditions
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
using QS.Fx.Value;
using QS.Fx.Base;
using QS.Fx.Serialization;

namespace Quilt.PubsubApp
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.PubSubData)]
    public sealed class PubSubData:
        ISerializable
    {
        #region Field

        public Name _data;
        public int _rate;
        
        #endregion

        #region Constructor

        public PubSubData()
        {
        }

        public PubSubData(Name _data, int _rate)
        {
            this._data = _data;
            this._rate = _rate;
        }

        #endregion


        #region ISerializable Members

        SerializableInfo ISerializable.SerializableInfo
        {
            get
            {               
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.PubSubData, sizeof(int));
                info.AddAnother(((ISerializable)_data).SerializableInfo);
                return info;
            }
        }

        unsafe void ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            try
            {
                fixed (byte* pheader0 = header.Array)
                {
                    byte* pheader = pheader0 + header.Offset;
                    *((int*)(pheader)) = _rate;
                }
                header.consume(sizeof(int));

                ((ISerializable)_data).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("PubSubData SerializeTo excption " + exc.Message);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                fixed (byte* pheader0 = header.Array)
                {
                    byte* pheader = pheader0 + header.Offset;
                    _rate = *((int*)(pheader));
                }
                header.consume(sizeof(int));

                this._data = new Name();
                ((ISerializable)_data).DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("PubSubData DeserializeFrom excption " + exc.Message);
            }
        }

        #endregion
    }
}
