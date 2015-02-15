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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QS.GUI.Components
{
    public partial class EnhancedDataWindowWithControls : UserControl, IEnhancedDataWindowWithControls, IDataWindow
    {
        public EnhancedDataWindowWithControls()
        {
            InitializeComponent();

            foreach (QS._core_e_.Data.IScale scale in QS._core_e_.Data.Scale.Scales)
            {
                toolStripComboBox1.Items.Add(scale);
                toolStripComboBox2.Items.Add(scale);
            }
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;

            ((IEnhancedDataWindow)enhancedDataWindow1).OnChange += new EventHandler(EnhancedDataWindowWithControls_OnChange);

            foreach (SelectableArea.SelectionMode mode in Enum.GetValues(typeof(SelectableArea.SelectionMode)))
                toolStripComboBox3.Items.Add(mode);
            toolStripComboBox3.SelectedIndex = 0;
        }

        private QS._core_e_.Data.Point position;
        private QS._core_e_.Data.Rectangle selection;
        private QS._core_e_.Data.IData data;
        private QS._core_e_.Data.IDataSet dataSet;

        #region Processing Selection Changes

        private void EnhancedDataWindowWithControls_OnChange(object sender, EventArgs e)
        {
            toolStripButton1.Enabled = ((IEnhancedDataWindow)enhancedDataWindow1).Selected;

            if (((IEnhancedDataWindow)enhancedDataWindow1).Selected)
            {
                QS._core_e_.Data.IView view = ((IEnhancedDataWindow)enhancedDataWindow1).View;
                QS._core_e_.Data.Rectangle coordinates = ((IEnhancedDataWindow)enhancedDataWindow1).Selection;
                double x1 = view.XScale.CoordinateToValue(view.XRange, coordinates.P1.X);
                double x2 = view.XScale.CoordinateToValue(view.XRange, coordinates.P2.X);
                double y1 = view.YScale.CoordinateToValue(view.YRange, coordinates.P1.Y);
                double y2 = view.YScale.CoordinateToValue(view.YRange, coordinates.P2.Y);
                selection = new QS._core_e_.Data.Rectangle(
                    new QS._core_e_.Data.Point(Math.Min(x1, x2), Math.Min(y1, y2)), new QS._core_e_.Data.Point(Math.Max(x1, x2), Math.Max(y1, y2)));

                toolStripStatusLabel3.Text = "Selection: [" + Double2String(selection.P1.X) + ", " + Double2String(selection.P2.X) +
                    "] x [" + Double2String(selection.P1.Y) + ", " + Double2String(selection.P2.Y) + "]";
                toolStripStatusLabel4.Text =
                    "Size: (" + Double2String(selection.P2.X - selection.P1.X) + ", " + Double2String(selection.P2.Y - selection.P1.Y) + ")";
            }
            else
            {
                QS._core_e_.Data.IView view = ((IEnhancedDataWindow)enhancedDataWindow1).View;
                if (view != null)
                {
                    QS._core_e_.Data.Point coordinates = ((IEnhancedDataWindow)enhancedDataWindow1).Position;
                    position = new QS._core_e_.Data.Point(view.XScale.CoordinateToValue(view.XRange, coordinates.X),
                        view.YScale.CoordinateToValue(view.YRange, coordinates.Y));
                    toolStripStatusLabel3.Text = "Position: (" + Double2String(position.X) + ", " + Double2String(position.Y) + ")";
                }
                toolStripStatusLabel4.Text = string.Empty;
            }
        }

        #endregion

        #region Managing Views

        private void DataChanged()
        {
            ((IEnhancedDataWindow)enhancedDataWindow1).Data = data;
            if (data != null)
            {
                toolStripStatusLabel5.Text = data.ToString();

                label1.Text = data.Name +
                    " (X = " + data.XAxis.Name + " [" + data.XAxis.Units + "], Y = " + data.YAxis.Name + " [" + data.YAxis.Units + "])";
                toolTip1.SetToolTip(label1, data.Description + "\nX: " + data.XAxis.Description + "\nY: " + data.YAxis.Description);
                toolStripStatusLabel1.Text = "Data: [" +
                    Double2String(data.XAxis.Range.Minimum) + ", " + Double2String(data.XAxis.Range.Maximum) +
                    "] x [" + Double2String(data.YAxis.Range.Minimum) + ", " + Double2String(data.YAxis.Range.Maximum) + "]";
                regions.Clear();
                selection = new QS._core_e_.Data.Rectangle(new QS._core_e_.Data.Point(data.XAxis.Range.Minimum, data.YAxis.Range.Minimum),
                    new QS._core_e_.Data.Point(data.XAxis.Range.Maximum, data.YAxis.Range.Maximum));
                ZoomIn();
            }
            else
                toolStripStatusLabel5.Text = "(no data)";
        }

        private void Shift(double dx, double dy)
        {
            selection = new QS._core_e_.Data.Rectangle(
                new QS._core_e_.Data.Point(selection.P1.X + dx, selection.P1.Y + dy),
                new QS._core_e_.Data.Point(selection.P2.X + dx, selection.P2.Y + dy));
            regions.Pop();
            regions.Push(selection);
            AdjustView();
        }

        private void ZoomIn()
        {
            regions.Push(selection);
            if (regions.Count > 1)
                toolStripButton2.Enabled = true;
            AdjustView();
        }

        private void ZoomOut()
        {
            if (regions.Count > 1)
            {
                regions.Pop();
                if (regions.Count < 2)
                    toolStripButton2.Enabled = false;
                AdjustView();
            }
        }

        private void AdjustView()
        {
            try
            {
                QS._core_e_.Data.Rectangle rectangle = regions.Peek();
                toolStripStatusLabel2.Text = "View: [" + Double2String(rectangle.P1.X) + ", " + Double2String(rectangle.P2.X) +
                    "] x [" + Double2String(rectangle.P1.Y) + ", " + Double2String(rectangle.P2.Y) + "]";
                QS._core_e_.Data.IScale xScale = (QS._core_e_.Data.IScale)toolStripComboBox1.SelectedItem;
                QS._core_e_.Data.IScale yScale = (QS._core_e_.Data.IScale)toolStripComboBox2.SelectedItem;
                if (xScale != null && yScale != null)
                {
                    QS._core_e_.Data.View view = new QS._core_e_.Data.View(
                        new QS._core_e_.Data.Range(rectangle.P1.X, rectangle.P2.X), new QS._core_e_.Data.Range(rectangle.P1.Y, rectangle.P2.Y), xScale, yScale);
                    ((IEnhancedDataWindow)enhancedDataWindow1).View = view;
                }
            }
            catch (Exception)
            {
            }
        }

        private Stack<QS._core_e_.Data.Rectangle> regions = new Stack<QS._core_e_.Data.Rectangle>();

        private const int accuracy = 6;
        private string Double2String(double value)
        {
            return ((value != 0) ? Math.Round(value, Math.Min(Math.Max(accuracy - 1 - (int) Math.Floor(Math.Log10(value)), 0), 15)) : 0).ToString();
        }

        #endregion

        #region IEnhancedDataWindowWithControls Members

        QS._core_e_.Data.IData IEnhancedDataWindowWithControls.Data
        {
            get { return data; }
            set
            {
                data = value;
                try
                {
                    dataSet = QS._qss_e_.Data_.Convert.ToDataSeries(data);
                }
                catch (Exception)
                {
                    dataSet = null;
                }

                DataChanged();
            }
        }

        QS._core_e_.Data.IView IEnhancedDataWindowWithControls.View
        {
            get { return ((IEnhancedDataWindow)enhancedDataWindow1).View; }
        }

        #endregion

        #region Clicking Around

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (((IEnhancedDataWindow)enhancedDataWindow1).Selected)
                ZoomIn();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustView();
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AdjustView();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void toolStripComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            enhancedDataWindow1.SelectionMode = (SelectableArea.SelectionMode)toolStripComboBox3.SelectedItem;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            double shift = Math.Min((selection.P2.X - selection.P1.X) / 10, selection.P1.X - data.XAxis.Range.Minimum);
            if (shift > 0)
                Shift(-shift, 0);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            double shift = Math.Min((selection.P2.X - selection.P1.X) / 10, data.XAxis.Range.Maximum - selection.P2.X);
            if (shift > 0)
                Shift(shift, 0);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            double shift = Math.Min((selection.P2.Y - selection.P1.Y) / 10, data.YAxis.Range.Maximum - selection.P2.Y);
            if (shift > 0)
                Shift(0, shift);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            double shift = Math.Min((selection.P2.Y - selection.P1.Y) / 10, selection.P1.Y - data.YAxis.Range.Minimum);
            if (shift > 0)
                Shift(0, -shift);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            enhancedDataWindow1.ConnectingLines = !enhancedDataWindow1.ConnectingLines;
            AdjustView();
        }

        #endregion

        #region IDataWindow Members

        QS._core_e_.Data.IDataSet IDataWindow.Data
        {
            get { return dataSet; }
            set 
            { 
                data = QS._qss_e_.Data_.Convert.ToData(dataSet = value);
                DataChanged();
            }
        }

        QS._core_e_.Data.Rectangle IDataWindow.Selection
        {
            get { throw new NotSupportedException(); }
        }

        event EventHandler IDataWindow.SelectionChanged
        {
            add { throw new NotSupportedException(); }
            remove { throw new NotSupportedException(); }
        }

        #endregion

        private void redrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {            
            QS._core_e_.Data.IData cropped = 
                ((IEnhancedDataWindow) enhancedDataWindow1).Data.Crop(
                    ((IEnhancedDataWindow) enhancedDataWindow1).View);

            (new StatisticsForm(cropped)).ShowDialog();
        }
    }
}
