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
    public partial class DataWindow : UserControl, QS.GUI.Components.IDataWindow
    {
        public DataWindow()
        {
            InitializeComponent();
        }

        private QS._core_e_.Data.IDataSet dataSet;
        private bool mouse_pressed = false;
        private Graphics graphics = null;
        private Point last_position;
        private double started_xcoord, started_ycoord;
        private QS._core_e_.Data.Rectangle selection;
        private event EventHandler selectionChanged;

        #region IDataWindow Members

        QS._core_e_.Data.IDataSet IDataWindow.Data
        {
            get { return dataSet; }
            set 
            { 
                dataSet = value;
                this.Refresh();
            }
        }

        QS._core_e_.Data.Rectangle IDataWindow.Selection
        {
            get { return selection; }
        }

        event EventHandler IDataWindow.SelectionChanged
        {
            add { selectionChanged += value; }
            remove { selectionChanged -= value; }
        }

        #endregion

        #region Paint

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (dataSet != null)
                {
                    dataSet.downsample(e.Graphics.VisibleClipBounds.Size.ToSize()).draw(e.Graphics);
                    data_rectangle = dataSet.Range;
                }
                else
                {
                    e.Graphics.Clear(Color.Ivory);
                }
            }
            catch (Exception exc)
            {
                try
                {
                    e.Graphics.Clear(Color.Ivory);
                    e.Graphics.DrawString(exc.ToString(), 
                        new Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Red, 20, 20,
                        StringFormat.GenericDefault);
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region Cross-Pointer in the Graphics Window

        QS._core_e_.Data.Rectangle data_rectangle = QS._core_e_.Data.Rectangle.Zero;

        private void calculateCoordinates(Point p, RectangleF bounds, out double xcoord, out double ycoord)
        {
            double min_x, min_y, max_x, max_y;

            min_x = data_rectangle.P1.X;
            max_x = data_rectangle.P2.X;
            min_y = data_rectangle.P1.Y;
            max_y = data_rectangle.P2.Y;

            xcoord = min_x + (max_x - min_x) * (p.X - bounds.Left) / ((double)bounds.Width);
            ycoord = min_y + (max_y - min_y) * (bounds.Bottom - p.Y) / ((double)bounds.Height);
        }

//        private string coordinateString(Point p, RectangleF bounds)
//        {
//            double xcoord, ycoord;
//            calculateCoordinates(p, bounds, out xcoord, out ycoord);
//            return "X = " + xcoord.ToString() + ", Y = " + ycoord.ToString();
//        }

        private void updateStatistics(double x1, double y1, double x2, double y2)
        {
            selection = new QS._core_e_.Data.Rectangle(new QS._core_e_.Data.Point(x1, y1), new QS._core_e_.Data.Point(x2, y2));
            if (selectionChanged != null)
                selectionChanged(this, null);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
			if (!mouse_pressed)
			{
				mouse_pressed = true;
				graphics = panel1.CreateGraphics();

				RectangleF bounds = graphics.VisibleClipBounds;
				Point b1 = panel1.PointToScreen(new Point((int)bounds.Left, (int)bounds.Top));
				Point b2 = panel1.PointToScreen(new Point((int)bounds.Right, (int)bounds.Bottom));

				last_position = new Point(e.X, e.Y);

				int x_coordinate = last_position.X + b1.X;
				int y_coordinate = last_position.Y + b1.Y;
				ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
				ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

				calculateCoordinates(last_position, bounds, out started_xcoord, out started_ycoord);
				updateStatistics(started_xcoord, started_ycoord, started_xcoord, started_ycoord);
			}
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
			if (mouse_pressed)
			{
				RectangleF bounds = graphics.VisibleClipBounds;
				Point b1 = panel1.PointToScreen(new Point((int)bounds.Left, (int)bounds.Top));
				Point b2 = panel1.PointToScreen(new Point((int)bounds.Right, (int)bounds.Bottom));

				int x_coordinate = last_position.X + b1.X;
				int y_coordinate = last_position.Y + b1.Y;
				ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
				ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

				mouse_pressed = false;
				graphics = null;
			}
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
			if (mouse_pressed)
			{
				RectangleF bounds = graphics.VisibleClipBounds;
				Point b1 = panel1.PointToScreen(new Point((int)bounds.Left, (int)bounds.Top));
				Point b2 = panel1.PointToScreen(new Point((int)bounds.Right, (int)bounds.Bottom));

				int x_coordinate = last_position.X + b1.X;
				int y_coordinate = last_position.Y + b1.Y;
				ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
				ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

				last_position = new Point(e.X, e.Y);

				if (bounds.Contains((float)e.X, (float)e.Y))
				{
					x_coordinate = last_position.X + b1.X;
					y_coordinate = last_position.Y + b1.Y;
					ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
					ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

					double xcoord, ycoord;
					calculateCoordinates(last_position, bounds, out xcoord, out ycoord);

					updateStatistics(started_xcoord, started_ycoord, xcoord, ycoord);
				}
				else
				{
					mouse_pressed = false;
					graphics = null;
				}
			}
        }

        #endregion
    }
}
