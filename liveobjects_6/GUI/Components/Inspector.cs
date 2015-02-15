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
    public partial class Inspector : UserControl, IInspector
    {
        public Inspector()
        {
            InitializeComponent();
            ((IObjectSelector) objectSelector).SelectionChanged += new EventHandler(
                delegate(object sender, EventArgs e)
                {
                    ((IObjectVizualizer)objectVisualizer).VisualizedObject = ((IObjectSelector)objectSelector).SelectedObject;
                });
        }

        public IObjectSelector ObjectSelector
        {
            get { return objectSelector; }
        }

        #region IInspector Members

        void QS._qss_e_.Inspection_.IInspector.Add(object o)
        {
            try
            {
                ((IObjectSelector)objectSelector).Add(o);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private void refreshResursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((IObjectSelector)objectSelector).RefreshRecursively();
            }
            catch (Exception)
            {
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((IObjectSelector)objectSelector).ExpandAll();
            }
            catch (Exception)
            {
            }
        }

        private void unrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((IObjectSelector)objectSelector).Unroll();
            }
            catch (Exception)
            {
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((IObjectSelector)objectSelector).Refresh();
                ((IObjectVizualizer)objectVisualizer).VisualizedObject = ((IObjectSelector)objectSelector).SelectedObject;
                ((IObjectVizualizer)objectVisualizer).ForceRefresh();
            }
            catch (Exception)
            {
            }
        }

        private void splitContainer1_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.SplitterDistance = (int)Math.Floor(splitContainer1.Height * 0.5);
            }
            catch (Exception)
            {
            }
        }

//        private void InitializeComponent()
//        {
//            this.SuspendLayout();
//            // 
//            // Inspector
//            // 
//            this.Name = "Inspector";
//            this.Size = new System.Drawing.Size(289, 249);
//            this.ResumeLayout(false);
//
//        }
    }
}
