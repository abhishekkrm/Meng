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

namespace QS._core_e_.Data
{
    [QS._core_c_.Serialization.BLOB]
    [QS.Fx.Serialization.ClassID(QS.ClassID.TMS_Data_DataCo)]
    public class DataCo : IData, IDataSet, QS.Fx.Serialization.ISerializable, QS._core_c_.Diagnostics.IDataCollector
    {
        public DataCo() : this(string.Empty, string.Empty)
        {
        }

        public DataCo(string name, string description) 
            : this(name, description, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public DataCo(string name, string description, 
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            this.name = name;
            this.description = description;
            xAxis = new Axis(x_name, x_units, new Range(), x_description);
            yAxis = new Axis(y_name, y_units, new Range(), y_description);
        }

        private IDictionary<string, IData> dataCollection = new Dictionary<string, IData>();
        private IAxis xAxis, yAxis;
        private string name, description;

        public void Add(IData data)
        {
            if (dataCollection.Count > 0)
            {
                xAxis.Range = new Range(Math.Min(xAxis.Range.Minimum, data.XAxis.Range.Minimum),
                    Math.Max(xAxis.Range.Maximum, data.XAxis.Range.Maximum));
                yAxis.Range = new Range(Math.Min(yAxis.Range.Minimum, data.YAxis.Range.Minimum),
                    Math.Max(yAxis.Range.Maximum, data.YAxis.Range.Maximum));
            }
            else
            {
                xAxis.Range = data.XAxis.Range;
                yAxis.Range = data.YAxis.Range;
            }

            dataCollection.Add(data.Name, data);
        }

        public IEnumerable<IData> Components
        {
            get { return dataCollection.Values; }
        }

        #region IData Members

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

        void IData.Draw(Graphics graphics, RectangleF region, IView view, IDrawingContext drawingContext)
        {
            int seriesIndex = 0;
            int consumed_v = 0;
            foreach (IData data in dataCollection.Values)
            {
                if (seriesIndex > 0)
                    drawingContext.ChangeColors();

                data.Draw(graphics, region, view, drawingContext);

                Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                SizeF sss = graphics.MeasureString(data.Name, font);

                Pen pen1 = new Pen(drawingContext.DataColor, 4);
                Pen pen2 = new Pen(drawingContext.ConnectionsColor, 4);
                graphics.DrawLine(pen1, (region.Left + region.Right - sss.Width - 50) / 2, region.Top + consumed_v + sss.Height / 2,
                    (region.Left + region.Right - sss.Width) / 2, consumed_v + sss.Height / 2);
                graphics.DrawLine(pen2, (region.Left + region.Right - sss.Width) / 2, region.Top + consumed_v + sss.Height / 2,
                    (region.Left + region.Right - sss.Width + 50) / 2, consumed_v + sss.Height / 2);
                graphics.DrawString(data.Name, font, Brushes.Black, (region.Left + region.Right - sss.Width + 50) / 2, region.Top + consumed_v);
                consumed_v += (int)sss.Height;

                seriesIndex++;
            }
        }

        #endregion

        #region IDataSet Members

        IDataSet IDataSet.downsample(System.Drawing.Size targetResolution)
        {
            return this;
        }

        void IDataSet.draw(System.Drawing.Graphics graphics)
        {
            ((IData)this).Draw(graphics, graphics.VisibleClipBounds, new View((IData) this), new DrawingContext());
        }

        Rectangle IDataSet.Range
        {
            get 
            {
                return new Rectangle(new Point(xAxis.Range.Minimum, yAxis.Range.Minimum), 
                    new Point(xAxis.Range.Maximum, yAxis.Range.Maximum));
            }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = 2 * sizeof(ushort) + sizeof(int) + dataCollection.Count * sizeof(ushort);
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_DataCo, size, size, 0);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(name)).SerializableInfo);
                info.AddAnother((new QS._core_c_.Base2.StringWrapper(description)).SerializableInfo);
                foreach (KeyValuePair<string, IData> element in dataCollection)
                {
                    info.AddAnother((new QS._core_c_.Base2.StringWrapper(element.Key)).SerializableInfo);
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element.Value).SerializableInfo);
                }
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
                *((int*)pheader) = dataCollection.Count;
                *((ushort*)(pheader + sizeof(int))) = ((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo.ClassID;
                *((ushort*)(pheader + sizeof(int) + sizeof(ushort))) = ((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo.ClassID;
            }
            header.consume(2 * sizeof(ushort) + sizeof(int));
            ((QS.Fx.Serialization.ISerializable)xAxis).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)yAxis).SerializeTo(ref header, ref data);
            foreach (KeyValuePair<string, IData> element in dataCollection)
            {
                (new QS._core_c_.Base2.StringWrapper(element.Key)).SerializeTo(ref header, ref data);
                fixed (byte* pbuffer = header.Array)
                {
                    *((ushort*)(pbuffer + header.Offset)) = ((QS.Fx.Serialization.ISerializable) element.Value).SerializableInfo.ClassID;
                }
                header.consume(sizeof(ushort));
                ((QS.Fx.Serialization.ISerializable)element.Value).SerializeTo(ref header, ref data);
            }
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
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                count = (int)(*((int*)pheader));
                classIDx = *((ushort*)(pheader + sizeof(int)));
                classIDy = *((ushort*)(pheader + sizeof(int) + sizeof(ushort)));
            }
            header.consume(2 * sizeof(ushort) + sizeof(int));
            QS.Fx.Serialization.ISerializable xAxis = (QS.Fx.Serialization.ISerializable)QS._core_c_.Base3.Serializer.CreateObject(classIDx);
            QS.Fx.Serialization.ISerializable yAxis = (QS.Fx.Serialization.ISerializable)QS._core_c_.Base3.Serializer.CreateObject(classIDx);
            xAxis.DeserializeFrom(ref header, ref data);
            yAxis.DeserializeFrom(ref header, ref data);
            this.xAxis = (IAxis)xAxis;
            this.yAxis = (IAxis)yAxis;
            dataCollection = new Dictionary<string, IData>(count);
            while (count-- > 0)
            {
                QS._core_c_.Base2.StringWrapper wrapper = new QS._core_c_.Base2.StringWrapper();
                wrapper.DeserializeFrom(ref header, ref data);
                ushort classID;
                fixed (byte* pbuffer = header.Array)
                {
                    classID = *((ushort*)(pbuffer + header.Offset));
                }
                header.consume(sizeof(ushort));
                QS.Fx.Serialization.ISerializable dataObject = (QS.Fx.Serialization.ISerializable)QS._core_c_.Base3.Serializer.CreateObject(classID);
                dataObject.DeserializeFrom(ref header, ref data);
                dataCollection.Add(wrapper.String, (QS._core_e_.Data.IData) dataObject);
            }
        }

        #endregion

        public override string ToString()
        {
            return "DataCo(\"" + ((name != null) ? name : "") + "\", " + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(dataCollection.Keys, ", ") + ")";
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

        IData IData.Crop(IView view)
        {
            throw new NotImplementedException();
        }

        IEnumerable<KeyValuePair<string, double>> IData.GetStatistics()
        {
            throw new NotImplementedException();
        }
    }
}
