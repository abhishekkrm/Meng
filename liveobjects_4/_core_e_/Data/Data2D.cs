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
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_Data2D)]
    [System.Serializable]
    public class Data2D : IData, QS.Fx.Serialization.ISerializable, IDataSet, QS._core_c_.Diagnostics.IDataCollector
    {
        public Data2D()
        {
        }

        public Data2D(XYSeries series) : this(series.Data)
        {
        }

        public Data2D(XY[] data) : this(string.Empty, data)
        {
        }

        public Data2D(string name, XY[] data) : this(name, data, string.Empty, string.Empty)
        {
        }

        public Data2D(string name, XY[] data, string x_name, string y_name)            
            : this(name, data, string.Empty, x_name, string.Empty, string.Empty, y_name, string.Empty, string.Empty)
        {
        }

        public Data2D(string name, XY[] data, string description, 
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            this.name = name;
            this.data = (data != null) ? data : new XY[0];
            this.description = description;

            System.Array.Sort<XY>(data, XYSeries.xComparer);

            double min_x = double.MaxValue;
            double max_x = double.MinValue;
            double min_y = double.MaxValue;
            double max_y = double.MinValue;

            for (int ind = 0; ind < data.Length; ind++)
            {
                if (data[ind].x < min_x)
                    min_x = data[ind].x;
                if (data[ind].x > max_x)
                    max_x = data[ind].x;
                if (data[ind].y < min_y)
                    min_y = data[ind].y;
                if (data[ind].y > max_y)
                    max_y = data[ind].y;
            }

            this.xAxis = new Axis(x_name, x_units, new Range(min_x, max_x), x_description);
            this.yAxis = new Axis(y_name, y_units, new Range(min_y, max_y), y_description);
        }

        private IAxis xAxis, yAxis;
        private XY[] data;
        private string name, description;

        public XY[] Data
        {
            get { return data; }
        }

/*
        private int ComparerXY(XY a, XY b)
        {
            int result = a.x.CompareTo(b.x);
            return (result != 0) ? result : a.y.CompareTo(b.y);
        }
*/

        #region IData Members

        void IData.Draw(Graphics graphics, RectangleF region, IView view, IDrawingContext drawingContext)
        {
            try
            {
                Pen pen1 = new Pen(drawingContext.ConnectionsColor);
                Pen pen2 = new Pen(drawingContext.DataColor);

                int ind1 = Array.BinarySearch<XY>(data, new XY(view.XRange.Minimum, double.NegativeInfinity), XYSeries.xComparer);
                if (ind1 < 0)
                    ind1 = - (ind1 + 1);
                if (ind1 >= data.Length)
                    return;

                int ind2 = Array.BinarySearch<XY>(data, new XY(view.XRange.Maximum, double.PositiveInfinity), XYSeries.xComparer);
                if (ind2 < 0)
                    ind2 = -ind2;
                if (ind2 >= data.Length)
                    ind2 = data.Length - 1;

                int last_xcoord = -1, last_ycoord = -1, min_ycoord = -1, max_ycoord = -1;
                bool connecting = false;
                bool[] marked = new bool[(int)region.Height + 1];

                for (int ind = ind1; ind <= ind2; ind++)
                {
                    double x_value = data[ind].x;
                    double y_value = data[ind].y;

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


                Drawing.DrawLine(graphics, pen1, last_xcoord, min_ycoord, last_xcoord, max_ycoord);

                for (int i = 0; i < marked.Length; i++)
                    if (marked[i])
                        Drawing.DrawEllipse(graphics, pen2, last_xcoord, region.Top + i, 2, 2);
            }
            catch (Exception exc)
            {
                throw new Exception("Could not draw.", exc);
            }
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
                int size = 2 * sizeof(ushort) + sizeof(int) + data.Length * 2 * sizeof(double);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_Data2D, size, size, 0);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(name)).SerializableInfo);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(description)).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
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
                    *((double*)pheader) = this.data[ind].x;
                    *((double*)(pheader + sizeof(double))) = this.data[ind].y;
                    pheader += 2 * sizeof(double);
                }
                *((ushort*)pheader) = ((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo.ClassID;
                *((ushort*)(pheader + sizeof(ushort))) = ((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo.ClassID;
            }
            header.consume(2 * sizeof(ushort) + sizeof(int) + this.data.Length * 2 * sizeof(double));
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
                this.data = new XY[count];
                for (int ind = 0; ind < count; ind++)
                {
                    this.data[ind] = new XY(*((double*)pheader), *((double*)(pheader + sizeof(double))));
                    pheader += 2 * sizeof(double);
                }
                classIDx = *((ushort*)pheader);
                classIDy = *((ushort*)(pheader + sizeof(ushort)));
            }
            header.consume(2 * sizeof(ushort) + sizeof(int) + this.data.Length * 2 * sizeof(double));
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
            ((IData) this).Draw(graphics, rec, view, new DrawingContext());          
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
            return "Data2D(" + data.Length.ToString() + " values)";
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
        [DataSource("AsXYSeries")]
        public IDataSet AsXYSeries
        {
            get { return new XYSeries(data); }
        }

        IData IData.Crop(IView view)
        {
            List<XY> samples = new List<XY>();

            for (int ind = 0; ind < data.Length; ind++)
            {
                if (view.XRange.Contains(data[ind].x) && view.YRange.Contains(data[ind].y))
                    samples.Add(data[ind]);
            }

            return new Data2D(samples.ToArray());
        }

        IEnumerable<KeyValuePair<string, double>> IData.GetStatistics()
        {
            Dictionary<string, double> statistics = new Dictionary<string, double>();
            statistics.Add("Number of Values", data.Length);
            double meanx, errorx, meany, errory;
            QS._core_e_.MyMath.ConfidenceIntervals.Calculate95XY(data, out meanx, out errorx, out meany, out errory);
            double[] xs, ys;
            xs = new double[data.Length];
            ys = new double[data.Length];
            for (int ind = 0; ind < data.Length; ind++)
            {
                xs[ind] = data[ind].x;
                ys[ind] = data[ind].y;
            }
            Array.Sort<double>(xs);
            Array.Sort<double>(ys);
            statistics.Add("Minimum X", xAxis.Range.Minimum);
            statistics.Add("Maximum X", xAxis.Range.Maximum);
            statistics.Add("Span of X", xAxis.Range.Maximum - xAxis.Range.Minimum);
            statistics.Add("Average X", meanx);
            statistics.Add("Error X (delta +/-  for 95% confidence)", errorx);
            statistics.Add("Median X",
                ((data.Length % 2) == 0) ? ((xs[data.Length / 2 - 1] + xs[data.Length / 2]) / 2) : xs[data.Length / 2 + 1]);
            statistics.Add("Minimum Y", yAxis.Range.Minimum);
            statistics.Add("Maximum Y", yAxis.Range.Maximum);
            statistics.Add("Span of Y", yAxis.Range.Maximum - yAxis.Range.Minimum);            
            statistics.Add("Average Y", meany);
            statistics.Add("Error Y (delta +/-  for 95% confidence)", errory);
            statistics.Add("Median Y",
                ((data.Length % 2) == 0) ? ((ys[data.Length / 2 - 1] + ys[data.Length / 2]) / 2) : ys[data.Length / 2 + 1]);
            return statistics;
        }
    }
}
