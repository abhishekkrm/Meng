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
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace QS._qss_d_.Scheduler_3_
{
    public class TaskListViewItem : ListViewItem
    {
        public TaskListViewItem(ITask task)
        {
            this.task = task;

            SubItems.Add(status_item = new ListViewSubItem(this, task.Status.ToString()));
            SubItems.Add(priority_item = new ListViewSubItem(this, task.Priority.ToString()));
            SubItems.Add(time_item = new ListViewSubItem(this, task.Time.ToShortTimeString()));
            SubItems.Add(name_item = new ListViewSubItem(this, task.Name));
            SubItems.Add(description_item = new ListViewSubItem(this, task.Description));

            UpdateAppearance();
        }

        private ITask task;
        private ListViewSubItem status_item, priority_item, time_item, name_item, description_item;
        
        public ITask Task
        {
            get { return task; }
        }

        public void UpdateAppearance()
        {
            this.Text = task.ID;
            TaskStatus status = task.Status;
            status_item.Text = status.ToString();
            switch (status)
            {
                case TaskStatus.Disabled:
                    BackColor = Color.White;
                    ForeColor = Color.Gray;
                    break;

                case TaskStatus.Pending:
                    BackColor = Color.White;
                    ForeColor = Color.Black;
                    break;

                case TaskStatus.Running:
                    BackColor = Color.Yellow;
                    ForeColor = Color.Black;
                    break;

                case TaskStatus.Completed:
                    BackColor = Color.White;
                    ForeColor = Color.Green;
                    break;

                case TaskStatus.Failed:
                case TaskStatus.Aborted:
                    BackColor = Color.White;
                    ForeColor = Color.Red;
                    break;
            }
            priority_item.Text = task.Priority.ToString();
            time_item.Text = task.Time.ToShortTimeString();
            name_item.Text = task.Name;
            description_item.Text = task.Description;
        }
    }
}
