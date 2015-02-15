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
using System.Drawing;

#endregion

namespace QS._core_e_.Data
{
    [QS._core_c_.Serialization.BLOB]
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_XYSeries)]
    [System.Serializable]
    public class XYSeries : IDataSet, QS.Fx.Serialization.ISerializable, QS._core_c_.Diagnostics.IDataCollector
    {
        public Rectangle Range
        {
            get { return new Rectangle(new Point(min_x, min_y), new Point(max_x, max_y)); }
        }

        public XYSeries()
        {
        }

        public XYSeries(XY[] values)
        {
            this.series = (values != null) ? values : new XY[0];
        }

        public XYSeries(int nvalues)
        {
            series = new XY[nvalues];
        }

        private XY[] series;
        private double min_x, min_y, max_x, max_y;
        private bool sorted = false;

        public XY this[int ind]
        {
            get { return series[ind]; }
            set
            {
                series[ind] = value;
                sorted = false;
            }
        }

        public XY[] Data
        {
            get { return series; }
            set
            {
                series = value;
                sorted = false;
            }
        }

        public static readonly XComparer xComparer = new XComparer();
        public class XComparer : IComparer<XY>
        {
            public XComparer()
            {
            }

            public int Compare(XY x, XY y)
            {
                int result = x.x.CompareTo(y.x);
                return (result != 0) ? result : x.y.CompareTo(y.y);
            }

            public bool Equals(XY x, XY y)
            {
                return x.x == y.x && x.y == y.y;
            }

            public int GetHashCode(XY x)
            {
                throw new NotImplementedException();
            }
        }

        public void sort()
        {
            if (!sorted)
            {
                System.Array.Sort<XY>(series, xComparer);

                min_x = series[0].x;
                max_x = series[series.Length - 1].x;

                min_y = double.MaxValue;
                max_y = double.MinValue;

                for (uint ind = 0; ind < series.Length; ind++)
                {
                    if (series[ind].y < min_y)
                        min_y = series[ind].y;

                    if (series[ind].y > max_y)
                        max_y = series[ind].y;
                }
            }
        }

        public override string ToString()
        {
            return "XYSeries(" + this.series.Length.ToString() + " values)";
        }

        #region IDataSet Members

        private const uint nlines_x = 10;
        private const uint nlines_y = 20;

        void IDataSet.draw(System.Drawing.Graphics graphics)
        {
            graphics.Clear(Color.White);
            RectangleF rec = graphics.VisibleClipBounds;

            IData data = new Data2D(series);
            IView view = new View(data);

            view.Draw(graphics, rec);
            data.Draw(graphics, rec, view, new DrawingContext());

            /*
                        this.sort();

                        graphics.Clear(Color.White);
                        RectangleF rec = graphics.VisibleClipBounds;
                        Pen pen = new Pen(Color.OrangeRed);
                        Pen pen3 = new Pen(Color.DarkRed);

                        double x_stretch = max_x - min_x;
                        double y_stretch = max_y - min_y;

                        double screen_height = ((double)(rec.Bottom - rec.Top - 1));
                        double screen_width = ((double)(rec.Right - rec.Left - 1));

                        double x_scaling = (x_stretch == 0) ? 1 : (screen_width / x_stretch);
                        double y_scaling = (y_stretch == 0) ? 1 : (screen_height / y_stretch);

                        int ind;

                        Pen pen2 = new Pen(Color.DarkGray);
                        Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
                        pen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        pen2.Width = (float)0.2;

                        for (ind = 1; ind <= nlines_y; ind++)
                        {
                            try
                            {
                                double multiplier = ((double)ind) / ((double)nlines_y);
                                float y = rec.Bottom - (float)(multiplier * screen_height);
                                double value_y = min_y + multiplier * y_stretch;
                                if (ind < nlines_y)
                                    graphics.DrawLine(pen2, rec.Left, y, rec.Right, y);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        for (ind = 1; ind <= nlines_x; ind++)
                        {
                            try
                            {
                                double multiplier = ((double)ind) / ((double) nlines_x);
                                float x = rec.Left + (float)(multiplier * screen_width);
                                double value_x = min_x + multiplier * x_stretch;
                                if (ind < nlines_x)
                                    graphics.DrawLine(pen2, x, rec.Top, x, rec.Bottom);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        // Font font2 = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
                        // graphics.DrawString(stats.ToString(), font2, Brushes.DarkGray, 50, 10);

                        int old_x = 0, old_y = 0;
                        for (ind = 0; ind < series.Length; ind++)
                        {
                            int new_x = (int)(rec.Left + (float)((series[ind].x - min_x) * x_scaling));
                            int new_y = (int)(rec.Bottom - 1 - (float)((series[ind].y - min_y) * y_scaling));

                            try
                            {
                                if (ind > 0)
                                {
                                    if (new_x != old_x || new_y != old_y)
                                    {
                                        graphics.DrawLine(pen, old_x, old_y, new_x, new_y);
                                        graphics.DrawEllipse(pen3, old_x - 1, old_y - 1, 2, 2);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }

                            old_x = new_x;
                            old_y = new_y;
                        }

                        try
                        {
                            graphics.DrawEllipse(pen3, old_x - 1, old_y - 1, 2, 2);
                        }
                        catch (Exception)
                        {
                        }

            /-*
                        for (ind = 1; ind < series.Length; ind++)
                        {
                            float x1 = rec.Left + (float)((series[ind - 1].x - min_x) * x_scaling);
                            float y1 = rec.Bottom - 1 - (float)((series[ind - 1].y - min_y) * y_scaling);
                            float x2 = rec.Left + (float)((series[ind].x - min_x) * x_scaling);
                            float y2 = rec.Bottom - 1 - (float)((series[ind].y - min_y) * y_scaling);
                            graphics.DrawLine(pen, x1, y1, x2, y2);
                            graphics.DrawEllipse(pen3, x1 - 1, y1 - 1, 2, 2);
                            graphics.DrawEllipse(pen3, x2 - 1, y2 - 1, 2, 2);
                        }
            *-/

            /-*
                        float average_y = rec.Bottom - 1 - (float)((stats.averageValue - stats.minimumValue) * y_stretch);
                        graphics.DrawLine(Pens.Blue, rec.Left, average_y, rec.Left + 50, average_y);
            *-/

                        for (ind = 1; ind <= nlines_y; ind++)
                        {
                            try
                            {
                                double multiplier = ((double)ind) / ((double)nlines_y);
                                float y = rec.Bottom - (float)(multiplier * screen_height);
                                double value_y = min_y + multiplier * y_stretch;
                                graphics.DrawString(value_y.ToString("f9"), font, Brushes.Black, 1, y + 1);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        for (ind = 1; ind <= nlines_x; ind++)
                        {
                            try
                            {
                                double multiplier = ((double)ind) / ((double)nlines_x);
                                float x = rec.Left + (float)(multiplier * screen_width);
                                double value_x = min_x + multiplier * x_stretch;
                                string s = value_x.ToString("f3");
                                SizeF ssize = graphics.MeasureString(s, font);
                                graphics.DrawString(s, font, Brushes.Black, x - ssize.Width - 1, rec.Bottom - ssize.Height - 1);
                            }
                            catch (Exception)
                            {
                            }
                        }
            */
        }

        IDataSet IDataSet.downsample(System.Drawing.Size targetResolution)
        {
            this.sort();

            double bucketsize_x = (max_x - min_x) / ((double)targetResolution.Width);
            double bucketsize_y = (max_y - min_y) / ((double)targetResolution.Height);

            List<XY> buckets = new List<XY>();
            int current_x = -1, current_y = -1;
            for (int ind = 0; ind < series.Length; ind++)
            {
                int bucket_x = (int)Math.Floor((series[ind].x - min_x) / bucketsize_x);
                if (bucket_x >= targetResolution.Width)
                    bucket_x = (targetResolution.Width - 1);
                int bucket_y = (int)Math.Floor((series[ind].y - min_y) / bucketsize_y);
                if (bucket_y > targetResolution.Height)
                    bucket_y = (targetResolution.Height - 1);

                if (bucket_x != current_x || bucket_y != current_y)
                {
                    current_x = bucket_x;
                    current_y = bucket_y;

                    double center_x = min_x + (((double)bucket_x) + 0.5) * bucketsize_x;
                    double center_y = min_y + (((double)bucket_y) + 0.5) * bucketsize_y;

                    buckets.Add(new XY(center_x, center_y));
                }
            }

            return new XYSeries(buckets.ToArray());

            /*


                        int ind, nsamples = 0, current_bucketno = -1;
                        int current_x = -1;
                        int current_y = -1;
                        double x_sum = 0, y_sum = 0;

                        int max_numbuckets =  targetResolution.Width + targetResolution.Height;            
                        XY[] downsampled_data = new XY[max_numbuckets];
            
                        for (ind = 0; ind < series.Length; ind++)
                        {
                            int this_x = (int) Math.Floor((series[ind].x - min_x) / bucketsize_x);
                            if (this_x > targetResolution.Width)
                                this_x = targetResolution.Width;
                            int this_y = (int) Math.Floor((series[ind].y - min_y) / bucketsize_y);
                            if (this_y > targetResolution.Width)
                                this_y = targetResolution.Width;

                            if (this_x != current_x || this_y != current_y)
                            {
                                if (current_bucketno >= 0)
                                {
                                    downsampled_data[current_bucketno] = 
                                        new XY(x_sum / ((double) nsamples), y_sum / ((double) nsamples));
                                }

                                current_bucketno++;
                                current_x = this_x;
                                current_y = this_y;
                                nsamples = 0;
                                x_sum = y_sum = 0;
                            }

                            x_sum += series[ind].x;
                            y_sum += series[ind].y;
                            nsamples++;
                        }

                        if (nsamples > 0)
                        {
                            downsampled_data[current_bucketno] = 
                                new XY(x_sum / ((double) nsamples), y_sum / ((double) nsamples));
                        }

                        int numbuckets_used = current_bucketno + 1;

                        XY[] result = new XY[numbuckets_used];
                        for (ind = 0; ind < numbuckets_used; ind++)
                            result[ind] = downsampled_data[ind];

                        return new XYSeries(result);
            */
        }

        #endregion

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
                List<XY> data = new List<XY>();
                foreach (XY e in series)
                    if (!Double.IsNaN(e.x) && !Double.IsInfinity(e.x) && !Double.IsNaN(e.y) && !Double.IsInfinity(e.y))
                        data.Add(e);
                return new XYSeries(data.ToArray());
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Flatten Y")]
        public IDataSet FlattenY
        {
            get
            {
                XY[] data = new XY[series.Length];
                for (int ind = 0; ind < series.Length; ind++)
                    data[ind] = new XY(series[ind].x, (double)1);
                return new XYSeries(data);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative_Sum_Y")]
        public IDataSet Cumulative_Sum_Y
        {
            get
            {
                XY[] data = new XY[series.Length];
                double sum_y = 0;
                for (int ind = 0; ind < series.Length; ind++)
                {
                    sum_y += series[ind].y;
                    data[ind] = new XY(series[ind].x, sum_y);
                }
                return new XYSeries(data);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative_Average_Y")]
        public IDataSet Cumulative_Average_Y
        {
            get
            {
                XY[] data = new XY[series.Length];
                double sum_y = 0;
                for (int ind = 0; ind < series.Length; ind++)
                {
                    sum_y += series[ind].y;
                    data[ind] = new XY(series[ind].x, sum_y / ((double)(ind + 1)));
                }
                return new XYSeries(data);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Growth_Rate")]
        public IDataSet Growth_Rate
        {
            get
            {
                XY[] data = new XY[series.Length - 1];
                for (int ind = 0; ind < series.Length; ind++)
                    data[ind] = new XY((series[ind].x + series[ind + 1].x) / 2, (series[ind + 1].y - series[ind].y) / (series[ind + 1].x - series[ind].x));
                return new XYSeries(data);
            }
        }

        private IDataSet GrowthRate_InIntervals(int nintervals)
        {
            sort();
            List<XY> data = new List<XY>();
            double interval_x = (max_x - min_x) / nintervals;
            double base_x = series[0].x;
            double base_y = series[0].y;
            for (int ind = 1; ind < series.Length; ind++)
            {
                if (series[ind].x >= (base_x + interval_x) || ind == (series.Length - 1))
                {
                    double sample_x = (base_x + series[ind].x) / 2;
                    double sample_y = (series[ind].y - base_y) / (series[ind].x - base_x);
                    data.Add(new XY(sample_x, sample_y));
                    base_x = series[ind].x;
                    base_y = series[ind].y;
                }
            }
            return new XYSeries(data.ToArray());
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Growth_Rate_25i")]
        public IDataSet Growth_Rate_25i
        {
            get { return GrowthRate_InIntervals(25); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Growth_Rate_50i")]
        public IDataSet Growth_Rate_50i
        {
            get { return GrowthRate_InIntervals(50); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Growth_Rate_100i")]
        public IDataSet Growth_Rate_100i
        {
            get { return GrowthRate_InIntervals(100); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Growth_Rate_200i")]
        public IDataSet Growth_Rate_200i
        {
            get { return GrowthRate_InIntervals(200); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("AddUp_Y")]
        public IDataSet AddUp_Y
        {
            get
            {
                sort();
                double sum_y = 0;
                XY[] new_data = new XY[series.Length];
                for (int ind = 0; ind < series.Length; ind++)
                {
                    sum_y += series[ind].y;
                    new_data[ind] = new XY(series[ind].x, sum_y);
                }
                return new XYSeries(new_data);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Integrate_Y")]
        public IDataSet Integrate_Y
        {
            get
            {
                sort();
                XY[] new_data = new XY[series.Length + 1];
                new_data[0] = new XY(series[0].x, 0);
                double sum = (series[1].x - series[0].x) * (series[0].y + series[1].y) / 4;
                new_data[1] = new XY((series[0].x + series[1].x) / 2, sum);
                for (int ind = 0; ind <= series.Length - 3; ind++)
                {
                    sum += (series[ind + 1].x - series[ind].x) * (series[ind].y + series[ind + 1].y) / 4
                        + (series[ind + 2].x - series[ind + 1].x) * (series[ind + 1].y + series[ind + 2].y) / 4;
                    new_data[ind + 2] = new XY((series[ind + 1].x + series[ind + 2].x) / 2, sum);
                }
                sum += (series[series.Length - 1].x - series[series.Length - 2].x) * (series[series.Length - 2].y + series[series.Length - 1].y) / 4;
                new_data[series.Length] = new XY(series[series.Length - 1].x, sum);
                return new XYSeries(new_data);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Rate")]
        public IDataSet Rate
        {
            get
            {
                sort();
                XY[] data = new XY[series.Length - 1];
                for (int ind = 0; ind < series.Length; ind++)
                    data[ind] = new XY((series[ind].x + series[ind + 1].x) / 2, (series[ind + 1].y - series[ind].y) / (series[ind + 1].x - series[ind].x));
                return new XYSeries(data);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("X = Raw, Y = Log 10")]
        public IDataSet WithYLog10
        {
            get
            {
                XY[] newValues = new XY[series.Length];
                for (int ind = 0; ind < newValues.Length; ind++)
                    newValues[ind] = new XY(series[ind].x, Math.Log10(series[ind].y));
                return new XYSeries(newValues);
            }
        }


        //        // [DataSource("Raw Data")]
        //        [System.Xml.Serialization.XmlIgnore]
        //        public IDataSet DiffX
        //        {
        //            get
        //            {
        //                XY[] diff_series = new XY[series.Length - 1];
        //                for (uint ind = 0; ind < diff_series.Length; ind++)
        //                {
        //                    double dx = series[ind + 1].x - series[ind].x;
        //                    double dy = series[ind + 1].y - series[ind].y;
        //                    diff_series[ind] = new XY(series[ind + 1].x, (dx != 0) ? (dy / dx) : 0);
        //                }
        //                return new XYSeries(diff_series);
        //            }
        //        }

        public void rescaleYlog10()
        {
            this.sort();

            double min_positive_y = double.MaxValue;
            for (int ind = 0; ind < series.Length; ind++)
                if (0 < series[ind].y && series[ind].y < min_positive_y)
                    min_positive_y = series[ind].y;
            double minlog = Math.Log10(min_positive_y);

            for (int ind = 0; ind < series.Length; ind++)
                series[ind].y = (series[ind].y > 0) ? Math.Log10(series[ind].y) : minlog;

            this.sorted = false;
        }

        private XY[] droptails(double percentage_toinclude, bool drop_x, bool drop_y)
        {
            MyMath.BaseStatistics xs = new QS._core_e_.MyMath.BaseStatistics();
            MyMath.BaseStatistics ys = new QS._core_e_.MyMath.BaseStatistics();

            for (int ind = 0; ind < series.Length; ind++)
            {
                xs.AddSample(series[ind].x);
                ys.AddSample(series[ind].y);
            }

            // throw new Exception("XS\n" + xs.ToString() + "\nYS\n" + ys.ToString());

            List<XY> good = new List<XY>();
            for (int ind = 0; ind < series.Length; ind++)
            {
                if ((!drop_x || System.Math.Abs(series[ind].x - xs.Average) < xs.CI95Size) &&
                    (!drop_y || System.Math.Abs(series[ind].y - ys.Average) < ys.CI95Size))
                    good.Add(series[ind]);
            }

            return good.ToArray();
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Droptail X, 95%")]
        public IDataSet DropTailX95
        {
            get { return new XYSeries(droptails(0.95, true, false)); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Droptail Y, 95%")]
        public IDataSet DropTailY95
        {
            get { return new XYSeries(droptails(0.95, false, true)); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Droptail XY, 95%")]
        public IDataSet DropTailXY95
        {
            get { return new XYSeries(droptails(0.95, true, true)); }
        }

        private IDataSet accumulateX(double percentage)
        {
            sort();

            double sum_xy = 0, sum_y = 0;
            for (int ind = 0; ind < series.Length; ind++)
            {
                sum_y += series[ind].y;
                sum_xy += series[ind].x * series[ind].y;
            }
            double masscenter_x = sum_xy / sum_y;

            double margin_width = (1 - percentage) / 2;

            int ind1 = 0, ind2 = series.Length - 1;
            double togo, cutoff = sum_xy * margin_width;
            for (togo = cutoff; togo > 0; ind1++)
                togo -= series[ind1].x * series[ind1].y;
            for (togo = cutoff; togo > 0; ind2--)
                togo -= series[ind2].x * series[ind2].y;

            XY[] result = new XY[ind2 - ind1 + 1];
            for (int ind = 0; ind < ind2 - ind1; ind++)
                result[ind] = series[ind + ind1];

            return new XYSeries(result);
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Accumulate X, 50%")]
        public IDataSet AccumulateX50
        {
            get { return accumulateX(0.5); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Accumulate X, 90%")]
        public IDataSet AccumulateX90
        {
            get { return accumulateX(0.9); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Accumulate X, 95%")]
        public IDataSet AccumulateX95
        {
            get { return accumulateX(0.95); }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Flip_XY")]
        public IDataSet FlipXY
        {
            get
            {
                XY[] flipped_series = new XY[series.Length];
                for (int ind = 0; ind < series.Length; ind++)
                    flipped_series[ind] = new XY(series[ind].y, series[ind].x);
                return new XYSeries(flipped_series);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Project on X")]
        public IDataSet ProjectOnX
        {
            get
            {
                double[] result = new double[series.Length];
                for (int ind = 0; ind < series.Length; ind++)
                    result[ind] = series[ind].x;
                return new DataSeries(result);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Project on Y")]
        public IDataSet ProjectOnY
        {
            get
            {
                double[] result = new double[series.Length];
                for (int ind = 0; ind < series.Length; ind++)
                    result[ind] = series[ind].y;
                return new DataSeries(result);
            }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = sizeof(int) + series.Length * 2 * sizeof(double);
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_XYSeries, size, size, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*)pheader) = series.Length;
                pheader += sizeof(int);
                for (int ind = 0; ind < series.Length; ind++)
                {
                    *((double*)pheader) = series[ind].x;
                    *((double*)(pheader + sizeof(double))) = series[ind].y;
                    pheader += 2 * sizeof(double);
                }
            }
            header.consume(sizeof(int) + series.Length * 2 * sizeof(double));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                int count = (int)(*((int*)pheader));
                pheader += sizeof(int);
                series = new XY[count];
                for (int ind = 0; ind < count; ind++)
                {
                    series[ind] = new XY(*((double*)pheader), *((double*)(pheader + sizeof(double))));
                    pheader += 2 * sizeof(double);
                }
            }
            header.consume(sizeof(int) + series.Length * 2 * sizeof(double));
        }

        #endregion

        private XYSeries RangeX(double from, double to)
        {
            sort();

            double from_x = min_x + (max_x - min_x) * from;
            double from_y = double.NegativeInfinity;

            double to_x = min_x + (max_x - min_x) * to;
            double to_y = double.PositiveInfinity;

            int min_ind = Array.BinarySearch<XY>(series, new XY(from_x, from_y), xComparer);
            if (min_ind < 0)
                min_ind = min_ind ^ (-1);
            if (min_ind >= series.Length)
                min_ind = series.Length - 1;

            int max_ind = Array.BinarySearch<XY>(series, new XY(to_x, to_y), xComparer);
            if (max_ind < 0)
                max_ind = max_ind ^ (-1);
            if (max_ind >= series.Length)
                max_ind = series.Length - 1;

            XY[] result = new XY[max_ind - min_ind + 1];
            for (int ind = 0; ind < result.Length; ind++)
                result[ind] = series[min_ind + ind];
            return new XYSeries(result);
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
            get { return RangeX(0.5, 1); }
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

        #region Class MinMax

        private class MinMax
        {
            public MinMax(double x) { min = max = x; }
            public double min, max;
            public void add(double x)
            {
                if (x < min)
                    min = x;
                if (x > max)
                    max = x;
            }
        }

        #endregion

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Span X (for same Y)")]
        public IDataSet SpanX
        {
            get
            {
                IDictionary<double, MinMax> minmaxdic = new Dictionary<double, MinMax>();
                int ind;
                for (ind = 0; ind < series.Length; ind++)
                {
                    MinMax minmax;
                    if (minmaxdic.TryGetValue(series[ind].y, out minmax))
                        minmax.add(series[ind].x);
                    else
                        minmaxdic.Add(series[ind].y, new MinMax(series[ind].x));
                }

                XY[] mins = new XY[minmaxdic.Count], maxs = new XY[minmaxdic.Count];
                ind = 0;
                foreach (KeyValuePair<double, MinMax> element in minmaxdic)
                {
                    mins[ind] = new XY(element.Value.min, element.Key);
                    maxs[ind] = new XY(element.Value.max, element.Key);
                    ind++;
                }

                DataCo result = new DataCo();
                result.Add(new Data2D("mins", mins));
                result.Add(new Data2D("maxs", maxs));
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Span Y (for same X)")]
        public IDataSet SpanY
        {
            get
            {
                IDictionary<double, MinMax> minmaxdic = new Dictionary<double, MinMax>();
                int ind;
                for (ind = 0; ind < series.Length; ind++)
                {
                    MinMax minmax;
                    if (minmaxdic.TryGetValue(series[ind].x, out minmax))
                        minmax.add(series[ind].y);
                    else
                        minmaxdic.Add(series[ind].x, new MinMax(series[ind].y));
                }

                XY[] mins = new XY[minmaxdic.Count], maxs = new XY[minmaxdic.Count];
                ind = 0;
                foreach (KeyValuePair<double, MinMax> element in minmaxdic)
                {
                    mins[ind] = new XY(element.Key, element.Value.min);
                    maxs[ind] = new XY(element.Key, element.Value.max);
                    ind++;
                }

                DataCo result = new DataCo();
                result.Add(new Data2D("mins", mins));
                result.Add(new Data2D("maxs", maxs));
                return result;
            }
        }




        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative Distribution, Raw")]
        public IDataSet CumulativeDistributionRaw
        {
            get
            {
                this.sort();
                double _min_y;
                if (min_y < 0)
                {
                    _min_y = 0;
                }
                else
                {
                    _min_y = min_y;
                }

                int _num_buckets = 1000;
                int[] _buckets = new int[_num_buckets];

                int _percent;


                for (int i = 0; i < series.Length; i++)
                {
                    _percent = (int)Math.Floor((_num_buckets - 1) * (series[i].y - _min_y) / (max_y - _min_y));
                    if (_percent >= _num_buckets)
                        _percent = _num_buckets - 1;
                    _buckets[_percent] += 1;
                }


                XY[] counts = new XY[_num_buckets];

                int _sum = 0;
                int _total = 0;
                for (int i = 0; i < _buckets.Length; i++)
                {
                    _total += _buckets[i];
                }


                for (int i = 0; i < _buckets.Length; i++)
                {
                    _sum += _buckets[i];
                    counts[i] = new XY(i * (1 / (double)_num_buckets) * (max_y - _min_y), _sum / (double)_total);
                }

                return new XYSeries(counts);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Cumulative Distribution, Scaled")]
        public IDataSet CumulativeDistributionScaled
        {
            get
            {

                this.sort();
                double _min_y;
                if (min_y < 0)
                {
                    _min_y = 0;
                }
                else
                {
                    _min_y = min_y;
                }
                int _num_buckets = 1000;
                int[] _buckets = new int[_num_buckets];
                
                int _percent;

                double _max_y = Convert.ToDouble(Microsoft.VisualBasic.Interaction.InputBox("Enter max y value: ", "Y Value", "0.1", -1, -1));
                //for (int i = 0; i < series.Length; i++)
                //{
                //        _percent = (int)Math.Floor((_num_buckets - 1) * (series[i].y - _min_y) / (max_y - _min_y));
                //        if (_percent >= _num_buckets)
                //            _percent = _num_buckets - 1;
                //        try
                //        {
                //            _buckets[_percent] += 1;
                //        }
                //        catch (Exception e)
                //        {

                //        }
                //}


                XY[] counts = new XY[_num_buckets];

                int _sum = 0;
                int _total = 0;
                //for (int i = 0; i < _buckets.Length; i++)
                //{
                //    _total += _buckets[i];
                //}

                
                //for (int i = 0; i < _buckets.Length; i++)
                //{
                //    _sum += _buckets[i];
                //    counts[i] = new XY(i * (1/(double)_num_buckets) * (max_y - _min_y), _sum/(double)_total);
                //}



                //double _cutoff = .99;
                //int _cut = -1;
                //for (int i = 0; i < _buckets.Length; i++)
                //{
                //    if (counts[i].y >= _cutoff)
                //    {
                //        _cut = i;
                //        break;
                //    }
                //}
                //if (_cut <= 0)
                //{
                //    _cut = 1;
                //}
                //double _max_y = counts[_cut].x;

                //_buckets = new int[_num_buckets];


                for (int i = 0; i < series.Length; i++)
                {
                    if (series[i].y <= _max_y && series[i].y >=_min_y)
                    {
                        _percent = (int)Math.Floor((_num_buckets - 1) * (series[i].y - _min_y) / (_max_y - _min_y));
                        if (_percent >= _num_buckets)
                        {
                            _percent = _num_buckets - 1;
                        }
                            _buckets[_percent] += 1;
                        
                    }

                }


                counts = new XY[_num_buckets];

                _sum = 0;
                _total = 0;
                for (int i = 0; i < _buckets.Length; i++)
                {
                    _total += _buckets[i];
                }


                for (int i = 0; i < _buckets.Length; i++)
                {
                    _sum += _buckets[i];
                    counts[i] = new XY(_min_y+(i * (1 / (double)_num_buckets) * (_max_y - _min_y)), _sum / (double)_total);
                }


                return new XYSeries(counts);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Count X (for same Y)")]
        public IDataSet CountX
        {
            get
            {


                IDictionary<double, int> countdic = new Dictionary<double, int>();
                int ind;
                for (ind = 0; ind < series.Length; ind++)
                {
                    int count;
                    if (!countdic.TryGetValue(series[ind].y, out count))
                        count = 0;
                    countdic[series[ind].y] = count + 1;
                }

                XY[] counts = new XY[countdic.Count];
                ind = 0;
                foreach (KeyValuePair<double, int> element in countdic)
                    counts[ind++] = new XY(element.Key, element.Value);

                return new XYSeries(counts);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Count Y (for same X)")]
        public IDataSet CountY
        {
            get
            {
                IDictionary<double, int> countdic = new Dictionary<double, int>();
                int ind;
                for (ind = 0; ind < series.Length; ind++)
                {
                    int count;
                    if (!countdic.TryGetValue(series[ind].x, out count))
                        count = 0;
                    countdic[series[ind].x] = count + 1;
                }

                XY[] counts = new XY[countdic.Count];
                ind = 0;
                foreach (KeyValuePair<double, int> element in countdic)
                    counts[ind++] = new XY(element.Key, element.Value);

                return new XYSeries(counts);
            }
        }

        #region Class DuplicateBin

        private class DuplicateBin
        {
            public DuplicateBin(double v) { original = v; }
            public double original;
            public List<double> duplicates = new List<double>();
            public void add(double v)
            {
                if (v < original)
                {
                    duplicates.Add(original);
                    original = v;
                }
                else
                    duplicates.Add(v);
            }
        }

        #endregion

        private void _SplitDuplicatesWithSameX(out XY[] _originals, out XY[] _duplicates)
        {
            IDictionary<double, DuplicateBin> duplicatebins = new Dictionary<double, DuplicateBin>();
            for (int ind = 0; ind < series.Length; ind++)
            {
                DuplicateBin bin;
                if (duplicatebins.TryGetValue(series[ind].x, out bin))
                    bin.add(series[ind].y);
                else
                    duplicatebins.Add(series[ind].x, new DuplicateBin(series[ind].y));
            }

            List<XY> originals = new List<XY>(), duplicates = new List<XY>();
            foreach (KeyValuePair<double, DuplicateBin> element in duplicatebins)
            {
                originals.Add(new XY(element.Key, element.Value.original));
                foreach (double v in element.Value.duplicates)
                    duplicates.Add(new XY(element.Key, v));
            }

            _originals = originals.ToArray();
            _duplicates = duplicates.ToArray();
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Split duplicates with same X")]
        public IDataSet SplitDuplicatesWithSameX
        {
            get
            {
                XY[] originals, duplicates;
                _SplitDuplicatesWithSameX(out originals, out duplicates);
                DataCo result = new DataCo();
                result.Add(new Data2D("originals", originals));
                result.Add(new Data2D("duplicates", duplicates));
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Get originals with same X")]
        public IDataSet GetOriginalsWithSameX
        {
            get
            {
                XY[] originals;
                _GetOriginalsWithSameY(out originals);
                return new XYSeries(originals);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Get duplicates with same X")]
        public IDataSet GetDuplicatesWithSameX
        {
            get
            {
                XY[] originals, duplicates;
                _SplitDuplicatesWithSameX(out originals, out duplicates);
                return new XYSeries(duplicates);
            }
        }

        private void _SplitDuplicatesWithSameY(out XY[] _originals, out XY[] _duplicates)
        {
            IDictionary<double, DuplicateBin> duplicatebins = new Dictionary<double, DuplicateBin>();
            for (int ind = 0; ind < series.Length; ind++)
            {
                DuplicateBin bin;
                if (duplicatebins.TryGetValue(series[ind].y, out bin))
                    bin.add(series[ind].x);
                else
                    duplicatebins.Add(series[ind].y, new DuplicateBin(series[ind].x));
            }

            List<XY> originals = new List<XY>(), duplicates = new List<XY>();
            foreach (KeyValuePair<double, DuplicateBin> element in duplicatebins)
            {
                originals.Add(new XY(element.Value.original, element.Key));
                foreach (double v in element.Value.duplicates)
                    duplicates.Add(new XY(v, element.Key));
            }

            _originals = originals.ToArray();
            _duplicates = duplicates.ToArray();
        }

        private void _GetOriginalsWithSameX(out XY[] _originals)
        {
            this.sort();
            List<XY> originals = new List<XY>();
            originals.Add(series[0]);
            double last_x = series[0].x;
            for (int ind = 1; ind < series.Length; ind++)
            {
                XY element = series[ind];
                if (element.x > last_x)
                {
                    originals.Add(series[ind]);
                    last_x = element.x;
                }
            }

            _originals = originals.ToArray();
        }

        private void _GetOriginalsWithSameY(out XY[] _originals)
        {
            XY[] originals;
            ((XYSeries)FlipXY)._GetOriginalsWithSameX(out originals);
            for (int ind = 0; ind < originals.Length; ind++)
                originals[ind] = new XY(originals[ind].y, originals[ind].x);
            _originals = originals;
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Split duplicates with same Y")]
        public IDataSet SplitDuplicatesWithSameY
        {
            get
            {
                XY[] originals, duplicates;
                _SplitDuplicatesWithSameY(out originals, out duplicates);
                DataCo result = new DataCo();
                result.Add(new Data2D("originals", originals));
                result.Add(new Data2D("duplicates", duplicates));
                return result;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Get originals with same Y")]
        public IDataSet GetOriginalsWithSameY
        {
            get
            {
                XY[] originals;
                _GetOriginalsWithSameY(out originals);
                return new XYSeries(originals);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DataSource("Get duplicates with same Y")]
        public IDataSet GetDuplicatesWithSameY
        {
            get
            {
                XY[] originals, duplicates;
                _SplitDuplicatesWithSameY(out originals, out duplicates);
                return new XYSeries(duplicates);
            }
        }
    }
}
