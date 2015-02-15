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

namespace QS._qss_e_.Postprocessing_
{
    public static class Losses
    {
        public static void Calculate(QS._core_e_.Data.DataSeries receiveTimes, 
            out QS._core_e_.Data.XYSeries indicator, out QS._core_e_.Data.DataSeries losses)
        {
            double[] data = receiveTimes.Data;
            Stack<QS._core_e_.Data.XY> indicator_stack = new Stack<QS._core_e_.Data.XY>();
            Stack<double> loss_stack = new Stack<double>();

            double level = double.MaxValue;
            int upper = data.Length - 1;
            double estimated_interval = 0;

            while (upper >= 0)
            {
                while (upper >= 0 && data[upper] <= level)
                {
                    estimated_interval = (estimated_interval + (level - data[upper])) / 2;
                    indicator_stack.Push(new QS._core_e_.Data.XY(level = data[upper], 1));
                    upper--;
                }

                int num_losses = 0;
                while (upper >= 0 && data[upper] > level)
                {
                    num_losses++;
                    // indicator_stack.Push(new QS.TMS.Data.XY(double.NaN, 0));
                    upper--;
                }

                if (upper >= 0)
                {
                    double uniform_interval = (level - data[upper]) / (num_losses + 1);
                    for (int ind = 0; ind < num_losses; ind++)
                    {
                        double estimated_loss = level - (ind + 1) * uniform_interval;
                        indicator_stack.Push(new QS._core_e_.Data.XY(estimated_loss, 0));
                        loss_stack.Push(estimated_loss);
                    }
                }
                else
                {
                    for (int ind = 0; ind < num_losses; ind++)
                    {
                        double estimated_loss = level - (ind + 1) * estimated_interval;
                        indicator_stack.Push(new QS._core_e_.Data.XY(estimated_loss, 0));
                        loss_stack.Push(estimated_loss);
                    }
                }
            }

            QS._core_e_.Data.XY[] indicator_data = new QS._core_e_.Data.XY[indicator_stack.Count];
            for (int ind = 0; ind < indicator_data.Length; ind++)
                indicator_data[ind] = indicator_stack.Pop();

            double[] loss_data = new double[loss_stack.Count];
            for (int ind = 0; ind < loss_data.Length; ind++)
                loss_data[ind] = loss_stack.Pop();

/*
            double last_time = 0, estimated_interval = 0;
            bool last_good = false;
            for (int ind = 0; ind < indicator_data.Length; ind++)
            {
                QS.TMS.Data.XY datapoint = indicator_stack.Pop();
                if (datapoint.y == 1)
                {
                    if (last_good)
                        estimated_interval = (estimated_interval + (datapoint.x - last_time)) / 2;
                    indicator_data[ind] = datapoint;
                    last_good = true;
                    last_time = datapoint.x;
                }
                else
                {
                    last_time = datapoint.x = last_time + estimated_interval;
                    last_good = false;
                }
            }
*/

            indicator = new QS._core_e_.Data.XYSeries(indicator_data);
            losses = new QS._core_e_.Data.DataSeries(loss_data);
        }
    }
}
