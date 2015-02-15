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

namespace QS._qss_e_.Data_
{
    [QS._core_c_.Serialization.BLOB]
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_MultiSeries)]
    // [System.Serializable]
    public class MultiSeries : QS._core_e_.Data.IDataSet, QS.Fx.Serialization.ISerializable, QS._core_c_.Diagnostics.IDataCollector
	{
		public MultiSeries()
		{			
		}

		private Dictionary<string, QS._core_e_.Data.DataSeries> seriesCollection = new Dictionary<string, QS._core_e_.Data.DataSeries>();
        private bool has_statistics;
        private double minimumValue, maximumValue;
        private int maxCount;

        public override string ToString()
        {
            return "MultiSeries" + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(seriesCollection.Keys, ", ");
        }

        private void _CalculateStatistics()
        {
            if (!has_statistics)
            {
                minimumValue = double.MaxValue;
                maximumValue = double.MinValue;
                maxCount = 0;

                foreach (KeyValuePair<string, QS._core_e_.Data.DataSeries> element in seriesCollection)
                {
                    string name = element.Key;
                    QS._core_e_.Data.DataSeries series = element.Value;

                    double[] data = series.Data;
                    if (data.Length > maxCount)
                        maxCount = data.Length;
                    for (int ind = 0; ind < data.Length; ind++)
                    {
                        if (data[ind] < minimumValue)
                            minimumValue = data[ind];
                        else if (data[ind] > maximumValue)
                            maximumValue = data[ind];
                    }
                }

                has_statistics = true;
            }
        }

        public Dictionary<string, QS._core_e_.Data.DataSeries> Series
        {
            get { return seriesCollection; }
            set { seriesCollection = value; }
        }

		#region IDataSet Members

		QS._core_e_.Data.IDataSet QS._core_e_.Data.IDataSet.downsample(System.Drawing.Size targetResolution)
		{
            MultiSeries ms = new MultiSeries();
            foreach (KeyValuePair<string, QS._core_e_.Data.DataSeries> series in seriesCollection)
                ms.Series.Add(series.Key, (QS._core_e_.Data.DataSeries) series.Value.downsample(targetResolution));
            return ms;
		}

        private const uint spacing_x = 90;
        private const uint spacing_y = 30;

        private readonly Color[] DataColors = new Color[] 
        {
            Color.DarkBlue, Color.DarkGreen, Color.DarkViolet, Color.DarkSlateBlue, Color.DarkTurquoise, Color.DarkSalmon,
            Color.DarkOliveGreen, Color.DarkCyan, Color.DarkKhaki, Color.DarkMagenta
        };

        private readonly Color[] ConnectionColors = new Color[] 
        {
            Color.Blue, Color.Green, Color.Violet, Color.SlateBlue, Color.Turquoise, Color.Salmon, Color.Olive, Color.Cyan,
            Color.Khaki, Color.Magenta
        };

		void QS._core_e_.Data.IDataSet.draw(System.Drawing.Graphics graphics)
		{
            graphics.Clear(Color.White);
            RectangleF rec = graphics.VisibleClipBounds;
            double screen_stretch = ((double)(rec.Bottom - rec.Top - 1));
            double screen_width = ((double)(rec.Right - rec.Left - 1));

            uint nlines_x = (uint)Math.Floor(screen_width / ((double)spacing_x));
            uint nlines_y = (uint)Math.Floor(screen_stretch / ((double)spacing_y));
            
            int ind;

            _CalculateStatistics();

            double data_stretch = maximumValue - minimumValue;
            double y_stretch = (data_stretch == 0) ? 1 : screen_stretch / data_stretch;
            double x_stretch = screen_width / (maxCount - 1);

            Pen pen2 = new Pen(Color.DarkGray);
            Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            pen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pen2.Width = (float)0.2;

            for (ind = 1; ind <= nlines_y; ind++)
            {
                double multiplier = ((double)ind) / ((double)nlines_y);
                float y = rec.Bottom - (float)(multiplier * screen_stretch);
                double value_y = minimumValue + multiplier * data_stretch;
                if (ind < nlines_y)
                    graphics.DrawLine(pen2, rec.Left, y, rec.Right, y);
                graphics.DrawString(value_y.ToString("f9"), font, Brushes.Black, 1, y + 1);
            }

            for (ind = 1; ind <= nlines_x; ind++)
            {
                double multiplier = ((double)ind) / ((double)nlines_x);
                float x = rec.Left + (float)(multiplier * screen_width);
                double value_x = multiplier * ((double) maxCount);
                if (ind < nlines_x)
                    graphics.DrawLine(pen2, x, rec.Top, x, rec.Bottom);
                string s = value_x.ToString("f3");
                SizeF ssize = graphics.MeasureString(s, font);
                graphics.DrawString(s, font, Brushes.Black, x - ssize.Width - 1, rec.Bottom - ssize.Height - 1);
            }

            int seriesIndex = 0;
            int consumed_v = 0;
            foreach (KeyValuePair<string, QS._core_e_.Data.DataSeries> element in seriesCollection)
            {
                if (seriesIndex > DataColors.Length || seriesIndex > ConnectionColors.Length)
                    break;

                string name = element.Key;
                double[] series = element.Value.Data;

                Pen pen = new Pen(DataColors[seriesIndex]);
                Pen pen3 = new Pen(ConnectionColors[seriesIndex]);

                for (ind = 1; ind < series.Length; ind++)
                {
                    try
                    {
                        float x1 = rec.Left + (float)((ind - 1) * x_stretch);
                        float y1 = rec.Bottom - 1 - (float)((series[ind - 1] - minimumValue) * y_stretch);
                        float x2 = rec.Left + (float)(ind * x_stretch);
                        float y2 = rec.Bottom - 1 - (float)((series[ind] - minimumValue) * y_stretch);

                        graphics.DrawLine(pen, x1, y1, x2, y2);
                        graphics.DrawEllipse(pen3, x1 - 1, y1 - 1, 2, 2);
                        graphics.DrawEllipse(pen3, x2 - 1, y2 - 1, 2, 2);
                    }
                    catch (Exception)
                    {
                    }
                }

                Font font2 = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF sss = graphics.MeasureString(name, font2);

                Pen pen4 = new Pen(DataColors[seriesIndex], 4);
                Pen pen5 = new Pen(ConnectionColors[seriesIndex], 4);
                graphics.DrawLine(pen4, (rec.Left + rec.Right - sss.Width - 50) / 2, rec.Top + consumed_v + sss.Height / 2,
                    (rec.Left + rec.Right - sss.Width) / 2, consumed_v + sss.Height / 2);
                graphics.DrawLine(pen5, (rec.Left + rec.Right - sss.Width) / 2, rec.Top + consumed_v + sss.Height / 2,
                    (rec.Left + rec.Right - sss.Width + 50) / 2, consumed_v + sss.Height / 2);
                graphics.DrawString(name, font2, Brushes.Black, (rec.Left + rec.Right - sss.Width + 50) / 2, rec.Top + consumed_v);
                consumed_v += (int) sss.Height;

                seriesIndex++;
            }
		}

        QS._core_e_.Data.Rectangle QS._core_e_.Data.IDataSet.Range
		{
			get 
            { 
                _CalculateStatistics();
                return new QS._core_e_.Data.Rectangle(new QS._core_e_.Data.Point(0, minimumValue), new QS._core_e_.Data.Point(maxCount - 1, maximumValue));
            }
		}

		#endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.TMS_Data_MultiSeries, (ushort) sizeof(int), sizeof(int), 0);
                foreach (KeyValuePair<string, QS._core_e_.Data.DataSeries> element in seriesCollection)
                {
                    info.AddAnother((new QS._core_c_.Base2.StringWrapper(element.Key)).SerializableInfo);
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element.Value).SerializableInfo);
                }
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*)pheader) = seriesCollection.Count;
            }
            header.consume(sizeof(int));
            foreach (KeyValuePair<string, QS._core_e_.Data.DataSeries> element in seriesCollection)
            {
                (new QS._core_c_.Base2.StringWrapper(element.Key)).SerializeTo(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)element.Value).SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                count = (int)(*((int*)pheader));
            }
            header.consume(sizeof(int));
            seriesCollection = new Dictionary<string,QS._core_e_.Data.DataSeries>(count);
            while (count-- > 0)
            {
                QS._core_c_.Base2.StringWrapper wrapper = new QS._core_c_.Base2.StringWrapper();
                wrapper.DeserializeFrom(ref header, ref data);
                QS._core_e_.Data.DataSeries ds = new QS._core_e_.Data.DataSeries();
                ((QS.Fx.Serialization.ISerializable) ds).DeserializeFrom(ref header, ref data);
                seriesCollection.Add(wrapper.String, ds);
            }
        }

        #endregion

        [System.Xml.Serialization.XmlIgnore]
        [QS._core_e_.Data.DataSource("_new")]
        public QS._core_e_.Data.IDataSet _New
        {
            get
            {
                QS._core_e_.Data.DataCo dataCo = new QS._core_e_.Data.DataCo();
                foreach (QS._core_e_.Data.DataSeries series in seriesCollection.Values)
                    dataCo.Add(new QS._core_e_.Data.Data1D(series));
                return dataCo;
            }
        }

        #region IDataCollector Members

        QS._core_e_.Data.IDataSet QS._core_c_.Diagnostics.IDataCollector.DataSet
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
