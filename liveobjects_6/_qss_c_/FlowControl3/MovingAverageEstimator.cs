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

namespace QS._qss_c_.FlowControl3
{
    public class MovingAverageEstimator : IEstimatorClass
    {
        public MovingAverageEstimator(int nsamples, double multiplier, double maxincrease,  double minimum, double maximum)
        {
            this.nsamples = nsamples;
            this.multiplier = multiplier;
            this.maxincrease = maxincrease;
            this.minimum = minimum;
            this.maximum = maximum;
        }

        private int nsamples;
        private double multiplier, maxincrease, minimum, maximum;

        private class Estimator : IEstimator
        {
            public Estimator(MovingAverageEstimator owner)
            {
                this.owner = owner;
                size = owner.nsamples;
                if (size < 1)
                    throw new ArgumentException("Size must be >0.");
                window = new double[size];
            }

            private MovingAverageEstimator owner;
            private double[] window;
            private double sum;
            private int offset, count, size;

/*
            public int WindowSize
            {
                get { return size; }
                set
                {
                    if (value < 1)
                        throw new ArgumentException("Size must be >0.");

                    double[] old_window = window;
                    int old_count = count;
                    int old_size = size;
                    int old_offset = offset;

                    size = value;
                    window = new double[size];
                    count = Math.Min(old_count, size);
                    offset = count % size;
                    sum = 0;
                    for (int ind = 0; ind < count; ind++)
                    {
                        double sample = old_window[(old_offset + old_size - old_count + ind) % old_size];
                        sum += sample;
                        window[ind] = sample;
                    }
                }
            }
*/

            #region IEstimator Members

            void IEstimator.AddSample(double sample)
            {
                if (count < size)
                    count++;
                else
                    sum -= window[offset % size];
                window[offset % size] = sample;
                sum += sample;
                offset++;
            }

            double IEstimator.Estimate
            {
                get
                {
                    if (count > 0)
                    {
                        double average = sum / count;
                        double estimate = Math.Min(average * owner.multiplier, average + owner.maxincrease);
                        if (estimate < owner.minimum)
                            estimate = owner.minimum;
                        else if (estimate > owner.maximum)
                            estimate = owner.maximum;
                        return estimate;
                    }
                    else
                        throw new Exception("Cannot provide estimate, no samples to base upon.");
                }
                set
                {
                    window[0] = sum = value;
                    offset = count = 1;
                }
            }

            #endregion
        }

        #region IEstimatorClass Members

        IEstimator IEstimatorClass.Create()
        {
            return new Estimator(this);
        }

        #endregion
    }
}
