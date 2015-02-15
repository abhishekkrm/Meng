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

#define DEBUG_NoIronPython

using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QS.GUI.Components
{
    public partial class PythonConsole : Form
    {
        public PythonConsole(QS.GUI.Components.IObjectSelector objectSelector)
        {
            InitializeComponent();

            this.objectSelector = objectSelector;
            objectSelector.SelectionChanged += new EventHandler(objectSelector_SelectionChanged);
        }

        void objectSelector_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                this.Me = objectSelector.SelectedObject;
            }
            catch (Exception exc)
            {
                richTextBox2.Text = exc.ToString();
            }
        }

        private QS.GUI.Components.IObjectSelector objectSelector;
#if !DEBUG_NoIronPython
        private IronPython.Hosting.PythonEngine pythonEngine = null;
#endif
        private object me = null;

#if !DEBUG_NoIronPython
        private void CheckEngine()
        {
            if (pythonEngine == null)
            {
                pythonEngine = new IronPython.Hosting.PythonEngine();
                pythonEngine.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(QS.ClassID)));
                pythonEngine.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(QS.CMS.Core.Core)));
                pythonEngine.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(QS.TMS.Runtime.RunApp)));
            }
        }
#endif

        public object Me
        {
            get { return me; }
            set
            {
#if !DEBUG_NoIronPython
                CheckEngine();
                pythonEngine.SetVariable("me", me = value); 
#endif
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
#if !DEBUG_NoIronPython
                CheckEngine();
                object obj = pythonEngine.Evaluate(richTextBox1.Text);
                richTextBox2.Text = obj.ToString();
#endif
            }
            catch (Exception exc)
            {
                richTextBox2.Text = exc.ToString();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
#if !DEBUG_NoIronPython
                CheckEngine();
                pythonEngine.Execute(richTextBox1.Text);
                richTextBox2.Clear();
#endif
            }
            catch (Exception exc)
            {
                richTextBox2.Text = exc.ToString();
            }
        }
    }
}
