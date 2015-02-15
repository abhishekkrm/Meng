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

namespace QS._qss_x_.Shell_
{
    public partial class ObjectView : Form
    {
        public ObjectView(string _objectdefinition)
        {
            InitializeComponent();

            if (_objectdefinition != null)
                _Set(_objectdefinition);
        }

        private string _objectdefinition;

        private void _Set(string _objectdefinition)
        {
            lock (this)
            {
                Controls.Clear();
                this._objectdefinition = _objectdefinition;
                if (_objectdefinition != null)
                {
                    RichTextBox textbox = new RichTextBox();
                    textbox.Text = _objectdefinition;
                    textbox.Dock = DockStyle.Fill;
                    Controls.Add(textbox);
                }
            }
        }

        private void ObjectView_DragEnter(object sender, DragEventArgs e)
        {
/*
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop, false))
*/ 
            e.Effect = DragDropEffects.All;
        }

        private void ObjectView_DragLeave(object sender, EventArgs e)
        {
        }

        private void ObjectView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string _text = (string) e.Data.GetData(DataFormats.Text);
                _Set(_text);
            }
/*
            else if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] files = (string []) e.Data.GetData(DataFormats.FileDrop);
            }
*/
        }
    }
}
