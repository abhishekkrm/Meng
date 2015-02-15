/*

Copyright (c) 2010 Matt Pearson. All rights reserved.

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
using System.Linq;
using System.Text;

namespace Generator
{
    /// <summary>
    /// A generator that creates intervals and sizes uniformly from a given range.
    /// </summary>
    [QS.Fx.Reflection.ComponentClass("D326B117A923441BA2F56C6A0EB0CAB6", "UniformInterval",
        "Create interval/size uniformly")]
    public sealed class UniformInterval : IIntervalInterface, IIntervalObject
    {
        #region Fields

        private QS.Fx.Object.IContext context;

        private double intervalMin = 5; // send interval in seconds
        private double intervalMax = 10;
        private int sizeMin = 10;
        private int sizeMax = 50;
        private double timeout = Double.MaxValue;

        private double currInterval;
        private int currSize;

        private Random random = new Random();

        private QS.Fx.Endpoint.Classes.IExportedInterface<IIntervalInterface> iface;

        #endregion

        public UniformInterval(
            QS.Fx.Object.IContext context,
            [QS.Fx.Reflection.Parameter("intervalMin", QS.Fx.Reflection.ParameterClass.Value)]
            double intervalMin,
            [QS.Fx.Reflection.Parameter("intervalMax", QS.Fx.Reflection.ParameterClass.Value)]
            double intervalMax,
            [QS.Fx.Reflection.Parameter("sizeMin", QS.Fx.Reflection.ParameterClass.Value)]
            int sizeMin,
            [QS.Fx.Reflection.Parameter("sizeMax", QS.Fx.Reflection.ParameterClass.Value)]
            int sizeMax,
            [QS.Fx.Reflection.Parameter("duration", QS.Fx.Reflection.ParameterClass.Value)]
            double duration)
        {
            this.context = context;

            // init paramters (I'm assuming that if they're not specified they are 0.
            // makes sense to me, but not much about this framework does...)
            if (intervalMin != 0)
                this.intervalMin = intervalMin;
            if (intervalMax != 0)
                this.intervalMax = intervalMax;
            if (sizeMin != 0)
                this.sizeMin = sizeMin;
            if (sizeMax != 0)
                this.sizeMax = sizeMax;

            if (this.intervalMin > this.intervalMax)
                throw new Exception("invalid interval");
            if (this.sizeMin > this.sizeMax)
                throw new Exception("invalid range of message sizes");

            if (duration != 0)
                this.timeout = context.Platform.Clock.Time + duration;

            this.iface = context.ExportedInterface<IIntervalInterface>(this);
        }

        #region IIntervalGenerator Members

        public bool Next()
        {
            if (this.timeout < this.context.Platform.Clock.Time)
                return false; // we're done!

            this.currInterval = this.intervalMin + random.NextDouble() * (this.intervalMax - this.intervalMin);
            this.currSize = random.Next(this.sizeMin, this.sizeMax);
            return true; // keep going, not time to stop yet
        }

        public double Interval()
        {
            return this.currInterval;
        }

        public int Length()
        {
            return this.currSize;
        }

        #endregion

        #region IIntervalObject Members

        public QS.Fx.Endpoint.Classes.IExportedInterface<IIntervalInterface> Generator
        {
            get { return iface; }
        }

        #endregion
    }
}
