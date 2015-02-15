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

namespace QS._qss_e_.Data_
{
    public static class Convert
    {
        public static QS._core_e_.Data.IDataSet ToDataSeries(object data)
        {
            if (data is QS._core_e_.Data.IDataSet)
                return ((QS._core_e_.Data.IDataSet)data);
            else if (data is QS._core_e_.Data.Data1D)
                return new QS._core_e_.Data.DataSeries(((QS._core_e_.Data.Data1D)data).Data);
            else if (data is QS._core_e_.Data.Data2D)
                return new QS._core_e_.Data.XYSeries(((QS._core_e_.Data.Data2D)data).Data);
            else if (data is QS._core_e_.Data.DataCo)
            {
                MultiSeries series = new MultiSeries();
                foreach (QS._core_e_.Data.IData component in ((QS._core_e_.Data.DataCo)data).Components)
                    series.Series.Add(component.Name, (QS._core_e_.Data.DataSeries) ToDataSeries(component));
                return series;
            }
            else
                return null;
        }

        public static QS._core_e_.Data.IData ToData(object data)
        {
            return ToData(null, data);
        }

        public static QS._core_e_.Data.IData ToData(string name, object data)
        {
            if (data is QS._core_e_.Data.IData)
                return ((QS._core_e_.Data.IData)data);
            else if (data is QS._core_e_.Data.DataSeries)
                return new QS._core_e_.Data.Data1D(name, (QS._core_e_.Data.DataSeries)data);
            else if (data is QS._core_e_.Data.XYSeries)
                return new QS._core_e_.Data.Data2D(name, ((QS._core_e_.Data.XYSeries)data).Data);
            else if (data is MultiSeries)
            {
                QS._core_e_.Data.DataCo dataCo = new QS._core_e_.Data.DataCo(name, "");
                foreach (KeyValuePair<string, QS._core_e_.Data.DataSeries> series in ((MultiSeries)data).Series)
                    dataCo.Add(new QS._core_e_.Data.Data1D(series.Key, series.Value));
                return dataCo;
            }
            else
                return null;
        }
    }
}
