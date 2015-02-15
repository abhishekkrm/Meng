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
    public partial class EnhancedDataWindow : UserControl, IEnhancedDataWindow
    {
        public EnhancedDataWindow()
        {
            InitializeComponent();
            ((ISelectableArea)selectableArea1).DrawCallback = new QS._qss_e_.Data_.DrawCallback(this.DrawCallback);
        }

        private QS._core_e_.Data.IView view;
        private QS._core_e_.Data.IData data;
        private bool connectingLines = true;

        public bool ConnectingLines
        {
            get { return connectingLines; }
            set { connectingLines = value; }
        }

        private void DrawCallback(Graphics graphics)
        {
            RectangleF bounds = graphics.VisibleClipBounds;
            RectangleF bounds2 = new RectangleF(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 3);

            string message = null;

            if (view != null && data != null)
            {
                if (view.XRange.Maximum < view.XRange.Minimum || view.YRange.Maximum < view.YRange.Minimum ||
                    data.XAxis.Range.Maximum < data.XAxis.Range.Minimum || data.YAxis.Range.Maximum < data.YAxis.Range.Minimum)
                {
                    message = "Either the view or the data is empty.";
                }
                else
                {
                    view.Draw(graphics, bounds2);
                    data.Draw(graphics, bounds2, view, new QS._core_e_.Data.DrawingContext(connectingLines));
                }
            }
            else
            {
                message = "Either the view or the data is NULL.";                
            }

            if (message != null)
            {
                Font font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Regular);
                SizeF string_size = graphics.MeasureString(message, font);
                graphics.DrawString(message, font, Brushes.Red,
                    (bounds.Left + bounds.Right - string_size.Width) / 2, (bounds.Top + bounds.Bottom - string_size.Height) / 2); 
            }
        }

        public SelectableArea.SelectionMode SelectionMode
        {
            get { return selectableArea1.Mode; }
            set { selectableArea1.Mode = value; }
        }

        #region IEnhancedDataWindow Members

        QS._core_e_.Data.IView IEnhancedDataWindow.View
        {
            get { return view; }
            set
            {
                view = value;
                Refresh();
            }
        }

        QS._core_e_.Data.IData IEnhancedDataWindow.Data
        {
            get { return data; }
            set
            {
                data = value;
                Refresh();
            }
        }

        QS._core_e_.Data.Point IEnhancedDataWindow.Position
        {
            get { return ((ISelectableArea)selectableArea1).Position; }
        }

        QS._core_e_.Data.Rectangle IEnhancedDataWindow.Selection
        {
            get { return ((ISelectableArea)selectableArea1).Selection; }
        }

        bool IEnhancedDataWindow.Selected
        {
            get { return ((ISelectableArea)selectableArea1).Selected; }
        }

        event EventHandler IEnhancedDataWindow.OnChange
        {
            add { ((ISelectableArea)selectableArea1).OnChange += value; }
            remove { ((ISelectableArea)selectableArea1).OnChange -= value; }
        }

        #endregion
    }
}
