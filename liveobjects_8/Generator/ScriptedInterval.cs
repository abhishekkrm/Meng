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
using System.IO;
using System.Linq;
using System.Text;

namespace Generator
{
    /// <summary>
    /// An interval based on input of a file. The file should contain lines with
    /// a timestamp in seconds and a length of message in bytes.
    /// </summary>
    [QS.Fx.Reflection.ComponentClass("5E0DD08BDD3648779E8F4B36F14501AB", "ScriptedInterval",
        "Reads interval and size data from a file")]
    public sealed class ScriptedInterval : IIntervalInterface, IIntervalObject
    {
        #region Fields
        private List<KeyValuePair<double, int>> vals;
        private int index = -1; // index of current values
        private double startTime;
        private QS.Fx.Endpoint.Classes.IExportedInterface<IIntervalInterface> iface;
        private QS.Fx.Object.IContext context;
        #endregion

        public ScriptedInterval(
            QS.Fx.Object.IContext context,
            [QS.Fx.Reflection.Parameter("file", QS.Fx.Reflection.ParameterClass.Value)]
            string filename)
        {
            this.context = context;
            this.vals = new List<KeyValuePair<double, int>>();
            StreamReader file = new StreamReader(filename);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                string[] toks = line.Split();
                double ts = Convert.ToDouble(toks[0]);
                int len = Convert.ToInt32(toks[1]);
                this.vals.Add(new KeyValuePair<double, int>(ts, len));
            }

            this.iface = context.ExportedInterface<IIntervalInterface>(this);
            this.startTime = context.Platform.Clock.Time;
        }

        #region IIntervalInterface Members

        public bool Next()
        {
            return ++this.index != this.vals.Count; // woo! one-liner!
        }

        public double Interval()
        {
            return Math.Max(0, this.vals[index].Key + this.startTime - this.context.Platform.Clock.Time);
        }

        public int Length()
        {
            return this.vals[index].Value;
        }

        #endregion

        #region IIntervalObject Members

        public QS.Fx.Endpoint.Classes.IExportedInterface<IIntervalInterface> Generator
        {
            get { return this.iface; }
        }

        #endregion
    }
}
