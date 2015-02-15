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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Arnie, 
        "Arnie", 
        "Arnie terminates other objects on the machines on which it runs with extreme prejudice.")]    
    public partial class Arnie : Form, QS.Fx.Object.Classes.IWindow
    {
        public Arnie(QS.Fx.Object.IContext _context,
            [QS.Fx.Reflection.Parameter("quiet mode", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _quiet
            )
        {
            InitializeComponent();

            StringBuilder sb = new StringBuilder();
            foreach (System.Diagnostics.Process _p in System.Diagnostics.Process.GetProcesses())
            {
                if (!_p.Id.Equals(System.Diagnostics.Process.GetCurrentProcess().Id) && _p.ProcessName.ToLower().Contains("liveobjects"))
                {
                    sb.AppendLine(_p.Id.ToString().PadLeft(8) + "\t\"" + _p.ProcessName + "\"");
                    _p.Kill();
                }
            }

            this.Text = "Arnie";
            this.richTextBox1.Text = "Arnie terminated:\n\n" + sb.ToString();

            if (_quiet)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        
        }
    }
}
