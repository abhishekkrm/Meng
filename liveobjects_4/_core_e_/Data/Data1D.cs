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
using System.Drawing;
using System.Xml.Serialization;

namespace QS._core_e_.Data
{
    [QS._core_c_.Serialization.BLOB]
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_Data1D)]
    [System.Serializable]
    public class Data1D : IData, QS.Fx.Serialization.ISerializable, IDataSet, QS._core_c_.Diagnostics.IDataCollector
    {
        public Data1D()
        {
        }

        public Data1D(DataSeries dataseries) : this(string.Empty, dataseries)
        {
        }

        public Data1D(string name, DataSeries dataseries) : this(name, 
            new Axis(string.Empty, string.Empty, new Range(dataseries.Range.P1.X, dataseries.Range.P2.X), string.Empty),
            new Axis(string.Empty, string.Empty, new Range(dataseries.Range.P1.Y, dataseries.Range.P2.Y), string.Empty),
            dataseries.Data, string.Empty)
        {
        }

        public Data1D(string name, double[] data, string description,
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            this.name = name;
            this.description = description;
            this.data = (data != null) ? data : new double[0];

            double min_y = double.MaxValue;
            double max_y = double.MinValue;
            foreach (double v in data)
                if (v < min_y)
                    min_y = v;
                else if (v > max_y)
                    max_y = v;

            this.xAxis = new Axis(x_name, x_units, new Range(0, data.Length - 1), x_description);
            this.yAxis = new Axis(y_name, y_units, new Range(min_y, max_y), y_description);
        }

        public Data1D(string name, IAxis xAxis, IAxis yAxis, double[] data, string description)
        {
            this.name = name;
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.data = (data != null) ? data : new double[0]; 
            this.description = description;
        }

        private IAxis xAxis, yAxis;
        private double[] data;
        private string name, description;

        public double[] Data
        {
            get { return data; }
        }

        #region IData Members

        private void CalculateIndices(Range xrange, out int ind1, out int ind2)
        {
            if (xAxis.Range.Maximum == xAxis.Range.Minimum)
            {
                if (xrange.Minimum != xAxis.Range.Minimum || xrange.Maximum != xAxis.Range.Minimum)
                    throw new Exception("xAxis is empty but view xRange is not");

                ind1 = 0;
                ind2 = data.Length - 1;
            }
            else
            {
                ind1 = Math.Max(0, (int)Math.Floor(((double)(data.Length - 1)) *
                    (xrange.Minimum - xAxis.Range.Minimum) / (xAxis.Range.Maximum - xAxis.Range.Minimum)));
                ind2 = Math.Min(data.Length - 1, (int)Math.Ceiling(((double)(data.Length - 1)) *
                    (xrange.Maximum - xAxis.Range.Minimum) / (xAxis.Range.Maximum - xAxis.Range.Minimum)));
            }
        }

        void IData.Draw(Graphics graphics, RectangleF region, IView view, IDrawingContext drawingContext)
        {
            Pen pen1 = new Pen(drawingContext.ConnectionsColor);
            Pen pen2 = new Pen(drawingContext.DataColor);

            int ind1, ind2;
            CalculateIndices(view.XRange, out ind1, out ind2);

            int last_xcoord = -1, last_ycoord = -1, min_ycoord = -1, max_ycoord = -1;
            bool connecting = false;
            bool[] marked = new bool[(int) region.Height + 1];

            for (int ind = ind1; ind <= ind2; ind++)
            {
                double x_value = xAxis.Range.Minimum +
                    (((double)ind) / ((double)(data.Length - 1))) * (xAxis.Range.Maximum - xAxis.Range.Minimum);
                double y_value = data[ind];

                if (double.IsNaN(y_value))
                    connecting = false;
                else
                {
                    int x_coord = (int)Math.Floor(region.Left + view.XScale.ValueToCoordinate(view.XRange, x_value) * (region.Width - 1));
                    int y_coord = (int)Math.Round(region.Bottom - view.YScale.ValueToCoordinate(view.YRange, y_value) * (region.Height - 1));

                    if (x_coord != last_xcoord)
                    {
                        if (connecting)
                            Drawing.DrawLine(graphics, pen1, last_xcoord, last_ycoord, x_coord, y_coord);

                        if (drawingContext.ConnectingLines)
                            Drawing.DrawLine(graphics, pen1, last_xcoord, min_ycoord, last_xcoord, max_ycoord);

                        if (last_xcoord >= 0)
                            for (int i = 0; i < marked.Length; i++)
                                if (marked[i])
                                    Drawing.DrawEllipse(graphics, pen2, last_xcoord, region.Top + i, 2, 2);

                        last_xcoord = x_coord;
                        for (int i = 0; i < marked.Length; i++)
                            marked[i] = false;
                        min_ycoord = max_ycoord = y_coord;
                    }

                    last_ycoord = y_coord;
                    int position = (int)(y_coord - region.Top);
                    if (position >= 0 && position < marked.Length)
                        marked[position] = true;
                    if (y_coord < min_ycoord)
                        min_ycoord = y_coord;
                    if (y_coord > max_ycoord)
                        max_ycoord = y_coord;

                    connecting = drawingContext.ConnectingLines;
                }
            }

            graphics.DrawLine(pen1, last_xcoord, min_ycoord, last_xcoord, max_ycoord);
            for (int i = 0; i < marked.Length; i++)
                if (marked[i])
                    graphics.DrawEllipse(pen2, last_xcoord, region.Top + i, 2, 2);
        }

        string IData.Name
        {
            get { return name; }
        }

        string IData.Description
        {
            get { return description; }
        }

        IAxis IData.XAxis
        {
            get { return xAxis; }
        }

        IAxis IData.YAxis
        {
            get { return yAxis; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = 2 * sizeof(ushort) + sizeof(int) + data.Length * sizeof(double);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_Data1D, size, size, 0);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(name)).SerializableInfo);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(description)).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            (new QS._core_c_.Base2.StringWrapper(name)).SerializeTo(ref header, ref data);
            (new QS._core_c_.Base2.StringWrapper(description)).SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*)pheader) = this.data.Length;
                pheader += sizeof(int);
                for (int ind = 0; ind < this.data.Length; ind++)
                {
                    *((double*)pheader) = this.data[ind];
                    pheader += sizeof(double);
                }
                *((ushort*)pheader) = ((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo.ClassID;
                *((ushort*)(pheader + sizeof(ushort))) = ((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo.ClassID;
            }
            header.consume(2 * sizeof(ushort) + sizeof(int) + this.data.Length * sizeof(double));
            ((QS.Fx.Serialization.ISerializable)xAxis).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)yAxis).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            QS._core_c_.Base2.StringWrapper wrapper1 = new QS._core_c_.Base2.StringWrapper();
            wrapper1.DeserializeFrom(ref header, ref data);
            name = wrapper1.String;
            QS._core_c_.Base2.StringWrapper wrapper2 = new QS._core_c_.Base2.StringWrapper();
            wrapper2.DeserializeFrom(ref header, ref data);
            description = wrapper2.String;
            ushort classIDx, classIDy;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                int count = (int)(*((int*)pheader));
                pheader += sizeof(int);
                this.data = new double[count];
                for (int ind = 0; ind < count; ind++)
                {
                    this.data[ind] = *((double*)pheader);
                    pheader += sizeof(double);
                }
                classIDx = *((ushort*)pheader);
                classIDy = *((ushort*)(pheader + sizeof(ushort)));
            }
            header.consume(2 * sizeof(ushort) + sizeof(int) + this.data.Length * sizeof(double));
            QS.Fx.Serialization.ISerializable xAxis = (QS.Fx.Serialization.ISerializable)QS._core_c_.Base3.Serializer.CreateObject(classIDx);
            QS.Fx.Serialization.ISerializable yAxis = (QS.Fx.Serialization.ISerializable)QS._core_c_.Base3.Serializer.CreateObject(classIDx);
            xAxis.DeserializeFrom(ref header, ref data);
            yAxis.DeserializeFrom(ref header, ref data);
            this.xAxis = (IAxis)xAxis;
            this.yAxis = (IAxis)yAxis;
        }

        #endregion

        #region IDataSet Members

        IDataSet IDataSet.downsample(Size targetResolution)
        {
            return this;
        }

        void IDataSet.draw(Graphics graphics)
        {
            graphics.Clear(Color.White);
            RectangleF rec = graphics.VisibleClipBounds;
            IView view = new View((IData) this);
            view.Draw(graphics, rec);
            ((IData)this).Draw(graphics, rec, view, new DrawingContext());
        }

        Rectangle IDataSet.Range
        {
            get
            {
                return new Rectangle(
                    new Point(((IData)this).XAxis.Range.Minimum, ((IData)this).YAxis.Range.Minimum),
                    new Point(((IData)this).XAxis.Range.Maximum, ((IData)this).YAxis.Range.Maximum));
            }
        }

        #endregion

        public override string ToString()
        {
            return "Data1D(" + data.Length.ToString() + " values)"; 
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

        [XmlIgnore]
        [DataSource("AsDataSeries")]
        public IDataSet AsDataSeries
        {
            get { return new DataSeries(data); }
        }

        IData IData.Crop(IView view)
        {
            List<XY> samples = new List<XY>();

            int ind1, ind2;
            CalculateIndices(view.XRange, out ind1, out ind2);
            for (int ind = ind1; ind <= ind2; ind++)
            {
                double x_value = xAxis.Range.Minimum +
                    (((double)ind) / ((double)(data.Length - 1))) * (xAxis.Range.Maximum - xAxis.Range.Minimum);
                double y_value = data[ind];

                if (view.YRange.Contains(y_value))
                    samples.Add(new XY(x_value, y_value));
            }

            return new Data2D(samples.ToArray());
        }

        IEnumerable<KeyValuePair<string, double>> IData.GetStatistics()
        {
            Dictionary<string, double> statistics = new Dictionary<string, double>();
            statistics.Add("Number of Values", data.Length);
            statistics.Add("Minimum", yAxis.Range.Minimum);
            statistics.Add("Maximum", yAxis.Range.Maximum);
            statistics.Add("Span", yAxis.Range.Maximum - yAxis.Range.Minimum);
            double mean, error;
            QS._core_e_.MyMath.ConfidenceIntervals.Calculate95(data, out mean, out error);
            statistics.Add("Average", mean);
            statistics.Add("Error (delta +/-  for 95% confidence)", error);
            double[] sorted_data = new double[data.Length];
            Array.Copy(data, sorted_data, data.Length);
            Array.Sort<double>(sorted_data);
            statistics.Add("Median",
                ((data.Length % 2) == 0) ? ((sorted_data[data.Length / 2 - 1] + sorted_data[data.Length / 2]) / 2) : sorted_data[data.Length / 2 + 1]);
            return statistics;
        }
    }
}
