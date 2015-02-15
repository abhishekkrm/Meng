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
    public class View : IView
    {
        public View(IData data) : this(data.XAxis.Range, data.YAxis.Range, Scale.Linear, Scale.Linear)
        {
        }

        public View(IDataSet data) : this(new Range(data.Range.P1.X, data.Range.P2.X),
            new Range(data.Range.P1.Y, data.Range.P2.Y), Scale.Linear, Scale.Linear)
        {
        }

        public View(Range xRange, Range yRange, IScale xScale, IScale yScale)
        {
            this.xRange = xRange;
            this.yRange = yRange;
            this.xScale = xScale;
            this.yScale = yScale;
        }

        private Range xRange, yRange;
        private IScale xScale, yScale;

        private static int spacing_x = 90;
        private static int spacing_y = 30;
        private static int accuracy = 5;

        public static void Draw(IView view, System.Drawing.Graphics graphics, System.Drawing.RectangleF bounds)
        {
            int nlines_x = (int)Math.Floor(bounds.Width / ((double)spacing_x));
            int nlines_y = (int)Math.Floor(bounds.Height / ((double)spacing_y));

            Pen pen = new Pen(Color.DarkGray);
            Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pen.Width = (float)0.2;

            for (int ind = 0; ind <= nlines_y; ind++)
            {
                double multiplier = (((double)ind) / ((double)nlines_y));
                float y = bounds.Bottom - (float)(multiplier * bounds.Height);
                double value_y = view.YScale.CoordinateToValue(view.YRange, multiplier);
                if (value_y != 0)
                {
                    int ndigits = accuracy - 1 - (int)Math.Floor(Math.Log10(value_y));
                    value_y = Math.Round(value_y, Math.Min(Math.Max(ndigits, 0), 15));
                }

                if (ind > 0 && ind < nlines_y)
                    graphics.DrawLine(pen, bounds.Left, y, bounds.Right, y);
                string value_string = value_y.ToString("g");
                if (ind > 0)
                    graphics.DrawString(value_string, font, Brushes.Black, 1, y + 1);
                //                    else
                //                    {
                //                        SizeF size = graphics.MeasureString(value_string, font);
                //                        graphics.DrawString(value_string, font, Brushes.Black, 1, y - size.Height - 1);
                //                    }
            }

            for (int ind = 1; ind <= nlines_x; ind++)
            {
                double multiplier = ((double)ind) / ((double)nlines_x);
                float x = bounds.Left + (float)(multiplier * bounds.Width);
                double value_x = view.XScale.CoordinateToValue(view.XRange, multiplier);
                if (value_x != 0)
                {
                    int ndigits = accuracy - 1 - (int)Math.Floor(Math.Log10(value_x));
                    value_x = Math.Round(value_x, Math.Min(Math.Max(ndigits, 0), 15));
                }

                if (ind > 0 && ind < nlines_x)
                    graphics.DrawLine(pen, x, bounds.Top, x, bounds.Bottom);
                string value_string = value_x.ToString("g");
                SizeF string_size = graphics.MeasureString(value_string, font);
                if (ind > 0)
                    graphics.DrawString(value_string, font, Brushes.Black,
                        x - string_size.Width - 1, bounds.Bottom - string_size.Height - 1);
            }
        }

        #region IView Members

        public Range XRange
        {
            get { return xRange; }
            set { xRange = value; }
        }

        public Range YRange
        {
            get { return yRange; }
            set { yRange = value; }
        }

        public IScale XScale
        {
            get { return xScale; }
            set { xScale = value; }
        }

        public IScale YScale
        {
            get { return yScale; }
            set { yScale = value; }
        }

        void IView.Draw(System.Drawing.Graphics graphics, System.Drawing.RectangleF bounds)
        {
            Draw(this, graphics, bounds);
        }

        #endregion
    }
}
