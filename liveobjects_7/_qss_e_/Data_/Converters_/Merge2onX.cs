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

namespace QS._qss_e_.Data_.Converters_
{
    public class Merge2onX : QS._core_e_.Data.IConverter
    {
        public Merge2onX()
        {
        }

        #region IConverter Members

        string QS._core_e_.Data.IConverter.Name
        {
            get { return "Merge 2 on X"; }
        }

        KeyValuePair<string, QS._core_e_.Data.IDataSet>[] QS._core_e_.Data.IConverter.Convert(KeyValuePair<string, QS._core_e_.Data.IDataSet>[] arguments)
        {
            if (arguments.Length != 2 || !(arguments[0].Value is QS._core_e_.Data.XYSeries) || !(arguments[1].Value is QS._core_e_.Data.XYSeries))
                throw new Exception("Bad arguments");

            QS._core_e_.Data.XYSeries series1 = (QS._core_e_.Data.XYSeries)arguments[0].Value;
            QS._core_e_.Data.XYSeries series2 = (QS._core_e_.Data.XYSeries)arguments[1].Value;
            
            series1.sort();
            series2.sort();

            QS._core_e_.Data.XY[] data1 = series1.Data;
            QS._core_e_.Data.XY[] data2 = series2.Data;

            List<QS._core_e_.Data.XY> merged = new List<QS._core_e_.Data.XY>();

            int ind1 = 0, ind2 = 0;

            while (ind1 < data1.Length && ind2 < data2.Length)
            {
                if (data1[ind1].x < data2[ind2].x)
                    ind1++;
                else if (data1[ind1].x > data2[ind2].x)
                    ind2++;
                else
                {
                    int ind1_bound = ind1 + 1;
                    while (ind1_bound < data1.Length && data1[ind1_bound].x == data1[ind1].x)
                        ind1_bound++;

                    int ind2_bound = ind2 + 1;
                    while (ind2_bound < data2.Length && data2[ind2_bound].x == data2[ind2].x)
                        ind2_bound++;

                    for (int i1 = ind1; i1 < ind1_bound; i1++)
                        for (int i2 = ind2; i2 < ind2_bound; i2++)
                            merged.Add(new QS._core_e_.Data.XY(data1[i1].y, data2[i2].y));

                    ind1 = ind1_bound;
                    ind2 = ind2_bound;
                }
            }
            
            return new KeyValuePair<string, QS._core_e_.Data.IDataSet>[] 
            {
                new KeyValuePair<string, QS._core_e_.Data.IDataSet>("Merged(" + arguments[0].Key + ", " + arguments[1].Key + ")", new QS._core_e_.Data.XYSeries(merged.ToArray()))
            };
        }

        #endregion
    }
}
