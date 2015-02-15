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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Statistics_
{
    public class Samples2D : QS.Fx.Inspection.Inspectable, QS._core_c_.Statistics.ISamples2D, QS._core_c_.Diagnostics.IDataCollector
	{
        #region Keeping track of instances

        public static Samples2D[] Registered
        {
            get
            {
                Samples2D[] result;
                lock (registered_samples)
                {
                    result = new Samples2D[registered_samples.Count];
                    registered_samples.CopyTo(result, 0);
                }
                return result;
            }
        }

        private static IList<Samples2D> registered_samples = new List<Samples2D>();
        private static void Register(Samples2D samples)
        {
            lock (registered_samples)
            {
                registered_samples.Add(samples);
            }
        }

        #endregion

		public Samples2D() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
		{
		}

        public Samples2D(string name) : this(name, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public Samples2D(string name, string x_name, string x_units, string y_name, string y_units)
            : this(name, string.Empty, x_name, x_units, string.Empty, y_name, y_units, string.Empty)
        {
        }

        public Samples2D(
            string name, string description, string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            this.name = name;
            this.description = description;
            this.x_name = x_name;
            this.x_units = x_units;
            this.x_description = x_description;
            this.y_name = y_name;
            this.y_units = y_units;
            this.y_description = y_description;

            Register(this);
        }

		private System.Collections.Generic.List<QS._core_e_.Data.XY> samples =
			new System.Collections.Generic.List<QS._core_e_.Data.XY>();
        private string name, description, x_name, x_units, x_description, y_name, y_units, y_description;
        private bool enabled = true;

        public QS._core_e_.Data.XY[] Samples
        {
            get { return samples.ToArray(); }
        }

		[QS.Fx.Base.Inspectable]
		public QS._core_e_.Data.IDataSet DataSet
		{
			get 
            { 
                return new QS._core_e_.Data.XYSeries(samples.ToArray());
                // return new QS.TMS.Data.Data2D(
                //    name, samples.ToArray(), description, x_name, x_units, x_description, y_name, y_units, y_description); 
            } 
		}
	
		public void Add(double x, double y)
		{
            if (enabled)
                samples.Add(new QS._core_e_.Data.XY(x, y));
            else
                throw new Exception(((name != null) ? ("Series \"" +  name + "\"") : "This series") + " is not enabled for adding samples.");
		}

        public override string ToString()
        {
            return "SamplesXY(" + name + ", \"" + description + "\", " + samples.Count.ToString() + " values, x = " + x_name + " [" + x_units + "], \"" +
                x_description + "\", y = " + y_name + " [" + y_units + "], \"" + y_description + "\")";
        }

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.DataCollector; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
            samples = new System.Collections.Generic.List<QS._core_e_.Data.XY>();
        }

        #endregion

        #region IDataCollector Members

        QS._core_e_.Data.IDataSet QS._core_c_.Diagnostics.IDataCollector.DataSet
        {
            get { return this.DataSet; }
        }

        #endregion
    }
}
