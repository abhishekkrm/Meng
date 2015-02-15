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
    public partial class SelectionViewer : UserControl, ISelectionViewer
    {
        public SelectionViewer()
        {
            InitializeComponent();

            List<Variable> vars = new List<Variable>();
            vars.Add(new Variable("X1"));
            vars.Add(new Variable("Y1"));
            vars.Add(new Variable("X2"));
            vars.Add(new Variable("Y2"));
            vars.Add(new Variable("DX"));
            vars.Add(new Variable("DY"));

            variables = vars.ToArray();
            dataGridView1.DataSource = variables;

            selectionChangedEventHandler = new EventHandler(dataWindow_SelectionChanged);
        }

        private Variable[] variables;
        private IDataWindow dataWindow;
        private EventHandler selectionChangedEventHandler;

        #region Class Variable

        public class Variable
        {
            public Variable(string name)
            {
                this.name = name;
                this.data = 0;
            }

            private string name;
            private double data;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public double Value
            {
                get { return data; }
                set { data = value; }
            }
        }

        #endregion

        #region ISelectionViewer Members

        void dataWindow_SelectionChanged(object sender, EventArgs e)
        {
            QS._core_e_.Data.Rectangle rec = dataWindow.Selection;
            double x1 = rec.P1.X, y1 = rec.P1.Y, x2 = rec.P2.X, y2 = rec.P2.Y;

            variables[0].Value = x1;
            variables[1].Value = y1;

            variables[2].Value = x2;
            variables[3].Value = y2;

            double dx = x2 - x1;
            double dy = y2 - y1;

            variables[4].Value = dx;
            variables[5].Value = dy;

/*
            double slope = (dx == 0) ? double.PositiveInfinity : (dy / dx);
            double inv_slope = (dy == 0) ? double.PositiveInfinity : (dx / dy);

            variables[4].Value = inv_slope;
            variables[5].Value = slope;

            double y10 = Math.Pow(10, y2);

            variables[6].Value = y10;
*/
            dataGridView1.Refresh();        
        }

        IDataWindow ISelectionViewer.DataWindow
        {
            get { return dataWindow; }
            set
            {
                lock (this)
                {
                    if (dataWindow != null)
                        dataWindow.SelectionChanged -= selectionChangedEventHandler;
                    dataWindow = value;
                    dataWindow.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        #endregion
    }
}
