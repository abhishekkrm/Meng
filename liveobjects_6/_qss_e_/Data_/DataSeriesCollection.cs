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
    public class DataSeriesCollection : DataSetCollection<QS._core_e_.Data.DataSeries>
    {
        public DataSeriesCollection(string name) : base(name)
        {
        }

        protected override void draw(System.Drawing.Graphics graphics)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [System.Xml.Serialization.XmlIgnore]
        [QS._core_e_.Data.DataSource("Raw Data")]
        public QS._core_e_.Data.IDataSet Interleaved
        {
            get
            {
                int size = 0;
                foreach (QS._core_e_.Data.DataSeries dataSeries in dataSetCollection.Values)
                {
                    if (size > 0)
                    {
                        if (dataSeries.Data.Length != size)
                            throw new Exception("All data series in the collection must be of the same size.");
                    }
                    else
                        size = dataSeries.Data.Length;
                }

                double[] series = new double[size * dataSetCollection.Count];

                int collectionNo = 0;
                foreach (QS._core_e_.Data.DataSeries dataSeries in dataSetCollection.Values)
                {
                    for (int ind = 0; ind < size; ind++)
                        series[ind * dataSetCollection.Count + collectionNo] = dataSeries.Data[ind];
                    collectionNo++;
                }

                return new QS._core_e_.Data.DataSeries(series);
            }
        }
    }
}
