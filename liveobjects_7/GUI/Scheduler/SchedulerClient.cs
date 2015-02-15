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

namespace QS.GUI.Scheduler
{
    public partial class SchedulerClient : UserControl
    {
        public SchedulerClient()
        {
            InitializeComponent();

            listView2.Items.Add(nameParameter = new Parameter("Name", typeof(string), true));
            listView2.Items.Add(descriptionParameter = new Parameter("Description", typeof(string), true));
            listView2.Items.Add(timeParameter = new Parameter("Time", typeof(DateTime), true));
            listView2.Items.Add(priorityParameter = new Parameter("Priority", typeof(double), true));
            listView2.Items.Add(executableParameter = new Parameter("Executable", typeof(string), true));
            listView2.Items.Add(argumentsParameter = new Parameter("Arguments", typeof(string), true));
            listView2.Items.Add(statusParameter = new Parameter("Status", typeof(string), false));
            listView2.Items.Add(filesParameter = new Parameter("Files", typeof(string[]), false));

            foreach (ColumnHeader column in listView2.Columns)
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private QS._qss_d_.Scheduler_3_.IScheduler scheduler;
        private Parameter nameParameter, descriptionParameter, timeParameter, priorityParameter, executableParameter,
            argumentsParameter, statusParameter, filesParameter;        

        public void Connect(string servername)
        {
            scheduler = QS._qss_d_.Scheduler_3_.Scheduler.GetClient(servername);
            ReloadTasks();
        }

        private void ReloadTasks()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (string id in scheduler.GetTasks())
                listView1.Items.Add(new Task(scheduler.GetTask(id)));
            foreach (ColumnHeader column in listView1.Columns)
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.EndUpdate();
        }

        private void UpdateParameters()
        {
            foreach (Task task in listView1.SelectedItems)
            {
                task.task.Name = (string) nameParameter.Value;
                task.task.Description = (string) descriptionParameter.Value;
                task.task.Priority = (double) priorityParameter.Value;
                task.task.Executable = (string) executableParameter.Value;
                task.task.Time = (DateTime) timeParameter.Value;
                task.task.Arguments = (string) argumentsParameter.Value;
                task.UpdateAppearance();
            }
        }

        #region Class Task

        private class Task : ListViewItem
        {
            public Task(QS._qss_d_.Scheduler_3_.ITask task) : base(task.ID)
            {
                this.task = task;

                this.SubItems.Add(time_item = new ListViewSubItem(this, task.Time.ToShortTimeString()));
                this.SubItems.Add(priority_item = new ListViewSubItem(this, task.Priority.ToString()));
                this.SubItems.Add(status_item = new ListViewSubItem(this, task.Status.ToString()));
                this.SubItems.Add(name_item = new ListViewSubItem(this, task.Name));
                this.SubItems.Add(description_item = new ListViewSubItem(this, task.Description));
            }

            public QS._qss_d_.Scheduler_3_.ITask task;

            private ListViewSubItem time_item, priority_item, status_item, name_item, description_item;

            public void UpdateAppearance()
            {
                this.Text = task.ID;
                time_item.Text = task.Time.ToShortTimeString();
                priority_item.Text = task.Priority.ToString();
                status_item.Text = task.Status.ToString();
                name_item.Text = task.Name;
                description_item.Text = task.Description;
            }
        }

        #endregion

        #region Class Parameter

        private class Parameter : ListViewItem
        {
            public Parameter(string name, Type type, bool editable) : base(name)
            {
                this.editable = editable;
                this.value = null;
                this.type = type;
                this.SubItems.Add(subitem = new ListViewSubItem(this, ""));
            }

            private bool editable;
            private object value;
            private Type type;
            private ListViewSubItem subitem;

            public bool Editable
            {
                get { return editable; }
            }

            public object Value
            {
                get { return value; }
            }

            public void UpdateValue(object value)
            {
                this.value = value;
                subitem.Text = this.ValueString;
            }

            public string ValueString
            {
                get 
                { 
                    return (value != null) ? 
                        ((value is string[]) ? (QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>((string[]) value, ";")) : value.ToString()) : ""; 
                }
                set
                {
                    if (type == typeof(string))
                        UpdateValue(value);
                    else if (type == typeof(double))
                        UpdateValue(Convert.ToDouble(value));
                    else if (type == typeof(string[]))
                        UpdateValue(value.Split(new char[] { ';' }));
                    else if (type == typeof(DateTime))
                        UpdateValue(DateTime.Parse(value));
                    else
                        throw new Exception("Bad format");                           
                }
            }
        }

        #endregion

        #region Clicking around

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Task task in listView1.SelectedItems)
            {
                nameParameter.UpdateValue(task.task.Name);
                descriptionParameter.UpdateValue(task.task.Description);
                timeParameter.UpdateValue(task.task.Time);
                priorityParameter.UpdateValue(task.task.Priority);
                executableParameter.UpdateValue(task.task.Executable);
                argumentsParameter.UpdateValue(task.task.Arguments);
                statusParameter.UpdateValue(task.task.Status);
                filesParameter.UpdateValue((task.task.Files != null) ?
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(task.task.Files, "; ") : null);
            }
            foreach (ColumnHeader column in listView2.Columns)
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (Parameter parameter in listView2.SelectedItems)
            {
                if (parameter.Editable)
                {
                    EditParameter editParameter = new EditParameter(parameter.Text, parameter.ValueString);
                    editParameter.StartPosition = FormStartPosition.Manual;
                    editParameter.SetDesktopLocation(MousePosition.X, MousePosition.Y);
                    if (editParameter.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            listView2.BeginUpdate();
                            parameter.ValueString = editParameter.Value;
                            listView2.EndUpdate();
                            UpdateParameters();
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.ToString());
                        }
                    }
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadTasks();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CreateTask createTask = new CreateTask();
            if (createTask.ShowDialog() == DialogResult.OK)
            {
                QS._qss_d_.Scheduler_3_.ITask task = scheduler.CreateTask(createTask.TaskName, createTask.TaskDescription, 
                    createTask.TaskTime, createTask.TaskPriority, createTask.TaskExecutable, createTask.TaskArguments);
                if (createTask.TaskFiles != null)
                {
                    foreach (string filename in createTask.TaskFiles)
                    {
                        byte[] bytes = null;
                        using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        {
                            bytes = new byte[stream.Length];
                            if (stream.Read(bytes, 0, (int)stream.Length) != stream.Length)
                                throw new Exception("Could not read the whole file.");
                        }

                        task.Upload(filename.Substring(filename.LastIndexOf("\\") + 1), bytes);
                    }
                }
                listView1.Items.Add(new Task(task));
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            foreach (Task task in listView1.SelectedItems)
            {
                // scheduler.StopTask(task.id);
            }
        }

        #endregion
    }
}
