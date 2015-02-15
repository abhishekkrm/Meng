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
using System.Diagnostics;

namespace QS.GUI.Components
{
    public partial class ThreadTree : UserControl
    {
        public ThreadTree()
        {
            InitializeComponent();
        }

        private Process process = Process.GetCurrentProcess();
        private IDictionary<int, ThreadItem> threadItems = new Dictionary<int, ThreadItem>();

        #region Update

        public void RescanThreads()
        {
            lock (this)
            {
                listView1.BeginUpdate();

                System.Collections.ObjectModel.Collection<int> existingIDs = new System.Collections.ObjectModel.Collection<int>();
                foreach (int id in threadItems.Keys)
                    existingIDs.Add(id);

                foreach (ProcessThread thread in process.Threads)
                {
                    ThreadItem node;
                    if (existingIDs.Contains(thread.Id))
                    {
                        node = threadItems[thread.Id];
                        existingIDs.Remove(thread.Id);
                    }
                    else
                    {
                        node = new ThreadItem(thread);
                        threadItems.Add(thread.Id, node);
                        listView1.Items.Add(node);
                    }

                    node.Update();
                }

                foreach (int id in existingIDs)
                {
                    threadItems[id].Remove();
                    threadItems.Remove(id);
                }

                listView1.EndUpdate();
            }
        }

        #endregion

        #region Class ThreadItem

        private class ThreadItem : ListViewItem
        {
            public ThreadItem(ProcessThread thread) : base(thread.Id.ToString())
            {
                this.thread = thread;
                Update();
                SubItems.Add(stateSubItem);
                SubItems.Add(totalProcessorTimeSubItem);
                SubItems.Add(userProcessorTimeSubItem);
                SubItems.Add(priviledgedProcessorTimeSubItem);
                SubItems.Add(waitReasonSubItem);
                SubItems.Add(nameSubItem);
            }

            private ProcessThread thread;
            private ListViewSubItem stateSubItem = new ListViewSubItem();
            private ListViewSubItem totalProcessorTimeSubItem = new ListViewSubItem();
            private ListViewSubItem userProcessorTimeSubItem = new ListViewSubItem();
            private ListViewSubItem priviledgedProcessorTimeSubItem = new ListViewSubItem();
            private ListViewSubItem waitReasonSubItem = new ListViewSubItem();
            private ListViewSubItem nameSubItem = new ListViewSubItem();

            public void Update()
            {
                stateSubItem.Text = thread.ThreadState.ToString();
                totalProcessorTimeSubItem.Text = thread.TotalProcessorTime.ToString();
                userProcessorTimeSubItem.Text = thread.UserProcessorTime.ToString();
                priviledgedProcessorTimeSubItem.Text = thread.PrivilegedProcessorTime.ToString();
                waitReasonSubItem.Text = thread.WaitReason.ToString();
                nameSubItem.Text = "";
            }
        }

        #endregion
    }
}
