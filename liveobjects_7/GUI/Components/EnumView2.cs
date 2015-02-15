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
    public partial class EnumView2 : UserControl
    {
        public EnumView2()
        {
            InitializeComponent();
        }

        private System.Collections.IEnumerable visualizedObject;
        private int index, nelements;
        private System.Collections.IEnumerator enumerator;
        private List<object> objects = new List<object>();

        public System.Collections.IEnumerable VisualizedObject
        {
            get { return visualizedObject; }
            set 
            { 
                visualizedObject = value;
                if (visualizedObject is System.Collections.ICollection)
                    nelements = ((System.Collections.ICollection)visualizedObject).Count;
                if (visualizedObject != null)
                    ResetEnumerator();
            }
        }

        private void AdvanceEnumerator()
        {
            while (objects.Count <= (index + 1) && enumerator.MoveNext())
                objects.Add(enumerator.Current);

            if (objects.Count > (index + 1))
            {
                index++;
                if (index + 1 > nelements)
                    nelements = index + 1;
                toolStripStatusLabel1.Text = (index + 1).ToString() + " / " + nelements.ToString() + "+";
                richTextBox1.Text = objects[index].ToString();
                if (index > 0)
                    toolStripButton1.Enabled = true;
            }
            else
            {
                nelements = index + 1;
                toolStripStatusLabel1.Text = (index + 1).ToString() + " / " + nelements.ToString() + "+";
                toolStripButton2.Enabled = false;
            }
        }

        private void RollbackEnumerator()
        {
            if (index > 0)
            {
                index--;
                toolStripStatusLabel1.Text = (index + 1).ToString() + " / " + nelements.ToString() + "+";
                richTextBox1.Text = objects[index].ToString();
                toolStripButton2.Enabled = true;
                if (index == 0)
                    toolStripButton1.Enabled = false;
            }
        }

        private void ResetEnumerator()
        {
            enumerator = visualizedObject.GetEnumerator();
            objects.Clear();
            index = -1;
            richTextBox1.Text = string.Empty;
            toolStripButton1.Enabled = false;

            AdvanceEnumerator();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RollbackEnumerator();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AdvanceEnumerator();
        }
    }
}
