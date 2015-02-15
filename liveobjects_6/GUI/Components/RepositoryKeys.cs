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
    public partial class RepositoryKeys : UserControl
    {
        private static string DefaultPath = "C:\\.QuickSilver\\.Repository";

        public RepositoryKeys()
        {
            InitializeComponent();

            try
            {
                textBox1.Text = DefaultPath;
                folderBrowserDialog1.SelectedPath = DefaultPath;

                TryPath(DefaultPath);
            }
            catch (Exception)
            {
            }
        }

        private QS._core_e_.Repository.IRepository repository;
        private KeyValuePair<string, QS._core_e_.Repository.IAttribute> selectedKey;

        public QS._core_e_.Repository.IRepository Repository
        {
            get { return repository; }
        }

        public KeyValuePair<string, QS._core_e_.Repository.IAttribute> SelectedKey
        {
            get { return selectedKey; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (textBox1.Text != "")
                {
                    try
                    {
                        folderBrowserDialog1.SelectedPath = textBox1.Text;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    TryPath(folderBrowserDialog1.SelectedPath);
                }
            }
        }

        private void TryPath(string path)
        {
            try
            {
                QS._core_e_.Repository.IRepository repository = new QS._qss_e_.Repository_.Repository(path);
                if (repository != null)
                {
                    this.repository = repository;
                    textBox1.Text = path;
                    DefaultPath = path;

                    listView1.BeginUpdate();
                    listView1.Items.Clear();
                    foreach (string key in repository.AttributeNames)
                        listView1.Items.Add(key);
                    listView1.EndUpdate();
                }
            }
            catch (Exception)
            {
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                if (repository != null && listView1.SelectedItems != null && listView1.SelectedItems.Count == 1)
                {
                    try
                    {
                        string name = (listView1.SelectedItems[0]).Text;
                        selectedKey =
                            new KeyValuePair<string, QS._core_e_.Repository.IAttribute>(
                                name, ((QS._core_e_.Repository.IAttribute) (repository[name])));
                    }
                    catch (Exception)
                    {
                        selectedKey = new KeyValuePair<string, QS._core_e_.Repository.IAttribute>(null, null);
                    }
                }
                else
                    selectedKey = new KeyValuePair<string, QS._core_e_.Repository.IAttribute>(null, null);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // ......................................................
        }
    }
}
