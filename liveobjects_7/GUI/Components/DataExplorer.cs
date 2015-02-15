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
using System.Reflection;
using System.IO;

namespace QS.GUI.Components
{
    public partial class DataExplorer : UserControl
    {
        public DataExplorer() : this(null)
        {
        }

        public DataExplorer(string _rootfolder)
        {
            InitializeComponent();

            ((QS.GUI.Components.IObjectSelector)objectSelector21).SelectionChanged += new EventHandler(
                delegate(object sender, EventArgs e)
                {
                    object obj = ((QS.GUI.Components.IObjectSelector)objectSelector21).SelectedObject;
                    textBox1.Text = "";
                    ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = QS._qss_e_.Data_.Convert.ToDataSeries(obj);
/*
                    QS.TMS.Data.IData data = 
                        QS.TMS.Data.Convert.ToData(((QS.GUI.Components.IObjectSelector)objectSelector21).SelectedObject);
                    ((QS.GUI.Components.IDataVisualizer)enhancedDataVisualizer1).Data = data;                        
*/ 
                });

            // find all the matching types automatically
            foreach (Type t in typeof(QS._qss_e_.Data_.Converters_.Overlay).Assembly.GetTypes())
            {
                ConstructorInfo constructorInfo;
                if (typeof(QS._core_e_.Data.IConverter).IsAssignableFrom(t) && ((constructorInfo = t.GetConstructor(Type.EmptyTypes)) != null))
                    AddConverter((QS._core_e_.Data.IConverter)constructorInfo.Invoke(new object[] { }));
            }

            if (_rootfolder != null)
            {
                folderBrowserDialog1.SelectedPath = _rootfolder;
                Queue<string> _paths = new Queue<string>();
                _paths.Enqueue(_rootfolder);
                while (_paths.Count > 0)
                {
                    string _path = _paths.Dequeue();
                    if ((File.GetAttributes(_path) & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        string _path_trimmed = _path.TrimEnd(' ', '\t', '\n', '\r', '/', '\\', Path.DirectorySeparatorChar);
                        int index = _path_trimmed.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, '/', '\\' }) + 1;
                        string _path_remainder = _path_trimmed.Substring(index);
                        if (_path_remainder.StartsWith("Process_"))
                        {
                            try
                            {
                                ((QS.GUI.Components.IObjectSelector)objectSelector21).Add(new QS._qss_e_.Repository_.Repository(_path));
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(exc.ToString());
                            }
                        }
                        else
                        {
                            foreach (string _subpath in Directory.GetDirectories(_path))
                                _paths.Enqueue(_subpath);
                        }
                    }
                }
            }
        }

        #region Converters

        public void AddConverter(QS._core_e_.Data.IConverter converter)
        {
            comboBox1.Items.Add(new ConverterItem(converter));
        }

        private class ConverterItem
        {
            public ConverterItem(QS._core_e_.Data.IConverter converter)
            {
                this.converter = converter;
            }

            private QS._core_e_.Data.IConverter converter;

            public QS._core_e_.Data.IConverter Converter
            {
                get { return converter; }
            }

            public override string ToString()
            {
                return converter.Name;
            }
        }

        #endregion

        #region Clicking

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((QS.GUI.Components.IObjectSelector)objectSelector21).Add(
                        new QS._qss_e_.Repository_.Repository(folderBrowserDialog1.SelectedPath));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            QS._core_e_.Data.IDataSet dataset = null;
            string name = "";

            if (listView1.SelectedItems.Count == 1)
            {
                MyItem item = listView1.SelectedItems[0] as MyItem;
                if (item != null)
                {
                    name = item.AssignedName;
                    dataset = item.Data;
                }
            }

            ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = dataset;
            textBox1.Text = name;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            // close
        }

        private void refreshRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((QS.GUI.Components.IObjectSelector)objectSelector21).RefreshRecursively();
        }

        private void unrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((QS.GUI.Components.IObjectSelector)objectSelector21).ExpandAll();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            QS._core_e_.Data.IDataSet data = ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).DisplayedData;
            string name = textBox1.Text;

            if (data != null && name != null && name.Length > 0)
            {
                listView1.Items.Add(new MyItem(name, data));
            }
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            ConverterItem converter_item = comboBox1.SelectedItem as ConverterItem;
            if (converter_item != null)
            {
                KeyValuePair<string, QS._core_e_.Data.IDataSet>[] arguments = new KeyValuePair<string, QS._core_e_.Data.IDataSet>[listView1.SelectedItems.Count];
                for (int ind = 0; ind < arguments.Length; ind++)
                {
                    MyItem item = listView1.SelectedItems[ind] as MyItem;
                    if (item == null)
                        throw new Exception("Cannot convert, some nulls encountered.");
                    arguments[ind] = new KeyValuePair<string, QS._core_e_.Data.IDataSet>(item.AssignedName, item.Data);
                }
                
                foreach (KeyValuePair<string, QS._core_e_.Data.IDataSet> result in converter_item.Converter.Convert(arguments))
                    listView1.Items.Add(new MyItem(result.Key, result.Value));
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QS._qss_e_.Repository_.Repository repository =
                ((QS.GUI.Components.IObjectSelector)objectSelector21).SelectedObject as QS._qss_e_.Repository_.Repository;
            if (repository != null)
            {
                ((QS.GUI.Components.IObjectSelector)objectSelector21).Remove(repository);

                repository = new QS._qss_e_.Repository_.Repository(((QS._core_e_.Repository.IRepository)repository).Root);
                ((QS.GUI.Components.IObjectSelector)objectSelector21).Add(repository);
            }
        }

        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // .......................
        }

        #endregion

        #region Class MyItem

        private class MyItem : ListViewItem
        {
            public MyItem(string name, QS._core_e_.Data.IDataSet data) : base(name)
            {
                this.name = name;
                this.data = data;
            }

            private string name;
            private QS._core_e_.Data.IDataSet data;

            public QS._core_e_.Data.IDataSet Data
            {
                get { return data; }
            }

            public string AssignedName
            {
                get { return name; }
            }
        }

        #endregion

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            object selected_obj = objectSelector21.SelectedActualObject;
            QS._core_e_.Repository.ScalarAttribute attribute = selected_obj as QS._core_e_.Repository.ScalarAttribute;
            if (attribute != null)
            {
                string root = ((QS._core_e_.Repository.IAttribute)attribute).Repository.Root;
                string marker = "\\Experiment_Results\\";
                string relative_root = root.Substring(root.IndexOf(marker) + marker.Length);
                QS._core_e_.Data.IView view = ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).View;
                double minx = view.XRange.Minimum;
                double maxx = view.XRange.Maximum;
                double miny = view.YRange.Minimum;
                double maxy = view.YRange.Maximum;
                string address = "http://" + toolStripTextBox1.Text + "/Draw.aspx?data=" + relative_root.Replace("\\", "/") + "/" +
                    ((QS._core_e_.Repository.IAttribute)attribute).Ref.Replace(";", "/") + "&w=1024&h=768&minx=" + minx.ToString() +
                    "&maxx=" + maxx.ToString() + "&miny=" + miny.ToString() + "&maxy=" + maxy.ToString() + "&filters=" + 
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(dataSetVisualizer1.FilterPropertyNames, ",");                
                Clipboard.SetDataObject(address);
            }
        }
    }
}
