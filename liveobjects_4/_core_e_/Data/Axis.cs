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

namespace QS._core_e_.Data
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_Axis)]
    public class Axis : IAxis, QS.Fx.Serialization.ISerializable
    {
        public Axis()
        {
        }

        public Axis(string name, string units, Range range, string description)
        {
            this.name = name;
            this.units = units;
            this.range = range;
            this.description = description;
        }

        private string name, units, description;
        private Range range;

        #region IAxis Members

        string IAxis.Name
        {
            get { return name; }
            set { name = value; }
        }

        string IAxis.Units
        {
            get { return units; }
            set { units = value; }
        }

        string IAxis.Description
        {
            get { return description; }
            set { description = value; }
        }

        Range IAxis.Range
        {
            get { return range; }
            set { range = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = 2 * sizeof(double);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_Axis, size, size, 0);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(name)).SerializableInfo);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(units)).SerializableInfo);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(description)).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            (new QS._core_c_.Base2.StringWrapper(name)).SerializeTo(ref header, ref data);
            (new QS._core_c_.Base2.StringWrapper(units)).SerializeTo(ref header, ref data);
            (new QS._core_c_.Base2.StringWrapper(description)).SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((double*)pheader) = range.Minimum;
                *((double*)(pheader + sizeof(double))) = range.Maximum;
            }
            header.consume(2 * sizeof(double));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            QS._core_c_.Base2.StringWrapper wrapper1 = new QS._core_c_.Base2.StringWrapper();
            wrapper1.DeserializeFrom(ref header, ref data);
            name = wrapper1.String;
            QS._core_c_.Base2.StringWrapper wrapper2 = new QS._core_c_.Base2.StringWrapper();
            wrapper2.DeserializeFrom(ref header, ref data);
            units = wrapper2.String;
            QS._core_c_.Base2.StringWrapper wrapper3 = new QS._core_c_.Base2.StringWrapper();
            wrapper3.DeserializeFrom(ref header, ref data);
            description = wrapper3.String;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                range = new Range(*((double*)pheader), *((double*)(pheader + sizeof(double))));
            }
            header.consume(2 * sizeof(double));
        }

        #endregion
    }
}
