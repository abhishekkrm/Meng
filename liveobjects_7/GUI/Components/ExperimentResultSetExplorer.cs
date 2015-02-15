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
using System.IO;

namespace QS.GUI.Components
{
    public partial class ExperimentResultSetExplorer : UserControl
    {
        public ExperimentResultSetExplorer(string data_folder, string[] dump_folders)
        {
            InitializeComponent();

            ((QS.GUI.Components.IObjectSelector) objectSelector21).SelectionChanged += new EventHandler(
                delegate(object sender, EventArgs e)
                {
                    object obj = ((QS.GUI.Components.IObjectSelector) objectSelector21).SelectedObject;
                    ((QS.GUI.Components.IDataSetVizualizer) dataSetVisualizer1).SourceData = QS._qss_e_.Data_.Convert.ToDataSeries(obj);
                });

            foreach (string x in Directory.GetDirectories(data_folder))
            {
                string node = x.Substring(x.LastIndexOf("\\") + 1);
                foreach (string y in Directory.GetDirectories(x))
                {
                    string process = y.Substring(y.LastIndexOf("\\") + 1);
                    listView1.Items.Add(new Repository(y, node, process));
                }
            }

            foreach (string dump_folder in dump_folders)
            {
                string name = dump_folder.Substring(dump_folder.LastIndexOf("\\") + 1).Trim();
                if (name.EndsWith(".info"))
                    name = name.Substring(0, name.Length - (".info").Length);
                listView1.Items.Add(new Repository(dump_folder, "$" + name, ""));
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Repository rep = (Repository) listView1.SelectedItems[0];
                ((QS.GUI.Components.IObjectSelector) objectSelector21).Add(new QS._qss_e_.Repository_.Repository(rep.path));
            }
        }

        private class Repository : ListViewItem
        {
            public Repository(string path, string node, string process) :
                base(new string[] { node, process })
            {
                this.path = path;
                this.node = node;
                this.process = process;
            }

            public string path, node, process;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((QS.GUI.Components.IObjectSelector)objectSelector21).RefreshRecursively();
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((QS.GUI.Components.IObjectSelector)objectSelector21).ExpandAll();
        }
    }
}
