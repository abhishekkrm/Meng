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

namespace QS._qss_c_.Base3_
{
    [QS.Fx.Serialization.ClassID(ClassID.Now)]
    public class Now : QS.Fx.Serialization.ISerializable
    {
        public Now(QS.Fx.Clock.IClock clock, QS._core_c_.Base3.InstanceID address)
        {
            this.clock = clock;
            this.time = double.NaN;
            this.address = address;
        }

        public Now()
        {
            this.clock = null;
            this.time = double.NaN;
            this.address = null;
        }

        private QS.Fx.Clock.IClock clock;
        private double time;
        private QS._core_c_.Base3.InstanceID address;

        public double Time
        {
            get 
            {
                if (!double.IsNaN(time))
                    return time;
                else if (clock != null)
                    return clock.Time;
                else
                    throw new Exception("Uninitialized.");
            }
        }

        public QS._core_c_.Base3.InstanceID Address
        {
            get { return address; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Now, sizeof(double), sizeof(double), 0);
                info.AddAnother(address.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            if (clock == null)
                throw new Exception("Uninitialized.");

            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((double*)pheader) = clock.Time;
            }
            header.consume(sizeof(double));
            address.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                time = *((int*)pheader);
            }
            header.consume(sizeof(double));
            address.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            return time.ToString() + "@" + address.ToString();
        }
    }
}
