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
    public partial class SelectableArea : UserControl, ISelectableArea
    {
        public enum SelectionMode
        {
            XY, X, Y
        }

        public SelectionMode Mode
        {
            get { return selectionMode; }
            set { selectionMode = value; }
        }

        public SelectableArea()
        {
            InitializeComponent();
        }

        private enum InternalMode
        {
            None, Pointing, Selecting, Selected
        }

        private InternalMode internalMode;
        private SelectionMode selectionMode = SelectionMode.XY;
        private Graphics graphics;
        private RectangleF bounds;
        private Point b1, b2, last_position, starting_position;
        private double starting_xcoord, starting_ycoord, current_xcoord, current_ycoord;
        private event EventHandler onChange;
        private QS._qss_e_.Data_.DrawCallback drawCallback;

        #region Handling the Mouse Movement

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            switch (internalMode)
            {
                case InternalMode.Selected:
                    {
                    }
                    break;

                default:
                    {
                        internalMode = InternalMode.Pointing;
                        GetGraphics();

                        last_position = new Point(Control.MousePosition.X, Control.MousePosition.Y);

                        int x_coordinate = last_position.X + b1.X;
                        int y_coordinate = last_position.Y + b1.Y;

                        ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
                        ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

                        CalculateCoordinates(last_position, bounds, out current_xcoord, out current_ycoord);
                        AnnounceUpdates();
                    }
                    break;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (internalMode)
            {
                case InternalMode.Pointing:
                    {
                        GetGraphics();
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

                            CalculateCoordinates(last_position, bounds, out current_xcoord, out current_ycoord);
                            AnnounceUpdates();
                        }
                        else
                        {
                            internalMode = InternalMode.None;
                            graphics = null;
                        }
                    }
                    break;

                case InternalMode.Selecting:
                    {
                        GetGraphics();

                        ControlPaint.DrawReversibleFrame(new Rectangle(
                            Math.Min(starting_position.X, last_position.X) + b1.X, Math.Min(starting_position.Y, last_position.Y) + b1.Y,
                            Math.Abs(last_position.X - starting_position.X), Math.Abs(last_position.Y - starting_position.Y)),
                            Color.White, FrameStyle.Dashed);

                        last_position = new Point(e.X, e.Y);

                        if (bounds.Contains((float)e.X, (float)e.Y))
                        {
                            switch (selectionMode)
                            {
                                case SelectionMode.X:
                                    last_position.Y = (int) bounds.Top;
                                    break;

                                case SelectionMode.Y:
                                    last_position.X = (int) bounds.Right;
                                    break;

                                default:
                                    break;
                            }

                            ControlPaint.DrawReversibleFrame(new Rectangle(
                                Math.Min(starting_position.X, last_position.X) + b1.X, Math.Min(starting_position.Y, last_position.Y) + b1.Y,
                                Math.Abs(last_position.X - starting_position.X), Math.Abs(last_position.Y - starting_position.Y)),
                                Color.White, FrameStyle.Dashed);

                            CalculateCoordinates(last_position, bounds, out current_xcoord, out current_ycoord);
                            AnnounceUpdates();
                        }
                        else
                        {
                            internalMode = InternalMode.None;
                            graphics = null;
                        }
                    }
                    break;

                case InternalMode.None:
                case InternalMode.Selected:
                default:
                    break;
            }
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            switch (internalMode)
            {
                case InternalMode.Pointing:
                    {
                        GetGraphics();

                        int x_coordinate = last_position.X + b1.X;
                        int y_coordinate = last_position.Y + b1.Y;
                        ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
                        ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

                        internalMode = InternalMode.None;
                        graphics = null;
                    }
                    break;

                case InternalMode.Selecting:
                    {
                        GetGraphics();

                        ControlPaint.DrawReversibleFrame(new Rectangle(
                            Math.Min(starting_position.X, last_position.X) + b1.X, Math.Min(starting_position.Y, last_position.Y) + b1.Y,
                            Math.Abs(last_position.X - starting_position.X), Math.Abs(last_position.Y - starting_position.Y)),
                            Color.White, FrameStyle.Dashed);

                        internalMode = InternalMode.None;
                        graphics = null;
                    }
                    break;

                case InternalMode.None:
                case InternalMode.Selected:
                default:
                    break;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            GetGraphics();

            switch (internalMode)
            {
                case InternalMode.Pointing:
                    {
                        int x_coordinate = last_position.X + b1.X;
                        int y_coordinate = last_position.Y + b1.Y;
                        ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
                        ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);
                    }
                    break;

                case InternalMode.Selected:
                    {
                        ControlPaint.DrawReversibleFrame(new Rectangle(
                            Math.Min(starting_position.X, last_position.X) + b1.X, Math.Min(starting_position.Y, last_position.Y) + b1.Y,
                            Math.Abs(last_position.X - starting_position.X), Math.Abs(last_position.Y - starting_position.Y)),
                            Color.White, FrameStyle.Dashed);
                    }
                    break;

                default:
                    break;
            }

            if (bounds.Contains((float)e.X, (float)e.Y))
            {
                internalMode = InternalMode.Selecting;

                starting_position = last_position = new Point(e.X, e.Y);
                switch (selectionMode)
                {
                    case SelectionMode.X:
                        starting_position.Y = (int)bounds.Bottom;
                        last_position.Y = (int)bounds.Top;
                        break;

                    case SelectionMode.Y:
                        starting_position.X = (int)bounds.Left;
                        last_position.X = (int)bounds.Right;
                        break;

                    default:
                        break;
                }

                ControlPaint.DrawReversibleFrame(
                    new Rectangle(starting_position.X + b1.X, starting_position.Y + b1.Y, 0, 0), Color.White, FrameStyle.Dashed);

                CalculateCoordinates(starting_position, bounds, out starting_xcoord, out starting_ycoord);
                CalculateCoordinates(last_position, bounds, out current_xcoord, out current_ycoord);

                AnnounceUpdates();
            }
            else
            {
                internalMode = InternalMode.None;
                graphics = null;
            }           
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (internalMode)
            {
                case InternalMode.Selecting:
                    {
                        GetGraphics();

                        if ((selectionMode == SelectionMode.XY && last_position.Equals(starting_position)) ||
                            (selectionMode == SelectionMode.X && last_position.X.Equals(starting_position.X)) ||
                            (selectionMode == SelectionMode.Y && last_position.Y.Equals(starting_position.Y)))
                        {
                            ControlPaint.DrawReversibleFrame(
                                new Rectangle(starting_position.X, starting_position.Y, 0, 0), Color.White, FrameStyle.Dashed);

                            internalMode = InternalMode.Pointing;

                            last_position = new Point(Control.MousePosition.X, Control.MousePosition.Y);

                            int x_coordinate = last_position.X + b1.X;
                            int y_coordinate = last_position.Y + b1.Y;

                            ControlPaint.DrawReversibleLine(new Point(x_coordinate, b1.Y), new Point(x_coordinate, b2.Y), Color.White);
                            ControlPaint.DrawReversibleLine(new Point(b1.X, y_coordinate), new Point(b2.X, y_coordinate), Color.White);

                            CalculateCoordinates(last_position, bounds, out current_xcoord, out current_ycoord);
                            AnnounceUpdates();
                        }
                        else
                        {
                            internalMode = InternalMode.Selected;
                            AnnounceUpdates();
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Helpers

        private void GetGraphics()
        {
            if (graphics == null)
                graphics = panel1.CreateGraphics();
            bounds = graphics.VisibleClipBounds;
            b1 = panel1.PointToScreen(new Point((int)bounds.Left, (int)bounds.Top));
            b2 = panel1.PointToScreen(new Point((int)bounds.Right, (int)bounds.Bottom));
        }

        private void CalculateCoordinates(Point p, RectangleF bounds, out double xcoord, out double ycoord)
        {
            xcoord = (p.X - bounds.Left) / ((double)bounds.Width);
            ycoord = (bounds.Bottom - p.Y) / ((double)bounds.Height);
        }

        private void AnnounceUpdates()
        {
            if (onChange != null)
                onChange(this, null);
        }

        #endregion

        private bool painting;
        private DateTime lastPainted = DateTime.MinValue;
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            internalMode = InternalMode.None;

            lock (this)
            {
                if (painting)
                    return;

                if (DateTime.Now < lastPainted + TimeSpan.FromSeconds(0.2))
                    return;

                painting = true;
            }

            Graphics graphics = e.Graphics;
            graphics.Clear(Color.White);

            try
            {
                if (drawCallback != null)
                    drawCallback(graphics);

                AnnounceUpdates();
            }
            catch (Exception exc)
            {
                graphics.DrawString(exc.ToString(), new Font(FontFamily.GenericSansSerif, 12), Brushes.Red,
                    graphics.VisibleClipBounds, StringFormat.GenericDefault);
            }

            lock (this)
            {
                painting = false;
                lastPainted = DateTime.Now;
            }
        }

        #region ISelectableArea Members

        QS._core_e_.Data.Point ISelectableArea.Position
        {
            get { return new QS._core_e_.Data.Point(current_xcoord, current_ycoord); }
        }

        QS._core_e_.Data.Rectangle ISelectableArea.Selection
        {
            get 
            {
                return new QS._core_e_.Data.Rectangle(
                    new QS._core_e_.Data.Point(starting_xcoord, starting_ycoord),
                    new QS._core_e_.Data.Point(current_xcoord, current_ycoord));
            }
        }

        bool ISelectableArea.Selected
        {
            get { return internalMode.Equals(InternalMode.Selected); }
        }

        event EventHandler ISelectableArea.OnChange
        {
            add { onChange += value; }
            remove { onChange -= value; }
        }

        QS._qss_e_.Data_.DrawCallback ISelectableArea.DrawCallback
        {
            get { return drawCallback; }
            set { drawCallback = value; }
        }

        #endregion
    }
}
