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

using System.Drawing;

namespace QS._core_e_.Data
{
    [QS._core_c_.Serialization.BLOB]
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_DataSeries)]
    [System.Serializable]
    public class DataSeries : IDataSet, QS.Fx.Serialization.ISerializable, QS._core_c_.Diagnostics.IDataCollector
    {
        public static explicit operator Data1D(DataSeries dataSeries)
        {
            return new Data1D(dataSeries);
        }

        public static explicit operator DataSeries(Data1D data)
        {
            return new DataSeries(data.Data);
        }

		public Rectangle Range
		{
			get { return new Rectangle(new Point(0, CurrentStatistics.minimumValue), new Point(dataSeries.Length - 1, CurrentStatistics.maximumValue)); }
		}

		public DataSeries()
        {
        }

        public DataSeries(int size)
        {
            this.dataSeries = new double[size];
        }

        public DataSeries(double[] dataSeries)
        {
            this.dataSeries = (dataSeries != null) ? dataSeries : new double[0];
        }

        private double[] dataSeries = new double[0];

        public double[] Data
        {
            get { return dataSeries; }
            set { dataSeries = (value != null) ? value : new double[0]; }
        }

        public double this[int ind]
        {
            get { return dataSeries[ind]; }
            set { dataSeries[ind] = value; }
        }

        #region Class Statistics

        [System.Serializable]
        public class Statistics 
        {
            public Statistics(DataSeries dataSeries)
            {
                numberOfValues = dataSeries.dataSeries.Length;
                maximumValue = double.MinValue;
                minimumValue = double.MaxValue;
                
                averageValue = 0;

                for (int ind = 0; ind < dataSeries.dataSeries.Length; ind++)
                {
                    double v = dataSeries.dataSeries[ind];
                    if (v < minimumValue)
                        minimumValue = v;
                    if (v > maximumValue)
                        maximumValue = v;
                    averageValue += v;
                }

                averageValue = averageValue / ((double) numberOfValues);
                stretchOfValues = maximumValue - minimumValue;
                stretchPerValue = stretchOfValues / ((double) numberOfValues);

                if (dataSeries.Data.Length > 0)
                {
                    // this is really dumb, but i don't have time for anything better
                    DataSeries tempSeries = dataSeries.SortedCopy;

                    int med_index = tempSeries.dataSeries.Length / 2;
                    medianOfValues = ((tempSeries.dataSeries.Length % 2) == 0)
                        ? ((tempSeries.dataSeries[med_index - 1] + tempSeries.dataSeries[med_index]) / 2.0)
                        : tempSeries.dataSeries[med_index];
                }
                else
                    medianOfValues = double.NaN;
            }

            public int numberOfValues;
            public double maximumValue, minimumValue, averageValue, medianOfValues, stretchOfValues, stretchPerValue;

            public override string ToString()
            {
                System.Text.StringBuilder output = new StringBuilder();
                output.AppendLine("Statistics:\n");
                output.AppendLine("NumberOfValues:\t" + numberOfValues.ToString());
                output.AppendLine("MaximumValue:\t" + maximumValue.ToString());
                output.AppendLine("MinimumValue:\t" + minimumValue.ToString());
                output.AppendLine("AverageValue:\t" + averageValue.ToString());
                output.AppendLine("MedianOfValues:\t" + medianOfValues.ToString());
                output.AppendLine("StretchOfValues:\t" + stretchOfValues.ToString());
                output.AppendLine("StretchPerValue:\t" + stretchPerValue.ToString());
                return output.ToString();
            }
        }

        #endregion

        [System.NonSerialized]
        private Statistics statistics = null;

        public void recalculateStatistics()
        {
            this.statistics = new Statistics(this);
        }

        [System.Xml.Serialization.XmlIgnore]
        public Statistics CurrentStatistics
        {
            get
            {
                if (statistics == null)
                    recalculateStatistics();
                return statistics;
            }
        }

        public override string ToString()
        {
            return "DataSeries(" + ((this.dataSeries != null) ? this.dataSeries.Length.ToString() : "no") + " values)";
        }

        #region IDataSet Members

/*
        private const uint spacing_x = 90;
        private const uint spacing_y = 30;
*/

        public void draw(Graphics graphics)
        {
            Statistics stats = this.CurrentStatistics;
            graphics.Clear(Color.White);
            RectangleF rec = graphics.VisibleClipBounds;

            IView view = new View(this);
            IData data = new Data1D(string.Empty, 
                new Axis(string.Empty, string.Empty, new Range(Range.P1.X, Range.P2.X), string.Empty), 
                new Axis(string.Empty, string.Empty, new Range(Range.P1.Y, Range.P2.Y), string.Empty),
                dataSeries, string.Empty);

            view.Draw(graphics, rec);
            data.Draw(graphics, rec, view, new DrawingContext());                 

/*
			Pen pen = new Pen(Color.OrangeRed);
			Pen pen3 = new Pen(Color.DarkRed);

			double screen_stretch = ((double) (rec.Bottom - rec.Top - 1));
            double screen_width = ((double)(rec.Right - rec.Left - 1));
            double data_stretch = (double) (stats.maximumValue - stats.minimumValue);
            double y_stretch = (data_stretch == 0) ? 1 : screen_stretch / data_stretch;
            double x_stretch = screen_width / (dataSeries.Length - 1);

            uint nlines_x = (uint) Math.Floor(screen_width / ((double) spacing_x));
            uint nlines_y = (uint)Math.Floor(screen_stretch / ((double)spacing_y));

            int ind;
            
            Pen pen2 = new Pen(Color.DarkGray);
            Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            pen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pen2.Width = (float) 0.2;

            for (ind = 1; ind <= nlines_y; ind++)
            {
                double multiplier = ((double) ind) / ((double) nlines_y);
                float y = rec.Bottom - (float)(multiplier * screen_stretch);
                double value_y = stats.minimumValue + multiplier * data_stretch;
                if (ind < nlines_y)
                    graphics.DrawLine(pen2, rec.Left, y, rec.Right, y);
                graphics.DrawString(value_y.ToString("f9"), font, Brushes.Black, 1, y + 1);
            }

            for (ind = 1; ind <= nlines_x; ind++)
            {
                double multiplier = ((double)ind) / ((double) nlines_x);
                float x = rec.Left + (float)(multiplier * screen_width);
                double value_x = multiplier * ((double) dataSeries.Length);
                if (ind < nlines_x)
                    graphics.DrawLine(pen2, x, rec.Top, x, rec.Bottom);
                string s = value_x.ToString("f3");
                SizeF ssize = graphics.MeasureString(s, font);
                graphics.DrawString(s,  font, Brushes.Black, x - ssize.Width - 1, rec.Bottom - ssize.Height - 1);
            }

            // Font font2 = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            // graphics.DrawString(stats.ToString(), font2, Brushes.DarkGray, 50, 10);

            for (ind = 1; ind < dataSeries.Length; ind++)
            {
                try
                {
                    float x1 = rec.Left + (float)((ind - 1) * x_stretch);
                    float y1 = rec.Bottom - 1 - (float)((dataSeries[ind - 1] - stats.minimumValue) * y_stretch);
                    float x2 = rec.Left + (float)(ind * x_stretch);
                    float y2 = rec.Bottom - 1 - (float)((dataSeries[ind] - stats.minimumValue) * y_stretch);

                    graphics.DrawLine(pen, x1, y1, x2, y2);
                    graphics.DrawEllipse(pen3, x1 - 1, y1 - 1, 2, 2);
                    graphics.DrawEllipse(pen3, x2 - 1, y2 - 1, 2, 2);
                }
                catch (Exception)
                {
                }
            }

            try
            {
                float average_y = rec.Bottom - 1 - (float)((stats.averageValue - stats.minimumValue) * y_stretch);
                graphics.DrawLine(Pens.Blue, rec.Left, average_y, rec.Left + 50, average_y);
            }
            catch (Exception)
            {
            }
*/
        }

        #endregion

        public IDataSet downsample(System.Drawing.Size targetResolution)
        {
            return this.downsample((uint) targetResolution.Width);
        }

        public DataSeries downsample(uint nsamples)
        {
            if (nsamples > dataSeries.Length)
            {
                // throw new Exception("Cannot downsample, resolution is already lower than the target resolution.");
                return this;
            }

            double scaling_factor = ((double) nsamples) / ((double) dataSeries.Length);

            double[] downsampled_data = new double[nsamples];
            int num_samples = 0, current_sampleno = -1;
            double cumulative_datavalue = 0;
            for (uint ind = 0; ind < dataSeries.Length; ind++)
            {
                int this_sampleno = (int) Math.Floor(((double)ind) * scaling_factor);
                if (this_sampleno > current_sampleno)
                {
                    if (current_sampleno >= 0)
                        downsampled_data[current_sampleno] = (num_samples > 0) ? (cumulative_datavalue / ((double)num_samples)) : 0;

                    cumulative_datavalue = 0;
                    num_samples = 0;
                    current_sampleno = this_sampleno;
                }

                cumulative_datavalue += dataSeries[ind];
                num_samples++;
            }

            if (num_samples > 0)
                downsampled_data[current_sampleno] = cumulative_datavalue / ((double)num_samples);

            return new DataSeries(downsampled_data);
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Downsample to 1000")]
		public IDataSet DownsampleTo1000
		{
			get { return downsample((uint) 1000); }
		}

		public DataSeries CreateCopy
        {
            get
            {
                double[] duplicate_data = new double[dataSeries.Length];
                for (uint ind = 0; ind < dataSeries.Length; ind++)
                    duplicate_data[ind] = dataSeries[ind];
                return new DataSeries(duplicate_data);
            }
        }

        public void sort()
        {
            System.Array.Sort<double>(dataSeries);
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Raw Data")]
        public IDataSet RawData
        {
            get
            {
                return this;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Meaningful Data")]
        public IDataSet MeaningfulData
        {
            get
            {
                List<double> data = new List<double>();
                foreach (double x in dataSeries)
                    if (!Double.IsNaN(x) && !Double.IsInfinity(x))
                        data.Add(x);
                return new DataSeries(data.ToArray());
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sorted Data")]
        public DataSeries SortedCopy
        {
            get
            {
                DataSeries result = this.CreateCopy;
                result.sort();
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Delta")]
        public IDataSet Delta
        {
            get
            {
                double[] deltas = new double[dataSeries.Length - 1];
                for (uint ind = 0; ind < deltas.Length; ind++)
                    deltas[ind] = dataSeries[ind + 1] - dataSeries[ind];
                return new DataSeries(deltas);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative_Sum")]
        public IDataSet Cumulative_Sum
        {
            get
            {
                double[] cum = new double[dataSeries.Length];
                double sum = 0;
                for (uint ind = 0; ind < dataSeries.Length; ind++)
                {
                    sum += dataSeries[ind];
                    cum[ind] = sum;
                }
                return new DataSeries(cum);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative_Average")]
        public IDataSet Cumulative_Average
        {
            get
            {
                double[] cum = new double[dataSeries.Length];
                double sum = 0;
                for (uint ind = 0; ind < dataSeries.Length; ind++)
                {
                    sum += dataSeries[ind];
                    cum[ind] = sum / ((double) (ind + 1));
                }
                return new DataSeries(cum);
            }
        }

        private double[] smoothed(double smoothing_factor)
        {
            double[] results = new double[dataSeries.Length];
            double current_value = results[0] = dataSeries[0];
            for (uint ind = 1; ind < dataSeries.Length; ind++)
                current_value = results[ind] = smoothing_factor * current_value + (1 - smoothing_factor) * dataSeries[ind];
            return results;
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sort + Delta")]
        public IDataSet DeltaOfSorted
        {
            get
            {
                return this.SortedCopy.Delta;
            }
        }

        private double[] movingAverage(int windowSize)
        {
            double[] result = new double[this.dataSeries.Length - windowSize];
            for (int ind = 0; ind < result.Length; ind++)
            {
                double v = 0;
                for (int k = 0; k < windowSize; k++)
                    v += dataSeries[ind + k];
                v = v / ((double)windowSize);
                result[ind] = v;
            }
            return result;
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Moving Average (window 500)")]
		public IDataSet MovingAverage500
		{
			get
			{
				return new DataSeries(movingAverage(500));
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Moving Average (window 200)")]
		public IDataSet MovingAverage200
		{
			get
			{
				return new DataSeries(movingAverage(200));
			}
		}

		[System.Xml.Serialization.XmlIgnore]
        [DataSource("Moving Average (window 100)")]
        public IDataSet MovingAverage100
        {
            get
            {
                return new DataSeries(movingAverage(100));
            }
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Moving Average (window 50)")]
		public IDataSet MovingAverage50
		{
			get
			{
				return new DataSeries(movingAverage(50));
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Moving Average (window 20)")]
		public IDataSet MovingAverage20
		{
			get
			{
				return new DataSeries(movingAverage(20));
			}
		}

		[System.Xml.Serialization.XmlIgnore]
        [DataSource("Sort + Delta + MovingAverage + Inverse + Sort")]
        public IDataSet SortDeltaMovAvgInvSort
        {
            get
            {
                return ((DataSeries)((DataSeries)((DataSeries)this.SortedCopy.Delta).MovingAverage100).Inverse).SortedCopy;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sort + Delta + Inverse + Sort")]
        public IDataSet SortDeltaInvSort
        {
            get
            {
                return ((DataSeries)((DataSeries)this.SortedCopy.Delta).Inverse).SortedCopy;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sort + Delta + MovingAverage + Inverse")]
        public IDataSet SortDeltaMovAvgInv
        {
            get
            {
                return ((DataSeries)((DataSeries)this.SortedCopy.Delta).MovingAverage100).Inverse;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sort + Delta + Inverse")]
        public IDataSet InverseOfDeltaOfSorted
        {
            get
            {
                return this.SortedCopy.InverseDelta;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sort + Delta + Smooth + Inverse")]
        public IDataSet InverseOfSmoothedDeltaOfSorted
        {
            get
            {
                return ((DataSeries) ((DataSeries) this.SortedCopy.Delta).Smoothed).Inverse;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Smoothed (factor 0.9)")]
        public IDataSet Smoothed
        {
            get
            {
                return new DataSeries(smoothed(0.9));
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Smoothed + Inverse Delta")]
        public IDataSet Smoothed_InverseDelta
        {
            get
            {
                return (this.InverseDelta as DataSeries).Smoothed;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Inverse Delta + Smoothed ")]
        public IDataSet InverseDelta_Smoothed
        {
            get
            {
                return (this.Smoothed as DataSeries).InverseDelta;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Log 10")]
        public IDataSet Log10
        {
            get
            {
                DataSeries result = this.CreateCopy;
                for (uint ind = 0; ind < result.dataSeries.Length; ind++)
                    result.dataSeries[ind] = (result.dataSeries[ind] > 0) ? Math.Log10(result.dataSeries[ind]) : 0;
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Inverse Delta")]
        public IDataSet InverseDelta
        {
            get
            {
                DataSeries result = (DataSeries) this.Delta;
                for (uint ind = 0; ind < result.dataSeries.Length; ind++)
                    result.dataSeries[ind] = (result.dataSeries[ind] != 0) ? (1.0 / result.dataSeries[ind]) : 0;
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Inverse")]
        public IDataSet Inverse
        {
            get
            {
                DataSeries result = (DataSeries) this.CreateCopy;
                for (uint ind = 0; ind < result.dataSeries.Length; ind++)
                    result.dataSeries[ind] = (result.dataSeries[ind] != 0) ? (1.0 / result.dataSeries[ind]) : 0;
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Sorted Inverse Delta")]
        public IDataSet SortedInverseDelta
        {
            get
            {
                DataSeries result = (DataSeries)this.InverseDelta;
                result.sort();
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
		[DataSource("XY")]
		public IDataSet AsXYSeries
		{
            get
            {
                XY[] xy_values = new XY[dataSeries.Length];
                for (uint ind = 0; ind < dataSeries.Length; ind++)
                    xy_values[ind] = new XY((double)ind, dataSeries[ind]);
                return new XYSeries(xy_values);
            }
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Flipped XY")]
		public IDataSet FlippedXY
		{
			get { return ((XYSeries)this.AsXYSeries).FlipXY; }
		}

		[System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative Distribution")]
        public IDataSet CumulativeDistribution
        {
            get
            {
                DataSeries sortedSeries = this.SortedCopy;
                XY[] xyvalues = new XY[sortedSeries.dataSeries.Length];
                double length = (double) sortedSeries.dataSeries.Length;
                for (uint ind = 0; ind < sortedSeries.dataSeries.Length; ind++)
                    xyvalues[ind] = new XY(sortedSeries.dataSeries[ind], ((double) (ind + 1)) / length);
                return new XYSeries(xyvalues);
            }
        }

//        [System.Xml.Serialization.XmlIgnore]
//        [DataSource("Distribution")]
//        public IDataSet Distribution
//        {
//            get
//            {
//                return ((XYSeries) this.CumulativeDistribution).DiffX;
//            }
//        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Distribution")]
        public IDataSet Distribution
        {
            get
            {
                double min_y = double.MaxValue;
                double max_y = double.MinValue;
                int ind;
                for (ind = 0; ind < dataSeries.Length; ind++)
                {
                    double y = dataSeries[ind];
                    if (y < min_y)
                        min_y = y;
                    if (y > max_y)
                        max_y = y;
                }
                double unit_value = 1.0 / ((double)dataSeries.Length);

                int num_buckets = dataSeries.Length;

                XY[] xyvalues = new XY[num_buckets];
                double bucket_size = (max_y - min_y) / ((double) num_buckets);

                for (ind = 0; ind < num_buckets; ind++)
                    xyvalues[ind] = new XY(min_y + (((double) ind) + 0.5) * bucket_size, 0);

                for (ind = 0; ind < dataSeries.Length; ind++)
                {
                    int bucketno = (int) Math.Floor((dataSeries[ind] - min_y) / bucket_size);
                    if (bucketno == dataSeries.Length)
                        bucketno = dataSeries.Length - 1;

                    xyvalues[bucketno].y = xyvalues[bucketno].y + unit_value;
                }

                return new XYSeries(xyvalues);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Distribution Log10")]
        public IDataSet DistributionLog10
        {
            get
            {
                XYSeries result = (XYSeries) this.Distribution;
                result.rescaleYlog10();
                return result;
            }
        }

        private const int MIN_WINDOWSIZE = 20;

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Rate")]
        public IDataSet Rate
        {
            get
            {
                if (dataSeries.Length < MIN_WINDOWSIZE + 2)
                    throw new ArgumentException("Data series too short.");

                DataSeries copy = this.CreateCopy;
                copy.sort();

                int min_windowsize = copy.dataSeries.Length / 10;
                if (min_windowsize < MIN_WINDOWSIZE)
                    min_windowsize = MIN_WINDOWSIZE;
                int result_size = copy.dataSeries.Length - min_windowsize;

                XY[] xy_values = new XY[result_size];
                for (int ind = 0; ind < result_size; ind++)
                {
                    double a = copy.dataSeries[ind];
                    double b = copy.dataSeries[ind + min_windowsize];
                    xy_values[ind] = new XY((a + b) / 2, min_windowsize / (b - a));
                }

                return new XYSeries(xy_values);
            }
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate2")]
		public IDataSet Rate2
		{
			get
			{
				DataSeries copy = this.CreateCopy;
				copy.sort();
				int dataseries_length = dataSeries.Length;
				int windowsize = dataseries_length / 10;
				if (windowsize < MIN_WINDOWSIZE)
					windowsize = MIN_WINDOWSIZE;

				XY[] xy_values = new XY[dataseries_length];
				double last_growthrate = 0;
				for (int ind = 0; ind < dataseries_length; ind++)
				{
					int minimum_ind = ind - windowsize;
					if (minimum_ind < 0)
						minimum_ind = 0;
					int maximum_ind = ind + windowsize;
					if (maximum_ind >= dataseries_length)
						maximum_ind = dataseries_length - 1;

					double y_diff = copy.dataSeries[maximum_ind] - copy.dataSeries[minimum_ind];
					double x_diff = maximum_ind - minimum_ind;
					double growth_rate = (y_diff != 0) ? (x_diff / y_diff) : last_growthrate;

					xy_values[ind] = new XY(copy.dataSeries[ind], growth_rate);
					last_growthrate = growth_rate;
				}
			
				return new XYSeries(xy_values);
			}
		}

/*
		private const int MINIMUM_THROUGHPUT_WINDOW = 100;
		private const int MAXIMUM_THROUGHPUT_WINDOW = 1000;
		// private const double stabilization_threshold = Math.PI / 6; // corresponds to 30 degrees
		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Slope-Based Throughput")]
		public IDataSet SlopeThroughput
		{
			get
			{
				DataSeries sorted_copy = this.CreateCopy;
				sorted_copy.sort();
				int series_length = dataSeries.Length;

				XY[] throughput_values = new XY[series_length];
				for (int ind = 0; ind < series_length; ind++)
				{
					bool stabilized = false;
					double last_throughput = double.NegativeInfinity;
					// double cumulative_thr = 0;
					// int nsamples = 0;
					double minimum_deviation = double.PositiveInfinity;
					double stable_throughput_estimate = 0;
					for (int win = MINIMUM_THROUGHPUT_WINDOW; win < MAXIMUM_THROUGHPUT_WINDOW; win++)
					{
						int min_ind = ind - win;
						if (min_ind < 0)
							min_ind = 0;
						int max_ind = ind + win;
						if (max_ind >= series_length)
							max_ind = series_length - 1;

						double time_difference = sorted_copy.dataSeries[max_ind] - sorted_copy.dataSeries[min_ind];
						int seqno_difference = max_ind - min_ind;
						double window_throughput = (time_difference != 0) ? ((double)seqno_difference) / time_difference : 0;

						double deviation = Math.Abs(Math.Atan(window_throughput) - Math.Atan(last_throughput)); // Math.Abs(window_throughput / last_throughput - 1);
						if (deviation < minimum_deviation)
						{
							stabilized = true;
							minimum_deviation = deviation;
							stable_throughput_estimate = window_throughput;
						}

//						if (stabilized)
//						{
//							if (deviation > stabilization_threshold)
//								break;
//							else
//							{
//								// cumulative_thr += window_throughput;
//								// nsamples++;
//							}
//						}
//						else
//						{
//							if (deviation < stabilization_threshold)
//							{
//								stabilized = true;
//
//								// cumulative_thr += window_throughput;
//								// nsamples++;
//							}
//						}


						last_throughput = window_throughput;
					}

					double throughput_estimate = stabilized ? stable_throughput_estimate : 0;  // last_throughput;
					// last_throughput; // stabilized ? (cumulative_thr / ((double) nsamples)) : last_throughput;

					throughput_values[ind] = new XY(sorted_copy.dataSeries[ind], throughput_estimate);
				}

				return new XYSeries(throughput_values);
			}
		}
*/

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Sort + Delta +Inverse + XY")]
		public IDataSet SortDeltaInverseXY
		{
			get
			{
				DataSeries copy = this.CreateCopy;
				copy.sort();
				double[] sorted_series = copy.dataSeries;
				int num_datapoints = sorted_series.Length - 1;

				XY[] output_values = new XY[num_datapoints];
				for (int ind = 0; ind < num_datapoints; ind++)
				{
					double d = sorted_series[ind + 1] - sorted_series[ind];
					output_values[ind] = new XY(sorted_series[ind], (d != 0) ? (1 / d) : 0);
				}

				return new XYSeries(output_values);
			}
		}

		private const double initial_deviation_multipler = 0.5;
		private const double deviation_multipler = 5;
		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate3")]
		public IDataSet Rate3
		{
			get
			{
				DataSeries copy = this.CreateCopy;
				copy.sort();
				double[] sorted_series = copy.dataSeries;
				int num_datapoints = sorted_series.Length - 1;

				XY[] output_values = new XY[num_datapoints];

				int interval_start = 0;
				while (interval_start < num_datapoints)
				{
					int interval_end = interval_start;
					int maximum_interval_end = num_datapoints - 1;

					while (interval_end < maximum_interval_end)
					{
						double sum_values = (sorted_series[interval_end + 1] - sorted_series[interval_start]);
						double sum_squares = 0;
						for (int ind = interval_start; ind <= interval_end; ind++)
						{
							double d = sorted_series[ind + 1] - sorted_series[ind];
							sum_squares += d * d;
						}
						int interval_size = interval_end - interval_start + 1;

						double mean = sum_values / interval_size;
						double max_deviation = 
							(interval_size > 1) ? (deviation_multipler * Math.Sqrt((sum_squares - mean * mean * (double)interval_size) / (interval_size - 1))) 
							: initial_deviation_multipler * Math.Abs(mean);
						// (interval_size > 1) ? (1.96 * (sum_squares / interval_size - mean * mean) / (interval_size - 1)) : double.PositiveInfinity;

						double x = sorted_series[interval_end + 2] - sorted_series[interval_end + 1];
						double deviation = Math.Abs(x - mean);

						if (deviation < max_deviation)
							interval_end++;
						else
							break;
					}

					double m = (sorted_series[interval_end + 1] - sorted_series[interval_start]) / (interval_end - interval_start + 1);
					double rr = (m != 0) ? (1 / m) : 0;
					for (int ind = interval_start; ind <= interval_end; ind++)
						output_values[ind] = new XY(sorted_series[ind], rr);
/*
					output_values[interval_start] = m;
					for (int ind = interval_start + 1; ind <= interval_end; ind++)
						output_values[ind] = new XY(0;
*/

					interval_start = interval_end + 1;
				}

				return new XYSeries(output_values);
			}
		}

		private XYSeries calculateSlopes(int radius)
		{
			DataSeries copy = this.CreateCopy;
			copy.sort();
			double[] sorted_series = copy.dataSeries;
			int num_datapoints = sorted_series.Length;

			int nslopes = num_datapoints - 2 * radius;
			if (nslopes < 0)
				throw new Exception("Not enough data");

			XY[] slopes = new XY[nslopes];

			for (int ind = 0; ind < nslopes; ind++)
			{
				double t1 = sorted_series[ind];
				double t2 = sorted_series[ind + 2 * radius];

				slopes[ind] = new XY((t1 + t2) / 2, 2 * ((double)radius) / (t2 - t1));
			}

			return new XYSeries(slopes);
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Slopes, R = 100")]
		public IDataSet SlopesR100
		{
			get { return calculateSlopes(100); }
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Slopes, R = 1000")]
		public IDataSet SlopesR1000
		{
			get { return calculateSlopes(1000); }
		}

		private XYSeries digitizedSlopes(int window_size, int window_dots)
		{
			DataSeries copy = this.CreateCopy;
			copy.sort();
			double[] sorted_series = copy.dataSeries;
			int num_datapoints = sorted_series.Length;

			int nwindows = (int) Math.Ceiling(((double) num_datapoints) / ((double) window_size));

			XY[] data_points = new XY[nwindows * window_dots];
			for (int window_ind = 0; window_ind < nwindows; window_ind++)
			{
				int ind1 = window_ind * window_size;
				int ind2 = (ind1 + window_size > num_datapoints - 1) ? (num_datapoints - 1) : (ind1 + window_size);
				double t1 = sorted_series[ind1];
				double t2 = sorted_series[ind2];
				double slope = (ind2 - ind1) / (t2 - t1);

				for (int ind = 0; ind < window_dots; ind++)
					data_points[window_ind * window_dots + ind] = 
						new XY(t1 + (t2 - t1) * (((double)ind) + 0.5) / window_dots, slope);
			}

			return new XYSeries(data_points);
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Digitized Slopes, x20")]
		public IDataSet DigitizedSlopes20
		{
			get { return digitizedSlopes(dataSeries.Length / 20, 50); }
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Digitized Slopes, x100")]
		public IDataSet DigitizedSlopes100
		{
			get { return digitizedSlopes(dataSeries.Length / 100, 10); }
		}

		DataSeries intervalsRate(double interval)
		{
			return intervalsRate(interval, 0);
		}

		DataSeries intervalsRate(int nbuckets)
		{
			return intervalsRate(0, nbuckets);
		}

		DataSeries intervalsRate(double interval, int nbuckets)
		{
			DataSeries sorted = this.SortedCopy;
			double x1 = sorted.dataSeries[0];
			double x2 = sorted.dataSeries[sorted.dataSeries.Length - 1];

			if (nbuckets > 0)
				interval = (x2 - x1) / ((double) nbuckets);
			else
				nbuckets = (int) Math.Floor((x2 - x1) / interval);

			double[] buckets = new double[nbuckets];
			int current_bucket = -1;
			double bucket_fraction = double.NaN;
			for (int ind = 0; ind < sorted.dataSeries.Length; ind++)
			{
				int bucket = (int) Math.Floor((sorted.dataSeries[ind] - sorted.dataSeries[0]) / interval);
				if (bucket >= nbuckets)
					bucket = nbuckets - 1;

				if (bucket < current_bucket)
					throw new Exception("Internal error: bucket = " + bucket.ToString() + ", current_bucket = " + 
						current_bucket.ToString() + ", nbuckets = " + nbuckets.ToString());

				if (bucket > current_bucket)
				{
					current_bucket++;
					while (current_bucket < bucket)
						buckets[current_bucket++] = 0;
					bucket_fraction = 1 / (((current_bucket < nbuckets - 1) ? 1 : ((x2 - x1) / interval - (nbuckets - 1))) * interval);
				}

				buckets[current_bucket] += bucket_fraction;
			}

			return new DataSeries(buckets);
		}
		
		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate-1s")]
		public IDataSet Rate1s
		{
			get { return intervalsRate((double) 1); }
		}

		DataSeries interpolateTo(int nsamples)
		{
			double[] samples = new double[nsamples];
			for (int ind = 0; ind < nsamples; ind++)
			{
				double x = (ind / (nsamples - 1)) * (dataSeries.Length - 1);
				int x1 = (int) Math.Floor(x);
				int x2 = (int) Math.Ceiling(x);

				if (x1 == x2)
					samples[ind] = dataSeries[x1];
				else if (x2 == x1 + 1)
					samples[ind] = dataSeries[x1] * (x - x1) + dataSeries[x2] * (x2 - x);
				else
					throw new Exception("Internal error.");
			}
			return new DataSeries(samples);
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Interpolate to 1000")]
		public IDataSet InterpolateTo1000
		{
			get { return interpolateTo(1000); }
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate-1000int")]
		public IDataSet Rate1000i
		{
			get { return intervalsRate((int)1000); }
		}

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Rate-500int")]
        public IDataSet Rate500i
        {
            get { return intervalsRate((int)500); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Rate-200int")]
        public IDataSet Rate200i
        {
            get { return intervalsRate((int)200); }
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate-100int")]
		public IDataSet Rate100i
		{
			get { return intervalsRate((int) 100); }
		}

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Rate-50int")]
        public IDataSet Rate50i
        {
            get { return intervalsRate((int)50); }
        }

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate-20int")]
		public IDataSet Rate20i
		{
			get { return intervalsRate((int)20); }
		}

		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Rate-10int")]
		public IDataSet Rate10i
		{
			get { return intervalsRate((int)10); }
		}

/*
		[System.Xml.Serialization.XmlIgnore]
		[DataSource("Fit_100")]
		public IDataSet Fit100
		{
			get 
			{ 
				if (dataSeries.
				return intervalsRate(1); 
			
			}
		}
*/

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("NonZero")]
        public IDataSet NonZero
        {
            get 
            {
                double[] nonZero = new double[dataSeries.Length];
                for (int ind = 0; ind < dataSeries.Length; ind++)
                    nonZero[ind] = (dataSeries[ind] == 0) ? 0 : 1;
                return new DataSeries(nonZero); 
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("HasValue")]
        public IDataSet HasValue
        {
            get
            {
                double[] hasValue = new double[dataSeries.Length];
                for (int ind = 0; ind < dataSeries.Length; ind++)
                    hasValue[ind] = double.IsNaN(dataSeries[ind]) ? 0 : 1;
                return new DataSeries(hasValue);
            }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                if (dataSeries == null)
                    dataSeries = new double[0];

                int size = sizeof(int) + dataSeries.Length * sizeof(double);
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_DataSeries, size, size, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*)pheader) = dataSeries.Length;
                pheader += sizeof(int);
                for (int ind = 0; ind < dataSeries.Length; ind++)
                {
                    *((double*)pheader) = dataSeries[ind];
                    pheader += sizeof(double);
                }
            }
            header.consume(sizeof(int) + dataSeries.Length * sizeof(double));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                int count = (int)(*((int*)pheader));
                pheader += sizeof(int);
                dataSeries = new double[count];
                for (int ind = 0; ind < count; ind++)
                {
                    dataSeries[ind] = *((double*)pheader);
                    pheader += sizeof(double);
                }
            }
            header.consume(sizeof(int) + dataSeries.Length * sizeof(double));
        }

        #endregion

        private DataSeries RangeX(double from, double to)
        {
            int min_ind = (int) Math.Floor(((double)(dataSeries.Length - 1)) * from);
            int max_ind = (int) Math.Floor(((double)(dataSeries.Length - 1)) * to);

            double[] result = new double[max_ind - min_ind + 1];
            for (int ind = 0; ind < result.Length; ind++)
                result[ind] = dataSeries[min_ind + ind];
            return new DataSeries(result);
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("L")]
        public IDataSet L
        {
            get { return RangeX(0, 0.5); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("R")]
        public IDataSet R
        {
            get { return RangeX(0.5,1); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i0")]
        public IDataSet i0
        {
            get { return RangeX(0, 0.1); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i1")]
        public IDataSet i1
        {
            get { return RangeX(0.1, 0.2); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i2")]
        public IDataSet i2
        {
            get { return RangeX(0.2, 0.3); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i3")]
        public IDataSet i3
        {
            get { return RangeX(0.3, 0.4); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i4")]
        public IDataSet i4
        {
            get { return RangeX(0.4, 0.5); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i5")]
        public IDataSet i5
        {
            get { return RangeX(0.5, 0.6); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i6")]
        public IDataSet i6
        {
            get { return RangeX(0.6, 0.7); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i7")]
        public IDataSet i7
        {
            get { return RangeX(0.7, 0.8); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i8")]
        public IDataSet i8
        {
            get { return RangeX(0.8, 0.9); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("i9")]
        public IDataSet i9
        {
            get { return RangeX(0.9, 1.0); }
        }

        #region IDataCollector Members

        IDataSet QS._core_c_.Diagnostics.IDataCollector.DataSet
        {
            get { return this; }
        }

        #endregion

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.DataCollector; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return true; }
            set { throw new NotSupportedException(); }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
