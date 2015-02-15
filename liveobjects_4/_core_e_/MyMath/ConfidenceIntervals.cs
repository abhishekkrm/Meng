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

namespace QS._core_e_.MyMath
{
    public static class ConfidenceIntervals
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Data to analyze.</param>
        /// <param name="mean">Average.</param>
        /// <param name="error">Value such that 95% confidence intrervals are mean +/- error</param>
        public static void Calculate95(double[] data, out double mean, out double error)
        {
            if (data.Length < 1)
                throw new Exception("No data!");
            else if (data.Length < 2)
            {
                mean = data[0];
                error = double.PositiveInfinity;
            }
            else
            {
                double sum = 0, sum2 = 0;
                int n = data.Length;
                for (int ind = 0; ind < n; ind++)
                {
                    sum += data[ind];
                    sum2 += data[ind] * data[ind];
                }

                mean = sum / ((double) n);
                error = 1.96 * Math.Sqrt((sum2 - sum * mean) / (((double) n) * ((double) (n - 1))));
            }
        }

        public static void Calculate95XY(Data.XY[] data, out double meanx, out double errorx, out double meany, out double errory)
        {
            if (data.Length < 1)
                throw new Exception("No data!");
            else if (data.Length < 2)
            {
                meanx = data[0].x;
                meany = data[0].y;
                errorx = errory = double.PositiveInfinity;
            }
            else
            {
                double sumx = 0, sum2x = 0, sumy = 0, sum2y = 0;
                int n = data.Length;
                for (int ind = 0; ind < n; ind++)
                {
                    sumx += data[ind].x;
                    sumy += data[ind].y;
                    sum2x += data[ind].x * data[ind].x;
                    sum2y += data[ind].y * data[ind].y;
                }

                meanx = sumx / ((double)n);
                meany = sumy / ((double)n);
                errorx = 1.96 * Math.Sqrt((sum2x - sumx * meanx) / (((double)n) * ((double)(n - 1))));
                errory = 1.96 * Math.Sqrt((sum2y - sumy * meany) / (((double)n) * ((double)(n - 1))));
            }
        }
    }
}
