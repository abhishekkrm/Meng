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
    public static class RecoveryLatency
    {
        public static void Calculate(QS._core_e_.Data.DataSeries receiveTimes, out QS._core_e_.Data.XYSeries recoveryLatencies)
        {
            double[] data = receiveTimes.Data;
            double level = double.MaxValue;
            Stack<QS._core_e_.Data.XY> latencies = new Stack<QS._core_e_.Data.XY>();
            int upper = data.Length - 1;

            while (upper >= 0)
            {
                while (upper >= 0 && data[upper] <= level)
                {
                    level = data[upper];
                    upper--;
                }

                while (upper >= 0 && data[upper] > level)
                {
                    latencies.Push(new QS._core_e_.Data.XY(upper, data[upper] - level));
                    upper--;
                }
            }

            QS._core_e_.Data.XY[] recovery_data = new QS._core_e_.Data.XY[latencies.Count];
            for (int ind = 0; ind < recovery_data.Length; ind++)
                recovery_data[ind] = latencies.Pop();

            recoveryLatencies = new QS._core_e_.Data.XYSeries(recovery_data);
        }

        public static void Calculate(QS._core_e_.Data.DataSeries receiveTimes,
            out QS._core_e_.Data.XYSeries multicastReceiveTimes, out QS._core_e_.Data.XYSeries retransmitReceiveTimes, 
            out QS._core_e_.Data.XYSeries recoveryLatencies, out QS._core_e_.Data.DataCo splitTimes)
        {
            double[] data = receiveTimes.Data;
            Stack<QS._core_e_.Data.XY> multicast = new Stack<QS._core_e_.Data.XY>(), retransmits = new Stack<QS._core_e_.Data.XY>();
            Stack<QS._core_e_.Data.XY> latencies = new Stack<QS._core_e_.Data.XY>();
            double level = double.MaxValue;
            int upper = data.Length - 1;

            while (upper >= 0)
            {
                while (upper >= 0 && data[upper] <= level)
                {
                    multicast.Push(new QS._core_e_.Data.XY(upper, level = data[upper]));
                    upper--;
                }

                while (upper >= 0 && data[upper] > level)
                {
                    retransmits.Push(new QS._core_e_.Data.XY(upper, data[upper]));
                    latencies.Push(new QS._core_e_.Data.XY(upper, data[upper] - level));
                    upper--;
                }
            }

            QS._core_e_.Data.XY[] multicast_data = new QS._core_e_.Data.XY[multicast.Count];
            for (int ind = 0; ind < multicast_data.Length; ind++)
                multicast_data[ind] = multicast.Pop();

            QS._core_e_.Data.XY[] retransmits_data = new QS._core_e_.Data.XY[retransmits.Count];
            for (int ind = 0; ind < retransmits_data.Length; ind++)
                retransmits_data[ind] = retransmits.Pop();

            QS._core_e_.Data.XY[] recovery_data = new QS._core_e_.Data.XY[latencies.Count];
            for (int ind = 0; ind < recovery_data.Length; ind++)
                recovery_data[ind] = latencies.Pop();

            multicastReceiveTimes = new QS._core_e_.Data.XYSeries(multicast_data);
            retransmitReceiveTimes = new QS._core_e_.Data.XYSeries(retransmits_data);
            recoveryLatencies = new QS._core_e_.Data.XYSeries(recovery_data);

            splitTimes = new QS._core_e_.Data.DataCo();
            splitTimes.Add(new QS._core_e_.Data.Data2D("multicast", multicast_data));
            splitTimes.Add(new QS._core_e_.Data.Data2D("retransmits", retransmits_data));
        }
    }
}
