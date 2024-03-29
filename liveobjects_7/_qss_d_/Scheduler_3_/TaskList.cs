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

namespace QS._qss_d_.Scheduler_3_
{
    public partial class TaskList : UserControl
    {
        public TaskList()
        {
            InitializeComponent();
        }

        private IScheduler scheduler;
        private IDictionary<string, TaskListViewItem> items = new Dictionary<string, TaskListViewItem>();

        public IScheduler Scheduler
        {
            get { return scheduler; }
            set
            {
                scheduler = value;
                UpdateAppearance();
            }
        }

        public TaskListViewItem Selected
        {
            get
            {
                ListView.SelectedListViewItemCollection selected_items = listView1.SelectedItems;
                if (selected_items.Count == 1)
                    return (TaskListViewItem) selected_items[0];
                else
                    return null;
            }
        }

        public void UpdateAppearance()
        {
            lock (this)
            {
                if (scheduler != null)
                {
                    listView1.BeginUpdate();
                    System.Collections.ObjectModel.Collection<string> taskIDs =
                        new System.Collections.ObjectModel.Collection<string>(new List<string>(scheduler.GetTasks()));
                    List<string> to_remove = new List<string>();
                    foreach (KeyValuePair<string, TaskListViewItem> element in items)
                    {
                        if (taskIDs.Contains(element.Key))
                        {
                            element.Value.UpdateAppearance();
                            taskIDs.Remove(element.Key);
                        }
                        else
                        {
                            listView1.Items.Remove(element.Value);
                            to_remove.Add(element.Key);
                        }
                    }

                    foreach (string taskid in to_remove)
                        items.Remove(taskid);

                    foreach (string taskID in taskIDs)
                    {
                        if (!items.ContainsKey(taskID))
                        {

                            TaskListViewItem item = new TaskListViewItem(scheduler.GetTask(taskID));
                            items.Add(taskID, item);
                            listView1.Items.Add(item);
                        }
                    }

                    listView1.EndUpdate();
                }
                else
                    listView1.Items.Clear();
            }
        }
    }
}
