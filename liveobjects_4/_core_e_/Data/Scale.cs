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
    public static class Scale
    {
        static Scale()
        {
            scales.Add(Linear);
            scales.Add(Logarithmic);
        }

        public static IEnumerable<IScale> Scales
        {
            get { return scales; }
        }

        private static List<IScale> scales = new List<IScale>();

        public static readonly IScale Linear = new LinearScale();
        public static readonly IScale Logarithmic = new LogarithmicScale();

        #region Class LinearScale

        private class LinearScale : IScale
        {
            public LinearScale()
            {
            }

            #region IScale Members

            string IScale.Name
            {
                get { return "Linear"; }
            }

            double IScale.ValueToCoordinate(Range range, double value)
            {
                return (value - range.Minimum) / (range.Maximum - range.Minimum);
            }

            double IScale.CoordinateToValue(Range range, double coordinate)
            {
                return range.Minimum + coordinate * (range.Maximum - range.Minimum);
            }

            #endregion

            public override string ToString()
            {
                return "Linear";
            }
        }

        #endregion

        #region Class LogarithmicScale

        private class LogarithmicScale : IScale
        {
            public LogarithmicScale()
            {
            }

            #region IScale Members

            string IScale.Name
            {
                get { return "Logarithmic"; }
            }

            double IScale.ValueToCoordinate(Range range, double value)
            {
                return Math.Log(value / range.Minimum, range.Maximum/ range.Minimum);
            }

            double IScale.CoordinateToValue(Range range, double coordinate)
            {
                return range.Minimum * Math.Pow(range.Maximum / range.Minimum, coordinate);
            }

            #endregion

            public override string ToString()
            {
                return "Logarithmic";
            }
        }

        #endregion
    }
}
