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
    public partial class CollectionOfDSWithView : UserControl
    {
        public CollectionOfDSWithView()
        {
            InitializeComponent();
            collectionOfDS1.SelectionChanged += new EventHandler(collectionOfDS1_SelectionChanged);
        }

        public IEnumerable<KeyValuePair<string, object>> Selected
        {
            get { return collectionOfDS1.Selected; }
        }

        public void Add(string name, object obj)
        {
            collectionOfDS1.Add(name, obj);
        }

        void collectionOfDS1_SelectionChanged(object sender, EventArgs e)
        {
            List<KeyValuePair<string, object>> selected = new List<KeyValuePair<string,object>>(collectionOfDS1.Selected);
            ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = 
                (selected.Count == 1) ? ((QS._core_e_.Data.IDataSet) selected[0].Value) : null;
        }
    }
}
