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
    public partial class ExperimentResultSetSelector : UserControl
    {
        public ExperimentResultSetSelector()
        {
            InitializeComponent();
            // _RecreateTheList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string newpath = folderBrowserDialog1.SelectedPath;
                if (newpath != textBox1.Text)
                {
                    textBox1.Text = newpath;
                    _RecreateTheList();
                }
            }
        }

        private void _RecreateTheList()
        {
            string newpath = textBox1.Text;
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (string folder in Directory.GetDirectories(newpath))
            {
                string experiment_spec_path = folder + "\\Experiment_Specification.xml";
                if (File.Exists(experiment_spec_path))
                {
                    QS._qss_e_.Launchers_.ExperimentSpecification experiment_spec;
                    try
                    {
                        experiment_spec = QS._qss_e_.Launchers_.ExperimentSpecification.Load(experiment_spec_path);
                    }
                    catch (Exception)
                    {
                        experiment_spec = null;
                    }

                    string data_path = folder + "\\data";
                    string name = folder.Substring(folder.LastIndexOf("\\") + 1);

                    IDictionary<string, string> dic = new Dictionary<string, string>();
                    if (experiment_spec != null)
                    {
                        foreach (QS._qss_e_.Launchers_.ExperimentSpecification.Argument arg in experiment_spec.Arguments)
                            dic.Add(arg.Name, arg.Value);
                    }

                    string[] dump_paths = Directory.GetDirectories(folder, "*.info");

                    string reports_path = folder + "\\reports";

                    bool data_exists = Directory.Exists(data_path) && _HasFiles(data_path);
                    bool reports_exist = Directory.Exists(reports_path) && _HasFiles(reports_path);

                    listView1.Items.Add(new Experiment(folder, name, data_exists, experiment_spec, dic, dump_paths, reports_exist)); 
                }
            }

            listView1.EndUpdate();
        }

        private static bool _HasFiles(string path)
        {
            if (Directory.GetFiles(path).Length > 0)
                return true;

            foreach (string subdir in Directory.GetDirectories(path))
            {
                if (_HasFiles(subdir))
                    return true;
            }

            return false;
        }

        private class Experiment : ListViewItem
        {
            public Experiment(string exp_folder, string exp_name, bool isok, 
                QS._qss_e_.Launchers_.ExperimentSpecification spec, IDictionary<string, string> dic, string[] dump_paths, bool profiled)
                : base(
                    new string[] 
                    { 
                        exp_name, 
                        (isok ? "yes" : "no"),   
                        ((spec != null) ? _countnodes(spec.Workers, exp_folder, spec.Machines).ToString() : string.Empty),
                        (dic.ContainsKey("scenario") ? dic["scenario"] : ""),
                        (dic.ContainsKey("ngroups") ? dic["ngroups"] : ""),
                        (dic.ContainsKey("nsenders") ? dic["nsenders"] : ""),
                        (dic.ContainsKey("rate") ? dic["rate"] : ""),
                        (dic.ContainsKey("count") ? dic["count"] : ""),
                        (dic.ContainsKey("size") ? dic["size"] : ""),
                        (dic.ContainsKey("nregions") ? dic["nregions"] : ""),
                        (dic.ContainsKey("rs_replication") ? dic["rs_replication"] : ""),
                        (dic.ContainsKey("rs_token_rate") ? dic["rs_token_rate"] : ""),
                        (dic.ContainsKey("drop_nodes") ? dic["drop_nodes"] : ""),
                        (dic.ContainsKey("crash_nodes") ? dic["crash_nodes"] : ""),
                        (dic.ContainsKey("sleep_nodes") ? dic["sleep_nodes"] : ""),
                        (dump_paths.Length.ToString()),
                        (profiled ? "yes" : "no")
                    })
            {
                this.exp_folder = exp_folder;
                this.exp_name = exp_name;
                this.isok = isok;
                this.spec = spec;
                this.dic = dic;
                this.dump_paths = dump_paths;
                this.profiled = profiled;

                if (isok)
                    this.ForeColor = Color.Blue;
                else
                    this.ForeColor = Color.Gray;
            }

            private static int _countnodes(string[] names, string rootfolder, string machinesfile)
            {
                int n = (names != null) ? names.Length : 0;
                if (machinesfile != null)
                {
                    using (StreamReader reader = new StreamReader(rootfolder + Path.DirectorySeparatorChar + machinesfile))
                    {
                        string s;
                        while ((s = reader.ReadLine()) != null && s.Trim(' ', '\r', '\t', '\n').Length > 0)
                            n++;
                    }
                }
                return n;
            }

            private string exp_folder, exp_name;
            private string[] dump_paths;
            private bool isok, /* dump_exists, */ profiled;
            private QS._qss_e_.Launchers_.ExperimentSpecification spec;
            private IDictionary<string, string> dic;

            public bool IsOK
            {
                get { return isok; }
            }

            public string ExperimentFolder
            {
                get { return exp_folder; }
            }

            public QS._qss_e_.Launchers_.ExperimentSpecification Spec
            {
                get { return spec; }
            }

            public IDictionary<string, string> Dic
            {
                get { return dic; }
            }

            public string ExperimentName
            {
                get { return exp_name; }
            }

            public string[] DumpPaths
            {
                get { return dump_paths; }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.BeginUpdate();
            listView2.Items.Clear();
            if (listView1.SelectedItems.Count == 1)
            {
                Experiment exp = (Experiment)listView1.SelectedItems[0];
                selectedpath = exp.ExperimentFolder;
                if (exp.IsOK && exp.Dic != null)
                {
                    foreach (KeyValuePair<string, string> element in exp.Dic)
                        listView2.Items.Add(new ListViewItem(new string[] { element.Key, element.Value }));
                }

                ContextMenu contextmenu = new ContextMenu();
                contextmenu.MenuItems.Add("Arguments", new EventHandler(this.ArgumentsCallback));

                foreach (string dumpfile in exp.DumpPaths)
                {
                    string filename = dumpfile + "\\statistics.txt";
                    TextViewer viewer = new TextViewer(filename, filename);
                    contextmenu.MenuItems.Add(filename, new EventHandler(viewer.StatisticsCallback));
                }

                contextmenu.MenuItems.Add("Processing", new EventHandler(this.ProcessingCallback));

                listView1.ContextMenu = contextmenu;
            }
            else
            {
                selectedpath = null;
                listView1.ContextMenu = null;
            }
            listView2.EndUpdate();
        }

        private string selectedpath;

        public string SelectedPath
        {
            get 
            {
                return selectedpath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _RecreateTheList();
        }

//        private EventHandler onDoubleClick;

//        public event EventHandler OnExperimentDoubleClick
//        {
//            add { onDoubleClick += value; }
//            remove { onDoubleClick -= value; }
//        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Experiment exp = (Experiment) listView1.SelectedItems[0];
                selectedpath = exp.ExperimentFolder;
                if (exp.IsOK)
                {
                    Form form = new Form();
                    Control control;
                    string _datafolder = exp.ExperimentFolder + "\\data";
                    control = new ExperimentResultSetExplorer(_datafolder, exp.DumpPaths);
                    // control = new DataExplorer(_datafolder);
                    form.Controls.Add(control);
                    control.Dock = DockStyle.Fill;
                    form.Show();
                    form.Text = "Results(" + exp.ExperimentName + ")";
                }
            }
        }

        private void ProcessingCallback(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Experiment exp = (Experiment)listView1.SelectedItems[0];
                selectedpath = exp.ExperimentFolder;
                if (exp.IsOK)
                {
                    Form form = new Form();
                    Control control;
                    string _datafolder = exp.ExperimentFolder + "\\data";
                    // control = new ExperimentResultSetExplorer(_datafolder, exp.DumpPaths);
                    control = new DataExplorer(_datafolder);
                    form.Controls.Add(control);
                    control.Dock = DockStyle.Fill;
                    form.Show();
                    form.Text = "Results(" + exp.ExperimentName + ")";
                }
            }
        }

        private class TextViewer
        {
            public TextViewer(string name, string filename)
            {
                this.name = name;
                this.filename = filename;
            }

            private string name, filename;

            public void StatisticsCallback(object sender, EventArgs e)
            {
                string statistics;
                using (StreamReader reader = new StreamReader(filename))
                {
                    statistics = reader.ReadToEnd();
                }
                RichTextBox box = new RichTextBox();
                box.Text = statistics;
                box.Font = new Font("Courier New", 14, FontStyle.Bold);
                Form form = new Form();
                form.Controls.Add(box);
                box.Dock = DockStyle.Fill;
                form.Text = name;
                form.Show();
            }
        }

        private void ArgumentsCallback(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Experiment exp = (Experiment)listView1.SelectedItems[0];
                List<string> keys = new List<string>(exp.Dic.Keys);
                keys.Sort();
                int maxsize = 0;
                foreach (string key in keys)
                    maxsize = Math.Max(maxsize, key.Length);
                StringBuilder s = new StringBuilder();
                foreach (string key in keys)
                    s.AppendLine(key.PadRight(maxsize) + " = " + exp.Dic[key]);
                RichTextBox box = new RichTextBox();
                box.Text = s.ToString();
                box.Font = new Font("Courier New", 14, FontStyle.Bold);
                Form form = new Form();
                form.Controls.Add(box);
                box.Dock = DockStyle.Fill;
                form.Text = "Arguments(" + exp.ExperimentName + ")";
                form.Show();
            }
        }
    }
}
