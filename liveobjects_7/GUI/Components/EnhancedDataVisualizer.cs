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
    public partial class EnhancedDataVisualizer : UserControl, IDataVisualizer
    {
        public EnhancedDataVisualizer()
        {
            InitializeComponent();

            win = (IEnhancedDataWindow)enhancedDataWindow1;
            win.OnChange += new EventHandler(this.SelectionChangeCallback);
        }

        private QS._core_e_.Data.IData data;
        private IEnhancedDataWindow win;
        private QS._core_e_.Data.Point position;
        private QS._core_e_.Data.Rectangle selection;

        private void SelectionChangeCallback(object sender, EventArgs e)
        {
            if (win.Selected)
            {
                selection = new QS._core_e_.Data.Rectangle(
                    new QS._core_e_.Data.Point(
                        win.View.XScale.CoordinateToValue(win.View.XRange, win.Selection.P1.X),
                        win.View.YScale.CoordinateToValue(win.View.YRange, win.Selection.P1.Y)),
                    new QS._core_e_.Data.Point(
                        win.View.XScale.CoordinateToValue(win.View.XRange, win.Selection.P2.X),
                        win.View.YScale.CoordinateToValue(win.View.YRange, win.Selection.P2.Y)));
                toolStripStatusLabel3.Text = "[" + selection.P1.X.ToString() + ", " + selection.P2.X.ToString() + "] x [" +
                    selection.P1.Y.ToString() + ", " + selection.P2.Y.ToString() + "]";
            }
            else
            {
                if (win.View != null)
                {
                    position = new QS._core_e_.Data.Point(win.View.XScale.CoordinateToValue(win.View.XRange, win.Position.X),
                        win.View.YScale.CoordinateToValue(win.View.YRange, win.Position.Y));
                    toolStripStatusLabel3.Text = "(" + position.X.ToString() + ", " + position.Y.ToString() + ")";
                }
            }
        }

        #region ToolStrip Buttons

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region IDataVisualizer Members

        QS._core_e_.Data.IData IDataVisualizer.Data
        {
            get { return data; }
            set 
            {
                data = value;
                win.Data = data;
                if (data != null)
                {
                    win.View = new QS._core_e_.Data.View(data);
                    toolStripStatusLabel1.Text =
                        "X (" + win.View.XRange.Minimum.ToString() + ", " + win.View.XRange.Maximum.ToString() + ")";
                    toolStripStatusLabel2.Text =
                        "Y (" + win.View.YRange.Minimum.ToString() + ", " + win.View.YRange.Maximum.ToString() + ")";
                    toolStripStatusLabel3.Text = string.Empty;
                }
                else
                    win.View = null;
            }
        }

        #endregion
    }
}
