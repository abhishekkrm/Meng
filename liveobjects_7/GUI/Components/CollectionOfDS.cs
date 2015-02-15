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
using System.Runtime.InteropServices;
namespace QS.GUI.Components
{
    public partial class CollectionOfDS : UserControl
    {
        public CollectionOfDS()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.Open))
                {
                    byte[] countBytes = new byte[Marshal.SizeOf(typeof(int))];
                    stream.Read(countBytes, 0, countBytes.Length);
                    byte[] nameBytes = new byte[BitConverter.ToInt32(countBytes, 0)];
                    stream.Read(nameBytes, 0, nameBytes.Length);
                    string name = Encoding.Unicode.GetString(nameBytes);
                    object obj = ((QS._core_c_.Serialization.ISerializer) QS._core_c_.Serialization.Serializer1.Serializer).Deserialize(stream);                    
                    listView1.Items.Add(new ItemDS(openFileDialog1.FileName, name, obj));
                    loadedObjects.Add(name, obj);
                }
            }
        }

        public void Add(string name, object obj)
        {
            listView1.Items.Add(new ItemDS("", name, obj));
            loadedObjects.Add(name, obj);
        }

        public IEnumerable<KeyValuePair<string, object>> LoadedObjects
        {
            get { return loadedObjects; }
        }

        public event EventHandler SelectionChanged;

        public IEnumerable<KeyValuePair<string, object>> Selected
        {
            get 
            { 
                foreach (ItemDS item in listView1.SelectedItems)
                    yield return new KeyValuePair<string,object>(item.name, item.obj);
            }                
        }

        private IDictionary<string, object> loadedObjects = new Dictionary<string, object>();

        private class ItemDS : ListViewItem
        {
            public ItemDS(string path, string name, object obj) : base(new string[] { name, obj.GetType().Name, path })
            {
                this.path = path;
                this.name = name;
                this.obj = obj;
            }

            public string name, path;
            public object obj;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectionChanged != null)
                SelectionChanged(sender, e);
        }
    }
}
